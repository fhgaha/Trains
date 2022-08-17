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

			var rails = Global.SplittedRailContainer.Rails;

			GD.Print("<<=RailPathFollowContainer.onStationsAreSelected");
			GD.PrintS("from: " + from, "to: " + to);
			foreach (var r in rails)
			{
				GD.PrintS(r.Start, r.End);
			}
			GD.Print("=>>");

			var vecs = Dijkstra.FindPath(from, to);
			var newPath = new RailPath();

			foreach (var v in vecs)
			{
				newPath.Curve.AddPoint(v);
			}

			newPath.Translation = stations[0].RailroadAlongside.Start;

			AddChild(newPath);
			var pf = new RailPathFollow();
			newPath.AddChild(pf);
			pf.AddChild(trainScene.Instance());
		}
	}
}