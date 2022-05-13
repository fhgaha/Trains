using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Trains.Scripts.Common
{
	public class DbGenerator : Node
	{
		public static void GenerateProductsDb(int rows, int cols)
		{
			Godot.File file = new Godot.File();

			//if no error return
			//file.Open("/Databases/products.json", File.ModeFlags.Read)

			file.Open("res://Databases/products.json", Godot.File.ModeFlags.Write);

			List<string> text = new List<string>();

			for (int i = 0; i < rows; i++)
				for (int j = 0; j < cols; j++)
				{
					string json = GenerateJSONFromCell(i, j);
					text.Add(json);
				}
			var content = JsonConvert.SerializeObject(text);

			file.StoreString(content);
			file.Close();
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
