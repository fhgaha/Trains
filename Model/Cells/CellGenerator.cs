using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;
using static Trains.Model.Common.DbGenerator;

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
	}
}