using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Common
{
	public static partial class Dijkstra
	{
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
	}
}
