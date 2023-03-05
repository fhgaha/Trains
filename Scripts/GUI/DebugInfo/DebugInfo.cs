using Godot;
using System;
using Trains.Model.Common;
using Trains.Model.Grids;

namespace Trains.Scripts.GUI
{
	public class DebugInfo : VBoxContainer
	{
		private Grid grid;
		private Camera camera;

		public void Init(Grid grid, Camera camera)
		{
			this.grid = grid;
			this.camera = camera;
		}

		public override void _Process(float delta)
		{
			GetNode<Label>("FPSCounter").Text = $"FPS: {Engine.GetFramesPerSecond()}";
			GetNode<Label>("mousePos").Text = $"mousePos: {grid.GetIntersection(camera)}";
		}
	}
}