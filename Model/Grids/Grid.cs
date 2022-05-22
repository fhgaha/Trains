using System;
using System.Linq;
using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Generators;

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

		public static Cell[,] Cells;
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
		private int cellsColsAmount = 10;
		private int cellsRowsAmount = 10;
		private Events events;

		//set cell size in editor: Cell/MeshInstance/Mesh/Size

		public override void _Ready()
		{
			Update();
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButton));

			//set value for a cell
			//Cells[0, 0].SetPrice(Enums.ProductType.Lumber, 400f);
		}

		public void Update()
		{
			// Code to execute in editor.
			if (Engine.EditorHint)
			{
				if (GetChildCount() > 0) return;

				Cells = CellGenerator.Generate(CellsRowsAmount, CellsColsAmount, cellScene);
				Build();
			}

			// Code to execute in game.
			if (!Engine.EditorHint)
			{
				var children = GetChildren().Cast<Cell>();
				Cells = CellGenerator.GetFromCollection(CellsRowsAmount, CellsColsAmount, children);
			}

			// Code to execute both in editor and in game.
		}

		private void Build()
		{
			//build from db
			for (int i = 0; i < CellsRowsAmount; i++)
				for (int j = 0; j < CellsColsAmount; j++)
				{
					AddChild(Cells[i, j]);
					Cells[i, j].Owner = GetTree().EditedSceneRoot;
				}

			if (GetChildCount() > CellsRowsAmount * CellsColsAmount) throw new Exception("too many cells");
		}

		public void onSpecificProductButton(Enums.ProductType productType)
		{
			DisplayProductDataAll(productType);
		}

		private void DisplayProductDataAll(Enums.ProductType productType)
		{
			for (int i = 0; i < Cells.GetLength(0); i++)
				for (int j = 0; j < Cells.GetLength(1); j++)
				{
					Cells[i, j].DisplayProductData(productType);
				}
		}
	}
}
