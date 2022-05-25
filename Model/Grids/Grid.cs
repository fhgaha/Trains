using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Generators;
using Trains.Scripts.CellScene;

namespace Trains.Model.Grids
{
	public class Grid : Spatial
	{
		public int CellsRowsAmount {get; set;} = 10;
		public int CellsColsAmount {get; set;} = 10;
		public Cell[,] Cells;
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
		PackedScene source = GD.Load<PackedScene>("res://Scenes/Buildings/Source.tscn");
		PackedScene stock = GD.Load<PackedScene>("res://Scenes/Buildings/Stock.tscn");
		private Events events;

		//set cell size in editor: Cell/MeshInstance/Mesh/Size

		public override void _Ready()
		{
			Cells = CellGenerator.Generate(this, CellsRowsAmount, CellsColsAmount, cellScene);
			Cells[0, 1].AddBuilding(source, Enums.ProductType.Lumber, 20f);
			Cells[3, 2].AddBuilding(stock, Enums.ProductType.Lumber, 3f);

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButtonPressed));
			events.Connect(nameof(Events.AllProductButtonPressed), this, nameof(onAllProductsButtonPressed));
		}

		public void onSpecificProductButtonPressed(Enums.ProductType productType)
		{
			DisplayProductDataAll(productType);
		}

		//show amount, prices, colors for selecte product
		private void DisplayProductDataAll(Enums.ProductType productType)
		{
			for (int i = 0; i < Cells.GetLength(0); i++)
			for (int j = 0; j < Cells.GetLength(1); j++)
				Cells[i, j].DisplayProductData(productType);
		}
		
		private void onAllProductsButtonPressed()
		{
			HideProductDataAll();
		}

		//should show sum amount of all products and no color, no prices
		private void HideProductDataAll()
		{
			for (int i = 0; i < Cells.GetLength(0); i++)
			for (int j = 0; j < Cells.GetLength(1); j++)
				Cells[i, j].DisplayProductDataAllProductsMode();
		}
	}
}
