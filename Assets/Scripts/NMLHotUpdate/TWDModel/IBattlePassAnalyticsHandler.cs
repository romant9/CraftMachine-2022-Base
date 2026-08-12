namespace TWDModel
{
	public interface IBattlePassAnalyticsHandler
	{
		void SeasonChange(int oldId, int newId);

		void AdvanceTier(CurrencyType currencyUsed, int amount);

		void ClaimReward(int tierIndex, int rewardIndex, bool premium, bool auto, int? overrideSeasonId = null);

		void ClaimBonusChest(LootEntry reward, CurrencyType currencyUsed, int amountUsed, int? overrideSeasonId = null);

		void GainPremium(bool fromSupport);

		void DailyKillReset();
	}
}
