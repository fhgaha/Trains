using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailCurve : Curve3D
	{
		public Vector3 Origin { get; set; }

		public RailCurve() { }

		public static RailCurve GetFrom(Path path)
		{
			var curve = (RailCurve)path.Curve;
			curve.Origin = path.Translation;
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

		public void RemoveCurve(RailCurve curveToDelete)
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

		public static bool AreCurvePointsEqual(Vector3 first, Vector3 second, float accuracy)
		{
			return first.IsEqualApprox(second);
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
	}
}
