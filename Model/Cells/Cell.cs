using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;
using System.Collections.Generic;
using Trains.Model.Generators.Noises;
using Trains.Scripts.CellScene;
using Trains.Model.Cells.Buildings;

namespace Trains.Model.Cells
{
	[Tool]
	public class Cell : Spatial
	{
		[Export] public string Id { get; set; }
		public Node Products { get; set; }
		public IBuilding Factory { get; set; }

		public int Size { get; } = 1;

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

		public Product GetProduct(ProductType productType)
		{
			foreach (Product p in Products.GetChildren())
				if (p.ProductType == productType) return p;
			throw new Exception("Products do not contain " + productType);
		}

		public void Init(int row, int col, Dictionary<ProductType, OpenSimplexNoise> noises)
		{
			if (!string.IsNullOrEmpty(Id)) throw new ArgumentException("You allowed to set Id only once");
			Id = row + "_" + col;
			Products = new Node();
			AddChild(Products);

			foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
			{
				Product product = new Product(type, -1);
				Products.AddChild(product);
				product.Price = GetPriceFromNoise(row, col, noises, product.ProductType);;
			}
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
		
		internal void DisplayProductData(ProductType productType)
		{
			//show price and color then subscribe in case value changes
			Product product = GetProduct(productType);

			var viewport = GetNode<ViewportScript>("Info/Viewport");
			var mesh = GetNode<MeshInstanceScript>("MeshInstance");

			if (!product.IsConnected(nameof(Product.PriceChanged), viewport, nameof(ViewportScript.SetPriceText)))
				product.Connect(nameof(Product.PriceChanged), viewport, nameof(ViewportScript.SetPriceText));

			if (!product.IsConnected(nameof(Product.PriceChanged), mesh, nameof(MeshInstanceScript.SetColor)))
				product.Connect(nameof(Product.PriceChanged), mesh, nameof(MeshInstanceScript.SetColor));

			//to call PriceChanged signal with no value change
			product.Price += 0f;
		}

		public void AddBuilding(PackedScene scene, ProductType productType, float amount)
		{
			var building = scene.Instance<Source>();
			building.Init(productType, amount);
			Factory = building;
			AddChild(building);
		}
	}
}