using Godot;
using System;

namespace Trains
{
	public class MyPathFollow : PathFollow
	{
		[Export] private float speed = 2;

		[Export] private bool goForth = true;

		public override void _PhysicsProcess(float delta)
		{
			Offset += goForth
				? speed * delta
				: -speed * delta;

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