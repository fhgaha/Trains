using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class EndSnapper : Snapper
	{
		public bool IsSnappedOnPathStartOrPathEnd
			=> SnappedDir != default
			&& SnappedPoint != default
			&& SnappedPath != default
			&& SnappedMidSegment == default;
		public bool IsSnappedOnSegment
			=> SnappedDir != default
			&& SnappedPoint != default
			&& SnappedPath != default
			&& SnappedMidSegment != default;
		public bool IsSnapped => IsSnappedOnPathStartOrPathEnd || IsSnappedOnSegment;

		public void TrySnap(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
		{
			foreach (var path in pathList)
			{
				if (path.Curve.GetPointCount() == 0) continue;

				var start = path.Start;
				var end = path.End;
				bool cursorIsOnBpStart = mousePos.IsEqualApprox(blueprint.Start, snapDistance);

				if (IsCursorOn(start, end, mousePos) && !cursorIsOnBpStart)
				{
					path.RegisterCrossing(start);
					SetVars(path.DirFromStart, start, path, default);
					return;
				}
				else if (IsCursorOn(end, start, mousePos) && !cursorIsOnBpStart)
				{
					path.RegisterCrossing(end);
					SetVars(path.DirFromEnd, end, path, default);
					return;
				}
				else if (TrySnapOnMidSegment(path, mousePos) is CurveSegment segment)
				{
					if (segment is null) continue;

					var bpEndDir = GetBpEndDir(mousePos, blueprint, segment);
					path.RegisterCrossing(segment.First);
					SetVars(bpEndDir, segment.First, path, segment);
					return;
				}
			}
			Reset();
		}

		private CurveSegment TrySnapOnMidSegment(RailPath path, Vector3 mousePos)
		{
			var segments = path.GetSegments();
			if (segments.Count < 2) return null;

			for (int i = 1; i < segments.Count; i++)
			{
				if (IsCursorOn(segments[i].First, segments[i].Second, mousePos))
					return segments[i];
			}

			return null;
		}

		private static Vector3 GetBpEndDir(Vector3 mousePos, RailPath blueprint, CurveSegment segment)
		{
			var bpStartCursorDir = (mousePos - blueprint.Start).Normalized();
			var codirectionalVector = bpStartCursorDir.Dot(segment.Direction) < 0
				? segment.Direction
				: segment.Direction.Rotated(Vector3.Up, Mathf.Pi);
			return codirectionalVector;
		}
	}
}