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
	public class RailBuilder : MapObjectBuilder
	{
		private Curve3D firstSegment;

		private bool duringBuilding = false;
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
		}

		protected override void PlaceObject(Vector3 position, Vector3 rotation)
		{
			var rail = scene.Instance<Spatial>();
			//rail.RemoveChild(rail.GetNode("Base"));
			//rail.Translation = position;
			//rail.Rotation = rotation;
			//rail.GetNode<StaticBody>("StaticBody").CollisionLayer = 0;
			//rail.GetNode<CollisionShape>("StaticBody/CollisionShape").Disabled = false;
			objectHolder.AddChild(rail);
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
			// Cell closestCell = cells.Aggregate((curMin, c)
			// 	=> c.Translation.DistanceSquaredTo(pos) < curMin.Translation.DistanceSquaredTo(pos) ? c : curMin);
			// blueprint.Translation = closestCell.Translation;

			//set base color
			//var collider = blueprint.GetNode<Area>("Base/Area");
			//var bodies = collider.GetOverlappingBodies();
			//canBuild = !(bodies.Count > 0);
			blueprint.Translation = pos;
			canBuild = true;
			var baseMaterial = (SpatialMaterial)blueprint.GetNode<MeshInstance>("MeshInstance").GetSurfaceMaterial(0);
			baseMaterial.AlbedoColor = canBuild ? yellow : red;

		}		

		protected override void onMainButtonPressed(MainButtonType buttonType)
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
		}

		private void CalculateTrajectory()
		{
			int numPoints = 50;
			float gravity = -9.8f;

			var points = new List<Vector2>();
			var startPos = GetNode<Sprite>("start").GlobalPosition;
			var endPos = GetNode<Sprite>("end").GlobalPosition;
			var startEndDir = (endPos - startPos).Normalized();
			var DOT = Vector2.Right.Dot(startEndDir);   //-1, 0 or 1
			var angle = 90 - 45 * DOT;

			var xDist = endPos.x - startPos.x;
			var yDist = -1f * (endPos.y - startPos.y);

			var speed = Sqrt(
				((0.5f * gravity * xDist * xDist) / Pow(Cos(Pi / 180 * angle), 2f))
				/ (yDist - (Tan(Pi / 180 * angle) * xDist)));   
			var xComp = Cos(Pi / 180 * angle) * speed;
			var yComp = Sin(Pi / 180 * angle) * speed;

			var totalTime = xDist / xComp;

			for (int i = 0; i < numPoints; i++)
			{
				var time = totalTime * i / numPoints;
				var dx = time * xComp;
				var dy = -1f * (time * yComp + 0.5f * gravity * time * time);
				points.Add(startPos + new Vector2(dx, dy));
			}

			GetNode<Line2D>("Line2D").Points = points.ToArray();
		}
	}
}
