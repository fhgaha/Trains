using Godot;
using System;

namespace Trains
{
	public class GridHelper : Spatial
	{
		const int Size = 12;
		public override void _Ready()
		{
			for (int x = 0; x < Size; x++)
			for (int z = 0; z < Size; z++)
				AddChild(BuilLabel3D(x, z));
		}

		private static Label3D BuilLabel3D(int x, int z)
		{
			var label = new Label3D();
			label.Translation = new Vector3(x, 0f, z);
			label.RotationDegrees = new Vector3(-90f, 0f, 0f);
			label.HorizontalAlignment = Label3D.Align.Left;
			label.VerticalAlignment = Label3D.VAlign.Bottom;
			label.Autowrap = true;
			label.Width = 100;
			label.Text = label.Translation.ToString();
			return label;
		}
	}
}