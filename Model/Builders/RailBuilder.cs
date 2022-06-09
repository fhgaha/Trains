using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;
using static Godot.Mathf;

namespace Trains.Model.Builders
{
	public class RailBuilder : Spatial
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
		protected MainButtonType mainButtonType;

		//in editor for CSGPolygon property Path Local should be On to place polygon where the cursor is with no offset
		public void Init(List<Cell> cells, Camera camera, Spatial objectHolder, PackedScene scene)
		{
			this.cells = cells;
			this.objectHolder = objectHolder;
			this.camera = camera;
			this.scene = scene;
			events = GetNode<Events>("/root/Events");
			//GD.Print("StationBuilder: " + events);
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		public override void _PhysicsProcess(float delta)
		{
			if (!(Global.MainButtonMode is MainButtonType.BuildRail)) return;
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

		protected void PlaceObject(Vector3 position, Vector3 rotation)
		{
			var station = scene.Instance<Spatial>();
			station.RemoveChild(station.GetNode("Base"));
			station.Translation = position;
			station.Rotation = rotation;
			station.GetNode<StaticBody>("StaticBody").CollisionLayer = 0;
			station.GetNode<CollisionShape>("StaticBody/CollisionShape").Disabled = false;
			objectHolder.AddChild(station);
		}

		protected virtual void UpdateBlueprint()
		{
			if (blueprint is null) return;

			var pos = GetIntersection();
			blueprint.Translation = pos;

			//set blueprint position
			// var pos = GetIntersection();
			// Cell closestCell = cells.Aggregate((curMin, c)
			// 	=> c.Translation.DistanceSquaredTo(pos) < curMin.Translation.DistanceSquaredTo(pos) ? c : curMin);
			// blueprint.Translation = closestCell.Translation;

			//set base color
			// var collider = blueprint.GetNode<Area>("Base/Area");
			// var bodies = collider.GetOverlappingBodies();
			// canBuild = !(bodies.Count > 0);
			// var baseMaterial = (SpatialMaterial)blueprint.GetNode<MeshInstance>("Base").GetSurfaceMaterial(0);
			// baseMaterial.AlbedoColor = canBuild ? yellow : red;
		}

		private Vector3 GetIntersection()
		{
			PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
			Vector2 mousePosition = GetViewport().GetMousePosition();
			Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
			Vector3 rayNormal = camera.ProjectRayNormal(mousePosition);
			Vector3 rayEnd = rayOrigin + rayNormal * rayLength;
			var intersection = spaceState.IntersectRay(rayOrigin, rayEnd);

			if (intersection.Count == 0) 
			{
				GD.Print("camera ray did not collide with an object.");
				return Vector3.Zero;
			}

			var pos = (Vector3)intersection["position"];
			return pos;
		}

		protected virtual void onMainButtonPressed(MainButtonType buttonType)
		{
			//GD.Print("onMainButtonPressed");
			//initialize blueprint

			if (buttonType != MainButtonType.BuildRail)
			{
				blueprint?.QueueFree();
				return;
			}

			if (Global.MainButtonMode is MainButtonType.BuildRail) Global.MainButtonMode = null;
			else Global.MainButtonMode = MainButtonType.BuildRail;

			if (!(Global.MainButtonMode is MainButtonType.BuildRail))
			{
				blueprint?.QueueFree();
				return;
			}

			blueprint = scene.Instance<Spatial>();
			AddChild(blueprint);

			//cube sample
			// blueprint = new Spatial();
			// var m = new MeshInstance();
			// m.Mesh = new CubeMesh();
			// m.Scale = new Vector3(0.2f, 0.2f, 0.2f);
			// blueprint.AddChild(m);
			// AddChild(blueprint);
		}
	}
}
