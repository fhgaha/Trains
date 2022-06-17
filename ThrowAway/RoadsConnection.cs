using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;

namespace Trains
{
	public class RoadsConnection : Spatial
	{
		private Path path;


		public override void _Ready()
		{
			path = GetNode<Path>("RailPath");
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton evMouseButton && evMouseButton.IsActionPressed("lmb"))
				PlaceRailSegment();
		}

		private void PlaceRailSegment()
		{
			path.Curve.AddPoint(path.Curve.Last() + Vector3.Forward);

			var newPath = new List<Vector3>
			{
				new Vector3(path.Curve.Last() + Vector3.Forward),
				new Vector3(path.Curve.Last() + Vector3.Forward + Vector3.Right ),
				new Vector3(path.Curve.Last() + Vector3.Forward + Vector3.Right +  Vector3.Forward )
			};

			newPath.ForEach(p => path.Curve.AddPoint(p));
		}
	}
}
