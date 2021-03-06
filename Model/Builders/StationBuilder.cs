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
		[Export] private readonly PackedScene stationScene;
		[Export] private readonly PackedScene railPathScene;
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
					PlaceRailPath();
				}
			}

			if (!(blueprint is null) && @event.IsActionPressed("Rotate"))
				blueprint.Rotate(Vector3.Up, Mathf.Pi / 2);
		}

		private void PlaceStation()
		{
			var station = stationScene.Instance<Spatial>();
			station.RemoveChild(station.GetNode("Base"));
			station.GlobalTransform = blueprint.GlobalTransform;
			station.GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = false;

			var stationPath = station.GetNode<RailPath>("RailPath");
			station.RemoveChild(stationPath);
			stations.AddChild(station);
		}

		private void PlaceRailPath()
		{
			//unite this with RailPath.InitOnPlacement(Path blueprint) maybe?
			var bpRailPath = blueprint.GetNode<RailPath>("RailPath");
			var path = railPathScene.Instance<RailPath>();
			path.Curve = RailCurve.GetFrom(bpRailPath.Curve);
			path.GlobalTransform = bpRailPath.GlobalTransform;
			path.Rotation = Vector3.Zero;

			var curve = (RailCurve)path.Curve;

			//for some reason if i leave
			//curve.Rotate(Vector3.Up, blueprint.Rotation.y);
			//calculator counts centers of left/right circles on opposite positions
			curve.Rotate(Vector3.Up, -blueprint.Rotation.y);

			railContainer.AddRailPath(path);
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
			var bpPath = blueprint.GetNode<RailPath>("RailPath");
			bpPath.Curve = RailCurve.GetFrom(bpPath.Curve);
			AddChild(blueprint);
			blueprint.Name = "blueprint";
		}
	}
}
