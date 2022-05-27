using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Products;
using System.Linq;
using Trains.Model.Cells.Buildings.Stocks;
using static Trains.Model.Common.Enums;
using Trains.Model.Cells.Buildings.Sources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trains.Model.Migration
{
	public class ProductMigrationManager : Node
	{
		public List<Cargo> Cargos { get; set; } = new List<Cargo>();
		private RandomNumberGenerator rnd = new RandomNumberGenerator();

		public void MoveProducts(Cell[,] cells)
		{
			//if cell has more than treshold amount of products (start)
			//find cell where price for that product is the highest (end)
			//move *amount* of product one cell closer to the end
			foreach (Cell cell in cells)
			foreach (Product product in cell.ProductList)
			{
				#region 
				// //if cell has source and other cell has stock build cargo and ship it
				// if (cell.Building is null || !(cell.Building is Source) || cell.Building.ProductType != product.ProductType ) continue;

				// Cell destination = GetProfitableCell(product, cell, cells);
				// if (destination is null) continue;

				// //build cargos, ship cargos
				// if (product.Amount <= 0) continue;

				// //wait a bit for amount to build up
				// rnd.Randomize();
				// await ToSignal(GetTree().CreateTimer(rnd.Randf() * 1.3f), "timeout");
				// rnd.Randomize();
				// float amount = rnd.Randf() * 1.5f;
				// amount = Mathf.Clamp(amount, 0, product.Amount);
				// if (amount == 0) continue;
				// product.Amount -= amount;

				// Cargo cargo = new Cargo();
				// Cell nextCell = GetNextCell(cell, destination, cells);
				// cargo.Load(cell, nextCell, product.ProductType, amount);
				// Cargos.Add(cargo);
				#endregion

				BuildCargo(cell, product, cells);
			}

			var cargosRemove = new List<Cargo>();
			foreach (Cargo cargo in Cargos)
			{
				cargo.Move(cells);
				if (cargo.CurrentCell != cargo.Destination) continue;
				cargo.Unload();
				cargosRemove.Add(cargo);
			}

			cargosRemove.ForEach(c => Cargos.Remove(c));
		}

		public async void BuildCargo(Cell cell, Product product, Cell[,] cells)
		{
			//if cell has source and other cell has stock build cargo and ship it
			if (cell.Building is null || !(cell.Building is Source) 
			|| cell.Building.ProductType != product.ProductType ) return;

			Cell destination = GetProfitableCell(product, cell, cells);
			if (destination is null) return;

			//build cargos, ship cargos
			if (product.Amount <= 0) return;

			//wait a bit for amount to build up
			rnd.Randomize();
			//await Task.Delay(TimeSpan.FromMilliseconds(200));
			//await ToSignal(GetTree().CreateTimer(rnd.Randf() * 1.3f), "timeout");
			float amount = rnd.Randf() * 1.5f;
			amount = Mathf.Clamp(amount, 0, product.Amount);
			if (amount == 0) return;
			product.Amount -= amount;

			Cargo cargo = new Cargo();
			//Cell nextCell = GetNextCell(cell, destination, cells);
			cargo.Load(cell, null, product.ProductType, amount, destination);
			Cargos.Add(cargo);
		}

		private Cell GetProfitableCell(Product product, Cell cell, Cell[,] cells)
		{
			
			Cell target = null;
			foreach (Cell c in cells)
			{
				if (c.Building is null) continue;
				var stock = c.Building as Stock;
				if (stock is null) continue;
				if (c.Building.ProductType != product.ProductType) continue;
				target = c;
			}
			return target;
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
			Cell to = GetNextCell(from, target, cells);

			//if not source or stock fill up some value then move

			from.GetProduct(product.ProductType).Amount -= Global.TravelAmount;
			to.GetProduct(product.ProductType).Amount += Global.TravelAmount;
		}

		private static Cell GetNextCell(Cell from, Cell target, Cell[,] cells)
		{
			//get neighbours, move to closest cell to target
			var neighbours = from.GetNeighbours(cells);
			Cell to = neighbours
				.OrderBy(c => Math.Sqrt(Math.Pow(c.Row - target.Row, 2) + Math.Pow(c.Col - target.Col, 2)))
				.First();
			return to;
		}
	}

	public class Cargo
	{
		public int Id { get; set; }
		public ProductType ProductType { get; set; }
		public float Amount { get; set; }
		public Cell CurrentCell { get; set; }
		public Cell NextCell { get; set; }
		public Cell Destination { get; set; }
		private static int newIdAddition = 1;

		public void Load(Cell start, Cell nextCell, ProductType productType, float amount, Cell destination)
		{
			Id = newIdAddition;
			newIdAddition++;
			ProductType = productType;
			Amount = amount;
			CurrentCell = start;
			NextCell = nextCell;
			Destination = destination;

			CurrentCell.GetProduct(ProductType).Amount -= this.Amount;
			GD.Print("Load: id: " + Id 
				+ ", cellId:" + CurrentCell.Id 
				+ ", product type: " + ProductType 
				+ ", amount: " + Amount 
				+ ", destination: " + Destination);
		}

		public void Move(Cell [,] cells)
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

		private static Cell GetNextCell(Cell from, Cell target, Cell[,] cells)
		{
			//get neighbours, move to closest cell to target
			var neighbours = from.GetNeighbours(cells);
			Cell to = neighbours
				.OrderBy(c => Math.Sqrt(Math.Pow(c.Row - target.Row, 2) + Math.Pow(c.Col - target.Col, 2)))
				.First();
			return to;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;
			
			Cargo cargo = (Cargo)obj;
			return Id == cargo.Id;
		}
		
		public override int GetHashCode()
		{
			return (Id + 1551) * 123;
		}
	}
}