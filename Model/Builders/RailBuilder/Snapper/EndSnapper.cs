using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class EndSnapper : Snapper
	{
		public bool IsBpSnappedOnPathStartOrPathEnd
		=> SnappedDir != Vector3.Zero && SnappedPoint != Vector3.Zero && SnappedPath != null;
		public bool IsBpSnappedOnSegment => SnappedMidSegment != null;

		public EndSnapper() { }

		public void TrySnapBpEnd(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint, RailPath currentPath)
		{
			foreach (var path in pathList)
			{
				if (path.Curve.GetPointCount() == 0) continue;

				var start = path.Start;
				var end = path.End;

				if (IsCursorOn(start, end, mousePos)
				&& !mousePos.IsEqualApprox(blueprint.Start, snapDistance))
				{
					Reset();
					SetVars(path.DirFromStart, start, path, default);
					return;
				}
				else if (IsCursorOn(end, start, mousePos)
				&& !mousePos.IsEqualApprox(blueprint.Start, snapDistance))
				{
					Reset();
					SetVars(path.DirFromEnd, end, path, default);
					return;
				}
				else if (TrySnapFilledBpOnMidSegment(path, mousePos))
				{
					if (SnappedMidSegment is null) continue;

					Reset();
					SetVars(default, default, path, SnappedMidSegment);
					return;
				}
			}
			Reset();
		}

		private bool TrySnapFilledBpOnMidSegment(RailPath path, Vector3 mousePos)
		{
			foreach (var s in path.GetSegments())
			{
				if (IsCursorOn(s.First, s.Second, mousePos))
				{
					SnappedMidSegment = s;
					return true;
				}
			}

			return false;
		}
	}
}