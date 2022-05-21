using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Products;

namespace Trains.Scripts.CellScene
{
	public class ViewportScript : Viewport
	{
		public override void _Ready()
		{
			 string id = GetParent().GetParent<Cell>().Id;
			 GetNode("VBoxContainer").GetNode<Label>("Id").Text = id;
		}

		public void SetPriceText(float value)
		{
			GetNode("VBoxContainer").GetNode<Label>("Price").Text = value.ToString("#.#");
			Size = GetNode<VBoxContainer>("VBoxContainer").RectSize;
		}
	}
}