using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class MainPanel : VBoxContainer
	{
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			Connect("mouse_entered", this, nameof(onMouseEntered));
			Connect("mouse_exited", this, nameof(onMouseExited));
		}

		private void onMouseEntered()
		{
			events.EmitSignal(nameof(Events.MainGUIPanelMouseEntered));
		}

		private void onMouseExited()
		{
			events.EmitSignal(nameof(Events.MainGUIPanelMouseExited));
		}
	}
}