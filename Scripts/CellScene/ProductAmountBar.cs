using Godot;
using System;

namespace Trains.Scripts.CellScene
{
	public class ProductAmountBar : Spatial
	{
        public void SetAmount(float value)
        {
            var bar = GetNode<TextureProgress>("TextureProgress");
            bar.Value = value;
        }
	}
}