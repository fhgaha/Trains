using Godot;
using System;
using System.Collections.Generic;
using Trains.Model.Cells;
using Trains.Model.Generators.Noises;
using Trains.Model.Grids;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Generators
{
    public class CellGenerator
    {
        //generate cells, smothify, return to Grid.cs and generate db
        internal static Cell[,] Generate(Grid grid, int rows, int cols, PackedScene cellScene)
        {
            var noises = new Dictionary<ProductType, OpenSimplexNoise>
            {
                [ProductType.Lumber] = new LumberNoise(),
                [ProductType.Grain] = new GrainNoise(),
                [ProductType.Dairy] = new DairyNoise()
            };

            Cell[,] cells = new Cell[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    GenerateCellForGrid(grid, cellScene, noises, cells, i, j);

            Cell.Cells = cells;

            return cells;
        }

        private static void GenerateCellForGrid(Spatial grid, PackedScene cellScene, Dictionary<ProductType, OpenSimplexNoise> noises, Cell[,] cells, int i, int j)
        {
            var cell = cellScene.Instance<Cell>();
            grid.AddChild(cell);
            cell.Init(i, j, noises);
            cell.Name = "Cell_" + cell.Id; //unique name in tree
            cell.Translate(new Vector3(i * Cell.Size, 0, j * Cell.Size));
            cells[i, j] = cell;
        }

        public static void Generate(GridMm grid, int CellsRowsAmount, int CellsColsAmount)
        {
            Random _rnd = new Random();

            var plane = new PlaneMesh
            {
                Size = new Vector2(1, 1),
                Material = new SpatialMaterial { VertexColorUseAsAlbedo = true },
                CenterOffset = new Vector3(0.5f, 0f, 0.5f)
            };

            grid.MultiMeshInstance.Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3d,
                ColorFormat = MultiMesh.ColorFormatEnum.Color8bit,
                CustomDataFormat = MultiMesh.CustomDataFormatEnum.None,
                InstanceCount = CellsRowsAmount * CellsColsAmount,
                VisibleInstanceCount = -1,
                Mesh = plane
            };

            for (int z = 0; z < CellsColsAmount; z++)
            for (int x = 0; x < CellsRowsAmount; x++)
            {
                //color
                var index = x + (z * CellsColsAmount);
                grid.MultiMeshInstance.Multimesh
                    .SetInstanceTransform(index, new Transform(Basis.Identity, new Vector3(x, 0, z)));
                var color = new Color((float)_rnd.NextDouble(), (float)_rnd.NextDouble(), (float)_rnd.NextDouble());
                grid.MultiMeshInstance.Multimesh.SetInstanceColor(index, color);

                //cell
                var cell = new Cell();
                cell.InitNoInstancing(z, x);
                cell.Name = "Cell_" + cell.Id; //unique name in tree

                //save in grid
                grid.Cells[z, x] = cell;
                grid.CellList.Add(cell);
            }

            //add plane with collider
            StaticBody staticBody = new StaticBody();
            var offsetX = (float)CellsRowsAmount / 2;
            var offsetZ = (float)CellsColsAmount / 2;
            CollisionShape collisionShape = new CollisionShape
            {
                Shape = new BoxShape { Extents = new Vector3(offsetX, 0.2f, offsetZ) },
                Translation = new Vector3(offsetX, -0.2f, offsetZ)
            };

            //transform
            staticBody.AddChild(collisionShape);
            grid.MultiMeshInstance.AddChild(staticBody);
        }
    }
}
