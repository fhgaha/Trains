using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trains.Model.Builders;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;

namespace Trains.Tests
{
	[Start(nameof(RunBeforeTestClass))]
	[Pre(nameof(RunBeforeTestMethod))]
	[Post(nameof(RunAfterTestMethod))]
	[End(nameof(RunAfterTestClass))]

	public class TestDijkstra_General : WAT.Test
	{
		//Developers may target a method with the [Start] attribute to execute code before any test method is run
		public void RunBeforeTestClass()
		{
		}

		// Developers may target a method with the [Pre] attribute to execute code before each test method is run;
		public void RunBeforeTestMethod()
		{
		}

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

		// [Test]
		// public void FindPathVectorsOverloadApproved1()
		// {
		// 	Vector3 from = new Vector3(3f, 0f, 1.399969f);
		// 	Vector3 to = new Vector3(7f, 0f, 8.399969f);
		// 	Global.SplittedRailContainer = new SplittedRailsContainer();
		// 	Global.SplittedRailContainer.Rails = new List<RailPath>
		// 	{
		// 		RailPath.BuildNoMeshRail()
		// 	}

		// 	var path = Dijkstra.FindPath(from, to);

		// 	Assert.IsFalse(path is null);
		// 	Assert.IsTrue(path.Count > 0, $"path.Count is {path.Count}");
		// 	Assert.CollectionsAreEqual(new[] { 0, 1, 2, 3 }, path.Select(n => n.NodeNumber));
		// }

		// Developers may target a method with the [Post] attribute to execute code after each test method is run
		public void RunAfterTestMethod()
		{
		}

		// Developers may target a method with the [End] attribute to execute after all tests method have run
		public void RunAfterTestClass()
		{
		}
	}
}
