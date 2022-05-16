using Godot;
using System.Collections.Generic;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells
{
	[Tool]
	public class Cell : Spatial
	{
		private string id;
		public string Id
		{
			get => id;
			set
			{
				if (!string.IsNullOrEmpty(id)) throw new ArgumentException("You allowed to set Id only once");
				id = value;
			}
		}
		//hue represents as fraction of degrees
		const float maxHueValue_green = 113f / 360f;    //0.36f
		const float minHueValue_red = 0f;

		public int Size { get; } = 1;
		private Color color;

		public List<Product> ProductList { get; set; }
		public Dictionary<ProductType, float> Products { get; set; }

		public override void _Ready() { }

		public override void _Process(float delta) { }

		public void Init(int row, int col)
		{
			if (string.IsNullOrEmpty(Id)) Id = row + "_" + col;
			ProductList = Product.BuildList();
			
			Products = new Dictionary<ProductType, float>();
			foreach (var p in ProductList)
				Products[p.ProductType] = p.Price;
		}

		public void SetColor()
		{
			MeshInstance mesh = GetNode<MeshInstance>("MeshInstance");
			var material = (SpatialMaterial)mesh.GetSurfaceMaterial(0);
			var color = material.AlbedoColor;
			float h, s, v;
			color.ToHsv(out h, out s, out v);
			h += GetHueBasedOfDemand(h);
			h = Mathf.Clamp(h, minHueValue_red, maxHueValue_green);
			this.color = Color.FromHsv(h, s, v);
			material.AlbedoColor = this.color;

			// GD.Print(this + ": prev color: " + color + ", new color: " + this.Color);
		}

		private float GetHueBasedOfDemand(float h)
		{
			//get change hue value based on changed product's price
			return -0.02f;
		}
	}
}