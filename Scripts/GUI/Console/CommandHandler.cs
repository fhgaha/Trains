using Godot;
using System;
using System.Collections.Generic;

namespace Trains.Scripts.GUI.Console
{
	public class CommandHandler : Node
	{
		public List<string> ValidCommands = new List<string>
		{
			"show_cell_info"
		};

		//should use attribute to tie "show_cell_info" and ShowCellInfo()
		public void ShowCellInfo()
		{
			GD.Print("ShowCellInfo");
		}
	}
}