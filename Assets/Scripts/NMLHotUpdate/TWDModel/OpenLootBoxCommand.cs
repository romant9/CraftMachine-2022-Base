using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class OpenLootBoxCommand : ModelCommand
	{
		private LootEntry lootEntry;

		private LootEntry lootEntry2;

		public LootScreenType ScreenType { get; set; }

		public int BoxIndex { get; set; }

		public bool GetLoot(out LootEntry loot, out LootEntry loot2)
		{
			loot = lootEntry;
			loot2 = lootEntry2;
			if (lootEntry == null)
			{
				return lootEntry2 != null;
			}
			return true;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult result = TWDModelResult.Error;
			tWDModelManager.Metrics.ResourceChangeIsByCharging = "2";
			this.lootEntry2 = null;
			if (ScreenType == LootScreenType.IAPBonusGift)
			{
				this.lootEntry = tWDModelManager.Player.BundleManager.IAPBonusGiftLootEntry;
				if (this.lootEntry != null)
				{
					tWDModelManager.Player.LootManager.GiveLoot(this.lootEntry);
					tWDModelManager.Metrics.ResourceChangeUsedReason = "buy_bundle";
					tWDModelManager.Metrics.AddFind().AddLoot(this.lootEntry).AddBonusIAPGift()
						.AddLootCrate(this.lootEntry)
						.Send();
					tWDModelManager.Player.BundleManager.IAPBonusGiftLootEntry = null;
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
				if (tWDModelManager.Player.BundleManager.WebShopLootEntrys.Count > 0)
				{
					this.lootEntry = tWDModelManager.Player.BundleManager.WebShopLootEntrys[0];
					tWDModelManager.Player.LootManager.GiveLoot(this.lootEntry);
					tWDModelManager.Metrics.ResourceChangeUsedReason = "banana_buy_bundle";
					tWDModelManager.Metrics.AddFind().AddLoot(this.lootEntry).AddBonusIAPGift()
						.AddLootCrate(this.lootEntry)
						.Send();
					tWDModelManager.Player.BundleManager.WebShopLootEntrys.Remove(this.lootEntry);
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
				if (tWDModelManager.Player.BundleManager.ShareRewardEntrys.Count > 0)
				{
					this.lootEntry = tWDModelManager.Player.BundleManager.ShareRewardEntrys[0];
					tWDModelManager.Metrics.ResourceChangeUsedReason = "share_reward";
					tWDModelManager.Metrics.AddFind().AddLoot(this.lootEntry).AddLootCrate(this.lootEntry)
						.Send();
					tWDModelManager.Player.BundleManager.ShareRewardEntrys.Remove(this.lootEntry);
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
			}
			else if (ScreenType == LootScreenType.GuildGift)
			{
				LootEntry lootEntry = tWDModelManager.Player.OpenGuildGift();
				if (lootEntry != null)
				{
					this.lootEntry = lootEntry;
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
			}
			else if (ScreenType == LootScreenType.TradeCrate)
			{
				LootEntry lootEntry2 = tWDModelManager.Player.LootManager.OpenPendingTradeCrateLoot();
				if (lootEntry2 != null)
				{
					this.lootEntry = lootEntry2;
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
			}
			else
			{
				LootManagerModel lootManager = tWDModelManager.Player.LootManager;
				if (lootManager != null)
				{
					this.lootEntry = null;
					if (ScreenType == LootScreenType.InUi)
					{
						this.lootEntry = tWDModelManager.Player.WeeklyChallenge.GiveReward();
						return new NGModelCommandRespond(this, TWDModelResult.OK);
					}
					if (ScreenType == LootScreenType.InUiSurvival)
					{
						tWDModelManager.Player.WeeklySurvival.GiveReward(out this.lootEntry, out this.lootEntry2);
						return new NGModelCommandRespond(this, TWDModelResult.OK);
					}
					if (ScreenType == LootScreenType.InUIPlayer)
					{
						this.lootEntry = tWDModelManager.Player.GetAndRemoveLootBoxToOpen();
						return new NGModelCommandRespond(this, TWDModelResult.OK);
					}
					if (ScreenType == LootScreenType.Ad)
					{
						if (!tWDModelManager.Player.IsVideoAdRewardAvailable(AdUsage.CinemaReward) || !tWDModelManager.Player.PendingVideoAdReward)
						{
							return new NGModelCommandRespond(this, TWDModelResult.Error);
						}
						lootManager.ShuffleRewards(new LootEntryGenParams
						{
							eventType = DropEventDefinition.DropEventType.VideoAd,
							targetLevel = manager.GetPlayer().Level,
							tag = DropEventDefinition.DropEventTag.VideoAds
						});
						if (tWDModelManager != null)
						{
							tWDModelManager.Metrics.ResourceChangeUsedReason = "watch_ads";
							tWDModelManager.Metrics.AddRewarded().AddVideoAd(AdProvider.None, AdStatus.OK).AddCinema()
								.Send();
						}
						if (tWDModelManager.Player.VideoAdsServed == 0)
						{
							tWDModelManager.Player.VideoAdRewardTime = tWDModelManager.Player.LifeTime;
						}
						tWDModelManager.Player.PendingVideoAdReward = false;
						tWDModelManager.Player.VideoAdsServed++;
						tWDModelManager.Player.LastVideoAdRewardTime = tWDModelManager.Player.LifeTime;
					}
					else if (ScreenType == LootScreenType.DailyQuestChest)
					{
						this.lootEntry = tWDModelManager.Player.DailyQuestManager.GiveChestReward();
					}
					else if (ScreenType == LootScreenType.BattlePassBonusChest)
					{
						BattlePassModel battlePass = tWDModelManager.Player.BattlePass;
						if (battlePass.CanClaimTheBonusChest)
						{
							this.lootEntry = battlePass.ClaimBonusChest();
						}
					}
					if (lootManager.CanOpenLootBox())
					{
						this.lootEntry = lootManager.OpenNextLoot(BoxIndex);
						if (ScreenType == LootScreenType.Combat)
						{
							tWDModelManager.Metrics.AddFind().AddLoot(this.lootEntry).AddMission()
								.AddMissionType()
								.AddLootCrate(this.lootEntry)
								.Send();
						}
						else if (ScreenType == LootScreenType.Ad)
						{
							tWDModelManager.Metrics.AddFind().AddLoot(this.lootEntry).AddCinema()
								.AddLootCrate(this.lootEntry)
								.Send();
						}
					}
					if (this.lootEntry != null)
					{
						(manager as TWDModelManager).Player.MissionStatistics.AddCardCollected(this.lootEntry.DropType);
						result = TWDModelResult.OK;
					}
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
