using Godot;
using System;

namespace Trains.Model.Builders
{
	public abstract class Snapper 
	{
		protected const float snapDistance = 0.5f;

		public Vector3 SnappedDir { get; set; } = Vector3.Zero;
		public Vector3 SnappedPoint { get; set; } = Vector3.Zero;	//not used in StartSnapper
		public RailPath SnappedPath { get; set; }
		public CurveSegment SnappedMidSegment { get; set; }

		public void Reset()
		{
			SetVars(direction: Vector3.Zero, point: Vector3.Zero, path: null, segment: null);
		}

		protected bool IsCursorOn(Vector3 start, Vector3 end, Vector3 mousePos)
		{
			return start.DistanceTo(mousePos) < snapDistance && start.DistanceTo(mousePos) < end.DistanceTo(mousePos);
		}

		protected void SetVars(Vector3 direction, Vector3 point, RailPath path, CurveSegment segment)
		{
			SnappedDir = direction;
			SnappedPoint = point;
			SnappedPath = path;
			SnappedMidSegment = segment;
		}
	}
}
