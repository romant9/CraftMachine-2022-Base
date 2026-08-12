using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class CampaignDeeplink
	{
		public string RewardCollectorImpl;

		public string ObjectNameInPlayer;

		public string NameLocalizationKey;

		public string URL;

		public bool TimerBased;

		public string RefreshLocKey;

		public string AllowedCurrencies;

		[JsonIgnore]
		public List<string> AllowedCurrencyNames;

		[JsonIgnore]
		public bool IsCurrencySpecific => !string.IsNullOrEmpty(AllowedCurrencies);

		public void SetupAllowedCurrencies()
		{
			if (IsCurrencySpecific)
			{
				AllowedCurrencyNames = new List<string>(AllowedCurrencies.Split(';'));
			}
		}
	}
}
