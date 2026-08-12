using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BaseModel;
using TWDModel;

public class Rewards
{
	public List<IReward> RewardsList = new List<IReward>();

	public List<Dictionary<string, object>> RewardResources = new List<Dictionary<string, object>>();

	public int Count
	{
		get
		{
			if (RewardsList == null)
			{
				return 0;
			}
			return RewardsList.Count;
		}
	}

	public Rewards()
	{
	}

	public Rewards(List<IReward> rewards)
	{
		RewardsList = rewards;
	}

	public Rewards(List<string> rewards)
		: this(string.Join(";", rewards))
	{
	}

	public Rewards(string rewardsString, TWDModelManager modelManager = null, int controlVariable = 0, EquipmentSource equipmentSource = EquipmentSource.Unknown, ModelRandom random = null)
	{
		if (random == null && modelManager != null)
		{
			random = modelManager.Player.PlayerRandom;
		}
		string[] array = rewardsString.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			string resourceName = array[i];
			if (string.IsNullOrEmpty(array[i]))
			{
				continue;
			}
			string[] array2 = array[i].Split('(');
			string text = array2[0].ToLowerInvariant();
			array2[1] = array2[1].Replace(")", "");
			switch (text)
			{
			case "class":
				AddRewardSurvivorClass((SurvivorClass)Enum.Parse(typeof(SurvivorClass), array2[1]));
				continue;
			case "equipconsumable":
			{
				string[] array5 = array2[1].Split(',');
				int num = 1;
				if (array5.Length > 1 && !string.IsNullOrEmpty(array5[1]))
				{
					num = int.Parse(array5[1]);
				}
				AddEquipmentConsumableClass(array5[0], num, equipmentSource);
				AddRewardResources("EquipConsumable(" + array5[0] + ")", num);
				continue;
			}
			case "equip":
			{
				string[] array3 = array2[1].Split(',');
				string equipmentId = array3[0];
				int rarityLevel = -1;
				int level = -1;
				int levelOffset = 0;
				if (array3.Length > 1 && !string.IsNullOrEmpty(array3[1]))
				{
					rarityLevel = int.Parse(array3[1]);
					if (array3.Length > 2 && !string.IsNullOrEmpty(array3[2]))
					{
						level = int.Parse(array3[2]);
					}
					if (array3.Length > 3 && !string.IsNullOrEmpty(array3[3]))
					{
						levelOffset = int.Parse(array3[3]);
					}
				}
				AddEquipmentClass(equipmentId, rarityLevel, level, levelOffset, equipmentSource);
				AddRewardResources(resourceName, 1);
				continue;
			}
			case "unlockbuilding":
				AddUnlockBuilding(array2[1]);
				continue;
			case "survivorslots":
			{
				int num2 = int.Parse(array2[1]);
				AddSurvivorSlot(num2);
				AddRewardResources(array2[0], num2);
				continue;
			}
			case "loot":
			{
				string text2 = array2[1].ToLowerInvariant();
				DropType dropType = DropType.Regular;
				if (text2 == "silver")
				{
					dropType = DropType.Silver;
				}
				else if (text2 == "gold")
				{
					dropType = DropType.Gold;
				}
				AddLootEntry(dropType);
				continue;
			}
			case "randomequipment":
			{
				string[] array4 = array2[1].Split(',');
				EquipmentCategory category = (EquipmentCategory)Enum.Parse(typeof(EquipmentCategory), array4[0]);
				int level2 = int.Parse(array4[1]);
				SurvivorClass survivorClass = (SurvivorClass)Enum.Parse(typeof(SurvivorClass), array4[2]);
				int rarityLevel2 = int.Parse(array4[3]);
				AddRandomEquipment(category, level2, survivorClass, rarityLevel2, equipmentSource);
				AddRewardResources(resourceName, 1);
				continue;
			}
			case "outfits":
			{
				string[] array6 = array2[1].Split(',');
				List<string> list = new List<string>();
				for (int j = 0; j < array6.Length; j++)
				{
					list.Add(array6[j]);
				}
				AddOutfit(list);
				AddRewardResources(resourceName, 1);
				continue;
			}
			case "heroskins":
			{
				string[] array7 = array2[1].Split(',');
				List<string> list2 = new List<string>();
				for (int k = 0; k < array7.Length; k++)
				{
					list2.Add(array7[k]);
				}
				AddHeroSkin(list2);
				foreach (string item in list2)
				{
					AddRewardResources(item, 1);
				}
				continue;
			}
			case "avatar":
			case "border":
			case "avatarcolor":
			{
				int result4 = -1;
				if (!int.TryParse(array2[1], out result4))
				{
					result4 = -1;
				}
				AddAvatars(text, result4);
				continue;
			}
			case "skiptoken":
			{
				int result3 = 0;
				if (!int.TryParse(array2[1], out result3))
				{
					result3 = 0;
				}
				AddChallangeSkillToken(result3);
				continue;
			}
			case "unlimitedgas":
			case "doublexp":
			{
				TimedBonusType timedBonusType = TimedBonusType.UnlimitedGas;
				if (text == "doublexp")
				{
					timedBonusType = TimedBonusType.DoubleXp;
				}
				if (array2[1] != null)
				{
					string text3 = Regex.Replace(array2[1], "\\d", string.Empty);
					FixedPoint duration = 0L;
					if (!string.IsNullOrEmpty(text3))
					{
						try
						{
							FixedPoint fixedPoint = Convert.ToDouble(array2[1].Replace(text3, string.Empty));
							switch (text3.Trim())
							{
							case "m":
								duration = new FixedPoint(fixedPoint * UtilsDateTime.MinuteInMilliseconds);
								break;
							case "h":
								duration = new FixedPoint(fixedPoint * UtilsDateTime.HourInMilliseconds);
								break;
							case "d":
								duration = new FixedPoint(fixedPoint * UtilsDateTime.DayInMilliseconds);
								break;
							}
						}
						catch (Exception)
						{
						}
					}
					else
					{
						try
						{
							duration = new FixedPoint(Convert.ToDouble(array2[1]) * (double)UtilsDateTime.DayInMilliseconds);
						}
						catch (Exception)
						{
						}
					}
					AddTimedBonus(timedBonusType, duration);
				}
				AddRewardResources(resourceName, 1);
				continue;
			}
			case "tradecrate":
				AddTradeCrate(array2[1]);
				continue;
			case "traitbonus":
				AddTraitBonus(array2[1]);
				continue;
			case "guildbattlevp":
			{
				int result2 = 0;
				if (!int.TryParse(array2[1], out result2))
				{
					result2 = 0;
				}
				AddGuildBattleVP(result2);
				continue;
			}
			case "guildbattlebonusrp":
			{
				float result = 0f;
				if (!float.TryParse(array2[1], out result))
				{
					result = 0f;
				}
				AddCurrencyWithMultiplier(CurrencyType.GuildBattleRP, result, isDiamondExchange: false, canOverflowMax: false);
				continue;
			}
			case "guildbattlebonusvp":
			{
				float result5 = 0f;
				if (!float.TryParse(array2[1], out result5))
				{
					result5 = 0f;
				}
				AddGuildBattleVPWithMultiplier(result5);
				continue;
			}
			}
			if (text.Contains("missing"))
			{
				string value = array2[0].Replace("Missing", "");
				CurrencyType rewardCurrencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), value);
				int max = int.Parse(array2[1]);
				AddMissingTokensReward(rewardCurrencyType, max);
				continue;
			}
			if (text.Contains("battlepasspremium"))
			{
				AddBattlePassPremiumReward();
				AddRewardResources(array2[0], 1);
				continue;
			}
			if (text.Contains("sevendaypremium"))
			{
				AddSevenDayPremiumReward();
				AddRewardResources(array2[0], 1);
				continue;
			}
			if (text.Contains("threedaypremium"))
			{
				AddThreeDayPremiumReward();
				AddRewardResources(array2[0], 1);
				continue;
			}
			if (text.Contains("returnthreedaypremium"))
			{
				AddReturnThreeDayPremiumReward();
				AddRewardResources(array2[0], 1);
				continue;
			}
			if (text.Contains("returnendlessdealpremium"))
			{
				string bundleId = array2[1];
				AddReturnEndlessDealPremiumReward(bundleId);
				AddRewardResources(array2[0], 1);
				continue;
			}
			if (text.Contains("activefoundationpremium"))
			{
				AddActiveFoundationPremiumReward();
				AddRewardResources(array2[0], 1);
				continue;
			}
			if (text.Contains("weeklysubscription"))
			{
				AddWeeklySubscriptionReward();
				AddRewardResources(array2[0], 1);
				continue;
			}
			if (text.Contains("monthlysubscription"))
			{
				AddMonthlySubscriptionReward();
				AddRewardResources(array2[0], 1);
				continue;
			}
			switch (text)
			{
			case "equiptoken":
			{
				string[] array10 = array2[1].Split(',');
				string text5 = array10[0];
				int num4 = int.Parse(array10[1]);
				AddEquipTokenReward(text5, num4);
				AddRewardResources(text5, num4);
				continue;
			}
			case "remoldskill":
			{
				string[] array9 = array2[1].Split(',');
				string text4 = array9[0];
				int num3 = 1;
				if (array9.Length > 1 && int.TryParse(array9[1], out var result7))
				{
					num3 = result7;
				}
				AddRemoldSkillReward(text4, num3);
				AddRewardResources(text4, num3);
				continue;
			}
			case "supplies":
			{
				if (!rewardsString.ToLower().Contains("CanOverflowMax".ToLower()))
				{
					break;
				}
				CurrencyType currency = CurrencyType.Supplies;
				string[] array8 = array2[1].Split(',');
				for (int l = 0; l < array8.Length; l++)
				{
					array8[l] = array8[l].Trim();
				}
				int result6 = 0;
				if (!int.TryParse(array8[0], out result6))
				{
					result6 = 0;
				}
				bool isDiamondExchange = false;
				AddRewardCurrency(currency, result6, isDiamondExchange, canOverflowMax: true);
				AddRewardResources(array2[0], result6);
				continue;
			}
			}
			if (text == "sptraitsupgradetoken" && rewardsString.ToLower().Contains("CanOverflowMax".ToLower()))
			{
				CurrencyType currency2 = CurrencyType.SPTraitsUpgradeToken;
				string[] array11 = array2[1].Split(',');
				for (int m = 0; m < array11.Length; m++)
				{
					array11[m] = array11[m].Trim();
				}
				int result8 = 0;
				if (!int.TryParse(array11[0], out result8))
				{
					result8 = 0;
				}
				bool isDiamondExchange2 = false;
				AddRewardCurrency(currency2, result8, isDiamondExchange2, canOverflowMax: true);
				AddRewardResources(array2[0], result8);
				continue;
			}
			CurrencyType currencyType = CurrencyType.None;
			if (array2[0] == "Gold")
			{
				currencyType = CurrencyType.Diamonds;
			}
			else if (modelManager != null && array2[0] == "RandomClassTokens")
			{
				CurrencyType[] classTokenCurrencyTypes = CurrencyModel.GetClassTokenCurrencyTypes();
				List<CurrencyType> list3 = new List<CurrencyType>();
				for (int n = 0; n < classTokenCurrencyTypes.Length; n++)
				{
					SurvivorClass survivorClassForUpgradeCurrencyType = SurvivorModel.GetSurvivorClassForUpgradeCurrencyType(classTokenCurrencyTypes[n]);
					if (survivorClassForUpgradeCurrencyType != SurvivorClass.None && modelManager.Player.SurvivorContainer.IsSurvivorClassUnlocked(survivorClassForUpgradeCurrencyType))
					{
						list3.Add(classTokenCurrencyTypes[n]);
					}
				}
				if (list3 != null && list3.Count > 0)
				{
					currencyType = list3[random.Next(list3.Count)];
				}
			}
			else if (modelManager != null && array2[0] == "RandomHeroTokens")
			{
				List<CurrencyType> availableHeroTokens = modelManager.GameEconomyData.GetAvailableHeroTokens();
				int index = 0;
				if (random != null)
				{
					index = random.Next(availableHeroTokens.Count);
				}
				currencyType = availableHeroTokens[index];
			}
			else
			{
				currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0]);
			}
			string[] array12 = array2[1].Split(',');
			for (int num5 = 0; num5 < array12.Length; num5++)
			{
				array12[num5] = array12[num5].Trim();
			}
			int result9 = 0;
			bool flag = true;
			if (modelManager != null && controlVariable != 0 && array12[0] == "MLvl")
			{
				if (modelManager != null)
				{
					bool num6 = modelManager.Player.ActivityManager.IsActivityOpen(ActivityType.TomatoMonday);
					DropCurrenciesAmountsDefinition dropCurrencyAmountDefinition = modelManager.GameEconomyData.GetDropCurrencyAmountDefinition(DropType.Gold, currencyType, controlVariable);
					result9 = (num6 ? dropCurrencyAmountDefinition.EventMinAmount : dropCurrencyAmountDefinition.MinAmount);
				}
			}
			else if (modelManager != null && array12.Length > 1 && array12[1] == "PlayerLevel")
			{
				DropType dropType2 = DropType.Regular;
				string text6 = array12[0].ToLowerInvariant();
				if (text6 == "silver")
				{
					dropType2 = DropType.Silver;
				}
				else if (text6 == "gold")
				{
					dropType2 = DropType.Gold;
				}
				bool num7 = modelManager.Player.ActivityManager.IsActivityOpen(ActivityType.TomatoMonday);
				flag = false;
				DropCurrenciesAmountsDefinition dropCurrencyAmountDefinition2 = modelManager.GameEconomyData.GetDropCurrencyAmountDefinition(dropType2, currencyType, controlVariable);
				result9 = (num7 ? dropCurrencyAmountDefinition2.EventMinAmount : dropCurrencyAmountDefinition2.MinAmount);
			}
			else if (!int.TryParse(array12[0], out result9))
			{
				result9 = 0;
			}
			bool isDiamondExchange3 = false;
			if (flag && array12.Length >= 2)
			{
				isDiamondExchange3 = bool.Parse(array12[1]);
			}
			AddRewardCurrency(currencyType, result9, isDiamondExchange3, currencyType == CurrencyType.ReplayToken || currencyType == CurrencyType.GvGGas);
			AddRewardResources(array2[0], result9);
		}
	}

	public void AddTimedBonus(TimedBonusType timedBonusType, FixedPoint duration)
	{
		RewardTimedBonus rewardTimedBonus = new RewardTimedBonus();
		rewardTimedBonus.Duration = duration;
		rewardTimedBonus.TimedBonusType = timedBonusType;
		RewardsList.Add(rewardTimedBonus);
	}

	public void AddRewardCurrency(CurrencyType currency, int amount, bool isDiamondExchange, bool canOverflowMax)
	{
		RewardCurrency rewardCurrency = new RewardCurrency();
		rewardCurrency.CurrencyType = currency;
		rewardCurrency.Amount = amount;
		rewardCurrency.IsDiamondExchange = isDiamondExchange;
		rewardCurrency.CanOverflowMax = canOverflowMax;
		RewardsList.Add(rewardCurrency);
	}

	public void AddMissingTokensReward(CurrencyType rewardCurrencyType, int max)
	{
		RewardMissingTokens item = new RewardMissingTokens
		{
			RewardCurrencyType = rewardCurrencyType,
			MaxTokensGiven = max
		};
		RewardsList.Add(item);
	}

	private void AddEquipTokenReward(string equipTokenId, int rewardAmount)
	{
		RewardEquipToken item = new RewardEquipToken
		{
			EquipTokenId = equipTokenId,
			RewardAmount = rewardAmount
		};
		RewardsList.Add(item);
	}

	private void AddRemoldSkillReward(string equipTokenId, int amount = 1)
	{
		RewardRemoldSkill item = new RewardRemoldSkill
		{
			SpRemoldSkillType = equipTokenId,
			Amount = amount
		};
		RewardsList.Add(item);
	}

	public void AddRewardSurvivorClass(SurvivorClass survivorClass)
	{
		RewardSurvivorClass rewardSurvivorClass = new RewardSurvivorClass();
		rewardSurvivorClass.SurvivorClass = survivorClass;
		RewardsList.Add(rewardSurvivorClass);
	}

	public void AddUnlockBuilding(string buildingTypeName)
	{
		RewardUnlockBuilding rewardUnlockBuilding = new RewardUnlockBuilding();
		rewardUnlockBuilding.BuildingTypeName = buildingTypeName;
		RewardsList.Add(rewardUnlockBuilding);
	}

	public void AddSurvivorSlot(int amount)
	{
		RewardSurvivorSlot rewardSurvivorSlot = new RewardSurvivorSlot();
		rewardSurvivorSlot.Amount = amount;
		RewardsList.Add(rewardSurvivorSlot);
	}

	public void AddEquipmentClass(string equipmentId, int rarityLevel, int level, int levelOffset, EquipmentSource equipmentSource = EquipmentSource.Unknown)
	{
		RewardEquipment rewardEquipment = new RewardEquipment();
		rewardEquipment.EquipmentId = equipmentId;
		rewardEquipment.RarityLevel = rarityLevel;
		rewardEquipment.StartingLevel = level;
		rewardEquipment.StartingLevelOffset = levelOffset;
		rewardEquipment.EquipmentSource = equipmentSource;
		RewardsList.Add(rewardEquipment);
	}

	public void AddEquipmentConsumableClass(string equipmentId, int amount, EquipmentSource equipmentSource = EquipmentSource.Unknown)
	{
		RewardEquipment rewardEquipment = new RewardEquipment();
		rewardEquipment.EquipmentId = equipmentId;
		rewardEquipment.EquipmentSource = equipmentSource;
		rewardEquipment.Amount = amount;
		RewardsList.Add(rewardEquipment);
	}

	public void AddRandomEquipment(EquipmentCategory category, int level, SurvivorClass survivorClass, int rarityLevel, EquipmentSource equipmentSource = EquipmentSource.Unknown)
	{
		RewardRandomEquipment rewardRandomEquipment = new RewardRandomEquipment();
		rewardRandomEquipment.Category = category;
		rewardRandomEquipment.StartingLevelOffset = level;
		rewardRandomEquipment.SurvivorClass = survivorClass;
		rewardRandomEquipment.RarityLevel = rarityLevel;
		rewardRandomEquipment.EquipmentSource = equipmentSource;
		RewardsList.Add(rewardRandomEquipment);
	}

	public void AddOutfit(List<string> outfitsPreferredOrder)
	{
		RewardOutfit rewardOutfit = new RewardOutfit();
		rewardOutfit.PreferredOrder = outfitsPreferredOrder;
		RewardsList.Add(rewardOutfit);
	}

	public void AddHeroSkin(List<string> skinsPreferredOrder)
	{
		RewardHeroSkin rewardHeroSkin = new RewardHeroSkin();
		rewardHeroSkin.PreferredOrder = skinsPreferredOrder;
		RewardsList.Add(rewardHeroSkin);
	}

	public void AddChallangeSkillToken(int amount)
	{
		RewardSkipChallange rewardSkipChallange = new RewardSkipChallange();
		rewardSkipChallange.Amount = amount;
		RewardsList.Add(rewardSkipChallange);
	}

	public void AddAvatars(string first, int index)
	{
		RewardAvatars rewardAvatars = new RewardAvatars();
		switch (first)
		{
		case "avatar":
			rewardAvatars.Avatar = index;
			break;
		case "border":
			rewardAvatars.Border = index;
			break;
		case "avatarcolor":
			rewardAvatars.Color = index;
			break;
		}
		RewardsList.Add(rewardAvatars);
	}

	public void AddLootEntry(DropType dropType)
	{
		RewardLootEntry rewardLootEntry = new RewardLootEntry();
		rewardLootEntry.DropType = dropType;
		RewardsList.Add(rewardLootEntry);
	}

	public void AddTradeCrate(string tradeCrateId)
	{
		RewardTradeCrate rewardTradeCrate = new RewardTradeCrate();
		rewardTradeCrate.TradeCrateId = tradeCrateId;
		RewardsList.Add(rewardTradeCrate);
	}

	public void AddTraitBonus(string traitId)
	{
		RewardTraitBonus rewardTraitBonus = new RewardTraitBonus();
		rewardTraitBonus.TraitId = traitId;
		RewardsList.Add(rewardTraitBonus);
	}

	public void AddGuildBattleVP(int amount)
	{
		RewardGuildBattleVP rewardGuildBattleVP = new RewardGuildBattleVP();
		rewardGuildBattleVP.Amount = amount;
		RewardsList.Add(rewardGuildBattleVP);
	}

	public void AddGuildBattleVPWithMultiplier(float multiplier)
	{
		RewardGuildBattleWithMultiplier rewardGuildBattleWithMultiplier = new RewardGuildBattleWithMultiplier();
		rewardGuildBattleWithMultiplier.Multiplier = multiplier;
		RewardsList.Add(rewardGuildBattleWithMultiplier);
	}

	public void AddCurrencyWithMultiplier(CurrencyType currency, float multiplier, bool isDiamondExchange, bool canOverflowMax)
	{
		RewardCurrencyWithMultiplier rewardCurrencyWithMultiplier = new RewardCurrencyWithMultiplier();
		rewardCurrencyWithMultiplier.Multiplier = multiplier;
		rewardCurrencyWithMultiplier.CurrencyType = currency;
		rewardCurrencyWithMultiplier.IsDiamondExchange = isDiamondExchange;
		rewardCurrencyWithMultiplier.CanOverflowMax = canOverflowMax;
		RewardsList.Add(rewardCurrencyWithMultiplier);
	}

	public void AddBattlePassPremiumReward()
	{
		RewardsList.Add(new RewardBattlePassPremium());
	}

	public void AddSevenDayPremiumReward()
	{
		RewardsList.Add(new RewardSevenDayPremium());
	}

	public void AddThreeDayPremiumReward()
	{
		RewardsList.Add(new ThreeDayPremium());
	}

	public void AddReturnThreeDayPremiumReward()
	{
		RewardsList.Add(new ReturnThreeDayPremium());
	}

	public void AddReturnEndlessDealPremiumReward(string bundleId)
	{
		RewardsList.Add(new ReturnEndlessDealPremium(bundleId));
	}

	public void AddActiveFoundationPremiumReward()
	{
		RewardsList.Add(new RewardActiveFoundationPremium());
	}

	public void AddWeeklySubscriptionReward()
	{
		RewardsList.Add(new RewardWeeklySubscription());
	}

	public void AddMonthlySubscriptionReward()
	{
		RewardsList.Add(new RewardMonthlySubscription());
	}

	public void Add(Rewards r)
	{
		for (int i = 0; i < r.RewardsList.Count; i++)
		{
			RewardsList.Add(r.RewardsList[i]);
		}
	}

	public void MultiplyCurrencies(FixedPoint multiplier)
	{
		for (int i = 0; i < RewardsList.Count; i++)
		{
			if (RewardsList[i] is RewardCurrency rewardCurrency)
			{
				rewardCurrency.Amount = (int)(rewardCurrency.Amount * multiplier);
			}
		}
	}

	public IReward GetRewardAt(int index)
	{
		if (RewardsList == null || index >= RewardsList.Count)
		{
			return null;
		}
		return RewardsList[index];
	}

	public RewardSurvivorClass GetSurvivorClassReward()
	{
		for (int i = 0; i < RewardsList.Count; i++)
		{
			if (RewardsList[i] is RewardSurvivorClass)
			{
				return RewardsList[i] as RewardSurvivorClass;
			}
		}
		return null;
	}

	public List<IReward> GetRewardsOfType(RewardType rewardType)
	{
		if (RewardsList != null)
		{
			List<IReward> list = new List<IReward>();
			for (int i = 0; i < RewardsList.Count; i++)
			{
				if (RewardsList[i].Type == rewardType)
				{
					list.Add(RewardsList[i]);
				}
			}
			return list;
		}
		return null;
	}

	public int GetTotalCurrencyRewardAmount(CurrencyType currency)
	{
		List<IReward> rewardsOfType = GetRewardsOfType(RewardType.Currency);
		int num = 0;
		if (rewardsOfType != null)
		{
			for (int i = 0; i < rewardsOfType.Count; i++)
			{
				if (rewardsOfType[i] is RewardCurrency rewardCurrency && rewardCurrency.CurrencyType == currency)
				{
					num += rewardCurrency.Amount;
				}
			}
		}
		return num;
	}

	public RewardCurrency GetStoryFirstCurrency()
	{
		List<IReward> rewardsOfType = GetRewardsOfType(RewardType.Currency);
		if (rewardsOfType == null || rewardsOfType.Count <= 0)
		{
			return null;
		}
		return rewardsOfType[0] as RewardCurrency;
	}

	public List<RewardCurrency> GetAllRewardCurrencies()
	{
		List<RewardCurrency> list = new List<RewardCurrency>();
		if (RewardsList != null)
		{
			for (int i = 0; i < RewardsList.Count; i++)
			{
				if (RewardsList[i] is RewardCurrency)
				{
					list.Add((RewardCurrency)RewardsList[i]);
				}
			}
		}
		return list;
	}

	public List<object> Give(TWDModelManager manager)
	{
		List<object> list = new List<object>();
		foreach (IReward rewards in RewardsList)
		{
			object obj = null;
			obj = ((!(rewards is RandomizedReward)) ? rewards.Give(manager) : rewards.Give(manager, new object[1] { manager.Player.PlayerRandom }));
			list.Add(obj);
		}
		return list;
	}

	public void SetupAnalytics(ref Dictionary<string, string> outDictionary)
	{
		for (int i = 0; i < RewardsList.Count; i++)
		{
			if (RewardsList[i].Type == RewardType.Currency)
			{
				RewardCurrency rewardCurrency = RewardsList[i] as RewardCurrency;
				if (!outDictionary.ContainsKey("reward_currency_" + rewardCurrency.CurrencyType))
				{
					outDictionary.Add("reward_currency_" + rewardCurrency.CurrencyType, rewardCurrency.Amount.ToString());
				}
			}
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < RewardsList.Count; i++)
		{
			if (RewardsList[i].Type == RewardType.Currency)
			{
				RewardCurrency rewardCurrency = (RewardCurrency)RewardsList[i];
				stringBuilder.Append(rewardCurrency.Amount.ToString()).Append(" ").Append(rewardCurrency.CurrencyType);
			}
			else
			{
				stringBuilder.Append(RewardsList[i].Type);
			}
			if (i < RewardsList.Count - 1)
			{
				stringBuilder.Append(", ");
			}
		}
		return stringBuilder.ToString();
	}

	public void MergeCurrencies(Rewards from)
	{
		List<IReward> rewardsOfType = from.GetRewardsOfType(RewardType.Currency);
		if (rewardsOfType == null)
		{
			return;
		}
		for (int i = 0; i < rewardsOfType.Count; i++)
		{
			RewardCurrency rewardCurrency = (RewardCurrency)rewardsOfType[i];
			bool flag = false;
			if (RewardsList != null)
			{
				for (int j = 0; j < RewardsList.Count; j++)
				{
					if (RewardsList[j] is RewardCurrency rewardCurrency2 && rewardCurrency2.CurrencyType == rewardCurrency.CurrencyType)
					{
						rewardCurrency2.Amount += rewardCurrency.Amount;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				AddRewardCurrency(rewardCurrency.CurrencyType, rewardCurrency.Amount, rewardCurrency.IsDiamondExchange, rewardCurrency.CanOverflowMax);
			}
		}
	}

	private void AddRewardResources(string resourceName, int resourceNum)
	{
		RewardResources.Add(new Dictionary<string, object>
		{
			{ "resource_name", resourceName },
			{ "resource_num", resourceNum }
		});
	}
}
