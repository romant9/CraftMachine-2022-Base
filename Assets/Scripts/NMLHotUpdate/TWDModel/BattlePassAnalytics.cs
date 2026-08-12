using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class BattlePassAnalytics : IBattlePassAnalyticsHandler
	{
		private readonly BattlePassModel battlePass;

		private readonly Metrics metrics;

		public BattlePassAnalytics(BattlePassModel battlePassModel)
		{
			battlePass = battlePassModel;
			metrics = battlePassModel.manager.Metrics;
		}

		public void SeasonChange(int oldId, int newId)
		{
			if (oldId > 0)
			{
				metrics.AddEnd().AddBattlePass(battlePass, oldId).AddBattlePassSeason()
					.Send();
			}
			if (newId > 0)
			{
				metrics.AddStart().AddBattlePass(battlePass).AddBattlePassSeason()
					.Send();
			}
		}

		public void AdvanceTier(CurrencyType currencyUsed, int amount)
		{
			metrics.AddSpend().AddResources(new Dictionary<CurrencyType, OverflowableAmount> { 
			{
				currencyUsed,
				new OverflowableAmount
				{
					Amount = -amount
				}
			} }).AddBattlePass(battlePass)
				.AddBattlePassAdvanceTier()
				.Send();
		}

		public void ClaimReward(int tierIndex, int rewardIndex, bool premium, bool auto, int? overrideSeasonId = null)
		{
			IReward reward = battlePass.GetReward(tierIndex, premium, rewardIndex);
			metrics.AddFind();
			if (reward is RewardCurrency rewardCurrency && battlePass.manager.GameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency.CurrencyType))
			{
				int num = rewardCurrency.Amount - rewardCurrency.AmountActuallyAdded;
				if (num > 0)
				{
					metrics.PushResource(CurrencyType.Diamonds, battlePass.manager.GameEconomyData.CurrencyToDiamonds(rewardCurrency.CurrencyType, num));
					metrics.AddResources();
				}
				else
				{
					metrics.AddNonLootReward(reward);
				}
				metrics.AddBattlePass(battlePass, overrideSeasonId).AddBattlePassClaimProperties(tierIndex, premium, rewardIndex, auto).AddBattlePassConvertedToGold(num > 0);
			}
			else
			{
				metrics.AddNonLootReward(reward).AddBattlePass(battlePass, overrideSeasonId).AddBattlePassClaimProperties(tierIndex, premium, rewardIndex, auto);
			}
			metrics.Send();
		}

		public void ClaimBonusChest(LootEntry reward, CurrencyType currencyUsed, int amountUsed, int? overrideSeasonId = null)
		{
			metrics.AddFind().AddLoot(reward).AddBattlePass(battlePass, overrideSeasonId)
				.AddBattlePassBonusChest()
				.Send();
			metrics.AddSpend().AddResources(new Dictionary<CurrencyType, OverflowableAmount> { 
			{
				currencyUsed,
				new OverflowableAmount
				{
					Amount = -amountUsed
				}
			} }).AddBattlePass(battlePass)
				.AddBattlePassBonusChest()
				.Send();
		}

		public void GainPremium(bool fromSupport)
		{
			metrics.AddFind().AddResources(CurrencyType.FreeGuildGiftPerk, 1, 1).AddBattlePassGetPremium()
				.AddBattlePass(battlePass);
			if (fromSupport)
			{
				metrics.AddSupport();
			}
			metrics.Send();
		}

		public void DailyKillReset()
		{
			metrics.AddBattlePassResetDailyKills(TimeSpan.FromMilliseconds(battlePass.KillCapExpiryDateMilliseconds - battlePass.CurrentSeasonStartDate).Days).AddBattlePass(battlePass).Send();
		}
	}
}
