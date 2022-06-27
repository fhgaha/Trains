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
		private const float snapDistance = 1f;
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

		private List<RailPath> pathList = new List<RailPath>();
		private bool firstSegmentIsPlaced => pathList.Count > 0;
		private Vector3 start = Vector3.Zero;
		private Vector3 prevDir = Vector3.Zero;

		//!in editor for CSGPolygon property Path Local should be "On" to place polygon where the cursor is with no offset

		//build order:
		//1. press BS button, blueprint will show up as simple straight road with it's end following cursor. 
		//2. using mouse select a place to build first segment. it cannot be built on obstacle.
		//3. press lmb to place first segment. 
		//4. a new blueprint will show up with start in the end of previous segment with the end following mouse pos.
		//	 this time path will be curved.
		//5. press lmb again to place blueprint road.
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
			blueprint = scene.Instance<RailPath>();
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
						Snap(start);
						state = State.SelectEnd;
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

			var points = calculator.CalculateCurvePoints(blueprint.Translation.ToVec2(), end.ToVec2(), 1f, rotationDeg, firstSegmentIsPlaced);

			var curve = new RailCurve();
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

		protected void PlaceObject(Vector3 position)
		{
			RailPath path = pathList.LastOrDefault();

			//place first segment
			if (pathList.Count == 0)
			{
				path = scene.Instance<RailPath>();
				AddChild(path);
				pathList.Add(path);
				path.Init(blueprint);

				//init path
				// path = scene.Instance<Path>();
				// AddChild(path);
				// pathList.Add(path);
				// path.Transform = blueprint.Transform;
				// path.Curve = blueprint.Curve;
				// //path.GetNode<CSGPolygon>("CSGPolygon").Polygon = path.GetNode<CSGPolygon>("CSGPolygon").Polygon;
				// path.GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;

				// //save 
				start += path.Curve.Last();
				prevDir = GetDir(path.Curve.TakeLast(2));
				path.Start = start;
				path.PrevDir = prevDir;
				return;
			}

			//copy blueprint
			var last = blueprint.Curve.Last();
			var pathOriginToBpOrigin = blueprint.Translation - path.Translation;
			var segment = new CurveSegment(blueprint.Curve.GetBakedPoints());
			((RailCurve)path.Curve).AppendSegment(pathOriginToBpOrigin, segment);

			//save 
			start = blueprint.Translation + last;
			prevDir = GetDir(path.Curve.TakeLast(2));
			path.Start = blueprint.Translation + last;
			path.PrevDir = GetDir(path.Curve.TakeLast(2));

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
			prevDir = Vector3.Zero;
		}

		private void Snap(Vector3 mousePos)
		{
			foreach (var path in pathList)
			{
				var start = path.Translation + path.Curve.First();
				var end = path.Translation + path.Curve.Last();

				if (start.DistanceTo(mousePos) < snapDistance && start.DistanceTo(mousePos) < end.DistanceTo(mousePos))
				{
					MoveBpUpdateStartAndPrevDir(start, isStart: true);
					return;
				}

				if (end.DistanceTo(mousePos) < snapDistance && end.DistanceTo(mousePos) < start.DistanceTo(mousePos))
				{
					MoveBpUpdateStartAndPrevDir(end, isStart: false);
					return;
				}

				void MoveBpUpdateStartAndPrevDir(Vector3 point, bool isStart)
				{
					blueprint.Translation = point;
					this.start = point;
					prevDir = isStart ? GetDir(path.Curve.TakeFirst(2)) : GetDir(path.Curve.TakeLast(2));
				}
			}
		}



		private Vector3 GetDir(List<Vector3> points) => (points[1] - points[0]).Normalized();
	}
}
