using Godot;
using System;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Common
{

	public static class Global 
	{
		//Colors

		/// <summary>
		///product travel cost
		/// </summary>
		public static float TransportationCost = 0.1f; //0.01f * Product.Price

		/// <summary>
		///amount above which product is ready to move
		/// </summary>
		public static float MoveTreshold = 1f;

		/// <summary>
		///the amount that moves per tick between cells 
		/// </summary>
		public static float TransportationAmount = 0.2f;

		//decreasing amount per tick
		public static float PriceDecay = 0.01f;

		public static ProductType? CurrentDisplayProductMode = null;



	}
}