using Godot;
using System;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Scripts.GUI
{
	public class CellToolTip : Control
	{
		private Events events;
		private Label productName;
		private Label priceAndAmount;
		private Cell cellHoveredOn;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");

			events.Connect(nameof(Events.MouseHoveredOnCell), this, nameof(onMouseHoveredOnCell));
			events.Connect(nameof(Events.MouseHoveredOffCell), this, nameof(onMouseHoveredOffCell));
			events.Connect(nameof(Events.Tick), this, nameof(onTick));

			productName = GetNode<Label>("VBoxContainer/Label");
			priceAndAmount = GetNode<Label>("VBoxContainer/Label2");
			GD.Print("CellToolTip is ready");
		}

		private void onMouseHoveredOnCell(Cell cell)
		{
			//GD.Print("onMouseHoveredOnCell recieved cell");
			if (Global.CurrentDisplayProductMode is null) 
			{ 
				Erase();
				string amount = cell.ProductList.Sum(p => p.Amount).ToString("0.0");
				priceAndAmount.Text = "Total quantity: " + amount;
				return; 
			}

			cellHoveredOn = cell;
			productName.Text = ((ProductType)Global.CurrentDisplayProductMode).ToString();
			SetPriceAndAmount();
		}

		private void onMouseHoveredOffCell(Cell cell)
		{
			//GD.Print("onMouseHoveredOffCell recieved cell");
			cellHoveredOn = null;
			Erase();
		}

		private void onTick()
		{
			if (cellHoveredOn is null) return;
			SetPriceAndAmount();
		}

		private void SetPriceAndAmount()
		{
			string price = cellHoveredOn.GetPrice((ProductType)Global.CurrentDisplayProductMode).ToString("0.0");
			string amount = cellHoveredOn.GetProduct((ProductType)Global.CurrentDisplayProductMode).Amount.ToString("0.0");
			priceAndAmount.Text = "Price: $" + price + " Available quantity: " + amount;
		}

		private void Erase()
		{
			productName.Text = "";
			priceAndAmount.Text = "";
		}
	}
}