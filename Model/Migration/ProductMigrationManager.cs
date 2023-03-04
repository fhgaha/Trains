using Godot;
using Trains.Model.Cells;
using Trains.Model.Products;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;
using System;

namespace Trains.Model.Migration
{
	public class ProductMigrationManager
	{
		private Cell[,] cells;
		private readonly List<Cargo> cargos = new List<Cargo>();
		private readonly RandomNumberGenerator rnd = new RandomNumberGenerator();

		// public void Init(Cell[,] cells)
		// {
		// 	this.cells = cells;
		// }

		public ProductMigrationManager(Cell[,] cells)
		{
			// return;

			this.cells = cells;
		}

		public void MoveProducts()
		{
			// return;

			//if cell has source of a product and cells has stock for the same product
			//build cargos from source cell product amount and ship them to the stock
			BuildCargoes();

			foreach (Cargo cargo in cargos)
				cargo.Move(cells);

			RemoveCargoesReachedDestination();
		}

		private void BuildCargoes()
		{
			// return;

			foreach (Cell cell in cells)
			{
				foreach (Product product in cell.ProductList)
					BuildCargo(cell, product);
			}
		}

		private void RemoveCargoesReachedDestination()
		{
			// return;

			var cargosToRemove = new List<Cargo>();

			foreach (Cargo cargo in cargos)
			{
				if (cargo.CurrentCell != cargo.Destination) continue;

				cargo.Unload(cells);
				cargosToRemove.Add(cargo);
			}
			cargosToRemove.ForEach(cargo => cargos.Remove(cargo));
		}

		public void BuildCargo(Cell cell, Product product)
		{
			// return;

			if (product.Amount <= 0) return;

			//if cell has source and other cell has stock build cargo and ship it
			if (cell.Building?.SourceProductType != product.ProductType) return;

			Cell destination = GetProfitableCell(product, cell);
			if (destination is null) return;

			rnd.Randomize();
			float amount = rnd.Randf() * Global.MaxProductAmount;
			amount = Mathf.Clamp(amount, 0, product.Amount);
			if (amount == 0) return;
			if (amount < Global.MinProductAmount) return;
			//product.Amount -= amount;

			Cargo cargo = new Cargo();
			cargo.Init(cell, product.ProductType, amount, destination);
			cargos.Add(cargo);
		}

		private Cell GetProfitableCell(Product product, Cell start)
		{
			// return null;

			Dictionary<Cell, float> destinationCostMap = new Dictionary<Cell, float>();

			foreach (Cell c in cells)
			{
				if (c.Building?.StockProductType != product.ProductType) continue;

				var distance = Cell.GetDistanceSquared(start, c);
				var travelCost = distance * Global.TransportationCost;
				var profitEstimation = c.GetProduct(product.ProductType).Price - travelCost;
				destinationCostMap.Add(c, profitEstimation);
			}
			return destinationCostMap.OrderByDescending(dc => dc.Value).First().Key;
		}
	}
}