using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trains.Model.Builders;
using Trains.Model.Common;

namespace Trains.Tests
{
	[Start(nameof(RunBeforeTestClass))]
	[Pre(nameof(RunBeforeTestMethod))]
	[Post(nameof(RunAfterTestMethod))]
	[End(nameof(RunAfterTestClass))]

	[Title("CurveCalculator Tests")]
	public class TestCurveCalculator : WAT.Test
	{
		CurveCalculator calculator;
		MethodInfo methodInfo;

		//Developers may target a method with the [Start] attribute to execute code before any test method is run
		public void RunBeforeTestClass()
		{
		}

		// Developers may target a method with the [Pre] attribute to execute code before each test method is run;
		public void RunBeforeTestMethod()
		{
			calculator = new CurveCalculator();
			methodInfo = typeof(CurveCalculator).GetMethod(
				"IsPointOnLineApprox", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		[Test]
		public void PointIsOnLineApprox()
		{
			var text = ""
			+ "[0]:(0, 0)"
			+ "[1]:(1, 1)"
			;

			var points = PointsParser.Parse(text).ToList();
			bool PointIsOnLine(Vector2 p) => (bool)methodInfo.Invoke(calculator, new object[] { points[0], points[1], p });

			Assert.IsTrue(PointIsOnLine(new Vector2(2, 2)));
		}

		[Test]
		public void PointIsOffLineApprox()
		{
			var text = ""
			+ "[0]:(0, 0)"
			+ "[1]:(1, 1)"
			;

			var points = PointsParser.Parse(text).ToList();
			bool PointIsOnLine(Vector2 p) => (bool)methodInfo.Invoke(calculator, new object[] { points[0], points[1], p });

			Assert.IsFalse(PointIsOnLine(new Vector2(2, 1)));
			Assert.IsFalse(PointIsOnLine(new Vector2(2, 3)));
			Assert.IsFalse(PointIsOnLine(new Vector2(1, 2)));
			Assert.IsFalse(PointIsOnLine(new Vector2(3, 2)));

			Assert.IsFalse(PointIsOnLine(new Vector2(2, 2.5f)));
			Assert.IsFalse(PointIsOnLine(new Vector2(2, 1.5f)));
			Assert.IsFalse(PointIsOnLine(new Vector2(1.5f, 2)));
			Assert.IsFalse(PointIsOnLine(new Vector2(2.5f, 2)));
		}

		// Developers may target a method with the [Post] attribute to execute code after each test method is run
		public void RunAfterTestMethod()
		{
			calculator = null;
			methodInfo = null;
		}

		// Developers may target a method with the [End] attribute to execute after all tests method have run
		public void RunAfterTestClass()
		{
		}
	}
}