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
		private Color yellow = new Color("86e3db6b");
		private Color red = new Color("86e36b6b");
		private List<Cell> cells;
		private Spatial stations;
		private Events events;
		private PackedScene stationScene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");
		private Spatial blueprint;
		private bool canBuild = false;
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
			UpdateBlueprint();
		}

		bool validBuildPlace = false;
		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton ev && ev.IsActionPressed("lmb"))
			{
				if (!(blueprint is null) && canBuild)
				{
					PlaceStation(blueprint.Translation, blueprint.Rotation);
				}
			}

			if (!(blueprint is null) && @event.IsActionPressed("Rotate"))
			{
				blueprint.Rotate(Vector3.Up, Mathf.Pi / 2);
			}
		}

		public void PlaceStation(Vector3 position, Vector3 rotation)
		{
			var station = stationScene.Instance<Spatial>();
			station.RemoveChild(station.GetNode("Base"));
			station.Translation = position;
			station.Rotation = rotation;
			station.GetNode<StaticBody>("StaticBody").CollisionLayer = 0;
			station.GetNode<CollisionShape>("StaticBody/CollisionShape").Disabled = false;
			stations.AddChild(station);
		}

		private void UpdateBlueprint()
		{
			if (blueprint is null) return;

			//set blueprint position
			PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
			Vector2 mousePosition = GetViewport().GetMousePosition();
			Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
			Vector3 rayNormal = camera.ProjectRayNormal(mousePosition);
			Vector3 rayEnd = rayOrigin + rayNormal * rayLength;
			var intersection = spaceState.IntersectRay(rayOrigin, rayEnd);

			if (intersection.Count == 0) return;

			var pos = (Vector3)intersection["position"];
			Cell closestCell = cells.Aggregate((curMin, c)
				=> c.Translation.DistanceSquaredTo(pos) < curMin.Translation.DistanceSquaredTo(pos) ? c : curMin);
			blueprint.Translation = closestCell.Translation;

			//set base color
			var collider = blueprint.GetNode<Area>("Base/Area");
			var bodies = collider.GetOverlappingBodies();
			canBuild = !(bodies.Count > 0);
			var baseMaterial = (SpatialMaterial)blueprint.GetNode<MeshInstance>("Base").GetSurfaceMaterial(0);
			baseMaterial.AlbedoColor = canBuild ? yellow : red;
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
