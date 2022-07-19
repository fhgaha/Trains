using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;
using System.Linq;

namespace Trains
{
	public class MapAndButtons : HBoxContainer
	{
		private Events events;
		private Dictionary<MainButtonType, Control> buttonMenuDict;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));

			buttonMenuDict = new Dictionary<MainButtonType, Control>
			{
				[MainButtonType.BuildRail] = GetNode<Control>("BuildRailMenu"),
				[MainButtonType.BuildStation] = GetNode<Control>("BuildStationMenu"),
				[MainButtonType.ShowProductMap] = GetNode<Control>("ProductsMenu")
			};

			buttonMenuDict.Values.ToList().ForEach(menu => menu.Visible = false);
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			HideButtonsExeptSpecified(buttonType);
		}

		private void HideButtonsExeptSpecified(MainButtonType buttonType)
		{
			buttonMenuDict.Values
				.Where(menu => menu != buttonMenuDict[buttonType])
				.ToList()
				.ForEach(menu => menu.Visible = false);

			buttonMenuDict[buttonType].Visible = !buttonMenuDict[buttonType].Visible;
		}
	}
}