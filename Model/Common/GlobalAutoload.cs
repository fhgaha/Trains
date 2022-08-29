using System.ComponentModel;
using System.Linq;
using Godot;
using Trains.Model.Cells;
using Trains.Model.Common;
using static Trains.Model.Common.Enums;

namespace Trains
{
	public class GlobalAutoload : Node
	{
		[Export] private float transportationCost { get => Global.TransportationCost; set => Global.TransportationCost = value; }
		[Export] private float minProductAmount { get => Global.MinProductAmount; set => Global.MinProductAmount = value; }
		[Export] private float maxProductAmount { get => Global.MaxProductAmount; set => Global.MaxProductAmount = value; }
		[Export] private float transportationAmount { get => Global.TransportationAmount; set => Global.TransportationAmount = value; }
		[Export] private float priceDecay { get => Global.PriceDecay; set => Global.PriceDecay = value; }

		[Export]
		private bool Reset
		{
			get => reset;
			set
			{
				transportationCost = transportationCostDefault;
				minProductAmount = minProductAmountDefault;
				maxProductAmount = maxProductAmountDefault;
				transportationAmount = transportationAmountDefault;
				priceDecay = priceDecayDefault;

				GetTree().GetNodesInGroup("Cell").Cast<Cell>().ToList()
				.ForEach(c => c.ProductList.ForEach(p => p.Price = DefaultPrice));
			}
		}

		private readonly bool reset;

		private float transportationCostDefault;
		private float minProductAmountDefault;
		private float maxProductAmountDefault;
		private float transportationAmountDefault;
		private float priceDecayDefault;
		private const float DefaultPrice = 20f;

		public override void _Ready()
		{
			transportationCostDefault = Global.TransportationCost;
			minProductAmountDefault = Global.MinProductAmount;
			maxProductAmountDefault = Global.MaxProductAmount;
			transportationAmountDefault = Global.TransportationAmount;
			priceDecayDefault = Global.PriceDecay;
		}
	}
}