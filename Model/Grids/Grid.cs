using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;

namespace Trains.Model.Grids
{
	[Tool]
	public class Grid : Spatial
	{
		[Export]
		public int CellsRowsAmount
		{
			get => cellsRowsAmount;
			set
			{
				cellsRowsAmount = value;
				Update();
			}
		}
		[Export]
		public int CellsColsAmount
		{
			get => cellsColsAmount;
			set
			{
				cellsColsAmount = value;
				Update();
			}
		}
		public Cell[,] Cells;
		private Timer timer;
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
		private int cellsRowsAmount = 1;
		private int cellsColsAmount = 1;

		public override void _Ready()
		{
			//generate db then parse cells from db
			timer = new Timer();
			AddChild(timer);
			Update();
			timer.Start(1.0f);
		}

		public void Update()
		{
			this.RemoveAllChildren();
			DbGenerator.GenerateProductsDb(CellsRowsAmount, CellsColsAmount);
			Cells = CellGenerator.Generate();
			Build();
		}

		private void Build()
		{
			//build from db
			for (int i = 0; i < CellsRowsAmount; i++)
				for (int j = 0; j < CellsColsAmount; j++)
				{
					var cell = cellScene.Instance<Cell>();
					cell.Id = i + "_" + j;
					cell.Translate(new Vector3(i * cell.Size, 0, j * cell.Size));
					//timer.Connect("timeout", cell, "SetColor");
					Cells[i, j] = cell;

					AddChild(cell);
				}
		}
	}
}
