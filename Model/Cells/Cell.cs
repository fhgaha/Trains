using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;
using System.Collections.Generic;
using Trains.Model.Generators.Noises;
using Trains.Scripts.CellScene;

namespace Trains.Model.Cells
{
	[Tool]
	public class Cell : Spatial
	{
		public string Id { get; set; }

		public int Size { get; } = 1;

		public Node Products { get; set; }

		public float GetPrice(ProductType type)
		{
			foreach (Product p in Products.GetChildren())
				if (p.ProductType == type)
					return p.Price;
			return -1;
		}

		public void SetPrice(ProductType type, float price)
		{
			foreach (Product p in Products.GetChildren())
				if (p.ProductType == type)
					p.Price = price;
		}

		public override void _Ready() { }

		public override void _Process(float delta) { }

		public void Init(int row, int col, Dictionary<ProductType, OpenSimplexNoise> noises)
		{
			if (!string.IsNullOrEmpty(Id)) throw new ArgumentException("You allowed to set Id only once");
			Id = row + "_" + col;
			Products = new Node();

			foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
			{
				Product product = new Product(type, -1);
				Products.AddChild(product);
				product.Price = GetPriceFromNoise(row, col, noises, product.ProductType);;
			}

			//If I want to build cell grid in editor I have to comment this. There will be no price numbers and no colors.
			//should subscribe and unsubscribe to these events when sertain product button is pressed
			// Product lumber = Products.GetChild<Product>(0);
			// lumber.PriceChangedEvent += GetNode<ViewportScript>("Sprite3D/Viewport").OnSetText;
			// lumber.PriceChangedEvent += GetNode<MeshInstanceScript>("MeshInstance").SetColor;
		}

		private static float GetPriceFromNoise(int row, int col, Dictionary<ProductType, OpenSimplexNoise> noises, ProductType productType)
		{
			Type noiseType = GetNoiseType(productType);
			var noise = noises[productType];
			object[] getMyNoiseArguments = new object[] { row, col, 100f };

			var price = (float)noiseType.GetMethod("GetMyNoise").Invoke(noise, getMyNoiseArguments);
			return price;
			
			Type GetNoiseType(ProductType productType_)
			{
				switch (productType_)
				{
					case ProductType.Lumber: return typeof(LumberNoise);
					case ProductType.Grain: return typeof(GrainNoise);
					case ProductType.Dairy: return typeof(DairyNoise);
				}

				return null;
			}
		}
		
		internal void ShowLumber()
		{
			//show price and color then subscribe in case value changes
			Product lumber = null;

			foreach (Product p in Products.GetChildren())
				if (p.ProductType == ProductType.Lumber) lumber = p;

			if (lumber == null) throw new Exception("Products do not contain lumber");

			// var viewport = GetNode<ViewportScript>("Sprite3D/Viewport");
			// viewport.OnSetText(lumber.Price);
			// var label = viewport.GetNode<Label>("Label");
			// //viewport.GetNode<Label>("Label").Text = lumber.Price.ToString();
			// GetNode<MeshInstanceScript>("MeshInstance").SetColor(lumber.Price);

			// lumber.PriceChangedEvent += GetNode<ViewportScript>("Sprite3D/Viewport").OnSetText;
			// lumber.PriceChangedEvent += GetNode<MeshInstanceScript>("MeshInstance").SetColor;

			lumber.PriceChangedEvent += GetNode<ViewportScript>("Sprite3D/Viewport").OnSetText;
			lumber.PriceChangedEvent += GetNode<MeshInstanceScript>("MeshInstance").SetColor;

			lumber.Price = lumber.Price;
			//lumber.EmitSignal(nameof(Product.PriceChanged), lumber.Price);

		}
	}
}