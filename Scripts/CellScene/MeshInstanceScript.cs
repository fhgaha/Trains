using Godot;
using System;
using Trains.Model.Products;

namespace Trains.Scripts.CellScene
{
	public class MeshInstanceScript : MeshInstance
	{
		//hue represents as fraction of degrees
		const float maxHueValue_green = 113f / 360f;    //0.36f
		const float minHueValue_red = 0f;
		private Color color;

		public void SetColor(float price)
		{
			var material = (SpatialMaterial)GetSurfaceMaterial(0);
			var color = material.AlbedoColor;
			float h, s, v;
			color.ToHsv(out h, out s, out v);
			//instead of 100 should be max product value
			h = maxHueValue_green * price / 100;
			this.color = Color.FromHsv(h, s, v);
			material.AlbedoColor = this.color;
		}
	}
}