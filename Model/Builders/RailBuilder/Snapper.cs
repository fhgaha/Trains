using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class Snapper : Node
	{
		public RailPath SnappedPath { get; set; }
		public Vector3 SnappedDir { get; set; } = Vector3.Zero;

		private const float snapDistance = 1f;
		private Vector3 mousePos;
		private CurveSegment foundSegment;
		public Snapper() { }

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

					//align bp if starts from currentPath start
					blueprint.GetNode<CSGPolygon>("CSGPolygon").Translation = new Vector3(0, 0, path.GetPolygonWidth());

					UpdateVars(path, path.DirFromStart);
					return;
				}
				else if (IsCursorOn(end, start))
				{
					blueprint.Translation = end;
					RotateBlueprint(blueprint, path.DirFromEnd);
					UpdateVars(path, path.DirFromEnd);
					return;
				}
				else if (IsCursorOnAnySegment(path))
				{
					blueprint.Translation = foundSegment.First;
					//blueprint.AcceptSegmentPosition();
					var directionOutOfSegmentPoint = Vector3.Zero;
					UpdateVars(path, directionOutOfSegmentPoint);
				}
				else
				{
					UnrotateBlueprint(blueprint);
				}
			}

			UpdateVars(null, Vector3.Zero);
		}

		private bool IsCursorOn(Vector3 start, Vector3 end)
		{
			return start.DistanceTo(mousePos) < snapDistance && start.DistanceTo(mousePos) < end.DistanceTo(mousePos);
		}

		private void RotateBlueprint(RailPath blueprint, Vector3 direction)
		{
			blueprint.SetSimpleCurve(Vector3.Zero, direction);
		}

		private bool IsCursorOnAnySegment(RailPath path)
		{
			foreach (var s in path.GetSegments())
			{
				if (IsCursorOn(s.First, s.Second))
				{
					foundSegment = s;
					return true;
				}
			}

			foundSegment = null;
			return false;
		}

		private void UnrotateBlueprint(RailPath blueprint)
		{
			blueprint.SetOriginalBpCurve();
		}

		private void UpdateVars(RailPath path, Vector3 direction)
		{
			SnappedPath = path;
			SnappedDir = direction;
		}
	}
}
