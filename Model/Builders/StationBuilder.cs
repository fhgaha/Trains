using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Builders
{
	public class StationBuilder : Spatial
	{
		private Cell[,] cells;
		private Events events;
		private PackedScene stationScene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");
		private Spatial station;
		private const float rayLength = 1000f;
		private Camera camera;

		public void Init(Cell[,] cells, Camera camera)
		{
			this.cells = cells;
			this.camera = camera;
			//cant get events if not in the scene
			events = GetNode<Events>("/root/Events");
			GD.Print("StationBuilder: " + events);
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			//GD.Print("onMainButtonPressed");

			//turn on building mode
			//turn cursor into station asset
			station = stationScene.Instance<Spatial>();
			AddChild(station);

		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseMotion ev)
			{
				if (station is null) return;
				
				// var pos = camera.ProjectRayOrigin(ev.Relative) + camera.ProjectRayNormal(ev.Relative) * rayLength;
				// pos.y = 0;
				// GD.Print(pos);
				// station.Translation = pos;
				// station.Translation = new Vector3(
				// 	station.Translation.x + ev.Relative.x * 0.001f, 
				// 	0, 
				// 	station.Translation.z + ev.Relative.y * 0.001f);
				//var from = camera.ProjectRayOrigin(eventMouseButton.Position);
				//var to = from + camera.ProjectRayNormal(eventMouseButton.Position) * rayLength;
			}
		}

		Vector3 rayOrigin;
		Vector3 rayNormal;
		Vector3 rayEnd;
		public override void _PhysicsProcess(float delta)
		{
			if (station is null) return;

			PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
			Vector2 mousePosition = GetViewport().GetMousePosition();
			rayOrigin = camera.ProjectRayOrigin(mousePosition);
			rayNormal = camera.ProjectRayNormal(mousePosition);
			rayEnd = rayOrigin + rayNormal * rayLength;
			Godot.Collections.Dictionary intersection = spaceState.IntersectRay(rayOrigin, rayEnd);

			if (intersection.Count != 0)
			{
				var pos = (Vector3)intersection["position"];
				station.Translation = pos;
			}
		}
	}
}