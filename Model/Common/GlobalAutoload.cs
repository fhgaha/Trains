using System.ComponentModel;
using Godot;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class GlobalAutoload : Node
	{
		[Export]
		private float transportationCost
		{
			get
			{
				GD.Print(Global.TransportationCost);
				return Global.TransportationCost;
			}

			set
			{
				Global.TransportationCost = value;
				GD.Print(Global.TransportationCost);
			}
		}
		[Export] private float moveTreshold { get => Global.MoveTreshold; set => Global.MoveTreshold = value; }
		[Export] private float transportationAmount { get => Global.TransportationAmount; set => Global.TransportationAmount = value; }
		[Export] private float priceDecay { get => Global.PriceDecay; set => Global.PriceDecay = value; }
		[Export]
		private bool Reset
		{
			get
			{
				return reset;
			}
			set
			{
				transportationCost = default;
			}
		}

		private bool reset;
	}
}