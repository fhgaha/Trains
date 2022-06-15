using System.Collections.Generic;
using Godot;
using System;

namespace Trains.Model.Common
{
	public static class Extensions
	{
		//Node
		public static void RemoveAllChildren(this Node node)
		{
			foreach (Node child in node.GetChildren())
			{
				node.RemoveChild(child);
				child.QueueFree();
			}
		}


		//Vector
		public static Vector3 ToVec3(this Vector2 vec) => new Vector3(vec.x, 0, vec.y);
		public static Vector2 ToVec2(this Vector3 vec) => new Vector2(vec.x, vec.z);


		//Curve3D
		public static Vector3 First(this Curve3D curve) => curve.GetPointPosition(0);
		public static Vector3 Last(this Curve3D curve) => curve.GetPointPosition(curve.GetPointCount() - 1);

		public static List<Vector3> TakeLast(this Curve3D curve, int amount)
		{
			var list = new List<Vector3>();
			var startIndex = curve.GetPointCount() - 1;
			var lastIndex = curve.GetPointCount() - amount;
			if (lastIndex < 0) throw new ArgumentException(
				"amount " + amount + " cannot be bigger that curve points count " + curve.GetPointCount());

			for (int i = startIndex; i >= lastIndex; i--)
				list.Add(curve.GetPointPosition(i));

			return list;
		}

		//Spatial
		public static Vector3 GetIntersection(this Spatial spatial, Camera camera, float rayLength)
		{
			PhysicsDirectSpaceState spaceState = spatial.GetWorld().DirectSpaceState;
			Vector2 mousePosition = spatial.GetViewport().GetMousePosition();
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
	}
}
