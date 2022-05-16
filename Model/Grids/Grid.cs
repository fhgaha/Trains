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
		PackedScene cellScene;


		public override void _Ready()
		{
			cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
			Update();
		}

		public void Update()
		{
			//generate db then parse cells from db
			Cells = CellGenerator.Generate(CellsRowsAmount, CellsColsAmount, cellScene);
			Build();
		}

		private void Build()
		{
			//build from db
			for (int i = 0; i < CellsRowsAmount; i++)
				for (int j = 0; j < CellsColsAmount; j++)
					AddChild(Cells[i, j]);
		}
	}
}
