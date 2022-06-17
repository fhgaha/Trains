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
		private List<Cell> cells;
		private Events events;
		private PackedScene scene;
		private Path blueprint;
		private bool canBuild = false;
		private const float rayLength = 1000f;
		private Camera camera;
		private Spatial objectHolder;   //Rails
		private State state = State.None;

		private bool firstSegmentIsPlaced = false;
		private Vector3 start = Vector3.Zero;
		private Vector3 prevDir = Vector3.Zero;


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
		}

		public override void _PhysicsProcess(float delta)
		{
			if (!(Global.MainButtonMode is MainButtonType.BuildRail)) return;
			if (state == State.None) return;
			if (start == Vector3.Zero) state = State.SelectStart;
			else state = State.SelectEnd;

			//GD.Print(state);
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

			var _rotationDeg = 0f;
			if (prevDir != Vector3.Zero)
			{
				_rotationDeg = Vector2.Up.AngleTo(prevDir.ToVec2()) * 180/Pi;
				GD.Print("_rotationDeg: " + _rotationDeg);
			}
			//GD.Print("_rotationRad: " + 180/Pi*_rotationRad);

			var points = CalculateCircledPath(start.ToVec2(), end.ToVec2(), 1f, 50, _rotationDeg);
			//GD.Print("start: " + start);
			var curve = new Curve3D();
			if (points.Count() > 0)
				points.ToList().ForEach(p => curve.AddPoint(p.ToVec3() - start));
			else
			{
				//add two points to prevent error "The faces count are 0, the mesh shape cannot be created"
				curve.AddPoint(Vector3.Zero);
				curve.AddPoint(Vector3.Forward);
			}
			blueprint.Curve = curve;
		}

		protected void PlaceObject(Vector3 position)
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

			// GD.Print(points[0]);
			// GD.Print(points[1]);
			// GD.Print(prevDir);
			// GD.Print();

			if (!firstSegmentIsPlaced) firstSegmentIsPlaced = true;
		}

		private void UpdateBlueprint()
		{
			blueprint.Translation = this.GetIntersection(camera, rayLength);

			//set base color
			var area = blueprint.GetNode<Area>("CSGPolygon/Area");
			var bodies = area.GetOverlappingBodies().Cast<Node>().Where(b => b.IsInGroup("Obstacles"));
			canBuild = bodies.Count() <= 0;
			var csgMaterial = (SpatialMaterial)blueprint.GetNode<CSGPolygon>("CSGPolygon").Material;
			csgMaterial.AlbedoColor = canBuild ? yellow : red;
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			//check buttonType
			if (buttonType != MainButtonType.BuildRail)
			{
				blueprint?.QueueFree();
				return;
			}

			if (Global.MainButtonMode is MainButtonType.BuildRail) Global.MainButtonMode = null;
			else Global.MainButtonMode = MainButtonType.BuildRail;

			if (!(Global.MainButtonMode is MainButtonType.BuildRail))
			{
				blueprint?.QueueFree();
				return;
			}

			//check state
			if (state != State.None) { state = State.None; return; }
			if (state == State.None) state = State.SelectStart;

			//set up blueprint
			blueprint = scene.Instance<Path>();
			AddChild(blueprint);
		}

		private IEnumerable<Vector2> CalculateCircledPath(
			Vector2 start, Vector2 end, float radius, int numPoints, float rotationDeg)
		{
			var prevDir = Vector2.Up.Rotated(Pi / 180 * rotationDeg);
			var startEndDir = (end - start).Normalized();
			var centerIsOnRight = prevDir.Rotated(Pi / 2).Dot(startEndDir) >= 0;   //-1, 0 or 1

			var d = Pi / 180 * rotationDeg >= Pi / 2 && Pi / 180 * rotationDeg < 3 * Pi / 2 ? -1 : 1;
			var prevDirPerp = new Vector2(d, -d * prevDir.x / prevDir.y).Normalized();
			var radVec = radius * (centerIsOnRight ? prevDirPerp : prevDirPerp.Rotated(Pi));
			var center = start + radVec;

			var points = new List<Vector2>();
			var tangent = Vector2.Zero;
			var accuracy = 0.1f;

			// if (firstSegmentIsPlaced)
			// {
			//go along circle
			var startAngle = (centerIsOnRight ? Pi : 0) + Pi / 180 * rotationDeg;
			startAngle = Mathf.Clamp(startAngle, -2 * Pi, 2 * Pi);
			var endAngle = (centerIsOnRight ? 2 * Pi + Pi / 2 : -Pi - Pi / 2) + Pi / 180 * rotationDeg;
			endAngle = Mathf.Clamp(endAngle, -2 * Pi, 2 * Pi);
			var dAngle = centerIsOnRight ? 0.1f : -0.1f;
			Func<float, bool> condition = i => centerIsOnRight ? i < endAngle : i > endAngle;

			for (float i = startAngle; condition(i); i += dAngle)
			{
				var x = radius * Cos(i);
				var y = radius * Sin(i);
				var point = new Vector2(x, y);
				points.Add(start + radVec + point);
			}

			//find tangent in circle points
			tangent = points.FirstOrDefault(p =>
			{
				var dirPointToCenter = (center - p).Normalized();
				var dirPointToEnd = (end - p).Normalized();
				//var dot = dirPointToCenter.Dot(dirPointToEnd);
				var dot = centerIsOnRight ? dirPointToCenter.Dot(dirPointToEnd) : dirPointToCenter.Dot(-dirPointToEnd);
				var requiredVal = 0;
				if (dot > requiredVal - accuracy && dot < requiredVal + accuracy) return true;
				return false;
			});

			//prevent drawing inside circle or from scene origin
			if (tangent == Vector2.Zero) return new List<Vector2>();

			//prevent drawing behind start
			var tangetXApproxEqualsStartX = Math.Abs(tangent.x - start.x) < 0.01f;
			var tangetYApproxEqualsStartY = Math.Abs(tangent.y - start.y) < 0.01f;
			var tangetApproxEqualsStart = tangetXApproxEqualsStartX && tangetYApproxEqualsStartY;

			if (tangetApproxEqualsStart && prevDir.Dot(startEndDir) < 0) return new List<Vector2>();
			points.RemoveAll(p => points.IndexOf(p) > points.IndexOf(tangent));

			//go straight
			var _dirPointToEnd = (end - tangent).Normalized();
			var _point = tangent;
			while (_point.DistanceSquaredTo(end) > accuracy)
			{
				_point += _dirPointToEnd * accuracy;
				points.Add(_point);
			}
			// }
			// else
			// {
			// 	var a_point = start;
			// 	while (a_point.DistanceSquaredTo(end) > accuracy)
			// 	{
			// 		a_point += startEndDir * accuracy;
			// 		points.Add(a_point);
			// 	}
			// }
			GetNode<MeshInstance>("dir").Translation = prevDir.ToVec3();
			GetNode<MeshInstance>("center").Translation = center.ToVec3();
			GetNode<MeshInstance>("tangent").Translation = tangent.ToVec3();

			return points;
		}

		private List<Vector2> CalculateTrajectory(Vector2 startPos, Vector2 endPos, int numPoints)
		{
			float gravity = -15f;
			var points = new List<Vector2>();
			var startEndDir = (endPos - startPos).Normalized();
			//this should be changed to previous segment direction?
			var DOT = Vector2.Right.Dot(startEndDir);   //-1, 0 or 1
			var angle = 90 - 45 * DOT;

			var xDist = endPos.x - startPos.x;
			var yDist = -1f * (endPos.y - startPos.y);  //flip y to do calculations in eucludus geometry. y flips back later.

			var speed = Sqrt(
				((0.5f * gravity * xDist * xDist) / Pow(Cos(Pi / 180 * angle), 2f))
				/ (yDist - (Tan(Pi / 180 * angle) * xDist)));
			var xComponent = Cos(Pi / 180 * angle) * speed;
			var yComponent = Sin(Pi / 180 * angle) * speed;

			var totalTime = xDist / xComponent;

			for (int i = 0; i < numPoints; i++)
			{
				var time = totalTime * i / numPoints;
				var dx = time * xComponent;
				var dy = -1f * (time * yComponent + 0.5f * gravity * time * time);
				points.Add(startPos + new Vector2(dx, dy));
			}

			return points;
		}
	}
}
