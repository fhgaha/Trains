using Godot;
using System;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Builders
{
	public class RailBuilder : MapObjectBuilder
	{
		private PackedScene railScene = GD.Load<PackedScene>("res://Scenes/Rail.tscn");
		public void Init()
		{
			
		}

		public override void _PhysicsProcess(float delta)
		{

		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton ev && ev.IsActionPressed("lmb"))
			{
				//start drawing rail
			}

			
		}
	}
}