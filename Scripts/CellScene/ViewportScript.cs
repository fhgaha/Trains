using Godot;
using System;

namespace Trains.Scripts.CellScene
{
	[Tool]
	public class ViewportScript : Viewport
	{
		public void OnSetText(float value)
		{
			GetNode<Label>("Label").Text = value.ToString();
			Size = GetNode<Label>("Label").RectSize;
		}
	}
}