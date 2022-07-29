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
		private readonly PackedScene stationScene = GD.Load<PackedScene>("res://Scenes/Stations/Station.tscn");
		private List<Cell> cells;
		private Spatial stations;
		private RailContainer railContainer;
		private Events events;
		private Spatial blueprint;
		private Camera camera;
		private bool canBuild = false;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		public void Init(List<Cell> cells, Camera camera, Spatial stations, RailContainer railsContainer)
		{
			this.cells = cells;
			this.stations = stations;
			this.railContainer = railsContainer;
			this.camera = camera;
		}

		public override void _PhysicsProcess(float delta)
		{
			if (Global.MainButtonMode == MainButtonType.BuildStation)
				UpdateBlueprint();
		}
		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton ev && ev.IsActionPressed("lmb"))
			{
				if (!(blueprint is null) && canBuild)
				{
					PlaceStation();
				}
			}

			if (!(blueprint is null) && @event.IsActionPressed("Rotate"))
				blueprint.Rotate(Vector3.Up, Mathf.Pi / 2);
		}

		private void PlaceStation()
		{
			var station = stationScene.Instance<Spatial>();
			station.RemoveChild(station.GetNode("Base"));
			station.Translation = blueprint.Translation;
			station.Rotation = blueprint.Rotation;
			station.GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = false;

			var stationRailPath = station.GetNode<RailPath>("RailPath");
			var bpRailPathClone = blueprint.GetNode<RailPath>("RailPath")
				.Clone();

			var f = bpRailPathClone.Curve.First().Rotated(Vector3.Up, blueprint.Rotation.y);
			var s = bpRailPathClone.Curve.Last().Rotated(Vector3.Up, blueprint.Rotation.y);
			bpRailPathClone.Curve = RailCurve.GetSimpleCurve(f, s);

			bpRailPathClone.Translation = blueprint.Translation;

			station.RemoveChild(stationRailPath);
			railContainer.AddRailPath(bpRailPathClone);

			stations.AddChild(station);
		}

		private void UpdateBlueprint()
		{
			if (blueprint is null) return;

			//set blueprint position
			var pos = this.GetIntersection(camera);
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
				ResetBlueprint();
				return;
			}

			if (Global.MainButtonMode == MainButtonType.BuildStation)
			{
				Global.MainButtonMode = MainButtonType.None;
				ResetBlueprint();
				return;
			}

			Global.MainButtonMode = MainButtonType.BuildStation;
			InitBlueprint();
		}

		private void ResetBlueprint()
		{
			blueprint?.QueueFree();
			blueprint = null;
		}

		private void InitBlueprint()
		{
			blueprint = stationScene.Instance<Spatial>();
			blueprint.GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = true;
			AddChild(blueprint);
		}
	}
}
