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
	}
}
