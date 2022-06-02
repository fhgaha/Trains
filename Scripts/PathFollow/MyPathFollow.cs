using Godot;
using System;

namespace Trains
{
	public class MyPathFollow : PathFollow
	{
		private float speed = 5;

		public override void _Process(float delta)
		{
			Offset += speed * delta;
		}
	}
}