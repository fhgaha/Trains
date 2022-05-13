using Godot;
using System.Collections.Generic;
using System;
using Trains.Scripts.Products;

namespace Trains.Scripts
{
	public class Cell : Spatial
	{
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
		float maxHueValue_green = 113f / 360f;    //0.36f
		float minHueValue_red = 0f;

		public int Size { get; set; } = 1;
		public Color Color { get; private set; }

		private List<Product> products;
		public Dictionary<Product, float> DemandRate { get; private set; }

		private RandomNumberGenerator random = new RandomNumberGenerator();
		private Timer timer;
		private string id;

		public override void _Ready() { }

		public override void _Process(float delta) { }

		public void SetColor()
		{
			MeshInstance mesh = GetNode<MeshInstance>("MeshInstance");
			var material = (SpatialMaterial)mesh.GetSurfaceMaterial(0);
			var color = material.AlbedoColor;
			float h, s, v;
			color.ToHsv(out h, out s, out v);
			random.Randomize();
			h += GetHueBasedOfDemand(h);
			h = Mathf.Clamp(h, minHueValue_red, maxHueValue_green);
			this.Color = Color.FromHsv(h, s, v);
			material.AlbedoColor = this.Color;

			// GD.Print(this + ": prev color: " + color + ", new color: " + this.Color);
		}

		private float GetHueBasedOfDemand(float h)
		{
			//get change hue value based on changed product's price
			return -0.02f;
		}
	}
}