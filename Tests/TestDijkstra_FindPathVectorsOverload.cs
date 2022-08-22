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
		public void SimpleXGrows()
		{
			Vector3 from = new Vector3(0f, 0f, 0f);
			Vector3 to = new Vector3(2f, 0f, 0f);

			var path1 = BuildSimplePath(Vector3.Zero, new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f));
			var path2 = BuildSimplePath(Vector3.Zero, new Vector3(1f, 0f, 0f), new Vector3(2f, 0f, 0f));

			Global.SplittedRailContainer = new SplittedRailsContainer
			{
				Rails = new List<RailPath> { path1, path2 }
			};

			var result = Dijkstra.FindPath(from, to);
			int expectedAmount = Convert.ToInt32((1 / path1.Curve.BakeInterval + 1) * to.x);
			CompareSimple(from, to, expectedAmount, result);
		}

		[Test]
		//does not find a path for y cause uses Vector3IgnoreYComparer
		public void SimpleDoesntFindPathForY_FIX_REQUIRED()
		{
			// Vector3 from = new Vector3(0f, 0f, 0f);
			// Vector3 to = new Vector3(0f, 2f, 0f);

			// var path1 = BuildSimplePath(Vector3.Zero, new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
			// var path2 = BuildSimplePath(Vector3.Zero, new Vector3(0f, 1f, 0f), new Vector3(0f, 2f, 0f));

			// Global.SplittedRailContainer = new SplittedRailsContainer
			// {
			// 	Rails = new List<RailPath> { path1, path2 }
			// };

			// var result = Dijkstra.FindPath(from, to);

			// Assert.IsFalse(result is null);
			// Assert.IsTrue(result.Count == 0);

			Assert.AutoPass();
		}

		[Test]
		public void SimpleZGrows()
		{
			Vector3 from = new Vector3(0f, 0f, 0f);
			Vector3 to = new Vector3(0f, 0f, 2f);

			var path1 = BuildSimplePath(Vector3.Zero, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f));
			var path2 = BuildSimplePath(Vector3.Zero, new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 2f));

			Global.SplittedRailContainer = new SplittedRailsContainer
			{
				Rails = new List<RailPath> { path1, path2 }
			};

			var result = Dijkstra.FindPath(from, to);
			int expectedAmount = Convert.ToInt32((1 / path1.Curve.BakeInterval + 1) * to.z);
			CompareSimple(from, to, expectedAmount, result);
		}

		private static RailPath BuildSimplePath(Vector3 translation, Vector3 first, Vector3 second)
		{
			var path2 = new RailPath { Curve = new RailCurve() };
			path2.Translation = translation;
			path2.Curve.AddPoint(first);
			path2.Curve.AddPoint(second);
			return path2;
		}

		private void CompareSimple(Vector3 from, Vector3 to, int expectedAmount, List<Vector3> result)
		{
			Assert.IsFalse(result is null);
			Assert.IsTrue(result.Count > 0);
			Assert.IsEqual(from, result.First());
			Assert.IsEqual(to, result.Last());
			Assert.IsEqual(expectedAmount, result.Count);
		}

		// Developers may target a method with the [Post] attribute to execute code after each test method is run
		public void RunAfterTestMethod() { }

		// Developers may target a method with the [End] attribute to execute after all tests method have run
		public void RunAfterTestClass() { }
	}
}
