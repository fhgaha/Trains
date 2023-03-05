using Godot;
using System;

public class squares : MultiMeshInstance
{
	private Random _rnd;
	private const int Width = 128;
	private const int Height = 128;
	private Timer timer;

	public override void _Ready()
	{
		timer = GetParent().GetNode<Timer>("Timer");
		timer.Connect("timeout", this, nameof(OnTimeout));
		timer.Autostart = true;
		// timer.Start(1);

		_rnd = new Random();

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
			InstanceCount = Width * Height,
			VisibleInstanceCount = -1,
			Mesh = plane
		};

		for (int z = 0; z < Height; z++)
			for (int x = 0; x < Width; x++)
			{
				var index = x + (z * Height);
				Multimesh.SetInstanceTransform(index, new Transform(Basis.Identity, new Vector3(x, 0, z)));
				var color = new Color((float)_rnd.NextDouble(), (float)_rnd.NextDouble(), (float)_rnd.NextDouble());
				Multimesh.SetInstanceColor(index, color);
			}

		//later colors can be set like that
		Multimesh.SetInstanceColor(0, Colors.Purple);
		Multimesh.SetInstanceColor(1, Colors.Purple);
		Multimesh.SetInstanceColor(2, Colors.Purple);
	}

	private void OnTimeout()
	{
		var incrVal = 0.1f;

		for (int z = 0; z < Height; z++)
			for (int x = 0; x < Width; x++)
			{
				var index = x + (z * Height);
				var color = Multimesh.GetInstanceColor(index);
				var newColor = new Color(
					color.r = color.r >= 1 ? 0 : color.r + incrVal,
					color.g = color.g >= 1 ? 0 : color.g + incrVal,
					color.b = color.b >= 1 ? 0 : color.b + incrVal
				);
				Multimesh.SetInstanceColor(index, newColor);
			}
	}
}
