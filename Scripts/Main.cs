using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Cells;
using Trains.Model.Cells.Buildings.Sources;
using Trains.Model.Common;
using Trains.Model.Grids;

namespace Trains.Scripts
{
	public class Main : Spatial
	{
		private Events events;
		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			var timer = GetNode<Timer>("MainTimer");
			timer.Connect("timeout", this, nameof(onTimeout));
			timer.Start(1f);
		}

		private void onTimeout()
		{
			events.EmitSignal(nameof(Events.Tick));

			//for each source find cell with best price, move products there

			// var grid = GetNode<Grid>("Grid");
			// List<Cell> cellsWithSources = grid.GetReadyToShipProductsCells();
			// foreach (Cell cell in cellsWithSources)
			// {
			// 	//cell is start
			// 	//get end
			// 	var end = grid.GetBestBuyCell(cell);

			// 	var path = new AStar2D().AddPoint()
			// }
			
		}
	}
}