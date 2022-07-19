using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class Snapper : Node
	{
		public RailPath SnappedPath { get; set; }
		public Vector3 SnappedDir { get; set; } = Vector3.Zero;
		public CurveSegment SnappedSegment { get; set; }

		private const float snapDistance = 1f;
		private Vector3 mousePos;
		
		public Snapper() { }

		public bool IsBlueprintSnappedOnSegment() => SnappedSegment != null;

		public void Reset() => SetVars(null, Vector3.Zero, null);

		public Vector3 GetSegmentToCursorDirUsingSnappedSegment(Vector3 mousePos)
		{
			if (SnappedSegment is null) throw new NullReferenceException("Snapper.SnappedSegment is null");

			var startToEnd = (SnappedSegment.Second - SnappedSegment.First).Normalized();
			var endToStart = (SnappedSegment.First - SnappedSegment.Second).Normalized();
			var startToCursor = (mousePos - SnappedSegment.First).Normalized();
			var segmentAndCursorAreOneDirectional = startToEnd.Dot(startToCursor) > 0;

			if (segmentAndCursorAreOneDirectional)
				return startToEnd;
			else
				return endToStart;
		}

		public void SnapBpIfNecessary(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
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
				else if (TrySnapOnAnySegment(path))
				{
					if (SnappedSegment is null) continue;

					blueprint.Translation = SnappedSegment.First;
					SetVars(path, Vector3.Zero, SnappedSegment);
					return;
				}
			}

			SnappedSegment = null;
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

		private bool TrySnapOnAnySegment(RailPath path)
		{
			foreach (var s in path.GetSegments())
			{
				if (IsCursorOn(s.First, s.Second))
				{
					SnappedSegment = s;
					return true;
				}
			}

			return false;
		}

		public void AlignBpForStart(RailPath blueprint, RailPath path)
		{
			//polygon translation starts from (0, 0, 0). we need to find additional relative to parent path translation 
			//for polygon. required translation depends of path rotation so trigonometry formulas come useful.
			var angleFromRightClockwise = -Vector3.Right.SignedAngleTo(path.DirFromStart, Vector3.Up);
			var x = path.GetPolygonWidth() * Mathf.Sin(angleFromRightClockwise);
			var z = -path.GetPolygonWidth() * Mathf.Cos(angleFromRightClockwise);
			blueprint.GetNode<CSGPolygon>("CSGPolygon").Translation = new Vector3(x, 0, z);
		}

		public void AlignBpForEnd(RailPath blueprint)
		{
			//reset polygon position if previously it was translated
			blueprint.GetNode<CSGPolygon>("CSGPolygon").Translation = new Vector3(0, 0, 0);
		}

		private void SetVars(RailPath path, Vector3 direction, CurveSegment segment)
		{
			SnappedPath = path;
			SnappedDir = direction;
			SnappedSegment = segment;
		}


	}
}
