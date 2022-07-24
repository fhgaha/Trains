using Godot;
using System;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Tests
{
	[Start(nameof(RunBeforeTestClass))]
	[Pre(nameof(RunBeforeTestMethod))]
	[Post(nameof(RunAfterTestMethod))]
	[End(nameof(RunAfterTestClass))]

	[Title("PointsParser Tests")]
	public class TestPointsParser : WAT.Test
	{
		const string text48 = ""
		+ "[0]:(7.618713, -0.5984715)"
		+ "[1]:(7.718474, -0.5921922)"
		+ "[2]:(7.81711, -0.5759848)"
		+ "[3]:(7.913635, -0.5500112)"
		+ "[4]:(8.007084, -0.514531)"
		+ "[5]:(8.096524, -0.4698986)"
		+ "[6]:(8.181063, -0.4165601)"
		+ "[7]:(8.259853, -0.3550484)"
		+ "[8]:(8.332109, -0.285978)"
		+ "[9]:(8.397109, -0.2100392)"
		+ "[10]:(8.454203, -0.1279906)"
		+ "[11]:(8.502819, -0.04065207)"
		+ "[12]:(8.542475, 0.05110377)"
		+ "[13]:(8.572771, 0.1463601)"
		+ "[14]:(8.593407, 0.2441651)"
		+ "[15]:(8.604175, 0.3435417)"
		+ "[16]:(8.604968, 0.4434968)"
		+ "[17]:(8.595778, 0.5430317)"
		+ "[18]:(8.576698, 0.641152)"
		+ "[19]:(8.547917, 0.7368771)"
		+ "[20]:(8.509724, 0.8292508)"
		+ "[21]:(8.462499, 0.9173501)"
		+ "[22]:(8.406714, 1.000294)"
		+ "[23]:(8.342929, 1.077255)"
		+ "[24]:(8.271778, 1.147464)"
		+ "[25]:(8.193974, 1.210219)"
		+ "[26]:(8.110293, 1.264892)"
		+ "[27]:(8.021572, 1.310939)"
		+ "[28]:(7.928698, 1.347898)"
		+ "[29]:(7.832597, 1.3754)"
		+ "[30]:(7.734231, 1.393171)"
		+ "[31]:(7.634583, 1.401033)"
		+ "[32]:(7.534647, 1.398907)"
		+ "[33]:(7.435423, 1.386816)"
		+ "[34]:(7.337901, 1.364878)"
		+ "[35]:(7.243056, 1.333314)"
		+ "[36]:(7.151837, 1.292439)"
		+ "[37]:(7.065154, 1.242662)"
		+ "[38]:(6.983873, 1.184479)"
		+ "[39]:(6.908807, 1.118473)"
		+ "[40]:(6.840706, 1.045302)"
		+ "[41]:(6.78025, 0.9656981)"
		+ "[42]:(6.728044, 0.8804561)"
		+ "[43]:(6.684608, 0.790428)"
		+ "[44]:(6.650376, 0.6965133)"
		+ "[45]:(6.625692, 0.5996503)"
		+ "[46]:(6.610801, 0.5008069)"
		+ "[47]:(6.605853, 0.4009708)"
		;

		//Developers may target a method with the [Start] attribute to execute code before any test method is run
		public void RunBeforeTestClass()
		{
		}

		// Developers may target a method with the [Pre] attribute to execute code before each test method is run;
		public void RunBeforeTestMethod()
		{
		}

		[Test]
		public void ParsedPointsCountIsCorrect()
		{
			var points = PointsParser.Parse(text48);
			Assert.IsEqual(points.Count(), 48);
		}

		[Test]
		public void SomeParsedPointsAreCorrect()
		{
			var points = PointsParser.Parse(text48);
			Assert.IsEqual(points.ElementAt(0), new Vector2(7.618713f, -0.5984715f));
			Assert.IsEqual(points.ElementAt(28), new Vector2(7.928698f, 1.347898f));
			Assert.IsEqual(points.ElementAt(47), new Vector2(6.605853f, 0.4009708f));
		}

		[Test]
		public void RandomTextIsNotParsed()
		{
			const string text = "some innaproppriate text";
			var points = PointsParser.Parse(text);
			Assert.IsEqual(points.Count(), 0);
		}

		[Test]
		public void EmptyStringIsNotParsed()
		{
			const string text = "";
			var points = PointsParser.Parse(text);
			Assert.IsEqual(points.Count(), 0);
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