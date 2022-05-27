using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;
using System.Collections.Generic;
using Trains.Model.Generators.Noises;
using Trains.Scripts.CellScene;
using Trains.Model.Cells.Buildings;
using Trains.Model.Cells.Buildings.Sources;
using System.Linq;
using Trains.Model.Migration;

namespace Trains.Model.Cells
{
	[Tool]
	public class Cell : Spatial
	{
		[Export] public string Id { get; set; }
		public int Row { get; private set; }
		public int Col { get; private set; }
		public Node Products { get; set; }
		public List<Product> ProductList { get; set; }
		public Building Building { get; set; }

		public int Size { get; } = 1;

		public float GetPrice(ProductType type) => ProductList.First(p => p.ProductType == type).Price;

		public void SetPrice(ProductType type, float price) => ProductList.First(p => p.ProductType == type).Price = price;

		public Product GetProduct(ProductType type) => ProductList.First(p => p.ProductType == type);

		public static float GetDistance(Cell first, Cell second) 
		=> (float)Math.Sqrt(Math.Pow(first.Row - second.Row, 2) + Math.Pow(first.Col - second.Col, 2));

		public void Init(int row, int col, Dictionary<ProductType, OpenSimplexNoise> noises)
		{
			if (!string.IsNullOrEmpty(Id)) throw new ArgumentException("You allowed to set Id only once");
			Id = row + "_" + col;
			Row = row; Col = col;

			Products = new Node();
			ProductList = new List<Product>();
			//AddChild(Products);

			GetNode<Info>("Info").SetId(Id);

			foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
			{
				Product product = new Product(type, -1);
				Products.AddChild(product);
				ProductList.Add(product);
				product.Price = GetPriceFromNoise(row, col, noises, product.ProductType);

				//connect product button to product
				var info = GetNode<Info>("Info");
				var mesh = GetNode<MeshInstanceScript>("MeshInstance");
				var amountBar = GetNode<ProductAmountBar>("Amount");
				
				product.Connect(nameof(Product.AmountChanged), amountBar, nameof(ProductAmountBar.DisplayValue));
				product.Connect(nameof(Product.PriceChanged), info, nameof(Info.SetPriceText));
				product.Connect(nameof(Product.PriceChanged), mesh, nameof(MeshInstanceScript.SetColor));
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
			//show price, color and amount for selected product
			Product product = GetProduct(productType);

			var info = GetNode<Info>("Info");
			var mesh = GetNode<MeshInstanceScript>("MeshInstance");
			var amountBar = GetNode<ProductAmountBar>("Amount");

			mesh.Visible = true;
			amountBar.ActiveProductType = product.ProductType;
			amountBar.DisplayValue(product.ProductType, product.Amount);

			//to call PriceChanged signal with no value change
			product.Price += 0f;
			
			//carefull! building can be source, stock or source for one prtoduct and stock for other product
			if (Building == null) return;
			if (Building.SourceProductType == productType)
				Building.DisplaySourceData();
			else 
				Building.HideSourceData();

			if (Building.StockProductType == productType)
				Building.DisplayStockData();
			else 
				Building.HideStockData();
		}

		internal void DisplayProductDataAllProductsMode()
		{
			var info = GetNode<Info>("Info");
			var mesh = GetNode<MeshInstanceScript>("MeshInstance");
			var amountBar = GetNode<ProductAmountBar>("Amount");

			//display sum of all products amounts
			var someProduct = ProductList.First();
			var amountSum = ProductList.Sum(p => p.Amount);
			amountBar.ActiveProductType = someProduct.ProductType;
			amountBar.DisplayValue(someProduct.ProductType, amountSum);

			mesh.Visible = false;
			Building?.HideSourceData();
			Building?.HideStockData();
		}

		public void AddBuilding(BuildingType buildingType, PackedScene scene, ProductType productType, float startAmount)
		{
			var building = scene.Instance<Building>();
			var product = GetProduct(productType);
			
			switch (buildingType)
			{
				case BuildingType.Source: 
					building.InitSource(product, startAmount);
					break;
				case BuildingType.Stock:
					building.InitStock(product, startAmount);
					break;
			}
			
			Building = building;
			AddChild(building);
			MoveChild(building, 0);
			building.Translate(new Vector3(-0.03f, 0, 0));
		}

		public List<Cell> GetNeighbours(Cell[,] cells)
		{
			int row = int.Parse(Id.Split('_')[0]);
			int col = int.Parse(Id.Split('_')[1]);
			List<Cell> neighbours = new List<Cell>();

			for (var dy = -1; dy <= 1; dy++)
			for (var dx = -1; dx <= 1; dx++)
			{
				if (dx == 0 && dy == 0) continue;
				if (row + dx < 0 || col + dy < 0) continue;
				if (row + dx > cells.GetLength(0) - 1 || col + dy > cells.GetLength(1) - 1) continue;

				Cell neighbour = cells[row + dx, col + dy];
				neighbours.Add(neighbour);
			}
			return neighbours;
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;
			
			Cell cell = (Cell)obj;
			return Id == cell.Id;
		}

		// override object.GetHashCode
		public override int GetHashCode() => Row * 3 + Col * 4 + 5;
	}
}