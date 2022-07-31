using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains
{
	public class RailCurve : Curve3D
	{
		public Vector3 Origin { get; set; }

		public RailCurve()
		{
			BakeInterval = 1f;
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
			newCurve.Origin = curve.GetPointPosition(0);

			return newCurve;
		}

		public static RailCurve GetSimpleCurve(Vector3 first, Vector3 second)
		{
			var curve = new RailCurve();
			curve.AddPoint(first);
			curve.AddPoint(second);
			return curve;
		}

		public void PrependCurve(Vector3 origin, Curve3D curve)
		{
			PlaceCurveOnMap(origin, curve, _atPosition: 0);
		}

		public void AppendCurve(Vector3 origin, Curve3D curve)
		{
			PlaceCurveOnMap(origin, curve);
		}

		private void PlaceCurveOnMap(Vector3 origin, Curve3D curve, int _atPosition = -1)
		{
			for (int i = 0; i < curve.GetPointCount(); i++)
				AddPoint(origin + curve.GetPointPosition(i), atPosition: _atPosition);
		}

		public void RemoveEdgeCurve(RailCurve curveToDelete)
		{
			var ctdFirst = curveToDelete.First() + curveToDelete.Origin;
			var ctdLast = curveToDelete.Last() + curveToDelete.Origin;
			var oldFirst = this.First() + Origin;
			var oldLast = this.Last() + Origin;

			var accuracy = 1.5f;
			var pointsToDeleteAmount = curveToDelete.GetPointCount();

			//PrintCurveIntersectionInfo(curveToDelete, accuracy);

			if (AreCurvePointsEqual(oldLast, ctdLast, accuracy))
				RemovePointsFromEnd(pointsToDeleteAmount);
			else if (AreCurvePointsEqual(oldFirst, ctdLast, accuracy))
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
			var oldFirst = this.First() + Origin;
			var oldLast = this.Last() + Origin;

			GD.Print("ctdFirst: " + ctdFirst);
			GD.Print("ctdLast: " + ctdLast);
			GD.Print("oldFirst: " + oldFirst);
			GD.Print("oldLast: " + oldLast);
			GD.Print();

			GD.Print("ctdFirst.IsEqualApsprox(oldFirst): ", ctdFirst.IsEqualApprox(oldFirst, accuracy));
			GD.Print("ctdFirst.IsEqualApprox(oldLast): " + ctdFirst.IsEqualApprox(oldLast, accuracy));
			GD.Print("ctdLast.IsEqualApprox(oldFirst): " + ctdLast.IsEqualApprox(oldFirst, accuracy));
			GD.Print("ctdLast.IsEqualApprox(oldLast): " + ctdLast.IsEqualApprox(oldLast, accuracy));
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
