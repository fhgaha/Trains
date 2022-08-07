using Godot;
using System;

namespace Trains
{
	public class MyPathFollow : PathFollow
	{
		[Export] private float speed = 2;

		public override void _Process(float delta)
		{
			Offset += speed * delta;
		}
	}
}