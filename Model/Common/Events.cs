using Godot;
using System;

// Event bus for distant nodes to communicate using signals.
// This is intended for cases where connecting the nodes directly creates more coupling
// or increases code complexity substantially.
public class Events : Node
{
	// Emitted when lumber button is pressed. 
	// Grid uses this to show lumber prices and colors
	[Signal] public delegate void LumberButtonPressed();


}
