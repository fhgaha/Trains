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
		private Color yellow = new Color("86e3db6b");
		private Color red = new Color("86e36b6b");
		private List<Cell> cells;
		private Events events;
		private PackedScene scene;
		private Spatial blueprint;
		private bool canBuild = false;
		private const float rayLength = 1000f;
		private Camera camera;
		private Spatial objectHolder;	//Rails
		private MainButtonType mainButtonType;
		private Spatial firstSegment = null;

		//in editor for CSGPolygon property Path Local should be "On" to place polygon where the cursor is with no offset

		//build order:
		//1. press BS button, simple straight road will show up following cursor.
		//2. using mouse select a place to build first segment. it cannot be built on obstacle.
		//3. press lmb to place first segment. path will show up from first segment to mouse pos, showing possible path
		//in blueprint mode.
		//4. press lmb again to place blueprint road.
		public void Init(List<Cell> cells, Camera camera, Spatial objectHolder, PackedScene scene)
		{
			this.cells = cells;
			this.objectHolder = objectHolder;
			this.camera = camera;
			this.scene = scene;
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		public override void _PhysicsProcess(float delta)
		{
			if (!(Global.MainButtonMode is MainButtonType.BuildRail)) return;
			UpdateBlueprint();
		}

		private Vector3 start = Vector3.Zero;
		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton evMouseButton && evMouseButton.IsActionPressed("lmb"))
				if (!(blueprint is null) && canBuild && firstSegment is null)
					PlaceObject(blueprint.Translation, blueprint.Rotation);

			if (@event is InputEventMouseMotion evMouseMotion)
			{
				if (!(firstSegment is null))
				{
					//draw trajectory
					DrawTrajectory();
				}
			}
		}

		private void DrawTrajectory()
		{
			//each time build new path and connect with old path
			var path = firstSegment.GetNode<Path>("Path");
			Vector3 end = GetIntersection() - path.GlobalTransform.origin;
			var points = CalculateTrajectory(start.ToVec2(), end.ToVec2(), 5);
			var curve = new Curve3D();
			points.ForEach(p => curve.AddPoint(p.ToVec3()));
			path.Curve = curve;
		}

		protected void PlaceObject(Vector3 position, Vector3 rotation)
		{
			//csg: set collision layer and mask
			var rail = scene.Instance<Spatial>();
			//rail.RemoveChild(rail.GetNode("Base"));
			rail.Translation = position;
			rail.Rotation = rotation;
			//rail.GetNode<CollisionPolygon>("Path/CSGPolygon/Area/CollisionPolygon").Disabled = false;
			objectHolder.AddChild(rail);
			firstSegment = rail;
		}

		private void UpdateBlueprint()
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
			var area = blueprint.GetNode<Area>("Path/CSGPolygon/Area");
			var bodies = area.GetOverlappingBodies().Cast<Node>().Where(b => b.IsInGroup("Obstacles"));
			canBuild = bodies.Count() <= 0;
			var csgMaterial = (SpatialMaterial)blueprint.GetNode<CSGPolygon>("Path/CSGPolygon").Material;
			csgMaterial.AlbedoColor = canBuild ? yellow : red;
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

		private void onMainButtonPressed(MainButtonType buttonType)
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

			//init blueprint
			//can be build new rail or continue existing one
			//for the time let it be always build first ever rail
			blueprint = scene.Instance<Spatial>();
			var csg = blueprint.GetNode<CSGPolygon>("Path/CSGPolygon");
			var collider = blueprint.GetNode<CollisionPolygon>("Path/CSGPolygon/Area/CollisionPolygon");
			collider.Polygon = csg.Polygon;
			AddChild(blueprint);
		}

		private List<Vector2> CalculateTrajectory(Vector2 startPos, Vector2 endPos, int numPoints)
		{
			float gravity = -15f;
			var points = new List<Vector2>();
			var startEndDir = (endPos - startPos).Normalized();
			//this should be changed to previous segment direction?
			var DOT = Vector2.Right.Dot(startEndDir);   //-1, 0 or 1
			var angle = 90 - 45 * DOT;

			var xDist = endPos.x - startPos.x;
			var yDist = -1f * (endPos.y - startPos.y);  //flip y to do calculations in eucludus geometry. y flips back later.

			var speed = Sqrt(
				((0.5f * gravity * xDist * xDist) / Pow(Cos(Pi / 180 * angle), 2f))
				/ (yDist - (Tan(Pi / 180 * angle) * xDist)));
			var xComponent = Cos(Pi / 180 * angle) * speed;
			var yComponent = Sin(Pi / 180 * angle) * speed;

			var totalTime = xDist / xComponent;

			for (int i = 0; i < numPoints; i++)
			{
				var time = totalTime * i / numPoints;
				var dx = time * xComponent;
				var dy = -1f * (time * yComponent + 0.5f * gravity * time * time);
				points.Add(startPos + new Vector2(dx, dy));
			}

			return points;
		}
	}
}