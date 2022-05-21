using Godot;
using System;
using Trains.Model.Common;

namespace Trains.Scripts.GUI.ProductButtons
{
	public class SpecificProductButton : Button
	{
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
            Connect("pressed", this, nameof(onButtonPressed));
		}

		private void onButtonPressed()
		{
			events.EmitSignal(nameof(Events.SpecificProductButtonPressed), Enums.ProductType.Lumber);
		}
	}
}