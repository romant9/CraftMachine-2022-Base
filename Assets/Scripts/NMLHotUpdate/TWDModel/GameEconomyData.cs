using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BaseModel;
using Newtonsoft.Json;
using UnityEngine;

namespace TWDModel
{
	[Serializable]
	public class GameEconomyData : IGameEconomyData
	{
		public string Id;

		public int Version;

		public bool Started;

		public List<AbilityDefinition> AbilityDefinitions;

		public List<ActorDefinition> ActorDefinitions;

		public List<CampType> CampTypes;

		[NonSerialized]
		[JsonIgnore]
		public MissionSpawnPointData MissionSpawnPointData;

		[NonSerialized]
		[JsonIgnore]
		public Dictionary<string, List<string>> GuildBattleMissionPoolDefinitionGrouped;

		[NonSerialized]
		[JsonIgnore]
		public Dictionary<string, List<GuildBattleMissionConfigBase>> GuildBattleMissionConfigsGrouped;

		[NonSerialized]
		[JsonIgnore]
		public Dictionary<string, List<FixedPoint>> GuildBattleMissionConfigsWeights;

		[NonSerialized]
		[JsonIgnore]
		public Dictionary<string, List<SurvivalMissionConfig>> GuildBattleMissionConfigPoolDefinitionGrouped;

		public TutorialDefinition Tutorial;

		public List<MissionData> MissionData;

		public Dictionary<string, MissionData> MissionDataById;

		[GEDSheet("Outfits")]
		public OutfitDefinition[] OutfitDefinitions;

		[GEDSheet("HeroSkinDefinitions")]
		public HeroSkinDefinition[] HeroSkinDefinitions;

		[GEDSheet("OutpostTiers")]
		public OutpostTier[] OutpostTiers;

		[GEDSheet("OutpostSeasons")]
		public OutpostSeason[] OutpostSeasons;

		[GEDSheet("OutpostRewards")]
		public OutpostRewardInfo[] OutpostRewards;

		[GEDSheet("AvatarsDefinition")]
		public List<AvatarsDefinition> AvatarsDefinitions;

		[GEDSheet("BordersDefinition")]
		public List<BordersDefinition> BordersDefinitions;

		[GEDSheet("AvatarColorsDefinition")]
		public List<AvatarColorsDefinition> AvatarColorsDefinitions;

		[GEDSheet("ReturnEndlessDealDefinition")]
		public List<ReturnEndlessDealDefinition> ReturnEndlessDealDefinitions;

		[GEDSheet("DifficultyIncrementalConfig")]
		public List<DifficultyIncrementalDebuff> DifficultyIncrementalConfigs;

		[GEDSheet("WeeklyChallengeApocalypseBuff")]
		public List<WeeklyChallengeApocalypseBuff> WeeklyChallengeApocalypseBuffs;

		[GEDSheet("WeeklyChallenges")]
		public List<WeeklyChallenge> WeeklyChallenges;

		private Dictionary<int, WeeklyChallenge> WeeklyChallengesById;

		[GEDSheet("WeeklyChallengeDeBuffSet")]
		public List<WeeklyChallengeDeBuffSet> WeeklyChallengeDeBuffSets;

		[GEDSheet("WeeklyChallengeApocalypseConfig")]
		public List<WeeklyChallengeApocalypseConfig> WeeklyChallengeApocalypseConfigs;

		[GEDSheet("WeeklySurvivals")]
		public List<WeeklySurvival> WeeklySurvivals;

		[GEDSheet("ScrollableMaps")]
		public ScrollableMapItem[] ScrollableMaps;

		[GEDSheet("WeeklyChallengeRewards")]
		public WeeklyChallengeReward[] WeeklyChallengeRewards;

		[GEDSheet("WeeklyChallengeRoundPassConfigs")]
		public WeeklyChallengeRoundPassConfig[] WeeklyChallengeRoundPassConfigs;

		[GEDSheet("WeeklyChallengeWarZones")]
		public WeeklyChallengeWarZone[] WeeklyChallengeWarZones;

		[GEDSheet("LastStandWarZones")]
		public LastStandWarZone[] LastStandWarZones;

		[GEDSheet("EquipPrizeWheelDefinition")]
		public EquipPrizeWheelDefinition[] EquipPrizeWheelDefinitions;

		[GEDSheet("EquipPrizeWheelRewards")]
		public EquipPrizeWheelReward[] EquipPrizeWheelRewards;

		[GEDSheet("WeeklyChallengesMapConfig")]
		public WeeklyChallengesMapConfig[] WeeklyChallengesMapConfigs;

		[GEDSheet("ClassTeamDefinition")]
		public ClassTeamDefinition[] ClassTeamDefinitions;

		[GEDSheet("ClassTeamExchangeDefinition")]
		public ClassTeamExchangeDefinition[] ClassTeamExchangeDefinitions;

		[GEDSheet("PersonalHighScoreRewards")]
		public PersonalHighScoreReward[] PersonalHighScoreRewards;

		[GEDSheet("WeeklySurvivalRewards")]
		public WeeklySurvivalReward[] WeeklySurvivalRewards;

		[GEDSheet("SurvivalMissionConfigs")]
		public SurvivalMissionConfig[] SurvivalMissionConfigs;

		[GEDSheet("SurvivalDifficultyLevels")]
		public SurvivalDifficultyLevel[] SurvivalDifficultyLevels;

		[GEDSheet("ActorLevels")]
		public ActorLevelDefinition[] ActorLevels;

		private Dictionary<string, List<ActorLevelDefinition>> ActorLevelsByActorDefinitionID;

		[GEDSheet("LevelBalanceModifiers")]
		public ActorLevelBalanceModifier[] LevelBalanceModifiers;

		[GEDSheet("RarityActorLevelModifiers")]
		public RarityActorLevelModifier[] RarityActorLevelModifiers;

		[GEDSheet("MissionCosts")]
		public MissionCost[] MissionCosts;

		[GEDSheet("BuildingTypes")]
		public BuildingType[] BuildingTypes;

		[GEDSheet("BuildingUpgradeLevels")]
		public BuildingUpgradeLevel[] BuildingUpgradeLevels;

		private Dictionary<string, List<BuildingUpgradeLevel>> BuildingUpgradeLevelsByBuildingType;

		[GEDSheet("BuildingsAmounts")]
		public BuildingsAmountsDefinition[] BuildingsAmounts;

		[GEDSheet("SteamStoreConfig")]
		public SteamStoreConfig[] SteamStoreConfigs;

		[GEDSheet("BundleDefinitions")]
		public BundleDefinition[] BundleDefinitions;

		[GEDSheet("CustomBundleDefinition")]
		public CustomBundleDefinition[] CustomBundleDefinitions;

		[GEDSheet("CustomBundleStorage")]
		public CustomBundleStorage[] CustomBundleStorages;

		[GEDSheet("SurvivorUpgrades")]
		public SurvivorUpgradeDefinition[] SurvivorUpgradeDefinitions;

		private Dictionary<SurvivorClass, List<SurvivorUpgradeDefinition>> SurvivorUpgradeDefinitionsBySurvivorClass;

		[GEDSheet("SurvivorUpgradeCosts")]
		public SurvivorUpgradeCost[] SurvivorUpgradeCosts;

		[GEDSheet("SurvivorUpgradeCostTokens")]
		public SurvivorTokenUpgradeCostDefinition[] SurvivorUpgradeCostTokens;

		[GEDSheet("EquipmentDefinitions")]
		public EquipmentDefinition[] EquipmentDefinitions;

		private Dictionary<string, EquipmentDefinition> EquipmentDefinitionsById;

		[GEDSheet("EquipmentSkillSuggestions")]
		public EquipmentSkillSuggestion[] EquipmentSkillSuggestions;

		private Dictionary<string, List<EquipmentSkillSuggestion>> EquipmentSkillSuggestionsByPackageId;

		[GEDSheet("EquipBreakthroughDefinitions")]
		public EquipBreakthroughDefinition[] EquipBreakthroughDefinitions;

		[GEDSheet("EquipBreakthroughTraits")]
		public EquipBreakthroughTrait[] EquipBreakthroughTraits;

		[GEDSheet("EquipmentLevelDefinitions")]
		public EquipmentLevelDefinition[] EquipmentLevelDefinitions;

		[GEDSheet("EquipTokenDefinitions")]
		public EquipTokenDefinition[] EquipTokenDefinitions;

		[GEDSheet("RecycleWeaponDefinitions")]
		public RecycleWeaponDefinition[] RecycleWeaponDefinitions;

		[GEDSheet("RecycleWeaponRewardDefinitions")]
		public RecycleWeaponRewardDefinition[] RecycleWeaponRewardDefinitions;

		[GEDSheet("RecycleWeaponSPSkillPackages")]
		public RecycleWeaponSPSkillPackage[] RecycleWeaponSPSkillPackages;

		private Dictionary<string, List<RecycleWeaponSPSkillPackage>> recycleWeaponSPSkillPackagesMap;

		[GEDSheet("RadioTentLevelData")]
		public RadioTentLevelData[] RadioTentLevelsData;

		[GEDSheet("RandomTalentChart")]
		public RandomTalentChart[] RandomTalentCharts;

		[GEDSheet("PhoneCallDefinition")]
		public PhoneCallDefinition[] PhoneCallDefinitions;

		private Dictionary<int, List<PhoneCallDefinition>> PhoneCallDefinitionsBySlotNumber;

		[GEDSheet("GvgSeasonDefinitions")]
		public GvGSeasonDefinition[] GvGSeasonDefinitions;

		[GEDSheet("GuildWarDefinition")]
		public GuildWarDefinition[] GuildWarDefinitions;

		[GEDSheet("GuildBattleMissionConfigs")]
		public GuildBattleMissionConfig[] GuildBattleMissionConfigs;

		[GEDSheet("GuildBattleMissionPools")]
		public GuildBattleMissionPoolDefinition[] GuildBattleMissionPoolDefinitions;

		[GEDSheet("GuildBattleSectorDefinitions")]
		public GuildBattleSectorDefinition[] GuildBattleSectorDefinitions;

		[GEDSheet("GuildBattleRewardDefinitions")]
		public GuildBattleReward[] GuildBattleRewardDefinitions;

		[GEDSheet("GuildTierDefinitions")]
		public GuildTierDefinition[] GuildTierDefinitions;

		[GEDSheet("FakeBattleDefinitions")]
		public FakeBattleDefinition[] FakeBattleDefinitions;

		[GEDSheet("GvGMissionRewardsDefinitions")]
		public GuildBattleMissionRewardsDefinition[] GvGMissionRewardsDefinitions;

		[GEDSheet("GvgMapConfig")]
		public GvgMapConfig GvgMapConfig;

		[GEDSheet("GvgMapIconConfigs")]
		public GvgMapIconConfig[] GvgMapIconConfigs;

		[GEDSheet("GvgBuffIconConfigs")]
		public GvgBuffIconConfig[] GvgBuffIconConfigs;

		[GEDSheet("GuildWarConfig")]
		public GuildWarConfig GuildWarConfig;

		[GEDSheet("RemotePushNotificationMessages")]
		public RemotePushNotificationConfig[] RemotePushNotificationConfigs;

		[GEDSheet("PhoneCallVisuals")]
		public PhoneCallVisual[] PhoneCallVisuals;

		[GEDSheet("FeaturedHeroDefinition")]
		public FeaturedHeroDefinition[] FeaturedHeroDefinitions;

		[GEDSheet("Traits")]
		public TraitDefinition[] TraitDefinitions;

		[GEDSheet("CommandSkillDefinitions")]
		public CommandSkillDefinition[] CommandSkillDefinitions;

		[GEDSheet("EquipTraits")]
		public EquipTraitsDefinition[] EquipTraitsDefinitions;

		[GEDSheet("EquipTraitsMutualExclusion")]
		public EquipTraitsMutualExclusion[] EquipTraitsMutualExclusion;

		[GEDSheet("SPTraitsRemoldDefinitions")]
		public SPTraitsRemoldDefinitions[] SPTraitsRemodeDefinition;

		public Dictionary<string, SPTraitsRemoldDefinitions> SPTraitsRemodeDefinitionByTypes;

		[GEDSheet("SPTraitsRemoldRandomPackage")]
		public SPTraitsRemoldRandomPackage[] SPTraitsRemoldRandomPackages;

		[GEDSheet("SPTraitsRemoldConfig")]
		public SPTraitsRemoldConfig SPTraitsRemoldConfigs;

		[GEDSheet("AttributeDefinition")]
		public AttributeDefinition[] AttributeDefinitions;

		[GEDSheet("TraitRerollCostDefinitions")]
		public TraitRerollCostDefinitions[] TraitRerollCosts;

		[GEDSheet("Quests")]
		public QuestDefinition[] QuestDefinitions;

		[GEDSheet("DailyQuests")]
		public DailyQuestDefinition[] DailyQuestDefinitions;

		[GEDSheet("DailyQuestSets")]
		public DailyQuestSetDefinition[] DailyQuestSetDefinitions;

		[GEDSheet("DailyQuestRewardSets")]
		public DailyQuestRewardSetDefinition[] DailyQuestRewardDefinitions;

		[GEDSheet("BroadcastsDefinition")]
		public BroadcastDefinition[] BroadcastDefinitions;

		[GEDSheet("DailyQuestChests")]
		public DailyQuestChestDefinition[] DailyQuestChestDefinitions;

		[GEDSheet("Achievements")]
		public AchievementDefinition[] AchievementDefinitions;

		private Dictionary<string, AchievementDefinition> AchievementDefinitionsById;

		[GEDSheet("MissionScoring")]
		public MissionScoringConfig[] MissionScoringConfig;

		[GEDSheet("MissionGeneration")]
		public MissionGenerationData[] MissionGenerationData;

		[GEDSheet("MissionFlavor")]
		public MissionFlavorData[] MissionFlavorData;

		[GEDSheet("MissionRewards")]
		public MissionRewards[] MissionRewards;

		[GEDSheet("Config")]
		public ConfigData ConfigData;

		[GEDSheet("RookieConfig")]
		public RookieConfigData RookieConfigData;

		[GEDSheet("SubscriptionConfig")]
		public SubscriptionConfig SubscriptionConfig;

		[GEDSheet("ConditionBundleConfig")]
		public ConditionBundleConfig ConditionBundleConfig;

		[GEDSheet("ConditionBundleDefinitions")]
		public ConditionBundleDefinition[] ConditionBundleDefinitions;

		[GEDSheet("ThreeDayDefinition")]
		public ReturnThreeDayDefinition[] ReturnThreeDayDefinitions;

		public ThreeDayDefinition[] ThreeDayDefinitions;

		[GEDSheet("NewbieSevenQuest")]
		public NewbieSevenQuest[] NewbieSevenQuests;

		[GEDSheet("NewbieStageReward")]
		public NewbieStageReward[] NewbieStageRewards;

		[GEDSheet("PlayerLevelData")]
		public PlayerLevelData[] PlayerLevelData;

		[GEDSheet("PurchaseProductsApple")]
		public InAppPurchaseProductApple[] InAppPurchaseProductsApple;

		[GEDSheet("RarityRandomization")]
		public RarityWeightData[] RarityWeightData;

		[GEDSheet("RarityBasedUpgradeDefinitions")]
		public RarityBasedUpgradeDefinition[] RarityBasedUpgradeDefinitions;

		[GEDSheet("SurvivorSlots")]
		public SurvivorSlotsData[] SurvivorSlots;

		[GEDSheet("DropEventsDefinitions")]
		public DropEventDefinition[] DropEventDefinitions;

		[GEDSheet("DropCurrenciesProbabilities")]
		public DropCurrenciesProbabilitiesDefinition[] DropCurrencyProbabilitiesDefinitions;

		[GEDSheet("DropCurrenciesStatic")]
		public DropCurrenciesStaticDefinition[] DropCurrencyStaticDefinitions;

		[GEDSheet("DropCurrenciesAmounts")]
		public DropCurrenciesAmountsDefinition[] DropCurrenciesAmountsDefinitions;

		[GEDSheet("DropRarities")]
		public DropEquipmentsAndSurvivorsRaritiesDefinition[] DropEquipmentsAndSurvivorsRaritiesDefinitions;

		[GEDSheet("DropStartingLevels")]
		public DropEquipmentsAndSurvivorsStartingLevelDefinition[] DropEquipmentsAndSurvivorsStartingLevelDefinitions;

		[GEDSheet("IncrementalDifficultyEffects")]
		public IncrementalDifficultyEffectDefinition[] IncrementalDifficultyEffects;

		[GEDSheet("TradeDefinitions")]
		public TradeDefinition[] TradeDefinitions;

		[GEDSheet("TradeSlotDefinitions")]
		public TradeSlotDefinition[] TradeSlotDefinitions;

		[GEDSheet("GuildShopDefinitions")]
		public GuildShopDefinition[] GuildShopDefinitions;

		[GEDSheet("ActiveInformationDefinitions")]
		public ActiveInformationDefinition[] ActiveInformationDefinitions;

		[GEDSheet("SystemBaseDefinitions")]
		public SystemBaseDefinition[] SystemBaseDefinitions;

		[GEDSheet("CurrencyRoundingRules")]
		public RoundingRules[] CurrencyRoundingRules;

		[GEDSheet("CageDefinition")]
		public CageDefinition[] CageDefinitions;

		[GEDSheet("OutpostTemplateDefinitions")]
		public OutpostTemplateDefinition[] OutpostTemplateDefinitions;

		[GEDSheet("AIMoveBehaviorData")]
		public AIMoveBehaviorData[] AIMoveBehaviorConfiguration;

		[GEDSheet("BundleContentDefinitions")]
		public List<BundleContentDefinition> BundleContentDefinitions;

		private Dictionary<string, BundleContentDefinition> BundleContentDefinitionsById;

		[GEDSheet("BundleStoreDefinitions")]
		public List<BundleStoreDefinition> BundleStoreDefinitions;

		private Dictionary<string, BundleStoreDefinition> BundleStoreDefinitionsById;

		[GEDSheet("BundleRotationDefinitions")]
		public BundleRotationSetupDefinition[] BundleRotationDefinitions;

		[GEDSheet("SpenderTierDefinitions")]
		public SpenderTierDefinition[] SpenderTierDefinitions;

		private Dictionary<string, SpenderTierDefinition> SpenderTierDefinitionsById;

		[GEDSheet("TradefairBundleContents")]
		public List<TradefairBundleContentDefinition> TradefairBundleContentDefinitions;

		[GEDSheet("TradefairBundleStores")]
		public List<TradefairBundleStoreDefinition> TradefairBundleStoreDefinitions;

		[GEDSheet("WeeklyClassEvents")]
		public WeeklyClassEvent[] WeeklyClassEvents;

		[GEDSheet("ProfanityFilters")]
		public ProfanityFilter[] ProfanityFilters;

		[GEDSheet("HeroTokenDropDefinitions")]
		public HeroTokenDropDefinition[] HeroTokenDropDefinitions;

		[GEDSheet("HeroTokenDistributions")]
		public HeroTokenDropDistributionDefinition[] HeroTokenDropDistributionDefinitions;

		[GEDSheet("Seasons")]
		public SeasonDefinition[] SeasonDefinitions;

		[GEDSheet("TokenToRarityAmounts")]
		public TokenToRarityAmount[] TokenToRarityAmounts;

		[GEDSheet("TokenDropAmounts")]
		public TokenDropAmount[] TokenDropAmounts;

		[GEDSheet("MissionHubContentList")]
		public MissionHubContent[] MissionHubContentList;

		[GEDSheet("GrindButtonDefinitions")]
		public GrindButtonDefinition[] GrindButtonDefinitions;

		[GEDSheet("MapDefinitions")]
		public List<MissionSpawnPointGroup> MapDefinitions;

		[GEDSheet("MissionDefinitions")]
		public MissionSpawnPoint[] MissionDefinitions;

		[GEDSheet("MissionHighlight")]
		public MissionHighlight[] MissionHighlights;

		[GEDSheet("ScavengeRewardCurrencyMultiplier")]
		public ScavengeRewardCurrencyMultiplier[] ScavengeRewardCurrencyMultipliers;

		[GEDSheet("WalkerExplosionDefinitions")]
		public WalkerExplosionDefinition[] WalkerExplosionDefinitions;

		[GEDSheet("ComponentDropTypes")]
		public ComponentDropType[] ComponentDropTypes;

		[GEDSheet("GoldShopDefinitions")]
		public List<GoldShopDefinition> GoldShopDefinitions;

		[GEDSheet("Features")]
		public Feature[] Features;

		[GEDSheet("BadgeBonusDefinitions")]
		public BadgeBonusDefinition[] BadgeBonusDefinitions;

		[GEDSheet("BadgeEffectDefinitions")]
		public BadgeEffectDefinition[] BadgeEffectDefinitions;

		[GEDSheet("BadgeEffectChances")]
		public BadgeEffectChances[] BadgeEffectChances;

		[GEDSheet("BadgeRecipes")]
		public BadgeRecipe[] BadgeRecipes;

		[GEDSheet("BadgeComponentRarityValues")]
		public BadgeComponentRarityValue[] BadgeComponentRarityValues;

		[GEDSheet("BadgeRarityResults")]
		public BadgeRarityResult[] BadgeRarityResults;

		[GEDSheet("BounsDefinitions")]
		public BounsInfoDefinition[] BounsDefinitions;

		[GEDSheet("BounsLevelDefinitions")]
		public BounsLevelDefinition[] BounsLevelDefinitions;

		[GEDSheet("BadgeRerollConfigs")]
		public BadgeRerollConfig[] BadgeRerollConfigs;

		[GEDSheet("CampaignDefinitions")]
		public CampaignDefinition[] CampaignDefinitions;

		[GEDSheet("CampaignRewardsDefinitions")]
		public CampaignRewardsDefinition[] CampaignRewardsDefinitions;

		[GEDSheet("CampaignDeeplinks")]
		public CampaignDeeplink[] CampaignDeeplinks;

		[GEDSheet("DailyLoginRewardsDefinitions")]
		public DailyLoginRewardsDefinition[] DailyLoginRewardsDefinitions;

		[GEDSheet("SevenDayConfig")]
		public SevenDayConfig SevenDayConfig;

		[GEDSheet("ReturnConfig")]
		public ReturnConfig ReturnConfig;

		[GEDSheet("ReturnLoginDefinitions")]
		public ReturnLoginDefinition[] ReturnLoginDefinitions;

		[GEDSheet("ReturnLoginRewardDefinitions")]
		public ReturnLoginRewardDefinition[] ReturnLoginRewardDefinitions;

		[GEDSheet("ReturnDailyQuest")]
		public ReturnDailyQuestDefinition[] ReturnDailyQuestDefinitions;

		[GEDSheet("ReturnRepeatQuest")]
		public ReturnRepeatQuestDefinition[] ReturnRepeatQuestDefinitions;

		[GEDSheet("RerurnExchangeStore")]
		public ReturnExchangeStoreDefinition[] ReturnExchangeStoreDefinitions;

		[GEDSheet("RouletteConfig")]
		public RouletteConfig[] RouletteConfigs;

		[GEDSheet("RouletteDefinitions")]
		public RouletteDefinition[] RouletteDefinitions;

		[GEDSheet("SevenDaysDefinitions")]
		public SevenDaysDefinition[] SevenDaysDefinitions;

		[GEDSheet("SevenDaysRewardDefinitions")]
		public SevenDaysRewardDefinition[] SevenDaysRewardDefinitions;

		[GEDSheet("ActiveFoundationConfig")]
		public ActiveFoundationConfig ActiveFoundationConfig;

		[GEDSheet("ActiveFoundationDefinitions")]
		public ActiveFoundationDefinition[] ActiveFoundationDefinitions;

		[GEDSheet("ActiveFoundationRewards")]
		public ActiveFoundationRewardDefinition[] ActiveFoundationRewardDefinitions;

		[GEDSheet("WalkerRandomizerSwaps")]
		public WalkerRandomizerSwap[] WalkerRandomizerSwaps;

		[GEDSheet("WalkerRandomizerWeights")]
		public WalkerRandomizerWeight[] WalkerRandomizerWeights;

		[GEDSheet("WebShopBundleContent")]
		public WebShopBundleContent[] WebShopBundleContents;

		[GEDSheet("ConsumablesData")]
		public ConsumablesData[] ConsumablesData;

		[GEDSheet("BlackMarketDefinitions")]
		public BlackMarketDefinition[] BlackMarketDefinitions;

		[GEDSheet("BlackMarketSlotDefinitions")]
		public BlackMarketSlotDefinition[] BlackMarketSlotDefinitions;

		[GEDSheet("BlackMarketHeroDefinitions")]
		public BlackMarketHeroDefinition[] BlackMarketHeroDefinitions;

		[GEDSheet("HillTopStoreDefinitions")]
		public HillTopStoreDefinition[] HillTopStoreDefinitions;

		[GEDSheet("HillTopStoreSlotDefinitions")]
		public HillTopStoreSlotDefinition[] HillTopStoreSlotDefinitions;

		[GEDSheet("GiftCodeDefinitions")]
		public GiftCodeDefinitionRaw[] GiftCodeDefinitions;

		[GEDSheet("DeepLinkDefinitions")]
		public DeepLinkDefinitionsRaw[] DeepLinkDefinitions;

		[GEDSheet("EndlessModeConfig")]
		public EndlessModeConfig EndlessModeConfig;

		[GEDSheet("EndlessModeExpertDebuffConfigs")]
		public EndlessModeExpertDebuffConfig[] EndlessModeExpertDebuffConfigs;

		public Dictionary<int, EndlessModeExpertDebuffConfig> EndlessModeExpertDebuffConfigById;

		[GEDSheet("EndlessModeNormalRewards")]
		public EndlessModeNormalRewardDefiniton[] EndlessModeNormalRewardDefinitons;

		[GEDSheet("EndlessModeSpawnDefinitions")]
		public EndlessModeSpawnDefinition[] EndlessModelSpawnDefinitions;

		[GEDSheet("EMSpawnCompositionDefinitions")]
		public EndlessModeSpawnCompositionDefinition[] EndlessModeSpawnCompositionDefinitions;

		[GEDSheet("EndlessModeWaveCatalog")]
		public EndlessModeWaveCatalog[] EndlessModeWaveCatalogs;

		[GEDSheet("EndlessModeScoringDefinitions")]
		public EndlessModeScoringDefinition[] EndlessModeScoringDefinitions;

		[GEDSheet("EndlessModeCalendarDefinitions")]
		public EndlessModeCalendarDefinition[] EndlessModeCalendarDefinitions;

		[GEDSheet("EndlessModeWaveRewards")]
		public EndlessModeWaveReward[] EndlessModeWaveRewards;

		[GEDSheet("EndlessModeWaveRegularRewards")]
		public EndlessModeWaveRegularReward[] EndlessModeWaveRegularRewards;

		[GEDSheet("EndlessModeLeaderBoardRewards")]
		public EndlessModeLeaderBoardReward[] EndlessModeLeaderBoardRewards;

		[GEDSheet("EMExpertHeroDefinitions")]
		public EndlessModeExpertModeHeroDefinition[] EndlessModeExpertModeHeroDefinitions;

		[GEDSheet("ChallengeNMSpawnDefinitions")]
		public ChallengeNightmareSpawnSetup[] ChallengeNightmareSpawnSetups;

		[GEDSheet("SupportDefinitions")]
		public SupportDefinitionRaw[] SupportDefinitions;

		[GEDSheet("SupportTalentTreeMain")]
		public SupportTalentTreeMainDefinition[] SupportTalentTreeMainDefinitions;

		[GEDSheet("SupportTalentTreeTrunk")]
		public SupportTalentTreeTrunkDefinition[] SupportTalentTreeTrunkDefinitions;

		[GEDSheet("SupportTalentTreeBranch")]
		public SupportTalentTreeBranchDefinition[] SupportTalentTreeBranchDefinitions;

		[GEDSheet("SupportTalentDefinitions")]
		public SupportTalentDefinition[] SupportTalentDefinitions;

		[GEDSheet("TeamPresets")]
		public TeamPresetData[] TeamPresets;

		[GEDSheet("SpeedupTokenDefinitions")]
		public SpeedupTokenDefinition SpeedupTokenDefinitions;

		[GEDSheet("SpeedupTokenTimeDefinitions")]
		public SpeedupTokenTimeDefinition[] SpeedupTokenTimeDefinitions;

		[GEDSheet("BattlePassConfig")]
		public BattlePassConfig BattlePassConfig;

		[GEDSheet("BattlePassSeasonDefinitions")]
		public BattlePassSeasonDefinition[] BattlePassSeasonDefinitions;

		[GEDSheet("BattlePassRewardDefinitions")]
		public BattlePassRewardDefinition[] BattlePassRewardDefinitions;

		[GEDSheet("BPNotificationDefinitions")]
		public BattlePassNotificationDefinition[] BattlePassNotificationDefinitions;

		[GEDSheet("BeginnerBattlePassConfig")]
		public BeginnerBattlePassConfig BeginnerBattlePassConfig;

		[GEDSheet("BeginnerBPRewardDefinitions")]
		public BattlePassRewardDefinition[] BeginnerBattlePassRewardDefinitions;

		[GEDSheet("ActivityDefinitions")]
		public ActivityDefinition[] ActivityDefinitions;

		[GEDSheet("CircularActivityDefinitions")]
		public CircularActivityDefinition[] CircularActivityDefinitions;

		[GEDSheet("CombatRevertConfig")]
		public CombatRevertConfig[] CombatRevertConfigs;

		[GEDSheet("TypeDefinition")]
		public TypeDefinition[] TypeDefinitions;

		[GEDSheet("ItemDefinition")]
		public ItemDefinition[] ItemDefinitions;

		[GEDSheet("ItemGetDefinition")]
		public ItemGetDefinition[] ItemGetDefinitions;

		[GEDSheet("AcquisitionDefinition")]
		public AcquisitionDefinition[] AcquisitionDefinitions;

		[GEDSheet("MissionEnterOrder")]
		public MissionEnterOrder[] MissionEnterOrder;

		[GEDSheet("SystemOpen")]
		public SystemOpen[] SystemOpens;

		private Dictionary<string, SystemOpen> SystemOpensById;

		[GEDSheet("SurvivalManualDefinitions")]
		public SurvivalManualDefinition[] SurvivalManualDefinitions;

		private Dictionary<int, SurvivalManualDefinition> SurvivalManualDefinitionById;

		[GEDSheet("SurvivalManualSkill")]
		public SurvivalManualSkill[] SurvivalManualSkills;

		private Dictionary<int, SurvivalManualSkill> SurvivalManualSkillByLevels;

		[GEDSheet("SurvivalManualStorySkill")]
		public SurvivalManualStorySkill[] SurvivalManualStorySkills;

		[GEDSheet("SurvivalManualActorLevel")]
		public SurvivalManualActorLevel[] SurvivalManualActorLevels;

		private Dictionary<int, List<SurvivalManualActorLevel>> SurvivalManualActorLevelsByType;

		[GEDSheet("SurvivalManualActorStory")]
		public SurvivalManualActorStory[] SurvivalManualActorStorys;

		public Dictionary<string, string> SurvivalManualActorIdMaps;

		public Dictionary<string, string> SurvivalManualStoryIdIdMaps;

		[GEDSheet("SurvivalManualAttributes")]
		public SurvivalManualAttribute[] SurvivalManualAttributes;

		private Dictionary<string, SurvivalManualAttribute> SurvivalManualAttributeById;

		[GEDSheet("AttributeUpgrade")]
		public AttributeUpgrade[] AttributeUpgrades;

		[GEDSheet("EquipmentScrapSPTokenPackage")]
		public EquipmentScrapSPTokenPackage[] EquipmentScrapSPTokenPackages;

		public Dictionary<string, List<EquipmentScrapSPTokenPackage>> EquipmentScrapSPTokenPackagesByPackageId;

		[GEDSheet("SPTraitsSkillKitTokenSet")]
		public SPTraitsSkillKitTokenSet[] SPTraitsSkillKitTokenSets;

		private Dictionary<string, int> BundleStoreDefinitionIndexCache = new Dictionary<string, int>();

		private Dictionary<string, int> BundleTradefairDefinitionIndexCache = new Dictionary<string, int>();

		private Dictionary<SurvivorClass, int> survivorClassMinimumTrainingGroundLevels;

		private IDictionary<string, GiftCodeDefinition> giftCodeDefinitions;

		private IDictionary<string, DeepLinkDefinition> deepLinkDefinitions;

		private IDictionary<int, FixedPoint> scoreMultiplierDecreaseRates;

		private IDictionary<string, SupportDefinition> supportDefinitionsMap;

		private IDictionary<int, List<SupportTalentDefinition>> supportTalentDefinitionsMap;

		private static readonly Dictionary<EquipmentType, EquipmentCategory> equipmentTypesCategoryMap = new Dictionary<EquipmentType, EquipmentCategory>
		{
			{
				EquipmentType.Shotgun,
				EquipmentCategory.RangeWeapon
			},
			{
				EquipmentType.SniperRifle,
				EquipmentCategory.RangeWeapon
			},
			{
				EquipmentType.AssaultRifle,
				EquipmentCategory.RangeWeapon
			},
			{
				EquipmentType.Pistol,
				EquipmentCategory.RangeWeapon
			},
			{
				EquipmentType.Fists,
				EquipmentCategory.MeleeWeapon
			},
			{
				EquipmentType.Knife,
				EquipmentCategory.MeleeWeapon
			},
			{
				EquipmentType.BaseballBat,
				EquipmentCategory.MeleeWeapon
			},
			{
				EquipmentType.Sword,
				EquipmentCategory.MeleeWeapon
			},
			{
				EquipmentType.Spear,
				EquipmentCategory.MeleeWeapon
			},
			{
				EquipmentType.Hammer,
				EquipmentCategory.MeleeWeapon
			},
			{
				EquipmentType.Chainsaw,
				EquipmentCategory.MeleeWeapon
			},
			{
				EquipmentType.Vest,
				EquipmentCategory.Armor
			},
			{
				EquipmentType.Helmet,
				EquipmentCategory.Armor
			},
			{
				EquipmentType.Medkit,
				EquipmentCategory.Utility
			}
		};

		private List<OutfitDefinition> AvailableOutfitDefinitions;

		private Dictionary<string, TraitDefinition> traitDefinitionIndex = new Dictionary<string, TraitDefinition>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, List<TraitDefinition>> traitDefinitionsByTag;

		private Dictionary<int, CommandSkillDefinition> commandSkillDefinitionIndex = new Dictionary<int, CommandSkillDefinition>();

		private Dictionary<SurvivorClass, List<EquipmentType>> usableEquipmentsByClass = new Dictionary<SurvivorClass, List<EquipmentType>>();

		private Dictionary<string, BundleRotationDefinition> orderedBundleRotationsDefinitions = new Dictionary<string, BundleRotationDefinition>();

		private Dictionary<WeeklyChallengeReward.ChallengeRewardType, List<WeeklyChallengeReward>> orderedWeeklyChallengeRewards = new Dictionary<WeeklyChallengeReward.ChallengeRewardType, List<WeeklyChallengeReward>>();

		private List<CurrencyType> heroTokenCurrencyTypes;

		private List<CurrencyType> classTokenCurrencyTypes;

		private GuildWarDefinition cachedNextWarDefinition;

		private List<BundleStoreDefinition> cachedOrderedStoreBundles;

		private long lastCacheTime;

		private const long CACHE_VALIDITY_DURATION = 5000L;

		private Dictionary<string, RemotePushNotificationConfig> remotePushNotificationConfigsFastLookup;

		private Dictionary<string, GvgBuffIconConfig> guildBattleBuffIcons;

		private Dictionary<int, GuildBattleMissionRewardsDefinition> GuildBattleMissionRewardFastLookup;

		private Dictionary<int, GuildBattleSectorDefinition> battlSectorsFastLookup;

		private Dictionary<int, FakeBattleDefinition> fakeBattleDefinitionsFastLookup;

		[GEDSheet("NoiseActivatedObjects")]
		public List<NoiseActivatedObjectData> NoiseActivatedObjects;

		private Dictionary<CurrencyType, ActorDefinition> tokenToActorDefinitionLookup;

		private FeaturedHeroDefinition cachedFeaturedHero;

		[GEDSheet("GoldRadioCallDenifition")]
		public GoldRadioCallDenifition[] GoldRadioCallDenifitions;

		int IGameEconomyData.Version => Version;

		public ICollection<string> SupportDefinitionIds { get; private set; }

		string IGameEconomyData.Id => Id;

		public DailyQuestSetDefinition GetDailyQuestSetDefinition(string id)
		{
			for (int i = 0; i < DailyQuestSetDefinitions.Length; i++)
			{
				if (DailyQuestSetDefinitions[i].Id == id)
				{
					return DailyQuestSetDefinitions[i];
				}
			}
			return null;
		}

		public DailyQuestDefinition GetDailyQuestDefinition(string id)
		{
			for (int i = 0; i < DailyQuestDefinitions.Length; i++)
			{
				if (DailyQuestDefinitions[i].Id == id)
				{
					return DailyQuestDefinitions[i];
				}
			}
			return null;
		}

		public NewbieSevenQuest GetNewbieSenvenQuest(int id)
		{
			for (int i = 0; i < NewbieSevenQuests.Length; i++)
			{
				if (NewbieSevenQuests[i].id == id)
				{
					return NewbieSevenQuests[i];
				}
			}
			return null;
		}

		public NewbieStageReward GetNewbieSenvenStageReward(int point)
		{
			for (int i = 0; i < NewbieStageRewards.Length; i++)
			{
				if (NewbieStageRewards[i].PointNeeded == point)
				{
					return NewbieStageRewards[i];
				}
			}
			return null;
		}

		public DailyQuestRewardSetDefinition GetDailyQuestRewardSetDefinition(string id)
		{
			for (int i = 0; i < DailyQuestRewardDefinitions.Length; i++)
			{
				if (DailyQuestRewardDefinitions[i].Id == id)
				{
					return DailyQuestRewardDefinitions[i];
				}
			}
			return null;
		}

		public BroadcastDefinition GetBroadcastDefinitionById(string id, int param = 0)
		{
			for (int i = 0; i < BroadcastDefinitions.Length; i++)
			{
				if (BroadcastDefinitions[i].EventID == id)
				{
					if (param == 0)
					{
						return BroadcastDefinitions[i];
					}
					if (BroadcastDefinitions[i].Params == param)
					{
						return BroadcastDefinitions[i];
					}
				}
			}
			return null;
		}

		public HillTopStoreDefinition GetHillTopStoreDefinition(int uniqueId)
		{
			for (int i = 0; i < HillTopStoreDefinitions.Length; i++)
			{
				if (HillTopStoreDefinitions[i].UniqueId == uniqueId)
				{
					return HillTopStoreDefinitions[i];
				}
			}
			return null;
		}

		public HillTopStoreSlotDefinition GetHillTopStoreSlotDefinition(HillTopSlotType hillTopSlotType)
		{
			for (int i = 0; i < HillTopStoreSlotDefinitions.Length; i++)
			{
				if (HillTopStoreSlotDefinitions[i].SlotType == hillTopSlotType)
				{
					return HillTopStoreSlotDefinitions[i];
				}
			}
			return null;
		}

		public EndlessModeNormalRewardDefiniton GetEndlessModeNormalRewardDefinitonById(int rewardIndex)
		{
			for (int i = 0; i < EndlessModeNormalRewardDefinitons.Length; i++)
			{
				if (EndlessModeNormalRewardDefinitons[i].RewardIndex == rewardIndex)
				{
					return EndlessModeNormalRewardDefinitons[i];
				}
			}
			return null;
		}

		public SevenDaysDefinition GetSevenDaysDefinition(int id)
		{
			for (int i = 0; i < SevenDaysDefinitions.Length; i++)
			{
				if (SevenDaysDefinitions[i].Id == id)
				{
					return SevenDaysDefinitions[i];
				}
			}
			return null;
		}

		public RouletteConfig GetRouletteConfig(int id)
		{
			if (RouletteConfigs == null)
			{
				return null;
			}
			for (int i = 0; i < RouletteConfigs.Length; i++)
			{
				if (RouletteConfigs[i] != null && RouletteConfigs[i].ID == id)
				{
					return RouletteConfigs[i];
				}
			}
			return null;
		}

		public RouletteConfig GetRouletteConfigByPeriod(int eventPeriod)
		{
			if (RouletteConfigs == null)
			{
				return null;
			}
			for (int i = 0; i < RouletteConfigs.Length; i++)
			{
				if (RouletteConfigs[i] != null && RouletteConfigs[i].EventPeriod == eventPeriod)
				{
					return RouletteConfigs[i];
				}
			}
			return null;
		}

		public RouletteConfig GetCurrentRouletteConfig(long currentUtcTime)
		{
			if (RouletteConfigs == null)
			{
				return null;
			}
			for (int i = 0; i < RouletteConfigs.Length; i++)
			{
				if (RouletteConfigs[i] != null && RouletteConfigs[i].IsActive(currentUtcTime))
				{
					return RouletteConfigs[i];
				}
			}
			return null;
		}

		public List<RouletteConfig> GetAllCurrentRouletteConfigs(long currentUtcTime, int playerLevel)
		{
			List<RouletteConfig> list = new List<RouletteConfig>();
			if (RouletteConfigs == null)
			{
				return list;
			}
			for (int i = 0; i < RouletteConfigs.Length; i++)
			{
				if (RouletteConfigs[i] != null && RouletteConfigs[i].IsActive(currentUtcTime) && (RouletteConfigs[i].OpenLevel < 0 || RouletteConfigs[i].OpenLevel <= playerLevel))
				{
					list.Add(RouletteConfigs[i]);
				}
			}
			list.Sort((RouletteConfig a, RouletteConfig b) => a.ID.CompareTo(b.ID));
			return list;
		}

		public RouletteDefinition GetRouletteDefinition(int uniqueId)
		{
			if (RouletteDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < RouletteDefinitions.Length; i++)
			{
				if (RouletteDefinitions[i] != null && RouletteDefinitions[i].UniqueId == uniqueId)
				{
					return RouletteDefinitions[i];
				}
			}
			return null;
		}

		public RouletteDefinition GetRouletteDefinition(int eventPeriod, int slotsIndex)
		{
			if (RouletteDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < RouletteDefinitions.Length; i++)
			{
				if (RouletteDefinitions[i] != null && RouletteDefinitions[i].EventPeriod == eventPeriod && RouletteDefinitions[i].SlotsIndex == slotsIndex)
				{
					return RouletteDefinitions[i];
				}
			}
			return null;
		}

		public List<RouletteDefinition> GetRouletteDefinitionsByPeriod(int eventPeriod)
		{
			List<RouletteDefinition> list = new List<RouletteDefinition>();
			if (RouletteDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < RouletteDefinitions.Length; i++)
			{
				if (RouletteDefinitions[i] != null && RouletteDefinitions[i].EventPeriod == eventPeriod)
				{
					list.Add(RouletteDefinitions[i]);
				}
			}
			return list;
		}

		private void SetupRecycleWeaponSPSkillPackages()
		{
			recycleWeaponSPSkillPackagesMap = new Dictionary<string, List<RecycleWeaponSPSkillPackage>>();
			if (RecycleWeaponSPSkillPackages == null)
			{
				return;
			}
			for (int i = 0; i < RecycleWeaponSPSkillPackages.Length; i++)
			{
				RecycleWeaponSPSkillPackage recycleWeaponSPSkillPackage = RecycleWeaponSPSkillPackages[i];
				if (recycleWeaponSPSkillPackage != null && !string.IsNullOrEmpty(recycleWeaponSPSkillPackage.PackageID))
				{
					if (!recycleWeaponSPSkillPackagesMap.ContainsKey(recycleWeaponSPSkillPackage.PackageID))
					{
						recycleWeaponSPSkillPackagesMap[recycleWeaponSPSkillPackage.PackageID] = new List<RecycleWeaponSPSkillPackage>();
					}
					recycleWeaponSPSkillPackagesMap[recycleWeaponSPSkillPackage.PackageID].Add(recycleWeaponSPSkillPackage);
				}
			}
		}

		public RecycleWeaponDefinition GetRecycleWeaponDefinition(int identifier)
		{
			if (RecycleWeaponDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < RecycleWeaponDefinitions.Length; i++)
			{
				if (RecycleWeaponDefinitions[i] != null && RecycleWeaponDefinitions[i].Identifier == identifier)
				{
					return RecycleWeaponDefinitions[i];
				}
			}
			return null;
		}

		public List<RecycleWeaponDefinition> GetActiveRecycleWeaponDefinitions(long currentUtcTime)
		{
			List<RecycleWeaponDefinition> list = new List<RecycleWeaponDefinition>();
			if (RecycleWeaponDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < RecycleWeaponDefinitions.Length; i++)
			{
				if (RecycleWeaponDefinitions[i] != null && RecycleWeaponDefinitions[i].IsActive(currentUtcTime))
				{
					list.Add(RecycleWeaponDefinitions[i]);
				}
			}
			return list;
		}

		public RecycleWeaponRewardDefinition GetRecycleWeaponRewardDefinition(int identifier, int level, int type)
		{
			if (RecycleWeaponRewardDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < RecycleWeaponRewardDefinitions.Length; i++)
			{
				if (RecycleWeaponRewardDefinitions[i] != null && RecycleWeaponRewardDefinitions[i].Identifier == identifier && RecycleWeaponRewardDefinitions[i].Level == level && RecycleWeaponRewardDefinitions[i].Type == type)
				{
					return RecycleWeaponRewardDefinitions[i];
				}
			}
			return null;
		}

		public RecycleWeaponRewardDefinition GetRecycleWeaponRewardDefinition(int identifier, int type)
		{
			if (RecycleWeaponRewardDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < RecycleWeaponRewardDefinitions.Length; i++)
			{
				if (RecycleWeaponRewardDefinitions[i] != null && RecycleWeaponRewardDefinitions[i].Identifier == identifier && RecycleWeaponRewardDefinitions[i].Type == type)
				{
					return RecycleWeaponRewardDefinitions[i];
				}
			}
			return null;
		}

		public RecycleWeaponRewardDefinition GetRecycleWeaponRewardDefinitionByLevel(int id, int level)
		{
			if (RecycleWeaponRewardDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < RecycleWeaponRewardDefinitions.Length; i++)
			{
				if (RecycleWeaponRewardDefinitions[i] != null && RecycleWeaponRewardDefinitions[i].Identifier == id && RecycleWeaponRewardDefinitions[i].Level == level)
				{
					return RecycleWeaponRewardDefinitions[i];
				}
			}
			return null;
		}

		public List<RecycleWeaponSPSkillPackage> GetRecycleWeaponSPSkillPackages(string packageId)
		{
			if (recycleWeaponSPSkillPackagesMap != null && recycleWeaponSPSkillPackagesMap.TryGetValue(packageId, out var value))
			{
				return value;
			}
			return new List<RecycleWeaponSPSkillPackage>();
		}

		public List<RouletteDefinition> GetRouletteDefinitionsByType(int eventPeriod, int rouletteType)
		{
			List<RouletteDefinition> list = new List<RouletteDefinition>();
			if (RouletteDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < RouletteDefinitions.Length; i++)
			{
				if (RouletteDefinitions[i] != null && RouletteDefinitions[i].EventPeriod == eventPeriod && RouletteDefinitions[i].RouletteType == rouletteType)
				{
					list.Add(RouletteDefinitions[i]);
				}
			}
			return list;
		}

		public List<RouletteDefinition> GetWeightedRouletteDefinitions(int eventPeriod, int currentDrawCount)
		{
			List<RouletteDefinition> list = new List<RouletteDefinition>();
			if (RouletteDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < RouletteDefinitions.Length; i++)
			{
				if (RouletteDefinitions[i] != null)
				{
					RouletteDefinition rouletteDefinition = RouletteDefinitions[i];
					if (rouletteDefinition.EventPeriod == eventPeriod && rouletteDefinition.ShouldIncludeWeight(currentDrawCount))
					{
						list.Add(rouletteDefinition);
					}
				}
			}
			return list;
		}

		public SevenDaysRewardDefinition GetSevenDaysRewardDefinition(int id)
		{
			for (int i = 0; i < SevenDaysRewardDefinitions.Length; i++)
			{
				if (SevenDaysRewardDefinitions[i].Id == id)
				{
					return SevenDaysRewardDefinitions[i];
				}
			}
			return null;
		}

		public List<SevenDaysRewardDefinition> GetSevenDaysRewardDefinitionListByPeriod(int periodId)
		{
			List<SevenDaysRewardDefinition> list = new List<SevenDaysRewardDefinition>();
			for (int i = 0; i < SevenDaysRewardDefinitions.Length; i++)
			{
				if (SevenDaysRewardDefinitions[i].PeriodId == periodId)
				{
					list.Add(SevenDaysRewardDefinitions[i]);
				}
			}
			list.OrderBy((SevenDaysRewardDefinition x) => x.Day);
			return list;
		}

		public SevenDaysRewardDefinition GetSevenDaysRewardDefinitionByPeriodDay(int periodId, int day)
		{
			for (int i = 0; i < SevenDaysRewardDefinitions.Length; i++)
			{
				if (SevenDaysRewardDefinitions[i].PeriodId == periodId && SevenDaysRewardDefinitions[i].Day == day)
				{
					return SevenDaysRewardDefinitions[i];
				}
			}
			return null;
		}

		public ReturnLoginDefinition GetReturnLoginDefinition(int id)
		{
			if (ReturnLoginDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < ReturnLoginDefinitions.Length; i++)
			{
				if (ReturnLoginDefinitions[i].Id == id)
				{
					return ReturnLoginDefinitions[i];
				}
			}
			return null;
		}

		public ReturnLoginDefinition GetReturnLoginDefinitionByCouncilLevel(int councilLevel)
		{
			if (ReturnLoginDefinitions == null)
			{
				return null;
			}
			ReturnLoginDefinition returnLoginDefinition = null;
			for (int i = 0; i < ReturnLoginDefinitions.Length; i++)
			{
				ReturnLoginDefinition returnLoginDefinition2 = ReturnLoginDefinitions[i];
				if (returnLoginDefinition2 != null && councilLevel >= returnLoginDefinition2.CouncilLevelMin && councilLevel <= returnLoginDefinition2.CouncilLevelMax && (returnLoginDefinition == null || returnLoginDefinition2.Id > returnLoginDefinition.Id))
				{
					returnLoginDefinition = returnLoginDefinition2;
				}
			}
			return returnLoginDefinition;
		}

		public ReturnLoginRewardDefinition GetReturnLoginRewardDefinition(int id)
		{
			if (ReturnLoginRewardDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < ReturnLoginRewardDefinitions.Length; i++)
			{
				if (ReturnLoginRewardDefinitions[i].Id == id)
				{
					return ReturnLoginRewardDefinitions[i];
				}
			}
			return null;
		}

		public List<ReturnLoginRewardDefinition> GetReturnLoginRewardDefinitions(int returnLoginId)
		{
			List<ReturnLoginRewardDefinition> list = new List<ReturnLoginRewardDefinition>();
			if (ReturnLoginRewardDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < ReturnLoginRewardDefinitions.Length; i++)
			{
				ReturnLoginRewardDefinition returnLoginRewardDefinition = ReturnLoginRewardDefinitions[i];
				if (returnLoginRewardDefinition != null && returnLoginRewardDefinition.ReturnLoginId == returnLoginId)
				{
					list.Add(returnLoginRewardDefinition);
				}
			}
			return list.OrderBy((ReturnLoginRewardDefinition x) => x.Day).ToList();
		}

		public List<ReturnDailyQuestDefinition> GetReturnDailyQuestDefinitions(int councilLevel)
		{
			List<ReturnDailyQuestDefinition> list = new List<ReturnDailyQuestDefinition>();
			if (ReturnDailyQuestDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < ReturnDailyQuestDefinitions.Length; i++)
			{
				ReturnDailyQuestDefinition returnDailyQuestDefinition = ReturnDailyQuestDefinitions[i];
				if (returnDailyQuestDefinition != null && IsReturnCouncilLevelMatched(councilLevel, returnDailyQuestDefinition.CouncilLevelMin, returnDailyQuestDefinition.CouncilLevelMax))
				{
					list.Add(returnDailyQuestDefinition);
				}
			}
			return list;
		}

		public List<ReturnDailyQuestDefinition> GetReturnDailyQuestDefinitions(int councilLevel, int group)
		{
			List<ReturnDailyQuestDefinition> list = new List<ReturnDailyQuestDefinition>();
			List<ReturnDailyQuestDefinition> returnDailyQuestDefinitions = GetReturnDailyQuestDefinitions(councilLevel);
			for (int i = 0; i < returnDailyQuestDefinitions.Count; i++)
			{
				if (returnDailyQuestDefinitions[i].Group == group)
				{
					list.Add(returnDailyQuestDefinitions[i]);
				}
			}
			return list;
		}

		public ReturnDailyQuestDefinition GetReturnDailyQuestDefinition(int id)
		{
			if (ReturnDailyQuestDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < ReturnDailyQuestDefinitions.Length; i++)
			{
				if (ReturnDailyQuestDefinitions[i] != null && ReturnDailyQuestDefinitions[i].Id == id)
				{
					return ReturnDailyQuestDefinitions[i];
				}
			}
			return null;
		}

		public List<ReturnRepeatQuestDefinition> GetReturnRepeatQuestDefinitions(int councilLevel)
		{
			List<ReturnRepeatQuestDefinition> list = new List<ReturnRepeatQuestDefinition>();
			if (ReturnRepeatQuestDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < ReturnRepeatQuestDefinitions.Length; i++)
			{
				ReturnRepeatQuestDefinition returnRepeatQuestDefinition = ReturnRepeatQuestDefinitions[i];
				if (returnRepeatQuestDefinition != null && councilLevel >= Math.Max(returnRepeatQuestDefinition.CouncilLevelMin, 0))
				{
					list.Add(returnRepeatQuestDefinition);
				}
			}
			return list;
		}

		public ReturnRepeatQuestDefinition GetReturnRepeatQuestDefinition(int id)
		{
			if (ReturnRepeatQuestDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < ReturnRepeatQuestDefinitions.Length; i++)
			{
				if (ReturnRepeatQuestDefinitions[i] != null && ReturnRepeatQuestDefinitions[i].Id == id)
				{
					return ReturnRepeatQuestDefinitions[i];
				}
			}
			return null;
		}

		public List<ReturnExchangeStoreDefinition> GetReturnExchangeStoreDefinitions(int councilLevel)
		{
			List<ReturnExchangeStoreDefinition> list = new List<ReturnExchangeStoreDefinition>();
			if (ReturnExchangeStoreDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < ReturnExchangeStoreDefinitions.Length; i++)
			{
				ReturnExchangeStoreDefinition returnExchangeStoreDefinition = ReturnExchangeStoreDefinitions[i];
				if (returnExchangeStoreDefinition != null && IsReturnCouncilLevelMatched(councilLevel, returnExchangeStoreDefinition.CouncilLevelMin, returnExchangeStoreDefinition.CouncilLevelMax))
				{
					list.Add(returnExchangeStoreDefinition);
				}
			}
			return list.OrderBy((ReturnExchangeStoreDefinition x) => x.Id).ToList();
		}

		public ReturnExchangeStoreDefinition GetReturnExchangeStoreDefinition(int id)
		{
			if (ReturnExchangeStoreDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < ReturnExchangeStoreDefinitions.Length; i++)
			{
				if (ReturnExchangeStoreDefinitions[i] != null && ReturnExchangeStoreDefinitions[i].Id == id)
				{
					return ReturnExchangeStoreDefinitions[i];
				}
			}
			return null;
		}

		public ActiveFoundationDefinition GetActiveFoundationDefinition(int id)
		{
			for (int i = 0; i < ActiveFoundationDefinitions.Length; i++)
			{
				if (ActiveFoundationDefinitions[i].Id == id)
				{
					return ActiveFoundationDefinitions[i];
				}
			}
			return null;
		}

		public ActiveFoundationRewardDefinition GetActiveFoundationRewardDefinition(int id)
		{
			for (int i = 0; i < ActiveFoundationRewardDefinitions.Length; i++)
			{
				if (ActiveFoundationRewardDefinitions[i].Id == id)
				{
					return ActiveFoundationRewardDefinitions[i];
				}
			}
			return null;
		}

		public List<ActiveFoundationRewardDefinition> GetActiveFoundationRewardDefinitionListByPeriod(int periodId)
		{
			List<ActiveFoundationRewardDefinition> list = new List<ActiveFoundationRewardDefinition>();
			for (int i = 0; i < ActiveFoundationRewardDefinitions.Length; i++)
			{
				if (ActiveFoundationRewardDefinitions[i].PeriodId == periodId)
				{
					list.Add(ActiveFoundationRewardDefinitions[i]);
				}
			}
			list.OrderBy((ActiveFoundationRewardDefinition x) => x.Day);
			return list;
		}

		public ActiveFoundationRewardDefinition GetActiveFoundationRewardDefinitionByPeriodDay(int periodId, int day)
		{
			for (int i = 0; i < ActiveFoundationRewardDefinitions.Length; i++)
			{
				if (ActiveFoundationRewardDefinitions[i].PeriodId == periodId && ActiveFoundationRewardDefinitions[i].Day == day)
				{
					return ActiveFoundationRewardDefinitions[i];
				}
			}
			return null;
		}

		public SupportTalentTreeMainDefinition GetSupportTalentTreeMainDefinitionById(int id)
		{
			for (int i = 0; i < SupportTalentTreeMainDefinitions.Length; i++)
			{
				if (SupportTalentTreeMainDefinitions[i].Id == id)
				{
					return SupportTalentTreeMainDefinitions[i];
				}
			}
			return null;
		}

		public SupportTalentTreeTrunkDefinition GetSupportTalentTreeTrunkDefinitionByTrunkId(int trunkId)
		{
			for (int i = 0; i < SupportTalentTreeTrunkDefinitions.Length; i++)
			{
				if (SupportTalentTreeTrunkDefinitions[i].TrunkId == trunkId)
				{
					return SupportTalentTreeTrunkDefinitions[i];
				}
			}
			return null;
		}

		public SupportTalentTreeTrunkDefinition GetSupportTalentTreeTrunkDefinitionByRequireTrunkId(int requireTrunkId)
		{
			for (int i = 0; i < SupportTalentTreeTrunkDefinitions.Length; i++)
			{
				if (SupportTalentTreeTrunkDefinitions[i].RequireTrunkId == requireTrunkId)
				{
					return SupportTalentTreeTrunkDefinitions[i];
				}
			}
			return null;
		}

		public List<SupportTalentTreeTrunkDefinition> GetSupportTalentTreeTrunkDefinitionsByTreeId(int treeId)
		{
			List<SupportTalentTreeTrunkDefinition> list = new List<SupportTalentTreeTrunkDefinition>();
			for (int i = 0; i < SupportTalentTreeTrunkDefinitions.Length; i++)
			{
				if (SupportTalentTreeTrunkDefinitions[i].TreeId == treeId)
				{
					list.Add(SupportTalentTreeTrunkDefinitions[i]);
				}
			}
			return list;
		}

		public SupportTalentTreeBranchDefinition GetSupportTalentTreeBranchDefinitionByBranchId(int branchId)
		{
			for (int i = 0; i < SupportTalentTreeBranchDefinitions.Length; i++)
			{
				if (SupportTalentTreeBranchDefinitions[i].BranchId == branchId)
				{
					return SupportTalentTreeBranchDefinitions[i];
				}
			}
			return null;
		}

		public List<SupportTalentTreeBranchDefinition> GetSupportTalentTreeBranchDefinitionsByTreeId(int treeId)
		{
			List<SupportTalentTreeBranchDefinition> list = new List<SupportTalentTreeBranchDefinition>();
			for (int i = 0; i < SupportTalentTreeBranchDefinitions.Length; i++)
			{
				if (SupportTalentTreeBranchDefinitions[i].TreeId == treeId)
				{
					list.Add(SupportTalentTreeBranchDefinitions[i]);
				}
			}
			return list;
		}

		public SupportTalentDefinition GetSupportTalentDefinitionByTalentIdAndLevel(int talentId, int level)
		{
			if (!supportTalentDefinitionsMap.TryGetValue(talentId, out var value))
			{
				return null;
			}
			return value.Find((SupportTalentDefinition x) => x.Level == level);
		}

		public SupportTalentDefinition GetSupportTalentDefinitionById(int id)
		{
			for (int i = 0; i < SupportTalentDefinitions.Length; i++)
			{
				if (SupportTalentDefinitions[i].Id == id)
				{
					return SupportTalentDefinitions[i];
				}
			}
			return null;
		}

		public bool IsBattlePassSeasonBundleIdentifier(string bundleIdentifier)
		{
			for (int i = 0; i < BattlePassSeasonDefinitions.Length; i++)
			{
				if (BattlePassSeasonDefinitions[i].BundleIdentifier == bundleIdentifier)
				{
					return true;
				}
			}
			return false;
		}

		public OutpostTemplateDefinition GetOutpostTemplateDefinitionForMissionId(string id)
		{
			for (int i = 0; i < OutpostTemplateDefinitions.Length; i++)
			{
				if (OutpostTemplateDefinitions[i].MissionID == id)
				{
					return OutpostTemplateDefinitions[i];
				}
			}
			return null;
		}

		public OutpostTemplateDefinition GetOutpostTemplateDefinition(string id)
		{
			for (int i = 0; i < OutpostTemplateDefinitions.Length; i++)
			{
				if (OutpostTemplateDefinitions[i].Id == id)
				{
					return OutpostTemplateDefinitions[i];
				}
			}
			return null;
		}

		public CageDefinition GetCageDefinition(string walkerId, int level)
		{
			for (int i = 0; i < CageDefinitions.Length; i++)
			{
				if (CageDefinitions[i].WalkerId == walkerId && CageDefinitions[i].Level == level)
				{
					return CageDefinitions[i];
				}
			}
			return null;
		}

		public int GetMaxCageWalkerLevel(string walkerId)
		{
			int num = 0;
			CageDefinition[] cageDefinitions = CageDefinitions;
			foreach (CageDefinition cageDefinition in cageDefinitions)
			{
				if (cageDefinition.WalkerId == walkerId && cageDefinition.Level > num)
				{
					num = cageDefinition.Level;
				}
			}
			return num;
		}

		public int GetMaxCageWalkerAmount(string walkerId)
		{
			int num = 0;
			CageDefinition[] cageDefinitions = CageDefinitions;
			foreach (CageDefinition cageDefinition in cageDefinitions)
			{
				if (cageDefinition.WalkerId == walkerId && cageDefinition.Level > num && cageDefinition.CostAmountOuptost != 0)
				{
					num = cageDefinition.Level;
				}
			}
			return num;
		}

		public OutpostRewardInfo GetOutpostReward(int level, OutpostRewardLevelType levelType)
		{
			if (level < 1)
			{
				return null;
			}
			int num = -1;
			for (int i = 0; i < OutpostRewards.Length; i++)
			{
				if (OutpostRewards[i].LevelType == levelType)
				{
					num = i;
					if (OutpostRewards[i].Level == level)
					{
						return OutpostRewards[i];
					}
				}
			}
			if (num != -1 && level > OutpostRewards[num].Level)
			{
				return OutpostRewards[num];
			}
			return null;
		}

		public List<OutpostTier> GetOutpostTiers(int tierSetId)
		{
			List<OutpostTier> list = new List<OutpostTier>();
			if (OutpostTiers != null)
			{
				for (int i = 0; i < OutpostTiers.Length; i++)
				{
					if (OutpostTiers[i].TierSetId == tierSetId)
					{
						list.Add(OutpostTiers[i]);
					}
				}
			}
			return list;
		}

		public OutpostTier GetOutpostTierById(string id, int tierSetId)
		{
			if (OutpostTiers != null)
			{
				for (int i = 0; i < OutpostTiers.Length; i++)
				{
					if (OutpostTiers[i].TierSetId == tierSetId && id.Equals(OutpostTiers[i].Id, StringComparison.InvariantCultureIgnoreCase))
					{
						return OutpostTiers[i];
					}
				}
			}
			return null;
		}

		public OutpostTier GetOutpostInfluenceTier(int influence, int tierSetId)
		{
			if (OutpostTiers != null)
			{
				for (int i = 0; i < OutpostTiers.Length; i++)
				{
					if (OutpostTiers[i].TierSetId == tierSetId && influence >= OutpostTiers[i].MinInfluence && influence <= OutpostTiers[i].MaxInfluence)
					{
						return OutpostTiers[i];
					}
				}
			}
			return null;
		}

		public OutpostTier GetOutpostRankTier(int rank, int tierSetId)
		{
			OutpostTier outpostTier = null;
			if (OutpostTiers != null)
			{
				for (int i = 0; i < OutpostTiers.Length; i++)
				{
					if (OutpostTiers[i].TierSetId == tierSetId && rank <= OutpostTiers[i].Rank && (outpostTier == null || OutpostTiers[i].Rank < outpostTier.Rank))
					{
						outpostTier = OutpostTiers[i];
					}
				}
			}
			return outpostTier;
		}

		public OutpostSeason GetOutpostSeason(long timeMillis)
		{
			if (OutpostSeasons != null)
			{
				for (int i = 0; i < OutpostSeasons.Length; i++)
				{
					if (timeMillis >= OutpostSeasons[i].StartTimeMilliseconds && timeMillis <= OutpostSeasons[i].EndTimeMilliseconds)
					{
						return OutpostSeasons[i];
					}
				}
			}
			return null;
		}

		public OutpostSeason GetPreviousOutpostSeason(long timeMillis)
		{
			if (OutpostSeasons != null)
			{
				for (int i = 0; i < OutpostSeasons.Length; i++)
				{
					if (timeMillis <= OutpostSeasons[i].EndTimeMilliseconds)
					{
						if (i <= 0)
						{
							return null;
						}
						return OutpostSeasons[i - 1];
					}
				}
			}
			return null;
		}

		public OutpostSeason GetNextOutpostSeason(long timeMillis)
		{
			if (OutpostSeasons != null)
			{
				OutpostSeason outpostSeason = null;
				for (int i = 0; i < OutpostSeasons.Length; i++)
				{
					if (timeMillis < OutpostSeasons[i].StartTimeMilliseconds && (outpostSeason == null || OutpostSeasons[i].StartTimeMilliseconds < outpostSeason.StartTimeMilliseconds))
					{
						outpostSeason = OutpostSeasons[i];
					}
				}
				return outpostSeason;
			}
			return null;
		}

		public List<OutpostSeason> GetOutpostSeasons(int id, long timeMillis)
		{
			if (OutpostSeasons != null)
			{
				bool flag = false;
				List<OutpostSeason> list = new List<OutpostSeason>();
				for (int i = 0; i < OutpostSeasons.Length; i++)
				{
					if (OutpostSeasons[i].Id == id)
					{
						flag = true;
					}
					else if (flag)
					{
						if (OutpostSeasons[i].EndTimeMilliseconds >= timeMillis)
						{
							break;
						}
						list.Add(OutpostSeasons[i]);
					}
				}
				return list;
			}
			return null;
		}

		public OutpostSeason GetOutpostSeasonById(int id)
		{
			if (OutpostSeasons != null)
			{
				for (int i = 0; i < OutpostSeasons.Length; i++)
				{
					if (id == OutpostSeasons[i].Id)
					{
						return OutpostSeasons[i];
					}
				}
			}
			return null;
		}

		public AvatarsDefinition GetAvatarsDefinition(int index)
		{
			for (int i = 0; i < AvatarsDefinitions.Count; i++)
			{
				if (AvatarsDefinitions[i].Index == index)
				{
					return AvatarsDefinitions[i];
				}
			}
			return null;
		}

		public BordersDefinition GetBordersDefinition(int index)
		{
			for (int i = 0; i < BordersDefinitions.Count; i++)
			{
				if (BordersDefinitions[i].Index == index)
				{
					return BordersDefinitions[i];
				}
			}
			return null;
		}

		public AvatarColorsDefinition GetAvatarColorsDefinition(int index)
		{
			for (int i = 0; i < AvatarColorsDefinitions.Count; i++)
			{
				if (AvatarColorsDefinitions[i].Index == index)
				{
					return AvatarColorsDefinitions[i];
				}
			}
			return null;
		}

		public List<WeeklyChallengeApocalypseBuff> GetCanRandomChallengeApocalypseBuff(string group, List<string> ExpectId)
		{
			return WeeklyChallengeApocalypseBuffs.Where((WeeklyChallengeApocalypseBuff x) => x.Group.Contains(group) && (x.Conflict == null || !x.Conflict.Intersect(ExpectId).Any())).ToList();
		}

		public DifficultyIncrementalDebuff GetChallengeDebuff(string identifier)
		{
			for (int i = 0; i < DifficultyIncrementalConfigs.Count; i++)
			{
				if (DifficultyIncrementalConfigs[i].Identifier == identifier)
				{
					return DifficultyIncrementalConfigs[i];
				}
			}
			return null;
		}

		public WeeklyChallenge GetWeeklyChallenge(int id)
		{
			if (WeeklyChallengesById.TryGetValue(id, out var value))
			{
				return value;
			}
			return null;
		}

		public List<ClassTeamExchangeDefinition> GetWeeklyChallengeClassTeamChallengeExchanges(int challengeId)
		{
			List<ClassTeamExchangeDefinition> list = new List<ClassTeamExchangeDefinition>();
			if (ClassTeamExchangeDefinitions == null)
			{
				return list;
			}
			for (int i = 0; i < ClassTeamExchangeDefinitions.Length; i++)
			{
				ClassTeamExchangeDefinition classTeamExchangeDefinition = ClassTeamExchangeDefinitions[i];
				if (classTeamExchangeDefinition != null && classTeamExchangeDefinition.ChallengeID == challengeId)
				{
					list.Add(classTeamExchangeDefinition);
				}
			}
			return list;
		}

		public WeeklyChallengeDeBuffSet GetWeeklyChallengeDeBuffSet(int id)
		{
			if (WeeklyChallengeDeBuffSets == null)
			{
				return null;
			}
			for (int i = 0; i < WeeklyChallengeDeBuffSets.Count; i++)
			{
				if (WeeklyChallengeDeBuffSets[i].Round == id)
				{
					return WeeklyChallengeDeBuffSets[i];
				}
			}
			return null;
		}

		public WeeklyChallengeApocalypseConfig GetWeeklyChallengeCircle(int id)
		{
			if (WeeklyChallengeApocalypseConfigs == null)
			{
				return null;
			}
			for (int i = 0; i < WeeklyChallengeApocalypseConfigs.Count; i++)
			{
				if (WeeklyChallengeApocalypseConfigs[i].Round == id)
				{
					return WeeklyChallengeApocalypseConfigs[i];
				}
			}
			return null;
		}

		public WeeklyChallenge GetNextWeeklyChallege(long currentEndTime, long playerLifeTime)
		{
			for (int i = 0; i < WeeklyChallenges.Count; i++)
			{
				WeeklyChallenge weeklyChallenge = WeeklyChallenges[i];
				if (weeklyChallenge != null && currentEndTime < weeklyChallenge.StartTimeMilliseconds && playerLifeTime < weeklyChallenge.EndTimeMilliseconds)
				{
					return weeklyChallenge;
				}
			}
			return null;
		}

		public WeeklyChallenge GetWeeklyChallengePlayableWhen(long playerLifeTime, long timeWindowLength)
		{
			long num = playerLifeTime + timeWindowLength;
			for (int i = 0; i < WeeklyChallenges.Count; i++)
			{
				WeeklyChallenge weeklyChallenge = WeeklyChallenges[i];
				if (weeklyChallenge != null && num < weeklyChallenge.EndTimeMilliseconds && playerLifeTime > weeklyChallenge.StartTimeMilliseconds)
				{
					return weeklyChallenge;
				}
			}
			return null;
		}

		public WeeklySurvival GetSurvivalPlayableWhen(long playerLifeTime, long timeWindowLength)
		{
			long num = playerLifeTime + timeWindowLength;
			for (int i = 0; i < WeeklySurvivals.Count; i++)
			{
				WeeklySurvival weeklySurvival = WeeklySurvivals[i];
				if (weeklySurvival != null && num < weeklySurvival.EndTimeMilliseconds && playerLifeTime > weeklySurvival.StartTimeMilliseconds)
				{
					return weeklySurvival;
				}
			}
			return null;
		}

		public WeeklyChallengeRoundPassConfig GetChallengeRoundPassConfig(int currentRound)
		{
			if (WeeklyChallengeRoundPassConfigs == null || WeeklyChallengeRoundPassConfigs.Length == 0)
			{
				return null;
			}
			WeeklyChallengeRoundPassConfig result = null;
			WeeklyChallengeRoundPassConfig[] weeklyChallengeRoundPassConfigs = WeeklyChallengeRoundPassConfigs;
			foreach (WeeklyChallengeRoundPassConfig weeklyChallengeRoundPassConfig in weeklyChallengeRoundPassConfigs)
			{
				if (currentRound < weeklyChallengeRoundPassConfig.FromRound)
				{
					break;
				}
				result = weeklyChallengeRoundPassConfig;
			}
			return result;
		}

		public WeeklyChallengeWarZone GetWeeklyChallengeWarZoneByCouncilLevel(int councilLevel)
		{
			if (WeeklyChallengeWarZones == null || WeeklyChallengeWarZones.Length == 0)
			{
				return null;
			}
			WeeklyChallengeWarZone result = null;
			WeeklyChallengeWarZone[] weeklyChallengeWarZones = WeeklyChallengeWarZones;
			foreach (WeeklyChallengeWarZone weeklyChallengeWarZone in weeklyChallengeWarZones)
			{
				if (weeklyChallengeWarZone.CouncilLevelRange[0] <= councilLevel && councilLevel <= weeklyChallengeWarZone.CouncilLevelRange[1])
				{
					result = weeklyChallengeWarZone;
					break;
				}
			}
			return result;
		}

		public int GetMapIdByDifficulty(int Identifier, int difficulty)
		{
			WeeklyChallengesMapConfig weeklyChallengesMapConfig = null;
			if (WeeklyChallengesMapConfigs != null && WeeklyChallengesMapConfigs.Length != 0)
			{
				weeklyChallengesMapConfig = WeeklyChallengesMapConfigs.FirstOrDefault((WeeklyChallengesMapConfig c) => c.ApocalypticMap == Identifier && c.ContainsDifficulty(difficulty));
			}
			return weeklyChallengesMapConfig?.MapID ?? Identifier;
		}

		public ClassTeamDefinition GetClassTeamDefinition(int challengeID)
		{
			if (ClassTeamDefinitions == null || ClassTeamDefinitions.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < ClassTeamDefinitions.Length; i++)
			{
				if (ClassTeamDefinitions[i].ChallengeID == challengeID)
				{
					return ClassTeamDefinitions[i];
				}
			}
			return null;
		}

		public ClassTeamDefinition GetCurrentClassTeamDefinition(long now)
		{
			if (ClassTeamDefinitions == null || ClassTeamDefinitions.Length == 0)
			{
				return null;
			}
			ClassTeamDefinition classTeamDefinition = null;
			for (int i = 0; i < ClassTeamDefinitions.Length; i++)
			{
				ClassTeamDefinition classTeamDefinition2 = ClassTeamDefinitions[i];
				if (classTeamDefinition2 != null && now >= classTeamDefinition2.StartTimeMilliseconds && now < classTeamDefinition2.EndTimeMilliseconds && (classTeamDefinition == null || classTeamDefinition2.ChallengeID > classTeamDefinition.ChallengeID))
				{
					classTeamDefinition = classTeamDefinition2;
				}
			}
			return classTeamDefinition;
		}

		public LastStandWarZone GetLastStandWarZoneByCouncilLevel(int councilLevel)
		{
			if (LastStandWarZones == null || LastStandWarZones.Length == 0)
			{
				return null;
			}
			LastStandWarZone result = null;
			LastStandWarZone[] lastStandWarZones = LastStandWarZones;
			foreach (LastStandWarZone lastStandWarZone in lastStandWarZones)
			{
				if (lastStandWarZone.CouncilLevelRange[0] <= councilLevel && councilLevel <= lastStandWarZone.CouncilLevelRange[1])
				{
					result = lastStandWarZone;
					break;
				}
			}
			return result;
		}

		public WeeklySurvival GetWeeklySurvival(int id)
		{
			for (int i = 0; i < WeeklySurvivals.Count; i++)
			{
				if (WeeklySurvivals[i].Identifier == id)
				{
					return WeeklySurvivals[i];
				}
			}
			return null;
		}

		public WeeklySurvival GetNextWeeklySurvival(long currentEndTime, long playerLifeTime)
		{
			for (int i = 0; i < WeeklySurvivals.Count; i++)
			{
				WeeklySurvival weeklySurvival = WeeklySurvivals[i];
				if (weeklySurvival != null && currentEndTime < weeklySurvival.StartTimeMilliseconds && playerLifeTime < weeklySurvival.EndTimeMilliseconds)
				{
					return weeklySurvival;
				}
			}
			return null;
		}

		public GvGSeasonDefinition FindGvGSeasonDefinition(int id)
		{
			if (GvGSeasonDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < GvGSeasonDefinitions.Length; i++)
			{
				if (GvGSeasonDefinitions[i].Identifier == id)
				{
					return GvGSeasonDefinitions[i];
				}
			}
			return null;
		}

		public GvGSeasonDefinition FindGvGSeasonWithTime(long utcTimeStamp)
		{
			if (GvGSeasonDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < GvGSeasonDefinitions.Length; i++)
			{
				GvGSeasonDefinition gvGSeasonDefinition = GvGSeasonDefinitions[i];
				if (gvGSeasonDefinition != null && gvGSeasonDefinition.IsOpen(utcTimeStamp))
				{
					return gvGSeasonDefinition;
				}
			}
			return null;
		}

		public GvGSeasonDefinition FindNextGvGSeason(long currentEndTime, long playerLifeTime)
		{
			if (GvGSeasonDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < GvGSeasonDefinitions.Length; i++)
			{
				GvGSeasonDefinition gvGSeasonDefinition = GvGSeasonDefinitions[i];
				if (gvGSeasonDefinition != null && currentEndTime < gvGSeasonDefinition.StartTimeMilliseconds && playerLifeTime < gvGSeasonDefinition.EndTimeMilliseconds)
				{
					return gvGSeasonDefinition;
				}
			}
			return null;
		}

		public GvGSeasonDefinition FindLastStartedSeason(long timeStamp)
		{
			GvGSeasonDefinition gvGSeasonDefinition = null;
			if (GvGSeasonDefinitions != null && GvGSeasonDefinitions.Length != 0)
			{
				int num = GvGSeasonDefinitions.Length;
				int num2 = 0;
				gvGSeasonDefinition = GvGSeasonDefinitions[num2];
				while (gvGSeasonDefinition.EndTimeMilliseconds < timeStamp && num2 < num)
				{
					if (GvGSeasonDefinitions[num2].StartTimeMilliseconds > timeStamp)
					{
						return gvGSeasonDefinition;
					}
					gvGSeasonDefinition = GvGSeasonDefinitions[num2];
					num2++;
				}
			}
			return gvGSeasonDefinition;
		}

		public List<GuildWarDefinition> FindGuildWarDefinitionInSeason(int seasonId)
		{
			if (GvGSeasonDefinitions == null)
			{
				return null;
			}
			GvGSeasonDefinition gvGSeasonDefinition = FindGvGSeasonDefinition(seasonId);
			if (gvGSeasonDefinition == null)
			{
				return null;
			}
			List<GuildWarDefinition> list = new List<GuildWarDefinition>();
			for (int i = 0; i < GuildWarDefinitions.Length; i++)
			{
				if (GuildWarDefinitions[i].StartTimeMilliseconds >= gvGSeasonDefinition.StartTimeMilliseconds && GuildWarDefinitions[i].EndTimeMilliseconds <= gvGSeasonDefinition.EndTimeMilliseconds)
				{
					list.Add(GuildWarDefinitions[i]);
				}
			}
			return list;
		}

		public GuildWarDefinition FindGuildWarWithId(int id)
		{
			for (int i = 0; i < GuildWarDefinitions.Length; i++)
			{
				if (GuildWarDefinitions[i].Identifier == id)
				{
					return GuildWarDefinitions[i];
				}
			}
			return null;
		}

		public GuildWarDefinition FindGuildWarWithTime(long utcTimeStamp)
		{
			for (int i = 0; i < GuildWarDefinitions.Length; i++)
			{
				GuildWarDefinition guildWarDefinition = GuildWarDefinitions[i];
				if (guildWarDefinition != null && guildWarDefinition.IsOpen(utcTimeStamp))
				{
					return guildWarDefinition;
				}
			}
			return null;
		}

		public GuildWarDefinition FindNextGuildWar(long currentEndTime, long playerLifeTime)
		{
			if (IsNextWarDefinition(cachedNextWarDefinition, currentEndTime, playerLifeTime))
			{
				return cachedNextWarDefinition;
			}
			for (int i = 0; i < GuildWarDefinitions.Length; i++)
			{
				GuildWarDefinition guildWarDefinition = GuildWarDefinitions[i];
				if (IsNextWarDefinition(guildWarDefinition, currentEndTime, playerLifeTime))
				{
					cachedNextWarDefinition = guildWarDefinition;
					return guildWarDefinition;
				}
			}
			return null;
		}

		public void ClearCachedNextWarDefinition()
		{
			cachedNextWarDefinition = null;
		}

		public static bool IsNextWarDefinition(GuildWarDefinition definition, long currentEndTime, long playerLifeTime)
		{
			if (definition == null)
			{
				return false;
			}
			if (currentEndTime < definition.StartTimeMilliseconds)
			{
				return playerLifeTime < definition.EndTimeMilliseconds;
			}
			return false;
		}

		public GuildWarDefinition FindNextGuildWarWithinSeason(long timeStamp, int seasonId, bool includeCurrentWar = false)
		{
			if (includeCurrentWar)
			{
				GuildWarDefinition guildWarDefinition = FindGuildWarWithTime(timeStamp);
				if (guildWarDefinition != null)
				{
					return guildWarDefinition;
				}
			}
			List<GuildWarDefinition> list = FindGuildWarDefinitionInSeason(seasonId);
			for (int i = 0; i < list.Count; i++)
			{
				GuildWarDefinition guildWarDefinition2 = list[i];
				if (IsNextWarDefinition(guildWarDefinition2, timeStamp, timeStamp))
				{
					return guildWarDefinition2;
				}
			}
			return null;
		}

		public int GetWarPositionInSeason(int seasonId, int warId)
		{
			List<GuildWarDefinition> list = FindGuildWarDefinitionInSeason(seasonId);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Identifier == warId)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public PhoneCallDefinition GetPhoneCallDefinition(long timeMillis, int callSlotNumber)
		{
			if (PhoneCallDefinitionsBySlotNumber.TryGetValue(callSlotNumber, out var value))
			{
				for (int i = 0; i < value.Count; i++)
				{
					PhoneCallDefinition phoneCallDefinition = value[i];
					if (timeMillis >= phoneCallDefinition.StartTimeMilliseconds && (string.IsNullOrEmpty(phoneCallDefinition.EndTimeUtc) || timeMillis < phoneCallDefinition.EndTimeMilliseconds))
					{
						return phoneCallDefinition;
					}
				}
			}
			return null;
		}

		public PhoneCallVisual GetPhoneCallVisual(PhoneCallDefinition definition)
		{
			PhoneCallVisual result = null;
			for (int i = 0; i < ((PhoneCallVisuals != null) ? PhoneCallVisuals.Length : 0); i++)
			{
				PhoneCallVisual phoneCallVisual = PhoneCallVisuals[i];
				if (definition != null && phoneCallVisual.Name == definition.VisualOverride)
				{
					return phoneCallVisual;
				}
				if (phoneCallVisual.Name == "FallbackVisual")
				{
					result = phoneCallVisual;
				}
			}
			return result;
		}

		public int GetPhoneCallDefinitionMaxSlotNumber()
		{
			int num = 0;
			foreach (KeyValuePair<int, List<PhoneCallDefinition>> item in PhoneCallDefinitionsBySlotNumber)
			{
				if (item.Key > num)
				{
					num = item.Key;
				}
			}
			return num;
		}

		public MissionCost GetMissionCost(int index)
		{
			if (MissionCosts == null)
			{
				return null;
			}
			MissionCost[] missionCosts = MissionCosts;
			foreach (MissionCost missionCost in missionCosts)
			{
				if (missionCost.Index == index)
				{
					return missionCost;
				}
			}
			if (MissionCosts.Length == 0)
			{
				return null;
			}
			return MissionCosts[MissionCosts.Length - 1];
		}

		public AIMoveBehaviorData GetAIMoveBehaviorData(Faction faction, AIMode mode, SurvivorClass survivorClass)
		{
			AIMoveBehaviorData aIMoveBehaviorData = null;
			AIMoveBehaviorData aIMoveBehaviorData2 = null;
			for (int i = 0; i < AIMoveBehaviorConfiguration.Length; i++)
			{
				AIMoveBehaviorData aIMoveBehaviorData3 = AIMoveBehaviorConfiguration[i];
				if (aIMoveBehaviorData3.Faction == faction && aIMoveBehaviorData3.AIMode == mode && aIMoveBehaviorData3.Class == SurvivorClass.None)
				{
					aIMoveBehaviorData = aIMoveBehaviorData3;
				}
				else if (aIMoveBehaviorData3.Faction == faction && aIMoveBehaviorData3.AIMode == mode && aIMoveBehaviorData3.Class == survivorClass)
				{
					aIMoveBehaviorData2 = aIMoveBehaviorData3;
				}
			}
			if (aIMoveBehaviorData != null && aIMoveBehaviorData2 != null)
			{
				aIMoveBehaviorData += aIMoveBehaviorData2;
			}
			return aIMoveBehaviorData;
		}

		public ActorLevelDefinition GetActorLevelDefinition(string actorDefinitionID, int level, bool returnBestMatch = true)
		{
			int num = -1;
			ActorLevelDefinition result = null;
			if (ActorLevelsByActorDefinitionID.TryGetValue(actorDefinitionID, out var value))
			{
				foreach (ActorLevelDefinition item in value)
				{
					if (item.Level == level)
					{
						return item;
					}
					if (item.Level > num)
					{
						num = item.Level;
						result = item;
					}
				}
			}
			if (returnBestMatch)
			{
				return result;
			}
			return null;
		}

		public BuildingsAmountsDefinition GetBuildingsAmountsAtCouncilLevel(int councilLevel)
		{
			BuildingsAmountsDefinition[] buildingsAmounts = BuildingsAmounts;
			foreach (BuildingsAmountsDefinition buildingsAmountsDefinition in buildingsAmounts)
			{
				if (buildingsAmountsDefinition.CouncilLevel == councilLevel)
				{
					return buildingsAmountsDefinition;
				}
			}
			return null;
		}

		public SteamStoreConfig GetSteamStoreConfig(string productID)
		{
			SteamStoreConfig[] steamStoreConfigs = SteamStoreConfigs;
			foreach (SteamStoreConfig steamStoreConfig in steamStoreConfigs)
			{
				if (steamStoreConfig.ProductID == productID)
				{
					return steamStoreConfig;
				}
			}
			return null;
		}

		public int GetAdditionalBuildingAmountAtCouncilLevel(int councilLevel, string buildingType)
		{
			if (councilLevel <= BuildingsAmounts.Length)
			{
				BuildingsAmountsDefinition buildingsAmountsAtCouncilLevel = GetBuildingsAmountsAtCouncilLevel(Math.Max(councilLevel - 1, 0));
				return GetBuildingsAmountsAtCouncilLevel(councilLevel).GetAmountsForBuilding(buildingType) - buildingsAmountsAtCouncilLevel.GetAmountsForBuilding(buildingType);
			}
			return 0;
		}

		public int GetTotalBuildingsAmountsEntries()
		{
			return BuildingsAmounts.Length;
		}

		public BuildingType GetBuildingType(string typeName)
		{
			BuildingType[] buildingTypes = BuildingTypes;
			foreach (BuildingType buildingType in buildingTypes)
			{
				if (buildingType.Name == typeName)
				{
					return buildingType;
				}
			}
			return null;
		}

		public BuildingUpgradeLevel GetBuildingUpgradeLevel(string buildingTypeName, int level)
		{
			if (BuildingUpgradeLevelsByBuildingType.TryGetValue(buildingTypeName, out var value))
			{
				foreach (BuildingUpgradeLevel item in value)
				{
					if (item.Level == level)
					{
						return item;
					}
				}
			}
			return null;
		}

		public BuildingUpgradeLevel GetBuildingUpgradeLevelByDependencyLevelRequired(string buildingTypeName, int level)
		{
			BuildingUpgradeLevel result = null;
			int num = int.MinValue;
			if (BuildingUpgradeLevelsByBuildingType.TryGetValue(buildingTypeName, out var value))
			{
				foreach (BuildingUpgradeLevel item in value)
				{
					if (item.Level > num && item.DependencyLevelRequired == level)
					{
						result = item;
						num = item.Level;
					}
				}
			}
			return result;
		}

		public int GetMaximumUpgradeLevel(string buildingTypeName)
		{
			int num = 0;
			if (BuildingUpgradeLevelsByBuildingType.TryGetValue(buildingTypeName, out var value))
			{
				foreach (BuildingUpgradeLevel item in value)
				{
					if (item.Level > num)
					{
						num = item.Level;
					}
				}
			}
			return num;
		}

		public int GetMaximumEquipmentLevel()
		{
			int result = -1;
			BuildingUpgradeLevel buildingUpgradeLevelByDependencyLevelRequired = GetBuildingUpgradeLevelByDependencyLevelRequired("Workshop", ConfigData.ForceCouncilMaxLevel);
			if (buildingUpgradeLevelByDependencyLevelRequired != null)
			{
				result = GetMaxAvailableEquipmentLevelWithWorkshopLevel(buildingUpgradeLevelByDependencyLevelRequired.Level);
			}
			return result;
		}

		public RadioTentLevelData GetRadioTentDataForLevel(int level, DropType dropType)
		{
			RadioTentLevelData result = null;
			for (int i = 0; i < RadioTentLevelsData.Length; i++)
			{
				if (RadioTentLevelsData[i].Level == level && RadioTentLevelsData[i].DropType == dropType)
				{
					return RadioTentLevelsData[i];
				}
				result = RadioTentLevelsData[i];
			}
			return result;
		}

		public RandomTalentChart GetRandomTalentChartById(int id)
		{
			RandomTalentChart result = null;
			RandomTalentChart[] randomTalentCharts = RandomTalentCharts;
			foreach (RandomTalentChart randomTalentChart in randomTalentCharts)
			{
				if (randomTalentChart.Id == id)
				{
					result = randomTalentChart;
					break;
				}
			}
			return result;
		}

		public TraitDefinition GetTraitDefinition(string traitIdentifier)
		{
			if (!string.IsNullOrEmpty(traitIdentifier))
			{
				TraitDefinition value = null;
				if (traitDefinitionIndex.TryGetValue(traitIdentifier, out value))
				{
					return value;
				}
			}
			return null;
		}

		public CommandSkillDefinition GetCommandSkillDefinition(int id)
		{
			CommandSkillDefinition value = null;
			if (commandSkillDefinitionIndex.TryGetValue(id, out value))
			{
				return value;
			}
			return null;
		}

		public AchievementDefinition GetAchievementDefinition(string achievementDefinitionID)
		{
			if (AchievementDefinitionsById.TryGetValue(achievementDefinitionID, out var value))
			{
				return value;
			}
			return null;
		}

		public AchievementDefinition GetRandomDailyQuestDefinition(int randomNumber, List<string> ignoreIDs)
		{
			List<AchievementDefinition> list = new List<AchievementDefinition>();
			for (int i = 0; i < AchievementDefinitions.Length; i++)
			{
				AchievementDefinition achievementDefinition = AchievementDefinitions[i];
				if (achievementDefinition.AchievementType == AchievementType.DailyQuest && !ignoreIDs.Contains(achievementDefinition.ID))
				{
					list.Add(achievementDefinition);
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list[randomNumber % list.Count];
		}

		public QuestDefinition GetQuestDefinition(string questIdentifier)
		{
			QuestDefinition[] questDefinitions = QuestDefinitions;
			foreach (QuestDefinition questDefinition in questDefinitions)
			{
				if (questDefinition.Identifier == questIdentifier)
				{
					return questDefinition;
				}
			}
			return null;
		}

		public int GetHighestQuestOrder()
		{
			int num = 0;
			for (int i = 0; i < QuestDefinitions.Length; i++)
			{
				QuestDefinition questDefinition = QuestDefinitions[i];
				if (questDefinition != null && questDefinition.Order > num)
				{
					num = questDefinition.Order;
				}
			}
			return num;
		}

		public QuestDefinition GetQuestDefinition(int questIndex, int giver)
		{
			QuestDefinition[] questDefinitions = QuestDefinitions;
			foreach (QuestDefinition questDefinition in questDefinitions)
			{
				if (questDefinition.Order == questIndex && questDefinition.Giver == giver)
				{
					return questDefinition;
				}
			}
			return null;
		}

		public int GetMaxAvailableEquipmentLevel()
		{
			int num = -1;
			EquipmentLevelDefinition[] equipmentLevelDefinitions = EquipmentLevelDefinitions;
			foreach (EquipmentLevelDefinition equipmentLevelDefinition in equipmentLevelDefinitions)
			{
				num = Math.Max(num, equipmentLevelDefinition.Level);
			}
			return num;
		}

		public int GetMaxAvailableEquipmentLevelWithWorkshopLevel(int workshopLevel)
		{
			int num = -1;
			EquipmentLevelDefinition[] equipmentLevelDefinitions = EquipmentLevelDefinitions;
			foreach (EquipmentLevelDefinition equipmentLevelDefinition in equipmentLevelDefinitions)
			{
				if (equipmentLevelDefinition.WorkshopLevelRequired <= workshopLevel)
				{
					num = Math.Max(num, equipmentLevelDefinition.Level);
				}
			}
			return num;
		}

		public int GetMaxAvailableDifficulty()
		{
			int num = -1;
			MissionGenerationData[] missionGenerationData = MissionGenerationData;
			foreach (MissionGenerationData missionGenerationData2 in missionGenerationData)
			{
				num = Math.Max(num, missionGenerationData2.MissionLevel);
			}
			return num;
		}

		public MissionData GetMissionData(string missionId)
		{
			if (string.IsNullOrEmpty(missionId))
			{
				return null;
			}
			if (MissionDataById.TryGetValue(missionId, out var value))
			{
				return value;
			}
			return null;
		}

		public MissionData GetMissionData(int missionIdHash)
		{
			foreach (MissionData missionDatum in MissionData)
			{
				if (missionDatum.Id.GetHashCode() == missionIdHash)
				{
					return missionDatum;
				}
			}
			return null;
		}

		public float GetLevelBalanceBodyShotChanceModifier(Faction attacker, Faction target, int levelDiff)
		{
			int num = int.MaxValue;
			int num2 = int.MinValue;
			float result = 0f;
			if (LevelBalanceModifiers != null)
			{
				for (int i = 0; i < LevelBalanceModifiers.Length; i++)
				{
					ActorLevelBalanceModifier actorLevelBalanceModifier = LevelBalanceModifiers[i];
					if (actorLevelBalanceModifier.Attacker != attacker || actorLevelBalanceModifier.Target != target)
					{
						continue;
					}
					if (levelDiff == actorLevelBalanceModifier.LevelDiff)
					{
						result = actorLevelBalanceModifier.BodyshotChance;
						break;
					}
					if (actorLevelBalanceModifier.LevelDiff < num)
					{
						num = actorLevelBalanceModifier.LevelDiff;
						if (levelDiff < num)
						{
							result = actorLevelBalanceModifier.BodyshotChance;
						}
					}
					if (actorLevelBalanceModifier.LevelDiff > num2)
					{
						num2 = actorLevelBalanceModifier.LevelDiff;
						if (levelDiff > num2)
						{
							result = actorLevelBalanceModifier.BodyshotChance;
						}
					}
				}
			}
			return result;
		}

		public int GetHighestLevelDiffForZeroBodyshot(Faction attackerFaction, Faction targetFaction)
		{
			int num = int.MinValue;
			if (LevelBalanceModifiers != null)
			{
				for (int i = 0; i < LevelBalanceModifiers.Length; i++)
				{
					ActorLevelBalanceModifier actorLevelBalanceModifier = LevelBalanceModifiers[i];
					if (actorLevelBalanceModifier.Attacker == attackerFaction && actorLevelBalanceModifier.Target == targetFaction && actorLevelBalanceModifier.BodyshotChance == 0f)
					{
						num = Math.Max(num, actorLevelBalanceModifier.LevelDiff);
					}
				}
			}
			return num;
		}

		public float GetLevelBalanceCriticalChanceModifier(Faction attacker, Faction target, int levelDiff)
		{
			int num = int.MaxValue;
			int num2 = int.MinValue;
			float result = 0f;
			if (LevelBalanceModifiers != null)
			{
				for (int i = 0; i < LevelBalanceModifiers.Length; i++)
				{
					ActorLevelBalanceModifier actorLevelBalanceModifier = LevelBalanceModifiers[i];
					if (actorLevelBalanceModifier.Attacker != attacker || actorLevelBalanceModifier.Target != target)
					{
						continue;
					}
					if (levelDiff == actorLevelBalanceModifier.LevelDiff)
					{
						result = actorLevelBalanceModifier.CriticalChance;
						break;
					}
					if (actorLevelBalanceModifier.LevelDiff < num)
					{
						num = actorLevelBalanceModifier.LevelDiff;
						if (levelDiff < num)
						{
							result = actorLevelBalanceModifier.CriticalChance;
						}
					}
					if (actorLevelBalanceModifier.LevelDiff > num2)
					{
						num2 = actorLevelBalanceModifier.LevelDiff;
						if (levelDiff > num2)
						{
							result = actorLevelBalanceModifier.CriticalChance;
						}
					}
				}
			}
			return result;
		}

		public int GetRarityActorLevelModifier(int rarityLevel)
		{
			if (RarityActorLevelModifiers != null)
			{
				for (int i = 0; i < RarityActorLevelModifiers.Length; i++)
				{
					if (RarityActorLevelModifiers[i].RarityLevel == rarityLevel)
					{
						return RarityActorLevelModifiers[i].LevelModifier;
					}
				}
			}
			return 0;
		}

		public static DateTime ParseDateTime(string value)
		{
			DateTime result = DateTime.MinValue;
			if (GEDDateTimeParser.TryParseDateTimeOptimized(value, ref result))
			{
				return result;
			}
			return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
		}

		public ActorDefinition GetActorDefinition(string actorID)
		{
			if (ActorDefinitions == null)
			{
				return null;
			}
			foreach (ActorDefinition actorDefinition in ActorDefinitions)
			{
				if (actorDefinition.ID == actorID)
				{
					return actorDefinition;
				}
			}
			return null;
		}

		public AbilityDefinition GetAbilityDefinition(string abilityID)
		{
			List<AbilityDefinition> abilityDefinitions = AbilityDefinitions;
			if (abilityDefinitions == null)
			{
				return null;
			}
			foreach (AbilityDefinition item in abilityDefinitions)
			{
				if (item.Identifier == abilityID)
				{
					return item;
				}
			}
			return null;
		}

		public EquipmentDefinition GetEquipmentDefinition(string equipmentID)
		{
			if (EquipmentDefinitionsById == null)
			{
				return null;
			}
			EquipmentDefinition value = null;
			if (!EquipmentDefinitionsById.TryGetValue(equipmentID, out value))
			{
				return null;
			}
			return value;
		}

		public EquipBreakthroughDefinition GetEquipBreakthroughDefinitionByRarityAndLevel(int rarity, int level)
		{
			if (EquipBreakthroughDefinitions == null)
			{
				return null;
			}
			EquipBreakthroughDefinition[] equipBreakthroughDefinitions = EquipBreakthroughDefinitions;
			foreach (EquipBreakthroughDefinition equipBreakthroughDefinition in equipBreakthroughDefinitions)
			{
				if (equipBreakthroughDefinition.Level == level && equipBreakthroughDefinition.WeaponMode == "Normal")
				{
					return equipBreakthroughDefinition;
				}
			}
			return null;
		}

		public EquipBreakthroughDefinition GetRemoldEquipBreakthroughDefinitionByRarityAndLevel(int rarity, int level)
		{
			if (EquipBreakthroughDefinitions == null)
			{
				return null;
			}
			EquipBreakthroughDefinition[] equipBreakthroughDefinitions = EquipBreakthroughDefinitions;
			foreach (EquipBreakthroughDefinition equipBreakthroughDefinition in equipBreakthroughDefinitions)
			{
				if (equipBreakthroughDefinition.Level == level && equipBreakthroughDefinition.WeaponMode == "Remold")
				{
					return equipBreakthroughDefinition;
				}
			}
			return null;
		}

		public EquipBreakthroughDefinition GetRemoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode(int rarity, int level, string type)
		{
			if (EquipBreakthroughDefinitions == null)
			{
				return null;
			}
			EquipBreakthroughDefinition[] equipBreakthroughDefinitions = EquipBreakthroughDefinitions;
			foreach (EquipBreakthroughDefinition equipBreakthroughDefinition in equipBreakthroughDefinitions)
			{
				if (equipBreakthroughDefinition.Level == level && equipBreakthroughDefinition.WeaponMode == type)
				{
					return equipBreakthroughDefinition;
				}
			}
			return null;
		}

		public EquipBreakthroughTrait[] GetEquipBreakthroughTraitsBySurvivolClassAndEquipmentCategory(SurvivorClass survivorClass, EquipmentCategory equipmentCategory)
		{
			return EquipBreakthroughTraits.Where((EquipBreakthroughTrait x) => x.SurvivorClass == survivorClass && x.EquipmentCategory == equipmentCategory).ToArray();
		}

		public EquipTokenDefinition GetEquipTokenDefinition(string equipTokenId)
		{
			EquipTokenDefinition[] equipTokenDefinitions = EquipTokenDefinitions;
			if (equipTokenDefinitions == null)
			{
				return null;
			}
			EquipTokenDefinition[] array = equipTokenDefinitions;
			foreach (EquipTokenDefinition equipTokenDefinition in array)
			{
				if (equipTokenDefinition.EquipTokenId == equipTokenId)
				{
					return equipTokenDefinition;
				}
			}
			return null;
		}

		public EquipTokenDefinition GetEquipTokenDefinitionByRelateEquipId(string relateEquipId)
		{
			EquipTokenDefinition[] equipTokenDefinitions = EquipTokenDefinitions;
			if (equipTokenDefinitions == null)
			{
				return null;
			}
			EquipTokenDefinition[] array = equipTokenDefinitions;
			foreach (EquipTokenDefinition equipTokenDefinition in array)
			{
				if (equipTokenDefinition.RelateEquipId == relateEquipId)
				{
					return equipTokenDefinition;
				}
			}
			return null;
		}

		public WalkerExplosionDefinition GetWalkerExplosionDefinition(string identifier)
		{
			for (int i = 0; i < ((WalkerExplosionDefinitions != null) ? WalkerExplosionDefinitions.Length : 0); i++)
			{
				WalkerExplosionDefinition walkerExplosionDefinition = WalkerExplosionDefinitions[i];
				if (walkerExplosionDefinition.TraitIdentifier == identifier)
				{
					return walkerExplosionDefinition;
				}
			}
			return null;
		}

		public void GetSurvivorUpgradeCost(int baseLevel, out int spCost, out int timeCost)
		{
			spCost = 0;
			timeCost = 0;
			for (int i = 0; i < ((SurvivorUpgradeCosts != null) ? SurvivorUpgradeCosts.Length : 0); i++)
			{
				SurvivorUpgradeCost survivorUpgradeCost = SurvivorUpgradeCosts[i];
				if (survivorUpgradeCost != null && survivorUpgradeCost.Level == baseLevel)
				{
					timeCost = survivorUpgradeCost.CostTime;
					spCost = survivorUpgradeCost.CostSP;
					break;
				}
			}
		}

		public List<IncrementalDifficultyEffectDefinition> GetDifficultyEffects(IncrementalDifficultyMissionType missionType, int incrementCount)
		{
			List<IncrementalDifficultyEffectDefinition> list = new List<IncrementalDifficultyEffectDefinition>();
			for (int i = 0; i < IncrementalDifficultyEffects.Length; i++)
			{
				IncrementalDifficultyEffectDefinition incrementalDifficultyEffectDefinition = IncrementalDifficultyEffects[i];
				if (incrementalDifficultyEffectDefinition.MissionType == missionType && incrementalDifficultyEffectDefinition.Increment <= incrementCount)
				{
					list.Add(incrementalDifficultyEffectDefinition);
				}
			}
			return list;
		}

		public int GetSurvivorTraitUpgradeCost(int traitRaritySum)
		{
			int result = 0;
			int num = int.MaxValue;
			for (int i = 0; i < SurvivorUpgradeCostTokens.Length; i++)
			{
				SurvivorTokenUpgradeCostDefinition survivorTokenUpgradeCostDefinition = SurvivorUpgradeCostTokens[i];
				if (traitRaritySum == survivorTokenUpgradeCostDefinition.TraitRaritySum)
				{
					return survivorTokenUpgradeCostDefinition.TokenCost;
				}
				if (survivorTokenUpgradeCostDefinition.TraitRaritySum - traitRaritySum < num)
				{
					result = survivorTokenUpgradeCostDefinition.TokenCost;
					num = survivorTokenUpgradeCostDefinition.TraitRaritySum - traitRaritySum;
				}
			}
			return result;
		}

		public SpeedupTokenTimeDefinition GetSpeedupTokenTimeDefinitionByCurrency(string currency)
		{
			for (int i = 0; i < SpeedupTokenTimeDefinitions.Length; i++)
			{
				SpeedupTokenTimeDefinition speedupTokenTimeDefinition = SpeedupTokenTimeDefinitions[i];
				if (currency == speedupTokenTimeDefinition.Currency)
				{
					return speedupTokenTimeDefinition;
				}
			}
			return null;
		}

		public int GetSurvivorDemoteSP(SurvivorClass survivorClass, int baseLevel, int upgradeLevel)
		{
			SurvivorUpgradeDefinition survivorsUpgradeDefinition = GetSurvivorsUpgradeDefinition(survivorClass, baseLevel);
			int num = 0;
			int spCost = 0;
			int timeCost = 0;
			for (int i = baseLevel; i < upgradeLevel; i++)
			{
				GetSurvivorUpgradeCost(i, out spCost, out timeCost);
				num += spCost;
			}
			return survivorsUpgradeDefinition.DemoteSPRefund * num / 100 + survivorsUpgradeDefinition.DemoteSPBase;
		}

		public int GetSurvivorsMaxUpgradeLevel(SurvivorClass survivorClass)
		{
			int num = 0;
			if (SurvivorUpgradeDefinitionsBySurvivorClass.TryGetValue(survivorClass, out var value))
			{
				foreach (SurvivorUpgradeDefinition item in value)
				{
					if (item.Level > num)
					{
						num = item.Level;
					}
				}
			}
			return num;
		}

		public SurvivorUpgradeDefinition GetSurvivorsUpgradeDefinition(SurvivorClass survivorClass, int survivorLevel)
		{
			if (SurvivorUpgradeDefinitionsBySurvivorClass.TryGetValue(survivorClass, out var value))
			{
				foreach (SurvivorUpgradeDefinition item in value)
				{
					if (item.Level == survivorLevel)
					{
						return item;
					}
				}
			}
			return null;
		}

		public ReturnThreeDayDefinition GetReturnThreeDayDefinition(int id)
		{
			if (ReturnThreeDayDefinitions != null)
			{
				for (int i = 0; i < ReturnThreeDayDefinitions.Length; i++)
				{
					if (ReturnThreeDayDefinitions[i].Id == id)
					{
						return ReturnThreeDayDefinitions[i];
					}
				}
			}
			return null;
		}

		public ThreeDayDefinition GetThreeDayDefinition(int id)
		{
			if (ThreeDayDefinitions != null)
			{
				for (int i = 0; i < ThreeDayDefinitions.Length; i++)
				{
					if (ThreeDayDefinitions[i].Id == id)
					{
						return ThreeDayDefinitions[i];
					}
				}
			}
			return null;
		}

		public ConditionBundleDefinition GetConditionBundleDefinition(string bundleId)
		{
			if (ConditionBundleDefinitions != null)
			{
				for (int i = 0; i < ConditionBundleDefinitions.Length; i++)
				{
					if (ConditionBundleDefinitions[i].BundleIdentifier == bundleId)
					{
						return ConditionBundleDefinitions[i];
					}
				}
			}
			return null;
		}

		public ReturnThreeDayDefinition GetCurOpenedReturnThreeDayDefinition(long now)
		{
			if (ReturnThreeDayDefinitions != null)
			{
				for (int i = 0; i < ReturnThreeDayDefinitions.Length; i++)
				{
					ReturnThreeDayDefinition returnThreeDayDefinition = ReturnThreeDayDefinitions[i];
					if (returnThreeDayDefinition != null && now >= returnThreeDayDefinition.StartTimeMilliseconds && now < returnThreeDayDefinition.EndTimeMilliseconds)
					{
						return returnThreeDayDefinition;
					}
				}
			}
			return null;
		}

		public ThreeDayDefinition GetCurOpenedThreeDayDefinition(long now)
		{
			if (ThreeDayDefinitions != null)
			{
				for (int i = 0; i < ThreeDayDefinitions.Length; i++)
				{
					ThreeDayDefinition threeDayDefinition = ThreeDayDefinitions[i];
					if (threeDayDefinition != null && now >= threeDayDefinition.StartTimeMilliseconds && now < threeDayDefinition.EndTimeMilliseconds)
					{
						return threeDayDefinition;
					}
				}
			}
			return null;
		}

		public int GetMinimumTrainingGroundLevelForClass(SurvivorClass survivorClass)
		{
			if (survivorClassMinimumTrainingGroundLevels == null)
			{
				survivorClassMinimumTrainingGroundLevels = new Dictionary<SurvivorClass, int>();
				SurvivorUpgradeDefinition[] survivorUpgradeDefinitions = SurvivorUpgradeDefinitions;
				foreach (SurvivorUpgradeDefinition survivorUpgradeDefinition in survivorUpgradeDefinitions)
				{
					if (survivorClassMinimumTrainingGroundLevels.ContainsKey(survivorUpgradeDefinition.SurvivorClass))
					{
						if (survivorUpgradeDefinition.TrainingGroundLevel < survivorClassMinimumTrainingGroundLevels[survivorUpgradeDefinition.SurvivorClass])
						{
							survivorClassMinimumTrainingGroundLevels[survivorUpgradeDefinition.SurvivorClass] = survivorUpgradeDefinition.TrainingGroundLevel;
						}
					}
					else
					{
						survivorClassMinimumTrainingGroundLevels[survivorUpgradeDefinition.SurvivorClass] = survivorUpgradeDefinition.TrainingGroundLevel;
					}
				}
			}
			if (survivorClassMinimumTrainingGroundLevels.ContainsKey(survivorClass))
			{
				return survivorClassMinimumTrainingGroundLevels[survivorClass];
			}
			return 0;
		}

		public List<OutfitDefinition> GetAvailableOutfitDefinitions(long playerUTCTimestamp)
		{
			if (AvailableOutfitDefinitions == null)
			{
				AvailableOutfitDefinitions = new List<OutfitDefinition>();
				for (int i = 0; i < OutfitDefinitions.Length; i++)
				{
					if (OutfitDefinitions[i].IsAvailableOnShop(playerUTCTimestamp))
					{
						AvailableOutfitDefinitions.Add(OutfitDefinitions[i]);
					}
				}
			}
			return AvailableOutfitDefinitions;
		}

		public OutfitDefinition GetOutfitDefinition(string id)
		{
			for (int i = 0; i < OutfitDefinitions.Length; i++)
			{
				OutfitDefinition outfitDefinition = OutfitDefinitions[i];
				if (outfitDefinition.ID == id)
				{
					return outfitDefinition;
				}
			}
			return null;
		}

		public HeroSkinDefinition GetSkinDefinition(string id)
		{
			return HeroSkinDefinitions.FirstOrDefault((HeroSkinDefinition x) => x.ID == id);
		}

		public EquipmentLevelDefinition GetEquipmentLevelDefinition(int equipmentLevel)
		{
			EquipmentLevelDefinition[] equipmentLevelDefinitions = EquipmentLevelDefinitions;
			foreach (EquipmentLevelDefinition equipmentLevelDefinition in equipmentLevelDefinitions)
			{
				if (equipmentLevelDefinition.Level == equipmentLevel)
				{
					return equipmentLevelDefinition;
				}
			}
			return null;
		}

		public string[] GetInAppPurchaseProductIdList()
		{
			return InAppPurchaseProductsApple.Select((InAppPurchaseProductApple x) => x.Id).ToArray();
		}

		public InAppPurchaseProductApple GetInAppPurchaseProduct(string id)
		{
			InAppPurchaseProductApple[] inAppPurchaseProductsApple = InAppPurchaseProductsApple;
			foreach (InAppPurchaseProductApple inAppPurchaseProductApple in inAppPurchaseProductsApple)
			{
				if (inAppPurchaseProductApple.Id == id)
				{
					return inAppPurchaseProductApple;
				}
			}
			return null;
		}

		public BundleDefinition LEGACY_GetBundleDefinition(string bundleId)
		{
			if (BundleDefinitions != null)
			{
				for (int i = 0; i < BundleDefinitions.Length; i++)
				{
					BundleDefinition bundleDefinition = BundleDefinitions[i];
					if (bundleDefinition.Identifier == bundleId)
					{
						return bundleDefinition;
					}
				}
			}
			return null;
		}

		public CustomBundleDefinition GetCustomBundleDefinition(string idIdentifier)
		{
			if (CustomBundleDefinitions != null)
			{
				return CustomBundleDefinitions.FirstOrDefault((CustomBundleDefinition x) => x.Identifier == idIdentifier);
			}
			return null;
		}

		public CustomBundleStorage GetCustomBundleStorage(int storageId)
		{
			if (CustomBundleStorages != null)
			{
				return CustomBundleStorages.FirstOrDefault((CustomBundleStorage x) => x.StorageID == storageId);
			}
			return null;
		}

		public Rewards GetCustomBundleStorageRewards(int storageId)
		{
			if (CustomBundleStorages != null)
			{
				return CustomBundleStorages.FirstOrDefault((CustomBundleStorage x) => x.StorageID == storageId).RewardEntries;
			}
			return null;
		}

		public RarityBasedUpgradeDefinition GetRarityBasedUpgradeDefinition(int rarityLevel, UpgradeType upgradeType)
		{
			RarityBasedUpgradeDefinition[] rarityBasedUpgradeDefinitions = RarityBasedUpgradeDefinitions;
			foreach (RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition in rarityBasedUpgradeDefinitions)
			{
				if (rarityBasedUpgradeDefinition.RarityLevel == rarityLevel && rarityBasedUpgradeDefinition.UpgradeType == upgradeType)
				{
					return rarityBasedUpgradeDefinition;
				}
			}
			return null;
		}

		public int GetMaxSurvivorSlotsLevel()
		{
			int num = SurvivorSlots.Length;
			int num2 = 1;
			for (int i = 0; i < num; i++)
			{
				SurvivorSlotsData survivorSlotsData = SurvivorSlots[i];
				num2 = Math.Max(num2, survivorSlotsData.Level);
			}
			return num2;
		}

		public SurvivorSlotsData GetSurvivorSlotsData(int level)
		{
			int num = SurvivorSlots.Length;
			for (int i = 0; i < num; i++)
			{
				SurvivorSlotsData survivorSlotsData = SurvivorSlots[i];
				if (survivorSlotsData.Level == level)
				{
					return survivorSlotsData;
				}
			}
			return null;
		}

		public bool BelongsToGeoSegment(SpenderTierDefinition spenderTierDefinition, string counry)
		{
			if (spenderTierDefinition == null)
			{
				return false;
			}
			if (spenderTierDefinition.GeoSegments.Contains("ALL"))
			{
				return true;
			}
			return spenderTierDefinition.GeoSegments.Contains(counry);
		}

		public bool IsInSpenderTier(PlayerModel playerModel, string spenderTierId, double moneySpent, int totalDaysPlayer, int totalPurchases, long secondsSinceLastPurchase, long playerCreationTimeMs, int playerCouncilLevel)
		{
			SpenderTierDefinition spenderTier = GetSpenderTier(spenderTierId);
			double num = ((totalPurchases > 0) ? (moneySpent / (double)totalPurchases) : 0.0);
			if (spenderTier != null && BelongsToGeoSegment(spenderTier, playerModel.Country) && spenderTier.MinMoneySpent <= moneySpent && spenderTier.MaxMoneySpent >= moneySpent && spenderTier.MinDaysPlayed <= totalDaysPlayer && spenderTier.MaxDaysPlayed >= totalDaysPlayer && spenderTier.MinPurchases <= totalPurchases && spenderTier.MaxPurchases >= totalPurchases && (spenderTier.MinTimeFromLastPurchase <= 0 || spenderTier.MinTimeFromLastPurchase <= secondsSinceLastPurchase) && spenderTier.MinCreationTimeMilliseconds <= playerCreationTimeMs && (spenderTier.MaxCreationTimeMilliseconds == 0L || spenderTier.MaxCreationTimeMilliseconds > playerCreationTimeMs) && playerCouncilLevel >= spenderTier.MinCouncilLevel && playerCouncilLevel <= spenderTier.MaxCouncilLevel && num >= (double)spenderTier.MinAveragePurchasePrice && (spenderTier.MaxAveragePurchasePrice == 0 || num < (double)spenderTier.MaxAveragePurchasePrice))
			{
				return true;
			}
			return false;
		}

		public bool IsInSpenderTier(PlayerModel playerModel, string spenderTierId)
		{
			if (playerModel == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(spenderTierId))
			{
				return true;
			}
			long secondsSinceLastPurchaseThatCostMoney = playerModel.BundleManager.GetSecondsSinceLastPurchaseThatCostMoney();
			return IsInSpenderTier(playerModel, spenderTierId, playerModel.TotalUSDSpent, (int)playerModel.LifeTimeInDays, playerModel.GetTotalPurchases(), secondsSinceLastPurchaseThatCostMoney, playerModel.CreationTimeStamp, playerModel.CouncilLevel);
		}

		public SpenderTierDefinition GetSpenderTier(string spenderTierId)
		{
			if (SpenderTierDefinitionsById.TryGetValue(spenderTierId, out var value))
			{
				return value;
			}
			return null;
		}

		private void AddToBundleStoreDefinitionCache(string identifier, int i)
		{
			if (!BundleStoreDefinitionIndexCache.ContainsKey(identifier))
			{
				BundleStoreDefinitionIndexCache.Add(identifier, i);
			}
		}

		private BundleStoreDefinition TryGetBundleStoreDefinitionFromCache(string identifier)
		{
			int value = -1;
			if (BundleStoreDefinitionIndexCache.TryGetValue(identifier, out value))
			{
				if (value >= 0 && value < BundleStoreDefinitions.Count && BundleStoreDefinitions[value].BundleIdentifier == identifier)
				{
					return BundleStoreDefinitions[value];
				}
				BundleStoreDefinitionIndexCache.Clear();
			}
			return null;
		}

		public BundleStoreDefinition GetBundleStoreDefinition(string identifier)
		{
			if (BundleStoreDefinitionsById != null)
			{
				BundleStoreDefinition value = null;
				if (!BundleStoreDefinitionsById.TryGetValue(identifier, out value))
				{
					return null;
				}
				return value;
			}
			return null;
		}

		public List<BundleRotationDefinition> GetTierAvailableBundleRotationDefinitions(PlayerModel playerModel, long secondsSinceLastPurchase)
		{
			List<BundleRotationDefinition> list = new List<BundleRotationDefinition>();
			if (playerModel != null)
			{
				long lifeTimeInDays = playerModel.LifeTimeInDays;
				foreach (KeyValuePair<string, BundleRotationDefinition> orderedBundleRotationsDefinition in orderedBundleRotationsDefinitions)
				{
					BundleRotationDefinition value = orderedBundleRotationsDefinition.Value;
					for (int i = 0; i < value.SpenderTiers.Count; i++)
					{
						if (IsInSpenderTier(playerModel, value.SpenderTiers[i], playerModel.TotalUSDSpent, (int)lifeTimeInDays, playerModel.GetTotalPurchases(), secondsSinceLastPurchase, playerModel.CreationTimeStamp, playerModel.CouncilLevel))
						{
							list.Add(value);
						}
					}
				}
			}
			return list;
		}

		public string GetBestSpenderTier(List<string> spenderTiers, PlayerModel playerModel, long secondsSinceLastPurchase)
		{
			if (spenderTiers == null || spenderTiers.Count < 1)
			{
				return null;
			}
			List<SpenderTierDefinition> list = new List<SpenderTierDefinition>();
			if (playerModel != null)
			{
				for (int i = 0; i < (spenderTiers?.Count ?? 0); i++)
				{
					string spenderTierId = spenderTiers[i];
					if (IsInSpenderTier(playerModel, spenderTierId, playerModel.TotalUSDSpent, (int)playerModel.LifeTimeInDays, playerModel.GetTotalPurchases(), secondsSinceLastPurchase, playerModel.CreationTimeStamp, playerModel.CouncilLevel))
					{
						list.Add(GetSpenderTier(spenderTierId));
					}
				}
			}
			list.Sort((SpenderTierDefinition a, SpenderTierDefinition b) => (a.MaxMoneySpent == b.MaxMoneySpent) ? Math.Sign(a.MaxDaysPlayed - b.MaxDaysPlayed) : Math.Sign(a.MaxMoneySpent - b.MaxMoneySpent));
			if (list.Count <= 0)
			{
				return null;
			}
			return list[list.Count - 1].TierIdentifier;
		}

		public BundleRotationDefinition GetBundleRotationDefinition(string rotationIdentifier)
		{
			if (orderedBundleRotationsDefinitions.ContainsKey(rotationIdentifier))
			{
				return orderedBundleRotationsDefinitions[rotationIdentifier];
			}
			return null;
		}

		public Dictionary<string, BundleRotationDefinition> GetAllBundleRotationDefinitions()
		{
			return orderedBundleRotationsDefinitions;
		}

		public void CreateTemporaryRewardBundleDefinitions(string bundleId, string rewardsList)
		{
			if (BundleStoreDefinitionsById.TryGetValue(bundleId, out var _))
			{
				BundleStoreDefinitionsById.Remove(bundleId);
				for (int i = 0; i < BundleStoreDefinitions.Count; i++)
				{
					if (BundleStoreDefinitions[i].BundleIdentifier == bundleId)
					{
						BundleStoreDefinitions.RemoveAt(i);
						break;
					}
				}
			}
			if (BundleContentDefinitionsById.TryGetValue(bundleId, out var _))
			{
				BundleContentDefinitionsById.Remove(bundleId);
				for (int j = 0; j < BundleContentDefinitions.Count; j++)
				{
					if (BundleContentDefinitions[j].Identifier == bundleId)
					{
						BundleContentDefinitions.RemoveAt(j);
						break;
					}
				}
			}
			BundleStoreDefinition bundleStoreDefinition = new BundleStoreDefinition();
			bundleStoreDefinition.BundleIdentifier = bundleId;
			BundleContentDefinition bundleContentDefinition = new BundleContentDefinition();
			bundleContentDefinition.Identifier = bundleId;
			bundleContentDefinition.Rewards = rewardsList;
			BundleStoreDefinitions.Add(bundleStoreDefinition);
			BundleStoreDefinitionsById.Add(bundleId, bundleStoreDefinition);
			BundleContentDefinitions.Add(bundleContentDefinition);
			BundleContentDefinitionsById.Add(bundleId, bundleContentDefinition);
			SetupBundleContentRewardEntries();
		}

		public void RemoveTemporaryRewardBundleDefinitions(string bundleId)
		{
			if (BundleStoreDefinitionsById.TryGetValue(bundleId, out var _))
			{
				BundleStoreDefinitionsById.Remove(bundleId);
				for (int i = 0; i < BundleStoreDefinitions.Count; i++)
				{
					if (BundleStoreDefinitions[i].BundleIdentifier == bundleId)
					{
						BundleStoreDefinitions.RemoveAt(i);
						break;
					}
				}
			}
			if (!BundleContentDefinitionsById.TryGetValue(bundleId, out var _))
			{
				return;
			}
			BundleContentDefinitionsById.Remove(bundleId);
			for (int j = 0; j < BundleContentDefinitions.Count; j++)
			{
				if (BundleContentDefinitions[j].Identifier == bundleId)
				{
					BundleContentDefinitions.RemoveAt(j);
					break;
				}
			}
		}

		public BundleStoreDefinition GetBundleStoreDefinitionFromProductID(string productID, long currentUTCTime)
		{
			BundleStoreDefinition result = null;
			for (int i = 0; i < BundleStoreDefinitions.Count; i++)
			{
				BundleStoreDefinition bundleStoreDefinition = BundleStoreDefinitions[i];
				BundleContentDefinition bundleContentDefinition = GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
				if (bundleContentDefinition != null && bundleContentDefinition.IAPProduct == productID)
				{
					result = bundleStoreDefinition;
					if (!bundleStoreDefinition.HasDateLimit || (currentUTCTime > bundleStoreDefinition.StartTimeMilliseconds && currentUTCTime < bundleStoreDefinition.EndTimeMilliseconds))
					{
						return bundleStoreDefinition;
					}
				}
			}
			return result;
		}

		public BundleContentDefinition GetBundleContentDefinition(string identifier)
		{
			if (BundleContentDefinitionsById != null)
			{
				BundleContentDefinition value = null;
				if (!BundleContentDefinitionsById.TryGetValue(identifier, out value))
				{
					return null;
				}
				return value;
			}
			return null;
		}

		public BundleContentDefinition GetBundleContentDefinitionWithIAPProduct(string IAPProduct)
		{
			if (BundleContentDefinitions != null)
			{
				for (int i = 0; i < BundleContentDefinitions.Count; i++)
				{
					if (BundleContentDefinitions[i].IAPProduct == IAPProduct)
					{
						return BundleContentDefinitions[i];
					}
				}
			}
			return null;
		}

		public List<BundleContentDefinition> GetBundleContentDefinitionsWithCategory(string category)
		{
			List<BundleContentDefinition> list = new List<BundleContentDefinition>();
			if (BundleContentDefinitions != null)
			{
				for (int i = 0; i < BundleContentDefinitions.Count; i++)
				{
					if (BundleContentDefinitions[i] != null && BundleContentDefinitions[i].Category == category)
					{
						list.Add(BundleContentDefinitions[i]);
					}
				}
			}
			return list;
		}

		public List<BundleStoreDefinition> GetOrderedStoreBundles(long currentTimeMs)
		{
			if (cachedOrderedStoreBundles != null && currentTimeMs - lastCacheTime < 5000)
			{
				return new List<BundleStoreDefinition>(cachedOrderedStoreBundles);
			}
			List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
			if (BundleStoreDefinitions != null)
			{
				for (int i = 0; i < BundleStoreDefinitions.Count; i++)
				{
					BundleStoreDefinition bundleStoreDefinition = BundleStoreDefinitions[i];
					if (!bundleStoreDefinition.HasDateLimit || (bundleStoreDefinition.HasDateLimit && currentTimeMs > bundleStoreDefinition.StartTimeMilliseconds && currentTimeMs < bundleStoreDefinition.EndTimeMilliseconds))
					{
						list.Add(bundleStoreDefinition);
					}
				}
			}
			list.Sort(delegate(BundleStoreDefinition a, BundleStoreDefinition b)
			{
				if (a == null && b == null)
				{
					return 0;
				}
				if (a == null)
				{
					return 1;
				}
				return (b == null) ? (-1) : a.DisplayOrder.CompareTo(b.DisplayOrder);
			});
			cachedOrderedStoreBundles = new List<BundleStoreDefinition>(list);
			lastCacheTime = currentTimeMs;
			return list;
		}

		public void ClearBundleStoreCache()
		{
			cachedOrderedStoreBundles = null;
			lastCacheTime = 0L;
		}

		public TradefairBundleStoreDefinition GetBundleTradefairDefinition(string identifier)
		{
			if (TradefairBundleStoreDefinitions == null || TradefairBundleStoreDefinitions.Count < 1)
			{
				return null;
			}
			TradefairBundleStoreDefinition tradefairBundleStoreDefinition = TryGetBundleTradefairDefinitionFromCache(identifier);
			if (tradefairBundleStoreDefinition != null)
			{
				return tradefairBundleStoreDefinition;
			}
			for (int i = 0; i < TradefairBundleStoreDefinitions.Count; i++)
			{
				if (TradefairBundleStoreDefinitions[i].BundleIdentifier == identifier)
				{
					AddToBundleTradefairDefinitionCache(identifier, i);
					return TradefairBundleStoreDefinitions[i];
				}
			}
			return null;
		}

		private void AddToBundleTradefairDefinitionCache(string identifier, int i)
		{
			if (!BundleTradefairDefinitionIndexCache.ContainsKey(identifier))
			{
				BundleTradefairDefinitionIndexCache.Add(identifier, i);
			}
		}

		private TradefairBundleStoreDefinition TryGetBundleTradefairDefinitionFromCache(string identifier)
		{
			int value = -1;
			if (BundleTradefairDefinitionIndexCache.TryGetValue(identifier, out value))
			{
				if (value >= 0 && value < TradefairBundleStoreDefinitions.Count && TradefairBundleStoreDefinitions[value].BundleIdentifier == identifier)
				{
					return TradefairBundleStoreDefinitions[value];
				}
				BundleTradefairDefinitionIndexCache.Clear();
			}
			return null;
		}

		public TradefairBundleContentDefinition GetTradefairBundleContentDefinition(string identifier)
		{
			if (TradefairBundleContentDefinitions != null)
			{
				for (int i = 0; i < TradefairBundleContentDefinitions.Count; i++)
				{
					if (TradefairBundleContentDefinitions[i].Identifier == identifier)
					{
						return TradefairBundleContentDefinitions[i];
					}
				}
			}
			return null;
		}

		public List<GoldShopDefinition> GetOrderedGoldShopDefinitionBundles(PlayerModel player, long currentTimeMs)
		{
			List<GoldShopDefinition> list = new List<GoldShopDefinition>();
			if (GoldShopDefinitions != null)
			{
				for (int i = 0; i < GoldShopDefinitions.Count; i++)
				{
					GoldShopDefinition goldShopDefinition = GoldShopDefinitions[i];
					if (goldShopDefinition.IsNewVersion && (goldShopDefinition.HasDateLimit || (goldShopDefinition.HasDateLimit && currentTimeMs > goldShopDefinition.StartTimeMilliseconds && currentTimeMs < goldShopDefinition.EndTimeMilliseconds)))
					{
						list.Add(goldShopDefinition);
					}
				}
				bool isActivityOpen = player.ActivityManager.IsActivityOpen(ActivityType.FreeBadgeUnequip);
				foreach (GoldShopDefinition goldShopDefinition2 in GetGoldShopDefinitions(isActivityOpen))
				{
					if (!goldShopDefinition2.IsNewVersion)
					{
						list.Add(goldShopDefinition2);
					}
				}
			}
			list.StableSort(delegate (GoldShopDefinition a, GoldShopDefinition b)
			{
				if (a == null || b == null)
				{
					return 0;
				}
				if (a.DisplayOrder < b.DisplayOrder)
				{
					return -1;
				}
				return (a.DisplayOrder > b.DisplayOrder) ? 1 : 0;
			});
			return list;
		}

		public List<TradefairBundleStoreDefinition> GetOrderedTradefairBundles(long currentTimeMs)
		{
			List<TradefairBundleStoreDefinition> list = new List<TradefairBundleStoreDefinition>();
			if (TradefairBundleStoreDefinitions != null)
			{
				for (int i = 0; i < TradefairBundleStoreDefinitions.Count; i++)
				{
					TradefairBundleStoreDefinition tradefairBundleStoreDefinition = TradefairBundleStoreDefinitions[i];
					if (!tradefairBundleStoreDefinition.HasDateLimit || (tradefairBundleStoreDefinition.HasDateLimit && currentTimeMs > tradefairBundleStoreDefinition.StartTimeMilliseconds && currentTimeMs < tradefairBundleStoreDefinition.EndTimeMilliseconds))
					{
						list.Add(tradefairBundleStoreDefinition);
					}
				}
			}
			list.StableSort(delegate(TradefairBundleStoreDefinition a, TradefairBundleStoreDefinition b)
			{
				if (a == null || b == null)
				{
					return 0;
				}
				if (a.DisplayOrder < b.DisplayOrder)
				{
					return -1;
				}
				return (a.DisplayOrder > b.DisplayOrder) ? 1 : 0;
			});
			return list;
		}

		public List<CustomBundleDefinition> GetOrderedCustomBundleDefinitions(long currentTimeMs)
		{
			List<CustomBundleDefinition> list = new List<CustomBundleDefinition>();
			if (CustomBundleDefinitions != null)
			{
				for (int i = 0; i < CustomBundleDefinitions.Length; i++)
				{
					CustomBundleDefinition customBundleDefinition = CustomBundleDefinitions[i];
					if (customBundleDefinition.RefreshTime > 0)
					{
						list.Add(customBundleDefinition);
					}
					else if (customBundleDefinition.HasDateLimit && currentTimeMs > customBundleDefinition.StartTimeMilliseconds && currentTimeMs < customBundleDefinition.EndTimeMilliseconds && customBundleDefinition.RefreshTime <= 0)
					{
						list.Add(customBundleDefinition);
					}
					else
					{
						list.Add(customBundleDefinition);
					}
				}
			}
			list.StableSort(delegate (CustomBundleDefinition a, CustomBundleDefinition b)
			{
				if (a == null || b == null)
				{
					return 0;
				}
				if (a.Order < b.Order)
				{
					return -1;
				}
				return (a.Order > b.Order) ? 1 : 0;
			});
			return list;
		}

		public int GetMaxNumberOfUpgrades(UpgradeType upgradeType)
		{
			int num = -1;
			RarityBasedUpgradeDefinition[] rarityBasedUpgradeDefinitions = RarityBasedUpgradeDefinitions;
			foreach (RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition in rarityBasedUpgradeDefinitions)
			{
				num = ((rarityBasedUpgradeDefinition.UpgradeType == upgradeType && rarityBasedUpgradeDefinition.UpgradesTotal > num) ? rarityBasedUpgradeDefinition.UpgradesTotal : num);
			}
			return num;
		}

		public int GetTotalInitialTraitCountForSurvivorRarityOnlyForBackwardCompatibility(int rarityLevel)
		{
			int result = 0;
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = GetRarityBasedUpgradeDefinition(rarityLevel, UpgradeType.SurvivorUpgrade);
			if (rarityBasedUpgradeDefinition != null)
			{
				result = rarityBasedUpgradeDefinition.InitialTraitCountLevel0;
				result += rarityBasedUpgradeDefinition.InitialTraitCountLevel1;
				result += rarityBasedUpgradeDefinition.InitialTraitCountLevel2;
				result += rarityBasedUpgradeDefinition.InitialTraitCountLevel3;
				result += rarityBasedUpgradeDefinition.InitialTraitCountLevel4;
				result += rarityBasedUpgradeDefinition.InitialTraitCountLevel5;
			}
			return result;
		}

		public Dictionary<TraitBucketsDefinition, int> GetInitialTraitCountsForSurvivorRarity(int rarityLevel)
		{
			Dictionary<TraitBucketsDefinition, int> dictionary = new Dictionary<TraitBucketsDefinition, int>();
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = GetRarityBasedUpgradeDefinition(rarityLevel, UpgradeType.SurvivorUpgrade);
			if (rarityBasedUpgradeDefinition != null)
			{
				dictionary.Add(new TraitBucketsDefinition
				{
					RarityLevel = 4
				}, rarityBasedUpgradeDefinition.InitialTraitCountLevel5);
				dictionary.Add(new TraitBucketsDefinition
				{
					RarityLevel = 3
				}, rarityBasedUpgradeDefinition.InitialTraitCountLevel4);
				dictionary.Add(new TraitBucketsDefinition
				{
					RarityLevel = 2
				}, rarityBasedUpgradeDefinition.InitialTraitCountLevel3);
				dictionary.Add(new TraitBucketsDefinition
				{
					RarityLevel = 1
				}, rarityBasedUpgradeDefinition.InitialTraitCountLevel2);
				dictionary.Add(new TraitBucketsDefinition
				{
					RarityLevel = 0
				}, rarityBasedUpgradeDefinition.InitialTraitCountLevel1);
				dictionary.Add(new TraitBucketsDefinition
				{
					IsLocked = true
				}, rarityBasedUpgradeDefinition.InitialTraitCountLevel0);
			}
			return dictionary;
		}

		public Dictionary<TraitBucketsDefinition, int> GetTraitCountRequirementsForNextSurvivorRarityUpgrade(int survivorRarity)
		{
			Dictionary<TraitBucketsDefinition, int> dictionary = new Dictionary<TraitBucketsDefinition, int>();
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = GetRarityBasedUpgradeDefinition(survivorRarity, UpgradeType.SurvivorUpgrade);
			if (rarityBasedUpgradeDefinition != null)
			{
				AddDefinitionsToRequirements(dictionary, 0, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel0, isLocked: true);
				AddDefinitionsToRequirements(dictionary, 0, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel1);
				AddDefinitionsToRequirements(dictionary, 1, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel2);
				AddDefinitionsToRequirements(dictionary, 2, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel3);
				AddDefinitionsToRequirements(dictionary, 3, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel4);
				AddDefinitionsToRequirements(dictionary, 4, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel5);
				AddDefinitionsToRequirements(dictionary, 5, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel6);
				AddDefinitionsToRequirements(dictionary, 6, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel7);
				AddDefinitionsToRequirements(dictionary, 7, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel8);
				AddDefinitionsToRequirements(dictionary, 8, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel9);
				AddDefinitionsToRequirements(dictionary, 9, rarityBasedUpgradeDefinition.RarityUpgradeTraitCountLevel10);
			}
			return dictionary;
		}

		private void AddDefinitionsToRequirements(Dictionary<TraitBucketsDefinition, int> requirements, int rarityLevel, int amountRequiredTraits, bool isLocked = false)
		{
			if (amountRequiredTraits > 0)
			{
				if (isLocked)
				{
					requirements.Add(new TraitBucketsDefinition
					{
						IsLocked = true
					}, amountRequiredTraits);
				}
				else
				{
					requirements.Add(new TraitBucketsDefinition
					{
						RarityLevel = rarityLevel
					}, amountRequiredTraits);
				}
			}
		}

		public List<SystemOpen> GetSystemOpenTimes(long currentTimeMs)
		{
			List<SystemOpen> list = new List<SystemOpen>();
			if (SystemOpens != null)
			{
				SystemOpen[] systemOpens = SystemOpens;
				foreach (SystemOpen systemOpen in systemOpens)
				{
					if (currentTimeMs >= systemOpen.StartTimeMilliseconds && currentTimeMs <= systemOpen.EndTimeMilliseconds)
					{
						list.Add(systemOpen);
					}
				}
			}
			return list;
		}

		public SystemOpen GetSystemOpenById(string systemId)
		{
			if (SystemOpensById != null && SystemOpensById.TryGetValue(systemId, out var value))
			{
				return value;
			}
			return null;
		}

		public TraitBucketsDefinition.BucketType GetLowestTraitLevelForSurvivorRarityForBackwardCompatibility(int rarityLevel)
		{
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = GetRarityBasedUpgradeDefinition(rarityLevel, UpgradeType.SurvivorUpgrade);
			if (rarityBasedUpgradeDefinition != null)
			{
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel0 > 0)
				{
					return TraitBucketsDefinition.BucketType.Locked;
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel1 > 0)
				{
					return TraitBucketsDefinition.BucketType.LowLevel;
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel2 > 0)
				{
					return TraitBucketsDefinition.BucketType.MidLevel;
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel3 > 0)
				{
					return TraitBucketsDefinition.BucketType.HighLevel;
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel4 > 0)
				{
					return TraitBucketsDefinition.BucketType.Epic;
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel5 > 0)
				{
					return TraitBucketsDefinition.BucketType.Legendary;
				}
			}
			return TraitBucketsDefinition.BucketType.Locked;
		}

		public TraitBucketsDefinition GetLowestTraitLevelForSurvivorRarity(int rarityLevel)
		{
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = GetRarityBasedUpgradeDefinition(rarityLevel, UpgradeType.SurvivorUpgrade);
			if (rarityBasedUpgradeDefinition != null)
			{
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel0 > 0)
				{
					return new TraitBucketsDefinition
					{
						IsLocked = true
					};
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel1 > 0)
				{
					return new TraitBucketsDefinition
					{
						RarityLevel = 0
					};
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel2 > 0)
				{
					return new TraitBucketsDefinition
					{
						RarityLevel = 1
					};
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel3 > 0)
				{
					return new TraitBucketsDefinition
					{
						RarityLevel = 2
					};
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel4 > 0)
				{
					return new TraitBucketsDefinition
					{
						RarityLevel = 3
					};
				}
				if (rarityBasedUpgradeDefinition.InitialTraitCountLevel5 > 0)
				{
					return new TraitBucketsDefinition
					{
						RarityLevel = 4
					};
				}
			}
			return new TraitBucketsDefinition
			{
				IsLocked = true
			};
		}

		public Dictionary<int, TraitBucketsDefinition> GetLevelsThatUnlockATrait(int rarityLevel, UpgradeType upgradeType, int startingLevel, bool replaceTacticalWithLowLevel)
		{
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = GetRarityBasedUpgradeDefinition(rarityLevel, upgradeType);
			int num = rarityBasedUpgradeDefinition.TraitsCountLevel0 + rarityBasedUpgradeDefinition.TraitsCountLevel1 + rarityBasedUpgradeDefinition.TraitsCountLevel2;
			int val = startingLevel + rarityBasedUpgradeDefinition.UpgradesTotal;
			Dictionary<int, int> obj = new Dictionary<int, int>
			{
				[0] = rarityBasedUpgradeDefinition.TraitsCountLevel0,
				[1] = rarityBasedUpgradeDefinition.TraitsCountLevel1,
				[2] = rarityBasedUpgradeDefinition.TraitsCountLevel2
			};
			Dictionary<int, TraitBucketsDefinition> dictionary = new Dictionary<int, TraitBucketsDefinition>();
			if (rarityBasedUpgradeDefinition.TacticalTraitsCount > 0 && !replaceTacticalWithLowLevel)
			{
				dictionary[startingLevel] = new TraitBucketsDefinition
				{
					IsTactical = true
				};
			}
			else if (rarityBasedUpgradeDefinition.TacticalTraitsCount > 0 && replaceTacticalWithLowLevel)
			{
				dictionary[startingLevel] = new TraitBucketsDefinition
				{
					RarityLevel = 0
				};
			}
			float num2 = ((rarityBasedUpgradeDefinition.TacticalTraitsCount > 0 && upgradeType != UpgradeType.EquipmentUpgrade) ? ((float)(rarityBasedUpgradeDefinition.UpgradesTotal - 1) / (float)num) : ((float)rarityBasedUpgradeDefinition.UpgradesTotal / (float)num));
			int num3 = 0;
			foreach (KeyValuePair<int, int> item in obj)
			{
				for (int i = 1; i <= item.Value; i++)
				{
					num3++;
					int key = Math.Min(((rarityBasedUpgradeDefinition.TacticalTraitsCount > 0 && upgradeType != UpgradeType.EquipmentUpgrade) ? (startingLevel + 1) : startingLevel) + (int)(num2 * (float)num3 + 0.5f), val);
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, new TraitBucketsDefinition
						{
							RarityLevel = item.Key
						});
					}
				}
			}
			return dictionary;
		}

		public List<EquipTraitsDefinition> getEquipTraitsDefinitions(SurvivorClass remodelClass, EquipmentCategory equipmentType, int traitsSlot, int traitsQualityLevel, List<string> list)
		{
			return EquipTraitsDefinitions.Where((EquipTraitsDefinition x) => x.SurvivorClass == remodelClass && x.EquipmentType == equipmentType && x.TraitsSlot == traitsSlot && x.TraitsQualityLevel == traitsQualityLevel && !list.Contains(x.TraitsGroup)).ToList();
		}

		public EquipTraitsMutualExclusion getEquipTraitsMutualExclusion(string traitId)
		{
			return EquipTraitsMutualExclusion.Where((EquipTraitsMutualExclusion x) => x.Traits == traitId).FirstOrDefault();
		}

		public SPTraitsRemoldDefinitions GetSPTraitsRemodeDefinition(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			return SPTraitsRemodeDefinition.Where((SPTraitsRemoldDefinitions x) => x.ID == id).FirstOrDefault();
		}

		public List<SPTraitsRemoldDefinitions> GetSPTraitsRemodeDefinitionByType(string type)
		{
			if (string.IsNullOrEmpty(type))
			{
				return null;
			}
			return SPTraitsRemodeDefinition.Where((SPTraitsRemoldDefinitions x) => x.Type == type).ToList();
		}

		public List<SPTraitsRemoldDefinitions> GetSPTraitsRemodeDefinitions(SurvivorClass? survivorClass, string equipType, int? star, List<string> excludedIds = null)
		{
			IEnumerable<SPTraitsRemoldDefinitions> source = SPTraitsRemodeDefinition.AsEnumerable();
			if (survivorClass.HasValue)
			{
				source = source.Where((SPTraitsRemoldDefinitions x) => x.SurvivorClass != null && x.SurvivorClass.Contains(survivorClass.Value.ToString()));
			}
			if (!string.IsNullOrEmpty(equipType))
			{
				source = source.Where((SPTraitsRemoldDefinitions x) => x.EquipType != null && x.EquipType.Contains(equipType));
			}
			if (star.HasValue)
			{
				source = source.Where((SPTraitsRemoldDefinitions x) => x.Star == star.Value);
			}
			if (excludedIds != null && excludedIds.Count > 0)
			{
				source = source.Where((SPTraitsRemoldDefinitions x) => !excludedIds.Contains(x.ID));
			}
			return source.ToList();
		}

		public List<string> GetSPTraitsActiveTraits(string spTraitsId)
		{
			return GetSPTraitsRemodeDefinition(spTraitsId)?.ActiveTraits ?? new List<string>();
		}

		public List<string> GetSPTraitsPassiveTraits(string spTraitsId)
		{
			return GetSPTraitsRemodeDefinition(spTraitsId)?.PassiveTraits ?? new List<string>();
		}

		public List<string> GetSPTraitsActiveTraitsForCharge(string spTraitsId)
		{
			return GetSPTraitsRemodeDefinition(spTraitsId)?.ActiveTraitsForCharge ?? new List<string>();
		}

		public SPTraitsRemoldRandomPackage GetSPTraitsRemoldRandomPackage(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			return SPTraitsRemoldRandomPackages.Where((SPTraitsRemoldRandomPackage x) => x.ID == id).FirstOrDefault();
		}

		public List<SPTraitsRemoldRandomPackage> GetSPTraitsRemoldRandomPackageDefinitions(string packageTag, int? packageStar, List<string> excludedIds = null)
		{
			IEnumerable<SPTraitsRemoldRandomPackage> source = SPTraitsRemoldRandomPackages.AsEnumerable();
			if (!string.IsNullOrEmpty(packageTag))
			{
				source = source.Where((SPTraitsRemoldRandomPackage x) => x.PackageTag == packageTag);
			}
			if (packageStar.HasValue)
			{
				source = source.Where((SPTraitsRemoldRandomPackage x) => x.PackageStar == packageStar.Value);
			}
			if (excludedIds != null && excludedIds.Count > 0)
			{
				source = source.Where((SPTraitsRemoldRandomPackage x) => !excludedIds.Contains(x.ID));
			}
			return source.ToList();
		}

		public AttributeDefinition GetAttributeDefinitionById(string Id)
		{
			return AttributeDefinitions.Where((AttributeDefinition x) => x.ID == Id).FirstOrDefault();
		}

		public List<AttributeDefinition> GetAttributeDefinition()
		{
			return AttributeDefinitions?.ToList();
		}

		public List<TraitDefinition> GetUpgradeTraits(List<string> tags, List<string> ownerFilters, int ownerLevel, SurvivorClass survivorClass)
		{
			List<TraitDefinition> list = new List<TraitDefinition>();
			IEnumerable<TraitDefinition> enumerable = null;
			if (tags != null && tags.Count > 0 && traitDefinitionsByTag != null)
			{
				List<TraitDefinition> list2 = null;
				for (int i = 0; i < tags.Count; i++)
				{
					if (traitDefinitionsByTag.TryGetValue(tags[i], out var value) && value != null)
					{
						if (list2 == null || value.Count < list2.Count)
						{
							list2 = value;
						}
						continue;
					}
					return list;
				}
				IEnumerable<TraitDefinition> enumerable2;
				if (list2 == null)
				{
					IEnumerable<TraitDefinition> traitDefinitions = TraitDefinitions;
					enumerable2 = traitDefinitions;
				}
				else
				{
					IEnumerable<TraitDefinition> traitDefinitions = list2;
					enumerable2 = traitDefinitions;
				}
				enumerable = enumerable2;
			}
			else
			{
				enumerable = TraitDefinitions;
			}
			foreach (TraitDefinition item in enumerable)
			{
				bool flag = true;
				if (tags != null)
				{
					for (int j = 0; j < tags.Count; j++)
					{
						if (!item.HasTag(tags[j]))
						{
							flag = false;
							break;
						}
					}
				}
				bool flag2 = survivorClass != SurvivorClass.None && item.OwnerFilters != null;
				if (flag)
				{
					bool flag3 = true;
					if (flag2 && item.HasSurvivorClassFilter)
					{
						flag3 = item.OwnerFilters.Contains(survivorClass.ToString());
					}
					if (item.OwnerFilters != null && item.OwnerFilters.Count > 0 && ownerFilters != null && ownerFilters.Count > 0)
					{
						bool flag4 = false;
						for (int k = 0; k < ownerFilters.Count; k++)
						{
							if (item.OwnerFilters.Contains(ownerFilters[k]))
							{
								flag4 = true;
								break;
							}
						}
						flag = flag4;
					}
					if (flag2)
					{
						flag = flag && flag3;
					}
				}
				if (flag)
				{
					list.Add(item);
				}
			}
			return list;
		}

		private bool HasSurvivorClassFilter(Regex regex, List<string> filters)
		{
			if (filters != null)
			{
				return regex.Match(string.Join(";", filters.ToArray())).Success;
			}
			return false;
		}

		public RewardCurrency GetUnlockShareRewardForSurvivor(ActorDefinition actorDefinition)
		{
			if (actorDefinition != null && ConfigData != null)
			{
				return new RewardCurrency
				{
					Amount = ConfigData.ShareUnlockRewardTokenAmount,
					CurrencyType = actorDefinition.TraitUpgradeCurrency
				};
			}
			return null;
		}

		public bool IsUnlockShareRewardEnabled()
		{
			return ConfigData.ShareUnlockRewardTokenAmount > 0;
		}

		public GrindButtonDefinition GetGrindButtonDefinition(int grindButtonDefinitionId)
		{
			if (GrindButtonDefinitions != null)
			{
				for (int i = 0; i < GrindButtonDefinitions.Length; i++)
				{
					if (GrindButtonDefinitions[i].Id == grindButtonDefinitionId)
					{
						return GrindButtonDefinitions[i];
					}
				}
			}
			return null;
		}

		public ScavengeRewardCurrencyMultiplier GetScavengeRewardCurrencyMultiplier(CurrencyType currencyType, DropEventDefinition.DropEventContext context)
		{
			if (ScavengeRewardCurrencyMultipliers != null)
			{
				for (int i = 0; i < ScavengeRewardCurrencyMultipliers.Length; i++)
				{
					if (ScavengeRewardCurrencyMultipliers[i] != null && ScavengeRewardCurrencyMultipliers[i].Currency != CurrencyType.None && ScavengeRewardCurrencyMultipliers[i].Currency == currencyType && ScavengeRewardCurrencyMultipliers[i].Context == context)
					{
						return ScavengeRewardCurrencyMultipliers[i];
					}
				}
			}
			return null;
		}

		public ComponentDropType GetComponentDropType(int scavengerLevel, DropEventDefinition.DropEventTag tag, ActivityManager activityManager)
		{
			for (int i = 0; i < ((ComponentDropTypes != null) ? ComponentDropTypes.Length : 0); i++)
			{
				ComponentDropType componentDropType = ComponentDropTypes[i];
				if (activityManager.CheckCanDrop(componentDropType) && componentDropType != null && componentDropType.LootTag == tag && componentDropType.ScavengerLevel == scavengerLevel)
				{
					return componentDropType;
				}
			}
			return null;
		}

		public List<ItemAmountProbabilityData> GetComponentProbabilities(int scavengerLevel, DropEventDefinition.DropEventTag tag, ActivityManager activityManager, string fixedType = null, int fixedRarity = -1)
		{
			ComponentDropType componentDropType = GetComponentDropType(scavengerLevel, tag, activityManager);
			List<ItemAmountProbabilityData> list = new List<ItemAmountProbabilityData>();
			if (componentDropType != null)
			{
				Type typeFromHandle = typeof(ComponentDropType);
				List<CurrencyType> list2 = (string.IsNullOrEmpty(fixedType) ? ComponentHelper.GetAllComponentBaseCurrencies() : new List<CurrencyType> { (CurrencyType)Enum.Parse(typeof(CurrencyType), fixedType + 0) });
				for (int i = 0; i < list2.Count; i++)
				{
					string text = list2[i].ToString();
					string name = text.Substring(0, text.Length - 1);
					FieldInfo field = typeFromHandle.GetField(name);
					if (field != null)
					{
						list.Add(new ItemAmountProbabilityData
						{
							Name = name,
							ItemEnumType = list2[i].GetType(),
							ItemEnumValue = (int)list2[i],
							Probability = (FixedPoint)field.GetValue(componentDropType) / componentDropType.SumOfProbabilities
						});
					}
					Dictionary<int, FixedPoint> componentRarityProbabilities = GetComponentRarityProbabilities(DropType.Regular, DropRewardType.Component, scavengerLevel, tag, DropEventDefinition.DropEventContext.Normal, fixedRarity);
					if (componentRarityProbabilities.Count <= 0 || list.Count <= 0)
					{
						continue;
					}
					ItemAmountProbabilityData itemAmountProbabilityData = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
					foreach (KeyValuePair<int, FixedPoint> item in componentRarityProbabilities)
					{
						ItemAmountProbabilityData itemAmountProbabilityData2 = new ItemAmountProbabilityData(itemAmountProbabilityData);
						itemAmountProbabilityData2.Rarity = item.Key;
						if (fixedType == null && fixedRarity < 0)
						{
							itemAmountProbabilityData2.Probability = item.Value * itemAmountProbabilityData.Probability;
						}
						else if (fixedType != null && fixedRarity < 0)
						{
							itemAmountProbabilityData2.Probability = item.Value;
						}
						else if (fixedType != null && fixedRarity >= 0)
						{
							itemAmountProbabilityData2.Probability = 1L;
						}
						list.Add(itemAmountProbabilityData2);
					}
				}
			}
			list.StableSort(SortItemAmountProbabilityData);
			return list;
		}

		public List<ItemAmountProbabilityData> GetBadgeProbabilities(List<CurrencyType> components)
		{
			List<ItemAmountProbabilityData> list = new List<ItemAmountProbabilityData>();
			list.Add(new ItemAmountProbabilityData
			{
				Probability = 1L
			});
			Dictionary<int, FixedPoint> badgeRarityProbabilities = GetBadgeRarityProbabilities(components);
			if (badgeRarityProbabilities.Count > 0 && list.Count > 0)
			{
				ItemAmountProbabilityData other = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				foreach (KeyValuePair<int, FixedPoint> item in badgeRarityProbabilities)
				{
					ItemAmountProbabilityData itemAmountProbabilityData = new ItemAmountProbabilityData(other);
					itemAmountProbabilityData.Rarity = item.Key;
					itemAmountProbabilityData.Probability *= item.Value;
					list.Add(itemAmountProbabilityData);
				}
			}
			list.StableSort(SortItemAmountProbabilityData);
			return list;
		}

		public List<ItemAmountProbabilityData> GetCurrencyProbabilities(DropEventDefinition.DropEventType eventType, DropType inDropType, DropEventDefinition.DropEventContext dropContext, DropEventDefinition.DropEventTag tag, int targetLevel, out DropType usedDropType, ActivityManager activityManager, DropCurrenciesProbabilitiesDefinition.DropCurrency forcedCurrency = DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency)
		{
			DropType dropType = inDropType;
			DropEventDefinition dropEvent = GetDropEvent(eventType, dropContext, tag);
			if (eventType == DropEventDefinition.DropEventType.TradeCrate && dropEvent != null)
			{
				int num = (int)FixedPoint.Round(dropEvent.GoldDropProbability / dropEvent.SumOfProbabilities) * 100;
				int num2 = (int)FixedPoint.Round(dropEvent.SilverDropProbability / dropEvent.SumOfProbabilities) * 100;
				if (num == 100)
				{
					dropType = DropType.Gold;
				}
				else if (num2 == 100)
				{
					dropType = DropType.Silver;
				}
			}
			DropCurrenciesProbabilitiesDefinition dropCurrenciesProbabilities = GetDropCurrenciesProbabilities(eventType, dropType, tag, targetLevel);
			List<ItemAmountProbabilityData> list = new List<ItemAmountProbabilityData>();
			int num3 = ((forcedCurrency != DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency) ? 1 : 16);
			if (dropCurrenciesProbabilities != null)
			{
				Type typeFromHandle = typeof(DropCurrenciesProbabilitiesDefinition);
				for (int i = 0; i < num3; i++)
				{
					FixedPoint fixedPoint = 0L;
					DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency;
					if (forcedCurrency == DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency)
					{
						dropCurrency = (DropCurrenciesProbabilitiesDefinition.DropCurrency)i;
						FieldInfo field = typeFromHandle.GetField(string.Concat(dropCurrency, "Probability"));
						if (field != null)
						{
							fixedPoint = new FixedPoint((float)field.GetValue(dropCurrenciesProbabilities));
							if (fixedPoint > 0L && dropCurrenciesProbabilities.SumOfProbabilities > 0f)
							{
								list.Add(new ItemAmountProbabilityData
								{
									Name = dropCurrency.ToString(),
									ItemEnumValue = (int)dropCurrency,
									ItemEnumType = dropCurrency.GetType(),
									Probability = fixedPoint / dropCurrenciesProbabilities.SumOfProbabilities
								});
							}
						}
					}
					else
					{
						dropCurrency = forcedCurrency;
						list.Add(new ItemAmountProbabilityData
						{
							Name = dropCurrency.ToString(),
							ItemEnumValue = (int)dropCurrency,
							ItemEnumType = dropCurrency.GetType(),
							Probability = new FixedPoint(1)
						});
					}
					if (!(fixedPoint > 0L) || (dropCurrency != DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor && dropCurrency != DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor && dropCurrency != DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon))
					{
						continue;
					}
					DropRewardType dropRewardType = DropRewardType.Armor;
					dropRewardType = ((dropCurrency != DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor) ? DropRewardType.Weapon : DropRewardType.Survivor);
					DropEventDefinition.DropEventTag tag2 = DropEventDefinition.DropEventTag.None;
					if (tag == DropEventDefinition.DropEventTag.TradeCrateGolden || tag == DropEventDefinition.DropEventTag.TradeCrateGearHigh || tag == DropEventDefinition.DropEventTag.TradeCrateGearMid || tag == DropEventDefinition.DropEventTag.TradeCrateGearLow || tag == DropEventDefinition.DropEventTag.ChallengeCrateGold || tag == DropEventDefinition.DropEventTag.ChallengeCrateSilver)
					{
						tag2 = tag;
					}
					Dictionary<int, FixedPoint> equipmentAndSurvivorRarityProbabilities = GetEquipmentAndSurvivorRarityProbabilities(dropType, dropRewardType, targetLevel, tag2, dropEvent.DropContext);
					if (equipmentAndSurvivorRarityProbabilities.Count <= 0 || list.Count <= 0)
					{
						continue;
					}
					ItemAmountProbabilityData other = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
					foreach (KeyValuePair<int, FixedPoint> item in equipmentAndSurvivorRarityProbabilities)
					{
						ItemAmountProbabilityData itemAmountProbabilityData = new ItemAmountProbabilityData(other);
						itemAmountProbabilityData.Rarity = item.Key;
						itemAmountProbabilityData.Probability = item.Value * itemAmountProbabilityData.Probability;
						list.Add(itemAmountProbabilityData);
					}
				}
				list.StableSort(SortItemAmountProbabilityData);
			}
			for (int j = 0; j < list.Count; j++)
			{
				ItemAmountProbabilityData itemAmountProbabilityData2 = list[j];
				string value = Enum.Parse(itemAmountProbabilityData2.ItemEnumType, itemAmountProbabilityData2.ItemEnumValue.ToString()).ToString();
				if (Enum.IsDefined(typeof(CurrencyType), value))
				{
					CurrencyType currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), value);
					DropCurrenciesAmountsDefinition dropCurrencyAmountDefinition = GetDropCurrencyAmountDefinition(dropType, currency, targetLevel, tag);
					if (dropCurrencyAmountDefinition == null)
					{
						continue;
					}
					if (activityManager.IsActivityOpen(ActivityType.TomatoMonday))
					{
						if (dropCurrencyAmountDefinition.EventMinAmount == dropCurrencyAmountDefinition.EventMaxAmount)
						{
							itemAmountProbabilityData2.Amount = dropCurrencyAmountDefinition.EventMinAmount.ToString();
						}
						else
						{
							itemAmountProbabilityData2.Amount = dropCurrencyAmountDefinition.EventMinAmount + " - " + dropCurrencyAmountDefinition.EventMaxAmount;
						}
					}
					else if (dropCurrencyAmountDefinition.MinAmount == dropCurrencyAmountDefinition.MaxAmount)
					{
						itemAmountProbabilityData2.Amount = dropCurrencyAmountDefinition.MinAmount.ToString();
					}
					else
					{
						itemAmountProbabilityData2.Amount = dropCurrencyAmountDefinition.MinAmount + " - " + dropCurrencyAmountDefinition.MaxAmount;
					}
				}
				else if (typeof(DropCurrenciesProbabilitiesDefinition.DropCurrency) == itemAmountProbabilityData2.ItemEnumType && itemAmountProbabilityData2.ItemEnumValue == 12)
				{
					itemAmountProbabilityData2.Amount = "1";
				}
			}
			usedDropType = dropType;
			return list;
		}

		private static int SortItemAmountProbabilityData(ItemAmountProbabilityData a, ItemAmountProbabilityData b)
		{
			int num = Math.Sign(a.Rarity - b.Rarity) * -1;
			if (num == 0)
			{
				num = Math.Sign(a.Probability.Value - b.Probability.Value);
			}
			if (num == 0)
			{
				num = a.Name.CompareTo(b.Name);
			}
			return num;
		}

		public RadioCallProbabilityData GetRadioCallProbabilities(DropEventDefinition.DropEventType eventType, DropEventDefinition.DropEventContext context, DropEventDefinition.DropEventTag tag, DropType dropType, int controlLevel, int callSlotNumber, long playerLifeTime, SpecialPhoneCallState callState = null)
		{
			RadioCallProbabilityData radioCallProbabilityData = new RadioCallProbabilityData();
			PhoneCallDefinition phoneCallDefinition = GetPhoneCallDefinition(playerLifeTime, callSlotNumber);
			int num = 0;
			List<ItemAmountProbabilityData> list = new List<ItemAmountProbabilityData>();
			if (phoneCallDefinition != null)
			{
				num = phoneCallDefinition.InitialProbabilityPercentage;
				CurrencyType[] parsedCurrencyTypeValues = phoneCallDefinition.GetParsedCurrencyTypeValues();
				if (parsedCurrencyTypeValues != null && parsedCurrencyTypeValues.Length != 0)
				{
					for (int i = 0; i < parsedCurrencyTypeValues.Length; i++)
					{
						if (callState != null)
						{
							num = callState.CumulativeProbability;
						}
						list.Add(new ItemAmountProbabilityData
						{
							Name = parsedCurrencyTypeValues[i].ToString(),
							ItemEnumValue = (int)parsedCurrencyTypeValues[i],
							ItemEnumType = typeof(CurrencyType),
							Probability = (float)num / 100f
						});
					}
				}
				else if (phoneCallDefinition.HeroGuaranteed)
				{
					radioCallProbabilityData.GuaranteedHero = true;
				}
			}
			radioCallProbabilityData.HighlightedProbabilities = list;
			FixedPoint fixedPoint = new FixedPoint(1f - (float)num / 100f);
			List<ItemAmountProbabilityData> list2 = new List<ItemAmountProbabilityData>();
			DropCurrenciesProbabilitiesDefinition dropCurrenciesProbabilities = GetDropCurrenciesProbabilities(eventType, dropType, tag, controlLevel);
			if (dropCurrenciesProbabilities != null)
			{
				Type typeFromHandle = typeof(DropCurrenciesProbabilitiesDefinition);
				for (int j = 0; j < 16; j++)
				{
					DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency = (DropCurrenciesProbabilitiesDefinition.DropCurrency)j;
					FieldInfo field = typeFromHandle.GetField(string.Concat(dropCurrency, "Probability"));
					if (!(field != null))
					{
						continue;
					}
					FixedPoint fixedPoint2 = new FixedPoint((float)field.GetValue(dropCurrenciesProbabilities));
					if (fixedPoint2 > 0L)
					{
						list2.Add(new ItemAmountProbabilityData
						{
							Name = dropCurrency.ToString(),
							ItemEnumValue = (int)dropCurrency,
							ItemEnumType = dropCurrency.GetType(),
							Rarity = -1,
							Probability = fixedPoint2 / dropCurrenciesProbabilities.SumOfProbabilities * fixedPoint
						});
					}
					if (dropCurrency != DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
					{
						continue;
					}
					List<KeyValuePair<int, FixedPoint>> heroTokenDistributionProbabilities = GetHeroTokenDistributionProbabilities(DropEventDefinition.DropEventType.RadioPhone, dropType, DropEventDefinition.DropEventTag.None, controlLevel);
					if (heroTokenDistributionProbabilities.Count <= 0 || list2.Count <= 0)
					{
						continue;
					}
					ItemAmountProbabilityData other = list2[list2.Count - 1];
					list2.RemoveAt(list2.Count - 1);
					foreach (KeyValuePair<int, FixedPoint> item in heroTokenDistributionProbabilities)
					{
						ItemAmountProbabilityData itemAmountProbabilityData = new ItemAmountProbabilityData(other);
						itemAmountProbabilityData.Rarity = item.Key;
						itemAmountProbabilityData.Probability = item.Value * itemAmountProbabilityData.Probability;
						list2.Add(itemAmountProbabilityData);
					}
				}
				radioCallProbabilityData.Probabilities = list2;
			}
			return radioCallProbabilityData;
		}

		public Dictionary<int, FixedPoint> GetComponentRarityProbabilities(DropType dropType, DropRewardType rewardType, int targetLevel, DropEventDefinition.DropEventTag tag = DropEventDefinition.DropEventTag.None, DropEventDefinition.DropEventContext context = DropEventDefinition.DropEventContext.Normal, int fixedRarity = -1)
		{
			DropEquipmentsAndSurvivorsRaritiesDefinition dropRarityDefinition = GetDropRarityDefinition(dropType, rewardType, targetLevel, tag, context);
			Dictionary<int, FixedPoint> dictionary = new Dictionary<int, FixedPoint>();
			if (dropRarityDefinition != null)
			{
				Type typeFromHandle = typeof(DropEquipmentsAndSurvivorsRaritiesDefinition);
				if (fixedRarity < 0)
				{
					for (int i = 0; i < 5; i++)
					{
						Rarity rarity = (Rarity)i;
						FieldInfo field = typeFromHandle.GetField(string.Concat(rarity, "Probability"));
						if (field != null)
						{
							FixedPoint fixedPoint = (FixedPoint)field.GetValue(dropRarityDefinition);
							if (fixedPoint > 0L)
							{
								dictionary.Add(i, fixedPoint / dropRarityDefinition.SumOfProbabilities);
							}
						}
					}
				}
				else
				{
					FieldInfo field2 = typeFromHandle.GetField(string.Concat((Rarity)fixedRarity, "Probability"));
					if (field2 != null)
					{
						FixedPoint fixedPoint2 = (FixedPoint)field2.GetValue(dropRarityDefinition);
						if (fixedPoint2 > 0L)
						{
							dictionary.Add(fixedRarity, fixedPoint2 / dropRarityDefinition.SumOfProbabilities);
						}
					}
				}
			}
			return dictionary;
		}

		public Dictionary<int, FixedPoint> GetEquipmentAndSurvivorRarityProbabilities(DropType dropType, DropRewardType rewardType, int targetLevel, DropEventDefinition.DropEventTag tag = DropEventDefinition.DropEventTag.None, DropEventDefinition.DropEventContext context = DropEventDefinition.DropEventContext.Normal)
		{
			Dictionary<int, FixedPoint> result = new Dictionary<int, FixedPoint>();
			DropEquipmentsAndSurvivorsRaritiesDefinition dropRarityDefinition = GetDropRarityDefinition(dropType, rewardType, targetLevel, tag, context);
			if (dropRarityDefinition != null)
			{
				result = GenericGetRarityToProbability(dropRarityDefinition, "Probability", dropRarityDefinition.SumOfProbabilities);
			}
			return result;
		}

		private Dictionary<int, FixedPoint> GenericGetRarityToProbability(object obj, string fieldSuffix, FixedPoint sumOfProbabilities)
		{
			Dictionary<int, FixedPoint> dictionary = new Dictionary<int, FixedPoint>();
			Type type = obj.GetType();
			for (int i = 0; i < 5; i++)
			{
				Rarity rarity = (Rarity)i;
				FieldInfo field = type.GetField(rarity.ToString() + fieldSuffix);
				if (field != null)
				{
					FixedPoint fixedPoint = (FixedPoint)field.GetValue(obj);
					if (fixedPoint > 0L)
					{
						dictionary.Add(i, fixedPoint / sumOfProbabilities);
					}
				}
			}
			return dictionary;
		}

		public Dictionary<int, FixedPoint> GetBadgeRarityProbabilities(List<CurrencyType> components)
		{
			Dictionary<int, FixedPoint> dictionary = new Dictionary<int, FixedPoint>();
			BadgeRarityResult obj = CalculateBadgeRarityResult(components);
			Type typeFromHandle = typeof(BadgeRarityResult);
			for (int i = 0; i < 5; i++)
			{
				Rarity rarity = (Rarity)i;
				FieldInfo field = typeFromHandle.GetField(rarity.ToString());
				if (field != null)
				{
					FixedPoint fixedPoint = (FixedPoint)field.GetValue(obj);
					if (fixedPoint > 0L)
					{
						dictionary.Add(i, fixedPoint / 100.0);
					}
				}
			}
			return dictionary;
		}

		public BadgeRarityResult CalculateBadgeRarityResult(List<CurrencyType> components)
		{
			if (components != null && components.Count == 5)
			{
				int num = 0;
				for (int i = 0; i < components.Count; i++)
				{
					BadgeComponentRarityValue componentRarityValue = GetComponentRarityValue(ComponentHelper.GetComponentRarityLevel(components[i]));
					if (componentRarityValue != null)
					{
						num = ((i != 0) ? (num + componentRarityValue.Other) : (num + componentRarityValue.Fixed));
					}
				}
				return GetBadgeRarityResult(num);
			}
			return null;
		}

		public Dictionary<int, List<CurrencyType>> GetRarityToHeroTokensMapping(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag dropTag, int controlLevel)
		{
			List<CurrencyType> possibleHeroTokensByRadioTentLevel = GetPossibleHeroTokensByRadioTentLevel(eventType, dropType, dropTag, controlLevel);
			Dictionary<int, List<CurrencyType>> dictionary = new Dictionary<int, List<CurrencyType>>();
			for (int i = 0; i < ((HeroTokenDropDistributionDefinitions != null) ? HeroTokenDropDistributionDefinitions.Length : 0); i++)
			{
				HeroTokenDropDistributionDefinition heroTokenDropDistributionDefinition = HeroTokenDropDistributionDefinitions[i];
				if (!(heroTokenDropDistributionDefinition.BucketId == "HeroGrouping"))
				{
					continue;
				}
				Type typeFromHandle = typeof(HeroTokenDropDistributionDefinition);
				for (int j = 0; j < (int)CurrencyType.Count; j++)
				{
					CurrencyType item = (CurrencyType)j;
					FieldInfo field = typeFromHandle.GetField(item.ToString());
					if (!(field != null))
					{
						continue;
					}
					FixedPoint fixedPoint = (FixedPoint)field.GetValue(heroTokenDropDistributionDefinition);
					if (fixedPoint >= 0L)
					{
						List<CurrencyType> value = null;
						if (!dictionary.TryGetValue((int)fixedPoint, out value))
						{
							value = new List<CurrencyType>();
							dictionary[(int)fixedPoint] = value;
						}
						if (possibleHeroTokensByRadioTentLevel.Contains(item))
						{
							value.Add(item);
						}
					}
				}
				break;
			}
			return dictionary;
		}

		public List<KeyValuePair<int, FixedPoint>> GetHeroTokenDistributionProbabilities(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, int targetLevel)
		{
			Dictionary<int, FixedPoint> dictionary = new Dictionary<int, FixedPoint>();
			FixedPoint fixedPoint = 0L;
			HeroTokenDropDefinition heroTokenDropDefinition = GetHeroTokenDropDefinition(eventType, dropType, tag, targetLevel);
			if (heroTokenDropDefinition != null)
			{
				Dictionary<CurrencyType, int> dictionary2 = new Dictionary<CurrencyType, int>();
				for (int i = 0; i < ((HeroTokenDropDistributionDefinitions != null) ? HeroTokenDropDistributionDefinitions.Length : 0); i++)
				{
					HeroTokenDropDistributionDefinition heroTokenDropDistributionDefinition = HeroTokenDropDistributionDefinitions[i];
					if (heroTokenDropDistributionDefinition.BucketId == "HeroGrouping")
					{
						Type typeFromHandle = typeof(HeroTokenDropDistributionDefinition);
						for (int j = 0; j < (int)CurrencyType.Count; j++)
						{
							CurrencyType key = (CurrencyType)j;
							FieldInfo field = typeFromHandle.GetField(key.ToString());
							if (field != null)
							{
								FixedPoint fixedPoint2 = (FixedPoint)field.GetValue(heroTokenDropDistributionDefinition);
								if (fixedPoint2 >= 0L)
								{
									dictionary2.Add(key, (int)fixedPoint2);
								}
							}
						}
					}
					else
					{
						if (!(heroTokenDropDistributionDefinition.BucketId == heroTokenDropDefinition.BucketId) || !(heroTokenDropDistributionDefinition.BucketId != "HeroGrouping"))
						{
							continue;
						}
						Type typeFromHandle2 = typeof(HeroTokenDropDistributionDefinition);
						for (int k = 0; k < (int)CurrencyType.Count; k++)
						{
							CurrencyType key2 = (CurrencyType)k;
							FieldInfo field2 = typeFromHandle2.GetField(key2.ToString());
							if (!(field2 != null))
							{
								continue;
							}
							FixedPoint fixedPoint3 = (FixedPoint)field2.GetValue(heroTokenDropDistributionDefinition);
							if (!(fixedPoint3 > 0L))
							{
								continue;
							}
							int value = 0;
							if (dictionary2.TryGetValue(key2, out value))
							{
								if (dictionary.TryGetValue(value, out var value2))
								{
									value2 += fixedPoint3;
								}
								else
								{
									value2 = fixedPoint3;
								}
								dictionary[value] = value2;
							}
						}
						fixedPoint = heroTokenDropDistributionDefinition.SumOfProbabilities;
					}
				}
			}
			foreach (int item in new List<int>(dictionary.Keys))
			{
				FixedPoint value3 = dictionary[item];
				if (fixedPoint > 0L)
				{
					value3 /= fixedPoint;
					dictionary[item] = value3;
				}
			}
			return dictionary.ToList();
		}

		public GoldShopDefinition GetGoldShopDefinition(string itemId, bool isActivityOpen)
		{
			for (int i = 0; i < GoldShopDefinitions.Count; i++)
			{
				if ((!isActivityOpen || !(GoldShopDefinitions[i].EventControl == "Free Badge Unequip2")) && (isActivityOpen || !(GoldShopDefinitions[i].EventControl == "Free Badge Unequip1")) && GoldShopDefinitions[i].ItemId == itemId)
				{
					return GoldShopDefinitions[i];
				}
			}
			return null;
		}

		public List<GoldShopDefinition> GetGoldShopDefinitions(bool isActivityOpen)
		{
			if (isActivityOpen)
			{
				return GoldShopDefinitions.Where((GoldShopDefinition x) => x.EventControl != "Free Badge Unequip2" && !x.IsNewVersion).ToList();
			}
			return GoldShopDefinitions.Where((GoldShopDefinition x) => x.EventControl != "Free Badge Unequip1" && !x.IsNewVersion).ToList();
		}

		public GoldShopDefinition GetGoldShopDefinition(string itemId)
		{
			for (int i = 0; i < GoldShopDefinitions.Count; i++)
			{
				if (GoldShopDefinitions[i].ItemId == itemId)
				{
					return GoldShopDefinitions[i];
				}
			}
			return null;
		}

		public BadgeBonusDefinition GetBadgeBonusDefinition(string id)
		{
			for (int i = 0; i < ((BadgeBonusDefinitions != null) ? BadgeBonusDefinitions.Length : 0); i++)
			{
				if (BadgeBonusDefinitions[i].ID == id)
				{
					return BadgeBonusDefinitions[i];
				}
			}
			return null;
		}

		public BadgeEffectDefinition GetBadgeEffectDefinition(string id, int level = 1)
		{
			if (level == 0)
			{
				level = 1;
			}
			for (int i = 0; i < ((BadgeEffectDefinitions != null) ? BadgeEffectDefinitions.Length : 0); i++)
			{
				if (BadgeEffectDefinitions[i].ID == id && (BadgeEffectDefinitions[i].Level == 0 || BadgeEffectDefinitions[i].Level == level))
				{
					return BadgeEffectDefinitions[i];
				}
			}
			return null;
		}

		public BadgeRarityResult GetBadgeRarityResult(int componentsValue)
		{
			BadgeRarityResult result = null;
			for (int i = 0; i < ((BadgeRarityResults != null) ? BadgeRarityResults.Length : 0); i++)
			{
				if (componentsValue >= BadgeRarityResults[i].Total)
				{
					result = BadgeRarityResults[i];
				}
			}
			return result;
		}

		public BadgeComponentRarityValue GetComponentRarityValue(int rarity)
		{
			for (int i = 0; i < ((BadgeComponentRarityValues != null) ? BadgeComponentRarityValues.Length : 0); i++)
			{
				if (BadgeComponentRarityValues[i].Rarity == rarity)
				{
					return BadgeComponentRarityValues[i];
				}
			}
			return null;
		}

		private void RecalculateAllDropProbabilities()
		{
			for (int i = 0; i < DropCurrencyProbabilitiesDefinitions.Length; i++)
			{
				DropCurrencyProbabilitiesDefinitions[i].PopulateProbabilitiesList();
			}
			for (int j = 0; j < DropEquipmentsAndSurvivorsRaritiesDefinitions.Length; j++)
			{
				DropEquipmentsAndSurvivorsRaritiesDefinitions[j].PopulateRaritiesProbabilitiesList();
			}
			for (int k = 0; k < HeroTokenDropDistributionDefinitions.Length; k++)
			{
				HeroTokenDropDistributionDefinitions[k].PopulateProbabilitiesList();
			}
		}

		private void CalculateEndlassDebuff()
		{
			if (EndlessModeExpertDebuffConfigs == null)
			{
				return;
			}
			EndlessModeExpertDebuffConfigById = new Dictionary<int, EndlessModeExpertDebuffConfig>();
			for (int i = 0; i < EndlessModeExpertDebuffConfigs.Length; i++)
			{
				EndlessModeExpertDebuffConfig endlessModeExpertDebuffConfig = EndlessModeExpertDebuffConfigs[i];
				List<DifficultyIncrementalDebuff> list = new List<DifficultyIncrementalDebuff>();
				if (EndlessModeExpertDebuffConfigs[i].Debuff != null)
				{
					foreach (string item in endlessModeExpertDebuffConfig.Debuff)
					{
						list.Add(GetChallengeDebuff(item));
					}
				}
				endlessModeExpertDebuffConfig.SetDebuffss(list);
				if (PhoneCallDefinitionsBySlotNumber.ContainsKey(endlessModeExpertDebuffConfig.Wave))
				{
					EndlessModeExpertDebuffConfigById.Add(endlessModeExpertDebuffConfig.Wave, endlessModeExpertDebuffConfig);
				}
			}
		}

		private void CreateFakeChallenges()
		{
			if (ConfigData == null || ConfigData.FakeChallengesInterval == 0)
			{
				return;
			}
			if (WeeklyChallenges != null)
			{
				WeeklyChallenges.Clear();
				WeeklyChallengesById.Clear();
			}
			else
			{
				WeeklyChallenges = new List<WeeklyChallenge>();
				WeeklyChallengesById = new Dictionary<int, WeeklyChallenge>();
			}
			DateTime today = DateTime.Today;
			DateTime dateTime = DateTime.Today;
			int identifier = 0;
			int num = 0;
			while (dateTime.Day == today.Day)
			{
				WeeklyChallenge weeklyChallenge = new WeeklyChallenge();
				weeklyChallenge.Identifier = identifier;
				if (ConfigData.FakeChallengesDetailMapId == null)
				{
					weeklyChallenge.DetailMapId = 1576780561;
					weeklyChallenge.ApocalypticMapId = 10014;
				}
				else
				{
					weeklyChallenge.DetailMapId = ConfigData.FakeChallengesDetailMapId[num];
					weeklyChallenge.ApocalypticMapId = ConfigData.FakeApocalypticChallengesDetailMapId[num];
					num++;
					if (num >= ConfigData.FakeChallengesDetailMapId.Count)
					{
						num = 0;
					}
				}
				dateTime = dateTime.AddMinutes(15.0);
				weeklyChallenge.StartTimeUTC = dateTime.ToUniversalTime().ToString();
				dateTime = dateTime.AddSeconds(ConfigData.FakeChallengesInterval);
				weeklyChallenge.EndTimeUTC = dateTime.ToUniversalTime().ToString();
				WeeklyChallenges.Add(weeklyChallenge);
				if (!WeeklyChallengesById.ContainsKey(weeklyChallenge.Identifier))
				{
					WeeklyChallengesById.Add(weeklyChallenge.Identifier, weeklyChallenge);
				}
			}
		}

		private void CreateFakeSurvival()
		{
			if (ConfigData == null || ConfigData.FakeSurvivalInterval == 0)
			{
				return;
			}
			if (WeeklySurvivals != null)
			{
				WeeklySurvivals.Clear();
			}
			else
			{
				WeeklySurvivals = new List<WeeklySurvival>();
			}
			DateTime today = DateTime.Today;
			DateTime dateTime = DateTime.Today;
			int identifier = 0;
			int num = 0;
			while (dateTime.Day == today.Day)
			{
				WeeklySurvival weeklySurvival = new WeeklySurvival();
				weeklySurvival.Identifier = identifier;
				if (ConfigData.FakeSurvivalDetailMapId == null)
				{
					weeklySurvival.DetailMapId = 1111122222;
				}
				else
				{
					weeklySurvival.DetailMapId = ConfigData.FakeSurvivalDetailMapId[num];
					num++;
					if (num >= ConfigData.FakeSurvivalDetailMapId.Count)
					{
						num = 0;
					}
				}
				dateTime = dateTime.AddMinutes(15.0);
				weeklySurvival.StartTimeUTC = dateTime.ToUniversalTime().ToString();
				dateTime = dateTime.AddSeconds(ConfigData.FakeSurvivalInterval);
				weeklySurvival.EndTimeUTC = dateTime.ToUniversalTime().ToString();
				WeeklySurvivals.Add(weeklySurvival);
			}
		}

		private void SetupGuildBattleDifficulty()
		{
			if (GuildBattleSectorDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < GuildBattleSectorDefinitions.Length; i++)
			{
				GuildBattleSectorDefinition guildBattleSectorDefinition = GuildBattleSectorDefinitions[i];
				if (guildBattleSectorDefinition != null)
				{
					guildBattleSectorDefinition.ColumnsDifficulty = new int[4];
					ParseStringArray(guildBattleSectorDefinition.PVEModifier, ref guildBattleSectorDefinition.ColumnsDifficulty);
					guildBattleSectorDefinition.PVPModifierPerArea = new int[4];
					ParseStringArray(guildBattleSectorDefinition.PVPModifier, ref guildBattleSectorDefinition.PVPModifierPerArea);
				}
			}
		}

		private void SetupFakeBattleTiersDifficulty()
		{
			if (FakeBattleDefinitions != null)
			{
				for (int i = 0; i < FakeBattleDefinitions.Length; i++)
				{
					ParseStringArray(FakeBattleDefinitions[i].Tiers, ref FakeBattleDefinitions[i].tiersDifficulty);
				}
			}
		}

		private void SetupGuildWar()
		{
			if (GvGSeasonDefinitions != null)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				for (int i = 0; i < GvGSeasonDefinitions.Length; i++)
				{
					GvGSeasonDefinition gvGSeasonDefinition = GvGSeasonDefinitions[i];
					if (gvGSeasonDefinition != null)
					{
						gvGSeasonDefinition.SetStartTime(dateTime);
						gvGSeasonDefinition.SetEndTime(dateTime);
					}
				}
			}
			if (GuildBattleSectorDefinitions != null && GvgMapConfig != null)
			{
				for (int j = 0; j < GuildBattleSectorDefinitions.Length; j++)
				{
					if (GuildBattleSectorDefinitions[j] == null)
					{
						continue;
					}
					char c = '&';
					if (GuildBattleSectorDefinitions[j].PrerequisitesIdsString != null && GuildBattleSectorDefinitions[j].PrerequisitesIdsString.Contains(c))
					{
						GuildBattleSectorDefinitions[j].AllPrerequisitesMustBeCompleted = true;
					}
					else
					{
						c = ',';
					}
					ParseStringArray(GuildBattleSectorDefinitions[j].PrerequisitesIdsString, ref GuildBattleSectorDefinitions[j].PrerequisitesSectorIds, c);
					for (int k = 0; k < GvgMapIconConfigs.Length; k++)
					{
						if (GvgMapIconConfigs[k]?.Id == GuildBattleSectorDefinitions[j].MapConfigId)
						{
							GuildBattleSectorDefinitions[j].MapIconConfig = GvgMapIconConfigs[k];
							break;
						}
					}
				}
			}
			if (GuildWarDefinitions != null)
			{
				DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				for (int l = 0; l < GuildWarDefinitions.Length; l++)
				{
					GuildWarDefinition guildWarDefinition = GuildWarDefinitions[l];
					if (guildWarDefinition != null)
					{
						ParseStringArray(guildWarDefinition.SectorString, ref guildWarDefinition.SectorsIds);
						guildWarDefinition.SetStartTime(dateTime2);
						guildWarDefinition.SetEndTime(dateTime2);
						guildWarDefinition.SetFirstBattleStartTime(dateTime2);
					}
				}
			}
			ParseStringTuple(GuildWarConfig.PVPSurvivorRangeColumn0String, ref GuildWarConfig.PVPSurvivorRangeColumns);
			ParseStringTuple(GuildWarConfig.PVPSurvivorRangeColumn1String, ref GuildWarConfig.PVPSurvivorRangeColumns);
			ParseStringTuple(GuildWarConfig.PVPSurvivorRangeColumn2String, ref GuildWarConfig.PVPSurvivorRangeColumns);
			ParseStringArray(GuildWarConfig.RetryCostsString, ref GuildWarConfig.RetryCosts);
		}

		private static void ParseStringTuple(string stringMinMax, ref List<Tuple<int, int>> tupleList, char seperator = ',')
		{
			if (string.IsNullOrEmpty(stringMinMax))
			{
				return;
			}
			string[] array = stringMinMax.Split(seperator);
			if (array.Length > 2)
			{
				return;
			}
			if (tupleList == null)
			{
				tupleList = new List<Tuple<int, int>>();
			}
			Tuple<int, int> tuple = new Tuple<int, int>();
			if (!string.IsNullOrEmpty(array[0]))
			{
				string s = array[0].Trim();
				int result = -1;
				if (int.TryParse(s, out result))
				{
					tuple.First = result;
				}
			}
			if (!string.IsNullOrEmpty(array[1]))
			{
				string s2 = array[1].Trim();
				int result2 = -1;
				if (int.TryParse(s2, out result2))
				{
					tuple.Second = result2;
				}
				tupleList.Add(tuple);
			}
		}

		private static void ParseStringArray<T>(string stringArray, ref T[] dataArray, char seperator = ',')
		{
			if (string.IsNullOrEmpty(stringArray))
			{
				return;
			}
			string[] array = stringArray.Split(seperator);
			dataArray = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					continue;
				}
				string text = array[i].Trim();
				if (typeof(T) == typeof(int))
				{
					int result = -1;
					if (int.TryParse(text, out result))
					{
						dataArray[i] = (T)(object)result;
					}
				}
				else if (typeof(T) == typeof(string))
				{
					dataArray[i] = (T)(object)text;
				}
			}
		}

		public RemotePushNotificationConfig GetRemotePushNotificationConfig(string id)
		{
			if (remotePushNotificationConfigsFastLookup == null)
			{
				remotePushNotificationConfigsFastLookup = new Dictionary<string, RemotePushNotificationConfig>();
			}
			if (remotePushNotificationConfigsFastLookup.TryGetValue(id, out var value))
			{
				return value;
			}
			if (RemotePushNotificationConfigs == null)
			{
				return null;
			}
			value = GetRemotePushNotificationConfigInternal(id);
			if (value != null)
			{
				remotePushNotificationConfigsFastLookup.Add(id, value);
			}
			return value;
		}

		private RemotePushNotificationConfig GetRemotePushNotificationConfigInternal(string id)
		{
			if (RemotePushNotificationConfigs == null)
			{
				return null;
			}
			RemotePushNotificationConfig remotePushNotificationConfig = RemotePushNotificationConfigs.SingleOrDefault((RemotePushNotificationConfig i) => i.Id == id);
			if (remotePushNotificationConfig == null)
			{
				return null;
			}
			return remotePushNotificationConfig;
		}

		public string GetGvGBuffIcon(string traitIdentifier)
		{
			if (guildBattleBuffIcons == null)
			{
				guildBattleBuffIcons = new Dictionary<string, GvgBuffIconConfig>();
			}
			if (guildBattleBuffIcons.TryGetValue(traitIdentifier, out var value))
			{
				return value.IconPath;
			}
			value = GetGvGBuffIconInternal(traitIdentifier);
			if (value != null)
			{
				guildBattleBuffIcons.Add(value.Id, value);
				return value.IconPath;
			}
			return null;
		}

		private GvgBuffIconConfig GetGvGBuffIconInternal(string traitIdentifier)
		{
			GvgBuffIconConfig gvgBuffIconConfig = GvgBuffIconConfigs.SingleOrDefault((GvgBuffIconConfig i) => i.Id == traitIdentifier);
			if (gvgBuffIconConfig == null)
			{
				return null;
			}
			return gvgBuffIconConfig;
		}

		public List<GuildShopDefinition> GetGuildShopItemsForSeason(int seasonId)
		{
			List<GuildShopDefinition> list = new List<GuildShopDefinition>();
			GuildShopDefinition[] guildShopDefinitions = GuildShopDefinitions;
			foreach (GuildShopDefinition guildShopDefinition in guildShopDefinitions)
			{
				if (guildShopDefinition.Season == seasonId)
				{
					list.Add(guildShopDefinition);
				}
			}
			return list;
		}

		public int GetGuildBattleMissionRewardRP(int difficultyOffset, bool isPvpMission = false)
		{
			GuildBattleMissionRewardsDefinition guildBattleMissionReward = GetGuildBattleMissionReward(difficultyOffset);
			if (guildBattleMissionReward != null)
			{
				if (isPvpMission)
				{
					return guildBattleMissionReward.PVPMissionRP;
				}
				return guildBattleMissionReward.PVEMissionRP;
			}
			return 0;
		}

		public int GetGuildBattleMissionRewardVP(int difficultyOffset, bool isPvpMission = false)
		{
			GuildBattleMissionRewardsDefinition guildBattleMissionReward = GetGuildBattleMissionReward(difficultyOffset);
			if (guildBattleMissionReward != null)
			{
				if (isPvpMission)
				{
					return guildBattleMissionReward.PVPMissionVP;
				}
				return guildBattleMissionReward.PVEMissionVP;
			}
			return 0;
		}

		public int GetGuildBattleSectorRewardVP(int sectorId)
		{
			return FindMissionSectorDefinition(sectorId)?.SectorVP ?? 0;
		}

		public int GetGuildBattleSectorMissionDifficulty(int sectorId, int column, bool isPvP = false)
		{
			GuildBattleSectorDefinition guildBattleSectorDefinition = FindMissionSectorDefinition(sectorId);
			if (guildBattleSectorDefinition != null)
			{
				if (!isPvP)
				{
					return guildBattleSectorDefinition.ColumnsDifficulty[column];
				}
				return guildBattleSectorDefinition.PVPModifierPerArea[column];
			}
			return 0;
		}

		private GuildBattleMissionRewardsDefinition GetGuildBattleMissionReward(int difficultyOffset)
		{
			GuildBattleMissionRewardsDefinition value = null;
			if (GuildBattleMissionRewardFastLookup == null)
			{
				GuildBattleMissionRewardFastLookup = new Dictionary<int, GuildBattleMissionRewardsDefinition>();
			}
			if (!GuildBattleMissionRewardFastLookup.TryGetValue(difficultyOffset, out value))
			{
				value = GetGuildBattleMissionRewardListInternal(difficultyOffset);
				GuildBattleMissionRewardFastLookup.Add(difficultyOffset, value);
			}
			if (value == null)
			{
				return null;
			}
			return value;
		}

		private GuildBattleMissionRewardsDefinition GetGuildBattleMissionRewardListInternal(int difficultyOffset)
		{
			GuildBattleMissionRewardsDefinition result = null;
			for (int i = 0; i < GvGMissionRewardsDefinitions.Length && GvGMissionRewardsDefinitions[i].Offset <= difficultyOffset; i++)
			{
				if (GvGMissionRewardsDefinitions[i].Offset == difficultyOffset)
				{
					result = GvGMissionRewardsDefinitions[i];
					break;
				}
			}
			return result;
		}

		public GuildBattleSectorDefinition FindMissionSectorDefinition(int id)
		{
			GuildBattleSectorDefinition value = null;
			if (battlSectorsFastLookup == null)
			{
				battlSectorsFastLookup = new Dictionary<int, GuildBattleSectorDefinition>();
			}
			if (!battlSectorsFastLookup.TryGetValue(id, out value))
			{
				value = FindMissionSectorDefinitionInternal(id);
				if (value != null)
				{
					battlSectorsFastLookup[value.Id] = value;
				}
			}
			return value;
		}

		private GuildBattleSectorDefinition FindMissionSectorDefinitionInternal(int id)
		{
			GuildBattleSectorDefinition[] guildBattleSectorDefinitions = GuildBattleSectorDefinitions;
			for (int i = 0; i < ((guildBattleSectorDefinitions != null) ? guildBattleSectorDefinitions.Length : 0); i++)
			{
				if (guildBattleSectorDefinitions[i] != null && guildBattleSectorDefinitions[i].Id == id)
				{
					return guildBattleSectorDefinitions[i];
				}
			}
			return null;
		}

		public FakeBattleDefinition FindFakeBattleDefinition(int tier)
		{
			if (fakeBattleDefinitionsFastLookup == null)
			{
				fakeBattleDefinitionsFastLookup = new Dictionary<int, FakeBattleDefinition>();
			}
			if (!fakeBattleDefinitionsFastLookup.TryGetValue(tier, out var value))
			{
				value = FindFakeBattleDefinitionInternal(tier);
				if (value != null)
				{
					fakeBattleDefinitionsFastLookup.Add(tier, value);
				}
			}
			return value;
		}

		private FakeBattleDefinition FindFakeBattleDefinitionInternal(int tier)
		{
			for (int i = 0; i < FakeBattleDefinitions.Length; i++)
			{
				if (FakeBattleDefinitions[i].tiersDifficulty.Contains(tier))
				{
					return FakeBattleDefinitions[i];
				}
			}
			return null;
		}

		private void UpdateSurvivalConfigCountsAndMasks(SurvivalMissionConfig[] configs, SurvivalMissionConfig.Type type)
		{
			if (configs != null)
			{
				foreach (SurvivalMissionConfig obj in configs)
				{
					obj.MissionType = type;
					obj.UpdateRaiderTypesCounts();
					obj.UpdateBossTypesMask();
					obj.UpdateBurningTypesMask();
				}
			}
		}

		public bool IsEpisodeWeeklyChallenge(int detailMapId)
		{
			foreach (var weeklyChallenge in WeeklyChallenges)
			{
				if (weeklyChallenge.DetailMapId == detailMapId)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsEpisodeApocalypticWeeklyChallenge(int apocalypticMapId)
		{
			foreach (var weeklyChallenge in WeeklyChallenges)
			{
				if (weeklyChallenge.ApocalypticMapId == apocalypticMapId)
				{
					return true;
				}
			}
			return false;
		}

		public WeeklyChallenge GetLastEndedWeeklyChallenge(long timeUTC)
		{
			WeeklyChallenge result = null;
			foreach (var weeklyChallenge in WeeklyChallenges)
			{
				if (weeklyChallenge.EndTimeMilliseconds < timeUTC)
				{
					result = weeklyChallenge;
					continue;
				}
				return result;
			}
			return result;
		}

		private void CalculateWeeklyChallengeTimes()
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			foreach (var weeklyChallenge in WeeklyChallenges)
			{
				weeklyChallenge.SetStartTime(dateTime);
				weeklyChallenge.SetEndTime(dateTime);
			}
		}

		private void CalculateWeeklyChallengeClassTeamChallengeTimes()
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			if (ClassTeamDefinitions != null)
			{
				for (int i = 0; i < ClassTeamDefinitions.Length; i++)
				{
					ClassTeamDefinition obj = ClassTeamDefinitions[i];
					obj.SetStartTime(dateTime);
					obj.SetEndTime(dateTime);
				}
			}
		}

		private void CalculateEquipPrizeWheelDefinition()
		{
			DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			if (EquipPrizeWheelDefinitions != null)
			{
				foreach (var definition in EquipPrizeWheelDefinitions)
				{
					definition.SetTime(time);
				}
			}
			if (EquipPrizeWheelRewards != null)
			{
				foreach (var reward in EquipPrizeWheelRewards)
				{
					reward.SetupWeightAndReward();
				}
			}
		}

		public EquipPrizeWheelDefinition GetEquipPrizeWheelDefinition(string identifier)
		{
			EquipPrizeWheelDefinition[] equipPrizeWheelDefinitions = EquipPrizeWheelDefinitions;
			foreach (EquipPrizeWheelDefinition equipPrizeWheelDefinition in equipPrizeWheelDefinitions)
			{
				if (equipPrizeWheelDefinition.Identifier == identifier)
				{
					return equipPrizeWheelDefinition;
				}
			}
			return null;
		}

		public List<EquipPrizeWheelDefinition> GetOpenEquipPrizeWheelDefinition(long time)
		{
			List<EquipPrizeWheelDefinition> list = new List<EquipPrizeWheelDefinition>();
			EquipPrizeWheelDefinition[] equipPrizeWheelDefinitions = EquipPrizeWheelDefinitions;
			foreach (EquipPrizeWheelDefinition equipPrizeWheelDefinition in equipPrizeWheelDefinitions)
			{
				if (equipPrizeWheelDefinition.IsOpen(time))
				{
					list.Add(equipPrizeWheelDefinition);
				}
			}
			return list;
		}

		private void CalculateWeeklyChallengeCircleBuff()
		{
			if (WeeklyChallengeApocalypseConfigs != null)
			{
				for (int i = 0; i < WeeklyChallengeApocalypseConfigs.Count; i++)
				{
					WeeklyChallengeApocalypseConfig weeklyChallengeApocalypseConfig = WeeklyChallengeApocalypseConfigs[i];
					List<DifficultyIncrementalDebuff> list = new List<DifficultyIncrementalDebuff>();
					if (weeklyChallengeApocalypseConfig.Debuff != null)
					{
						foreach (string item in weeklyChallengeApocalypseConfig.Debuff)
						{
							if (item != null && GetChallengeDebuff(item) != null)
							{
								list.Add(GetChallengeDebuff(item));
							}
						}
					}
					weeklyChallengeApocalypseConfig.SetDebuffConfs(list);
					list = new List<DifficultyIncrementalDebuff>();
					if (weeklyChallengeApocalypseConfig.BaseDebuff != null)
					{
						foreach (string item2 in weeklyChallengeApocalypseConfig.BaseDebuff)
						{
							if (item2 != null && GetChallengeDebuff(item2) != null)
							{
								list.Add(GetChallengeDebuff(item2));
							}
						}
					}
					weeklyChallengeApocalypseConfig.SetBaseDebuffConfs(list);
					list = new List<DifficultyIncrementalDebuff>();
					if (weeklyChallengeApocalypseConfig.LTDebuff != null)
					{
						foreach (string item3 in weeklyChallengeApocalypseConfig.LTDebuff)
						{
							if (item3 != null && GetChallengeDebuff(item3) != null)
							{
								list.Add(GetChallengeDebuff(item3));
							}
						}
					}
					weeklyChallengeApocalypseConfig.SetlTDebuffss(list);
				}
			}
			if (WeeklyChallengeDeBuffSets == null)
			{
				return;
			}
			for (int j = 0; j < WeeklyChallengeDeBuffSets.Count; j++)
			{
				WeeklyChallengeDeBuffSet weeklyChallengeDeBuffSet = WeeklyChallengeDeBuffSets[j];
				List<DifficultyIncrementalDebuff> list2 = new List<DifficultyIncrementalDebuff>();
				if (weeklyChallengeDeBuffSet.Debuff != null)
				{
					foreach (string item4 in weeklyChallengeDeBuffSet.Debuff)
					{
						if (item4 != null && GetChallengeDebuff(item4) != null)
						{
							list2.Add(GetChallengeDebuff(item4));
						}
					}
				}
				weeklyChallengeDeBuffSet.SetDebuffConfs(list2);
			}
		}

		public WeeklySurvival GetLastEndedWeeklySurvival(long timeUTC)
		{
			WeeklySurvival result = null;
			for (int i = 0; i < WeeklySurvivals.Count; i++)
			{
				if (WeeklySurvivals[i].EndTimeMilliseconds < timeUTC)
				{
					result = WeeklySurvivals[i];
					continue;
				}
				return result;
			}
			return result;
		}

		private void CalculateWeeklySurvivalTimes()
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			for (int i = 0; i < WeeklySurvivals.Count; i++)
			{
				WeeklySurvival weeklySurvival = WeeklySurvivals[i];
				weeklySurvival.SetStartTime(dateTime);
				weeklySurvival.SetEndTime(dateTime);
			}
		}

		public Rewards GetWeeklyChallengePersonalHighScoreRewards(int playerLevel, FixedPoint completionRatio, FixedPoint discardRatio)
		{
			if (PersonalHighScoreRewards != null && PersonalHighScoreRewards.Length != 0 && ConfigData.ChallengePersonalHighScoreRatios != null)
			{
				int num = -1;
				for (int i = 0; i < ConfigData.ChallengePersonalHighScoreRatios.Count && completionRatio >= ConfigData.ChallengePersonalHighScoreRatios[i]; i++)
				{
					if (ConfigData.ChallengePersonalHighScoreRatios[i] > discardRatio)
					{
						num = i;
					}
				}
				if (num > -1)
				{
					int num2 = Math.Max(0, Math.Min(playerLevel - 1, PersonalHighScoreRewards.Length - 1));
					PersonalHighScoreReward personalHighScoreReward = PersonalHighScoreRewards[num2];
					if (personalHighScoreReward != null && personalHighScoreReward.RewardEntries != null && personalHighScoreReward.RewardEntries.Count > 0)
					{
						num = Math.Min(num, personalHighScoreReward.RewardEntries.Count - 1);
						return personalHighScoreReward.RewardEntries[num];
					}
				}
			}
			return null;
		}

		public WeeklyChallengeReward GetWeeklyChallengeReward(WeeklyChallengeReward.ChallengeRewardType rewardType, int control, bool controlExactMatch)
		{
			WeeklyChallengeReward result = null;
			if (orderedWeeklyChallengeRewards != null && orderedWeeklyChallengeRewards.ContainsKey(rewardType))
			{
				List<WeeklyChallengeReward> list = orderedWeeklyChallengeRewards[rewardType];
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						WeeklyChallengeReward weeklyChallengeReward = list[i];
						if (weeklyChallengeReward != null)
						{
							if (controlExactMatch && weeklyChallengeReward.Control == control)
							{
								return weeklyChallengeReward;
							}
							if (control < weeklyChallengeReward.Control)
							{
								return result;
							}
							result = weeklyChallengeReward;
						}
					}
				}
			}
			return result;
		}

		private void CalculateGuildLeaderboardDayZero()
		{
			DateTime guildLeaderboardDayZero = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			ConfigData.SetGuildLeaderboardDayZero(guildLeaderboardDayZero);
		}

		private void CalculateBundleLimitTimes()
		{
			if (BundleStoreDefinitions != null)
			{
				DateTime timeLimits = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				for (int i = 0; i < BundleStoreDefinitions.Count; i++)
				{
					BundleStoreDefinition bundleStoreDefinition = BundleStoreDefinitions[i];
					if (!string.IsNullOrEmpty(bundleStoreDefinition.StartTimestamp) && !string.IsNullOrEmpty(bundleStoreDefinition.EndTimestamp))
					{
						bundleStoreDefinition.SetTimeLimits(timeLimits);
					}
				}
			}
			if (TradefairBundleStoreDefinitions == null)
			{
				return;
			}
			DateTime timeLimits2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			for (int j = 0; j < TradefairBundleStoreDefinitions.Count; j++)
			{
				TradefairBundleStoreDefinition tradefairBundleStoreDefinition = TradefairBundleStoreDefinitions[j];
				if (!string.IsNullOrEmpty(tradefairBundleStoreDefinition.StartTimestamp) && !string.IsNullOrEmpty(tradefairBundleStoreDefinition.EndTimestamp))
				{
					tradefairBundleStoreDefinition.SetTimeLimits(timeLimits2);
				}
			}
		}

		private void CalculateActiveInformationTimes()
		{
			if (ActiveInformationDefinitions != null)
			{
				DateTime epoch = TWDModelManager.Epoch;
				DateTime dateTime = epoch.ToUniversalTime();
				for (int i = 0; i < ActiveInformationDefinitions.Length; i++)
				{
					ActiveInformationDefinitions[i].SetShowTime(dateTime);
					ActiveInformationDefinitions[i].SetEndTime(dateTime);
				}
			}
		}

		private void CustomBundleRwards()
		{
			if (CustomBundleDefinitions == null)
			{
				return;
			}
			DateTime timeLimits = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			for (int i = 0; i < CustomBundleDefinitions.Length; i++)
			{
				CustomBundleDefinition customBundleDefinition = CustomBundleDefinitions[i];
				if (customBundleDefinition != null)
				{
					if (customBundleDefinition.EndTimestamp != null && customBundleDefinition.StartTimestamp != null)
					{
						customBundleDefinition.SetTimeLimits(timeLimits);
					}
					if (customBundleDefinition.RefreshTime > 0)
					{
						customBundleDefinition.SetCustomizedBundleType(CustomizedBundleType.Loop);
					}
					else if (customBundleDefinition.StartTimeMilliseconds > 0 && customBundleDefinition.EndTimeMilliseconds > 0 && customBundleDefinition.RefreshTime <= 0)
					{
						customBundleDefinition.SetCustomizedBundleType(CustomizedBundleType.TimeBundle);
					}
					else
					{
						customBundleDefinition.SetCustomizedBundleType(CustomizedBundleType.OneTime);
					}
					if (!string.IsNullOrEmpty(customBundleDefinition.Rewards))
					{
						customBundleDefinition.RewardEntries = new Rewards(customBundleDefinition.Rewards);
					}
				}
			}
		}

		private void CustomBundleStoragesRwards()
		{
			if (CustomBundleStorages == null)
			{
				return;
			}
			for (int i = 0; i < CustomBundleStorages.Length; i++)
			{
				CustomBundleStorage customBundleStorage = CustomBundleStorages[i];
				if (customBundleStorage != null && customBundleStorage.RewardEntries == null && !string.IsNullOrEmpty(customBundleStorage.Rewards))
				{
					customBundleStorage.RewardEntries = new Rewards(customBundleStorage.Rewards);
				}
			}
		}

		private void SystemOpensByIds()
		{
			if (SystemOpens == null)
			{
				return;
			}
			SystemOpensById = new Dictionary<string, SystemOpen>();
			SystemOpen[] systemOpens = SystemOpens;
			foreach (SystemOpen systemOpen in systemOpens)
			{
				if (SystemOpensById.ContainsKey(systemOpen.SystemID))
				{
					DebugError("SystemOpen with SystemID " + systemOpen.SystemID + " already exists!");
				}
				else
				{
					SystemOpensById.Add(systemOpen.SystemID, systemOpen);
				}
			}
		}

		private void SurvivalManualDefinitionsById()
		{
			if (SurvivalManualDefinitions == null)
			{
				return;
			}
			SurvivalManualDefinitionById = new Dictionary<int, SurvivalManualDefinition>();
			SurvivalManualDefinition[] survivalManualDefinitions = SurvivalManualDefinitions;
			foreach (SurvivalManualDefinition survivalManualDefinition in survivalManualDefinitions)
			{
				if (SurvivalManualDefinitionById.ContainsKey(survivalManualDefinition.ID))
				{
					DebugError("SurvivalManualDefinition with ID " + survivalManualDefinition.ID + " already exists!");
				}
				else
				{
					SurvivalManualDefinitionById.Add(survivalManualDefinition.ID, survivalManualDefinition);
				}
			}
		}

		private void SPTraitsRemodeDefinitionsByType()
		{
			if (SPTraitsRemodeDefinition == null)
			{
				return;
			}
			SPTraitsRemodeDefinitionByTypes = new Dictionary<string, SPTraitsRemoldDefinitions>();
			SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition = SPTraitsRemodeDefinition;
			foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in sPTraitsRemodeDefinition)
			{
				if (!SPTraitsRemodeDefinitionByTypes.ContainsKey(sPTraitsRemoldDefinitions.Type))
				{
					SPTraitsRemodeDefinitionByTypes.Add(sPTraitsRemoldDefinitions.Type, sPTraitsRemoldDefinitions);
				}
			}
		}

		private void SPTraitsRemoldRandomPackageInfo()
		{
			if (SPTraitsRemoldRandomPackages == null)
			{
				return;
			}
			SPTraitsRemoldRandomPackage[] sPTraitsRemoldRandomPackages = SPTraitsRemoldRandomPackages;
			foreach (SPTraitsRemoldRandomPackage sPTraitsRemoldRandomPackage in sPTraitsRemoldRandomPackages)
			{
				if (sPTraitsRemoldRandomPackage.TraitsRemoldInfos == null)
				{
					sPTraitsRemoldRandomPackage.TraitsRemoldInfos = new Dictionary<string, int>();
				}
				if (sPTraitsRemoldRandomPackage.TraitsRemold == null)
				{
					continue;
				}
				foreach (string item in sPTraitsRemoldRandomPackage.TraitsRemold)
				{
					if (string.IsNullOrEmpty(item))
					{
						continue;
					}
					string[] array = item.Split(':');
					if (array.Length != 2)
					{
						continue;
					}
					string text = array[0].Trim();
					if (sPTraitsRemoldRandomPackage.TraitsRemoldInfos.ContainsKey(text))
					{
						DebugError("SPTraitsRemoldRandomPackage with trait ID " + text + " already exists in package " + sPTraitsRemoldRandomPackage.ID);
						continue;
					}
					if (int.TryParse(array[1].Trim(), out var result))
					{
						sPTraitsRemoldRandomPackage.TraitsRemoldInfos.Add(text, result);
					}
					if (SPTraitsRemodeDefinition == null)
					{
						continue;
					}
					sPTraitsRemoldRandomPackage.TraitsRemoldDefinitions = new List<SPTraitsRemoldDefinitions>();
					SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition = SPTraitsRemodeDefinition;
					foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in sPTraitsRemodeDefinition)
					{
						if (sPTraitsRemoldDefinitions.Type == text)
						{
							sPTraitsRemoldRandomPackage.TraitsRemoldDefinitions.Add(sPTraitsRemoldDefinitions);
						}
					}
				}
			}
		}

		public List<SurvivalManualDefinition> GetSurvivalManualOpenTimes(long currentTimeMs)
		{
			List<SurvivalManualDefinition> list = new List<SurvivalManualDefinition>();
			if (SurvivalManualDefinitions != null)
			{
				SurvivalManualDefinition[] survivalManualDefinitions = SurvivalManualDefinitions;
				foreach (SurvivalManualDefinition survivalManualDefinition in survivalManualDefinitions)
				{
					if (currentTimeMs >= survivalManualDefinition.StoryShowTimeMilliseconds)
					{
						list.Add(survivalManualDefinition);
					}
				}
			}
			return list;
		}

		public SurvivalManualDefinition GetSurvivalManualDefinitionById(int Id)
		{
			if (SurvivalManualDefinitionById != null && SurvivalManualDefinitionById.TryGetValue(Id, out var value))
			{
				return value;
			}
			return null;
		}

		public SurvivalManualActorStory GetSurvivalManualActorStory(string storyActorID, string linkActorID, int memoryId)
		{
			if (SurvivalManualActorStorys == null || SurvivalManualActorStorys.Length == 0)
			{
				return null;
			}
			return SurvivalManualActorStorys.FirstOrDefault((SurvivalManualActorStory s) => s.StoryActorID == storyActorID && s.LinkActorID == linkActorID && s.MemoryID == memoryId);
		}

		public SurvivalManualActorStory GetSurvivalManualActorStory(string storyActorID, int memoryId)
		{
			if (SurvivalManualActorStorys == null || SurvivalManualActorStorys.Length == 0)
			{
				return null;
			}
			return SurvivalManualActorStorys.FirstOrDefault((SurvivalManualActorStory s) => s.StoryActorID == storyActorID && s.MemoryID == memoryId);
		}

		public List<SurvivalManualActorStory> GetSurvivalManualActorStories(string storyActorID)
		{
			if (string.IsNullOrEmpty(storyActorID) || SurvivalManualActorStorys == null || SurvivalManualActorStorys.Length == 0)
			{
				return new List<SurvivalManualActorStory>();
			}
			return SurvivalManualActorStorys.Where((SurvivalManualActorStory s) => s != null && s.StoryActorID == storyActorID).ToList();
		}

		public SurvivalManualSkill GetSurvivalManualSkillByLevel(int level)
		{
			if (SurvivalManualSkillByLevels != null && SurvivalManualSkillByLevels.TryGetValue(level, out var value))
			{
				return value;
			}
			return null;
		}

		public int GetSurvivalManualSkillMaxLevel()
		{
			if (SurvivalManualSkills == null || SurvivalManualSkills.Length == 0)
			{
				return 0;
			}
			SurvivalManualSkill survivalManualSkill = null;
			for (int i = 0; i < SurvivalManualSkills.Length; i++)
			{
				SurvivalManualSkill survivalManualSkill2 = SurvivalManualSkills[i];
				if (survivalManualSkill == null || survivalManualSkill2.Level > survivalManualSkill.Level)
				{
					survivalManualSkill = survivalManualSkill2;
				}
			}
			return survivalManualSkill.Level;
		}

		public string GetSurvivalManualActorId(string storyActorID)
		{
			if (string.IsNullOrEmpty(storyActorID) || SurvivalManualActorIdMaps == null || SurvivalManualActorIdMaps.Count == 0)
			{
				return null;
			}
			if (!SurvivalManualActorIdMaps.TryGetValue(storyActorID, out var value))
			{
				return null;
			}
			return value;
		}

		public string GetSurvivalManualStoryId(string linkId)
		{
			if (string.IsNullOrEmpty(linkId) || SurvivalManualStoryIdIdMaps == null || SurvivalManualStoryIdIdMaps.Count == 0)
			{
				return null;
			}
			if (!SurvivalManualStoryIdIdMaps.TryGetValue(linkId, out var value))
			{
				return null;
			}
			return value;
		}

		public List<SurvivalManualActorLevel> GetSurvivalManualActorLevels(int type)
		{
			if (SurvivalManualActorLevels == null || SurvivalManualActorLevels.Length == 0)
			{
				return new List<SurvivalManualActorLevel>();
			}
			if (SurvivalManualActorLevelsByType.TryGetValue(type, out var value))
			{
				return value;
			}
			return new List<SurvivalManualActorLevel>();
		}

		public int GetMaxLevelByType(int type)
		{
			if (SurvivalManualActorLevels == null || SurvivalManualActorLevels.Length == 0)
			{
				return 0;
			}
			SurvivalManualActorLevel survivalManualActorLevel = null;
			if (SurvivalManualActorLevelsByType.TryGetValue(type, out var value))
			{
				foreach (SurvivalManualActorLevel item in value)
				{
					if (survivalManualActorLevel == null || item.Level > survivalManualActorLevel.Level)
					{
						survivalManualActorLevel = item;
					}
				}
			}
			return survivalManualActorLevel?.Level ?? 0;
		}

		public List<SurvivalManualStorySkill> GetSurvivalManualStorSkills(string type)
		{
			if (SurvivalManualStorySkills == null || SurvivalManualStorySkills.Length == 0)
			{
				return new List<SurvivalManualStorySkill>();
			}
			return SurvivalManualStorySkills.Where((SurvivalManualStorySkill x) => x.Type == type).ToList();
		}

		public SurvivalManualStorySkill GetSurvivalManualStorySkillLevel(string type, int level)
		{
			if (SurvivalManualStorySkills == null || SurvivalManualStorySkills.Length == 0)
			{
				return null;
			}
			return SurvivalManualStorySkills.FirstOrDefault((SurvivalManualStorySkill x) => x.Type == type && x.Level == level);
		}

		public SurvivalManualStorySkill GetSurvivalManualStorySkillByID(int ID)
		{
			if (SurvivalManualStorySkills == null || SurvivalManualStorySkills.Length == 0)
			{
				return null;
			}
			return SurvivalManualStorySkills.FirstOrDefault((SurvivalManualStorySkill x) => x.ID == ID);
		}

		public int GetMaxStorySkillLevelByType(string type)
		{
			if (SurvivalManualStorySkills == null || SurvivalManualStorySkills.Length == 0)
			{
				return 0;
			}
			SurvivalManualStorySkill survivalManualStorySkill = null;
			for (int i = 0; i < SurvivalManualStorySkills.Length; i++)
			{
				SurvivalManualStorySkill survivalManualStorySkill2 = SurvivalManualStorySkills[i];
				if (survivalManualStorySkill2.Type == type && (survivalManualStorySkill == null || survivalManualStorySkill2.Level > survivalManualStorySkill.Level))
				{
					survivalManualStorySkill = survivalManualStorySkill2;
				}
			}
			return survivalManualStorySkill.Level;
		}

		public SurvivalManualActorLevel GetSurvivalManualActorLevel(int type, int level)
		{
			if (SurvivalManualActorLevels == null || SurvivalManualActorLevels.Length == 0)
			{
				return null;
			}
			if (SurvivalManualActorLevelsByType.TryGetValue(type, out var value))
			{
				foreach (SurvivalManualActorLevel item in value)
				{
					if (item.Level == level)
					{
						return item;
					}
				}
			}
			return null;
		}

		public SurvivalManualActorLevel GetActorLeveDefinition(int type, int level)
		{
			if (SurvivalManualActorLevels == null || SurvivalManualActorLevels.Length == 0)
			{
				return null;
			}
			if (SurvivalManualActorLevelsByType.TryGetValue(type, out var value))
			{
				foreach (SurvivalManualActorLevel item in value)
				{
					if (item.Level == level)
					{
						return item;
					}
				}
			}
			return null;
		}

		private void SurvivalManualSkillsByLevel()
		{
			if (SurvivalManualSkills == null)
			{
				return;
			}
			SurvivalManualSkillByLevels = new Dictionary<int, SurvivalManualSkill>();
			SurvivalManualSkill[] survivalManualSkills = SurvivalManualSkills;
			foreach (SurvivalManualSkill survivalManualSkill in survivalManualSkills)
			{
				if (SurvivalManualSkillByLevels.ContainsKey(survivalManualSkill.Level))
				{
					DebugError("SurvivalManualSkill with ID " + survivalManualSkill.ID + " already exists!");
				}
				else
				{
					SurvivalManualSkillByLevels.Add(survivalManualSkill.Level, survivalManualSkill);
				}
			}
		}

		private void SurvivalManualAttributesById()
		{
			if (SurvivalManualAttributes == null)
			{
				return;
			}
			SurvivalManualAttributeById = new Dictionary<string, SurvivalManualAttribute>();
			SurvivalManualAttribute[] survivalManualAttributes = SurvivalManualAttributes;
			foreach (SurvivalManualAttribute survivalManualAttribute in survivalManualAttributes)
			{
				if (SurvivalManualAttributeById.ContainsKey(survivalManualAttribute.ID))
				{
					DebugError("SurvivalManualAttribute with ID " + survivalManualAttribute.ID + " already exists!");
				}
				else
				{
					SurvivalManualAttributeById.Add(survivalManualAttribute.ID, survivalManualAttribute);
				}
			}
		}

		public void InitEquipmentScrapSPTokenPackagesByPackageId()
		{
			if (EquipmentScrapSPTokenPackages == null)
			{
				return;
			}
			if (EquipmentScrapSPTokenPackagesByPackageId == null)
			{
				EquipmentScrapSPTokenPackagesByPackageId = new Dictionary<string, List<EquipmentScrapSPTokenPackage>>();
			}
			else
			{
				EquipmentScrapSPTokenPackagesByPackageId.Clear();
			}
			EquipmentScrapSPTokenPackage[] equipmentScrapSPTokenPackages = EquipmentScrapSPTokenPackages;
			foreach (EquipmentScrapSPTokenPackage equipmentScrapSPTokenPackage in equipmentScrapSPTokenPackages)
			{
				if (EquipmentScrapSPTokenPackagesByPackageId.TryGetValue(equipmentScrapSPTokenPackage.PackageID, out var value))
				{
					value.Add(equipmentScrapSPTokenPackage);
					continue;
				}
				EquipmentScrapSPTokenPackagesByPackageId.Add(equipmentScrapSPTokenPackage.PackageID, new List<EquipmentScrapSPTokenPackage> { equipmentScrapSPTokenPackage });
			}
		}

		private void SurvivalManualActorIdMap()
		{
			if (SurvivalManualActorStorys == null)
			{
				return;
			}
			SurvivalManualActorIdMaps = new Dictionary<string, string>();
			SurvivalManualStoryIdIdMaps = new Dictionary<string, string>();
			SurvivalManualActorStory[] survivalManualActorStorys = SurvivalManualActorStorys;
			foreach (SurvivalManualActorStory survivalManualActorStory in survivalManualActorStorys)
			{
				if (!string.IsNullOrEmpty(survivalManualActorStory.StoryActorID) && !SurvivalManualActorIdMaps.ContainsKey(survivalManualActorStory.StoryActorID))
				{
					SurvivalManualActorIdMaps.Add(survivalManualActorStory.StoryActorID, survivalManualActorStory.LinkActorID);
				}
				if (!string.IsNullOrEmpty(survivalManualActorStory.LinkActorID) && !SurvivalManualStoryIdIdMaps.ContainsKey(survivalManualActorStory.LinkActorID))
				{
					SurvivalManualStoryIdIdMaps.Add(survivalManualActorStory.LinkActorID, survivalManualActorStory.StoryActorID);
				}
			}
		}

		private void GoldShopDefinitionRwards()
		{
			if (GoldShopDefinitions == null)
			{
				return;
			}
			DateTime timeLimits = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			for (int i = 0; i < GoldShopDefinitions.Count; i++)
			{
				GoldShopDefinition goldShopDefinition = GoldShopDefinitions[i];
				if (goldShopDefinition != null && goldShopDefinition.Reward != null)
				{
					goldShopDefinition.SetTimeLimits(timeLimits);
					if (!string.IsNullOrEmpty(goldShopDefinition.Reward))
					{
						goldShopDefinition.RewardEntries = new Rewards(goldShopDefinition.Reward);
					}
				}
			}
		}

		private void CalculateSpenderTierMinCreationTime()
		{
			if (SpenderTierDefinitions == null)
			{
				return;
			}
			DateTime timeLimits = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			for (int i = 0; i < SpenderTierDefinitions.Length; i++)
			{
				SpenderTierDefinition spenderTierDefinition = SpenderTierDefinitions[i];
				if (!string.IsNullOrEmpty(spenderTierDefinition.MinCreationTimeStamp) || !string.IsNullOrEmpty(spenderTierDefinition.MaxCreationTimeStamp))
				{
					spenderTierDefinition.SetTimeLimits(timeLimits);
				}
			}
		}

		private void CalculateActorsUnlockTime()
		{
			if (ActorDefinitions == null)
			{
				return;
			}
			DateTime unlockTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			for (int i = 0; i < ActorDefinitions.Count; i++)
			{
				ActorDefinition actorDefinition = ActorDefinitions[i];
				if (!string.IsNullOrEmpty(actorDefinition.UnlockDate))
				{
					actorDefinition.SetUnlockTime(unlockTime);
				}
			}
		}

		private void CalculateOutfitsShopAvailabilityTimes()
		{
			DateTime shopAvailabilityTimes = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			for (int i = 0; i < OutfitDefinitions.Length; i++)
			{
				OutfitDefinition outfitDefinition = OutfitDefinitions[i];
				if (!string.IsNullOrEmpty(outfitDefinition.ShopAvailableStartTimestamp) || !string.IsNullOrEmpty(outfitDefinition.ShopAvailableEndTimestamp))
				{
					outfitDefinition.SetShopAvailabilityTimes(shopAvailabilityTimes);
				}
			}
		}

		private void CalculateCampaignTimes()
		{
			if (CampaignDefinitions != null)
			{
				DateTime startAndEndTimes = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				for (int i = 0; i < CampaignDefinitions.Length; i++)
				{
					CampaignDefinitions[i].SetStartAndEndTimes(startAndEndTimes);
				}
			}
		}

		private void SetupBundleContentRewardEntries()
		{
			if (BundleContentDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < BundleContentDefinitions.Count; i++)
			{
				BundleContentDefinition bundleContentDefinition = BundleContentDefinitions[i];
				if (bundleContentDefinition.RewardEntries == null)
				{
					try
					{
						bundleContentDefinition.RewardEntries = new Rewards(bundleContentDefinition.Rewards, null, 0, EquipmentSource.Bundle);
					}
					catch (Exception)
					{
						bundleContentDefinition.RewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupTradefairBundleContentRewardEntries()
		{
			if (TradefairBundleContentDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < TradefairBundleContentDefinitions.Count; i++)
			{
				TradefairBundleContentDefinition tradefairBundleContentDefinition = TradefairBundleContentDefinitions[i];
				if (tradefairBundleContentDefinition.RewardEntries == null)
				{
					try
					{
						tradefairBundleContentDefinition.RewardEntries = new Rewards(tradefairBundleContentDefinition.Rewards, null, 0, EquipmentSource.Bundle);
					}
					catch (Exception)
					{
						tradefairBundleContentDefinition.RewardEntries = new Rewards();
					}
				}
				if (!string.IsNullOrEmpty(tradefairBundleContentDefinition.BananaBonus) && tradefairBundleContentDefinition.ExtraRewardEntries == null)
				{
					try
					{
						tradefairBundleContentDefinition.ExtraRewardEntries = new Rewards(tradefairBundleContentDefinition.BananaBonus, null, 0, EquipmentSource.Bundle);
					}
					catch (Exception)
					{
						tradefairBundleContentDefinition.ExtraRewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupHillTopStoreRewardEntries()
		{
			if (HillTopStoreDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < HillTopStoreDefinitions.Length; i++)
			{
				HillTopStoreDefinition hillTopStoreDefinition = HillTopStoreDefinitions[i];
				if (hillTopStoreDefinition.RewardEntries == null)
				{
					try
					{
						hillTopStoreDefinition.RewardEntries = new Rewards(hillTopStoreDefinition.Reward);
					}
					catch (Exception)
					{
						hillTopStoreDefinition.RewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupWeeklyChallengeRewardEntries()
		{
			if (WeeklyChallengeRewards != null)
			{
				for (int i = 0; i < WeeklyChallengeRewards.Length; i++)
				{
					WeeklyChallengeReward weeklyChallengeReward = WeeklyChallengeRewards[i];
					if (weeklyChallengeReward != null && weeklyChallengeReward.RewardEntries == null)
					{
						try
						{
							weeklyChallengeReward.RewardEntries = new Rewards(weeklyChallengeReward.Rewards, null, 0, EquipmentSource.MissionLoot);
						}
						catch (Exception)
						{
							weeklyChallengeReward.RewardEntries = new Rewards();
						}
					}
				}
			}
			if (PersonalHighScoreRewards == null)
			{
				return;
			}
			for (int j = 0; j < PersonalHighScoreRewards.Length; j++)
			{
				PersonalHighScoreReward personalHighScoreReward = PersonalHighScoreRewards[j];
				if (personalHighScoreReward == null || personalHighScoreReward.RewardEntries != null)
				{
					continue;
				}
				personalHighScoreReward.RewardEntries = new List<Rewards>();
				if (personalHighScoreReward.CompletionRatio == null)
				{
					continue;
				}
				for (int k = 0; k < personalHighScoreReward.CompletionRatio.Count; k++)
				{
					Rewards rewards = null;
					try
					{
						rewards = new Rewards(personalHighScoreReward.CompletionRatio[k], null, 0, EquipmentSource.MissionLoot);
					}
					catch (Exception)
					{
						rewards = new Rewards();
					}
					if (rewards != null)
					{
						personalHighScoreReward.RewardEntries.Add(rewards);
					}
				}
			}
		}

		private void SetupBadgeRerollCost()
		{
			if (BadgeRerollConfigs != null)
			{
				BadgeRerollConfig[] badgeRerollConfigs = BadgeRerollConfigs;
				foreach (BadgeRerollConfig badgeRerollConfig in badgeRerollConfigs)
				{
					ParseStringArray(badgeRerollConfig.PriceString, ref badgeRerollConfig.Price);
				}
			}
		}

		private void SetupGiftCodeDefinitions()
		{
			giftCodeDefinitions = new Dictionary<string, GiftCodeDefinition>();
			if (GiftCodeDefinitions != null)
			{
				GiftCodeDefinitionRaw[] array = GiftCodeDefinitions;
				foreach (GiftCodeDefinitionRaw giftCodeDefinitionRaw in array)
				{
					giftCodeDefinitions[giftCodeDefinitionRaw.Code] = new GiftCodeDefinition(giftCodeDefinitionRaw);
				}
			}
		}

		private void SetupDeepLinkDefinitions()
		{
			deepLinkDefinitions = new Dictionary<string, DeepLinkDefinition>();
			if (DeepLinkDefinitions != null)
			{
				DeepLinkDefinitionsRaw[] array = DeepLinkDefinitions;
				foreach (DeepLinkDefinitionsRaw deepLinkDefinitionsRaw in array)
				{
					deepLinkDefinitions[deepLinkDefinitionsRaw.Deeplink] = new DeepLinkDefinition(deepLinkDefinitionsRaw);
				}
			}
		}

		private void SetupEndlessModeCombatMultipliers()
		{
			scoreMultiplierDecreaseRates = new Dictionary<int, FixedPoint>();
			foreach (string item in EndlessModeConfig.ScoreMultiplierDecreaseRate.Split(';').ToList())
			{
				if (int.TryParse(item.Split(',')[0], out var result))
				{
					FixedPoint value = (FixedPoint)item.Split(',')[1];
					scoreMultiplierDecreaseRates.Add(result, value);
				}
			}
		}

		private void SetupSupportDefinitions()
		{
			supportDefinitionsMap = new Dictionary<string, SupportDefinition>();
			if (SupportDefinitions == null)
			{
				SupportDefinitions = new SupportDefinitionRaw[0];
			}
			SupportDefinitionRaw[] supportDefinitions = SupportDefinitions;
			foreach (SupportDefinitionRaw supportDefinitionRaw in supportDefinitions)
			{
				if (!(supportDefinitionRaw.Identifier.ToLower(CultureInfo.InvariantCulture) == "disabled"))
				{
					string[] array = supportDefinitionRaw.Parameters.Split(';');
					string[] array2 = supportDefinitionRaw.SupportTalentTree.Split(';');
					if (!supportDefinitionsMap.TryGetValue(supportDefinitionRaw.Identifier, out var value))
					{
						value = new SupportDefinition(supportDefinitionRaw.Identifier, supportDefinitionRaw.Index, array.Length, array2.Length);
						supportDefinitionsMap.Add(value.Identifier, value);
					}
					value.SetLevelData(supportDefinitionRaw.Level, supportDefinitionRaw.TokensToUnlock, supportDefinitionRaw.Cooldown, array, array2, supportDefinitionRaw.SupportTalentSlot, supportDefinitionRaw.ChallengeCooldown, supportDefinitionRaw.DistanceCooldown, supportDefinitionRaw.GVGCooldown, supportDefinitionRaw.Category, supportDefinitionRaw.UpgradeCost, supportDefinitionRaw.InnerCooldown);
				}
			}
			SupportDefinitionIds = supportDefinitionsMap.Keys;
		}

		private void SetupSupportTalentDefinitions()
		{
			supportTalentDefinitionsMap = new Dictionary<int, List<SupportTalentDefinition>>();
			if (SupportTalentDefinitions == null || SupportTalentDefinitions.Length == 0)
			{
				return;
			}
			SupportTalentDefinition[] supportTalentDefinitions = SupportTalentDefinitions;
			foreach (SupportTalentDefinition supportTalentDefinition in supportTalentDefinitions)
			{
				if (!supportTalentDefinitionsMap.TryGetValue(supportTalentDefinition.SupportTalentId, out var value))
				{
					value = new List<SupportTalentDefinition>();
					supportTalentDefinitionsMap.Add(supportTalentDefinition.SupportTalentId, value);
				}
				value.Add(supportTalentDefinition);
			}
		}

		public SupportDefinitionRaw GetSupportDefinitions(int index)
		{
			SupportDefinitionRaw[] supportDefinitions = SupportDefinitions;
			foreach (SupportDefinitionRaw supportDefinitionRaw in supportDefinitions)
			{
				if (supportDefinitionRaw.Index == index)
				{
					return supportDefinitionRaw;
				}
			}
			return null;
		}

		private void SetupFeaturedHero()
		{
			if (FeaturedHeroDefinitions != null)
			{
				DateTime epoch = TWDModelManager.Epoch;
				DateTime dateTime = epoch.ToUniversalTime();
				for (int i = 0; i < FeaturedHeroDefinitions.Length; i++)
				{
					FeaturedHeroDefinitions[i].SetStartTime(dateTime);
					FeaturedHeroDefinitions[i].SetEndTime(dateTime);
				}
			}
		}

		private void SetupCampaignRewardEntries()
		{
			if (CampaignRewardsDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < CampaignRewardsDefinitions.Length; i++)
			{
				CampaignRewardsDefinition campaignRewardsDefinition = CampaignRewardsDefinitions[i];
				if (campaignRewardsDefinition.RewardEntries == null)
				{
					try
					{
						campaignRewardsDefinition.RewardEntries = new Rewards(campaignRewardsDefinition.Reward, null, 0, EquipmentSource.Campaign);
					}
					catch (Exception)
					{
						campaignRewardsDefinition.RewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupDailyLoginRewardEntries()
		{
			if (DailyLoginRewardsDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < DailyLoginRewardsDefinitions.Length; i++)
			{
				DailyLoginRewardsDefinition dailyLoginRewardsDefinition = DailyLoginRewardsDefinitions[i];
				if (dailyLoginRewardsDefinition.RewardEntries == null)
				{
					try
					{
						dailyLoginRewardsDefinition.RewardEntries = new Rewards(dailyLoginRewardsDefinition.Reward, null, 0, EquipmentSource.DailyLoginCampaign);
					}
					catch (Exception)
					{
						dailyLoginRewardsDefinition.RewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupSevenDaysRewardEntries()
		{
			if (SevenDaysRewardDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < SevenDaysRewardDefinitions.Length; i++)
			{
				SevenDaysRewardDefinition sevenDaysRewardDefinition = SevenDaysRewardDefinitions[i];
				if (sevenDaysRewardDefinition.FreeRewardEntries == null)
				{
					try
					{
						sevenDaysRewardDefinition.FreeRewardEntries = new Rewards(sevenDaysRewardDefinition.FreeReward, null, 0, EquipmentSource.SevenDayLogin);
					}
					catch (Exception)
					{
						sevenDaysRewardDefinition.FreeRewardEntries = new Rewards();
					}
				}
				if (sevenDaysRewardDefinition.PremiumRewardEntries == null)
				{
					try
					{
						sevenDaysRewardDefinition.PremiumRewardEntries = new Rewards(sevenDaysRewardDefinition.PremiumReward, null, 0, EquipmentSource.SevenDayLogin);
					}
					catch (Exception)
					{
						sevenDaysRewardDefinition.PremiumRewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupReturnLoginRewardEntries()
		{
			if (ReturnLoginRewardDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < ReturnLoginRewardDefinitions.Length; i++)
			{
				ReturnLoginRewardDefinition returnLoginRewardDefinition = ReturnLoginRewardDefinitions[i];
				if (returnLoginRewardDefinition != null && returnLoginRewardDefinition.RewardEntries == null)
				{
					try
					{
						returnLoginRewardDefinition.RewardEntries = new Rewards(returnLoginRewardDefinition.Reward, null, 0, EquipmentSource.DailyLoginCampaign);
					}
					catch (Exception)
					{
						returnLoginRewardDefinition.RewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupReturnDailyQuestRewardEntries()
		{
			if (ReturnDailyQuestDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < ReturnDailyQuestDefinitions.Length; i++)
			{
				ReturnDailyQuestDefinition returnDailyQuestDefinition = ReturnDailyQuestDefinitions[i];
				if (returnDailyQuestDefinition != null && returnDailyQuestDefinition.RewardEntries == null)
				{
					try
					{
						returnDailyQuestDefinition.RewardEntries = new Rewards(returnDailyQuestDefinition.Reward);
					}
					catch (Exception)
					{
						returnDailyQuestDefinition.RewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupReturnRepeatQuestRewardEntries()
		{
			if (ReturnRepeatQuestDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < ReturnRepeatQuestDefinitions.Length; i++)
			{
				ReturnRepeatQuestDefinition returnRepeatQuestDefinition = ReturnRepeatQuestDefinitions[i];
				if (returnRepeatQuestDefinition != null && returnRepeatQuestDefinition.RewardEntries == null)
				{
					try
					{
						returnRepeatQuestDefinition.RewardEntries = new Rewards(returnRepeatQuestDefinition.Reward);
					}
					catch (Exception)
					{
						returnRepeatQuestDefinition.RewardEntries = new Rewards();
					}
				}
			}
		}

		private void SetupReturnExchangeStoreRewardEntries()
		{
			if (ReturnExchangeStoreDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < ReturnExchangeStoreDefinitions.Length; i++)
			{
				ReturnExchangeStoreDefinition returnExchangeStoreDefinition = ReturnExchangeStoreDefinitions[i];
				if (returnExchangeStoreDefinition == null)
				{
					continue;
				}
				if (returnExchangeStoreDefinition.RewardEntries == null)
				{
					try
					{
						returnExchangeStoreDefinition.RewardEntries = new Rewards(returnExchangeStoreDefinition.Reward);
					}
					catch (Exception)
					{
						returnExchangeStoreDefinition.RewardEntries = new Rewards();
					}
				}
				if (returnExchangeStoreDefinition.CostRewardEntries == null)
				{
					try
					{
						returnExchangeStoreDefinition.CostRewardEntries = new Rewards(returnExchangeStoreDefinition.Cost);
					}
					catch (Exception)
					{
						returnExchangeStoreDefinition.CostRewardEntries = new Rewards();
					}
				}
			}
		}

		private bool IsReturnCouncilLevelMatched(int councilLevel, int min, int max)
		{
			int num = Math.Max(min, 0);
			int num2 = ((max > 0) ? max : int.MaxValue);
			if (councilLevel >= num)
			{
				return councilLevel <= num2;
			}
			return false;
		}

		public void SetupTradeSlotEntries()
		{
			if (TradeSlotDefinitions != null)
			{
				for (int i = 0; i < TradeSlotDefinitions.Length; i++)
				{
					TradeSlotDefinitions[i].Setup();
				}
			}
		}

		public void SetupTradeDefinitions()
		{
			if (TradeDefinitions != null)
			{
				for (int i = 0; i < TradeDefinitions.Length; i++)
				{
					TradeDefinitions[i].Setup();
				}
			}
		}

		public void SetupGuildShopDefinitions()
		{
			if (GuildShopDefinitions != null)
			{
				for (int i = 0; i < GuildShopDefinitions.Length; i++)
				{
					GuildShopDefinitions[i].Setup();
				}
			}
		}

		public void SetupMissionHighlights()
		{
			if (MissionHighlights != null)
			{
				for (int i = 0; i < MissionHighlights.Length; i++)
				{
					MissionHighlights[i].Setup();
				}
			}
		}

		public void SetupEquipmentUsageByClass()
		{
			if (EquipmentDefinitions == null)
			{
				return;
			}
			for (int i = 0; i < EquipmentDefinitions.Length; i++)
			{
				EquipmentDefinition equipmentDefinition = EquipmentDefinitions[i];
				if (!usableEquipmentsByClass.TryGetValue(equipmentDefinition.SurvivorClass, out var value))
				{
					value = new List<EquipmentType>();
					usableEquipmentsByClass.Add(equipmentDefinition.SurvivorClass, value);
				}
				if (!value.Contains(equipmentDefinition.Type))
				{
					value.Add(equipmentDefinition.Type);
				}
			}
		}

		private void SetupOrderedRotationBundleDefinitions()
		{
			if (BundleRotationDefinitions == null || orderedBundleRotationsDefinitions == null || orderedBundleRotationsDefinitions.Count != 0)
			{
				return;
			}
			int num = 0;
			string text = "";
			int num2 = 0;
			for (int i = 0; i < BundleRotationDefinitions.Length; i++)
			{
				BundleRotationSetupDefinition bundleRotationSetupDefinition = BundleRotationDefinitions[i];
				if (bundleRotationSetupDefinition.RotationIdentifier != text)
				{
					text = bundleRotationSetupDefinition.RotationIdentifier;
					num = 0;
				}
				else
				{
					num++;
				}
				BundleRotationDefinition bundleRotationDefinition = null;
				if (!orderedBundleRotationsDefinitions.ContainsKey(bundleRotationSetupDefinition.RotationIdentifier))
				{
					bundleRotationDefinition = new BundleRotationDefinition();
					bundleRotationDefinition.RotationIdentifier = bundleRotationSetupDefinition.RotationIdentifier;
					bundleRotationDefinition.RequiredRotation = bundleRotationSetupDefinition.RequiredRotation;
					bundleRotationDefinition.SpenderTiers = bundleRotationSetupDefinition.SpenderTiers;
					bundleRotationDefinition.BundlesToRandomizeOnSteps = new List<List<string>>();
					bundleRotationDefinition.BundlesToRandomizeIgnoresHighesUnlockClass = new List<bool>();
					bundleRotationDefinition.RotationNumber = num2;
					num2++;
					orderedBundleRotationsDefinitions.Add(bundleRotationSetupDefinition.RotationIdentifier, bundleRotationDefinition);
				}
				else
				{
					bundleRotationDefinition = orderedBundleRotationsDefinitions[bundleRotationSetupDefinition.RotationIdentifier];
				}
				if (bundleRotationDefinition != null)
				{
					if (bundleRotationSetupDefinition.IsRestartingPoint)
					{
						bundleRotationDefinition.RestartingPoint = num;
					}
					bundleRotationDefinition.BundlesToRandomizeOnSteps.Add(bundleRotationSetupDefinition.BundlesToRandomize);
					bundleRotationDefinition.BundlesToRandomizeIgnoresHighesUnlockClass.Add(bundleRotationSetupDefinition.IgnoreHighestUnlockedClassForBundles);
				}
			}
		}

		public void SetupOrderedWeeklyChallengeRewards()
		{
			if (orderedWeeklyChallengeRewards == null || orderedWeeklyChallengeRewards.Count != 0)
			{
				return;
			}
			for (int i = 0; i < WeeklyChallengeRewards.Length; i++)
			{
				WeeklyChallengeReward weeklyChallengeReward = WeeklyChallengeRewards[i];
				if (weeklyChallengeReward != null)
				{
					if (!orderedWeeklyChallengeRewards.ContainsKey(weeklyChallengeReward.RewardType) || orderedWeeklyChallengeRewards[weeklyChallengeReward.RewardType] == null)
					{
						orderedWeeklyChallengeRewards[weeklyChallengeReward.RewardType] = new List<WeeklyChallengeReward>();
					}
					orderedWeeklyChallengeRewards[weeklyChallengeReward.RewardType].Add(weeklyChallengeReward);
				}
			}
			foreach (KeyValuePair<WeeklyChallengeReward.ChallengeRewardType, List<WeeklyChallengeReward>> orderedWeeklyChallengeReward in orderedWeeklyChallengeRewards)
			{
				orderedWeeklyChallengeReward.Value.StableSort((WeeklyChallengeReward a, WeeklyChallengeReward b) => (a != null && b != null) ? Math.Sign(a.Control - b.Control) : 0);
			}
		}

		public void SetupCurrencyMappings()
		{
			heroTokenCurrencyTypes = new List<CurrencyType>();
			classTokenCurrencyTypes = new List<CurrencyType>();
			for (int i = 0; i < ActorDefinitions.Count; i++)
			{
				if (ActorDefinitions[i] != null && ActorDefinitions[i].TraitUpgradeCurrency != CurrencyType.None && ActorDefinitions[i].TraitUpgradeCurrency != CurrencyType.Count && !heroTokenCurrencyTypes.Contains(ActorDefinitions[i].TraitUpgradeCurrency))
				{
					heroTokenCurrencyTypes.Add(ActorDefinitions[i].TraitUpgradeCurrency);
				}
			}
			SurvivorClass[] array = Enum.GetValues(typeof(SurvivorClass)) as SurvivorClass[];
			CurrencyType[] array2 = Enum.GetValues(typeof(CurrencyType)) as CurrencyType[];
			if (array == null || array2 == null)
			{
				return;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(array[j].ToString().ToLower());
			}
			for (int k = 0; k < array2.Length; k++)
			{
				list2.Add(array2[k].ToString().ToLower());
			}
			for (int l = 0; l < array.Length; l++)
			{
				if (array[l] == SurvivorClass.None)
				{
					continue;
				}
				for (int m = 0; m < list2.Count; m++)
				{
					if (list2[m].Contains(list[l]) && !classTokenCurrencyTypes.Contains(array2[m]))
					{
						classTokenCurrencyTypes.Add(array2[m]);
					}
				}
			}
		}

		public bool IsHeroToken(CurrencyType currencyType)
		{
			if (heroTokenCurrencyTypes != null)
			{
				return heroTokenCurrencyTypes.Contains(currencyType);
			}
			return false;
		}

		public bool IsClassToken(CurrencyType currencyType)
		{
			if (classTokenCurrencyTypes != null)
			{
				return classTokenCurrencyTypes.Contains(currencyType);
			}
			return false;
		}

		public bool IsSupportToken(CurrencyType currencyType)
		{
			foreach (KeyValuePair<string, SupportDefinition> item in supportDefinitionsMap)
			{
				if (item.Value.Currency == currencyType)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsToken(CurrencyType currencyType)
		{
			if (!IsClassToken(currencyType) && !IsHeroToken(currencyType))
			{
				return IsSupportToken(currencyType);
			}
			return true;
		}

		public bool IsSpeedUpTokenCurrencyType(CurrencyType type)
		{
			if (type != CurrencyType.BuildingTokenBP && type != CurrencyType.SuperBuildingTokenBP && type != CurrencyType.TrainingTokenBP && type != CurrencyType.SuperTrainingTokenBP && type != CurrencyType.EquipmentTokenBP && type != CurrencyType.SuperEquipmentTokenBP && type != CurrencyType.HealingTokenBP && type != CurrencyType.BuildingToken1min && type != CurrencyType.BuildingToken5min && type != CurrencyType.BuildingToken30min && type != CurrencyType.BuildingToken10min && type != CurrencyType.BuildingToken1h && type != CurrencyType.BuildingToken6h && type != CurrencyType.BuildingToken12h && type != CurrencyType.BuildingToken24h && type != CurrencyType.TrainingToken5min && type != CurrencyType.TrainingToken20min && type != CurrencyType.TrainingToken1h && type != CurrencyType.TrainingToken3h && type != CurrencyType.TrainingToken8h && type != CurrencyType.TrainingToken16h && type != CurrencyType.EquipmentToken1min && type != CurrencyType.EquipmentToken10min && type != CurrencyType.EquipmentToken20min && type != CurrencyType.EquipmentToken1h && type != CurrencyType.EquipmentToken3h && type != CurrencyType.EquipmentToken7h && type != CurrencyType.EquipmentToken14h && type != CurrencyType.HealingToken1min && type != CurrencyType.HealingToken5min && type != CurrencyType.HealingToken10min && type != CurrencyType.HealingToken1h && type != CurrencyType.HealingToken2h)
			{
				return type == CurrencyType.HealingToken4h;
			}
			return true;
		}

		public List<EquipmentType> GetEquipmentsUsableByClass(SurvivorClass survivorClass)
		{
			if (usableEquipmentsByClass != null)
			{
				return usableEquipmentsByClass[survivorClass];
			}
			return new List<EquipmentType>();
		}

		public DropEventDefinition GetDropEvent(DropEventDefinition.DropEventType eventType, DropEventDefinition.DropEventContext context, DropEventDefinition.DropEventTag tag)
		{
			for (int i = 0; i < DropEventDefinitions.Length; i++)
			{
				DropEventDefinition dropEventDefinition = DropEventDefinitions[i];
				if (dropEventDefinition.EventType == eventType && dropEventDefinition.Tag == tag)
				{
					if (eventType != DropEventDefinition.DropEventType.MissionRescue && eventType != DropEventDefinition.DropEventType.MissionScavenge)
					{
						return dropEventDefinition;
					}
					if (dropEventDefinition.DropContext == context)
					{
						return dropEventDefinition;
					}
				}
			}
			return null;
		}

		public DropCurrenciesProbabilitiesDefinition GetDropCurrenciesProbabilities(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, int targetLevel)
		{
			for (int i = 0; i < DropCurrencyProbabilitiesDefinitions.Length; i++)
			{
				DropCurrenciesProbabilitiesDefinition dropCurrenciesProbabilitiesDefinition = DropCurrencyProbabilitiesDefinitions[i];
				if (dropCurrenciesProbabilitiesDefinition.EventType == eventType && dropCurrenciesProbabilitiesDefinition.DropType == dropType && dropCurrenciesProbabilitiesDefinition.Tag == tag && dropCurrenciesProbabilitiesDefinition.ControlLevelMin <= targetLevel && dropCurrenciesProbabilitiesDefinition.ControlLevelMax >= targetLevel)
				{
					return dropCurrenciesProbabilitiesDefinition;
				}
			}
			return null;
		}

		public DropCurrenciesStaticDefinition GetDropCurrencyStaticDefinition(DropEventDefinition.DropEventTag tag, int targetLevel)
		{
			for (int i = 0; i < DropCurrencyStaticDefinitions.Length; i++)
			{
				DropCurrenciesStaticDefinition dropCurrenciesStaticDefinition = DropCurrencyStaticDefinitions[i];
				if (dropCurrenciesStaticDefinition.Tag == tag && targetLevel >= dropCurrenciesStaticDefinition.ControlLevelMin && targetLevel <= dropCurrenciesStaticDefinition.ControlLevelMax)
				{
					return dropCurrenciesStaticDefinition;
				}
			}
			return null;
		}

		public DropCurrenciesAmountsDefinition GetDropCurrencyAmountDefinition(DropType dropType, CurrencyType currency, int targetLevel, DropEventDefinition.DropEventTag tag = DropEventDefinition.DropEventTag.None)
		{
			DropCurrenciesAmountsDefinition dropCurrenciesAmountsDefinition = null;
			for (int i = 0; i < DropCurrenciesAmountsDefinitions.Length; i++)
			{
				DropCurrenciesAmountsDefinition dropCurrenciesAmountsDefinition2 = DropCurrenciesAmountsDefinitions[i];
				if (dropCurrenciesAmountsDefinition2.DropType == dropType && dropCurrenciesAmountsDefinition2.Currency == currency && targetLevel >= dropCurrenciesAmountsDefinition2.ControlLevelMin && targetLevel <= dropCurrenciesAmountsDefinition2.ControlLevelMax)
				{
					if (dropCurrenciesAmountsDefinition == null && dropCurrenciesAmountsDefinition2.Tag == DropEventDefinition.DropEventTag.None)
					{
						dropCurrenciesAmountsDefinition = dropCurrenciesAmountsDefinition2;
					}
					if (dropCurrenciesAmountsDefinition2.Tag == tag)
					{
						return dropCurrenciesAmountsDefinition2;
					}
				}
			}
			return dropCurrenciesAmountsDefinition;
		}

		public DropEquipmentsAndSurvivorsRaritiesDefinition GetDropRarityDefinition(DropType dropType, DropRewardType rewardType, int targetLevel, DropEventDefinition.DropEventTag tag = DropEventDefinition.DropEventTag.None, DropEventDefinition.DropEventContext context = DropEventDefinition.DropEventContext.Normal)
		{
			int num = -1;
			for (int i = 0; i < DropEquipmentsAndSurvivorsRaritiesDefinitions.Length; i++)
			{
				DropEquipmentsAndSurvivorsRaritiesDefinition dropEquipmentsAndSurvivorsRaritiesDefinition = DropEquipmentsAndSurvivorsRaritiesDefinitions[i];
				if (dropEquipmentsAndSurvivorsRaritiesDefinition.DropType == dropType && dropEquipmentsAndSurvivorsRaritiesDefinition.RewardType == rewardType && dropEquipmentsAndSurvivorsRaritiesDefinition.Tag == tag)
				{
					num = Math.Max(num, dropEquipmentsAndSurvivorsRaritiesDefinition.ControlLevelMax);
				}
			}
			for (int j = 0; j < DropEquipmentsAndSurvivorsRaritiesDefinitions.Length; j++)
			{
				DropEquipmentsAndSurvivorsRaritiesDefinition dropEquipmentsAndSurvivorsRaritiesDefinition2 = DropEquipmentsAndSurvivorsRaritiesDefinitions[j];
				if (dropEquipmentsAndSurvivorsRaritiesDefinition2.DropType == dropType && dropEquipmentsAndSurvivorsRaritiesDefinition2.RewardType == rewardType && dropEquipmentsAndSurvivorsRaritiesDefinition2.Tag == tag && dropEquipmentsAndSurvivorsRaritiesDefinition2.DropContext == context && targetLevel >= dropEquipmentsAndSurvivorsRaritiesDefinition2.ControlLevelMin && (targetLevel <= dropEquipmentsAndSurvivorsRaritiesDefinition2.ControlLevelMax || (targetLevel >= num && dropEquipmentsAndSurvivorsRaritiesDefinition2.ControlLevelMax == num)))
				{
					return dropEquipmentsAndSurvivorsRaritiesDefinition2;
				}
			}
			return null;
		}

		public DropEquipmentsAndSurvivorsStartingLevelDefinition GetDropStartingLevelDefinition(DropType dropType, DropRewardType rewardType, int targetLevel, DropEventDefinition.DropEventTag tag = DropEventDefinition.DropEventTag.None)
		{
			int num = -1;
			for (int i = 0; i < DropEquipmentsAndSurvivorsStartingLevelDefinitions.Length; i++)
			{
				DropEquipmentsAndSurvivorsStartingLevelDefinition dropEquipmentsAndSurvivorsStartingLevelDefinition = DropEquipmentsAndSurvivorsStartingLevelDefinitions[i];
				if (dropEquipmentsAndSurvivorsStartingLevelDefinition.DropType == dropType && dropEquipmentsAndSurvivorsStartingLevelDefinition.RewardType == rewardType && dropEquipmentsAndSurvivorsStartingLevelDefinition.Tag == tag)
				{
					num = Math.Max(num, dropEquipmentsAndSurvivorsStartingLevelDefinition.ControlLevelMax);
				}
			}
			for (int j = 0; j < DropEquipmentsAndSurvivorsStartingLevelDefinitions.Length; j++)
			{
				DropEquipmentsAndSurvivorsStartingLevelDefinition dropEquipmentsAndSurvivorsStartingLevelDefinition2 = DropEquipmentsAndSurvivorsStartingLevelDefinitions[j];
				if (dropEquipmentsAndSurvivorsStartingLevelDefinition2.DropType == dropType && dropEquipmentsAndSurvivorsStartingLevelDefinition2.RewardType == rewardType && dropEquipmentsAndSurvivorsStartingLevelDefinition2.Tag == tag && targetLevel >= dropEquipmentsAndSurvivorsStartingLevelDefinition2.ControlLevelMin && (targetLevel <= dropEquipmentsAndSurvivorsStartingLevelDefinition2.ControlLevelMax || (targetLevel >= num && dropEquipmentsAndSurvivorsStartingLevelDefinition2.ControlLevelMax == num)))
				{
					return dropEquipmentsAndSurvivorsStartingLevelDefinition2;
				}
			}
			return null;
		}

		public int[] GetSurvivorStartingLevelsForMission(int missionDifficulty, int survivorRarity)
		{
			int[] array = new int[2] { 1, 1 };
			int targetLevel = (int)((float)missionDifficulty / 6f + 0.5f);
			DropEquipmentsAndSurvivorsStartingLevelDefinition dropStartingLevelDefinition = GetDropStartingLevelDefinition(DropType.Silver, DropRewardType.Survivor, targetLevel);
			if (dropStartingLevelDefinition != null)
			{
				List<int> startingLevelForRarity = dropStartingLevelDefinition.GetStartingLevelForRarity(survivorRarity);
				if (startingLevelForRarity.Count > 1)
				{
					array[0] = startingLevelForRarity[1];
					array[1] = startingLevelForRarity[1];
				}
				else
				{
					array[0] = startingLevelForRarity[0];
					array[1] = startingLevelForRarity[0];
				}
			}
			return array;
		}

		public int GetRoundedValueForCurrency(CurrencyType currency, int inputValue)
		{
			if (currency == CurrencyType.LootKeys)
			{
				for (int i = 0; i < 100; i++)
				{
					UnityEngine.Debug.LogError("Lootkey is beeing used");
				}
			}
			RoundingRules roundingRules = null;
			for (int j = 0; j < CurrencyRoundingRules.Length; j++)
			{
				RoundingRules roundingRules2 = CurrencyRoundingRules[j];
				if (roundingRules == null && roundingRules2.Currency == CurrencyType.None)
				{
					roundingRules = roundingRules2;
				}
				else if (roundingRules2.Currency == currency)
				{
					roundingRules = roundingRules2;
					break;
				}
			}
			if (roundingRules != null)
			{
				int num = 0;
				int num2 = 6;
				double num3 = inputValue;
				for (int k = 1; k <= num2; k++)
				{
					double num4 = Math.Pow(10.0, k);
					if (!(num3 / num4 >= 1.0))
					{
						break;
					}
					num = k;
				}
				if (roundingRules.OutputRoundBase != null && roundingRules.OutputRoundBase.Count > num && roundingRules.OutputRoundBase[num] > 0)
				{
					int num5 = roundingRules.OutputRoundBase[num];
					int num6 = inputValue % num5;
					int num7 = (((double)num6 <= (double)num5 / 2.0) ? (-num6) : (num5 - num6));
					return inputValue + num7;
				}
			}
			return inputValue;
		}

		public Feature GetFeature(string featureId)
		{
			for (int i = 0; i < ((Features != null) ? Features.Length : 0); i++)
			{
				Feature feature = Features[i];
				if (!string.IsNullOrEmpty(featureId) && featureId == feature.ID)
				{
					return feature;
				}
			}
			return new Feature
			{
				Enabled = true
			};
		}

		public DailyQuestChestDefinition GetDailyQuestChest(string chestId)
		{
			if (string.IsNullOrEmpty(chestId))
			{
				return null;
			}
			for (int i = 0; i < DailyQuestChestDefinitions.Length; i++)
			{
				DailyQuestChestDefinition dailyQuestChestDefinition = DailyQuestChestDefinitions[i];
				if (dailyQuestChestDefinition.Id == chestId)
				{
					return dailyQuestChestDefinition;
				}
			}
			return null;
		}

		public CampaignDefinition GetCampaignDefinition(long playerLifeTime)
		{
			for (int i = 0; i < ((CampaignDefinitions != null) ? CampaignDefinitions.Length : 0); i++)
			{
				CampaignDefinition campaignDefinition = CampaignDefinitions[i];
				if (campaignDefinition != null && playerLifeTime >= campaignDefinition.StartTimeMilliseconds && playerLifeTime < campaignDefinition.RewardsAvailableMilliseconds)
				{
					return CampaignDefinitions[i];
				}
			}
			return null;
		}

		public CampaignDefinition GetCampaignDefinition(int id)
		{
			for (int i = 0; i < ((CampaignDefinitions != null) ? CampaignDefinitions.Length : 0); i++)
			{
				CampaignDefinition campaignDefinition = CampaignDefinitions[i];
				if (campaignDefinition != null && campaignDefinition.Id == id)
				{
					return CampaignDefinitions[i];
				}
			}
			return null;
		}

		public List<CampaignRewardsDefinition> GetCampaignRewards(int id)
		{
			List<CampaignRewardsDefinition> list = new List<CampaignRewardsDefinition>();
			for (int i = 0; i < ((CampaignRewardsDefinitions != null) ? CampaignRewardsDefinitions.Length : 0); i++)
			{
				CampaignRewardsDefinition campaignRewardsDefinition = CampaignRewardsDefinitions[i];
				if (campaignRewardsDefinition != null && campaignRewardsDefinition.Id == id)
				{
					list.Add(campaignRewardsDefinition);
				}
			}
			return list;
		}

		public List<CampaignRewardsDefinition> GetCampaignRewardsFrom(int id, int fromControl)
		{
			List<CampaignRewardsDefinition> list = new List<CampaignRewardsDefinition>();
			for (int i = 0; i < ((CampaignRewardsDefinitions != null) ? CampaignRewardsDefinitions.Length : 0); i++)
			{
				CampaignRewardsDefinition campaignRewardsDefinition = CampaignRewardsDefinitions[i];
				if (campaignRewardsDefinition != null && campaignRewardsDefinition.Id == id && campaignRewardsDefinition.Control > fromControl)
				{
					list.Add(campaignRewardsDefinition);
				}
			}
			return list;
		}

		public CampaignRewardsDefinition GetCampaignRewardDefinition(int id, int control)
		{
			for (int i = 0; i < ((CampaignRewardsDefinitions != null) ? CampaignRewardsDefinitions.Length : 0); i++)
			{
				CampaignRewardsDefinition campaignRewardsDefinition = CampaignRewardsDefinitions[i];
				if (campaignRewardsDefinition != null && campaignRewardsDefinition.Id == id && campaignRewardsDefinition.Control == control)
				{
					return campaignRewardsDefinition;
				}
			}
			return null;
		}

		public WalkerRandomizerSwap GetWalkerRandomizerSwap(MapCategory category, int missionLevel)
		{
			if (WalkerRandomizerSwaps == null)
			{
				return null;
			}
			WalkerRandomizerSwap[] walkerRandomizerSwaps = WalkerRandomizerSwaps;
			foreach (WalkerRandomizerSwap walkerRandomizerSwap in walkerRandomizerSwaps)
			{
				if (category.ToString().ToLower() == walkerRandomizerSwap.MissionType.ToLower() && walkerRandomizerSwap.MinLevel <= missionLevel && walkerRandomizerSwap.MaxLevel >= missionLevel)
				{
					return walkerRandomizerSwap;
				}
			}
			return null;
		}

		public WalkerRandomizerWeight GetWalkerRandomizerWeight(MapCategory category, int missionLevel)
		{
			if (WalkerRandomizerWeights == null)
			{
				return null;
			}
			WalkerRandomizerWeight[] walkerRandomizerWeights = WalkerRandomizerWeights;
			foreach (WalkerRandomizerWeight walkerRandomizerWeight in walkerRandomizerWeights)
			{
				if (category.ToString().ToLower() == walkerRandomizerWeight.MissionType.ToLower() && walkerRandomizerWeight.MinLevel <= missionLevel && walkerRandomizerWeight.MaxLevel >= missionLevel)
				{
					return walkerRandomizerWeight;
				}
			}
			return null;
		}

		public WebShopBundleContent GetWebshopBundleContentByBundleContentBundleId(string bundleContentBundleId)
		{
			string text = bundleContentBundleId + "_WB";
			for (int i = 0; i < WebShopBundleContents.Length; i++)
			{
				if (WebShopBundleContents[i].Bundleid == text)
				{
					return WebShopBundleContents[i];
				}
			}
			return null;
		}

		public static T GetTypeEnum<T>(string value)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value.Replace(" ", "").Replace("'", ""), ignoreCase: true);
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public Dictionary<int, List<WalkerType>> GetCurrentSpawnCompositions(in EndlessModeSpawnDefinition currentSpawnDefinition, int round)
		{
			Dictionary<int, List<WalkerType>> dictionary = new Dictionary<int, List<WalkerType>>();
			List<string> roundCompositionIDs = new List<string>();
			List<string> list = ReplaceNewlines(currentSpawnDefinition.SpawnCompositionID).Split(';').ToList();
			if (round >= list.Count - 1)
			{
				roundCompositionIDs = list.LastOrDefault().Split(',').ToList();
			}
			else
			{
				roundCompositionIDs = list[round].Split(',').ToList();
			}
			IEnumerable<EndlessModeSpawnCompositionDefinition> source = from x in EndlessModeSpawnCompositionDefinitions.ToList()
				where roundCompositionIDs.Any((string y) => y == x.ID)
				select x;
			List<string> list2 = source.Select((EndlessModeSpawnCompositionDefinition x) => ReplaceNewlines(x.SpawmComposition)).ToList();
			int spawnPointCount = source.OrderByDescending((EndlessModeSpawnCompositionDefinition x) => x.SpawnPointCount).FirstOrDefault().SpawnPointCount;
			for (int num = 0; num < spawnPointCount; num++)
			{
				List<WalkerType> list3 = new List<WalkerType>();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					List<string> list4 = list2[num2].Split(';').ToList();
					List<string> list5 = list4[Math.Min(num, list4.Count - 1)].Split(',').ToList();
					for (int num3 = 0; num3 < list5.Count; num3++)
					{
						string catalogId = ReplaceNewlines(list5[num3]);
						List<WalkerType> collection = (from x in Array.Find(EndlessModeWaveCatalogs, (EndlessModeWaveCatalog x) => x.ID == catalogId).SpawnComposition.Split(',').ToList()
							select Enum.Parse(typeof(WalkerType), x)).Cast<WalkerType>().ToList();
						list3.AddRange(collection);
					}
				}
				dictionary.Add(num, list3);
			}
			return dictionary;
			static string ReplaceNewlines(string text)
			{
				return Regex.Replace(text, "\\t|\\n|\\r", "");
			}
		}

		public GameEconomyData()
		{
			CampTypes = new List<CampType>();
			ActorDefinitions = new List<ActorDefinition>();
			AbilityDefinitions = new List<AbilityDefinition>();
			MissionData = new List<MissionData>();
			MissionDataById = new Dictionary<string, MissionData>();
			Tutorial = new TutorialDefinition();
			ConfigData = new ConfigData();
		}

		public void Start()
		{
			if (Started)
			{
				DebugTWD.LogWarning("GED started twice!");
				return;
				//throw new Exception("GED started twice!");
			}
			DebugTWD.LogWarning("GED is started!");

			Started = true;
			EquipmentDefinitionsById = new Dictionary<string, EquipmentDefinition>();
			EquipmentDefinition[] equipmentDefinitions = EquipmentDefinitions;
			foreach (EquipmentDefinition equipmentDefinition in equipmentDefinitions)
			{
				if (EquipmentDefinitionsById.ContainsKey(equipmentDefinition.ID))
				{
					DebugError("EquipmentDefinition with ID " + equipmentDefinition.ID + " already exists!");
				}
				else
				{
					EquipmentDefinitionsById.Add(equipmentDefinition.ID, equipmentDefinition);
				}
			}
			BundleContentDefinitionsById = new Dictionary<string, BundleContentDefinition>();
			foreach (BundleContentDefinition bundleContentDefinition in BundleContentDefinitions)
			{
				if (BundleContentDefinitionsById.ContainsKey(bundleContentDefinition.Identifier))
				{
					DebugError("BundleContentDefinition with Identifier " + bundleContentDefinition.Identifier + " already exists!");
				}
				else
				{
					BundleContentDefinitionsById.Add(bundleContentDefinition.Identifier, bundleContentDefinition);
				}
			}
			BundleStoreDefinitionsById = new Dictionary<string, BundleStoreDefinition>();
			foreach (BundleStoreDefinition bundleStoreDefinition in BundleStoreDefinitions)
			{
				if (BundleStoreDefinitionsById.ContainsKey(bundleStoreDefinition.BundleIdentifier))
				{
					DebugError("BundleStoreDefinition with BundleIdentifier " + bundleStoreDefinition.BundleIdentifier + " already exists!");
				}
				else
				{
					BundleStoreDefinitionsById.Add(bundleStoreDefinition.BundleIdentifier, bundleStoreDefinition);
				}
			}
			PhoneCallDefinitionsBySlotNumber = new Dictionary<int, List<PhoneCallDefinition>>();
			for (int j = 0; j < PhoneCallDefinitions.Length; j++)
			{
				PhoneCallDefinition phoneCallDefinition = PhoneCallDefinitions[j];
				if (PhoneCallDefinitionsBySlotNumber.ContainsKey(phoneCallDefinition.SlotNumber))
				{
					PhoneCallDefinitionsBySlotNumber[phoneCallDefinition.SlotNumber].Add(phoneCallDefinition);
					continue;
				}
				PhoneCallDefinitionsBySlotNumber.Add(phoneCallDefinition.SlotNumber, new List<PhoneCallDefinition> { phoneCallDefinition });
			}
			WeeklyChallengesById = new Dictionary<int, WeeklyChallenge>();
			foreach (WeeklyChallenge weeklyChallenge in WeeklyChallenges)
			{
				if (WeeklyChallengesById.ContainsKey(weeklyChallenge.Identifier))
				{
					DebugError("WeeklyChallenge with Identifier " + weeklyChallenge.Identifier + " already exists!");
				}
				else
				{
					WeeklyChallengesById.Add(weeklyChallenge.Identifier, weeklyChallenge);
				}
			}
			ActorLevelsByActorDefinitionID = new Dictionary<string, List<ActorLevelDefinition>>();
			ActorLevelDefinition[] actorLevels = ActorLevels;
			foreach (ActorLevelDefinition actorLevelDefinition in actorLevels)
			{
				if (!ActorLevelsByActorDefinitionID.ContainsKey(actorLevelDefinition.ActorDefinitionID))
				{
					ActorLevelsByActorDefinitionID.Add(actorLevelDefinition.ActorDefinitionID, new List<ActorLevelDefinition>());
				}
				ActorLevelsByActorDefinitionID[actorLevelDefinition.ActorDefinitionID].Add(actorLevelDefinition);
			}
			BuildingUpgradeLevelsByBuildingType = new Dictionary<string, List<BuildingUpgradeLevel>>();
			BuildingUpgradeLevel[] buildingUpgradeLevels = BuildingUpgradeLevels;
			foreach (BuildingUpgradeLevel buildingUpgradeLevel in buildingUpgradeLevels)
			{
				if (!BuildingUpgradeLevelsByBuildingType.ContainsKey(buildingUpgradeLevel.BuildingType))
				{
					BuildingUpgradeLevelsByBuildingType.Add(buildingUpgradeLevel.BuildingType, new List<BuildingUpgradeLevel>());
				}
				BuildingUpgradeLevelsByBuildingType[buildingUpgradeLevel.BuildingType].Add(buildingUpgradeLevel);
			}
			AchievementDefinitionsById = new Dictionary<string, AchievementDefinition>();
			AchievementDefinition[] achievementDefinitions = AchievementDefinitions;
			foreach (AchievementDefinition achievementDefinition in achievementDefinitions)
			{
				if (AchievementDefinitionsById.ContainsKey(achievementDefinition.ID))
				{
					DebugError("AchievementDefinition with ID " + achievementDefinition.ID + " already exists!");
				}
				else
				{
					AchievementDefinitionsById.Add(achievementDefinition.ID, achievementDefinition);
				}
			}
			SurvivorUpgradeDefinitionsBySurvivorClass = new Dictionary<SurvivorClass, List<SurvivorUpgradeDefinition>>();
			SurvivorUpgradeDefinition[] survivorUpgradeDefinitions = SurvivorUpgradeDefinitions;
			foreach (SurvivorUpgradeDefinition survivorUpgradeDefinition in survivorUpgradeDefinitions)
			{
				if (!SurvivorUpgradeDefinitionsBySurvivorClass.ContainsKey(survivorUpgradeDefinition.SurvivorClass))
				{
					SurvivorUpgradeDefinitionsBySurvivorClass.Add(survivorUpgradeDefinition.SurvivorClass, new List<SurvivorUpgradeDefinition>());
				}
				SurvivorUpgradeDefinitionsBySurvivorClass[survivorUpgradeDefinition.SurvivorClass].Add(survivorUpgradeDefinition);
			}
			SurvivalManualActorLevelsByType = new Dictionary<int, List<SurvivalManualActorLevel>>();
			SurvivalManualActorLevel[] survivalManualActorLevels = SurvivalManualActorLevels;
			foreach (SurvivalManualActorLevel survivalManualActorLevel in survivalManualActorLevels)
			{
				if (!SurvivalManualActorLevelsByType.ContainsKey(survivalManualActorLevel.Type))
				{
					SurvivalManualActorLevelsByType.Add(survivalManualActorLevel.Type, new List<SurvivalManualActorLevel>());
				}
				SurvivalManualActorLevelsByType[survivalManualActorLevel.Type].Add(survivalManualActorLevel);
			}
			foreach (List<SurvivalManualActorLevel> value2 in SurvivalManualActorLevelsByType.Values)
			{
				value2.Sort((SurvivalManualActorLevel a, SurvivalManualActorLevel b) => a.Level.CompareTo(b.Level));
			}
			SpenderTierDefinitionsById = new Dictionary<string, SpenderTierDefinition>();
			SpenderTierDefinition[] spenderTierDefinitions = SpenderTierDefinitions;
			foreach (SpenderTierDefinition spenderTierDefinition in spenderTierDefinitions)
			{
				if (SpenderTierDefinitionsById.ContainsKey(spenderTierDefinition.TierIdentifier))
				{
					DebugError("SpenderTierDefinition with TierIdentifier " + spenderTierDefinition.TierIdentifier + " already exists!");
				}
				else
				{
					SpenderTierDefinitionsById.Add(spenderTierDefinition.TierIdentifier, spenderTierDefinition);
				}
			}
			RecalculateAllDropProbabilities();
			CreateFakeChallenges();
			CalculateWeeklyChallengeTimes();
			CalculateWeeklyChallengeClassTeamChallengeTimes();
			CalculateEquipPrizeWheelDefinition();
			CalculateWeeklyChallengeCircleBuff();
			CreateFakeSurvival();
			CalculateWeeklySurvivalTimes();
			CalculateEndlassDebuff();
			SetupGuildWar();
			SetupGuildBattleDifficulty();
			UpdateSurvivalConfigCountsAndMasks(SurvivalMissionConfigs, SurvivalMissionConfig.Type.Survival);
			SetupMapDefinitions();
			SetupGuildBattleMissionPools();
			SetupGuildBattleMissionConfigsPools();
			SetupFakeBattleTiersDifficulty();
			MissionSpawnPointData.CreateHarderDetailMaps(MapDefinitions, ConfigData.HarderEpisodeNumberLevels, ConfigData.HarderEpisodeLevelIncrease, ConfigData.HarderEpisodeGrindLevelIncrease);
			MissionSpawnPointData = new MissionSpawnPointData();
			MissionSpawnPointData.MissionSpawnPointGroups = MapDefinitions;
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in MissionSpawnPointData.MissionSpawnPointGroups)
			{
				MissionSpawnPointData.MissionSpawnPointGroupsById.Add(missionSpawnPointGroup.Id, missionSpawnPointGroup);
			}
			foreach (MissionData missionDatum in MissionData)
			{
				if (!MissionDataById.ContainsKey(missionDatum.Id))
				{
					MissionDataById.Add(missionDatum.Id, missionDatum);
				}
			}
			SetupMissionUnlocking();
			CalculateGuildLeaderboardDayZero();
			CalculateBundleLimitTimes();
			CustomBundleRwards();
			CustomBundleStoragesRwards();
			SystemOpensByIds();
			SurvivalManualDefinitionsById();
			SurvivalManualSkillsByLevel();
			SurvivalManualAttributesById();
			SurvivalManualActorIdMap();
			SPTraitsRemodeDefinitionsByType();
			SPTraitsRemoldRandomPackageInfo();
			InitEquipmentScrapSPTokenPackagesByPackageId();
			GoldShopDefinitionRwards();
			CalculateSpenderTierMinCreationTime();
			CalculateActorsUnlockTime();
			CalculateCampaignTimes();
			SetupBundleContentRewardEntries();
			SetupTradefairBundleContentRewardEntries();
			SetupHillTopStoreRewardEntries();
			SetupWeeklyChallengeRewardEntries();
			SetupCampaignRewardEntries();
			SetupDailyLoginRewardEntries();
			SetupSevenDaysRewardEntries();
			SetupReturnLoginRewardEntries();
			SetupReturnDailyQuestRewardEntries();
			SetupReturnRepeatQuestRewardEntries();
			SetupReturnExchangeStoreRewardEntries();
			CalculateActiveInformationTimes();
			Regex classFilterRegex = GetClassFilterRegex();
			traitDefinitionsByTag = new Dictionary<string, List<TraitDefinition>>(StringComparer.OrdinalIgnoreCase);
			for (int num = 0; num < TraitDefinitions.Length; num++)
			{
				TraitDefinition traitDefinition = TraitDefinitions[num];
				traitDefinition.HasSurvivorClassFilter = HasSurvivorClassFilter(classFilterRegex, traitDefinition.OwnerFilters);
				traitDefinitionIndex.Add(traitDefinition.Identifier, traitDefinition);
				if (traitDefinition.Tags == null)
				{
					continue;
				}
				for (int num2 = 0; num2 < traitDefinition.Tags.Count; num2++)
				{
					string key = traitDefinition.Tags[num2] ?? string.Empty;
					if (!traitDefinitionsByTag.TryGetValue(key, out var value))
					{
						value = new List<TraitDefinition>();
						traitDefinitionsByTag[key] = value;
					}
					value.Add(traitDefinition);
				}
			}
			for (int num3 = 0; num3 < CommandSkillDefinitions.Length; num3++)
			{
				commandSkillDefinitionIndex.Add(CommandSkillDefinitions[num3].ID, CommandSkillDefinitions[num3]);
			}
			SetupTradeSlotEntries();
			SetupTradeDefinitions();
			SetupGuildShopDefinitions();
			SetupMissionHighlights();
			SetupEquipmentUsageByClass();
			SetupOrderedWeeklyChallengeRewards();
			SetupOrderedRotationBundleDefinitions();
			SetupCurrencyMappings();
			SetupFeaturedHero();
			SetupBadgeRerollCost();
			SetupGiftCodeDefinitions();
			SetupDeepLinkDefinitions();
			SetupEndlessModeCombatMultipliers();
			SetupSupportDefinitions();
			SetupRecycleWeaponSPSkillPackages();
			SetupSupportTalentDefinitions();
			CalculateItemTypeDefinitions();
			for (int num4 = 0; num4 < ((GoldShopDefinitions != null) ? GoldShopDefinitions.Count : 0); num4++)
			{
				GoldShopDefinitions[num4].InitializeSubItems();
			}
			for (int num5 = 0; num5 < DailyQuestSetDefinitions.Length; num5++)
			{
				DailyQuestSetDefinitions[num5].LoadSelectionDefinitions();
			}
			for (int num6 = 0; num6 < GuildBattleRewardDefinitions.Length; num6++)
			{
				GuildBattleRewardDefinitions[num6].LoadDefinitions();
			}
			for (int num7 = 0; num7 < CampaignDeeplinks.Length; num7++)
			{
				CampaignDeeplinks[num7].SetupAllowedCurrencies();
			}
			if (ReturnThreeDayDefinitions != null)
			{
				for (int num8 = 0; num8 < ReturnThreeDayDefinitions.Length; num8++)
				{
					ReturnThreeDayDefinitions[num8].CalcReward();
				}
			}
			if (ThreeDayDefinitions != null)
			{
				for (int num9 = 0; num9 < ThreeDayDefinitions.Length; num9++)
				{
					ThreeDayDefinitions[num9].CalcReward();
				}
			}
			if (NewbieStageRewards != null)
			{
				for (int num10 = 0; num10 < NewbieStageRewards.Length; num10++)
				{
					NewbieStageRewards[num10].CalcReward();
				}
			}
			if (RecycleWeaponRewardDefinitions != null)
			{
				for (int num11 = 0; num11 < RecycleWeaponRewardDefinitions.Length; num11++)
				{
					RecycleWeaponRewardDefinitions[num11].CalcReward();
				}
			}
			if (RecycleWeaponDefinitions != null)
			{
				for (int num12 = 0; num12 < RecycleWeaponDefinitions.Length; num12++)
				{
					RecycleWeaponDefinitions[num12].InitRewardPic();
				}
			}
			WalkerRandomizerWeight[] walkerRandomizerWeights = WalkerRandomizerWeights;
			for (int i = 0; i < walkerRandomizerWeights.Length; i++)
			{
				walkerRandomizerWeights[i].Start();
			}
			if (ConfigData != null)
			{
				ConfigData.Start();
			}
		}

		private Regex GetClassFilterRegex()
		{
			string[] array = Enum.GetNames(typeof(SurvivorClass));
			Array.Resize(ref array, array.Length - 1);
			return new Regex("\\b(" + string.Join("|", array.ToArray()) + ")\\b", RegexOptions.IgnoreCase);
		}

		private void DebugError(string s)
		{
		}

		private void SetupGuildBattleMissionConfigsPools()
		{
			GuildBattleMissionConfigsGrouped = new Dictionary<string, List<GuildBattleMissionConfigBase>>();
			GuildBattleMissionConfigsWeights = new Dictionary<string, List<FixedPoint>>();
			for (int i = 0; i < GuildBattleMissionConfigs.Length; i++)
			{
				ParseFromGuildBattleMissionConfig<GuildBattleMissionConfigObjective>("Objectives", GuildBattleMissionConfigs[i], GuildBattleMissionConfigs[i].Objectives);
				ParseFromGuildBattleMissionConfig<GuildBattleMissionConfigEnemies>("Enemies", GuildBattleMissionConfigs[i], GuildBattleMissionConfigs[i].Enemies);
			}
		}

		private void ParseFromGuildBattleMissionConfig<T>(string columnName, GuildBattleMissionConfig row, string rowContent) where T : GuildBattleMissionConfigBase
		{
			string groupKey = GuildBattleMissionConfig.GetGroupKey(columnName, row.ConfigName);
			List<GuildBattleMissionConfigBase> list = null;
			if (!GuildBattleMissionConfigsGrouped.ContainsKey(groupKey))
			{
				list = new List<GuildBattleMissionConfigBase>();
				GuildBattleMissionConfigsGrouped.Add(groupKey, list);
			}
			else
			{
				list = GuildBattleMissionConfigsGrouped[groupKey];
			}
			List<FixedPoint> list2 = null;
			if (!GuildBattleMissionConfigsWeights.ContainsKey(groupKey))
			{
				list2 = new List<FixedPoint>();
				GuildBattleMissionConfigsWeights.Add(groupKey, list2);
			}
			else
			{
				list2 = GuildBattleMissionConfigsWeights[groupKey];
			}
			OutputRowToList<T>(ref list, ref list2, rowContent);
		}

		private static void OutputRowToList<T>(ref List<GuildBattleMissionConfigBase> list, ref List<FixedPoint> weightsList, string rowContent) where T : GuildBattleMissionConfigBase
		{
			if (list == null)
			{
				list = new List<GuildBattleMissionConfigBase>();
			}
			if (string.IsNullOrEmpty(rowContent))
			{
				return;
			}
			string[] array = rowContent.Split(';');
			string text = array[^1];
			int result = 0;
			string wrapperName = "";
			string stringParams = "";
			int[] intParams = new int[2];
			string errorMessage = "";
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			int.TryParse(text, out result);
			GuildBattleMissionConfigBase guildBattleMissionConfigBase = null;
			for (int i = 0; i < array.Length - 1; i++)
			{
				if (GuildBattleMissionConfigBase.TryParseDataFromRow(ref array[i], ref wrapperName, ref stringParams, ref intParams, ref errorMessage))
				{
					if (guildBattleMissionConfigBase == null)
					{
						guildBattleMissionConfigBase = Activator.CreateInstance(typeof(T)) as T;
					}
					guildBattleMissionConfigBase.Parse(ref wrapperName, ref stringParams, ref intParams, ref errorMessage);
				}
			}
			if (guildBattleMissionConfigBase.IsValid())
			{
				list.Add(guildBattleMissionConfigBase);
				weightsList.Add(new FixedPoint(result));
			}
		}

		public static void ParseGuildBattleMissionConfigSingleRow<T>(ref List<GuildBattleMissionConfigBase> list, ref List<FixedPoint> weightsList, string rowContent) where T : GuildBattleMissionConfigBase
		{
			OutputRowToList<T>(ref list, ref weightsList, rowContent);
		}

		private void SetupGuildBattleMissionPools()
		{
			GuildBattleMissionPoolDefinitionGrouped = new Dictionary<string, List<string>>();
			for (int i = 0; i < ((GuildBattleMissionPoolDefinitions != null) ? GuildBattleMissionPoolDefinitions.Length : 0); i++)
			{
				GuildBattleMissionPoolDefinition guildBattleMissionPoolDefinition = GuildBattleMissionPoolDefinitions[i];
				List<string> value = null;
				if (!GuildBattleMissionPoolDefinitionGrouped.TryGetValue(guildBattleMissionPoolDefinition.PoolName, out value))
				{
					value = new List<string>();
				}
				value.Add(guildBattleMissionPoolDefinition.MissionId);
				GuildBattleMissionPoolDefinitionGrouped[guildBattleMissionPoolDefinition.PoolName] = value;
			}
		}

		private void SetupMapDefinitions()
		{
			Dictionary<string, MissionSpawnPointGroup> dictionary = new Dictionary<string, MissionSpawnPointGroup>();
			for (int i = 0; i < MapDefinitions.Count; i++)
			{
				MissionSpawnPointGroup missionSpawnPointGroup = MapDefinitions[i];
				if (missionSpawnPointGroup.MapId == null)
				{
					DebugError("Null name on map definition " + missionSpawnPointGroup.DisplayName);
				}
				if (dictionary.ContainsKey(missionSpawnPointGroup.MapId))
				{
					DebugError("Duplicate map definition, id: " + missionSpawnPointGroup.MapId + ", display name: " + missionSpawnPointGroup.DisplayName);
				}
				dictionary.Add(missionSpawnPointGroup.MapId, missionSpawnPointGroup);
			}
			for (int j = 0; j < MissionDefinitions.Length; j++)
			{
				MissionSpawnPoint missionSpawnPoint = MissionDefinitions[j];
				if (string.IsNullOrEmpty(missionSpawnPoint.MapId))
				{
					continue;
				}
				if (!dictionary.ContainsKey(missionSpawnPoint.MapId))
				{
					DebugError("Map id not found: " + missionSpawnPoint.MapId);
					continue;
				}
				MissionSpawnPointGroup missionSpawnPointGroup2 = dictionary[missionSpawnPoint.MapId];
				missionSpawnPointGroup2.MissionSpawnPoints.Add(missionSpawnPoint);
				if (!missionSpawnPointGroup2.MissionSpawnPointsById.ContainsKey(missionSpawnPoint.MissionId))
				{
					missionSpawnPointGroup2.MissionSpawnPointsById.Add(missionSpawnPoint.MissionId, missionSpawnPoint);
				}
				missionSpawnPoint.OwningGroup = missionSpawnPointGroup2;
			}
		}

		public string GetMapIdByMissionId(string missionId)
		{
			for (int i = 0; i < MissionDefinitions.Length; i++)
			{
				MissionSpawnPoint missionSpawnPoint = MissionDefinitions[i];
				if (missionSpawnPoint != null && missionSpawnPoint.MissionId == missionId)
				{
					return missionSpawnPoint.MapId;
				}
			}
			return null;
		}

		private void SetupMissionUnlocking()
		{
			for (int i = 0; i < MissionSpawnPointData.MissionSpawnPointGroups.Count; i++)
			{
				MissionSpawnPointGroup missionSpawnPointGroup = MissionSpawnPointData.MissionSpawnPointGroups[i];
				for (int j = 0; j < missionSpawnPointGroup.MissionSpawnPoints.Count; j++)
				{
					MissionSpawnPoint missionSpawnPoint = missionSpawnPointGroup.MissionSpawnPoints[j];
					MissionSpawnPoint missionSpawnPoint2 = ((j == missionSpawnPointGroup.MissionSpawnPoints.Count - 1) ? null : missionSpawnPointGroup.MissionSpawnPoints[j + 1]);
					if (missionSpawnPoint2 != null && missionSpawnPoint2.MapId == missionSpawnPoint.MapId && (missionSpawnPointGroup.Category == MapCategory.Story || missionSpawnPointGroup.Category == MapCategory.Season))
					{
						missionSpawnPoint.SpawnPointsToUnlock = new List<MissionSpawnPoint>();
						missionSpawnPoint.SpawnPointsToUnlock.Add(missionSpawnPoint2);
					}
				}
			}
		}

		public List<MissionSpawnPointGroup> GetAvailableMapsByCategory(MapCategory category, string subcategory = null)
		{
			List<MissionSpawnPointGroup> list = new List<MissionSpawnPointGroup>();
			for (int i = 0; i < MapDefinitions.Count; i++)
			{
				MissionSpawnPointGroup missionSpawnPointGroup = MapDefinitions[i];
				if (missionSpawnPointGroup.Category == category && (subcategory == null || missionSpawnPointGroup.Subcategory == subcategory) && (missionSpawnPointGroup.EpisodeDifficultyLevel == 1 || missionSpawnPointGroup.EpisodeDifficultyLevel == 0) && !ConfigData.DisabledEpisodes.Contains(missionSpawnPointGroup.DisplayName))
				{
					list.Add(missionSpawnPointGroup);
				}
			}
			return list;
		}

		public int GetEpisodeAmount(MapCategory category, string subcategory = null)
		{
			return GetAvailableMapsByCategory(category, subcategory).Count;
		}

		public MissionSpawnPointGroup GetMapDefinitionById(string mapId)
		{
			for (int i = 0; i < ((MapDefinitions != null) ? MapDefinitions.Count : 0); i++)
			{
				MissionSpawnPointGroup missionSpawnPointGroup = MapDefinitions[i];
				if (missionSpawnPointGroup.MapId == mapId)
				{
					return missionSpawnPointGroup;
				}
			}
			return null;
		}

		public MissionSpawnPointGroup GetMapDefinitionById(int id)
		{
			for (int i = 0; i < ((MapDefinitions != null) ? MapDefinitions.Count : 0); i++)
			{
				MissionSpawnPointGroup missionSpawnPointGroup = MapDefinitions[i];
				if (missionSpawnPointGroup.Id == id)
				{
					return missionSpawnPointGroup;
				}
			}
			return null;
		}

		public SeasonDefinition GetHighlightedSeasonDefinition()
		{
			for (int i = 0; i < ((SeasonDefinitions != null) ? SeasonDefinitions.Length : 0); i++)
			{
				if (SeasonDefinitions[i].Highlighted)
				{
					return SeasonDefinitions[i];
				}
			}
			if (SeasonDefinitions != null && SeasonDefinitions.Length != 0)
			{
				return SeasonDefinitions[SeasonDefinitions.Length - 1];
			}
			return null;
		}

		public SeasonDefinition GetSeasonDefinition(string id)
		{
			for (int i = 0; i < ((SeasonDefinitions != null) ? SeasonDefinitions.Length : 0); i++)
			{
				if (SeasonDefinitions[i].Id == id)
				{
					return SeasonDefinitions[i];
				}
			}
			return null;
		}

		public SeasonDefinition GetSeasonDefinitionForMap(MissionSpawnPointGroup map)
		{
			if (map == null || map.Category != MapCategory.Season)
			{
				return null;
			}
			return GetSeasonDefinition(map.Subcategory);
		}

		public List<MissionSpawnPointGroup> GetAllMapsInSeason(SeasonDefinition season)
		{
			List<MissionSpawnPointGroup> list = new List<MissionSpawnPointGroup>();
			for (int i = 0; i < MapDefinitions.Count; i++)
			{
				if (MapDefinitions[i].Category == MapCategory.Season && MapDefinitions[i].Subcategory == season.Id)
				{
					list.Add(MapDefinitions[i]);
				}
			}
			return list;
		}

		public long GetFirstSeasonMissionUnlockTime(SeasonDefinition season)
		{
			long num = -1L;
			if (season != null)
			{
				List<MissionSpawnPointGroup> allMapsInSeason = GetAllMapsInSeason(season);
				for (int i = 0; i < allMapsInSeason.Count; i++)
				{
					if (allMapsInSeason[i].UnlockTimeMilliseconds < num || num == -1)
					{
						num = allMapsInSeason[i].UnlockTimeMilliseconds;
					}
				}
			}
			return num;
		}

		public EquipmentCategory GetCategoryOfEquipmentType(EquipmentType equipmentType)
		{
			return equipmentTypesCategoryMap[equipmentType];
		}

		public int TimeToDiamonds(long milliSeconds)
		{
			int value = (int)(milliSeconds / 1000);
			return ConvertToDiamonds(value, ConfigData.TimeToDiamondsConversion);
		}

		public bool CanConvertToDiamonds(CurrencyType type)
		{
			if (type != CurrencyType.Supplies && type != CurrencyType.SurvivalPoints && type != CurrencyType.Outpost && type != CurrencyType.Phone && type != CurrencyType.Inhabitants && type != CurrencyType.ReplayToken && type != CurrencyType.EndlessPassToken)
			{
				return type == CurrencyType.EndlessPassExpertToken;
			}
			return true;
		}

		public int CurrencyToDiamonds(CurrencyType currencyType, int amount, PlayerModel playerModel = null)
		{
			switch (currencyType)
			{
			case CurrencyType.Supplies:
				return ConvertToDiamonds(amount, ConfigData.SuppliesToDiamondsConversion);
			case CurrencyType.SurvivalPoints:
				return ConvertToDiamonds(amount, ConfigData.SPToDiamondsConversion);
			case CurrencyType.Outpost:
				return ConvertToDiamonds(amount, ConfigData.OutpostToDiamondsConversion);
			case CurrencyType.Phone:
				return ConvertToDiamonds(amount, ConfigData.PhoneToDiamondsConversion);
			case CurrencyType.ReplayToken:
			{
				if (playerModel == null)
				{
					return 100;
				}
				if (playerModel.Blackboard.IsToggleOn("BuyJustEnoughGasForMission"))
				{
					return ConfigData.ReplayTokensRechargePrice * amount;
				}
				CurrencyModel currency = playerModel.GetCurrency(CurrencyType.ReplayToken);
				int num = currency.Max - currency.Value;
				return ConfigData.ReplayTokensRechargePrice * num;
			}
			case CurrencyType.EndlessPassToken:
				return EndlessModeConfig.MissionBaseCost * EndlessModeConfig.MissionTicketCost;
			case CurrencyType.EndlessPassExpertToken:
				return EndlessModeConfig.MissionBaseCost * EndlessModeConfig.MissionTicketCostExpert;
			case CurrencyType.Inhabitants:
				return 0;
			case CurrencyType.BuildingTokenBP:
				return SpeedupTokenDefinitions.BuildingTokenGoldExchange * amount;
			case CurrencyType.SuperBuildingTokenBP:
				return SpeedupTokenDefinitions.SuperBuildingTokenGoldExchange * amount;
			case CurrencyType.TrainingTokenBP:
				return SpeedupTokenDefinitions.TrainingTokenGoldExchange * amount;
			case CurrencyType.SuperTrainingTokenBP:
				return SpeedupTokenDefinitions.SuperTrainingTokenGoldExchange * amount;
			case CurrencyType.EquipmentTokenBP:
				return SpeedupTokenDefinitions.WorkshopTokenGoldExchange * amount;
			case CurrencyType.SuperEquipmentTokenBP:
				return SpeedupTokenDefinitions.SuperWorkshopTokenGoldExchange * amount;
			case CurrencyType.HealingTokenBP:
				return SpeedupTokenDefinitions.HealingTokenGoldExchange * amount;
			case CurrencyType.BuildingToken1min:
				return SpeedupTokenDefinitions.BuildingToken1minGoldExchange * amount;
			case CurrencyType.BuildingToken5min:
				return SpeedupTokenDefinitions.BuildingToken5minGoldExchange * amount;
			case CurrencyType.BuildingToken10min:
				return SpeedupTokenDefinitions.BuildingToken10minGoldExchange * amount;
			case CurrencyType.BuildingToken30min:
				return SpeedupTokenDefinitions.BuildingToken30minGoldExchange * amount;
			case CurrencyType.BuildingToken1h:
				return SpeedupTokenDefinitions.BuildingToken1hGoldExchange * amount;
			case CurrencyType.BuildingToken6h:
				return SpeedupTokenDefinitions.BuildingToken6hGoldExchange * amount;
			case CurrencyType.BuildingToken12h:
				return SpeedupTokenDefinitions.BuildingToken12hGoldExchange * amount;
			case CurrencyType.BuildingToken24h:
				return SpeedupTokenDefinitions.BuildingToken24hGoldExchange * amount;
			case CurrencyType.TrainingToken5min:
				return SpeedupTokenDefinitions.TrainingToken5minGoldExchange * amount;
			case CurrencyType.TrainingToken20min:
				return SpeedupTokenDefinitions.TrainingToken20minGoldExchange * amount;
			case CurrencyType.TrainingToken1h:
				return SpeedupTokenDefinitions.TrainingToken1hGoldExchange * amount;
			case CurrencyType.TrainingToken3h:
				return SpeedupTokenDefinitions.TrainingToken3hGoldExchange * amount;
			case CurrencyType.TrainingToken8h:
				return SpeedupTokenDefinitions.TrainingToken8hGoldExchange * amount;
			case CurrencyType.TrainingToken16h:
				return SpeedupTokenDefinitions.TrainingToken16hGoldExchange * amount;
			case CurrencyType.EquipmentToken1min:
				return SpeedupTokenDefinitions.EquipmentToken1minGoldExchange * amount;
			case CurrencyType.EquipmentToken10min:
				return SpeedupTokenDefinitions.EquipmentToken10minGoldExchange * amount;
			case CurrencyType.EquipmentToken20min:
				return SpeedupTokenDefinitions.EquipmentToken20minGoldExchange * amount;
			case CurrencyType.EquipmentToken1h:
				return SpeedupTokenDefinitions.EquipmentToken1hGoldExchange * amount;
			case CurrencyType.EquipmentToken3h:
				return SpeedupTokenDefinitions.EquipmentToken3hGoldExchange * amount;
			case CurrencyType.EquipmentToken7h:
				return SpeedupTokenDefinitions.EquipmentToken7hGoldExchange * amount;
			case CurrencyType.EquipmentToken14h:
				return SpeedupTokenDefinitions.EquipmentToken14hGoldExchange * amount;
			case CurrencyType.HealingToken1min:
				return SpeedupTokenDefinitions.HealingToken1minGoldExchange * amount;
			case CurrencyType.HealingToken5min:
				return SpeedupTokenDefinitions.HealingToken5minGoldExchange * amount;
			case CurrencyType.HealingToken10min:
				return SpeedupTokenDefinitions.HealingToken10minGoldExchange * amount;
			case CurrencyType.HealingToken1h:
				return SpeedupTokenDefinitions.HealingToken1hGoldExchange * amount;
			case CurrencyType.HealingToken2h:
				return SpeedupTokenDefinitions.HealingToken2hGoldExchange * amount;
			case CurrencyType.HealingToken4h:
				return SpeedupTokenDefinitions.HealingToken4hGoldExchange * amount;
			case CurrencyType.EquipTraitsRemodelToken:
				return ConfigData.EquipTraitsRemodelToken[1] * amount;
			default:
				return int.MaxValue;
			}
		}

		private int ConvertToDiamonds(int value, List<int> conversionTable)
		{
			if (conversionTable == null)
			{
				return 100;
			}
			if (conversionTable.Count == 0 || (conversionTable.Count & 1) != 0)
			{
				return -1;
			}
			if (value == 0)
			{
				return 0;
			}
			if (value < conversionTable[0])
			{
				return conversionTable[1];
			}
			for (int i = 0; i < conversionTable.Count - 2; i += 2)
			{
				if (value < conversionTable[i + 2])
				{
					int num = conversionTable[i];
					int num2 = conversionTable[i + 1];
					int num3 = conversionTable[i + 2];
					int num4 = conversionTable[i + 3];
					return (int)UtilsMath.Map(value, num, num3, num2, num4);
				}
			}
			int num5 = conversionTable[conversionTable.Count - 2];
			int num6 = conversionTable[conversionTable.Count - 1];
			if (num5 == 0)
			{
				return -1;
			}
			return (int)((float)value / (float)num5 * (float)num6);
		}

		public int ScaleToGrid(int value)
		{
			return (int)Math.Ceiling((double)value / (double)ConfigData.GridScale);
		}

		public MissionGenerationData GetMissionGenerationData(int Level)
		{
			if (Level <= 0)
			{
				return MissionGenerationData[0];
			}
			for (int i = 0; i < MissionGenerationData.Length; i++)
			{
				if (MissionGenerationData[i].MissionLevel == Level)
				{
					return MissionGenerationData[i];
				}
			}
			return MissionGenerationData[MissionGenerationData.Length - 1];
		}

		public MissionGenerationData GetMissionGenerationDataForMaxWalkerLevel(int maxWalkerLevel)
		{
			if (maxWalkerLevel <= 0)
			{
				return MissionGenerationData[0];
			}
			for (int i = 0; i < MissionGenerationData.Length; i++)
			{
				if (MissionGenerationData[i].MaxWalkerLevel == maxWalkerLevel)
				{
					return MissionGenerationData[i];
				}
			}
			return MissionGenerationData[MissionGenerationData.Length - 1];
		}

		public MissionFlavorData GetMissionFlavorData(string name)
		{
			if (name == null)
			{
				return MissionFlavorData[0];
			}
			string text = name.ToLower();
			MissionFlavorData[] missionFlavorData = MissionFlavorData;
			foreach (MissionFlavorData missionFlavorData2 in missionFlavorData)
			{
				if (missionFlavorData2.Name.ToLower() == text)
				{
					return missionFlavorData2;
				}
			}
			return MissionFlavorData[0];
		}

		public MissionScoringConfig GetMissionScoringConfig(string categoryId)
		{
			MissionScoringConfig[] missionScoringConfig = MissionScoringConfig;
			foreach (MissionScoringConfig missionScoringConfig2 in missionScoringConfig)
			{
				if (missionScoringConfig2.Id == categoryId)
				{
					return missionScoringConfig2;
				}
			}
			return null;
		}

		public int GetScoreForCategory(string categoryId, int value)
		{
			MissionScoringConfig missionScoringConfig = GetMissionScoringConfig(categoryId);
			if (missionScoringConfig != null)
			{
				switch (missionScoringConfig.Function)
				{
				case ScoringFunction.Linear:
					return Math.Min(missionScoringConfig.MaxValue, value) * missionScoringConfig.ScoreScale;
				case ScoringFunction.InverseLinear:
					return (missionScoringConfig.MaxValue - Math.Min(missionScoringConfig.MaxValue, value)) * missionScoringConfig.ScoreScale;
				case ScoringFunction.Exponential:
				{
					float num3 = (float)Math.Min(missionScoringConfig.MaxValue, value) / (float)missionScoringConfig.MaxValue;
					return (int)(num3 * num3 * (float)missionScoringConfig.MaxValue) * missionScoringConfig.ScoreScale;
				}
				case ScoringFunction.InverseExponential:
				{
					float num = (float)Math.Min(missionScoringConfig.MaxValue, value) / (float)missionScoringConfig.MaxValue;
					int num2 = (int)(num * num * (float)missionScoringConfig.MaxValue);
					return (missionScoringConfig.MaxValue - num2) * missionScoringConfig.ScoreScale;
				}
				}
			}
			return 0;
		}

		public MissionRewards GetMissionRewardsData(string missionId)
		{
			if (missionId == null || MissionRewards == null)
			{
				return null;
			}
			string text = missionId.ToLower();
			for (int i = 0; i < MissionRewards.Length; i++)
			{
				if (MissionRewards[i].MissionId.ToLower() == text)
				{
					return MissionRewards[i];
				}
			}
			return null;
		}

		public MissionSpawnPointGroup GetOutpostTutorialSpawnPointGroup()
		{
			return MissionSpawnPointData.GetSpawnPointGroup(ConfigData.OutpostTutorialSpawnPointGroupId);
		}

		public Rewards GetAdsCompensationRewards()
		{
			if (!string.IsNullOrEmpty(ConfigData.NoAdsCompensationReward))
			{
				return new Rewards(ConfigData.NoAdsCompensationReward);
			}
			return null;
		}

		public int GetRarityLevel(ModelRandom random, int rarityPreference)
		{
			int[] array = new int[5] { 0, 1, 2, 3, 4 };
			if (rarityPreference == -1 || RarityWeightData == null || rarityPreference >= RarityWeightData.Length)
			{
				return 0;
			}
			RarityWeightData rarityWeightData = RarityWeightData[rarityPreference];
			FixedPoint[] weights = new FixedPoint[5] { rarityWeightData.Common, rarityWeightData.Uncommon, rarityWeightData.Rare, rarityWeightData.Epic, rarityWeightData.Legendary };
			int num = random.WeightedRandom(weights);
			return array[num];
		}

		public List<int> GetPossibleRarities(int rarityPreference)
		{
			List<int> list = new List<int>();
			int[] array = new int[5] { 0, 1, 2, 3, 4 };
			if (rarityPreference == -1 || RarityWeightData == null || rarityPreference >= RarityWeightData.Length)
			{
				list.Add(0);
				return list;
			}
			for (int i = 0; i < RarityWeightData[rarityPreference].GetWeightCount(); i++)
			{
				if (RarityWeightData[rarityPreference].GetWeight(i) > 0f)
				{
					list.Add(array[i]);
				}
			}
			return list;
		}

		public HeroTokenDropDefinition GetHeroTokenDropDefinition(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, int targetLevel)
		{
			HeroTokenDropDefinition result = null;
			for (int i = 0; i < ((HeroTokenDropDefinitions != null) ? HeroTokenDropDefinitions.Length : 0); i++)
			{
				HeroTokenDropDefinition heroTokenDropDefinition = HeroTokenDropDefinitions[i];
				if (heroTokenDropDefinition.EventType == eventType && heroTokenDropDefinition.DropType == dropType && heroTokenDropDefinition.Tag == tag && heroTokenDropDefinition.ControlLevelMin <= targetLevel && heroTokenDropDefinition.ControlLevelMax >= targetLevel)
				{
					result = heroTokenDropDefinition;
					break;
				}
			}
			return result;
		}

		public SurvivorToken GetHeroTokenForGatcha(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, int targetLevel, CurrencyType forceTokenType, ModelRandom random, int forceRarityLevel = -1, PhoneCallDefinition phoneCallDefinition = null)
		{
			HeroTokenDropDefinition heroTokenDropDefinition = GetHeroTokenDropDefinition(eventType, dropType, tag, targetLevel);
			if (heroTokenDropDefinition != null)
			{
				for (int i = 0; i < ((HeroTokenDropDistributionDefinitions != null) ? HeroTokenDropDistributionDefinitions.Length : 0); i++)
				{
					HeroTokenDropDistributionDefinition heroTokenDropDistributionDefinition = HeroTokenDropDistributionDefinitions[i];
					if (!(heroTokenDropDistributionDefinition.BucketId == heroTokenDropDefinition.BucketId) || !(heroTokenDropDistributionDefinition.BucketId != "HeroGrouping"))
					{
						continue;
					}
					DropEquipmentsAndSurvivorsRaritiesDefinition dropRarityDefinition = GetDropRarityDefinition(dropType, DropRewardType.HeroToken, targetLevel, tag);
					int num = ((forceRarityLevel != -1) ? forceRarityLevel : dropRarityDefinition.GetDropRarityForRandomNumber(random.Next() * 100f));
					SurvivorToken survivorToken = new SurvivorToken();
					if (forceTokenType != CurrencyType.None)
					{
						survivorToken.Type = forceTokenType;
					}
					else
					{
						survivorToken.Type = heroTokenDropDistributionDefinition.GetTokenTypeForRandomNumber(random.Next() * 100f);
					}
					survivorToken.Amount = GetHeroTokenAmountForRarity(survivorToken.Type, num);
					if (OfflineManager.IsLoadDataManager)
					{
						var hashList = new List<CurrencyType>() { CurrencyType.PerlieToken, CurrencyType.GauntletAaronToken, CurrencyType.SimonToken, CurrencyType.ProtectorDarylToken, CurrencyType.LydiaToken };
						if (survivorToken.Amount == 0 && hashList.Contains(survivorToken.Type))
						{
							DebugTWD.Log("SurvivorToken is 0 for " + survivorToken.Type, DebugType.Call);
							survivorToken.Amount = GetHeroTokenAmountForRarity(CurrencyType.TaraToken, num);
						}
					}
					survivorToken.AmountRarityLevel = num;
					if (phoneCallDefinition != null && phoneCallDefinition.HeroTokensDropNumber != null)
					{
						bool parseError;
						List<int> hreoKensDropNumberValues = phoneCallDefinition.getHreoKensDropNumberValues(out parseError);
						if (parseError)
						{
							DebugError("$ Unable to parse HreoKensDropNumber as an integer.");
						}
						bool parseError2;
						CurrencyType[] source = phoneCallDefinition.ParseCurrencyTypeValues(out parseError2);
						if (parseError2)
						{
							DebugError(":CurrencyType error " + phoneCallDefinition.SlotNumber);
						}
						if (!parseError2 && source.Contains(survivorToken.Type))
						{
							switch (num)
							{
							case 2:
								survivorToken.Amount = hreoKensDropNumberValues[0];
								break;
							case 3:
								survivorToken.Amount = hreoKensDropNumberValues[1];
								break;
							case 4:
							case 5:
							case 6:
							case 7:
							case 8:
							case 9:
								survivorToken.Amount = hreoKensDropNumberValues[2];
								break;
							}
						}
					}
					if (survivorToken.Type == CurrencyType.None && heroTokenDropDistributionDefinition.GlennToken > 0L)
					{
						survivorToken.Type = CurrencyType.GlennToken;
						survivorToken.Amount = 8;
						survivorToken.AmountRarityLevel = 0;
					}
					return survivorToken;
				}
			}
			return null;
		}

		public List<CurrencyType> GetPossibleHeroTokensByRadioTentLevel(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, int targetLevel)
		{
			HeroTokenDropDefinition heroTokenDropDefinition = GetHeroTokenDropDefinition(eventType, dropType, tag, targetLevel);
			List<CurrencyType> list = new List<CurrencyType>();
			if (heroTokenDropDefinition != null)
			{
				HeroTokenDropDistributionDefinition heroTokenDropDistributionDefinition = null;
				for (int i = 0; i < ((HeroTokenDropDistributionDefinitions != null) ? HeroTokenDropDistributionDefinitions.Length : 0); i++)
				{
					HeroTokenDropDistributionDefinition heroTokenDropDistributionDefinition2 = HeroTokenDropDistributionDefinitions[i];
					if (heroTokenDropDistributionDefinition2.BucketId == heroTokenDropDefinition.BucketId && heroTokenDropDistributionDefinition2.BucketId != "HeroGrouping")
					{
						heroTokenDropDistributionDefinition = heroTokenDropDistributionDefinition2;
						break;
					}
				}
				if (heroTokenDropDistributionDefinition != null)
				{
					Type typeFromHandle = typeof(HeroTokenDropDistributionDefinition);
					for (int j = 0; j < (int)CurrencyType.Count; j++)
					{
						CurrencyType item = (CurrencyType)j;
						FieldInfo field = typeFromHandle.GetField(item.ToString());
						if (field != null && (FixedPoint)field.GetValue(heroTokenDropDistributionDefinition) > 0L)
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		public SurvivorToken GetHeroTokenForChallenge(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, DropCurrenciesProbabilitiesDefinition.DropCurrency currencyType, int targetLevel, ModelRandom random)
		{
			HeroTokenDropDefinition heroTokenDropDefinition = null;
			for (int i = 0; i < ((HeroTokenDropDefinitions != null) ? HeroTokenDropDefinitions.Length : 0); i++)
			{
				HeroTokenDropDefinition heroTokenDropDefinition2 = HeroTokenDropDefinitions[i];
				if (heroTokenDropDefinition2.EventType == eventType && heroTokenDropDefinition2.DropType == dropType && heroTokenDropDefinition2.Tag == tag && heroTokenDropDefinition2.ControlLevelMin <= targetLevel && heroTokenDropDefinition2.ControlLevelMax >= targetLevel)
				{
					heroTokenDropDefinition = heroTokenDropDefinition2;
					break;
				}
			}
			if (heroTokenDropDefinition != null)
			{
				for (int j = 0; j < ((HeroTokenDropDistributionDefinitions != null) ? HeroTokenDropDistributionDefinitions.Length : 0); j++)
				{
					HeroTokenDropDistributionDefinition heroTokenDropDistributionDefinition = HeroTokenDropDistributionDefinitions[j];
					if (heroTokenDropDistributionDefinition.BucketId == heroTokenDropDefinition.BucketId && heroTokenDropDistributionDefinition.BucketId != "HeroGrouping")
					{
						SurvivorToken survivorToken = new SurvivorToken();
						CurrencyType tokenTypeForRandomNumber = heroTokenDropDistributionDefinition.GetTokenTypeForRandomNumber(random.Next() * 100f);
						int randomTokenAmount = GetRandomTokenAmount(eventType, dropType, tag, currencyType, targetLevel, random);
						if (randomTokenAmount > 0 && tokenTypeForRandomNumber != CurrencyType.None)
						{
							survivorToken.Amount = randomTokenAmount;
							survivorToken.Type = tokenTypeForRandomNumber;
							return survivorToken;
						}
					}
				}
			}
			return null;
		}

		public SurvivorToken GetClassTokenTypeAndAmount(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency, int targetLevel, ModelRandom random, List<SurvivorClass> availableClasses)
		{
			int amount = 0;
			for (int i = 0; i < ((TokenDropAmounts != null) ? TokenDropAmounts.Length : 0); i++)
			{
				TokenDropAmount tokenDropAmount = TokenDropAmounts[i];
				if (tokenDropAmount != null && tokenDropAmount.EventType == eventType && tokenDropAmount.DropType == dropType && tokenDropAmount.Tag == tag && tokenDropAmount.DropCurrency == dropCurrency && targetLevel >= tokenDropAmount.ControlLevelMin && targetLevel <= tokenDropAmount.ControlLevelMax)
				{
					amount = random.GetRandomInRange(tokenDropAmount.Min, tokenDropAmount.Max);
					break;
				}
			}
			SurvivorClass randomElement = random.GetRandomElement(availableClasses.ToArray());
			return new SurvivorToken
			{
				Type = SurvivorToken.GetClassAsCurrency(randomElement),
				Amount = amount
			};
		}

		public SurvivorToken GetHeroTokenTypeAndAmount(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency, int targetLevel, ModelRandom random)
		{
			int amount = 0;
			for (int i = 0; i < ((TokenDropAmounts != null) ? TokenDropAmounts.Length : 0); i++)
			{
				TokenDropAmount tokenDropAmount = TokenDropAmounts[i];
				if (tokenDropAmount != null && tokenDropAmount.EventType == eventType && tokenDropAmount.DropType == dropType && tokenDropAmount.Tag == tag && tokenDropAmount.DropCurrency == dropCurrency && targetLevel >= tokenDropAmount.ControlLevelMin && targetLevel <= tokenDropAmount.ControlLevelMax)
				{
					amount = random.GetRandomInRange(tokenDropAmount.Min, tokenDropAmount.Max);
					break;
				}
			}
			List<CurrencyType> availableHeroTokens = GetAvailableHeroTokens();
			CurrencyType randomElement = random.GetRandomElement(availableHeroTokens, remove: false);
			return new SurvivorToken
			{
				Type = randomElement,
				Amount = amount
			};
		}

		public int GetRandomTokenAmount(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency, int targetLevel, ModelRandom random)
		{
			for (int i = 0; i < ((TokenDropAmounts != null) ? TokenDropAmounts.Length : 0); i++)
			{
				TokenDropAmount tokenDropAmount = TokenDropAmounts[i];
				if (tokenDropAmount != null && tokenDropAmount.EventType == eventType && tokenDropAmount.DropType == dropType && tokenDropAmount.Tag == tag && tokenDropAmount.DropCurrency == dropCurrency && targetLevel >= tokenDropAmount.ControlLevelMin && targetLevel <= tokenDropAmount.ControlLevelMax)
				{
					return random.GetRandomInRange(tokenDropAmount.Min, tokenDropAmount.Max);
				}
			}
			return 0;
		}

		public Dictionary<int, FixedPoint> GetGenericHeroRarityToTokenAmounts(DropType dropType, int controlLevel, DropEventDefinition.DropEventContext context)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (TokenToRarityAmounts != null && TokenToRarityAmounts.Length != 0)
			{
				TokenToRarityAmount tokenToRarityAmount = TokenToRarityAmounts[0];
				Type type = tokenToRarityAmount.GetType();
				for (int i = 0; i < 5; i++)
				{
					Rarity rarity = (Rarity)i;
					FieldInfo field = type.GetField(rarity.ToString());
					if (field != null)
					{
						int value = (int)field.GetValue(tokenToRarityAmount);
						dictionary.Add(i, value);
					}
				}
			}
			Dictionary<int, FixedPoint> dictionary2 = new Dictionary<int, FixedPoint>();
			Dictionary<int, FixedPoint> equipmentAndSurvivorRarityProbabilities = GetEquipmentAndSurvivorRarityProbabilities(dropType, DropRewardType.HeroToken, controlLevel, DropEventDefinition.DropEventTag.None, context);
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				if (equipmentAndSurvivorRarityProbabilities.TryGetValue(item.Key, out var value2) && value2 > 0L)
				{
					dictionary2.Add(item.Value, value2);
				}
			}
			return dictionary2;
		}

		public int GetHeroTokenAmountForRarity(CurrencyType type, int rarity)
		{
			return GetRarityAmountFromList(TokenToRarityAmounts, type, rarity);
		}

		public int GetClassTokenAmountForRarity(CurrencyType type, int rarity)
		{
			return GetRarityAmountFromList(TokenToRarityAmounts, type, rarity);
		}

		private int GetRarityAmountFromList(TokenToRarityAmount[] list, CurrencyType type, int rarity)
		{
			int result = 0;
			for (int i = 0; i < ((list != null) ? list.Length : 0); i++)
			{
				TokenToRarityAmount tokenToRarityAmount = list[i];
				if (tokenToRarityAmount.Type == type)
				{
					switch (rarity)
					{
					case 0:
						return tokenToRarityAmount.Common;
					case 1:
						return tokenToRarityAmount.Uncommon;
					case 2:
						return tokenToRarityAmount.Rare;
					case 3:
						return tokenToRarityAmount.Epic;
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						return tokenToRarityAmount.Legendary;
					default:
						return 0;
					}
				}
			}
			return result;
		}

		public static void GetProbabilitiesAsList<T>(ref List<KeyValuePair<FixedPoint, T>> probabilities, object obj)
		{
			probabilities.Clear();
			FixedPoint fixedPoint = 0L;
			MemberInfo[] members = obj.GetType().GetMembers();
			for (int i = 0; i < ((members != null) ? members.Length : 0); i++)
			{
				Attribute[] customAttributes = Attribute.GetCustomAttributes(members[i], typeof(Probability), inherit: true);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					FixedPoint fixedPoint2 = 0L;
					if (members[i] is FieldInfo)
					{
						fixedPoint2 = (FixedPoint)((FieldInfo)members[i]).GetValue(obj);
					}
					else if (members[i] is PropertyInfo)
					{
						fixedPoint2 = (FixedPoint)((PropertyInfo)members[i]).GetValue(obj, null);
					}
					if (fixedPoint2 > 0L)
					{
						KeyValuePair<FixedPoint, T> item = new KeyValuePair<FixedPoint, T>(fixedPoint + fixedPoint2, (T)Enum.Parse(typeof(T), members[i].Name));
						probabilities.Add(item);
						fixedPoint += fixedPoint2;
					}
				}
			}
		}

		public GuildTierDefinition GetGuildTierDefinition(int guildTier)
		{
			if (guildTier == 0)
			{
				return GuildTierDefinitions[GuildTierDefinitions.Length - 1];
			}
			if (guildTier <= GuildTierDefinitions.Length)
			{
				return GuildTierDefinitions[guildTier - 1];
			}
			DebugError("No tier definition found for tier: " + guildTier);
			return null;
		}

		public GuildTierDefinition GetGuildTierForVictoryPoints(int victoryPoints)
		{
			GuildTierDefinition guildTierDefinition = GuildTierDefinitions[GuildWarConfig.GuildBattleMinimumTier - 1];
			GuildTierDefinition guildTierDefinition2 = GetGuildTierDefinition(guildTierDefinition.Tier - 1);
			while (guildTierDefinition.Tier > 1 && victoryPoints >= guildTierDefinition2.VictoryPointsRequired)
			{
				guildTierDefinition = guildTierDefinition2;
				guildTierDefinition2 = GetGuildTierDefinition(guildTierDefinition.Tier - 1);
			}
			return guildTierDefinition;
		}

		public float GetGuildBattleVictoryPointsMultiplierForTier(int guildTier)
		{
			if (GuildTierDefinitions != null)
			{
				GuildTierDefinition guildTierDefinition = GetGuildTierDefinition(guildTier);
				if (guildTierDefinition != null)
				{
					return guildTierDefinition.VictoryPointsMultiplier;
				}
			}
			return 0f;
		}

		public float GetGuildBattleVictoryRewardPointsMultiplierForTier(int guildTier)
		{
			if (GuildTierDefinitions != null)
			{
				GuildTierDefinition guildTierDefinition = GetGuildTierDefinition(guildTier);
				if (guildTierDefinition != null)
				{
					return guildTierDefinition.RewardPointsMultiplier;
				}
			}
			return 0f;
		}

		public float GetGuildBattleDrawPointsMultiplierForTier(int guildTier)
		{
			if (GuildTierDefinitions != null)
			{
				GuildTierDefinition guildTierDefinition = GetGuildTierDefinition(guildTier);
				if (guildTierDefinition != null)
				{
					return guildTierDefinition.DrawPointsMultiplier;
				}
			}
			return 0f;
		}

		public float GetGuildBattleDrawRewardPointsMultiplierForTier(int guildTier)
		{
			if (GuildTierDefinitions != null)
			{
				GuildTierDefinition guildTierDefinition = GetGuildTierDefinition(guildTier);
				if (guildTierDefinition != null)
				{
					return guildTierDefinition.RewardPointsDrawMultiplier;
				}
			}
			return 0f;
		}

		public ActorDefinition GetActorDefinitionForToken(CurrencyType token)
		{
			if (tokenToActorDefinitionLookup == null)
			{
				tokenToActorDefinitionLookup = new Dictionary<CurrencyType, ActorDefinition>();
			}
			if (!tokenToActorDefinitionLookup.TryGetValue(token, out var value))
			{
				value = GetActorDefinitionForTokenInternal(token);
				if (value != null)
				{
					tokenToActorDefinitionLookup.Add(token, value);
				}
			}
			return value;
		}

		private ActorDefinition GetActorDefinitionForTokenInternal(CurrencyType token)
		{
			string heroId = SurvivorToken.GetHeroId(token);
			if (!string.IsNullOrEmpty(heroId))
			{
				return GetActorDefinition(heroId);
			}
			return null;
		}

		public List<CurrencyType> GetAvailableHeroTokens()
		{
			List<CurrencyType> list = new List<CurrencyType>();
			CurrencyType[] array = CurrencyModel.GetHeroTokenCurrencyTypes();
			int num = CurrencyModel.GetHeroTokenCurrencyTypes().Length;
			for (int i = 0; i < num; i++)
			{
				ActorDefinition actorDefinitionForToken = GetActorDefinitionForToken(array[i]);
				if (actorDefinitionForToken != null && actorDefinitionForToken.IncludedInTokenPool)
				{
					list.Add(array[i]);
				}
			}
			return list;
		}

		public FeaturedHeroDefinition GetActiveFeaturedHero(long utcTimeStamp)
		{
			if (cachedFeaturedHero != null)
			{
				if (cachedFeaturedHero.IsActivePeriod(utcTimeStamp))
				{
					return cachedFeaturedHero;
				}
				cachedFeaturedHero = null;
			}
			for (int i = 0; i < ((FeaturedHeroDefinitions != null) ? FeaturedHeroDefinitions.Length : 0); i++)
			{
				if (FeaturedHeroDefinitions[i].IsActivePeriod(utcTimeStamp))
				{
					cachedFeaturedHero = FeaturedHeroDefinitions[i];
					break;
				}
			}
			return cachedFeaturedHero;
		}

		public void Optimize()
		{
		}

		public TraitRerollCostDefinitions GetTraitRerollCost(int traitLevel)
		{
			for (int i = 0; i < ((TraitRerollCosts != null) ? TraitRerollCosts.Length : 0); i++)
			{
				if (TraitRerollCosts[i].TraitLevel == traitLevel)
				{
					return TraitRerollCosts[i];
				}
			}
			return null;
		}

		public bool TryGetGiftCodeDefinition(string code, out GiftCodeDefinition giftCodeDefinition)
		{
			return giftCodeDefinitions.TryGetValue(code, out giftCodeDefinition);
		}

		public bool TryGetDeepLinkDefinition(string deepLink, out DeepLinkDefinition deepLinkDefinition)
		{
			return deepLinkDefinitions.TryGetValue(deepLink, out deepLinkDefinition);
		}

		public SupportDefinition GetSupportDefinition(string id)
		{
			return supportDefinitionsMap != null ? supportDefinitionsMap[id] : null;
		}

		public FixedPoint TryGetEndlessModeMultiplierDecreaseRate(int wave)
		{
			foreach (KeyValuePair<int, FixedPoint> scoreMultiplierDecreaseRate in scoreMultiplierDecreaseRates)
			{
				if (wave < scoreMultiplierDecreaseRate.Key)
				{
					return scoreMultiplierDecreaseRate.Value;
				}
			}
			return scoreMultiplierDecreaseRates.Last().Value;
		}

		public EndlessModeCalendarDefinition GetCurrentEndlessCalendarDefinition(long currentTimeStamp)
		{
			if (EndlessModeCalendarDefinitions == null)
			{
				return null;
			}
			EndlessModeCalendarDefinition[] endlessModeCalendarDefinitions = EndlessModeCalendarDefinitions;
			foreach (EndlessModeCalendarDefinition endlessModeCalendarDefinition in endlessModeCalendarDefinitions)
			{
				if (currentTimeStamp >= endlessModeCalendarDefinition.StartTimeMilliseconds && currentTimeStamp < endlessModeCalendarDefinition.EndTimeMilliseconds)
				{
					return endlessModeCalendarDefinition;
				}
			}
			return null;
		}

		public string GetEndlessLeaderboardRewardsSetForDefinitionId(int id)
		{
			return EndlessModeCalendarDefinitions.FirstOrDefault((EndlessModeCalendarDefinition x) => x.Identifier == id)?.LeaderBoardRewardSetID;
		}

		public string GetEndlessLeaderboardRewardsSetForDefinitionIdWithZoneId(int id, int zoneId)
		{
			EndlessModeCalendarDefinition endlessModeCalendarDefinition = EndlessModeCalendarDefinitions.FirstOrDefault((EndlessModeCalendarDefinition x) => x.Identifier == id);
			return zoneId switch
			{
				1 => endlessModeCalendarDefinition?.LeaderBoardRewardSetID1,
				2 => endlessModeCalendarDefinition?.LeaderBoardRewardSetID2,
				3 => endlessModeCalendarDefinition?.LeaderBoardRewardSetID,
				_ => null,
			};
		}

		public EndlessModeCalendarDefinition GetNextEndlessCalendarDefinition(long currentTimeStamp, long playerUTC)
		{
			if (EndlessModeCalendarDefinitions == null)
			{
				return null;
			}
			EndlessModeCalendarDefinition[] endlessModeCalendarDefinitions = EndlessModeCalendarDefinitions;
			foreach (EndlessModeCalendarDefinition endlessModeCalendarDefinition in endlessModeCalendarDefinitions)
			{
				if (currentTimeStamp < endlessModeCalendarDefinition.StartTimeMilliseconds && playerUTC < endlessModeCalendarDefinition.StartTimeMilliseconds)
				{
					return endlessModeCalendarDefinition;
				}
			}
			return null;
		}

		public EndlessModeCalendarDefinition GetEndlessModeCalendarDefinitionById(int identifier)
		{
			if (EndlessModeCalendarDefinitions == null)
			{
				return null;
			}
			for (int i = 0; i < EndlessModeCalendarDefinitions.Length; i++)
			{
				if (EndlessModeCalendarDefinitions[i].Identifier == identifier)
				{
					return EndlessModeCalendarDefinitions[i];
				}
			}
			return null;
		}

		public EndlessModeScoringDefinition GetEndlessModeScoringDefinitionByWalkerType(string walkerType)
		{
			WalkerType typeEnum = GetTypeEnum<WalkerType>(walkerType);
			if (EndlessModeScoringDefinitions == null)
			{
				return null;
			}
			EndlessModeScoringDefinition[] endlessModeScoringDefinitions = EndlessModeScoringDefinitions;
			foreach (EndlessModeScoringDefinition endlessModeScoringDefinition in endlessModeScoringDefinitions)
			{
				WalkerType typeEnum2 = GetTypeEnum<WalkerType>(endlessModeScoringDefinition.Enemy);
				if (typeEnum == typeEnum2)
				{
					return endlessModeScoringDefinition;
				}
			}
			return null;
		}

		public ChallengeNightmareSpawnSetup GetChallengeNightmareDefinitionByRound(int round)
		{
			for (int i = 0; i < ChallengeNightmareSpawnSetups.Length; i++)
			{
				if (ChallengeNightmareSpawnSetups[i].Round == round)
				{
					return ChallengeNightmareSpawnSetups[i];
				}
			}
			return ChallengeNightmareSpawnSetups.LastOrDefault();
		}

		public List<BounsInfoDefinition> GetBounsInfoDefinitionsByOwner(string ownerId)
		{
			List<BounsInfoDefinition> list = new List<BounsInfoDefinition>();
			for (int i = 0; i < BounsDefinitions.Length; i++)
			{
				if (BounsDefinitions[i].Owner == ownerId)
				{
					list.Add(BounsDefinitions[i]);
				}
			}
			return list;
		}

		public BounsInfoDefinition GetBounsInfo(int itemId)
		{
			for (int i = 0; i < BounsDefinitions.Length; i++)
			{
				if (BounsDefinitions[i].ItemID == itemId)
				{
					return BounsDefinitions[i];
				}
			}
			return null;
		}

		public BounsLevelDefinition GetBounsLevelDefinition(int itemId, int Level)
		{
			for (int i = 0; i < BounsLevelDefinitions.Length; i++)
			{
				if (BounsLevelDefinitions[i].ItemID == itemId && BounsLevelDefinitions[i].Level == Level)
				{
					return BounsLevelDefinitions[i];
				}
			}
			return null;
		}

		public int GetBounsMaxLevel(int ItemId)
		{
			int num = 0;
			for (int i = 0; i < BounsLevelDefinitions.Length; i++)
			{
				if (BounsLevelDefinitions[i].ItemID == ItemId && BounsLevelDefinitions[i].Level > num)
				{
					num = BounsLevelDefinitions[i].Level;
				}
			}
			return num;
		}

		private void CalculateItemTypeDefinitions()
		{
			if (TypeDefinitions != null && ItemDefinitions != null)
			{
				for (int i = 0; i < TypeDefinitions.Length; i++)
				{
					TypeDefinition typeDefinition = TypeDefinitions[i];
					if (typeDefinition.SubType != null)
					{
						typeDefinition.SetItems(ItemDefinitions.Where((ItemDefinition x) => typeDefinition.SubType.Contains(x.Type) || (x.IsSubType && typeDefinition.SubType.Contains(x.ItemName))).ToList());
					}
				}
			}
			if (ItemDefinitions == null || AcquisitionDefinitions == null)
			{
				return;
			}
			for (int num = 0; num < ItemDefinitions.Length; num++)
			{
				ItemDefinition item = ItemDefinitions[num];
				if (item.Acquisition != null)
				{
					item.SetAcquisitionLocalization((from x in AcquisitionDefinitions
						where item.Acquisition.Contains(x.Index)
						select x.Localization).ToList());
				}
			}
		}

		public ItemGetType GetItemGetDefinition(string itemName)
		{
			if (ItemDefinitions != null)
			{
				for (int i = 0; i < ItemGetDefinitions.Length; i++)
				{
					if (ItemGetDefinitions[i].ItemName.Equals(itemName))
					{
						return ItemGetDefinitions[i].TargetUI;
					}
				}
			}
			return ItemGetType.None;
		}

		public ItemDefinition GetItemDefinition(string itemName)
		{
			if (ItemDefinitions == null || ItemDefinitions.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < ItemDefinitions.Length; i++)
			{
				if (ItemDefinitions[i] != null && ItemDefinitions[i].ItemName == itemName)
				{
					return ItemDefinitions[i];
				}
			}
			return null;
		}

		public SPTraitsSkillKitTokenSet GetSPTraitsSkillKitTokenSetByID(string id)
		{
			for (int i = 0; i < SPTraitsSkillKitTokenSets.Length; i++)
			{
				if (SPTraitsSkillKitTokenSets[i].ID == id)
				{
					return SPTraitsSkillKitTokenSets[i];
				}
			}
			return null;
		}

		public EquipmentSkillSuggestion GetEquipmentSkillSuggestionByID(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			if (EquipmentSkillSuggestions == null)
			{
				return null;
			}
			for (int i = 0; i < EquipmentSkillSuggestions.Length; i++)
			{
				EquipmentSkillSuggestion equipmentSkillSuggestion = EquipmentSkillSuggestions[i];
				if (equipmentSkillSuggestion != null && equipmentSkillSuggestion.ID == id)
				{
					return equipmentSkillSuggestion;
				}
			}
			return null;
		}

		public List<EquipmentSkillSuggestion> GetEquipmentSkillSuggestionByEquipID(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			if (EquipmentSkillSuggestionsByPackageId == null)
			{
				EquipmentSkillSuggestionsByPackageId = new Dictionary<string, List<EquipmentSkillSuggestion>>();
			}
			if (EquipmentSkillSuggestionsByPackageId.TryGetValue(id, out var value))
			{
				return value;
			}
			value = new List<EquipmentSkillSuggestion>();
			if (EquipmentSkillSuggestions == null)
			{
				EquipmentSkillSuggestionsByPackageId.Add(id, value);
				return value;
			}
			for (int i = 0; i < EquipmentSkillSuggestions.Length; i++)
			{
				EquipmentSkillSuggestion equipmentSkillSuggestion = EquipmentSkillSuggestions[i];
				if (equipmentSkillSuggestion != null && equipmentSkillSuggestion.EquipmentList != null && equipmentSkillSuggestion.EquipmentList.Contains(id))
				{
					value.Add(equipmentSkillSuggestion);
				}
			}
			EquipmentSkillSuggestionsByPackageId.Add(id, value);
			return value;
		}

		public EndlessModeLeaderBoardReward GetEndlessModeLeaderBoardReward(string setId, long position, long entryCount)
		{
			if (GetLeaderBoardRewardTypeByPosition(position, setId) == EndlessModeLeaderBoardRewardType.Ranked)
			{
				return GetRankRewardForEndless(position, EndlessModeLeaderBoardRewards.Where((EndlessModeLeaderBoardReward x) => x.RewardSetID == setId && x.RewardType == EndlessModeLeaderBoardRewardType.Ranked).ToArray());
			}
			return GetPercentageRewardForEndless(position, entryCount, EndlessModeLeaderBoardRewards.Where((EndlessModeLeaderBoardReward x) => x.RewardSetID == setId && x.RewardType == EndlessModeLeaderBoardRewardType.Percentage).ToArray());
		}

		public EndlessModeLeaderBoardRewardType GetLeaderBoardRewardTypeByPosition(long position, string setId)
		{
			EndlessModeLeaderBoardReward endlessModeLeaderBoardReward = EndlessModeLeaderBoardRewards.LastOrDefault((EndlessModeLeaderBoardReward x) => x.RewardSetID == setId && x.RewardType == EndlessModeLeaderBoardRewardType.Ranked);
			if (endlessModeLeaderBoardReward == null)
			{
				return EndlessModeLeaderBoardRewardType.None;
			}
			if ((!endlessModeLeaderBoardReward.RewardBracket.Contains('-')) ? int.TryParse(endlessModeLeaderBoardReward.RewardBracket, out var result) : int.TryParse(endlessModeLeaderBoardReward.RewardBracket.Split('-')[1], out result))
			{
				if (position <= result)
				{
					return EndlessModeLeaderBoardRewardType.Ranked;
				}
				return EndlessModeLeaderBoardRewardType.Percentage;
			}
			DebugError("Unable to parse rank reward for endless: " + endlessModeLeaderBoardReward.RewardBracket);
			return EndlessModeLeaderBoardRewardType.None;
		}

		private EndlessModeLeaderBoardReward GetRankRewardForEndless(long position, in EndlessModeLeaderBoardReward[] rewards)
		{
			EndlessModeLeaderBoardReward[] array = rewards;
			foreach (EndlessModeLeaderBoardReward endlessModeLeaderBoardReward in array)
			{
				string rewardBracket = endlessModeLeaderBoardReward.RewardBracket;
				int result3;
				if (rewardBracket.Contains('-'))
				{
					string[] array2 = rewardBracket.Split('-');
					if (int.TryParse(array2[0], out var result) && int.TryParse(array2[1], out var result2) && position >= result && position <= result2)
					{
						return endlessModeLeaderBoardReward;
					}
				}
				else if (int.TryParse(rewardBracket, out result3) && position == result3)
				{
					return endlessModeLeaderBoardReward;
				}
			}
			return null;
		}

		private EndlessModeLeaderBoardReward GetPercentageRewardForEndless(long position, long entryCount, in EndlessModeLeaderBoardReward[] rewards)
		{
			FixedPoint fixedPoint = (FixedPoint)position / (FixedPoint)entryCount * 100L;
			EndlessModeLeaderBoardReward[] array = rewards;
			foreach (EndlessModeLeaderBoardReward endlessModeLeaderBoardReward in array)
			{
				string rewardBracket = endlessModeLeaderBoardReward.RewardBracket;
				int result3;
				if (rewardBracket.Contains('-'))
				{
					string[] array2 = rewardBracket.Split('-');
					if (int.TryParse(array2[0], out var result) && int.TryParse(array2[1], out var result2))
					{
						result--;
						if (fixedPoint > result && fixedPoint <= result2)
						{
							return endlessModeLeaderBoardReward;
						}
					}
				}
				else if (int.TryParse(rewardBracket, out result3) && fixedPoint <= result3)
				{
					return endlessModeLeaderBoardReward;
				}
			}
			return null;
		}

		public EndlessModeLeaderSurvivorClassLeaderBoardReward GetEndlessModeLeaderSurvivorClassLeaderBoardReward(string setId, long position, long entryCount, SurvivorClass survivorClass)
		{
			if (GetLeaderBoardRewardTypeByPosition(position, setId) == EndlessModeLeaderBoardRewardType.Ranked)
			{
				return GetLeaderSurvivorClassRankRewardForEndless(position, EndlessModeLeaderBoardRewards.Where((EndlessModeLeaderBoardReward x) => x.RewardSetID == setId && x.RewardType == EndlessModeLeaderBoardRewardType.Ranked).ToArray(), survivorClass);
			}
			return GetLeaderSurvivorClassPercentageRewardForEndless(position, entryCount, EndlessModeLeaderBoardRewards.Where((EndlessModeLeaderBoardReward x) => x.RewardSetID == setId && x.RewardType == EndlessModeLeaderBoardRewardType.Percentage).ToArray(), survivorClass);
		}

		private EndlessModeLeaderSurvivorClassLeaderBoardReward GetLeaderSurvivorClassRankRewardForEndless(long position, in EndlessModeLeaderBoardReward[] rewards, SurvivorClass survivorClass)
		{
			EndlessModeLeaderBoardReward[] array = rewards;
			foreach (EndlessModeLeaderBoardReward endlessModeLeaderBoardReward in array)
			{
				string rewardBracket = endlessModeLeaderBoardReward.RewardBracket;
				int result3;
				if (rewardBracket.Contains('-'))
				{
					string[] array2 = rewardBracket.Split('-');
					if (int.TryParse(array2[0], out var result) && int.TryParse(array2[1], out var result2) && position >= result && position <= result2)
					{
						return ReorganizeEndlessModeLeaderSurvivorClassLeaderBoardReward(endlessModeLeaderBoardReward, survivorClass);
					}
				}
				else if (int.TryParse(rewardBracket, out result3) && position == result3)
				{
					return ReorganizeEndlessModeLeaderSurvivorClassLeaderBoardReward(endlessModeLeaderBoardReward, survivorClass);
				}
			}
			return null;
		}

		private EndlessModeLeaderSurvivorClassLeaderBoardReward GetLeaderSurvivorClassPercentageRewardForEndless(long position, long entryCount, in EndlessModeLeaderBoardReward[] rewards, SurvivorClass survivorClass)
		{
			if (entryCount <= 0)
			{
				return new EndlessModeLeaderSurvivorClassLeaderBoardReward();
			}
			FixedPoint fixedPoint = (FixedPoint)position / (FixedPoint)entryCount * 100L;
			EndlessModeLeaderBoardReward[] array = rewards;
			foreach (EndlessModeLeaderBoardReward endlessModeLeaderBoardReward in array)
			{
				string rewardBracket = endlessModeLeaderBoardReward.RewardBracket;
				int result3;
				if (rewardBracket.Contains('-'))
				{
					string[] array2 = rewardBracket.Split('-');
					if (int.TryParse(array2[0], out var result) && int.TryParse(array2[1], out var result2))
					{
						result--;
						if (fixedPoint > result && fixedPoint <= result2)
						{
							return ReorganizeEndlessModeLeaderSurvivorClassLeaderBoardReward(endlessModeLeaderBoardReward, survivorClass);
						}
					}
				}
				else if (int.TryParse(rewardBracket, out result3) && fixedPoint <= result3)
				{
					return ReorganizeEndlessModeLeaderSurvivorClassLeaderBoardReward(endlessModeLeaderBoardReward, survivorClass);
				}
			}
			return null;
		}

		private EndlessModeLeaderSurvivorClassLeaderBoardReward ReorganizeEndlessModeLeaderSurvivorClassLeaderBoardReward(EndlessModeLeaderBoardReward endlessModeLeaderBoardReward, SurvivorClass survivorClass)
		{
			EndlessModeLeaderSurvivorClassLeaderBoardReward endlessModeLeaderSurvivorClassLeaderBoardReward = new EndlessModeLeaderSurvivorClassLeaderBoardReward
			{
				RewardSetID = endlessModeLeaderBoardReward.RewardSetID,
				RewardBracket = endlessModeLeaderBoardReward.RewardBracket
			};
			switch (survivorClass)
			{
			case SurvivorClass.Shooter:
				endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards = endlessModeLeaderBoardReward.RewardShooter;
				break;
			case SurvivorClass.Hunter:
				endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards = endlessModeLeaderBoardReward.RewardHunter;
				break;
			case SurvivorClass.Bruiser:
				endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards = endlessModeLeaderBoardReward.RewardBruiser;
				break;
			case SurvivorClass.Warrior:
				endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards = endlessModeLeaderBoardReward.RewardWarrior;
				break;
			case SurvivorClass.Scout:
				endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards = endlessModeLeaderBoardReward.RewardScout;
				break;
			case SurvivorClass.Assault:
				endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards = endlessModeLeaderBoardReward.RewardAssault;
				break;
			default:
				endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards = null;
				break;
			}
			return endlessModeLeaderSurvivorClassLeaderBoardReward;
		}

		public GoldRadioCallDenifition GetGoldRadioCallDenifitionByID(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			if (GoldRadioCallDenifitions == null)
			{
				return null;
			}
			for (int i = 0; i < GoldRadioCallDenifitions.Length; i++)
			{
				GoldRadioCallDenifition goldRadioCallDenifition = GoldRadioCallDenifitions[i];
				if (goldRadioCallDenifition != null && goldRadioCallDenifition.Identifier.ToString() == id)
				{
					return goldRadioCallDenifition;
				}
			}
			return null;
		}



		#region mycode
		public EquipTraitsDefinition GetEquipTraitsDefinition(SurvivorClass remodelClass, EquipmentCategory equipmentType, string identifier)
		{
			return EquipTraitsDefinitions.FirstOrDefault((EquipTraitsDefinition x) => x.SurvivorClass == remodelClass && x.EquipmentType == equipmentType && x.TraitsGroup == identifier);
		}
		#endregion
	}
}
