using System.Linq;
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

			// var noise = new OpenSimplexNoise();
			// for (int i = 0; i < Cells.GetLength(0); i++)
			// for (int j = 0; j < Cells.GetLength(1); j++)
			// {
			// 	Cells[i, j].Products[0].Price = noise.GetNoise2d(i, j) * 50 + 50;
			// }

			//set value for each cell
			var _productLumber = Cells[0, 0].Products.First(p => p.ProductType == Enums.ProductType.Lumber);
			_productLumber.Price = 400f;
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
