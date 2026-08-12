using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class DropEventDefinition
	{
		public enum DropEventType
		{
			MissionScavenge = 0,
			MissionRescue = 1,
			RadioPhone = 2,
			VideoAd = 3,
			WalkerTapping = 4,
			GuildGift = 5,
			TradeCrate = 6,
			IAPBonusGift = 7,
			Quiz = 8,
			MissionChallenge = 9,
			MissionSurvival = 10,
			DailyQuestChest = 11,
			Campaign = 12,
			GuildShop = 13,
			BattlePassCrate = 14,
			BeginnerBattlePassCrate = 15,
			EventWalkerTapping = 16
		}

		public enum DropEventContext
		{
			Normal = 0,
			Deadly = 1,
			ScavengeNormal = 2,
			ScavengeHard = 3
		}

		public enum DropEventTag
		{
			None = 0,
			PreferSP = 1,
			PreferSupplies = 2,
			PreferEquipment = 3,
			TradeCrateGolden = 4,
			TradeCrateSilver = 5,
			TradeCrateGearLow = 6,
			TradeCrateGearMid = 7,
			TradeCrateGearHigh = 8,
			TokenCrate = 9,
			ChallengeCrateGold = 10,
			ChallengeCrateSilver = 11,
			HeroLeader = 12,
			WasComponent = 13,
			ComponentCrate = 14,
			SurvivalCrateGold = 15,
			SurvivalCrateSilver = 16,
			QuestChestSilver = 17,
			QuestChestGold = 18,
			QuestChestClassToken = 19,
			QuestChestHeroToken = 20,
			VideoAds = 21,
			BonusCrate = 22
		}

		public DropEventType EventType;

		public DropEventContext DropContext;

		public float RegularDropProbability;

		public float SilverDropProbability;

		public float GoldDropProbability;

		public float SilverDropProbabilityIncrement;

		public float GoldDropProbabilityIncrement;

		public int ControlLevelOffset;

		public int CurrenciesAmountPercentageMultiplier;

		public FixedPoint ComponentProbabilityMultiplier;

		public int MaxNonRegularDrops;

		public DropEventTag Tag;

		[JsonIgnore]
		public FixedPoint SumOfProbabilities => RegularDropProbability + GoldDropProbability + SilverDropProbability;

		public DropEventDefinition()
		{
			MaxNonRegularDrops = 6;
		}
	}
}
