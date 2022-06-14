using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class RoadsConnection : Spatial
	{
		private PackedScene scene = GD.Load<PackedScene>("res://Scenes/Rails/RailCSG.tscn");
		private Path path;


		public override void _Ready()
		{
			path = GetNode<Path>("RailCSG/Path");
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton evMouseButton && evMouseButton.IsActionPressed("lmb"))
				PlaceRailSegment();
		}

		private void PlaceRailSegment()
		{
			var newPath = scene.Instance<Spatial>();
			AddChild(newPath);
			newPath.Translation = path.Curve.Last() + Vector3.Forward;
			path = newPath.GetNode<Path>("Path");

			GD.Print(path.Curve.Last());
			GD.Print(newPath.Translation);
			GD.Print();
		}
	}
}