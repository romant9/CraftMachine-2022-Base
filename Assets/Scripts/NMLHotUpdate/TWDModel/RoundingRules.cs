using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class RoundingRules
	{
		public CurrencyType Currency;

		public List<int> OutputRoundBase;
	}
}
