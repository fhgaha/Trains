using Godot;
using System;
using Trains.Model.Common;

namespace Trains
{
	public class ActualRailsContainer : Spatial
	{
		private Events events;
		
		public override void _Ready()
		{
			events = GetNode<Events>("/root/Events");
			
		}

	}
}