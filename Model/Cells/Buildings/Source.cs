using Godot;
using System;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells.Buildings
{
	public class Source : Spatial, IBuilding
	{
		public ProductType Type { get; set; }
        public float Amount { get; set; }
		public override void _Ready()
		{
			
		}
	}
}