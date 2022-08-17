using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Model.Builders
{
	public class ActualRailBuilder
	{
		public void UpdateActualRails()
		{
			Global.ActualRailContainer.Rails = SplitRails(Global.RailContainer.Rails.ToList());
		}

		private static List<RailPath> SplitRails(List<RailPath> rails)
		{
			var allCrossings = rails.SelectMany(r => r.Crossings).ToList();
			var dict = new Dictionary<RailPath, List<List<Vector3>>>();
			var newRails = new List<RailPath>();

			foreach (var railPath in rails)
			{
				if (railPath.Crossings.Count <= 2)
				{
					newRails.Add(railPath);
					continue;
				}

				SplitRailPath(railPath, allCrossings, newRails);
			}

			return newRails;
		}

		private static void SplitRailPath(RailPath railPath, List<Vector3> allCrossings, List<RailPath> newRails)
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

		private static void MakeNewPathAndUpdateNewRails(List<RailPath> newRails, List<Vector3> points)
		{
			var newPath = MakePath(points);
			newRails.Add(newPath);
		}

		private static RailPath MakePath(List<Vector3> points)
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
			return newPath;
		}
	}
}