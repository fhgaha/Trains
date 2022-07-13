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
		private readonly PackedScene stationScene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");
		private Spatial blueprint;
		private const float rayLength = 1000f;
		private Camera camera;
		private bool canBuild = false;

		public void Init(List<Cell> cells, Camera camera, Spatial stations)
		{
			this.cells = cells;
			this.stations = stations;
			this.camera = camera;
			//cant get events if not in the scene
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		public override void _PhysicsProcess(float delta)
		{
			if (!(Global.MainButtonMode is MainButtonType.BuildStation)) return;
			UpdateBlueprint();
		}
		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton ev && ev.IsActionPressed("lmb"))
			{
				if (!(blueprint is null) && canBuild)
				{
					//place station
					var station = stationScene.Instance<Spatial>();
					station.RemoveChild(station.GetNode("Base"));
					station.Translation = blueprint.Translation;
					station.Rotation = blueprint.Rotation;
					station.GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = false;
					stations.AddChild(station);
				}
			}

			if (!(blueprint is null) && @event.IsActionPressed("Rotate"))
				blueprint.Rotate(Vector3.Up, Mathf.Pi / 2);
		}

		private void UpdateBlueprint()
		{
			if (blueprint is null) return;

			//set blueprint position
			var pos = this.GetIntersection(camera, rayLength);
			var closestCell = cells.OrderBy(c => c.Translation.DistanceSquaredTo(pos)).First();
			blueprint.Translation = closestCell.Translation;

			//set base color
			var area = blueprint.GetNode<Area>("Base/Area");
			var bodies = area.GetOverlappingBodies().Cast<Node>().Where(b => b.IsInGroup("Obstacles"));
			var baseMaterial = (SpatialMaterial)blueprint.GetNode<MeshInstance>("Base").GetSurfaceMaterial(0);
			canBuild = !bodies.Any();
			baseMaterial.AlbedoColor = canBuild ? yellow : red;
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			if (buttonType != MainButtonType.BuildStation)
			{
				blueprint?.QueueFree();
				blueprint = null;
				return;
			}

			if (Global.MainButtonMode is MainButtonType.BuildStation)
				Global.MainButtonMode = null;
			else
				Global.MainButtonMode = MainButtonType.BuildStation;

			if (!(Global.MainButtonMode is MainButtonType.BuildStation))
			{
				blueprint?.QueueFree();
				blueprint = null;
				return;
			}

			blueprint = stationScene.Instance<Spatial>();
			blueprint.GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = true;
			AddChild(blueprint);
		}
	}
}
