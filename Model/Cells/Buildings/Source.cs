using Godot;
using System;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells.Buildings
{
	public class Source : Spatial, IBuilding
	{
		[Export(PropertyHint.Enum)] public ProductType ProductType { get; set; }
        [Export] public float Amount { get; set; }
		private PackedScene scene = GD.Load<PackedScene>("res://Scenes/Buildings/Source.tscn");
		public override void _Ready()
		{
			
		}

		internal void Init(ProductType productType, float amount)
		{
			ProductType = productType;
			Amount = amount;
		}
	}
}