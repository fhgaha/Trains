using Godot;
using System;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class MainButton : Button
	{
		private Events events;
		private bool wasPressed = false;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			Connect("pressed", this, nameof(onButtonPressed));
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		private void onButtonPressed()
		{
			wasPressed = true;
			events.EmitSignal(nameof(Events.MainButtonPressed), GetButtonType());
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			if (buttonType != GetButtonType())
				Unpress();
		}

		private MainButtonType GetButtonType()
		{
			switch (Text)
			{
				case "BR": return MainButtonType.BuildRail;
				case "BS": return MainButtonType.BuildStation;
				case "BT": return MainButtonType.BuildTrain;
				default: return MainButtonType.ShowProductMap;
			}
		}

		private void Unpress()
		{
			wasPressed = false;
			Pressed = false;
		}
	}
}
