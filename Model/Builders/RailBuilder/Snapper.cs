using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Builders;

namespace Trains
{
	public class Snapper : Node
	{
		private const float snapDistance = 1f;
		public RailPath SnappedPath = null;
		public Vector3 SnappedDir = Vector3.Zero;

		public Snapper()
		{
			
		}

		public void SnapIfNecessary(Vector3 mousePos, List<RailPath> pathList, RailPath blueprint)
		{
			foreach (var path in pathList)
			{
				var start = path.Start;
				var end = path.End;
				bool cursorIsOnStart = start.DistanceTo(mousePos) < snapDistance && start.DistanceTo(mousePos) < end.DistanceTo(mousePos);
				bool cursorIsOnEnd = end.DistanceTo(mousePos) < snapDistance && end.DistanceTo(mousePos) < start.DistanceTo(mousePos);
				
				if (cursorIsOnStart)
				{
					SnapAndUpdateVars(start);
					SnappedDir = path.DirFromStart;
					return;
				}

				if (cursorIsOnEnd)
				{
					SnapAndUpdateVars(end);
					SnappedDir = path.DirFromEnd;
					return;
				}

				void SnapAndUpdateVars(Vector3 point)
				{
					blueprint.Translation = point;
					SnappedPath = path;
				}
			}

			SnappedPath = null;
			SnappedDir = Vector3.Zero;
		}
	}
}