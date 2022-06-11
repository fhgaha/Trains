using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using static Godot.Mathf;

namespace Trains
{
	public class Trajectory3d : Spatial
	{
		private int numPoints = 50;
		private float gravity = -9.8f;
		private Spatial start;
		private Spatial end;
		private Path path;
		//private PackedScene scene = GD.Load<PackedScene>("res://Scenes/Rails/RailCSG.tscn");

		public override void _Ready()
		{
			start = GetNode<Spatial>("start");
			end = GetNode<Spatial>("end");
			path = GetNode<Path>("RailCSG/Path");
			path.Translation = start.GlobalTransform.origin;
		}

		public override void _PhysicsProcess(float delta)
		{
			Vector3 startPos = start.Translation;
			Vector3 endPos = end.Translation;
			
			var points = CalculateTrajectory(startPos.ToVec2(), endPos.ToVec2(), 20);
			var curve = new Curve3D();
			points.ForEach(p => curve.AddPoint(p.ToVec3() - startPos));
			path.Curve = curve;


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