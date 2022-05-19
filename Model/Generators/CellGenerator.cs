using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Trains.Model.Cells;
using Trains.Model.Generators.Noises;
using Trains.Model.Products;
using Trains.Scripts.CellScene;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Generators
{
	public class CellGenerator : Node
	{
		// internal static Cell[,] Generate()
		// {
		// 	string path = @"Databases\products.json";
		// 	var source = System.IO.File.ReadAllText(path);
		// 	var lines = JsonConvert.DeserializeObject<List<string>>(source);

		// 	List<ListTo2dArrayHelper> elements = new List<ListTo2dArrayHelper>();

		// 	int maxRow = 0; int maxCol = 0;

		// 	FillElements(lines, elements, ref maxRow, ref maxCol);

		// 	Cell[,] cells = new Cell[maxRow + 1, maxCol + 1];

		// 	foreach (ListTo2dArrayHelper element in elements)
		// 		cells[element.Row, element.Col] = element.Cell;

		// 	return cells;
		// }

		// private static void FillElements(List<string> lines, List<ListTo2dArrayHelper> elements,
		// 	ref int maxRow, ref int maxCol)
		// {
		// 	foreach (string line in lines)
		// 	{
		// 		CellForJson cellForJson = JsonConvert.DeserializeObject<CellForJson>(line);
		// 		Cell cell = new Cell() { Id = cellForJson.Id, Products = cellForJson.Products };

		// 		int row = int.Parse(cellForJson.Id.Split("_")[0]);
		// 		int col = int.Parse(cellForJson.Id.Split("_")[1]);

		// 		if (row > maxRow) maxRow = row;
		// 		if (col > maxCol) maxCol = col;

		// 		ListTo2dArrayHelper element = new ListTo2dArrayHelper { Row = row, Col = col, Cell = cell };
		// 		elements.Add(element);
		// 	}
		// }

		class ListTo2dArrayHelper
		{
			public int Row;
			public int Col;
			public Cell Cell;
		}
		
		//generate cells, smothify, return to Grid.cs and generate db
		internal static Cell[,] Generate(int rows, int cols, PackedScene cellScene)
		{
			var noises = new Dictionary<ProductType, OpenSimplexNoise>
			{
				[ProductType.Lumber] = new LumberNoise(),
				[ProductType.Grain] = new GrainNoise(),
				[ProductType.Dairy] = new DairyNoise()
			};

			Cell[,] cells = new Cell[rows, cols];

			for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
			{
				var cell = cellScene.Instance<Cell>();
				cell.Init(i, j, noises);
				//cell.Init(i, j, lumberNoise.GetNoise2d(i, j) * 50 + 50);
				cell.Translate(new Vector3(i * cell.Size, 0, j * cell.Size));

				//cell.SetPrice(Enums.ProductType.Lumber, noise.GetNoise2d(i, j) * 50 + 50);
				
				LabelInit(cell);

				cells[i, j] = cell;
			}

			return cells;
		}

		private static void LabelInit(Cell cell)
		{
			var viewport = cell.GetNode<ViewportScript>("Sprite3D/Viewport");
			//temporary to set price to labels
			var lumber = cell.Products.GetChildren()[0] as Product;
			viewport.GetNode<Label>("Label").Text = lumber.Price.ToString();
			lumber.PriceChangedEvent += viewport.OnSetText;
		}

		private static void SmothifyPrices(Cell[,] cells)
		{
			for (int i = 0; i < cells.GetLength(0); i++)
			for (int j = 0; j < cells.GetLength(1); j++)
			{
				List<Cell> neighbours = GetNeighbours(cells, i, j);
				neighbours.Add(cells[i, j]);

				//average?

				neighbours.Clear();
			}
		}

		private static List<Cell> GetNeighbours(Cell[,] cells, int i, int j)
		{
			List<Cell> neighbours = new List<Cell>();

			for (var dy = -1; dy <= 1; dy++)
			for (var dx = -1; dx <= 1; dx++)
			{
				if (dx == 0 && dy == 0) continue;
				if (i + dx < 0 || j + dy < 0) continue;
				if (i + dx > cells.GetLength(0) - 1 || j + dy > cells.GetLength(1) - 1) continue;

				Cell neighbour = cells[i + dx, j + dy];
				neighbours.Add(neighbour);
			}

			return neighbours;
		}
	}
}