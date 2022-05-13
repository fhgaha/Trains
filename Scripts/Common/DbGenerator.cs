using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Trains.Scripts.Common
{
	public class DbGenerator : Node
	{
		public static void GenerateProducts(Cell[,] cells)
		{
			Godot.File file = new Godot.File();

			//if no error return
			//file.Open("/Databases/products.json", File.ModeFlags.Read)

			file.Open("res://Databases/products.json", Godot.File.ModeFlags.Write);

			List<string> content = new List<string>();

			foreach (Cell cell in cells)
			{
				string json = GenerateJSONFromCell(cell);
				content.Add(json);
			}

			var con = JsonConvert.SerializeObject(content);

			file.StoreString(con);
			file.Close();
		}

		private static string GenerateJSONFromCell(Cell cell)
		{
			var cellForJSON = new CellForJSON
			{
				Id = cell.Id, 
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