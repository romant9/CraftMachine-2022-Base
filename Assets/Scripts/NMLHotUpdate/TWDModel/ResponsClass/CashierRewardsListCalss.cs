using System.Collections.Generic;

namespace TWDModel.ResponsClass
{
	public class CashierRewardsListCalss
	{
		public Rewards Rewards;

		public Dictionary<CurrencyType, int> SurvivorClassRew { get; set; }

		public Dictionary<CurrencyType, int> ScrapSpTokenRewards { get; set; }

		public Dictionary<string, int> EquiTokenRewards { get; set; }

		public int ScrapAmount { get; set; }

		public int apocalypticEquipTokencount { get; set; }

		public CashierRewardsListCalss()
		{
		}

		public CashierRewardsListCalss(Rewards rewards, Dictionary<CurrencyType, int> survivorClassRew, Dictionary<CurrencyType, int> scrapSpTokenRewards, Dictionary<string, int> equiTokenRewards, int scrapAmount, int apocalypticEquipTokencount)
		{
			Rewards = rewards;
			SurvivorClassRew = survivorClassRew;
			ScrapSpTokenRewards = scrapSpTokenRewards;
			EquiTokenRewards = equiTokenRewards;
			ScrapAmount = scrapAmount;
			apocalypticEquipTokencount = apocalypticEquipTokencount;
		}
	}
}
