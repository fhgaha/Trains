using Godot;
using System;
using System.Globalization;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Scripts.GUI
{
	public class CellToolTip : Control
	{
		private Events events;
		private Label labelTop;
		private Label labelBottom;
		private Cell cellHoveredOn;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");

			events.Connect(nameof(Events.MouseHoveredOnCell), this, nameof(onMouseHoveredOnCell));
			events.Connect(nameof(Events.MouseHoveredOffCell), this, nameof(onMouseHoveredOffCell));
			events.Connect(nameof(Events.Tick), this, nameof(onTick));

			labelTop = GetNode<Label>("VBoxContainer/Label");
			labelBottom = GetNode<Label>("VBoxContainer/Label2");
			//GD.Print("CellToolTip is ready");
		}

		private void onMouseHoveredOnCell(Cell cell)
		{
			//GD.Print("onMouseHoveredOnCell recieved cell");
			cellHoveredOn = cell;
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
			if (Global.CurrentDisplayProductMode is null) SetAmountOnly();
			else SetFullInfo();
		}
		
		private void SetAmountOnly()
		{
			string amount = cellHoveredOn.ProductList.Sum(p => p.Amount).ToString("0.0");
			labelTop.Text = "Total quantity: " + amount;
		}

		private void SetFullInfo()
		{
			labelTop.Text = ((ProductType)Global.CurrentDisplayProductMode).ToString();
			string price = cellHoveredOn.GetPrice((ProductType)Global.CurrentDisplayProductMode).ToString("0.0");
			string amount = cellHoveredOn.GetProduct((ProductType)Global.CurrentDisplayProductMode).Amount.ToString("0.0");
			labelBottom.Text = "Price: $" + price + "   Available quantity: " + amount;
		}

		private void Erase()
		{
			labelTop.Text = "";
			labelBottom.Text = "";
		}
	}
}