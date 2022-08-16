using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common.GraphRelated;

namespace Trains
{
	public class StationContainer : Spatial
	{
		private List<Station> stations = new List<Station>();


		//make this in station builder class?
		//keep links to every container in global?
		public void UpdateStationConnections()
		{
			stations.ForEach(s => s.ConnectedStatoins.Clear());

			var visited = new List<Station>();

			foreach (var s1 in stations.Where(s => !visited.Contains(s)))
			{
				visited.Add(s1);

				foreach (var s2 in stations.Where(s => !visited.Contains(s)))
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
	}
}