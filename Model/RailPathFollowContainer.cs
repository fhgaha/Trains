using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class RailPathFollowContainer : Spatial
	{
		[Export] private PackedScene trainScene;
		private List<Spatial> trains;
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.StationsAreSelected), this, nameof(onStationsAreSelected));
		}

		private void onStationsAreSelected(List<Station> stations)
		{
			//create path follow with train whose route is between these stations

			
			
		}
	}
}