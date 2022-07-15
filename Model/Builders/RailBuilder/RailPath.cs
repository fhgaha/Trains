using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailPath : Path
	{
		public Vector3 Start { get => Translation + Curve.First(); }
		public Vector3 End { get => Translation + Curve.Last(); }

		public Vector3 DirFromStart
		{
			get
			{
				var points = Curve.TakeFirst(2);
				return (points[1] - points[0]).Normalized();
			}
		}
		public Vector3 DirFromEnd
		{
			get
			{
				var points = Curve.TakeLast(2);
				return (points[1] - points[0]).Normalized();
			}
		}

		private Curve3D originalBpCurve;

		public RailPath() { }

		public override void _Ready()
		{
			originalBpCurve = Curve;
		}

		public void Init(Path blueprint)
		{
			Transform = blueprint.Transform;
			Curve = (RailCurve)blueprint.Curve;
			GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;
		}

		public void SetSimpleCurve(Vector3 direction)
		{
			var curve = new Curve3D();
			curve.AddPoint(Vector3.Zero);
			curve.AddPoint(direction);
			Curve = curve;
		}

		public void SetOriginalBpCurve()
		{
			Curve = originalBpCurve;
		}
	}
}
