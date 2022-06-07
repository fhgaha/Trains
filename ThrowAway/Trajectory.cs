//https://www.youtube.com/watch?v=F47dmKpAIW0

using Godot;
using System;
using System.Collections.Generic;
using static Godot.Mathf;

namespace Trains
{
	public class Trajectory : Node2D
	{
		private int numPoints = 50;
		private float gravity = -9.8f;

		public override void _Ready()
		{
			CalculateTrajectory();
		}

		public override void _Process(float delta)
		{
			GetNode<Sprite>("end").GlobalPosition = GetGlobalMousePosition();
			CalculateTrajectory();
		}

		private void CalculateTrajectory()
		{
			var points = new List<Vector2>();
			var startPos = GetNode<Sprite>("start").GlobalPosition;
			var endPos = GetNode<Sprite>("end").GlobalPosition;
			var startEndDir = (endPos - startPos).Normalized();
			var DOT = Vector2.Right.Dot(startEndDir);   //-1, 0 or 1
			var angle = 90 - 45 * DOT;

			var xDist = endPos.x - startPos.x;
			var yDist = -1f * (endPos.y - startPos.y);

			var speed = Sqrt(
				((0.5f * gravity * xDist * xDist) / Pow(Cos(Pi / 180 * angle), 2f))
				/ (yDist - (Tan(Pi / 180 * angle) * xDist)));   //make sure
			var xComp = Cos(Pi / 180 * angle) * speed;
			var yComp = Sin(Pi / 180 * angle) * speed;

			var totalTime = xDist / xComp;

			for (int i = 0; i < numPoints; i++)
			{
				var time = totalTime * i / numPoints;
				var dx = time * xComp;
				var dy = -1f * (time * yComp + 0.5f * gravity * time * time);
				points.Add(startPos + new Vector2(dx, dy));
			}

			GetNode<Line2D>("Line2D").Points = points.ToArray();
		}
	}
}