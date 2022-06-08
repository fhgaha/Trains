using Godot;
using Trains.Model.Builders;
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
		private StationBuilder stationBuilder;
		private RailBuilder railBuilder;

		public override void _Ready()
		{
			FloatDisplayDotsInsteadOfCommas();

			events = GetNode<Events>("/root/Events");
			//GD.Print("Main: " + events);
			mover = new ProductMigrationManager();
			var timer = GetNode<Timer>("MainTimer");
			timer.Connect("timeout", this, nameof(onTimeout));
			timer.Start(timeSec);

			//init station builder
			stationBuilder = new StationBuilder();
			AddChild(stationBuilder);
			stationBuilder.Name = "StationBuilder";
			var cells = GetNode<Grid>("Grid").CellList;
			var camera = GetNode<Camera>("MainCameraController/Elevation/Camera");
			var scene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");
			stationBuilder.Init(cells, camera, GetNode<Spatial>("Stations"));

			//init rail builder
			railBuilder = new RailBuilder();
			AddChild(railBuilder);
			railBuilder.Name = "RailBuilder";
			scene = GD.Load<PackedScene>("res://Scenes/Rails/Rail.tscn");
			railBuilder.Init(cells, camera, GetNode<Spatial>("Rails"), scene);
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

		private static void FloatDisplayDotsInsteadOfCommas()
		{
			System.Globalization.CultureInfo customCulture
				= (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
			customCulture.NumberFormat.NumberDecimalSeparator = ".";
			System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
		}
	}
}
