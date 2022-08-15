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

	//[Title("TestDijkstra")]
	public class TestRailBuilder : WAT.Test
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
		public void foo()
		{
			Assert.IsTrue(true);
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
			Assert.IsFalse(path is null);
			Assert.IsEqual(expected.Count(), path.Count());
			for (int i = 0; i < path.Count(); i++)
			{
				Assert.IsEqual(expected.ElementAt(i), path.ElementAt(i));
			}
		}
	}
}
