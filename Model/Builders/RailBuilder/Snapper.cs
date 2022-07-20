using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class Snapper : Node
	{
		public RailPath SnappedStartPath { get; set; }
		public Vector3 SnappedStartDir { get; set; } = Vector3.Zero;
		public CurveSegment SnappedStartSegment { get; set; }

		private const float snapDistance = 1f;
		private Vector3 mousePos;

		public Snapper() { }

		public bool IsBpStartSnappedOnSegment() => SnappedStartSegment != null;

		public void Reset() => SetVars(null, Vector3.Zero, null);

		public Vector3 GetBpStartSnappedSegmentToCursorDirection(Vector3 mousePos)
		{
			if (SnappedStartSegment is null) throw new NullReferenceException("Snapper.SnappedSegment is null");

			var startToEnd = (SnappedStartSegment.Second - SnappedStartSegment.First).Normalized();
			var endToStart = (SnappedStartSegment.First - SnappedStartSegment.Second).Normalized();
			var startToCursor = (mousePos - SnappedStartSegment.First).Normalized();
			var segmentAndCursorAreOneDirectional = startToEnd.Dot(startToCursor) > 0;

			if (segmentAndCursorAreOneDirectional)
				return startToEnd;
			else
				return endToStart;
		}

		public void TrySnapBpStart(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
		{
			this.mousePos = mousePos;

			foreach (var path in pathList)
			{
				if (path.Curve.GetPointCount() == 0) continue;

				var start = path.Start;
				var end = path.End;

				if (IsCursorOn(start, end))
				{
					blueprint.Translation = start;
					RotateBlueprint(blueprint, path.DirFromStart);
					SetVars(path, path.DirFromStart, null);
					return;
				}
				else if (IsCursorOn(end, start))
				{
					blueprint.Translation = end;
					RotateBlueprint(blueprint, path.DirFromEnd);
					SetVars(path, path.DirFromEnd, null);
					return;
				}
				else if (TrySnapEmptyBpOnMidSegment(path))
				{
					if (SnappedStartSegment is null) continue;

					blueprint.Translation = SnappedStartSegment.First;
					SetVars(path, Vector3.Zero, SnappedStartSegment);
					return;
				}
			}

			SnappedStartSegment = null;
			SetVars(null, Vector3.Zero, null);
		}

		private bool IsCursorOn(Vector3 start, Vector3 end)
		{
			return start.DistanceTo(mousePos) < snapDistance && start.DistanceTo(mousePos) < end.DistanceTo(mousePos);
		}

		private void RotateBlueprint(RailPath blueprint, Vector3 direction)
		{
			blueprint.SetSimpleCurve(Vector3.Zero, direction);
		}

		private bool TrySnapEmptyBpOnMidSegment(RailPath path)
		{
			foreach (var s in path.GetSegments())
			{
				if (IsCursorOn(s.First, s.Second))
				{
					SnappedStartSegment = s;
					return true;
				}
			}

			return false;
		}

		public void AlignBpForEnd(RailPath blueprint)
		{
			//reset polygon position if previously it was translated
			blueprint.GetNode<CSGPolygon>("CSGPolygon").Translation = new Vector3(0, 0, 0);
		}

		private void SetVars(RailPath path, Vector3 direction, CurveSegment segment)
		{
			SnappedStartPath = path;
			SnappedStartDir = direction;
			SnappedStartSegment = segment;
		}

		public void TrySnapBpEnd(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
		{
			foreach (var path in pathList)
			{
				if (path.Curve.GetPointCount() == 0) continue;

				var start = path.Start;
				var end = path.End;

				var calculator = new CurveCalculator();
				
				if (IsCursorOn(start, end))
				{
					
					//кривая, пярмая, кривая
				}
				else if (IsCursorOn(end, start))
				{
					
				}
				else if (TrySnapEmptyBpOnMidSegment(path))
				{
					
				}
			}
		}
	}
}
