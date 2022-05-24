using Godot;
using System;
using Trains.Model.Cells;

namespace Trains.Scripts.CellScene
{
	public class Info : Spatial
	{
		public override void _Ready()
		{
			 string id = GetParent<Cell>().Id;
			 GetNode<Label>("Viewport/VBoxContainer/Id").Text = id;
		}

		public void SetPriceText(float value)
		{
			GetNode<Label>("Viewport/VBoxContainer/Price").Text = value.ToString("#.#");
			GetNode<Viewport>("Viewport").Size = GetNode<VBoxContainer>("Viewport/VBoxContainer").RectSize;
		}
	}
}