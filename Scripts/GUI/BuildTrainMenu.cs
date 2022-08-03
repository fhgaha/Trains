using Godot;
using System;
using System.Linq;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class BuildTrainMenu : Control
	{
		[Export] private float zoom = 1.5f;
		private PackedScene selectStationButtonScene
			= GD.Load<PackedScene>("res://Scenes/GUI/BuildTrainMenu/MinimapStationButton.tscn");

		private Events events;
		private TextureRect minimapTexture;

		public override void _Ready()
		{
			minimapTexture = GetNode<TextureRect>("MarginContainer/GridContainer/StationSelectionMinimap/TextureRect");

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			if (IsWrongButtonPressed(buttonType)) return;

			Global.MainButtonMode = MainButtonType.BuildTrain;
			//do
			var stations = GetTree().GetNodesInGroup("Stations").Cast<Station>();
			GD.Print(stations.Count());
			//foreach station add button
		}

		private bool IsWrongButtonPressed(MainButtonType buttonType)
		{
			if (buttonType != MainButtonType.BuildTrain)
			{
				return true;
			}

			if (Global.MainButtonMode == MainButtonType.BuildTrain)
			{
				Global.MainButtonMode = MainButtonType.None;
				return true;
			}
			return false;
		}
	}
}