using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Builders;
using Trains.Model.Cells;

namespace Trains.Tests
{
	[Start(nameof(RunBeforeTestClass))]
	[Pre(nameof(RunBeforeTestMethod))]
	[Post(nameof(RunAfterTestMethod))]
	[End(nameof(RunAfterTestClass))]

	public class TestActualRailBuilder : WAT.Test
	{
		//Developers may target a method with the [Start] attribute to execute code before any test method is run
		public void RunBeforeTestClass() { }

		// Developers may target a method with the [Pre] attribute to execute code before each test method is run;
		public void RunBeforeTestMethod() { }

		// <--
		// [Path:11239]
		// 	(1.808818, 0, 1.999234)
		// 	(5.808769, 0, 1.979503)
		// [Path:11247]
		// 	(5.808769, 0, 1.979503)
		// 	(9.993267, 0, 1.958865)
		// [Path:9256]
		// 	(1.891796, 4.842877E-08, 7.823064)
		// 	(9.99446, 4.842877E-08, 7.629604)
		// [Path:11161]
		// 	(4.890941, 4.74975E-08, 7.751453)
		// 	(5.808769, 4.74975E-08, 1.979503)
		// -->

		[Test]
		public void foo()
		{
			Assert.IsTrue(true);
		}

		[Test]
		public void foo2()
		{
			// var paths = new List<RailPath>();

			// var bp = new RailPath();
			// bp.GlobalTransform = new Transform
			// (
			// 	new Vector3(1, 0, 0),
			// 	new Vector3(0, 1, 0),
			// 	new Vector3(0, 0, 1),
			// 	new Vector3(1, 0, 0)
			// );
			// AddChild(bp);

			// for (int i = 0; i < 10; i++)
			// {
			// 	bp.Curve.AddPoint(new Vector3(i, 0, 0));
			// }

			// var path1 = new RailPath();
			// path1.InitOnPlacement(bp);




			Assert.IsTrue(true);
		}

		// Developers may target a method with the [Post] attribute to execute code after each test method is run
		public void RunAfterTestMethod() { }

		// Developers may target a method with the [End] attribute to execute after all tests method have run
		public void RunAfterTestClass() { }
	}
}