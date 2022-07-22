using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class Snapper : Node
	{
		public RailPath SnappedStartPath { get; set; }
		public Vector3 SnappedStartDir { get; set; } = Vector3.Zero;
		public CurveSegment SnappedStartMidSegment { get; set; }

		public RailPath SnappedEndPath { get; set; }
		public Vector3 SnappedEndDir { get; set; } = Vector3.Zero;
		public CurveSegment SnappedEndMidSegment { get; set; }

		private const float snapDistance = 0.5f;

		public Snapper() { }

		public bool IsBpStartSnappedOnPathStartOrPathEnd() => SnappedStartPath != null && SnappedStartDir != Vector3.Zero;
		public bool IsBpStartSnappedOnSegment() => SnappedStartMidSegment != null;
		public bool IsBpEndSnappedOnPathStartOrPathEnd() => SnappedEndPath != null && SnappedEndDir != Vector3.Zero;
		public bool IsBpEndSnappedOnSegment() => SnappedEndMidSegment != null;

		public void Reset()
		{
			SetStartVars(null, Vector3.Zero, null);
			SetEndVars(null, Vector3.Zero, null);
		}

		public Vector3 GetBpStartSnappedSegmentToCursorDirection(Vector3 mousePos)
		{
			if (SnappedStartMidSegment is null) throw new NullReferenceException("Snapper.SnappedSegment is null");

			var startToEnd = (SnappedStartMidSegment.Second - SnappedStartMidSegment.First).Normalized();
			var endToStart = (SnappedStartMidSegment.First - SnappedStartMidSegment.Second).Normalized();
			var startToCursor = (mousePos - SnappedStartMidSegment.First).Normalized();
			var segmentAndCursorAreOneDirectional = startToEnd.Dot(startToCursor) > 0;

			if (segmentAndCursorAreOneDirectional)
				return startToEnd;
			else
				return endToStart;
		}

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
					SetStartVars(path, path.DirFromStart, null);
					return;
				}
				else if (IsCursorOn(end, start, mousePos))
				{
					blueprint.Translation = end;
					RotateBlueprint(blueprint, path.DirFromEnd);
					SetStartVars(path, path.DirFromEnd, null);
					return;
				}
				else if (TrySnapEmptyBpOnMidSegment(path, mousePos))
				{
					if (SnappedStartMidSegment is null) continue;

					blueprint.Translation = SnappedStartMidSegment.First;
					SetStartVars(path, Vector3.Zero, SnappedStartMidSegment);
					return;
				}
			}

			SnappedStartMidSegment = null;
			SetStartVars(null, Vector3.Zero, null);
		}

		private bool IsCursorOn(Vector3 start, Vector3 end, Vector3 mousePos)
		{
			return start.DistanceTo(mousePos) < snapDistance && start.DistanceTo(mousePos) < end.DistanceTo(mousePos);
		}

		private void RotateBlueprint(RailPath blueprint, Vector3 direction)
		{
			blueprint.SetSimpleCurve(Vector3.Zero, direction);
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
					SnappedStartMidSegment = segments[i];
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

		private void SetStartVars(RailPath path, Vector3 direction, CurveSegment segment)
		{
			SnappedStartPath = path;
			SnappedStartDir = direction;
			SnappedStartMidSegment = segment;
		}

		private void SetEndVars(RailPath path, Vector3 direction, CurveSegment segment)
		{
			//GD.PrintS(DateTime.Now.Ticks, path, direction, segment);
			SnappedEndPath = path;
			SnappedEndDir = direction;
			SnappedEndMidSegment = segment;
		}

		public void TrySnapBpEnd(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint, RailPath currentPath)
		{
			foreach (var path in pathList)
			{
				if (path.Curve.GetPointCount() == 0) continue;

				var start = path.Start;
				var end = path.End;

				// GD.PrintS("\n",
				// 	"end:", end, "\n",
				// 	"mousePos:", mousePos, "\n",
				// 	"IsCursorOn(end, start, mousePos):", IsCursorOn(end, start, mousePos), "\n",
				// 	"!end.IsEqualApprox(mousePos, snapDistance):", !end.IsEqualApprox(mousePos, snapDistance)
				// );

				if (IsCursorOn(start, end, mousePos)
				&& !mousePos.IsEqualApprox(blueprint.Start, snapDistance)
				)
				{
					//blueprint.Translation = start;
					//RotateBlueprint(blueprint, path.DirFromStart);
					SetEndVars(path, path.DirFromStart, null);
					GD.PrintS(DateTime.Now.Ticks, "snapped bp end to path start");
					return;
				}
				else if (IsCursorOn(end, start, mousePos)
				&& !mousePos.IsEqualApprox(blueprint.Start, snapDistance)
				)
				{
					//blueprint.Translation = end;
					//RotateBlueprint(blueprint, path.DirFromEnd);
					SetEndVars(path, path.DirFromEnd, null);

					GD.PrintS(DateTime.Now.Ticks, "snapped bp end to path end");

					return;
				}
				else if (TrySnapFilledBpOnMidSegment(path, mousePos))
				{
					if (SnappedStartMidSegment is null) continue;

					//blueprint.Translation = SnappedStartSegment.First;
					SetEndVars(path, Vector3.Zero, SnappedStartMidSegment);
					GD.PrintS(DateTime.Now.Ticks, "snapped bp end to path mid segment");
					return;
				}
			}
		}

		private bool TrySnapFilledBpOnMidSegment(RailPath path, Vector3 mousePos)
		{
			foreach (var s in path.GetSegments())
			{
				if (IsCursorOn(s.First, s.Second, mousePos))
				{
					SnappedStartMidSegment = s;
					return true;
				}
			}

			return false;
		}
	}
}
