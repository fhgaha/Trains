using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Products
{

	public class Product : Node
	{
		public delegate void PriceChanged(float value);
		public event PriceChanged PriceChangedEvent;
		
		
		[JsonConverter(typeof(StringEnumConverter))]
		public ProductType ProductType { get; private set; }

		public float Price
		{
			get => price;
			set
			{
				price = value;
				PriceChangedEvent?.Invoke(value);
			}
		}

		public float Amount { get; set; }

		private float price;

		public Product(ProductType type, float price)
		{
			ProductType = type;
			this.Price = price;
		}

		// public static List<Product> BuildList()
		// {
		// 	return new List<Product>
		// 	{
		// 		new Product(ProductType.Lumber, 20),
		// 		new Product(ProductType.Grain, 30),
		// 		new Product(ProductType.Dairy, 50)
		// 	};
		// }

		// public override string ToString()
		// {
		// 	return "{Product{"
		// 		+ "ProductType: " + productType.ToString()
		// 		+ ", Price: " + Price
		// 		+ "}}";
		// }
	}
}