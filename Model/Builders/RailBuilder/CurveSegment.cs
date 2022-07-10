using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Builders
{
	public class CurveSegment : Spatial
	{
		public Vector3[] Points { get; set; } = new Vector3[2];

		//Godot requires parameterless constructor:  Cannot construct temporary 
		// MonoObject because the class does not define a parameterless constructor
		public CurveSegment() { }

		public CurveSegment(IEnumerable<Vector3> points)
		{
			if (points.Count() != 2) throw new ArgumentException("Point count should be 2");

			Points = points.ToArray();
		}

		public CurveSegment(Vector3 first, Vector3 second)
		{
			Points[0] = first;
			Points[1] = second;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			var segment = (CurveSegment)obj;
			return segment.Points[0].Equals(this.Points[0])
				&& segment.Points[1].Equals(this.Points[1]);
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + Points[0].GetHashCode();
			hash = hash * 23 + Points[1].GetHashCode();
			return hash;
		}
	}
}