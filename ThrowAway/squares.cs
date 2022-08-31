using Godot;
using System;

public class squares : MultiMeshInstance
{
	private Random rnd;
	private const int width = 256;
	private const int height = 256;

	public override void _Ready()
	{
		rnd = new Random();

		var plane = new PlaneMesh
		{
			Size = new Vector2(1, 1),
			Material = new SpatialMaterial { VertexColorUseAsAlbedo = true }
		};

		Multimesh = new MultiMesh
		{
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
			ColorFormat = MultiMesh.ColorFormatEnum.Color8bit,
			CustomDataFormat = MultiMesh.CustomDataFormatEnum.None,
			InstanceCount = width*height,
			VisibleInstanceCount = -1,
			Mesh = plane
		};

		int instancesCount = 0;

		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z < height; z++)
			{
				Multimesh.SetInstanceTransform(instancesCount, new Transform(Basis.Identity, new Vector3(x, 0, z)));
				var color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
				Multimesh.SetInstanceColor(instancesCount, color);
				instancesCount++;
			}
		}

		//later colors can be set like that
		Multimesh.SetInstanceColor(0, Colors.Purple);
		Multimesh.SetInstanceColor(1, Colors.Purple);
		Multimesh.SetInstanceColor(2, Colors.Purple);
	}
}
