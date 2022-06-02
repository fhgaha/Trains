using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Builders
{
	public class StationBuilder : Node
	{
		private Cell[,] cells;
		private Events events;

		public void Init(Cell[,] cells)
		{
			this.cells = cells;
			//cant get events if not in the scene
			events = GetNode<Events>("/root/Events");
			GD.Print("StationBuilder: " + events);
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			//GD.Print("onMainButtonPressed");
		}
	}
}