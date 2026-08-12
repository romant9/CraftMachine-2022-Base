using System;

namespace TWDModel
{
	[Serializable]
	public class BattlePassRewardDefinition
	{
		public int Id;

		public int RequiredBC;

		public string FreeReward;

		public string PremiumReward;

		public bool IsPremiumRewardSpecial;

		public bool IsApocalypseFreeReward;

		public bool IsApocalypsePremiumReward;
	}
}
