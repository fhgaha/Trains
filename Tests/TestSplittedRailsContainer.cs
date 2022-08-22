using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trains.Model.Builders;
using Trains.Model.Cells;

namespace Trains.Tests
{
	public class TestSplittedRailsContainer : WAT.Test
	{
		// [Test]
		// //unable to make. needed scene load
		// public void SplitRails()
		// {
		// 	var railToSplit = new RailPath();

		// 	railToSplit.Curve = new RailCurve();
		// 	railToSplit.Curve.AddPoint(Vector3.Zero);
		// 	railToSplit.Curve.AddPoint(new Vector3(1f, 0f, 0f));
		// 	railToSplit.Curve.AddPoint(new Vector3(2f, 0f, 0f));

		// 	railToSplit.EnlistCrossing(Vector3.Zero);
		// 	railToSplit.EnlistCrossing(new Vector3(1f, 0f, 0f));
		// 	railToSplit.EnlistCrossing(new Vector3(2f, 0f, 0f));

		// 	var splitter = new SplittedRailsContainer();

		// 	MethodInfo methodInfo = typeof(SplittedRailsContainer)
		// 		.GetMethod("SplitRails", BindingFlags.Instance | BindingFlags.NonPublic);

		// 	void SplitRails(IEnumerable<RailPath> _inputRails)
		// 		=> methodInfo.Invoke(splitter, new object[] { _inputRails });

		// 	SplitRails(new List<RailPath> { railToSplit });

		// 	// splitter.SplitRails(new List<RailPath> { railToSplit });


		// 	Assert.IsTrue(true);
		// 	// Assert.IsTrue(splitter.Rails.ToList().Count > 0);
		// }
	}
}