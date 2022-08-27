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
        private MeshInstance dir;       //green big
        private MeshInstance center;    //green small
        private MeshInstance tangent;   //light blue

        //constants
        private const float Accuracy = 0.1f;

        //vars
        DubinsGeneratePaths dubinsPathGenerator;
        private Vector2 start;
        private Vector2 end;
        private float radius;
        private Vector2 prevDir;

        public override void _Ready()
        {
            dir = GetNode<MeshInstance>("dir");
            center = GetNode<MeshInstance>("center");
            tangent = GetNode<MeshInstance>("tangent");

            dir.Visible = Global.DebugMode;
            center.Visible = Global.DebugMode;
            tangent.Visible = Global.DebugMode;

            dubinsPathGenerator = new DubinsGeneratePaths();
        }

        //if prevDir == Vector2.Zero path will be straight
        public List<Vector2> CalculateCurvePoints(Vector2 start, Vector2 end, Vector2 prevDir)
        {
            this.start = start;
            this.end = end;
            this.radius = 1f;
            this.prevDir = prevDir;
            return prevDir == Vector2.Zero 
                ? GoStraight(start, end).ToList() 
                : CalculateCircleBasedCurve();
        }

        private List<Vector2> CalculateCircleBasedCurve()
        {
            var rotationAngleDeg = GetRotationAngleDeg(prevDir);
            var startEndDir = (end - start).Normalized();
            var prevDirPerp = prevDir.Rotated(Pi / 2);
            var centerIsOnRight = prevDirPerp.Dot(startEndDir) >= 0;   //-1, 0 or 1
            var center = CalculateCenter(centerIsOnRight, start);

            var circlePoints = GetCirclePoints(rotationAngleDeg, centerIsOnRight, center).ToList();
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

        private Vector2 CalculateCenter(bool centerIsOnRight, Vector2 start)
        {
            var prevDirPerp = prevDir.Rotated(Pi / 2).Normalized();
            var radVec = radius * (centerIsOnRight ? prevDirPerp : prevDirPerp.Rotated(Pi));
            return start + radVec;
        }

        private IEnumerable<Vector2> GoStraight(Vector2 start, Vector2 end)
        {
            var dirPointToEnd = (end - start).Normalized();
            var point = start;
            var points = new List<Vector2>();
            while (point.DistanceSquaredTo(end) > Accuracy)
            {
                point += dirPointToEnd * Accuracy;
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
                var tangentIsAhead = dot > perpendicularDot - Accuracy && dot < perpendicularDot + Accuracy;
                return tangentIsAhead;
            });
        }

        private bool CurveShouldNotBeDrawnHere(Vector2 tangent, Vector2 startEndDir)
        {
            //prevent drawing inside circle or from scene origin
            if (tangent == Vector2.Zero) return true;

            //prevent drawing behind start
            var tangetApproxEqualsStart = tangent.IsEqualApprox(start, 0.01f);
            var endIsBehindStartDir = prevDir.Dot(startEndDir) < 0;
            bool curveGoesBehindStartDir = tangetApproxEqualsStart && endIsBehindStartDir;
            return curveGoesBehindStartDir;
        }

        private static void RemoveCirclePointsAfterTangent(Vector2 tangent, List<Vector2> points)
        {
            points.RemoveAll(p => points.IndexOf(p) > points.IndexOf(tangent));
        }

        private void UpdateHelpersPositions(Vector2 center, Vector2 tangent)
        {
            this.dir.Translation = prevDir.ToVec3();
            this.center.Translation = center.ToVec3();
            this.tangent.Translation = tangent.ToVec3();
        }

        public List<Vector2> CalculateCurvePointsWithSnappedEnd(
            Vector2 start, Vector2 end, Vector2 startDir, Vector2 finishDir)
        {
            var paths = dubinsPathGenerator.GetAllDubinsPaths(
                startPos: start.ToVec3(),
                startHeading: startDir.AngleTo(Vector2.Down),
                goalPos: end.ToVec3(),
                goalHeading: finishDir.AngleTo(Vector2.Down));
            return paths[0].pathCoordinates.ConvertAll(p => p.ToVec2());
        }
    }
}
