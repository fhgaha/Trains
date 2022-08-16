using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;

namespace Trains
{
	public class ActualRailsContainer : Spatial
	{
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.StationsAreSelected), this, nameof(onStationsAreSelected));
		}

		private void onStationsAreSelected(List<Station> stations)
		{
			for (int i = 0; i < stations.Count; i++)
			{
				// Dijkstra.FindPath(
				// 	stations[i].RailroadAlongside.Start,
				// 	stations[i + 1].RailroadAlongside.Start,
				// 	Global.ActualRails)
				
			}

			
			
		}
	}
}