using Godot;
using System;

namespace Trains.Scripts.MainCamera
{
	public class MainCameraController : Spatial
	{
		////////////////////////////////
		//export
		////////////////////////////////
		//movement
		[Export(PropertyHint.Range, "0,1000,")]
		public float MovementSpeed { get; set; } = 20f;

		//rotation
		[Export(PropertyHint.Range, "0,90,")]
		public int MinElevationAngle { get; set; } = 10;

		[Export(PropertyHint.Range, "0,90,")]
		public int MaxElevationAngle { get; set; } = 90;

		[Export(PropertyHint.Range, "0,1000,0.1")]
		public float RotationSpeed { get; set; } = 10;

		//zoom 
		[Export(PropertyHint.Range, "0,500,")]
		public float MinZoom { get; set; } = 0.5f;

		[Export(PropertyHint.Range, "0,1000,")]
		public float MaxZoom { get; set; } = 90f;

		[Export(PropertyHint.Range, "0,1000,0.1")]
		public float ZoomSpeed { get; set; } = 10f;

		[Export(PropertyHint.Range, "0,1,0.1")]
		public float ZoomSpeedDamp { get; set; } = 0.5f;

		//flags
		[Export]
		public bool RotationAllowed { get; set; } = true;

		[Export]
		public bool InvertRotationHorizontal { get; set; } = false;

		[Export]
		public bool InvertRotationVertical { get; set; } = false;

		////////////////////////////////
		//params
		////////////////////////////////
		private Vector2 lastMousePos = new Vector2();
		private bool isRotating = false;
		private Spatial elevation;
		private float zoomDirection = 0f;
		private Camera camera;

		////////////////////////////////
		//methods
		////////////////////////////////
		public override void _Ready()
		{
			elevation = GetNode<Spatial>("Elevation");
			camera = GetNode<Spatial>("Elevation").GetNode<Camera>("Camera");
		}

		public override void _Process(float delta)
		{
			Move(delta);
			Rotate(delta);
			Zoom(delta);
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			//check if rotating is happening
			if (@event.IsActionPressed("camera_rotate"))
			{
				isRotating = true;
				lastMousePos = GetViewport().GetMousePosition();
			}
			if (@event.IsActionReleased("camera_rotate")) isRotating = false;

			//check if zooming is happening
			if (@event.IsActionPressed("camera_zoom_in")) zoomDirection = -1;
			if (@event.IsActionPressed("camera_zoom_out")) zoomDirection = 1;
		}

		private void Move(float delta)
		{
			//initialize velocity vector, populate it, translate accordingly

			var velocity = new Vector3();

			if (Input.IsActionPressed("camera_forward")) velocity -= Transform.basis.z;
			if (Input.IsActionPressed("camera_backwards")) velocity += Transform.basis.z;
			if (Input.IsActionPressed("camera_left")) velocity -= Transform.basis.x;
			if (Input.IsActionPressed("camera_right")) velocity += Transform.basis.x;

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
			elevation.RotationDegrees
				= new Vector3(newElevation, elevation.RotationDegrees.y, elevation.RotationDegrees.z);
		}

		private void Zoom(float delta)
		{
			float newZoom = camera.Translation.z + ZoomSpeed * delta * zoomDirection;
			newZoom = Mathf.Clamp(newZoom, MinZoom, MaxZoom);
			camera.Translate(new Vector3(0, 0, newZoom));
			camera.Translation = new Vector3(camera.Translation.x, camera.Translation.y, newZoom);
			//stop scrolling
			zoomDirection *= ZoomSpeedDamp;
			if (Mathf.Abs(zoomDirection) <= 0.0001f) zoomDirection = 0;
			//ElevateUsingZoom(delta, InvertRotationVertical);
		}
		
		//the idea is to rotate cam vertically on zoom like in paradox games or in railroad tycoon 3
		private void ElevateUsingZoom(float delta, bool invert = false)
		{
			float invertCoeff = invert ? 1 : -1;
			float newElevation = elevation.RotationDegrees.x 
			- delta * RotationSpeed * invertCoeff / camera.Translation.z;
			newElevation = Mathf.Clamp(newElevation, -MaxElevationAngle, -MinElevationAngle);
			elevation.RotationDegrees
				= new Vector3(newElevation, elevation.RotationDegrees.y, elevation.RotationDegrees.z);
			
			GD.Print("elev: " + elevation.RotationDegrees.x + ", zoom: " + camera.Translation.z );
		}
	}
}