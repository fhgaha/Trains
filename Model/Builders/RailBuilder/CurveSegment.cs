using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Builders
{
	public class CurveSegment : Spatial
	{
		private Vector3 first, second;
		public Vector3 First { get => first; private set => first = value; }
		public Vector3 Second { get => second; private set => second = value; }

		//Godot requires parameterless constructor:  Cannot construct temporary 
		// MonoObject because the class does not define a parameterless constructor
		public CurveSegment() { }

		public CurveSegment(Vector3 first, Vector3 second)
		{
			this.first = first;
			this.second = second;
		}

		public CurveSegment(IEnumerable<Vector3> points)
		{
			if (points.Count() != 2) throw new ArgumentException("Point count should be 2");

			first = points.ElementAt(0);
			second = points.ElementAt(1);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			var segment = (CurveSegment)obj;
			return segment.First.Equals(this.first)
				&& segment.Second.Equals(this.second);
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + first.GetHashCode();
			hash = hash * 23 + second.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			return "{ " + first + "; " + second + " }";
		}
	}
}