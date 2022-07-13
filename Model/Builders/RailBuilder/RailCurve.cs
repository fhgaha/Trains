using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailCurve : Curve3D
	{
		public List<CurveSegment> Segments { get; set; } = new List<CurveSegment>();
		private List<Vector3> points = new List<Vector3>();
		public Vector3 Origin { get; set; }

		public RailCurve() { }

		public RailCurve(Curve3D curve)
		{
			AppendCurve(curve.First(), curve);
		}

		public RailCurve(RailPath path) : this(path.Curve) { }

		public void PrependCurve(Vector3 origin, Curve3D curve)
		{
			var indexWhereInsertTo = 0;
			PlaceCurveOnMap(origin, curve, _atPosition: 0);
			AddCurveToSegments(curve, indexWhereInsertTo);
		}

		public void AppendCurve(Vector3 origin, Curve3D curve)
		{
			var indexWhereInsertTo = Segments.Count == 0 ? 0 : Segments.IndexOf(Segments.Last());
			PlaceCurveOnMap(origin, curve);
			AddCurveToSegments(curve, indexWhereInsertTo);
		}

		public void PlaceCurveOnMap(Vector3 origin, Curve3D curve, int _atPosition = -1)
		{
			var points = curve.Tessellate();

			for (int i = 0; i < points.Length; i++)
			{
				AddPoint(origin + points[i], atPosition: _atPosition);
			}
		}

		public void AddCurveToSegments(Curve3D curve, int index)
		{
			var segments = curve.ToSegments();
			Segments.InsertRange(index, segments);
		}

		public void RemoveCurve(RailCurve curveToDelete)
		{
			var ctdLast = curveToDelete.Last() + curveToDelete.Origin;
			var oldFirst = this.First() + this.Origin;
			var oldLast = this.Last() + this.Origin;

			var accuracy = 1.5f;
			var pointsToDeleteAmount = curveToDelete.GetPointCount();

			PrintCurveIntersectionInfo(curveToDelete, accuracy);

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
	}
}
