using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Builders;

namespace Trains
{
	public class RailContainer : Spatial
	{
		public List<RailPath> PathList = new List<RailPath>();

		internal void AddRailPath(RailPath path)
		{
			PathList.Add(path);
			AddChild(path);
			path.GetNode<CSGPolygon>("CSGPolygon").UseCollision = true;
		}

		internal void AddToPathList(RailPath path)
		{
			PathList.Add(path);
		}

		internal void RemovePath(RailPath path)
		{
			PathList.Remove(path);
		}


		// public static IEnumerable<Node> BreadthSearch_Correct(Node startNode)
		// {
		// 	var visited = new HashSet<Node>();
		// 	var queue = new Queue<Node>();
		// 	visited.Add(startNode);
		// 	queue.Enqueue(startNode);
		// 	while (queue.Count != 0)
		// 	{
		// 		var node = queue.Dequeue();
		// 		yield return node;
		// 		foreach (var nextNode in node.IncidentNodes.Where(n => !visited.Contains(n)))
		// 		{
		// 			visited.Add(nextNode);
		// 			queue.Enqueue(nextNode);
		// 		}
		// 	}
		// }


		// public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
		// {
		// 	var paths = new HashSet<SinglyLinkedList<Point>>();
		// 	var visited = new HashSet<Point>();
		// 	var queue = new Queue<SinglyLinkedList<Point>>();
		// 	queue.Enqueue(new SinglyLinkedList<Point>(start));
		// 	while (queue.Count != 0)
		// 	{
		// 		var list = queue.Dequeue();
		// 		var point = list.Value;

		// 		for (int dy = -1; dy <= 1; dy++)
		// 		{
		// 			for (int dx = -1; dx <= 1; dx++)
		// 			{
		// 				if (dx != 0 && dy != 0) continue;
		// 				if (dx == 0 && dy == 0) continue;

		// 				var newPoint = new Point(point.X + dx, point.Y + dy);

		// 				if (!map.InBounds(newPoint)) continue;
		// 				if (map.Dungeon[newPoint.X, newPoint.Y] == MapCell.Wall) continue;
		// 				if (visited.Contains(newPoint)) continue;
		// 				queue.Enqueue(new SinglyLinkedList<Point>(newPoint, list));
		// 				visited.Add(newPoint);
		// 				if (chests.Contains(newPoint))
		// 					paths.Add(new SinglyLinkedList<Point>(newPoint, list));
		// 			}
		// 		}
		// 	}

		// 	foreach (var e in paths)
		// 		yield return e;
		// }

		public static IEnumerable<Vector3> FindPaths(Vector3 start, Vector3 target, List<RailPath> RailPaths)
		{
			var vertices = RailPaths.SelectMany(p => new List<Vector3> { p.Start, p.End }.Union(p.Crossings.Keys));
			GD.Print(vertices);
			var paths = new HashSet<Vector3>();

			return paths;

		}
	}
}





