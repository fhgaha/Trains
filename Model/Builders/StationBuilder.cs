using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Builders
{
	//it is required to extend spatial to access GetWorld() method to get mouse position on the grid
	//also connecting signal of GUI
	public class StationBuilder : MapObjectBuilder
	{
		public void Init(List<Cell> cells, Camera camera, Spatial objectHolder, PackedScene scene)
		{
			base.cells = cells;
			base.objectHolder = objectHolder;
			base.camera = camera;
			base.scene = scene;
			events = GetNode<Events>("/root/Events");
			//GD.Print("StationBuilder: " + events);
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
				if (!(blueprint is null) && canBuild)
					PlaceObject(blueprint.Translation, blueprint.Rotation);

			if (!(blueprint is null) && @event.IsActionPressed("Rotate"))
				blueprint.Rotate(Vector3.Up, Mathf.Pi / 2);
		}

		protected override void PlaceObject(Vector3 position, Vector3 rotation)
		{
			var station = scene.Instance<Spatial>();
			station.RemoveChild(station.GetNode("Base"));
			station.Translation = position;
			station.Rotation = rotation;
			station.GetNode<StaticBody>("StaticBody").CollisionLayer = 0;
			station.GetNode<CollisionShape>("StaticBody/CollisionShape").Disabled = false;
			objectHolder.AddChild(station);
		}
	}
}
