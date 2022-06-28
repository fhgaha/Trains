using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class RailPath : Path
	{
		public CurvePoint Start { get; set; }
		public CurvePoint End { get; set; }

		public Vector3 _Start { get => Translation + Curve.First(); }
		public Vector3 _End { get => Translation + Curve.Last(); }

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


		public RailPath()
		{
			Start = new CurvePoint();
			End = new CurvePoint();
		}

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