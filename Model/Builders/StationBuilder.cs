using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Builders
{
	public class StationBuilder : Spatial
	{
		private List<Cell> cells;
		private Events events;
		private PackedScene stationScene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");
		private Spatial station;
		private const float rayLength = 1000f;
		private Camera camera;

		public void Init(List<Cell> cells, Camera camera)
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

			if (buttonType != MainButtonType.BuildStation)
			{
				station?.QueueFree();
				return;
			}

			if (Global.MainButtonMode is MainButtonType.BuildStation) Global.MainButtonMode = null;
			else Global.MainButtonMode = MainButtonType.BuildStation;

			if (!(Global.MainButtonMode is MainButtonType.BuildStation))
			{
				station?.QueueFree();
				return;
			}

			station = stationScene.Instance<Spatial>();
			AddChild(station);
		}

		public override void _PhysicsProcess(float delta)
		{
			if (!(Global.MainButtonMode is MainButtonType.BuildStation)) return;
			SetBlueprintPosition();
		}

		private void SetBlueprintPosition()
		{
			if (station is null) return;

			PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
			Vector2 mousePosition = GetViewport().GetMousePosition();
			Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
			Vector3 rayNormal = camera.ProjectRayNormal(mousePosition);
			Vector3 rayEnd = rayOrigin + rayNormal * rayLength;
			var intersection = spaceState.IntersectRay(rayOrigin, rayEnd);

			if (intersection.Count == 0) return;

			var pos = (Vector3)intersection["position"];
			var closestCell = cells.OrderBy(c => c.Translation.DistanceSquaredTo(pos)).First();
			station.Translation = closestCell.Translation;
		}
	}
}