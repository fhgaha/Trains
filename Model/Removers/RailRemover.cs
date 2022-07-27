using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class RailRemover : Spatial
	{
		private Camera camera;
		private CollisionShape collider;

		public override void _Ready()
		{
			collider = GetNode<CollisionShape>("Circle/RailCollider/CollisionShape");
		}

		public void Init(Camera camera)
		{
			this.camera = camera;
		}

		public override void _PhysicsProcess(float delta)
		{
			var mousePos = this.GetIntersection(camera);
			Translation = mousePos;
		}
	}
}