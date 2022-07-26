using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Builders
{
	public class CurveSegment : Spatial
	{
		public Vector3 First { get; private set; }
		public Vector3 Second { get; private set; }

		public Vector3 Direction => (Second - First).Normalized();

		//Godot requires parameterless constructor:  Cannot construct temporary 
		// MonoObject because the class does not define a parameterless constructor
		public CurveSegment() { }

		public CurveSegment(Vector3 first, Vector3 second)
		{
			First = first;
			Second = second;
		}

		public CurveSegment(IEnumerable<Vector3> points)
		{
			if (points.Count() != 2) throw new ArgumentException("Point count should be 2");

			First = points.ElementAt(0);
			Second = points.ElementAt(1);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			var segment = (CurveSegment)obj;
			return segment.First.Equals(this.First)
				&& segment.Second.Equals(this.Second);
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + First.GetHashCode();
			hash = hash * 23 + Second.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			return "{ " + First + "; " + Second + " }";
		}
	}
}