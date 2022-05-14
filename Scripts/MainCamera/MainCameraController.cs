using Godot;
using System;

namespace Trains.Scripts.MainCamera
{
	public class MainCameraController : Spatial
	{
		[Export(PropertyHint.Range, "0,1000,")]
		public float MovementSpeed { get; set; } = 20f;

		//rotation
		[Export(PropertyHint.Range, "0,90,")]
		public int MinElevationAngle { get; set; } = 10;

		[Export(PropertyHint.Range, "0,90,")]
		public int MaxElevationAngle { get; set; } = 90;

		[Export(PropertyHint.Range, "0,1000,0.1")]
		public float RotationSpeed { get; set; } = 10;

		//flags
		[Export]
		public bool RotationAllowed { get; set; } = true;

		[Export]
		public bool InvertRotationHorizontal {get; set;} = false;

		[Export]
		public bool InvertRotationVertical { get; set; } = false;

		private Vector2 lastMousePos = new Vector2();
		private bool isRotating = false;
		private Spatial elevation;

		public override void _Ready()
		{
			elevation = GetNode<Spatial>("Elevation");
		}

		public override void _Process(float delta)
		{
			Move(delta);
			Rotate(delta);
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed("camera_rotate"))
			{
				isRotating = true;
				lastMousePos = GetViewport().GetMousePosition();
			}
			if (@event.IsActionReleased("camera_rotate")) isRotating = false;
		}

		private void Move(float delta)
		{
			//initialize velocity vector, populate it, translate accordingly

			var velocity = new Vector3();

			if (Input.IsActionPressed("camera_forward"))
				velocity -= Transform.basis.z;

			if (Input.IsActionPressed("camera_backwards"))
				velocity += Transform.basis.z;

			if (Input.IsActionPressed("camera_left"))
				velocity -= Transform.basis.x;

			if (Input.IsActionPressed("camera_right"))
				velocity += Transform.basis.x;

			velocity = velocity.Normalized();

			Translation += velocity * delta * MovementSpeed;
		}

		private void Rotate(float delta)
		{
			//get mouse position, use horizontal displacement to rotate, use vertical displacement to rotate

			if (!isRotating || !RotationAllowed) return;

			Vector2 displacement = GetMouseDisplacement();
			RotateLeftRight(delta, displacement.x, InvertRotationHorizontal);
			Elevate(delta, displacement.y, InvertRotationVertical); //rotate up down
		}

		private Vector2 GetMouseDisplacement()
		{
			Vector2 currentMousePos = GetViewport().GetMousePosition();
			Vector2 displacement = currentMousePos - lastMousePos;
			lastMousePos = currentMousePos;
			//GD.Print(displacement);
			return displacement;
		}

		private void RotateLeftRight(float delta, float value, bool invert = false)
		{
			float invertCoeff = invert ? 1 : -1;
			Vector3 vector = RotationDegrees;
			value *= delta * RotationSpeed * invertCoeff;
			RotationDegrees = new Vector3(vector.x, vector.y + value, vector.z);
		}

		private void Elevate(float delta, float value, bool invert = false)
		{
			//get elevation, clamp it, set current elevation to calculated one

			float invertCoeff = invert ? 1 : -1;
			float newElevation = elevation.RotationDegrees.x + value * delta * RotationSpeed * invertCoeff;
			newElevation = Mathf.Clamp(newElevation, -MaxElevationAngle, -MinElevationAngle);
			elevation.RotationDegrees = new Vector3(
				newElevation, elevation.RotationDegrees.y, elevation.RotationDegrees.z);
		}
	}
}