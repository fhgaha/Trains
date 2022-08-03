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
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			if (mode == MainButtonType.BuildTrain)
			{
				Visible = true;
			}
			else
			{
				Visible = false;
			}
		}
	}
}