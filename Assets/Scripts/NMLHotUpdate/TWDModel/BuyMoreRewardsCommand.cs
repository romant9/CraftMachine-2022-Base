using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class BuyMoreRewardsCommand : ModelCommand
	{
		public bool BuyWithDiamonds { get; set; }

		public bool BuyWithKeys { get; set; }

		private int GetRewardsLeft(LootManagerModel lootManager)
		{
			return lootManager.AvailableKeys;
		}

		private int GetCardsLeftToOpen(LootManagerModel lootManager)
		{
			return lootManager.GetLootsLeftToOpenCount();
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			LootManagerModel lootManager = tWDModelManager.Player.LootManager;
			if (lootManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (GetRewardsLeft(lootManager) >= GetCardsLeftToOpen(lootManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			LootKeySource lootKeySource = LootKeySource.DiamondPurchase;
			if (BuyWithDiamonds)
			{
				PlayerModel obj = manager.GetPlayer() as PlayerModel;
				int num = tWDModelManager.GameEconomyData.ConfigData.ThreeRewardsCost;
				if (obj.ActivityManager.TryGetActivityParam(ActivityType.Jackpot, out var activityParams))
				{
					num = int.Parse(activityParams[2]);
				}
				if (obj.Tutorial.CurrentPartId == "RewardsScreen3")
				{
					num = 0;
				}
				if (num > 0)
				{
					Cashier cashier = Cashier.CreateOneItemCashier(tWDModelManager, PurchaseType.RewardUnlock, CurrencyType.Diamonds, num);
					if (!cashier.CanAfford() || cashier.Pay() != TWDModelResult.OK)
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
				}
				lootKeySource = LootKeySource.DiamondPurchase;
			}
			else if (BuyWithKeys)
			{
				PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
				int num2 = 1;
				if (playerModel.Tutorial.CurrentPartId == "RewardsScreen3")
				{
					num2 = 0;
				}
				if (num2 > 0)
				{
					Cashier cashier2 = Cashier.CreateOneItemCashier(tWDModelManager, PurchaseType.RewardUnlock, CurrencyType.LootKeys, num2);
					int lootKeySoftCap = playerModel.ActivityManager.GetLootKeySoftCap(playerModel.gameEconomyData.ConfigData);
					if (playerModel.GetCurrency(CurrencyType.LootKeys).Value == lootKeySoftCap)
					{
						playerModel.LootKeysFirstSpentTime = playerModel.UtcTimeStamp;
					}
					if (!cashier2.CanAfford() || cashier2.Pay() != TWDModelResult.OK)
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
				}
				if (playerModel.Combat != null)
				{
					playerModel.Combat.HasSpentLootKeyCurrency = true;
				}
				lootKeySource = LootKeySource.LootKeyCurrencyPurchase;
			}
			else
			{
				if (!tWDModelManager.Player.IsVideoAdRewardAvailable(AdUsage.CombatRewardKey) || !tWDModelManager.Player.PendingVideoAdRewardInRewardScreen)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				tWDModelManager?.Metrics.AddRewarded().AddVideoAd(AdProvider.UnityAds, AdStatus.OK).AddMission()
					.Send();
				if (tWDModelManager.Player.VideoAdsServedRewardScreen == 0)
				{
					tWDModelManager.Player.VideoAdRewardTimeRewardScreen = tWDModelManager.Player.LifeTime;
				}
				tWDModelManager.Player.PendingVideoAdRewardInRewardScreen = false;
				tWDModelManager.Player.VideoAdsServedRewardScreen++;
				tWDModelManager.Player.LastVideoAdRewardTimeRewardScreen = tWDModelManager.Player.LifeTime;
				lootKeySource = LootKeySource.Ads;
			}
			lootManager.AvailableKeys += 3;
			if (tWDModelManager.Player.LootManager.LootKeysSources != null)
			{
				for (int i = 0; i < 3; i++)
				{
					tWDModelManager.Player.LootManager.LootKeysSources.Add(lootKeySource);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
