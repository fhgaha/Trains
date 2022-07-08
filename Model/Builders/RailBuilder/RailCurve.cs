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
			//this is not correct. how are you gonna to pick a segment from a tesselated thing?
			var tesselated = curve.Tessellate();
			var segmentsTesselated = new List<CurveSegment>();

			for (int i = 0; i < tesselated.Length; i++)
			{
				Vector3 p = tesselated[i];
				AddPoint(origin + p, atPosition: 0);

				if (i > 0)
				{
					var segment = new CurveSegment(new Vector3[] { tesselated[i - 1], tesselated[i] });
					segmentsTesselated.Add(segment);
				}
			}

			Segments.InsertRange(0, segmentsTesselated);
		}

		public void AppendCurve(Vector3 origin, Curve3D curve)
		{
			//this is not correct. how are you gonna to pick a segment from a tesselated thing?
			var tesselated = curve.Tessellate();
			var segmentsTesselated = new List<CurveSegment>();

			for (int i = 0; i < tesselated.Length; i++)
			{
				Vector3 p = tesselated[i];
				AddPoint(origin + p);

				if (i > 0)
				{
					var segment = new CurveSegment(new Vector3[] { tesselated[i - 1], tesselated[i] });
					segmentsTesselated.Add(segment);
				}
			}

			Segments.AddRange(segmentsTesselated);
		}
	}
}
