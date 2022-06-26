using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class CurveSegment : Spatial
	{
		public Vector3[] Points { get; set; }
		public Vector3 Origin { get; set; }

		public CurveSegment() { }	//Godot requires parameterless constructor:  Cannot construct temporary 
		// MonoObject because the class does not define a parameterless constructor

		public CurveSegment(Vector3 origin, Vector3[] points)
		{
			this.Origin = origin;
			this.Points = points;
		}

	}
}