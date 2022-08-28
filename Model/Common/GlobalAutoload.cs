using System.ComponentModel;
using Godot;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class GlobalAutoload : Node
	{
		[Export] private float transportationCost { get => Global.TransportationCost; set => Global.TransportationCost = value; }
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
				transportationCost = transportationCostDefault;
				moveTreshold = moveTresholdDefault;
				transportationAmount = transportationAmountDefault;
				priceDecay = priceDecayDefault;
			}
		}

		private bool reset;

		private float transportationCostDefault;
		private float moveTresholdDefault;
		private float transportationAmountDefault;
		private float priceDecayDefault;

		public override void _Ready()
		{
			transportationCostDefault = Global.TransportationCost;
			moveTresholdDefault = Global.MoveTreshold;
			transportationAmountDefault = Global.TransportationAmount;
			priceDecayDefault = Global.PriceDecay;
		}
	}
}