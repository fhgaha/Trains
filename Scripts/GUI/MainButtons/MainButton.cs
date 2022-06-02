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
		}

		private void onButtonPressed()
		{
			MainButtonType buttonType = 0;
			switch (Text)
			{
				case "BS": buttonType = MainButtonType.BuildStation; break;
			}

			events.EmitSignal(nameof(Events.MainButtonPressed), buttonType);
			//GD.Print("MainButton onButtonPressed");
			//toggle build station mode, show station asset instead of mouse cursor, on press place asset on floor
		}
	}
}