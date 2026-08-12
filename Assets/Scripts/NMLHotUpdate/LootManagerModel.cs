using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseModel;
using Newtonsoft.Json;
using TwdCustomMod;
using TWDModel;

public class LootManagerModel : TWDModelObject
{
	public static int DefaultDropCount = 9;

	public static string LootKeyFoundEvent = "LootKeyFoundEvent";

	public static string TradeCrateLootAdded = "TradeCrateLootAdded";

	public static string BadgeCreatedEvent = "BadgeCreated";

	[JsonIgnore]
	public const CurrencyType RequiredFirstSlotBadgeComponentType = CurrencyType.Badge0;

	public int AvailableKeys { get; set; }

	public List<LootKeySource> LootKeysSources { get; set; }

	public ModelList<LootEntry> Loots { get; set; }

	public ModelList<LootEntry> PendingTradeCrates { get; set; }

	public int CurrentBadgeAnalyticsId { get; set; }

	[JsonIgnore]
	public EquipmentItemModel LastTradedEquipment { get; set; }

	public List<LootCummulativeProbabilityEntry> DropCummulativeProbabilities { get; set; }

	public Dictionary<string, ModelRandom> DedicatedRandoms { get; set; }

	public List<SpecialPhoneCallState> SpecialPhoneCallStates { get; set; }

	public List<CurrencyType> LastReceivedComponents { get; set; }

