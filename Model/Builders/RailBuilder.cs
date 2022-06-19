using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;
using static Godot.Mathf;

namespace Trains.Model.Builders
{
	enum State { None, SelectStart, SelectEnd }
	public class RailBuilder : Spatial
	{
		private Color yellow = new Color("86e3db6b");
		private Color red = new Color("86e36b6b");
		private const float rayLength = 1000f;
		private List<Cell> cells;
		private Events events;
		private CurveCalculator calculator;
		private PackedScene scene;
		private Path blueprint;
		private bool canBuild = false;
		private Camera camera;
		private Spatial objectHolder;   //Rails
		private State state = State.None;

		private bool firstSegmentIsPlaced => pathList.Count > 0;
		private Vector3 start = Vector3.Zero;
		private Vector3 prevDir = Vector3.Zero;
		private List<Path> pathList = new List<Path>();
		private const float snapDistance = 1f;

		//!in editor for CSGPolygon property Path Local should be "On" to place polygon where the cursor is with no offset

		//build order:
		//1. press BS button, simple straight road will show up following cursor.
		//2. using mouse select a place to build first segment. it cannot be built on obstacle.
		//3. press lmb to place first segment. path will show up from first segment to mouse pos, showing possible path
		//	 in blueprint mode.
		//4. press lmb again to place blueprint road.
		public void Init(List<Cell> cells, Camera camera, Spatial objectHolder, PackedScene scene)
		{
			this.cells = cells;
			this.objectHolder = objectHolder;
			this.camera = camera;
			this.scene = scene;
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
			calculator = GetNode<CurveCalculator>("Calculator");
		}

		public override void _PhysicsProcess(float delta)
		{
			//if (!(Global.MainButtonMode is MainButtonType.BuildRail)) return;
			if (state == State.None) return;
			if (start == Vector3.Zero) state = State.SelectStart;
			else state = State.SelectEnd;
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			//other button is clicked
			if (buttonType != MainButtonType.BuildRail)
			{
				Reset();
				return;
			}

			//"Build Rail" button is clicked
			if (Global.MainButtonMode is MainButtonType.BuildRail)
			{
				Global.MainButtonMode = null;
				Reset();
				return;
			}

			//init blueprint
			Global.MainButtonMode = MainButtonType.BuildRail;
			state = State.SelectStart;
			blueprint = scene.Instance<Path>();
			AddChild(blueprint);
			blueprint.Name = "blueprint";
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton evMouseButton && evMouseButton.IsActionPressed("lmb"))
			{
				switch (state)
				{
					case State.None: return;
					case State.SelectStart:
						start = this.GetIntersection(camera, rayLength);
						break;
					case State.SelectEnd:
						PlaceObject(this.GetIntersection(camera, rayLength));
						break;
				}
			}

			if (@event is InputEventMouseMotion evMouseMotion)
			{
				switch (state)
				{
					case State.None: return;
					case State.SelectStart:
						UpdateBlueprint();
						break;
					case State.SelectEnd:
						DrawTrajectory();
						break;
				}
			}
		}



		private void DrawTrajectory()
		{
			Vector3 end = this.GetIntersection(camera, rayLength);
			blueprint.Translation = start;

			var rotationDeg = 0f;
			if (prevDir != Vector3.Zero)
			{
				rotationDeg = Vector2.Up.AngleTo(prevDir.ToVec2()) * 180 / Pi;
				if (rotationDeg < 0) rotationDeg += 360;
			}
			var points = calculator.CalculateCurvePoints(start.ToVec2(), end.ToVec2(), 1f, rotationDeg, firstSegmentIsPlaced);

			var curve = new Curve3D();
			if (points.Count() > 0)
				points.ForEach(p => curve.AddPoint(p.ToVec3() - start));
			else
			{
				//add two points to prevent error "The faces count are 0, the mesh shape cannot be created"
				curve.AddPoint(Vector3.Zero);
				curve.AddPoint(prevDir == Vector3.Zero ? Vector3.Forward : prevDir);
			}
			blueprint.Curve = curve;
		}

		///use this method to place multiple paths instead of continuing a single path
		protected void PlaceObjectMultiplePathsCase(Vector3 position)
		{
			//copy blueprint
			var path = scene.Instance<Path>();
			AddChild(path);
			path.Transform = blueprint.Transform;
			path.Curve = blueprint.Curve;
			path.GetNode<CSGPolygon>("CSGPolygon").Polygon = path.GetNode<CSGPolygon>("CSGPolygon").Polygon;

			//save 
			start += path.Curve.Last();
			var points = path.Curve.TakeLast(2);
			prevDir = (points[1] - points[0]).Normalized();
		}

		protected void PlaceObject(Vector3 position)
		{
			Path path = pathList.LastOrDefault();

			//place first segment
			if (pathList.Count == 0)
			{
				//init path
				path = scene.Instance<Path>();
				AddChild(path);
				pathList.Add(path);
				path.Transform = blueprint.Transform;
				path.Curve = blueprint.Curve;
				//path.GetNode<CSGPolygon>("CSGPolygon").Polygon = path.GetNode<CSGPolygon>("CSGPolygon").Polygon;
				path.GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;

				//save 
				start += path.Curve.Last();
				var _points = path.Curve.TakeLast(2);
				prevDir = (_points[1] - _points[0]).Normalized();
				return;
			}

			//copy blueprint
			var last = blueprint.Curve.Last();
			var pathOriginToBpOrigin = blueprint.Translation - path.Translation;
			foreach (var p in blueprint.Curve.GetBakedPoints())
				path.Curve.AddPoint(pathOriginToBpOrigin + p);

			//save 
			start = blueprint.Translation + last;
			var points = path.Curve.TakeLast(2);
			prevDir = (points[1] - points[0]).Normalized();

			DrawTrajectory();   //this is called so that there is no overlap of blueprint and path
		}

		private void UpdateBlueprint()
		{
			var mousePos = this.GetIntersection(camera, rayLength);
			blueprint.Translation = mousePos;
			Snap(mousePos);

			//set base color
			var area = blueprint.GetNode<Area>("CSGPolygon/Area");
			var bodies = area.GetOverlappingBodies().Cast<Node>().Where(b => b.IsInGroup("Obstacles"));
			canBuild = bodies.Count() <= 0;
			var csgMaterial = (SpatialMaterial)blueprint.GetNode<CSGPolygon>("CSGPolygon").Material;
			csgMaterial.AlbedoColor = canBuild ? yellow : red;
		}

		private void Reset()
		{
			blueprint?.QueueFree();
			blueprint = null;
			state = State.None;
			start = Vector3.Zero;
		}

		private void Snap(Vector3 mousePos)
		{
			var firstPoints = pathList.Select(path => path.Translation + path.Curve.First());
			var lastPoints = pathList.Select(path => path.Translation + path.Curve.Last());
			var points = firstPoints.Union(lastPoints);

			foreach (var p in points)
			{
				if (p.DistanceTo(mousePos) < snapDistance)
				{
					blueprint.Translation = p;
					return;
				}
			}
		}
	}
}
