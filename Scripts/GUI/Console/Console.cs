using Godot;
using System;
using System.Linq;

namespace Trains.Scripts.GUI.Console
{
	public class Console : Node
	{
		public LineEdit InputBox { get; set; }		
		public TextEdit OutputBox { get; set; }
		private CommandHandler commandHandler;

		public override void _Ready()
		{
			InputBox = GetNode<LineEdit>("Input");
			OutputBox = GetNode<TextEdit>("Output");
			InputBox.Connect("text_entered", this, nameof(onTextEntered));
			InputBox.GrabFocus();
			commandHandler = GetNode<CommandHandler>("CommandHandler");
		}

		public void OutputText(string text)
		{
			OutputBox.Text += "\n>" + InputBox.Text + "\n" + text;
		}

		private void onTextEntered(string newText)
		{
			ProcessCommand(newText);
			InputBox.Clear();
		}

		private void ProcessCommand(string command)
		{
			if (command.Length == 0) return;
			var commands = command.Trim().Split(' ').Where(c => !string.IsNullOrEmpty(c)).ToList();
			var commandName = commands[0];

			if (!commandHandler.ValidCommands.Contains(commandName))
				OutputText("Non existing command: " + commandName);

			//sheck if argument is correct type

		}
	}
}