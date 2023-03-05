using System;
using Godot;
using Trains.Model.Builders;
using Trains.Model.Common;
using Trains.Model.Grids;
using Trains.Model.Migration;
using Trains.Scripts.GUI;
using static Trains.Model.Common.Enums;

namespace Trains.Scripts
{
	public class Main : Spatial
	{
		private const float TimeSec = 0.1f;
		private readonly PackedScene consoleScene = GD.Load<PackedScene>("res://Scenes/GUI/Cosnole/Console.tscn");
		private Timer timer;
		private Events events;
		private Camera camera;
		private ProductMigrationManager mover;
		private Grid grid;

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

			SetUpTimer();
			camera = GetNode<Camera>("MainCameraController/Elevation/Camera");
			grid = GetNode<Grid>("Grid");
			// mover = new ProductMigrationManager(grid.Cells);
			// mover.Init(grid.Cells);

			Global.StationContainer = GetNode<StationContainer>("Stations");
			Global.VisibleRailContainer = GetNode<VisibleRailContainer>("VisibleRails");
			Global.SplittedRailContainer = GetNode<SplittedRailsContainer>("SplittedRails");
			Global.ActualRailsContainer = GetNode<ActualRailsContainer>("ActualRails");

			//init station builder
			// stationBuilder = GetNode<StationBuilder>("StationBuilder");
			// stationBuilder.Init(grid.CellList, camera);

			//init rail builder
			// railBuilder = GetNode<RailBuilder>("RailBuilder");
			// railBuilder.Init(grid.CellList, camera);

			GetNode<DebugInfo>("GUI/CanvasLayer/DebugInfo").Init(grid, camera);
			SetUpFloorSize();

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));
		}

		private void SetUpFloorSize()
		{
			var floor = GetNode<MeshInstance>("Landmarks/Floor");
			//why x/2 and z/2?
			floor.Scale = new Vector3(grid.CellsRowsAmount / 2, 0, grid.CellsColsAmount / 2);
			floor.Translation = new Vector3(grid.CellsRowsAmount / 2, 0, grid.CellsColsAmount / 2);
		}

		private void SetUpTimer()
		{
			timer = GetNode<Timer>("MainTimer");
			timer.Connect("timeout", this, nameof(onTimeout));
			timer.Start(TimeSec);
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
			// mover.MoveProducts();
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			GetNode<MeshInstance>("Landmarks/Floor").Visible = mode != MainButtonType.ShowProductMap;
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
