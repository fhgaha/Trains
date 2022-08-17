using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common.GraphRelated;

namespace Trains
{
	public class StationContainer : Spatial
	{
		public List<Station> Stations { get; private set; } = new List<Station>();

		public void UpdateStationConnections()
		{
			Stations.ForEach(s => s.ConnectedStatoins.Clear());

			var visited = new List<Station>();

			foreach (var s1 in Stations)
			{
				foreach (var s2 in Stations.Where(s => s != s1))
				{
					if (visited.Contains(s2)) continue;

					var path = Dijkstra.FindPath(s1.RailroadAlongside.Start, s2.RailroadAlongside.Start);

					if (path.Count > 0)
					{
						s1.ConnectedStatoins.Add(s2);
						s2.ConnectedStatoins.Add(s1);
					}
				}

				visited.Add(s1);
			}
		}

		public void AddStation(Station station)
		{
			AddChild(station);
			Stations.Add(station);
		}

		public void RemoveStation(Station station)
		{
			var childStation = GetChildren().Cast<Station>().First(s => s == station);
			childStation.QueueFree();
			Stations.Remove(station);
		}

		public void Add(Station station) => Stations.Add(station);

		public void Remove(Station station) => Stations.Remove(station);
	}
}