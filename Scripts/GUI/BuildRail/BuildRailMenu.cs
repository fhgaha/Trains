using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class BuildRailMenu : Control
	{
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			var undoBtn = GetNode<Button>("Undo");
			undoBtn.Connect("pressed", this, nameof(onUndoPressed));
		}

		private void onUndoPressed()
		{
			events.EmitSignal(nameof(Events.UndoRailPressed));
		}
	}
}