using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells.Buildings
{
	public class Source : Spatial, IBuilding
	{
		[Export(PropertyHint.Enum)] public ProductType ProductType { get; set; }
		private PackedScene scene = GD.Load<PackedScene>("res://Scenes/Buildings/Source.tscn");
		public override void _Ready()
		{
			
		}

		internal void Init(Product product, float amount)
		{
			ProductType = product.ProductType;
			product.Amount += amount;
		}
	}
}