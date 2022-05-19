using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;
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

		public void Init(int row, int col, float price)
		{
			if (!string.IsNullOrEmpty(Id)) throw new ArgumentException("You allowed to set Id only once");
			
			Id = row + "_" + col;

			Products = new Node();

			var lumber = new Product(ProductType.Lumber, 20f);
			var grain = new Product(ProductType.Grain, 30f);
			var dairy = new Product(ProductType.Dairy, 40f);

			Products.AddChild(lumber);
			Products.AddChild(grain);
			Products.AddChild(dairy);

			//If I want to build cell grid in editor I have comment this.There will be no price numbers and no colors.

			var mesh = GetNode("MeshInstance") as MeshInstanceScript;
			var viewport = GetNode("Sprite3D/Viewport") as ViewportScript;

			lumber.PriceChangedEvent += mesh.SetColor;
			lumber.PriceChangedEvent += viewport.OnSetText;
			
			SetPrice(ProductType.Lumber, price);
		}
	}
}