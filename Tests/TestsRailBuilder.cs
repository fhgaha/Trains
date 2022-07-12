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
			var list = new List<Vector3> { first, second };

			var segment = new CurveSegment(list);

			Assert.IsEqual(segment.First, first, "segment.First should contain first passed parameter");
			Assert.IsEqual(segment.Second, second, "segment.Second should contain second passed parameter");
		}

		[Test]
		public void AddCurveToSegmentsTest()
		{
			var p1 = new Vector3(0, 0, 0);
			var p2 = new Vector3(1, 0, 0);
			var p3 = new Vector3(0, 1, 0);
			var curve = new RailCurve();
			var curveToAdd = new Curve3D();
			curveToAdd.AddPoint(p1);
			curveToAdd.AddPoint(p2);
			curveToAdd.AddPoint(p3);
			var firstSegment = new CurveSegment(p1, p2);
			var secondSegment = new CurveSegment(p2, p3);

			curve.AddCurveToSegments(curveToAdd, 0);

			Assert.IsEqual(curve.Segments.Count, 2, "two segments should be added");
			Assert.IsEqual(curve.Segments[0], firstSegment, "first segment should be CurveSegment with correct points oreder");
			Assert.IsEqual(curve.Segments[1], secondSegment, "second segment should be CurveSegment with correct points oreder");
			//are segments instantiated?
		}







		public void RunAfterTestMethod()
		{
			var text = "Developers may target a method with the" +
			"[Post] attribute to execute code after each test method is run";
		}

		public void RunAfterTestClass()
		{
			var text = "Developers may target a method with the" +
			"[End] attribute to execute after all tests method have run";
		}
	}
}