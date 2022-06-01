using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Scripts.GUI
{
	public class CellToolTip : Popup
	{
		private Events events;
		private Label productName;
		private Label priceAndAmount;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");

			events.Connect(nameof(Events.MouseHoveredOnCell), this, nameof(onMouseHoveredOnCell));
			events.Connect(nameof(Events.MouseHoveredOffCell), this, nameof(onMouseHoveredOffCell));
			//GD.Print("CellToolTip is ready");

			productName = GetNode<Label>("VBoxContainer/Label");
			priceAndAmount = GetNode<Label>("VBoxContainer/Label2");
		}

		private void onMouseHoveredOnCell(Cell cell)
		{
			//GD.Print("onMouseHoveredOnCell recieved cell");
			if (Global.CurrentDisplayProductMode is null)
			{
				Erase();
				return;
			}
			string _productName = Global.CurrentDisplayProductMode.ToString();
			var price = cell.GetPrice((ProductType)Global.CurrentDisplayProductMode);
			var amount = cell.GetProduct((ProductType)Global.CurrentDisplayProductMode).Amount;
			Popup_();
			productName.Text = _productName;
			priceAndAmount.Text = "Price: $" + price + " Available quantity: " + amount;
		}

		private void onMouseHoveredOffCell(Cell cell)
		{
			//GD.Print("onMouseHoveredOffCell recieved cell");
			Erase();
		}

		private void Erase()
		{
			productName.Text = "";
			priceAndAmount.Text = "";
		}
	}
}