//https://www.youtube.com/watch?v=F47dmKpAIW0&t=774s
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using Trains.Scripts.DragDrop;
using static Godot.Mathf;

namespace Trains
{
	public class Trajectory3d : Spatial
	{
		[Export(PropertyHint.Range, "0, 359,")]
		public float _Rotation { get; set; } = 0;
		private int numPoints = 50;
		private float gravity = -9.8f;
		private Spatial startNode;
		private Spatial endNode;
		private Path path;
		private PackedScene scene = GD.Load<PackedScene>("res://Scenes/Heplers/DraggableObj_.tscn");
		//private PackedScene scene = GD.Load<PackedScene>("res://Scenes/Rails/RailCSG.tscn");

		public override void _Ready()
		{
			startNode = GetNode<Spatial>("start");
			endNode = GetNode<Spatial>("end");
			path = GetNode<Path>("RailPath");
			path.Translation = startNode.GlobalTransform.origin;
		}

		public override void _PhysicsProcess(float delta)
		{
			Vector3 start = startNode.Translation;
			Vector3 end = endNode.Translation;
			path.Translation = start;

			var points = CalculateCircledPath(start.ToVec2(), end.ToVec2(), 3f, 50, Pi/180 * _Rotation);
			//var points = CalculateTrajectory(start.ToVec2(), end.ToVec2(), 50);
			//var points = CalculateLine(start.ToVec2(), end.ToVec2(), 2);

			var curve = new Curve3D();
			if (points.Count() > 0)
				points.ToList().ForEach(p => curve.AddPoint(p.ToVec3() - start));
			else
			{
				//add two points to prevent error "The faces count are 0, the mesh shape cannot be created"
				curve.AddPoint(Vector3.Zero);
				curve.AddPoint(Vector3.Forward);
			}
			path.Curve = curve;

			// GD.Print("start: " + start);
			// GD.Print("end: " + end);
			// GD.Print("points:");
			// points.ToList().ForEach(p => GD.Print(start + p.ToVec3()));
			// GD.Print();
		}

		private IEnumerable<Vector2> CalculateCircledPath(
			Vector2 start, Vector2 end, float radius, int numPoints, float rotation)
		{
			var prevDir = new Vector2(0, -1).Rotated(rotation);
			var startEndDir = (end - start).Normalized();
			var leftRight = prevDir.Rotated(Pi / 2).Dot(startEndDir);   //-1, 0 or 1

			var d = rotation >= Pi/2 && rotation < 3*Pi/2 ? -1 : 1;
			var prevDirPerp = new Vector2(d, -d * prevDir.x / prevDir.y).Normalized();
			var radVec = radius * (leftRight >= 0 ? prevDirPerp : prevDirPerp.Rotated(Pi));
			var center = start + radVec;

			var points = new List<Vector2>();
			var tangent = Vector2.Zero;
			var accuracy = 0.1f;

			//go along circle
			var startAngle = (leftRight >= 0 ? Pi : 0) + rotation;
			var endAngle = (leftRight >= 0 ? 2 * Pi + Pi / 2 : -Pi - Pi / 2) + rotation;
			var dAngle = leftRight >= 0 ? 0.1f : -0.1f;
			Func<float, bool> condition = i => leftRight >= 0 ? i < endAngle : i > endAngle;

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
				var dot = leftRight >= 0 ? dirPointToCenter.Dot(dirPointToEnd) : dirPointToCenter.Dot(-dirPointToEnd);
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

			//draw circle points
			// foreach (var p in points)
			// {
			// 	var dupl = (MeshInstance)GetNode<MeshInstance>("center").Duplicate();
			// 	AddChild(dupl);
			// 	dupl.Translation = p.ToVec3();
			// }

			GetNode<MeshInstance>("dir").Translation = prevDir.ToVec3();
			GetNode<MeshInstance>("center").Translation = center.ToVec3();
			GetNode<MeshInstance>("tangent").Translation = tangent.ToVec3();

			return points;
		}

		private List<Vector2> CalculateLine(Vector2 start, Vector2 end, int numPoints)
		{
			var points = new List<Vector2>();
			var startEnd = end - start;

			for (int i = 0; i < numPoints + 1; i++)
			{
				var point = new Vector2(i * startEnd.x / numPoints, i * startEnd.y / numPoints);
				points.Add(point);
			}

			return points;
		}

		private IEnumerable<Vector2> CalculateTrajectory(Vector2 start, Vector2 end, int numPoints)
		{
			float gravity = -15f;
			var points = new List<Vector2>();
			var startEndDir = (end - start).Normalized();
			//this should be changed to previous segment direction?
			var DOT = Vector2.Right.Dot(startEndDir);   //-1, 0 or 1
			var angle = 90 - 45 * DOT;

			var xDist = end.x - start.x;
			var yDist = -1f * (end.y - start.y);  //flip y to do calculations in eucludus geometry. y flips back later.

			var speed = Sqrt(
				((0.5f * gravity * xDist * xDist) / Pow(Cos(Pi / 180 * angle), 2f))
				/ (yDist - (Tan(Pi / 180 * angle) * xDist)));
			var xComponent = Cos(Pi / 180 * angle) * speed;
			var yComponent = Sin(Pi / 180 * angle) * speed;

			var totalTime = xDist / xComponent;

			for (int i = 0; i < numPoints + 1; i++)
			{
				var time = totalTime * i / numPoints;
				var dx = time * xComponent;
				var dy = -1f * (time * yComponent + 0.5f * gravity * time * time);
				//points.Add(new Vector2(dx, dy));
				yield return new Vector2(dx, dy);
			}
		}
	}
}
