using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseModel;
using BaseModel.ContentTypes;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class Metrics
	{
		public class MetricsResourcesData
		{
			public Dictionary<CurrencyType, OverflowableAmount> Resources = new Dictionary<CurrencyType, OverflowableAmount>();

			public int FreeGold { get; set; }

			public int BoughtGold { get; set; }

			public void Reset()
			{
				Resources.Clear();
				BoughtGold = 0;
				FreeGold = 0;
			}

			public MetricsResourcesData()
			{
			}

			public MetricsResourcesData(CurrencyType currencyType, int amount)
			{
				SetOrAdd(currencyType, amount);
			}

			public void SetOrAdd(CurrencyType currencyType, int amount, int overflow = 0)
			{
				if (Resources.ContainsKey(currencyType))
				{
					OverflowableAmount value = Resources[currencyType];
					value.Amount += amount;
					value.Overflow += overflow;
					Resources[currencyType] = value;
				}
				else
				{
					Resources[currencyType] = new OverflowableAmount
					{
						Amount = amount,
						Overflow = overflow
					};
				}
			}

			public bool HasResources()
			{
				return Resources.Count > 0;
			}
		}

		public enum UpgradeTypes
		{
			SpeedUp = 0,
			Instant = 1,
			Regular = 2,
			SpeedUpAndUpgrade = 3,
			AdSpeedUpgrade = 4
		}

		public enum BundleSource
		{
			Unknown = 0,
			Auto = 1,
			Shop = 2,
			LimitedOfferCamp = 3,
			LimitedOfferSeason7Featured = 4,
			LimitedOfferMissionHub = 5,
			PhoneScreen = 6,
			PlayerHub = 7,
			Support = 8,
			Cheat = 9,
			MiniShop = 10,
			MissionStart = 11,
			IAPromo = 12,
			Combat = 13,
			Banana = 14,
			Subscription = 15,
			ConditionBundle = 16,
			TradeFairPay = 17,
			IAPBundleBanana = 18,
			None = 19
		}

		private static readonly List<CurrencyType> ResourceChangeCurrencies = new List<CurrencyType>
		{
			CurrencyType.Diamonds,
			CurrencyType.Phone,
			CurrencyType.Fairmoney,
			CurrencyType.HillTopCoin,
			CurrencyType.GoldRadio
		};

		public int ResourceChangeTmpDiamondSubtract;

		private StringBuilder eventType = new StringBuilder();

		private TWDModelManager manager;

		private Metrics walkerTapMetrics;

		private int numberWalkersTapped;

		public Dictionary<string, string> properties { get; private set; }

		public string TdEventType { private get; set; }

		private string OriginEventType { get; set; }

		private bool UseOriginalEventType { get; set; }

		public List<string> TdEventPropertyTypes { get; set; }

		public Dictionary<string, Dictionary<string, object>> tdProperties { get; private set; }

		public string ResourceChangeIsByCharging { private get; set; }

		public string ResourceChangeObtainReason { private get; set; } = "";

		public string ResourceChangeUsedReason { private get; set; } = "Consume";

		public MetricsResourcesData metricsResourcesData { get; private set; }

		public Metrics(TWDModelManager manager)
		{
			this.manager = manager;
			properties = new Dictionary<string, string>();
			tdProperties = new Dictionary<string, Dictionary<string, object>>();
			TdEventPropertyTypes = new List<string>();
			metricsResourcesData = new MetricsResourcesData();
		}

		public Metrics Reset()
		{
			eventType.Length = 0;
			properties.Clear();
			metricsResourcesData.Reset();
			return this;
		}

		public Metrics ResetTdEvent()
		{
			UseOriginalEventType = false;
			TdEventType = "";
			OriginEventType = "";
			TdEventPropertyTypes.Clear();
			tdProperties.Clear();
			return this;
		}

		private void AddAndResetOneTdPropertyType(string propertyType)
		{
			if (tdProperties.ContainsKey(propertyType))
			{
				tdProperties[propertyType].Clear();
			}
			else
			{
				tdProperties[propertyType] = new Dictionary<string, object>();
			}
		}

		private void AddTdProperty(string propertyType, string propertyKey, object propertyValue)
		{
			if (tdProperties.ContainsKey(propertyType))
			{
				tdProperties[propertyType].Add(propertyKey, propertyValue);
			}
		}

		private void AddEventType(string type)
		{
			if (eventType.Length == 0)
			{
				eventType.Append(type);
			}
			else
			{
				eventType.Append("_" + type);
			}
		}

		private void AddProperty(string name, string value)
		{
			if (properties.ContainsKey(name))
			{
				if (manager.GameEconomyData.GetFeature("BBQMetricError").Enabled)
				{
					manager.Debug.LogError("BBQ property was already added: " + name);
				}
			}
			else
			{
				properties.Add(name, value);
			}
		}

		private void AddProperty(string name, bool value)
		{
			AddProperty(name, value ? "1" : "0");
		}

		private void AddProperty(string name, int value)
		{
			AddProperty(name, value.ToString());
		}

		private void AddProperty(string name, long value)
		{
			AddProperty(name, value.ToString());
		}

		public Metrics AddBuilding(BuildingModel buildingModel)
		{
			AddEventType("Building");
			if (buildingModel != null)
			{
				AddProperty("Building_Name", string.IsNullOrEmpty(buildingModel.TypeName) ? "unknown_building" : buildingModel.TypeName);
				AddProperty("Building_Level", buildingModel.Level.ToString());
			}
			return this;
		}

		public Metrics AddRecycleWeapon(string weaponId, int breakthroughLv)
		{
			AddEventType("RecycleWeapon");
			AddProperty("RecycleWeaponID", weaponId);
			AddProperty("RecycleWeaponBreakthroughLv", breakthroughLv);
			return this;
		}

		public Metrics AddRecycleBlueprints(string blueprintsId, int num)
		{
			AddEventType("RecycleBlueprints");
			AddProperty("RecycleBlueprintsID", blueprintsId);
			AddProperty("RecycleBlueprintsNum", num);
			return this;
		}

		private void UpdateFreeAndBoughtGold(CurrencyType currencyType, int currencyAmount, bool freeResource = true)
		{
			if (currencyType == CurrencyType.Diamonds)
			{
				if (freeResource)
				{
					metricsResourcesData.FreeGold += currencyAmount;
				}
				else
				{
					metricsResourcesData.BoughtGold += currencyAmount;
				}
			}
			if (ResourceChangeIsByCharging == "")
			{
				if (freeResource)
				{
					ResourceChangeIsByCharging = "0";
				}
				else
				{
					ResourceChangeIsByCharging = "1";
				}
			}
		}

		public void PushResource(CurrencyType currencyType, int currencyAmount, int amountOverflown = 0, bool freeResource = true)
		{
			metricsResourcesData.SetOrAdd(currencyType, currencyAmount, amountOverflown);
			UpdateFreeAndBoughtGold(currencyType, currencyAmount, freeResource);
		}

		public Metrics AddResources(Dictionary<CurrencyType, OverflowableAmount> refundedAmounts)
		{
			if (refundedAmounts != null)
			{
				foreach (KeyValuePair<CurrencyType, OverflowableAmount> refundedAmount in refundedAmounts)
				{
					metricsResourcesData.SetOrAdd(refundedAmount.Key, refundedAmount.Value.Amount, refundedAmount.Value.Overflow);
				}
			}
			return AddResources();
		}

		public Metrics AddResources(Cashier cashier, bool refund = false, int percentageOfCost = 100)
		{
			List<CashierItem> cashierItems = cashier.GetCashierItems();
			for (int i = 0; i < cashierItems.Count; i++)
			{
				for (int j = 0; j < cashierItems[i].Cost.Length; j++)
				{
					int num = cashierItems[i].Cost[j];
					if (num > 0)
					{
						num = num * percentageOfCost / 100;
						if (!refund)
						{
							num = -num;
						}
						metricsResourcesData.SetOrAdd((CurrencyType)j, num);
					}
				}
			}
			AddExchangedResources(cashier);
			return AddResources();
		}

		public Metrics AddGuildVictoryPointsResources(int amountAdded)
		{
			AddEventType("Resources");
			AddProperty("GuildBattleVP_Delta", amountAdded);
			return this;
		}

		private void AddExchangedResources(Cashier cashier)
		{
			if (cashier.ExchangedDiamonds != 0)
			{
				metricsResourcesData.SetOrAdd(CurrencyType.Diamonds, -cashier.ExchangedDiamonds);
			}
			if (cashier.ExchangedCurrencies == null)
			{
				return;
			}
			foreach (KeyValuePair<CurrencyType, int> exchangedCurrency in cashier.ExchangedCurrencies)
			{
				if (exchangedCurrency.Value != 0 && exchangedCurrency.Key != CurrencyType.ReplayToken)
				{
					metricsResourcesData.SetOrAdd(exchangedCurrency.Key, exchangedCurrency.Value);
				}
			}
		}

		public Metrics AddResources(CashierItem cashierItem, bool refund = false, int percentageOfCost = 100)
		{
			for (int i = 0; i < cashierItem.Cost.Length; i++)
			{
				int num = cashierItem.Cost[i];
				if (num > 0)
				{
					num = num * percentageOfCost / 100;
					if (!refund)
					{
						num = -num;
					}
					metricsResourcesData.SetOrAdd((CurrencyType)i, num);
				}
			}
			return AddResources();
		}

		public Metrics AddResources(CurrencyType currencyType, int rewardedAmount, int actualAmountAdded, bool freeResource = true)
		{
			if (rewardedAmount == actualAmountAdded)
			{
				metricsResourcesData.SetOrAdd(currencyType, rewardedAmount);
			}
			else
			{
				metricsResourcesData.SetOrAdd(currencyType, actualAmountAdded, rewardedAmount - actualAmountAdded);
			}
			UpdateFreeAndBoughtGold(currencyType, rewardedAmount, freeResource);
			return AddResources();
		}

		public Metrics AddResources(MetricsResourcesData data, bool freeResource = true, bool combineDuplicates = false)
		{
			metricsResourcesData = data;
			foreach (KeyValuePair<CurrencyType, OverflowableAmount> resource in metricsResourcesData.Resources)
			{
				UpdateFreeAndBoughtGold(resource.Key, resource.Value.Amount, freeResource);
			}
			return AddResources(combineDuplicates);
		}

		public Metrics AddResources(bool combineDuplicates = false)
		{
			AddEventType("Resources");
			if (metricsResourcesData != null)
			{
				IDictionary<CurrencyType, OverflowableAmount> dictionary;
				if (combineDuplicates)
				{
					dictionary = new Dictionary<CurrencyType, OverflowableAmount>();
					foreach (KeyValuePair<CurrencyType, OverflowableAmount> resource in metricsResourcesData.Resources)
					{
						if (!dictionary.ContainsKey(resource.Key))
						{
							dictionary[resource.Key] = default(OverflowableAmount);
						}
						OverflowableAmount value = dictionary[resource.Key];
						value.Amount += resource.Value.Amount;
						value.Overflow += resource.Value.Overflow;
						dictionary[resource.Key] = value;
					}
				}
				else
				{
					dictionary = metricsResourcesData.Resources;
				}
				foreach (KeyValuePair<CurrencyType, OverflowableAmount> item in dictionary)
				{
					string text = ((item.Key == CurrencyType.SurvivalPoints) ? "XP" : ((item.Key == CurrencyType.Outpost) ? "TradeGoods" : ((item.Key != CurrencyType.Diamonds) ? item.Key.ToString() : "All_Gold")));
					if (item.Value.Amount != 0)
					{
						AddProperty(text + "_Delta", item.Value.Amount.ToString());
						AddAndResetOneTdPropertyType("RadioCall_Resource_Num");
						AddTdProperty("RadioCall_Resource_Num", "delta_type", text);
						AddTdProperty("RadioCall_Resource_Num", "delta_num", item.Value.Amount.ToString());
						AddAndResetOneTdPropertyType("ChallengeReward");
						AddTdProperty("ChallengeReward", "resource_name", text + "_Delta");
						AddTdProperty("ChallengeReward", "resource_num", item.Value.Amount.ToString());
						bool flag = item.Value.Amount > 0;
						CurrencyModel currency = manager.Player.GetCurrency(item.Key);
						if (ResourceChangeCurrencies.Contains(item.Key))
						{
							manager.TdMetrics.SetEventType("resource_change");
							if (flag)
							{
								if (item.Key == CurrencyType.Fairmoney || item.Key == CurrencyType.HillTopCoin)
								{
									if (!string.IsNullOrEmpty(ResourceChangeObtainReason))
									{
										manager.TdMetrics.AddProperty("change_reason", ResourceChangeObtainReason);
									}
									else
									{
										manager.TdMetrics.AddProperty("change_reason", "buy_bundle");
									}
								}
								else
								{
									manager.TdMetrics.AddProperty("change_reason", ResourceChangeObtainReason);
								}
							}
							else if (item.Key == CurrencyType.Fairmoney || item.Key == CurrencyType.HillTopCoin)
							{
								manager.TdMetrics.AddProperty("change_reason", "redeem_bundle");
							}
							else
							{
								manager.TdMetrics.AddProperty("change_reason", ResourceChangeUsedReason);
							}
						}
						else
						{
							manager.TdMetrics.SetEventType("item_change");
							manager.TdMetrics.AddProperty("change_reason", flag ? ResourceChangeObtainReason : ResourceChangeUsedReason);
						}
						if (flag)
						{
							manager.TdMetrics.AddProperty("is_by_recharging", ResourceChangeIsByCharging);
						}
						manager.TdMetrics.AddProperty("resource_id", item.Key.ToString()).AddProperty("resource_name", item.Key.ToString()).AddProperty("change_type", flag ? 1 : 0)
							.AddProperty("change_before", currency.Value - ResourceChangeTmpDiamondSubtract - item.Value.Amount)
							.AddProperty("change_num", flag ? item.Value.Amount : (-item.Value.Amount))
							.AddProperty("change_after", currency.Value + ResourceChangeTmpDiamondSubtract)
							.Send();
						bool flag2 = ResourceChangeCurrencies.Contains(item.Key);
						string value2 = ((!flag2) ? (flag ? ResourceChangeObtainReason : ResourceChangeUsedReason) : (flag ? ((item.Key != CurrencyType.Fairmoney && item.Key != CurrencyType.HillTopCoin) ? ResourceChangeObtainReason : ((!string.IsNullOrEmpty(ResourceChangeObtainReason)) ? ResourceChangeObtainReason : "buy_bundle")) : ((item.Key != CurrencyType.Fairmoney && item.Key != CurrencyType.HillTopCoin) ? ResourceChangeUsedReason : "redeem_bundle")));
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>
						{
							{
								"resource_id",
								item.Key.ToString()
							},
							{
								"resource_name",
								item.Key.ToString()
							},
							{
								"change_type",
								(flag ? 1 : 0).ToString()
							},
							{
								"change_before",
								(currency.Value - ResourceChangeTmpDiamondSubtract - item.Value.Amount).ToString()
							},
							{
								"change_num",
								(flag ? item.Value.Amount : (-item.Value.Amount)).ToString()
							},
							{
								"change_after",
								(currency.Value + ResourceChangeTmpDiamondSubtract).ToString()
							},
							{ "change_reason", value2 }
						};
						if (flag)
						{
							dictionary2["is_by_recharging"] = ResourceChangeIsByCharging;
						}
						manager.SendMetricsEvent(flag2 ? "resource_change" : "item_change", dictionary2);
					}
					if (item.Value.Overflow != 0)
					{
						AddProperty(text + "_Overflow", item.Value.Overflow.ToString());
					}
				}
				ResourceChangeObtainReason = "";
				ResourceChangeUsedReason = "";
				ResourceChangeIsByCharging = "";
				if (metricsResourcesData.FreeGold != 0)
				{
					AddProperty("Free_Gold_Delta", metricsResourcesData.FreeGold.ToString());
				}
				if (metricsResourcesData.BoughtGold != 0)
				{
					AddProperty("Bought_Gold_Delta", metricsResourcesData.BoughtGold.ToString());
				}
				metricsResourcesData.Reset();
			}
			return this;
		}

		public Metrics AddComponentCrate(GoldShopDefinition goldShopDefinition)
		{
			AddEventType("ComponentCrate");
			AddProperty("ItemId", goldShopDefinition.ItemId.ToString());
			AddProperty("Price", goldShopDefinition.Price.ToString());
			string value = "";
			if (goldShopDefinition.GuaranteedComponents != null)
			{
				value = string.Join(",", goldShopDefinition.GuaranteedComponents.ToArray());
			}
			AddProperty("GuaranteedComponents", value);
			AddProperty("RandomComponentCount", goldShopDefinition.RandomComponentCount.ToString());
			return this;
		}

		public Metrics AddRadioCallRewards(List<Dictionary<string, object>> list)
		{
			AddAndResetOneTdPropertyType("RewareRadioCall");
			AddTdProperty("RewareRadioCall", "delta_detail", JsonConvert.SerializeObject(list));
			return this;
		}

		public Metrics AddCrafting(CraftingType craftingType, string craftingId)
		{
			AddEventType("Crafting");
			AddProperty("Crafting_Type", craftingType.ToString());
			AddProperty("Crafting_Id", craftingId);
			AddAndResetOneTdPropertyType("Crafting");
			AddTdProperty("Crafting", "Crafting_Type", craftingType.ToString());
			AddTdProperty("Crafting", "Crafting_Id", craftingId);
			return this;
		}

		public Metrics AddEquip()
		{
			AddEventType("Equip");
			return this;
		}

		public Metrics AddUnequip()
		{
			AddEventType("Unequip");
			return this;
		}

		public Metrics AddSurvivor(SurvivorModel survivorModel, string eventType = "Survivor", string prefix = "Survivor")
		{
			AddEventType(eventType);
			AddAndResetOneTdPropertyType("Survivor");
			if (survivorModel != null)
			{
				AddProperty(prefix + "_Id", survivorModel.IdForAnalytics);
				AddProperty(prefix + "_Type", survivorModel.IsHero ? "Hero" : "Survivor");
				AddProperty(prefix + "_Class", survivorModel.SurvivorClass.ToString());
				if (survivorModel.IsHero)
				{
					AddProperty(prefix + "_Hero_Type", survivorModel.ActorDefinitionID.ToLower());
				}
				AddProperty(prefix + "_Starting_Level", survivorModel.StartingLevel.ToString());
				AddProperty(prefix + "_Level", survivorModel.Level.ToString());
				AddProperty(prefix + "_Name", survivorModel.Name);
				AddProperty(prefix + "_Starting_Rarity", ModelHelpers.GetRarityNameForAnalytics(survivorModel.StartingRarityLevel));
				AddProperty(prefix + "_Rarity", ModelHelpers.GetRarityNameForAnalytics(survivorModel.SurvivorRarityLevel));
				AddTdProperty("Survivor", prefix + "_Id", survivorModel.IdForAnalytics);
				AddTdProperty("Survivor", prefix + "_Type", survivorModel.IsHero ? "Hero" : "Survivor");
				AddTdProperty("Survivor", prefix + "_Class", survivorModel.SurvivorClass.ToString());
				if (survivorModel.IsHero)
				{
					AddTdProperty("Survivor", prefix + "_Hero_Type", survivorModel.ActorDefinitionID.ToLower());
				}
				AddTdProperty("Survivor", prefix + "_Starting_Level", survivorModel.StartingLevel.ToString());
				AddTdProperty("Survivor", prefix + "_Level", survivorModel.Level.ToString());
				AddTdProperty("Survivor", prefix + "_Name", survivorModel.Name);
				AddTdProperty("Survivor", prefix + "_Starting_Rarity", ModelHelpers.GetRarityNameForAnalytics(survivorModel.StartingRarityLevel));
				AddTdProperty("Survivor", prefix + "_Rarity", ModelHelpers.GetRarityNameForAnalytics(survivorModel.SurvivorRarityLevel));
				if (survivorModel.TraitContainer != null && survivorModel.TraitContainer.Traits != null)
				{
					SetUpgradeTraits(prefix, survivorModel.UpgradeTraits);
				}
			}
			return this;
		}

		public Metrics AddWinAndLose(ECombatResult combatResult)
		{
			AddAndResetOneTdPropertyType("ifwin");
			AddTdProperty("ifwin", "ifwin", combatResult.ToString());
			return this;
		}

		public Metrics AddMissionCategory(SurvivorModel survivorModel)
		{
			AddAndResetOneTdPropertyType("MissionCategory");
			manager.Metrics.ResourceChangeUsedReason = survivorModel.manager.CombatModel?.MapCategory.ToString();
			AddTdProperty("MissionCategory", "MissionCategory", survivorModel.manager.CombatModel?.MapCategory.ToString());
			return this;
		}

		public Metrics AddOldSurvivor(SurvivorModel survivorModel)
		{
			return AddSurvivor(survivorModel, "OldSurvivor", "Old_Survivor");
		}

		public Metrics AddNewSurvivor(SurvivorModel survivorModel)
		{
			return AddSurvivor(survivorModel, "NewSurvivor", "New_Survivor");
		}

		public Metrics AddHeroToken(CurrencyType heroCurrency, int tokenCount)
		{
			string text = heroCurrency.ToString();
			if (text.EndsWith("Token", StringComparison.InvariantCulture))
			{
				text = text.Substring(0, text.Length - 5);
			}
			AddEventType("HeroToken");
			AddProperty("Hero", text);
			AddProperty("N_Token", tokenCount.ToString());
			return this;
		}

		private void SetUpgradeTraits(string prefix, List<UpgradeTraitsData> upgradeTraits)
		{
			int num = 0;
			int num2 = 0;
			if (upgradeTraits != null)
			{
				for (int i = 0; i < upgradeTraits.Count; i++)
				{
					UpgradeTraitsData upgradeTraitsData = upgradeTraits[i];
					if (!upgradeTraitsData.IsTactical)
					{
						AddProperty(prefix + "_Trait_Name_" + num2, upgradeTraitsData.Identifier);
						int num3 = upgradeTraitsData.RarityLevel + 1;
						AddProperty(prefix + "_Trait_Level_" + num2, num3.ToString());
						num += num3;
						num2++;
					}
				}
			}
			AddProperty(prefix + "_Total_Trait_Count", num2.ToString());
			AddProperty(prefix + "_Total_Trait_Level", num.ToString());
		}

		public Metrics AddSurvivorResult(SurvivorModel survivorModel)
		{
			AddEventType("SurvivorResult");
			AddAndResetOneTdPropertyType("SurvivorResult");
			CombatModel combat = manager.Player.Combat;
			if (survivorModel != null && combat != null)
			{
				AddProperty("Survivor_Attacks_In_Mission", survivorModel.Statistics.HitsInflictedInMission.ToString());
				AddProperty("Survivor_Damage_Done", survivorModel.Statistics.TotalDamageInflictedInCombat.ToString());
				AddProperty("Survivor_Charge_Ability_Used", survivorModel.Statistics.NumberOfChargeAbilitiesUsedInMission.ToString());
				AddProperty("Survivor_Injuries_Taken", survivorModel.Statistics.HitsTakenInMission.ToString());
				AddProperty("Walkers_Killed_In_Mission", combat.MissionStatistics.WalkersKilled.ToString());
				AddProperty("Survivor_Total_Walkers_Killed", survivorModel.Statistics.NumberWalkersKilled.ToString());
				AddProperty("Survivor_Missions_Played", survivorModel.Statistics.NumberMissionPlayed.ToString());
				AddProperty("Survivor_Days_Alive", survivorModel.Statistics.GetNumberDaysAlive().ToString());
				DetermineSurvivorMissionInjuries(survivorModel, out var _, out var healthPercentageAfterCombat);
				AddProperty("Survivor_Injury_End", healthPercentageAfterCombat.ToString());
				AddProperty("Survivor_Injury", survivorModel.InjuryType.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Attacks_In_Mission", survivorModel.Statistics.HitsInflictedInMission.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Damage_Done", survivorModel.Statistics.TotalDamageInflictedInCombat.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Charge_Ability_Used", survivorModel.Statistics.NumberOfChargeAbilitiesUsedInMission.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Injuries_Taken", survivorModel.Statistics.HitsTakenInMission.ToString());
				AddTdProperty("SurvivorResul", "Walkers_Killed_In_Mission", combat.MissionStatistics.WalkersKilled.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Total_Walkers_Killed", survivorModel.Statistics.NumberWalkersKilled.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Missions_Played", survivorModel.Statistics.NumberMissionPlayed.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Days_Alive", survivorModel.Statistics.GetNumberDaysAlive().ToString());
				AddTdProperty("SurvivorResul", "Survivor_Injury_End", healthPercentageAfterCombat.ToString());
				AddTdProperty("SurvivorResul", "Survivor_Injury", survivorModel.InjuryType.ToString());
			}
			return this;
		}

		public Metrics AddBadge(BadgeModel badge)
		{
			AddEventType("Badge");
			AddProperty("Badge_Id", badge.AnalyticsId.ToString());
			AddProperty("Badge_Name", badge.GenerateName());
			AddProperty("Badge_Shape", badge.SlotIndex.ToString());
			AddProperty("Badge_Rarity", badge.Rarity.ToString());
			AddProperty("Badge_Type", badge.Type.ToString());
			AddProperty("Badge_Effect", badge.EffectId);
			AddProperty("Badge_Bonus_Condition", badge.BonusId);
			AddAndResetOneTdPropertyType("Badge");
			AddTdProperty("Badge", "Badge_Id", badge.AnalyticsId);
			AddTdProperty("Badge", "Badge_Name", badge.GenerateName());
			AddTdProperty("Badge", "Badge_Shape", badge.SlotIndex);
			AddTdProperty("Badge", "Badge_Rarity", badge.Rarity);
			AddTdProperty("Badge", "Badge_Type", badge.Type.ToString());
			AddTdProperty("Badge", "Badge_Effect", badge.EffectId);
			AddTdProperty("Badge", "Badge_Bonus_Condition", badge.BonusId);
			return this;
		}

		public Metrics AddEquipmentWeapon(EquipmentItemModel equipment)
		{
			return AddEquipment(equipment, "EquipmentWeapon");
		}

		public Metrics AddEquipmentArmor(EquipmentItemModel equipment)
		{
			return AddEquipment(equipment, "EquipmentArmor");
		}

		public Metrics AddBadgeList(ModelList<BadgeModel> badges, SurvivorModel survivor, List<SurvivorModel> missionRoster)
		{
			AddEventType("BadgeList");
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < badges.Count; i++)
			{
				BadgeModel badgeModel = badges[i];
				stringBuilder.Append(badgeModel.AnalyticsId.ToString());
				if (i + 1 < badges.Count)
				{
					stringBuilder.Append(",");
				}
			}
			AddProperty("BadgeList_Id", stringBuilder.ToString());
			List<ActorModel> list = new List<ActorModel>();
			foreach (SurvivorModel item in missionRoster)
			{
				list.Add(item);
			}
			BadgeContext context = new BadgeContext(survivor, list);
			stringBuilder = new StringBuilder();
			for (int j = 0; j < badges.Count; j++)
			{
				BadgeModel badgeModel2 = badges[j];
				bool flag = false;
				if (badgeModel2.BonusCondition != null)
				{
					flag = badgeModel2.BonusCondition.Evaluate(context);
				}
				stringBuilder.Append(flag ? "1" : "0");
				if (j + 1 < badges.Count)
				{
					stringBuilder.Append(",");
				}
			}
			AddProperty("BadgeList_Bonus_Condition_Activity", stringBuilder.ToString());
			stringBuilder = new StringBuilder();
			for (int k = 0; k < badges.Count; k++)
			{
				BadgeModel badgeModel3 = badges[k];
				bool flag2 = survivor.BadgeContainer.HasSetBonus(badgeModel3.Type);
				stringBuilder.Append(flag2 ? "1" : "0");
				if (k + 1 < badges.Count)
				{
					stringBuilder.Append(",");
				}
			}
			AddProperty("BadgeList_SetBonus", stringBuilder.ToString());
			return this;
		}

		public Metrics AddEquipment(EquipmentItemModel equipment, string equipmentPropertyPrefix = "Equipment", int amount = 1)
		{
			AddEventType(equipmentPropertyPrefix);
			AddAndResetOneTdPropertyType("ChallengeReward_challenge_equip");
			AddAndResetOneTdPropertyType("Equipment");
			if (equipment != null)
			{
				AddProperty(equipmentPropertyPrefix + "_Id", string.IsNullOrEmpty(equipment.IdForAnalytics) ? "0" : equipment.IdForAnalytics);
				AddProperty(equipmentPropertyPrefix + "_Category", equipment.IsConsumable ? "Consumable" : (equipment.IsWeaponEquipment ? "Weapon" : "Armor"));
				AddTdProperty("Equipmen", equipmentPropertyPrefix + "_Id", string.IsNullOrEmpty(equipment.IdForAnalytics) ? "0" : equipment.IdForAnalytics);
				AddTdProperty("Equipmen", equipmentPropertyPrefix + "_Category", equipment.IsConsumable ? "Consumable" : (equipment.IsWeaponEquipment ? "Weapon" : "Armor"));
				EquipmentDefinition definition = equipment.Definition;
				if (definition != null)
				{
					AddProperty(equipmentPropertyPrefix + "_Type", definition.Type.ToString());
					AddProperty(equipmentPropertyPrefix + "_Name", definition.ID);
					AddProperty(equipmentPropertyPrefix + "_Starting_Level", equipment.StartingLevel.ToString());
					AddProperty(equipmentPropertyPrefix + "_Level", equipment.Level.ToString());
					AddProperty(equipmentPropertyPrefix + "_Rarity", ModelHelpers.GetRarityNameForAnalytics(equipment.RarityLevel));
					AddTdProperty("Equipmen", equipmentPropertyPrefix + "_Type", definition.Type.ToString());
					AddTdProperty("Equipmen", equipmentPropertyPrefix + "_Name", definition.ID);
					AddTdProperty("Equipmen", equipmentPropertyPrefix + "_Starting_Level", equipment.StartingLevel.ToString());
					AddTdProperty("Equipmen", equipmentPropertyPrefix + "_Level", equipment.Level.ToString());
					AddTdProperty("Equipmen", equipmentPropertyPrefix + "_Rarity", ModelHelpers.GetRarityNameForAnalytics(equipment.RarityLevel));
					string propertyValue = definition.ID + "_" + ModelHelpers.GetRarityNameForAnalytics(equipment.RarityLevel) + "_" + equipment.Level;
					AddTdProperty("ChallengeReward_challenge_equip", "resource_equip_name", propertyValue);
					AddTdProperty("ChallengeReward_challenge_equip", "resource_equip_num", "1");
				}
				SetUpgradeTraits(equipmentPropertyPrefix, equipment.UpgradeTraits);
				AddProperty(equipmentPropertyPrefix + "_Amount", (amount == 0) ? 1 : amount);
			}
			return this;
		}

		public Metrics AddMission(Cashier cashier = null)
		{
			AddEventType("Mission");
			AddAndResetOneTdPropertyType("Mission");
			PlayerModel player = manager.Player;
			MapContainerModel mapContainerModel = player.MapContainerModel;
			MapMissionModel attackTargetMissionModel = player.MapContainerModel.AttackTargetMissionModel;
			GuildBattleMapMissionModel attackTargetMissionModel2 = manager.Player.GuildBattlePlayer.AttackTargetMissionModel;
			CombatModel combat = player.Combat;
			if (combat != null)
			{
				AddProperty("Combat_Id", combat.IdForAnalytics);
				AddTdProperty("Mission", "Combat_Id", combat.IdForAnalytics);
				string text = "{";
				for (int i = 0; i < combat.AllActors.Count; i++)
				{
					ActorModel actorModel = combat.AllActors[i];
					if (actorModel.Faction == Faction.Survivor)
					{
						text += actorModel.Name;
						text += ": ";
						text += "{";
						text += ((actorModel.EquipmentItems.Count() < 2) ? "" : actorModel.EquipmentItems[0]?.Definition.ID);
						text += "}, {";
						text += ((actorModel.EquipmentItems.Count() < 2) ? "" : actorModel.EquipmentItems[1]?.Definition.ID);
						text += "},     ";
						text += "\n";
					}
				}
				text += "}";
				AddProperty("HeroWithWeapon", text);
				AddTdProperty("Mission", "HeroWithWeapon", text);
			}
			if (attackTargetMissionModel2 != null)
			{
				AddProperty("Mission_Kind", GetMissionKind());
				AddProperty("Mission_Name_Eng", attackTargetMissionModel2.Id);
				AddTdProperty("Mission", "Mission_Kind", GetMissionKind());
				AddTdProperty("Mission", "Mission_Name_Eng", attackTargetMissionModel2.Id);
				if (combat != null)
				{
					List<TWDModelObject> models = combat.GetModels<CoverModel>();
					bool flag = models != null && models.Count > 0;
					AddProperty("Mission_Has_Covers", flag.ToString());
					AddTdProperty("Mission", "Mission_Has_Covers", flag);
					AddWalkerTypes(combat.SurvivalMission);
				}
				if (manager.Player.GuildModel != null)
				{
					GuildBattleMapModel currentMapModel = manager.Player.GuildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.CurrentMapModel;
					int sectorId = attackTargetMissionModel2.SectorIdOwner;
					int areaIndex = attackTargetMissionModel2.AreaIndex;
					if (areaIndex > -1)
					{
						GuildBattlePvpTeam guildBattlePvpTeam = currentMapModel.PVPTeamsListPerSector[sectorId][areaIndex];
						int index = -1;
						currentMapModel.PvpTeamsIndexPerMission.TryGetValue(guildBattlePvpTeam.MissionId, out var value);
						GuildBattleMapModel.ParsePvpTeamIndexId(value, out sectorId, out index);
						bool isPvPCombat = manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMission.IsPvPCombat;
						AddProperty("Mission_Difficulty", attackTargetMissionModel2.MissionDifficultyLevel);
						AddProperty("Enemy_Id", guildBattlePvpTeam.OwnerHashedPlayerId);
						AddProperty("Enemy_Index", index);
						AddProperty("Sector_Number", attackTargetMissionModel2.SectorIdOwner);
						AddProperty("Sector_Area", attackTargetMissionModel2.AreaIndex);
						AddProperty("Mission_Position", attackTargetMissionModel2.MissionPositionWithinArea);
						AddTdProperty("Mission", "Mission_Difficulty", attackTargetMissionModel2.MissionDifficultyLevel);
						if (attackTargetMissionModel2.SectorModelOwner != null)
						{
							AddProperty("Mission_Id", attackTargetMissionModel2.MissionIdFromDefinition);
							AddTdProperty("Mission", "Mission_ID", attackTargetMissionModel2.MissionIdFromDefinition);
						}
						AddProperty("is_GvG_PvP", isPvPCombat);
						SurvivalMissionConfig.SurvivalObjectiveType survivalMissionObjectiveType = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat);
						AddProperty("Mission_Objective_Type", survivalMissionObjectiveType.ToString());
						if (survivalMissionObjectiveType == SurvivalMissionConfig.SurvivalObjectiveType.KillAmountAndExit)
						{
							AddProperty("Mission_Kills_Required", combat.SurvivalMission.KillsRequired.ToString());
						}
					}
				}
			}
			else if (attackTargetMissionModel != null && attackTargetMissionModel.manager != null)
			{
				if (attackTargetMissionModel.MissionId == attackTargetMissionModel.manager.GameEconomyData.ConfigData.OutpostTutorialMissionId)
				{
					AddProperty("Mission_Kind", "pvp");
					AddProperty("Mission_Code", "O00");
					AddTdProperty("Mission", "Mission_Kind", "pvp");
					AddTdProperty("Mission", "Mission_Code", "000");
					if (combat != null && combat.SceneName != null)
					{
						AddProperty("Mission_Name_Eng", combat.SceneName);
						AddTdProperty("Mission", "Mission_Name_Eng", combat.SceneName);
					}
					return this;
				}
				MissionData missionData = attackTargetMissionModel.MissionData;
				MapMissionGroupModel missionGroupModelThatContains = mapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
				MapCategory mapCategory = ((attackTargetMissionModel.MissionSpawnPointGroup != null) ? attackTargetMissionModel.MissionSpawnPointGroup.Category : MapCategory.None);
				int num = manager.Player.MapContainerModel.GetEpisodeIndex(attackTargetMissionModel) + 1;
				int num2 = manager.Player.MapContainerModel.GetMissionIndex(attackTargetMissionModel) + 1;
				AddProperty("Mission_Difficulty", attackTargetMissionModel.MissionLevel.ToString());
				AddTdProperty("Mission", "Mission_Difficulty", attackTargetMissionModel.MissionLevel);
				if (missionData != null)
				{
					AddProperty("Mission_Kind", GetMissionKind());
					AddProperty("Mission_ID", missionData.Id);
					AddProperty("Mission_Name_Eng", GetMissionNameEnglishForAnalytics(null, attackTargetMissionModel.MissionSpawnPointGroup.Category));
					AddTdProperty("Mission", "Mission_Kind", GetMissionKind());
					AddTdProperty("Mission", "Mission_ID", missionData.Id);
					AddTdProperty("Mission", "Mission_Name_Eng", GetMissionNameEnglishForAnalytics(null, attackTargetMissionModel.MissionSpawnPointGroup.Category));
					if (combat != null)
					{
						List<TWDModelObject> models2 = combat.GetModels<CoverModel>();
						bool flag2 = models2 != null && models2.Count > 0;
						AddProperty("Mission_Has_Covers", flag2.ToString());
						AddTdProperty("Mission", "Mission_Has_Covers", flag2);
					}
					AddProperty("Mission_Loot_Preference_Tag", attackTargetMissionModel.LootTag.ToString());
					if (missionGroupModelThatContains != null)
					{
						AddProperty("Mission_Number", (mapContainerModel.GetMissionIndex(attackTargetMissionModel) + 1).ToString());
						AddTdProperty("Mission", "Mission_Number", mapContainerModel.GetMissionIndex(attackTargetMissionModel) + 1);
						if (mapCategory == MapCategory.Challenge || mapCategory == MapCategory.ApocalypticChallenge)
						{
							AddProperty("Mission_Code", missionGroupModelThatContains.MissionSpawnPointGroup.MapId + num2.ToString("D2"));
							AddTdProperty("Mission", "Mission_Code", missionGroupModelThatContains.MissionSpawnPointGroup.MapId + num2.ToString("D2"));
							AddProperty("Is_Master_Mission", attackTargetMissionModel.IsMasterMission);
						}
					}
				}
				if (combat.SurvivalMission != null)
				{
					AddWalkerTypes(combat.SurvivalMission);
				}
				else if (missionData != null)
				{
					AddWalkerTypes(missionData);
				}
				switch (mapCategory)
				{
				case MapCategory.Season:
				{
					int result = -1;
					if (attackTargetMissionModel.MissionSpawnPointGroup.MapId.Length > 1)
					{
						int.TryParse(attackTargetMissionModel.MissionSpawnPointGroup.MapId.Substring(1, 1), out result);
					}
					AddProperty("Mission_Code", "S" + result.ToString("D2") + "E" + num.ToString("D2") + "M" + num2.ToString("D2"));
					AddTdProperty("Mission", "Mission_Code", "S" + result.ToString("D2") + "E" + num.ToString("D2") + "M" + num2.ToString("D2"));
					break;
				}
				case MapCategory.Story:
					AddProperty("Mission_Code", "E" + num.ToString("D2") + "M" + num2.ToString("D2"));
					AddTdProperty("Mission", "Mission_Code", "E" + num.ToString("D2") + "M" + num2.ToString("D2"));
					break;
				case MapCategory.Grind:
					AddProperty("Mission_Code", "G" + attackTargetMissionModel.MissionLevel.ToString("D2"));
					AddTdProperty("Mission", "Mission_Code", "G" + attackTargetMissionModel.MissionLevel.ToString("D2"));
					break;
				case MapCategory.Survival:
					AddProperty("Mission_Code", "D" + num2.ToString("D2"));
					AddTdProperty("Mission", "Mission_Code", "D" + num2.ToString("D2"));
					break;
				case MapCategory.Endless:
					AddProperty("Mission_Code", $"Endless-{manager.Player.EndlessModeManager.Id}-" + num2.ToString("D2"));
					AddTdProperty("Mission", "Mission_Code", $"Endless-{manager.Player.EndlessModeManager.Id}-" + num2.ToString("D2"));
					break;
				}
				if (mapCategory == MapCategory.Endless)
				{
					AddProperty("Endless_Id", manager.Player.EndlessModeManager.Id);
					int num3 = 0;
					num3 = ((manager.Player.EndlessModeManager.EndlessModeGameModeType != EndlessModeGameModeType.Expert) ? manager.Player.EndlessModeManager.CurrentGoldAttemptCount : manager.Player.EndlessModeManager.CurrentExpertGoldAttemptCount);
					if (cashier != null && cashier.ExchangedDiamonds > 0)
					{
						num3++;
					}
					AddProperty("Endless_Iteration", num3);
				}
			}
			else if (combat != null && combat.HasPvPRules)
			{
				AddProperty("Mission_Kind", GetMissionKind());
				AddTdProperty("Mission", "Mission_Kind", GetMissionKind());
				if (player != null && player.Tutorial != null && player.Tutorial.CurrentPartId != null)
				{
					AddProperty("Mission_Code", "O00");
					AddTdProperty("Mission", "Mission_Code", "O00");
				}
				else
				{
					AddProperty("Mission_Code", "O");
					AddTdProperty("Mission", "Mission_Code", "O");
				}
				if (combat.SceneName != null)
				{
					AddProperty("Mission_Name_Eng", combat.SceneName);
					AddTdProperty("Mission", "Mission_Name_Eng", combat.SceneName);
				}
			}
			else if (player != null && player.Tutorial != null && player.Tutorial.CurrentPartId != null)
			{
				AddProperty("Mission_Kind", "story");
				AddProperty("Mission_ID", "0");
				AddProperty("Mission_Number", "0");
				AddProperty("Mission_Code", "T01");
				AddTdProperty("Mission", "Mission_Kind", "story");
				AddTdProperty("Mission", "Mission_ID", "0");
				AddTdProperty("Mission", "Mission_Number", 0);
				AddTdProperty("Mission", "Mission_Code", "T01");
				if (combat != null && combat.SceneName != null)
				{
					AddProperty("Mission_Name_Eng", combat.SceneName);
					AddTdProperty("Mission", "Mission_Name_Eng", combat.SceneName);
				}
			}
			return this;
		}

		public void AddWalkerTypes(MissionData missionData)
		{
			foreach (WalkerType value in Enum.GetValues(typeof(WalkerType)))
			{
				if (value != WalkerType.WalkerNormal && missionData.HasWalker(value))
				{
					AddProperty(value.ToString() + "_Included", 1);
				}
			}
		}

		public void AddWalkerTypes(SurvivalMissionConfig missionConfig)
		{
			foreach (WalkerType value in Enum.GetValues(typeof(WalkerType)))
			{
				if (value != WalkerType.WalkerNormal && missionConfig.HasWalker(value))
				{
					AddProperty(value.ToString() + "_Included", 1);
				}
			}
		}

		public Metrics AddGrind()
		{
			AddEventType("Grind");
			AddAndResetOneTdPropertyType("Grind");
			MapMissionModel attackTargetMissionModel = manager.Player.MapContainerModel.AttackTargetMissionModel;
			if (attackTargetMissionModel != null)
			{
				AddProperty("Grind_Difficulty", manager.GameEconomyData.GetGrindButtonDefinition(attackTargetMissionModel.GrindButtonDefinitionId).GrindDifficulty.ToString());
				AddTdProperty("Grind", "Grind_Difficulty", manager.GameEconomyData.GetGrindButtonDefinition(attackTargetMissionModel.GrindButtonDefinitionId).GrindDifficulty.ToString());
			}
			return this;
		}

		public Metrics AddStory()
		{
			AddEventType("Story");
			AddAndResetOneTdPropertyType("Story");
			MapContainerModel mapContainerModel = manager.Player.MapContainerModel;
			MapMissionModel attackTargetMissionModel = mapContainerModel.AttackTargetMissionModel;
			MapMissionGroupModel missionGroupModelThatContains = mapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
			if (missionGroupModelThatContains != null)
			{
				int num = mapContainerModel.GetEpisodeIndex(attackTargetMissionModel) + 1;
				AddProperty("Episode_Number", num.ToString());
				AddProperty("Episode_Difficulty", missionGroupModelThatContains.MissionSpawnPointGroup.EpisodeDifficultyLevel.ToString());
				AddProperty("Episode_Name", missionGroupModelThatContains.MissionSpawnPointGroup.DisplayName);
				AddTdProperty("Story", "Story_Episode_Number", num);
				AddTdProperty("Story", "Story_Episode_Difficulty", missionGroupModelThatContains.MissionSpawnPointGroup.EpisodeDifficultyLevel);
				AddTdProperty("Story", "Story_Episode_Name", missionGroupModelThatContains.MissionSpawnPointGroup.DisplayName);
			}
			return this;
		}

		public Metrics AddSeason()
		{
			AddEventType("Season");
			AddAndResetOneTdPropertyType("Season");
			MapContainerModel mapContainerModel = manager.Player.MapContainerModel;
			MapMissionModel attackTargetMissionModel = mapContainerModel.AttackTargetMissionModel;
			MapMissionGroupModel missionGroupModelThatContains = mapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
			if (missionGroupModelThatContains != null)
			{
				int num = mapContainerModel.GetEpisodeIndex(attackTargetMissionModel) + 1;
				AddProperty("Episode_Number", num.ToString());
				AddProperty("Episode_Difficulty", missionGroupModelThatContains.MissionSpawnPointGroup.EpisodeDifficultyLevel.ToString());
				AddProperty("Episode_Name", missionGroupModelThatContains.MissionSpawnPointGroup.DisplayName);
				AddTdProperty("Season", "Season_Episode_Number", num);
				AddTdProperty("Season", "Season_Episode_Difficulty", missionGroupModelThatContains.MissionSpawnPointGroup.EpisodeDifficultyLevel);
				AddTdProperty("Season", "Season_Episode_Name", missionGroupModelThatContains.MissionSpawnPointGroup.DisplayName);
				if (attackTargetMissionModel != null)
				{
					int result = -1;
					if (attackTargetMissionModel.MissionSpawnPointGroup.MapId.Length > 1)
					{
						int.TryParse(attackTargetMissionModel.MissionSpawnPointGroup.MapId.Substring(1, 1), out result);
					}
					AddProperty("Season_Number", result.ToString());
					AddTdProperty("Season", "Season_Number", result);
					if (attackTargetMissionModel.IsLastInGroup)
					{
						AddProperty("Season_Trial_Iteration", attackTargetMissionModel.CompletionTimes.ToString());
						AddTdProperty("Season", "Season_Trial_Iteration", attackTargetMissionModel.CompletionTimes);
					}
				}
			}
			return this;
		}

		public Metrics AddMissionTeam()
		{
			AddEventType("MissionTeam");
			if (manager == null || manager.Player == null || manager.Player.SurvivorContainer == null || manager.Player.SurvivorContainer.CombatSurvivors == null)
			{
				return this;
			}
			AddTeam(manager.Player.SurvivorContainer.CombatSurvivors, "Combat");
			if (manager.CombatModel.SupportManager == null)
			{
				manager.CombatModel.InitializeSupportManager();
			}
			CombatSupportManager supportManager = manager.CombatModel.SupportManager;
			int count = supportManager.Supports.Count;
			AddProperty("Combat_Support_Amount", count);
			if (count > 0)
			{
				int num = 0;
				int num2 = 0;
				foreach (CombatSupportModel support in supportManager.Supports)
				{
					num += support.SupportModel.Level;
					num2 += support.SupportModel.Cooldown;
				}
				AddProperty("Combat_Avg_Support_Rarity", num / count);
				AddProperty("Combat_Avg_Support_Cooldown", num2 / count);
			}
			return this;
		}

		private void DetermineSurvivorMissionInjuries(SurvivorModel survivor, out int healthPercentageBeforeCombat, out int healthPercentageAfterCombat)
		{
			SurvivalCharacterStateModel survivalCharacterStateModel = null;
			if (manager.Player.SurvivorContainer.SurvivalCharacters.IsSurvivorInSurvivalMode(survivor))
			{
				survivalCharacterStateModel = manager.Player.SurvivorContainer.SurvivalCharacters.GetSurvivorStateInSurvivalMode(survivor);
			}
			if (survivalCharacterStateModel == null)
			{
				healthPercentageBeforeCombat = 0;
				healthPercentageAfterCombat = 0;
				return;
			}
			healthPercentageBeforeCombat = survivalCharacterStateModel.HealthPercentageBeforeCombat;
			healthPercentageAfterCombat = (int)survivalCharacterStateModel.HealthPercentage;
			if (survivalCharacterStateModel.StrugglesLeftBeforeCombat > 0)
			{
				healthPercentageBeforeCombat += 100;
			}
			if (survivalCharacterStateModel.StrugglesLeft > 0)
			{
				healthPercentageAfterCombat += 100;
			}
			if (survivalCharacterStateModel.OutOfAction)
			{
				healthPercentageAfterCombat = 0;
			}
		}

		private void AddTeam(IList<SurvivorModel> survivors, string prefix)
		{
			if (survivors == null)
			{
				return;
			}
			int[] array = new int[6];
			Array.Clear(array, 0, array.Length);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int count = survivors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < count; i++)
			{
				SurvivorModel survivorModel = survivors[i];
				array[(int)survivorModel.SurvivorClass]++;
				if (survivorModel.IsHero)
				{
					num++;
				}
				else
				{
					num2++;
				}
				num3 += survivorModel.Level;
				num4 += survivorModel.SurvivorRarityLevel;
				int count2 = survivorModel.BadgeContainer.Badges.Count;
				num5 += count2;
				for (int j = 0; j < count2; j++)
				{
					if (survivorModel.BadgeContainer.HasSetBonus(survivorModel.BadgeContainer.Badges[j].Type))
					{
						num6++;
					}
				}
				DetermineSurvivorMissionInjuries(survivorModel, out var healthPercentageBeforeCombat, out var _);
				stringBuilder.Append(healthPercentageBeforeCombat.ToString());
				if (i + 1 < count)
				{
					stringBuilder.Append(",");
				}
			}
			AddProperty(prefix + "_Badges_Equiped", num5.ToString());
			AddProperty(prefix + "_Badge_Bonus_Condition_Activity", num6.ToString());
			AddProperty(prefix + "_Injuries_Start", stringBuilder.ToString());
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k] > 0)
				{
					SurvivorClass survivorClass = (SurvivorClass)k;
					AddProperty(prefix + "_" + survivorClass.ToString() + "_Amount", array[k].ToString());
				}
			}
			AddProperty(prefix + "_Hero_Amount", num.ToString());
			AddProperty(prefix + "_Regular_Amount", num2.ToString());
			if (count != 0)
			{
				AddProperty(prefix + "_Avg_Survivor_Level", (num3 / count).ToString());
				AddProperty(prefix + "_Avg_Survivor_Rarity", (num4 / count).ToString());
			}
			if (count > 0 && survivors[0] != null && survivors[0].IsHero)
			{
				TraitDefinition traitWithTag = survivors[0].GetTraitWithTag("FactionBuffTrait");
				AddProperty(prefix + "_Active_Leader_Trait", traitWithTag.Identifier);
			}
		}

		private void AddTeam(List<SurvivorClass> survivorClasses, List<int> survivorLevels, List<int> survivorRarityLevels, string prefix)
		{
			int[] array = new int[6];
			Array.Clear(array, 0, array.Length);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (survivorClasses != null)
			{
				for (int i = 0; i < survivorClasses.Count; i++)
				{
					array[(int)survivorClasses[i]]++;
				}
			}
			if (survivorLevels != null)
			{
				num3 = survivorLevels.Count;
				for (int j = 0; j < survivorLevels.Count; j++)
				{
					num += survivorLevels[j];
				}
			}
			if (survivorRarityLevels != null)
			{
				for (int k = 0; k < survivorRarityLevels.Count; k++)
				{
					num2 += survivorRarityLevels[k];
				}
			}
			for (int l = 0; l < array.Length; l++)
			{
				if (array[l] > 0)
				{
					SurvivorClass survivorClass = (SurvivorClass)l;
					AddProperty(prefix + "_" + survivorClass.ToString() + "_Amount", array[l].ToString());
				}
			}
			if (num3 != 0)
			{
				AddProperty(prefix + "_Avg_Survivor_Level", (num / num3).ToString());
				AddProperty(prefix + "_Avg_Survivor_Rarity", (num2 / num3).ToString());
			}
		}

		public Metrics AddMissionResult(ECombatResult combatResult)
		{
			AddEventType("MissionResult");
			AddAndResetOneTdPropertyType("MissionResult");
			CombatModel combat = manager.Player.Combat;
			if (combat == null)
			{
				return this;
			}
			switch (combatResult)
			{
			case ECombatResult.Successful:
				AddProperty("Combat_Result", "1");
				AddTdProperty("MissionResult", "Combat_Result", 1);
				break;
			case ECombatResult.Flee:
				AddProperty("Combat_Result", "2");
				AddTdProperty("MissionResult", "Combat_Result", 2);
				break;
			case ECombatResult.Failed:
				if (combat.MapCategory == MapCategory.Endless)
				{
					AddProperty("Combat_Result", combat.EndlessModeCombatModel.DefeatedByOverrun ? "4" : "3");
					AddTdProperty("MissionResult", "Combat_Result", combat.EndlessModeCombatModel.DefeatedByOverrun ? 4 : 3);
				}
				else
				{
					AddProperty("Combat_Result", "3");
					AddTdProperty("MissionResult", "Combat_Result", 3);
				}
				break;
			}
			if (combat.CombatStartTime > 0)
			{
				long num = manager.Player.UtcTimeStamp - combat.CombatStartTime;
				AddProperty("Combat_Gameplay_Time", (num / 1000).ToString());
				AddTdProperty("MissionResult", "Combat_Gameplay_Time", num / 1000);
				AddProperty("Combat_Resumes", "0");
			}
			else
			{
				AddProperty("Combat_Resumes", combat.SessionResumeCount.ToString());
				AddProperty("Combat_Gameplay_Time", "0");
				AddTdProperty("MissionResult", "Combat_Gameplay_Time", 0);
			}
			AddProperty("Combat_Turns_Used", combat.TurnManager.TurnCount.ToString());
			AddProperty("Combat_Threat_Level", combat.ThreatMeter.ThreatLevel.ToString());
			AddProperty("Combat_Walkers_Spawned", combat.SpawnedWalkerCount.ToString());
			AddProperty("Combat_Walkers_Killed", combat.MissionStatistics.WalkersKilled.ToString());
			AddProperty("Combat_Raiders_Killed", combat.MissionStatistics.RaidersKilled.ToString());
			AddProperty("Combat_Avg_Raiders_Level", Math.Round(UtilsMath.AverageOfList(combat.RaiderLevels), 2).ToString());
			AddProperty("Combat_Average_Walker_Level", Math.Round(UtilsMath.AverageOfList(combat.WalkerLevels), 2).ToString());
			if (combat.MissionRoster != null)
			{
				StringBuilder stringBuilder = null;
				StringBuilder stringBuilder2 = null;
				if (combat.MapCategory == MapCategory.Survival)
				{
					stringBuilder = new StringBuilder();
					stringBuilder2 = new StringBuilder();
				}
				int[] array = new int[5];
				Array.Clear(array, 0, array.Length);
				for (int i = 0; i < combat.MissionRoster.Count; i++)
				{
					SurvivorModel survivorModel = combat.MissionRoster[i];
					array[(int)survivorModel.InjuryType]++;
					if (combat.MapCategory == MapCategory.Survival)
					{
						DetermineSurvivorMissionInjuries(survivorModel, out var healthPercentageBeforeCombat, out var healthPercentageAfterCombat);
						stringBuilder.Append(healthPercentageBeforeCombat.ToString());
						stringBuilder2.Append(healthPercentageAfterCombat.ToString());
						if (i + 1 < combat.MissionRoster.Count)
						{
							stringBuilder.Append(",");
							stringBuilder2.Append(",");
						}
					}
				}
				AddProperty("Combat_Injuries_Minor", array[1].ToString());
				AddProperty("Combat_Injuries_Major", array[2].ToString());
				AddProperty("Combat_Injuries_Critical", array[3].ToString());
				if (combat.MapCategory == MapCategory.Survival)
				{
					AddProperty("Combat_Injuries_Start", stringBuilder.ToString());
					AddProperty("Combat_Injuries_End", stringBuilder2.ToString());
				}
			}
			if (GetMissionKind() == "weekly_challenge")
			{
				if (manager.Player != null && manager.Player.MapContainerModel != null)
				{
					MapMissionModel attackTargetMissionModel = manager.Player.MapContainerModel.AttackTargetMissionModel;
					if (attackTargetMissionModel != null && attackTargetMissionModel.manager != null && combatResult == ECombatResult.Successful)
					{
						MapMissionStars stars = attackTargetMissionModel.Stars;
						if (stars != null)
						{
							AddProperty("Challenge_Star_Result", stars.NumberStars.ToString());
							int num2 = stars.NumberStars - attackTargetMissionModel.PreviousNumberStars;
							if (num2 >= 0)
							{
								AddProperty("Challenge_Star_Delta", num2.ToString());
							}
						}
					}
				}
				if (manager.Player.WeeklyChallenge != null)
				{
					AddProperty("SkipDoubleRewardsActive", (manager.Player.WeeklyChallenge.ActiveSkipTokens > 0) ? "1" : "0");
				}
			}
			if (combat.MapCategory == MapCategory.Outpost)
			{
				int num3 = 0;
				if (combat.IsPvpDefendersKilled)
				{
					num3++;
				}
				if (combat.IsPvPFlagCollected)
				{
					num3++;
				}
				if (combat.IsPvPLootCollected)
				{
					num3++;
				}
				AddProperty("Pvp_Goal_Count", num3.ToString());
				AddProperty("Pvp_Goal_Defenders_Killed", combat.IsPvpDefendersKilled ? "1" : "0");
				AddProperty("Pvp_Goal_Flag_Collected", combat.IsPvPFlagCollected ? "1" : "0");
				AddProperty("Pvp_Goal_Resource_Looted", combat.IsPvPLootCollected ? "1" : "0");
				OutpostCombat outpostCombat = combat.OutpostCombat;
				if (outpostCombat != null && manager != null && manager.Player != null)
				{
					int finalRankingScoreChange = manager.Player.GetFinalRankingScoreChange(outpostCombat.AttackerInfluenceGain);
					int num4 = -outpostCombat.AttackerInfluenceLoss;
					AddProperty("Influence_Delta", ((combat.MissionResult == ECombatResult.Successful) ? finalRankingScoreChange : (-num4)).ToString());
					int finalRankingScoreChange2 = manager.Player.GetFinalRankingScoreChange(outpostCombat.DefenderInfluenceLoss);
					int defenderInfluenceGain = outpostCombat.DefenderInfluenceGain;
					AddProperty("Defender_Influence_Delta", ((combat.MissionResult == ECombatResult.Successful) ? (-finalRankingScoreChange2) : defenderInfluenceGain).ToString());
				}
			}
			else if (combat.MapCategory == MapCategory.Survival)
			{
				SurvivalMissionConfig.SurvivalObjectiveType survivalMissionObjectiveType = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat);
				AddProperty("Mission_Objective_Type", survivalMissionObjectiveType.ToString());
				if (survivalMissionObjectiveType == SurvivalMissionConfig.SurvivalObjectiveType.KillAmountAndExit)
				{
					AddProperty("Mission_Kills_Required", combat.SurvivalMission.KillsRequired.ToString());
				}
			}
			else if (combat.MapCategory == MapCategory.GuildBattle)
			{
				AddProperty("GvG_Enemies_Killed", combat.MissionStatistics.RaidersKilled);
			}
			else if (combat.MapCategory == MapCategory.Endless)
			{
				AddProperty("Endless_TotalScore", combat.EndlessModeCombatModel.CurrentScore);
				AddTdProperty("MissionResult", "Endless_Score", combat.EndlessModeCombatModel.CurrentScore);
				AddProperty("Endless_MultiplierReached", combat.EndlessModeCombatModel.MaxMultiplierReached.ToString());
				AddProperty("Endless_Max_Wave", combat.EndlessModeCombatModel.GetCurrentOverAllWaveIndex);
				AddProperty("Endless_Kill_Count", combat.MissionStatistics.WalkersKilled);
			}
			AddProperty("Consumables_Grenade_Amount_Used", combat.MissionStatistics.GrenadesUsed);
			AddProperty("Consumables_MedKit_Amount_Used", combat.MissionStatistics.MedKitsUsed);
			AddProperty("Consumables_Flare_Amount_Used", combat.MissionStatistics.FlaresUsed);
			AddProperty("Consumables_BlastGrenade_Amount_Used", combat.MissionStatistics.BlastGrenadesUsed);
			AddProperty("Consumables_Gore_Amount_Used", combat.MissionStatistics.GoreUsed);
			AddProperty("Consumables_Grenade_Turns", ListToString(combat.MissionStatistics.TurnsForGrenade, ','));
			AddProperty("Consumables_MedKit_Turns", ListToString(combat.MissionStatistics.TurnsForMedKits, ','));
			AddProperty("Consumables_Flare_Turns", ListToString(combat.MissionStatistics.TurnsForFlare, ','));
			AddProperty("Consumables_BlastGrenade_Turns", ListToString(combat.MissionStatistics.TurnsForBlastGrenade, ','));
			AddProperty("Consumables_Gore_Turns", ListToString(combat.MissionStatistics.TurnsForGore, ','));
			return this;
		}

		private static string ListToString<T>(List<T> items, char separator)
		{
			if (items == null)
			{
				return string.Empty;
			}
			string text = "";
			foreach (T item in items)
			{
				text = text + item.ToString() + separator;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text.Substring(0, text.Length - 1);
			}
			return "";
		}

		public Metrics AddSkipTokens(int skipTokenDelta, int currentSkipTokens)
		{
			AddEventType("SkipTokens");
			if (manager.Player.WeeklyChallenge != null)
			{
				AddProperty("SkipToken_Delta", skipTokenDelta.ToString());
				AddProperty("SkipToken_State", currentSkipTokens.ToString());
			}
			return this;
		}

		public Metrics AddApocalypseSkipTokens(int skipTokenDelta, int currentSkipTokens)
		{
			AddEventType("ApocalypseSkipTokens");
			if (manager.Player.ApocalypseWeeklyChallenge != null)
			{
				AddProperty("SkipToken_Delta", skipTokenDelta.ToString());
				AddProperty("SkipToken_State", currentSkipTokens.ToString());
			}
			return this;
		}

		public Metrics AddSkipRounds(int roundsSkipped, int difficultyBeforeSkips)
		{
			AddEventType("SkipRounds");
			AddProperty("Rounds_Skipped", roundsSkipped.ToString());
			AddProperty("Original_Starting_Difficulty", difficultyBeforeSkips.ToString());
			return this;
		}

		public Metrics AddApocalypeSkipRounds(int roundsSkipped, int difficultyBeforeSkips)
		{
			AddEventType("SkipRounds");
			AddProperty("Rounds_Skipped", roundsSkipped.ToString());
			AddProperty("Original_Starting_Difficulty", difficultyBeforeSkips.ToString());
			return this;
		}

		public Metrics AddRest(int restingSurvivorCount)
		{
			AddEventType("Rest");
			AddProperty("Rest_Type", "RestAll");
			AddProperty("Rest_Survivors_Affected", restingSurvivorCount.ToString());
			return this;
		}

		public Metrics AddSPEquipmentRemoldTraits(EquipmentItemModel equipmentItemModel)
		{
			AddEventType("SPEquipmentRemoldTraits");
			AddProperty("Remold_Equipment_Id", equipmentItemModel.Definition.ID);
			AddProperty("Remold_Original_Traits", string.Join(",", equipmentItemModel.SpEquipmentRemoldModel.SPTraitSlots.Select((SPTraitSlot t) => t.ToString())));
			AddProperty("Remold_New_Traits", string.Join(",", equipmentItemModel.SpEquipmentRemoldModel.PendingSPTraitSlots.Select((SPTraitSlot t) => t.ToString())));
			return this;
		}

		public Metrics AddEquipmentBreakthrough(EquipmentBreakthroughModel equipmentBreakthroughModel, string metricsOptions)
		{
			AddEventType("EquipmentBreakthrough");
			AddProperty("Breakthrough_Equipment_Id", metricsOptions);
			return this;
		}

		public Metrics AddRestart()
		{
			AddEventType("Restart");
			return this;
		}

		public Metrics DoubleRewards()
		{
			AddEventType("DoubleRewards");
			return this;
		}

		public Metrics DoubleRewards(bool partial)
		{
			AddEventType("DoubleRewards");
			AddProperty("PartiallyDoubleRewards", partial);
			return this;
		}

		public Metrics AddRadioCall()
		{
			AddEventType("RadioCall");
			PhoneCallModel phoneCall = manager.Player.PhoneCall;
			if (phoneCall != null)
			{
				AddAndResetOneTdPropertyType("RadioCall");
				AddProperty("Survivor_Found_Situation_Id", (phoneCall.IdForAnalytics != null) ? phoneCall.IdForAnalytics : "0");
				if (phoneCall.CallType == PhoneCallDefinitionType.GuaranteedHero)
				{
					AddProperty("Call_Type", "Platinum");
					AddTdProperty("RadioCall", "Call_Type", "Platinum");
				}
				else
				{
					DropType[] array = new DropType[3]
					{
						DropType.Regular,
						DropType.Silver,
						DropType.Gold
					};
					AddProperty("Call_Type", (phoneCall.CurrentSlotNumber < 3) ? array[phoneCall.CurrentSlotNumber].ToString() : phoneCall.CurrentCallDraDropType.ToString());
					AddTdProperty("RadioCall", "Call_Type", (phoneCall.CurrentSlotNumber < 3) ? array[phoneCall.CurrentSlotNumber].ToString() : phoneCall.CurrentCallDraDropType.ToString());
				}
				AddProperty("Call_Drop_Type", phoneCall.CurrentCallDraDropType.ToString());
				AddProperty("Call_Slot", phoneCall.CurrentSlotNumber.ToString());
				AddTdProperty("RadioCall", "Call_Drop_Type", phoneCall.CurrentCallDraDropType.ToString());
				AddTdProperty("RadioCall", "Call_Slot", phoneCall.CurrentSlotNumber.ToString());
				if (phoneCall.CurrentCashier != null)
				{
					AddProperty("Method", (phoneCall.CurrentCashier.GetTotalCost(CurrencyType.Phone) == 0) ? "Free" : "Paid");
					AddTdProperty("RadioCall", "Method", (phoneCall.CurrentCashier.GetTotalCost(CurrencyType.Phone) == 0) ? "Free" : "Paid");
				}
				PhoneCallDefinition phoneCallDefinition = phoneCall.GetPhoneCallDefinition(phoneCall.CurrentSlotNumber);
				if (phoneCallDefinition != null)
				{
					AddTdProperty("RadioCall", "phone_used", phoneCallDefinition.Price.ToString());
					AddTdProperty("RadioCall", "choose_num", Convert.ToString((phoneCallDefinition.Rerolls == 0) ? 1 : 3));
				}
			}
			if (manager != null && manager.CampModel != null)
			{
				AddProperty("Radio_Tent_Level", manager.CampModel.GetBuildingLevel("RadioTent").ToString());
				AddTdProperty("RadioCall", "Radio_Tent_Level", manager.CampModel.GetBuildingLevel("RadioTent").ToString());
			}
			return this;
		}

		public Metrics AddAchivement(AchievementDefinition achievement)
		{
			AddEventType("Achievement");
			if (achievement != null)
			{
				AddProperty("Achievement_Id", achievement.ID);
			}
			return this;
		}

		public Metrics AddDailyQuest(AchievementDefinition achievementDefinition)
		{
			AddEventType("DailyQuest");
			if (achievementDefinition != null)
			{
				AddProperty("Daily_Quest_Id", achievementDefinition.ID);
			}
			return this;
		}

		public Metrics AddAccept()
		{
			AddEventType("Accept");
			AddProperty("Acceptance", "1");
			AddAndResetOneTdPropertyType("RadioCall_Acceptance");
			AddTdProperty("RadioCall_Acceptance", "Acceptance", "1");
			return this;
		}

		public Metrics AddReject()
		{
			AddEventType("Reject");
			AddProperty("Acceptance", "2");
			return this;
		}

		public Metrics AddIgnore()
		{
			AddEventType("Ignore");
			return this;
		}

		public Metrics AddChallenge()
		{
			AddEventType("Challenge");
			AddAndResetOneTdPropertyType("Challenge");
			WeeklyChallenge currentDefinition = manager.Player.WeeklyChallenge.CurrentDefinition;
			if (currentDefinition != null)
			{
				int detailMapId = currentDefinition.DetailMapId;
				AddProperty("Challenge_Number", currentDefinition.Identifier.ToString());
				AddProperty("Challenge_Name", (manager.Player.WeeklyChallenge.GetMissionSpawnPointGroup() != null) ? manager.Player.WeeklyChallenge.GetMissionSpawnPointGroup().DisplayName : "");
				AddProperty("Challenge_Id", detailMapId.ToString());
				AddProperty("Challenge_Round", manager.Player.WeeklyChallenge.CurrentCycle + 1);
				AddTdProperty("Challenge", "Challenge_Number", currentDefinition.Identifier);
				AddTdProperty("Challenge", "Challenge_Name", (manager.Player.WeeklyChallenge.GetMissionSpawnPointGroup() != null) ? manager.Player.WeeklyChallenge.GetMissionSpawnPointGroup().DisplayName : "");
				AddTdProperty("Challenge", "Challenge_Id", detailMapId);
				AddTdProperty("Challenge", "Challenge_Star", manager.Player.WeeklyChallenge.NumberStars);
				AddTdProperty("Challenge", "Challenge_Round", manager.Player.WeeklyChallenge.CurrentCycle + 1);
			}
			GuildModel guildModel = manager.Player.GuildModel;
			if (guildModel != null)
			{
				AddProperty("Cum_Challenge_Stars_Guild", guildModel.CurrentChallengeStars.ToString());
				AddProperty("Guild_ID", guildModel.Id.ToString());
			}
			AddProperty("Cum_challenge_Stars_Player", manager.Player.WeeklyChallenge.NumberStars.ToString());
			AddProperty("Challenge_Round_Number", manager.Player.WeeklyChallenge.CurrentCycle.ToString());
			AddProperty("Challenge_Round_Difficulty_Level", manager.Player.WeeklyChallenge.CurrentRequiredSurvivorLevel.ToString());
			AddProperty("PTS_Start", manager.Player.WeeklyChallenge.PTSAtChallengeStart.ToString());
			AddProperty("PTS_Current", manager.Player.WeeklyChallenge.CurrentPotentialTeamStrength.ToString());
			AddProperty("Star_Hero_Bonus", manager.Player.MapContainerModel.AttackTargetMissionModel?.Stars?.FeaturedHeroExtraChallengeStar == true);
			return this;
		}

		public Metrics AddApocalyChallenge()
		{
			AddEventType("ApocalypticChallenge");
			AddAndResetOneTdPropertyType("ApocalypticChallenge");
			MapMissionModel attackTargetMissionModel = manager.Player.MapContainerModel.AttackTargetMissionModel;
			WeeklyChallenge currentDefinition = manager.Player.ApocalypseWeeklyChallenge.CurrentDefinition;
			if (currentDefinition != null)
			{
				_ = currentDefinition.DetailMapId;
				AddProperty("ApocalypticChallenge_Number", currentDefinition.Identifier);
				AddProperty("ApocalypticChallenge_Name", (manager.Player.ApocalypseWeeklyChallenge.GetMissionSpawnPointGroup() != null) ? manager.Player.ApocalypseWeeklyChallenge.GetMissionSpawnPointGroup().DisplayName : "");
				AddProperty("ApocalypticChallenge_Id", currentDefinition.ApocalypticMapId.ToString());
				AddProperty("ApocalypticChallenge_Round", manager.Player.WeeklyChallenge.CurrentCycle + 1);
				AddProperty("ApocalypticChallenge_Star", manager.Player.ApocalypseWeeklyChallenge.NumberStars);
				AddTdProperty("ApocalypticChallenge", "ApocalypticChallenge_Number", currentDefinition.Identifier.ToString());
				AddTdProperty("ApocalypticChallenge", "ApocalypticChallenge_Name", (manager.Player.ApocalypseWeeklyChallenge.GetMissionSpawnPointGroup() != null) ? manager.Player.ApocalypseWeeklyChallenge.GetMissionSpawnPointGroup().DisplayName : "");
				AddTdProperty("ApocalypticChallenge", "ApocalypticChallenge_Id", currentDefinition.ApocalypticMapId.ToString());
				AddTdProperty("ApocalypticChallenge", "ApocalypticChallenge_Round", manager.Player.ApocalypseWeeklyChallenge.CurrentCycle + 1);
				AddTdProperty("ApocalypticChallenge", "ApocalypticChallenge_Star", manager.Player.ApocalypseWeeklyChallenge.NumberStars);
				AddTdProperty("ApocalypticChallenge", "ApocalypticChallenge_MasterMission", attackTargetMissionModel?.IsMasterMission);
			}
			GuildModel guildModel = manager.Player.GuildModel;
			if (guildModel != null)
			{
				AddProperty("Cum_Apocalyptic_Stars_Guild", guildModel.CurrentChallengeStars.ToString());
				AddProperty("Guild_ID", guildModel.Id.ToString());
			}
			AddProperty("Cum_challenge_Stars_Player", manager.Player.ApocalypseWeeklyChallenge.NumberStars.ToString());
			AddProperty("Apocalyptic_Round_Number", manager.Player.ApocalypseWeeklyChallenge.CurrentCycle.ToString());
			AddProperty("Apocalyptic_Round_Difficulty_Level", manager.Player.WeeklyChallenge.CurrentRequiredSurvivorLevel.ToString());
			AddProperty("PTS_Start", manager.Player.WeeklyChallenge.PTSAtChallengeStart.ToString());
			AddProperty("PTS_Current", manager.Player.WeeklyChallenge.CurrentPotentialTeamStrength.ToString());
			AddProperty("Star_Hero_Bonus", manager.Player.MapContainerModel.AttackTargetMissionModel?.Stars?.FeaturedHeroExtraChallengeStar == true);
			return this;
		}

		public Metrics AddChallengeReward(LootEntry lootEntry)
		{
			if (lootEntry.Type == LootEntryType.ChallengeGuildReward)
			{
				return AddGuild(manager.Player.GuildModel);
			}
			return AddPersonal();
		}

		public Metrics AddDistance()
		{
			AddEventType("Distance");
			AddAndResetOneTdPropertyType("Distance");
			WeeklySurvivalModel weeklySurvival = manager.Player.WeeklySurvival;
			WeeklySurvival currentDefinition = weeklySurvival.CurrentDefinition;
			if (currentDefinition != null)
			{
				AddProperty("Distance_Number", currentDefinition.Identifier.ToString());
				AddProperty("Distance_Name", (weeklySurvival.GetMissionSpawnPointGroup() != null) ? weeklySurvival.GetMissionSpawnPointGroup().DisplayName : "");
				AddProperty("Distance_Iteration", weeklySurvival.CurrentMapRestarts.ToString());
				AddProperty("Distance_Difficulty", weeklySurvival.CurrentDifficulty.ToString());
				AddTdProperty("Distance", "Distance_Number", currentDefinition.Identifier);
				AddTdProperty("Distance", "Distance_Difficulty", weeklySurvival.CurrentDifficulty.ToString());
			}
			return this;
		}

		public Metrics AddSurvivalReward(LootEntry lootEntry)
		{
			return AddPersonal();
		}

		public Metrics AddSurvivalRoundReward()
		{
			AddEventType("SurvivalRoundReward");
			return this;
		}

		public Metrics AddPersonal()
		{
			AddEventType("Personal");
			AddProperty("Source", "Personal");
			return this;
		}

		public Metrics AddGuild(GuildModel guild)
		{
			AddEventType("Guild");
			if (guild != null)
			{
				AddProperty("Guild_Id", guild.Id);
				AddProperty("Guild_Name", guild.Name);
				AddProperty("Guild_Member_Count", guild.NumberMembers.ToString());
				AddProperty("Guild_Pending_Requests_Count", guild.NumberPendingRequests.ToString());
				AddProperty("Guild_Leader_Country", (guild.CountryCode != null) ? guild.CountryCode : "");
				AddProperty("Guild_Challenge_Starts", guild.NumberChallengeStarted.ToString());
				AddProperty("Guild_Challenge_Stars", guild.CurrentChallengeStars.ToString());
				AddProperty("Guild_Member_Delta", "0");
				AddProperty("Guild_Join_Type", guild.JoinType.ToString());
				if (guild.Purpose != null)
				{
					AddProperty("Guild_Purpose", guild.Purpose.ToString());
				}
				else
				{
					AddProperty("Guild_Purpose", "");
				}
				AddProperty("Guild_Average_Member_Level", guild.AverageMemberLevel.ToString());
				AddProperty("Guild_Highest_Member_Level", guild.HighestMemberLevel.ToString());
				AddProperty("Guild_Lowest_Member_Level", guild.LowestMemberLevel.ToString());
				AddProperty("Guild_Advertise_Remaining_Seconds", guild.AdAvailableTimeSeconds.ToString());
			}
			AddProperty("Source", "Guild");
			return this;
		}

		public Metrics AddPlayer(PlayerModel player)
		{
			AddEventType("Player");
			if (player != null)
			{
				AddProperty("Player_Id", player.HashedId);
				AddProperty("Player_Name", (player.Name != null) ? player.Name : "");
				AddProperty("Player_Guild_Id", (!string.IsNullOrEmpty(player.GuildId)) ? player.GuildId : "0");
				AddProperty("Player_Level", player.Level.ToString());
				AddProperty("Player_LifeTime", player.LifeTime.ToString());
				AddProperty("Player_OutpostLevel", player.OutpostLevel.ToString());
				AddProperty("Player_TotalUSDSpent", player.TotalUSDSpent.ToString());
			}
			return this;
		}

		public Metrics AddMember(GuildMemberInfo guildMember)
		{
			return AddGuildMember(guildMember, "Member");
		}

		public Metrics AddModerator(GuildMemberInfo guildMember)
		{
			return AddGuildMember(guildMember, "Moderator");
		}

		public Metrics AddGuildMember(GuildMemberInfo guildMember, string guildMemberPropertyPrefix)
		{
			AddEventType(guildMemberPropertyPrefix);
			if (guildMember != null)
			{
				AddProperty(guildMemberPropertyPrefix + "_Hashed_Id", guildMember.MemberId);
				AddProperty(guildMemberPropertyPrefix + "_Role", guildMember.Role.ToString());
				AddProperty(guildMemberPropertyPrefix + "_Current_Challenge_Stars", guildMember.CurrentChallengeStars.ToString());
				AddProperty(guildMemberPropertyPrefix + "_State", guildMember.State.ToString());
				AddProperty(guildMemberPropertyPrefix + "_Player_Level", guildMember.PlayerLevel.ToString());
				int num = (int)((manager.Player.UtcTimeStamp - guildMember.LastActiveDate) / 86400000);
				int num2 = (int)((guildMember.GuildJoinedDate > 0) ? ((manager.Player.UtcTimeStamp - guildMember.GuildJoinedDate) / 86400000) : (-1));
				AddProperty(guildMemberPropertyPrefix + "_Days_In_Guild", num2.ToString());
				AddProperty(guildMemberPropertyPrefix + "_Total_Inactive_Days", num.ToString());
			}
			return this;
		}

		public Metrics AddCreateGuild()
		{
			AddEventType("CreateGuild");
			return this;
		}

		public Metrics AddKick()
		{
			AddEventType("Kick");
			return this;
		}

		public Metrics AddKick(long banMinutes)
		{
			AddKick();
			AddProperty("Ban_Duration", banMinutes);
			return this;
		}

		public Metrics AddJoinRequest(string searchId, int searchPosition)
		{
			AddEventType("JoinRequest");
			AddProperty("Search_Id", searchId);
			AddProperty("Search_Position", searchPosition.ToString());
			return this;
		}

		public Metrics AddJoinConfirmation()
		{
			AddEventType("JoinConfirmation");
			return this;
		}

		public Metrics AddJoinAcceptance()
		{
			AddEventType("JoinAcceptance");
			return this;
		}

		public Metrics AddJoinRefusal()
		{
			AddEventType("JoinRefusal");
			return this;
		}

		public Metrics AddLeaves()
		{
			AddEventType("Leaves");
			return this;
		}

		public Metrics AddSend()
		{
			AddEventType("Send");
			return this;
		}

		public Metrics AddSendMessage(string message)
		{
			AddEventType("SendMessage");
			if (message != null)
			{
				AddProperty("Message_Length", message.Length.ToString());
			}
			return this;
		}

		public Metrics AddTradeCrate(TradeSlotInfo tradeSlotDefinition)
		{
			AddEventType("TradeCrate");
			if (tradeSlotDefinition != null)
			{
				properties["Trade_Crate_Slot_Id"] = tradeSlotDefinition.SlotDefinition.SlotId.ToString();
				properties["Trade_Crate_Item_Id"] = tradeSlotDefinition.CurrentTradeDefinition.UniqueId.ToString();
				properties["Trade_Crate_Loot"] = tradeSlotDefinition.CurrentTradeDefinition.SoldItem;
				properties["Trade_Crate_Buckets"] = tradeSlotDefinition.SlotDefinition.Bucket;
				properties["Trade_Crate_Bucket_Id"] = tradeSlotDefinition.CurrentTradeDefinition.BucketId;
				properties["Trade_Crate_Price_Category"] = tradeSlotDefinition.SlotDefinition.PriceCategory.ToString();
				properties["Trade_Crate_Tag_Name"] = tradeSlotDefinition.CurrentTradeDefinition.TagName;
				properties["Trade_Crate_Tag_Amount"] = tradeSlotDefinition.CurrentTradeDefinition.TagAmount.ToString();
			}
			return this;
		}

		public Metrics AddLoot(LootEntry loot)
		{
			if (loot != null)
			{
				AddAndResetOneTdPropertyType("ChallengeReward_challenge_type");
				if (loot.RewardedEquipment != null)
				{
					AddTdProperty("ChallengeReward_challenge_type", "challenge_type", loot.Type.ToString());
					return AddEquipment(loot.RewardedEquipment);
				}
				if (loot.RewardedCurrency != CurrencyType.None)
				{
					AddTdProperty("ChallengeReward_challenge_type", "challenge_type", loot.Type.ToString());
					return AddResources(loot.RewardedCurrency, loot.RewardedAmount, loot.ActualAmountAdded);
				}
			}
			return this;
		}

		public Metrics AddNonLootReward(IReward reward)
		{
			if (reward is RewardEquipment rewardEquipment)
			{
				return AddEquipment(rewardEquipment.GivenEquipment);
			}
			if (reward is RewardRandomEquipment rewardRandomEquipment)
			{
				return AddEquipment(rewardRandomEquipment.GivenEquipment);
			}
			if (reward is RewardCurrency rewardCurrency)
			{
				return AddResources(rewardCurrency.CurrencyType, rewardCurrency.Amount, rewardCurrency.AmountActuallyAdded);
			}
			if (reward is RewardOutfit rewardOutfit)
			{
				foreach (string item in rewardOutfit.PreferredOrder)
				{
					AddOutfit(item);
				}
			}
			return this;
		}

		public Metrics AddLootCrate(LootEntry loot)
		{
			AddEventType("LootCrate");
			if (loot != null)
			{
				AddProperty("Loot_Crate_Type", loot.DropType.ToString());
				if (loot.DropEventDefinition != null)
				{
					AddProperty("Loot_Crate_Event_Type", loot.DropEventDefinition.EventType.ToString());
					AddProperty("Loot_Crate_Tag", loot.DropEventDefinition.Tag.ToString());
					AddProperty("Loot_Crate_DropContext", loot.DropEventDefinition.DropContext.ToString());
				}
			}
			return this;
		}

		public Metrics AddBonusStars(int bonusStars)
		{
			AddEventType("BonusStars");
			AddProperty("Challenge_Star_Delta", bonusStars.ToString());
			return this;
		}

		public Metrics AddApocalypseBonusStars(int bonusStars)
		{
			AddEventType("ApocalypseBonusStars");
			AddProperty("ApocalypseChallenge_Star_Delta", bonusStars.ToString());
			return this;
		}

		public Metrics AddChallengeRoundReward()
		{
			AddEventType("ChallengeRoundReward");
			if (manager.Player.WeeklyChallenge != null)
			{
				AddProperty("SkipDoubleRewardsActive", (manager.Player.WeeklyChallenge.ActiveSkipTokens > 0) ? "1" : "0");
			}
			return this;
		}

		public Metrics AddApocalypseChallengeRoundReward()
		{
			AddEventType("ApocalypseChallengeRoundReward");
			if (manager.Player.WeeklyChallenge != null)
			{
				AddProperty("SkipDoubleRewardsActive", (manager.Player.ApocalypseWeeklyChallenge.ActiveSkipTokens > 0) ? "1" : "0");
			}
			return this;
		}

		public Metrics AddOutfit(OutfitDefinition outfitDefinition)
		{
			return AddOutfit(outfitDefinition?.ID);
		}

		public Metrics AddOutfit(string id)
		{
			AddEventType("Outfit");
			if (!string.IsNullOrEmpty(id))
			{
				AddProperty("Outfit_Name", id);
			}
			return this;
		}

		public Metrics AddGuildGift()
		{
			AddEventType("GuildGift");
			GuildModel guildModel = manager.Player.GuildModel;
			if (guildModel != null)
			{
				AddProperty("Guild_ID", guildModel.Id);
			}
			AddProperty("Guild_Gift_Sender_Id", manager.Player.HashedId);
			return this;
		}

		public Metrics AddGuildAd(string adUniqueId)
		{
			AddEventType("GuildAd");
			AddProperty("Guild_Ad_Id", adUniqueId);
			return this;
		}

		public Metrics AddWalkerTap(int walkerTapNumber)
		{
			AddEventType("WalkerTap");
			AddProperty("Player_Level", manager.Player.Level.ToString());
			AddProperty("Tap_Number", walkerTapNumber.ToString());
			return this;
		}

		public Metrics AddExtraUnlock()
		{
			AddEventType("ExtraUnlock");
			AddProperty("Extra_Unlock_Name", "1");
			return this;
		}

		public Metrics AddHeroUnlock()
		{
			AddEventType("HeroUnlock");
			return this;
		}

		public Metrics AddEquipmentRemodel()
		{
			AddEventType("EquipmentRemodel");
			return this;
		}

		public Metrics AddSurvivorSlot()
		{
			AddEventType("SurvivorSlot");
			AddProperty("Survivor_Slot_Name", "survivor_slot");
			AddProperty("Slot_Level", manager.Player.SurvivorContainer.SurvivorSlotsUpgradeLevel.ToString());
			return this;
		}

		public Metrics AddSurvivalManualActorlevel(SurvivalManualActorLevel survivalManualActorLevel)
		{
			AddEventType("SurvivalManual_Upgrade_Actor_level");
			if (survivalManualActorLevel != null)
			{
				AddProperty("type", survivalManualActorLevel.Type.ToString());
				AddProperty("Actor_Level", survivalManualActorLevel.Level.ToString());
				AddProperty("Actor_to_Level", (survivalManualActorLevel.Level + 1).ToString());
				AddProperty("CostToken", survivalManualActorLevel.CostToken.ToString());
			}
			return this;
		}

		public Metrics AddSurvivalManualSkillLevel(SurvivalManualSkill survivalManualSkill)
		{
			AddEventType("SurvivalManual_Upgrade_Skill_level");
			if (survivalManualSkill != null)
			{
				AddProperty("type", survivalManualSkill.Type.ToString());
				AddProperty("skill_Level", survivalManualSkill.Level.ToString());
				AddProperty("skill_to_Level", (survivalManualSkill.Level + 1).ToString());
				AddProperty("CostToken", survivalManualSkill.UpgradeCost.ToString());
			}
			return this;
		}

		public Metrics AddSurvivalManualStorySkillLevel(SurvivalManualStorySkill survivalManualStorySkill)
		{
			AddEventType("SurvivalManual_Upgrade_Skill_level");
			if (survivalManualStorySkill != null)
			{
				AddProperty("type", survivalManualStorySkill.Type.ToString());
				AddProperty("skill_Level", survivalManualStorySkill.Level.ToString());
				AddProperty("skill_to_Level", (survivalManualStorySkill.Level + 1).ToString());
				AddProperty("CostToken", survivalManualStorySkill.UpgradeCost.ToString());
			}
			return this;
		}

		public Metrics AddBuy()
		{
			AddEventType("Buy");
			return this;
		}

		public Metrics AddUpgrade(UpgradeTypes upgradeType)
		{
			AddEventType("Upgrade");
			AddProperty("Upgrade_Type", upgradeType.ToString());
			return this;
		}

		public Metrics AddUpgrade()
		{
			AddEventType("Upgrade");
			return this;
		}

		public Metrics AddCancelUpgrade(BuildingModel buildingModel)
		{
			AddEventType("CancelUpgrade");
			AddProperty("Upgrade_Time_Passed", ((buildingModel.OriginalUpgradeTimer - buildingModel.UpgradeTimer) / 1000 / 60).ToString());
			AddProperty("Upgrade_Time_Left", (buildingModel.UpgradeTimer / 1000 / 60).ToString());
			return this;
		}

		public Metrics AddEpisode(MapMissionGroupModel model)
		{
			AddEventType("Episode");
			AddProperty("Episode_Name", model.MissionSpawnPointGroup.DisplayName);
			AddProperty("Episode_Difficulty", model.MissionSpawnPointGroup.EpisodeDifficultyLevel.ToString());
			return this;
		}

		public Metrics AddIAP()
		{
			AddEventType("IAP");
			return this;
		}

		public Metrics AddOupostWalker(OutpostWalkerModel outpostWalkerModel)
		{
			AddEventType("OutpostWalker");
			if (outpostWalkerModel != null)
			{
				if (outpostWalkerModel.ActorDefinition != null)
				{
					AddProperty("Outpost_Walker_Class", outpostWalkerModel.ActorDefinition.Class.ToString());
				}
				AddProperty("Outpost_Walker_Level", outpostWalkerModel.Level.ToString());
				AddProperty("Outpost_Walker_Amount", outpostWalkerModel.Amount.ToString());
			}
			return this;
		}

		public Metrics AddLevel()
		{
			AddEventType("Level");
			AddProperty("Upgrade_Subject", "Level");
			return this;
		}

		public Metrics AddTraitReroll(SurvivorModel survivor)
		{
			AddEventType("TraitReroll");
			AddProperty("Trait_To_Be_Rerolled", survivor.TraitToBeRerolledCandidate);
			for (int i = 0; i < survivor.RandomTraitsFromReroll.Count; i++)
			{
				AddProperty("Random_Trait_" + (i + 1), survivor.RandomTraitsFromReroll[i]);
			}
			return this;
		}

		public Metrics AddTraitRerollOutcome(string chosenTrait)
		{
			AddEventType("TraitRerollOutCome");
			AddProperty("Trait_Chosen", chosenTrait);
			return this;
		}

		public Metrics AddTraitRerollTokenRefund()
		{
			AddEventType("TraitRerollTokenRefund");
			return this;
		}

		public Metrics AddRarity()
		{
			AddEventType("Rarity");
			AddProperty("Upgrade_Subject", "Rarity");
			return this;
		}

		public Metrics AddAmount()
		{
			AddEventType("Amount");
			AddProperty("Upgrade_Subject", "Amount");
			return this;
		}

		public Metrics AddTraitLevel()
		{
			AddEventType("TraitLevel");
			AddProperty("Upgrade_Subject", "TraitLevel");
			return this;
		}

		public Metrics AddPvp()
		{
			AddEventType("Pvp");
			AddAndResetOneTdPropertyType("PvP");
			CombatModel combat = manager.Player.Combat;
			if (combat == null)
			{
				return this;
			}
			OutpostCombat outpostCombat = combat.OutpostCombat;
			if (outpostCombat != null)
			{
				AddProperty("Pvp_Match_Id", outpostCombat.IdForAnalytics);
				AddTdProperty("PvP", "PvP_Match_Id", outpostCombat.IdForAnalytics);
				int attackerInfluenceGain = outpostCombat.AttackerInfluenceGain;
				int tradeGoodsGain = outpostCombat.TradeGoodsGain;
				AddProperty("Potential_Influence_Gain", attackerInfluenceGain.ToString());
				AddProperty("Potential_TradeGoods_Gain", Math.Max(tradeGoodsGain, 0).ToString());
				AddProperty("Defender_Is_Fake", outpostCombat.IsFake ? "1" : "0");
				AddProperty("Defender_Influence_State", outpostCombat.DefenderInitialRankingScore.ToString());
				AddProperty("Attacker_Influence_State", manager.Player.RankingScore.ToString());
				AddProperty("Cycle_Id", manager.Player.CurrentOutpostSeasonId.ToString());
			}
			return this;
		}

		public Metrics AddPvpCycle()
		{
			AddEventType("PvpCycle");
			if (manager == null || manager.Player == null)
			{
				return this;
			}
			PlayerModel player = manager.Player;
			AddProperty("Cycle_Tier", player.CurrentOutpostTier.Id);
			AddProperty("Cycle_Tier_Set", player.CurrentOutpostTier.TierSetId.ToString());
			AddProperty("Cycle_Season_Id", player.CurrentOutpostSeasonId.ToString());
			AddProperty("Influence_Delta", (player.RankingScore - player.PreviousSeasonRankingScore).ToString());
			return this;
		}

		public Metrics AddPvpAttacker()
		{
			AddEventType("PvpAttacker");
			if (manager == null)
			{
				return this;
			}
			return AddPvpPlayer(manager.Player, isAttacker: true);
		}

		public Metrics AddPvpDefender(PlayerModel defender)
		{
			AddEventType("PvpDefender");
			return AddPvpPlayer(defender, isAttacker: false);
		}

		private Metrics AddPvpPlayer(PlayerModel playerModel, bool isAttacker)
		{
			if (playerModel == null || manager == null)
			{
				return this;
			}
			string text = (isAttacker ? "Attacker" : "Defender");
			AddProperty(text + "_Hashed_Id", playerModel.HashedId);
			AddProperty(text + "_Outpost_Level", playerModel.OutpostLevel.ToString());
			if (playerModel.CurrentOutpostTier != null)
			{
				AddProperty(text + "_Tier", playerModel.CurrentOutpostTier.Id);
				AddProperty(text + "_Tier_Set", playerModel.CurrentOutpostTier.TierSetId.ToString());
			}
			if (playerModel.SurvivorContainer != null)
			{
				AddTeam(playerModel.SurvivorContainer.CombatSurvivors, text);
			}
			if (!isAttacker)
			{
				AddProperty(text + "_Shield_Remaining", playerModel.GetShieldTimeMillisLeft(manager.Player.UtcTimeStamp).ToString());
				if (playerModel.OutpostModel != null && playerModel.OutpostModel.StoredLevelModel != null)
				{
					AddProperty(text + "_Outpost_Background", playerModel.OutpostModel.StoredLevelModel.BaseRunLocationID);
					AddProperty(text + "_Outpost_Background_Slice_0", playerModel.OutpostModel.StoredLevelModel.GetChosenSliceViewId(SlicePosition.First));
					AddProperty(text + "_Outpost_Background_Slice_1", playerModel.OutpostModel.StoredLevelModel.GetChosenSliceViewId(SlicePosition.Second));
					AddProperty(text + "_Outpost_Background_Slice_2", playerModel.OutpostModel.StoredLevelModel.GetChosenSliceViewId(SlicePosition.Third));
				}
				if (playerModel.OutpostModel != null)
				{
					AddOutpostWalkers(playerModel.OutpostModel.WalkerModels);
				}
			}
			return this;
		}

		public Metrics AddPvpDefender(OutpostCombat outpostCombat)
		{
			AddEventType("PvpDefender");
			if (outpostCombat == null)
			{
				return this;
			}
			AddProperty("Defender_Hashed_Id", outpostCombat.DefenderHashedId);
			AddProperty("Defender_Outpost_Level", outpostCombat.DefenderOutpostLevel.ToString());
			AddTeam(outpostCombat.DefendingSurvivors, "Defender");
			AddOutpostWalkers(outpostCombat.DefendingWalkers);
			return this;
		}

		public Metrics AddPvpDefender(MatchInfo matchInfo)
		{
			AddEventType("PvpDefender");
			if (matchInfo == null)
			{
				return this;
			}
			AddProperty("Defender_Outpost_Level", matchInfo.DefendingOutpostLevel.ToString());
			AddTeam(matchInfo.DefendingSurvivorClasses, matchInfo.DefendingSurvivorLevels, matchInfo.DefendingSurvivorRarityLevels, "Defender");
			return this;
		}

		private void AddOutpostWalkers(IList<OutpostWalkerModel> walkerModels)
		{
			if (walkerModels == null)
			{
				return;
			}
			for (int i = 0; i < walkerModels.Count; i++)
			{
				OutpostWalkerModel outpostWalkerModel = walkerModels[i];
				if (outpostWalkerModel.Level > 0 && outpostWalkerModel.Amount > 0)
				{
					AddProperty("Defender_Walker_" + outpostWalkerModel.Id.Replace("Walker", "") + "_Level", outpostWalkerModel.Level.ToString());
					AddProperty("Defender_Walker_" + outpostWalkerModel.Id.Replace("Walker", "") + "_Count", outpostWalkerModel.Amount.ToString());
				}
			}
		}

		public Metrics AddGvGAttacker()
		{
			AddEventType("GvGAttacker");
			AddTeam(manager.Player.SurvivorContainer.CombatSurvivors, "Attacker");
			return this;
		}

		public Metrics GvgDefendersUpdated(bool auto = false)
		{
			AddEventType("GvGDefenders");
			AddProperty("Auto", auto);
			for (int i = 0; i < manager.Player.GvGDefenders.Count; i++)
			{
				AddProperty("DefenderName" + i, manager.Player.GvGDefenders[i].Name);
				AddProperty("DefenderAnalyticsId" + i, manager.Player.GvGDefenders[i].AnalyticsId);
			}
			return this;
		}

		public Metrics AddGvGDefender(GuildBattleParticipantInfo playerInfo, GuildBattlePvpTeam playerTeam, int survivorLevel)
		{
			AddEventType("GvGDefender");
			string text = "Defender";
			AddProperty(text + "_Hashed_Id", playerInfo.HashedPlayerId);
			List<SurvivorMockData> survivors = playerTeam.Survivors;
			int count = survivors.Count;
			int[] array = new int[6];
			Array.Clear(array, 0, array.Length);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			SurvivorMockData survivorMockData = null;
			foreach (SurvivorMockData item in survivors)
			{
				array[(int)item.SurvivorClass]++;
				if (item.IsHero)
				{
					num++;
				}
				else
				{
					num2++;
				}
				num3 += survivorLevel;
				num4 += item.RarityLevel;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0)
				{
					SurvivorClass survivorClass = (SurvivorClass)i;
					AddProperty(text + "_" + survivorClass.ToString() + "_Amount", array[i].ToString());
				}
			}
			AddProperty(text + "_Hero_Amount", num.ToString());
			AddProperty(text + "_Regular_Amount", num2.ToString());
			if (count != 0)
			{
				AddProperty(text + "_Avg_Survivor_Level", (num3 / count).ToString());
				AddProperty(text + "_Avg_Survivor_Rarity", (num4 / count).ToString());
				survivorMockData = playerInfo.SelectedSurvivors[0];
			}
			if (survivorMockData != null && playerInfo.SelectedSurvivors[0].IsHero)
			{
				AddProperty(text + "_Active_Leader_Trait", survivorMockData.GetLeaderTraitId());
			}
			return this;
		}

		public Metrics AddStart()
		{
			AddEventType("Start");
			return this;
		}

		public Metrics AddEnd()
		{
			AddEventType("End");
			return this;
		}

		public Metrics AddEdit()
		{
			AddEventType("Edit");
			return this;
		}

		public Metrics AddSwitch()
		{
			AddEventType("Switch");
			return this;
		}

		public Metrics AddScrap()
		{
			AddEventType("Scrap");
			return this;
		}

		public Metrics AddRemove()
		{
			AddEventType("Remove");
			return this;
		}

		public Metrics AddSpend()
		{
			AddEventType("Spend");
			return this;
		}

		public Metrics AddFind()
		{
			AddEventType("Find");
			return this;
		}

		public Metrics AddItemChange()
		{
			AddEventType("item_change");
			return this;
		}

		public Metrics AddSearch()
		{
			AddEventType("Search");
			return this;
		}

		public Metrics AddSkip()
		{
			AddEventType("Skip");
			return this;
		}

		public Metrics AddReceive()
		{
			AddEventType("Receive");
			return this;
		}

		public Metrics AddRewarded()
		{
			AddEventType("Rewarded");
			return this;
		}

		public Metrics AddCinema()
		{
			AddEventType("Cinema");
			return this;
		}

		public Metrics AddView()
		{
			AddEventType("View");
			return this;
		}

		public Metrics AddVideoAd(AdProvider provider, AdStatus status)
		{
			AddEventType("VideoAd");
			AddProperty("Provider", provider.ToString());
			AddProperty("Status", status.ToString());
			return this;
		}

		public Metrics AddFill(bool available)
		{
			AddEventType("Fill");
			AddProperty("Video_Availability", available.ToString());
			return this;
		}

		public Metrics AddSeasonVideo(string EpisodeId)
		{
			AddEventType("SeasonVideo");
			AddProperty("Episode_Name", EpisodeId);
			return this;
		}

		public Metrics AddSeasonVideo(string episodeId, string location)
		{
			AddEventType("SeasonVideo");
			AddProperty("Episode_Name", episodeId);
			AddProperty("Season_Video_Location", location);
			return this;
		}

		public Metrics AddStaticReward()
		{
			AddEventType("StaticReward");
			return this;
		}

		public Metrics AddTradeCrate()
		{
			AddEventType("TradeCrate");
			return this;
		}

		public Metrics AddDebug()
		{
			AddEventType("Debug");
			return this;
		}

		public Metrics AddSupport()
		{
			AddEventType("Support");
			return this;
		}

		public Metrics AddSupport(long timestamp, string supportEntityGUID)
		{
			AddProperty("Support_Timestamp", timestamp.ToString());
			AddProperty("Support_Action_Id", supportEntityGUID);
			return AddSupport();
		}

		public Metrics AddInflueceFixed(int score)
		{
			AddEventType("InfluenceFixed");
			AddProperty("Influence", score.ToString());
			return this;
		}

		public Metrics AddWalkersKilled()
		{
			AddEventType("WalkersKilled");
			return this;
		}

		public Metrics AddBonusIAPGift()
		{
			AddEventType("BonusIAPGift");
			return this;
		}

		public Metrics AddHeal(bool healAll)
		{
			AddEventType("Heal");
			AddProperty("Healing_Type", healAll ? "Heal All" : "Heal Individual");
			return this;
		}

		public Metrics AddTutorial()
		{
			AddEventType("Tutorial");
			if (manager.Player != null && manager.Player.Tutorial != null)
			{
				TutorialModel tutorial = manager.Player.Tutorial;
				AddProperty("Part_Id", tutorial.CurrentPartId);
				AddProperty("Step", tutorial.CurrentStep.ToString());
			}
			return this;
		}

		public Metrics AddTutorial(int currentStep)
		{
			AddEventType("Tutorial");
			if (manager.Player != null && manager.Player.Tutorial != null)
			{
				TutorialModel tutorial = manager.Player.Tutorial;
				AddProperty("Part_Id", tutorial.CurrentPartId);
				AddProperty("Step", currentStep);
			}
			return this;
		}

		public Metrics AddOutpostTutorial(OutpostTutorialStateForAnalytics analyticsState)
		{
			AddEventType("Tutorial");
			AddProperty("Part_Id", "Outpost_" + analyticsState);
			int num = (int)analyticsState;
			AddProperty("Step", num.ToString());
			return this;
		}

		public Metrics AddBundle(BundleStoreDefinition bundleStoreDefinition, BundleSource bundleSource = BundleSource.Unknown)
		{
			AddEventType("Bundle");
			if (bundleStoreDefinition != null)
			{
				AddProperty("Bundle_Id", bundleStoreDefinition.BundleIdentifier);
				AddProperty("Bundle_Source", bundleSource.ToString());
				BundleContentDefinition bundleContentDefinition = manager.GameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
				if (bundleContentDefinition != null)
				{
					string value = ((bundleContentDefinition.Category == BundleContentDefinition.CategoryOffer) ? "1" : "0");
					AddProperty("Bundle_Is_Offer", value);
					AddProperty("Bundle_Contents", bundleContentDefinition.Rewards);
					if (manager.Player != null && manager.Player.BundleManager != null)
					{
						LimitedBundleData initiatedLimitedBundle = manager.Player.BundleManager.GetInitiatedLimitedBundle(bundleContentDefinition.Identifier);
						if (initiatedLimitedBundle != null)
						{
							AddProperty("Bundle_Hours_Left", ((int)Math.Round((double)initiatedLimitedBundle.Timer / 3600000.0)).ToString());
						}
					}
					InAppPurchaseProductApple inAppPurchaseProduct = manager.GameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct);
					if (inAppPurchaseProduct != null)
					{
						AddProperty("Bundle_Price_Tier", inAppPurchaseProduct.PriceTier.ToString());
						AddProperty("Bundle_Price_Usd", inAppPurchaseProduct.PriceUSD.ToString());
						AddProperty("Bundle_Product_Id", (inAppPurchaseProduct.Id != null) ? inAppPurchaseProduct.Id : "");
					}
				}
				bool isPartOfRotation = bundleStoreDefinition.IsPartOfRotation;
				string value2 = "";
				if (isPartOfRotation)
				{
					string text = manager.Player.BundleManager.RotatingBundleManager.GetRotationPurchasedFromBundle(bundleStoreDefinition.BundleIdentifier);
					bool flag = !string.IsNullOrEmpty(text);
					int num = -1;
					if (!flag)
					{
						num = manager.Player.BundleManager.RotatingBundleManager.GetBestRotationStepThatContainsBundle(bundleStoreDefinition.BundleIdentifier);
						text = ((num > -1) ? manager.Player.BundleManager.RotatingBundleManager.CurrentRotationIdentifier : "");
					}
					if (!string.IsNullOrEmpty(text))
					{
						AddProperty("Rotation_Id", text);
						BundleRotationDefinition bundleRotationDefinition = manager.GameEconomyData.GetBundleRotationDefinition(text);
						if (bundleRotationDefinition != null)
						{
							AddProperty("Rotation_Number", bundleRotationDefinition.RotationNumber.ToString());
							if (bundleRotationDefinition.SpenderTiers != null && bundleRotationDefinition.SpenderTiers.Count > 0)
							{
								value2 = manager.GameEconomyData.GetBestSpenderTier(bundleRotationDefinition.SpenderTiers, manager.Player, manager.Player.SecondsSinceLastPurchase);
							}
							AddProperty("Rotation_Step_Number", (flag ? manager.Player.BundleManager.RotatingBundleManager.GetRotationStepPurchasedFromBundle(bundleStoreDefinition.BundleIdentifier) : num).ToString());
						}
					}
				}
				else if (bundleStoreDefinition.SpenderTiers != null && bundleStoreDefinition.SpenderTiers.Count > 0)
				{
					value2 = manager.GameEconomyData.GetBestSpenderTier(bundleStoreDefinition.SpenderTiers, manager.Player, manager.Player.SecondsSinceLastPurchase);
				}
				if (!string.IsNullOrEmpty(value2))
				{
					AddProperty("Bundle_Spender_Tier", value2);
				}
			}
			return this;
		}

		public Metrics AddCustomBundle(CustomBundleDefinition customBundleDefinition, BundleSource bundleSource = BundleSource.Unknown)
		{
			AddEventType("Bundle");
			if (customBundleDefinition != null)
			{
				AddProperty("Bundle_Id", customBundleDefinition.Identifier);
				AddProperty("Bundle_Source", bundleSource.ToString());
				AddProperty("Bundle_Is_Offer", 1);
				AddProperty("Bundle_Contents", customBundleDefinition.Rewards);
				if (manager.Player != null && manager.Player.BundleManager != null)
				{
					LimitedBundleData initiatedLimitedBundle = manager.Player.BundleManager.GetInitiatedLimitedBundle(customBundleDefinition.Identifier);
					if (initiatedLimitedBundle != null)
					{
						AddProperty("Bundle_Hours_Left", ((int)Math.Round((double)initiatedLimitedBundle.Timer / 3600000.0)).ToString());
					}
				}
				InAppPurchaseProductApple inAppPurchaseProduct = manager.GameEconomyData.GetInAppPurchaseProduct(customBundleDefinition.IAPProduct);
				if (inAppPurchaseProduct != null)
				{
					AddProperty("Bundle_Price_Tier", inAppPurchaseProduct.PriceTier.ToString());
					AddProperty("Bundle_Price_Usd", inAppPurchaseProduct.PriceUSD.ToString());
					AddProperty("Bundle_Product_Id", (inAppPurchaseProduct.Id != null) ? inAppPurchaseProduct.Id : "");
				}
			}
			return this;
		}

		public Metrics AddCustomBundle(CustomBundleDefinition customBundleDefinition)
		{
			AddEventType("CustomBundle");
			if (customBundleDefinition != null)
			{
				AddProperty("CustomBundle_Id", customBundleDefinition.Identifier);
				BundleContentDefinition bundleContentDefinition = manager.GameEconomyData.GetBundleContentDefinition(customBundleDefinition.Identifier);
				if (bundleContentDefinition != null)
				{
					InAppPurchaseProductApple inAppPurchaseProduct = manager.GameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct);
					if (inAppPurchaseProduct != null)
					{
						AddProperty("Bundle_Price_Tier", inAppPurchaseProduct.PriceTier.ToString());
						AddProperty("Bundle_Price_Usd", inAppPurchaseProduct.PriceUSD.ToString());
						AddProperty("Bundle_Product_Id", (inAppPurchaseProduct.Id != null) ? inAppPurchaseProduct.Id : "");
					}
				}
			}
			return this;
		}

		public Metrics AddSupportTalent(int treeId, int nodeId, int currentLevel)
		{
			AddEventType("SupportTalent");
			AddProperty("Tree_Id", treeId);
			AddProperty("Node_Id", nodeId);
			AddProperty("Current_Level", currentLevel);
			return this;
		}

		public Metrics AddSupportTalentNextLevel(int nextLevel)
		{
			AddEventType("SupportTalentNextLevel");
			AddProperty("Next_Level", nextLevel);
			return this;
		}

		public Metrics AddSupportAssembleTrait(int traitSlot, int traitTalentId)
		{
			AddEventType("SupportAssembleTrait");
			AddProperty("Trait_Slot", traitSlot);
			AddProperty("Trait_Talent_Id", traitTalentId);
			return this;
		}

		public Metrics AddPlayerHub(string activity)
		{
			AddEventType("PlayerHub");
			AddProperty("Hub_Activity", activity);
			return this;
		}

		public Metrics AddClick(string itemId)
		{
			AddEventType("Click");
			AddProperty("Item_Id", itemId);
			return this;
		}

		public Metrics AddTimedBonus(RewardTimedBonus rewardTimedBonus)
		{
			if (rewardTimedBonus == null)
			{
				return this;
			}
			AddEventType("TimedBonus");
			AddProperty("Bonus_Type", rewardTimedBonus.TimedBonusType.ToString());
			AddProperty("Bonus_Duration", rewardTimedBonus.Duration.ToString());
			return this;
		}

		public Metrics AddEquipToken(RewardEquipToken rewardEquipToken)
		{
			if (rewardEquipToken == null)
			{
				return this;
			}
			AddEventType("EquipToken");
			AddProperty("EquipTokenId", rewardEquipToken.EquipTokenId.ToString());
			AddProperty("EquipTokenRewardAmount", rewardEquipToken.RewardAmount.ToString());
			return this;
		}

		public Metrics AddHeroSkin(RewardHeroSkin rewardHeroSkin)
		{
			if (rewardHeroSkin == null)
			{
				return this;
			}
			AddEventType("HeroSkin");
			AddProperty("HeroSkins", JsonConvert.SerializeObject(rewardHeroSkin.PreferredOrder));
			return this;
		}

		public Metrics SurvivorSlot()
		{
			AddEventType("SurvivorSlot");
			return this;
		}

		public Metrics AddSurvivorClassUnlock(SurvivorClass classUnlocked)
		{
			AddEventType("SurvivorClassUnlock");
			AddProperty("Class", classUnlocked.ToString());
			return this;
		}

		public Metrics AddFillTank()
		{
			AddEventType("FillTank");
			AddProperty("Max_Tank", manager.Player.GetCurrency(CurrencyType.ReplayToken).Max.ToString());
			return this;
		}

		public Metrics AddAutoFillTank()
		{
			AddEventType("AutoFillTank");
			AddProperty("Max_Tank", manager.Player.GetCurrency(CurrencyType.ReplayToken).Max.ToString());
			return this;
		}

		public Metrics AddFromLootDecision(LootDecision lootDecision)
		{
			return lootDecision switch
			{
				LootDecision.Accept => AddAccept(),
				LootDecision.Reject => AddReject(),
				_ => AddIgnore(),
			};
		}

		public Metrics AddFromSurvivorSource(NewSurvivorSource newSurvivorSource)
		{
			if (newSurvivorSource == NewSurvivorSource.Mission)
			{
				return AddMission();
			}
			return AddRadioCall();
		}

		public Metrics AddFromEquipmentSource(EquipmentSource source)
		{
			switch (source)
			{
			case EquipmentSource.Cinema:
			case EquipmentSource.Bundle:
				return AddCinema();
			case EquipmentSource.Debug:
			case EquipmentSource.MissionLoot:
			case EquipmentSource.TradeGoodsShop:
			case EquipmentSource.GuildGift:
				return AddDebug();
			default:
				return AddRadioCall();
			}
		}

		public Metrics AddUpdateGift()
		{
			AddEventType("UpdateGift");
			return this;
		}

		public Metrics AddMissionType()
		{
			switch (GetMissionKind())
			{
			case "gvg":
				return AddGvG();
			case "pvp":
				return AddPvp();
			case "grind":
				return AddGrind();
			case "weekly_challenge":
				return AddChallenge();
			case "weekly_apocalyptic_challenge":
				return AddApocalyChallenge();
			case "distance":
				return AddDistance();
			case "season":
				return AddSeason();
			case "endless":
			{
				string endlessModeGameModeType = manager.Player.EndlessModeManager.EndlessModeGameModeType.ToString();
				return AddEndless(endlessModeGameModeType);
			}
			default:
				return AddStory();
			}
		}

		public string GetMissionKind()
		{
			MapMissionModel attackTargetMissionModel = manager.Player.MapContainerModel.AttackTargetMissionModel;
			MapMissionGroupModel attackTargetMissionGroupModel = manager.Player.MapContainerModel.AttackTargetMissionGroupModel;
			if (manager.Player.GuildBattlePlayer.AttackTargetMissionModel != null)
			{
				return "gvg";
			}
			if (manager.Player.Combat != null && manager.Player.Combat.HasPvPRules)
			{
				return "pvp";
			}
			if (attackTargetMissionModel != null && attackTargetMissionModel.manager != null && attackTargetMissionGroupModel.MissionSpawnPointGroup != null)
			{
				switch (attackTargetMissionModel.MissionSpawnPointGroup.Category)
				{
				case MapCategory.Story:
					return "story";
				case MapCategory.Grind:
					return "grind";
				case MapCategory.ApocalypticChallenge:
					return "weekly_apocalyptic_challenge";
				case MapCategory.Challenge:
					return "weekly_challenge";
				case MapCategory.Season:
					return "season";
				case MapCategory.Survival:
					return "distance";
				case MapCategory.Endless:
					return "endless";
				case MapCategory.GuildBoss:
					return "guild_boss";
				case MapCategory.GuildBossPVE:
					return "guild_boss_pve";
				case MapCategory.GuildBossPVP:
					return "guild_boss_pvp";
				case MapCategory.None:
					return "none";
				}
			}
			return "?";
		}

		private string GetMissionNameEnglishForAnalytics(MapMissionModel attackTargetMissionModel, MapCategory category)
		{
			CombatModel combatModel = manager.CombatModel;
			if (combatModel == null)
			{
				return "";
			}
			string result = "";
			if (attackTargetMissionModel == null)
			{
				result = (string.IsNullOrEmpty(combatModel.SceneName) ? "outpost" : combatModel.SceneName);
			}
			else
			{
				switch (category)
				{
				case MapCategory.ApocalypticChallenge:
				{
					MapMissionGroupModel missionGroupModelThatContains4 = manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains4 != null && missionGroupModelThatContains4.MissionSpawnPointGroup != null)
					{
						result = $"A_{missionGroupModelThatContains4.MissionSpawnPointGroup.DisplayName}_M_{combatModel.MissionNameEnglish}";
					}
					break;
				}
				case MapCategory.Challenge:
				{
					MapMissionGroupModel missionGroupModelThatContains3 = manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains3 != null && missionGroupModelThatContains3.MissionSpawnPointGroup != null)
					{
						result = $"C_{missionGroupModelThatContains3.MissionSpawnPointGroup.DisplayName}_M_{combatModel.MissionNameEnglish}";
					}
					break;
				}
				case MapCategory.Survival:
				{
					MapMissionGroupModel missionGroupModelThatContains2 = manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains2 != null && missionGroupModelThatContains2.MissionSpawnPointGroup != null)
					{
						result = $"S_{missionGroupModelThatContains2.MissionSpawnPointGroup.DisplayName}_M_{combatModel.MissionNameEnglish}";
					}
					break;
				}
				case MapCategory.Story:
					result = string.Format("E{0}M{1}_{2}", (manager.Player.MapContainerModel.GetEpisodeIndex(attackTargetMissionModel) + 1).ToString("D2"), (manager.Player.MapContainerModel.GetMissionIndex(attackTargetMissionModel) + 1).ToString("D2"), combatModel.MissionNameEnglish);
					break;
				case MapCategory.Grind:
					result = string.Format("GL{0}", attackTargetMissionModel.MissionLevel.ToString("D2"));
					break;
				case MapCategory.Season:
					result = "SEASON";
					break;
				case MapCategory.GuildBoss:
				case MapCategory.GuildBossPVE:
				case MapCategory.GuildBossPVP:
				{
					MapMissionGroupModel missionGroupModelThatContains = manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains != null && missionGroupModelThatContains.MissionSpawnPointGroup != null)
					{
						result = string.Format("{0}_{1}_M_{2}", category switch
						{
							MapCategory.GuildBossPVP => "GB_PVP",
							MapCategory.GuildBossPVE => "GB_PVE",
							_ => "GB",
						}, missionGroupModelThatContains.MissionSpawnPointGroup.DisplayName, combatModel.MissionNameEnglish);
					}
					break;
				}
				default:
					result = "unknown_mission_kind";
					break;
				}
			}
			return result;
		}

		public Metrics AddIAPInitiation()
		{
			AddEventType("IapInitiation");
			return this;
		}

		public Metrics AddIAPConfirmationResult(PurchaseConfirmationResult result)
		{
			AddEventType("IapConfirmation");
			AddProperty("Iap_Confirmation_Result", result.ToString());
			return this;
		}

		public Metrics AddIAPValidationResult(PurchaseValidationResult result, string trackingId, int councilLevel, int shopTabIndex, int shopPosition)
		{
			AddEventType("IapValidation");
			AddProperty("Iap_Validation", result.ToString());
			AddProperty("trackingId", trackingId);
			AddProperty("council_level", councilLevel);
			AddProperty("shopTabIndex", shopTabIndex);
			AddProperty("shopPosition", shopPosition);
			return this;
		}

		public Metrics AddResetCombat(bool isInCombat)
		{
			AddEventType("ResetCombat");
			AddProperty("is_in_combat", isInCombat.ToString());
			return this;
		}

		public Metrics AddChangeName(string newName, string oldName)
		{
			AddEventType("ChangeName");
			AddProperty("New_Name", newName);
			AddProperty("Old_Name", oldName);
			return this;
		}

		public Metrics AddShare()
		{
			AddEventType("Share");
			return this;
		}

		public Metrics AddShopVisit(string listOfBundleIds, BundleSource bundleSource, int viewTimeInSeconds, int shopTabIndex)
		{
			AddEventType("ShopVisit");
			AddProperty("View_Time", viewTimeInSeconds.ToString());
			AddProperty("Bundle_Source", bundleSource.ToString());
			AddProperty("Shop_Content", listOfBundleIds);
			if (shopTabIndex > -1)
			{
				AddProperty("Shop_Tab_Index", shopTabIndex.ToString());
			}
			return this;
		}

		public Metrics AddEndSearchGuild(GuildSearchInfo guildSearchInfo)
		{
			AddEventType("End_SearchGuild");
			AddProperty("Search_Id", guildSearchInfo.SearchId);
			AddProperty("Search_Position", guildSearchInfo.GetSearchPositions());
			string searchType = guildSearchInfo.GetSearchType();
			if (searchType == null)
			{
				manager.Debug.LogWarning("Got null search type string, unsupported search type? (" + guildSearchInfo.Type.ToString() + ")");
			}
			AddProperty("Search_Type", searchType);
			AddProperty("Search_Keyword", guildSearchInfo.SearchKeyword);
			AddProperty("Search_Duration", guildSearchInfo.SearchDuration.ToString());
			AddProperty("Search_GuildCount_QuerySet", guildSearchInfo.GetGuildCountsQueried());
			AddProperty("Search_GuildCount_SelectedSet", guildSearchInfo.GetGuildCountsSelected());
			AddProperty("Selected_Guild_Size", guildSearchInfo.GetSelectedGuildSizes());
			AddProperty("Selected_Guild_Query_Id", guildSearchInfo.GetSelectedGuildQueryIds());
			AddProperty("Selected_Guild_Country_Code", guildSearchInfo.GetSelectedGuildCountryCodes());
			AddProperty("Selected_Guild_Avg_Player_Level", guildSearchInfo.GetSelectedGuildAvgPlayerLevels());
			AddProperty("player_level", guildSearchInfo.PlayerLevel.ToString());
			AddProperty("countryCode", guildSearchInfo.PlayerCountryCode);
			return this;
		}

		public Metrics AddReseedRandom()
		{
			AddEventType("ReseedRandom");
			return this;
		}

		public Metrics AddOriginalEventType()
		{
			UseOriginalEventType = true;
			return this;
		}

		public void WalkerTapped(LootEntry loot)
		{
			if (walkerTapMetrics == null)
			{
				walkerTapMetrics = new Metrics(manager);
			}
			if (!walkerTapMetrics.metricsResourcesData.HasResources())
			{
				walkerTapMetrics.AddFind();
			}
			numberWalkersTapped++;
			walkerTapMetrics.PushResource(loot.RewardedCurrency, loot.ActualAmountAdded, (loot.RewardedAmount != loot.ActualAmountAdded) ? (loot.RewardedAmount - loot.ActualAmountAdded) : 0);
		}

		public void SendWalkersTapMetric()
		{
			if (walkerTapMetrics != null && walkerTapMetrics.metricsResourcesData != null && walkerTapMetrics.metricsResourcesData.HasResources())
			{
				walkerTapMetrics.AddResources().AddWalkerTap(numberWalkersTapped).Send();
				numberWalkersTapped = 0;
			}
		}

		public void Send()
		{
			if (OfflineManager.IsUseSendMetrics)
			{
				SendWalkersTapMetric();
				manager.SendMetricsEvent(eventType.ToString(), properties);
			}
			OriginEventType = eventType.ToString();
			Reset();
		}

		public void SendTdEvent()
		{
			if (TdEventType.Length == 0 || tdProperties.Count == 0)
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (string tdEventPropertyType in TdEventPropertyTypes)
			{
				if (!tdProperties.ContainsKey(tdEventPropertyType))
				{
					continue;
				}
				foreach (KeyValuePair<string, object> item in tdProperties[tdEventPropertyType])
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			if (dictionary.Count != 0)
			{
				if (UseOriginalEventType)
				{
					dictionary.Add("Mission_Type", OriginEventType);
				}
				manager.TdMetrics.SetEventType(TdEventType).SetProperties(dictionary).Send();
				ResetTdEvent();
			}
		}

		public Metrics AddReward(IReward reward)
		{
			if (reward != null && reward is RewardCurrency)
			{
				RewardCurrency rewardCurrency = reward as RewardCurrency;
				Dictionary<CurrencyType, OverflowableAmount> dictionary = new Dictionary<CurrencyType, OverflowableAmount>();
				dictionary.Add(rewardCurrency.CurrencyType, new OverflowableAmount
				{
					Amount = rewardCurrency.AmountActuallyAdded,
					Overflow = rewardCurrency.Amount - rewardCurrency.AmountActuallyAdded
				});
				AddResources(dictionary);
			}
			return this;
		}

		public Metrics AddGenerate()
		{
			AddEventType("Generate");
			return this;
		}

		public Metrics AddProgress()
		{
			AddEventType("Progress");
			return this;
		}

		private void AddNewDailyQuestDefinitionShared(DailyQuestDefinition def, int size)
		{
			AddProperty("Quest_Category", def.Category);
			string value = "Invalid";
			if (size == def.S)
			{
				value = "S";
			}
			else if (size == def.M)
			{
				value = "M";
			}
			else if (size == def.L)
			{
				value = "L";
			}
			AddProperty("Quest_Size", value);
		}

		public Metrics AddNewDailyQuest(DailyQuestItemModel quest, string setId, int slotIndex, int progressDelta = 0, int questPointsDelta = 0)
		{
			AddEventType("NewDailyQuest");
			AddProperty("Quest_Id", quest.Id);
			DailyQuestDefinition definition = quest.Definition;
			if (definition != null)
			{
				int completionTotalCap = quest.CompletionTotalCap;
				AddNewDailyQuestDefinitionShared(definition, completionTotalCap);
			}
			AddProperty("Quest_SizeN", quest.CompletionTotalCap.ToString());
			AddProperty("Quest_Number", (quest.SlotIndex + 1).ToString());
			AddProperty("Quest_X", quest.CompletedCount.ToString());
			if (progressDelta != 0)
			{
				AddProperty("Quest_X_Delta", progressDelta.ToString());
			}
			string value = (quest.IsCompleted ? "2" : "1");
			AddProperty("Quest_Status", value);
			if (questPointsDelta != 0)
			{
				AddProperty("Quest_Trophies_Delta", questPointsDelta.ToString());
			}
			AddProperty("Quest_SetId", setId);
			AddProperty("Quest_SetSlot", (slotIndex + 1).ToString());
			return this;
		}

		public Metrics AddNewDailyQuestGenerationFailed(string setId, int slotIndex)
		{
			AddEventType("NewDailyQuest");
			AddProperty("Quest_SetId", setId);
			AddProperty("Quest_Status", "0");
			AddProperty("Quest_SetSlot", (slotIndex + 1).ToString());
			return this;
		}

		public Metrics AddDailyQuestChest(int questPointsRequired, string questChestId)
		{
			AddEventType("DailyQuestChest");
			AddProperty("Quest_Points_Required", questPointsRequired.ToString());
			AddProperty("Quest_Chest_Id", questChestId);
			AddProperty("Quest_Trophies_Delta", (-questPointsRequired).ToString());
			return this;
		}

		public Metrics AddCampaign(string campaignId, string control)
		{
			AddEventType("Campaign");
			AddProperty("Campaign_Id", campaignId);
			AddProperty("Campaign_Control", control);
			return this;
		}

		public Metrics AddStartGdpr(string dialogueName, string dialogueDecision = null, string dialogueDecisionDate = null)
		{
			AddEventType("Start_GDPR");
			AddProperty("Dialogue_Name", dialogueName);
			AddProperty("Dialogue_Decision", dialogueDecision);
			AddProperty("Dialogue_DeletionDate", dialogueDecisionDate);
			return this;
		}

		public Metrics AddEndGdpr(string dialogueName, string dialogueDecision = null, string dialogueDecisionDate = null)
		{
			AddEventType("End_GDPR");
			AddProperty("Dialogue_Name", dialogueName);
			AddProperty("Dialogue_Decision", dialogueDecision);
			AddProperty("Dialogue_DeletionDate", dialogueDecisionDate);
			return this;
		}

		public Metrics AddOpenGdprLink(string dialogueName, string linkName)
		{
			AddEventType("Open_GDPR_Link");
			AddProperty("Dialogue_Name", dialogueName);
			AddProperty("Link_Name", linkName);
			return this;
		}

		public Metrics AddGuildInvite()
		{
			AddEventType("Guild_Invite");
			return this;
		}

		public Metrics AddInvitedToGuildTutorial(string guildId, string inviterPlayerId)
		{
			AddEventType("Invited_To_Guild_Tutorial");
			AddProperty("Inviter_Guild_Id", guildId);
			AddProperty("Inviter_Player_Id", inviterPlayerId);
			return this;
		}

		public Metrics AddInvitedToGuildCombat(string guildId, string inviterPlayerId)
		{
			AddEventType("Invited_To_Guild_Combat");
			AddProperty("Inviter_Guild_Id", guildId);
			AddProperty("Inviter_Player_Id", inviterPlayerId);
			return this;
		}

		public Metrics AddInvitedToGuildPopup(string guildId, string guildStatus, bool isAlreadyInGuild, bool hasLeftGuild)
		{
			AddEventType("Invited_To_Guild_Popup");
			AddProperty("Inviter_Guild_Id", guildId);
			AddProperty("Guild_Status", guildStatus);
			AddProperty("Is_In_Guild", isAlreadyInGuild);
			AddProperty("Has_Left_Guild", hasLeftGuild);
			return this;
		}

		public Metrics AddGvG(bool fromPlayer = true)
		{
			GuildModel guildModel = manager.Player.GuildModel;
			if (guildModel == null)
			{
				return this;
			}
			if (fromPlayer)
			{
				GvGSeasonModelPlayer gvGSeasonModelPlayer = manager.Player.GvGSeasonModelPlayer;
				return AddGvG(guildModel.Id, gvGSeasonModelPlayer.StartedGvGSeasonId, gvGSeasonModelPlayer.GuildWarModelPlayer.StartedWarId, guildModel.GuildBattleTier, guildModel.CurrentVictoryPoints);
			}
			return AddGvG(guildModel.Id, guildModel.GvGSeasonModel.SeasonDefinitionId, guildModel.GuildWarModel.WarDefinitionId, guildModel.GuildBattleTier, guildModel.CurrentVictoryPoints);
		}

		public Metrics AddGvG(string guildId, int seasonId, int warId, int guildTier, int victoryPoints)
		{
			AddEventType("GvG");
			AddProperty("Guild_Id", guildId);
			AddProperty("Guild_Season_Id", seasonId);
			AddProperty("Guild_War_Id", warId);
			AddProperty("Guild_Tier", guildTier);
			AddProperty("Guild_VP_State", victoryPoints);
			AddAndResetOneTdPropertyType("GvG");
			AddTdProperty("GvG", "GvG_Guild_Id", guildId);
			AddTdProperty("GvG", "GvG_Guild_Season_Id", seasonId);
			AddTdProperty("GvG", "GvG_Guild_War_Id", warId);
			AddTdProperty("GvG", "GvG_Guild_Tier", guildTier);
			AddTdProperty("GvG", "GvG_Guild_VP_State", victoryPoints);
			return this;
		}

		public Metrics AddBattleSignup(long battleTimeSlot, List<string> registeredPlayers)
		{
			AddEventType("BattleSignup");
			AddProperty("Battle_Timeslot", battleTimeSlot);
			AddProperty("Sign_In_Count", registeredPlayers.Count);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < registeredPlayers.Count - 1; i++)
			{
				stringBuilder.Append(registeredPlayers[i]);
				stringBuilder.Append(",");
			}
			stringBuilder.Append(registeredPlayers[registeredPlayers.Count - 1]);
			AddProperty("Registered_players", stringBuilder.ToString());
			return this;
		}

		public Metrics AddBattleSignupKick(long battleTimeSlot)
		{
			AddEventType("BattleSignup");
			AddProperty("Battle_Timeslot", battleTimeSlot);
			AddProperty("is_sign_in", value: false);
			return this;
		}

		public Metrics AddGvGBattle(string battleId, long battleTimeSlot, bool isFake)
		{
			AddEventType("GvGBattle");
			AddProperty("GvG_Battle_Id", battleId);
			AddProperty("Battle_Timeslot", battleTimeSlot);
			AddProperty("Battle_Is_Fake", isFake);
			AddAndResetOneTdPropertyType("GvGBattle");
			AddTdProperty("GvGBattle", "GvGBattle_GvG_Battle_Id", battleId);
			AddTdProperty("GvGBattle", "GvGBattle_Battle_Timeslot", battleTimeSlot);
			AddTdProperty("GvGBattle", "GvGBattle_Battle_Is_Fake", isFake);
			return this;
		}

		public Metrics AddWorldBoss(WorldBossBattlegroundDefinition battlegroundDefinition, WorldBossCycleDefinition cycleDefinition, int battleDifficulty)
		{
			AddEventType("WorldBoss");
			AddAndResetOneTdPropertyType("WorldBoss");
			if (battlegroundDefinition == null || cycleDefinition == null)
			{
				return this;
			}
			AddProperty("WorldBoss_Battle_Type", battlegroundDefinition.CapturePointType);
			AddProperty("WorldBoss_Battle_Bg", battlegroundDefinition.CapturePoint);
			AddProperty("WorldBoss_Battle_Cycle", cycleDefinition.ID);
			AddProperty("WorldBoss_Battle_Difficulty", battleDifficulty);
			AddTdProperty("WorldBoss", "WorldBoss_Battle_Type", battlegroundDefinition.CapturePointType);
			AddTdProperty("WorldBoss", "WorldBoss_Battle_Bg", battlegroundDefinition.CapturePoint);
			AddTdProperty("WorldBoss", "WorldBoss_Battle_Cycle", cycleDefinition.ID);
			AddTdProperty("WorldBoss", "WorldBoss_Battle_Difficulty", battleDifficulty);
			return this;
		}

		public Metrics AddWorldBossBattleResult(int scoreChange, string heroWeaponUse, int cycleDefinitionId, int battleDifficulty)
		{
			AddEventType("WorldBoss");
			AddProperty("WorldBoss_Battle_ScoreChange", scoreChange);
			AddProperty("WorldBoss_Battle_HeroWeaponUse", heroWeaponUse ?? string.Empty);
			AddProperty("WorldBoss_Battle_Cycle", cycleDefinitionId);
			AddProperty("WorldBoss_Battle_Difficulty", battleDifficulty);
			AddAndResetOneTdPropertyType("WorldBoss");
			AddTdProperty("WorldBoss", "WorldBoss_Battle_ScoreChange", scoreChange);
			AddTdProperty("WorldBoss", "WorldBoss_Battle_HeroWeaponUse", heroWeaponUse ?? string.Empty);
			AddTdProperty("WorldBoss", "WorldBoss_Battle_Cycle", cycleDefinitionId);
			AddTdProperty("WorldBoss", "WorldBoss_Battle_Difficulty", battleDifficulty);
			return this;
		}

		public Metrics AddGuildBoss(string bossName, int round, int difficulty, long points)
		{
			AddEventType("GuildBoss");
			AddProperty("GuildBoss_Name", bossName);
			AddProperty("GuildBoss_Round", round);
			AddProperty("GuildBoss_Difficulty", difficulty);
			AddProperty("GuildBoss_Points", points);
			AddAndResetOneTdPropertyType("GuildBoss");
			AddTdProperty("GuildBoss", "GuildBoss_Name", bossName);
			AddTdProperty("GuildBoss", "GuildBoss_Round", round);
			AddTdProperty("GuildBoss", "GuildBoss_Difficulty", difficulty);
			AddTdProperty("GuildBoss", "GuildBoss_Points", points);
			return this;
		}

		public Metrics AddGvGBattleResult()
		{
			GuildBattleModel currentBattle = manager.Player.GuildModel.GvGSeasonModel.GuildWarModel.CurrentBattle;
			AddEventType("GvGBattleResult");
			AddProperty("Battle_OurscoreVP", currentBattle.EndVictoryPoints);
			AddProperty("Battle_TheirscoreVP", currentBattle.EndEnemyVictoryPoints);
			AddProperty("Battle_Result", currentBattle.IsVictory() ? "1" : "0");
			if (currentBattle.IsVictory())
			{
				AddProperty("Battle_WinBonusVP", currentBattle.GetBattleWonBonusVictoryPoints());
			}
			if (currentBattle.IsDraw())
			{
				AddProperty("Battle_DrawBonusPoints", currentBattle.GetBattleDrawPoints());
			}
			return this;
		}

		public Metrics AddLeaderEvent(SurvivorModel survivor)
		{
			AddEventType("CombatLeader");
			AddProperty("Leader_Name", survivor.IsHero ? survivor.Definition.FullName : survivor.Definition.Class);
			return this;
		}

		public Metrics AddGvGBattle(long battleTimeSlot)
		{
			AddEventType("GvGBattle");
			AddProperty("Battle_Timeslot", battleTimeSlot);
			return this;
		}

		public Metrics AddGvGBattle(bool fromPlayer = true)
		{
			if (fromPlayer)
			{
				GuildBattleModelPlayer guildBattleModelPlayer = manager.Player.GvGSeasonModelPlayer?.GuildWarModelPlayer?.GuildBattleModel;
				if (guildBattleModelPlayer == null)
				{
					return this;
				}
				return AddGvGBattle(guildBattleModelPlayer.CurrentBattleId, guildBattleModelPlayer.CurrentBattleTimeSlot, guildBattleModelPlayer.IsFakeBattle);
			}
			GuildBattleModel currentBattle = manager.Player.GuildModel.GuildWarModel.CurrentBattle;
			return AddGvGBattle(currentBattle.BattleId, currentBattle.TimeSlot, currentBattle.IsFakeBattle);
		}

		public Metrics AddPvP()
		{
			AddEventType("PVP");
			return this;
		}

		public Metrics AddTierUp()
		{
			AddEventType("TierUp");
			return this;
		}

		public Metrics AddGvGSeason()
		{
			AddEventType("GvGSeason");
			return this;
		}

		public Metrics AddBattlePassRefresh()
		{
			AddEventType("BattlePassRefresh");
			return this;
		}

		public Metrics AddLootKeysRefresh()
		{
			AddEventType("LootKeysRefresh");
			return this;
		}

		public Metrics AddSector(GuildBattleMapSectorModel sectorModel)
		{
			AddEventType("Sector");
			if (sectorModel != null)
			{
				AddProperty("Sector_Number", sectorModel.SectorId);
			}
			return this;
		}

		public Metrics AddBattleBonus(string bonusId)
		{
			TraitDefinition traitDefinition = manager.GameEconomyData.GetTraitDefinition(bonusId);
			if (traitDefinition != null)
			{
				AddEventType("BattleBonus");
				AddProperty("BattleBonus_Id", bonusId);
				if (traitDefinition.ConstructionParameters.Count > 0)
				{
					AddProperty("BattleBonus_Value", traitDefinition.ConstructionParameters[0]);
				}
			}
			return this;
		}

		public Metrics AddGvGPvPInfoIfNeeded()
		{
			if (manager.Player.GuildWarModel != null)
			{
				GuildBattleModel currentBattle = manager.Player.GuildWarModel.CurrentBattle;
				GuildBattleModelPlayer guildBattlePlayer = manager.Player.GuildBattlePlayer;
				if (guildBattlePlayer.AttackTargetMission.IsPvPCombat)
				{
					GuildBattlePvpTeam pvpTeamForMission = currentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattlePlayer.AttackTargetMissionModel.Id);
					if (pvpTeamForMission != null)
					{
						GuildBattleParticipantInfo currentGuildBattlePlayerInfo = currentBattle.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission.OwnerHashedPlayerId);
						AddGvGAttacker().AddGvGDefender(currentGuildBattlePlayerInfo, pvpTeamForMission, GvGModelHelper.GetPlayerSpecificDifficulty(manager.Player));
					}
				}
			}
			return this;
		}

		public Metrics AddGvGTierUp(GuildTierDefinition tierReached)
		{
			AddEventType("GvGTierUp");
			AddProperty("Tier_Number", tierReached.Tier);
			AddProperty("Tier_VP_Requirement", tierReached.VictoryPointsRequired);
			return this;
		}

		public Metrics AddGvGCrate(GuildShopItemInfo itemInfo)
		{
			AddEventType("GvGCrate");
			AddProperty("GvG_Item_Id", itemInfo.ItemDefinition.ID);
			AddProperty("GvG_Item_Season", itemInfo.ItemDefinition.Season);
			AddProperty("GvG_Item_Requirement", itemInfo.ItemDefinition.TierRequirement);
			AddProperty("GvG_Item_Stock_Remaining", itemInfo.ItemDefinition.LimitedPurchases ? itemInfo.AvailableAmount : 999);
			AddProperty("GvG_Available_Slots", manager.Player.GuildShopModel.GetSlotsWithStockCount());
			return this;
		}

		public Metrics AddSurvivorsHealth()
		{
			AddEventType("Health");
			string text = string.Empty;
			if (manager?.CombatModel?.Survivors != null)
			{
				foreach (ActorModel survivor in manager.CombatModel.Survivors)
				{
					text = text + ((float)((!survivor.OnRedHealthBar) ? survivor.MaxHitPoints : 0) + (float)survivor.Hitpoints) / (float)(survivor.MaxHitPoints * 2) + ",";
				}
			}
			if (text != string.Empty)
			{
				text = text.Substring(0, text.Length - 1);
			}
			AddProperty("Survivor_Health", text);
			return this;
		}

		public Metrics AddCalendar(int day)
		{
			AddEventType("Calendar");
			AddProperty("Calendar_Day", day);
			return this;
		}

		public Metrics AddSevenDay(int periodId, int day)
		{
			AddEventType("SevenDay");
			AddProperty("SevenDay_PeriodId", periodId);
			AddProperty("SevenDay_Day", day);
			return this;
		}

		public Metrics AddActiveFoundation(int periodId, int day)
		{
			AddEventType("ActiveFoundation");
			AddProperty("ActiveFoundation_PeriodId", periodId);
			AddProperty("ActiveFoundation_Day", day);
			return this;
		}

		public Metrics AddBadgeReroll(BadgeReroll rerollType, int rerollCost, string oldValue)
		{
			AddEventType("BadgeReroll");
			AddProperty("RerollType", rerollType.ToString());
			AddProperty("RerollCost", rerollCost);
			AddProperty("OldValue", oldValue);
			return this;
		}

		public Metrics AddIDFAPopupMetric(bool nativePopup, int position, string action)
		{
			AddEventType("IdfaPopup");
			AddProperty("IdfaPopupType", nativePopup ? "system" : "custom");
			AddProperty("IdfaPopupLocation", position);
			AddProperty("IdfaPopupAction", action);
			return this;
		}

		public Metrics AddPLTVValue(int value)
		{
			AddEventType("AnalyticsCallback");
			AddProperty("DataType", "SkAdNetworkValue");
			AddProperty("Result", value);
			return this;
		}

		public Metrics AddIDFAStatus(string status)
		{
			AddEventType("IdfaStatus");
			AddProperty("IdfaStatus", status);
			return this;
		}

		public Metrics AddRetryScreen()
		{
			AddEventType("RetryScreen");
			PlayerModel player = manager.Player;
			GuildBattleMapMissionModel attackTargetMissionModel = player.GuildBattlePlayer.AttackTargetMissionModel;
			if (attackTargetMissionModel != null && manager.Player.Combat.MissionResult != ECombatResult.Successful)
			{
				Cashier retryGvGMissionCashier = attackTargetMissionModel.GetRetryGvGMissionCashier(manager);
				int num = manager.GameEconomyData.GuildWarConfig.MaxAmountOfRetries - player.GuildBattlePlayer.CurrentMissionRetriedAttempts;
				bool flag = player.GuildBattlePlayer.IsCurrentGuildBattle() && player.GuildWarModel.CurrentBattle.IsOngoing(player.UtcTimeStamp + 5000);
				bool value = retryGvGMissionCashier.CanAfford() && num > 0 && flag;
				AddProperty("CanRetry", value);
				AddProperty("HasGas", retryGvGMissionCashier.CanAfford());
				AddProperty("RetriesLeft", num);
				AddProperty("IsCurrentBattle", flag);
				AddProperty("RetryCost", retryGvGMissionCashier.GetTotalCost(CurrencyType.GvGGas));
			}
			return this;
		}

		public Metrics AddBlackMarket(BlackMarketDefinition blackMarketDefinition)
		{
			AddEventType("BlackMarket");
			AddProperty("HeroId", blackMarketDefinition.ActorDefinitionID);
			return this;
		}

		public Metrics AddHillTopStore(HillTopStoreDefinition hillTopStoreDefinition)
		{
			AddEventType("HillTopStore");
			AddProperty("UniqueId", hillTopStoreDefinition.UniqueId);
			return this;
		}

		public Metrics AddBlackMarketRefresh()
		{
			AddEventType("BlackMarketRefresh");
			return this;
		}

		public Metrics AddGiftCodeRedeem(GiftCodeDefinition giftCodeDefinition)
		{
			AddEventType("GiftCode");
			AddProperty("Gift_Code", giftCodeDefinition.Code);
			return this;
		}

		public Metrics AddSupportUnit(SupportModel supportModel)
		{
			AddEventType("SupportUnit");
			AddAndResetOneTdPropertyType("SupportUnit");
			if (supportModel != null)
			{
				AddProperty("Support_Id", supportModel.SupportId);
				AddProperty("Support_Rarity", supportModel.Level);
				AddProperty("Support_Cooldown", supportModel.Cooldown);
				AddTdProperty("SupportUnit", "Support_Id", supportModel.SupportId);
				AddTdProperty("SupportUnit", "Support_Rarity", supportModel.Level);
				AddTdProperty("SupportUnit", "Support_Cooldown", supportModel.Cooldown);
			}
			return this;
		}

		public Metrics AddSupportResult(CombatSupportModel combatSupportModel)
		{
			AddEventType("SupportResult");
			AddAndResetOneTdPropertyType("SupportResult");
			if (combatSupportModel != null)
			{
				AddProperty("Support_Affected_Targets", combatSupportModel.AffectedTargets.Count);
				AddProperty("Support_Affected_Unique_Targets", combatSupportModel.AffectedTargets.Distinct().Count());
				AddProperty("Support_Missions_Played", combatSupportModel.SupportModel.MissionsPlayedCount);
				AddProperty("Support_Used", combatSupportModel.UsedTurns.Count);
				AddProperty("Support_Turns", ListToString(combatSupportModel.UsedTurns, ','));
				AddTdProperty("SupportResult", "Support_Affected_Targets", combatSupportModel.AffectedTargets.Count);
				AddTdProperty("SupportResult", "Support_Affected_Unique_Targets", combatSupportModel.AffectedTargets.Distinct().Count());
				AddTdProperty("SupportResult", "Support_Missions_Played", combatSupportModel.SupportModel.MissionsPlayedCount);
				AddTdProperty("SupportResult", "Support_Used", combatSupportModel.UsedTurns.Count);
				AddTdProperty("SupportResult", "Support_Turns", ListToString(combatSupportModel.UsedTurns, ','));
			}
			return this;
		}

		public Metrics AddUnlock()
		{
			AddEventType("Unlock");
			return this;
		}

		public Metrics AddEndless(string endlessModeGameModeType)
		{
			AddEventType("Endless");
			AddAndResetOneTdPropertyType("Endless");
			AddProperty("Endless_Difficulty", endlessModeGameModeType);
			AddTdProperty("Endless", "Endless_Difficulty", endlessModeGameModeType);
			return this;
		}

		public Metrics AddEndlessModeNormalProgressReward(int rewardIndex)
		{
			AddEventType("EndlessModeNormalProgressReward");
			AddProperty("RewardIndex", rewardIndex);
			return this;
		}

		public Metrics AddRetry()
		{
			AddEventType("Retry");
			return this;
		}

		public Metrics AddEndCombatAnalyticsSource(string source)
		{
			source = (string.IsNullOrEmpty(source) ? "Unknown" : source);
			AddProperty("End_Combat_Analytics_Source", source);
			return this;
		}

		public Metrics AddCombatFailureReason(string failureReason)
		{
			if (!string.IsNullOrEmpty(failureReason))
			{
				AddProperty("Combat_Failure_Reason", failureReason);
			}
			return this;
		}

		public Metrics AddEndlessRefresh()
		{
			AddEventType("EndlessRefresh");
			return this;
		}

		public Metrics AddEndlessSubscriptionAdd()
		{
			AddEventType("EndlessSubscriptionAdd");
			return this;
		}

		public Metrics AddPerformanceRewards(int wave)
		{
			AddEventType("PerformanceRewards");
			AddProperty("Wave_Reward", wave);
			return this;
		}

		public Metrics AddEndlessCycle(int leaderboardId, string rewardBracket, long score)
		{
			AddEventType("EndlessCycle");
			AddProperty("Endless_Id", leaderboardId);
			AddProperty("Endless_Position", rewardBracket);
			AddProperty("Rewarded_Score", score);
			return this;
		}

		public Metrics AddEndlessLeaderSurvivorClassCycle(int leaderboardId, SurvivorClass survivorClass, long leaderBoardPosition, long leaderBoardEntryCount)
		{
			AddEventType("EndlessLeaderSurvivorClassCycle");
			AddProperty("Endless_Id", leaderboardId);
			AddProperty("Survivor_Class", survivorClass.ToString());
			AddProperty("LeaderBoard_Position", leaderBoardPosition);
			AddProperty("LeaderBoard_Entry_Count", leaderBoardEntryCount);
			return this;
		}

		public Metrics AddBattlePass(BattlePassModel battlePass, int? overrideSeason = null)
		{
			bool num = (overrideSeason.HasValue ? (overrideSeason.Value == int.MaxValue) : battlePass.IsBeginnerBattlePass);
			AddEventType("BattlePass");
			AddProperty("BP_Current_Tier", battlePass.ReachedTier + 1);
			if (!num)
			{
				AddProperty("BP_Season", overrideSeason ?? battlePass.CurrentSeasonId);
			}
			if (num)
			{
				AddEventType("Beginner");
			}
			return this;
		}

		public Metrics AddBattlePassSeason()
		{
			AddEventType("BPSeason");
			return this;
		}

		public Metrics AddBattlePassAdvanceTier()
		{
			AddEventType("BPAdvanceTier");
			return this;
		}

		public Metrics AddBattlePassGetPremium()
		{
			AddEventType("BPGetPremium");
			return this;
		}

		public Metrics AddBattlePassResetDailyKills(int day)
		{
			AddEventType("BPDailyResetKills");
			AddProperty("BP_Day_Of_Season", day);
			return this;
		}

		public Metrics AddBattlePassClaimProperties(int tier, bool premium, int rewardIndex, bool auto)
		{
			AddProperty("BP_Relevant_Tier", tier + 1);
			AddProperty("BP_Premium", premium);
			AddProperty("BP_Reward_Index", rewardIndex);
			AddProperty("BP_Autoclaim", auto);
			return this;
		}

		public Metrics AddBattlePassEnemiesKilledProperty(int amount)
		{
			AddProperty("BP_Enemies_Killed", amount);
			return this;
		}

		public Metrics AddBattlePassConvertedToGold(bool converted)
		{
			AddProperty("BP_Converted_To_Gold", converted);
			return this;
		}

		public Metrics AddBattlePassBonusChest()
		{
			AddEventType("BPBonusChestClaim");
			return this;
		}

		public Metrics AddClassTeamReward(string rewardInfo)
		{
			AddEventType("Class_Team_Challenge_Reward");
			AddProperty("reward_info", rewardInfo);
			return this;
		}

		public Metrics AddClassTeamExchange(string exchangeContent, int exchangeTime)
		{
			AddEventType("Class_Team_Challenge_Exchange");
			AddProperty("exchange_content", exchangeContent);
			AddProperty("exchange_time", exchangeTime);
			return this;
		}
	}
}
