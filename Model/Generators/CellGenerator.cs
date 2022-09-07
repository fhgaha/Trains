using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Trains.Model.Cells;
using Trains.Model.Generators.Noises;
using Trains.Model.Grids;
using Trains.Model.Products;
using Trains.Scripts.CellScene;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Generators
{
	public class CellGenerator : Node
	{
		//generate cells, smothify, return to Grid.cs and generate db
		internal static Cell[,] Generate(Spatial grid, int rows, int cols, PackedScene cellScene)
		{
			var noises = new Dictionary<ProductType, OpenSimplexNoise>
			{
				[ProductType.Lumber] = new LumberNoise(),
				[ProductType.Grain] = new GrainNoise(),
				[ProductType.Dairy] = new DairyNoise()
			};

			Cell[,] cells = new Cell[rows, cols];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					var cell = cellScene.Instance<Cell>();
					grid.AddChild(cell);
					cell.Init(i, j, noises);
					cell.Name = "Cell_" + cell.Id;  //unique name in tree
					cell.Translate(new Vector3(i * cell.Size, 0, j * cell.Size));
					cells[i, j] = cell;
				}
			}

			Cell.Cells = cells;

			return cells;
		}
	}
}