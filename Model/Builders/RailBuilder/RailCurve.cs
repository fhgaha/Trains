using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trains.Model.Common;

namespace Trains
{
	public class RailCurve : Curve3D
	{
		public Vector3 Origin { get; set; }

		public RailCurve()
		{
			BakeInterval = 0.2f;
		}

		public static RailCurve GetFrom(Path path)
		{
			var curve = GetFrom(path.Curve);
			curve.Origin = path.Translation;
			return curve;
		}

		public static RailCurve GetFrom(Curve3D curve)
		{
			var newCurve = new RailCurve();

			for (int i = 0; i < curve.GetPointCount(); i++)
				newCurve.AddPoint(curve.GetPointPosition(i));
			//newCurve.Origin = curve.GetPointPosition(0);

			return newCurve;
		}

		public static RailCurve GetSimpleCurve(Vector3 first, Vector3 second)
		{
			var curve = new RailCurve();
			curve.AddPoint(first);
			curve.AddPoint(second);
			return curve;
		}

		public RailCurve PrependCurve(Vector3 origin, Curve3D curve)
		{
			// return PlaceCurveOnMap(origin, curve, _atPosition: 0);
			return PlaceCurveOnMapParallel(origin, curve, _atPosition: 0);
		}

		public RailCurve AppendCurve(Vector3 origin, Curve3D curve)
		{
			// return PlaceCurveOnMap(origin, curve);
			return PlaceCurveOnMapParallel(origin, curve);
		}

		private RailCurve PlaceCurveOnMap(Vector3 origin, Curve3D curve, int _atPosition = -1)
		{
			for (int i = 0; i < curve.GetPointCount(); i++)
				AddPoint(origin + curve.GetPointPosition(i), atPosition: _atPosition);
			return (RailCurve)curve;
		}

		private RailCurve PlaceCurveOnMapParallel(Vector3 origin, Curve3D curve, int _atPosition = -1)
		{
			Parallel.ForEach(curve.ToArray(), (p, state, index) =>
			{
				AddPoint(origin + curve.GetPointPosition((int)index), atPosition: _atPosition);
			});
			return (RailCurve)curve;
		}

		public void RemoveEdgeCurve(RailCurve curveToDelete)
		{
			var ctdFirst = curveToDelete.First() + curveToDelete.Origin;
			var ctdLast = curveToDelete.Last() + curveToDelete.Origin;
			var thisFirst = this.First() + this.Origin;
			var thisLast = this.Last() + this.Origin;

			var accuracy = 1.5f;
			var pointsToDeleteAmount = curveToDelete.GetPointCount();

			//PrintCurveIntersectionInfo(curveToDelete, accuracy);

			var thisLastEqualsCtdLast = AreCurvePointsEqual(thisLast, ctdLast, accuracy);
			var thisFirstEqualsCtdLast = AreCurvePointsEqual(thisFirst, ctdLast, accuracy);

			if (thisLastEqualsCtdLast)
				RemovePointsFromEnd(pointsToDeleteAmount);
			else if (thisFirstEqualsCtdLast)
				RemovePointsFromStart(pointsToDeleteAmount);
		}

		private void RemovePointsFromStart(int pointsToDeleteAmount)
		{
			for (int i = 0; i < pointsToDeleteAmount; i++)
				RemovePoint(0);
		}

		private void RemovePointsFromEnd(int pointsToDeleteAmount)
		{
			for (int i = 0; i < pointsToDeleteAmount; i++)
				RemovePoint(GetPointCount() - 1);
		}

		public static bool AreCurvePointsEqual(Vector3 first, Vector3 second)
		{
			return first.IsEqualApprox(second);
		}

		public static bool AreCurvePointsEqual(Vector3 first, Vector3 second, float accuracy)
		{
			return first.IsEqualApprox(second, accuracy);
		}

		private void PrintCurveIntersectionInfo(RailCurve curveIntesected, float accuracy)
		{
			var ctdFirst = curveIntesected.First() + curveIntesected.Origin;
			var ctdLast = curveIntesected.Last() + curveIntesected.Origin;
			var thisFirst = this.First() + Origin;
			var thisLast = this.Last() + Origin;

			GD.Print("ctdFirst: " + ctdFirst);
			GD.Print("ctdLast: " + ctdLast);
			GD.Print("thisFirst: " + thisFirst);
			GD.Print("thisLast: " + thisLast);
			GD.Print();

			GD.Print("ctdFirst.IsEqualApsprox(thisFirst): ", ctdFirst.IsEqualApprox(thisFirst, accuracy));
			GD.Print("ctdFirst.IsEqualApprox(thisLast): " + ctdFirst.IsEqualApprox(thisLast, accuracy));
			GD.Print("ctdLast.IsEqualApprox(thisFirst): " + ctdLast.IsEqualApprox(thisFirst, accuracy));
			GD.Print("ctdLast.IsEqualApprox(thisLast): " + ctdLast.IsEqualApprox(thisLast, accuracy));
			GD.Print();
		}

		internal List<CurveSegment> GetSegments(Vector3 origin)
		{
			var segments = new List<CurveSegment>();
			var baked = GetBakedPoints();

			for (int i = 1; i < baked.Length; i++)
			{
				var first = origin + baked[i - 1];
				var second = origin + baked[i];
				var segment = new CurveSegment(first, second);
				segments.Add(segment);
			}

			if (segments.Count == 0)
				GD.PushWarning("RailCurve.GetSegments() returns empty list");

			return segments;
		}

		internal void Rotate(Vector3 axis, float angle)
		{
			for (int i = 0; i < GetPointCount(); i++)
			{
				var newPos = GetPointPosition(i).Rotated(axis, angle);
				SetPointPosition(i, newPos);
			}
		}
	}
}
