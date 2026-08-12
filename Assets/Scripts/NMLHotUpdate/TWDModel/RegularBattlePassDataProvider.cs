using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class RegularBattlePassDataProvider : IBattlePassDataProvider
	{
		private readonly PlayerModel player;

		private readonly GameEconomyData gameEconomyData;

		private long? cachedNextSeasonDate;

		public string CapRefreshUTC => gameEconomyData.BattlePassConfig.CapRefreshUTC;

		public int BCPerKill => gameEconomyData.BattlePassConfig.BCPerKill;

		public int MaxDailyBCFromKills => gameEconomyData.BattlePassConfig.MaxDailyBCFromKills;

		public int BonusChestCost => gameEconomyData.BattlePassConfig.BonusChestCost;

		public int[] TierUnlockGoldPrice { get; }

		public string TitleColor => GetCurrentSeasonRaw().TitleColor;

		public string BackgroundColor => GetCurrentSeasonRaw().BackgroundColor;

		public string BundleIdentifier => GetCurrentSeasonRaw().BundleIdentifier;

		public string PopupIcon => GetCurrentSeasonRaw()?.PopupIcon ?? "";

		public long NextSeasonStartDate
		{
			get
			{
				if (!cachedNextSeasonDate.HasValue)
				{
					GetCurrentSeasonRaw();
				}
				return cachedNextSeasonDate ?? long.MaxValue;
			}
		}

		public bool IsActive => player.CouncilLevel >= gameEconomyData.BattlePassConfig.CouncilLockLevel;

		public DropEventDefinition.DropEventType BonusChestDropType => DropEventDefinition.DropEventType.BattlePassCrate;

		public RegularBattlePassDataProvider(TWDModelManager manager)
		{
			player = manager.Player;
			gameEconomyData = manager.GameEconomyData;
			TierUnlockGoldPrice = gameEconomyData.BattlePassConfig.TierUnlockGoldPrice.Split(';').Select(int.Parse).ToArray();
		}

		private BattlePassSeasonDefinition GetCurrentSeasonRaw()
		{
			BattlePassSeasonDefinition result = null;
			cachedNextSeasonDate = null;
			long utcTimeStamp = player.UtcTimeStamp;
			BattlePassSeasonDefinition[] battlePassSeasonDefinitions = gameEconomyData.BattlePassSeasonDefinitions;
			foreach (BattlePassSeasonDefinition battlePassSeasonDefinition in battlePassSeasonDefinitions)
			{
				long num = GameEconomyData.ParseDateTime(battlePassSeasonDefinition.StartTimeUtc).TotalMilliseconds();
				long num2 = GameEconomyData.ParseDateTime(battlePassSeasonDefinition.EndTimeUtc).TotalMilliseconds();
				if (utcTimeStamp >= num && utcTimeStamp < num2)
				{
					result = battlePassSeasonDefinition;
				}
				else if (utcTimeStamp < num)
				{
					cachedNextSeasonDate = num;
					break;
				}
			}
			return result;
		}

		public BattlePassSeason GetCurrentSeason()
		{
			BattlePassSeasonDefinition currentSeasonRaw = GetCurrentSeasonRaw();
			if (currentSeasonRaw != null)
			{
				return new BattlePassSeason
				{
					Id = currentSeasonRaw.Id,
					EndTimeUtc = GameEconomyData.ParseDateTime(currentSeasonRaw.EndTimeUtc).TotalMilliseconds(),
					StartTimeUtc = GameEconomyData.ParseDateTime(currentSeasonRaw.StartTimeUtc).TotalMilliseconds()
				};
			}
			return null;
		}

		public IEnumerable<BattlePassRewardDefinition> GetRewardsForSeason(int seasonId)
		{
			if (gameEconomyData.BattlePassSeasonDefinitions.All((BattlePassSeasonDefinition season) => season.Id != seasonId))
			{
				return null;
			}
			return gameEconomyData.BattlePassRewardDefinitions.Where((BattlePassRewardDefinition def) => def.Id == seasonId);
		}
	}
}
