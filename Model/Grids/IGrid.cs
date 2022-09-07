using System.Collections.Generic;
using Trains.Model.Cells;

namespace Trains.Model.Grids
{
    public interface IGrid
    {
        List<Cell> CellList { get; }
        Cell[,] Cells { get; }
    }
}
