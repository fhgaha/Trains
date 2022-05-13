using Godot;
using System;

namespace Trains.Scripts.Cells
{
	public class CellGenerator : Node
	{
		public static void GenerateFromDb()
		{
            var file = new File();
            file.Open("res://Databases/products.json", Godot.File.ModeFlags.Read);

            
		}
	}
}