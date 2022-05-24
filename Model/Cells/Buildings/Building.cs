using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells.Buildings
{
	public class Building : Spatial
	{
		[Export(PropertyHint.Enum)] 
		public ProductType ProductType { get; set; }

		//Producing/consuming amount per tick
		public float DeltaAmount { get; set; } = 2;

		internal void Init(Product product, float startAmount)
		{
			ProductType = product.ProductType;
			product.Amount += startAmount;
		}

		//the bigger/lower price the larger triangle is
		public void SetTriangleBar(float price)
		{
			var bar = GetNode<Spatial>("TriangleUpBar");
			bar.Scale = new Vector3(0, price, 0);
		}

		public void DisplayData() => GetNode<Spatial>("TriangleUpBar").Visible = true;

		public void HideData() => GetNode<Spatial>("TriangleUpBar").Visible = false;
	}
}