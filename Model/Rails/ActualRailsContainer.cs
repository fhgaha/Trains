using Godot;
using System.Collections.Generic;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;

namespace Trains
{
	public class ActualRailsContainer : Spatial
	{
		[Export] private PackedScene railScene;
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
			BuildRailConnecting(stations);
		}

		private void BuildRailConnecting(List<Station> stations)
		{
			var from = stations[0].Dock;
			var to = stations[1].Dock;
			var vecs = Dijkstra.FindPath(from, to);
			var newPath = RailPath.BuildNoMeshRail(railScene, vecs, Vector3.Zero);// stations[0].RailroadAlongside.Translation);
			AddChild(newPath);

			var pf = new RailPathFollow();
			pf.AddChild(trainScene.Instance());
			newPath.AddChild(pf);
		}

		private static void PrintSplitedRails(Vector3 from, Vector3 to, IEnumerable<RailPath> rails)
		{
			GD.Print("<<=ActualRailsContainer.BuildRailConnecting");
			GD.PrintS("from: " + from, "to: " + to);
			foreach (var r in rails)
			{
				GD.PrintS(r.Start, r.End);
			}
			GD.Print("=>>");
		}
	}
}
