using System.Collections.Generic;
using System.IO;
using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trains.Scripts.Cells;
using Newtonsoft.Json;
using Trains.Scripts.Products;

namespace Trains.Scripts.Common
{
	public class DbGenerator : Node
	{
		public static void GenerateProductsDb(int rows, int cols)
		{
			string path = @"Databases\products.json";
			
			// if (System.IO.File.Exists(path) && new FileInfo(path).Length != 0) return;

			List<string> jsonList = new List<string>();

			for (int i = 0; i < rows; i++)
				for (int j = 0; j < cols; j++)
				{
					string json = GenerateCellJSON(i, j);
					jsonList.Add(json);
				}
				
			var content = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
			//var content = JsonSerializer.Serialize(jsonList);

			System.IO.File.WriteAllText(path, content);
		}

		private static string GenerateCellJSON(int row, int col)
		{
			// var cellForJSON = new CellJsonHelper
			// {
			// 	Id = row + "_" + col,
			// 	Prices = new Dictionary<Enums.ProductType, float>[3]
			// };

			// cellForJSON.Prices[0] = new Dictionary<Enums.ProductType, float>{[Enums.ProductType.Lumber] = 50f};
			// cellForJSON.Prices[1] = new Dictionary<Enums.ProductType, float>{[Enums.ProductType.Grain] = 50f};
			// cellForJSON.Prices[2] = new Dictionary<Enums.ProductType, float>{[Enums.ProductType.Dairy] = 50f};

			Cell cell = new Cell{Id = row + "_" + col};
			cell.Init();

			CellForJson cellForJson = new CellForJson{Id = cell.Id, Products = cell.Products};

			string json = JsonConvert.SerializeObject(cellForJson, Formatting.Indented);
			//var options = new JsonSerializerOptions { WriteIndented = true };
			//string json = JsonSerializer.Serialize(cell, options);
			return json;
		}

		public class CellForJson
		{
			public string Id {get; set;}
			//public Dictionary<Enums.ProductType, float>[] Prices {get; set;}
			public List<Product> Products {get; set;}
		}

		
	}

	
}
