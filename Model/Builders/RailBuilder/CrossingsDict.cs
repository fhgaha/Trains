using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains
{
	public class CrossingsDict
	{
		private Dictionary<Vector3, List<RailPath>> crossings;

		public List<Vector3> Keys { get => crossings.Keys.ToList(); }

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
				AddNewPath(snappedPoint, pathThisPathIsConnectedWith);
			}
		}

		private void UpdateExistingKey(Vector3 snappedPoint, RailPath pathThisPathIsConnectedWith)
		{
			if (crossings[snappedPoint] is null)
			{
				crossings[snappedPoint] = new List<RailPath> { pathThisPathIsConnectedWith };
			}
			else if (!crossings[snappedPoint].Contains(pathThisPathIsConnectedWith))
			{
				crossings[snappedPoint].Add(pathThisPathIsConnectedWith);
			}
		}

		private void AddNewPath(Vector3 snappedPoint, RailPath pathThisPathIsConnectedWith)
		{
			crossings.Add(snappedPoint, new List<RailPath> { pathThisPathIsConnectedWith });
		}
	}
}