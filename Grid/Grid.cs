using Godot;
using System;
using System.Collections.Generic;

public class Grid : Spatial
{
	PackedScene cellScene = GD.Load<PackedScene>("res://Cell/Cell.tscn");
	// Color red = new Color(0.188235f, 0.8f, 0.078431f);
	// Color green = new Color(0.8f, 0.236f, 0.08f);
	// int maxHueValue_green = 113;
	// int minHueValue_red = 0;

	public override void _Ready()
	{
		//build grid
		for (int i = 0; i < 1; i++)
		{
			for (int j = 0; j < 1; j++)
			{
				var cell = (Cell)cellScene.Instance();
				cell.Translate(new Vector3(i * cell.Size, 0, j * cell.Size));

				MeshInstance mesh = (MeshInstance)cell.GetNode(@"MeshInstance");
				var material = (SpatialMaterial)mesh.GetSurfaceMaterial(0);
				var color = material.AlbedoColor;

				AddChild(cell);
			}
		}
	}


}
