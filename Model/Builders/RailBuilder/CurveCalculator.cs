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
		[Export]
		private bool ShowHelpers
		{
			get => showHelpers;
			set
			{
				if (dir == null || center == null || tangent == null) return;

				dir.Visible = value;
				center.Visible = value;
				tangent.Visible = value;
				showHelpers = value;
			}
		}
		private bool showHelpers = true;

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

		//if prevDir == Vector2.Zero path will be straight
		public List<Vector2> CalculateCurvePoints(Vector2 start, Vector2 end, Vector2 prevDir)
		{
			this.start = start;
			this.end = end;
			this.radius = 1f;
			this.prevDir = prevDir;
			points = new List<Vector2>();

			if (prevDir == Vector2.Zero)
				return GoStraight(start, end).ToList();

			return CalculateCircleBasedCurve();
		}

		private List<Vector2> CalculateCircleBasedCurve()
		{
			var rotationDeg = GetRotationAngleDeg(prevDir);
			var startEndDir = (end - start).Normalized();
			var prevDirPerp = prevDir.Rotated(Pi / 2);
			var centerIsOnRight = prevDirPerp.Dot(startEndDir) >= 0;   //-1, 0 or 1
			var center = CalculateCenter(rotationDeg, centerIsOnRight);

			var circlePoints = GetCirclePoints(rotationDeg, centerIsOnRight, center).ToList();
			var tangent = CalculateTangent(centerIsOnRight, center, circlePoints);

			if (CurveShouldNotBeDrawnHere(tangent, startEndDir))
				return new List<Vector2>();

			RemoveCirclePointsAfterTangent(tangent, circlePoints);
			var straightPoints = GoStraight(tangent, end);

			if (Global.DebugMode)
				UpdateHelpersPositions(center, tangent);
			return circlePoints.Concat(straightPoints).ToList();
		}

		private float GetRotationAngleDeg(Vector2 prevDir)
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

		private IEnumerable<Vector2> GoStraight(Vector2 start, Vector2 end)
		{
			var dirPointToEnd = (end - start).Normalized();
			var point = start;
			var points = new List<Vector2>();
			while (point.DistanceSquaredTo(end) > accuracy)
			{
				point += dirPointToEnd * accuracy;
				points.Add(point);
			}
			points.Add(end);
			return points;
		}

		private IEnumerable<Vector2> GetCirclePoints(float rotationDeg, bool centerIsOnRight, Vector2 center)
		{
			var startAngle = (centerIsOnRight ? Pi : 0) + (Pi / 180 * rotationDeg);
			var endAngle = (centerIsOnRight ? Pi + startAngle : -Pi - startAngle) + (Pi / 180 * rotationDeg);
			var dAngle = centerIsOnRight ? 0.1f : -0.1f;
			bool EndAngleIsNotReached(float angle) => centerIsOnRight ? angle < endAngle : angle > endAngle;

			for (float i = startAngle; EndAngleIsNotReached(i); i += dAngle)
			{
				var x = radius * Cos(i);
				var y = radius * Sin(i);
				var point = center + new Vector2(x, y);
				yield return point;
			}
		}

		private Vector2 CalculateTangent(bool centerIsOnRight, Vector2 center, List<Vector2> circlePoints)
		{
			Vector2 tangent = Vector2.Zero;
			return circlePoints.Find(p =>
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

		private void RemoveCirclePointsAfterTangent(Vector2 tangent, List<Vector2> points)
		{
			points.RemoveAll(p => points.IndexOf(p) > points.IndexOf(tangent));
		}

		private void UpdateHelpersPositions(Vector2 center, Vector2 tangent)
		{
			this.dir.Translation = prevDir.ToVec3();
			this.center.Translation = center.ToVec3();
			this.tangent.Translation = tangent.ToVec3();
		}

		public List<Vector2> CalculateCurvePointsWithSnappedEnd(Vector2 start, Vector2 end, Vector2 startDir, Vector2 finishDir)
		{
			this.start = start;
			this.end = end;
			this.radius = 1f;
			this.prevDir = startDir;
			points = new List<Vector2>();

			//вычисляем точку дуги от старта
			//вычисляем направление дуги старта
			//вычисляем точку дуги от конца
			//вычисляем направление дуги конца
			//сравниваем направления
			//если направления противоположны, прерываемся, строим прямую от последней точки стартовой дуги
			//к последней точке конечной дуги
			//разворачиваем точки конечной дуги
			//объединяем все в один список и возвращаем

			var s_rotationAngleDeg = GetRotationAngleDeg(startDir);
			var s_startEndDir = (end - start).Normalized();
			var s_prevDirPerp = startDir.Rotated(Pi / 2);
			var s_centerIsOnRight = s_prevDirPerp.Dot(s_startEndDir) >= 0;   //-1, 0 or 1
			var s_center = CalculateCenter(s_rotationAngleDeg, s_centerIsOnRight);
			var startCirclePoints = GetCirclePoints(s_rotationAngleDeg, s_centerIsOnRight, s_center).ToList();

			var s_tangent = CalculateTangent(s_centerIsOnRight, s_center, startCirclePoints);
			if (CurveShouldNotBeDrawnHere(s_tangent, s_startEndDir))
				return new List<Vector2>();
			RemoveCirclePointsAfterTangent(s_tangent, startCirclePoints);

			// return startCirclePoints;

			var f_rotationDeg = GetRotationAngleDeg(finishDir);
			var f_startEndDir = (start - end).Normalized();
			var f_finishDirPerp = finishDir.Rotated(Pi / 2);
			var f_centerIsOnRight = f_finishDirPerp.Dot(f_startEndDir) >= 0;   //-1, 0 or 1
			var f_center = CalculateCenter(f_rotationDeg, f_centerIsOnRight);
			var finishCirclePoints = GetCirclePoints(f_rotationDeg, f_centerIsOnRight, f_center).ToList();
			finishCirclePoints.Reverse();

			var f_tangent = CalculateTangent(f_centerIsOnRight, f_center, finishCirclePoints);
			if (CurveShouldNotBeDrawnHere(f_tangent, f_startEndDir))
				return new List<Vector2>();
			RemoveCirclePointsAfterTangent(f_tangent, finishCirclePoints);

			var straightPoints = GoStraight(s_tangent, f_tangent);

			return startCirclePoints
				.Concat(straightPoints)
				.Concat(finishCirclePoints)
				.ToList();
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
