using Godot;
using System;
using System.Linq;
using Trains.Model.Common;

namespace Trains
{
	public class RailPathFollow : PathFollow
	{
		[Export] private float speed = 2;

		[Export] private bool goForth = true;

		public override void _PhysicsProcess(float delta)
		{
			Offset += goForth
				? speed * delta
				: -speed * delta;

			if (ReachedEnd() || ReachedStart())
			{
				var otherRails = GetTree().GetNodesInGroup("Rails")
					.Cast<RailPath>()
					.Where(r => r != GetParent<RailPath>());

				foreach (var r in otherRails)
				{
					var train = GetNode<Spatial>("Train");

					var firstPoint = r.GlobalTranslation + r.Curve.First();
					if (firstPoint.IsEqualApprox(train.GlobalTranslation, 0.2f))
					{
						GD.Print("firstPoint.IsEqualApprox(train.GlobalTranslation, 0.2f)");
						break;
					}

					var lastPoint = r.GlobalTranslation + r.Curve.Last();
					if (lastPoint.IsEqualApprox(train.GlobalTranslation, 0.2f))
					{
						GD.Print("lastPoint.IsEqualApprox(train.GlobalTranslation, 0.2f)");
						break;
					}
				}
			}

			ChangeDirectionIfNeeded();
		}

		private void ChangeDirectionIfNeeded()
		{
			if (ReachedEnd())
			{
				goForth = false;
			}
			else if (ReachedStart())
			{
				goForth = true;
			}
		}

		private bool ReachedStart() => UnitOffset == 0;

		private bool ReachedEnd() => UnitOffset == 1;
	}
}
