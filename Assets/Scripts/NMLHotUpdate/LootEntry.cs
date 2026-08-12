using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;
using TWDModel;

public class LootEntry : TWDModelObject
{
	public string ComponentType;

	public LootEntryType Type { get; set; }

	public int ChallengeRoundCompletionRewardMultiplier { get; set; }

	public DropEventDefinition DropEventDefinition { get; set; }

	public DropType DropType { get; set; }

	public int TargetLevel { get; set; }

	public DropCurrenciesProbabilitiesDefinition.DropCurrency DropCurrencyType { get; set; }

	public CurrencyType RewardedCurrency { get; set; }

	public bool CanOverflowMax { get; set; }

	public int RewardedAmount { get; set; }

	public int ActualAmountAdded { get; set; }

	public Rarity RewardedRarity { get; set; }

	public int RewardedRarityLevel { get; set; }

	public int RewardedStartingLevel { get; set; }

	public SurvivorClass RewardedEquipmentClass { get; set; }

	public EquipmentItemModel GeneratedEquipment { get; set; }

	public SurvivorModel GeneratedSurvivor { get; set; }

	[IgnoreModelProperty]
	public EquipmentItemModel RewardedEquipment { get; set; }

	public int BoxIndex { get; set; }

	public string GeneratorIdentifier { get; set; }

	public ModelRandom Random { get; set; }

	public bool Opened { get; set; }

	public int Control { get; set; }

	public string ModifiedByTrait { get; set; }

	public int IconIndex { get; set; }

	public int BorderIndex { get; set; }

	public int ColorIndex { get; set; }

	public int ChallengeSkipToken { get; set; }

	public string EquipTokenId { get; set; }

	public int EquipTokenAmount { get; set; }

	public string SpRemoldSkillType { get; set; }

	public SurvivorClass SurvivorClass { get; set; }

	[JsonIgnore]
	public List<SurvivorClass> ExcludeSurvivorClasses { get; set; }

	public override void Initialize()
	{
		base.Initialize();
		ChallengeRoundCompletionRewardMultiplier = 1;
	}

	public override bool IsValid()
	{
		return true;
	}

	public bool IsComponent()
	{
		return !string.IsNullOrEmpty(ComponentType);
	}

	public void SetupAnalytics(ref Dictionary<string, string> outDictionary)
	{
		if (outDictionary != null)
		{
			if (!outDictionary.ContainsKey("loot_entry_type"))
			{
				outDictionary.Add("loot_entry_type", Type.ToString());
			}
			if (!outDictionary.ContainsKey("drop_type"))
			{
				outDictionary.Add("drop_type", DropCurrencyType.ToString());
			}
			if (!outDictionary.ContainsKey("reward_start_level"))
			{
				outDictionary.Add("reward_start_level", RewardedStartingLevel.ToString());
			}
			if (!outDictionary.ContainsKey("reward_rarity"))
			{
				outDictionary.Add("reward_rarity", ModelHelpers.GetRarityNameForAnalytics(RewardedRarityLevel));
			}
			if (!outDictionary.ContainsKey("reward_currency"))
			{
				outDictionary.Add("reward_currency", RewardedCurrency.ToString());
			}
			if (!outDictionary.ContainsKey("reward_amount"))
			{
				outDictionary.Add("reward_amount", RewardedAmount.ToString());
			}
		}
	}

	public bool IsChallengeReward()
	{
		if (Type != LootEntryType.ChallengeGuildAchiever && Type != LootEntryType.ChallengeGuildReward && Type != LootEntryType.ChallengePersonalHighScore && Type != LootEntryType.ChallengePersonalReward)
		{
			return Type == LootEntryType.ChallengeRoundCompletionReward;
		}
		return true;
	}

	public bool IsSurvivalReward()
	{
		if (Type != LootEntryType.SurvivalPersonalReward)
		{
			return Type == LootEntryType.SurvivalFullCompletionReward;
		}
		return true;
	}

	public bool IsDailyQuestReward()
	{
		return Type == LootEntryType.DailyQuest;
	}
}
