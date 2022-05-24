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

		//Used by res://Scenes/Buildings/ProductAmountBar.tscn to set TextureProgress bar value
		[Signal] public delegate void AmountChanged(ProductType productType, float value);

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
		public float Amount
		{
			get => amount;
			set
			{
				amount = value;
				EmitSignal(nameof(AmountChanged), ProductType, value);
			}
		}

		private float price;
		private float amount;

		public Product(ProductType type, float price)
		{
			ProductType = type;
			this.Price = price;
		}
	}
}