using System;
using System.Collections.Generic;
using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Generators;
using Trains.Scripts.CellScene;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Grids
{
	public class Grid : Spatial
	{
		public int CellsRowsAmount { get; set; } = 10;
		public int CellsColsAmount { get; set; } = 10;
		public Cell[,] Cells;
		public List<Cell> CellList { get; private set; }
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
		PackedScene building = GD.Load<PackedScene>("res://Scenes/Buildings/Building.tscn");
		// PackedScene source = GD.Load<PackedScene>("res://Scenes/Buildings/Source.tscn");
		// PackedScene stock = GD.Load<PackedScene>("res://Scenes/Buildings/Stock.tscn");
		private Events events;

		//set cell size in editor: Cell/MeshInstance/Mesh/Size

		public override void _Ready()
		{
			Cells = CellGenerator.Generate(this, CellsRowsAmount, CellsColsAmount, cellScene);
			CellList = new List<Cell>();
			for (int i = 0; i < Cells.GetLength(0); i++)
				for (int j = 0; j < Cells.GetLength(1); j++)
					CellList.Add(Cells[i, j]);

			AddBuildings();

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
			events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButtonPressed));
			events.Connect(nameof(Events.AllProductButtonPressed), this, nameof(onAllProductsButtonPressed));

			Visible = false;
		}

		private void AddBuildings()
		{
			//sources
			Cells[0, 0].AddBuilding(BuildingType.Source, building, ProductType.Lumber, 20f);
			Cells[3, 8].AddBuilding(BuildingType.Source, building, ProductType.Lumber, 5f);
			Cells[3, 3].AddBuilding(BuildingType.Source, building, ProductType.Grain, 5f);
			Cells[0, 4].AddBuilding(BuildingType.Source, building, ProductType.Grain, 5f);
			Cells[1, 2].AddBuilding(BuildingType.Source, building, ProductType.Grain, 5f);
			Cells[6, 1].AddBuilding(BuildingType.Source, building, ProductType.Dairy, 5f);
			Cells[9, 3].AddBuilding(BuildingType.Source, building, ProductType.Dairy, 5f);

			//stocks
			Cells[5, 5].AddBuilding(BuildingType.Stock, building, ProductType.Lumber, 0f);
			Cells[8, 4].AddBuilding(BuildingType.Stock, building, ProductType.Lumber, 0f);
			Cells[7, 5].AddBuilding(BuildingType.Stock, building, ProductType.Grain, 0f);
			Cells[0, 6].AddBuilding(BuildingType.Stock, building, ProductType.Dairy, 0f);

			//both

		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			//other button is pressed
			if (buttonType != MainButtonType.ShowProductMap)
			{
				Visible = false;
				return;
			}

			//"Show Product Menu" button was pressed and we press it again
			if (Global.MainButtonMode is MainButtonType.ShowProductMap)
			{
				Global.MainButtonMode = null;
				Visible = false;
				return;
			}

			Global.MainButtonMode = MainButtonType.ShowProductMap;
			Visible = true;
		}
		public void onSpecificProductButtonPressed(ProductType productType)
		{
			Global.CurrentDisplayProductMode = productType;
			DisplayProductDataAll(productType);
		}

		//show amount, prices, colors for selecte product
		private void DisplayProductDataAll(ProductType productType)
		{
			for (int i = 0; i < Cells.GetLength(0); i++)
				for (int j = 0; j < Cells.GetLength(1); j++)
					Cells[i, j].DisplayProductData(productType);
		}

		private void onAllProductsButtonPressed()
		{
			Global.CurrentDisplayProductMode = null;
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
