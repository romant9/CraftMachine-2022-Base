using System;

namespace TWDModel
{
	[Serializable]
	public class DropCurrenciesAmountsDefinition
	{
		public DropType DropType;

		public CurrencyType Currency;

		public DropEventDefinition.DropEventTag Tag;

		public int ControlLevelMin;

		public int ControlLevelMax;

		public int MinAmount;

		public int MaxAmount;

		public int EventMinAmount;

		public int EventMaxAmount;
	}
}
