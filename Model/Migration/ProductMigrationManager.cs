using Godot;
using Trains.Model.Cells;
using Trains.Model.Products;
using System.Collections.Generic;
using System.Linq;
using Trains.Model.Common;

namespace Trains.Model.Migration
{
	public class ProductMigrationManager : Node
	{
		private List<Cargo> cargos = new List<Cargo>();
		private RandomNumberGenerator rnd = new RandomNumberGenerator();

		public void MoveProducts(Cell[,] cells)
		{
			//if cell has source of a product and cells has stock for the same product
			//build cargos from source cell product amount and ship them to the stock
			foreach (Cell cell in cells)
			foreach (Product product in cell.ProductList)
				BuildCargo(cell, product, cells);

			//remove the cargos that have reached their destination
			var cargosToRemove = new List<Cargo>();
			foreach (Cargo cargo in cargos)
			{
				cargo.Move(cells);
				if (cargo.CurrentCell != cargo.Destination) continue;
				//have reached destination
				cargo.Unload();
				cargosToRemove.Add(cargo);
			}
			cargosToRemove.ForEach(c => cargos.Remove(c));
		}

		public void BuildCargo(Cell cell, Product product, Cell[,] cells)
		{
			//if cell has source and other cell has stock build cargo and ship it
			if (cell.Building is null || cell.Building.SourceProductType is null 
			|| cell.Building.SourceProductType != product.ProductType) return;

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
			cargos.Add(cargo);
		}

		private Cell GetProfitableCell(Product product, Cell start, Cell[,] cells)
		{
			Dictionary<Cell, float> destinationCostMap = new Dictionary<Cell, float>();

			foreach (Cell c in cells)
			{
				if (c.Building is null || c.Building.StockProductType is null 
				|| c.Building.StockProductType != product.ProductType) continue;
				//target = c;
				var distance = Cell.GetDistance(start, c);
				var travelCost = distance * Global.TransportationCost;
				var profitEstimation = c.GetProduct(product.ProductType).Price - travelCost;
				destinationCostMap.Add(c, profitEstimation);
			}
			var bestPiceCandidate = destinationCostMap.OrderByDescending(dc => dc.Value).First().Key;
			return bestPiceCandidate;
		}
	}
}