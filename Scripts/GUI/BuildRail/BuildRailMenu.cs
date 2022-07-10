using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class BuildRailMenu : Control
	{
		private Events events;
		private Button stopBuildingCurrentRoad;
		private Button undoBtn;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			stopBuildingCurrentRoad = GetNode<Button>("StopBuildingCurrentRoad");
			undoBtn = GetNode<Button>("Undo");

			stopBuildingCurrentRoad.Connect("pressed", this, nameof(onStopBuildingCurrentRoadPressed));
			undoBtn.Connect("pressed", this, nameof(onUndoPressed));
		}

		private void onStopBuildingCurrentRoadPressed()
		{
			events.EmitSignal(nameof(Events.StopBuildingCurrentRoadPressed));
		}

		private void onUndoPressed()
		{
			events.EmitSignal(nameof(Events.UndoRailPressed));
		}
	}
}