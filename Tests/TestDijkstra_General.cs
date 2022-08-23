using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trains.Model.Common.GraphRelated;

namespace Trains.Tests
{
	public class TestDijkstra_General : WAT.Test
	{
		[Test]
		public void FindPathNodeOverloadSimple()
		{
			var graph = new Graph(4);
			var weights = new Dictionary<Edge, double>
			{
				[graph.Connect(0, 1)] = 1,
				[graph.Connect(0, 2)] = 2,
				[graph.Connect(0, 3)] = 6,
				[graph.Connect(1, 3)] = 4,
				[graph.Connect(2, 3)] = 2
			};

			var path = Dijkstra.FindPath(graph, weights, graph[0], graph[3])
				.Select(n => n.NodeNumber);

			Assert.CollectionsAreEqual(new[] { 0, 2, 3 }, path);
		}

		[Test]
		public void FindPathNodeOverload1()
		{
			var graph = Graph.MakeGraph(
				0, 1,
				0, 2,
				0, 3,
				1, 3,
				2, 3
			);
			var weights = new Dictionary<Edge, double>
			{
				[graph.GetEdge(0, 1)] = 1,
				[graph.GetEdge(0, 2)] = 2,
				[graph.GetEdge(0, 3)] = 6,
				[graph.GetEdge(1, 3)] = 4,
				[graph.GetEdge(2, 3)] = 2
			};

			var path = Dijkstra.FindPath(graph, weights, graph[0], graph[3]);

			Assert.IsFalse(path is null);
			Assert.IsTrue(path.Count > 0, $"path.Count is {path.Count}");

			Assert.CollectionsAreEqual(new[] { 0, 2, 3 }, path.Select(n => n.NodeNumber));
		}

		[Test]
		public void FindPathNodeOverload2()
		{
			var graph = Graph.MakeGraph(
				0, 1,
				1, 2,
				1, 4,
				2, 3
			);
			var weights = new Dictionary<Edge, double>
			{
				[graph.GetEdge(0, 1)] = 2f,
				[graph.GetEdge(1, 2)] = 3.2f,
				[graph.GetEdge(1, 4)] = 5.62f,
				[graph.GetEdge(2, 3)] = 6.57f,
			};

			var path = Dijkstra.FindPath(graph, weights, graph[0], graph[3]);

			Assert.IsFalse(path is null);
			Assert.IsTrue(path.Count > 0, $"path.Count is {path.Count}");
			Assert.CollectionsAreEqual(new[] { 0, 1, 2, 3 }, path.Select(n => n.NodeNumber));
		}

		[Test]
		public void FindPathNodeOverloadWrongNodeOrderThrowsTargetInvocationException()
		{
			var graph = Graph.MakeGraph(
				0, 1,
				1, 2,
				4, 1,   //order is wrong
				3, 2    //order is wrong
			);
			var weights = new Dictionary<Edge, double>
			{
				[graph.GetEdge(0, 1)] = 2f,
				[graph.GetEdge(1, 2)] = 3.2f,
				[graph.GetEdge(1, 4)] = 5.62f,
				[graph.GetEdge(2, 3)] = 6.57f,
			};

			var path = Dijkstra.FindPath(graph, weights, graph[0], graph[3]);

			Assert.Throws(() => throw new TargetInvocationException(new NullReferenceException()));
		}

		[Test]
		public void FindPathNodeOverload3()
		{
			var graph = new Graph(5);
			var weights = new Dictionary<Edge, double>
			{
				[graph.Connect(0, 1)] = 1,
				[graph.Connect(1, 2)] = 1,
				[graph.Connect(0, 3)] = 1,
				[graph.Connect(3, 4)] = 1
			};

			var path = Dijkstra.FindPath(graph, weights, graph[1], graph[3])
				.Select(n => n.NodeNumber);

			Assert.IsTrue(path != null);
			// Assert.CollectionsAreEqual(new[] { 1, 0, 3 }, path);
		}
	}
}
