using System.Collections.Generic;
using Godot;
using System;
using Trains.Model.Builders;

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

		//Vector2
		public static Vector3 ToVec3(this Vector2 vec) => new Vector3(vec.x, 0, vec.y);

		public static bool IsEqualApprox(this Vector2 first, Vector2 second, float accuracy)
		{
			var equalByX = Math.Abs(first.x - second.x) < accuracy;
			var equalByY = Math.Abs(first.y - second.y) < accuracy;
			return equalByX && equalByY;
		}

		//Vector3
		public static Vector2 ToVec2(this Vector3 vec) => new Vector2(vec.x, vec.z);

		public static bool IsEqualApprox(this Vector3 first, Vector3 second, float accuracy)
		{
			var equalByX = Math.Abs(first.x - second.x) < accuracy;
			var equalByY = Math.Abs(first.y - second.y) < accuracy;
			var equalByZ = Math.Abs(first.z - second.z) < accuracy;
			return equalByX && equalByY && equalByZ;
		}

		//Curve3D
		public static Vector3 First(this Curve3D curve)
		{
			if (curve.GetPointCount() == 0) return Vector3.Zero;

			return curve.GetPointPosition(0);
		}

		public static Vector3 Last(this Curve3D curve)
		{
			if (curve.GetPointCount() == 0) return Vector3.Zero;

			return curve.GetPointPosition(curve.GetPointCount() - 1);
		}

		public static List<Vector3> TakeFirst(this Curve3D curve, int amount)
		{
			if (amount > curve.GetPointCount())
			{
				throw new ArgumentException(
				"amount " + amount + " cannot be bigger that curve points count " + curve.GetPointCount());
			}

			var list = new List<Vector3>();

			for (int i = 0; i < amount; i++)
				list.Add(curve.GetPointPosition(i));

			list.Reverse();
			return list;
		}

		public static List<Vector3> TakeLast(this Curve3D curve, int amount)
		{
			var list = new List<Vector3>();
			var startIndex = curve.GetPointCount() - 1;
			var lastIndex = curve.GetPointCount() - amount;
			if (lastIndex < 0)
			{
				throw new ArgumentException(
				"amount " + amount + " cannot be bigger that curve points count " + curve.GetPointCount());
			}

			for (int i = startIndex; i >= lastIndex; i--)
				list.Add(curve.GetPointPosition(i));

			list.Reverse();
			return list;
		}

		public static List<CurveSegment> ToSegments(this Curve3D curve)
		{
			var points = curve.Tessellate();
			var segments = new List<CurveSegment>();

			for (int i = 1; i < points.Length; i++)
			{
				var segment = new CurveSegment(points[i - 1], points[i]);
				segments.Add(segment);
			}

			return segments;
		}

		public static Vector3[] ToArray(this Curve3D curve)
		{
			var amount = curve.GetPointCount();
			var points = new Vector3[amount];
			for (int i = 0; i < amount; i++)
				points[i] = curve.GetPointPosition(i);
			return points;
		}

		public static void PrintPoints(this Curve3D curve)
		{
			for (int i = 0; i < curve.GetPointCount(); i++)
			{
				GD.Print(curve.GetPointPosition(i));
			}
		}

		//Spatial
		public static Vector3 GetIntersection(this Spatial spatial, Camera camera)
		{
			PhysicsDirectSpaceState spaceState = spatial.GetWorld().DirectSpaceState;
			Vector2 mousePosition = spatial.GetViewport().GetMousePosition();
			Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
			Vector3 rayNormal = camera.ProjectRayNormal(mousePosition);
			Vector3 rayEnd = rayOrigin + rayNormal * Global.RayLength;
			var intersection = spaceState.IntersectRay(rayOrigin, rayEnd);

			if (intersection.Count == 0)
			{
				//GD.Print("camera ray did not collide with an object.");
				return Vector3.Zero;
			}

			var pos = (Vector3)intersection["position"];
			return pos;
		}
	}
}
