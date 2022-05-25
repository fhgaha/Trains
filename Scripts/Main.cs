using Godot;
using Trains.Model;
using Trains.Model.Common;
using Trains.Model.Grids;

namespace Trains.Scripts
{
	public class Main : Spatial
	{
		private Events events;
		private MoveProductsClass mover;
		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			mover = new MoveProductsClass();
			var timer = GetNode<Timer>("MainTimer");
			timer.Connect("timeout", this, nameof(onTimeout));
			timer.Start(1f);
		}

		private void onTimeout()
		{
			events.EmitSignal(nameof(Events.Tick));

			//for each source find cell with best price, move products there
			var grid = GetNode<Grid>("Grid");
			mover.MoveProducts(grid.Cells);
			
		}
	}
}
