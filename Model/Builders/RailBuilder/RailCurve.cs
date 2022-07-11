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

		public RailCurve() { }

		public RailCurve(IEnumerable<Vector3> points)
		{
			AppendCurve(points.First(), new RailCurve(points));
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

		public void AddCurveToSegments(Curve3D curve, int index)
		{
			var points = curve.Tessellate();
			var segmentsTesselated = new List<CurveSegment>();

			for (int i = 1; i < points.Length; i++)
			{
				var segment = new CurveSegment(points[i - 1], points[i]);
				segmentsTesselated.Add(segment);
			}

			Segments.InsertRange(index, segmentsTesselated);
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

		public void RemoveLastCurve()
		{

		}
	}
}
