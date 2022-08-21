using Godot;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains
{
	public class SplittedRailsContainer : Spatial
	{
		[Export] private PackedScene railScene;
		private List<RailPath> rails = new List<RailPath>();
		public IEnumerable<RailPath> Rails
		{
			get { foreach (var r in rails) { yield return r; } }
			set
			{
				rails = value.ToList();
				//PrintPathWithCrossings(value);
			}
		}

		public void UpdateRails()
		{
			SplitRails(Global.VisibleRailContainer.Rails);

			this.RemoveAllChildren();
			rails.ForEach(r => AddChild(r));

			//PrintPathWithCrossings(rails);
		}

		private void SplitRails(IEnumerable<RailPath> inputRails)
		{
			var allCrossings = inputRails.SelectMany(r => r.Crossings).ToList();
			var newRails = new List<RailPath>();

			foreach (var r in inputRails)
			{
				if (r.Crossings.Count <= 2)
				{
					var newRail = RailPath.BuildNoMeshRail(railScene, r.Curve.GetBakedPoints(), Vector3.Zero); //rail.Translation);
					PrintStartEnd(newRail);
					newRails.Add(newRail);

					continue;
				}

				SplitRailPath(r, allCrossings, newRails);
			}

			rails = newRails;
		}

		private void SplitRailPath(RailPath inputRail, List<Vector3> allCrossings, List<RailPath> newRails)
		{
			var points = new List<Vector3>();
			var railPoints = inputRail.Curve.GetBakedPoints();

			for (int i = 0; i < railPoints.Length; i++)
			{
				var currentPoint = inputRail.GlobalTranslation + railPoints[i];
				points.Add(currentPoint);

				if (RailHasCrossing(inputRail, allCrossings, currentPoint))
				{
					var newPath = BuildNewPath(points);
					newRails.Add(newPath);

					var lastPoint = points.Last();
					points.Clear();
					points.Add(lastPoint);
				}

				bool reachedLastPoint = i == railPoints.Length - 1;

				if (reachedLastPoint)
				{
					var newPath = BuildNewPath(points);
					newRails.Add(newPath);
					points.Clear();
				}
			}
		}

		private RailPath BuildNewPath(List<Vector3> points)
		{
			var newPath = RailPath.BuildNoMeshRail(railScene, points, Vector3.Zero);
			PrintStartEnd(newPath);
			return newPath;
		}

		private static bool RailHasCrossing(RailPath railPath, List<Vector3> allCrossings, Vector3 currentPoint)
		{
			return allCrossings.Any(c => currentPoint.IsEqualApprox(c))
				&& !currentPoint.IsEqualApprox(railPath.Start)
				&& !currentPoint.IsEqualApprox(railPath.End);
		}

		private void PrintStartEnd(RailPath rail)
		{
			GD.PrintS("new splitted:", rail.Start + ",", rail.End);
		}

		private static void PrintPathWithCrossings(IEnumerable<RailPath> _paths)
		{
			var paths = _paths.ToList();

			GD.Print("<--SplittedRailsContainer. New Actual rails:");
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
