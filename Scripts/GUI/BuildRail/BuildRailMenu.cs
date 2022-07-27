using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class BuildRailMenu : Control
	{
		private Events events;
		private Button startNewRoadBtn;
		private Button undoBtn;
		private Button removeBtn;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			startNewRoadBtn = GetNode<Button>("StartNewRoad");
			undoBtn = GetNode<Button>("Undo");
			removeBtn = GetNode<Button>("Remove");

			startNewRoadBtn.Connect("pressed", this, nameof(onStartNewRoadPressed));
			undoBtn.Connect("pressed", this, nameof(onUndoPressed));
			removeBtn.Connect("pressed", this, nameof(onRemovePressed));
		}

		private void onStartNewRoadPressed()
		{
			events.EmitSignal(nameof(Events.StartNewRoadPressed));
		}

		private void onUndoPressed()
		{
			events.EmitSignal(nameof(Events.UndoRailPressed));
		}

		private void onRemovePressed()
		{
			events.EmitSignal(nameof(Events.RemoveRailPressed), removeBtn.Pressed);
		}
	}
}
