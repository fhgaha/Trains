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
		private PackedScene selectStationButtonScene
			= GD.Load<PackedScene>("res://Scenes/GUI/BuildTrainMenu/MinimapStationButton.tscn");

		private Events events;
		private TextureRect minimapTexture;
		private Dictionary<TextureButton, Station> btnStationDict;

		public override void _Ready()
		{
			minimapTexture = GetNode<TextureRect>("MarginContainer/GridContainer/StationSelectionMinimap/TextureRect");
			btnStationDict = new Dictionary<TextureButton, Station>();

			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			if (mode == MainButtonType.BuildTrain)
			{
				Visible = true;

				//build select buttons
				const float scaleCoeff = 3f;
				var stations = GetTree().GetNodesInGroup("Stations").Cast<Station>().ToList();

				for (int i = 0; i < stations.Count; i++)
				{
					var station = stations[i];
					var btn = selectStationButtonScene.Instance<TextureButton>();
					btn.RectPosition = station.Translation.ToVec2() * scaleCoeff;
					
					//GD.PrintS("btn:", btn.RectPosition, "station:", station.Translation);
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

		List<Station> stationsToConnect = new List<Station>();
		int index = 1;
		private void onSelectionButtonPressed(TextureButton btn)
		{
			//first press: Pressed, second press: False
			//if pressed - add somewhere, else - remove

			if (btn.Pressed)
			{
				stationsToConnect.Add(btnStationDict[btn]);
				btn.GetNode<Label>("Number").Text = index.ToString();
				index++;
			}
			else
			{
				stationsToConnect.Remove(btnStationDict[btn]);
				btn.GetNode<Label>("Number").Text = "";
				index--;
			}

			stationsToConnect.ForEach(s => GD.Print(s));
			GD.Print();
		}
	}
}