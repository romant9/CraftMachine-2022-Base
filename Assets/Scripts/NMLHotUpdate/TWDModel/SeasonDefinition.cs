using System;

namespace TWDModel
{
	[Serializable]
	public class SeasonDefinition
	{
		public string Id;

		public CurrencyType RewardCurrency;

		public string RewardLocalisationTitle;

		public string RewardLocalisationDesc;

		public bool Highlighted;

		public int FirstEpisodeNumber;

		public string SeasonVideoUrl;
	}
}
