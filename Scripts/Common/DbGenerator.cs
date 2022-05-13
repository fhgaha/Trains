using System.Collections.Generic;
using System.IO;
using Godot;
using Newtonsoft.Json;

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
					string json = GenerateJSONFromCell(i, j);
					jsonList.Add(json);
				}
			var content = JsonConvert.SerializeObject(jsonList);

			System.IO.File.WriteAllText(path, content);
		}

		private static string GenerateJSONFromCell(int row, int col)
		{
			var cellForJSON = new CellForJSON
			{
				Id = row + "_" + col,
				Demand = new Dictionary<Enums.ProductType, float>
				{
					[Enums.ProductType.Lumber] = 50,
					[Enums.ProductType.Grain] = 50,
					[Enums.ProductType.Dairy] = 50
				}
			};

			string json = JsonConvert.SerializeObject(cellForJSON);
			return json;
		}

		class CellForJSON
		{
			public string Id;
			public Dictionary<Enums.ProductType, float> Demand;
		}
	}
}
