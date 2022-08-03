using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class TrainContainer : Spatial
	{
		[Export] private PackedScene trainScene;
		private List<Spatial> trains;
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			if (IsWrongButtonPressed(buttonType)) return;

			Global.MainButtonMode = MainButtonType.BuildTrain;
			
			//temp
			//var train = trainScene.Instance<Train>();
		}

		private bool IsWrongButtonPressed(MainButtonType buttonType)
		{
			//other main button is pressed
			if (buttonType != MainButtonType.BuildTrain)
			{
				//Reset something
				return true;
			}

			//"Build Rail" button was pressed and we press it again
			if (Global.MainButtonMode == MainButtonType.BuildTrain)
			{
				Global.MainButtonMode = MainButtonType.None;
				//Reset something
				return true;
			}
			return false;
		}
	}
}