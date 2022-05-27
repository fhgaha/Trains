using Godot;
using System;
using Trains.Model.Products;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells.Buildings
{
	public class Building : Spatial
	{
		public ProductType? SourceProductType { get; set; } = null;
		public ProductType? StockProductType { get; set; } = null;

		//Producing amount per tick
		public float SourceDeltaAmount
		{
			get => sourceDeltaAmount;
			set
			{
				sourceDeltaAmount = value;
				SetTriangleUpBar();
			}
		}

		//Consuming amount per tick
		public float StockDeltaAmount
		{
			get => stockDeltaAmount;
			set
			{
				stockDeltaAmount = value;
				SetTriangleDownBar();
			}
		}

		private float sourceDeltaAmount = 2;
		private float stockDeltaAmount = 2;

		internal void InitSource(Product product, float startAmount)
		{
			SourceProductType = product.ProductType;
			product.Amount += startAmount;
			SetTriangleUpBar();
		}

		internal void InitStock(Product product, float startAmount)
		{
			StockProductType = product.ProductType;
			product.Amount += startAmount;
			SetTriangleDownBar();
		}

		//the bigger producing speed the larger triangle is
		public void SetTriangleUpBar()
		{
			var bar = GetNode<Sprite3D>("TriangleUpBar/Sprite3D");
			var scaleYValue = SourceDeltaAmount * 0.8f;
			bar.Scale = new Vector3(1, scaleYValue, 1);
		}

		public void SetTriangleDownBar()
		{
			var bar = GetNode<Sprite3D>("TriangleDownBar/Sprite3D");
			var scaleYValue = StockDeltaAmount * 0.8f;
			bar.Scale = new Vector3(1, scaleYValue, 1);
		}

		public void DisplaySourceData() => GetNode<Spatial>("TriangleUpBar").Visible = true;
		public void HideSourceData() => GetNode<Spatial>("TriangleUpBar").Visible = false;

		public void DisplayStockData() => GetNode<Spatial>("TriangleDownBar").Visible = true;
		public void HideStockData() => GetNode<Spatial>("TriangleDownBar").Visible = false;
	}
}