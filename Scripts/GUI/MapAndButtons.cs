using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;
using System.Linq;

namespace Trains
{
	public class MapAndButtons : Control
	{
		private Events events;
		private Dictionary<MainButtonType, Control> buttonMenuDict;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonModeChanged), this, nameof(onMainButtonModeChanged));

			buttonMenuDict = new Dictionary<MainButtonType, Control>
			{
				[MainButtonType.BuildRail] = GetNode<Control>("HBoxContainer/BuildRailMenu"),
				[MainButtonType.BuildStation] = GetNode<Control>("HBoxContainer/BuildStationMenu"),
				[MainButtonType.BuildTrain] = GetNode<Control>("BuildTrainMenu"),
				[MainButtonType.ShowProductMap] = GetNode<Control>("HBoxContainer/ProductsMenu")
			};

			buttonMenuDict.Values.ToList().ForEach(menu => menu.Visible = false);
		}

		private void onMainButtonModeChanged(MainButtonType mode)
		{
			HideMenusExeptSpecified(mode);
		}

		private void HideMenusExeptSpecified(MainButtonType mode)
		{
			buttonMenuDict.Values
				.ToList()
				.ForEach(menu => menu.Visible = false);

			if (mode != MainButtonType.None)
			{
				buttonMenuDict[mode].Visible = true;
			}
		}
	}
}
