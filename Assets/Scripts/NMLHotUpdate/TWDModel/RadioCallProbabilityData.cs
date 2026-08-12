using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class RadioCallProbabilityData
	{
		public List<ItemAmountProbabilityData> HighlightedProbabilities;

		public List<ItemAmountProbabilityData> Probabilities;

		public bool GuaranteedHero;
	}
}
