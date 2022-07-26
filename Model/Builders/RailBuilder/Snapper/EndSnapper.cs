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

				if (IsCursorOn(start, end, mousePos)
				&& !mousePos.IsEqualApprox(blueprint.Start, snapDistance))
				{
					SetVars(path.DirFromStart, start, path, default);
					return;
				}
				else if (IsCursorOn(end, start, mousePos)
				&& !mousePos.IsEqualApprox(blueprint.Start, snapDistance))
				{
					SetVars(path.DirFromEnd, end, path, default);
					return;
				}
				else if (TrySnapOnMidSegment(path, mousePos))
				{
					if (SnappedMidSegment is null) continue;

					SetVars(default, SnappedMidSegment.First, path, SnappedMidSegment);
					return;
				}
			}
			Reset();
		}

		private bool TrySnapOnMidSegment(RailPath path, Vector3 mousePos)
		{
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
	}
}