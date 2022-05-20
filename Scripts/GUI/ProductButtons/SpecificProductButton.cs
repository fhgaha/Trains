using Godot;
using System;

namespace Trains.Scripts.GUI.ProductButtons
{
	public class SpecificProductButton : Button
	{
        //connect signals here instead of connecting every button
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
            //Events.Connect("mouse_entered", this, nameof(onButtonMouseEntered));
            Connect("pressed", this, nameof(onButtonPressed));
		}

		private void onButtonPressed()
		{
			GD.Print("onButtonPressed from SpecificProductButton");
			events.EmitSignal(nameof(Events.LumberButtonPressed));
		}

		private void onButtonMouseEntered()
		{
			GrabFocus();
		}


	}
}