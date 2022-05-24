using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Generators;

namespace Trains.Model.Grids
{
	public class Grid : Spatial
	{
		public int CellsRowsAmount {get; set;} = 10;
		public int CellsColsAmount {get; set;} = 10;
		public Cell[,] Cells;
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
		PackedScene source = GD.Load<PackedScene>("res://Scenes/Buildings/Source.tscn");
		private int cellsColsAmount = 10;
		private int cellsRowsAmount = 10;
		private Events events;

		//set cell size in editor: Cell/MeshInstance/Mesh/Size

		public override void _Ready()
		{
			Cells = CellGenerator.Generate(this, CellsRowsAmount, CellsColsAmount, cellScene);
			//Build();
			Cells[0, 2].AddBuilding(source, Enums.ProductType.Lumber, 20f);


			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButtonPressed));
			events.Connect(nameof(Events.AllProductButtonPressed), this, nameof(onAllProductsButtonPressed));

			//set value for a cell
			//Cells[0, 0].SetPrice(Enums.ProductType.Lumber, 400f);
		}

		private void Build()
		{
			//build from db
			for (int i = 0; i < CellsRowsAmount; i++)
			for (int j = 0; j < CellsColsAmount; j++)
				AddChild(Cells[i, j]);
		}

		public void onSpecificProductButtonPressed(Enums.ProductType productType)
		{
			DisplayProductDataAll(productType);
		}

		private void DisplayProductDataAll(Enums.ProductType productType)
		{
			for (int i = 0; i < Cells.GetLength(0); i++)
			for (int j = 0; j < Cells.GetLength(1); j++)
				Cells[i, j].DisplayProductData(productType);
		}
		
		private void onAllProductsButtonPressed()
		{
			//should show only amount
			HideProductDataAll();
		}

		private void HideProductDataAll()
		{
			for (int i = 0; i < Cells.GetLength(0); i++)
			for (int j = 0; j < Cells.GetLength(1); j++)
				Cells[i, j].HideProductData();
		}
	}
}
