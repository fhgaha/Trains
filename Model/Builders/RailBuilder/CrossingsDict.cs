using Godot;
using System;
using System.Collections.Generic;

namespace Trains
{
	public class CrossingsDict
	{
		private Dictionary<Vector3, List<RailPath>> crossings;

		public CrossingsDict()
		{
			crossings = new Dictionary<Vector3, List<RailPath>>();
		}

		public void RegisterCrossing(Vector3 snappedPoint, RailPath pathThisPathIsConnectedWith)
		{
			if (crossings.ContainsKey(snappedPoint))
			{
				UpdateExistingKey(snappedPoint, pathThisPathIsConnectedWith);
			}
			else
			{
				crossings.Add(snappedPoint, new List<RailPath> { pathThisPathIsConnectedWith });
			}
		}

		private void UpdateExistingKey(Vector3 snappedPoint, RailPath pathThisPathIsConnectedWith)
		{
			if (crossings[snappedPoint] is null)
			{
				crossings[snappedPoint] = new List<RailPath>();
				crossings[snappedPoint].Add(pathThisPathIsConnectedWith);
			}
			else if (!crossings[snappedPoint].Contains(pathThisPathIsConnectedWith))
			{
				crossings[snappedPoint].Add(pathThisPathIsConnectedWith);
			}
		}
	}
}