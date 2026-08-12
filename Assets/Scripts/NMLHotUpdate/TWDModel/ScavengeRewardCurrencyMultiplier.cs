using System;

namespace TWDModel
{
	[Serializable]
	public class ScavengeRewardCurrencyMultiplier
	{
		public CurrencyType Currency;

		public DropEventDefinition.DropEventContext Context;

		public FixedPoint Multiplier;
	}
}
