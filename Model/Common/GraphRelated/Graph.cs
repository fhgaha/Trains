using System;
using System.Collections.Generic;
using System.Linq;

namespace Trains.Model.Common.GraphRelated
{
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

		public Edge Connect(int index1, int index2)
		{
			return Node.Connect(nodes[index1], nodes[index2], this);
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

		public Edge GetEdge(int index1, int index2)
		{
			var edge = Edges.FirstOrDefault(e =>
				(e.From.NodeNumber == index1 && e.To.NodeNumber == index2)
			 || (e.To.NodeNumber == index1 && e.From.NodeNumber == index2));

			if (edge is null) throw new ArgumentException($"There is no such edge [{index1} - {index2}]");
			return edge;
		}
	}
}
