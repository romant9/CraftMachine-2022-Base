using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BaseModel;
using BaseModel.ContentTypes;
using Newtonsoft.Json;
using GooglePlayGames.BasicApi;
using TwdCustomMod;

namespace TWDModel
{
	public class PlayerModel : TWDModelObject, IPlayerModel, ICustomLoggerDebugInfo
	{
		public const int NameMinLength = 3;

		public const int NameMaxLength = 15;

		public AutoScrapEquipmentType IsEquipmentAutoScrap;

		public ModelList<EquipmentItemModel> AutoScrapmentEquipment;

		public bool ResetCombat790;

		public bool ResetCombat790_01;

		public string Platform;

		public static int UnlimitedCapacityAmount = 2146483647;

		public List<string> RedeemedCodes;

		public List<string> RedeemedDeeplinks;

		private IDictionary<string, SupportModel> supportModelsMap;

		public int PlayerRandomSeed;

		public StorePurchaseInfo CurrentIAP;

		public string Version = "";

		public const string GuildChangedEvent = "guildChanged";

		public const string IAPOfferAvailableEvent = "iapOfferAvailableEvent";

		public const string IAPOfferExpiredEvent = "iapOfferExpiredEvent";

		public const string AutoScrapEquipmentMessage = "AutoScrapEquipmentMessage";

		public const string GuildGiftClaimed = "guildGiftClaimed";

		public const string GuildGiftAvailable = "guildGiftAvailable";

		public const string TradeShopRefreshed = "TradeShopRefreshed";

		public const string TradeShopSlotBought = "TradeShopSlotBought";

		public const string TradeShopItemBought = "TradeShopItemBought";

		public const string PlayerEmblemChanged = "PlayerEmblemChanged";

		private string guildId;

		private bool cacheIsGoreDisabled;

		public const string CurrencyChangedEvent = "currencyChangedEvent";

		public const string CurrencyConvertToDiamondsEvent = "CurrencyConvertToDiamondsEvent";

		public const string SpeedUpTokenAcquired = "SpeedUpTokenAcquired";

		public const string SpeedUpTokenUsed = "SpeedUpTokenUsed";

		public const string ConsumableAcquired = "ConsumableAcquired";

		public const string SpEquipmentRemoldTraitsUpgrade = "SpEquipmentRemoldTraitsUpgrade";

		public static int DevRandomSeed = -1;

		private CombatModel combatModel;

		public OutpostTutorialState OutpostTutorialState;

		[NonSerialized]
		[JsonIgnore]
		public AchievementManager AchievementManager;

		public const string CombatModelDeletedEvent = "CombatModelDeleted";

		private IMapMissionModel _attackTargetMissionModel;

		public PlayerAttributeContainerModel PlayerAttributeContainer { get; set; }

		public long LifeTime { get; protected set; }

		[Segmental]
		[JsonIgnore]
		public long LifeTimeInDays => LifeTime / 86400000;

		[JsonIgnore]
		public long SecondsSinceLastPurchase => (UtcTimeStamp - BundleManager.LastPurchaseUTCTime) / 1000;

		[JsonIgnore]
		public long SecondsSinceFirstPurchase => (UtcTimeStamp - BundleManager.FirstPurchaseUTCTime) / 1000;

		public string Name { get; set; }

		public List<PlayerNameData> PreviousNames { get; set; }

		public long LastNameSetTimestamp { get; set; }

		[Segmental(Name = "GameLanguage", SegmentalPropertyHandlerType = SegmentalPropertyHandlerType.ModelLanguageList)]
		public string Language { get; set; }

		[Segmental(Name = "GameCountryCode", SegmentalPropertyHandlerType = SegmentalPropertyHandlerType.CountryList)]
		public string Country { get; set; }

		public ModelList<SupportModel> SupportModels { get; set; }

		public string[] EquippedSupportIds { get; set; }

		public PcPlatform PcPlatform { get; set; }

		public TeamPresetsManager TeamPresetsManager { get; set; }

		public BattlePassModel BattlePass { get; set; }

		public BeginnerBattlePassInfo BeginnerBattlePassInfo { get; set; }

		public ModelList<CombatBackup> CombatBackups { get; set; }

		[Segmental]
		public int Score
		{
			get
			{
				if (WeeklyChallenge == null)
				{
					return 0;
				}
				return WeeklyChallenge.AllTimeNumberStars;
			}
		}

		public int HighestWeeklyChallengeScore { get; set; }

		public int HighestWeeklyChallengeDifficulty { get; set; }

		public int HighestWeeklySurvivalScore { get; set; }

		public int HighestWeeklySurvivalDifficulty { get; set; }

		[Segmental(Name = "WalkersKilled")]
		public int SecondaryScore => MissionStatistics.WalkersKilled;

		[Segmental]
		public int Level { get; protected set; }

		[Segmental]
		public int Xp { get; private set; }

		public List<string> BoughtIAPs { get; set; }

		public List<int> BoughtIAPsQuantity { get; set; }

		public List<SessionEntry> SessionHistory { get; set; }

		public int LastSessionDay { get; set; }

		public List<CombatHistoryEntry> CombatHistory { get; set; }

		public List<PendingPurchaseInfo> PendingIAPs { get; set; }

		public List<StorePurchaseInfo> UnhandledPurchases { get; set; }

		public List<BundleOfferPurchaseEntry> DisallowedOffers { get; set; }

		public List<GuildGift> PendingGuildGiftsToOpen { get; set; }

		public List<LootEntry> PendingGuildGiftsLootToOpen { get; set; }

		public List<GuildGift> OpenedGuildGifts { get; set; }

		public LootEntry LastOpenedGuildGiftLoot { get; set; }

		public long GiftCoolDownTimer { get; set; }

		public ModelList<LootEntry> LootBoxesToOpen { get; set; }

		public string MapName { get; protected set; }

		public bool ScrappedExcessItems { get; set; }

		public Rewards MigratedAchievementRewards { get; set; }

		public bool ShouldConsumeMissionCurrency { get; set; }

		[JsonIgnore]
		public bool ModelHotfixWasApplied { get; set; }

		[JsonIgnore]
		public int TeamPotentialStrength
		{
			get
			{
				if (SurvivorContainer != null && SurvivorContainer.Survivors != null && base.gameEconomyData != null && base.gameEconomyData.ConfigData != null)
				{
					List<SurvivorModel> list = new List<SurvivorModel>();
					int count = SurvivorContainer.Survivors.Count;
					for (int i = 0; i < count; i++)
					{
						SurvivorModel item = SurvivorContainer.Survivors[i];
						list.Add(item);
					}
					if (list.Count > 0)
					{
						list.StableSort(delegate(SurvivorModel a, SurvivorModel b)
						{
							int num4 = ((base.gameEconomyData.ConfigData.ChallengeDifficultySurvivorRarityThreshold > 0) ? Math.Max(0, a.SurvivorRarityLevel - base.gameEconomyData.ConfigData.ChallengeDifficultySurvivorRarityThreshold) : 0);
							int num5 = ((base.gameEconomyData.ConfigData.ChallengeDifficultySurvivorRarityThreshold > 0) ? Math.Max(0, b.SurvivorRarityLevel - base.gameEconomyData.ConfigData.ChallengeDifficultySurvivorRarityThreshold) : 0);
							return Math.Sign(b.Level + num5 - (a.Level + num4));
						});
						int num = Math.Max(0, list.Count - base.gameEconomyData.ConfigData.ChallengeDifficultyTopSurvivorsAmount);
						if (num > 0)
						{
							list.RemoveRange(base.gameEconomyData.ConfigData.ChallengeDifficultyTopSurvivorsAmount, num);
						}
						int num2 = 0;
						if (list != null && list.Count > 0)
						{
							for (int num3 = 0; num3 < list.Count; num3++)
							{
								num2 += list[num3].Level;
								if (base.gameEconomyData.ConfigData.ChallengeDifficultySurvivorRarityThreshold > 0)
								{
									num2 += Math.Max(0, list[num3].SurvivorRarityLevel - base.gameEconomyData.ConfigData.ChallengeDifficultySurvivorRarityThreshold);
								}
							}
							return Math.Min(base.manager.GameEconomyData.ConfigData.ChallengeMaxPTS, num2 / list.Count);
						}
					}
				}
				return 0;
			}
		}

