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
		private Station blueprint;
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
			var station = stationScene.Instance<Station>();
			station.Init(blueprint);
			stations.AddChild(station);
		}

		private void PlaceRailPath()
		{
			var bpRailPath = blueprint.GetNode<RailPath>("RailPath");
			var path = railPathScene.Instance<RailPath>();
			path.InitOnPlacementFromStationBuilder(bpRailPath);

			//for some reason if i leave
			//curve.Rotate(Vector3.Up, blueprint.Rotation.y);
			//calculator counts centers of left/right circles on opposite positions when path is horizontal 
			//and goes left
			var curve = (RailCurve)path.Curve;
			curve.Rotate(Vector3.Up, -blueprint.Rotation.y);

			//for some reason if add path right after instance the path all station paths rotates. 
			//but if i add it in the end of this method station paths do not rotate.
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
			if (IsPressedWrongButton(buttonType)) return;

			Global.MainButtonMode = MainButtonType.BuildStation;
			InitEmptyBlueprint();
		}

		private bool IsPressedWrongButton(MainButtonType buttonType)
		{
			if (buttonType != MainButtonType.BuildStation)
			{
				ResetBlueprint();
				return true;
			}

			if (Global.MainButtonMode == MainButtonType.BuildStation)
			{
				Global.MainButtonMode = MainButtonType.None;
				ResetBlueprint();
				return true;
			}
			return false;
		}

		private void ResetBlueprint()
		{
			blueprint?.QueueFree();
			blueprint = null;
		}

		private void InitEmptyBlueprint()
		{
			blueprint = stationScene.Instance<Station>();
			blueprint.GetNode<CollisionShape>("Obstacle/CollisionShape").Disabled = true;
			var bpPath = blueprint.GetNode<RailPath>("RailPath");
			bpPath.Curve = RailCurve.GetFrom(bpPath.Curve);
			AddChild(blueprint);
			blueprint.Name = "blueprint";
		}
	}
}
