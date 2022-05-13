using Godot;
using Trains.Scripts.Common;
using Trains.Scripts.Cells;

namespace Trains.Scripts
{
	public class Grid : Spatial
	{
		[Export]
		public int CellsRowsAmount { get; set; } = 2;
		[Export]
		public int CellsColsAmount { get; set; } = 2;
		public Cell[,] Cells;
		private Timer timer;
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");

		public override void _Ready()
		{
			Cells = new Cell[CellsRowsAmount, CellsColsAmount];
			timer = new Timer();
			AddChild(timer);
			DbGenerator.GenerateProductsDb(CellsRowsAmount, CellsColsAmount);
			Build();
			timer.Start(1.0f);
		}

		public override void _Process(float delta) { }

		private void Build()
		{
			//build from db
			for (int i = 0; i < CellsRowsAmount; i++)
				for (int j = 0; j < CellsColsAmount; j++)
				{
					var cell = cellScene.Instance<Cell>();
					cell.Id = i + "_" + j;
					cell.Translate(new Vector3(i * cell.Size, 0, j * cell.Size));
					timer.Connect("timeout", cell, "SetColor");
					Cells[i, j] = cell;

					AddChild(cell);
				}
		}
	}
}
