using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Products;
using System.Linq;

namespace Trains.Model
{
	public class MoveProductsClass : Node
	{
		//private Global global;

		public void MoveProducts(Cell[,] cells)
		{
			//if (global is null) global = GetNode<Global>("/root/Global");
			//var tr = global.TresholdAmount;

			//if cell has more than trashold amount of products (start)
			//find cell where price for that product is the highest (end)
			//move *amount* of product one cell closer to the end
			foreach (Cell cell in cells)
			{
				foreach (Product product in cell.Products.GetChildren())
				{
					bool isReady = product.Amount > Global.TresholdAmount;
					if (isReady)
					{
						Cell target = GetHighestPriceCell(product, cell, cells);

						if (target != null)
							MoveProduct(product, cell, target, cells);
					}
				}
			}
		}

		private Cell GetHighestPriceCell(Product product, Cell cell, Cell[,] cells)
		{
			Cell target = null;
			foreach (Cell c in cells)
			{
				if (c.Id == cell.Id) continue;
				if (c.GetPrice(product.ProductType) > product.Price)
					target = c;
			}
			return target;
		}

		private void MoveProduct(Product product, Cell from, Cell to, Cell[,] cells)
		{
			//get neighbours, move to closest cell to target
			var neighbours = from.GetNeighbours(cells);

			Cell target = neighbours
				.OrderBy(c => Math.Sqrt(Math.Pow(c.Row - to.Row, 2) + Math.Pow(c.Col - to.Col, 2)))
				.First();

			from.GetProduct(product.ProductType).Amount -= Global.TravelAmount;
			from.GetProduct(product.ProductType).Amount += Global.TravelAmount;
		}
	}
}