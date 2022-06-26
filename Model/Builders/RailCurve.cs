using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Builders;

namespace Trains
{
	public class RailCurve : Curve3D
	{
		public List<CurveSegment> Curve { get; set; }

		internal void PrependSegment(CurveSegment segment)
		{
			throw new NotImplementedException();
		}

		internal void AppendSegment(CurveSegment segment)
		{
			foreach (var p in segment.Points)
			{
				AddPoint(segment.Origin + p);;
			}
		}
	}
}