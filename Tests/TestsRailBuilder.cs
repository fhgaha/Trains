using Godot;
using System;
using Trains.Model.Builders;

namespace Trains.Tests
{
	[Title("TestsRailBuilder")]
	public class TestsRailBuilder : WAT.Test
	{
		[Test]
		public void RailCurveAddsNewSegmentToSegmentsListTest()
		{
			var curve = new RailCurve();
			var curveToAdd = new Curve3D();
			var p1 = new Vector3(0, 0, 0);
			var p2 = new Vector3(1, 0, 0);
			var p3 = new Vector3(0, 1, 0);
			curveToAdd.AddPoint(p1);
			curveToAdd.AddPoint(p2);
			curveToAdd.AddPoint(p3);
			var firstSegment = new CurveSegment(p1, p2);
			var secondSegment = new CurveSegment(p2, p3);

			curve.AddCurveToSegments(curveToAdd, 0);

			Assert.IsEqual(curve.Segments.Count, 2, "two segments should be added");
			Assert.IsEqual(curve.Segments[0], firstSegment, "ayyyy");
			Assert.IsEqual(curve.Segments[1], secondSegment, "yooooo");
		}

	}
}