using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Trains.Model.Common
{
	public static class PointsParser
	{
		//input example: "[0]:(7.618713, -0.5984715)[1]:(7.718474, -0.5921922)"

		public static IEnumerable<Vector2> Parse(string text)
		{
			foreach (var vec in ParseUsingRegex(text))
				yield return vec;
		}

		private static IEnumerable<Vector2> ParseUsingRegex(string text)
		{
			const string regexPatternToSplitString = @"\[\d+\]\:\(([+-]?[0-9]*[.]?[0-9]+)\,\ ([+-]?[0-9]*[.]?[0-9]+)\)";

			foreach (System.Text.RegularExpressions.Match m in
				System.Text.RegularExpressions.Regex.Matches(text, regexPatternToSplitString))
			{
				var xString = m.Groups[1].Value;
				var yString = m.Groups[2].Value;
				yield return new Vector2(
					Convert.ToSingle(xString, CultureInfo.InvariantCulture),
					Convert.ToSingle(yString, CultureInfo.InvariantCulture));
			}
		}

		private static IEnumerable<Vector2> ParseUsingLinq(string text)
		{
			var coordsString = text.Split('[', ':')
				.Where(s => s.Length > 0 && s.First() == '(' && s.Last() == ')')
				.Select(s => s.Remove(0, 1))
				.Select(s => s.Remove(s.Length - 1, 1))
				.Select(s => s.Trim())
				.ToList();

			//s example: "7.618713, -0.5984715"
			foreach (var s in coordsString)
			{
				var coords = s.Split(", ");
				yield return new Vector2(
					Convert.ToSingle(coords[0], CultureInfo.InvariantCulture),
					Convert.ToSingle(coords[1], CultureInfo.InvariantCulture));
			}
		}
	}
}