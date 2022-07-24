using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Trains.Model.Common
{
	public static class PointsParser
	{
		public static IEnumerable<Vector2> Parse(string text)
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