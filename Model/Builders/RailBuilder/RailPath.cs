using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailPath : Path
	{
		public Vector3 Start { get; set; } = Vector3.Zero;
		public Vector3 End { get; set; } = Vector3.Zero;
		public Vector3 PrevDir { get; set; }

		public void Init(Path blueprint)
		{
			Transform = blueprint.Transform;
			Curve = (RailCurve)blueprint.Curve;
			GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;
		}

		private Vector3 GetDir(List<Vector3> points) => (points[1] - points[0]).Normalized();
	}
}