using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class StartSnapper : Snapper
	{
		public bool IsSnappedOnPathStartOrPathEnd => SnappedDir != Vector3.Zero && SnappedPath != null;
		public bool IsSnappedOnSegment => SnappedPath != null && SnappedMidSegment != null;
		public bool IsSnapped => IsSnappedOnPathStartOrPathEnd || IsSnappedOnSegment;

		public StartSnapper() : base() { }

		public void TrySnap(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
		{
			foreach (var path in pathList)
			{
				if (path.Curve.GetPointCount() == 0) continue;
				Reset();

				var start = path.Start;
				var end = path.End;

				if (IsCursorOn(start, end, mousePos))
				{
					blueprint.Translation = start;
					RotateBlueprint(blueprint, path.DirFromStart);
					SetVars(path.DirFromStart, default, path, default);
					return;
				}
				else if (IsCursorOn(end, start, mousePos))
				{
					blueprint.Translation = end;
					RotateBlueprint(blueprint, path.DirFromEnd);
					SetVars(path.DirFromEnd, default, path, default);
					return;
				}
				else if (TrySnapOnMidSegment(path, mousePos) is CurveSegment segment)
				{
					if (segment is null) continue;

					blueprint.Translation = segment.First;
					SetVars(default, default, path, segment);
					return;
				}
				else
				{
					blueprint.SetOriginalBpCurve();
				}
			}
		}

		private CurveSegment TrySnapOnMidSegment(RailPath path, Vector3 mousePos)
		{
			//snapping can be done to path start or path end. 
			//so we dont want to snap on mid segment if there is only one segment in path.
			//also we dont want to snap on the first segment, cause we would snap on the first point.
			//thats why we skip first point of path or here the first segment.

			var segments = path.GetSegments();
			if (segments.Count < 2) return null;

			for (int i = 1; i < segments.Count; i++)
			{
				if (IsCursorOn(segments[i].First, segments[i].Second, mousePos))
				{
					return segments[i];
				}
			}

			return null;
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
