using Godot;
using System;

namespace Trains
{
	public class railroadTest2 : Spatial
	{
		[Export] private PackedScene trainScene;

		public override void _Ready()
		{
			var pf = new PathFollow();
			GetNode<RailPath>("RailPath").AddChild(pf);
			pf.AddChild(trainScene.Instance<Spatial>());
		}
	}
}