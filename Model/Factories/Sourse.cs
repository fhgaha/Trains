using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Factories
{
	public class Sourse : Node
	{
		public ProductType Type { get; set; }
        public float Amount { get; set; }
		public override void _Ready()
		{

		}
	}
}