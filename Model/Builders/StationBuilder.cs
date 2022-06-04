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

		protected override void UpdateBlueprint()
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

		protected override void onMainButtonPressed(MainButtonType buttonType)
		{
			//GD.Print("onMainButtonPressed");
			//initialize blueprint

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

			blueprint = scene.Instance<Spatial>();
			AddChild(blueprint);
		}
	}
}
