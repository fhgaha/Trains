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
		private RailPath blueprint;
		private Camera camera;
		private Spatial objectHolder;   //Rails
		private State state = State.None;

		private List<RailPath> pathList = new List<RailPath>();
		private bool firstSegmentIsPlaced => pathList.Count > 0;
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
				ResetBlueprint();
				return;
			}

			//"Build Rail" button is clicked
			if (Global.MainButtonMode is MainButtonType.BuildRail)
			{
				Global.MainButtonMode = null;
				ResetBlueprint();
				return;
			}

			Global.MainButtonMode = MainButtonType.BuildRail;
			state = State.SelectStart;
			//init blueprint
			blueprint = scene.Instance<RailPath>();
			AddChild(blueprint);
			blueprint.Name = "blueprint";
		}

		private void ResetBlueprint()
		{
			state = State.None;
			blueprint?.QueueFree();
			blueprint = null;
			prevDir = Vector3.Zero;
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton evMouseButton && evMouseButton.IsActionPressed("lmb"))
			{
				switch (state)
				{
					case State.None: return;
					case State.SelectStart:
						SelectStart();
						break;
					case State.SelectEnd:
						PlaceObject();
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

		private void SelectStart()
		{
			var mousePos = this.GetIntersection(camera, rayLength);
			blueprint.Start.Position = mousePos;
			TrySnap(mousePos);  //path should begin from snapped point with no offset
			state = State.SelectEnd;
		}

		private void TrySnap(Vector3 mousePos)
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
					blueprint.Start.Position = point;
					prevDir = isStart ? path.GetDirFromStart() : path.GetDirFromEnd();

					if (isStart)
						blueprint.Start = new CurvePoint(point, path.GetDirFromStart());
					else
						blueprint.End = new CurvePoint(point, path.GetDirFromStart());
				}
			}
		}

		private void UpdateBlueprint()
		{
			var mousePos = this.GetIntersection(camera, rayLength);
			blueprint.Translation = mousePos;
			TrySnap(mousePos);

			//set base color
			var area = blueprint.GetNode<Area>("CSGPolygon/Area");
			var bodies = area.GetOverlappingBodies().Cast<Node>().Where(b => b.IsInGroup("Obstacles"));
			var canBuild = bodies.Count() <= 0;
			var csgMaterial = (SpatialMaterial)blueprint.GetNode<CSGPolygon>("CSGPolygon").Material;
			csgMaterial.AlbedoColor = canBuild ? yellow : red;
		}

		private void DrawTrajectory()
		{
			var mousePos = this.GetIntersection(camera, rayLength);
			blueprint.Translation = blueprint.Start.Position;

			var points = calculator.CalculateCurvePoints(
				blueprint.Translation.ToVec2(), mousePos.ToVec2(), 1f, GetRotationDeg(), firstSegmentIsPlaced);

			var curve = new RailCurve();
			//convert points to segments and add segments to curve
			// var pointsAsVec3 = points.Select(p => p.ToVec3()).ToList();
			// if (points.Count() > 0)
			// {
			// 	var segments = CurveSegment.ConvertToSegments(pointsAsVec3);
			// 	//segments.ForEach(s => curve.AppendSegment());
			// }


			if (points.Count() > 0)
				points.ForEach(p => curve.AddPoint(p.ToVec3() - blueprint.Translation));
			else
			{
				//add two points to prevent error "The faces count are 0, the mesh shape cannot be created"
				curve.AddPoint(Vector3.Zero);
				curve.AddPoint(prevDir == Vector3.Zero ? Vector3.Forward : prevDir);
			}
			blueprint.Curve = curve;
		}

		private float GetRotationDeg()
		{
			var rotationDeg = 0f;
			if (prevDir != Vector3.Zero)
			{
				rotationDeg = Vector2.Up.AngleTo(prevDir.ToVec2()) * 180 / Pi;
				if (rotationDeg < 0) rotationDeg += 360;
			}
			return rotationDeg;
		}

		protected void PlaceObject()
		{
			RailPath path = pathList.LastOrDefault();

			//place first segment
			if (path is null)
			{
				//init path
				path = scene.Instance<RailPath>();
				AddChild(path);
				pathList.Add(path);
				path.Init(blueprint);

				//save 
				blueprint.Start.Position += path.Curve.Last();
				prevDir = path.GetDirFromEnd();
				path.Start = blueprint.Start;
				//path.PrevDir = prevDir;
				return;
			}

			//copy blueprint
			var last = blueprint.Curve.Last();
			var pathOriginToBpOrigin = blueprint.Translation - path.Translation;
			var segment = new CurveSegment(blueprint.Curve.GetBakedPoints());
			((RailCurve)path.Curve).AppendSegment(pathOriginToBpOrigin, segment);

			//save 
			blueprint.Start.Position = blueprint.Translation + last;
			prevDir = path.GetDirFromEnd();
			path.Start.Position = blueprint.Translation + last;
			//path.PrevDir = path.GetDirFromEnd();

			path.Start = new CurvePoint { Position = path.Translation + path.Curve.First(), Direction = path.GetDirFromStart() };
			path.End = new CurvePoint { Position = path.Translation + path.Curve.Last(), Direction = path.GetDirFromEnd() };


			DrawTrajectory();   //this is called so that there is no overlap of blueprint and path
		}
	}
}
