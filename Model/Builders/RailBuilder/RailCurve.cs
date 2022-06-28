using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailCurve : Curve3D
	{
		public List<CurveSegment> Segments { get; set; }

		public RailCurve() { }

		public RailCurve(List<Vector2> points)
		{
			//points.ForEach(p => AddPoint(p.ToVec3()));
		}

		public void Copy(Curve3D original)
		{
			var segment = new CurveSegment(original.GetBakedPoints());
			Segments.Add(segment);
		}

		public void PrependSegment(Vector3 origin, CurveSegment segment)
		{
			var points = segment.Points;
			//points.Reverse();
			foreach (var p in points)
			{
				AddPoint(origin + p, atPosition: 0);
			}
		}

		public void AppendSegment(Vector3 origin, CurveSegment segment)
		{
			foreach (var p in segment.Points)
			{
				AddPoint(origin + p);
			}
		}
	}
}