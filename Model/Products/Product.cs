using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Products
{
	public class Product : Node
	{
		//used by Cell/Sprite3D/Viewport to set label text
		//used by Cell/MeshInstance to set cell color 
		[Signal] public delegate void PriceChanged(float value);

		//[JsonConverter(typeof(StringEnumConverter))]
		[Export(PropertyHint.Enum)]
		public ProductType ProductType { get; private set; }

		[Export]
		public float Price
		{
			get => price;
			set
			{
				price = value;
				EmitSignal(nameof(PriceChanged), value);
			}
		}

		[Export]
		public float Amount { get; set; }

		private float price;

		public Product(ProductType type, float price)
		{
			ProductType = type;
			this.Price = price;
		}
	}
}