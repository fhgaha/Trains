//https://www.youtube.com/watch?v=F47dmKpAIW0&t=774s
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using static Godot.Mathf;

namespace Trains
{
	public class Trajectory3d : Spatial
	{
		private int numPoints = 50;
		private float gravity = -9.8f;
		private Spatial startNode;
		private Spatial endNode;
		private Path path;
		//private PackedScene scene = GD.Load<PackedScene>("res://Scenes/Rails/RailCSG.tscn");

		public override void _Ready()
		{
			startNode = GetNode<Spatial>("start");
			endNode = GetNode<Spatial>("end");
			path = GetNode<Path>("RailCSG/Path");
			path.Translation = startNode.GlobalTransform.origin;
		}

		public override void _PhysicsProcess(float delta)
		{
			Vector3 start = startNode.Translation;
			Vector3 end = endNode.Translation;
			path.Translation = start;

			var points = CalculateTrajectory(start.ToVec2(), end.ToVec2(), 50);
			//var points = CalculateLine(start.ToVec2(), end.ToVec2(), 2);
			var curve = new Curve3D();
			points.ToList().ForEach(p => curve.AddPoint(p.ToVec3()));
			path.Curve = curve;

			// GD.Print("start: " + start);
			// GD.Print("end: " + end);
			// GD.Print("points:");
			// points.ToList().ForEach(p => GD.Print(start + p.ToVec3()));
			// GD.Print();
		}

		private List<Vector2> CalculateLine(Vector2 start, Vector2 end, int numPoints)
		{
			var points = new List<Vector2>();
			var startEnd = end - start;

			for (int i = 0; i < numPoints + 1; i++)
			{
				var point = new Vector2(i * startEnd.x/numPoints, i * startEnd.y/numPoints);
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

			//return points;
		}
	}
}