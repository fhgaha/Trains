using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Common
{
	public static class Dijkstra
	{
		public class Edge
		{
			public readonly Node From;
			public readonly Node To;
			public Edge(Node first, Node second)
			{
				this.From = first;
				this.To = second;
			}
			public bool IsIncident(Node node)
			{
				return From == node || To == node;
			}
			public Node OtherNode(Node node)
			{
				if (!IsIncident(node)) throw new ArgumentException();
				if (From == node) return To;
				return From;
			}
		}

		public class Node
		{
			readonly List<Edge> edges = new List<Edge>();
			public readonly int NodeNumber;

			public Node(int number)
			{
				NodeNumber = number;
			}

			public IEnumerable<Node> IncidentNodes => edges.Select(z => z.OtherNode(this));
			public IEnumerable<Edge> IncidentEdges { get { foreach (var e in edges) yield return e; } }

			public static Edge Connect(Node node1, Node node2, Graph graph)
			{
				if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
				var edge = new Edge(node1, node2);
				node1.edges.Add(edge);
				node2.edges.Add(edge);
				return edge;
			}

			public static void Disconnect(Edge edge)
			{
				edge.From.edges.Remove(edge);
				edge.To.edges.Remove(edge);
			}
		}

		public class Graph
		{
			private Node[] nodes;

			public Graph(int nodesCount)
			{
				nodes = Enumerable.Range(0, nodesCount).Select(z => new Node(z)).ToArray();
			}

			public int Length { get { return nodes.Length; } }

			public Node this[int index] { get { return nodes[index]; } }

			public IEnumerable<Node> Nodes
			{
				get
				{
					foreach (var node in nodes) yield return node;
				}
			}

			public void Connect(int index1, int index2)
			{
				Node.Connect(nodes[index1], nodes[index2], this);
			}

			public void Delete(Edge edge)
			{
				Node.Disconnect(edge);
			}

			public IEnumerable<Edge> Edges
			{
				get
				{
					return nodes.SelectMany(z => z.IncidentEdges).Distinct();
				}
			}

			public static Graph MakeGraph(params int[] incidentNodes)
			{
				var graph = new Graph(incidentNodes.Max() + 1);
				for (int i = 0; i < incidentNodes.Length - 1; i += 2)
					graph.Connect(incidentNodes[i], incidentNodes[i + 1]);
				return graph;
			}
		}

		class DijkstraData
		{
			public Node Previous { get; set; }
			public double Price { get; set; }
		}

		//stab
		internal static List<Vector3> FindPaths(Vector3 from, Vector3 to, List<RailPath> rails)
		{
			PrintCrossings(rails);

			List<RailPath> splitted = ConvertToSplittedRails(rails);

			//build graph
			var allCrossings = splitted.SelectMany(rail => rail.Crossings);

			var nodeNumbers = new Dictionary<Vector3, int>();
			int index = 0;
			foreach (var crossing in allCrossings)
			{
				if (!nodeNumbers.ContainsKey(crossing))
				{
					nodeNumbers.Add(crossing, index);
					index++;
				}
			}

			var graph = Graph.MakeGraph(nodeNumbers.Values.ToArray());

			var weights = new Dictionary<Edge, double>();
			var edges = graph.Edges.ToList();
			for (int i = 0; i < edges.Count; i++)
			{
				weights[edges[i]] = splitted[i].Curve.GetPointCount();
			}

			//'no key in dict' error if from == (9.976347, 0, 0.9210863) and
			//nodeNumbers[from] == (9.976347, -4.74975E-08, 0.9210863)
			//why are they different?
			int startNodeNumber = nodeNumbers[from];
			int endNodeNumber = nodeNumbers[to];

			Node start = graph[startNodeNumber];
			Node end = graph[endNodeNumber];

			var paths = FindPaths(graph, weights, start, end);

			if (paths is null)
			{
				GD.Print("no paths found");
				return new List<Vector3>();
			}

			var existingPaths =	paths.Select(n => n.NodeNumber)
				.Select(i => nodeNumbers.First(kv => kv.Value == i).Key)
				.ToList();

			foreach (var item in existingPaths)
			{
				GD.Print(new object[] { item + ", " });
			}

			return existingPaths;
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

				//split
				var points = new List<Vector3>();

				for (int i = 0; i < railPath.Curve.GetBakedPoints().Length; i++)
				{
					var currentPoint = railPath.GlobalTranslation + railPath.Curve.GetBakedPoints()[i];
					points.Add(currentPoint);

					if (allCrossings.Any(c => currentPoint.IsEqualApprox(c))
						&& !currentPoint.IsEqualApprox(railPath.Start)
						&& !currentPoint.IsEqualApprox(railPath.End))
					{
						//new path
						var newPath = MakePath(points);
						newRails.Add(newPath);

						var lastPoint = points.Last();
						points.Clear();
						points.Add(lastPoint);
					}

					if (i == railPath.Curve.GetBakedPoints().Length - 1)
					{
						//reached last point
						var newPath = MakePath(points);
						newRails.Add(newPath);

						points.Clear();
					}
				}
			}

			//print
			// GD.Print("new list---------");
			// foreach (var path in newRails)
			// {
			// 	foreach (var crossing in path.Crossings)
			// 	{
			// 		GD.PrintS(path, crossing);
			// 	}
			// }
			// GD.Print("---------");

			return newRails;
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

		public static List<Node> FindPaths(Graph graph, Dictionary<Edge, double> weights, Node start, Node end)
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
}
