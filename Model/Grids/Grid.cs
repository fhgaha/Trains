using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;

namespace Trains.Model.Grids
{
	//[Tool]
	public class Grid : Spatial
	{
		private int cellsRowsAmount = 10;

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

		private int cellsColsAmount = 10;

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
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");


		public override void _Ready()
		{
			Update();
		}

		public void Update()
		{
			//generate db then parse cells from db
			//this.RemoveAllChildren();
			//DbGenerator.GenerateProductsDb(CellsRowsAmount, CellsColsAmount);
			Cells = CellGenerator.Generate(CellsRowsAmount, CellsColsAmount);

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
					Cells[i, j] = cell;
					cell.SetColor();

					AddChild(cell);
				}
		}
	}
}
