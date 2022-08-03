using Godot;
using System;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class MainButton : Button
	{
		private Events events;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			Connect("pressed", this, nameof(onButtonPressed));
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));
		}

		private void onButtonPressed()
		{
			events.EmitSignal(nameof(Events.MainButtonPressed), GetThisButtonType());
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			if (ThisButtonIsPressedSecondTimeInRow())
			{
				Global.MainButtonMode = MainButtonType.None;
				events.EmitSignal(nameof(Events.MainButtonModeChanged), MainButtonType.None);
				Unpress();
			}
			else if (ThisButtonIsPressedFirstTime())
			{
				Global.MainButtonMode = buttonType;
				events.EmitSignal(nameof(Events.MainButtonModeChanged), buttonType);
			}
			else
			{
				Unpress();
			}

			bool ThisButtonIsPressedSecondTimeInRow()
				=> buttonType == GetThisButtonType() && GetThisButtonType() == Global.MainButtonMode;

			bool ThisButtonIsPressedFirstTime() => buttonType == GetThisButtonType();
		}

		private MainButtonType GetThisButtonType()
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
			Pressed = false;
		}
	}
}
