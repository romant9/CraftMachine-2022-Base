using System;

namespace TWDModel
{
	[Serializable]
	public class DropCurrenciesStaticDefinition
	{
		public DropEventDefinition.DropEventTag Tag;

		public int ControlLevelMin;

		public int ControlLevelMax;

		public int MinSupplies;

		public int MaxSupplies;

		public int MinSurvivalPoints;

		public int MaxSurvivalPoints;

		public int EventMinSupplies;

		public int EventMaxSupplies;

		public int EventMinSurvivalPoints;

		public int EventMaxSurvivalPoints;
	}
}
