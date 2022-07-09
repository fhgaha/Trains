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
	}
}