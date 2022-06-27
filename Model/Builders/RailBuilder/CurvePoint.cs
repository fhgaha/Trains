using Godot;
using System;

namespace Trains
{
	public class CurvePoint : Spatial
	{
		public Vector3 Position { get; set; } = Vector3.Zero;
		public Vector3 Direction { get; set; } = Vector3.Zero;

		public CurvePoint() { }

		public CurvePoint(Vector3 position, Vector3 direction)
		{
			Position = position;
			Direction = direction;
		}
	}
}