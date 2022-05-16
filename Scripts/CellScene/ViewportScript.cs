using Godot;
using System;

namespace Trains.Scripts.CellScene
{
	[Tool]
	public class ViewportScript : Viewport
	{
		Label label;

		public override void _Ready()
		{
		    label = GetNode<Label>("Label");   
		    Size = label.RectSize;
		}

		// public override void _Process(float delta)
		// {
		// 	this.Size = GetNode<Label>("Label").RectSize;
		// }

		public void onSetText(string text)
		{
			label.Text = text;
			Size = label.RectSize;
		}
	}
}