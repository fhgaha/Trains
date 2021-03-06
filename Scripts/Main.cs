using Godot;
using Trains.Model.Builders;
using Trains.Model.Common;
using Trains.Model.Grids;
using Trains.Model.Migration;

namespace Trains.Scripts
{
	public class Main : Spatial
	{
		private const float timeSec = 0.1f;
		private PackedScene consoleScene = GD.Load<PackedScene>("res://Scenes/GUI/Cosnole/Console.tscn");
		private Timer timer;
		private Events events;
		private Camera camera;
		private ProductMigrationManager mover;

		private StationBuilder stationBuilder;
		private RailBuilder railBuilder;

		public override void _EnterTree()
		{
			//do this here since it doesnt work if set it in editor
			OS.SetWindowAlwaysOnTop(true);
		}

		public override void _Ready()
		{
			FloatDisplayDotsInsteadOfCommas();

			events = GetNode<Events>("/root/Events");

			mover = new ProductMigrationManager();
			timer = GetNode<Timer>("MainTimer");
			timer.Connect("timeout", this, nameof(onTimeout));
			timer.Start(timeSec);
			camera = GetNode<Camera>("MainCameraController/Elevation/Camera");

			//init station builder
			var cells = GetNode<Grid>("Grid").CellList;
			var scene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");

			stationBuilder = GetNode<StationBuilder>("StationBuilder");
			stationBuilder.Init(cells, camera, GetNode<Spatial>("Stations"), GetNode<RailContainer>("Rails"));

			//init rail builder
			scene = GD.Load<PackedScene>("res://Scenes/Rails/RailPath.tscn");
			railBuilder = GetNode<RailBuilder>("RailBuilder");
			railBuilder.Init(cells, camera, GetNode<RailContainer>("Rails"), scene);
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
