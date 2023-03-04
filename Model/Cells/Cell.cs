using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;
using System.Collections.Generic;
using Trains.Model.Generators.Noises;
using Trains.Scripts.CellScene;
using Trains.Model.Cells.Buildings;
using System.Linq;
using Trains.Model.Migration;
using Trains.Model.Common;

namespace Trains.Model.Cells
{
	[Tool]
	public class Cell : Spatial
	{
		public static Cell[,] Cells { get; set; }
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

		public static float GetDistanceSquared(Cell first, Cell second)
		=> (float)(Math.Pow(first.Row - second.Row, 2) + Math.Pow(first.Col - second.Col, 2));

		internal void CargoArrived(Cargo cargo, ProductType productType, float amount, Cell[,] cells)
		{
			Product product = GetProduct(productType);
			//product.Amount += amount;
			//price change
			//product.Price -= 1 / product.Amount;
			//delta price should be (newPrice - oldPrice) * 0.25
			float deltaPrice = -cargo.Amount * 0.5f;
			product.Price += deltaPrice;
			//if stock price grows every tick, this goes down when cargo comes
			//UpdateNearbyPrices(productType, cells, deltaPrice);
		}

		private Events events;

		public void Init(int row, int col, Dictionary<ProductType, OpenSimplexNoise> noises)
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.Tick), this, nameof(onTick));

			if (!string.IsNullOrEmpty(Id)) throw new ArgumentException("You allowed to set Id only once");
			Id = row + "_" + col;
			Row = row; Col = col;

			Products = new Node();
			ProductList = new List<Product>();

			GetNode<Info>("Info").SetId(Id);

			foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
			{
				Product product = new Product(type, -1);
				Products.AddChild(product);
				ProductList.Add(product);
				product.Price = 30f;

				//connect product button to product
				var amountBar = GetNode<ProductAmountBar>("Amount");
				product.Connect(nameof(Product.AmountChanged), amountBar, nameof(ProductAmountBar.DisplayValue));
				product.Connect(nameof(Product.PriceChanged), this, nameof(onPriceChanged));
				events.Connect(nameof(Events.Tick), product, nameof(Product.onTick));
			}

			var staticBody = GetNode<StaticBody>("MeshInstance/StaticBody");
			staticBody.Connect("mouse_entered", this, nameof(onMouseEntered));
			staticBody.Connect("mouse_exited", this, nameof(onMouseExited));
		}

		private static float GetPriceFromNoise(
			int row, int col, 
			Dictionary<ProductType, OpenSimplexNoise> noises, 
			ProductType productType)
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
			if (Building.SourceProductType == productType) Building.DisplaySourceData();
			else Building.HideSourceData();

			if (Building.StockProductType == productType) Building.DisplayStockData();
			else Building.HideStockData();
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

			//set default color
			((SpatialMaterial)mesh.GetSurfaceMaterial(0)).AlbedoColor = new Color("0ed00f");
			Building?.HideSourceData();
			Building?.HideStockData();
		}

		//add building with both source and stock functions
		public void AddBuilding(BuildingType buildingType, PackedScene scene,
			ProductType sourceProductType, float sourceStartAmount,
			ProductType stockProductType, float stockStartAmount)
		{
			if (!(Building is null)) throw new ArgumentException("Building field is taken. Two buildings in the same cell is not allowed");
			if (buildingType != BuildingType.Both) throw new ArgumentException("buildingType should be \"both\"");

			var building = scene.Instance<Building>();
			Product sourceProduct = null;
			Product stockProduct = null;

			switch (buildingType)
			{
				case BuildingType.Source:
					sourceProduct = GetProduct(sourceProductType);
					building.InitSource(sourceProduct, sourceStartAmount);
					break;
				case BuildingType.Stock:
					stockProduct = GetProduct(stockProductType);
					building.InitStock(stockProduct, stockStartAmount);
					break;
				case BuildingType.Both:
					sourceProduct = GetProduct(sourceProductType);
					building.InitSource(sourceProduct, sourceStartAmount);
					stockProduct = GetProduct(stockProductType);
					building.InitStock(stockProduct, stockStartAmount);
					break;
			}

			Building = building;
			AddChild(building);
			MoveChild(building, 0);
			building.Translate(new Vector3(-0.03f, 0, 0));
		}

		//add source or stock
		public void AddBuilding(BuildingType buildingType, PackedScene scene,
			ProductType productType, float startAmount)
		{
			if (!(Building is null)) throw new ArgumentException("Building field is taken. Two buildings in the same cell is not allowed");
			if (buildingType == BuildingType.Both) throw new ArgumentException("buildingType should not be \"both\"");

			var building = scene.Instance<Building>();
			var product = GetProduct(productType);
			switch (buildingType)
			{
				case BuildingType.Source: building.InitSource(product, startAmount); break;
				case BuildingType.Stock: building.InitStock(product, startAmount); break;
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

		////////////////////////////////////////
		//methods called by signals
		////////////////////////////////////////
		public void onTick()
		{
			UpdateAmountAndPrice();
		}

		//increase amount of source product, decrease amount of stock product
		private void UpdateAmountAndPrice()
		{
			if (Building is null) return;

			if (Building.IsSource)
			{
				var product = GetProduct((ProductType)Building.SourceProductType);
				product.Amount += Building.SourceDeltaAmount;
				product.Price -= Global.PriceDecay;

				UpdateNearbyPrices(product.ProductType, Cells, -Global.PriceDecay * 0.5f);
			}

			if (Building.IsStock)
			{
				//stock amount will be decreased by consuming
				var product = GetProduct((ProductType)Building.StockProductType);
				//product.Amount -= Building.StockDeltaAmount;
				product.Price += Global.PriceDecay;

				UpdateNearbyPrices(product.ProductType, Cells, Global.PriceDecay * 0.5f);
			}
		}

		public void UpdateNearbyPrices(ProductType productType, Cell[,] cells, float deltaPrice)
		{
			//updated grows all the time
			//price always go down
			List<Cell> updated = new List<Cell>();
			updated.Add(this);
			int depth = 5;
			deltaPrice *= 1f;
			var neighbours = GetNeighbours(cells);

			foreach (Cell n in neighbours)
			{
				if (updated.Contains(n)) continue;
				n.GetProduct(productType).Price += deltaPrice;
				updated.Add(n);
			}

			while (depth > 0)
			{
				depth--;
				deltaPrice *= 0.5f;
				var nextNeighbours = new List<Cell>();
				foreach (Cell n in neighbours)
				{
					var nNeighbours = n.GetNeighbours(cells).Where(c => !updated.Contains(c));
					nextNeighbours.AddRange(nNeighbours.Where(_n => !nextNeighbours.Contains(_n)));
				}

				foreach (Cell n in nextNeighbours)
				{
					n.GetProduct(productType).Price += deltaPrice;
					updated.Add(n);
				}
				neighbours = nextNeighbours;
			}
		}

		private void onPriceChanged(Product sender, float value)
		{
			//if mode is the same as sender producttype change color, info
			if (Global.CurrentDisplayProductMode != sender.ProductType) return;

			var info = GetNode<Info>("Info");
			var mesh = GetNode<MeshInstanceScript>("MeshInstance");
			info.SetPriceText(value);
			mesh.SetColor(value);
		}

		private void onMouseEntered()
		{
			//GD.Print(Id + ": onMouseEntered");
			events.EmitSignal(nameof(Events.MouseHoveredOnCell), this);
		}

		private void onMouseExited()
		{
			//GD.Print(Id + ": onMouseExited");
			events.EmitSignal(nameof(Events.MouseHoveredOffCell), this);
		}


		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			Cell cell = (Cell)obj;
			return Id == cell.Id;
		}

		public override int GetHashCode() => Row * 3 + Col * 4 + 5;
	}
}
