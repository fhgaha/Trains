using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using Trains.Scripts.Common;
using static Trains.Scripts.Common.Enums;

namespace Trains.Scripts.Products
{
	public struct Product
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ProductType productType { get; set; }
		public float Price { get; set; }

		//list of factories

		public static List<Product> GetBuildList()
		{
			return new List<Product>
			{
				new Product{productType = Enums.ProductType.Lumber, Price = 20},
				new Product{productType = Enums.ProductType.Grain, Price = 30},
				new Product{productType = Enums.ProductType.Dairy, Price = 50}
			};
		}

		// public override string ToString()
		// {
		// 	return "{Product{"
		// 		+ "ProductType: " + productType.ToString()
		// 		+ ", Price: " + Price
		// 		+ "}}";
		// }
	}
}