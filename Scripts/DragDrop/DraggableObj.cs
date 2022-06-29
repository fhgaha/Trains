//https://github.com/christinoleo/godot-plugin-DragDrop3D
using Godot;
using System;

namespace Trains.Scripts.DragDrop
{
	//[Tool]
	public class DraggableObj : Spatial
	{
		private Color color = new Color("fdff00");

		[Export] public Color Color
		{
			get => color;
			set
			{
				color = value;
				if (Engine.EditorHint)
				SetColor(value);
			}
		} 

		public override void _Ready()
		{
			SetColor(Color);

			var draggable = GetNode<Draggable>("Area/Draggable_");
			draggable.Connect(nameof(Draggable.DragMove), this, nameof(onDragMove));
		}

		public void onDragMove(Node node, Godot.Collections.Dictionary cast)
		{
			Translation = (Vector3)cast["position"];
		}

		private void SetColor(Color value)
		{
			var mesh = (PrimitiveMesh)GetNode<MeshInstance>("MeshInstance").Mesh;
			var material = (SpatialMaterial)mesh.Material;
			material.AlbedoColor = value;
			material.FlagsTransparent = true;
		}
	}
}