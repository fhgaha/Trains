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

		public RailCurve() { }

		public void AddCurveToSegments(Curve3D curve, int index)
		{
			var segments = curve.ToSegments();
			Segments.InsertRange(index, segments);
		}

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

		public void RemoveLastSegment()
		{
			if (GetPointCount() < 2)
				return;

			var lastSegment = Segments.Last();
			Segments.Remove(lastSegment);

			var index = GetPointCount() - 1;
			RemoveSegmentFromMap(index);
		}

		private void RemoveSegmentFromMap(int index)
		{
			RemovePoint(index);
			RemovePoint(index - 1);
		}

		public void RemoveCurve(Curve3D curveToDelete)
		{
			var pointsToDeleteAmount = curveToDelete.GetPointCount();
			for (int i = 0; i < pointsToDeleteAmount; i++)
			{
				var index = GetPointCount() - 1;
				RemovePoint(index);
			}
		}
	}
}
