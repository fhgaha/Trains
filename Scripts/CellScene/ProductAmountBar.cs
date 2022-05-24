using Godot;
using System;
using static Trains.Model.Common.Enums;

namespace Trains.Scripts.CellScene
{
	public class ProductAmountBar : Spatial
	{
        public ProductType ActiveProductType { get; set; }

        //amount changed signal is connected to this
        //so every time any product changes amount this value is set to the last product changed value
        public void DisplayValue(ProductType productType, float value)
        {
            if (ActiveProductType != productType) return;

            var bar = GetNode<TextureProgress>("Viewport/TextureProgress");
            bar.Value = value;
        }
	}
}