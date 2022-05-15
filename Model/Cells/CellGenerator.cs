using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using Trains.Model.Products;
using static Trains.Model.Common.DbGenerator;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells
{
	public class CellGenerator : Node
	{
		internal static Cell[,] Generate()
		{
			string path = @"Databases\products.json";
			var source = System.IO.File.ReadAllText(path);
			var lines = JsonConvert.DeserializeObject<List<string>>(source);

			List<ListTo2dArrayHelper> elements = new List<ListTo2dArrayHelper>();

			int maxRow = 0; int maxCol = 0;

			FillElements(lines, elements, ref maxRow, ref maxCol);

			Cell[,] cells = new Cell[maxRow + 1, maxCol + 1];

			foreach (ListTo2dArrayHelper element in elements)
				cells[element.Row, element.Col] = element.Cell;

			return cells;
		}

		private static void FillElements(List<string> lines, List<ListTo2dArrayHelper> elements,
			ref int maxRow, ref int maxCol)
		{
			foreach (string line in lines)
			{
				CellForJson cellForJson = JsonConvert.DeserializeObject<CellForJson>(line);
				Cell cell = new Cell() { Id = cellForJson.Id, Products = cellForJson.Products };

				int row = int.Parse(cellForJson.Id.Split("_")[0]);
				int col = int.Parse(cellForJson.Id.Split("_")[1]);

				if (row > maxRow) maxRow = row;
				if (col > maxCol) maxCol = col;

				ListTo2dArrayHelper element = new ListTo2dArrayHelper { Row = row, Col = col, Cell = cell };
				elements.Add(element);
			}
		}

		class ListTo2dArrayHelper
		{
			public int Row;
			public int Col;
			public Cell Cell;
		}

		//generate cells, smothify, return to Grid.cs and generate db
		internal static Cell[,] Generate(int rows, int cols)
		{
			var rng = new RandomNumberGenerator();	
			Cell[,] cells = new Cell[rows, cols];

			for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
			{
				Cell cell = new Cell{Id = i + "_" + j};
				cell.Init();

				//set spike price for a random cell

				cells[i, j] = cell;
			}

			//temporarily set some values for cells			
			var product = new Product(Enums.ProductType.Lumber, 400f);
			cells[0, 0].Products[0] = product;

			SmothifyPrices(cells);

			return cells;
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