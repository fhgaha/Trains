using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class RailRemover : Spatial
	{
		private Camera camera;

		public void Init(Camera camera)
		{
			this.camera = camera;
		}

		public void Do()
		{
			var mousePos = this.GetIntersection(camera);
		}
	}
}