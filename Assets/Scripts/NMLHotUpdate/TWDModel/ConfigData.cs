using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ConfigData
	{
		public int ged_version;

		public int GridScale;

		public string DependencyLevelBuilding;

		public bool UseOnlyLocalLocalizationFiles;

		public bool DisableFacebookAnalytics;

		public List<string> MobileAppTrackingEvents;

		[GEDType(GEDSpecialType.UrlHTTPS)]
		public string ReplayUrl;

		[GEDType(GEDSpecialType.UrlHTTPS)]
		public string CDNBaseUrl;

		public int ThreatIncreasePerTurn;

		public int ThreatWaveLevelIncrease;

		public int ThreatWaveInitialLevelOffset;

		public int ThreatWaveLevelIncreasePvP;

		public int ThreatWaveInitialLevelOffsetPvP;

		public int MissionMaxEnemiesKillGivingXP;

		public int MissionKillAfterMaxGivenXP;

		public int CancelUpgradeRefundPercentage;

		public int StruggleBaseThreshold;

		public int StruggleBaseChance;

		public int DeadlyMissionProbability;

		public int ThreeRewardsCost;

		public int ThreeRewardsLootKeyCost;

		public int LootKeySoftCap;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int LootKeyRefreshRate;

		public int ProductionPercentShowCollect;

		public int InjuryCriticalBelowHealthPercentage;

		public int InjuryMajorBelowHealthPercentage;

		public int InjuryMinorBelowHealthPercentage;

		public int EquipmentGenerationLevel;

		public List<string> UISupportedLanguages;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int CampDefenseWaveDelay;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int CampDefenseSpawnDelay;

		public int CampDefenseWalkerCountPerWave;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int VideoAdTimeInterval;

		public int VideoAdLimit;

		public int NoAdsWaitingDays;

		public string NoAdsCompensationReward;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int VideoAdTimeIntervalRewardScreen;

		public int VideoAdLimitRewardScreen;

		public int VideoAdRewardScreenLimit;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int ReplayTokensRechargeSpeed;

		public int ReplayTokensRechargePrice;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int ChallengeMissionRespawnTime;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int ProceduralMissionRespawnTime;

		public bool SocialFeaturesEnabled;

		public List<string> DisabledHotfixes;

		public List<string> SocialFeaturesCountryFilter;

		public string AppStoreUrl;

		public bool HideMissionGoldSilverChest;

		public int ForceCouncilMaxLevel;

		public bool EnableCheckPlayerModelCommand;

		public int DebugPostLevel;

		public int InitialDiamonds;

		public List<string> InitialSurvivors;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int BundleShowInterval;

		public int BundleMaxPromtAmount;

		public List<string> DisabledEpisodes;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int BannerShowInterval;

		public int NumEpisodesContainingGrindMissions;

		public bool EnableAchievements;

		public List<int> TimeToDiamondsConversion;

		public List<int> SuppliesToDiamondsConversion;

		public List<int> SPToDiamondsConversion;

		public List<int> PhoneToDiamondsConversion;

		public List<int> OutpostToDiamondsConversion;

		public bool CanRenameSurvivors;

		public bool BetaFlag_Outfits;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int FakeChallengesInterval;

		public List<int> FakeChallengesDetailMapId;

		public List<int> FakeApocalypticChallengesDetailMapId;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int FakeSurvivalInterval;

		public List<int> FakeSurvivalDetailMapId;

		public bool DisableOutpostLoadingScreen;

		public bool DisableOutpostSeasons;

		public bool OutpostEnabled;

		public int OutpostLevelDataVersion;

		public bool OutpostMatchMakingEnabled;

		public bool OutpostMatchMakingFakeNamesEnabled;

		public int OutpostMatchMakingBracketSize;

		public List<int> OutpostMatchMakingLevelRange;

		public List<int> OutpostMatchMakingInfluenceRange;

		public FixedPoint OutpostMMRCampLevelMultiplier;

		public FixedPoint OutpostMMRInfluenceMultiplier;

		public int OutpostMatchMakingNextCost;

		public int OutpostWalkerNormalDeploymentCost;

		public int OutpostWalkerTankDeploymentCost;

		public int OutpostWalkerArmoredDeploymentCost;

		public int OutpostSurvivorDeploymentCost;

		public int OutpostResourceContainerDeploymentCost;

		public int OutpostFlagDeploymentCost;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int OutpostAttackDeactivationCooldown;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int OutpostDefeatedShieldDuration;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int OutpostResumeTimeLimit;

		public int MaxMissionTime;

		public int OutpostVisitLogSize;

		public int OutpostTutorialSpawnPointGroupId;

		public int OutpostTutorialSpawnPointId;

		public string OutpostTutorialMissionId;

		public int OutpostUnlockEditingAtBuilingLevel;

		public int OutpostRaidGasCost;

		public int InitialRankingScore;

		public int MinRankingScore;

		public int MaxRankingScore;

		public int OutpostLevelMatchMakingMultiplier;

		public int MatchMakingVersion;

		public int OutpostTutorialResourceReward;

		public int OutpostTutorialInfluenceReward;

		public int OutpostMinimumInfluenceReward;

		public int OutpostStealResourcePercentage;

		public int OutpostDefenderStealResourceMultiplierPercentage;

		public int OutpostFakeOpponentResourceRewardPercentage;

		public int OutpostResourceProtectedAmount;

		public bool OutpostResourceProtectionByPercentage;

		public int OutpostCratesCompletedResourcePercentage;

		public int OutpostCratesCompletedInfluencePercentage;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long OutpostCratesCompletedProductionHalted;

		public int OutpostFlagCompletedResourcePercentage;

		public int OutpostFlagCompletedInfluencePercentage;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long OutpostFlagCompletedProductionHalted;

		public int OutpostDefendersCompletedResourcePercentage;

		public int OutpostDefendersCompletedInfluencePercentage;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long OutpostDefendersCompletedProductionHalted;

		public int OutpostDefendersWonInfluencePercentage;

		public bool OutpostResourceProtectionEnabledForAttacker;

		public int WalkerCageInitialNormalWalkerCount;

		public string OutpostMinimumWalkerChangeDate;

		public int OutpostWalkersToAddIfCreatedWithDifferentMinimum;

		public int RankingScoreMaxDifference;

		public int RankingScoreResultChangePercentage;

		public FixedPoint InfluenceWeightOnMatchMakingSort;

		public int MaxItemCount;

		public int AutoScrapItemThreshold;

		public bool AskForGore;

		public List<string> GoreDisabledCountryCodes;

		public bool VoiceOverEnabled;

		public bool ForceQueueMessageWhenSavingOpponent;

		public bool EnableFBLinkTrackingOnClient;

		public int MaxDailyQuests;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int DailyQuestSpawnInterval;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int DailyQuestDiscardCooldown;

		public bool IAPCardAscending;

		public bool CanBuyRadioPhones;

		public float ChallengeMissionLevelMultiplier;

		public float ChallengeGasCostMultiplier;

		public int PhoneSilverUnlockAtLevel;

		public int PhoneGoldUnlockAtLevel;

		public List<string> GuildPurposeTypes;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int GuildPurposeChangeInterval;

		public string GuildLeaderboardDayZero;

		public int GuildLeaderboardScoreBufferSize;

		public int GuildLeaderboardRotationInterval;

		public int HarderEpisodeNumberLevels;

		public int HarderEpisodeLevelIncrease;

		public int HarderEpisodeGrindLevelIncrease;

		public bool DisableLinkDevice;

		private long guildLeaderboardDayZeroSeconds;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long GuildGiftExpireTimer;

		public long GuildGiftCooldownTimer;

		public int GuildGiftGoldPrice;

		public int GuildGiftSingleGoldValue;

		public bool GuildGiftsEnabled;

		public bool GuildAdEnabled;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long GuildAdExpirationTime;

		public int GuildAdBucketCount;

		public int GuildAdGoldPrice;

		public string GuildAdSearchQuery;

		public string GuildDefaultSearchQuery;

		public string GuildEmptySearchQuery;

		public string GuildSuggestionsNewQuery;

		public string GuildSuggestionsEarlyLevelQuery;

		public string GuildSuggestionsSimilarLevelQuery;

		public string GuildSuggestionsSameCountryQuery;

		public string GuildSuggestionsSameLargeCountryQuery;

		public string GuildSuggestionsFallbackQuery;

		public int GuildSuggestionsEarlyLevelLimit;

		public int GuildSuggestionsSimilarLevelThreshold;

		public List<string> GuildSuggestionsLargeCountries;

		public List<string> bufatuzhi;

		public List<string> EquipmentSkillSuggestionTags;

		public string Hyperlink_Discord_EquipSkillSuggestion;

		public int GuildSuggestionsRecentDays;

		public bool GuildSuggestionPopupEnabled;

		public long GuildSuggestionPopupShowBasePeriod;

		public int GuildSuggestionPopupQueryMaxDelay;

		public int GuildSuggestionPopupCouncilLevelMin;

		public int GuildSuggestionPopupQueryIterationsMax;

		public string GuildSuggestionPopupEarlyLevelLargeCountryQuery1;

		public string GuildSuggestionPopupEarlyLevelLargeCountryQuery2;

		public string GuildSuggestionPopupEarlyLevelSmallCountryQuery1;

		public string GuildSuggestionPopupEarlyLevelSmallCountryQuery2;

		public string GuildSuggestionPopupLateLevelLargeCountryQuery1;

		public string GuildSuggestionPopupLateLevelLargeCountryQuery2;

		public string GuildSuggestionPopupLateLevelSmallCountryQuery1;

		public string GuildSuggestionPopupLateLevelSmallCountryQuery2;

		public bool TradeCratesEnabled;

		public bool TradeCratesGoldConversionAllowed;

		public List<int> NotifyPlayersAfterDays;

		public int NotifyPlayersAtLocalHour;

		public List<string> NotifyPlayersLocalizationKey;

		public FixedPoint HalfCoverModifier;

		public FixedPoint CoverAngle;

		public bool EnableCombatGridSnap;

		public FixedPoint CombatNormalSpeed;

		public FixedPoint CombatHighSpeed;

		public int TutorialSpawnPointGroupId;

		public int ChallengesUnlockAtCouncilLevel;

		public string ChallengesUnlockAtAfterTutorialPartId;

		public int SurvivalUnlockAtCouncilLevel;

		public int SurvivalHardUnlockAtCouncilLevel;

		public int SurvivalNightmareUnlockAtCouncilLevel;

		public string SurvivalNormalRewardPreviewIcons;

		public string SurvivalHardRewardPreviewIcons;

		public string SurvivalNightmareRewardPreviewIcons;

		public string SurvivalUnlockAtAfterTutorialPartId;

		public int SurvivalRestCost;

		public int SurvivalRestEffectPercentage;

		public int SurvivalRestartCost;

		public int SurvivalDoubleRewardsCost;

		public string SurvivorClassUnlockOrder;

		public string PCPlatformType;

		public List<string> RadioCallBundlesToShow;

		public bool ShowClassIntroVideos;

		public string IntroVideoScout;

		public string IntroVideoBruiser;

		public string IntroVideoHunter;

		public string IntroVideoWarrior;

		public string IntroVideoShooter;

		public string IntroVideoAssault;

		public PromoCampaignType CurrentCampaign;

		public bool IgnoreRuntimeExceptions;

		public bool HockeyAppPluginEnabled;

		public int TokenSpentRefundPercentage;

		public string DefaultStaticMissionReward;

		public string OutOfGasPopupBundle;

		public string UpdateInfoVideoUrl;

		public int MaxRarityLevel;

		public int ShareUnlockRewardTokenAmount;

		public int ForcedDisplayForcedDisplayLimit;

		public string RewardedCrossbowForMigrationID;

		public List<long> FreeCallTimeMs;

		public List<int> FreeCallMaxStackable;

		public int MinIAPPriceGift;

		public int MaxPhonesUsedToGetGlenn;

		public int MaxCrappyCallsInARowRegular;

		public int MaxCrappyCallsInARowSilver;

		public int MaxCrappyCallsInARowGold;

		public bool SkipOutpostMatchPreview;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int TradeShopRefreshInterval;

		public int TradeShopRefreshCost;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int FreeCallNotificationDelay;

		public int OutpostUnlockAtCouncilLevel;

		public int GrindMissionGasCostBase;

		public int GrindMissionGasCostDivider;

		public List<string> GrindMissionMaps;

		public List<int> GrindMissionMinPlayerLevels;

		public List<int> GrindMissionMaxPlayerLevels;

		public int SeasonMissionGasPrice;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int SeasonMissionGateTime;

		public List<int> SeasonTrialDifficultyLevels;

		public CurrencyType Season7RewardHero;

		public bool TradeGoodShopRestockNotificationEnabled;

		public int WeeklyEventProbabilityAllMissionGoldBoxes;

		public int WeeklyEventProbabilityAllMissionSilverBoxes;

		public SurvivorClass WeeklyEventClassMoment;

		public SurvivorClass WeeklyEventClassSurvivorUpgrade5s;

		public bool WeeklyEventAllSurvivorUpgrade5s;

		public SurvivorClass WeeklyEventClassEquipmentUpgrade5s;

		public bool WeeklyEventAllEquipmentUpgrade5s;

		public SurvivorClass WeeklyEventClassHealTimeReduction;

		public bool WeeklyEventAllHealTimeReduction;

		public int WeeklyEventClassHealTimeReductionPercentage;

		public int WeeklyEventStartThreatIncreasePercentage;

		public string QuizItemId;

		public int QuizAnswerIndex;

		public List<FixedPoint> HealingTimeModifiers;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long ForceReloadTimeout;

		public FixedPoint HeroSPMultiplier;

		public int ChallengeDifficultyTopSurvivorsAmount;

		public int ChallengeDifficultySurvivorRarityThreshold;

		public int ChallengeDifficultyCyclesAmountToRecalculatePTS;

		public FixedPoint ChallengeDifficultyStartDifficultyMultiplier;

		public int ChallengeDifficultyUnderThreshold;

		public int ChallengeDifficultyCycleNormalSpeed;

		public FixedPoint ChallengeDifficultyCycleLowLevelPTSRatio;

		public int ChallengeDifficultyEndBrakeLevel;

		public int ChallengeDifficultyEndBrakeNumCycles;

		public int ChallengeDifficultyHardLimit;

		public int CurrentChallengeLeaderboardMaxSize;

		public int ChallangeMasterMissionCouncilLevelUnlock;

		public int ChallengeMasterMissionDifficultyOffset;

		public int ChallengeMasterMissionGasCostMultiplier;

		public int ChallengeRoundCap;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int ChallengeRoundTimer;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int ChallengeRoundTimerPeriod;

		public int ChallengeTimerFreeCount;

		public int ChallengeMaxPTS;

		public List<FixedPoint> ChallengePersonalHighScoreRatios;

		public List<int> ChallengeMinimumPersonalHighScore;

		public FixedPoint ChallengeGuildAchieverTopPlayersRatio;

		public int ChallengeGuildAchieverMinimumMembers;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long PlayerNameChangeMinTime;

		public int PlayerNameChangeMaxTimes;

		public string UpdateGift;

		public string VersionValidUntil;

		public string VersionUpdateContentEntryId;

		public int SeasonVersion;

		public bool EnableDoubleXPEventUI;

		public int MaxPromptedUnlocksPerActor;

		public List<int> ComponentRarityPointValues;

		public PortraitManagerModeType PortraitManagerMode;

		public int BadgeReclaimCost;

		public int BadgeSetBonus;

		public int MaxSimilarBadgeCount;

		public FixedPoint MaximumDamageReduction;

		public FixedPoint MaximumCriticalChance;

		public FixedPoint MaximumStunResistance;

		public int MaxBadgeInventorySize;

		public bool EnableIapConfirmList;

		public int NetworkConnectivityTimeout;

		public int DailyQuestsVersion;

		public string DailyQuestsResetTimeUtc;

		public string DailyQuestsTimeWrapThreshold24h;

		private TimeSpan dailyQuestsResetTime;

		private TimeSpan dailyQuestsExtraTimeWrapThreshold24h;

		public bool EnableHeroUnlockInMultiCardCall;

		public string ClientAnalyticsBlackList;

		public bool DamageVariation;

		public string TermsOfServiceURL;

		public string PrivacyPolicyURL;

		public string FairPlayPolicyURL;

		public string PlayerHubSocialFacebookWeb;

		public string PlayerHubSocialFacebookApp;

		public string PlayerHubSocialInsWeb;

		public string PlayerHubSocialInsApp;

		public string PlayerHubSocialTwitterWeb;

		public string PlayerHubSocialTwitterApp;

		public string PlayerHubSocialDiscussWeb;

		public string PlayerHubSocialDiscordWeb;

		public string PlayerHubSocialDiscordApp;

		public int GuildUnlockAtCouncilLevel;

		public bool OpenBananaButtonOnApp;

		public bool OpenBananaButtonOnAppIOS;

		public string BananaURL;

		public string BananaStagingURL;

		public string BananaTestURL;

		public string BananaDevURL;

		public string MyDataHelpshiftPublishID;

		public string MyDataHelpshiftKoreanPublishID;

		public string ZendeskChannelID;

		public bool GdprPrivacyPolicyChanged;

		public bool GdprAskCookieConsent;

		public bool GdprTosChanged;

		public float LoadingAdsPopupTimeout;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int MarkedForDeletionGracePeriodSec;

		public string MarkedForDeletionMagicWords;

		public int HeroPreviewSurvivorLevel;

		public int HeroPreviewSurvivorRarityLevel;

		public int EquipmentLevelUpTokenMinimumRarity;

		public int MinArmorReductionPercentage;

		public bool SetDefaultFPSTo60;

		public bool CombatGridStateByDefault;

		public long DailyLoginCalendarRefreshRate;

		public int DailyLoginCalendarMaxCouncilLevel;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long GvGDefendersCooldown;

		public string BlackMarketRefreshTimerSlot1;

		public string BlackMarketRefreshTimerSlot2;

		public string BlackMarketRefreshTimerSlot3;

		public int BlackMarketRefreshCost;

		public int BlackMarketHeroHistorySize;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long BlackMarketPlayerRefreshLockTime;

		public bool SurvivorsStartCharged;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long LeaderInactivityTimeThreshold;

		public bool IDFAVariantPrefab;

		public string IDFAPopupHeader;

		public string IDFAPopupParagraph1;

		public string IDFAPopupParagraph2;

		public string IDFAPopupParagraph3;

		public FixedPoint AdsBuildingSpeedUpMultiplier;

		public bool AdsBuildingSpeedUpEnabled;

		public bool AdsBlackMarketRefreshEnabled;

		public int ChallengeNightMareStartRound;

		public FixedPoint EnemySurvivorsMaxRange;

		public long GWKickSoftBanDurationMinutes;

		public bool IngameBanana;

		public bool IngameBananaIOS;

		public string IngameBananaImage;

		public string IngameBananaImageIOS;

		public string ShareToDiscordReward;

		public string ShareToDiscordRewardImage;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long ChangeGuildNameColdTime;

		public int GuildNameChangeCost;

		public string BeginerBPcompensate;

		public string NormalBPcompensate;

		public int SpeedUpHourConfirm;

		public bool BananaButtonSwitch;

		public bool BananaButtonSwitchIOS;

		public bool BundleButtonSwitch;

		public bool BundleButtonSwitchIOS;

		public string BananaTime;

		public string BananaTimeIOS;

		public string BananaEnterButtonIcon;

		public string BananaEnterButtonIconIOS;

		public string BananaPopupImage;

		public string BananaPopupImageIOS;

		public string SwitchBackOldVersion;

		public string BundleButtonJump;

		public string BundleBananaImage;

		public string BundleBananaImageIOS;

		public string BundleBananaImageINPACK;

		public string BundleBananaImageIOSINPACK;

		public string IngameBananaImageINPACK;

		public string IngameBananaImageIOSINPACK;

		public bool NewBannerSwitchBundle;

		public bool NewBannerSwitchBundleIOS;

		public bool NewBannerSwitchTF;

		public bool NewBannerSwitchTFIOS;

		public int WeeklyChallengesApocalypticMapIdSwitchToPackage;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long BananaPopupFreLong;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long BananaPopupFreLongIOS;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long BananaPopupFreShort;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long BananaPopupFreShortIOS;

		public int BananaPopupFreTimes;

		public int BananaPopupFreTimesIOS;

		public int BananaPopupLimitCouncilLevel;

		public int BananaPopupLimitCouncilLevelIOS;

		public int ChallengDebuffStartsRound;

		public int AvatarToGold;

		public int BorderrToGold;

		public int AvatarColorToGold;

		public int ChallengeApocalypticModeStartRound;

		public List<int> EquipTraitsRemodelToken;

		public int EquipTraitsRemodelGold;

		public int EquipmentRemodelRarity;

		public int GoldWeaponsBreakDownFragmentsNumber;

		public int ApocalypticWeaponsBreakDownFragmentsNumber;

		public int Star7WeaponsBreakDownFragmentsNumber;

		public bool Star7CanBeBreakDown;

		public int EquipmentDecompositionRarity;

		public int EquipmentBreakthroughsRarity;

		public int InitialHit;

		public int MinHit;

		public int EquipPrizeWheelLuckPoint;

		public int EquipPrizeWheelLuckPoint_GoldRadio;

		public double RechargeLimitWB;

		public double RechargeLimitWBIOS;

		public List<string> CountryControlios;

		public bool ClickInternal;

		public bool ClickInternalIOS;

		public bool FirstPic;

		public bool FirstPicIOS;

		public int ChallengeApocalypticMode90RoundRewards;

		public int ChallengeApocalypticMode90RoundRewardsStar;

		public int ChallengeApocalypticModeMaxRound;

		public int ApocalypticSkipTokenCap;

		public int SpeedUpBeyond;

		public bool ItemListSwitch;

		public string ItemListUnlockLimit;

		public bool ThreeDaySwich;

		public int ReloadTimer;

		public int RevertSaveTurn;

		public bool CustomBundleSwitch;

		public int CustomCouncilLevel;

		public int AngleValueCompareForBackAttack;

		public int NewbieCouncilUnlock;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long NewbieSevenQuestDuration;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long NewbieSevenQuestRefresh;

		public bool NewbieSevenQuestSwich;

		public List<int> MissionEnterOrderEndStory;

		public List<int> MissionEnterOrderEndSeason;

		public bool DisableOutpostHeroLimits;

		public bool CurrencyScientificNotation;

		public bool WeeklyChallengeWarZone;

		public int LeaderboardDisplayLevel;

		public bool LastStandWarZoneSwitch;

		public bool ControlResourceBundle;

		public bool ResourceBundleSwitch;

		public bool PayBananaSwitch;

		public BananaPartyLink BananaPartylink;

		public bool EndlessNormalSwitch;

		public bool EndlessExpertSwitch;

		public bool EndlessExpertClassLeaderboardSwitch;

		public bool ChallengeNormalSwitch;

		public bool ApocalypticChallengeSwitch;

		public bool SupportTalentUnlockToggle;

		public int SupportTalentUnlockAtCouncilLevel;

		public bool EnableRouletteSystem;

		public bool RemoldEquipCanBeBreakDown;

		public string PriceRange;

		public string ExtraGift;

		public bool GoldRadioCallNotice;

		public string BananaEnterButtonGoTo;

		[JsonIgnore]
		public long BananaStartTime
		{
			get
			{
				string[] array = BananaTime.Split('|');
				if (array.Length < 2)
				{
					return -1L;
				}
				return DateTime.Parse(array[0], new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal).ToUniversalTime().TotalMilliseconds();
			}
		}

		[JsonIgnore]
		public long BananaStartTimeIOS
		{
			get
			{
				string[] array = BananaTimeIOS.Split('|');
				if (array.Length < 2)
				{
					return -1L;
				}
				return DateTime.Parse(array[0], new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal).ToUniversalTime().TotalMilliseconds();
			}
		}

		[JsonIgnore]
		public TimeSpan DailyQuestsResetTime => dailyQuestsResetTime;

		[JsonIgnore]
		public TimeSpan DailyQuestsExtraTimeWrapThreshold => dailyQuestsExtraTimeWrapThreshold24h;

		[JsonIgnore]
		public bool IsPriceRangeEnabled
		{
			get
			{
				if (!string.IsNullOrEmpty(PriceRange))
				{
					return PriceRange != "-1";
				}
				return false;
			}
		}

		public int EquipBreakApocalypticEquipTokenCount(int rarityLevel)
		{
			int result = 0;
			switch (rarityLevel)
			{
			case 4:
				result = GoldWeaponsBreakDownFragmentsNumber;
				break;
			case 5:
				result = ApocalypticWeaponsBreakDownFragmentsNumber;
				break;
			case 6:
				result = Star7WeaponsBreakDownFragmentsNumber;
				break;
			}
			return result;
		}

		public bool InBananaTime(DateTime utcTime)
		{
			string[] array = BananaTime.Split('|');
			if (array.Length < 2)
			{
				return false;
			}
			DateTime dateTime = DateTime.Parse(array[0], new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal).ToUniversalTime();
			DateTime dateTime2 = DateTime.Parse(array[1], new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal).ToUniversalTime();
			if (dateTime <= utcTime)
			{
				return utcTime < dateTime2;
			}
			return false;
		}

		public bool InBananaTimeIOS(DateTime utcTime)
		{
			string[] array = BananaTimeIOS.Split('|');
			if (array.Length < 2)
			{
				return false;
			}
			DateTime dateTime = DateTime.Parse(array[0], new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal).ToUniversalTime();
			DateTime dateTime2 = DateTime.Parse(array[1], new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal).ToUniversalTime();
			if (dateTime <= utcTime)
			{
				return utcTime < dateTime2;
			}
			return false;
		}

		public ConfigData()
		{
			GridScale = 1;
			DailyQuestsVersion = 0;
		}

		public void Start()
		{
			if (!string.IsNullOrEmpty(DailyQuestsResetTimeUtc))
			{
				dailyQuestsResetTime = TimeSpan.Parse(DailyQuestsResetTimeUtc);
			}
			else
			{
				dailyQuestsResetTime = TimeSpan.Zero;
			}
			if (!string.IsNullOrEmpty(DailyQuestsTimeWrapThreshold24h))
			{
				dailyQuestsExtraTimeWrapThreshold24h = TimeSpan.Parse(DailyQuestsTimeWrapThreshold24h);
			}
			else
			{
				dailyQuestsExtraTimeWrapThreshold24h = TimeSpan.Zero;
			}
		}

		public bool IsSocialEnabled(string countryCode)
		{
			if (!SocialFeaturesEnabled)
			{
				return false;
			}
			if (SocialFeaturesCountryFilter == null || SocialFeaturesCountryFilter.Count == 0)
			{
				return true;
			}
			return SocialFeaturesCountryFilter.Contains(countryCode.ToLower());
		}

		public void SetGuildLeaderboardDayZero(DateTime origin)
		{
			if (!string.IsNullOrEmpty(GuildLeaderboardDayZero))
			{
				guildLeaderboardDayZeroSeconds = (long)(GameEconomyData.ParseDateTime(GuildLeaderboardDayZero) - origin).TotalSeconds;
			}
			else
			{
				guildLeaderboardDayZeroSeconds = 0L;
			}
		}

		public FixedPoint GetHealingTimeModifier(MapCategory category)
		{
			if (HealingTimeModifiers != null && category > MapCategory.None && (int)category <= HealingTimeModifiers.Count)
			{
				return HealingTimeModifiers[(int)(category - 1)];
			}
			return new FixedPoint(1.0);
		}

		public int GetGrindMissionCost(int playerLevel)
		{
			return GrindMissionGasCostBase + (int)FixedPoint.Ceiling((FixedPoint)playerLevel / (FixedPoint)GrindMissionGasCostDivider);
		}

		public SurvivorClass[] ParseSurvivorClassUnlockOrder()
		{
			if (SurvivorClassUnlockOrder == null)
			{
				return new SurvivorClass[0];
			}
			string[] array = SurvivorClassUnlockOrder.Split(';');
			SurvivorClass[] array2 = new SurvivorClass[array.Length];
			string[] names = Enum.GetNames(typeof(SurvivorClass));
			for (int i = 0; i < array2.Length; i++)
			{
				for (int j = 0; j < names.Length; j++)
				{
					if (array[i] == names[j])
					{
						array2[i] = (SurvivorClass)j;
						break;
					}
				}
			}
			return array2;
		}

		public string GetPCPlatformType(int key)
		{
			if (string.IsNullOrEmpty(PCPlatformType))
			{
				return null;
			}
			string text = key + ":";
			int num = 0;
			while (num < PCPlatformType.Length)
			{
				int num2 = PCPlatformType.IndexOf(text, num, StringComparison.Ordinal);
				if (num2 < 0)
				{
					return null;
				}
				if (num2 == 0 || PCPlatformType[num2 - 1] == ';')
				{
					int num3 = num2 + text.Length;
					int num4 = PCPlatformType.IndexOf(';', num3);
					string text2 = ((num4 >= 0) ? PCPlatformType.Substring(num3, num4 - num3) : PCPlatformType.Substring(num3));
					if (!string.IsNullOrEmpty(text2))
					{
						return text2;
					}
					return null;
				}
				num = num2 + text.Length;
			}
			return null;
		}

		public bool IsInCountryControlIOS(string countryCode)
		{
			if (string.IsNullOrEmpty(countryCode))
			{
				return false;
			}
			if (CountryControlios == null || CountryControlios.Count <= 0)
			{
				return false;
			}
			for (int i = 0; i < CountryControlios.Count; i++)
			{
				if (countryCode.ToLower() == CountryControlios[i].ToLower())
				{
					return true;
				}
			}
			return false;
		}

		private bool TryGetPriceRange(out float minPrice, out float maxPrice)
		{
			minPrice = 0f;
			maxPrice = 0f;
			if (!IsPriceRangeEnabled)
			{
				return false;
			}
			string[] array = PriceRange.Split(',');
			if (array.Length != 2)
			{
				return false;
			}
			if (float.TryParse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture, out minPrice) && float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out maxPrice))
			{
				return true;
			}
			return false;
		}

		public bool IsPriceInRange(float price)
		{
			if (TryGetPriceRange(out var minPrice, out var maxPrice))
			{
				if (price >= minPrice)
				{
					return price <= maxPrice;
				}
				return false;
			}
			return false;
		}

		public Rewards GetExtraGiftRewards()
		{
			return new Rewards(ExtraGift);
		}
	}
}
