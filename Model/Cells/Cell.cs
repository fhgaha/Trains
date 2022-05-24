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
			amountBar.Visible = true;
			amountBar.ActiveProductType = product.ProductType;
			amountBar.DisplayValue(product.ProductType, product.Amount);

			if (Building != null)
				if (Building.ProductType == productType) Building.DisplayData();
				else Building.HideData();

			//to call PriceChanged signal with no value change
			//product.Price += 0f;
			// product.Amount += 0f;
		}

		internal void HideProductData()
		{
			var info = GetNode<Info>("Info");
			var mesh = GetNode<MeshInstanceScript>("MeshInstance");
			var amountBar = GetNode<ProductAmountBar>("Amount");
			//info.Visible = false;
			mesh.Visible = false;
			amountBar.Visible = true;
			Building?.DisplayData();
		}

		public void AddBuilding(PackedScene scene, ProductType productType, float startAmount)
		{
			var building = scene.Instance<SourceTierI>();
			var product = GetProduct(productType);
			building.Init(product, startAmount);
			Building = building;
			AddChild(building);
			MoveChild(building, 0);
			building.Translate(new Vector3(-0.03f, 0, 0));

			if (building is SourceTierI)
				product.Connect(nameof(Product.PriceChanged), building, nameof(SourceTierI.SetTriangleBar));
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
		public override int GetHashCode()
		{
			return Row * 3 + Col * 4 + 5;
		}
	}
}