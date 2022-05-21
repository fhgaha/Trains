using Godot;
using System;
using static Trains.Model.Common.Enums;

// Event bus for distant nodes to communicate using signals.
// This is intended for cases where connecting the nodes directly creates more coupling
// or increases code complexity substantially.

namespace Trains.Model.Common
{
	public class Events : Node
	{
		// Emitted when lumber button is pressed. 
		// Grid uses this to show lumber prices and colors
		[Signal] public delegate void SpecificProductButtonPressed(ProductType productType);
	}
}
