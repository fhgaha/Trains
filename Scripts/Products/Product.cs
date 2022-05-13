using Godot;
using System;
using static Trains.Scripts.Common.Enums;

namespace Trains.Scripts.Products
{
	public struct Product
	{
		public ProductType productType { get; set; }
		public float Price { get; set; }
        
		//list of factories
	}
}