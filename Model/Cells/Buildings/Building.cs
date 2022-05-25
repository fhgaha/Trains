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
		public float DeltaAmount
		{
			get => deltaAmount;
			set
			{
				deltaAmount = value;
				SetTriangleBar();
			}
		}

		private float deltaAmount = 2;

		internal void Init(Product product, float startAmount)
		{
			ProductType = product.ProductType;
			product.Amount += startAmount;
			SetTriangleBar();
		}

		//the bigger producing speed the larger triangle is
		public void SetTriangleBar()
		{
			var bar = GetNode<Sprite3D>("TriangleUpBar/Sprite3D");
			var scaleYValue = DeltaAmount * 0.8f;
			bar.Scale = new Vector3(1, scaleYValue, 1);
		}

		public void DisplayData() => GetNode<Spatial>("TriangleUpBar").Visible = true;

		public void HideData() => GetNode<Spatial>("TriangleUpBar").Visible = false;
	}
}