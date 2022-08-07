using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class TrainContainer : Spatial
	{
		[Export] private PackedScene trainScene;
		private List<Spatial> trains;
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));
			events.Connect(nameof(Events.StationsAreSelected), this, nameof(onStationsAreSelected));
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			//temp
			//var train = trainScene.Instance<Train>();
		}

		private void onStationsAreSelected(List<Station> stations)
		{
			//create trian that route is between these stations

			var train = trainScene.Instance<Train>();
			
		}
	}
}