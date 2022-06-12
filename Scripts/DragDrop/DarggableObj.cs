//https://github.com/christinoleo/godot-plugin-DragDrop3D
using Godot;
using System;

namespace Trains.Scripts.DragDrop
{
	public class DarggableObj : Spatial
	{
		public override void _Ready()
		{
			var draggable = GetNode<Draggable>("Area/Draggable_");
			draggable.Connect(nameof(Draggable.DragMove), this, nameof(onDragMove));
		}

		
// func _on_Draggable_drag_move(node, cast):
// 	set_translation(cast['position'])

		public void onDragMove(Node node, Godot.Collections.Dictionary cast)
		{
			Translation = (Vector3)cast["position"];
		}
	}
}