		[JsonIgnore]
		public bool CanChangePlayerName
		{
			get
			{
				if (TimeToBeAbleToChangeName <= 0)
				{
					if (NameChangedCount >= base.gameEconomyData.ConfigData.PlayerNameChangeMaxTimes)
					{
						return base.gameEconomyData.ConfigData.PlayerNameChangeMaxTimes < 1;
					}
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public long TimeToBeAbleToChangeName
		{
			get
			{
				long num = UtcTimeStamp - LastNameSetTimestamp;
				return base.gameEconomyData.ConfigData.PlayerNameChangeMinTime - num;
			}
		}

		[JsonIgnore]
		public long NameChangedCount => (PreviousNames != null) ? PreviousNames.Count : 0;

		public ModelList<CurrencyModel> Currencies { get; set; }

		public string HashedId { get; set; }

		public DateTime Created { get; set; }

		public List<string> NewsLetterItemsRead { get; set; }

		public List<string> NewsLetterItemsInteracted { get; set; }

		public long LastReadChatTime { get; set; }

		public bool HasSeenSocial { get; set; }

		public string GuildId
		{
			get
			{
				if (string.IsNullOrEmpty(guildId) && IsLoadDataManager)
				{
					guildId = GWTeamUtils.Instance.GuildID;
				}
				return guildId;
			}
			set
			{
				if (guildId != value)
				{
					guildId = value;
					NotifyChange("guildChanged");
				}
			}
		}

		public List<SurvivorMockData> GvGDefenders { get; set; }

		public long UtcTimestampLastGvgDefendersUpdate { get; set; }

		[Segmental]
		[JsonIgnore]
		public bool HasGuild => !string.IsNullOrEmpty(GuildId);

		[Segmental(Name = "IsGuildMember")]
		[JsonIgnore]
		public bool? HasManagerAndIsGuildMember
		{
			get
			{
				if (base.manager != null)
				{
					return IsGuildMember;
				}
				return null;
			}
		}

		[JsonIgnore]
		public bool IsGuildMember
		{
			get
			{
				GuildModel guildModel = GuildModel;
				if (guildModel != null && guildModel.GetMemberInfo(HashedId) != null)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public string GuildAnalyticId => GuildModel?.Id;

		[JsonIgnore]
		public string GuildName => GuildModel?.Name;

		[JsonIgnore]
		public GuildModel GuildModel
		{
			get
			{
				if (IsLoadDataManager)
				{
					return GWTeamUtils.Instance.GuildModel;
				}
				if (GuildId == null)
				{
					return null;
				}
				return base.manager.GetGroupModel(GuildId) as GuildModel;
			}
		}

		[JsonIgnore]
		public GvGSeasonModel GvGSeasonModel
		{
			get
			{
				if (GuildModel == null)
				{
					return null;
				}
				return GuildModel.GvGSeasonModel;
			}
		}

		[JsonIgnore]
		public GuildWarModel GuildWarModel
		{
			get
			{
				if (GuildModel == null)
				{
					return null;
				}
				return GuildModel.GuildWarModel;
			}
		}

		[JsonIgnore]
		public GuildBattleModelPlayer GuildBattlePlayer => GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel;

		[JsonIgnore]
		public long UtcTimeStamp => TimeStamp;

		[JsonIgnore]
		private long TimeStamp
		{
			get
			{
				long overrideTime;
				if (IsLoadDataManager && StartGWBattle.Instance && HelpersModel.IsUnlockAllSectors && StartGWBattle.Instance.OverrideHours > 0)
				{
					long delta = (long)TimeSpan.FromHours(StartGWBattle.Instance.OverrideHours).TotalMilliseconds;
					overrideTime = StartGWBattle.Instance.guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.TimeSlot + delta;
				}
				else
				{
					DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
					overrideTime = (long)(Created.ToUniversalTime() - dateTime).TotalSeconds * 1000 + LifeTime;
				}
				return overrideTime;
			}
		}

		[JsonIgnore]
		public DateTime UtcTime => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime() + TimeSpan.FromMilliseconds(UtcTimeStamp);

		[JsonIgnore]
		public long CreationTimeStamp
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(Created.ToUniversalTime() - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		[Segmental]
		public bool IsGoreDisabled
		{
			get
			{
				if (Blackboard == null)
				{
					return false;
				}
				return Blackboard.IsToggleOn("Toggle.GoreDisabled");
			}
		}

		public CampModel Camp { get; protected set; }

		public CampMoverModel CampMover { get; protected set; }

		public MapContainerModel MapContainerModel { get; protected set; }

		public TutorialModel Tutorial { get; protected set; }

		public bool CombatTutorialCompleted { get; set; }

		public PhoneCallModel PhoneCall { get; protected set; }

		public BlackboardModel Blackboard { get; set; }

		public EndlessModeManagerModel EndlessModeManager { get; set; }

		public EquipPrizeWheelModel EquipPrizeWheelModel { get; set; }

		[JsonIgnore]
		public AbilityManagerModel AbilityManager { get; set; }

		public EquipmentModel Equipment { get; private set; }

		public EquipTokenContainerModel EquipTokenContainer { get; set; }

		public SurvivorContainerModel SurvivorContainer { get; private set; }

		public SurvivalSavedMissionModel SavedSurvivalMissionData { get; set; }

		public LootManagerModel LootManager { get; set; }

		public string LastCompletedMissionId { get; private set; }

		public string SelectedMissionId { get; private set; }

		public string SelectedMissionFlavor { get; private set; }

		public int SelectedMissionDifficulty { get; private set; }

		public bool SelectedMissionIsDeadly { get; private set; }

		public int SelectedSectorId { get; private set; }

		public DropEventDefinition.DropEventTag SelectedMissionLootTag { get; private set; }

		public int SelectedMissionRandomSeed { get; private set; }

		public MissionStatistics MissionStatistics { get; private set; }

		public ModelRandom PlayerRandom { get; set; }

		public bool PendingVideoAdReward { get; set; }

		public long VideoAdRewardTime { get; set; }

		public long LastVideoAdRewardTime { get; set; }

		public int VideoAdsServed { get; set; }

		public bool PendingVideoAdRewardInRewardScreen { get; set; }

		public long VideoAdRewardTimeRewardScreen { get; set; }

		public long LastVideoAdRewardTimeRewardScreen { get; set; }

		public int VideoAdsServedRewardScreen { get; set; }

		public bool PendingVideoAdRewardInBuildingMenu { get; set; }

		public long VideoAdRewardBuildingMenuScreen { get; set; }

		public int VideoAdsServedBuildingMenuScreen { get; set; }

		public bool PendingVideoAdRewardInBlackMarketScreen { get; set; }

		public long VideoAdRewardBlackMarketScreen { get; set; }

		public int VideoAdsServedBlackMarketScreen { get; set; }

		public long LootKeysFirstSpentTime { get; set; }

		[JsonIgnore]
		public ActivityManager ActivityManager { get; set; }

		public int TotalDiamondsBought { get; set; }

		[IgnoreModelProperty]
		public BadgeModel LastCraftedBadge { get; set; }

		[Segmental(Name = "AmountSpent")]
		public double TotalUSDSpent { get; set; }

		public Dictionary<string, TimestampedActionResult> GdprActions { get; set; }

		public long MarkedForDeletion { get; set; }

		[Segmental]
		[JsonIgnore]
		public int CouncilLevel => GetBuildingLevel("Council");

		[Segmental]
		[JsonIgnore]
		public int RadioTentLevel => GetBuildingLevel("RadioTent");

		[Segmental]
		[JsonIgnore]
		public bool IsInCombat
		{
			get
			{
				if (Combat != null)
				{
					return true;
				}
				return false;
			}
		}

		public List<DailyQuest> DailyQuests { get; set; }

		public long LastDailyQuestCreationTime { get; set; }

		public long LastDailyQuestDiscardTime { get; set; }

		public DailyQuestModel DailyQuestManager { get; set; }

		public long LastTradeShopRefreshTime { get; set; }

		public int BoughtTradeCrateSlotAmount { get; set; }

		public List<int> BoughtTradeCrateTimeLimitedOffers { get; set; }

		public List<TradeSlotInfo> CurrentTradeSlots { get; set; }

		public CombatModel Combat
		{
			get
			{
				return combatModel;
			}
			protected set
			{
				if (combatModel == value)
				{
					return;
				}
				if (combatModel != null)
				{
					combatModel.Uninitialize();
					combatModel = null;
					UpdateModelObjects();
					if (base.manager != null)
					{
						base.manager.DeregisterUnreferencedModels();
					}
				}
				combatModel = value;
			}
		}

		public ModelList<TimedBonusModel> TimedBonusModels { get; set; }

		public OutpostModel OutpostModel { get; set; }

		public List<OutpostVisitEntry> AttackOutpostVisitLog { get; set; }

		public List<OutpostVisitEntry> DefenseOutpostVisitLog { get; set; }

		public long LastSeenDefenseLogUtcTime { get; set; }

		public WeeklyChallengeModel WeeklyChallenge { get; set; }

		public ApocalypseWeeklyChallengeModel ApocalypseWeeklyChallenge { get; set; }

		public WeeklySurvivalModel WeeklySurvival { get; set; }

		public RFMGiftManager RFMGiftManager { get; set; }

		public CustomizedBundleManager CustomizedBundleManager { get; set; }

		public RouletteManager RouletteManager { get; set; }

		public RecycleWeaponManager RecycleWeaponManager { get; set; }

		public ThreeDayModel ThreeDayModel { get; set; }

		public WeeklyChallengeClassTeamActivityModel WeeklyChallengeClassTeamActivity { get; set; }

		public NewbieSevenQuestModel NewbieSenvenQuest { get; set; }

		public BundleManagerModel BundleManager { get; set; }

		public TradefairManagerModel TradefairManager { get; set; }

		public GoldShopDefinitionManagerModel GoldShopDefinitionManager { get; set; }

		public string SelectedOutpostTemplateDefinitionId { get; private set; }

		[JsonIgnore]
		private Dictionary<string, RunLocationModel> OutpostTemplateCache { get; set; }

		public int CurrentOutpostSeasonId { get; set; }

		public int PreviousOutpostSeasonId { get; set; }

		public int PreviousSeasonRankingScore { get; set; }

		public bool OutpostSeasonChanged { get; set; }

		public string LastKnownOutpostTierId { get; set; }

		public int CurrentSeasonVersion { get; set; }

		public int GuildSuggestionPopupShownCount { get; set; }

		public long GuildSuggestionPopupLastShownTime { get; set; }

		public GvGSeasonModelPlayer GvGSeasonModelPlayer { get; set; }

		public Dictionary<string, int> TotalAlltimeGvGVpAccumulatedPerGuild { get; set; }

		public CampaignModel CampaignModel { get; set; }

		public DailyLoginCampaignModel DailyLoginCalendar { get; set; }

		public SevenDayLoginManager SevenDayLoginManager { get; set; }

		public ActiveFoundationManager ActiveFoundationManager { get; set; }

		public ReturnActivityManager ReturnActivityManager { get; set; }

		public ActivityIntegrationManager ActivityIntegrationManager { get; set; }

		public SubscriptionManager SubscriptionManager { get; set; }

		public GuildShopModel GuildShopModel { get; set; }

		public PlayerEmblem PlayerEmblem { get; set; }

		public BlackMarket BlackMarket { get; set; }

		public HillTopStore HillTopStore { get; set; }

		public ShareManagerModel ShareManagerModel { get; set; }

		public SurvivalManualManager SurvivalManualManager { get; set; }

		public ModSkillManager ModSkillManager { get; set; }

		[JsonIgnore]
		public int NumNewDefenseLogEntries
		{
			get
			{
				int num = 0;
				if (DefenseOutpostVisitLog != null)
				{
					int num2 = DefenseOutpostVisitLog.Count - 1;
					while (num2 >= 0 && DefenseOutpostVisitLog[num2].UtcTime > LastSeenDefenseLogUtcTime)
					{
						num++;
						num2--;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public bool HasPublishedOutpost
		{
			get
			{
				if (OutpostModel != null)
				{
					return OutpostModel.StoredLevelModel != null;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasStartedOutpostTutorial => OutpostTutorialState != OutpostTutorialState.None;

		[JsonIgnore]
		public bool IsOutpostUnlocked
		{
			get
			{
				if (Camp.GetCouncilLevel() < base.gameEconomyData.ConfigData.OutpostUnlockAtCouncilLevel)
				{
					return HasStartedOutpostTutorial;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsMasterMissionUnlocked => Camp.GetCouncilLevel() >= base.gameEconomyData.ConfigData.ChallangeMasterMissionCouncilLevelUnlock;

		[JsonIgnore]
		public bool IsCraftingAvailable => Camp.GetBuildingLevel("Residence") > 0;

		[JsonIgnore]
		public string LastVisitDebugInfo { get; set; }

		[JsonIgnore]
		public bool AdsCompensationReceived => Blackboard.IsToggleOn("Toggle.ToggleAdsCompensationReceived");

		[JsonIgnore]
		public GridModel Grid
		{
			get
			{
				if (Combat == null)
				{
					return Camp.Grid;
				}
				return Combat.Grid;
			}
		}

		[JsonIgnore]
		public bool HasLootBoxesToOpen
		{
			get
			{
				if (LootBoxesToOpen != null)
				{
					return LootBoxesToOpen.Count > 0;
				}
				return false;
			}
		}

		[JsonIgnore]
		[Segmental]
		public bool HasValidOutpost
		{
			get
			{
				if (OutpostModel != null && OutpostModel.StoredLevelModel != null && OutpostModel.OutpostRunLocation != null && SurvivorContainer.OutpostDefendingSurvivors != null)
				{
					return SurvivorContainer.OutpostDefendingSurvivors.Count == 3;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CombatAutoResolved { get; set; }

		public int RankingScore { get; set; }

		[JsonIgnore]
		public int OutpostPower
		{
			get
			{
				if (OutpostLevel > 0)
				{
					int num = 0;
					for (int i = 0; i < SurvivorContainer.OutpostDefendingSurvivors.Count; i++)
					{
						num += SurvivorContainer.OutpostDefendingSurvivors[i].Level;
					}
					return num + OutpostWalkerPower;
				}
				return 0;
			}
		}

		[JsonIgnore]
		public int OutpostWalkerPower
		{
			get
			{
				if (OutpostLevel == 0 || OutpostModel == null)
				{
					return 0;
				}
				return OutpostModel.GetWalkerPower();
			}
		}

		public long ShieldTimeStamp { get; set; }

		public long LastPvPAttackCompletionUtcTime { get; set; }

		[JsonIgnore]
		public string[] ExcludedMatchMakingTargets { get; set; }

		[JsonIgnore]
		public OutpostSeason CurrentOutpostSeason
		{
			get
			{
				if (base.manager != null && !base.manager.GameEconomyData.ConfigData.DisableOutpostSeasons)
				{
					return base.manager.GameEconomyData.GetOutpostSeasonById(CurrentOutpostSeasonId);
				}
				return null;
			}
		}

		[JsonIgnore]
		public OutpostTier CurrentOutpostTier
		{
			get
			{
				if (base.manager != null && CurrentOutpostSeason != null && !base.manager.GameEconomyData.ConfigData.DisableOutpostSeasons)
				{
					return base.manager.GameEconomyData.GetOutpostInfluenceTier(RankingScore, CurrentOutpostSeason.TierSetId);
				}
				return null;
			}
		}

		[JsonIgnore]
		public int TierAttackWinMultiplier => CurrentOutpostTier?.AttackerWinInfluence ?? 100;

		[JsonIgnore]
		public int TierAttackLossMultiplier => CurrentOutpostTier?.AttackerLossInfluence ?? 100;

		public HashSet<int> IconIndexs { get; private set; }

		public HashSet<int> BorderIndexs { get; private set; }

		public HashSet<int> ColorIndexs { get; private set; }

		[JsonIgnore]
		public int OutpostLevel
		{
			get
			{
				if (Camp == null)
				{
					return 0;
				}
				return Camp.GetBuildingLevel("Outpost");
			}
		}

		[JsonIgnore]
		public int WalkerPitLevel
		{
			get
			{
				if (Camp == null)
				{
					return 0;
				}
				return Camp.GetBuildingLevel("Cage");
			}
		}

		[JsonIgnore]
		private long ProductionHaltedTime
		{
			get
			{
				long num = 0L;
				ConfigData configData = base.manager.GameEconomyData.ConfigData;
				if (Combat != null && Combat.MissionResult == ECombatResult.Successful)
				{
					num += (Combat.IsPvPLootCollected ? configData.OutpostCratesCompletedProductionHalted : 0);
					num += (Combat.IsPvPFlagCollected ? configData.OutpostFlagCompletedProductionHalted : 0);
					num += (Combat.IsPvpDefendersKilled ? configData.OutpostDefendersCompletedProductionHalted : 0);
				}
				return num;
			}
		}

		public List<string> WebShopBuyedBundleIds { get; set; }

		public List<string> WebShopBuyedTradeFairBundleIds { get; set; }

		public List<WebshopBuyedBundleSingularSyncData> WebshopBuyedBundleSingularSyncDatas { get; set; }

		public long WebShopPopupLastFreshTime { get; set; }

		public long WebShopPopupLastTime { get; set; }

		public int WebShopPopupTimes { get; set; }

		public List<string> SubscriptionBuyedBundleIds { get; set; }

		public event Action<CurrencyType, int> OnCurrencySpentEvent;

		public event Action<int> OnWalkersKilledEvent;

		public event Action<int> OnCouncilLevelUpEvent;

		public event Action<ReturnQuestType> OnItemUpgradedEvent;

		public event Action OnMissionCompletedEvent;

		public int GetBuildingLevel(string typeName)
		{
			if (Blackboard == null)
			{
				return 0;
			}
			return Blackboard.GetCounter(BuildingModel.GetBuildingLevelBlackboardKey(typeName, 0));
		}

		public void SetDefenseLogSeen()
		{
			if (DefenseOutpostVisitLog != null && DefenseOutpostVisitLog.Count > 0)
			{
				LastSeenDefenseLogUtcTime = DefenseOutpostVisitLog[DefenseOutpostVisitLog.Count - 1].UtcTime;
			}
		}

		public void SetLevelAndXp(int level, int xp)
		{
			Level = level;
			Xp = xp;
		}

		public void SetSelectedMission(MapMissionParameters parameters)
		{
			SelectedMissionId = parameters.MissionId;
			SelectedMissionDifficulty = parameters.MissionLevel;
			SelectedMissionFlavor = parameters.MissionFlavor;
			SelectedMissionIsDeadly = parameters.IsDeadly;
			SelectedMissionLootTag = parameters.LootTag;
			SelectedMissionRandomSeed = parameters.RandomSeed;
			SelectedSectorId = parameters.MissionSectorId;
		}

		public void ReportMissionStatistics(MissionStatistics combatStatistics, CasualtyReport casualtyReport)
		{
			ECombatResult lastCombatResult = combatStatistics.LastCombatResult;
			MissionStatistics += combatStatistics;
			SurvivorClass survivorClass = ((combatModel.MissionRoster.Count > 0) ? combatModel.MissionRoster[0].SurvivorClass : SurvivorClass.None);
			EquipmentCategory equipmentCategory = ((combatModel.MissionRoster.Count > 0 && combatModel.MissionRoster[0].GetWeaponEquipment() != null) ? combatModel.MissionRoster[0].GetWeaponEquipment().Definition.Category : EquipmentCategory.None);
			if (equipmentCategory == EquipmentCategory.None && combatModel.MissionRoster.Count > 0)
			{
				equipmentCategory = ((!combatModel.MissionRoster[0].IsMeleeClass) ? EquipmentCategory.RangeWeapon : EquipmentCategory.MeleeWeapon);
			}
			foreach (SurvivorModel item in combatModel.MissionRoster)
			{
				item.Statistics.AddMissionPlayed();
				SurvivorClass survivorClass2 = item.SurvivorClass;
				EquipmentCategory equipmentCategory2 = ((item.GetWeaponEquipment() != null) ? item.GetWeaponEquipment().Definition.Category : EquipmentCategory.None);
				if (equipmentCategory2 == EquipmentCategory.None)
				{
					equipmentCategory2 = ((!item.IsMeleeClass) ? EquipmentCategory.RangeWeapon : EquipmentCategory.MeleeWeapon);
				}
				if (survivorClass2 != survivorClass)
				{
					survivorClass = SurvivorClass.None;
				}
				if (equipmentCategory2 != equipmentCategory)
				{
					equipmentCategory = EquipmentCategory.None;
				}
			}
			if (lastCombatResult != ECombatResult.Successful || casualtyReport == null)
			{
				return;
			}
			if (survivorClass != SurvivorClass.None && base.manager.Player.Tutorial.StaticTutorialComplete)
			{
				Blackboard.IncreaseCounter(BlackboardModel.GetSameClassMissionCompleteKey(survivorClass));
			}
			if (equipmentCategory != EquipmentCategory.None && base.manager.Player.Tutorial.StaticTutorialComplete)
			{
				Blackboard.IncreaseCounter(BlackboardModel.GetSameEquipmentTypeMissionCompleteKey(equipmentCategory));
			}
			if (casualtyReport.NoDamage)
			{
				Blackboard.IncreaseCounter("Counter.NumberMissionCompletedNoDamage");
			}
			if (casualtyReport.NoStruggle)
			{
				Blackboard.IncreaseCounter("Counter.NumberMissionCompletedNoStruggle");
			}
			if (MapContainerModel.AttackTargetMissionModel != null && base.manager.Player.Tutorial.StaticTutorialComplete)
			{
				if (MapContainerModel.AttackTargetMissionModel.IsGrindMission)
				{
					Blackboard.IncreaseCounter("Counter.NumberGrindMissionCompleted");
				}
				else if (MapContainerModel.AttackTargetMissionModel.IsInWeeklyChallenge || MapContainerModel.AttackTargetMissionModel.IsInApocalyptiWeeklyChallenge)
				{
					Blackboard.IncreaseCounter("Counter.NumberChallengeMissionCompleted");
				}
				else if (MapContainerModel.AttackTargetMissionModel.IsInWeeklySurvival)
				{
					Blackboard.IncreaseCounter("Counter.NumberSurvivalMissionCompleted");
				}
				else
				{
					Blackboard.IncreaseCounter("Counter.NumberStoryMissionCompleted");
				}
			}
			if (combatStatistics.WalkersKilled == 0)
			{
				Blackboard.IncreaseCounter("Counter.NumberMissionsCompletedNoWalkerKills");
			}
		}

		public PlayerRandomChanceResult RollDice(RollDiceType rollType, FixedPoint successProbability)
		{
			return RollDice(rollType, successProbability, 0.0);
		}

		public PlayerRandomChanceResult RollDice(RollDiceType rollType, FixedPoint successProbability, FixedPoint successProbabilityExtension)
		{
			if (successProbability <= 0L)
			{
				successProbability = 0.0;
			}
			if (successProbabilityExtension <= 0L)
			{
				successProbabilityExtension = 0.0;
			}
			FixedPoint fixedPoint = PlayerRandom.Next();
			FixedPoint fixedPoint2 = successProbability * (1.0 + successProbabilityExtension);
			if (rollType == RollDiceType.DeathsDoorPursuit)
			{
				base.manager.Debug.Log($"[DeathsDoor] roll={fixedPoint} min=0 extendedLimit={fixedPoint2}");
			}
			if (fixedPoint <= fixedPoint2)
			{
				if (fixedPoint > successProbability)
				{
					if (base.manager.CurrentCommandLogEntry != null)
					{
						base.manager.CurrentCommandLogEntry.RollDice(successProbability, successProbabilityExtension, fixedPoint, fixedPoint2, PlayerRandomChanceResult.SuccessDueToExtension, rollType);
					}
					return PlayerRandomChanceResult.SuccessDueToExtension;
				}
				if (base.manager.CurrentCommandLogEntry != null)
				{
					base.manager.CurrentCommandLogEntry.RollDice(successProbability, successProbabilityExtension, fixedPoint, fixedPoint2, PlayerRandomChanceResult.Success, rollType);
				}
				return PlayerRandomChanceResult.Success;
			}
			if (base.manager.CurrentCommandLogEntry != null)
			{
				base.manager.CurrentCommandLogEntry.RollDice(successProbability, successProbabilityExtension, fixedPoint, fixedPoint2, PlayerRandomChanceResult.Failed, rollType);
			}
			return PlayerRandomChanceResult.Failed;
		}

		public PlayerRandomChanceResult RollDice(RollDiceType rollType, int chance, int chanceExtension = 0)
		{
			int num = PlayerRandom.Next(100) + 1;
			if (num <= chance)
			{
				if (base.manager.CurrentCommandLogEntry != null)
				{
					base.manager.CurrentCommandLogEntry.RollDice(chance, chanceExtension, num, PlayerRandomChanceResult.Success, rollType);
				}
				return PlayerRandomChanceResult.Success;
			}
			if (num <= chance + chanceExtension)
			{
				if (base.manager.CurrentCommandLogEntry != null)
				{
					base.manager.CurrentCommandLogEntry.RollDice(chance, chanceExtension, num, PlayerRandomChanceResult.SuccessDueToExtension, rollType);
				}
				return PlayerRandomChanceResult.SuccessDueToExtension;
			}
			if (base.manager.CurrentCommandLogEntry != null)
			{
				base.manager.CurrentCommandLogEntry.RollDice(chance, chanceExtension, num, PlayerRandomChanceResult.Failed, rollType);
			}
			return PlayerRandomChanceResult.Failed;
		}

		public PlayerModel()
		{
			Currencies = new ModelList<CurrencyModel>();
			Equipment = new EquipmentModel();
			MissionStatistics = new MissionStatistics();
			DisallowedOffers = new List<BundleOfferPurchaseEntry>();
			SupportModels = new ModelList<SupportModel>();
			TeamPresetsManager = new TeamPresetsManager();
			BattlePass = new BattlePassModel();
			BeginnerBattlePassInfo = new BeginnerBattlePassInfo();
			AutoScrapmentEquipment = new ModelList<EquipmentItemModel>();
		}

		public override bool IsValid()
		{
			return Camp != null;
		}

		public override void Initialize()
		{
			base.Initialize();
			DebugTWD.Log("Initialize PlayerModel", DebugType.Load);
			if (DevRandomSeed != -1)
			{
				PlayerRandomSeed = DevRandomSeed;
			}
			else
			{
				PlayerRandomSeed = 1321616566;
			}
			PlayerRandom = new ModelRandom(PlayerRandomSeed);
			AttackOutpostVisitLog = new List<OutpostVisitEntry>();
			DefenseOutpostVisitLog = new List<OutpostVisitEntry>();
			PendingIAPs = new List<PendingPurchaseInfo>();
			DailyQuestManager = new DailyQuestModel();
			DailyQuestManager.SetManager(base.manager);
			DailyQuestManager.Initialize();
			Blackboard = new BlackboardModel();
			Blackboard.SetManager(base.manager);
			Blackboard.Initialize();
			EndlessModeManager = new EndlessModeManagerModel();
			EndlessModeManager.SetManager(base.manager);
			EndlessModeManager.Initialize();
			Level = 1;
			Xp = 0;
			for (int i = 0; i < (int)CurrencyType.Count; i++)
			{
				CurrencyModel currencyModel = new CurrencyModel((CurrencyType)i);
				currencyModel.SetManager(base.manager);
				Currencies.Add(currencyModel);
			}
			UpdateCurrenciesCapacity();
			GetCurrency(CurrencyType.ReplayToken).CanOverflowOnBuyDiamonds = false;
			GetCurrency(CurrencyType.GvGGas).CanOverflowOnBuyDiamonds = true;
			GetCurrency(CurrencyType.BattlePass).CanOverflowOnBuyDiamonds = false;
			Camp = new CampModel();
			Camp.Player = this;
			Camp.SetManager(base.manager);
			Camp.Initialize();
			CampMover = new CampMoverModel();
			CampMover.SetManager(base.manager);
			CampMover.Initialize();
			PhoneCall = new PhoneCallModel();
			PhoneCall.SetManager(base.manager);
			PhoneCall.Initialize();
			Tutorial = new TutorialModel();
			Tutorial.SetManager(base.Manager);
			Tutorial.Initialize();
			SurvivorContainer = new SurvivorContainerModel();
			SurvivorContainer.SetManager(base.manager);
			SurvivorContainer.Initialize();
			LootManager = new LootManagerModel();
			LootManager.SetManager(base.manager);
			LootManager.Initialize();
			Equipment.SetManager(base.Manager);
			Equipment.Initialize();
			EquipTokenContainer = new EquipTokenContainerModel();
			EquipTokenContainer.SetManager(base.Manager);
			EquipTokenContainer.Initialize();
			PlayerAttributeContainer = new PlayerAttributeContainerModel();
			PlayerAttributeContainer.SetManager(base.Manager);
			PlayerAttributeContainer.Initialize();
			MapContainerModel = new MapContainerModel();
			MapContainerModel.SetManager(base.manager);
			MapContainerModel.Initialize();
			LootBoxesToOpen = new ModelList<LootEntry>();
			BoughtIAPs = new List<string>();
			BoughtIAPsQuantity = new List<int>();
			RFMGiftManager = new RFMGiftManager();
			RFMGiftManager.SetManager(base.manager);
			RFMGiftManager.Initialize();
			CustomizedBundleManager = new CustomizedBundleManager();
			CustomizedBundleManager.SetManager(base.manager);
			CustomizedBundleManager.Initialize();
			RouletteManager = new RouletteManager();
			RouletteManager.SetManager(base.manager);
			RouletteManager.Initialize();
			RecycleWeaponManager = new RecycleWeaponManager();
			RecycleWeaponManager.SetManager(base.manager);
			RecycleWeaponManager.Initialize();
			NewbieSenvenQuest = new NewbieSevenQuestModel();
			NewbieSenvenQuest.SetManager(base.manager);
			NewbieSenvenQuest.Initialize();
			ThreeDayModel = new ThreeDayModel();
			ThreeDayModel.SetManager(base.manager);
			ThreeDayModel.Initialize();
			WeeklyChallenge = new WeeklyChallengeModel();
			WeeklyChallenge.SetManager(base.manager);
			WeeklyChallenge.Initialize();
			ApocalypseWeeklyChallenge = new ApocalypseWeeklyChallengeModel();
			ApocalypseWeeklyChallenge.SetManager(base.manager);
			ApocalypseWeeklyChallenge.Initialize();
			WeeklyChallengeClassTeamActivity = new WeeklyChallengeClassTeamActivityModel();
			WeeklyChallengeClassTeamActivity.SetManager(base.manager);
			WeeklyChallengeClassTeamActivity.Initialize();
			SavedSurvivalMissionData = new SurvivalSavedMissionModel();
			SavedSurvivalMissionData.SetManager(base.manager);
			SavedSurvivalMissionData.Initialize();
			WeeklySurvival = new WeeklySurvivalModel();
			WeeklySurvival.SetManager(base.manager);
			WeeklySurvival.Initialize();
			BundleManager = new BundleManagerModel();
			BundleManager.SetManager(base.manager);
			BundleManager.Initialize();
			OutpostModel = new OutpostModel();
			OutpostModel.SetManager(base.manager);
			OutpostModel.Initialize();
			CampaignModel = new CampaignModel();
			CampaignModel.SetManager(base.manager);
			CampaignModel.Initialize();
			DailyLoginCalendar = new DailyLoginCampaignModel();
			DailyLoginCalendar.SetManager(base.manager);
			DailyLoginCalendar.Initialize();
			SevenDayLoginManager = new SevenDayLoginManager();
			SevenDayLoginManager.SetManager(base.manager);
			SevenDayLoginManager.Initialize();
			ActiveFoundationManager = new ActiveFoundationManager();
			ActiveFoundationManager.SetManager(base.manager);
			ActiveFoundationManager.Initialize();
			ReturnActivityManager = new ReturnActivityManager();
			ReturnActivityManager.SetManager(base.manager);
			ReturnActivityManager.Initialize();
			ActivityIntegrationManager = new ActivityIntegrationManager();
			ActivityIntegrationManager.SetManager(base.manager);
			ActivityIntegrationManager.Initialize();
			SubscriptionManager = new SubscriptionManager();
			SubscriptionManager.SetManager(base.manager);
			SubscriptionManager.Initialize();
			GuildShopModel = new GuildShopModel();
			GuildShopModel.SetManager(base.manager);
			GuildShopModel.Initialize();
			BlackMarket = new BlackMarket();
			BlackMarket.SetManager(base.manager);
			BlackMarket.Initialize();
			HillTopStore = new HillTopStore();
			HillTopStore.SetManager(base.manager);
			HillTopStore.Initialize();
			ShareManagerModel = new ShareManagerModel();
			ShareManagerModel.SetManager(base.manager);
			ShareManagerModel.Initialize();
			SurvivalManualManager = new SurvivalManualManager();
			SurvivalManualManager.SetManager(base.manager);
			SurvivalManualManager.Initialize();
			ModSkillManager = new ModSkillManager();
			ModSkillManager.SetManager(base.manager);
			ModSkillManager.Initialize();
			CurrentOutpostSeasonId = -1;
			PreviousOutpostSeasonId = -1;
			PendingGuildGiftsToOpen = new List<GuildGift>();
			PendingGuildGiftsLootToOpen = new List<LootEntry>();
			NewsLetterItemsRead = new List<string>();
			RankingScore = base.manager.GameEconomyData.ConfigData.InitialRankingScore;
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in base.gameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			MapContainerModel.SpawnSeasonEpisodes();
			MapContainerModel.SpawnEndlessModeMissions();
			ScrappedExcessItems = false;
			GvGSeasonModelPlayer = new GvGSeasonModelPlayer();
			GvGSeasonModelPlayer.SetManager(base.manager);
			GvGSeasonModelPlayer.Initialize();
			GdprActions = new Dictionary<string, TimestampedActionResult>();
			PlayerEmblem = new PlayerEmblem();
			TotalAlltimeGvGVpAccumulatedPerGuild = new Dictionary<string, int>();
		}

		public void AddLootBoxToOpen(LootEntry lootEntry)
		{
			if (LootBoxesToOpen == null)
			{
				LootBoxesToOpen = new ModelList<LootEntry>();
			}
			LootBoxesToOpen.Add(lootEntry);
		}

		public LootEntry GetAndRemoveLootBoxToOpen()
		{
			LootEntry lootEntry = null;
			if (LootBoxesToOpen.Count > 0)
			{
				lootEntry = LootBoxesToOpen[0];
				LootManager.GiveLoot(lootEntry);
				LootBoxesToOpen.RemoveAt(0);
			}
			return lootEntry;
		}

		public int GetRankingScoreChange(int attackingPlayerRankingScore, int defendingPlayerRankingScore)
		{
			int val = (defendingPlayerRankingScore - attackingPlayerRankingScore + base.manager.GameEconomyData.ConfigData.RankingScoreMaxDifference) / 2;
			val = Math.Min(val, base.manager.GameEconomyData.ConfigData.RankingScoreMaxDifference);
			val = Math.Max(0, val);
			return Math.Min(Math.Min(val * base.manager.GameEconomyData.ConfigData.RankingScoreResultChangePercentage / 100, defendingPlayerRankingScore - base.manager.GameEconomyData.ConfigData.MinRankingScore), base.manager.GameEconomyData.ConfigData.MaxRankingScore - attackingPlayerRankingScore);
		}

		public int GetOutpostTutorialTradeGoodsReward()
		{
			return base.manager.GameEconomyData.ConfigData.OutpostTutorialResourceReward;
		}

		public int GetOutpostTutorialInfluenceReward()
		{
			return base.manager.GameEconomyData.ConfigData.OutpostTutorialInfluenceReward;
		}

		public void SetRankingScore(int newScore)
		{
			int num = Math.Max(Math.Min(newScore, base.manager.GameEconomyData.ConfigData.MaxRankingScore), base.manager.GameEconomyData.ConfigData.MinRankingScore);
			if (RankingScore != num)
			{
				RankingScore = num;
			}
		}

		public long GetShieldTimeMillisLeft(long timeNow)
		{
			long num = ShieldTimeStamp - timeNow;
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		public void SetOutpostShield(long timestamp)
		{
			ShieldTimeStamp = timestamp + base.gameEconomyData.ConfigData.OutpostDefeatedShieldDuration * 1000;
		}

		public void SetOutpostShieldDebug(long whenShieldExpires)
		{
			ShieldTimeStamp = whenShieldExpires;
		}

		private void ResetOutpostShield()
		{
			if (DefenseOutpostVisitLog != null && DefenseOutpostVisitLog.Count > 0)
			{
				ShieldTimeStamp = DefenseOutpostVisitLog[DefenseOutpostVisitLog.Count - 1].UtcTime;
			}
		}

		public void OutpostDefenseLogDebug()
		{
			OutpostVisitEntry outpostVisitEntry = new OutpostVisitEntry
			{
				EntryType = OutpostVisitEntryType.Defended,
				OtherPlayerHashedId = HashedId,
				UtcTime = UtcTimeStamp,
				OtherPlayerName = "Debug!",
				OtherPlayerLevel = Level,
				OtherOutpostLevel = Camp.GetBuildingLevel("Outpost"),
				ResourcesStolen = 5377,
				RankingScoreChange = 150,
				CombatResult = ((PlayerRandom.GetRandomInRange(0, 9) % 2 == 0) ? ECombatResult.Successful : ECombatResult.Failed),
				OutpostVisitId = ModelHelpers.MD5Sum(HashedId + HashedId + UtcTimeStamp)
			};
			int count = SurvivorContainer.CombatSurvivors.Count;
			outpostVisitEntry.OtherSurvivorLevels = new int[count];
			outpostVisitEntry.OtherSurvivorClasses = new SurvivorClass[count];
			outpostVisitEntry.OtherSurvivorRarityLevels = new int[count];
			outpostVisitEntry.OtherSurvivorDefeated = new bool[count];
			outpostVisitEntry.SurvivorLevels = new int[count];
			outpostVisitEntry.SurvivorClasses = new SurvivorClass[count];
			outpostVisitEntry.SurvivorRarityLevels = new int[count];
			outpostVisitEntry.SurvivorDefeated = new bool[count];
			for (int i = 0; i < count; i++)
			{
				outpostVisitEntry.OtherSurvivorLevels[i] = SurvivorContainer.CombatSurvivors[i].Level;
				outpostVisitEntry.OtherSurvivorClasses[i] = SurvivorContainer.CombatSurvivors[i].SurvivorClass;
				outpostVisitEntry.OtherSurvivorRarityLevels[i] = SurvivorContainer.CombatSurvivors[i].SurvivorRarityLevel;
				outpostVisitEntry.OtherSurvivorDefeated[i] = PlayerRandom.GetRandomInRange(0, 9) % 2 == 0;
				outpostVisitEntry.SurvivorLevels[i] = SurvivorContainer.CombatSurvivors[i].Level;
				outpostVisitEntry.SurvivorClasses[i] = SurvivorContainer.CombatSurvivors[i].SurvivorClass;
				outpostVisitEntry.SurvivorRarityLevels[i] = SurvivorContainer.CombatSurvivors[i].SurvivorRarityLevel;
				outpostVisitEntry.SurvivorDefeated[i] = PlayerRandom.GetRandomInRange(0, 9) % 2 == 0;
			}
			outpostVisitEntry.MissionType = ((PlayerRandom.GetRandomInRange(0, 9) % 2 != 0) ? PvPMissionType.PVPMultiLoot : PvPMissionType.PVPMultiFlag);
			outpostVisitEntry.FirstObjectiveCompleted = outpostVisitEntry.CombatResult == ECombatResult.Successful;
			outpostVisitEntry.SecondObjectiveCompleted = outpostVisitEntry.CombatResult == ECombatResult.Successful && PlayerRandom.GetRandomInRange(0, 9) % 2 == 0;
			bool defendersObjectiveCompleted = true;
			for (int j = 0; j < outpostVisitEntry.SurvivorDefeated.Length; j++)
			{
				if (!outpostVisitEntry.SurvivorDefeated[j])
				{
					defendersObjectiveCompleted = false;
					break;
				}
			}
			outpostVisitEntry.DefendersObjectiveCompleted = defendersObjectiveCompleted;
			AddDefenseOutpostVisitLog(outpostVisitEntry);
		}

		public void OutpostAttackLogDebug()
		{
			OutpostVisitEntry outpostVisitEntry = new OutpostVisitEntry
			{
				EntryType = OutpostVisitEntryType.Attacked,
				OtherPlayerHashedId = HashedId,
				UtcTime = UtcTimeStamp,
				OtherPlayerName = "Debug!",
				OtherPlayerLevel = Level,
				OtherOutpostLevel = Camp.GetBuildingLevel("Outpost"),
				ResourcesStolen = 5377,
				RankingScoreChange = 150,
				CombatResult = ((PlayerRandom.GetRandomInRange(0, 9) % 2 == 0) ? ECombatResult.Successful : ECombatResult.Failed),
				OutpostVisitId = ModelHelpers.MD5Sum(HashedId + HashedId + UtcTimeStamp)
			};
			int count = SurvivorContainer.CombatSurvivors.Count;
			outpostVisitEntry.OtherSurvivorLevels = new int[count];
			outpostVisitEntry.OtherSurvivorClasses = new SurvivorClass[count];
			outpostVisitEntry.OtherSurvivorRarityLevels = new int[count];
			outpostVisitEntry.OtherSurvivorDefeated = new bool[count];
			outpostVisitEntry.SurvivorLevels = new int[count];
			outpostVisitEntry.SurvivorClasses = new SurvivorClass[count];
			outpostVisitEntry.SurvivorRarityLevels = new int[count];
			outpostVisitEntry.SurvivorDefeated = new bool[count];
			for (int i = 0; i < count; i++)
			{
				outpostVisitEntry.OtherSurvivorLevels[i] = SurvivorContainer.CombatSurvivors[i].Level;
				outpostVisitEntry.OtherSurvivorClasses[i] = SurvivorContainer.CombatSurvivors[i].SurvivorClass;
				outpostVisitEntry.OtherSurvivorRarityLevels[i] = SurvivorContainer.CombatSurvivors[i].SurvivorRarityLevel;
				outpostVisitEntry.OtherSurvivorDefeated[i] = PlayerRandom.GetRandomInRange(0, 9) % 2 == 0;
				outpostVisitEntry.SurvivorLevels[i] = SurvivorContainer.CombatSurvivors[i].Level;
				outpostVisitEntry.SurvivorClasses[i] = SurvivorContainer.CombatSurvivors[i].SurvivorClass;
				outpostVisitEntry.SurvivorRarityLevels[i] = SurvivorContainer.CombatSurvivors[i].SurvivorRarityLevel;
				outpostVisitEntry.SurvivorDefeated[i] = PlayerRandom.GetRandomInRange(0, 9) % 2 == 0;
			}
			outpostVisitEntry.MissionType = PvPMissionType.PVPMultiFlag;
			outpostVisitEntry.FirstObjectiveCompleted = outpostVisitEntry.CombatResult == ECombatResult.Successful;
			outpostVisitEntry.SecondObjectiveCompleted = outpostVisitEntry.CombatResult == ECombatResult.Successful && PlayerRandom.GetRandomInRange(0, 9) % 2 == 0;
			bool defendersObjectiveCompleted = true;
			for (int j = 0; j < outpostVisitEntry.OtherSurvivorDefeated.Length; j++)
			{
				if (!outpostVisitEntry.OtherSurvivorDefeated[j])
				{
					defendersObjectiveCompleted = false;
					break;
				}
			}
			outpostVisitEntry.DefendersObjectiveCompleted = defendersObjectiveCompleted;
			AddAttackOutpostVisitLog(outpostVisitEntry);
		}

		public OutpostTier GetOutpostTier(PlayerModel player)
		{
			if (base.manager != null && CurrentOutpostSeason != null && !base.manager.GameEconomyData.ConfigData.DisableOutpostSeasons)
			{
				return base.manager.GameEconomyData.GetOutpostInfluenceTier(player.RankingScore, CurrentOutpostSeason.TierSetId);
			}
			return null;
		}

		public int GetTierDefenderWinMultiplier(PlayerModel player)
		{
			return GetOutpostTier(player)?.DefenderWinInfluence ?? 100;
		}

		public int GetTierDefenderLossMultiplier(PlayerModel player)
		{
			return GetOutpostTier(player)?.DefenderLossInfluence ?? 100;
		}

		private void StartOutpostSeason(OutpostSeason season)
		{
			if (season != null)
			{
				CurrentOutpostSeasonId = season.Id;
				base.manager.UpdateOutpostLeaderboardEntry();
			}
		}

		private void EndOutpostSeason()
		{
			OutpostSeason outpostSeasonById = base.gameEconomyData.GetOutpostSeasonById(CurrentOutpostSeasonId);
			if (outpostSeasonById == null)
			{
				return;
			}
			OutpostTier outpostInfluenceTier = base.gameEconomyData.GetOutpostInfluenceTier(RankingScore, outpostSeasonById.TierSetId);
			if (outpostInfluenceTier != null)
			{
				Rewards rewards = outpostInfluenceTier.GetRewards();
				int value = base.manager.Player.GetCurrency(CurrencyType.Outpost).Value;
				int totalCurrencyRewardAmount = rewards.GetTotalCurrencyRewardAmount(CurrencyType.Outpost);
				rewards?.Give(base.manager);
				int value2 = base.manager.Player.GetCurrency(CurrencyType.Outpost).Value;
				Blackboard.IncreaseCounter(BlackboardModel.GetOutpostInfluenceTierCounterKey(outpostInfluenceTier.Id));
				PreviousSeasonRankingScore = RankingScore;
				if (outpostInfluenceTier.ResetInfluence >= 0)
				{
					SetRankingScore(outpostInfluenceTier.ResetInfluence);
				}
				base.manager.Metrics.AddFind().AddResources(CurrencyType.Outpost, totalCurrencyRewardAmount, value2 - value).AddPvpCycle()
					.AddEnd()
					.Send();
			}
			PreviousOutpostSeasonId = CurrentOutpostSeasonId;
			CurrentOutpostSeasonId = -1;
			OutpostSeasonChanged = true;
		}

		public bool HasOutpostSeasonChanged()
		{
			if (!base.gameEconomyData.ConfigData.DisableOutpostSeasons && HasValidOutpost)
			{
				int num = base.gameEconomyData.GetOutpostSeason(TimeStamp)?.Id ?? (-1);
				return CurrentOutpostSeasonId < num;
			}
			return false;
		}

		private void CheckSkippedSeasons(long timeStamp)
		{
			List<OutpostSeason> outpostSeasons = base.gameEconomyData.GetOutpostSeasons(PreviousOutpostSeasonId, timeStamp);
			if (outpostSeasons == null)
			{
				return;
			}
			for (int i = 0; i < outpostSeasons.Count; i++)
			{
				OutpostSeason outpostSeason = outpostSeasons[i];
				OutpostTier outpostInfluenceTier = base.gameEconomyData.GetOutpostInfluenceTier(RankingScore, outpostSeason.TierSetId);
				if (outpostInfluenceTier != null && outpostInfluenceTier.ResetInfluence >= 0)
				{
					SetRankingScore(outpostInfluenceTier.ResetInfluence);
				}
			}
		}

		public bool UpdateOutpostSeason()
		{
			if (base.gameEconomyData.ConfigData.DisableOutpostSeasons)
			{
				return false;
			}
			if (HasValidOutpost)
			{
				OutpostSeason outpostSeason = base.gameEconomyData.GetOutpostSeason(TimeStamp);
				int num = outpostSeason?.Id ?? (-1);
				if (CurrentOutpostSeasonId < num)
				{
					EndOutpostSeason();
					CheckSkippedSeasons(TimeStamp);
					if (outpostSeason != null)
					{
						StartOutpostSeason(outpostSeason);
					}
					return true;
				}
			}
			return false;
		}

		public bool UpdateOutpostSeasonAtAttackTime(long timeOfAttack)
		{
			if (HasValidOutpost)
			{
				OutpostSeason outpostSeason = base.gameEconomyData.GetOutpostSeason(timeOfAttack);
				int num = outpostSeason?.Id ?? (-1);
				if (CurrentOutpostSeasonId < num)
				{
					EndOutpostSeason();
					CheckSkippedSeasons(timeOfAttack);
					if (outpostSeason != null && !base.gameEconomyData.ConfigData.DisableOutpostSeasons)
					{
						StartOutpostSeason(outpostSeason);
					}
					return true;
				}
			}
			return false;
		}

		public void ReseedRandom()
		{
			int state = PlayerRandom.State;
			PlayerRandom = new ModelRandom(state);
		}

		public override void Start()
		{
			ActivityManager = new ActivityManager(base.manager);
			if (BoughtIAPs == null)
			{
				BoughtIAPs = new List<string>();
			}
			if (BoughtIAPsQuantity == null)
			{
				BoughtIAPsQuantity = new List<int>();
			}
			if (TimedBonusModels == null)
			{
				TimedBonusModels = new ModelList<TimedBonusModel>();
				TimedBonusModels.SetManager(base.manager);
				TimedBonusModels.Initialize();
			}
			if (TradefairManager == null)
			{
				TradefairManager = new TradefairManagerModel();
				TradefairManager.SetManager(base.manager);
				TradefairManager.Initialize();
			}
			if (GoldShopDefinitionManager == null)
			{
				GoldShopDefinitionManager = new GoldShopDefinitionManagerModel();
				GoldShopDefinitionManager.SetManager(base.manager);
				GoldShopDefinitionManager.Initialize();
			}
			if (EquipPrizeWheelModel == null)
			{
				EquipPrizeWheelModel = new EquipPrizeWheelModel();
				EquipPrizeWheelModel.SetManager(base.manager);
				EquipPrizeWheelModel.Initialize();
			}
			if (CombatBackups == null)
			{
				CombatBackups = new ModelList<CombatBackup>();
				CombatBackups.SetManager(base.manager);
				CombatBackups.Initialize();
			}
			if (OutpostTemplateCache == null)
			{
				OutpostTemplateCache = new Dictionary<string, RunLocationModel>();
			}
			if (ApocalypseWeeklyChallenge == null)
			{
				ApocalypseWeeklyChallenge = new ApocalypseWeeklyChallengeModel();
				ApocalypseWeeklyChallenge.SetManager(base.manager);
				ApocalypseWeeklyChallenge.Initialize();
			}
			if (WeeklyChallengeClassTeamActivity == null)
			{
				WeeklyChallengeClassTeamActivity = new WeeklyChallengeClassTeamActivityModel();
				WeeklyChallengeClassTeamActivity.SetManager(base.manager);
				WeeklyChallengeClassTeamActivity.Initialize();
			}
			if (RFMGiftManager == null)
			{
				RFMGiftManager = new RFMGiftManager();
				RFMGiftManager.SetManager(base.manager);
				RFMGiftManager.Initialize();
			}
			if (CustomizedBundleManager == null)
			{
				CustomizedBundleManager = new CustomizedBundleManager();
				CustomizedBundleManager.SetManager(base.manager);
				CustomizedBundleManager.Initialize();
			}
			if (RouletteManager == null)
			{
				RouletteManager = new RouletteManager();
				RouletteManager.SetManager(base.manager);
				RouletteManager.Initialize();
			}
			if (RecycleWeaponManager == null)
			{
				RecycleWeaponManager = new RecycleWeaponManager();
				RecycleWeaponManager.SetManager(base.manager);
				RecycleWeaponManager.Initialize();
			}
			if (ThreeDayModel == null)
			{
				ThreeDayModel = new ThreeDayModel();
				ThreeDayModel.SetManager(base.manager);
				ThreeDayModel.Initialize();
			}
			if (ReturnActivityManager == null)
			{
				ReturnActivityManager = new ReturnActivityManager();
				ReturnActivityManager.LastLoginTimestamp = UtcTimeStamp;
				ReturnActivityManager.SetManager(base.manager);
				ReturnActivityManager.Initialize();
			}
			if (NewbieSenvenQuest == null)
			{
				NewbieSenvenQuest = new NewbieSevenQuestModel();
				NewbieSenvenQuest.SetManager(base.manager);
				NewbieSenvenQuest.Initialize();
			}
			if (NewsLetterItemsInteracted == null)
			{
				NewsLetterItemsInteracted = new List<string>();
			}
			if (RedeemedCodes == null)
			{
				RedeemedCodes = new List<string>();
			}
			if (RedeemedDeeplinks == null)
			{
				RedeemedDeeplinks = new List<string>();
			}
			if (SurvivalManualManager == null)
			{
				SurvivalManualManager = new SurvivalManualManager();
				SurvivalManualManager.SetManager(base.manager);
				SurvivalManualManager.Initialize();
			}
			if (ModSkillManager == null)
			{
				ModSkillManager = new ModSkillManager();
				ModSkillManager.SetManager(base.manager);
				ModSkillManager.Initialize();
			}
			InitializeSupportModels();
			if (!IsLoadDataManager)
			{
				AchievementManager = new AchievementManager(this);
				AchievementManager.CheckAchievements();
			}
			else
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				DebugTWD.Log("Ignore Initialize Achievements", DebugType.System);
			}
			AbilityManager = new AbilityManagerModel();
			AbilityManager.SetManager(base.manager);
			AbilityManager.Initialize();
			if (EquipTokenContainer == null)
			{
				EquipTokenContainer = new EquipTokenContainerModel();
				EquipTokenContainer.SetManager(base.manager);
				EquipTokenContainer.Initialize();
			}
			if (PlayerAttributeContainer == null)
			{
				PlayerAttributeContainer = new PlayerAttributeContainerModel();
				PlayerAttributeContainer.SetManager(base.manager);
				PlayerAttributeContainer.Initialize();
			}
			if (!IsLoadDataManager)
			{
				Camp.Player = this;
				Camp.Changed += OnCampChange;
				Camp.Changed += Equipment.OnCouncilBuildingChange;
			}
			else
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				DebugTWD.Log("Ignore Initialize Camp", DebugType.System);
			}

			GetCurrency(CurrencyType.ReplayToken)?.SetRechargeTime(ActivityManager.GetReplayTokensRechargeSpeed(base.gameEconomyData.ConfigData));
			GetCurrency(CurrencyType.GvGGas)?.SetRechargeTime(0L);
			GetCurrency(CurrencyType.BattlePass)?.SetRechargeTime(0L);
			int count = Currencies.Count;
			for (int i = 0; i < count; i++)
			{
				Currencies[i].Changed += OnCurrencyChange;
			}
			CurrencyModel currency = GetCurrency(CurrencyType.CampaignToken);
			if (currency != null)
			{
				currency.Changed += CampaignModel.OnTokensChanged;
			}
			CurrencyModel currency2 = GetCurrency(CurrencyType.Diamonds);
			if (currency2 != null)
			{
				currency2.Changed += OnCurrencyConvertToDiamonds;
			}
			for (int j = 0; j < Camp.Buildings.Count; j++)
			{
				Camp.Buildings[j].Changed += OnBuildingChange;
			}
			if (base.gameEconomyData.MissionSpawnPointData != null)
			{
				foreach (MissionSpawnPointGroup missionSpawnPointGroup2 in base.gameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
				{
					MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup2);
				}
			}
			MapContainerModel.CheckOutpostTutorialGroupInstances();
			if (base.manager.GameEconomyData.ConfigData.FakeChallengesInterval != 0 && WeeklyChallenge.Id != 0 && WeeklyChallenge.CurrentDefinition == null)
			{
				WeeklyChallenge.Reset(0);
				ApocalypseWeeklyChallenge.Reset(0);
			}
			if (base.manager.GameEconomyData.ConfigData.FakeSurvivalInterval != 0 && (WeeklySurvival == null || (WeeklySurvival.Id != 0 && WeeklySurvival.CurrentDefinition == null)))
			{
				WeeklySurvival.ResetCurrentToDifficultySelection();
			}
			base.Start();
			if (Combat == null)
			{
				SurvivorCombatCleanup();
			}
			if (PendingIAPs.Count > 4)
			{
				base.Debug.LogWarning("Accumulated pending IAP receipts, total " + PendingIAPs.Count + " receipts.");
			}
			ClearExpiredGifts();
			GuildModel guildModel = GuildModel;
			if (guildModel != null && guildModel.Created.Year < 2015)
			{
				base.Debug.LogWarning("MODEL_FATAL: guildmodel time is weird " + guildModel.Created.ToUniversalTime().ToString() + " guildId = " + guildModel.Id);
			}
			if (LastKnownOutpostTierId == null)
			{
				LastKnownOutpostTierId = "";
			}
			if (ShieldTimeStamp == 0L)
			{
				ShieldTimeStamp = UtcTimeStamp;
			}
			if (CurrentTradeSlots != null)
			{
				for (int k = 0; k < CurrentTradeSlots.Count; k++)
				{
					CurrentTradeSlots[k].Setup();
				}
			}
			if ((base.manager.GameEconomyData.Started && GetTimeLeftToTradeShopRefresh() <= 0) || LastTradeShopRefreshTime == 0L)
			{
				RefreshTradeSlotsAndItems();
			}
			if (base.manager.GameEconomyData.GetFeature("SeasonVersionReset").Enabled && CurrentSeasonVersion < base.manager.GameEconomyData.ConfigData.SeasonVersion && MapContainerModel != null && MapContainerModel.MapMissionGroups != null)
			{
				base.Debug.Log("New Season version. Relocking and reseting the season missions to version: " + base.manager.GameEconomyData.ConfigData.SeasonVersion);
				MapMissionGroupModel mapMissionGroupModel;
				MapMissionModel mapMissionModel;
				MissionSpawnPointGroup missionSpawnPointGroup;
				for (int l = 0; l < MapContainerModel.MapMissionGroups.Count; l++)
				{
					mapMissionGroupModel = MapContainerModel.MapMissionGroups[l];
					if (mapMissionGroupModel == null)
					{
						continue;
					}
					for (int m = 0; m < mapMissionGroupModel.Missions.Count; m++)
					{
						mapMissionModel = mapMissionGroupModel.Missions[m];
						if (mapMissionModel == null)
						{
							continue;
						}
						missionSpawnPointGroup = mapMissionModel.MissionSpawnPointGroup;
						if (missionSpawnPointGroup != null && missionSpawnPointGroup.Category == MapCategory.Season)
						{
							mapMissionModel.CompletionTimes = 0;
							mapMissionModel.RespawnTimer = 0;
							if (mapMissionModel.IsFirstInGroup)
							{
								mapMissionModel.State = MapMissionState.Respawning;
							}
							else
							{
								mapMissionModel.State = MapMissionState.Locked;
							}
						}
					}
				}
				mapMissionGroupModel = null;
				mapMissionModel = null;
				missionSpawnPointGroup = null;
				CurrentSeasonVersion = base.manager.GameEconomyData.ConfigData.SeasonVersion;
			}
			if (LastNameSetTimestamp == 0L)
			{
				LastNameSetTimestamp = UtcTimeStamp - LifeTime;
			}
			if (IconIndexs == null)
			{
				IconIndexs = new HashSet<int>();
			}
			if (BorderIndexs == null)
			{
				BorderIndexs = new HashSet<int>();
			}
			if (ColorIndexs == null)
			{
				ColorIndexs = new HashSet<int>();
			}
			if (base.manager.GameEconomyData.AvatarsDefinitions != null)
			{
				foreach (AvatarsDefinition avatarsDefinition in base.manager.GameEconomyData.AvatarsDefinitions)
				{
					if (avatarsDefinition.InitialUnlock)
					{
						AddIconIndex(avatarsDefinition.Index);
					}
				}
			}
			if (base.manager.GameEconomyData.BordersDefinitions != null)
			{
				foreach (BordersDefinition bordersDefinition in base.manager.GameEconomyData.BordersDefinitions)
				{
					if (bordersDefinition.InitialUnlock)
					{
						AddBorderIndex(bordersDefinition.Index);
					}
				}
			}
			if (base.manager.GameEconomyData.AvatarColorsDefinitions != null)
			{
				foreach (AvatarColorsDefinition avatarColorsDefinition in base.manager.GameEconomyData.AvatarColorsDefinitions)
				{
					if (avatarColorsDefinition.InitialUnlock)
					{
						AddColorIndex(avatarColorsDefinition.Index);
					}
				}
			}
			UpdateCurrenciesCapacity();
		}

		public bool IsTimedBonusActive(TimedBonusType type)
		{
			return GetTimedBonus(type)?.IsActive ?? false;
		}

		public bool HasSurvivalPointsDoubleBonus()
		{
			if (!IsTimedBonusActive(TimedBonusType.DoubleXp))
			{
				ReturnActivityManager returnActivityManager = ReturnActivityManager;
				if (returnActivityManager == null)
				{
					return false;
				}
				return returnActivityManager.ReturnPrivilege?.HasDoubleSurvivalPointsBonus() == true;
			}
			return true;
		}

		public int GetSurvivalPointsMultiplierValue()
		{
			int num = 1;
			if (IsTimedBonusActive(TimedBonusType.DoubleXp))
			{
				num *= 2;
			}
			ReturnActivityManager returnActivityManager = ReturnActivityManager;
			if (returnActivityManager != null && returnActivityManager.ReturnPrivilege?.HasDoubleSurvivalPointsBonus() == true)
			{
				num *= 2;
			}
			return num;
		}

		public int GetSuppliesMultiplierValue()
		{
			ReturnActivityManager returnActivityManager = ReturnActivityManager;
			if (returnActivityManager == null || returnActivityManager.ReturnPrivilege?.HasDoubleSuppliesBonus() != true)
			{
				return 1;
			}
			return 2;
		}

		public void NotifyCurrencySpent(CurrencyType type, int amount)
		{
			if (this.OnCurrencySpentEvent != null)
			{
				this.OnCurrencySpentEvent(type, amount);
			}
		}

		public void NotifyWalkersKilled(int amount)
		{
			if (this.OnWalkersKilledEvent != null)
			{
				this.OnWalkersKilledEvent(amount);
			}
		}

		public void NotifyCouncilLevelUp(int level)
		{
			if (this.OnCouncilLevelUpEvent != null)
			{
				this.OnCouncilLevelUpEvent(level);
			}
		}

		public void NotifyItemUpgraded(ReturnQuestType upgradeQuestType)
		{
			if (this.OnItemUpgradedEvent != null)
			{
				this.OnItemUpgradedEvent(upgradeQuestType);
			}
		}

		public void NotifyMissionCompleted()
		{
			if (this.OnMissionCompletedEvent != null)
			{
				this.OnMissionCompletedEvent();
			}
		}

		public void RefreshSurvivalPointsAddMultiplier()
		{
			CurrencyModel currency = GetCurrency(CurrencyType.SurvivalPoints);
			if (currency != null)
			{
				FixedPoint fixedPoint = GetSurvivalPointsMultiplierValue();
				if (currency.AddMultiplier != fixedPoint)
				{
					currency.AddMultiplier = fixedPoint;
					NotifyChange("currencyChangedEvent");
				}
			}
		}

		public void RefreshSuppliesAddMultiplier()
		{
			CurrencyModel currency = GetCurrency(CurrencyType.Supplies);
			if (currency != null)
			{
				FixedPoint fixedPoint = GetSuppliesMultiplierValue();
				if (currency.AddMultiplier != fixedPoint)
				{
					currency.AddMultiplier = fixedPoint;
					NotifyChange("currencyChangedEvent");
				}
			}
		}

		public TimedBonusModel GetTimedBonus(TimedBonusType type)
		{
			if (TimedBonusModels == null)
			{
				return null;
			}
			for (int i = 0; i < TimedBonusModels.Count; i++)
			{
				if (TimedBonusModels[i].TimedBonusTypeType == type)
				{
					return TimedBonusModels[i];
				}
			}
			return null;
		}

		public void AddTimedBonus(TimedBonusType type, FixedPoint timedBonusDuration)
		{
			TimedBonusModel timedBonusModel = GetTimedBonus(type);
			if (timedBonusModel == null)
			{
				switch (type)
				{
				case TimedBonusType.UnlimitedGas:
					timedBonusModel = new TimedBonusModelUnlimitedGasModel();
					break;
				case TimedBonusType.DoubleXp:
					timedBonusModel = new DoubleXpTimedBonusModelModel();
					break;
				}
				if (timedBonusModel != null)
				{
					timedBonusModel.SetManager(base.manager);
					timedBonusModel.Initialize();
					timedBonusModel.Start();
					TimedBonusModels.Add(timedBonusModel);
				}
			}
			timedBonusModel?.SetDuration(timedBonusDuration);
		}

		public TWDModelResult FetchGuildGifts(GuildModel guild)
		{
			if (guild != null)
			{
				List<GuildGift> availableGifts = guild.AvailableGifts;
				if (availableGifts != null)
				{
					bool flag = false;
					foreach (GuildGift item in availableGifts)
					{
						if (IsPlayerInReceiversList(item) && !isGiftInPendingList(item) && !isGiftInCollectedList(item))
						{
							GuildGift guildGift = new GuildGift();
							guildGift.Id = item.Id;
							guildGift.Type = item.Type;
							guildGift.Creationtime = item.Creationtime;
							guildGift.ExpireTime = item.ExpireTime;
							guildGift.GuildId = item.GuildId;
							guildGift.SenderId = item.SenderId;
							guildGift.SenderName = item.SenderName;
							guildGift.SenderMessage = item.SenderMessage;
							guildGift.Recipients = item.Recipients;
							if (PendingGuildGiftsToOpen == null)
							{
								PendingGuildGiftsToOpen = new List<GuildGift>();
							}
							if (PendingGuildGiftsLootToOpen == null)
							{
								PendingGuildGiftsLootToOpen = new List<LootEntry>();
							}
							PendingGuildGiftsToOpen.Add(guildGift);
							LootEntry lootEntry = LootManager.ShuffleOneLootWithoutTag(new LootEntryGenParams
							{
								eventType = DropEventDefinition.DropEventType.GuildGift,
								targetLevel = Level,
								dropType = guildGift.Type
							});
							lootEntry.Type = LootEntryType.GuildGift;
							PendingGuildGiftsLootToOpen.Add(lootEntry);
							flag = true;
						}
					}
					if (flag)
					{
						NotifyChange("guildGiftAvailable");
					}
				}
				ClearExpiredGifts();
				return TWDModelResult.OK;
			}
			base.Debug.LogError("TWDModel Received guild is null ");
			return TWDModelResult.Error;
		}

		private bool IsPlayerInReceiversList(GuildGift gift)
		{
			if (gift != null && gift.Recipients != null && gift.Recipients.Count > 0 && gift.Recipients.IndexOf(HashedId) > -1)
			{
				return true;
			}
			return false;
		}

		private bool isGiftInPendingList(GuildGift targetGift)
		{
			if (PendingGuildGiftsToOpen != null)
			{
				foreach (GuildGift item in PendingGuildGiftsToOpen)
				{
					if (item.Id == targetGift.Id)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool isGiftInCollectedList(GuildGift targetGift)
		{
			if (targetGift != null && OpenedGuildGifts != null)
			{
				foreach (GuildGift openedGuildGift in OpenedGuildGifts)
				{
					if (openedGuildGift.Id == targetGift.Id)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsGiftStillInGuild(GuildGift targetGift)
		{
			if (targetGift != null && GuildModel != null && GuildModel.AvailableGifts != null)
			{
				foreach (GuildGift availableGift in GuildModel.AvailableGifts)
				{
					if (availableGift != null && availableGift.Id == targetGift.Id)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int GetQuestChallengeBonusStars()
		{
			int num = 0;
			List<DailyQuest> dailyQuests = DailyQuests;
			if (dailyQuests != null && dailyQuests.Count > 0)
			{
				for (int i = 0; i < dailyQuests.Count; i++)
				{
					if (dailyQuests[i].ChallengeBonusStars > 0 && !dailyQuests[i].IsCompleted)
					{
						num++;
					}
				}
			}
			return num;
		}

		public void UpdateCurrenciesCapacity()
		{
			int count = Currencies.Count;
			for (int i = 0; i < count; i++)
			{
				CurrencyModel currencyModel = Currencies[i];
				currencyModel.SetCapacity(GetCapacity(currencyModel.Type));
			}
		}

		public void UpdateCurrencyCapacity(CurrencyType type)
		{
			GetCurrency(type)?.SetCapacity(GetCapacity(type));
		}

		public void UpdatePersonalTotalVpForGuild(string guildIdKey, int newAmount)
		{
			if (IsGuildMember && !string.IsNullOrEmpty(guildIdKey))
			{
				if (TotalAlltimeGvGVpAccumulatedPerGuild == null)
				{
					TotalAlltimeGvGVpAccumulatedPerGuild = new Dictionary<string, int>();
				}
				if (TotalAlltimeGvGVpAccumulatedPerGuild.ContainsKey(guildIdKey))
				{
					TotalAlltimeGvGVpAccumulatedPerGuild[guildIdKey] = newAmount;
				}
				else
				{
					TotalAlltimeGvGVpAccumulatedPerGuild.Add(guildIdKey, newAmount);
				}
			}
		}

		public int CalculateLifeTimeGvGVpAccumulated()
		{
			if (TotalAlltimeGvGVpAccumulatedPerGuild == null)
			{
				return 0;
			}
			int num = 0;
			foreach (KeyValuePair<string, int> item in TotalAlltimeGvGVpAccumulatedPerGuild)
			{
				num += item.Value;
			}
			return num;
		}

		public RunLocationModel GetOutpostTemplate(string missionId)
		{
			if (OutpostTemplateCache == null)
			{
				return null;
			}
			if (missionId != null)
			{
				if (!OutpostTemplateCache.ContainsKey(missionId))
				{
					return null;
				}
				return OutpostTemplateCache[missionId];
			}
			return null;
		}

		public void SetOutpostTemplateByMissionId(string missionId, RunLocationModel runLocation)
		{
			if (OutpostTemplateCache == null)
			{
				OutpostTemplateCache = new Dictionary<string, RunLocationModel>();
			}
			if (!OutpostTemplateCache.ContainsKey(missionId))
			{
				OutpostTemplateCache.Add(missionId, runLocation);
			}
		}

		public void SetOutpostTemplate(string outpostId, RunLocationModel runLocation)
		{
			if (OutpostTemplateCache == null)
			{
				OutpostTemplateCache = new Dictionary<string, RunLocationModel>();
			}
			OutpostTemplateDefinition outpostTemplateDefinition = base.manager.GameEconomyData.GetOutpostTemplateDefinition(outpostId);
			if (outpostTemplateDefinition != null)
			{
				string missionID = outpostTemplateDefinition.MissionID;
				if (OutpostTemplateCache.ContainsKey(missionID))
				{
					OutpostTemplateCache[missionID] = runLocation;
				}
				else
				{
					OutpostTemplateCache.Add(missionID, runLocation);
				}
			}
		}

		public void SetRunLocation(RunLocationModel runLocation, PlayerModel defendingPlayer = null)
		{
			Combat = new CombatModel();
			Combat.SetManager(base.manager);
			Combat.Initialize();
			Combat.SceneName = runLocation.SceneName;
			Combat.BackgroundSceneName = runLocation.BackgroundSceneName;
			Combat.SetGridModel(runLocation.Grid);
			if (defendingPlayer != null)
			{
				Combat.SetupOutpostCombat(defendingPlayer);
				OutpostModel.MatchMakingPaid = false;
			}
			string selectedMissionId = base.manager.Player.SelectedMissionId;
			MissionModel missionModel = runLocation.GetMission(selectedMissionId);
			if (missionModel == null)
			{
				base.Debug.Log(">>> Mission " + selectedMissionId + " not found in run location " + runLocation.DisplayName + ", selecting first mission!!!");
				missionModel = runLocation.Missions[0];
			}
			Combat.CurrentMissionId = missionModel.Id;
			ModelList<TWDModelObject> modelList = new ModelList<TWDModelObject>();
			foreach (TWDModelObject model in runLocation.Models)
			{
				if (model != null)
				{
					modelList.Add(model);
				}
				else
				{
					base.manager.Debug.LogError("Null model object run location models!");
				}
			}
			foreach (TWDModelObject model2 in missionModel.Models)
			{
				if (model2 != null)
				{
					modelList.Add(model2);
				}
				else
				{
					base.manager.Debug.LogError("Null model object in currentMission models!");
				}
			}
			foreach (OutpostSliceModel outpostSlice in missionModel.OutpostSlices)
			{
				foreach (TWDModelObject model3 in outpostSlice.Models)
				{
					if (model3 != null)
					{
						modelList.Add(model3);
					}
					else
					{
						base.manager.Debug.LogError("Null model object in one of the slices!");
					}
				}
				modelList.Add(outpostSlice);
			}
			foreach (MissionModel mission in runLocation.Missions)
			{
				if (mission == missionModel)
				{
					continue;
				}
				foreach (TWDModelObject model4 in mission.Models)
				{
					if (model4 is CombatColliderModel combatColliderModel)
					{
						combatColliderModel.IsEnabled = false;
						modelList.Add(combatColliderModel);
					}
				}
			}
			modelList.Add(missionModel);
			if (missionModel.PVPType == PvPMissionType.PVPMultiFlag || missionModel.PVPType == PvPMissionType.PVPMultiLoot)
			{
				OutpostCombat outpostCombat = Combat.OutpostCombat;
				List<WalkerSpawnPointModel> list = new List<WalkerSpawnPointModel>();
				List<WalkerSpawnPointModel> list2 = new List<WalkerSpawnPointModel>();
				int count = modelList.Count;
				for (int i = 0; i < count; i++)
				{
					if (modelList[i] is WalkerSpawnPointModel walkerSpawnPointModel)
					{
						if (walkerSpawnPointModel.ActivationType == ActivationType.OutpostInitial)
						{
							list.Add(walkerSpawnPointModel);
						}
						else
						{
							list2.Add(walkerSpawnPointModel);
						}
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					modelList.Remove(list[j]);
				}
				for (int k = 0; k < list.Count; k++)
				{
					modelList.Add(list[k]);
				}
				if (list.Count > 0)
				{
					string walkerId = WalkerType.WalkerNormal.ToString();
					int overrideWalkerLevel = outpostCombat?.GetWalkerModel(walkerId).Level ?? 1;
					int num;
					int val = (int)Math.Round((float)(num = outpostCombat?.GetWalkerModel(walkerId).Amount ?? 1) / (float)list.Count);
					foreach (WalkerSpawnPointModel item in list)
					{
						item.OverrideWalkerLevel = overrideWalkerLevel;
						item.SpawnCountPerAction = Math.Min(val, num);
						item.ActivationCount = 1;
						num -= item.SpawnCountPerAction;
					}
				}
				foreach (WalkerSpawnPointModel item2 in list2)
				{
					int overrideWalkerLevel2 = outpostCombat?.GetWalkerModel(item2.OverrideWalkerType.ToString()).Level ?? 1;
					item2.OverrideWalkerLevel = overrideWalkerLevel2;
					item2.ActivationCount = 1;
				}
			}
			Combat.MissionType = missionModel.TypeOfMission;
			Combat.IsDeadly = base.manager.Player.SelectedMissionIsDeadly;
			Combat.LootTag = base.manager.Player.SelectedMissionLootTag;
			Combat.InitialTurnCountToWave = missionModel.InitialTurnCountToWave;
			Combat.AfterAlarmTurns = missionModel.PvPAfterAlarmTurns;
			Combat.PvPMissionType = missionModel.PVPType;
			Combat.InitialThreatLevel = missionModel.InitialThreatLevel;
			Combat.OptionalLootKeys = missionModel.OptionalLootKeys;
			Combat.InitialLootKeys = missionModel.CompletionBonusLootKeys;
			Combat.CurrentMissionTextID = missionModel.DisplayTextID;
			Combat.FactionNames = missionModel.FactionNames;
			Combat.SpMultiplier = 1.0;
			if (MapContainerModel != null && MapContainerModel.AttackTargetMissionModel != null && base.manager.Player.Tutorial.StaticTutorialComplete && base.gameEconomyData.WeeklyClassEvents != null)
			{
				for (int l = 0; l < base.gameEconomyData.WeeklyClassEvents.Length; l++)
				{
					WeeklyClassEvent weeklyClassEvent = base.gameEconomyData.WeeklyClassEvents[l];
					if (weeklyClassEvent.SurvivorClass == SurvivorClass.None && weeklyClassEvent.Multiplier != 0L && weeklyClassEvent.MissionCategory == combatModel.MapCategory)
					{
						Combat.SpMultiplier = weeklyClassEvent.Multiplier;
					}
				}
			}
			if (string.IsNullOrEmpty(runLocation.ExportedVisibility) || string.IsNullOrEmpty(runLocation.ExportedMovement))
			{
				string visibilityData = GridColliderData.ConvertToString(runLocation.GridVisibility);
				string movementData = GridColliderData.ConvertToString(runLocation.GridMovement);
				base.manager.Debug.Log("Converting deprecated visibility/movement data on run location " + selectedMissionId);
				Combat.SetGridColliderData(visibilityData, movementData);
			}
			else
			{
				Combat.SetGridColliderData(runLocation.ExportedVisibility, runLocation.ExportedMovement);
			}
			Combat.SetModels(modelList);
			bool flag = false;
			if (base.manager.IsUsingCustomConfigForSurvivalMission)
			{
				for (int m = 0; m < base.manager.GameEconomyData.SurvivalMissionConfigs.Length; m++)
				{
					SurvivalMissionConfig survivalMissionConfig = base.manager.GameEconomyData.SurvivalMissionConfigs[m];
					if (survivalMissionConfig.ConfigName == base.manager.CustomSurvivalMissionConfigName && survivalMissionConfig.MissionOrderInSection == base.manager.CustomSurvivalMissionOrderInSection)
					{
						Combat.ApplySurvivalConfig(survivalMissionConfig, SavedSurvivalMissionData);
						flag = true;
						break;
					}
				}
			}
			else if (base.manager.IsUsingCustomConfigForGuildBattleMission)
			{
				SurvivalMissionConfig survivalMissionConfig2 = GuildBattleMapMissionModel.GenerateSurvivalMissionConfigDebug(base.manager.CustomGuildBattleMissionConfigObjectivesString, base.manager.CustomGuildBattleMissionConfigEnemiesString);
				Combat.ApplySurvivalConfig(survivalMissionConfig2, SavedSurvivalMissionData);
			}
			else if (MapContainerModel.AttackTargetMissionGroupModel != null && MapContainerModel.AttackTargetMissionGroupModel.MissionSpawnPointGroup != null && MapContainerModel.AttackTargetMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Survival)
			{
				int nextMissionOrderNumber = base.manager.Player.WeeklySurvival.NextMissionOrderNumber;
				if (nextMissionOrderNumber >= 0 && nextMissionOrderNumber < MapContainerModel.AttackTargetMissionGroupModel.Missions.Count)
				{
					SurvivalMissionConfig survivalMissionConfig3 = MapContainerModel.AttackTargetMissionGroupModel.Missions[nextMissionOrderNumber].SolveCurrentlyApplicableSurvivalConfigForOrderNumber(nextMissionOrderNumber);
					if (survivalMissionConfig3 != null)
					{
						Combat.ApplySurvivalConfig(survivalMissionConfig3, SavedSurvivalMissionData);
						flag = true;
					}
					else
					{
						base.Debug.LogError("Survival mission config solving failed, cannot apply survival mode to mission");
					}
				}
				else
				{
					base.Debug.LogError("Survival mission changes apply failed due to inability to properly solve the mission in expected range and thus survival config.");
				}
			}
			if (MapContainerModel.AttackTargetMissionGroupModel != null && MapContainerModel.AttackTargetMissionGroupModel.MissionSpawnPointGroup != null)
			{
				if (MapContainerModel.AttackTargetMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Challenge)
				{
					IncrementalDifficultyMissionType incrementalDifficultyMissionType = missionModel.IncrementalDifficultyType;
					if (incrementalDifficultyMissionType == IncrementalDifficultyMissionType.Automatic)
					{
						incrementalDifficultyMissionType = DetermineIncrementalDifficultyType();
					}
					int currentCycle = base.manager.Player.WeeklyChallenge.CurrentCycle;
					int currentPotentialTeamStrength = base.manager.Player.WeeklyChallenge.CurrentPotentialTeamStrength;
					GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
					int currentRequiredSurvivorLevel = base.manager.Player.WeeklyChallenge.CurrentRequiredSurvivorLevel;
					int totalCyclesSinceDifficultyChanged = base.manager.Player.WeeklyChallenge.TotalCyclesSinceDifficultyChanged;
					base.Debug.Log("Incremental Difficulty Applied: MissionType=" + incrementalDifficultyMissionType.ToString() + " Cycle=" + currentCycle + " PTS=" + currentPotentialTeamStrength + " MissionLevel=" + currentRequiredSurvivorLevel + " Increments=" + totalCyclesSinceDifficultyChanged);
					int incrementCount = totalCyclesSinceDifficultyChanged;
					List<IncrementalDifficultyEffectDefinition> difficultyEffects = gameEconomyData.GetDifficultyEffects(incrementalDifficultyMissionType, incrementCount);
					for (int n = 0; n < difficultyEffects.Count; n++)
					{
						Combat.ApplyDifficultyEffect(difficultyEffects[n]);
					}
				}
				else if (MapContainerModel.AttackTargetMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.ApocalypticChallenge)
				{
					List<IncrementalDifficultyEffectDefinition> difficultyEffects2 = ApocalypseWeeklyChallenge.GetDifficultyEffects();
					for (int num2 = 0; num2 < difficultyEffects2.Count; num2++)
					{
						Combat.ApplyDifficultyEffect(difficultyEffects2[num2]);
					}
				}
			}
			if (GuildWarModel != null && GuildWarModel.CurrentBattle != null && GuildWarModel.CurrentBattle.CurrentMapModel != null && GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel != null)
			{
				_ = GuildWarModel.CurrentBattle.CurrentMapModel;
				GuildBattleMapMissionModel attackTargetMissionModel = GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel;
				GuildBattleMapSectorModel sectorModelOwner = GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel.SectorModelOwner;
				SurvivalMissionConfig survivalMissionConfig4 = attackTargetMissionModel.SolveSurvivalConfigForCurrentMission();
				if (survivalMissionConfig4 != null)
				{
					Combat.ApplySurvivalConfig(sectorModelOwner.MissionConfigPoolName, survivalMissionConfig4, attackTargetMissionModel.MissionConfigIndexObjective, attackTargetMissionModel.MissionConfigIndexEnemies);
				}
				else
				{
					base.Debug.LogError("Survival mission config solving failed, cannot apply survival mode to mission");
				}
			}
			Combat.InitializeMission();
			Combat.SetManager(base.manager);
			Combat.Start();
			if (flag)
			{
				SurvivalCombatHelper.ApplySavedPlayerCharacterStates(Combat, SurvivorContainer.SurvivalCharacters);
			}
			ApplyReadyForActionTrait();
			CombatApplyReadyForTrait();
			CombatApplySurvivorManualHP();
			ApplyAttributeSystems();
			if (GetAttackTargetMissionModel() is MapMissionModel { IsInApocalyptiWeeklyChallenge: not false })
			{
				ApocalypseWeeklyChallenge.ApplyBuffAtCombatStart(Combat);
			}
			Combat.Tick(0L);
		}

		private void ApplyReadyForActionTrait()
		{
			if (Combat.MissionStarted)
			{
				return;
			}
			for (int i = 0; i < Combat.Survivors.Count; i++)
			{
				FixedPoint value = 0.0;
				FixedPoint value2 = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, Combat.Survivors[i]);
				AbilityManager.VisitParameter("AbilityModifieAddChargePointAtStart", ref value, Combat.Survivors[i]);
				if (value == 0.0 || Combat.Survivors[i].ChargeMeter == null)
				{
					continue;
				}
				int chargeLevel = Combat.Survivors[i].ChargeMeter.ChargeLevel;
				for (int j = Combat.Survivors[i].ChargeMeter.ChargeLevel; j < Combat.Survivors[i].ChargeMeter.MaxLevel; j++)
				{
					if (base.manager.Player.RollDice(RollDiceType.GainChargePointAtStart, value, (chargeLevel == Combat.Survivors[i].ChargeMeter.ChargeLevel) ? value2 : ((FixedPoint)0L)) != PlayerRandomChanceResult.Failed)
					{
						Combat.Survivors[i].AddChargePoints(1);
						Combat.Survivors[i].NumberChargePointAtStart++;
					}
				}
			}
		}

		private IncrementalDifficultyMissionType DetermineIncrementalDifficultyType()
		{
			List<TWDModelObject> models = Combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				if (models[i] is ActorSpawnPointModel { IsThreatActivated: not false })
				{
					return IncrementalDifficultyMissionType.ThreatMission;
				}
			}
			for (int j = 0; j < models.Count; j++)
			{
				if (models[j] is ActorSpawnPointModel { Faction: Faction.Raider })
				{
					return IncrementalDifficultyMissionType.RaiderMission;
				}
			}
			return IncrementalDifficultyMissionType.OtherMission;
		}

		public void RecordCombatStatus()
		{
			if (Combat != null && SubscriptionManager.IsSubscriptionActive)
			{
				if (CombatBackups.Count > Combat.TurnManager.TurnCount)
				{
					base.Debug.LogError($"RecordCombatStatus Turn:{Combat.TurnManager.TurnCount},RecordCount:{CombatBackups.Count}");
					CombatBackups.RemoveAfter(Combat.TurnManager.TurnCount - 1);
				}
				CombatBackup combatBackup = new CombatBackup();
				combatBackup.Initialize();
				combatBackup.RecordStatus(base.manager);
				combatBackup.SetManager(base.manager);
				combatBackup.Start();
				AddcombatBackup(combatBackup);
			}
		}

		private void AddcombatBackup(CombatBackup combatBackup)
		{
			CombatBackups.Add(combatBackup);
			int revertSaveTurn = base.gameEconomyData.ConfigData.RevertSaveTurn;
			while (CombatBackups.Count > revertSaveTurn)
			{
				CombatBackups.RemoveAt(0);
			}
		}

		private void SurvivorCombatCleanup()
		{
			if (CombatBackups != null)
			{
				CombatBackups.Clear();
			}
			int count = SurvivorContainer.Survivors.Count;
			for (int i = 0; i < count; i++)
			{
				SurvivorContainer.Survivors[i].CombatCleanup();
			}
			if (base.manager != null && base.manager.Player != null && base.manager.Player.AbilityManager != null)
			{
				base.manager.Player.AbilityManager.RemoveAllFactionBuffs();
				base.manager.Player.AbilityManager.RemoveAllGuildBattleBuffs();
				base.manager.Player.AbilityManager.RemoveAllFeaturedHeroBuffs();
			}
			if (base.manager != null && base.manager.Player != null && base.manager.Player.combatModel != null && base.manager.Player.combatModel.Survivors != null && base.manager.Player.combatModel.Survivors.Count > 0 && base.manager.CombatModel.Survivors[0] is SurvivorModel survivorModel && !base.manager.Player.SurvivorContainer.ContainsSurvivor(survivorModel))
			{
				survivorModel.UnregisterLeaderTraits();
			}
			if (base.manager != null && base.manager.Player != null && base.manager.Player.combatModel != null && base.manager.Player.combatModel.ClearRaiderLeaderTraitsPostCombat)
			{
				AbilityManager.ClearLeaderModifiersForFaction(Faction.Raider);
			}
			else
			{
				if (base.manager == null || base.manager.Player == null || base.manager.Player.combatModel == null || base.manager.Player.combatModel.Raiders == null || base.manager.Player.combatModel.Raiders.Count <= 0)
				{
					return;
				}
				for (int j = 0; j < base.manager.Player.combatModel.Raiders.Count; j++)
				{
					SurvivorModel survivorModel2 = base.manager.CombatModel.Raiders[j] as SurvivorModel;
					bool num;
					if (base.manager.Player.combatModel.SurvivalMissionConfigType != SurvivalMissionConfig.Type.GuildBattle)
					{
						if (survivorModel2 == null)
						{
							continue;
						}
						num = survivorModel2.PvPDefenderIndex == 0;
					}
					else
					{
						if (survivorModel2 == null)
						{
							continue;
						}
						num = survivorModel2.GuildBattlePvPSurvivorIndex == 0;
					}
					if (num)
					{
						survivorModel2.UnregisterLeaderTraits();
					}
				}
			}
		}

		public void DeleteCombatModel(bool notify = true, bool isForRetry = false)
		{
			if (base.manager != null && base.manager.Debug != null)
			{
				base.manager.Debug.LogInfo("DeleteCombatModel " + Environment.StackTrace);
			}
			if (LootManager != null)
			{
				LootManager.Clear();
			}
			if (base.manager != null)
			{
				if (Combat != null && Combat.IsGuildBattleMission && GvGSeasonModelPlayer != null && GuildWarModel != null && GuildWarModel.CurrentBattle != null)
				{
					UpdateLiveDataGroupCommand command = new UpdateLiveDataGroupCommand
					{
						Timestamp = UtcTimeStamp,
						UniqueMissionId = null
					};
					HelpersModel.ExecuteGroupCommand(base.manager, command);
				}
				base.manager.Player.MapContainerModel.ClearAttackTargetMissionData();
				if (Combat != null && Combat.IsGuildBattle() && base.manager.Player.GvGSeasonModelPlayer != null && !Combat.MissionCompleted && !isForRetry)
				{
					GuildWarDefinition guildWarDefinition = base.gameEconomyData.FindGuildWarWithId(GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentBattleWarId);
					int value = base.gameEconomyData.GetMissionCost(guildWarDefinition.CostIndex)?.EnergyCost ?? 1;
					int max = base.manager.Player.GetCurrency(CurrencyType.GvGMissionKey).Max;
					int amount = UtilsMath.Clamp(value, 0, max);
					GetCurrency(CurrencyType.GvGMissionKey).Add(amount);
				}
				if (Combat != null && Combat.IsGuildBattleMission && base.manager.Player.GvGSeasonModelPlayer != null)
				{
					base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.ReturnFromCombat();
				}
			}
			SurvivorCombatCleanup();
			Combat = null;
			LastCompletedMissionId = SelectedMissionId;
			UpdateModelObjects();
			if (notify)
			{
				NotifyChange("CombatModelDeleted");
			}
		}

		public bool IsValidNameLength(string name)
		{
			if (name.Length >= 3)
			{
				return name.Length <= 15;
			}
			return false;
		}

		public bool IsValidNameCharacters(string name)
		{
			return new Regex("^[\\w\\-]+( [\\w\\-]+)*$").IsMatch(name);
		}

		public TWDModelResult SetName(string name)
		{
			if (!IsValidNameCharacters(name))
			{
				return TWDModelResult.Error;
			}
			if (!IsValidNameLength(name))
			{
				return TWDModelResult.Error;
			}
			if (!CanChangePlayerName)
			{
				return TWDModelResult.PlayerNameChangingNotAllowed;
			}
			if (!string.IsNullOrEmpty(Name))
			{
				if (PreviousNames == null)
				{
					PreviousNames = new List<PlayerNameData>();
				}
				PlayerNameData playerNameData = new PlayerNameData();
				playerNameData.Name = Name;
				playerNameData.TimeStampAtChange = LastNameSetTimestamp;
				PreviousNames.Add(playerNameData);
			}
			base.manager.Metrics.AddStart().AddChangeName(name, string.IsNullOrEmpty(Name) ? "" : Name).Send();
			Name = name;
			LastNameSetTimestamp = UtcTimeStamp;
			NotifyChange("name");
			return TWDModelResult.OK;
		}

		public bool IsAvatarUnlock<T>(T data) where T : AvatarBaseDefinition
		{
			bool result = false;
			if (!(data is AvatarsDefinition))
			{
				if (!(data is BordersDefinition))
				{
					if (data is AvatarColorsDefinition)
					{
						result = ColorIndexs.Contains(data.Index);
					}
				}
				else
				{
					result = BorderIndexs.Contains(data.Index);
				}
			}
			else
			{
				result = IconIndexs.Contains(data.Index);
			}
			return result;
		}

		public void AddIconIndex(int index)
		{
			if (IconIndexs == null)
			{
				IconIndexs = new HashSet<int>();
			}
			IconIndexs.Add(index);
		}

		public void AddBorderIndex(int index)
		{
			if (BorderIndexs == null)
			{
				BorderIndexs = new HashSet<int>();
			}
			BorderIndexs.Add(index);
		}

		public void AddColorIndex(int index)
		{
			if (ColorIndexs == null)
			{
				ColorIndexs = new HashSet<int>();
			}
			ColorIndexs.Add(index);
		}

		public TWDModelResult SetPlayerEmblem(PlayerEmblem newEmblem)
		{
			if (!IconIndexs.Contains(newEmblem.IconIndex) || !BorderIndexs.Contains(newEmblem.BorderIndex) || !ColorIndexs.Contains(newEmblem.ColorIndex))
			{
				return TWDModelResult.Error;
			}
			PlayerEmblem = new PlayerEmblem(newEmblem);
			if (WeeklyChallenge != null)
			{
				WeeklyChallenge.UpdateChallengePlayerLeaderboards();
			}
			if (EndlessModeManager != null)
			{
				EndlessModeManager.UpdateLeaderBoardEntry();
			}
			NotifyChange("PlayerEmblemChanged");
			return TWDModelResult.OK;
		}

		public void SetMapName(string name)
		{
			MapName = name;
		}

		public int GetTutorialStep()
		{
			if (Tutorial == null)
			{
				return -1;
			}
			return Tutorial.CurrentStep;
		}

		public PendingPurchaseInfo GetPendingPurchase(string transactionId)
		{
			if (PendingIAPs == null)
			{
				return null;
			}
			for (int i = 0; i < PendingIAPs.Count; i++)
			{
				if (PendingIAPs[i].Transaction.TransactionIdentifier == transactionId)
				{
					return PendingIAPs[i];
				}
			}
			return null;
		}

		public bool RemovePendingPurchase(string transactionId)
		{
			PendingPurchaseInfo pendingPurchase = GetPendingPurchase(transactionId);
			if (pendingPurchase == null)
			{
				return false;
			}
			return PendingIAPs.Remove(pendingPurchase);
		}

		public void StoreIAPPurchase(IAPPurchase purchase)
		{
			if (purchase == null || purchase.Transaction == null || purchase.Transaction.ProductIdentifier == null)
			{
				return;
			}
			string productIdentifier = purchase.Transaction.ProductIdentifier;
			InAppPurchaseProductApple inAppPurchaseProduct = base.gameEconomyData.GetInAppPurchaseProduct(productIdentifier);
			if (inAppPurchaseProduct != null)
			{
				int num = BoughtIAPs.IndexOf(productIdentifier);
				if (num < 0)
				{
					BoughtIAPs.Add(productIdentifier);
					BoughtIAPsQuantity.Add(1);
				}
				else if (BoughtIAPsQuantity.Count > num)
				{
					BoughtIAPsQuantity[num] += 1;
				}
				TotalUSDSpent += inAppPurchaseProduct.PriceUSD;
			}
		}

		public void RegisterCustomBundleIAPPurchase(IAPTransaction transaction, CustomBundleDefinition bundleContent)
		{
			if (transaction != null && transaction.ProductIdentifier != null && bundleContent != null)
			{
				string productIdentifier = transaction.ProductIdentifier;
				if (BoughtIAPs == null)
				{
					BoughtIAPs = new List<string>();
				}
				if (BoughtIAPsQuantity == null)
				{
					BoughtIAPsQuantity = new List<int>();
				}
				int num = BoughtIAPs.IndexOf(productIdentifier);
				if (num < 0)
				{
					BoughtIAPs.Add(productIdentifier);
					BoughtIAPsQuantity.Add(1);
				}
				else if (BoughtIAPsQuantity.Count > num)
				{
					BoughtIAPsQuantity[num] += 1;
				}
				InAppPurchaseProductApple inAppPurchaseProduct = base.gameEconomyData.GetInAppPurchaseProduct(bundleContent.IAPProduct);
				if (inAppPurchaseProduct != null)
				{
					TotalUSDSpent += inAppPurchaseProduct.PriceUSD;
					base.manager.TdUserMetrics.SetEventType("total_pay_amount").AddProperty("total_pay_amount", inAppPurchaseProduct.PriceUSD).SendUser();
				}
			}
		}

		public void RegisterIAPPurchase(IAPTransaction transaction, BundleContentDefinition bundleContent)
		{
			if (transaction != null && transaction.ProductIdentifier != null && bundleContent != null)
			{
				string productIdentifier = transaction.ProductIdentifier;
				if (BoughtIAPs == null)
				{
					BoughtIAPs = new List<string>();
				}
				if (BoughtIAPsQuantity == null)
				{
					BoughtIAPsQuantity = new List<int>();
				}
				int num = BoughtIAPs.IndexOf(productIdentifier);
				if (num < 0)
				{
					BoughtIAPs.Add(productIdentifier);
					BoughtIAPsQuantity.Add(1);
				}
				else if (BoughtIAPsQuantity.Count > num)
				{
					BoughtIAPsQuantity[num] += 1;
				}
				InAppPurchaseProductApple inAppPurchaseProduct = base.gameEconomyData.GetInAppPurchaseProduct(bundleContent.IAPProduct);
				if (inAppPurchaseProduct != null)
				{
					TotalUSDSpent += inAppPurchaseProduct.PriceUSD;
					base.manager.TdUserMetrics.SetEventType("total_pay_amount").AddProperty("total_pay_amount", inAppPurchaseProduct.PriceUSD).SendUser();
				}
				if (bundleContent.Identifier != base.manager.GameEconomyData.SubscriptionConfig.WeeklySubscriptionPrice && bundleContent.Identifier != base.manager.GameEconomyData.SubscriptionConfig.MonthlySubscriptionPrice)
				{
					base.manager.Player.RFMGiftManager.AddPurchargeInfo(inAppPurchaseProduct.PriceUSD);
				}
				base.manager.Player.RFMGiftManager.OnBuyBundle(bundleContent.Identifier);
			}
		}

		public int GetCurrencyAmount(CurrencyType currencyType)
		{
			return GetCurrency(currencyType)?.Value ?? 0;
		}

		public CurrencyModel GetCurrency(CurrencyType currencyType)
		{
			return Currencies[(int)currencyType];
		}

		public CurrencyType GetComponentCurrencyType(string component, int rarityLevel)
		{
			return (CurrencyType)Enum.Parse(typeof(CurrencyType), component + rarityLevel);
		}

		public CurrencyModel GetComponentCurrency(string component, int rarityLevel)
		{
			return GetCurrency(GetComponentCurrencyType(component, rarityLevel));
		}

		public int GetTotalPurchases()
		{
			int num = 0;
			for (int i = 0; i < BoughtIAPsQuantity.Count; i++)
			{
				num += BoughtIAPsQuantity[i];
			}
			return num;
		}

		public int GetCapacity(CurrencyType currencyType)
		{
			switch (currencyType)
			{
			case CurrencyType.Survivor:
				return 10;
			case CurrencyType.LootKeys:
				return 6;
			case CurrencyType.ApocalypticSkipToken:
				return base.manager.GameEconomyData.ConfigData.ApocalypticSkipTokenCap;
			case CurrencyType.SuperBuildingTokenBP:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.SuperBuildingTokenCap;
			case CurrencyType.BuildingTokenBP:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingTokenCap;
			case CurrencyType.SuperEquipmentTokenBP:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.SuperWorkshopTokenCap;
			case CurrencyType.EquipmentTokenBP:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.WorkshopTokenCap;
			case CurrencyType.SuperTrainingTokenBP:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.SuperTrainingTokenCap;
			case CurrencyType.TrainingTokenBP:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.TrainingTokenCap;
			case CurrencyType.HealingTokenBP:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.HealingTokenCap;
			case CurrencyType.BuildingToken1min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken1minCap;
			case CurrencyType.BuildingToken5min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken5minCap;
			case CurrencyType.BuildingToken10min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken10minCap;
			case CurrencyType.BuildingToken30min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken30minCap;
			case CurrencyType.BuildingToken1h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken1hCap;
			case CurrencyType.BuildingToken6h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken6hCap;
			case CurrencyType.BuildingToken12h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken12hCap;
			case CurrencyType.BuildingToken24h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.BuildingToken24hCap;
			case CurrencyType.TrainingToken5min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.TrainingToken5minCap;
			case CurrencyType.TrainingToken20min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.TrainingToken20minCap;
			case CurrencyType.TrainingToken1h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.TrainingToken1hCap;
			case CurrencyType.TrainingToken3h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.TrainingToken3hCap;
			case CurrencyType.TrainingToken8h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.TrainingToken8hCap;
			case CurrencyType.TrainingToken16h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.TrainingToken16hCap;
			case CurrencyType.EquipmentToken1min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.EquipmentToken1minCap;
			case CurrencyType.EquipmentToken10min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.EquipmentToken10minCap;
			case CurrencyType.EquipmentToken20min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.EquipmentToken20minCap;
			case CurrencyType.EquipmentToken1h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.EquipmentToken1hCap;
			case CurrencyType.EquipmentToken3h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.EquipmentToken3hCap;
			case CurrencyType.EquipmentToken7h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.EquipmentToken7hCap;
			case CurrencyType.EquipmentToken14h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.EquipmentToken14hCap;
			case CurrencyType.HealingToken1min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.HealingToken1minCap;
			case CurrencyType.HealingToken5min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.HealingToken5minCap;
			case CurrencyType.HealingToken10min:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.HealingToken10minCap;
			case CurrencyType.HealingToken1h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.HealingToken1hCap;
			case CurrencyType.HealingToken2h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.HealingToken2hCap;
			case CurrencyType.HealingToken4h:
				return base.manager.GameEconomyData.SpeedupTokenDefinitions.HealingToken4hCap;
			default:
			{
				int buildingsCapacityForCurrency = GetBuildingsCapacityForCurrency(currencyType);
				if (buildingsCapacityForCurrency <= 0)
				{
					return UnlimitedCapacityAmount;
				}
				return buildingsCapacityForCurrency;
			}
			}
		}

		private int GetBuildingsCapacityForCurrency(CurrencyType currencyType)
		{
			int num = 0;
			if (Camp != null && Camp.Buildings != null)
			{
				for (int i = 0; i < Camp.Buildings.Count; i++)
				{
					BuildingModel buildingModel = Camp.Buildings[i];
					if (buildingModel.BuildingRepaired)
					{
						num += buildingModel.GetCurrentUpgradeCapacity(currencyType);
					}
				}
			}
			return num;
		}

		public int GetProductionPerHour(CurrencyType currencyType)
		{
			int num = 0;
			for (int i = 0; i < Camp.Buildings.Count; i++)
			{
				if (Camp.Buildings[i].Producer != null && Camp.Buildings[i].Producer.CurrencyType == currencyType)
				{
					num += Camp.Buildings[i].Producer.Rate;
				}
			}
			return num;
		}

		public override void Tick(long deltaTime)
		{
			LifeTime += deltaTime;
			base.Tick(deltaTime);
			if (Combat != null)
			{
				Combat.Tick(deltaTime);
			}
			if (GiftCoolDownTimer > 0)
			{
				GiftCoolDownTimer -= deltaTime;
				GiftCoolDownTimer = Math.Max(GiftCoolDownTimer, 0L);
			}
			if (IsEquipmentAutoScrap == AutoScrapEquipmentType.None || AutoScrapmentEquipment.Count() <= 0 || IsLoadDataManager)
			{
				return;
			}
			for (int i = 0; i < AutoScrapmentEquipment.Count(); i++)
			{
				if (!AutoScrapmentEquipment[i].IsConsumable)
				{
					_ = AutoScrapmentEquipment[i].Definition;
					if ((base.manager.GetPlayer() as PlayerModel).Equipment.ScrapEquipmentItem(AutoScrapmentEquipment[i]).Result == TWDModelResult.OK)
					{
						TWDModelManager tWDModelManager = base.manager;
						tWDModelManager.Player.DailyQuestManager.StartAction("Scrap").TargetType = "Equipment";
						tWDModelManager.Player.DailyQuestManager.CommitAction();
						tWDModelManager.Player.NotifyChange("AutoScrapEquipmentMessage");
					}
				}
			}
			AutoScrapmentEquipment.Clear();
		}

		public void OnCurrencyChange(ModelObject m, string changed, object args)
		{
			NotifyChange("currencyChangedEvent", m as CurrencyModel);
			if (changed == "SpeedUpTokenAcquired")
			{
				NotifyChange("SpeedUpTokenAcquired");
			}
			else if (changed == "SpeedUpTokenUsed")
			{
				NotifyChange("SpeedUpTokenUsed");
			}
		}

		public void OnCurrencyConvertToDiamonds(ModelObject m, string changed, object args)
		{
			NotifyChange("CurrencyConvertToDiamondsEvent");
		}

		public void OnBuildingChange(ModelObject m, string changed, object args)
		{
			if (changed == "level")
			{
				int count = Currencies.Count;
				for (int i = 0; i < count; i++)
				{
					CurrencyModel currencyModel = Currencies[i];
					currencyModel.SetCapacity(GetCapacity(currencyModel.Type));
				}
				NotifyChange("currencyChangedEvent");
			}
		}

		private void OnCampChange(ModelObject model, string changed, object args)
		{
			if (changed == "EventAddBuilding")
			{
				((BuildingModel)args).Changed += OnBuildingChange;
			}
			if (!(changed == "EventLevelUpBuilding"))
			{
				return;
			}
			BuildingModel buildingModel = (BuildingModel)args;
			if (base.manager == null || !(buildingModel.TypeName == "Council") || !base.manager.GameEconomyData.ConfigData.SupportTalentUnlockToggle || CouncilLevel < base.manager.GameEconomyData.ConfigData.SupportTalentUnlockAtCouncilLevel)
			{
				return;
			}
			foreach (SupportModel supportModel in SupportModels)
			{
				if (!supportModel.InitializedTalent)
				{
					supportModel.InitializeTalentTrees();
				}
			}
		}

		public void AddXp(int amount)
		{
			if (amount == 0)
			{
				return;
			}
			Xp += amount;
			NotifyChange("xp");
			PlayerLevelData[] playerLevelData = base.manager.GameEconomyData.PlayerLevelData;
			if (Level >= playerLevelData.Length || Level - 1 >= playerLevelData.Length)
			{
				return;
			}
			PlayerLevelData playerLevelData2 = playerLevelData[Level - 1];
			while (Xp >= playerLevelData2.NextLevelXp)
			{
				TdMetrics tdMetrics = base.manager.TdMetrics.SetEventType("levelup").AddProperty("before_level", Level);
				Level++;
				Xp -= playerLevelData2.NextLevelXp;
				NotifyChange("level");
				base.manager.TdUserMetrics.SetEventType("level").AddProperty("level", Level).SendUser();
				tdMetrics.AddProperty("after_level", Level).Send();
				if (Level - 1 < playerLevelData.Length)
				{
					playerLevelData2 = playerLevelData[Level - 1];
					continue;
				}
				break;
			}
		}

		public PlayerLevelData GetCurrentPlayerLevelData()
		{
			if (base.manager.GameEconomyData.PlayerLevelData.Length >= Level)
			{
				return base.manager.GameEconomyData.PlayerLevelData[Level - 1];
			}
			return null;
		}

		private static CapData GetCapData(MediationData mediationData, int gameplayDuration)
		{
			if (mediationData.Caps == null || mediationData.Caps.Count == 0)
			{
				return null;
			}
			for (int i = 1; i < mediationData.Caps.Count; i++)
			{
				if (gameplayDuration < mediationData.Caps[i].GameplayDuration)
				{
					return mediationData.Caps[i - 1];
				}
			}
			return mediationData.Caps[mediationData.Caps.Count - 1];
		}

		public CapData GetCapData()
		{
			MediationData mediationData = base.manager.MediationData;
			if (mediationData == null)
			{
				return null;
			}
			int gameplayDuration = (int)(LifeTime / 1000 / 60);
			return GetCapData(mediationData, gameplayDuration);
		}

		public long GetVideoAdAvailabilityTimeByType(AdUsage adUsage)
		{
			CapData capData = GetCapData();
			long result = 0L;
			if (capData != null)
			{
				if (adUsage == AdUsage.BuildUpgradeSpeedUp)
				{
					result = GetCapData().BuildingUpgradeSessionLength * 60 * 1000 - (LifeTime - VideoAdRewardBuildingMenuScreen);
				}
				if (adUsage == AdUsage.RefreshBlackMarketSlot)
				{
					result = GetCapData().BlackMarketRefreshSessionLength * 60 * 1000 - (LifeTime - VideoAdRewardBlackMarketScreen);
				}
				if (adUsage == AdUsage.CinemaReward)
				{
					result = GetCapData().TheaterSessionLength * 60 * 1000 - (LifeTime - VideoAdRewardTime);
				}
			}
			return result;
		}

		public bool IsVideoAdRewardAvailable(AdUsage adUsage)
		{
			CapData capData = GetCapData();
			if (capData == null)
			{
				return true;
			}
			switch (adUsage)
			{
			case AdUsage.CinemaReward:
				if (LifeTime - VideoAdRewardTime > capData.TheaterSessionLength * 60 * 1000)
				{
					VideoAdsServed = 0;
				}
				return VideoAdsServed < capData.TheaterSessionCap;
			case AdUsage.CombatRewardKey:
				if (LifeTime - VideoAdRewardTimeRewardScreen > capData.AfterMissionSessionLength * 60 * 1000)
				{
					VideoAdsServedRewardScreen = 0;
				}
				return VideoAdsServedRewardScreen < capData.AfterMissionSessionCap;
			case AdUsage.BuildUpgradeSpeedUp:
				if (LifeTime - VideoAdRewardBuildingMenuScreen > capData.BuildingUpgradeSessionLength * 60 * 1000)
				{
					VideoAdsServedBuildingMenuScreen = 0;
				}
				return VideoAdsServedBuildingMenuScreen < capData.BuildingUpgradeSessionCap;
			case AdUsage.RefreshBlackMarketSlot:
				if (LifeTime - VideoAdRewardBlackMarketScreen > capData.BlackMarketRefreshSessionLength * 60 * 1000)
				{
					VideoAdsServedBlackMarketScreen = 0;
				}
				return VideoAdsServedBlackMarketScreen < capData.BlackMarketRefreshSessionCap;
			default:
				return false;
			}
		}

		public bool CanGiveGuildGift()
		{
			return GiftCoolDownTimer <= 0;
		}

		public void ResetGuildGiftCooldownTimer()
		{
			GiftCoolDownTimer = base.gameEconomyData.ConfigData.GuildGiftCooldownTimer;
		}

		public LootEntry OpenGuildGift()
		{
			if (PendingGuildGiftsToOpen == null || PendingGuildGiftsToOpen.Count < 1 || PendingGuildGiftsLootToOpen == null || PendingGuildGiftsLootToOpen.Count < 1)
			{
				return null;
			}
			GuildGift item = PendingGuildGiftsToOpen[0];
			LootEntry lootEntry = PendingGuildGiftsLootToOpen[0];
			LootManager.GiveLoot(lootEntry);
			GuildMemberInfo guildMember = ((GuildModel != null) ? GuildModel.GetLeaderMemberInfo() : null);
			base.manager.Metrics.AddFind().AddLoot(lootEntry).AddGuildGift()
				.AddLootCrate(lootEntry)
				.AddGuild(GuildModel)
				.AddModerator(guildMember)
				.Send();
			if (OpenedGuildGifts == null)
			{
				OpenedGuildGifts = new List<GuildGift>();
			}
			OpenedGuildGifts.Add(item);
			PendingGuildGiftsToOpen.RemoveAt(0);
			PendingGuildGiftsLootToOpen.RemoveAt(0);
			NotifyChange("guildGiftClaimed");
			return lootEntry;
		}

		public bool HasPendingGuildGiftsToOpen()
		{
			if (PendingGuildGiftsToOpen != null)
			{
				return PendingGuildGiftsToOpen.Count > 0;
			}
			return false;
		}

		public TWDModelResult PayForGuildGift(bool usePerk)
		{
			return GetCashierForGuildGift(usePerk)?.Pay() ?? TWDModelResult.Error;
		}

		public TWDModelResult PayForGuildCreation()
		{
			return GetCashierForGuildCreation()?.Pay() ?? TWDModelResult.Error;
		}

		public Cashier GetCashierForGuildCreation()
		{
			return Cashier.CreateOneItemCashier(base.manager, PurchaseType.GuildCreate, CurrencyType.SurvivalPoints, 300);
		}

		public TWDModelResult PayForChangeGuildName()
		{
			return GetCashierForChangeGuildName()?.Pay() ?? TWDModelResult.Error;
		}

		public Cashier GetCashierForChangeGuildName()
		{
			return Cashier.CreateOneItemCashier(base.manager, PurchaseType.TradeCrate, CurrencyType.Diamonds, base.gameEconomyData.ConfigData.GuildNameChangeCost);
		}

		public Cashier GetCashierForGuildGift(bool usePerk)
		{
			int num = base.gameEconomyData.ConfigData.GuildGiftGoldPrice;
			if (ActivityManager.TryGetActivityParam(ActivityType.GoldGuildGifts100, out var activityParams))
			{
				num = int.Parse(activityParams[0]);
			}
			Cashier cashier = Cashier.CreateOneItemCashier(base.manager, PurchaseType.GuildGift, usePerk ? CurrencyType.FreeGuildGiftPerk : CurrencyType.Diamonds, usePerk ? 1 : num);
			cashier.UseDiamondsAmount = -2;
			return cashier;
		}

		public Cashier GetCashierForGuildAd()
		{
			return Cashier.CreateOneItemCashier(base.manager, PurchaseType.GuildAd, CurrencyType.Diamonds, base.gameEconomyData.ConfigData.GuildAdGoldPrice);
		}

		public TWDModelResult PayForGuildAd(string adUniqueId)
		{
			Cashier cashierForGuildAd = GetCashierForGuildAd();
			if (cashierForGuildAd != null)
			{
				cashierForGuildAd.UseDiamondsAmount = -2;
				return cashierForGuildAd.Pay(adUniqueId);
			}
			return TWDModelResult.Error;
		}

		private bool clearExpiredOffersFromDisallowed()
		{
			bool result = false;
			if (DisallowedOffers != null)
			{
				List<BundleOfferPurchaseEntry> list = new List<BundleOfferPurchaseEntry>();
				foreach (BundleOfferPurchaseEntry disallowedOffer in DisallowedOffers)
				{
					if (disallowedOffer != null && disallowedOffer.OfferEndTimeMilliseconds < UtcTimeStamp)
					{
						result = true;
						list.Add(disallowedOffer);
					}
				}
				foreach (BundleOfferPurchaseEntry item in list)
				{
					int num = DisallowedOffers.IndexOf(item);
					if (num > -1)
					{
						DisallowedOffers.RemoveAt(num);
					}
				}
				list.Clear();
			}
			return result;
		}

		private void ClearExpiredGifts()
		{
			if (PendingGuildGiftsToOpen != null)
			{
				List<GuildGift> list = new List<GuildGift>();
				List<LootEntry> list2 = new List<LootEntry>();
				for (int i = 0; i < PendingGuildGiftsToOpen.Count; i++)
				{
					GuildGift guildGift = PendingGuildGiftsToOpen[i];
					if ((guildGift.ExpireTime > -1 && guildGift.ExpireTime < UtcTimeStamp) || (GuildModel != null && !IsGiftStillInGuild(guildGift)))
					{
						list.Add(guildGift);
						LootEntry lootEntry = PendingGuildGiftsLootToOpen[i];
						if (lootEntry != null)
						{
							list2.Add(lootEntry);
						}
					}
				}
				foreach (GuildGift item in list)
				{
					PendingGuildGiftsToOpen.Remove(item);
				}
				foreach (LootEntry item2 in list2)
				{
					PendingGuildGiftsLootToOpen.Remove(item2);
				}
			}
			if (OpenedGuildGifts == null)
			{
				return;
			}
			List<GuildGift> list3 = new List<GuildGift>();
			foreach (GuildGift openedGuildGift in OpenedGuildGifts)
			{
				if (GuildModel != null && !IsGiftStillInGuild(openedGuildGift))
				{
					list3.Add(openedGuildGift);
				}
			}
			foreach (GuildGift item3 in list3)
			{
				OpenedGuildGifts.Remove(item3);
			}
		}

		public void ResetLifetime(long value)
		{
			LifeTime = value;
		}

		public void SetSelectedOutpostTemplateDefinitionId(string TemplateId)
		{
			SelectedOutpostTemplateDefinitionId = TemplateId;
		}

		public string GetSelectedOutpostTemplateMissionId()
		{
			if (SelectedOutpostTemplateDefinitionId != null)
			{
				return base.manager.GameEconomyData.GetOutpostTemplateDefinition(SelectedOutpostTemplateDefinitionId)?.MissionID;
			}
			return null;
		}

		public void AddAttackOutpostVisitLog(OutpostVisitEntry visitEntry)
		{
			if (AttackOutpostVisitLog == null)
			{
				AttackOutpostVisitLog = new List<OutpostVisitEntry>();
			}
			AttackOutpostVisitLog.Add(visitEntry);
			while (AttackOutpostVisitLog.Count > base.manager.GameEconomyData.ConfigData.OutpostVisitLogSize)
			{
				AttackOutpostVisitLog.RemoveAt(0);
			}
		}

		public void AddDefenseOutpostVisitLog(OutpostVisitEntry visitEntry)
		{
			if (DefenseOutpostVisitLog == null)
			{
				DefenseOutpostVisitLog = new List<OutpostVisitEntry>();
			}
			DefenseOutpostVisitLog.Add(visitEntry);
			while (DefenseOutpostVisitLog.Count > base.manager.GameEconomyData.ConfigData.OutpostVisitLogSize)
			{
				DefenseOutpostVisitLog.RemoveAt(0);
			}
		}

		public string ValidateStringsAgainstProfanity(string input)
		{
			if (input != null)
			{
				string text = input.Normalize();
				text = text.ToLower();
				bool flag = false;
				ProfanityFilter[] profanityFilters = base.gameEconomyData.ProfanityFilters;
				if (profanityFilters != null)
				{
					foreach (ProfanityFilter profanityFilter in profanityFilters)
					{
						if (text.Contains(profanityFilter.Target))
						{
							flag = true;
							text = text.Replace(profanityFilter.Target, "***");
						}
					}
				}
				if (flag)
				{
					return text;
				}
				return input;
			}
			return null;
		}

		public bool GiveExtraOutpostSurvivorsAndSlots()
		{
			if (OutpostLevel > 0 && !Blackboard.IsToggleOn("Toggle.OutpostGiftSurvivorsGiven"))
			{
				SurvivorContainer.SurvivorGiftSlotsCount += 3;
				int num = 1;
				BuildingModel building = Camp.GetBuilding("RadioTent");
				if (building != null)
				{
					num = building.Level;
				}
				for (int i = 0; i < 3; i++)
				{
					SurvivorModel survivorModel = base.manager.Player.SurvivorContainer.CreateRandomSurvivor(0, num, num, 0);
					SurvivorContainer.AddSurvivor(survivorModel);
					while (SurvivorContainer.OutpostDefendingSurvivors.Count > 2)
					{
						SurvivorContainer.OutpostDefendingSurvivors.RemoveAt(0);
					}
					SurvivorContainer.AddSurvivorToOutpostDefense(survivorModel);
				}
				Blackboard.SetToggle("Toggle.OutpostGiftSurvivorsGiven");
				return true;
			}
			return false;
		}

		public int GetFinalRankingScoreChange(int fullRankingScoreChange)
		{
			int firstValue = 0;
			int secondValue = 0;
			int thirdValue = 0;
			ConfigData configData = base.manager.GameEconomyData.ConfigData;
			UtilsMath.SplitValue(fullRankingScoreChange, configData.OutpostCratesCompletedInfluencePercentage, configData.OutpostFlagCompletedInfluencePercentage, configData.OutpostDefendersCompletedInfluencePercentage, out firstValue, out secondValue, out thirdValue);
			return 0 + (Combat.IsPvPLootCollected ? firstValue : 0) + (Combat.IsPvPFlagCollected ? secondValue : 0) + (Combat.IsPvpDefendersKilled ? thirdValue : 0);
		}

		public int GetFinalResourcesStolen(int fullResourcesToBeStolen)
		{
			int num = 0;
			if (Combat != null && (Combat.MissionResult == ECombatResult.Flee || Combat.MissionResult == ECombatResult.Successful))
			{
				int firstValue = 0;
				int secondValue = 0;
				int thirdValue = 0;
				ConfigData configData = base.manager.GameEconomyData.ConfigData;
				UtilsMath.SplitValue(fullResourcesToBeStolen, configData.OutpostCratesCompletedResourcePercentage, configData.OutpostFlagCompletedResourcePercentage, configData.OutpostDefendersCompletedResourcePercentage, out firstValue, out secondValue, out thirdValue);
				num += (Combat.IsPvPLootCollected ? firstValue : 0);
				num += (Combat.IsPvPFlagCollected ? secondValue : 0);
				num += (Combat.IsPvpDefendersKilled ? thirdValue : 0);
			}
			return num;
		}

		public PvPDefenderSaveType UpdateDefenderAfterPvP(string outpostVIsitId)
		{
			PvPDefenderSaveType result = PvPDefenderSaveType.None;
			if (Combat != null && Combat.OutpostCombat != null && !Combat.OutpostCombat.IsFake)
			{
				int finalRankingScoreChange = GetFinalRankingScoreChange(Combat.OutpostCombat.DefenderInfluenceLoss);
				int defenderInfluenceGain = Combat.OutpostCombat.DefenderInfluenceGain;
				int rankingScoreChange = ((Combat.MissionResult == ECombatResult.Successful) ? (-finalRankingScoreChange) : ((Combat.MissionResult == ECombatResult.Failed) ? defenderInfluenceGain : 0));
				int defenderInitialTradeGoods = Combat.OutpostCombat.DefenderInitialTradeGoods;
				int fullResourcesToBeStolen = defenderInitialTradeGoods * base.manager.GameEconomyData.ConfigData.OutpostStealResourcePercentage / 100;
				int val = (int)((long)GetFinalResourcesStolen(fullResourcesToBeStolen) * (long)base.manager.GameEconomyData.ConfigData.OutpostDefenderStealResourceMultiplierPercentage / 100);
				int resourcesStolen = Math.Min(defenderInitialTradeGoods, val);
				OutpostVisitEntry outpostVisitEntry = new OutpostVisitEntry
				{
					EntryType = OutpostVisitEntryType.Defended,
					OtherPlayerHashedId = HashedId,
					UtcTime = UtcTimeStamp,
					OtherPlayerName = Name,
					OtherPlayerLevel = Level,
					OtherOutpostLevel = Camp.GetBuildingLevel("Outpost"),
					ResourcesStolen = resourcesStolen,
					RankingScoreChange = rankingScoreChange,
					ProductionHaltedTime = ProductionHaltedTime,
					CombatResult = Combat.MissionResult,
					OutpostVisitId = outpostVIsitId
				};
				int count = SurvivorContainer.CombatSurvivors.Count;
				outpostVisitEntry.OtherSurvivorLevels = new int[count];
				outpostVisitEntry.OtherSurvivorClasses = new SurvivorClass[count];
				outpostVisitEntry.OtherSurvivorRarityLevels = new int[count];
				outpostVisitEntry.OtherSurvivorDefeated = new bool[count];
				for (int i = 0; i < count; i++)
				{
					outpostVisitEntry.OtherSurvivorLevels[i] = SurvivorContainer.CombatSurvivors[i].Level;
					outpostVisitEntry.OtherSurvivorClasses[i] = SurvivorContainer.CombatSurvivors[i].SurvivorClass;
					outpostVisitEntry.OtherSurvivorRarityLevels[i] = SurvivorContainer.CombatSurvivors[i].SurvivorRarityLevel;
					outpostVisitEntry.OtherSurvivorDefeated[i] = SurvivorContainer.CombatSurvivors[i].CombatEndCondition == CombatEndCondition.Incapacitated;
				}
				int count2 = Combat.OutpostCombat.DefendingSurvivors.Count;
				outpostVisitEntry.SurvivorLevels = new int[count2];
				outpostVisitEntry.SurvivorClasses = new SurvivorClass[count2];
				outpostVisitEntry.SurvivorRarityLevels = new int[count2];
				outpostVisitEntry.SurvivorDefeated = new bool[count2];
				for (int j = 0; j < count2; j++)
				{
					outpostVisitEntry.SurvivorLevels[j] = Combat.OutpostCombat.DefendingSurvivors[j].Level;
					outpostVisitEntry.SurvivorClasses[j] = Combat.OutpostCombat.DefendingSurvivors[j].SurvivorClass;
					outpostVisitEntry.SurvivorRarityLevels[j] = Combat.OutpostCombat.DefendingSurvivors[j].SurvivorRarityLevel;
					outpostVisitEntry.SurvivorDefeated[j] = Combat.GetPvPDefenderKilled(j);
				}
				outpostVisitEntry.MissionType = combatModel.PvPMissionType;
				outpostVisitEntry.FirstObjectiveCompleted = combatModel.IsPvPFlagCollected;
				outpostVisitEntry.SecondObjectiveCompleted = combatModel.IsPvPLootCollected;
				outpostVisitEntry.DefendersObjectiveCompleted = combatModel.IsPvpDefendersKilled;
				if (base.manager.ServerService != null)
				{
					base.manager.AddQueueMessage(Combat.OutpostCombat.DefenderHashedId, new OutpostResultLoadQueueMessage(outpostVisitEntry));
					if (outpostVisitEntry.RequiresShield())
					{
						base.manager.Debug.Log("activating shield for player=" + Combat.OutpostCombat.DefenderHashedId);
						base.manager.UpdateMatchMakingAvailability(Combat.OutpostCombat.DefenderHashedId, outpostVisitEntry.UtcTime + base.gameEconomyData.ConfigData.OutpostDefeatedShieldDuration * 1000);
					}
					result = PvPDefenderSaveType.LoadMessage;
					if (base.manager.ServerService != null)
					{
						base.manager.Debug.Log("UpdateDefenderAfterPvP() -> Player hashed id = " + HashedId + " defender PVP result processed, save type = " + result);
					}
				}
				base.manager.UpdateOutpostLeaderboardEntryForDefender(Combat.OutpostCombat, outpostVisitEntry.RankingScoreChange);
			}
			return result;
		}

		public void UpdateAttackerAfterPvP(string outpostVIsitId)
		{
			LastPvPAttackCompletionUtcTime = UtcTimeStamp;
			ResetOutpostShield();
			int finalRankingScoreChange = GetFinalRankingScoreChange(Combat.OutpostCombat.AttackerInfluenceGain);
			int attackerInfluenceLoss = Combat.OutpostCombat.AttackerInfluenceLoss;
			int finalResourcesStolen = GetFinalResourcesStolen(Combat.OutpostCombat.TradeGoodsGain);
			int num = ((Combat.MissionResult == ECombatResult.Successful) ? finalRankingScoreChange : ((Combat.MissionResult == ECombatResult.Failed) ? (-attackerInfluenceLoss) : 0));
			SetRankingScore(RankingScore + num);
			GetCurrency(CurrencyType.Outpost).Add(finalResourcesStolen);
			OutpostVisitEntry outpostVisitEntry = new OutpostVisitEntry
			{
				EntryType = OutpostVisitEntryType.Attacked,
				OtherPlayerHashedId = Combat.OutpostCombat.DefenderHashedId,
				UtcTime = UtcTimeStamp,
				OtherPlayerName = Combat.OutpostCombat.DefenderDisplayName,
				OtherPlayerLevel = Combat.OutpostCombat.DefenderPlayerLevel,
				OtherOutpostLevel = Combat.OutpostCombat.DefenderOutpostLevel,
				ResourcesStolen = Math.Max(finalResourcesStolen, 0),
				RankingScoreChange = num,
				CombatResult = Combat.MissionResult,
				OutpostVisitId = outpostVIsitId
			};
			int count = Combat.OutpostCombat.DefendingSurvivors.Count;
			outpostVisitEntry.OtherSurvivorLevels = new int[count];
			outpostVisitEntry.OtherSurvivorClasses = new SurvivorClass[count];
			outpostVisitEntry.OtherSurvivorRarityLevels = new int[count];
			outpostVisitEntry.OtherSurvivorDefeated = new bool[count];
			for (int i = 0; i < count; i++)
			{
				outpostVisitEntry.OtherSurvivorLevels[i] = Combat.OutpostCombat.DefendingSurvivors[i].Level;
				outpostVisitEntry.OtherSurvivorClasses[i] = Combat.OutpostCombat.DefendingSurvivors[i].SurvivorClass;
				outpostVisitEntry.OtherSurvivorRarityLevels[i] = Combat.OutpostCombat.DefendingSurvivors[i].SurvivorRarityLevel;
				outpostVisitEntry.OtherSurvivorDefeated[i] = Combat.GetPvPDefenderKilled(i);
			}
			int count2 = SurvivorContainer.CombatSurvivors.Count;
			outpostVisitEntry.SurvivorLevels = new int[count2];
			outpostVisitEntry.SurvivorClasses = new SurvivorClass[count2];
			outpostVisitEntry.SurvivorRarityLevels = new int[count2];
			outpostVisitEntry.SurvivorDefeated = new bool[count2];
			for (int j = 0; j < count2; j++)
			{
				outpostVisitEntry.SurvivorLevels[j] = SurvivorContainer.CombatSurvivors[j].Level;
				outpostVisitEntry.SurvivorClasses[j] = SurvivorContainer.CombatSurvivors[j].SurvivorClass;
				outpostVisitEntry.SurvivorRarityLevels[j] = SurvivorContainer.CombatSurvivors[j].SurvivorRarityLevel;
				outpostVisitEntry.SurvivorDefeated[j] = SurvivorContainer.CombatSurvivors[j].CombatEndCondition == CombatEndCondition.Incapacitated;
			}
			outpostVisitEntry.MissionType = combatModel.PvPMissionType;
			outpostVisitEntry.FirstObjectiveCompleted = ((combatModel.PvPMissionType == PvPMissionType.PVPMultiFlag) ? combatModel.IsPvPFlagCollected : combatModel.IsPvPLootCollected);
			outpostVisitEntry.SecondObjectiveCompleted = ((combatModel.PvPMissionType == PvPMissionType.PVPMultiFlag) ? combatModel.IsPvPLootCollected : combatModel.IsPvPFlagCollected);
			outpostVisitEntry.DefendersObjectiveCompleted = combatModel.IsPvpDefendersKilled;
			AddAttackOutpostVisitLog(outpostVisitEntry);
			base.manager.UpdateOutpostLeaderboardEntry();
		}

		public PvPDefenderSaveType ResolvePvPResult()
		{
			PvPDefenderSaveType result = PvPDefenderSaveType.None;
			OutpostCombat outpostCombat = Combat.OutpostCombat;
			if (outpostCombat == null && Combat.IsPVPMission)
			{
				base.Debug.LogError("Can not resolve PVP result - OutpostCombat is NULL");
				return PvPDefenderSaveType.None;
			}
			if (Combat != null && outpostCombat != null && !outpostCombat.PVPResultResolved)
			{
				outpostCombat.PVPResultResolved = true;
				string outpostVIsitId = ModelHelpers.MD5Sum(HashedId + outpostCombat.DefenderHashedId + UtcTimeStamp);
				result = UpdateDefenderAfterPvP(outpostVIsitId);
				UpdateAttackerAfterPvP(outpostVIsitId);
				int finalResourcesStolen = GetFinalResourcesStolen(outpostCombat.TradeGoodsGain);
				base.manager.Player.Blackboard.IncreaseCounter("Counter.CompletedOutpostAttacks");
				if (base.manager.Player.Blackboard.GetCounter("Counter.CompletedOutpostAttacks") == 1)
				{
					OutpostTutorialStateForAnalytics analyticsState = OutpostTutorialStateForAnalytics.FirstAttackDone;
					base.manager.Metrics.AddStart().AddOutpostTutorial(analyticsState).Send();
				}
				int num = Math.Max(finalResourcesStolen, 0);
				if (num > 0)
				{
					base.manager.Metrics.AddFind().AddResources(CurrencyType.Outpost, num, GetCurrency(CurrencyType.Outpost).LastAdded).AddMission()
						.AddMissionType()
						.Send();
				}
			}
			else if (Combat != null && Combat.MissionResult == ECombatResult.Successful)
			{
				string outpostTutorialMissionId = base.gameEconomyData.ConfigData.OutpostTutorialMissionId;
				if (outpostTutorialMissionId != null)
				{
					MapMissionModel missionModelForSpawnPoint = MapContainerModel.GetMissionModelForSpawnPoint(base.gameEconomyData.MissionSpawnPointData.FindFirstSpawnPointByMissionId(outpostTutorialMissionId));
					if (missionModelForSpawnPoint != null)
					{
						MissionData missionData = base.gameEconomyData.GetMissionData(missionModelForSpawnPoint.MissionId);
						if (missionData != null && base.manager.SelectedMissionData == missionData)
						{
							int outpostTutorialInfluenceReward = GetOutpostTutorialInfluenceReward();
							int outpostTutorialTradeGoodsReward = GetOutpostTutorialTradeGoodsReward();
							SetRankingScore(RankingScore + outpostTutorialInfluenceReward);
							GetCurrency(CurrencyType.Outpost).Add(outpostTutorialTradeGoodsReward);
						}
					}
				}
			}
			return result;
		}

		public TWDModelResult SetChatTime(long time)
		{
			LastReadChatTime = time;
			return TWDModelResult.OK;
		}

		public TWDModelResult BuyTradeShopRefresh()
		{
			if (RefreshTradeSlotsAndItems())
			{
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}

		public bool RefreshTradeSlotsAndItems()
		{
			if (base.gameEconomyData.TradeSlotDefinitions != null)
			{
				CurrentTradeSlots = new List<TradeSlotInfo>();
				int num = 0;
				for (int i = 0; i < base.gameEconomyData.TradeSlotDefinitions.Length; i++)
				{
					TradeSlotDefinition tradeSlotDefinition = base.gameEconomyData.TradeSlotDefinitions[i];
					if (!ActivityManager.CheckTradeSlotEventControl(tradeSlotDefinition))
					{
						continue;
					}
					bool ruleRejected = false;
					TradeDefinition randomTradeDefinition = GetRandomTradeDefinition(tradeSlotDefinition, out ruleRejected);
					if (randomTradeDefinition == null)
					{
						if (!ruleRejected)
						{
							base.Debug.LogError("Could not find any trade definition suitable for trade slot " + tradeSlotDefinition.SlotId);
						}
						continue;
					}
					TradeSlotInfo tradeSlotInfo = new TradeSlotInfo();
					TradeSlotDefinition tradeSlotDefinition2 = new TradeSlotDefinition();
					tradeSlotDefinition2.SlotId = tradeSlotDefinition.SlotId;
					tradeSlotDefinition2.PriceCategory = tradeSlotDefinition.PriceCategory;
					tradeSlotDefinition2.Bucket = tradeSlotDefinition.Bucket;
					tradeSlotDefinition2.UnlockRequirement = tradeSlotDefinition.UnlockRequirement;
					tradeSlotDefinition2.GoldRepeat = tradeSlotDefinition.GoldRepeat;
					tradeSlotInfo.PurchaseCount = (HasBoughtSpecialOffer(randomTradeDefinition.UniqueId) ? 1 : 0);
					tradeSlotDefinition2.Setup();
					if (tradeSlotDefinition2.CurrencyUnlock == CurrencyType.Diamonds && tradeSlotDefinition2.CurrencyUnlockAmount > 0)
					{
						num = (tradeSlotInfo.GoldUnlockSlot = num + 1);
					}
					TradeDefinition tradeDefinition = new TradeDefinition(randomTradeDefinition);
					tradeDefinition.Setup();
					tradeSlotInfo.SlotDefinition = tradeSlotDefinition2;
					tradeSlotInfo.CurrentTradeDefinition = tradeDefinition;
					CurrentTradeSlots.Add(tradeSlotInfo);
				}
				LastTradeShopRefreshTime = LifeTime;
				CurrentTradeSlots.StableSort((TradeSlotInfo a, TradeSlotInfo b) => a.SlotDefinition.SlotId.CompareTo(b.SlotDefinition.SlotId));
				NotifyChange("TradeShopRefreshed");
				base.manager.Debug.Log("RefreshTradeSlotsAndItems: Refreshed at " + LastTradeShopRefreshTime);
			}
			return true;
		}

		public bool HasBoughtSpecialOffer(int id)
		{
			if (BoughtTradeCrateTimeLimitedOffers != null)
			{
				for (int i = 0; i < BoughtTradeCrateTimeLimitedOffers.Count; i++)
				{
					if (BoughtTradeCrateTimeLimitedOffers[i] == id)
					{
						return true;
					}
				}
			}
			return false;
		}

		public TradeDefinition GetRandomTradeDefinition(TradeSlotDefinition slot, out bool ruleRejected)
		{
			TradeDefinition result = null;
			ruleRejected = false;
			List<TradeDefinition> list = new List<TradeDefinition>();
			for (int i = 0; i < base.gameEconomyData.TradeDefinitions.Length; i++)
			{
				TradeDefinition tradeDefinition = base.gameEconomyData.TradeDefinitions[i];
				if (!tradeDefinition.IsAvailable(UtcTimeStamp))
				{
					ruleRejected = true;
					continue;
				}
				int num = 0;
				if (Camp != null)
				{
					num = Camp.GetCouncilLevel();
				}
				if (tradeDefinition.CouncilLevelRequired > num)
				{
					ruleRejected = true;
					continue;
				}
				if (tradeDefinition.SoldItems.RewardsList[0] is RewardOutfit rewardOutfit && SurvivorContainer.HasOutfit(rewardOutfit.PreferredOrder[0]))
				{
					ruleRejected = true;
					continue;
				}
				if (tradeDefinition.SoldItems.RewardsList[0] is RewardCurrency rewardCurrency)
				{
					SurvivorClass survivorClassForUpgradeCurrencyType = SurvivorModel.GetSurvivorClassForUpgradeCurrencyType(rewardCurrency.CurrencyType);
					if (survivorClassForUpgradeCurrencyType != SurvivorClass.None && !SurvivorContainer.IsSurvivorClassUnlocked(survivorClassForUpgradeCurrencyType))
					{
						ruleRejected = true;
						continue;
					}
				}
				if (tradeDefinition.SoldItems.RewardsList[0] is RewardEquipment rewardEquipment)
				{
					EquipmentDefinition equipmentDefinition = base.gameEconomyData.GetEquipmentDefinition(rewardEquipment.EquipmentId);
					if (equipmentDefinition == null || !SurvivorContainer.IsSurvivorClassUnlocked(equipmentDefinition.SurvivorClass))
					{
						ruleRejected = true;
						continue;
					}
				}
				for (int j = 0; j < slot.Buckets.Count; j++)
				{
					if (!(tradeDefinition.BucketId == slot.Buckets[j]))
					{
						continue;
					}
					int num2 = 0;
					for (int k = 0; k < CurrentTradeSlots.Count; k++)
					{
						if (CurrentTradeSlots[k].CurrentTradeDefinition.UniqueId == tradeDefinition.UniqueId)
						{
							num2 = int.MaxValue;
							break;
						}
						if (CurrentTradeSlots[k].CurrentTradeDefinition.TagName == tradeDefinition.TagName)
						{
							num2++;
						}
					}
					if (num2 < tradeDefinition.TagAmount)
					{
						list.Add(tradeDefinition);
					}
				}
			}
			if (list.Count > 0)
			{
				ruleRejected = false;
				result = PlayerRandom.GetRandomElement(list.ToArray());
			}
			return result;
		}

		public TradeSlotInfo GetCurrentTradeSlotDefinitionById(int id)
		{
			if (CurrentTradeSlots != null)
			{
				for (int i = 0; i < CurrentTradeSlots.Count; i++)
				{
					if (CurrentTradeSlots[i].SlotDefinition.SlotId == id)
					{
						return CurrentTradeSlots[i];
					}
				}
			}
			return null;
		}

		public TWDModelResult UnlockNextTradeCrateSlot()
		{
			BoughtTradeCrateSlotAmount++;
			NotifyChange("TradeShopSlotBought");
			return TWDModelResult.OK;
		}

		public void AddBoughtTimeLimitedTradeOffer(int id)
		{
			if (BoughtTradeCrateTimeLimitedOffers == null)
			{
				BoughtTradeCrateTimeLimitedOffers = new List<int>();
			}
			BoughtTradeCrateTimeLimitedOffers.Add(id);
		}

		[ModelAvailableTimer]
		public long GetTimeLeftToTradeShopRefresh()
		{
			return base.gameEconomyData.ConfigData.TradeShopRefreshInterval * 1000 - (LifeTime - LastTradeShopRefreshTime);
		}

		public int GetFreeTradeShopItemsCount()
		{
			int num = 0;
			for (int i = 0; i < CurrentTradeSlots.Count; i++)
			{
				TradeSlotInfo tradeSlotInfo = CurrentTradeSlots[i];
				if (!tradeSlotInfo.Bought && tradeSlotInfo.CurrentTradeDefinition.PriceNormalAmount == 0 && tradeSlotInfo.CurrentTradeDefinition.PriceDiscountAmount == 0 && BoughtTradeCrateSlotAmount >= tradeSlotInfo.GoldUnlockSlot && (string.IsNullOrEmpty(tradeSlotInfo.SlotDefinition.UnlockRequirement) || tradeSlotInfo.SlotDefinition.CurrencyUnlock != CurrencyType.None || tradeSlotInfo.SlotDefinition.CurrencyUnlockAmount <= RankingScore))
				{
					num++;
				}
			}
			return num;
		}

		public void BeginSession()
		{
			if (SessionHistory == null)
			{
				SessionHistory = new List<SessionEntry>();
			}
			if (CombatHistory == null)
			{
				CombatHistory = new List<CombatHistoryEntry>();
			}
			int num = (int)(LifeTime / 86400000);
			if (num != LastSessionDay)
			{
				LastSessionDay = num;
				Blackboard.IncreaseCounter("Counter.SessionDaysPlayed");
			}
			long num2 = LifeTime - 604800000;
			bool isWeekBreak = SessionHistory.Count > 0 && SessionHistory[SessionHistory.Count - 1].StartTime < num2;
			int num3 = 0;
			for (int i = 0; i < SessionHistory.Count && SessionHistory[i].StartTime <= num2; i++)
			{
				num3++;
			}
			SessionHistory.RemoveRange(0, num3);
			if (SessionHistory.Count == 0 || (SessionHistory.Count > 0 && SessionHistory[SessionHistory.Count - 1].Length > 0))
			{
				SessionHistory.Add(new SessionEntry
				{
					StartTime = LifeTime,
					Length = 0L,
					IsWeekBreak = isWeekBreak
				});
			}
		}

		public void EndSession()
		{
			if (SessionHistory == null)
			{
				SessionHistory = new List<SessionEntry>();
			}
			if (SessionHistory.Count > 0 && SessionHistory[SessionHistory.Count - 1].Length == 0L)
			{
				SessionHistory[SessionHistory.Count - 1].Length = LifeTime - SessionHistory[SessionHistory.Count - 1].StartTime;
			}
		}

		public void SaveCombatHistory(bool pvp)
		{
			if (CombatHistory == null)
			{
				CombatHistory = new List<CombatHistoryEntry>();
			}
			long num = LifeTime - 604800000;
			int num2 = 0;
			for (int i = 0; i < CombatHistory.Count && CombatHistory[i].Time <= num; i++)
			{
				num2++;
			}
			CombatHistory.RemoveRange(0, num2);
			CombatHistory.Add(new CombatHistoryEntry
			{
				Time = LifeTime,
				IsPVP = pvp
			});
		}

		public IMapMissionModel GetAttackTargetMissionModel()
		{
			if (_attackTargetMissionModel == null && MapContainerModel != null)
			{
				_attackTargetMissionModel = MapContainerModel.AttackTargetMissionModel;
			}
			if (_attackTargetMissionModel == null && GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel != null)
			{
				_attackTargetMissionModel = GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel;
			}
			return _attackTargetMissionModel;
		}

		public void ResetIAttackTargetMapMission()
		{
			_attackTargetMissionModel = null;
		}

		public void SetGdprAction(string key, TimestampedActionResult actionResult)
		{
			GdprActions[key] = actionResult;
		}

		public bool HasTakenGdprAction(string key)
		{
			TimestampedActionResult value = null;
			if (GdprActions.TryGetValue(key, out value))
			{
				return value.ActionTaken;
			}
			return false;
		}

		public bool HasAcceptedGdprAction(string key)
		{
			TimestampedActionResult value = null;
			if (GdprActions.TryGetValue(key, out value))
			{
				return value.Accepted;
			}
			return false;
		}

		public void SetMarkedForDeletion(bool setToBeDeleted)
		{
			if (setToBeDeleted && MarkedForDeletion == 0L)
			{
				int markedForDeletionGracePeriodSec = base.manager.GameEconomyData.ConfigData.MarkedForDeletionGracePeriodSec;
				MarkedForDeletion = LifeTime + markedForDeletionGracePeriodSec * 1000;
			}
			else if (!setToBeDeleted)
			{
				MarkedForDeletion = 0L;
			}
		}

		public void ClearGuildRelatedData()
		{
			if (GvGSeasonModelPlayer.GuildWarModelPlayer != null)
			{
				GvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots.Clear();
			}
			if (base.gameEconomyData != null)
			{
				base.gameEconomyData.ClearCachedNextWarDefinition();
			}
		}

		public string GetDebugInfo()
		{
			string text = $"PlayerSeasonID : {GvGSeasonModelPlayer.StartedGvGSeasonId}, PlayerWarID : {GvGSeasonModelPlayer.GuildWarModelPlayer.StartedWarId}, PlayerBattleTimeSlot : {GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentBattleTimeSlot}, PlayerBattleId : {GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentBattleId}";
			GuildModel guildModel = base.manager.Player.GuildModel;
			if (guildModel != null)
			{
				string text2 = $" GuildSeasonID : {guildModel.GvGSeasonModel.SeasonDefinitionId}, GuildWarID : {guildModel.GuildWarModel.WarDefinitionId}, GuildBattleTimeSlot : {guildModel.GuildWarModel.CurrentBattle.TimeSlot}, GuildBattleId : {guildModel.GuildWarModel.CurrentBattle.BattleId}";
				text += text2;
			}
			return text;
		}

		private void InitializeSupportModels()
		{
			if (EquippedSupportIds == null)
			{
				EquippedSupportIds = new string[3];
			}
			if (base.gameEconomyData.SupportDefinitionIds != null)
			{
				foreach (string id in base.gameEconomyData.SupportDefinitionIds)
				{
					if (SupportModels.All((SupportModel supportModel4) => supportModel4.SupportId != id))
					{
						SupportModel supportModel = new SupportModel(id);
						supportModel.SetManager(base.manager);
						SupportModels.Add(supportModel);
					}
				}
			}
			if (base.manager.GameEconomyData.ConfigData.SupportTalentUnlockToggle && CouncilLevel >= base.manager.GameEconomyData.ConfigData.SupportTalentUnlockAtCouncilLevel)
			{
				foreach (SupportModel supportModel4 in SupportModels)
				{
					if (!supportModel4.InitializedTalent)
					{
						supportModel4.InitializeTalentTrees();
					}
				}
			}
			supportModelsMap = new Dictionary<string, SupportModel>();
			foreach (SupportModel supportModel5 in SupportModels)
			{
				supportModelsMap[supportModel5.SupportId] = supportModel5;
				if (supportModel5.SupportTalentTreeModels == null)
				{
					supportModel5.SupportTalentTreeModels = new ModelList<SupportTalentTreeModel>();
					if (supportModel5.SupportTalentTreeModels.Manager == null)
					{
						supportModel5.SupportTalentTreeModels.SetManager(base.manager);
					}
				}
			}
		}

		public SupportModel GetSupportModel(string supportId)
		{
			if (string.IsNullOrEmpty(supportId))
			{
				return null;
			}
			if (supportModelsMap == null)
			{
				return SupportModels.FirstOrDefault((SupportModel model) => model.SupportId == supportId);
			}
			if (supportModelsMap.TryGetValue(supportId, out var value))
			{
				return value;
			}
			return null;
		}

		public long GetItemNumByName(string itemname)
		{
			if (Enum.TryParse<CurrencyType>(itemname, ignoreCase: true, out var currencyType))
			{
				return Currencies.Find((CurrencyModel x) => x.Type == currencyType).TotalValue;
			}
			if (Enum.TryParse<EquipmentModel.ConsumableType>(itemname, ignoreCase: true, out var result))
			{
				return Equipment.GetConsumablesOfType(result).Count;
			}
			if (itemname == "EquipToken")
			{
				return EquipTokenContainer.EquipTokenItems?.Sum((EquipTokenItemModel x) => x.OwnedTokensAmount) ?? 0;
			}
			return 0L;
		}

		public int GetEquippedSupportIndex(string id)
		{
			for (int i = 0; i < EquippedSupportIds.Length; i++)
			{
				if (id == EquippedSupportIds[i])
				{
					return i;
				}
			}
			return -1;
		}

		private void CombatApplyReadyForTrait()
		{
			if (Combat.MissionStarted)
			{
				return;
			}
			for (int i = 0; i < Combat.Survivors.Count; i++)
			{
				ActorModel actorModel = Combat.Survivors[i];
				if (actorModel.ChargeMeter != null && actorModel.IsHaveOverloadTrait())
				{
					actorModel.AddChargePoints(actorModel.Overload_ChargePointNum());
				}
			}
			List<ActorModel> allActors = combatModel.GetAllActors();
			for (int j = 0; j < allActors.Count; j++)
			{
				allActors[j]?.SetDeadlyFocusAI();
			}
			allActors = combatModel.GetAllActors();
			for (int k = 0; k < allActors.Count; k++)
			{
				allActors[k]?.ShadowedGuardAddChargeNum();
			}
			List<ActorModel> list = new List<ActorModel>(combatModel.Raiders.Models);
			list.AddRange(combatModel.Survivors.Models);
			for (int l = 0; l < list.Count; l++)
			{
				ActorModel actorModel2 = list[l];
				if (actorModel2 != null && actorModel2.HasAnyLevelTrait("LeaderBuffCitadel"))
				{
					actorModel2.ExecuteCitadelTrait();
				}
			}
		}

		private void CombatApplySurvivorManualHP()
		{
			if (Combat.MissionStarted || base.manager?.Player?.SurvivalManualManager == null)
			{
				return;
			}
			for (int i = 0; i < Combat.Survivors.Count; i++)
			{
				ActorModel actorModel = Combat.Survivors[i];
				FixedPoint fixedPoint = base.manager.Player.SurvivalManualManager.GetPrivateHp(actorModel) + base.manager.Player.SurvivalManualManager.GetSystemHP();
				FixedPoint fixedPoint2 = base.manager.Player.SurvivalManualManager.GetPrivateHpRatio(actorModel) + base.manager.Player.SurvivalManualManager.GetAttributeHpRatio();
				int num = actorModel.Hitpoints;
				int num2 = actorModel.MaxHitPoints;
				bool flag = false;
				if (fixedPoint > 0.0)
				{
					flag = true;
					num2 = Math.Max(num2 + (int)fixedPoint, num);
					num += (int)fixedPoint;
				}
				if (fixedPoint2 > 0.0)
				{
					flag = true;
					int num3 = num2;
					num2 = Math.Max(num2, (int)(num2 * (1L + fixedPoint2)));
					num = Math.Max(num + (num2 - num3), num);
				}
				if (flag)
				{
					actorModel.SetHitPoints(num, num2, setConfig: true);
					actorModel.MinHitpoints = actorModel.MaxHitPoints;
					actorModel.SetUpShieldHitPoints();
				}
			}
		}

		private void ApplyAttributeSystems()
		{
			CombatApplySupportTalent();
		}

		private void CombatApplySupportTalent()
		{
			if (Combat.MissionStarted || Combat.SupportManager == null || Combat.SupportManager.Supports == null || Combat.SupportManager.Supports.Count == 0)
			{
				return;
			}
			foreach (CombatSupportModel support in Combat.SupportManager.Supports)
			{
				if (support.AttachedSurvivor == null)
				{
					continue;
				}
				support.AttachedSurvivor.ActorAttributeContainer.SetSupportModel(support.SupportModel);
				if (support.SupportModel.SlotAssembledTalentIds != null && support.SupportModel.SlotAssembledTalentIds.Count > 0)
				{
					foreach (KeyValuePair<int, int> slotAssembledTalentId in support.SupportModel.SlotAssembledTalentIds)
					{
						SupportTalentDefinition supportTalentDefinitionById = base.manager.GameEconomyData.GetSupportTalentDefinitionById(slotAssembledTalentId.Value);
						support.AttachedSurvivor.AddTrait(supportTalentDefinitionById.TalentTrait);
						support.AttachedSurvivor.SupportTalentAssembledTraitRecords.Add(supportTalentDefinitionById.TalentTrait);
					}
				}
				ApplyCombatSupportHPAttributeData(support.AttachedSurvivor);
				ApplyCombatAttributeContainerAttributeData(support.AttachedSurvivor);
			}
		}

		private void ApplyCombatSupportHPAttributeData(ActorModel combatSupportAttachedSurvivor)
		{
			if (!Combat.MissionStarted && combatSupportAttachedSurvivor != null && combatSupportAttachedSurvivor.ActorAttributeContainer != null && combatSupportAttachedSurvivor.ActorAttributeContainer.SupportModel != null)
			{
				int num = combatSupportAttachedSurvivor.Hitpoints;
				int num2 = combatSupportAttachedSurvivor.MaxHitPoints;
				FixedPoint hP = combatSupportAttachedSurvivor.ActorAttributeContainer.SupportModel.GetHP();
				bool flag = false;
				if (hP > 0.0)
				{
					flag = true;
					num += (int)hP;
					num2 = Math.Max(num2 + (int)hP, num);
				}
				if (flag)
				{
					combatSupportAttachedSurvivor.SetHitPoints(num, num2, setConfig: true);
					combatSupportAttachedSurvivor.MinHitpoints = combatSupportAttachedSurvivor.MaxHitPoints;
					combatSupportAttachedSurvivor.SetUpShieldHitPoints();
				}
			}
		}

		private void ApplyCombatAttributeContainerAttributeData(ActorModel combatSupportAttachedSurvivor)
		{
			if (Combat.MissionStarted || combatSupportAttachedSurvivor == null)
			{
				return;
			}
			Dictionary<AttributeType, FixedPoint> dictionary = combatSupportAttachedSurvivor.CombatAttributeSnapshots ?? new Dictionary<AttributeType, FixedPoint>();
			for (int i = 100; i < 109; i++)
			{
				AttributeType attributeType = (AttributeType)i;
				if (dictionary.ContainsKey(attributeType))
				{
					dictionary[attributeType] += combatSupportAttachedSurvivor.ActorAttributeContainer.GetAttributeValueByAttributeType(attributeType);
				}
				else
				{
					dictionary[attributeType] = combatSupportAttachedSurvivor.ActorAttributeContainer.GetAttributeValueByAttributeType(attributeType);
				}
			}
			combatSupportAttachedSurvivor.CombatAttributeSnapshots = dictionary;
		}



		#region myparams
		private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
		#endregion

		#region mycode
		public void SetCurrency(CurrencyType currencyType, int amount)
		{
			bool IsNegative = amount < 0;
			int amountAbs = Math.Abs(amount);

			var originValue = Currencies[(int)currencyType].Value;

			if (IsNegative)
			{
				Currencies[(int)currencyType].Subtract(originValue - amountAbs > 0 ? amountAbs : originValue);
			}
			else
			{
				Currencies[(int)currencyType].Add(amount);
			}
		}

		public void SetManagers(TWDModelManager manager)
		{
			AbilityManager = new AbilityManagerModel();
			AbilityManager.SetManager(manager);
			AbilityManager.Initialize();

			ActivityManager = new ActivityManager(manager);

			if (EquipPrizeWheelModel == null)
			{
				EquipPrizeWheelModel = new EquipPrizeWheelModel();
				EquipPrizeWheelModel.SetManager(manager);
				EquipPrizeWheelModel.Initialize();
			}
			if (ModSkillManager == null)
			{
				ModSkillManager = new ModSkillManager();
				ModSkillManager.SetManager(manager);
				ModSkillManager.Initialize();
			}
			if (LootManager == null)
			{
				LootManager = new LootManagerModel();
				LootManager.SetManager(manager);
				LootManager.Initialize();
			}
		}
		#endregion
	}
}
