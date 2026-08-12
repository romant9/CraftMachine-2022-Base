using System.Collections.Generic;

namespace TWDModel
{
	public interface IBattlePassDataProvider
	{
		string CapRefreshUTC { get; }

		int BCPerKill { get; }

		int MaxDailyBCFromKills { get; }

		int BonusChestCost { get; }

		int[] TierUnlockGoldPrice { get; }

		string TitleColor { get; }

		string BackgroundColor { get; }

		string BundleIdentifier { get; }

		string PopupIcon { get; }

		long NextSeasonStartDate { get; }

		bool IsActive { get; }

		DropEventDefinition.DropEventType BonusChestDropType { get; }

		BattlePassSeason GetCurrentSeason();

		IEnumerable<BattlePassRewardDefinition> GetRewardsForSeason(int season);
	}
}
