using System.Collections.Generic;

namespace TWDModel
{
	public class BattlePassDataManager
	{
		private readonly IBattlePassDataProvider[] battlePassDataProviders;

		private readonly IBattlePassAnalyticsHandler regularAnalyticsHandler;

		public IBattlePassDataProvider CurrentDataProvider
		{
			get
			{
				IBattlePassDataProvider[] array = battlePassDataProviders;
				foreach (IBattlePassDataProvider battlePassDataProvider in array)
				{
					if (battlePassDataProvider.IsActive)
					{
						return battlePassDataProvider;
					}
				}
				return null;
			}
		}

		public IBattlePassAnalyticsHandler CurrentAnalyticsHandler => regularAnalyticsHandler;

		public BattlePassDataManager(TWDModelManager modelManager)
		{
			regularAnalyticsHandler = new BattlePassAnalytics(modelManager.Player.BattlePass);
			battlePassDataProviders = new IBattlePassDataProvider[2]
			{
				new BeginnerBattlePassDataProvider(modelManager),
				new RegularBattlePassDataProvider(modelManager)
			};
		}

		public IEnumerable<BattlePassRewardDefinition> GetRewards(int seasonId)
		{
			IBattlePassDataProvider[] array = battlePassDataProviders;
			for (int i = 0; i < array.Length; i++)
			{
				IEnumerable<BattlePassRewardDefinition> rewardsForSeason = array[i].GetRewardsForSeason(seasonId);
				if (rewardsForSeason != null)
				{
					return rewardsForSeason;
				}
			}
			return null;
		}
	}
}
