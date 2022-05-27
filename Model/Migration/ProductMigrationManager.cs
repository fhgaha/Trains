using Godot;
using System;
using Trains.Model.Cells;
using Trains.Model.Common;
using Trains.Model.Products;
using System.Linq;
using Trains.Model.Cells.Buildings.Stocks;
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
			//if cell has source of a product and cells has stock for the same product
			//build cargos from source cell product amount and ship them to the stock
			foreach (Cell cell in cells)
			foreach (Product product in cell.ProductList)
				BuildCargo(cell, product, cells);

			//remove cargos reached their destinations
			var cargosToRemove = new List<Cargo>();
			foreach (Cargo cargo in Cargos)
			{
				cargo.Move(cells);
				if (cargo.CurrentCell != cargo.Destination) continue;
				//reached destination
				cargo.Unload();
				cargosToRemove.Add(cargo);
			}
			cargosToRemove.ForEach(c => Cargos.Remove(c));
		}

		public void BuildCargo(Cell cell, Product product, Cell[,] cells)
		{
			//if cell has source and other cell has stock build cargo and ship it
			if (cell.Building is null || !(cell.Building is Source) || cell.Building.ProductType != product.ProductType) return;

			Cell destination = GetProfitableCell(product, cell, cells);
			if (destination is null) return;
			if (product.Amount <= 0) return;

			float maxAmountCoeff = 1.5f;
			rnd.Randomize();
			float amount = rnd.Randf() * maxAmountCoeff;
			amount = Mathf.Clamp(amount, 0, product.Amount);
			if (amount == 0) return;
			product.Amount -= amount;

			Cargo cargo = new Cargo();
			cargo.Load(cell, product.ProductType, amount, destination);
			Cargos.Add(cargo);
		}

		private Cell GetProfitableCell(Product product, Cell cell, Cell[,] cells)
		{
			Cell target = null;
			foreach (Cell c in cells)
			{
				if (c.Building is null || !(c.Building is Stock) || c.Building.ProductType != product.ProductType) continue;
				target = c;
			}
			return target;
		}
	}
}