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

		//make this in station builder class?
		//keep links to every container in global?
		public void UpdateStationConnections()
		{
			Stations.ForEach(s => s.ConnectedStatoins.Clear());

			var visited = new List<Station>();

			foreach (var s1 in Stations.Where(s => !visited.Contains(s)))
			{
				visited.Add(s1);

				foreach (var s2 in Stations.Where(s => !visited.Contains(s)))
				{
					var path = Dijkstra.FindPath(s1.RailroadAlongside.Start, s2.RailroadAlongside.Start);

					if (path.Count > 0)
					{
						s1.ConnectedStatoins.Add(s2);
						s2.ConnectedStatoins.Add(s1);
					}
				}
			}
		}

		public void Add(Station station) => Stations.Add(station);

		public void Remove(Station station) => Stations.Remove(station);
	}
}