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

		public void Load(Cell start, ProductType productType, float amount, Cell destination)
		{
			Id = newIdAddition;
			newIdAddition++;
			ProductType = productType;
			Amount = amount;
			CurrentCell = start;
			Destination = destination;

			CurrentCell.GetProduct(ProductType).Amount -= this.Amount;
		}

		public void Move(Cell[,] cells)
		{
			NextCell = GetNextCell(CurrentCell, Destination, cells);
			CurrentCell.GetProduct(ProductType).Amount -= this.Amount;
			CurrentCell = NextCell;
			CurrentCell.GetProduct(ProductType).Amount += this.Amount;
		}

		public void Unload()
		{
			Destination.GetProduct(ProductType).Amount += this.Amount;
		}

		//get neighbours, move to closest cell to target
		private static Cell GetNextCell(Cell from, Cell destiantion, Cell[,] cells) =>
			from.GetNeighbours(cells)
				.OrderBy(c => Math.Sqrt(Math.Pow(c.Row - destiantion.Row, 2) + Math.Pow(c.Col - destiantion.Col, 2)))
				.First();

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