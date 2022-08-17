using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using Trains.Model.Common.GraphRelated;

namespace Trains
{
	public class ActualRailsContainer : Spatial
	{
		private List<RailPath> rails;
		public IEnumerable<RailPath> Rails
		{
			get { foreach (var r in rails) { yield return r; } }
			set
			{
				rails = value.ToList();
				Build(value);
				PrintPathWithCrossings(value);
			}
		}

		private void Build(IEnumerable<RailPath> rails)
		{
			foreach (var c in GetChildren().Cast<RailPath>())
			{
				c.QueueFree();
			}

			foreach (var r in rails)
			{
				AddChild(r);
			}
		}

		private static void PrintPathWithCrossings(IEnumerable<RailPath> _paths)
		{
			var paths = _paths.ToList();

			GD.Print("<--ActualRailsContainer. New Actual rails:");
			for (int i = 0; i < paths.Count; i++)
			{
				GD.Print($"{i + 1}. {paths[i]}");
				foreach (var cr in paths[i].Crossings)
				{
					GD.Print("	" + cr);
				}
			}
			GD.Print("-->");
		}
	}
}