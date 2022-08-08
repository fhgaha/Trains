using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class BuildTrainMenu : Control
	{
		[Export] private float zoom = 1.5f;
		[Export] private NodePath acceptBtn;
		[Export] private NodePath cancelBtn;
		private PackedScene selectStationButtonScene
			= GD.Load<PackedScene>("res://Scenes/GUI/BuildTrainMenu/MinimapStationButton.tscn");

		private Events events;
		private TextureRect minimapTexture;
		private Dictionary<TextureButton, Station> btnStationDict;
		List<Station> stationsToConnect;

		public override void _Ready()
		{
			minimapTexture = GetNode<TextureRect>("MarginContainer/GridContainer/StationSelectionMinimap/TextureRect");
			btnStationDict = new Dictionary<TextureButton, Station>();
			stationsToConnect = new List<Station>();

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));
			GetNode<Button>(acceptBtn).Connect("pressed", this, nameof(onAcceptButtonPressed));
			GetNode<Button>(cancelBtn).Connect("pressed", this, nameof(onCancelButtonPressed));
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			if (mode == MainButtonType.BuildTrain)
			{
				Visible = true;

				//build select buttons
				const float scaleCoeff = 5f;
				var stations = GetTree().GetNodesInGroup("Stations").Cast<Station>().ToList();

				foreach (var station in stations)
				{
					var btn = selectStationButtonScene.Instance<TextureButton>();
					btn.RectPosition = station.Translation.ToVec2() * scaleCoeff;

					minimapTexture.AddChild(btn);
					btnStationDict.Add(btn, station);

					btn.Connect("pressed", this, nameof(onSelectionButtonPressed), new Godot.Collections.Array { btn });
				}
			}
			else
			{
				Visible = false;

				btnStationDict.Clear();
				minimapTexture.RemoveAllChildren();
			}
		}

		int btnNumber = 1;
		private void onSelectionButtonPressed(TextureButton btn)
		{
			if (btn.Pressed)
			{
				if (StationIsSelectable(btnStationDict[btn]))
				{
					stationsToConnect.Add(btnStationDict[btn]);
					btn.GetNode<Label>("Number").Text = btnNumber.ToString();
					btnNumber++;
				}
				else
				{
					btn.Pressed = false;
				}
			}
			else
			{
				stationsToConnect.Remove(btnStationDict[btn]);
				btn.GetNode<Label>("Number").Text = "";
				btnNumber--;
			}
		}

		private bool StationIsSelectable(Station station)
		{
			//statioin is selectable if it is only one on map or there are connected stations by road



			return true;
		}

		private void onAcceptButtonPressed()
		{
			events.EmitSignal(nameof(Events.StationsAreSelected), stationsToConnect);
			GD.Print("onAcceptButtonPressed");
		}

		private void onCancelButtonPressed()
		{
			stationsToConnect.Clear();
			GD.Print("onCancelButtonPressed");
		}
	}
}