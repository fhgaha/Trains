using Godot;

namespace Trains.Scripts
{
	public class Grid : Spatial
	{
		public int CellsRowsAmount {get; set;} = 2;
		public int CellsColsAmount {get; set;} = 2;
		public Cell[,] Cells;
		private Timer timer;
		PackedScene cellScene = GD.Load<PackedScene>("res://Scenes/Cell.tscn");

		public override void _Ready()
		{
			Cells = new Cell[2,2];
			timer = new Timer();
			AddChild(timer);
			Build();
			//timer.Start(1.0f);
		}

		public override void _Process(float delta) 
		{ 
			//var children = GetChildren();
			var cell = GetChild<Cell>(1);
			cell.SetColor();
		}

		private void Build()
		{
			for (int i = 0; i < CellsRowsAmount; i++)
			for (int j = 0; j < CellsColsAmount; j++)
			{
				var cell = cellScene.Instance<Cell>();
				cell.Translate(new Vector3(i * cell.Size, 0, j * cell.Size));
				timer.Connect("timeout", cell, "SetColor");
				Cells[i, j] = cell;

				AddChild(cell);
			}
		}
	}
}
