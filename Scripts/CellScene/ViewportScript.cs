using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Products;

namespace Trains.Scripts.CellScene
{
	public class ViewportScript : Viewport
	{
		public void SetText(float value)
		{
			GetNode<Label>("Label").Text = value.ToString("#.#");
			Size = GetNode<Label>("Label").RectSize;
		}
	}
}