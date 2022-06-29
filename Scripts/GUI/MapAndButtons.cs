using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class MapAndButtons : HBoxContainer
	{
		private Events events;
		private Control productsMenu;
		private Dictionary<MainButtonType, Control> btnToBtnZone;

		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			events.Connect(nameof(Events.MainButtonPressed), this, nameof(onMainButtonPressed));

			btnToBtnZone = new Dictionary<MainButtonType, Control>
			{
				[MainButtonType.ShowProductMap] = GetNode<Control>("ProductsMenu")
			};

			
		}

		private void onMainButtonPressed(MainButtonType buttonType)
		{
			btnToBtnZone[buttonType].Visible = !btnToBtnZone[buttonType].Visible;

			// switch (buttonType)
			// {
			// 	case MainButtonType.BuildRail:
			// 		break;
			// 	case MainButtonType.BuildStation:
			// 		break;
			// 	case MainButtonType.ShowProductMap:
			// 		productsMenu.Visible = !productsMenu.Visible;
			// 		break;
			// }
		}
	}
}