using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class BeginnerBattlePassDataProvider : IBattlePassDataProvider
	{
		private readonly PlayerModel player;

		private readonly GameEconomyData gameEconomyData;

		private readonly BeginnerBattlePassInfo beginnerBattlePassInfo;

		public string CapRefreshUTC => gameEconomyData.BeginnerBattlePassConfig.CapRefreshUTC;

		public int BCPerKill => gameEconomyData.BeginnerBattlePassConfig.BCPerKill;

		public int MaxDailyBCFromKills => gameEconomyData.BeginnerBattlePassConfig.MaxDailyBCFromKills;

		public int BonusChestCost => gameEconomyData.BeginnerBattlePassConfig.BonusChestCost;

		public int[] TierUnlockGoldPrice { get; }

		public string TitleColor => gameEconomyData.BeginnerBattlePassConfig.TitleColor;

		public string BackgroundColor => gameEconomyData.BeginnerBattlePassConfig.BackgroundColor;

		public string BundleIdentifier => gameEconomyData.BeginnerBattlePassConfig.BundleIdentifier;

		public string PopupIcon => "";

		public long NextSeasonStartDate => long.MaxValue;

		public bool IsActive
		{
			get
			{
				if (!gameEconomyData.GetFeature("BeginnerBattlePass").Enabled)
				{
					if (beginnerBattlePassInfo.State == BeginnerBattlePassState.Ongoing)
					{
						beginnerBattlePassInfo.State = BeginnerBattlePassState.Completed;
					}
					else if (player.CouncilLevel >= gameEconomyData.BattlePassConfig.CouncilLockLevel)
					{
						beginnerBattlePassInfo.State = BeginnerBattlePassState.Skipped;
					}
					return false;
				}
				long utcTimeStamp = player.UtcTimeStamp;
				if (beginnerBattlePassInfo.State == BeginnerBattlePassState.Ongoing && utcTimeStamp >= beginnerBattlePassInfo.EndTimestamp)
				{
					beginnerBattlePassInfo.State = BeginnerBattlePassState.Completed;
				}
				if (player.BeginnerBattlePassInfo.State != BeginnerBattlePassState.NotStarted || gameEconomyData.BeginnerBattlePassConfig.CouncilLockLevel > player.CouncilLevel)
				{
					return player.BeginnerBattlePassInfo.State == BeginnerBattlePassState.Ongoing;
				}
				return true;
			}
		}

		public DropEventDefinition.DropEventType BonusChestDropType => DropEventDefinition.DropEventType.BeginnerBattlePassCrate;

		public BattlePassSeason GetCurrentSeason()
		{
			if (!IsActive || beginnerBattlePassInfo.State == BeginnerBattlePassState.NotStarted)
			{
				return null;
			}
			return new BattlePassSeason
			{
				Id = int.MaxValue,
				EndTimeUtc = beginnerBattlePassInfo.EndTimestamp,
				StartTimeUtc = beginnerBattlePassInfo.StartTimestamp
			};
		}

		public BeginnerBattlePassDataProvider(TWDModelManager manager)
		{
			player = manager.Player;
			gameEconomyData = manager.GameEconomyData;
			beginnerBattlePassInfo = player.BeginnerBattlePassInfo;
			TierUnlockGoldPrice = gameEconomyData.BattlePassConfig.TierUnlockGoldPrice.Split(';').Select(int.Parse).ToArray();
		}

		public IEnumerable<BattlePassRewardDefinition> GetRewardsForSeason(int season)
		{
			if (season == int.MaxValue)
			{
				return player.BeginnerBattlePassInfo.CachedRewards;
			}
			return null;
		}
	}
}
