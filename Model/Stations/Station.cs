using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;

namespace Trains
{
	public class Station : Spatial
	{
		public int Id { get; set; }
		public RailPath RailroadAlongside { get; private set; }
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
			var paths = Dijkstra.FindPaths(from, to, Global.Rails);
			return paths.Any();
		}
	}

	public static class Dijkstra
	{
		public static IEnumerable<Vector3> FindPaths(Vector3 start, Vector3 target, List<RailPath> RailPaths)
		{
			var paths = RailPaths.SelectMany(p
				//=> new List<Vector3> { p.Start, p.End }.Union(p.Crossings.Keys));
				=> p.Crossings.Keys);



			foreach (var p in RailPaths)
			{
				foreach (var path in p.Crossings.Keys)
				{
					foreach (var vec in p.Crossings[path])
					{
						GD.PrintS(path, vec);
					}
				}
			}
			GD.Print();

			// foreach (var item in vertices)
			// {
			// 	GD.Print(item);
			// }



			return new List<Vector3>();

		}
	}
}
