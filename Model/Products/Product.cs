using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Products
{
	public struct Product
	{

		[JsonConverter(typeof(StringEnumConverter))]
		public ProductType productType { get; private set; }
		private float price;

		public float GetPrice() => price;
		public void SetPrice(float value) => price = value;

		//list of factories

		public Product(ProductType type, float price)
		{
			productType = type;
			this.price = price;
		}

		public static List<Product> GetBuildList()
		{
			return new List<Product>
			{
				new Product{productType = ProductType.Lumber, price = 20},
				new Product{productType = ProductType.Grain, price = 30},
				new Product{productType = ProductType.Dairy, price = 50}
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