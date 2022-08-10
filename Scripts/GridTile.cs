using Godot;
using System;

namespace Trains
{
	public class GridTile : MeshInstance
	{
		public override void _Ready()
		{
			GetNode<Label3D>("Label3D").Text = GlobalTranslation.ToString();
		}
	}
}