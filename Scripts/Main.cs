using Godot;
using Trains.Model.Common;
using Trains.Model.Grids;
using Trains.Model.Migration;

namespace Trains.Scripts
{
	public class Main : Spatial
	{
		private Events events;
		private ProductMigrationManager mover;
		private float timeSec = 0.1f;
		private PackedScene consoleScene = GD.Load<PackedScene>("res://Scenes/GUI/Cosnole/Console.tscn");

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			mover = new ProductMigrationManager();
			var timer = GetNode<Timer>("MainTimer");
			timer.Connect("timeout", this, nameof(onTimeout));
			timer.Start(timeSec);
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			//open console
			if (@event.IsActionPressed("toggle_console"))
			{
				AddChild(consoleScene.Instance());
				//freeze game so camera wont move while typing
				//bad solution i want the game running when console is open
				GetTree().Paused = true;	
			}
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
