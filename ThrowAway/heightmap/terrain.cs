using Godot;
using System;
using System.Linq;

namespace Trains
{
	public class terrain : MeshInstance
	{
		private CollisionShape colShape;
		[Export] private float chunkSize = 2f;
		[Export] private float heightRatio = 1f;
		[Export] private float colShapeSizeRatio = 0.1f;

		Image img = new Image();
		HeightMapShape shape = new HeightMapShape();

		public override void _Ready()
		{
			colShape = GetNode<CollisionShape>("StaticBody/CollisionShape");

			colShape.Shape = shape;
			((PlaneMesh)Mesh).Size = new Vector2(chunkSize, chunkSize);
			UpdateTerrain(heightRatio, colShapeSizeRatio);
		}

		private void UpdateTerrain(float heightRatio, float colShapeSizeRatio)
		{
			MaterialOverride.Set("shader_param/height_ratio", heightRatio);
			img.Load("res://ThrowAway/heightmap/heightmap.exr");
			img.Convert(Image.Format.Rf);
			img.Resize((int)(img.GetWidth() * colShapeSizeRatio), (int)(img.GetHeight() * colShapeSizeRatio));
			var data = Array.ConvertAll(img.GetData(), b => (float)b);

			for (int i = 0; i < data.Length; i++)
				data[i] *= heightRatio;

			shape.MapWidth = img.GetWidth();
			shape.MapDepth = img.GetHeight();
			shape.MapData = data;
			var scaleRatio = chunkSize / (float)img.GetWidth();
			colShape.Scale = new Vector3(scaleRatio, 1f, scaleRatio);
		}
	}
}
