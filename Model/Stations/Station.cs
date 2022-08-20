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
		[Export] public Vector3 Dock { get; set; }
		[Export] public RailPath RailroadAlongside { get; set; }
		[Export] public List<Station> ConnectedStatoins { get; set; }
		private Cell cell;

		public void Init(Station blueprint)
		{
			ConnectedStatoins = new List<Station>();

			RemoveChild(GetNode("Base"));
			GlobalTransform = blueprint.GlobalTransform;
			GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = false;

			var railPath = GetNode<RailPath>("RailPath");
			RailroadAlongside = railPath;
			RemoveChild(railPath);

			var railPoints = RailroadAlongside.Curve.GetBakedPoints();
			Dock = RailroadAlongside.Start + railPoints[railPoints.Length / 2];
		}

		public void LoadTrain() { }
		public void UnloadTrain() { }
		public void DepartTrain() { }
		public void onTrainArrived() { }

		public bool IsConnectedWith(Station station) => ConnectedStatoins.Contains(station);
	}
}
