using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class EndSnapper : Snapper
	{
		public bool IsSnappedOnPathStartOrPathEnd
		=> SnappedDir != Vector3.Zero && SnappedPoint != Vector3.Zero && SnappedPath != null;
		public bool IsSnappedOnSegment => SnappedPath != null && SnappedMidSegment != null;
		public bool IsSnapped => IsSnappedOnPathStartOrPathEnd || IsSnappedOnSegment;

		public EndSnapper() { }

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
					SetVars(path.DirFromStart, start, path, default);
					return;
				}
				else if (IsCursorOn(end, start, mousePos) && !cursorIsOnBpStart)
				{
					SetVars(path.DirFromEnd, end, path, default);
					return;
				}
				else if (TrySnapOnMidSegment(path, mousePos) is CurveSegment segment)
				{
					if (segment is null) continue;

					var bpEndDir = GetBpEndDir(mousePos, blueprint, segment);
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