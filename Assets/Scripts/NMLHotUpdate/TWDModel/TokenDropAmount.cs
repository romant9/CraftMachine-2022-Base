using System;

namespace TWDModel
{
	[Serializable]
	public class TokenDropAmount
	{
		public int ControlLevelMin;

		public int ControlLevelMax;

		public DropEventDefinition.DropEventType EventType;

		public DropType DropType;

		public DropEventDefinition.DropEventTag Tag;

		public DropCurrenciesProbabilitiesDefinition.DropCurrency DropCurrency;

		public int Min;

		public int Max;
	}
}
