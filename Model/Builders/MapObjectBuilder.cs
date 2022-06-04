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
	public class MapObjectBuilder : Spatial
	{
		protected Color yellow = new Color("86e3db6b");
		protected Color red = new Color("86e36b6b");
		protected List<Cell> cells;
		protected Events events;
		protected PackedScene scene;
		protected Spatial blueprint;
		protected bool canBuild = false;
		protected const float rayLength = 1000f;
		protected Camera camera;
		protected Spatial objectHolder;

		protected virtual void PlaceObject(Vector3 position, Vector3 rotation) { }

		protected virtual void UpdateBlueprint()
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

		protected virtual void onMainButtonPressed(MainButtonType buttonType)
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
