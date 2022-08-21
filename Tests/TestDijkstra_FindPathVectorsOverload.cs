using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;

namespace Trains.Tests
{
	[Start(nameof(RunBeforeTestClass))]
	[Pre(nameof(RunBeforeTestMethod))]
	[Post(nameof(RunAfterTestMethod))]
	[End(nameof(RunAfterTestClass))]

	public class TestDijkstra_FindPathVectorsOverload : WAT.Test
	{
		//Developers may target a method with the [Start] attribute to execute code before any test method is run
		public void RunBeforeTestClass() { }

		// Developers may target a method with the [Pre] attribute to execute code before each test method is run;
		public void RunBeforeTestMethod() { }

		[Test]
		public void Simple1()
		{
			Vector3 from = new Vector3(0f, 0f, 0f);
			Vector3 to = new Vector3(2f, 0f, 0f);

			var path1 = new RailPath { Curve = new RailCurve() };
			path1.Translation = new Vector3(0f, 0f, 0f);
			path1.Curve.AddPoint(new Vector3(0f, 0f, 0f));
			path1.Curve.AddPoint(new Vector3(1f, 0f, 0f));

			var path2 = new RailPath { Curve = new RailCurve() };
			path2.Translation = new Vector3(0f, 0f, 0f);
			path2.Curve.AddPoint(new Vector3(1f, 0f, 0f));
			path2.Curve.AddPoint(new Vector3(2f, 0f, 0f));

			Global.SplittedRailContainer = new SplittedRailsContainer();
			Global.SplittedRailContainer.Rails = new List<RailPath> { path1, path2 };

			var result = Dijkstra.FindPath(from, to);

			Assert.IsFalse(result is null);
			Assert.IsTrue(result.Count > 0);
			Assert.IsEqual(from, result.First());
			Assert.IsEqual(to, result.Last());
			//Assert.CollectionsAreEqual(new[] { 0, 1, 2, 3 }, path.Select(n => n.NodeNumber));
		}

		// Developers may target a method with the [Post] attribute to execute code after each test method is run
		public void RunAfterTestMethod() { }

		// Developers may target a method with the [End] attribute to execute after all tests method have run
		public void RunAfterTestClass() { }
	}
}
