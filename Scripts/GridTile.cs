using Godot;
using System;

namespace Trains
{
	public class GridTile : MeshInstance
	{
		private Label3D label;
		
		public override void _Ready()
		{
			label = GetNode<Label3D>("Label3D");
			label.Text = GlobalTranslation.ToString();
		}
	}
}