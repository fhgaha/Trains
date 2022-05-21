using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Products;

namespace Trains.Scripts.CellScene
{
	public class ViewportScript : Viewport
	{
		public void Init(string value)
		{
			GetNode<VBoxContainer>("VBoxContainer").GetNode<Label>("Id").Text = value;
			Size = GetNode<VBoxContainer>("VBoxContainer").RectSize;
		}

		public void SetPriceText(float value)
		{
			GetNode<VBoxContainer>("VBoxContainer").GetNode<Label>("Price").Text = value.ToString("#.#");
			Size = GetNode<VBoxContainer>("VBoxContainer").RectSize;
		}
	}
}