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
		private Spatial train;

		public override void _Ready()
		{
			Loop = false;
			RotationMode = RotationModeEnum.Oriented;
			train = GetNode<Spatial>("Train");
		}

		public override void _PhysicsProcess(float delta)
		{
			Offset += goForth
				? speed * delta
				: -speed * delta;

			// JumpToOtherRailIfNecessary();
			ChangeDirectionIfNecessary();
		}

		private void JumpToOtherRailIfNecessary()
		{
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
						GetParent().RemoveChild(this);
						r.AddChild(this);
						UnitOffset = 0;
						break;
					}

					var lastPoint = r.GlobalTranslation + r.Curve.Last();
					if (lastPoint.IsEqualApprox(train.GlobalTranslation, 0.2f))
					{
						GetParent().RemoveChild(this);
						r.AddChild(this);
						UnitOffset = 1;
						break;
					}
				}
			}
		}

		private void ChangeDirectionIfNecessary()
		{
			if (ReachedEnd())
			{
				goForth = false;
				train.Rotate(Vector3.Up, Mathf.Pi);
			}
			else if (ReachedStart())
			{
				goForth = true;
				train.Rotate(Vector3.Up, Mathf.Pi);
			}
		}

		private bool ReachedStart() => UnitOffset == 0;

		private bool ReachedEnd() => UnitOffset == 1;
	}
}
