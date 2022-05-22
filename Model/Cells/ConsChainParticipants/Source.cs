using Godot;
using System;
using Trains.Model.Cells.ConsChainParticipants;
using static Trains.Model.Common.Enums;

namespace Trains.Model.Cells.Factories
{
	public class Source : Spatial, IConsChainParticipant
	{
		public ProductType Type { get; set; }
        public float Amount { get; set; }
		public override void _Ready()
		{
			
		}
	}
}