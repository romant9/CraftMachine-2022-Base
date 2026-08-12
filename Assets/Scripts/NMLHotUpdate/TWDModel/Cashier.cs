using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class Cashier
	{
		public const int UseDiamondsAmountNotSet = -1;

		public const int UseDiamondsAmountIgnored = -2;

		public static string PurchaseEventBoughtDiamondsProperty = "paid_price_bought_diamonds";

		public static string PurchaseEventFreeDiamondsProperty = "paid_price_free_diamonds";

		public static string BuildCreatedAnalyticsEvent = "guild_created";

		private List<CashierItem> items = new List<CashierItem>();

		private TWDModelManager manager;

		public bool useTokensForPayment;

		public int UseDiamondsAmount { get; set; }

		[JsonIgnore]
		public Dictionary<CurrencyType, int> ExchangedCurrencies { get; private set; }

		[JsonIgnore]
		public int ExchangedDiamonds { get; private set; }

		[JsonIgnore]
		public string UsedReason { get; set; } = "Consume";

		public Dictionary<CurrencyType, int> UseExtraTokens { get; set; }

		[JsonIgnore]
		public Dictionary<CurrencyType, OverflowableAmount> LastRefundAmounts { get; protected set; }

		public Cashier(TWDModelManager manager)
		{
			this.manager = manager;
			UseDiamondsAmount = -1;
		}

		public static Cashier CreateOneItemCashier(TWDModelManager manager, PurchaseType purchaseType, CurrencyType currency, int cost)
		{
			Cashier cashier = new Cashier(manager);
			CashierItem cashierItem = new CashierItem(purchaseType);
			cashierItem.SetCost(currency, cost);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public void AddItem(CashierItem item)
		{
			items.Add(item);
		}

		public int GetMissing(CurrencyType currencyType)
		{
			int num = 0;
			int value = manager.Player.GetCurrency(currencyType).Value;
			foreach (CashierItem item in items)
			{
				num += item.GetCost(currencyType);
			}
			if (num < 0)
			{
				return num;
			}
			return Math.Max(num - value, 0);
		}

		public int GetTotalCost(CurrencyType currencyType)
		{
			int num = 0;
			foreach (CashierItem item in items)
			{
				num += item.GetCost(currencyType);
			}
			return num;
		}

		public List<CashierItem> GetCashierItems()
		{
			return items;
		}

		public bool CanPay(CurrencyType currencyType)
		{
			return GetMissing(currencyType) == 0;
		}

		public bool CanAfford()
		{
			if (OfflineManager.IsLoadDataManager && OfflineManager.IsFreeAll)
			{
				return true;
			}
			Dictionary<CurrencyType, bool> dictionary = new Dictionary<CurrencyType, bool>();
			foreach (CashierItem item in items)
			{
				foreach (CurrencyType item2 in item.CurrencyTypesExisted)
				{
					if (!dictionary.ContainsKey(item2))
					{
						dictionary.Add(item2, value: true);
						if (GetMissing(item2) > 0)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public bool IsFree()
		{
			for (int i = 0; i < items.Count; i++)
			{
				for (int j = 0; j < items[i].Cost.Length; j++)
				{
					if (items[i].Cost[j] > 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool CanAffordWithDiamonds()
		{
			int num = 0;
			if (!CanConvertToDiamonds())
			{
				return false;
			}
			for (int i = 0; i < (int)CurrencyType.Count; i++)
			{
				CurrencyType currencyType = (CurrencyType)i;
				if (GetMissing(currencyType) > 0)
				{
					num += manager.GameEconomyData.CurrencyToDiamonds(currencyType, GetMissing(currencyType), manager.Player);
				}
			}
			if (num > manager.Player.GetCurrency(CurrencyType.Diamonds).Value)
			{
				return false;
			}
			return true;
		}

		public bool CanConvertToDiamonds()
		{
			for (int i = 0; i < (int)CurrencyType.Count; i++)
			{
				CurrencyType currencyType = (CurrencyType)i;
				if (GetMissing(currencyType) > 0 && !manager.GameEconomyData.CanConvertToDiamonds(currencyType))
				{
					return false;
				}
			}
			return true;
		}

		public TWDModelResult Pay(object sourceObject = null, string metricsOptions = null, object targetObject = null, Func<TWDModelResult> nonCurrencyTokenSubtractFunc = null)
		{
			if (OfflineManager.IsLoadDataManager && OfflineManager.IsFreeAll) return TWDModelResult.OK;

			int previousBoughtDiamondsCount = 0;
			CurrencyModel currency = manager.Player.GetCurrency(CurrencyType.Diamonds);
			if (currency != null)
			{
				previousBoughtDiamondsCount = currency.Bought;
			}
			int outExchangedDiamonds = 0;
			ExchangedCurrencies = GetTotalExchangedCurrencies();
			if (ExchangeDiamondsIfNeeded(ref outExchangedDiamonds) == TWDModelResult.DiamondExchangeDesync)
			{
				return TWDModelResult.DiamondExchangeDesync;
			}
			ExchangedDiamonds = outExchangedDiamonds;
			int count = manager.Player.Currencies.Count;
			for (int i = 0; i < count; i++)
			{
				CurrencyModel currencyModel = manager.Player.Currencies[i];
				if (currencyModel.Value < GetTotalCost(currencyModel.Type))
				{
					return TWDModelResult.NotEnoughCurrency;
				}
			}
			for (int j = 0; j < count; j++)
			{
				CurrencyModel currencyModel2 = manager.Player.Currencies[j];
				if (GetTotalCost(currencyModel2.Type) != 0)
				{
					currencyModel2.Subtract(GetTotalCost(currencyModel2.Type));
				}
			}
			if (nonCurrencyTokenSubtractFunc != null)
			{
				TWDModelResult tWDModelResult = nonCurrencyTokenSubtractFunc();
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}
			SendPurchaseAnalyticsEvent(sourceObject, previousBoughtDiamondsCount, metricsOptions, targetObject);
			return TWDModelResult.OK;
		}

		public TWDModelResult PayWithTokens(object sourceObject = null, string metricsOptions = null, object targetObject = null)
		{
			int num = (int)CurrencyType.Count;
			for (int i = 0; i < num; i++)
			{
				CurrencyModel currencyModel = manager.Player.Currencies[i];
				if (currencyModel.Value < GetTotalCost(currencyModel.Type))
				{
					return TWDModelResult.NotEnoughCurrency;
				}
			}
			for (int j = 0; j < num; j++)
			{
				CurrencyModel currencyModel2 = manager.Player.Currencies[j];
				if (GetTotalCost(currencyModel2.Type) != 0)
				{
					currencyModel2.Subtract(GetTotalCost(currencyModel2.Type));
					manager.Player.NotifyChange("SpeedUpTokenUsed");
				}
			}
			SendPurchaseAnalyticsEvent(sourceObject, 0, metricsOptions, targetObject);
			return TWDModelResult.OK;
		}

		private Dictionary<CurrencyType, int> GetTotalExchangedCurrencies()
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			for (int i = 0; i < manager.Player.Currencies.Count; i++)
			{
				dictionary.Add(manager.Player.Currencies[i].Type, GetMissing(manager.Player.Currencies[i].Type));
			}
			return dictionary;
		}

		private void SendPurchaseAnalyticsEvent(object sourceObject, int previousBoughtDiamondsCount, string metricsOptions = null, object targetObject = null)
		{
			if (manager == null || manager.Player == null || manager.Player.Camp == null || manager.Player.Camp.GetBuilding("Council") == null || !OfflineManager.IsUseSendMetrics)
			{
				return;
			}
			int boughtDiamondsCount = previousBoughtDiamondsCount;
			Dictionary<PurchaseType, Dictionary<CurrencyType, int>> dictionary = new Dictionary<PurchaseType, Dictionary<CurrencyType, int>>();
			bool flag = false;
			foreach (CashierItem item in items)
			{
				Dictionary<CurrencyType, int> dictionary2 = null;
				if (dictionary.ContainsKey(item.PurchaseType))
				{
					dictionary2 = dictionary[item.PurchaseType];
					int count = manager.Player.Currencies.Count;
					for (int i = 0; i < count; i++)
					{
						CurrencyModel currencyModel = manager.Player.Currencies[i];
						int cost = item.GetCost(currencyModel.Type);
						dictionary2[currencyModel.Type] += cost;
					}
				}
				else
				{
					dictionary2 = new Dictionary<CurrencyType, int>();
					int count2 = manager.Player.Currencies.Count;
					for (int j = 0; j < count2; j++)
					{
						CurrencyModel currencyModel2 = manager.Player.Currencies[j];
						int cost2 = item.GetCost(currencyModel2.Type);
						if (dictionary2.ContainsKey(currencyModel2.Type))
						{
							dictionary2[currencyModel2.Type] += cost2;
						}
						else
						{
							dictionary2.Add(currencyModel2.Type, cost2);
						}
					}
					dictionary.Add(item.PurchaseType, dictionary2);
				}
				if (dictionary2 != null && ExchangedDiamonds > 0 && !flag)
				{
					if (dictionary2.ContainsKey(CurrencyType.Diamonds))
					{
						dictionary2[CurrencyType.Diamonds] += ExchangedDiamonds;
					}
					else
					{
						dictionary2.Add(CurrencyType.Diamonds, ExchangedDiamonds);
					}
					flag = true;
				}
			}
			foreach (KeyValuePair<PurchaseType, Dictionary<CurrencyType, int>> item2 in dictionary)
			{
				SubtractFromBoughtDiamonds(item2.Value, ref previousBoughtDiamondsCount);
			}
			if (manager == null)
			{
				return;
			}
			foreach (KeyValuePair<PurchaseType, Dictionary<CurrencyType, int>> item3 in dictionary)
			{
				manager.Metrics.ResourceChangeUsedReason = UsedReason;
				manager.Metrics.metricsResourcesData.BoughtGold = 0;
				manager.Metrics.metricsResourcesData.FreeGold = 0;
				foreach (KeyValuePair<CurrencyType, int> item4 in item3.Value)
				{
					if (item4.Key == CurrencyType.Diamonds)
					{
						int num = 0;
						num = ((boughtDiamondsCount <= item4.Value) ? boughtDiamondsCount : item4.Value);
						manager.Metrics.metricsResourcesData.BoughtGold = -num;
						manager.Metrics.metricsResourcesData.FreeGold = -Math.Max(0, item4.Value - boughtDiamondsCount);
						boughtDiamondsCount -= num;
					}
				}
				Metrics.UpgradeTypes upgradeType = Metrics.UpgradeTypes.Regular;
				if (item3.Key == PurchaseType.InstantSurvivorUpgrade || item3.Key == PurchaseType.InstantWalkerUpgrade || item3.Key == PurchaseType.InstantBuild || item3.Key == PurchaseType.InstantBuildingUpgrade || item3.Key == PurchaseType.InstantEquipmentUpgrade)
				{
					upgradeType = Metrics.UpgradeTypes.Instant;
				}
				else if (item3.Key == PurchaseType.SpeedUp || item3.Key == PurchaseType.SpeedUpCuringAllSurvivors || item3.Key == PurchaseType.SpeedUpEquipmentTypeUpgrade || item3.Key == PurchaseType.SpeedUpEquipmentUpgrade || item3.Key == PurchaseType.SpeedUpSurvivorUpgrade || item3.Key == PurchaseType.SpeedUpWalkerUpgrade)
				{
					upgradeType = Metrics.UpgradeTypes.SpeedUp;
				}
				else if (item3.Key == PurchaseType.SpeedupAndBuildingUpgrade)
				{
					upgradeType = Metrics.UpgradeTypes.SpeedUpAndUpgrade;
				}
				if (sourceObject is SurvivorModel)
				{
					SurvivorModel survivorModel = sourceObject as SurvivorModel;
					if (item3.Key == PurchaseType.UpgradeTrait)
					{
						if (survivorModel.CanUpgradeSurvivorRarity())
						{
							manager.Metrics.AddSpend().AddResources(this).AddUpgrade()
								.AddSurvivor(survivorModel)
								.AddRarity()
								.Send();
						}
						else
						{
							manager.Metrics.AddSpend().AddResources(this).AddUpgrade()
								.AddSurvivor(survivorModel)
								.AddTraitLevel()
								.Send();
						}
					}
					else if (item3.Key == PurchaseType.UpgradeSurvivor || item3.Key == PurchaseType.InstantSurvivorUpgrade || item3.Key == PurchaseType.SpeedUpSurvivorUpgrade)
					{
						manager.Metrics.AddSpend().AddResources(this).AddUpgrade(upgradeType)
							.AddSurvivor(survivorModel)
							.AddLevel()
							.Send();
					}
					else if (item3.Key == PurchaseType.SpeedUpCuringSurvivor)
					{
						manager.Metrics.AddSpend().AddResources(this).AddHeal(healAll: false)
							.AddSurvivor(survivorModel)
							.Send();
					}
					else if (item3.Key == PurchaseType.ReclaimBadge && targetObject is BadgeModel badge)
					{
						manager.Metrics.AddSpend().AddResources(this).AddUnequip()
							.AddBadge(badge)
							.AddSurvivor(survivorModel)
							.Send();
					}
				}
				else if (item3.Key == PurchaseType.SpeedUpCuringAllSurvivors)
				{
					manager.Metrics.AddSpend().AddResources(this).AddHeal(healAll: true)
						.Send();
				}
				else if (sourceObject is EquipmentItemModel)
				{
					EquipmentItemModel equipment = sourceObject as EquipmentItemModel;
					if (item3.Key == PurchaseType.UpgradeEquipment || item3.Key == PurchaseType.InstantEquipmentUpgrade || item3.Key == PurchaseType.SpeedUpEquipmentUpgrade || item3.Key == PurchaseType.UpgradeEquipmentLevel)
					{
						manager.Metrics.AddSpend().AddResources(this).AddUpgrade(upgradeType)
							.AddEquipment(equipment)
							.AddLevel()
							.Send();
					}
				}
				else if (sourceObject is TradefairBundleContentDefinition)
				{
					manager.Metrics.AddSpend().AddResources(this).Send();
				}
				else if (sourceObject is CustomBundleDefinition)
				{
					manager.Metrics.AddSpend().AddResources(this).Send();
				}
				else if (sourceObject is OutpostWalkerModel)
				{
					OutpostWalkerModel outpostWalkerModel = sourceObject as OutpostWalkerModel;
					if (item3.Key == PurchaseType.InstantWalkerUpgrade || item3.Key == PurchaseType.UpgradeWalker || item3.Key == PurchaseType.SpeedUpWalkerUpgrade)
					{
						manager.Metrics.AddSpend().AddResources(this).AddUpgrade(upgradeType)
							.AddOupostWalker(outpostWalkerModel)
							.AddLevel()
							.Send();
					}
					else if (item3.Key == PurchaseType.UpgradeWalkerAmount)
					{
						manager.Metrics.AddSpend().AddResources(this).AddUpgrade(Metrics.UpgradeTypes.Regular)
							.AddOupostWalker(outpostWalkerModel)
							.AddAmount()
							.Send();
					}
				}
				else if (item3.Key == PurchaseType.GuildGift)
				{
					GuildMemberInfo guildMember = ((manager.Player.GuildModel != null) ? manager.Player.GuildModel.GetMemberInfo(manager.Player.HashedId) : null);
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddGuildGift()
						.AddGuild(manager.Player.GuildModel)
						.AddModerator(guildMember)
						.Send();
				}
				else if (item3.Key == PurchaseType.GuildAd)
				{
					GuildMemberInfo guildMember2 = ((manager.Player.GuildModel != null) ? manager.Player.GuildModel.GetMemberInfo(manager.Player.HashedId) : null);
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddGuildAd(sourceObject as string)
						.AddGuild(manager.Player.GuildModel)
						.AddModerator(guildMember2)
						.Send();
				}
				else if (sourceObject is OutfitDefinition)
				{
					if (item3.Key == PurchaseType.Outfit)
					{
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddOutfit(sourceObject as OutfitDefinition)
							.Send();
					}
				}
				else if (sourceObject is BuildingModel)
				{
					BuildingModel buildingModel = sourceObject as BuildingModel;
					if (item3.Key == PurchaseType.BuildingPurchase || item3.Key == PurchaseType.BuildingUpgrade || item3.Key == PurchaseType.InstantBuildingUpgrade || item3.Key == PurchaseType.SpeedUp || item3.Key == PurchaseType.SpeedupAndBuildingUpgrade)
					{
						if (manager.Player.Tutorial != null && !manager.Player.Tutorial.HasCompletedPart("Tutorial") && buildingModel.TypeName == "BuildingProduceSupplies")
						{
							manager.Metrics.AddSpend().AddResources(this).AddUpgrade(upgradeType)
								.AddBuilding(buildingModel)
								.AddLevel()
								.AddTutorial()
								.Send();
						}
						else
						{
							manager.Metrics.AddSpend().AddResources(this).AddUpgrade(upgradeType)
								.AddBuilding(buildingModel)
								.AddLevel()
								.Send();
						}
					}
					else if (item3.Key == PurchaseType.CutVegetation)
					{
						manager.Metrics.AddSpend().AddResources(this).AddRemove()
							.AddBuilding(buildingModel)
							.Send();
					}
				}
				else if (sourceObject is PhoneCallModel)
				{
					if (item3.Key == PurchaseType.PhoneCall)
					{
						DebugTWD.Log("Buy RadioCall");

						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddRadioCall()
							.Send();
					}
				}
				else if (sourceObject is SurvivalManualActorLevel)
				{
					if (item3.Key == PurchaseType.UpgradeSurvivalManualActor)
					{
						SurvivalManualActorLevel survivalManualActorLevel = sourceObject as SurvivalManualActorLevel;
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddSurvivalManualActorlevel(survivalManualActorLevel)
							.Send();
					}
				}
				else if (sourceObject is SurvivalManualSkill)
				{
					if (item3.Key == PurchaseType.UpgradeSurvivalManualSkill)
					{
						SurvivalManualSkill survivalManualSkill = sourceObject as SurvivalManualSkill;
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddSurvivalManualSkillLevel(survivalManualSkill)
							.Send();
					}
				}
				else if (sourceObject is SurvivalManualStorySkill)
				{
					if (item3.Key == PurchaseType.UpgradeSurvivalManualStorySkill)
					{
						SurvivalManualStorySkill survivalManualStorySkill = sourceObject as SurvivalManualStorySkill;
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddSurvivalManualStorySkillLevel(survivalManualStorySkill)
							.Send();
					}
				}
				else if (sourceObject is SurvivorContainerModel)
				{
					if (item3.Key == PurchaseType.AdditionalSurvivorSlots)
					{
						manager.Metrics.AddSpend().AddResources(this).AddUpgrade()
							.AddSurvivorSlot()
							.Send();
					}
				}
				else if (sourceObject is SurvivalCharacterContainerModel)
				{
					if (item3.Key == PurchaseType.SurvivalRest)
					{
						SurvivalCharacterContainerModel survivalCharacterContainerModel = sourceObject as SurvivalCharacterContainerModel;
						manager.Metrics.AddSpend().AddResources(this).AddDistance()
							.AddRest(survivalCharacterContainerModel.ComputeSurvivorsForRestingCount())
							.Send();
					}
				}
				else if (item3.Key == PurchaseType.SPEquipmentRemoldTraits)
				{
					EquipmentItemModel equipmentItemModel = sourceObject as EquipmentItemModel;
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddSPEquipmentRemoldTraits(equipmentItemModel)
						.Send();
				}
				else if (item3.Key == PurchaseType.EquipBreakthrough)
				{
					EquipmentBreakthroughModel equipmentBreakthroughModel = sourceObject as EquipmentBreakthroughModel;
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddEquipmentBreakthrough(equipmentBreakthroughModel, metricsOptions)
						.Send();
				}
				else if (item3.Key == PurchaseType.SurvivalRestart)
				{
					manager.Metrics.AddSpend().AddResources(this).AddDistance()
						.AddRestart()
						.Send();
				}
				else if (item3.Key == PurchaseType.SurvivalDoubleRewards)
				{
					manager.Metrics.AddSpend().AddResources(this).AddDistance()
						.DoubleRewards()
						.Send();
				}
				else if (sourceObject is TradeSlotInfo)
				{
					if (item3.Key == PurchaseType.TradeCrate)
					{
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddTradeCrate(sourceObject as TradeSlotInfo)
							.Send();
					}
				}
				else if (sourceObject is GuildShopItemInfo)
				{
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddGvGCrate(sourceObject as GuildShopItemInfo)
						.AddGvG()
						.Send();
				}
				else if (item3.Key == PurchaseType.RewardUnlock)
				{
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddExtraUnlock()
						.AddMission()
						.AddMissionType()
						.Send();
				}
				else if (sourceObject is CombatModel)
				{
					if (item3.Key == PurchaseType.RechargeCurrency)
					{
						if ((sourceObject as CombatModel).MapCategory != MapCategory.Outpost)
						{
							manager.Metrics.metricsResourcesData.BoughtGold = 0;
							manager.Metrics.metricsResourcesData.FreeGold = 0;
							ExchangedDiamonds = 0;
							manager.Metrics.ResetTdEvent().AddOriginalEventType();
							manager.Metrics.AddSpend().AddResources(this).AddStart()
								.AddMission()
								.AddMissionType()
								.AddMissionTeam()
								.Send();
							manager.Metrics.TdEventType = "Start_Mission";
							manager.Metrics.TdEventPropertyTypes = new List<string> { "Mission", "GvG", "PvP", "Grind", "Challenge", "ApocalypticChallenge", "Distance", "Season", "Endless", "Story" };
							manager.Metrics.SendTdEvent();
						}
					}
					else if (item3.Key == PurchaseType.GuildBattleAttackMission || item3.Key == PurchaseType.GvGMissionRetry)
					{
						manager.Metrics.ResetTdEvent().AddOriginalEventType();
						manager.Metrics.AddSpend().AddResources(this).AddStart()
							.AddGvG()
							.AddGvGBattle()
							.AddMission()
							.AddGvGPvPInfoIfNeeded()
							.Send();
						manager.Metrics.TdEventType = "Start_Mission";
						manager.Metrics.TdEventPropertyTypes = new List<string> { "Mission", "GvG", "GvGBattle" };
						manager.Metrics.SendTdEvent();
					}
					else if (item3.Key == PurchaseType.EndlessPass)
					{
						string endlessModeGameModeType = manager.Player.EndlessModeManager.EndlessModeGameModeType.ToString();
						manager.Metrics.ResetTdEvent().AddOriginalEventType();
						manager.Metrics.ResourceChangeUsedReason = " LastStandMC";
						manager.Metrics.AddSpend().AddResources(this).AddStart()
							.AddMission(this)
							.AddEndless(endlessModeGameModeType)
							.AddMissionTeam()
							.Send();
						manager.Metrics.TdEventType = "Start_Mission";
						manager.Metrics.TdEventPropertyTypes = new List<string> { "Mission", "Endless" };
						manager.Metrics.SendTdEvent();
					}
				}
				else if (item3.Key == PurchaseType.Attack)
				{
					manager.Metrics.AddSpend().AddResources(this).AddSearch()
						.AddPvp()
						.AddPvpAttacker()
						.Send();
				}
				else if (sourceObject is ActorDefinition)
				{
					if (item3.Key == PurchaseType.UnlockHero && sourceObject is ActorDefinition)
					{
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddHeroUnlock()
							.Send();
					}
				}
				else if (sourceObject is UpgradeTraitsData)
				{
					if (sourceObject is UpgradeTraitsData)
					{
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddEquipmentRemodel()
							.Send();
					}
				}
				else if (item3.Key == PurchaseType.GoldShopDefinition)
				{
					if (sourceObject is GoldShopDefinition goldShopDefinition)
					{
						manager.Metrics.AddSpend().AddResources(this).AddBuy()
							.AddComponentCrate(goldShopDefinition)
							.Send();
					}
				}
				else if (item3.Key == PurchaseType.CraftBadge)
				{
					manager.Metrics.ResetTdEvent().AddOriginalEventType();
					manager.Metrics.AddSpend().AddResources(this).AddCrafting(CraftingType.Badge, metricsOptions)
						.Send();
					manager.Metrics.TdEventType = "Spend_Resources_Crafting";
					manager.Metrics.TdEventPropertyTypes = new List<string> { "Crafting" };
					manager.Metrics.SendTdEvent();
				}
				else if (item3.Key == PurchaseType.GuildBattleStart)
				{
					GvGSeasonModelPlayer gvGSeasonModelPlayer = manager.Player.GvGSeasonModelPlayer;
					_ = gvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel;
					manager.Metrics.AddSpend().AddResources(this).AddGvG();
					bool fromPlayer = gvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentGuildBattle();
					manager.Metrics.AddGvGBattle(fromPlayer);
					manager.Metrics.AddStart().Send();
				}
				else if (item3.Key == PurchaseType.BlackMarket)
				{
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddBlackMarket(sourceObject as BlackMarketDefinition)
						.Send();
				}
				else if (item3.Key == PurchaseType.RefreshBlackMarket)
				{
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddBlackMarketRefresh()
						.Send();
				}
				else if (item3.Key == PurchaseType.HillTopStore)
				{
					manager.Metrics.AddSpend().AddResources(this).AddBuy()
						.AddHillTopStore(sourceObject as HillTopStoreDefinition)
						.Send();
				}
				SubtractFromBoughtDiamonds(item3.Value, ref boughtDiamondsCount);
				manager.Metrics.Reset();
			}
		}

		public void FakeSendPurchaseAnalyticsEvent()
		{
			manager.Metrics.ResetTdEvent();
			manager.Metrics.AddSpend().AddResources(this).AddStart()
				.AddMission()
				.AddMissionType()
				.AddMissionTeam()
				.Reset();
			manager.Metrics.Reset();
			manager.Metrics.TdEventType = "Start_Mission";
			manager.Metrics.TdEventPropertyTypes = new List<string> { "Mission", "GvG", "PvP", "Grind", "Challenge", "ApocalypticChallenge", "Distance", "Season", "Endless", "Story" };
			manager.Metrics.SendTdEvent();
		}

		private void SubtractFromBoughtDiamonds(Dictionary<CurrencyType, int> priceDictionary, ref int boughtDiamondsCount)
		{
			if (priceDictionary.ContainsKey(CurrencyType.Diamonds))
			{
				int num = priceDictionary[CurrencyType.Diamonds];
				boughtDiamondsCount = Math.Max(boughtDiamondsCount - num, 0);
			}
		}

		public Dictionary<CurrencyType, OverflowableAmount> Refund(int percentage = 100, bool dontAllowMultiplier = false, List<CurrencyType> fullRefundForCurrencies = null)
		{
			Dictionary<CurrencyType, OverflowableAmount> dictionary = new Dictionary<CurrencyType, OverflowableAmount>();
			int count = manager.Player.Currencies.Count;
			for (int i = 0; i < count; i++)
			{
				CurrencyModel currencyModel = manager.Player.Currencies[i];
				int num = GetTotalCost(currencyModel.Type);
				if (num > 0)
				{
					if (fullRefundForCurrencies == null || !fullRefundForCurrencies.Contains(currencyModel.Type))
					{
						num = num * percentage / 100;
					}
					currencyModel.Add(num, canOverflowMax: false, dontAllowMultiplier);
					num = ((!dontAllowMultiplier) ? ((int)(num * currencyModel.AddMultiplier)) : num);
					int overflow = ((num != currencyModel.LastAdded) ? (num - currencyModel.LastAdded) : 0);
					dictionary[currencyModel.Type] = new OverflowableAmount
					{
						Amount = currencyModel.LastAdded,
						Overflow = overflow
					};
				}
			}
			LastRefundAmounts = dictionary;
			return dictionary;
		}

		protected TWDModelResult ExchangeDiamondsIfNeeded(ref int outExchangedDiamonds)
		{
			int num = 0;
			for (int i = 0; i < (int)CurrencyType.Count; i++)
			{
				CurrencyType currencyType = (CurrencyType)i;
				if (GetMissing(currencyType) > 0)
				{
					if (!manager.GameEconomyData.CanConvertToDiamonds(currencyType))
					{
						return TWDModelResult.Error;
					}
					num += manager.GameEconomyData.CurrencyToDiamonds(currencyType, GetMissing(currencyType), manager.Player);
				}
			}
			if (GetMissing(CurrencyType.Diamonds) > 0)
			{
				return TWDModelResult.NotEnoughCurrency;
			}
			if (num > manager.Player.GetCurrency(CurrencyType.Diamonds).Value)
			{
				return TWDModelResult.NotEnoughCurrency;
			}
			if (manager.GameEconomyData.GetFeature("ClientServerGoldCheck").Enabled && UseDiamondsAmount != -2)
			{
				if (UseDiamondsAmount == -1)
				{
					if (manager.GameEconomyData.GetFeature("ClientServerGoldCheckWarning").Enabled)
					{
						manager.Debug.LogWarning("We tried to pay but the UseDiamondsAmount was equal to UseDiamondsAmountNotSet");
					}
				}
				else if (num != UseDiamondsAmount)
				{
					return TWDModelResult.DiamondExchangeDesync;
				}
			}
			if (!CanAfford())
			{
				TWDModelResult currencyWithDiamonds = GetCurrencyWithDiamonds();
				if (currencyWithDiamonds != TWDModelResult.OK)
				{
					return currencyWithDiamonds;
				}
				outExchangedDiamonds = num;
			}
			return TWDModelResult.OK;
		}

		public TWDModelResult GetCurrencyWithDiamonds()
		{
			PlayerModel player = manager.Player;
			CurrencyModel currency = player.GetCurrency(CurrencyType.Diamonds);
			int num = 0;
			for (int i = 0; i < (int)CurrencyType.Count; i++)
			{
				int missing = GetMissing((CurrencyType)i);
				if (missing == 0)
				{
					continue;
				}
				if (!manager.GameEconomyData.CanConvertToDiamonds((CurrencyType)i))
				{
					return TWDModelResult.Error;
				}
				int num2 = manager.GameEconomyData.CurrencyToDiamonds((CurrencyType)i, missing, player);
				num += num2;
				if (i == 10)
				{
					int num3 = ((currency.Bought >= num2) ? num2 : currency.Bought);
					int num4 = ((currency.Bought < num2) ? (num2 - currency.Bought) : 0);
					manager.Metrics.PushResource(CurrencyType.Diamonds, -num3, 0, freeResource: false);
					manager.Metrics.PushResource(CurrencyType.Diamonds, -num4);
					if (manager.Metrics.metricsResourcesData.HasResources())
					{
						manager.Metrics.ResourceChangeTmpDiamondSubtract = -num;
						manager.Metrics.AddSpend().AddResources().AddBuy()
							.AddFillTank()
							.Send();
						manager.Metrics.ResourceChangeTmpDiamondSubtract = 0;
					}
				}
			}
			if (num > currency.Value)
			{
				return TWDModelResult.NotEnoughCurrency;
			}
			currency.Subtract(num);
			int count = manager.Player.Currencies.Count;
			for (int j = 0; j < count; j++)
			{
				CurrencyModel currencyModel = manager.Player.Currencies[j];
				if (currencyModel.Type == CurrencyType.ReplayToken && !manager.Blackboard.IsToggleOn("BuyJustEnoughGasForMission"))
				{
					if (GetMissing(currencyModel.Type) > 0)
					{
						manager.Metrics.AddFind().AddResources(CurrencyType.ReplayToken, currencyModel.Max - currencyModel.Value, currencyModel.Max - currencyModel.Value).AddFillTank()
							.Send();
						currencyModel.AddFromDiamondExchange(currencyModel.Max);
					}
				}
				else
				{
					currencyModel.AddFromDiamondExchange(GetMissing(currencyModel.Type));
					if (currencyModel.Type == CurrencyType.ReplayToken)
					{
						manager.Blackboard.ClearToggle("BuyJustEnoughGasForMission");
					}
				}
			}
			return TWDModelResult.OK;
		}
	}
}
