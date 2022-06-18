using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class CurveSegment : Spatial
	{
		public List<Vector3> Points { get; set; }
	}
}