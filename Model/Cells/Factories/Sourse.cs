using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells.Factories
{
	public class Sourse : Spatial, IFactory
	{
		public ProductType Type { get; set; }
        public float Amount { get; set; }
		public override void _Ready()
		{

		}
	}
}