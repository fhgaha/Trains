using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common.GraphRelated;

namespace Trains
{
	public class StationContainer : Spatial
	{
		private readonly List<Station> stations = new List<Station>();
		public IEnumerable<Station> Stations { get { foreach (var s in stations) { yield return s; } } }

		public void UpdateStationConnections()
		{
			stations.ForEach(s => s.ConnectedStatoins.Clear());

			var visited = new List<Station>();

			foreach (var s1 in stations)
			{
				foreach (var s2 in stations.Where(s => s != s1))
				{
					if (visited.Contains(s2)) continue;

					// var path = Dijkstra.FindPath(s1.RailroadAlongside.Start, s2.RailroadAlongside.Start);
					var path = Dijkstra.FindPath(s1.Dock, s2.Dock);

					if (path.Count > 0)
					{
						s1.ConnectedStatoins.Add(s2);
						s2.ConnectedStatoins.Add(s1);
					}
					else
					{
						GD.PrintS("Station connection failed: ", s1, "could not find path to", s2);
					}
				}

				visited.Add(s1);
			}
		}

		public void AddStation(Station station)
		{
			AddChild(station);
			stations.Add(station);
		}

		public void RemoveStation(Station station)
		{
			var childStation = GetChildren().Cast<Station>().First(s => s == station);
			childStation.QueueFree();
			stations.Remove(station);
		}

		public void Add(Station station) => stations.Add(station);

		public void Remove(Station station) => stations.Remove(station);
	}
}