using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Products;
using System.Linq;
using Trains.Model.Cells.Buildings.Stocks;

namespace Trains.Model.Migration
{
	public class ProductMigrationManager : Node
	{

		public void MoveProducts(Cell[,] cells)
		{
			//if cell has more than treshold amount of products (start)
			//find cell where price for that product is the highest (end)
			//move *amount* of product one cell closer to the end
			foreach (Cell cell in cells)
			foreach (Product product in cell.ProductList)
			{
				bool isReady = product.Amount > Global.MoveTreshold;
				if (isReady)
				{
					//Cell target = GetHighestPriceCell(product, cell, cells);
					Cell target = GetProfitableCell(product, cell, cells);
					if (target == null || target == cell) continue;
					MoveProduct(product, cell, target, cells);
				}
			}
		}

		private Cell GetProfitableCell(Product product, Cell cell, Cell[,] cells)
		{


			//temp
			return GetHighestPriceCell(product, cell, cells);
		}

		private Cell GetHighestPriceCell(Product product, Cell cell, Cell[,] cells)
		{
			Cell target = cell;
			foreach (Cell c in cells)
			{
				//if (c.GetPrice(product.ProductType) > target.GetPrice(product.ProductType))
				if (c.Building != null && c.Building is Stock && c.Building.ProductType == product.ProductType)
					target = c;
			}
			return target;
		}

		private void MoveProduct(Product product, Cell from, Cell target, Cell[,] cells)
		{
			//get neighbours, move to closest cell to target
			var neighbours = from.GetNeighbours(cells);

			Cell to = neighbours
				.OrderBy(c => Math.Sqrt(Math.Pow(c.Row - target.Row, 2) + Math.Pow(c.Col - target.Col, 2)))
				.First();

			from.GetProduct(product.ProductType).Amount -= Global.TravelAmount;
			to.GetProduct(product.ProductType).Amount += Global.TravelAmount;
		}
	}
}