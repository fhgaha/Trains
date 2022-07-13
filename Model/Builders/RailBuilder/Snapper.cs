using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Model.Builders
{
	public class Snapper : Node
	{
		public RailPath SnappedPath { get; set; }
		public Vector3 SnappedDir {get; set;} = Vector3.Zero;

		private const float snapDistance = 1f;
		private Vector3 mousePos;

		public Snapper()
		{

		}

		public void SnapIfNecessary(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
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
					UpdateVars(path, path.DirFromStart);
					return;
				}

				if (IsCursorOn(end, start))
				{
					blueprint.Translation = end;
					UpdateVars(path, path.DirFromEnd);
					return;
				}
			}

			UpdateVars(null, Vector3.Zero);
		}

		private bool IsCursorOn(Vector3 start, Vector3 end)
		{
			return start.DistanceTo(mousePos) < snapDistance && start.DistanceTo(mousePos) < end.DistanceTo(mousePos);
		}

		private void UpdateVars(RailPath path, Vector3 newDir)
		{
			SnappedPath = path;
			SnappedDir = newDir;
		}
	}
}
