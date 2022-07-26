using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class StartSnapper : Snapper
	{
		public bool IsBpSnappedOnPathStartOrPathEnd => SnappedDir != Vector3.Zero && SnappedPath != null;
		public bool IsBpSnappedOnSegment => SnappedMidSegment != null;

		public StartSnapper() : base() { }

		public void TrySnapBpStart(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
		{
			foreach (var path in pathList)
			{
				if (path.Curve.GetPointCount() == 0) continue;

				var start = path.Start;
				var end = path.End;

				if (IsCursorOn(start, end, mousePos))
				{
					blueprint.Translation = start;
					RotateBlueprint(blueprint, path.DirFromStart);
					Reset();
					SetVars(path.DirFromStart, default, path, default);
					return;
				}
				else if (IsCursorOn(end, start, mousePos))
				{
					blueprint.Translation = end;
					RotateBlueprint(blueprint, path.DirFromEnd);
					Reset();
					SetVars(path.DirFromEnd, default, path, default);
					return;
				}
				else if (TrySnapEmptyBpOnMidSegment(path, mousePos))
				{
					if (SnappedMidSegment is null) continue;

					blueprint.Translation = SnappedMidSegment.First;
					Reset();
					SetVars(default, default, path, SnappedMidSegment);
					return;
				}
			}
		}

		private bool TrySnapEmptyBpOnMidSegment(RailPath path, Vector3 mousePos)
		{
			//snapping can be done to path start or path end. 
			//so we dont want to snap on mid segment if there is only one segment in path.
			//also we dont want to snap on the first segment, cause we would snap on the first point.
			//thats why we skip first point of path or here the first segment.

			var segments = path.GetSegments();
			if (segments.Count < 2) return false;

			for (int i = 1; i < segments.Count; i++)
			{
				if (IsCursorOn(segments[i].First, segments[i].Second, mousePos))
				{
					SnappedMidSegment = segments[i];
					return true;
				}
			}

			return false;
		}

		public Vector3 GetBpStartSnappedSegmentToCursorDirection(Vector3 mousePos)
		{
			if (SnappedMidSegment is null) throw new NullReferenceException("Snapper.SnappedSegment is null");

			var startToEnd = (SnappedMidSegment.Second - SnappedMidSegment.First).Normalized();
			var endToStart = (SnappedMidSegment.First - SnappedMidSegment.Second).Normalized();
			var startToCursor = (mousePos - SnappedMidSegment.First).Normalized();
			var segmentAndCursorAreOneDirectional = startToEnd.Dot(startToCursor) > 0;

			if (segmentAndCursorAreOneDirectional)
				return startToEnd;
			else
				return endToStart;
		}

		private void RotateBlueprint(RailPath blueprint, Vector3 direction)
		{
			blueprint.SetSimpleCurve(Vector3.Zero, direction);
		}
	}
}