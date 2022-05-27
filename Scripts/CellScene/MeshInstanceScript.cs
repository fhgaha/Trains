using System;
using Godot;

namespace Trains.Scripts.CellScene
{
	[Tool]
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
			var maxProductPrice = 100f;	
			h = maxHueValue_green * price / maxProductPrice;
			h = Mathf.Clamp(h, 0, maxHueValue_green);
			this.color = Color.FromHsv(h, s, v);
			material.AlbedoColor = this.color;
			//GD.Print("old color: " + color + ", new color: " + this.color);
		}
	}
}
