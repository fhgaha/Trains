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

		//Developers may target a method with the [Start] attribute to execute code before any test method is run
		public void RunBeforeTestClass()
		{
		}

		// Developers may target a method with the [Pre] attribute to execute code before each test method is run;
		public void RunBeforeTestMethod()
		{
			calculator = new CurveCalculator();
		}

		// [Test]
		// public void CalculateCenter()
		// {
		// 	var methodInfo = typeof(CurveCalculator).GetMethod(
		// 		"CalculateCenter", BindingFlags.NonPublic | BindingFlags.Instance);
		// 	Vector2 CalculateCenter(bool centerIsOnRight, Vector2 start)
		//   		=> (Vector2)methodInfo.Invoke(calculator, new object[] { centerIsOnRight, start });

		// 	//in this case prevDir is always zero

		// 	Assert.IsEqual(Vector2.Right, CalculateCenter(centerIsOnRight: true, start: Vector2.Down));
		// }

		// Developers may target a method with the [Post] attribute to execute code after each test method is run
		public void RunAfterTestMethod()
		{
			calculator = null;
		}

		// Developers may target a method with the [End] attribute to execute after all tests method have run
		public void RunAfterTestClass()
		{
		}
	}
}
