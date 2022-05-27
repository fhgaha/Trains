using Godot;
using System;
using static Trains.Model.Common.Enums;

// Event bus for distant nodes to communicate using signals.
// This is intended for cases where connecting the nodes directly creates more coupling
// or increases code complexity substantially.

// usage example:
// private Events events;
// 	public override void _Ready()
// 	{
// 		events = GetNode<Events>("/root/Events");
// 		events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButtonPressed));
// 	}

namespace Trains.Model.Common
{
	public class Events : Node
	{
		// Emitted when main timer ticks
		// Cell usies this to update product amount
		[Signal] public delegate void Tick();

		// Emitted when specific product button is pressed. 
		// Grid uses this to display product prices and colors
		[Signal] public delegate void SpecificProductButtonPressed(ProductType productType);

		// Emitted when AllProducts button is pressed. 
		// Grid uses this to hide prices and colors
		[Signal] public delegate void AllProductButtonPressed();

		public override void _Ready()
		{
			GD.Print("Events is ready");
		}
	}
}
