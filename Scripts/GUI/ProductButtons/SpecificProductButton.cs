using Godot;
using System;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Scripts.GUI.ProductButtons
{
	public class SpecificProductButton : Button
	{
		private Events events;
		private bool wasPressed = false;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			Connect("pressed", this, nameof(onButtonPressed));
			events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButton));
			events.Connect(nameof(Events.AllProductButtonPressed), this, nameof(onAllProductsButton));
		}

		private void onButtonPressed()
		{
			if (wasPressed)
			{
				Pressed = true;
				return;
			}

			wasPressed = true;
			var productType = GetProductType(Text);
			events.EmitSignal(nameof(Events.SpecificProductButtonPressed), productType);
		}

		private ProductType GetProductType(string text)
		{
			switch (Text)
			{
				case "Lumber": return ProductType.Lumber;
				case "Grain": return ProductType.Grain;
				default: return ProductType.Dairy;
			}
		}

		private void onAllProductsButton() => Unpress();

		private void onSpecificProductButton(ProductType productType)
		{
			if (GetProductType(Text) == productType) return;
			Unpress();
		}

		private void Unpress()
		{
			wasPressed = false;
			Pressed = false;
		}

	}
}