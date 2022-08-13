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
		public void MakeGraph()
		{
			var graph = Graph.MakeGraph(
				0, 1,
				0, 2,
				0, 3,
				1, 3,
				2, 3
			);

			Assert.IsEqual(graph[0], new Node(0));
			Assert.IsEqual(graph[1], new Node(1));
			Assert.IsEqual(graph[2], new Node(2));
			Assert.IsEqual(graph[3], new Node(3));
			Assert.IsEqual(graph[4], new Node(4));
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
