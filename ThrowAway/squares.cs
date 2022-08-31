using Godot;
using System;

public class squares : MultiMeshInstance
{
	private Random rnd;

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
			InstanceCount = 65536,
			VisibleInstanceCount = -1,
			Mesh = plane
		};

		int instancesCount = 0;

		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				Multimesh.SetInstanceTransform(instancesCount, new Transform(Basis.Identity, new Vector3(i, 0, j)));
				var color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
				Multimesh.SetInstanceColor(instancesCount, color);
				instancesCount++;
			}
		}
	}
}
