using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Common.GraphRelated
{
	public static class Dijkstra
	{
		class DijkstraData
		{
			public Node Previous { get; set; }
			public double Price { get; set; }
		}

		internal static List<Vector3> FindPath(Vector3 from, Vector3 to, List<RailPath> rails)
		{
			PrintCrossings(rails);

			var splitted = ConvertToSplittedRails(rails);
			var allCrossings = splitted.SelectMany(p => new[] { p.Start, p.End }).ToList();
			var nodeNumbers = BuildNodeNumbers(allCrossings);
			var allCrossingsAsIntegers = ParseVectorsToNodeNumbers(allCrossings, nodeNumbers);

			var graph = Graph.MakeGraph(allCrossingsAsIntegers);

			var weights = new Dictionary<Edge, double>();
			var edges = graph.Edges.ToList();
			for (int i = 0; i < edges.Count; i++)
			{
				weights[edges[i]] = splitted[i].Curve.GetBakedLength();
			}

			int startNodeNumber = nodeNumbers[from];
			int endNodeNumber = nodeNumbers[to];

			Node start = graph[startNodeNumber];
			Node end = graph[endNodeNumber];

			var paths = FindPath(graph, weights, start, end);

			if (paths is null)
			{
				GD.Print("path was not found");
				return new List<Vector3>();
			}

			var existingPaths = paths.Select(n => n.NodeNumber)
				.Select(i => nodeNumbers.First(kv => kv.Value == i).Key)
				.ToList();

			GD.Print("---found path crossings: ---");
			foreach (var item in existingPaths)
			{
				GD.Print(new[] { item + ", " });
			}
			GD.Print("------");

			return existingPaths;
		}

		private static int[] ParseVectorsToNodeNumbers(
			List<Vector3> allCrossings, Dictionary<Vector3, int> nodeNumbers)
		{
			var allCrossingsAsIntegers = allCrossings.Select(cr => nodeNumbers[cr]).ToArray();
			//nodenumbers should be from lesser to greater
			for (int i = 0; i < allCrossingsAsIntegers.Length; i += 2)
			{
				if (allCrossingsAsIntegers[i] > allCrossingsAsIntegers[i + 1])
				{
					//swap
					var temp = allCrossingsAsIntegers[i];
					allCrossingsAsIntegers[i] = allCrossingsAsIntegers[i + 1];
					allCrossingsAsIntegers[i + 1] = temp;
				}
			}

			return allCrossingsAsIntegers;
		}

		private static Dictionary<Vector3, int> BuildNodeNumbers(List<Vector3> allCrossings)
		{
			var nodeNumbers = new Dictionary<Vector3, int>(new MyVector3EqualityComparer());
			int index = 0;
			foreach (var crossing in allCrossings)
			{
				if (!nodeNumbers.ContainsKey(crossing))
				{
					nodeNumbers.Add(crossing, index);
					index++;
				}
			}

			return nodeNumbers;
		}

		private static List<RailPath> ConvertToSplittedRails(List<RailPath> rails)
		{
			var allCrossings = rails.SelectMany(r => r.Crossings).ToList();
			var dict = new Dictionary<RailPath, List<List<Vector3>>>();
			var newRails = new List<RailPath>();

			foreach (var railPath in rails)
			{
				if (railPath.Crossings.Count <= 2)
				{
					newRails.Add(railPath);
					continue;
				}

				SplitRailPath(railPath, allCrossings, newRails);
			}

			return newRails;
		}

		private static void SplitRailPath(RailPath railPath, List<Vector3> allCrossings, List<RailPath> newRails)
		{
			var points = new List<Vector3>();

			for (int i = 0; i < railPath.Curve.GetBakedPoints().Length; i++)
			{
				var currentPoint = railPath.GlobalTranslation + railPath.Curve.GetBakedPoints()[i];
				points.Add(currentPoint);

				if (RailPathHasCrossing(railPath, allCrossings, currentPoint))
				{
					MakeNewPathAndUpdateNewRails(newRails, points);

					var lastPoint = points.Last();
					points.Clear();
					points.Add(lastPoint);
				}

				bool reachedLastPoint = i == railPath.Curve.GetBakedPoints().Length - 1;

				if (reachedLastPoint)
				{
					MakeNewPathAndUpdateNewRails(newRails, points);

					points.Clear();
				}
			}
		}

		private static bool RailPathHasCrossing(RailPath railPath, List<Vector3> allCrossings, Vector3 currentPoint)
		{
			return allCrossings.Any(c => currentPoint.IsEqualApprox(c))
								&& !currentPoint.IsEqualApprox(railPath.Start)
								&& !currentPoint.IsEqualApprox(railPath.End);
		}

		private static void MakeNewPathAndUpdateNewRails(List<RailPath> newRails, List<Vector3> points)
		{
			var newPath = MakePath(points);
			newRails.Add(newPath);
		}

		private static RailPath MakePath(List<Vector3> points)
		{
			var newPath = new RailPath();

			for (int i = 0; i < points.Count; i++)
			{
				//addpoint is wrong points are baked?
				newPath.Curve.AddPoint(points[i]);
			}

			newPath.Curve = RailCurve.GetFrom(newPath.Curve);
			newPath.EnlistCrossing(newPath.Start);
			newPath.EnlistCrossing(newPath.End);
			return newPath;
		}

		public static List<Node> FindPath(Graph graph, Dictionary<Edge, double> weights, Node start, Node end)
		{
			var notVisited = graph.Nodes.ToList();
			var track = new Dictionary<Node, DijkstraData>();
			track[start] = new DijkstraData { Price = 0, Previous = null };

			while (true)
			{
				Node toOpen = null;
				var bestPrice = double.PositiveInfinity;
				foreach (var e in notVisited)
				{
					if (track.ContainsKey(e) && track[e].Price < bestPrice)
					{
						bestPrice = track[e].Price;
						toOpen = e;
					}
				}

				if (toOpen == null) return null;
				if (toOpen == end) break;

				foreach (var e in toOpen.IncidentEdges.Where(z => z.From == toOpen))
				{
					var currentPrice = track[toOpen].Price + weights[e];
					var nextNode = e.OtherNode(toOpen);
					if (!track.ContainsKey(nextNode) || track[nextNode].Price > currentPrice)
					{
						track[nextNode] = new DijkstraData { Previous = toOpen, Price = currentPrice };
					}
				}

				notVisited.Remove(toOpen);
			}

			var result = new List<Node>();
			while (end != null)
			{
				result.Add(end);
				end = track[end].Previous;
			}
			result.Reverse();
			return result;
		}

		private static void PrintCrossings(List<RailPath> RailPaths)
		{
			foreach (var p in RailPaths)
			{
				foreach (var vec in p.Crossings)
				{
					GD.PrintS(p, vec);
				}
			}
			GD.Print();
		}
	}

	class MyVector3EqualityComparer : IEqualityComparer<Vector3>
	{
		public bool Equals(Vector3 v1, Vector3 v2)
		{
			if (v2 == null && v1 == null)
				return true;
			else if (v1 == null || v2 == null)
				return false;
			// else if (v1.IsEqualApprox(v2))
			//ignore y
			else if (v1.x == v2.x && v1.z == v2.z)
				return true;
			else
				return false;
		}

		public int GetHashCode(Vector3 v)
		{
			// return v.x.GetHashCode() ^ v.y.GetHashCode() << 2 ^ v.z.GetHashCode() >> 2;
			return 0;
		}
	}
}
