using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class RailPathFollowContainer : Spatial
	{
		[Export] private PackedScene trainScene;
		private List<RailPathFollow> trains;
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.StationsAreSelected), this, nameof(onStationsAreSelected));
		}

		private void onStationsAreSelected(List<Station> stations)
		{
			//create path follow with train whose route is between these stations

			var from = stations[0].RailroadAlongside.Start;
			var to = stations[1].RailroadAlongside.Start;

			var rails = Global.ActualRailContainer.Rails;
			var rail = rails.First(r => r.Curve.GetBakedPoints().Contains(from)
									 && r.Curve.GetBakedPoints().Contains(to));
			
			var pf = new RailPathFollow();
			rail.AddChild(pf);
			pf.AddChild(trainScene.Instance());

		}
	}
}