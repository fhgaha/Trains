using System.Collections.Generic;
using Godot;
using Trains.Model.Cells;

namespace Trains.Model.Grids
{
    public interface IGrid
    {
        List<Cell> CellList { get; }
        Cell[,] Cells { get; }
    }
}