	public override void Initialize()
	{
		Loots = new ModelList<LootEntry>();
		Loots.SetManager(base.manager);
		Loots.Initialize();
		PendingTradeCrates = new ModelList<LootEntry>();
		PendingTradeCrates.SetManager(base.manager);
		PendingTradeCrates.Initialize();
		LootKeysSources = new List<LootKeySource>();
		DropCummulativeProbabilities = new List<LootCummulativeProbabilityEntry>();
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry.EventType = DropEventDefinition.DropEventType.MissionScavenge;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry);
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry2 = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry2.EventType = DropEventDefinition.DropEventType.MissionRescue;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry2);
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry3 = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry3.EventType = DropEventDefinition.DropEventType.RadioPhone;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry3);
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry4 = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry4.EventType = DropEventDefinition.DropEventType.VideoAd;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry4);
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry5 = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry5.EventType = DropEventDefinition.DropEventType.TradeCrate;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry5);
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry6 = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry6.EventType = DropEventDefinition.DropEventType.WalkerTapping;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry6);
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry7 = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry7.EventType = DropEventDefinition.DropEventType.EventWalkerTapping;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry7);
		SpecialPhoneCallStates = new List<SpecialPhoneCallState>();

		Debug.Log("LootManagerModel Initialized: " + manager.Player.Name);
	}

	public ModelRandom GetDedicatedRandom(string identifier)
	{
		if (DedicatedRandoms == null)
		{
			DedicatedRandoms = new Dictionary<string, ModelRandom>();
		}
		if (!DedicatedRandoms.ContainsKey(identifier))
		{
			int seed = (int)ModelHelpers.MD5SumLong(base.manager.Player.HashedId + identifier);
			DedicatedRandoms[identifier] = new ModelRandom(seed);
		}
		return DedicatedRandoms[identifier];
	}

	public override bool IsValid()
	{
		return true;
	}

	public void AddCombatFoundKey(int amount)
	{
		AvailableKeys += amount;
		if (AvailableKeys > CombatModel.MaxLootKeyAmount)
		{
			AvailableKeys = CombatModel.MaxLootKeyAmount;
		}
		if (LootKeysSources != null)
		{
			LootKeysSources.Add(LootKeySource.Combat);
		}
		NotifyChange(LootKeyFoundEvent);
	}

	public void Clear()
	{
		if (Loots != null)
		{
			Loots.Clear();
		}
	}

	public int GetSilverBoxCount()
	{
		int num = 0;
		for (int i = 0; i < Loots.Count; i++)
		{
			if (Loots[i].DropType == DropType.Silver)
			{
				num++;
			}
		}
		return num;
	}

	public int GetGoldenBoxCount()
	{
		int num = 0;
		for (int i = 0; i < Loots.Count; i++)
		{
			if (Loots[i].DropType == DropType.Gold)
			{
				num++;
			}
		}
		return num;
	}

	public int GetNonRegularBoxCount()
	{
		return GetSilverBoxCount() + GetGoldenBoxCount();
	}

	public void ShuffleRewards(LootEntryGenParams lootParams)
	{
		AvailableKeys = ((lootParams.eventType != DropEventDefinition.DropEventType.MissionScavenge && lootParams.eventType != DropEventDefinition.DropEventType.MissionRescue) ? 1 : 0);
		Loots.Clear();
		LootKeysSources.Clear();
		if (lootParams.eventType == DropEventDefinition.DropEventType.MissionScavenge && base.manager.Player.Tutorial.MissionHasFakeMissionRewards())
		{
			MapMissionModel attackTargetMissionModel = base.manager.Player.MapContainerModel.AttackTargetMissionModel;
			if (attackTargetMissionModel != null && attackTargetMissionModel.MissionData.DisplayTextID == "S01E01M03OutOfTheWoods")
			{
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints, CurrencyType.SurvivalPoints, 170));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies, CurrencyType.SurvivalPoints, 190));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Diamonds, CurrencyType.Supplies, 200));
			}
			else if (attackTargetMissionModel != null && attackTargetMissionModel.MissionData.DisplayTextID == "S01E01M02CheckTheCamp")
			{
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints, CurrencyType.SurvivalPoints, 200));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies, CurrencyType.Supplies, 160));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Phone, CurrencyType.Phone, 1));
			}
			else if (attackTargetMissionModel != null && attackTargetMissionModel.MissionData.DisplayTextID == "S01E01M04AlongTheTracks")
			{
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints, CurrencyType.SurvivalPoints, 100));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies, CurrencyType.Supplies, 100));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Phone, CurrencyType.Phone, 2, 0, DropType.Silver));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies, CurrencyType.Supplies, 100));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints, CurrencyType.SurvivalPoints, 100));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon, CurrencyType.None, 1, 2, DropType.Gold, 2));
			}
			else
			{
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints, CurrencyType.SurvivalPoints, 100));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies, CurrencyType.Supplies, 100));
				Loots.Add(GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Diamonds, CurrencyType.Supplies, 100));
			}
			return;
		}
		int num = ((lootParams.eventType != DropEventDefinition.DropEventType.MissionScavenge) ? 1 : DefaultDropCount);
		if (lootParams.eventType == DropEventDefinition.DropEventType.MissionScavenge)
		{
			DropType dropType = DropType.None;
			int num2 = base.manager.GameEconomyData.ConfigData.WeeklyEventProbabilityAllMissionGoldBoxes;
			int num3 = base.manager.GameEconomyData.ConfigData.WeeklyEventProbabilityAllMissionSilverBoxes;
			if (base.manager.Player.ActivityManager.TryGetActivityParam(ActivityType.Jackpot, out var activityParams))
			{
				num2 = int.Parse(activityParams[0]);
				num3 = int.Parse(activityParams[1]);
			}
			if (num2 != 0 && base.manager.Player.PlayerRandom.GetRandomInRange(1, 100) <= num2)
			{
				dropType = DropType.Gold;
			}
			if (dropType == DropType.None && num3 != 0 && base.manager.Player.PlayerRandom.GetRandomInRange(1, 100) <= num3)
			{
				dropType = DropType.Silver;
			}
			if (dropType != DropType.None)
			{
				DropEventDefinition dropEvent = base.manager.GameEconomyData.GetDropEvent(lootParams.eventType, lootParams.context, lootParams.tag);
				lootParams.dropEventDefinition = dropEvent;
				lootParams.dropType = dropType;
				lootParams.context = DropEventDefinition.DropEventContext.Normal;
				lootParams.random = base.manager.Player.PlayerRandom;
				for (int i = 0; i < num; i++)
				{
					Loots.Add(GenerateLootEntry(lootParams));
				}
				return;
			}
		}
		for (int j = 0; j < num; j++)
		{
			lootParams.random = base.manager.Player.PlayerRandom;
			Loots.Add(ShuffleOneLoot(lootParams));
		}
	}

	public LootEntry ShuffleOneLootWithoutTag(LootEntryGenParams lootParams)
	{
		if (lootParams.random == null)
		{
			lootParams.random = base.manager.Player.PlayerRandom;
		}
		DropEventDefinition dropEvent = base.manager.GameEconomyData.GetDropEvent(lootParams.eventType, lootParams.context, DropEventDefinition.DropEventTag.None);
		lootParams.dropEventDefinition = dropEvent;
		return GenerateLootEntry(lootParams);
	}

	public LootEntry ShuffleOneLoot(LootEntryGenParams lootParams, bool ignoreCumulativeProbability = false)
	{
		DropEventDefinition dropEvent = base.manager.GameEconomyData.GetDropEvent(lootParams.eventType, lootParams.context, lootParams.tag);
		if (dropEvent == null)
		{
			base.Debug.LogError("Drop event definition not found for " + lootParams.eventType.ToString() + " " + lootParams.context.ToString() + " " + lootParams.tag);
			return null;
		}
		lootParams.dropEventDefinition = dropEvent;
		if (GetNonRegularBoxCount() < dropEvent.MaxNonRegularDrops || ignoreCumulativeProbability)
		{
			LootCummulativeProbabilityEntry cummulativeProbability = GetCummulativeProbability(lootParams.eventType);
			int num = (int)FixedPoint.Round(dropEvent.GoldDropProbability + ((!ignoreCumulativeProbability) ? cummulativeProbability.GoldDropCummulativeProbability : ((FixedPoint)0L)));
			int num2 = (int)FixedPoint.Round(dropEvent.SilverDropProbability + ((!ignoreCumulativeProbability) ? cummulativeProbability.SilverDropCummulativeProbability : ((FixedPoint)0L)));
			int randomInRange = lootParams.random.GetRandomInRange(1, 100);
			if (randomInRange <= num)
			{
				if (!ignoreCumulativeProbability)
				{
					cummulativeProbability.GoldDropCummulativeProbability = 0L;
					cummulativeProbability.SilverDropCummulativeProbability = 0L;
				}
				lootParams.dropType = DropType.Gold;
				return GenerateLootEntry(lootParams);
			}
			if (randomInRange > num && randomInRange <= num + num2)
			{
				if (!ignoreCumulativeProbability)
				{
					cummulativeProbability.GoldDropCummulativeProbability = 0L;
					cummulativeProbability.SilverDropCummulativeProbability = 0L;
				}
				lootParams.dropType = DropType.Silver;
				return GenerateLootEntry(lootParams);
			}
		}
		if (!ignoreCumulativeProbability)
		{
			IncreaseCummulativeProbability(lootParams.eventType, dropEvent);
		}
		lootParams.dropType = DropType.Regular;
		return GenerateLootEntry(lootParams);
	}

	public bool DetermineComponentDrop(ref LootEntry lootEntry, DropEventDefinition.DropEventTag lootTag)
	{
		BuildingModel building = base.manager.Player.Camp.GetBuilding("Scavenger");
		if (building != null)
		{
			int level = building.Level;
			ComponentDropType componentDropType = base.manager.GameEconomyData.GetComponentDropType(level, lootTag, base.manager.Player.ActivityManager);
			if (componentDropType != null)
			{
				ModelRandom dedicatedRandom = GetDedicatedRandom("ComponentRandom");
				string dropComponentForRandomNumber = componentDropType.GetDropComponentForRandomNumber(dedicatedRandom.GetRandomInRange(1, 100));
				if (dropComponentForRandomNumber != null)
				{
					DropEquipmentsAndSurvivorsRaritiesDefinition dropRarityDefinition = base.manager.GameEconomyData.GetDropRarityDefinition(lootEntry.DropType, DropRewardType.Component, level, lootTag);
					int rewardedRarityLevel = 0;
					if (dropRarityDefinition != null)
					{
						rewardedRarityLevel = dropRarityDefinition.GetDropRarityForRandomNumber(dedicatedRandom.Next() * 100f);
					}
					lootEntry.ComponentType = dropComponentForRandomNumber;
					lootEntry.RewardedRarityLevel = rewardedRarityLevel;
					lootEntry.RewardedAmount = 1;
					return true;
				}
			}
		}
		return false;
	}

	public List<CurrencyType> GiveGoldShopDefinition(GoldShopDefinition shopDefinition)
	{
		List<CurrencyType> list = new List<CurrencyType>();
		for (int i = 0; i < shopDefinition.SubItems.Count; i++)
		{
			ComponentCrateItem componentCrateItem = shopDefinition.SubItems[i];
			for (int j = 0; j < componentCrateItem.Count; j++)
			{
				LootEntry lootEntry = new LootEntry();
				lootEntry.DropType = DropType.Regular;
				DetermineComponentDrop(ref lootEntry, DropEventDefinition.DropEventTag.ComponentCrate);
				if (componentCrateItem.Rarity != -1)
				{
					lootEntry.RewardedRarityLevel = componentCrateItem.Rarity;
				}
				if (!string.IsNullOrEmpty(componentCrateItem.Type))
				{
					lootEntry.ComponentType = componentCrateItem.Type;
				}
				CurrencyType componentCurrencyType = base.manager.Player.GetComponentCurrencyType(lootEntry.ComponentType, lootEntry.RewardedRarityLevel);
				if (componentCurrencyType == CurrencyType.None)
				{
					base.manager.Debug.LogError("Could not find currency type " + lootEntry.ComponentType + "/" + lootEntry.RewardedRarityLevel);
				}
				else
				{
					list.Add(componentCurrencyType);
				}
			}
		}
		Dictionary<CurrencyType, OverflowableAmount> dictionary = new Dictionary<CurrencyType, OverflowableAmount>();
		List<IReward> list2 = new List<IReward>();
		for (int k = 0; k < list.Count; k++)
		{
			base.manager.Debug.Log("Adding " + list[k]);
			base.manager.Player.GetCurrency(list[k]).Add(1);
			OverflowableAmount value = new OverflowableAmount
			{
				Amount = 0,
				Overflow = 0
			};
			bool num = dictionary.ContainsKey(list[k]);
			if (num)
			{
				value = dictionary[list[k]];
			}
			list2.Add(new RewardCurrency
			{
				CurrencyType = list[k],
				Amount = 1
			});
			value.Amount++;
			if (num)
			{
				dictionary[list[k]] = value;
			}
			else
			{
				dictionary.Add(list[k], value);
			}
		}
		Rewards lastReceivedComponents = new Rewards(list2);
		if (shopDefinition.Price > 0)
		{
			base.manager.Metrics.ResourceChangeObtainReason = "GoldShop";
		}
		else
		{
			base.manager.Metrics.ResourceChangeObtainReason = "GoldShopGift";
		}
		base.manager.Metrics.AddFind().AddResources(dictionary).AddComponentCrate(shopDefinition)
			.Send();
		LastReceivedComponents = list;
		if (base.manager.Player.GoldShopDefinitionManager.LastReceivedComponents == null)
		{
			base.manager.Player.GoldShopDefinitionManager.LastReceivedComponents = new Rewards();
		}
		base.manager.Player.GoldShopDefinitionManager.LastReceivedComponents = lastReceivedComponents;
		return list;
	}

	private LootEntry CreateAndStartLootEntry()
	{
		LootEntry lootEntry = new LootEntry();
		lootEntry.SetManager(base.manager);
		lootEntry.Initialize();
		lootEntry.Start();
		return lootEntry;
	}

	private LootEntry GenerateLootEntry(LootEntryGenParams lootParams)
	{
		LootEntry lootEntry = new LootEntry();
		lootEntry.SetManager(base.manager);
		lootEntry.Initialize();
		if (!OfflineManager.IsLoadDataManager)
		{
			DebugTWD.Log($"Ignore Save Loot in Inventory: {lootEntry.RewardedCurrency}({lootEntry.RewardedAmount}). Проверить!");
			DebugTWD.LogMycode("if (!OfflineManager.IsLoadDataManager)");
			lootEntry.Start();
		}
		lootEntry.DropEventDefinition = lootParams.dropEventDefinition;
		lootEntry.DropType = lootParams.dropType;
		lootEntry.TargetLevel = lootParams.targetLevel;
		lootEntry.RewardedEquipmentClass = SurvivorClass.None;
		FixedPoint number = lootParams.random.GetRandomInRange(1, 100);
		DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency = DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency;
		if (lootParams.forcedCurrency != DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency)
		{
			dropCurrency = lootParams.forcedCurrency;
		}
		else
		{
			DropCurrenciesProbabilitiesDefinition dropCurrenciesProbabilities = base.manager.GameEconomyData.GetDropCurrenciesProbabilities(lootParams.dropEventDefinition.EventType, lootParams.dropType, lootParams.tag, lootParams.targetLevel);
			if (dropCurrenciesProbabilities == null)
			{
				base.Debug.LogError("DropCurrenciesProbabilities not found for " + lootParams.dropEventDefinition.EventType.ToString() + " " + lootParams.dropType.ToString() + " " + lootParams.tag.ToString() + " " + lootParams.targetLevel);
				return null;
			}
			dropCurrency = dropCurrenciesProbabilities.GetDropCurrencyForRandomNumber(number);
		}
		if ((lootParams.dropEventDefinition.EventType == DropEventDefinition.DropEventType.IAPBonusGift || lootParams.dropEventDefinition.EventType == DropEventDefinition.DropEventType.Quiz) && dropCurrency == DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies && base.manager.Player.GetCurrency(CurrencyType.Supplies).IsFull)
		{
			dropCurrency = DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints;
		}
		bool flag = false;
		if (dropCurrency == DropCurrenciesProbabilitiesDefinition.DropCurrency.Component && base.manager.Player.Camp.GetBuildingLevel("Scavenger") <= 0)
		{
			flag = true;
			dropCurrency = ((lootParams.random.Next(2) != 0) ? DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor : DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon);
		}
		lootEntry.DropCurrencyType = dropCurrency;
		switch (dropCurrency)
		{
		case DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon:
		case DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor:
		case DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor:
		{
			DropRewardType rewardType = DropRewardType.Armor;
			switch (dropCurrency)
			{
			case DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor:
				rewardType = DropRewardType.Survivor;
				break;
			case DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon:
				rewardType = DropRewardType.Weapon;
				break;
			}
			DropEventDefinition.DropEventTag dropEventTag = DropEventDefinition.DropEventTag.None;
			if (lootParams.tag == DropEventDefinition.DropEventTag.TradeCrateGolden || lootParams.tag == DropEventDefinition.DropEventTag.TradeCrateGearHigh || lootParams.tag == DropEventDefinition.DropEventTag.TradeCrateGearMid || lootParams.tag == DropEventDefinition.DropEventTag.TradeCrateGearLow || lootParams.tag == DropEventDefinition.DropEventTag.ChallengeCrateGold || lootParams.tag == DropEventDefinition.DropEventTag.ChallengeCrateSilver)
			{
				dropEventTag = lootParams.tag;
			}
			DropEventDefinition.DropEventTag tag = dropEventTag;
			if (flag)
			{
				tag = DropEventDefinition.DropEventTag.WasComponent;
			}
			DropEquipmentsAndSurvivorsRaritiesDefinition dropRarityDefinition = base.manager.GameEconomyData.GetDropRarityDefinition(lootParams.dropType, rewardType, lootParams.targetLevel, tag, lootParams.dropEventDefinition.DropContext);
			DropEquipmentsAndSurvivorsStartingLevelDefinition dropStartingLevelDefinition = base.manager.GameEconomyData.GetDropStartingLevelDefinition(lootParams.dropType, rewardType, lootParams.targetLevel, dropEventTag);
			if (dropRarityDefinition != null)
			{
				FixedPoint number2 = lootParams.random.Next() * 100f;
				lootEntry.RewardedRarityLevel = dropRarityDefinition.GetDropRarityForRandomNumber(number2);
				if (dropStartingLevelDefinition != null)
				{
					List<int> startingLevelForRarity = dropStartingLevelDefinition.GetStartingLevelForRarity(lootEntry.RewardedRarityLevel);
					int num3 = startingLevelForRarity[0];
					int num4 = ((startingLevelForRarity.Count > 1) ? startingLevelForRarity[1] : num3);
					DropCurrencyTraitModifier traitModifier2 = lootParams.GetTraitModifier(DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon);
					if (traitModifier2 != null && traitModifier2.Modifier > 0.0)
					{
						bool wasModified = false;
						lootEntry.RewardedStartingLevel = GetHigherLevelStartingLevel(num3, num4, lootParams.random, traitModifier2.Modifier * 100.0, out wasModified);
						if (wasModified)
						{
							lootEntry.ModifiedByTrait = traitModifier2.TraitId;
						}
					}
					else
					{
						lootEntry.RewardedStartingLevel = lootParams.random.GetRandomInRange(num3, num4);
					}
				}
				else
				{
					base.Debug.LogError("Could not find LevelDefinition for dropType: " + lootParams.dropType.ToString() + " rewardType: " + rewardType.ToString() + " targetLevel: " + lootParams.targetLevel + " tagToUse: " + dropEventTag);
				}
			}
			else
			{
				base.Debug.LogError("Could not find RaritiesDefinition for dropType: " + lootParams.dropType.ToString() + " rewardType: " + rewardType.ToString() + " targetLevel: " + lootParams.targetLevel + " tagToUse: " + dropEventTag.ToString() + " DropContext: " + lootParams.dropEventDefinition.DropContext);
			}
			break;
		}
		case DropCurrenciesProbabilitiesDefinition.DropCurrency.Component:
			DetermineComponentDrop(ref lootEntry, lootParams.tag);
			break;
		default:
		{
			Dictionary<DropCurrenciesProbabilitiesDefinition.DropCurrency, CurrencyType> dictionary = new Dictionary<DropCurrenciesProbabilitiesDefinition.DropCurrency, CurrencyType>();
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies] = CurrencyType.Supplies;
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints] = CurrencyType.SurvivalPoints;
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.Diamonds] = CurrencyType.Diamonds;
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.Phone] = CurrencyType.Phone;
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.ReplayToken] = CurrencyType.ReplayToken;
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.Inhabitant] = CurrencyType.Inhabitants;
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.GvGGas] = CurrencyType.GvGGas;
			dictionary[DropCurrenciesProbabilitiesDefinition.DropCurrency.BattlePass] = CurrencyType.BattlePass;
			CurrencyType currencyType = CurrencyType.None;
			if (dictionary.ContainsKey(dropCurrency))
			{
				currencyType = (lootEntry.RewardedCurrency = dictionary[dropCurrency]);
				int num = Math.Max(1, lootParams.dropEventDefinition.ControlLevelOffset);
				bool flag2 = base.manager.Player.ActivityManager.IsActivityOpen(ActivityType.TomatoMonday);
				DropCurrenciesAmountsDefinition dropCurrencyAmountDefinition = base.manager.GameEconomyData.GetDropCurrencyAmountDefinition(lootParams.dropType, currencyType, lootParams.targetLevel + num, lootParams.tag);
				int val = 1;
				int val2 = 1;
				FixedPoint fixedPoint = (FixedPoint)lootParams.dropEventDefinition.CurrenciesAmountPercentageMultiplier / (FixedPoint)100.0;
				if (dropCurrencyAmountDefinition != null)
				{
					val = (int)((flag2 ? dropCurrencyAmountDefinition.EventMinAmount : dropCurrencyAmountDefinition.MinAmount) * fixedPoint);
					val2 = (int)((flag2 ? dropCurrencyAmountDefinition.EventMaxAmount : dropCurrencyAmountDefinition.MaxAmount) * fixedPoint);
				}
				val = Math.Max(val, 1);
				val2 = Math.Max(val2, 1);
				int num2 = lootParams.random.GetRandomInRange(val, val2);
				DropCurrencyTraitModifier traitModifier = lootParams.GetTraitModifier(dropCurrency);
				if (traitModifier != null)
				{
					num2 = (int)(num2 * (1.0 + traitModifier.Modifier));
					lootEntry.ModifiedByTrait = traitModifier.TraitId;
				}
				ScavengeRewardCurrencyMultiplier scavengeRewardCurrencyMultiplier = base.manager.GameEconomyData.GetScavengeRewardCurrencyMultiplier(currencyType, lootParams.context);
				if (scavengeRewardCurrencyMultiplier != null)
				{
					num2 = (int)(num2 * scavengeRewardCurrencyMultiplier.Multiplier);
				}
				lootEntry.RewardedAmount = base.manager.GameEconomyData.GetRoundedValueForCurrency(currencyType, num2);
				lootEntry.ActualAmountAdded = lootEntry.RewardedAmount;
			}
			break;
			}
		}
		return lootEntry;
	}

	private int GetHigherLevelStartingLevel(int minStartingLevel, int maxStartingLevel, ModelRandom random, FixedPoint higherChance, out bool wasModified)
	{
		int val = 0;
		wasModified = false;
		int num = minStartingLevel;
		if (higherChance > 0.0)
		{
			FixedPoint fixedPoint = random.Next() * 100f;
			if (fixedPoint <= higherChance)
			{
				int num2 = num + 1;
				if (num != maxStartingLevel && num2 <= maxStartingLevel)
				{
					wasModified = true;
				}
				num = Math.Min(num2, maxStartingLevel);
			}
			int num3 = maxStartingLevel + 1 - num;
			FixedPoint fixedPoint2 = 100f / (float)num3;
			fixedPoint = random.Next() * 100f;
			for (int i = 1; i <= num3; i++)
			{
				if (fixedPoint <= i * fixedPoint2)
				{
					val = num + (i - 1);
					break;
				}
			}
		}
		return Math.Min(Math.Max(val, num), maxStartingLevel);
	}

	public bool CanOpenLootBox()
	{
		int num = 0;
		for (int i = 0; i < Loots.Count; i++)
		{
			if (Loots[i] != null && !Loots[i].Opened)
			{
				num++;
			}
		}
		if (num > 0)
		{
			return AvailableKeys > 0;
		}
		return false;
	}

	public LootEntry OpenNextLoot(int boxIndex)
	{
		LootEntry lootEntry = null;
		for (int i = 0; i < ((Loots != null) ? Loots.Count : 0); i++)
		{
			if (!Loots[i].Opened)
			{
				lootEntry = Loots[i];
				break;
			}
		}
		if (lootEntry != null)
		{
			lootEntry.BoxIndex = boxIndex;
			GiveLoot(lootEntry);
			lootEntry.Opened = true;
			AvailableKeys--;
		}
		return lootEntry;
	}

	public LootEntry GiveForcedSurvivor(SurvivorClass survivorClass = SurvivorClass.None, int rarityLevel = 0)
	{
		LootEntry forcedCurrencyLoot = GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor, CurrencyType.Survivor, 1, rarityLevel);
		if (forcedCurrencyLoot.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
		{
			SurvivorModel generatedSurvivor = base.manager.Player.SurvivorContainer.CreateRandomSurvivor(0, forcedCurrencyLoot.RewardedStartingLevel, forcedCurrencyLoot.RewardedStartingLevel, forcedCurrencyLoot.RewardedRarityLevel, survivorClass);
			forcedCurrencyLoot.GeneratedSurvivor = generatedSurvivor;
		}
		return forcedCurrencyLoot;
	}

	public LootEntry GiveForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency, CurrencyType currency, int amount, int rarity = 0)
	{
		LootEntry forcedCurrencyLoot = GetForcedCurrencyLoot(dropCurrency, currency, amount, rarity);
		GiveLoot(forcedCurrencyLoot);
		return forcedCurrencyLoot;
	}

	public LootEntry GetForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency, CurrencyType currency, int amount, int rarity = 0, DropType dropType = DropType.Regular, int startLevel = 1)
	{
		LootEntry lootEntry = new LootEntry();
		lootEntry.SetManager(base.manager);
		lootEntry.Initialize();
		if (!OfflineManager.IsLoadDataManager)
		{
			DebugTWD.Log($"GetForcedCurrencyLoot Start() ignore: {lootEntry.RewardedCurrency}({lootEntry.RewardedAmount}). Проверить!");
			lootEntry.Start();
		}
		DropEventDefinition.DropEventTag tag = DropEventDefinition.DropEventTag.None;
		DropEventDefinition dropEvent = base.manager.GameEconomyData.GetDropEvent(DropEventDefinition.DropEventType.MissionScavenge, DropEventDefinition.DropEventContext.Normal, tag);
		lootEntry.DropEventDefinition = dropEvent;
		lootEntry.RewardedStartingLevel = startLevel;
		lootEntry.RewardedRarityLevel = rarity;
		lootEntry.DropCurrencyType = dropCurrency;
		lootEntry.RewardedCurrency = currency;
		lootEntry.RewardedAmount = amount;
		lootEntry.ActualAmountAdded = amount;
		lootEntry.DropType = dropType;
		lootEntry.RewardedEquipmentClass = SurvivorClass.None;

		return lootEntry;
	}

	public void GiveLoot(LootEntry lootEntry, PhoneCallDefinition phoneCallDefinition = null, SurvivorClass forceSurvivorClass = SurvivorClass.None, bool allowLockedClasses = false)
	{
		ModelRandom modelRandom = lootEntry.Random;
		if (modelRandom == null)
		{
			modelRandom = base.manager.Player.PlayerRandom;
		}
		if (lootEntry.IsComponent())
		{
			CurrencyModel componentCurrency = base.manager.Player.GetComponentCurrency(lootEntry.ComponentType, lootEntry.RewardedRarityLevel);
			if (componentCurrency != null)
			{
				componentCurrency.Add(lootEntry.RewardedAmount);
				lootEntry.ActualAmountAdded = componentCurrency.LastAdded;
				lootEntry.RewardedCurrency = componentCurrency.Type;
			}
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor || lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon)
		{
			EquipmentItemModel equipmentItemModel = base.manager.Player.Equipment.GenerateRandomEquipmentFromMission(lootEntry.RewardedStartingLevel, lootEntry.RewardedStartingLevel, lootEntry.RewardedRarityLevel, lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor, lootEntry.RewardedEquipmentClass, modelRandom);
			if (equipmentItemModel != null)
			{
				EquipmentSource source = EquipmentSource.Unknown;
				switch (lootEntry.DropEventDefinition.EventType)
				{
				case DropEventDefinition.DropEventType.MissionScavenge:
					source = EquipmentSource.MissionLoot;
					break;
				case DropEventDefinition.DropEventType.MissionRescue:
					source = EquipmentSource.MissionLoot;
					break;
				case DropEventDefinition.DropEventType.VideoAd:
					source = EquipmentSource.Cinema;
					break;
				case DropEventDefinition.DropEventType.GuildGift:
					source = EquipmentSource.GuildGift;
					break;
				case DropEventDefinition.DropEventType.TradeCrate:
					source = EquipmentSource.TradeGoodsShop;
					break;
				case DropEventDefinition.DropEventType.Campaign:
					source = EquipmentSource.Campaign;
					break;
				case DropEventDefinition.DropEventType.GuildShop:
					source = EquipmentSource.GuildShop;
					break;
				}
				base.manager.Player.Equipment.AddEquipment(equipmentItemModel, source);
				lootEntry.RewardedEquipment = equipmentItemModel;
				lootEntry.GeneratedEquipment = null;
			}
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Consumable)
		{
			EquipmentSource source2 = EquipmentSource.Unknown;
			switch (lootEntry.DropEventDefinition.EventType)
			{
			case DropEventDefinition.DropEventType.MissionScavenge:
				source2 = EquipmentSource.MissionLoot;
				break;
			case DropEventDefinition.DropEventType.MissionRescue:
				source2 = EquipmentSource.MissionLoot;
				break;
			case DropEventDefinition.DropEventType.VideoAd:
				source2 = EquipmentSource.Cinema;
				break;
			case DropEventDefinition.DropEventType.GuildGift:
				source2 = EquipmentSource.GuildGift;
				break;
			case DropEventDefinition.DropEventType.TradeCrate:
				source2 = EquipmentSource.TradeGoodsShop;
				break;
			case DropEventDefinition.DropEventType.Campaign:
				source2 = EquipmentSource.Campaign;
				break;
			case DropEventDefinition.DropEventType.GuildShop:
				source2 = EquipmentSource.GuildShop;
				break;
			}
			for (int i = 0; i < lootEntry.RewardedAmount; i++)
			{
				base.manager.Player.Equipment.AddEquipment(lootEntry.GeneratedEquipment, source2);
			}
			lootEntry.RewardedEquipment = lootEntry.GeneratedEquipment;
			lootEntry.GeneratedEquipment = null;
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
		{
			SurvivorModel generatedSurvivor = base.manager.Player.SurvivorContainer.CreateRandomSurvivor(0, lootEntry.RewardedStartingLevel, lootEntry.RewardedStartingLevel, lootEntry.RewardedRarityLevel, forceSurvivorClass, null, 1, 0, lootEntry.ExcludeSurvivorClasses, includeGachaOnly: true, modelRandom, SurvivorClass.None, 0, allowLockedClasses);
			lootEntry.GeneratedSurvivor = generatedSurvivor;
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
		{
			RewardHeroToken(lootEntry, phoneCallDefinition);
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
		{
			RewardClassToken(lootEntry);
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Avatars)
		{
			RewardAvatars(lootEntry);
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ChallengeSkipToken)
		{
			base.manager.Player.WeeklyChallenge.PendingSkipTokens += lootEntry.ChallengeSkipToken;
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.EquipToken)
		{
			base.manager.Player.EquipTokenContainer.AddEquipToken(lootEntry.EquipTokenId, lootEntry.EquipTokenAmount);
		}
		else
		{
			CurrencyModel currency = base.manager.Player.GetCurrency(lootEntry.RewardedCurrency);
			lootEntry.RewardedAmount *= lootEntry.ChallengeRoundCompletionRewardMultiplier;
			lootEntry.ChallengeRoundCompletionRewardMultiplier = 1;
			if (base.manager.GameEconomyData.IsSpeedUpTokenCurrencyType(lootEntry.RewardedCurrency))
			{
				currency.AddWithOverflowToDiamonds(lootEntry.RewardedAmount);
			}
			else
			{
				currency.Add(lootEntry.RewardedAmount, lootEntry.RewardedCurrency == CurrencyType.ReplayToken || lootEntry.CanOverflowMax);
			}
			lootEntry.ActualAmountAdded = currency.LastAdded;
			if (currency.LastAdded != lootEntry.RewardedAmount)
			{
				lootEntry.RewardedAmount = (int)(lootEntry.RewardedAmount * currency.AddMultiplier);
			}
		}
	}

	public LootEntry GetRadioPhoneLoot(DropType dropType, PhoneCallDefinition phoneCallDefinition, List<SurvivorClass> ExcludeSurvivorClasses = null, int forceRarity = -1, int probabilityOverride = 0)
	{
		int targetLevel = 0;
		if (base.manager.CampModel != null)
		{
			BuildingModel building = base.manager.CampModel.GetBuilding("RadioTent");
			if (building != null)
			{
				targetLevel = building.Level;
			}
		}
		ModelRandom dedicatedRandom = GetDedicatedRandom("RadioPhone" + dropType);
		bool enabled = base.manager.GameEconomyData.GetFeature("AllowLockedClassesOnRadio").Enabled;
		LootEntry lootEntry = null;
		SurvivorClass forceSurvivorClass = SurvivorClass.None;
		if (phoneCallDefinition != null && phoneCallDefinition.InitialProbabilityPercentage > 0)
		{
			int num = ((probabilityOverride > 0) ? probabilityOverride : phoneCallDefinition.InitialProbabilityPercentage);
			SpecialPhoneCallState specialPhoneCallState = GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
			if (specialPhoneCallState != null)
			{
				num = ((probabilityOverride > 0) ? probabilityOverride : specialPhoneCallState.CumulativeProbability);
			}
			else
			{
				AddSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition);
			}
			if (dedicatedRandom.GetRandomInRange(1, 100) <= num)
			{
				CurrencyType currencyType = CurrencyType.None;
				CurrencyType[] parsedCurrencyTypeValues = phoneCallDefinition.GetParsedCurrencyTypeValues();
				if (parsedCurrencyTypeValues.Length >= 1)
				{
					if (parsedCurrencyTypeValues.Length == 1)
					{
						currencyType = parsedCurrencyTypeValues[0];
					}
					else
					{
						int num2 = 0;
						int[] parsedCurrencyTypeDistributionValues = phoneCallDefinition.GetParsedCurrencyTypeDistributionValues();
						for (int i = 0; i < parsedCurrencyTypeValues.Length; i++)
						{
							num2 += parsedCurrencyTypeDistributionValues[i];
						}
						int randomInRange = dedicatedRandom.GetRandomInRange(1, num2);
						int num3 = 0;
						for (int j = 0; j < parsedCurrencyTypeValues.Length; j++)
						{
							num3 += parsedCurrencyTypeDistributionValues[j];
							if (num3 >= randomInRange)
							{
								currencyType = parsedCurrencyTypeValues[j];
								break;
							}
						}
					}
				}
				DropCurrenciesProbabilitiesDefinition.DropCurrency forcedCurrency = ((currencyType != CurrencyType.None || phoneCallDefinition.HeroGuaranteed) ? DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken : DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor);
				lootEntry = base.manager.Player.LootManager.ShuffleOneLoot(new LootEntryGenParams
				{
					eventType = DropEventDefinition.DropEventType.RadioPhone,
					targetLevel = targetLevel,
					context = DropEventDefinition.DropEventContext.Normal,
					dropType = dropType,
					random = dedicatedRandom,
					forcedCurrency = forcedCurrency
				});
				lootEntry.DropType = phoneCallDefinition.DropType;
				if (currencyType != CurrencyType.None || phoneCallDefinition.HeroGuaranteed)
				{
					lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken;
					if (currencyType != CurrencyType.None)
					{
						lootEntry.RewardedCurrency = currencyType;
					}
				}
				else
				{
					lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor;
					forceSurvivorClass = phoneCallDefinition.SurvivorClass;
				}
			}
		}
		if (lootEntry == null)
		{
			lootEntry = base.manager.Player.LootManager.ShuffleOneLootWithoutTag(new LootEntryGenParams
			{
				eventType = DropEventDefinition.DropEventType.RadioPhone,
				targetLevel = targetLevel,
				context = DropEventDefinition.DropEventContext.Normal,
				dropType = dropType,
				random = dedicatedRandom
			});
			if (phoneCallDefinition != null)
			{
				lootEntry.DropType = phoneCallDefinition.DropType;
			}
		}
		lootEntry.ExcludeSurvivorClasses = ExcludeSurvivorClasses;
		lootEntry.Random = dedicatedRandom;
		if (forceRarity != -1)
		{
			lootEntry.RewardedRarityLevel = forceRarity;
		}
		GiveLoot(lootEntry, phoneCallDefinition, forceSurvivorClass, enabled);
		return lootEntry;
	}

	public SurvivorModel GetGeneratedSurvivor()
	{
		for (int i = 0; i < Loots.Count; i++)
		{
			LootEntry lootEntry = Loots[i];
			if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
			{
				return base.manager.Player.SurvivorContainer.CreateRandomSurvivor(0, lootEntry.RewardedStartingLevel, lootEntry.RewardedStartingLevel, lootEntry.RewardedRarityLevel);
			}
		}
		return null;
	}

	private LootCummulativeProbabilityEntry GetCummulativeProbability(DropEventDefinition.DropEventType dropEventType)
	{
		for (int i = 0; i < DropCummulativeProbabilities.Count; i++)
		{
			LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry = DropCummulativeProbabilities[i];
			if (lootCummulativeProbabilityEntry.EventType == dropEventType)
			{
				return lootCummulativeProbabilityEntry;
			}
		}
		LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry2 = new LootCummulativeProbabilityEntry();
		lootCummulativeProbabilityEntry2.EventType = dropEventType;
		DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry2);
		return lootCummulativeProbabilityEntry2;
	}

	public List<LootEntry> GetOpenedLoots()
	{
		List<LootEntry> list = new List<LootEntry>();
		for (int i = 0; i < ((Loots != null) ? Loots.Count : 0); i++)
		{
			LootEntry lootEntry = Loots[i];
			if (lootEntry != null && lootEntry.Opened)
			{
				list.Add(lootEntry);
			}
		}
		return list;
	}

	public int GetLootsLeftToOpenCount()
	{
		if (Loots == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < Loots.Count; i++)
		{
			if (Loots[i] != null && !Loots[i].Opened)
			{
				num++;
			}
		}
		return num;
	}

	public Cashier GetCashierForTradeCrate(TradeSlotInfo tradeSlotDefinition)
	{
		if (tradeSlotDefinition != null)
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrate);
			if (tradeSlotDefinition.SlotDefinition.PriceCategory == PriceCategory.Discount)
			{
				cashierItem.SetCost(tradeSlotDefinition.CurrentTradeDefinition.PriceDiscountType, tradeSlotDefinition.CurrentTradeDefinition.PriceDiscountAmount);
			}
			else
			{
				cashierItem.SetCost(tradeSlotDefinition.CurrentTradeDefinition.PriceNormalType, tradeSlotDefinition.CurrentTradeDefinition.PriceNormalAmount);
			}
			cashier.AddItem(cashierItem);
			return cashier;
		}
		return null;
	}

	public Cashier GetCashierForTradeSlot(TradeSlotInfo tradeSlotDefinition)
	{
		if (tradeSlotDefinition != null)
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.TradeCrateSlot);
			cashierItem.SetCost(tradeSlotDefinition.SlotDefinition.CurrencyUnlock, tradeSlotDefinition.SlotDefinition.CurrencyUnlockAmount);
			cashier.AddItem(cashierItem);
			return cashier;
		}
		return null;
	}

	public LootEntry OpenPendingTradeCrateLoot()
	{
		if (PendingTradeCrates != null && PendingTradeCrates.Count > 0)
		{
			LootEntry lootEntry = PendingTradeCrates[0];
			GiveLoot(lootEntry);
			if (base.manager != null && base.manager.Player != null && lootEntry != null)
			{
				if (lootEntry.Type == LootEntryType.TradeCrate)
				{
					base.manager.Metrics.AddFind().AddLoot(lootEntry).AddTradeCrate()
						.Send();
				}
				else
				{
					base.manager.Metrics.AddFind().AddStaticReward().AddLoot(lootEntry)
						.Send();
				}
			}
			PendingTradeCrates.Remove(lootEntry);
			return lootEntry;
		}
		return null;
	}

	public int GetEquipmentStartingLevel(int levelOffset, SurvivorClass survivorClass = SurvivorClass.None)
	{
		int num = base.manager.Player.SurvivorContainer.GetHighestLevelOfSurvivorClass(survivorClass);
		if (num == 0)
		{
			num = base.manager.Player.SurvivorContainer.GetHighestLevelSurvivor();
		}
		int num2 = num + levelOffset;
		int maximumEquipmentLevel = base.manager.Player.gameEconomyData.GetMaximumEquipmentLevel();
		if (maximumEquipmentLevel > 0)
		{
			num2 = Math.Min(num2, maximumEquipmentLevel);
		}
		return num2;
	}

	public LootEntry CreateRandomEquipmentLoot(RewardRandomEquipment reward)
	{
		LootEntry lootEntry = new LootEntry();
		lootEntry.SetManager(base.manager);
		lootEntry.Initialize();
		lootEntry.Start();
		if (reward.Category == EquipmentCategory.Armor)
		{
			lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor;
		}
		else
		{
			lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon;
		}
		lootEntry.RewardedRarityLevel = reward.RarityLevel;
		lootEntry.RewardedEquipmentClass = reward.SurvivorClass;
		lootEntry.RewardedStartingLevel = GetEquipmentStartingLevel(reward.StartingLevelOffset);
		lootEntry.DropType = DropType.Regular;
		return lootEntry;
	}

	public LootEntry CreateConsumablesLoot(RewardEquipment reward, DropType dropType)
	{
		LootEntry lootEntry = new LootEntry();
		lootEntry.SetManager(base.manager);
		lootEntry.Initialize();
		lootEntry.Start();
		lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.Consumable;
		lootEntry.RewardedAmount = reward.Amount;
		lootEntry.DropType = dropType;
		lootEntry.GeneratedEquipment = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(reward.EquipmentId, reward.RarityLevel, reward.StartingLevel);
		return lootEntry;
	}

	public LootEntry CreateAvatarsLootEntry(IReward reward, DropType dropType)
	{
		LootEntry lootEntry = null;
		RewardAvatars rewardAvatars = (RewardAvatars)reward;
		if (rewardAvatars != null)
		{
			lootEntry = new LootEntry();
			lootEntry.SetManager(base.manager);
			lootEntry.Initialize();
			lootEntry.Start();
			lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.Avatars;
			lootEntry.DropType = dropType;
			lootEntry.Type = LootEntryType.TradeCrate;
			lootEntry.TargetLevel = base.manager.Player.Level;
			lootEntry.RewardedEquipmentClass = SurvivorClass.None;
			lootEntry.IconIndex = rewardAvatars.Avatar;
			lootEntry.BorderIndex = rewardAvatars.Border;
			lootEntry.ColorIndex = rewardAvatars.Color;
		}
		return lootEntry;
	}

	public LootEntry CreateCurrencyLoot(IReward reward, DropType dropType, DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency)
	{
		LootEntry lootEntry = null;
		if (reward != null)
		{
			if (reward.Type == RewardType.Currency && reward is RewardCurrency)
			{
				RewardCurrency rewardCurrency = (RewardCurrency)reward;
				if (rewardCurrency.Amount > 0)
				{
					lootEntry = CreateCurrencyLoot(rewardCurrency.CurrencyType, rewardCurrency.Amount, dropType, dropCurrency);
				}
				else
				{
					base.Debug.LogError("LootModelManager::CreateCurrencyLoot() Cannot create LootEntries with amount -1. IReward type: " + reward.Type.ToString() + ", currency: " + rewardCurrency.CurrencyType);
				}
			}
			else if (reward.Type == RewardType.RewardSkipChallange)
			{
				RewardSkipChallange rewardSkipChallange = (RewardSkipChallange)reward;
				lootEntry = CreateCurrencyLoot(CurrencyType.None, 0, dropType, DropCurrenciesProbabilitiesDefinition.DropCurrency.ChallengeSkipToken);
				lootEntry.ChallengeSkipToken = rewardSkipChallange.Amount;
			}
			else if (reward.Type == RewardType.EquipToken)
			{
				RewardEquipToken rewardEquipToken = (RewardEquipToken)reward;
				lootEntry = CreateCurrencyLoot(CurrencyType.None, 0, dropType, DropCurrenciesProbabilitiesDefinition.DropCurrency.EquipToken);
				lootEntry.EquipTokenId = rewardEquipToken.EquipTokenId;
				lootEntry.EquipTokenAmount = rewardEquipToken.RewardAmount;
			}
			else if (reward.Type == RewardType.RemoldSkill)
			{
				RewardRemoldSkill rewardRemoldSkill = (RewardRemoldSkill)reward;
				lootEntry = CreateCurrencyLoot(CurrencyType.None, 0, dropType, DropCurrenciesProbabilitiesDefinition.DropCurrency.RemoldSkill);
				lootEntry.SpRemoldSkillType = rewardRemoldSkill.SpRemoldSkillType;
			}
			else
			{
				base.Debug.LogError("LootModelManager::CreateCurrencyLoot() Only supports Currencies. Cant create LootEntry of unsupported IReward type: " + reward.Type.ToString() + ", object: " + reward);
			}
		}
		return lootEntry;
	}

	public LootEntry CreateCurrencyLoot(CurrencyType currency, int amount, DropType dropType, DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency)
	{
		LootEntry lootEntry = new LootEntry();
		lootEntry.SetManager(base.manager);
		lootEntry.Initialize();
		lootEntry.Start();
		lootEntry.DropType = dropType;
		lootEntry.Type = LootEntryType.TradeCrate;
		lootEntry.TargetLevel = base.manager.Player.Level;
		lootEntry.DropCurrencyType = dropCurrency;
		lootEntry.RewardedCurrency = currency;
		lootEntry.RewardedAmount = amount;
		lootEntry.ActualAmountAdded = amount;
		lootEntry.RewardedEquipmentClass = SurvivorClass.None;
		return lootEntry;
	}

	public LootEntry AddTraitBonusReward(string traitId)
	{
		return null;
	}

	public LootEntry AddTradeCrateLoot(string lootId)
	{
		LootEntry lootEntry = CreateTradeCrateLoot(lootId);
		if (PendingTradeCrates == null)
		{
			PendingTradeCrates = new ModelList<LootEntry>();
			PendingTradeCrates.SetManager(base.manager);
		}
		if (lootEntry != null)
		{
			PendingTradeCrates.Add(lootEntry);
		}
		return lootEntry;
	}

	public LootEntry CreateTradeCrateLoot(string lootId, DropEventDefinition.DropEventType dropEventType = DropEventDefinition.DropEventType.TradeCrate, bool ignoreCummulativeProbability = false, string dedicatedRandomOverride = "")
	{
		if (lootId != null)
		{
			LootEntry lootEntry = null;
			ModelRandom dedicatedRandom = GetDedicatedRandom((!string.IsNullOrEmpty(dedicatedRandomOverride)) ? dedicatedRandomOverride : lootId);
			DropEventDefinition.DropEventTag tag = (DropEventDefinition.DropEventTag)Enum.Parse(typeof(DropEventDefinition.DropEventTag), lootId);
			lootEntry = ShuffleOneLoot(new LootEntryGenParams
			{
				eventType = dropEventType,
				targetLevel = base.manager.Player.Level,
				context = DropEventDefinition.DropEventContext.Normal,
				tag = tag,
				random = dedicatedRandom
			}, ignoreCummulativeProbability);
			if (lootEntry != null)
			{
				lootEntry.Type = LootEntryType.TradeCrate;
				lootEntry.GeneratorIdentifier = tag.ToString();
				lootEntry.Random = dedicatedRandom;
				return lootEntry;
			}
		}
		return null;
	}

	private void IncreaseCummulativeProbability(DropEventDefinition.DropEventType dropEventType, DropEventDefinition dropEventDefinition)
	{
		for (int i = 0; i < DropCummulativeProbabilities.Count; i++)
		{
			LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry = DropCummulativeProbabilities[i];
			if (lootCummulativeProbabilityEntry.EventType != dropEventType)
			{
				continue;
			}
			bool flag = lootCummulativeProbabilityEntry.SilverDropCummulativeProbability < 100.0;
			lootCummulativeProbabilityEntry.SilverDropCummulativeProbability += (FixedPoint)dropEventDefinition.SilverDropProbabilityIncrement;
			if (lootCummulativeProbabilityEntry.SilverDropCummulativeProbability > 100.0)
			{
				if (flag)
				{
					lootCummulativeProbabilityEntry.SilverDropCummulativeProbability = 100.0;
				}
				else
				{
					lootCummulativeProbabilityEntry.SilverDropCummulativeProbability -= (FixedPoint)100.0;
				}
			}
			bool flag2 = lootCummulativeProbabilityEntry.GoldDropCummulativeProbability < 100.0;
			lootCummulativeProbabilityEntry.GoldDropCummulativeProbability += (FixedPoint)dropEventDefinition.GoldDropProbabilityIncrement;
			if (lootCummulativeProbabilityEntry.GoldDropCummulativeProbability > 100.0)
			{
				if (flag2)
				{
					lootCummulativeProbabilityEntry.GoldDropCummulativeProbability = 100.0;
				}
				else
				{
					lootCummulativeProbabilityEntry.GoldDropCummulativeProbability -= (FixedPoint)100.0;
				}
			}
			break;
		}
	}

	public void RewardHeroToken(LootEntry lootEntry, PhoneCallDefinition phoneCallDefinition = null)
	{
		GameEconomyData gameEconomyData = base.manager.GameEconomyData;
		DropEventDefinition dropEventDefinition = lootEntry.DropEventDefinition;
		if (dropEventDefinition != null && dropEventDefinition.Tag == DropEventDefinition.DropEventTag.None)
		{
			int buildingLevel = base.manager.Player.Camp.GetBuildingLevel("RadioTent");
			SurvivorToken heroTokenForGatcha = gameEconomyData.GetHeroTokenForGatcha(lootEntry.DropEventDefinition.EventType, lootEntry.DropType, DropEventDefinition.DropEventTag.None, buildingLevel, lootEntry.RewardedCurrency, GetDedicatedRandom("HeroToken"), -1, phoneCallDefinition);
			if (heroTokenForGatcha != null && heroTokenForGatcha.Type != CurrencyType.None)
			{
				lootEntry.RewardedCurrency = heroTokenForGatcha.Type;
				lootEntry.RewardedAmount = heroTokenForGatcha.Amount;
				lootEntry.ActualAmountAdded = lootEntry.RewardedAmount;
				lootEntry.RewardedRarityLevel = heroTokenForGatcha.AmountRarityLevel;
			}
			else
			{
				base.manager.Debug.LogError("LootModelManager: rewarding hero token '" + ((heroTokenForGatcha == null) ? "null" : heroTokenForGatcha.Type.ToString()) + "'. Please check the token distribution in GED.");
			}
		}
		else if (dropEventDefinition != null && dropEventDefinition.Tag == DropEventDefinition.DropEventTag.TokenCrate)
		{
			int buildingLevel2 = base.manager.Player.Camp.GetBuildingLevel("RadioTent");
			ModelRandom dedicatedRandom = GetDedicatedRandom("ChallengeToken");
			SurvivorToken heroTokenForChallenge = gameEconomyData.GetHeroTokenForChallenge(lootEntry.DropEventDefinition.EventType, lootEntry.DropType, lootEntry.DropEventDefinition.Tag, lootEntry.DropCurrencyType, buildingLevel2, dedicatedRandom);
			if (heroTokenForChallenge != null && heroTokenForChallenge.Type != CurrencyType.None)
			{
				base.manager.Player.GetCurrency(heroTokenForChallenge.Type).Add(heroTokenForChallenge.Amount);
				lootEntry.RewardedCurrency = heroTokenForChallenge.Type;
				lootEntry.RewardedAmount = heroTokenForChallenge.Amount;
				lootEntry.ActualAmountAdded = heroTokenForChallenge.Amount;
				lootEntry.RewardedRarityLevel = heroTokenForChallenge.AmountRarityLevel;
			}
		}
		else
		{
			if (dropEventDefinition == null || dropEventDefinition.Tag != DropEventDefinition.DropEventTag.QuestChestHeroToken)
			{
				return;
			}
			int buildingLevel3 = base.manager.Player.Camp.GetBuildingLevel("RadioTent");
			ModelRandom dedicatedRandom2 = GetDedicatedRandom("DailyQuestChestTokens");
			SurvivorToken heroTokenTypeAndAmount = gameEconomyData.GetHeroTokenTypeAndAmount(lootEntry.DropEventDefinition.EventType, lootEntry.DropType, lootEntry.DropEventDefinition.Tag, lootEntry.DropCurrencyType, buildingLevel3, dedicatedRandom2);
			if (heroTokenTypeAndAmount != null && heroTokenTypeAndAmount.Type != CurrencyType.None)
			{
				if (heroTokenTypeAndAmount.Amount == 0)
				{
					base.manager.Debug.LogError($"Did not get proper amount for hero tokens when looking up with EventType {lootEntry.DropEventDefinition.EventType.ToString()}, DropType {lootEntry.DropType.ToString()}, Tag {lootEntry.DropEventDefinition.Tag.ToString()}, DropCurrencyType {lootEntry.DropCurrencyType.ToString()} and RadioTentLevel {buildingLevel3}.");
				}
				base.manager.Player.GetCurrency(heroTokenTypeAndAmount.Type).Add(heroTokenTypeAndAmount.Amount);
				lootEntry.RewardedCurrency = heroTokenTypeAndAmount.Type;
				lootEntry.RewardedAmount = heroTokenTypeAndAmount.Amount;
				lootEntry.ActualAmountAdded = heroTokenTypeAndAmount.Amount;
				lootEntry.RewardedRarityLevel = heroTokenTypeAndAmount.AmountRarityLevel;
			}
		}
	}

	public void RewardAvatars(LootEntry lootEntry)
	{
		int num = 0;
		if (lootEntry.IconIndex >= 0)
		{
			if (!base.manager.Player.IconIndexs.Contains(lootEntry.IconIndex))
			{
				base.manager.Player.AddIconIndex(lootEntry.IconIndex);
			}
			else
			{
				lootEntry.IconIndex = -1;
				num = base.manager.GameEconomyData.ConfigData.AvatarToGold;
			}
		}
		else if (lootEntry.BorderIndex >= 0)
		{
			if (!base.manager.Player.BorderIndexs.Contains(lootEntry.BorderIndex))
			{
				base.manager.Player.AddBorderIndex(lootEntry.BorderIndex);
			}
			else
			{
				lootEntry.BorderIndex = -1;
				num = base.manager.GameEconomyData.ConfigData.BorderrToGold;
			}
		}
		else if (lootEntry.ColorIndex >= 0)
		{
			if (!base.manager.Player.ColorIndexs.Contains(lootEntry.ColorIndex))
			{
				base.manager.Player.AddColorIndex(lootEntry.ColorIndex);
			}
			else
			{
				lootEntry.ColorIndex = -1;
				num = base.manager.GameEconomyData.ConfigData.AvatarColorToGold;
			}
		}
		if (num > 0)
		{
			base.manager.Player.GetCurrency(CurrencyType.Diamonds).Add(num);
			lootEntry.RewardedCurrency = CurrencyType.Diamonds;
			lootEntry.RewardedAmount = num;
		}
	}

	public void RewardClassToken(LootEntry lootEntry)
	{
		GameEconomyData obj = base.manager.GameEconomyData;
		int buildingLevel = base.manager.Player.Camp.GetBuildingLevel("RadioTent");
		int trainingGroundLevel = base.manager.Player.Camp.GetTrainingGroundLevel();
		SurvivorToken survivorToken = obj.GetClassTokenTypeAndAmount(availableClasses: base.manager.Player.SurvivorContainer.GetAvailableClasses(trainingGroundLevel), eventType: lootEntry.DropEventDefinition.EventType, dropType: lootEntry.DropType, tag: lootEntry.DropEventDefinition.Tag, dropCurrency: lootEntry.DropCurrencyType, targetLevel: buildingLevel, random: GetDedicatedRandom("ChallengeToken"));
		if (survivorToken != null && survivorToken.Type != CurrencyType.None)
		{
			if (survivorToken.Amount == 0)
			{
				base.manager.Debug.LogError($"Did not get proper amount for class tokens when looking up with EventType {lootEntry.DropEventDefinition.EventType.ToString()}, DropType {lootEntry.DropType.ToString()}, Tag {lootEntry.DropEventDefinition.Tag.ToString()}, DropCurrencyType {lootEntry.DropCurrencyType.ToString()} and RadioTentLevel {buildingLevel}.");
			}
			base.manager.Player.GetCurrency(survivorToken.Type).Add(survivorToken.Amount);
			lootEntry.RewardedCurrency = survivorToken.Type;
			lootEntry.RewardedAmount = survivorToken.Amount;
			lootEntry.ActualAmountAdded = survivorToken.Amount;
			lootEntry.RewardedRarityLevel = survivorToken.AmountRarityLevel;
		}
		else
		{
			base.manager.Debug.LogError("LootModelManager: rewarding class token '" + ((survivorToken == null) ? "null" : survivorToken.Type.ToString()) + "'. Please check the token distribution in GED.");
		}
	}

	public SpecialPhoneCallState GetSpecialPhoneCallState(int slotIndex, string endTimeUtc)
	{
		if (SpecialPhoneCallStates == null)
		{
			SpecialPhoneCallStates = new List<SpecialPhoneCallState>();
		}
		for (int i = 0; i < SpecialPhoneCallStates.Count; i++)
		{
			SpecialPhoneCallState specialPhoneCallState = SpecialPhoneCallStates[i];
			if (specialPhoneCallState.SlotNumber == slotIndex && specialPhoneCallState.EndTimeUtc == endTimeUtc)
			{
				return specialPhoneCallState;
			}
		}
		return null;
	}

	public void AddSpecialPhoneCallState(int slotIndex, PhoneCallDefinition phoneCallDefinition)
	{
		if (phoneCallDefinition != null && GetSpecialPhoneCallState(slotIndex, phoneCallDefinition.EndTimeUtc) == null)
		{
			SpecialPhoneCallState specialPhoneCallState = new SpecialPhoneCallState();
			specialPhoneCallState.SlotNumber = slotIndex;
			specialPhoneCallState.EndTimeUtc = phoneCallDefinition.EndTimeUtc;
			specialPhoneCallState.CumulativeProbability = phoneCallDefinition.InitialProbabilityPercentage;
			SpecialPhoneCallStates.Add(specialPhoneCallState);
		}
	}

	public void IncrementSpecialPhoneCallProbability(int slotIndex, string endTimeUtc)
	{
		SpecialPhoneCallState specialPhoneCallState = GetSpecialPhoneCallState(slotIndex, endTimeUtc);
		if (specialPhoneCallState != null)
		{
			PhoneCallDefinition phoneCallDefinition = base.manager.GameEconomyData.GetPhoneCallDefinition(base.manager.Player.UtcTimeStamp, slotIndex);
			if (phoneCallDefinition != null && phoneCallDefinition.EndTimeUtc == endTimeUtc)
			{
				specialPhoneCallState.CumulativeProbability += phoneCallDefinition.ProbabilityPercentageIncrease;
			}
		}
	}

	public void ResetSpecialPhoneCallProbability(int slotIndex, string endTimeUtc)
	{
		SpecialPhoneCallState specialPhoneCallState = GetSpecialPhoneCallState(slotIndex, endTimeUtc);
		if (specialPhoneCallState != null)
		{
			PhoneCallDefinition phoneCallDefinition = base.manager.GameEconomyData.GetPhoneCallDefinition(base.manager.Player.UtcTimeStamp, slotIndex);
			if (phoneCallDefinition != null && phoneCallDefinition.EndTimeUtc == endTimeUtc)
			{
				specialPhoneCallState.CumulativeProbability = phoneCallDefinition.InitialProbabilityPercentage;
			}
		}
	}

	public static LootEntryType GetLootEntryTypeFromChallengeReward(WeeklyChallengeReward reward)
	{
		LootEntryType result = LootEntryType.ChallengePersonalReward;
		if (reward != null)
		{
			result = reward.RewardType switch
			{
				WeeklyChallengeReward.ChallengeRewardType.None => LootEntryType.ChallengePersonalReward,
				WeeklyChallengeReward.ChallengeRewardType.PersonalStars => LootEntryType.ChallengePersonalReward,
				WeeklyChallengeReward.ChallengeRewardType.GuildStars => LootEntryType.ChallengeGuildReward,
				WeeklyChallengeReward.ChallengeRewardType.RoundCompletion => LootEntryType.ChallengeRoundCompletionReward,
				WeeklyChallengeReward.ChallengeRewardType.PersonalHighScore => LootEntryType.ChallengePersonalHighScore,
				WeeklyChallengeReward.ChallengeRewardType.GuildAchiever => LootEntryType.ChallengeGuildAchiever,
				WeeklyChallengeReward.ChallengeRewardType.ApocalypticStars => LootEntryType.ApocalypticStars,
				WeeklyChallengeReward.ChallengeRewardType.ApocalypticRoundStars => LootEntryType.ApocalypticRoundStars,
				_ => LootEntryType.ChallengePersonalReward,
			};
		}
		return result;
	}

	public bool CraftBadge(List<CurrencyType> components, string analyticsId)
	{
		BadgeModel badgeModel = GenerateBadge(components);
		if (badgeModel != null)
		{
			badgeModel.Initialize();
			badgeModel.SetManager(base.manager);
			badgeModel.Start();
			base.manager.Player.Equipment.AddBadge(badgeModel);
			if (OfflineManager.IsLoadDataManager)
			{
				BadgeCraft.Instance.LastCraftedBadge = badgeModel;
			}
			else
			{
				base.manager.Player.LastCraftedBadge = badgeModel;
			}
			base.manager.Player.NotifyChange(BadgeCreatedEvent);
			base.manager.Metrics.ResetTdEvent();
			base.manager.Metrics.AddFind().AddBadge(badgeModel).AddCrafting(CraftingType.Badge, analyticsId)
				.Send();
			base.manager.Metrics.TdEventType = "Find_Badge_Crafting";
			base.manager.Metrics.TdEventPropertyTypes = new List<string> { "Badge", "Crafting" };
			base.manager.Metrics.SendTdEvent();
			return true;
		}
		return false;
	}

	public static bool IsFirstBadgeSlotBadgeComponent(List<CurrencyType> components)
	{
		if (components != null && components.Count > 0)
		{
			return ComponentHelper.GetComponentBaseCurrency(components[0]) == CurrencyType.Badge0;
		}
		return false;
	}

	public BadgeModel GenerateBadge(List<CurrencyType> components)
	{
		BadgeRarityResult badgeRarityResult = base.manager.GameEconomyData.CalculateBadgeRarityResult(components);
		if (badgeRarityResult == null)
		{
			return null;
		}
		int analyticsId = ++CurrentBadgeAnalyticsId;
		ModelRandom dedicatedRandom = base.manager.Player.LootManager.GetDedicatedRandom("BadgeRandom");
		int level = base.manager.Player.Camp.GetBuilding("Residence")?.Level ?? 1;
		int maxRarity;
		int badgeRarity = GetBadgeRarity(badgeRarityResult, dedicatedRandom.GetRandomInRange(1, 100), out maxRarity);
		string effect = GetEffect(components, dedicatedRandom);
		if (string.IsNullOrEmpty(effect))
		{
			return null;
		}
		int randomInRange = dedicatedRandom.GetRandomInRange(1, 100);
		randomInRange += (maxRarity - badgeRarity) * 10;
		randomInRange = Math.Min(randomInRange, 100);
		int randomInRange2 = dedicatedRandom.GetRandomInRange(0, 5);
		BadgeType randomInRange3 = (BadgeType)dedicatedRandom.GetRandomInRange(0, 4);
		BadgeModel badgeModel = new BadgeModel(analyticsId, randomInRange2, badgeRarity, randomInRange3, effect, randomInRange, level);
		string typeIndex = CreateBonusTypeIndex(components.GetRange(1, 4));
		badgeModel.BonusId = dedicatedRandom.GetRandomElement(CreateBadgeGatchaDeckOfIds(typeIndex, base.manager.GameEconomyData.BadgeBonusDefinitions), remove: false);
		BadgeBonusDefinition badgeBonusDefinition = base.gameEconomyData.GetBadgeBonusDefinition(badgeModel.BonusId);
		if (badgeBonusDefinition != null)
		{
			CreateBonusCondition(badgeBonusDefinition, dedicatedRandom, ref badgeModel);
		}
		return badgeModel;
	}

	public BadgeModel RerollBadge(BadgeModel badgeToReroll, BadgeReroll reroll)
	{
		int analyticsId = ++CurrentBadgeAnalyticsId;
		ModelRandom dedicatedRandom;
		if (OfflineManager.IsLoadDataManager)
		{
			dedicatedRandom = BadgeCraft.Instance.modelRandomLast;
		}
		else
		{
			dedicatedRandom = base.manager.Player.LootManager.GetDedicatedRandom("BadgeRandom");
		}
		int num = badgeToReroll.SlotIndex;
		BadgeType badgeType = badgeToReroll.Type;
		int num2 = badgeToReroll.RerollsSlot;
		int num3 = badgeToReroll.RerollsSet;
		int num4 = badgeToReroll.RerollsBonus;
		switch (reroll)
		{
		case BadgeReroll.Slot:
			num2++;
			if (badgeToReroll.HistorySlots == null)
			{
				badgeToReroll.HistorySlots = new List<int>();
			}
			if (badgeToReroll.HistorySlots.Count == 5)
			{
				badgeToReroll.HistorySlots.Clear();
			}
			badgeToReroll.HistorySlots.Add(num);
			while (badgeToReroll.HistorySlots.Contains(num))
			{
				num = dedicatedRandom.GetRandomInRange(0, 5);
			}
			break;
		case BadgeReroll.Set:
			num3++;
			if (badgeToReroll.HistorySet == null)
			{
				badgeToReroll.HistorySet = new List<BadgeType>();
			}
			if (badgeToReroll.HistorySet.Count == 4)
			{
				badgeToReroll.HistorySet.Clear();
			}
			badgeToReroll.HistorySet.Add(badgeType);
			while (badgeToReroll.HistorySet.Contains(badgeType))
			{
				badgeType = (BadgeType)dedicatedRandom.GetRandomInRange(0, 4);
			}
			break;
		}
		BadgeModel badgeModel = new BadgeModel(analyticsId, num, badgeToReroll.Rarity, badgeType, badgeToReroll.EffectId, badgeToReroll.EffectRoll, badgeToReroll.Level);
		if (reroll == BadgeReroll.Bonus)
		{
			if (badgeToReroll.BonusId == "Constant")
			{
				return null;
			}
			num4++;
			List<string> list = (from x in base.manager.GameEconomyData.BadgeBonusDefinitions
				where x.ID != "Constant"
				select x.ID).ToList();
			badgeToReroll.AddBonusToHistory();
			string id = (badgeModel.BonusId = dedicatedRandom.GetRandomElement(list, remove: false));
			BadgeBonusDefinition badgeBonusDefinition = base.gameEconomyData.GetBadgeBonusDefinition(id);
			CreateBonusCondition(badgeBonusDefinition, dedicatedRandom, ref badgeModel);
			while (badgeToReroll.BonusHistoryContain(badgeModel))
			{
				id = (badgeModel.BonusId = dedicatedRandom.GetRandomElement(list, remove: false));
				badgeBonusDefinition = base.gameEconomyData.GetBadgeBonusDefinition(id);
				CreateBonusCondition(badgeBonusDefinition, dedicatedRandom, ref badgeModel);
			}
		}
		else
		{
			BadgeBonusDefinition badgeBonusDefinition2 = base.gameEconomyData.GetBadgeBonusDefinition(badgeToReroll.BonusId);
			badgeModel.BonusId = badgeToReroll.BonusId;
			if (badgeBonusDefinition2 != null)
			{
				CreateCopyOfBonusCondition(badgeBonusDefinition2, ref badgeModel, badgeToReroll);
			}
		}
		badgeModel.RerollsSlot = num2;
		badgeModel.RerollsSet = num3;
		badgeModel.RerollsBonus = num4;
		badgeModel.HistorySlots = badgeToReroll.HistorySlots;
		badgeModel.HistorySet = badgeToReroll.HistorySet;
		badgeModel.HistoryBonus = badgeToReroll.HistoryBonus;
		return badgeModel;
	}

	private string GetEffect(List<CurrencyType> components, ModelRandom random)
	{
		string text = string.Empty;
		BadgeRecipe badgeRecipe = base.gameEconomyData.BadgeRecipes.FirstOrDefault((BadgeRecipe recipe) => recipe.CanBeBuiltWith(components));
		if (badgeRecipe != null)
		{
			int chanceToCraftRecipe = GetChanceToCraftRecipe(components);
			if (random.Next(100) < chanceToCraftRecipe)
			{
				text = badgeRecipe.GetRandomEffect(random);
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			List<BadgeRecipe> list = base.gameEconomyData.BadgeRecipes.Where((BadgeRecipe recipe) => recipe != badgeRecipe).ToList();
			int index = random.Next(list.Count);
			text = list[index].GetRandomEffect(random);
		}
		return text;
	}

	public int GetChanceToCraftRecipe(List<CurrencyType> components)
	{
		int num = 0;
		foreach (CurrencyType component in components)
		{
			string currencyTypeString = component.ToString();
			BadgeEffectChances badgeEffectChances = base.gameEconomyData.BadgeEffectChances.FirstOrDefault((BadgeEffectChances badgeEffectChance) => badgeEffectChance.ComponentId == currencyTypeString);
			if (badgeEffectChances != null)
			{
				num += badgeEffectChances.Chance;
			}
		}
		return num;
	}

	private List<KeyValuePair<FixedPoint, int>> CreateBadgeRarityProbabilities(BadgeRarityResult result)
	{
		FixedPoint fixedPoint = 0L;
		List<KeyValuePair<FixedPoint, int>> list = new List<KeyValuePair<FixedPoint, int>>();
		if (result.Common > 0L)
		{
			list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Common, 0));
			fixedPoint += result.Common;
		}
		if (result.Uncommon > 0L)
		{
			list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Uncommon, 1));
			fixedPoint += result.Uncommon;
		}
		if (result.Rare > 0L)
		{
			list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Rare, 2));
			fixedPoint += result.Rare;
		}
		if (result.Epic > 0L)
		{
			list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Epic, 3));
			fixedPoint += result.Epic;
		}
		if (result.Legendary > 0L)
		{
			list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Legendary, 4));
			fixedPoint += result.Legendary;
		}
		return list;
	}

	public Cashier GetBadgeCraftCashier(List<CurrencyType> components)
	{
		Cashier cashier = new Cashier(base.manager);
		if (components != null)
		{
			for (int i = 0; i < components.Count; i++)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.CraftBadge);
				cashierItem.SetCost(components[i], 1);
				cashier.AddItem(cashierItem);
			}
		}
		BuildingUpgradeLevel buildingUpgradeLevel = base.manager.GameEconomyData.GetBuildingUpgradeLevel("Council", base.manager.Player.CouncilLevel);
		if (buildingUpgradeLevel == null)
		{
			base.Debug.LogWarning("Missing council update level info for " + base.manager.Player.CouncilLevel);
			return null;
		}
		if (buildingUpgradeLevel.BadgeCreationCost > 0)
		{
			CashierItem cashierItem = new CashierItem(PurchaseType.CraftBadge);
			cashierItem.SetCost(CurrencyType.Supplies, buildingUpgradeLevel.BadgeCreationCost);
			cashier.AddItem(cashierItem);
		}
		return cashier;
	}

	private void CreateBonusCondition(BadgeBonusDefinition bonusDef, ModelRandom random, ref BadgeModel badgeModel)
	{
		Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
		if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
		{
			base.Debug.LogError("Failed to instantiate condition class " + bonusDef.ConditionClassName);
		}
		List<string> list = new List<string> { bonusDef.ConstructionParameters[0] };
		if (bonusDef.ConstructionParameters.Count > 1)
		{
			list.Add(random.GetRandomElement(bonusDef.ConstructionParameters.GetRange(1, bonusDef.ConstructionParameters.Count - 1), remove: false));
		}
		badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, list) as BaseBonusCondition) : null);
		badgeModel.BonusParameters = list;
	}

	private void CreateCopyOfBonusCondition(BadgeBonusDefinition bonusDef, ref BadgeModel badgeModel, BadgeModel oldBadgeModel)
	{
		Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
		if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
		{
			base.Debug.LogError("Failed to instantiate condition class " + bonusDef.ConditionClassName);
		}
		badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, oldBadgeModel.BonusParameters) as BaseBonusCondition) : null);
		badgeModel.BonusParameters = oldBadgeModel.BonusParameters;
	}

	public string CreateBonusTypeIndex(List<CurrencyType> usedComponents)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		CurrencyType[] array = new CurrencyType[4]
		{
			CurrencyType.Metal0,
			CurrencyType.Cloth0,
			CurrencyType.Chemicals0,
			CurrencyType.Food0
		};
		for (int i = 0; i < (usedComponents?.Count ?? 0); i++)
		{
			int value = 0;
			CurrencyType componentBaseCurrency = ComponentHelper.GetComponentBaseCurrency(usedComponents[i]);
			int key = Array.IndexOf(array, componentBaseCurrency) + 1;
			if (dictionary.TryGetValue(key, out value))
			{
				dictionary[key] = value + 1;
			}
			else
			{
				dictionary.Add(key, 1);
			}
		}
		List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
		list.AddRange(dictionary);
		list.StableSort(delegate(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
		{
			KeyValuePair<int, int> keyValuePair = a;
			KeyValuePair<int, int> keyValuePair2 = b;
			return keyValuePair.Value.CompareTo(keyValuePair2.Value) * -1;
		});
		ResetKeysToZeroForSameValues(ref list);
		StringBuilder stringBuilder = new StringBuilder();
		for (int num = 0; num < list.Count && num < 2; num++)
		{
			stringBuilder.Append(list[num].Key);
		}
		if (stringBuilder.Length < 2)
		{
			stringBuilder.Append('0', 2 - stringBuilder.Length);
		}
		return stringBuilder.ToString();
	}

	private void ResetKeysToZeroForSameValues(ref List<KeyValuePair<int, int>> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			KeyValuePair<int, int> keyValuePair = list[i];
			bool flag = false;
			for (int j = i + 1; j < list.Count; j++)
			{
				KeyValuePair<int, int> keyValuePair2 = list[j];
				if (keyValuePair.Value == keyValuePair2.Value)
				{
					list[j] = new KeyValuePair<int, int>(0, keyValuePair.Value);
					flag = true;
				}
			}
			if (flag)
			{
				list[i] = new KeyValuePair<int, int>(0, keyValuePair.Value);
			}
		}
	}

	private List<string> CreateBadgeGatchaDeckOfIds<T>(string typeIndex, T[] listToBeUsed) where T : TypeIndexDefinition
	{
		List<string> list = new List<string>();
		for (int i = 0; i < ((listToBeUsed != null) ? listToBeUsed.Length : 0); i++)
		{
			list.Add(listToBeUsed[i].ID);
			list.Add(listToBeUsed[i].ID);
			if (!string.IsNullOrEmpty(typeIndex) && listToBeUsed[i].TypeIndex == typeIndex)
			{
				list.Add(listToBeUsed[i].ID);
			}
		}
		return list;
	}

	private int GetBadgeRarity(BadgeRarityResult badgeRarityResult, FixedPoint roll, out int maxRarity)
	{
		List<KeyValuePair<FixedPoint, int>> list = CreateBadgeRarityProbabilities(badgeRarityResult);
		maxRarity = 0;
		int result = 0;
		bool flag = false;
		foreach (KeyValuePair<FixedPoint, int> item in list)
		{
			if (item.Key > 0L)
			{
				maxRarity = item.Value;
			}
			if (item.Key >= roll && !flag)
			{
				result = item.Value;
				flag = true;
			}
		}
		return result;
	}

	public static LootEntryType GetLootEntryTypeFromSurvivalReward(WeeklySurvivalReward reward)
	{
		LootEntryType result = LootEntryType.SurvivalPersonalReward;
		if (reward != null)
		{
			result = reward.RewardType switch
			{
				WeeklySurvivalReward.SurvivalRewardType.None => LootEntryType.SurvivalPersonalReward,
				WeeklySurvivalReward.SurvivalRewardType.MissionCompletions => LootEntryType.SurvivalPersonalReward,
				WeeklySurvivalReward.SurvivalRewardType.FullCompletion => LootEntryType.SurvivalFullCompletionReward,
				_ => LootEntryType.SurvivalPersonalReward,
			};
		}
		return result;
	}

	public static LootEntryType GetLootEntryTypeFromGuildBattleReward(GuildBattleReward reward)
	{
		LootEntryType result = LootEntryType.GuildBattleMissionCompletion;
		if (reward != null)
		{
			switch (reward.RewardType)
			{
			case GuildBattleReward.GuildRewardType.None:
				result = LootEntryType.GuildBattleMissionCompletion;
				break;
			case GuildBattleReward.GuildRewardType.BattleLost:
				result = LootEntryType.GuildBattleLost;
				break;
			case GuildBattleReward.GuildRewardType.BattleWin:
				result = LootEntryType.GuildBatttleWon;
				break;
			case GuildBattleReward.GuildRewardType.SectorCompletion:
				result = LootEntryType.GuildBattleSectorCompletion;
				break;
			case GuildBattleReward.GuildRewardType.SectorBonus:
				result = LootEntryType.GuildBattleSectorBonus;
				break;
			case GuildBattleReward.GuildRewardType.MissionCompletion:
				result = LootEntryType.GuildBattleMissionCompletion;
				break;
			}
		}
		return result;
	}

	public int GetBadgeReRollCost(int badgeModelId, BadgeReroll reRollType)
	{
		BadgeModel badgeModel;
		if (!OfflineManager.IsLoadDataManager)
		{
			badgeModel = base.manager.Player.Equipment.Badges.Get(badgeModelId);
		}
		else
		{
			BadgeInfo badgeInfo = DataManager.Instance.PlayerBadges.FirstOrDefault(x => x.Model.ModelId == badgeModelId);
			if (badgeInfo != null)
			{
				badgeModel = badgeInfo.Model;
			}
			else
			{
				badgeModel = BadgeCraft.Instance.LastCraftedBadge;
			}
		}

		if (badgeModel == null)
		{
			return -1;
		}
		int badgeRerolls = badgeModel.GetBadgeRerolls(reRollType);
		if (badgeRerolls == -1)
		{
			return -1;
		}
		BadgeRerollConfig badgeRerollConfig = base.gameEconomyData.BadgeRerollConfigs.FirstOrDefault((BadgeRerollConfig x) => x.Type == reRollType.ToString());
		if (badgeRerollConfig == null)
		{
			return -1;
		}
		return badgeRerollConfig.Price[Math.Min(badgeRerollConfig.Price.Length - 1, badgeRerolls)];
	}
}
