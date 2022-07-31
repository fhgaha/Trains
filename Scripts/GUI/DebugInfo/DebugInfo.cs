using Godot;
using System;

namespace Trains.Scripts.GUI
{
	public class DebugInfo : VBoxContainer
	{
		public override void _Process(float delta)
		{
			GetNode<Label>("FPSCounter").Text = $"FPS: {Engine.GetFramesPerSecond()}";
			//GetNode<Label>("mousePos").Text = $"mousePos: {}";
		}
	}
}