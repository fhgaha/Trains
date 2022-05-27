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
		PackedScene building = GD.Load<PackedScene>("res://Scenes/Buildings/Building.tscn");
		// PackedScene source = GD.Load<PackedScene>("res://Scenes/Buildings/Source.tscn");
		// PackedScene stock = GD.Load<PackedScene>("res://Scenes/Buildings/Stock.tscn");
		private Events events;

		//set cell size in editor: Cell/MeshInstance/Mesh/Size

		public override void _Ready()
		{
			Cells = CellGenerator.Generate(this, CellsRowsAmount, CellsColsAmount, cellScene);
			AddBuildings();

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButtonPressed));
			events.Connect(nameof(Events.AllProductButtonPressed), this, nameof(onAllProductsButtonPressed));
		}

		private void AddBuildings()
		{
			Cells[0, 1].AddBuilding(Enums.BuildingType.Source, building, Enums.ProductType.Lumber, 20f);
			Cells[8, 4].AddBuilding(Enums.BuildingType.Stock, building, Enums.ProductType.Lumber, 3f);

			//Cells[0, 1].AddBuilding(Enums.BuildingType.Source, building, Enums.ProductType.Lumber, 20f);
			Cells[5, 5].AddBuilding(Enums.BuildingType.Stock, building, Enums.ProductType.Lumber, 3f);
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
