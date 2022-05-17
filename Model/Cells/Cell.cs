using Godot;
using System.Collections.Generic;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;
using System.Linq;

namespace Trains.Model.Cells
{
	[Tool]
	public class Cell : Spatial
	{
		public string Id { get; set; }

		public int Size { get; } = 1;

		public List<Product> Products { get; set; }

		public float GetPrice(ProductType type) => Products.First(p => p.ProductType == type).Price;
		public void SetPrice(ProductType type, float price) => Products.First(p => p.ProductType == type).Price = price;

		public override void _Ready() { }

		public override void _Process(float delta) { }

		public void Init(int row, int col)
		{
			if (!string.IsNullOrEmpty(Id)) throw new ArgumentException("You allowed to set Id only once");
			Id = row + "_" + col;
			Products = Product.BuildList();

			Products[0].PriceChangedEvent += GetNode<MeshInstanceScript>("MeshInstance").SetColor;
		}
	}
}