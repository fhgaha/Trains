using Godot;
using System;
using Trains.Model.Cells;
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

		//Emitted when Build station button was pressed.
		[Signal] public delegate void MainButtonPressed(MainButtonType buttonType);

		// Emitted when AllProducts button is pressed. 
		// Grid uses this to hide prices and colors
		[Signal] public delegate void AllProductButtonPressed();

		// Emitted when specific product button is pressed. 
		// Grid uses this to display product prices and colors
		[Signal] public delegate void SpecificProductButtonPressed(ProductType productType);

		//Emitted when you hover mouse on cell.
		//GUI uses this to display cell data in bottom section
		[Signal] public delegate void MouseHoveredOnCell(Cell cell);

		//Emitted when you stoped hover mouse on cell.
		//GUI uses this to hide cell data in bottom section
		[Signal] public delegate void MouseHoveredOffCell(Cell cell);

		[Signal] public delegate void StartNewRoadPressed();

		[Signal] public delegate void UndoRailPressed();


		public override void _Ready()
		{
			GD.Print("Events is ready");
		}
	}
}
