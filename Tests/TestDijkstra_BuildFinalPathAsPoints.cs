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
	public class TestDijkstra_BuildFinalPathAsPoints : WAT.Test
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
		public void BuildFinalPathAsPoints1()
		{
			var path1 = new RailPath { Curve = new RailCurve() };
			path1.Curve.AddPoint(new Vector3(0, 0, 0));
			path1.Curve.AddPoint(new Vector3(1, 0, 0));

			var path2 = new RailPath { Curve = new RailCurve() };
			path2.Curve.AddPoint(new Vector3(0, 0, 0));
			path2.Curve.AddPoint(new Vector3(1, 0, 0));

			var splitted = new List<RailPath> { path1, path2 };

			var turningPoints = new List<Vector3>
			{
				new Vector3(0, 0, 0), new Vector3(1, 0, 0)
			};

			var methodInfo = typeof(Dijkstra).GetMethod(
				"BuildFinalPathAsPoints",
				BindingFlags.NonPublic | BindingFlags.Static);

			List<Vector3> BuildFinalPathAsPoints(List<RailPath> _splitted, List<Vector3> _turningPoints)
				=> (List<Vector3>)methodInfo.Invoke(null, new object[] { _splitted, _turningPoints });

			var result = BuildFinalPathAsPoints(splitted, turningPoints);

			Assert.IsEqual(new Vector3(0, 0, 0), result[0]);
			Assert.IsEqual(new Vector3(1, 0, 0), result.Last());
		}

		[Test]
		public void BuildFinalPathAsPointsApproved1()
		{
			var path1 = new RailPath { Curve = new RailCurve() };
			path1.Translation = new Vector3(2.987319f, 0f, 4.471903f);
			path1.Curve.AddPoint(new Vector3(2.976347f, 1.95679E-08f, 4.921086f) - path1.Translation);
			path1.Curve.AddPoint(new Vector3(4.318431f, 0f, 3.419493f) - path1.Translation);

			var path2 = new RailPath { Curve = new RailCurve() };
			path2.Translation = new Vector3(7.987319f, 0f, 2.471903f);
			path2.Curve.AddPoint(new Vector3(7.976347f, 1.95679E-08f, 2.921086f) - path2.Translation);
			path2.Curve.AddPoint(new Vector3(4.318431f, 0f, 3.419493f) - path2.Translation);

			var splitted = new List<RailPath> { path1, path2 };

			var turningPoints = new List<Vector3>
			{
				new Vector3(2.976347f, 1.95679E-08f, 4.921086f),
				new Vector3(4.318431f, 0f, 3.419493f),
				new Vector3(7.976347f, 1.95679E-08f, 2.921086f)
			};

			var methodInfo = typeof(Dijkstra).GetMethod(
				"BuildFinalPathAsPoints",
				BindingFlags.NonPublic | BindingFlags.Static);

			List<Vector3> BuildFinalPathAsPoints(List<RailPath> _splitted, List<Vector3> _turningPoints)
				=> (List<Vector3>)methodInfo.Invoke(null, new object[] { _splitted, _turningPoints });

			var result = BuildFinalPathAsPoints(splitted, turningPoints);

			Assert.IsTrue(new Vector3(-0.01097202f, 1.95679E-08f, 0.449183f).IsEqualApprox(result.First()));
			Assert.IsTrue(new Vector3(2.017419f, 3.671378E-09f, -1.145922f).IsEqualApprox(result[result.Count / 2]));
			Assert.IsTrue(new Vector3(4.989028f, 1.95679E-08f, -1.550817f).IsEqualApprox(result.Last()));
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
