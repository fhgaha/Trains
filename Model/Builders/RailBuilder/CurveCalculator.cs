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
		//helpers
		private MeshInstance dir;
		private MeshInstance center;
		private MeshInstance tangent;

		//constants
		private const float accuracy = 0.1f;

		//vars
		private Vector2 start;
		private Vector2 end;
		private float radius;
		private Vector2 prevDir;
		private List<Vector2> points;

		public override void _Ready()
		{
			dir = GetNode<MeshInstance>("dir");
			center = GetNode<MeshInstance>("center");
			tangent = GetNode<MeshInstance>("tangent");

			dir.Visible = Global.DebugMode;
			center.Visible = Global.DebugMode;
			tangent.Visible = Global.DebugMode;
		}

		public List<Vector2> CalculateCurvePoints(Vector2 start, Vector2 end, Vector2 prevDir, bool firstSegmentIsPlaced)
		{
			this.start = start;
			this.end = end;
			this.radius = 1f;
			this.prevDir = prevDir;
			points = new List<Vector2>();

			if (!firstSegmentIsPlaced)
			{
				GoStraight(start, end);
				return points;
			}

			return CalculateCircleBasedCurve();
		}

		private List<Vector2> CalculateCircleBasedCurve()
		{
			var rotationDeg = GetRotationDeg();
			var startEndDir = (end - start).Normalized();
			var prevDirPerp = prevDir.Rotated(Pi / 2);
			var centerIsOnRight = prevDirPerp.Dot(startEndDir) >= 0;   //-1, 0 or 1
			var center = CalculateCenter(rotationDeg, centerIsOnRight);

			GoAlongCircle(rotationDeg, centerIsOnRight, center);
			var tangent = CalculateTangent(centerIsOnRight, center);

			if (CurveShouldNotBeDrawnHere(tangent, startEndDir))
				return new List<Vector2>();

			RemoveCirclePointsAfterTangent(tangent);
			GoStraight(tangent, end);

			if (Global.DebugMode)
				UpdateHelpersPositions(center, tangent);
			return points;
		}

		private float GetRotationDeg()
		{
			if (prevDir == Vector2.Zero) return 0f;

			var rotationDeg = Vector2.Up.AngleTo(prevDir) * 180 / Pi;
			if (rotationDeg < 0) rotationDeg += 360;
			return rotationDeg;
		}

		private Vector2 CalculateCenter(float rotationDeg, bool centerIsOnRight)
		{
			var d = Pi / 180 * rotationDeg >= Pi / 2 && Pi / 180 * rotationDeg < 3 * Pi / 2 ? -1 : 1;
			var prevDirPerp = new Vector2(d, -d * prevDir.x / prevDir.y).Normalized();
			var radVec = radius * (centerIsOnRight ? prevDirPerp : prevDirPerp.Rotated(Pi));
			var center = start + radVec;
			return center;
		}

		private void GoStraight(Vector2 start, Vector2 end)
		{
			var dirPointToEnd = (end - start).Normalized();
			var point = start;
			while (point.DistanceSquaredTo(end) > accuracy)
			{
				point += dirPointToEnd * accuracy;
				points.Add(point);
			}
			points.Add(end);
		}

		private void GoAlongCircle(float rotationDeg, bool centerIsOnRight, Vector2 center)
		{
			var startAngle = (centerIsOnRight ? Pi : 0) + (Pi / 180 * rotationDeg);
			var endAngle = (centerIsOnRight ? 2 * Pi + Pi / 2 : -Pi - Pi / 2) + Pi / 180 * rotationDeg;
			var dAngle = centerIsOnRight ? 0.1f : -0.1f;
			bool EndAngleIsNotReached(float angle) => centerIsOnRight ? angle < endAngle : angle > endAngle;

			for (float i = startAngle; EndAngleIsNotReached(i); i += dAngle)
			{
				var x = radius * Cos(i);
				var y = radius * Sin(i);
				var point = new Vector2(x, y);
				points.Add(center + point);
			}
		}

		private Vector2 CalculateTangent(bool centerIsOnRight, Vector2 center)
		{
			Vector2 tangent = Vector2.Zero;
			return points.Find(p =>
			{
				var dirPointToCenter = (center - p).Normalized();
				var dirPointToEnd = (end - p).Normalized();
				var dot = centerIsOnRight ? dirPointToCenter.Dot(dirPointToEnd) : dirPointToCenter.Dot(-dirPointToEnd);
				var perpendicularDot = 0;
				var tangentIsAhead = dot > perpendicularDot - accuracy && dot < perpendicularDot + accuracy;
				return tangentIsAhead;
			});
		}

		private bool CurveShouldNotBeDrawnHere(Vector2 tangent, Vector2 startEndDir)
		{
			//prevent drawing inside circle or from scene origin
			if (tangent == Vector2.Zero)
				return true;

			//prevent drawing behind start
			var tangetApproxEqualsStart = tangent.IsEqualApprox(start, 0.01f);
			var endIsBehindStartDir = prevDir.Dot(startEndDir) < 0;
			bool curveGoesBehindStartDir = tangetApproxEqualsStart && endIsBehindStartDir;
			return curveGoesBehindStartDir;
		}

		private void RemoveCirclePointsAfterTangent(Vector2 tangent)
		{
			points.RemoveAll(p => points.IndexOf(p) > points.IndexOf(tangent));
		}

		private void UpdateHelpersPositions(Vector2 center, Vector2 tangent)
		{
			this.dir.Translation = prevDir.ToVec3();
			this.center.Translation = center.ToVec3();
			this.tangent.Translation = tangent.ToVec3();
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