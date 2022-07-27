////https://github.com/christinoleo/godot-plugin-DragDrop3D
using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains.Scripts.DragDrop
{
	public class DragDropController : Node
	{
		private List<Draggable> draggables = new List<Draggable>();
		private Camera camera;
		private Draggable dragging;

		public override void _Ready()
		{
			camera = GetTree().Root.GetCamera();
			SetPhysicsProcess(false);
		}

		public void RegisterDraggable(Draggable draggable)
		{
			draggables.Add(draggable);
			draggable.Connect(nameof(Draggable.DragStart), this, nameof(onDragStart));
			draggable.Connect(nameof(Draggable.DragStop), this, nameof(onDragStop));
		}

		private void onDragStart(Draggable node)
		{
			dragging = node;
			SetPhysicsProcess(true);
		}

		private void onDragStop(Draggable node)
		{
			SetPhysicsProcess(false);
		}

		public override void _PhysicsProcess(float delta)
		{
			var mouse = GetViewport().GetMousePosition();
			var from = camera.ProjectRayOrigin(mouse);
			var to = from + camera.ProjectRayNormal(mouse) * Global.RayLength;

			var cast = camera.GetWorld().DirectSpaceState.IntersectRay(from, to, 
				new Godot.Collections.Array { dragging.GetParent() }, dragging.GetParent<CollisionObject>().CollisionMask, true, true);
			
			if (cast.Count != 0) dragging.onHover(cast);
		}
	}
}