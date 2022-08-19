using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class ActualRailsContainer : Spatial
	{
		[Export] private PackedScene pathScene;
		[Export] private PackedScene trainScene;
		[Export] private PackedScene helperScene;
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
			// var newPath = new RailPath();

			var curve = new Curve3D();

			foreach (var v in vecs)
			{
				curve.AddPoint(v);
				AddHelper(stations[0].RailroadAlongside.Start +v);
			}

			var newPath = pathScene.Instance<RailPath>();
			newPath.Curve = RailCurve.GetFrom(curve);

			newPath.Translation = new Vector3(
				stations[0].RailroadAlongside.Start.x,
				stations[0].RailroadAlongside.Start.y,
				stations[0].RailroadAlongside.Start.z + stations[0].RailroadAlongside.GetPolygonWidth());//newPath.GetPolygonWidth());

			var pf = new RailPathFollow();
			pf.AddChild(trainScene.Instance());
			newPath.AddChild(pf);
			newPath.Owner = this;
			AddChild(newPath);
		}

		private void AddHelper(Vector3 position)
		{
			var helper = helperScene.Instance<MeshInstance>();
			helper.MaterialOverride = new SpatialMaterial() { AlbedoColor = new Color("8730abf3") };
			helper.Translation = position;
			AddChild(helper);
		}
	}
}