using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Common.GraphRelated
{
	public static class Dijkstra
	{
		private readonly static bool shouldPrint = false;

		internal static List<Vector3> FindPath(Vector3 from, Vector3 to)
		{
			var splitted = Global.SplittedRailContainer.Rails.ToList();

			if (splitted is null || splitted.Count == 0) return new List<Vector3>();

			var allCrossings = splitted.SelectMany(p => new[] { p.Start, p.End }).ToList();
			var nodeNumbers = BuildNodeNumbers(allCrossings);
			var edgesAsIntegers = ParseVectorsToNodeNumbers(allCrossings, nodeNumbers);
			var graph = Graph.MakeGraph(edgesAsIntegers);
			var weights = GetWeightsOfPaths(splitted, graph, nodeNumbers);

			Vector3 fromKey = nodeNumbers.Keys.ToList().First(k => k.IsEqualApprox(from, 0.11f));
			Vector3 toKey = nodeNumbers.Keys.ToList().First(k => k.IsEqualApprox(to, 0.11f));

			int startNodeNumber = nodeNumbers[fromKey];
			int endNodeNumber = nodeNumbers[toKey];

			Node start = graph[startNodeNumber];
			Node end = graph[endNodeNumber];

			var turningNodes = FindPath(graph, weights, start, end);

			if (turningNodes is null)
			{
				if (shouldPrint) GD.Print("Dijkstra path was not found");
				return new List<Vector3>();
			}

			var turningPoints = turningNodes.Select(n => n.NodeNumber)
				.Select(i => nodeNumbers.First(kv => kv.Value == i).Key)
				.ToList();

			if (shouldPrint)
			{
				GD.Print("---Dijkstra found path crossings: ---");
				foreach (var item in turningPoints)
				{
					GD.Print(new[] { item + ", " });
				}
				GD.Print("------");
			}

			var result = BuildFinalPathAsPoints(splitted, turningPoints);
			return result;
		}

		private static List<Vector3> BuildFinalPathAsPoints(List<RailPath> splitted, List<Vector3> turningPoints)
		{
			var result = new List<Vector3>();

			for (int i = 0; i < turningPoints.Count - 1; i++)
			{
				var firstTurn = turningPoints[i];
				var secondTurn = turningPoints[i + 1];

				var path = splitted.FindAll(p => ShouldGoForward(p) || ShouldGoBackwards(p))
					.OrderBy(p => p.Curve.GetBakedLength())
					.First();

				// var path = splitted.First(p => ShouldGoForward(p) || ShouldGoBackwards(p));
				var currentPoints = path.Curve.GetBakedPoints();
				var origin = result.Count == 0 ? Vector3.Zero : secondTurn - turningPoints[0];

				if (ShouldGoForward(path))
				{
					result.AddRange(currentPoints);
				}
				else if (ShouldGoBackwards(path))
				{
					currentPoints = currentPoints.Reverse().ToArray();
					result.AddRange(currentPoints);
				}

				bool ShouldGoForward(RailPath p) => p.Start.IsEqualApprox(firstTurn) && p.End.IsEqualApprox(secondTurn);
				bool ShouldGoBackwards(RailPath p) => p.Start.IsEqualApprox(secondTurn) && p.End.IsEqualApprox(firstTurn);
			}

			return result;
		}

		private static Dictionary<Edge, double> GetWeightsOfPaths(
			List<RailPath> splitted,
			Graph graph,
			Dictionary<Vector3, int> nodeNumbers)
		{
			var edges = graph.Edges.ToList();
			var splittedNoLongerDuplicates = new List<RailPath>();
			var weights = new Dictionary<Edge, double>();

			for (int i = 0; i < edges.Count; i++)
			{
				RailPath rail = FindRailByEdge(splitted, nodeNumbers, edges[i]);

				if (!splittedNoLongerDuplicates.Contains(rail))
				{
					splittedNoLongerDuplicates.Add(rail);
					weights[edges[i]] = rail.Curve.GetBakedLength();
				}
				else
				{
					var index = splittedNoLongerDuplicates.FindIndex(s => s == rail);

					if (rail.Curve.GetBakedLength() < splittedNoLongerDuplicates[index].Curve.GetBakedLength())
					{
						splittedNoLongerDuplicates[index] = rail;
						weights[edges[i]] = rail.Curve.GetBakedLength();
					}
				}
			}

			return weights;
		}

		private static RailPath FindRailByEdge(
			List<RailPath> splitted, Dictionary<Vector3, int> nodeNumbers, Edge edge)
		{
			Vector3 from = nodeNumbers.Keys.First(k => nodeNumbers[k] == edge.From.NodeNumber);
			Vector3 to = nodeNumbers.Keys.First(k => nodeNumbers[k] == edge.To.NodeNumber);

			var rail = splitted.First(
				s => (s.Start.IsEqualApprox(from) && s.End.IsEqualApprox(to))
				|| (s.Start.IsEqualApprox(to) && s.End.IsEqualApprox(from)));
			return rail;
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
			var nodeNumbers = new Dictionary<Vector3, int>(new Vector3IgnoreYComparer());
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

		class DijkstraData
		{
			public Node Previous { get; set; }
			public double Price { get; set; }
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

				foreach (var e in toOpen.IncidentEdges)//.Where(z => z.From == toOpen || z.To == toOpen))
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
	}
}
