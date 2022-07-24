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
			var center = CalculateCenter(rotationDeg, centerIsOnRight, start);

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

		private Vector2 CalculateCenter(float rotationDeg, bool centerIsOnRight, Vector2 start)
		{
			var k = rotationDeg >= 90 && rotationDeg < 270 ? -1 : 1;
			var prevDirPerp = new Vector2(k, -k * prevDir.x / prevDir.y).Normalized();
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
			//дуги могут иметь разное кол-во т-к, поэтому направляения чаще всего сходиться 
			//не будутю нужно расчитать сперва полностью окружность одной дуги, затем расчитываеть точки другой 
			//и когда первая точка первой окр-ти окажется на прямой, выходящей из направления второй окружности, 
			//тогда цикл надо прерывать и удалть все точки после найденной

			//return CalculateCurvePoints(start, end, startDir);

			this.start = start;
			this.end = end;
			this.radius = 1f;
			this.prevDir = startDir;

			var s_rotationAngleDeg = GetRotationAngleDeg(startDir);
			var s_startEndDir = (end - start).Normalized();
			var s_prevDirPerp = startDir.Rotated(Pi / 2);
			var s_centerIsOnRight = s_prevDirPerp.Dot(s_startEndDir) >= 0;   //-1, 0 or 1
			var s_center = CalculateCenter(s_rotationAngleDeg, s_centerIsOnRight, start);

			var f_rotationDeg = GetRotationAngleDeg(finishDir);
			var f_startEndDir = (start - end).Normalized();
			var f_finishDirPerp = finishDir.Rotated(Pi / 2);
			var f_centerIsOnRight = f_finishDirPerp.Dot(f_startEndDir) >= 0;   //-1, 0 or 1
			var f_center = CalculateCenter(f_rotationDeg, f_centerIsOnRight, end);

			//do
			var finishCirclePoints = GetCirclePoints(f_rotationDeg, f_centerIsOnRight, f_center).ToList();
			var startCirclePoints = new List<Vector2>();
			var firstAndLastStraightPoints = new Vector2[2];
			firstAndLastStraightPoints = CalculateStartCirclePointsAndFirstAndLastStarightPoints(
				s_rotationAngleDeg, s_centerIsOnRight, s_center,
				finishCirclePoints, startCirclePoints, firstAndLastStraightPoints);

			if (firstAndLastStraightPoints.Length < 2)
				return new List<Vector2>();

			var s_tangent = firstAndLastStraightPoints[0];
			var f_tangent = firstAndLastStraightPoints[1];

			if (CurveShouldNotBeDrawnHere(s_tangent, s_startEndDir))
				return new List<Vector2>();
			//RemoveCirclePointsAfterTangent(s_tangent, startCirclePoints);
			RemoveCirclePointsAfterTangent(f_tangent, finishCirclePoints);

			finishCirclePoints.Reverse();
			var straightPoints = GoStraight(s_tangent, f_tangent);

			//return finishCirclePoints;

			return startCirclePoints
				.Concat(straightPoints)
				.Concat(finishCirclePoints)
				.ToList();

			return new List<Vector2>();
		}

		private Vector2[] CalculateStartCirclePointsAndFirstAndLastStarightPoints(
			float s_rotationAngleDeg, bool s_centerIsOnRight, Vector2 s_center,
			List<Vector2> finishCirclePoints, List<Vector2> startCirclePoints,
			Vector2[] firstAndLastStraightPoints)
		{
			var startAngleDeg = (s_centerIsOnRight ? 90 : 0) + (int)s_rotationAngleDeg;
			var endAngleDeg = (s_centerIsOnRight ? startAngleDeg + 180 : startAngleDeg - 180) + s_rotationAngleDeg;
			var dAngleDeg = s_centerIsOnRight ? 1 : -1;
			bool EndAngleIsNotReached(int angleDeg) => s_centerIsOnRight ? angleDeg < endAngleDeg : angleDeg > endAngleDeg;

			for (int i = startAngleDeg; EndAngleIsNotReached(i); i += dAngleDeg)
			{
				var x = radius * Cos(i * Pi / 180);
				var y = radius * Sin(i * Pi / 180);
				var point = s_center + new Vector2(x, y);
				startCirclePoints.Add(point);

				if (startCirclePoints.Count > 1)
				{
					firstAndLastStraightPoints = CalculateFirstAndLastStraightLinepoints(
						s_centerIsOnRight, s_center, startCirclePoints, finishCirclePoints);

					if (firstAndLastStraightPoints.Length == 2)
						break;
				}
			}

			return firstAndLastStraightPoints;
		}

		private Vector2[] CalculateFirstAndLastStraightLinepoints(
			bool centerIsOnRight,
			Vector2 center,
			List<Vector2> circlePoints,
			List<Vector2> pointsToPickFrom)
		{
			var lastPoint = circlePoints[circlePoints.Count - 1];   //this is wrong
			var prelastPoint = circlePoints[circlePoints.Count - 2];
			var direction = lastPoint - prelastPoint;
			var ray = direction * 1000;

			var lastAngle = centerIsOnRight ? 2 * Pi : -2 * Pi;
			bool notReachedLastAngle(float i) => centerIsOnRight ? i < lastAngle : i > lastAngle;
			var dAngle = centerIsOnRight ? 0.1f : -0.1f;

			for (float i = 0; notReachedLastAngle(i); i += dAngle)
			{
				foreach (var p in pointsToPickFrom)
				{
					if (IsPointOnLineApprox(lastPoint, ray, p, 0.01f))
						return new Vector2[] { lastPoint, p };
				}

				ray = ray.Rotated(i);
			}

			return new Vector2[0];
		}

		private bool IsPointOnLineApprox(Vector2 lineStartPoint, Vector2 lineEndPoint, Vector2 p, float accuracy)
		{
			//https://lms2.sseu.ru/courses/eresmat/gloss/g115.htm
			//Даны две точки M1 (x1, y1) и M2 (x2, y2). Уравнение прямой, проходящей через две данные точки:
			// (y - y1)/(y2 - y1) = (x - x1)/(x2 - x1)

			float yk = (p.y - lineStartPoint.y) / (lineEndPoint.y - lineStartPoint.y);
			float xk = (p.x - lineStartPoint.x) / (lineEndPoint.x - lineStartPoint.x);

			var pointBelongsPresisely = yk == xk;
			var pointBelongsApprox = Math.Abs(yk - xk) < accuracy && Math.Abs(xk - yk) < accuracy;
			return pointBelongsApprox;
		}

		private void CalculateStartAndFinishCirclePoints(
			List<Vector2> startCirclePoints,
			List<Vector2> finishCirclePoints,
			float s_rotationAngleDeg,
			bool s_centerIsOnRight,
			Vector2 s_center,
			float f_rotationAngleDeg,
			bool f_centerIsOnRight,
			Vector2 f_center)
		{
			var s_startAngle = (s_centerIsOnRight ? Pi : 0) + (Pi / 180 * s_rotationAngleDeg);
			var s_dAngle = s_centerIsOnRight ? 0.1f : -0.1f;

			var f_startAngle = (f_centerIsOnRight ? Pi : 0) + (Pi / 180 * f_rotationAngleDeg);
			var f_dAngle = f_centerIsOnRight ? 0.1f : -0.1f;

			while (true)
			{
				var s_x = radius * Cos(s_startAngle);
				var s_y = radius * Sin(s_startAngle);
				var s_point = s_center + new Vector2(s_x, s_y);
				startCirclePoints.Add(s_point);

				var f_x = radius * Cos(f_startAngle);
				var f_y = radius * Sin(f_startAngle);
				var f_point = f_center + new Vector2(f_x, f_y);
				finishCirclePoints.Add(f_point);

				if (startCirclePoints.Count > 1 && finishCirclePoints.Count > 1)
				{
					var fromStartDir = (startCirclePoints[startCirclePoints.Count - 1]
									  - startCirclePoints[startCirclePoints.Count - 2])
									  .Normalized();

					var fromFinishDir = (finishCirclePoints[finishCirclePoints.Count - 1]
									  - finishCirclePoints[finishCirclePoints.Count - 2])
									  .Normalized();

					GD.PrintS("\n",
						"fromStartDir:", fromStartDir, "\n",
						"fromFinishDir:", fromFinishDir
						);

					var fromFinishDirRotated = fromFinishDir.Rotated(Pi);
					var equal = fromStartDir.IsEqualApprox(fromFinishDirRotated);
					if (equal)
						return;
				}

				s_startAngle += s_dAngle;
				f_startAngle += f_dAngle;
			}
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
