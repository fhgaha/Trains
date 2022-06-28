using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Builders
{
	public class CurveSegment : Spatial
	{
		public List<Vector3> Points { get; set; }

		//Godot requires parameterless constructor:  Cannot construct temporary 
		// MonoObject because the class does not define a parameterless constructor
		public CurveSegment() { }

		public CurveSegment(List<Vector3> points)
		{
			Points = points;
		}

		public CurveSegment(Vector3[] points)
		{
			Points = points.ToList();
		}

		public CurveSegment(RailPath path) : this(path.Curve.GetBakedPoints()) { }

		public static List<CurveSegment> ConvertToSegments(List<Vector3> points)
		{
			var segmentLength = 2;
			var segments = new List<CurveSegment>();

			for (int i = 0; i < points.Count; i += segmentLength + 1)
			{
				var portion = new List<Vector3>();
				for (int j = i; j < i + segmentLength; j++)
				{
					if (j > points.Count)
						break;
					portion.Add(points[j]);
				}

				segments.Add(new CurveSegment(portion));
			}
			return segments;
		}

	}
}