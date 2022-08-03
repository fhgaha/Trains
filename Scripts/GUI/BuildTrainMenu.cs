using Godot;
using System;

namespace Trains
{
	public class BuildTrainMenu : Control
	{
		[Export] private float zoom = 1.5f;
		private PackedScene selectStationButtonScene 
			= GD.Load<PackedScene>("res://Scenes/GUI/BuildTrainMenu/MinimapStationButton.tscn");

		private TextureRect minimapTexture;

		public override void _Ready()
		{
			minimapTexture = GetNode<TextureRect>("MarginContainer/GridContainer/StationSelectionMinimap/TextureRect");
		}
	}
}