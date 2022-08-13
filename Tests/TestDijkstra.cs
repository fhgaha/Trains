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

	[Title("TestDijkstra")]
	public class TestDijkstra : WAT.Test
	{
		CurveCalculator calculator;

		//Developers may target a method with the [Start] attribute to execute code before any test method is run
		public void RunBeforeTestClass()
		{
		}

		// Developers may target a method with the [Pre] attribute to execute code before each test method is run;
		public void RunBeforeTestMethod()
		{
			calculator = new CurveCalculator();
		}

		[Test]
		public void FindPath()
		{
			var graph = new Graph(4);
			var weights = new Dictionary<Edge, double>();
			weights[graph.Connect(0, 1)] = 1;
			weights[graph.Connect(0, 2)] = 2;
			weights[graph.Connect(0, 3)] = 6;
			weights[graph.Connect(1, 3)] = 4;
			weights[graph.Connect(2, 3)] = 2;

			var path = Dijkstra.FindPaths(graph, weights, graph[0], graph[3])
				.Select(n => n.NodeNumber);

			CollectionAssertAreEqual(new[] { 0, 2, 3 }, path);
		}

		// Developers may target a method with the [Post] attribute to execute code after each test method is run
		public void RunAfterTestMethod()
		{
		}

		// Developers may target a method with the [End] attribute to execute after all tests method have run
		public void RunAfterTestClass()
		{
		}

		private void CollectionAssertAreEqual(IEnumerable<int> expected, IEnumerable<int> path)
		{
			Assert.IsEqual(3, path.Count());
			for (int i = 0; i < path.Count(); i++)
			{
				Assert.IsEqual(expected.ElementAt(i), path.ElementAt(i));
			}
		}
	}
}
