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
		private Spatial stations;
		private Events events;
		private PackedScene stationScene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");
		private Spatial blueprint;
		private const float rayLength = 1000f;
		private Camera camera;

		public void Init(List<Cell> cells, Camera camera, Spatial stations)
		{
			this.cells = cells;
			this.stations = stations;
			this.camera = camera;
			//cant get events if not in the scene
			events = GetNode<Events>("/root/Events");
			GD.Print("StationBuilder: " + events);
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}
		
		public override void _PhysicsProcess(float delta)
		{
			if (!(Global.MainButtonMode is MainButtonType.BuildStation)) return;
			SetBlueprintPosition();
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton ev)
			{
				if (!(blueprint is null) && ev.ButtonIndex == (int)ButtonList.Left)
				{
					GD.Print("ev");
					//place station
					var station = stationScene.Instance<Spatial>();
					station.RemoveChild(station.GetNode("Base"));
					station.Translation = blueprint.Translation;
					stations.AddChild(station);
				}
			}
		}

		private void SetBlueprintPosition()
		{
			if (blueprint is null) return;

			PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
			Vector2 mousePosition = GetViewport().GetMousePosition();
			Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
			Vector3 rayNormal = camera.ProjectRayNormal(mousePosition);
			Vector3 rayEnd = rayOrigin + rayNormal * rayLength;
			var intersection = spaceState.IntersectRay(rayOrigin, rayEnd);

			if (intersection.Count == 0) return;

			var pos = (Vector3)intersection["position"];
			var closestCell = cells.OrderBy(c => c.Translation.DistanceSquaredTo(pos)).First();
			blueprint.Translation = closestCell.Translation;
		}
		
		private void onMainButtonPressed(MainButtonType buttonType)
		{
			//GD.Print("onMainButtonPressed");
			//turn on building mode
			//turn cursor into station asset

			if (buttonType != MainButtonType.BuildStation)
			{
				blueprint?.QueueFree();
				return;
			}

			if (Global.MainButtonMode is MainButtonType.BuildStation) Global.MainButtonMode = null;
			else Global.MainButtonMode = MainButtonType.BuildStation;

			if (!(Global.MainButtonMode is MainButtonType.BuildStation))
			{
				blueprint?.QueueFree();
				return;
			}

			blueprint = stationScene.Instance<Spatial>();
			AddChild(blueprint);
		}
	}
}