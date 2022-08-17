using Godot;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

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
				PrintPathWithCrossings(value);
			}
		}

		public void UpdateActualRails()
		{
			Global.ActualRailContainer.Rails = SplitRails(Global.RailContainer.Rails.ToList());
		}

		private List<RailPath> SplitRails(List<RailPath> rails)
		{
			var allCrossings = rails.SelectMany(r => r.Crossings).ToList();
			var dict = new Dictionary<RailPath, List<List<Vector3>>>();
			var newRails = new List<RailPath>();

			foreach (var c in GetChildren().Cast<Godot.Node>())
			{
				c.QueueFree();
			}

			foreach (var railPath in rails)
			{
				if (railPath.Crossings.Count <= 2)
				{
					//should i add new rail with samee tanslation and curve?
					
					// newRails.Add(railPath);

					var newRail = new RailPath();
					newRails.Add(newRail);
					newRail.Curve = railPath.Curve;
					newRail.GlobalTranslation = railPath.GlobalTranslation;

					continue;
				}

				SplitRailPath(railPath, allCrossings, newRails);
			}

			return newRails;
		}

		private void SplitRailPath(RailPath railPath, List<Vector3> allCrossings, List<RailPath> newRails)
		{
			var points = new List<Vector3>();

			for (int i = 0; i < railPath.Curve.GetBakedPoints().Length; i++)
			{
				var currentPoint = railPath.GlobalTranslation + railPath.Curve.GetBakedPoints()[i];
				points.Add(currentPoint);

				if (RailPathHasCrossing(railPath, allCrossings, currentPoint))
				{
					MakeNewPathAndUpdateNewRails(newRails, points);

					var lastPoint = points.Last();
					points.Clear();
					points.Add(lastPoint);
				}

				bool reachedLastPoint = i == railPath.Curve.GetBakedPoints().Length - 1;

				if (reachedLastPoint)
				{
					MakeNewPathAndUpdateNewRails(newRails, points);

					points.Clear();
				}
			}
		}

		private static bool RailPathHasCrossing(RailPath railPath, List<Vector3> allCrossings, Vector3 currentPoint)
		{
			return allCrossings.Any(c => currentPoint.IsEqualApprox(c))
								&& !currentPoint.IsEqualApprox(railPath.Start)
								&& !currentPoint.IsEqualApprox(railPath.End);
		}

		private void MakeNewPathAndUpdateNewRails(List<RailPath> newRails, List<Vector3> points)
		{
			var newPath = BuildPath(points);
			newRails.Add(newPath);
			AddChild(newPath);
		}

		private static RailPath BuildPath(List<Vector3> points)
		{
			var newPath = new RailPath();

			for (int i = 0; i < points.Count; i++)
			{
				//addpoint is wrong points are baked?
				newPath.Curve.AddPoint(points[i]);
			}

			newPath.Curve = RailCurve.GetFrom(newPath.Curve);
			newPath.EnlistCrossing(newPath.Start);
			newPath.EnlistCrossing(newPath.End);
			//shouldnt i enlist mid crossing as well?

			return newPath;
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