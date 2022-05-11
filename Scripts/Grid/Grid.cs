using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Scripts
{
	public class Grid : Spatial
	{
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");

		public override void _Ready()
		{
			//build grid
			for (int i = 0; i < 1 ; i++)
			{
				for (int j = 0; j < 1 ; j++)
				{
					var cell = cellScene.Instance<Cell>();
					cell.Translate(new Vector3(i * cell.Size, 0, j * cell.Size));

					MeshInstance mesh = cell.GetNode<MeshInstance>(@"MeshInstance");
					var material = (SpatialMaterial)mesh.GetSurfaceMaterial(0);
					var color = material.AlbedoColor;

					AddChild(cell);
				}
			}
		}
	}
}
