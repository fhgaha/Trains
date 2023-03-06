using System;
using System.Collections.Generic;
using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Generators;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Grids
{
	public class Grid : MultiMeshInstance
	{
		public int CellsRowsAmount { get; set; } = 20;
		public int CellsColsAmount { get; set; } = 30;
		public Cell[,] Cells;
		public List<Cell> CellList { get; private set; }
		// private readonly PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
		private readonly PackedScene building = GD.Load<PackedScene>("res://Scenes/Buildings/Building.tscn");
		// PackedScene source = GD.Load<PackedScene>("res://Scenes/Buildings/Source.tscn");
		// PackedScene stock = GD.Load<PackedScene>("res://Scenes/Buildings/Stock.tscn");
		private Events events;

		//set cell size in editor: Cell/MeshInstance/Mesh/Size

		private readonly Random rnd = new Random();

		public override void _Ready()
		{
			Cells = CellGenerator.GenerateNoCellScenes(CellsRowsAmount, CellsColsAmount);
			FillCellList();
			//AddBuildings();
			SetUpMultimesh();

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));
			events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButtonPressed));
			events.Connect(nameof(Events.AllProductButtonPressed), this, nameof(onAllProductsButtonPressed));

			Visible = false;
		}

		private void SetUpMultimesh()
		{
			var onePlane = new PlaneMesh
			{
				Size = new Vector2(1, 1),
				Material = new SpatialMaterial { VertexColorUseAsAlbedo = true },
				CenterOffset = new Vector3(0.5f, 0, 0.5f)
			};

			Multimesh = new MultiMesh
			{
				TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
				ColorFormat = MultiMesh.ColorFormatEnum.Color8bit,
				CustomDataFormat = MultiMesh.CustomDataFormatEnum.None,
				InstanceCount = CellsColsAmount * CellsColsAmount,
				VisibleInstanceCount = -1,
				Mesh = onePlane
			};

			for (int z = 0; z < CellsColsAmount; z++)
				for (int x = 0; x < CellsRowsAmount; x++)
				{
					var index = x + (z * CellsColsAmount);
					Multimesh.SetInstanceTransform(index, new Transform(Basis.Identity, new Vector3(x, 0, z)));
					var color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
					Multimesh.SetInstanceColor(index, color);
				}

			//later colors can be set like that
			// Multimesh.SetInstanceColor(0, Colors.Purple);
			// Multimesh.SetInstanceColor(1, Colors.Purple);
			// Multimesh.SetInstanceColor(2, Colors.Purple);
		}

		private void FillCellList()
		{
			CellList = new List<Cell>();
			for (int i = 0; i < Cells.GetLength(0); i++)
				for (int j = 0; j < Cells.GetLength(1); j++)
					CellList.Add(Cells[i, j]);
		}

		private void AddBuildings()
		{
			try
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
			catch (IndexOutOfRangeException e)
			{
				GD.PrintErr(e);
				throw;
			}
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			//other button is pressed
			Visible = mode == MainButtonType.ShowProductMap;
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
