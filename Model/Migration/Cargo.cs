using System;
using Trains.Model.Cells;
using System.Linq;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Migration
{
	public class Cargo
	{
		public int Id { get; set; }
		public ProductType ProductType { get; set; }
		public float Amount { get; set; }
		public Cell CurrentCell { get; set; }
		public Cell NextCell { get; set; }
		public Cell Destination { get; set; }
		private static int newIdAddition = 1;

		public void Init(Cell start, ProductType productType, float amount, Cell destination)
		{
			Id = newIdAddition;
			newIdAddition++;
			ProductType = productType;
			Amount = amount;
			CurrentCell = start;
			Destination = destination;

			//CurrentCell.GetProduct(ProductType).Amount -= this.Amount;
		}

		private int moveCallsCount = 0;
		public void Move(Cell[,] cells)
		{
			moveCallsCount++;

			if (moveCallsCount == 5)
			{
				moveCallsCount = 0;

				NextCell = GetNextCell(CurrentCell, Destination, cells);
				CurrentCell.GetProduct(ProductType).Amount -= this.Amount;
				CurrentCell = NextCell;
				CurrentCell.GetProduct(ProductType).Amount += this.Amount;
			}
		}

		public void Unload(Cell[,] cells)
		{
			Destination.CargoArrived(this, ProductType, Amount, cells);
		}

		//get neighbours, move to closest cell to target
		private static Cell GetNextCell(Cell from, Cell destination, Cell[,] cells)
		{
			return from.GetNeighbours(cells)
				.OrderBy(c => Cell.GetDistanceSquared(c, destination))
				.First();
		}

		// private static Cell GetNextCellMostValuableBased(Cell from, Cell destination, Cell[,] cells)
		// {
		// 	var profitable

		// 	foreach (var c in cells)
		// 	{

		// 	}
		// }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			Cargo cargo = (Cargo)obj;
			return Id == cargo.Id;
		}

		public override int GetHashCode() => (Id + 1551) * 123;

		public override string ToString()
		{
			return "Load: id: " + Id
				+ ", cellId:" + CurrentCell.Id
				+ ", product type: " + ProductType
				+ ", amount: " + Amount
				+ ", destination: " + Destination.Id;
		}
	}
}