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
			var paths = Dijkstra.FindPaths(RailroadAlongside.Start, station.RailroadAlongside.Start, Global.Rails);
			return paths.Any();
		}
	}

	public static class Dijkstra
	{
		public static IEnumerable<Vector3> FindPaths(Vector3 start, Vector3 target, List<RailPath> RailPaths)
		{
			var vertices = RailPaths.SelectMany(p 
				//=> new List<Vector3> { p.Start, p.End }.Union(p.Crossings.Keys));
				=> p.Crossings.Keys);

			GD.Print(RailPaths.Count);
			
			foreach (var path in RailPaths)
			{
				GD.Print(path.Crossings.Keys);

				foreach (var k in path.Crossings.Keys)
				{
					GD.Print(k);
				}
			}

			// GD.Print(vertices);
			//GD.Print(vertices.Count());
			foreach (var item in vertices)
			{
				GD.Print(item);
			}

			var paths = new HashSet<Vector3>();




			return paths;

		}
	}
}
