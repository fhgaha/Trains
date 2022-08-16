using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Builders;
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
		public const float TransportationCost = 0.1f; //0.01f * Product.Price

		/// <summary>
		///amount above which product is ready to move
		/// </summary>
		public const float MoveTreshold = 1f;

		/// <summary>
		///the amount that moves per tick between cells 
		/// </summary>
		public const float TransportationAmount = 0.2f;

		//decreasing amount per tick
		public const float PriceDecay = 0.1f;

		public static ProductType? CurrentDisplayProductMode = null;
		public static MainButtonType MainButtonMode = MainButtonType.None;


		//min max prices
		public const float LumberMinPrice = 30f;
		public const float LumberMaxPrice = 100f;

		public const float GrainMinPrice = 50f;
		public const float GrainMaxPrice = 150f;

		public const float DairyMinPrice = 100f;
		public const float DairyMaxPrice = 300f;

		//
		public static List<RailPath> CosmeticRails { get; internal set; } = new List<RailPath>();

		private static List<RailPath> actualRails;
		public static List<RailPath> ActualRails
		{
			get { return actualRails; }
			set
			{
				actualRails = value;
				PrintPathWithCrossings(value);
			}
		}

		//builders
		public static ActualRailBuilder ActualRailBuilder = new ActualRailBuilder();

		//containers
		public static StationContainer StationContainer;
		public static RailContainer RailContainer;


		private static void PrintPathWithCrossings(List<RailPath> paths)
		{
			GD.Print("<--");
			for (int i = 0; i < paths.Count; i++)
			{
				GD.Print($"{i + 1}. {paths[i]}");
				foreach (var cr in paths[i].Crossings)
				{
					GD.Print("	" + cr);
				}
			}
			GD.Print("-->");
		}
	}
}