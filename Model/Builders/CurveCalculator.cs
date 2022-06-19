using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using static Godot.Mathf;

namespace Trains.Model.Builders
{
	public class CurveCalculator : Spatial
	{
		private MeshInstance dir;
		private MeshInstance center;
		private MeshInstance tangent;

		public override void _Ready()
		{
			dir = GetNode<MeshInstance>("dir");
			center = GetNode<MeshInstance>("center");
			tangent = GetNode<MeshInstance>("tangent");
		}

		public List<Vector2> CalculateCurvePoints(Vector2 start, Vector2 end, float radius, float rotationDeg, bool firstSegmentIsPlaced)
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

			if (!firstSegmentIsPlaced)
			{
				GoStraight(start, end, points, accuracy);
				return points;
			}

			//go along circle
			var startAngle = (centerIsOnRight ? Pi : 0) + Pi / 180 * rotationDeg;
			var endAngle = (centerIsOnRight ? 2 * Pi + Pi / 2 : -Pi - Pi / 2) + Pi / 180 * rotationDeg;
			var dAngle = centerIsOnRight ? 0.1f : -0.1f;
			Func<float, bool> condition = i => centerIsOnRight ? i < endAngle : i > endAngle;

			for (float i = startAngle; condition(i); i += dAngle)
			{
				var x = radius * Cos(i);
				var y = radius * Sin(i);
				var point = new Vector2(x, y);
				points.Add(center + point);
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
			GoStraight(tangent, end, points, accuracy);

			this.dir.Translation = prevDir.ToVec3();
			this.center.Translation = center.ToVec3();
			this.tangent.Translation = tangent.ToVec3();

			return points;
		}
		
		private static void GoStraight(Vector2 start, Vector2 end, List<Vector2> points, float accuracy)
		{
			//go straight
			var dirPointToEnd = (end - start).Normalized(); ;
			var point = start;
			while (point.DistanceSquaredTo(end) > accuracy)
			{
				point += dirPointToEnd * accuracy;
				points.Add(point);
			}
			points.Add(end);
		}

		public List<Vector2> CalculateBezierPoints(Vector2 startPos, Vector2 endPos, int numPoints)
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