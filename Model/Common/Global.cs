using static Trains.Model.Common.Enums;

namespace Trains.Model.Common
{
	public static class Global
	{
		public const float RayLength = 1000f;
		public static bool DebugMode = true;

		//Colors

		/// <summary>
		///product travel cost
		/// </summary>
		public static float TransportationCost = 0.1f; //0.01f * Product.Price

		/// <summary>
		///amount above which product is ready to move
		/// </summary>
		public static float MinProductAmount = 1f;

		public static float MaxProductAmount = 1.5f;

		/// <summary>
		///the amount that moves per tick between cells 
		/// </summary>
		public static float TransportationAmount = 0.2f;

		//decreasing amount per tick
		public static float PriceDecay = 0.1f;

		public static ProductType? CurrentDisplayProductMode = null;
		public static MainButtonType MainButtonMode = MainButtonType.None;


		//min max prices
		public const float LumberMinPrice = 30f;
		public const float LumberMaxPrice = 100f;

		public const float GrainMinPrice = 50f;
		public const float GrainMaxPrice = 150f;

		public const float DairyMinPrice = 100f;
		public const float DairyMaxPrice = 300f;

		//containers
		public static VisibleRailContainer VisibleRailContainer;
		public static SplittedRailsContainer SplittedRailContainer;
		public static ActualRailsContainer ActualRailsContainer;
		public static StationContainer StationContainer;

		public static void UpdateRailData()
		{
			SplittedRailContainer.UpdateRails();
			StationContainer.UpdateStationConnections();
		}
	}
}