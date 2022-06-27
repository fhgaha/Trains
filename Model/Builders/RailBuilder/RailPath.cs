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

		public Vector3 GetDirFromStart()
		{
			var points = Curve.TakeFirst(2);
			return (points[1] - points[0]).Normalized();
		}

		public Vector3 GetDirFromEnd()
		{
			var points = Curve.TakeLast(2);
			return (points[1] - points[0]).Normalized();
		}
	}
}