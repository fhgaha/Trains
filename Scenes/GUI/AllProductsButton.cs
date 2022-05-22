using Godot;
using System;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

public class AllProductsButton : Button
{
	private Events events;
	private bool wasPressed = true;

	public override void _Ready()
	{
		events = GetNode<Events>("/root/Events");
		Connect("pressed", this, nameof(onButtonPressed));
		events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButton));
	}

	private void onButtonPressed()
	{
		if (wasPressed)
		{
			Pressed = true;
			return;
		}

		wasPressed = true;
		events.EmitSignal(nameof(Events.AllProductButtonPressed));
	}

	private void onSpecificProductButton(ProductType productType)
	{
		Unpress();
	}

	private void Unpress()
	{
		wasPressed = false;
		Pressed = false;
	}
}
