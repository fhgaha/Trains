using Godot;
using System;

namespace Trains
{
	public class Station : Spatial
	{
		public string Id { get; set; }
		public int Row { get; set; }
		public int Col { get; set; }
	}
}