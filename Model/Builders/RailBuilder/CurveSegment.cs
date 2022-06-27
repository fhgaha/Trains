using Godot;
using System;

namespace Trains.Model.Builders
{
	public class CurveSegment : Spatial
	{
		public Vector3[] Points { get; set; }

		//Godot requires parameterless constructor:  Cannot construct temporary 
		// MonoObject because the class does not define a parameterless constructor
		public CurveSegment() { }

		public CurveSegment(Vector3[] points)
		{
			this.Points = points;
		}
	}
}