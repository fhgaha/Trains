using System.Collections.Generic;
using Godot;
using Trains.Model.Cells;
using Newtonsoft.Json;
using Trains.Model.Products;

namespace Trains.Model.Common
{
	public class DbGenerator : Node
	{
		// public static void GenerateProductsDb(int rows, int cols)
		// {
		// 	string path = @"Databases\products.json";
		// 	List<string> jsonList = new List<string>();

		// 	for (int i = 0; i < rows; i++)
		// 		for (int j = 0; j < cols; j++)
		// 		{
		// 			string json = GenerateCellJSON(i, j);
		// 			jsonList.Add(json);
		// 		}
				
		// 	var content = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
		// 	System.IO.File.WriteAllText(path, content);
		// }

		// private static string GenerateCellJSON(int row, int col)
		// {
		// 	Cell cell = new Cell();
		// 	cell.Init(row, col);

		// 	CellForJson cellForJson = new CellForJson{Id = cell.Id, Products = cell.Products};

		// 	string json = JsonConvert.SerializeObject(cellForJson, Formatting.Indented);
		// 	return json;
		// }

		// public class CellForJson
		// {
		// 	public string Id {get; set;}
		// 	public List<Product> Products {get; set;}
		// }
	}
}
