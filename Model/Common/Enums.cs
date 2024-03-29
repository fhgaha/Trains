using Godot;

namespace Trains.Model.Common
{
	public class Enums : Node
	{
		public enum ProductType
		{
			Lumber, Grain, Dairy
		}

		public enum BuildingType
		{
			Source, Stock, Both
		}

		//GUI
		public enum MainButtonType
		{
			None, BuildRail, BuildStation, BuildTrain, ShowProductMap
		}
	}
}
