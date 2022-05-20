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
		public Cell[,] Cells;
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");
		private int cellsColsAmount = 10;
		private int cellsRowsAmount = 10;
		private Events events;

		public override void _Ready()
		{
			Update();
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.LumberButtonPressed), this, nameof(onLumberButtonPressed));

			//set value for a cell
			//Cells[0, 0].SetPrice(Enums.ProductType.Lumber, 400f);
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

		public void onLumberButtonPressed()
		{
			ShowLumber();
		}

		private void ShowLumber()
		{
			for (int i = 0; i < Cells.GetLength(0); i++)
			for (int j = 0; j < Cells.GetLength(1); j++)
			{
				Cells[i, j].ShowLumber();
			}
		}
	}
}
