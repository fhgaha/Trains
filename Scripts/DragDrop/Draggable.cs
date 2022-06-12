////https://github.com/christinoleo/godot-plugin-DragDrop3D
using Godot;
using System;

namespace Trains.Scripts.DragDrop
{
	//if body has a collision object it will be detected as ray cast target 
	//so either disable collision body of that object or use specific layer
	[Tool]
	public class Draggable : Node
	{
		[Signal] public delegate void DragStart(Draggable draggable);
		[Signal] public delegate void DragStop(Draggable draggable);
		[Signal] public delegate void DragMove(Draggable draggable);
		[Export(PropertyHint.Range, "0,19,")] public int Bit = 19;

		private DragDropController controller;
		private uint areaLayer;
		private uint areaMask;
		private Node current = null;
		private Vector2 dragOffset = new Vector2();
		private CollisionObject hovered = null;

		public override string _GetConfigurationWarning()
		=> GetParent<Node>() is CollisionObject ? "" : "Not under a collision object";

		public override void _Ready()
		{
			controller = GetNode<DragDropController>("/root/DragDropController");
			areaLayer = GetParent<Area>().CollisionLayer;
			areaMask = GetParent<Area>().CollisionMask;

			if (Engine.EditorHint)
			{
				SetProcess(false);
				return;
			}

			if (controller is null)
			{
				GD.PrintErr("Missing DragDropController singletron!");
				return;
			}

			var draggable = GetParent();
			draggable.Connect("mouse_entered", this, nameof(onMouseEntered), new Godot.Collections.Array { draggable });
			draggable.Connect("mouse_exited", this, nameof(onMouseExited), new Godot.Collections.Array { draggable });
			draggable.Connect("input_event", this, nameof(onInputEvent), new Godot.Collections.Array { draggable });
			controller.RegisterDraggable(this);
		}

		private void onMouseEntered(CollisionObject collider) => hovered = collider;

		private void onMouseExited(CollisionObject collider) => hovered = null;

		public void onHover(Godot.Collections.Dictionary cast) => EmitSignal(nameof(DragMove), this, cast);

		private void onInputEvent(Godot.Object camera, InputEvent @event, Vector3 position, Vector3 normal, int shape_idx, Node node)
		{
			if (@event is InputEventMouseButton ev && ev.ButtonIndex == (int)ButtonList.Left)
			{
				if (ev.IsPressed())
				{
					if (!(hovered is null))
					{
						current = hovered.GetParent();
						EmitSignal(nameof(DragStart), this);
					}
				}
				else if (!(current is null))
				{
					EmitSignal(nameof(DragStop), this);
				}
			}
		}

		// 	func depth_sort(a,b):
		// return b.get_index()<a.get_index()
	}
}