using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common.GraphRelated;

namespace Trains.Tests
{
	[Start(nameof(RunBeforeTestClass))]
	[Pre(nameof(RunBeforeTestMethod))]
	[Post(nameof(RunAfterTestMethod))]
	[End(nameof(RunAfterTestClass))]

	//[Title("TestDijkstra")]
	public class TestGraph : WAT.Test
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
		public void TestMakeGraph()
		{
			var graph = Graph.MakeGraph(
				0, 1,
				1, 2,
				4, 1,
				2, 3
			);

			Assert.IsEqual(5, graph.Length);
			Assert.IsEqual(graph[0], new Node(0));
			Assert.IsEqual(graph[1], new Node(1));
			Assert.IsEqual(graph[2], new Node(2));
			Assert.IsEqual(graph[3], new Node(3));
			Assert.IsEqual(graph[4], new Node(4));

			Assert.IsEqual(4, graph.Edges.ToArray().Length);
			Assert.IsTrue(graph.GetEdge(0, 1).IsIncident(graph[0]));
			Assert.IsTrue(graph.GetEdge(1, 2).IsIncident(graph[2]));
			Assert.IsTrue(graph.GetEdge(4, 1).IsIncident(graph[1]));
			Assert.IsTrue(graph.GetEdge(2, 3).IsIncident(graph[3]));
		}

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
