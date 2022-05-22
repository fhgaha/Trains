using Godot;
using System;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Scripts.GUI.ProductButtons
{
	public class SpecificProductButton : Button
	{
		[Export] public Events Events { get; set; }
		//private Events Events;
		private bool wasPressed = false;

		public override void _Ready()
		{
			Events = GetNode<Events>("/root/Events");
			Connect("pressed", this, nameof(onButtonPressed));
			Events.Connect(nameof(Events.SpecificProductButtonPressed), this, nameof(onSpecificProductButton));
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
			Events.EmitSignal(nameof(Events.SpecificProductButtonPressed), productType);
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

		private void onSpecificProductButton(ProductType productType)
		{
			if (GetProductType(Text) == productType) return;

			wasPressed = false;
			Pressed = false;
		}
	}
}