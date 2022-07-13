using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Builders;

namespace Trains.Tests
{
	[Start(nameof(RunBeforeTestClass))]
	[Pre(nameof(RunBeforeTestMethod))]
	[Post(nameof(RunAfterTestMethod))]
	[End(nameof(RunAfterTestClass))]

	[Title("RailBuilder Tests")]
	public class TestsRailBuilder : WAT.Test
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
		public void BuildSegmentUsingVectorsTest()
		{
			var first = new Vector3(1, 0, 0);
			var second = new Vector3(0, 1, 0);

			var segment = new CurveSegment(first, second);

			Assert.IsEqual(segment.First, first, "segment.First should contain first passed parameter");
			Assert.IsEqual(segment.Second, second, "segment.Second should contain second passed parameter");
		}

		[Test]
		public void BuildSegmentUsingCollectionTest()
		{
			var first = new Vector3(1, 0, 0);
			var second = new Vector3(0, 1, 0);
			var collection = new List<Vector3> { first, second };

			var segment = new CurveSegment(collection);

			Assert.IsEqual(segment.First, first, "segment.First should contain first passed parameter");
			Assert.IsEqual(segment.Second, second, "segment.Second should contain second passed parameter");
		}






		public void RunAfterTestMethod()
		{
			// Developers may target a method with the [Post] attribute to execute code after each test method is run
		}

		public void RunAfterTestClass()
		{
			// Developers may target a method with the [End] attribute to execute after all tests method have run
		}
	}
}