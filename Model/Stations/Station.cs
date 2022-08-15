using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;

namespace Trains
{
	public class Station : Spatial
	{
		public int Id { get; set; }
		public RailPath RailroadAlongside { get; set; }
		private Cell cell;

		public void Init(Station blueprint)
		{
			RemoveChild(GetNode("Base"));
			GlobalTransform = blueprint.GlobalTransform;
			GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = false;

			var railPath = GetNode<RailPath>("RailPath");
			RailroadAlongside = railPath;
			RemoveChild(railPath);
		}

		public void LoadTrain() { }
		public void UnloadTrain() { }
		public void DepartTrain() { }
		public void onTrainArrived() { }

		public bool IsConnectedWith(Station station)
		{
			var from = RailroadAlongside.Start;
			var to = station.RailroadAlongside.Start;
			var path = Dijkstra.FindPath(from, to, Global.ActualRails);
			return path.Count > 0;
		}
	}
}
