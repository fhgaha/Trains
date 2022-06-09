using Godot;

namespace Trains.Model.Common
{
	public static class Extensions
	{
		public static void RemoveAllChildren(this Node node)
		{
			foreach (Node child in node.GetChildren())
			{
				node.RemoveChild(child);
				child.QueueFree();
			}
		}

		// func vec2_vec3(vec3, axis):
		// var array = [vec3.x, vec3.y, vec3.z]
		// array.remove(axis)
		// return Vector2(array[0], array[1])

		// func vec3_vec2(vec2, axis, value):
		// var array = [vec2.x, vec2.y]
		// array.insert(axis, value)
		// return Vector3(array[0], array[1], array[2])
		public static Vector3 ToVec3(this Vector2 vec) => new Vector3(vec.x, 0, vec.y);
		public static Vector2 ToVec2(this Vector3 vec) => new Vector2(vec.x, vec.z);
	}
}
