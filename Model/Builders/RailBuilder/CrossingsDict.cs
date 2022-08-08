using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains
{
	public class CrossingsDictVecKeys
	{
		private Dictionary<Vector3, List<RailPath>> crossings;

		public List<Vector3> Keys { get => crossings.Keys.ToList(); }

		public List<RailPath> this[Vector3 key]
		{
			get => crossings[key];
		}

		public CrossingsDictVecKeys()
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

	public class CrossingsDictPathKeys
	{
		private Dictionary<RailPath, List<Vector3>> crossings;

		public List<RailPath> Keys { get => crossings.Keys.ToList(); }

		public List<Vector3> this[RailPath key]
		{
			get => crossings[key];
		}

		public CrossingsDictPathKeys()
		{
			crossings = new Dictionary<RailPath, List<Vector3>>();
		}

		public void RegisterCrossing(RailPath path, Vector3 point)
		{
			if (crossings.ContainsKey(path))
			{
				UpdateExistingKey(path, point);
			}
			else
			{
				AddNewPath(path, point);
			}
		}

		private void UpdateExistingKey(RailPath path, Vector3 point)
		{
			if (crossings[path] is null)
			{
				crossings[path] = new List<Vector3> { point };
			}
			else if (!crossings[path].Contains(point))
			{
				crossings[path].Add(point);
			}
		}

		private void AddNewPath(RailPath path, Vector3 point)
		{
			crossings.Add(path, new List<Vector3> { point });
		}
	}
}