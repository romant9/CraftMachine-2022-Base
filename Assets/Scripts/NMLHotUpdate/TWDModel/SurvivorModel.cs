using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SurvivorModel : ActorModel, IMockData<SurvivorMockData>
	{
		public static int MAX_UPGRADEABLE_TRAITS = 5;

		private RarityBasedUpgradeDefinition rarityDefinition;

		public int TokensSpent;

		public int MoveRangeOverride;

		private int damageBase;

		private AbilityModifierIncrementer meleeDamageModifier;

		private AbilityModifierIncrementer rangedDamageModifier;

		public Dictionary<int, List<ModelModifier>> badgeModifiers;

		public TimedActionModel TimedActionModel { get; protected set; }

		public SurvivorMetaData MetaData { get; private set; }

		public SurvivorStatistics Statistics { get; set; }

		public SurvivorClass SurvivorClass { get; set; }

		public ActorAge Age { get; set; }

		public int StartingLevel { get; set; }

		public Rarity StartingRarity { get; set; }

		public int StartingRarityLevel { get; set; }

		public Rarity SurvivorRarity { get; set; }

		public int SurvivorRarityLevel { get; set; }

		public string SurvivorName { get; set; }

		public InjuryType InjuryType { get; set; }

		public InjuryType PreviousCombatInjuryType { get; set; }

		public List<UpgradeTraitsData> UpgradeTraits { get; set; }

		public List<string> RandomTraitsFromReroll { get; set; }

		public List<string> PreviousRandomRolledTraits { get; set; }

		public string TraitToBeRerolledCandidate { get; set; }

		public string LastUpgradedTraitId { get; set; }

		public int SurvivedUntilWave { get; set; }

		public bool IsNotGivenToPlayer { get; set; }

		public BadgeContainerModel BadgeContainer { get; private set; }

		[IgnoreModelProperty]
		public BounsModel UsingBounsModel { get; private set; }

		public string IdForAnalytics { get; set; }

		public bool IsFavourite { get; set; }

		[JsonIgnore]
		public override string Name => SurvivorName;

		[JsonIgnore]
		public SurvivorUpgradeDefinition CurrentUpgradeDefinition => base.gameEconomyData.GetSurvivorsUpgradeDefinition(SurvivorClass, base.Level);

		[JsonIgnore]
		public RarityBasedUpgradeDefinition RarityDefinition
		{
			get
			{
				if (rarityDefinition == null)
				{
					rarityDefinition = base.manager.GameEconomyData.GetRarityBasedUpgradeDefinition(SurvivorRarityLevel, UpgradeType.SurvivorUpgrade);
				}
				return rarityDefinition;
			}
		}

		[JsonIgnore]
		public SurvivorUpgradeDefinition NextUpgradeDefinition => base.gameEconomyData.GetSurvivorsUpgradeDefinition(SurvivorClass, base.Level + 1);

		[JsonIgnore]
		public int UpgradeTime
		{
			get
			{
				PlayerModel playerModel = base.manager?.Player;
				if (playerModel?.ActivityManager != null && playerModel.ActivityManager.TryGetActivityParam(ActivityType.WeaponSurvivorUpgrades5s, out var activityParams))
				{
					return int.Parse(activityParams[0]);
				}
				int timeInSeconds = 0;
				if (playerModel?.ReturnActivityManager != null && playerModel.ReturnActivityManager.TryGetFastUpgradeTime(out timeInSeconds))
				{
					return timeInSeconds;
				}
				if (SurvivorClass == base.gameEconomyData.ConfigData.WeeklyEventClassSurvivorUpgrade5s)
				{
					return 5;
				}
				base.gameEconomyData.GetSurvivorUpgradeCost(base.Level, out var _, out var timeCost);
				return timeCost;
			}
		}

		[JsonIgnore]
		public int UpgradeCost
		{
			get
			{
				base.gameEconomyData.GetSurvivorUpgradeCost(base.Level, out var spCost, out var _);
				return spCost;
			}
		}

		[JsonIgnore]
		public int DemoteSP => base.gameEconomyData.GetSurvivorDemoteSP(SurvivorClass, StartingLevel, base.Level);

		[JsonIgnore]
		public int DemoteTokens
		{
			get
			{
				int tokenSpentRefundPercentage = base.gameEconomyData.ConfigData.TokenSpentRefundPercentage;
				int num = TokensSpent * tokenSpentRefundPercentage / 100;
				CurrencyType survivorTraitUpgradeCurrencyType = GetSurvivorTraitUpgradeCurrencyType(this);
				return base.gameEconomyData.GetClassTokenAmountForRarity(survivorTraitUpgradeCurrencyType, SurvivorRarityLevel) + num;
			}
		}

		[JsonIgnore]
		public override bool IsMeleeClass
		{
			get
			{
				if (SurvivorClass != SurvivorClass.Bruiser && SurvivorClass != SurvivorClass.Scout)
				{
					return SurvivorClass == SurvivorClass.Warrior;
				}
				return true;
			}
		}

		[JsonIgnore]
		public override bool IsRangedClass
		{
			get
			{
				if (SurvivorClass != SurvivorClass.Assault && SurvivorClass != SurvivorClass.Hunter)
				{
					return SurvivorClass == SurvivorClass.Shooter;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsShooterAndHunterClass
		{
			get
			{
				if (SurvivorClass != SurvivorClass.Hunter)
				{
					return SurvivorClass == SurvivorClass.Shooter;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsHero
		{
			get
			{
				if (base.Definition != null)
				{
					return IsHeroFormActorDefinition(base.Definition.ID);
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsAlternativeHero
		{
			get
			{
				if (base.Definition != null)
				{
					return base.Definition.IsAltHero;
				}
				return false;
			}
		}

		[JsonIgnore]
		public int UnlockShareRewardedAmount
		{
			get
			{
				if (base.Definition != null)
				{
					return base.manager.Blackboard.GetCounter(UnlockShareRewardKey);
				}
				return 0;
			}
		}

		[JsonIgnore]
		public string UnlockShareRewardKey
		{
			get
			{
				if (base.Definition != null)
				{
					return "UnlockShareReward." + base.Definition.ID + ".Amount";
				}
				return "";
			}
		}

		public int PvPDefenderIndex { get; set; }

		public int GuildBattlePvPSurvivorIndex { get; set; }

		public ModelRandom TraitRandom { get; set; }

		[JsonIgnore]
		public bool IsLeader => IsSurvivorLeaderInDefenseOrAttack();

		[JsonIgnore]
		public bool IsFeaturedHero => FeaturedDefinition != null;

		[JsonIgnore]
		public FeaturedHeroDefinition FeaturedDefinition
		{
			get
			{
				FeaturedHeroDefinition activeFeaturedHero = base.manager.GameEconomyData.GetActiveFeaturedHero(base.manager.Player.UtcTimeStamp);
				if (activeFeaturedHero != null && activeFeaturedHero.ActorDefinitionID == base.ActorDefinitionID)
				{
					return activeFeaturedHero;
				}
				return null;
			}
		}

		[JsonIgnore]
		public FeaturedHeroDefinition FeaturedDefinitionNext
		{
			get
			{
				FeaturedHeroDefinition activeFeaturedHero = base.manager.GameEconomyData.GetActiveFeaturedHero(base.manager.Player.UtcTimeStamp + MyTools.TimeSpanToLong(TimeSpan.FromDays(7)));
				if (activeFeaturedHero != null && activeFeaturedHero.ActorDefinitionID == base.ActorDefinitionID)
				{
					return activeFeaturedHero;
				}
				return null;
			}
		}

		[JsonIgnore]
		public bool HasReachedMaxLevel => base.Level >= MaxUpgradeLevel;

		[JsonIgnore]
		public bool CanUpgrade
		{
			get
			{
				if (base.Level < MaxUpgradeLevel && base.Level <= base.manager.Player.Camp.GetTrainingGroundLevel())
				{
					return !IsUpgrading();
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanRerollTrait
		{
			get
			{
				if (RarityDefinition.RarityLevel > 3 && !IsUpgrading() && string.IsNullOrEmpty(TraitToBeRerolledCandidate) && RandomTraitsFromReroll == null && base.manager != null)
				{
					return base.manager.Player.SurvivorContainer.HasHero(base.Definition.ID);
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool PendingReroll
		{
			get
			{
				if (!IsUpgrading() && !string.IsNullOrEmpty(TraitToBeRerolledCandidate) && RandomTraitsFromReroll != null)
				{
					return RandomTraitsFromReroll.Count == 2;
				}
				return false;
			}
		}

		[JsonIgnore]
		public int MaxUpgradeLevel => base.gameEconomyData.GetSurvivorsMaxUpgradeLevel(SurvivorClass);

		[JsonIgnore]
		public int GetTotalUpgrades => RarityDefinition.UpgradesTotal;

		[JsonIgnore]
		public string GenerateName => string.Join("_", SurvivorName, "stl_" + StartingLevel, "class_" + SurvivorClass, "lvl_" + base.Level, "rarity_" + SurvivorRarity, "traits_" + string.Join(",", UpgradeTraits.Select((UpgradeTraitsData x) => x.Identifier).ToList()));

		public static CurrencyType GetSurvivorTraitUpgradeCurrencyType(SurvivorModel survivor)
		{
			CurrencyType currencyType = CurrencyType.None;
			if (survivor != null && survivor.Definition != null)
			{
				if (Enum.IsDefined(typeof(CurrencyType), survivor.Definition.TraitUpgradeCurrency))
				{
					currencyType = survivor.Definition.TraitUpgradeCurrency;
				}
				if (currencyType == CurrencyType.None)
				{
					switch (survivor.SurvivorClass)
					{
					case SurvivorClass.Assault:
						currencyType = CurrencyType.AssaultToken;
						break;
					case SurvivorClass.Bruiser:
						currencyType = CurrencyType.BruiserToken;
						break;
					case SurvivorClass.Hunter:
						currencyType = CurrencyType.HunterToken;
						break;
					case SurvivorClass.Scout:
						currencyType = CurrencyType.ScoutToken;
						break;
					case SurvivorClass.Shooter:
						currencyType = CurrencyType.ShooterToken;
						break;
					case SurvivorClass.Warrior:
						currencyType = CurrencyType.WarriorToken;
						break;
					}
				}
			}
			return currencyType;
		}

		public static SurvivorClass GetSurvivorClassForUpgradeCurrencyType(CurrencyType currencyType)
		{
			SurvivorClass result = SurvivorClass.None;
			switch (currencyType)
			{
			case CurrencyType.AssaultToken:
				result = SurvivorClass.Assault;
				break;
			case CurrencyType.BruiserToken:
				result = SurvivorClass.Bruiser;
				break;
			case CurrencyType.HunterToken:
				result = SurvivorClass.Hunter;
				break;
			case CurrencyType.ScoutToken:
				result = SurvivorClass.Scout;
				break;
			case CurrencyType.ShooterToken:
				result = SurvivorClass.Shooter;
				break;
			case CurrencyType.WarriorToken:
				result = SurvivorClass.Warrior;
				break;
			}
			return result;
		}

		public UpgradeTraitsData GetUpgradeTraitsDataForLevel(int level)
		{
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				if (upgradeTraitsData.UnlockingLevel == level)
				{
					return upgradeTraitsData;
				}
			}
			return null;
		}

		public SurvivorModel()
		{
			base.Faction = Faction.Survivor;
			StartingLevel = 1;
			base.Level = StartingLevel;
			StartingRarityLevel = 0;
			SurvivorRarityLevel = 0;
		}

		public SurvivorModel(int startingLevel, int rarityLevel)
		{
			base.Faction = Faction.Survivor;
			StartingLevel = startingLevel;
			base.Level = StartingLevel;
			SurvivorRarityLevel = rarityLevel;
			StartingRarityLevel = rarityLevel;
		}

		public override void Start()
		{
			meleeDamageModifier = null;
			rangedDamageModifier = null;
			base.Start();
			SetupBadgeBonuses();
			TimedActionModel.Changed += OnTimedActionModelChanged;
			if (IsUpgrading())
			{
				TimedActionModel.SetCashier(GetUpgradeCashier(instantUpgrade: false));
			}
			if (IdForAnalytics == "0" || string.IsNullOrEmpty(IdForAnalytics))
			{
				IdForAnalytics = CreateIdForAnalytics();
			}
		}

		protected override void CreateAbilities()
		{
			base.CreateAbilities();
		}

		public override void Initialize()
		{
			base.Initialize();
			Statistics = new SurvivorStatistics();
			Statistics.SetManager(base.manager);
			Statistics.Initialize();
			if (base.Definition != null)
			{
				SurvivorClass = (SurvivorClass)Enum.Parse(typeof(SurvivorClass), base.Definition.Class);
			}
			TimedActionModel = new TimedActionModel();
			TimedActionModel.SetManager(base.manager);
			TimedActionModel.Initialize();
			TimedActionModel.PurchaseType = PurchaseType.SpeedUpSurvivorUpgrade;
			BadgeContainer = new BadgeContainerModel();
			BadgeContainer.SetManager(base.manager);
			BadgeContainer.Initialize();
			IdForAnalytics = "0";
		}

		public void TriggerTrainedDailyQuestAction()
		{
			if (base.manager != null)
			{
				QuestVariables questVariables = base.manager.Player.DailyQuestManager.StartAction("Upgrade");
				questVariables.TargetType = "Survivor";
				questVariables.SurvivorClass.Add(SurvivorClass.ToString());
				if (IsHero)
				{
					questVariables.Hero.Add(base.Definition.GetNonAlternativeHeroDefinition());
				}
				base.manager.Player.DailyQuestManager.CommitAction();
				base.manager.Player.NotifyItemUpgraded(ReturnQuestType.UpgradeSurvivor);
			}
		}

		public override void SetupForCombat(CombatModel combatModel)
		{
			base.SetupForCombat(combatModel);
			if (combatModel.IsGuildBattleMission && base.IsFriendlyHuman)
			{
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				FixedPoint successProbabilityExtension = 0.0;
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("GuildBattleAbilityModifierFullChargeChance", ref value, this);
				if (value > 0.0)
				{
					playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Dodge, value, successProbabilityExtension);
				}
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					base.ChargeMeter.ChangeChargeLevel(1000);
				}
			}
			bool flag = false;
			bool flag2 = false;
			if (base.manager.Player.ActivityManager.TryGetActivityParam(ActivityType.Classstartscharged, out var activityParams))
			{
				if (activityParams[0] == "Survivors")
				{
					if (!IsHero)
					{
						flag2 = true;
					}
				}
				else if (SurvivorClass == (SurvivorClass)Enum.Parse(typeof(SurvivorClass), activityParams[0]))
				{
					flag = true;
				}
			}
			if (!combatModel.IsGuildBattleMission && (flag || flag2))
			{
				base.ChargeMeter.ChangeChargeLevel(1000);
			}
			PreviousCombatInjuryType = InjuryType.None;
			SurvivedUntilWave = 0;
			base.AIController = CreateAIController();
			SnapshotCombatAttributeData();
		}

		private void SnapshotCombatAttributeData()
		{
			if (base.manager != null && base.manager.Player != null)
			{
				Dictionary<AttributeType, FixedPoint> dictionary = new Dictionary<AttributeType, FixedPoint>();
				for (int i = 100; i < 109; i++)
				{
					AttributeType key = (AttributeType)i;
					dictionary[key] = 0.0;
				}
				dictionary[AttributeType.Attack] += base.manager.Player.SurvivalManualManager.GetPrivateAttack(this);
				dictionary[AttributeType.AttackRatio] += base.manager.Player.SurvivalManualManager.GetPrivateAttackRatio(this);
				dictionary[AttributeType.Critical] += base.manager.Player.SurvivalManualManager.GetPrivateCritical(this);
				dictionary[AttributeType.DmgCriticalRatio] += base.manager.Player.SurvivalManualManager.GetPrivateDmgCriticalRatio(this);
				dictionary[AttributeType.DmgTotalRefRatio] += base.manager.Player.SurvivalManualManager.GetPrivateDmgTotalRefRatio(this);
				dictionary[AttributeType.Attack] += base.manager.Player.SurvivalManualManager.GetSystemAttack();
				dictionary[AttributeType.AttackRatio] += base.manager.Player.SurvivalManualManager.GetAttributeAttackRatio();
				dictionary[AttributeType.HitrateMelee] += base.manager.Player.SurvivalManualManager.GetAttributeHitrateMelee();
				dictionary[AttributeType.HitrateRange] += base.manager.Player.SurvivalManualManager.GetAttributeHitrateRange();
				dictionary[AttributeType.CriticalRef] += base.manager.Player.SurvivalManualManager.GetAttributeCriticalRef();
				dictionary[AttributeType.DmgCriticalRatioRef] += base.manager.Player.SurvivalManualManager.GetAttributeDmgCriticalRatioRef();
				base.CombatAttributeSnapshots = dictionary;
			}
		}

		protected override void SetupTraits()
		{
			base.SetupTraits();
			if (UpgradeTraits != null)
			{
				for (int i = 0; i < UpgradeTraits.Count; i++)
				{
					UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
					if (!upgradeTraitsData.IsLocked)
					{
						AddTrait(upgradeTraitsData.Identifier);
					}
				}
			}
			SurvivorContainerModel survivorContainer = base.manager.Player.SurvivorContainer;
			if (survivorContainer != null && survivorContainer.CombatSurvivors != null && survivorContainer.CombatSurvivors.Count > 0 && IsSurvivorLeaderInDefenseOrAttack())
			{
				RegisterLeaderTraits();
			}
		}

		public override void SetupMockTraits()
		{
			base.SetupMockTraits();
			if (UpgradeTraits == null)
			{
				return;
			}
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				if (!upgradeTraitsData.IsLocked)
				{
					AddMockTrait(upgradeTraitsData.Identifier);
				}
			}
		}

		private bool IsSurvivorLeaderInDefenseOrAttack()
		{
			SurvivorContainerModel survivorContainer = base.manager.Player.SurvivorContainer;
			if (survivorContainer.CombatSurvivors.Count == 0)
			{
				if (base.Faction != Faction.Survivor || base.manager.CombatModel?.Survivors?[0] != this)
				{
					if (base.Faction == Faction.Raider)
					{
						return base.manager.CombatModel?.Raiders?[0] == this;
					}
					return false;
				}
				return true;
			}
			if (survivorContainer.CombatSurvivors[0] != this && (PvPDefenderIndex != 0 || base.manager.Player.Combat == null || !base.manager.Player.Combat.IsPVPMission || base.IsFriendlyHuman))
			{
				if (GuildBattlePvPSurvivorIndex == 0 && base.manager.Player.Combat != null && base.manager.Player.Combat.SurvivalMissionConfigType == SurvivalMissionConfig.Type.GuildBattle)
				{
					return !base.IsFriendlyHuman;
				}
				return false;
			}
			return true;
		}

		public InjuryType GetInjuryType()
		{
			int num = base.MinHitpoints + base.MaxHitPoints * base.StrugglesLeft;
			int num2 = base.MaxHitPoints * 2;
			FixedPoint fixedPoint = new FixedPoint((float)num / (float)num2 * 100f);
			InjuryType result = InjuryType.None;
			if (fixedPoint < base.gameEconomyData.ConfigData.InjuryCriticalBelowHealthPercentage)
			{
				result = InjuryType.Critical;
			}
			else if (fixedPoint < base.gameEconomyData.ConfigData.InjuryMajorBelowHealthPercentage)
			{
				result = InjuryType.Major;
			}
			else if (fixedPoint < base.gameEconomyData.ConfigData.InjuryMinorBelowHealthPercentage)
			{
				result = InjuryType.Minor;
			}
			return result;
		}

		public void InitUpgradeTraits()
		{
			UpgradeTraits = new List<UpgradeTraitsData>();
			GiveRandomUpgradeTrait(0, TraitRandom, isTactical: true);
			List<string> upgradeTraits = base.Definition.UpgradeTraits;
			foreach (KeyValuePair<TraitBucketsDefinition, int> item in base.manager.GameEconomyData.GetInitialTraitCountsForSurvivorRarity(SurvivorRarityLevel))
			{
				for (int i = 0; i < item.Value; i++)
				{
					if (upgradeTraits != null && upgradeTraits.Count > i)
					{
						GiveUpgradeTrait(upgradeTraits[i], item.Key.RarityLevel, isTactical: false, item.Key.IsLocked);
					}
					else
					{
						GiveRandomUpgradeTrait(item.Key.RarityLevel, TraitRandom, isTactical: false, item.Key.IsLocked);
					}
				}
			}
			SortUpgradeTraits();
		}

		public void InitUpgradeTraitsFromMockData(List<TraitMockData> traitMockData)
		{
			UpgradeTraits = new List<UpgradeTraitsData>();
			for (int i = 0; i < traitMockData.Count; i++)
			{
				TraitMockData traitMockData2 = traitMockData[i];
				GiveUpgradeTraitFromMockData(traitMockData2);
			}
			SortUpgradeTraits();
		}

		public string GetOwnedUpgradeTraitIdentifier(string traitIdentifier)
		{
			string result = "";
			string text = UpgradeTraitsData.StripTraitLevelIdentifier(traitIdentifier);
			text = text.ToLower();
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				if (UpgradeTraitsData.StripTraitLevelIdentifier(upgradeTraitsData.Identifier).ToLower() == text)
				{
					result = upgradeTraitsData.Identifier;
					break;
				}
			}
			return result;
		}

		public bool HasUpgradeTrait(string traitIdentifier)
		{
			return !string.IsNullOrEmpty(GetOwnedUpgradeTraitIdentifier(traitIdentifier));
		}

		public UpgradeTraitsData GiveUpgradeTrait(string traitIdentifier, int traitLevel, bool isTactical = false, bool isLocked = false, int UnlockingLevel = 1, int index = -1)
		{
			UpgradeTraitsData upgradeTraitsData = null;
			if (!isTactical)
			{
				traitIdentifier = UpgradeTraitsData.CompileUpgradeTraitIdentifier(traitIdentifier, traitLevel, isLocked);
			}
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier);
			if (traitDefinition != null)
			{
				upgradeTraitsData = new UpgradeTraitsData();
				upgradeTraitsData.Identifier = traitDefinition.Identifier;
				upgradeTraitsData.UnlockingLevel = UnlockingLevel;
				upgradeTraitsData.RarityLevel = traitLevel;
				upgradeTraitsData.IsLocked = isLocked;
				upgradeTraitsData.IsTactical = isTactical;
				if (index == -1)
				{
					UpgradeTraits.Add(upgradeTraitsData);
				}
				else
				{
					UpgradeTraits.Insert(index, upgradeTraitsData);
				}
			}
			else if (!ActorTraitContainerModel.IsDeprecated(traitIdentifier))
			{
				base.manager.Debug.LogWarning("Could not give upgrade trait for identifier: '" + traitIdentifier + "'. Could not find trait definition!");
			}
			return upgradeTraitsData;
		}

		public UpgradeTraitsData GiveUpgradeTraitFromMockData(TraitMockData traitMockData)
		{
			UpgradeTraitsData upgradeTraitsData = null;
			if (!traitMockData.IsTactical)
			{
				traitMockData.Identifier = UpgradeTraitsData.CompileUpgradeTraitIdentifier(traitMockData.Identifier, traitMockData.RarityLevel, isLocked: false);
			}
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitMockData.Identifier);
			if (traitDefinition != null)
			{
				upgradeTraitsData = new UpgradeTraitsData();
				upgradeTraitsData.Identifier = traitDefinition.Identifier;
				upgradeTraitsData.UnlockingLevel = 1;
				upgradeTraitsData.RarityLevel = traitMockData.RarityLevel;
				upgradeTraitsData.IsLocked = false;
				upgradeTraitsData.IsTactical = traitMockData.IsTactical;
				upgradeTraitsData.ConstructionMultiplier = 0L;
				UpgradeTraits.Add(upgradeTraitsData);
			}
			else if (!ActorTraitContainerModel.IsDeprecated(traitMockData.Identifier))
			{
				base.manager.Debug.LogWarning("Could not give upgrade trait for identifier: '" + traitMockData.Identifier + "'. Could not find trait definition!");
			}
			return upgradeTraitsData;
		}

		private UpgradeTraitsData GiveUpgradeTraitForBackwardCompatibility(string traitIdentifier, TraitBucketsDefinition.BucketType traitLevel)
		{
			UpgradeTraitsData upgradeTraitsData = null;
			bool flag = traitLevel == TraitBucketsDefinition.BucketType.Locked;
			int traitLevel2 = (int)((!flag) ? (traitLevel - 1) : TraitBucketsDefinition.BucketType.Tactical);
			if (traitLevel != TraitBucketsDefinition.BucketType.Tactical)
			{
				traitIdentifier = UpgradeTraitsData.CompileUpgradeTraitIdentifier(traitIdentifier, traitLevel2, flag);
			}
			TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier);
			if (traitDefinition != null)
			{
				upgradeTraitsData = new UpgradeTraitsData();
				upgradeTraitsData.Identifier = traitDefinition.Identifier;
				upgradeTraitsData.UnlockingLevel = 1;
				upgradeTraitsData.BucketType = traitLevel;
				UpgradeTraits.Add(upgradeTraitsData);
			}
			else if (!ActorTraitContainerModel.IsDeprecated(traitIdentifier))
			{
				base.manager.Debug.LogWarning("Could not give upgrade trait for identifier: '" + traitIdentifier + "'. Could not find trait definition!");
			}
			return upgradeTraitsData;
		}

		public UpgradeTraitsData GiveRandomUpgradeTrait(int traitLevel, ModelRandom random, bool isTactical = false, bool isLocked = false)
		{
			UpgradeTraitsData result = null;
			string text = TraitDefinition.TRAIT_TAG_RARITY_LEVEL + ((!isLocked) ? traitLevel : 0);
			text = (isTactical ? TraitDefinition.TRAIT_TAG_TACTICAL : text);
			List<string> list = new List<string>(new string[2]
			{
				UpgradeType.SurvivorUpgrade.ToString(),
				text
			});
			List<TraitDefinition> upgradeTraits = base.manager.GameEconomyData.GetUpgradeTraits(list, null, base.Level, SurvivorClass);
			if (upgradeTraits != null && upgradeTraits.Count > 0)
			{
				TraitDefinition traitDefinition = PickRandomTraitDefinition(upgradeTraits, random);
				if (traitDefinition == null)
				{
					base.manager.Debug.LogError("Could not find random trait definition for SurvivorModel: [" + ToString() + "]" + text + "," + SurvivorClass);
				}
				else
				{
					result = GiveUpgradeTrait(traitDefinition.Identifier, traitLevel, isTactical, isLocked);
				}
			}
			else
			{
				string text2 = "";
				for (int i = 0; i < list.Count; i++)
				{
					text2 = text2 + list[i] + ", ";
				}
				string text3 = SurvivorClass.ToString();
				base.manager.Debug.LogError("Could not find upgrade traits for [" + ToString() + "] - with tags: {" + text2 + "}, ownerFilters: {" + text3 + "}, Level: " + base.Level);
			}
			return result;
		}

		public UpgradeTraitsData GiveRandomUpgradeTraitForBackwardCompatibility(TraitBucketsDefinition.BucketType traitLevel, ModelRandom random)
		{
			UpgradeTraitsData result = null;
			bool num = traitLevel == TraitBucketsDefinition.BucketType.Locked;
			bool flag = traitLevel == TraitBucketsDefinition.BucketType.Tactical;
			string text = TraitDefinition.TRAIT_TAG_RARITY_LEVEL + (int)((!num) ? (traitLevel - 1) : TraitBucketsDefinition.BucketType.Tactical);
			text = (flag ? TraitDefinition.TRAIT_TAG_TACTICAL : text);
			List<string> list = new List<string>(new string[2]
			{
				UpgradeType.SurvivorUpgrade.ToString(),
				text
			});
			List<TraitDefinition> upgradeTraits = base.manager.GameEconomyData.GetUpgradeTraits(list, null, base.Level, SurvivorClass);
			if (upgradeTraits != null && upgradeTraits.Count > 0)
			{
				TraitDefinition traitDefinition = PickRandomTraitDefinition(upgradeTraits, random);
				if (traitDefinition == null)
				{
					base.manager.Debug.LogError("Could not find random trait definition for SurvivorModel: [" + ToString() + "]" + text + "," + SurvivorClass);
				}
				else
				{
					result = GiveUpgradeTraitForBackwardCompatibility(traitDefinition.Identifier, traitLevel);
				}
			}
			else
			{
				string text2 = "";
				for (int i = 0; i < list.Count; i++)
				{
					text2 = text2 + list[i] + ", ";
				}
				string text3 = SurvivorClass.ToString();
				base.manager.Debug.LogError("Could not find upgrade traits for [" + ToString() + "] - with tags: {" + text2 + "}, ownerFilters: {" + text3 + "}, Level: " + base.Level);
			}
			return result;
		}

		public UpgradeTraitsData GetLowestLevelUpgradeTrait()
		{
			UpgradeTraitsData upgradeTraitsData = null;
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData2 = UpgradeTraits[i];
				if (upgradeTraitsData2.IsLocked)
				{
					return upgradeTraitsData2;
				}
				if (upgradeTraitsData2.IsTactical)
				{
					continue;
				}
				if (upgradeTraitsData == null)
				{
					int maxRarityLevel = base.manager.GameEconomyData.ConfigData.MaxRarityLevel;
					if (upgradeTraitsData2.RarityLevel >= 0 && upgradeTraitsData2.RarityLevel <= maxRarityLevel)
					{
						upgradeTraitsData = upgradeTraitsData2;
					}
				}
				else if (upgradeTraitsData2.RarityLevel <= upgradeTraitsData.RarityLevel)
				{
					upgradeTraitsData = upgradeTraitsData2;
				}
			}
			return upgradeTraitsData;
		}

		public bool UpgradeLowestLevelTrait(bool doNotInstantiateTrait = false, TdMetrics tdMetrics = null)
		{
			bool result = false;
			UpgradeTraitsData lowestLevelUpgradeTrait = GetLowestLevelUpgradeTrait();
			if (lowestLevelUpgradeTrait != null)
			{
				result = UpgradeTraitRarity(lowestLevelUpgradeTrait, doNotInstantiateTrait, tdMetrics);
			}
			return result;
		}

		public bool CanUpgradeTraitRarity()
		{
			UpgradeTraitsData lowestLevelUpgradeTrait = GetLowestLevelUpgradeTrait();
			int maxRarityLevel = base.manager.GameEconomyData.ConfigData.MaxRarityLevel;
			if (lowestLevelUpgradeTrait == null || lowestLevelUpgradeTrait.RarityLevel >= maxRarityLevel)
			{
				return false;
			}
			return GetNextUpgradeLevelTraitDefinition(lowestLevelUpgradeTrait) != null;
		}

		public int GetUpgradeTraitRaritySum()
		{
			int num = 0;
			for (int i = 0; i < ((UpgradeTraits != null) ? UpgradeTraits.Count : 0); i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				num += upgradeTraitsData.RarityLevel + 1;
			}
			return num;
		}

		public static string GetUpgradedTraitIdentifier(UpgradeTraitsData traitData)
		{
			string result = "";
			switch (traitData.BucketType)
			{
			case TraitBucketsDefinition.BucketType.Locked:
				result = traitData.Identifier;
				break;
			case TraitBucketsDefinition.BucketType.LowLevel:
				result = traitData.Identifier.Replace(TraitBucketsDefinition.BucketType.LowLevel.ToString(), TraitBucketsDefinition.BucketType.MidLevel.ToString());
				break;
			case TraitBucketsDefinition.BucketType.MidLevel:
				result = traitData.Identifier.Replace(TraitBucketsDefinition.BucketType.MidLevel.ToString(), TraitBucketsDefinition.BucketType.HighLevel.ToString());
				break;
			case TraitBucketsDefinition.BucketType.HighLevel:
				result = traitData.Identifier.Replace(TraitBucketsDefinition.BucketType.HighLevel.ToString(), TraitBucketsDefinition.BucketType.Epic.ToString());
				break;
			case TraitBucketsDefinition.BucketType.Epic:
				result = traitData.Identifier.Replace(TraitBucketsDefinition.BucketType.Epic.ToString(), TraitBucketsDefinition.BucketType.Legendary.ToString());
				break;
			}
			return result;
		}

		public TraitDefinition GetNextUpgradeLevelTraitDefinition(UpgradeTraitsData traitData)
		{
			string traitIdentifier = "";
			if (traitData != null)
			{
				if (traitData.IsLocked)
				{
					traitIdentifier = traitData.Identifier;
				}
				else
				{
					int num = traitData.Identifier.LastIndexOf(".");
					if (num >= 0)
					{
						traitIdentifier = traitData.Identifier.Substring(0, num) + "." + TraitDefinition.TRAIT_TAG_RARITY_LEVEL + (traitData.RarityLevel + 1);
					}
				}
			}
			return base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier);
		}

		public bool UpgradeTraitRarity(UpgradeTraitsData traitData, bool doNotInstantiateTrait = false, TdMetrics tdMetrics = null)
		{
			bool result = false;
			if (traitData != null && UpgradeTraits.Contains(traitData))
			{
				TraitDefinition nextUpgradeLevelTraitDefinition = GetNextUpgradeLevelTraitDefinition(traitData);
				if (nextUpgradeLevelTraitDefinition != null)
				{
					RemoveTrait(traitData.Identifier);
					traitData.Identifier = nextUpgradeLevelTraitDefinition.Identifier;
					int maxRarityLevel = base.manager.GameEconomyData.ConfigData.MaxRarityLevel;
					tdMetrics?.AddProperty("trait_level_before", traitData.RarityLevel);
					traitData.RarityLevel = Math.Min(maxRarityLevel, traitData.RarityLevel + 1);
					tdMetrics?.AddProperty("trait_level_after", traitData.RarityLevel);
					tdMetrics?.AddProperty("trait_id", traitData.Identifier);
					AddTrait(traitData.Identifier, default(FixedPoint), doNotInstantiateTrait);
					if (IsSurvivorLeaderInDefenseOrAttack())
					{
						RegisterLeaderTraits();
					}
					ConfigureBaseAttributes();
					LastUpgradedTraitId = traitData.Identifier;
					result = true;
				}
			}
			return result;
		}

		public bool RerollTrait(string TraitToBeRerolled)
		{
			string text = TraitDefinition.TRAIT_TAG_RARITY_LEVEL + UpgradeTraitsData.GetTraitLevelIdentifier(TraitToBeRerolled);
			List<string> list = new List<string>(new string[2]
			{
				UpgradeType.SurvivorUpgrade.ToString(),
				text
			});
			bool flag = false;
			if (CanRerollTrait)
			{
				List<TraitDefinition> upgradeTraits = base.manager.GameEconomyData.GetUpgradeTraits(list, null, base.Level, SurvivorClass);
				if (upgradeTraits != null && upgradeTraits.Count > 0)
				{
					foreach (UpgradeTraitsData trait in UpgradeTraits)
					{
						int num = upgradeTraits.FindIndex((TraitDefinition t) => UpgradeTraitsData.StripTraitLevelIdentifier(t.Identifier) == UpgradeTraitsData.StripTraitLevelIdentifier(trait.Identifier));
						if (num != -1)
						{
							upgradeTraits.RemoveAt(num);
						}
					}
					if (PreviousRandomRolledTraits != null)
					{
						foreach (string previouslyRolledTrait in PreviousRandomRolledTraits)
						{
							int num2 = upgradeTraits.FindIndex((TraitDefinition t) => UpgradeTraitsData.StripTraitLevelIdentifier(t.Identifier) == previouslyRolledTrait);
							if (num2 != -1)
							{
								upgradeTraits.RemoveAt(num2);
							}
						}
					}
					if (upgradeTraits.Count > 2)
					{
						RandomTraitsFromReroll = new List<string>();
						for (int num3 = 0; num3 < 2; num3++)
						{
							RandomTraitsFromReroll.Add(UpgradeTraitsData.StripTraitLevelIdentifier(PickRandomTraitDefinition(upgradeTraits, TraitRandom).Identifier));
						}
						flag = true;
					}
					else
					{
						base.manager.Debug.LogError("There are not enough trait candidates for the reroll token for rerolling " + TraitToBeRerolled);
					}
				}
				else
				{
					string text2 = "";
					for (int num4 = 0; num4 < list.Count; num4++)
					{
						text2 = text2 + list[num4] + ", ";
					}
					string text3 = SurvivorClass.ToString();
					base.manager.Debug.LogError("Could not find upgrade traits for [" + ToString() + "] - with tags: {" + text2 + "}, ownerFilters: {" + text3 + "}, Level: " + base.Level);
				}
			}
			if (flag)
			{
				TraitToBeRerolledCandidate = TraitToBeRerolled;
			}
			return flag;
		}

		public bool ChooseRerolledTrait(int index)
		{
			bool result = false;
			if (index != -1)
			{
				string traitIdentifier = RandomTraitsFromReroll[index];
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits.Find((UpgradeTraitsData t) => UpgradeTraitsData.StripTraitLevelIdentifier(t.Identifier) == UpgradeTraitsData.StripTraitLevelIdentifier(TraitToBeRerolledCandidate));
				if (upgradeTraitsData != null)
				{
					int index2 = UpgradeTraits.IndexOf(upgradeTraitsData);
					UpgradeTraitsData upgradeTraitsData2 = GiveUpgradeTrait(traitIdentifier, upgradeTraitsData.RarityLevel, isTactical: false, isLocked: false, upgradeTraitsData.UnlockingLevel, index2);
					if (upgradeTraitsData2 != null)
					{
						UpgradeTraits.Remove(upgradeTraitsData);
						RemoveTrait(upgradeTraitsData.Identifier);
						UnregisterTraitAbilityDependencies(upgradeTraitsData.Identifier);
						AddTrait(upgradeTraitsData2.Identifier);
						ConfigureBaseAttributes();
						if (LastUpgradedTraitId == upgradeTraitsData.Identifier)
						{
							LastUpgradedTraitId = upgradeTraitsData2.Identifier;
						}
						PreviousRandomRolledTraits = null;
						ClearRerolledData();
						result = true;
					}
				}
			}
			else
			{
				PreviousRandomRolledTraits = new List<string>(RandomTraitsFromReroll);
				ClearRerolledData();
				result = true;
			}
			return result;
		}

		public Cashier RefundTokens(string traitToBeRerolledCandidate)
		{
			Cashier traitRerollCashier = GetTraitRerollCashier(traitToBeRerolledCandidate, ignoreRerollTokens: true, PurchaseType.Refund);
			traitRerollCashier.Refund(50, dontAllowMultiplier: true);
			return traitRerollCashier;
		}

		private void ClearRerolledData()
		{
			TraitToBeRerolledCandidate = null;
			RandomTraitsFromReroll = null;
		}

		public bool IsValidForNextUpgradeTrait()
		{
			foreach (KeyValuePair<TraitBucketsDefinition, int> item in base.manager.GameEconomyData.GetTraitCountRequirementsForNextSurvivorRarityUpgrade(SurvivorRarityLevel))
			{
				if (GetUpgradeTraitsOfLevelCount(item.Key) < item.Value)
				{
					return false;
				}
			}
			return true;
		}

		public int GetUpgradeTraitsOfLevelCount(TraitBucketsDefinition definition)
		{
			int num = 0;
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				if (upgradeTraitsData.IsLocked && definition.IsLocked)
				{
					num++;
				}
				else if (upgradeTraitsData.RarityLevel == definition.RarityLevel)
				{
					num++;
				}
			}
			return num;
		}

		public bool CanUpgradeSurvivorRarity()
		{
			bool result = false;
			int maxRarityLevel = base.manager.GameEconomyData.ConfigData.MaxRarityLevel;
			if (SurvivorRarityLevel < maxRarityLevel)
			{
				result = IsValidForNextUpgradeTrait();
			}
			return result;
		}

		public bool UpgradeSurvivorRarity(bool doNotInstantiateTrait = false, TdMetrics tdMetrics = null)
		{
			bool result = false;
			UpgradeTraitsData upgradeTraitsData = null;
			int maxRarityLevel = base.manager.GameEconomyData.ConfigData.MaxRarityLevel;
			if (SurvivorRarityLevel < maxRarityLevel)
			{
				tdMetrics?.AddProperty("trait_level_before", SurvivorRarityLevel);
				SurvivorRarityLevel++;
				tdMetrics?.AddProperty("trait_level_after", SurvivorRarityLevel);
				rarityDefinition = base.manager.GameEconomyData.GetRarityBasedUpgradeDefinition(SurvivorRarityLevel, UpgradeType.SurvivorUpgrade);
				int num = UpgradeTraits.Count - 1;
				if (num < MAX_UPGRADEABLE_TRAITS)
				{
					TraitBucketsDefinition lowestTraitLevelForSurvivorRarity = base.manager.GameEconomyData.GetLowestTraitLevelForSurvivorRarity(SurvivorRarityLevel);
					List<string> upgradeTraits = base.Definition.UpgradeTraits;
					upgradeTraitsData = ((upgradeTraits == null || num <= 0 || num >= upgradeTraits.Count) ? GiveRandomUpgradeTrait(lowestTraitLevelForSurvivorRarity.RarityLevel, TraitRandom) : GiveUpgradeTrait(upgradeTraits[num], lowestTraitLevelForSurvivorRarity.RarityLevel));
					if (upgradeTraitsData != null && !upgradeTraitsData.IsLocked)
					{
						AddTrait(upgradeTraitsData.Identifier, default(FixedPoint), doNotInstantiateTrait);
						LastUpgradedTraitId = upgradeTraitsData.Identifier;
						if (IsHero && tdMetrics != null)
						{
							((TdMetrics)tdMetrics.Clone()).SetEventType("upgrade_hero_trait").AddProperty("is_upgrade_rarity", false).AddProperty("trait_level_after", upgradeTraitsData.RarityLevel)
								.AddProperty("trait_id", upgradeTraitsData.Identifier)
								.AddProperty("hero_id", base.ActorDefinitionID)
								.Send();
						}
					}
					result = upgradeTraitsData != null;
				}
				else
				{
					result = true;
				}
				ConfigureBaseAttributes();
			}
			return result;
		}

		public void SortUpgradeTraits()
		{
			UpgradeTraits.StableSort((UpgradeTraitsData a, UpgradeTraitsData b) => (a.RarityLevel < b.RarityLevel && !a.IsTactical) ? 1 : (-1));
		}

		public void ResetMissionSpecifcStatistics()
		{
			Statistics.HitsInflictedInMission = 0;
			Statistics.NumberOfChargeAbilitiesUsedInMission = 0;
			Statistics.TotalDamageTakenInMission = 0;
			Statistics.TotalDamageInflictedInCombat = 0;
			Statistics.TotalHealingReceivedInCombat = 0;
			Statistics.HitsTakenInMission = 0;
		}

		public override void ConfigureBaseAttributes()
		{
			SurvivorUpgradeDefinition currentUpgradeDefinition = CurrentUpgradeDefinition;
			if (currentUpgradeDefinition == null)
			{
				base.manager.Debug.LogError("Missing GED upgrade for " + SurvivorClass.ToString() + " at level " + base.Level);
				return;
			}
			int num = 0;
			if (base.manager.Player.IsInCombat && base.MaxHitPoints > base.Hitpoints)
			{
				num = base.MaxHitPoints;
			}
			else
			{
				num = GetHitpoints(addEquipmentValue: true, includeFeaturedBonus: true);
				num = Math.Max(num, base.MaxHitPoints);
				if (base.manager.Player.GetAttackTargetMissionModel() is MapMissionModel { IsInApocalyptiWeeklyChallenge: not false })
				{
					num += (int)(num * (base.manager.Player.ApocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.HpCap) / 100L));
				}
			}
			int num2 = Math.Max(base.MaxHitPoints - base.Hitpoints, 0);
			int currentHitPoints = Math.Max(num - num2, 1);
			SetHitPoints(currentHitPoints, num, setConfig: true);
			FixedPoint fixedPoint = (FixedPoint)RarityDefinition.MovementMultiplier / (FixedPoint)100.0;
			FixedPoint additionalIncreasedMoveRange = GetAdditionalIncreasedMoveRange();
			base.MoveRange = (int)(currentUpgradeDefinition.MovementBase * (1.0 + fixedPoint) + additionalIncreasedMoveRange);
			if (MoveRangeOverride > 0)
			{
				base.MoveRange = MoveRangeOverride;
			}
			if (base.Faction == Faction.Survivor)
			{
				IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider != null)
				{
					List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
					base.MoveRange = Math.Max(1, base.MoveRange - (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffMoveShorten));
				}
			}
			FixedPoint fixedPoint2 = (FixedPoint)RarityDefinition.DamageMultiplier / (FixedPoint)100.0;
			FixedPoint fixedPoint3 = (FixedPoint)base.Definition.DamageMultiplier / (FixedPoint)100.0;
			damageBase = (int)(currentUpgradeDefinition.DamageBase * (1.0 + fixedPoint2 + fixedPoint3));
			if (base.Modifiers != null)
			{
				base.Modifiers.RemoveModifier(meleeDamageModifier);
				meleeDamageModifier = new AbilityModifierIncrementer("AddMeleeDamage", damageBase);
				base.Modifiers.RegisterModifier(meleeDamageModifier);
				base.Modifiers.RemoveModifier(rangedDamageModifier);
				rangedDamageModifier = new AbilityModifierIncrementer("AddRangedDamage", damageBase);
				base.Modifiers.RegisterModifier(rangedDamageModifier);
			}
		}

		public int GetDamageForPreferredWeapon(bool addEquipmentValue = true, bool includeFeaturedBonus = false)
		{
			return GetDamageForPreferredWeaponForLevel(base.Level, addEquipmentValue, includeFeaturedBonus);
		}

		public int GetDamageForPreferredWeaponForLevel(int level, bool addEquipmentValue = true, bool includeFeaturedBonus = false)
		{
			return GetDamageForPreferredWeaponForLevel(level, RarityDefinition, addEquipmentValue, includeFeaturedBonus);
		}

		public FixedPoint GetAdditionalIncreasedMoveRange()
		{
			AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
			EquipmentItemModel weaponEquipment = GetWeaponEquipment();
			if (abilityManager != null && weaponEquipment != null && weaponEquipment.Definition != null)
			{
				FixedPoint value = 0.0;
				if (weaponEquipment.Definition.Category == EquipmentCategory.MeleeWeapon)
				{
					abilityManager.VisitParameter("AbilityModifierLightMovementSpeedIsIncreasedBySpaces", ref value, this);
				}
				else if (weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon)
				{
					abilityManager.VisitParameter("AbilityModifierLightMovementSpeedIsIncreasedBySpaces", ref value, this);
				}
				return value;
			}
			return 0L;
		}

		public int GetDamageForPreferredWeaponForLevel(int level, RarityBasedUpgradeDefinition rarityDefinition, bool addEquipmentValue = true, bool includeFeaturedBonus = false)
		{
			AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
			EquipmentItemModel weaponEquipment = GetWeaponEquipment();
			if (weaponEquipment != null)
			{
				if (NGUIManager.IsEditorRuntime) DebugTWD.Log("DamageMultiplier is " + rarityDefinition.DamageMultiplier + " " + weaponEquipment.Damage, DebugType.BattleDamage);

				FixedPoint fixedPoint = (FixedPoint)rarityDefinition.DamageMultiplier / (FixedPoint)100.0;
				SurvivorUpgradeDefinition survivorsUpgradeDefinition = base.gameEconomyData.GetSurvivorsUpgradeDefinition(SurvivorClass, level);
				FixedPoint fixedPoint2 = (FixedPoint)base.Definition.DamageMultiplier / (FixedPoint)100.0;
				FixedPoint value = survivorsUpgradeDefinition.DamageBase * (1.0 + fixedPoint + fixedPoint2);
				if (abilityManager != null)
				{
					FixedPoint value2 = 0.0;
					abilityManager.VisitParameter("PercentageIncreaseBaseDamage", ref value2, this);
					FixedPoint fixedPoint3 = weaponEquipment.Damage;
					fixedPoint3 += fixedPoint3 * value2;
					FixedPoint fixedPoint4 = 0.0;
					fixedPoint3 += fixedPoint3 * fixedPoint4;
					abilityManager.VisitParameter("AbilityModifierIncreaseBaseDamageFlat", ref value, this);
					if (addEquipmentValue)
					{
						value += fixedPoint3;
					}
					FixedPoint value3 = 1.0;
					if (weaponEquipment.Definition.Category == EquipmentCategory.MeleeWeapon)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageMelee", ref value3, this);
						abilityManager.VisitParameter("PercentageIncreaseMeleeDamage", ref value3, this);
						abilityManager.VisitParameter("AbilityModifierEquipPercentageIncreaseMeleeDamage", ref value3, this);
					}
					if (weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageRanged", ref value3, this);
						abilityManager.VisitParameter("PercentageIncreaseRangeDamage", ref value3, this);
						abilityManager.VisitParameter("PercentageNewIncreaseRangeDamage", ref value3, this);
					}
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageIncrementer", ref value3, this);
					if (includeFeaturedBonus)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageFeaturedHero", ref value3, this);
					}
					FixedPoint value4 = 1.0;
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyFinalDamageIncrementerBadges", ref value4, this);
					value = value * value3 * value4;
				}
				return (int)value;
			}
			base.manager.Debug.LogWarning("GetDamageForPreferredWeaponForLevel : No valid weapon equipment!");
			return 0;
		}

		public void GetStatsDifferenceToPreviousRarityLevel(out int damageDiff, out int healthDiff)
		{
			GameEconomyData obj = base.manager.Player.gameEconomyData;
			int survivorRarityLevel = SurvivorRarityLevel;
			int rarityLevel = ((survivorRarityLevel != 0) ? (survivorRarityLevel - 1) : 0);
			RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = obj.GetRarityBasedUpgradeDefinition(rarityLevel, UpgradeType.SurvivorUpgrade);
			int damageForPreferredWeaponForLevel = GetDamageForPreferredWeaponForLevel(base.Level, RarityDefinition);
			int damageForPreferredWeaponForLevel2 = GetDamageForPreferredWeaponForLevel(base.Level, rarityBasedUpgradeDefinition);
			damageDiff = damageForPreferredWeaponForLevel - damageForPreferredWeaponForLevel2;
			int hitpointsForLevel = GetHitpointsForLevel(base.Level, RarityDefinition);
			int hitpointsForLevel2 = GetHitpointsForLevel(base.Level, rarityBasedUpgradeDefinition);
			healthDiff = hitpointsForLevel - hitpointsForLevel2;
		}

		public void GetStatsDifferenceToPreviousLevel(out int damageDiff, out int healthDiff)
		{
			int level = base.Level;
			int level2 = ((base.Level > 1) ? (base.Level - 1) : base.Level);
			int hitpointsForLevel = GetHitpointsForLevel(level2);
			int hitpointsForLevel2 = GetHitpointsForLevel(level);
			healthDiff = hitpointsForLevel2 - hitpointsForLevel;
			int damageForPreferredWeaponForLevel = GetDamageForPreferredWeaponForLevel(level2);
			int damageForPreferredWeaponForLevel2 = GetDamageForPreferredWeaponForLevel(level);
			damageDiff = damageForPreferredWeaponForLevel2 - damageForPreferredWeaponForLevel;
		}

		public int GetHitpoints(bool addEquipmentValue = true, bool includeFeaturedBonus = false)
		{
			return GetHitpointsForLevel(base.Level, addEquipmentValue, includeFeaturedBonus);
		}

		public int GetHitpointsForLevel(int level, bool addEquipmentValue = true, bool includeFeaturedBonus = false)
		{
			return GetHitpointsForLevel(level, RarityDefinition, addEquipmentValue, includeFeaturedBonus);
		}

		public int GetHitpointsForLevel(int level, RarityBasedUpgradeDefinition rarityDefinition, bool addEquipmentValue = true, bool includeFeaturedBonus = false)
		{
			AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
			FixedPoint fixedPoint = (FixedPoint)rarityDefinition.HealthMultiplier / (FixedPoint)100.0;
			FixedPoint fixedPoint2 = (FixedPoint)base.Definition.HealthMultiplier / (FixedPoint)100.0;
			FixedPoint value = base.gameEconomyData.GetSurvivorsUpgradeDefinition(SurvivorClass, level).HealthBase * (1.0 + fixedPoint + fixedPoint2);
			EquipmentItemModel equipmentOfCategory = GetEquipmentOfCategory(EquipmentCategory.Armor);
			if (addEquipmentValue && equipmentOfCategory != null)
			{
				value += (FixedPoint)equipmentOfCategory.Defense;
				if (abilityManager != null)
				{
					FixedPoint value2 = 0.0;
					abilityManager.VisitParameter("Health", ref value2, this);
					value += equipmentOfCategory.Defense * value2;
				}
			}
			if (abilityManager != null)
			{
				FixedPoint value3 = 1.0;
				FixedPoint value4 = 1.0;
				switch (SurvivorClass)
				{
				case SurvivorClass.Assault:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthAssault", ref value3, this);
					break;
				case SurvivorClass.Bruiser:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthBruiser", ref value3, this);
					break;
				case SurvivorClass.Hunter:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthHunter", ref value3, this);
					break;
				case SurvivorClass.Scout:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthScout", ref value3, this);
					break;
				case SurvivorClass.Shooter:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthShooter", ref value3, this);
					break;
				case SurvivorClass.Warrior:
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthWarrior", ref value3, this);
					break;
				}
				abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthAll", ref value3, this);
				if (includeFeaturedBonus)
				{
					abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthFeaturedHero", ref value4, this);
				}
				EquipmentItemModel weaponEquipment = GetWeaponEquipment();
				if (weaponEquipment != null && weaponEquipment.Definition != null)
				{
					if (weaponEquipment.Definition.Category == EquipmentCategory.MeleeWeapon)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthMelee", ref value3, this);
					}
					else if (weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon)
					{
						abilityManager.VisitParameter("AbilityModifierPercentageMultiplyHealthRanged", ref value3, this);
					}
				}
				abilityManager.VisitParameter("AbilityModifierHealthBoostBounsHealth", ref value3, this);
				abilityManager.VisitParameter("SurvivalManualStorySkill_EParm1", ref value3, this);
				value = value * value3 * value4;
				abilityManager.VisitParameter("AbilityModifierIncreaseBaseHealthFlat", ref value, this);
			}
			return (int)value;
		}

		public int GetMoveRangeForLevel(int level)
		{
			FixedPoint fixedPoint = (FixedPoint)RarityDefinition.HealthMultiplier / (FixedPoint)100.0;
			FixedPoint additionalIncreasedMoveRange = GetAdditionalIncreasedMoveRange();
			return (int)(base.gameEconomyData.GetSurvivorsUpgradeDefinition(SurvivorClass, level).MovementBase * (1L + fixedPoint) + additionalIncreasedMoveRange);
		}

		public bool IsUpgrading()
		{
			return TimedActionModel.IsActionUnderway();
		}

		public TWDModelResult UpgradeInstant(Cashier cashier = null)
		{
			if (cashier != null && cashier.useTokensForPayment && CanUpgrade && GetUpgradeCashier(instantUpgrade: true, !IsUpgrading(), useTokens: true).CanAfford())
			{
				TWDModelResult num = TimedActionModel.StartActionInstant(GetUpgradeCashier(instantUpgrade: true, !IsUpgrading(), useTokens: true), this);
				if (num == TWDModelResult.OK)
				{
					TriggerTrainedDailyQuestAction();
				}
				return num;
			}
			if (cashier == null && CanUpgrade && GetUpgradeCashier(instantUpgrade: true, !IsUpgrading()).CanAfford())
			{
				Cashier upgradeCashier = GetUpgradeCashier(instantUpgrade: true, !IsUpgrading());
				upgradeCashier.UsedReason = "UpgradeSurvivorInstant";
				TWDModelResult num2 = TimedActionModel.StartActionInstant(upgradeCashier, this);
				if (num2 == TWDModelResult.OK)
				{
					TriggerTrainedDailyQuestAction();
				}
				return num2;
			}
			return TWDModelResult.Error;
		}

		public TWDModelResult StartUpgrade(int useDiamondsAmount)
		{
			if (CanUpgrade)
			{
				Cashier upgradeCashier = GetUpgradeCashier(instantUpgrade: false);
				upgradeCashier.UseDiamondsAmount = useDiamondsAmount;
				return TimedActionModel.StartAction(UpgradeTime, upgradeCashier, this);
			}
			return TWDModelResult.Error;
		}

		public Cashier GetUpgradeCashier(bool instantUpgrade, bool addInitialSurvivorPoints = false, bool useTokens = false)
		{
			Cashier cashier = new Cashier(base.manager);
			if (instantUpgrade && useTokens)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.InstantSurvivorUpgrade);
				cashierItem.SetCost(CurrencyType.SuperTrainingTokenBP, 1);
				cashier.AddItem(cashierItem);
				cashier.useTokensForPayment = true;
			}
			else if (instantUpgrade)
			{
				CashierItem cashierItem2 = new CashierItem(PurchaseType.InstantSurvivorUpgrade);
				int num = base.gameEconomyData.TimeToDiamonds(UpgradeTime * 1000);
				if (addInitialSurvivorPoints)
				{
					num += base.gameEconomyData.CurrencyToDiamonds(CurrencyType.SurvivalPoints, UpgradeCost);
				}
				cashierItem2.SetCost(CurrencyType.Diamonds, num);
				cashier.AddItem(cashierItem2);
			}
			else
			{
				CashierItem cashierItem3 = new CashierItem(PurchaseType.UpgradeSurvivor);
				cashierItem3.SetCost(CurrencyType.SurvivalPoints, UpgradeCost);
				cashier.AddItem(cashierItem3);
			}
			return cashier;
		}

		public Cashier GetUpgradeTraitCashier()
		{
			Cashier cashier = null;
			int upgradeTraitRaritySum = GetUpgradeTraitRaritySum();
			int survivorTraitUpgradeCost = base.manager.GameEconomyData.GetSurvivorTraitUpgradeCost(upgradeTraitRaritySum);
			CurrencyType survivorTraitUpgradeCurrencyType = GetSurvivorTraitUpgradeCurrencyType(this);
			if (survivorTraitUpgradeCurrencyType != CurrencyType.None)
			{
				cashier = new Cashier(base.manager);
				CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeTrait);
				cashierItem.SetCost(survivorTraitUpgradeCurrencyType, survivorTraitUpgradeCost);
				cashier.AddItem(cashierItem);
			}
			return cashier;
		}

		public Cashier GetTraitRerollCashier(string trait, bool ignoreRerollTokens = false, PurchaseType type = PurchaseType.TraitReroll)
		{
			Cashier cashier = null;
			int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(trait);
			if (traitLevelIdentifier > 0)
			{
				TraitRerollCostDefinitions traitRerollCost = base.manager.GameEconomyData.GetTraitRerollCost(traitLevelIdentifier);
				if (traitRerollCost != null)
				{
					cashier = new Cashier(base.manager);
					CashierItem cashierItem = new CashierItem(type);
					if (!ignoreRerollTokens)
					{
						cashierItem.SetCost(CurrencyType.TraitRerollToken, IsHero ? traitRerollCost.HeroRerollTokenCost : traitRerollCost.SurvivorRerollTokenCost);
					}
					cashierItem.SetCost(GetSurvivorTraitUpgradeCurrencyType(this), IsHero ? traitRerollCost.HeroTokenCost : traitRerollCost.ClassTokenCost);
					cashier.AddItem(cashierItem);
				}
			}
			return cashier;
		}

		private TraitDefinition PickRandomTraitDefinition(List<TraitDefinition> traitDefinitions, ModelRandom random)
		{
			if (traitDefinitions.Count == 0)
			{
				return null;
			}
			TraitDefinition traitDefinition = random.GetRandomElement(traitDefinitions, remove: true);
			if (traitDefinition != null && !traitDefinition.CanBeDuplicate && HasUpgradeTrait(traitDefinition.Identifier))
			{
				traitDefinition = PickRandomTraitDefinition(traitDefinitions, random);
			}
			return traitDefinition;
		}

		private void OnTimedActionModelChanged(ModelObject m, string changed, object args)
		{
			if (changed == "ActionStartEvent")
			{
				NotifyChange("ActionStartEvent", this);
			}
			else
			{
				if (!(changed == "ActionFinishedEvent"))
				{
					return;
				}
				base.Level++;
				TimedActionModel timedActionModel = args as TimedActionModel;
				Metrics.UpgradeTypes upgradeType = Metrics.UpgradeTypes.Regular;
				if (timedActionModel != null)
				{
					if (timedActionModel.WasInstant)
					{
						upgradeType = Metrics.UpgradeTypes.Instant;
					}
					else if (timedActionModel.WasSpeedUp)
					{
						upgradeType = Metrics.UpgradeTypes.SpeedUp;
					}
				}
				base.manager.Metrics.AddEnd().AddUpgrade(upgradeType).AddSurvivor(this)
					.AddLevel()
					.Send();
				if (base.Manager.Mode == ModelManagerMode.Client && !base.manager.Player.Camp.InCamp && (timedActionModel == null || !timedActionModel.WasSpeedUp))
				{
					base.manager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.Survivor, base.ModelId, Name, base.Level);
				}
				ConfigureBaseAttributes();
				NotifyChange("ActionFinishedEvent", this);
			}
		}

		public void OnInjuryCured()
		{
			NotifyChange("ActionFinishedEvent", this);
		}

		public Cashier GetDemoteCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.Refund);
			cashierItem.SetCost(CurrencyType.SurvivalPoints, DemoteSP);
			cashierItem.SetCost(SurvivorToken.GetClassAsCurrency(SurvivorClass), DemoteTokens);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public Dictionary<CurrencyType, OverflowableAmount> Demote()
		{
			return GetDemoteCashier().Refund(100, dontAllowMultiplier: true);
		}

		public override bool CanEquip(EquipmentItemModel equipment)
		{
			if (!equipment.Definition.CanBeEquippedBySurvivorClass(SurvivorClass))
			{
				return false;
			}
			return base.CanEquip(equipment);
		}

		public BadgeModel GetBadgeWithSlotIndex(int index)
		{
			if (BadgeContainer != null)
			{
				return BadgeContainer.GetBadge(index);
			}
			return null;
		}

		public TWDModelResult ReclaimBadge(BadgeModel badge, bool pay, bool returnBadgeInventory)
		{
			if (badge == null)
			{
				return TWDModelResult.Error;
			}
			if (pay)
			{
				TWDModelResult tWDModelResult = GetBadgeReclaimCashier().Pay(this, null, badge);
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}
			BadgeContainer.Badges.Remove(badge);
			if (returnBadgeInventory)
			{
				base.manager.Player.Equipment.AddBadge(badge);
			}
			ClearBadgeModifiers();
			List<ActorModel> list = new List<ActorModel>();
			bool flag = false;
			for (int i = 0; i < base.manager.Player.SurvivorContainer.CombatSurvivors.Count; i++)
			{
				SurvivorModel survivorModel = base.manager.Player.SurvivorContainer.CombatSurvivors[i];
				if (survivorModel == this)
				{
					flag = true;
				}
				list.Add(survivorModel);
			}
			EvaluateBadges(new BadgeContext(this, flag ? list : null));
			ConfigureBaseAttributes();
			return TWDModelResult.OK;
		}

		public TWDModelResult EquipBadge(BadgeModel badge, bool saveExisting = false)
		{
			if (!base.manager.Player.Equipment.Badges.Contains(badge))
			{
				base.manager.Debug.LogError("Equipping badge which was not found in inventory");
				return TWDModelResult.Error;
			}
			BadgeModel badge2 = BadgeContainer.GetBadge(badge.SlotIndex);
			int maxSimilarBadgeCount = base.gameEconomyData.ConfigData.MaxSimilarBadgeCount;
			if (maxSimilarBadgeCount != 0 && BadgeContainer.GetSimilarBadgeCount(badge, badge2) >= maxSimilarBadgeCount)
			{
				return TWDModelResult.Error;
			}
			if (saveExisting)
			{
				if (badge2 == null)
				{
					return TWDModelResult.Error;
				}
				TWDModelResult tWDModelResult = ReclaimBadge(badge2, pay: true, returnBadgeInventory: false);
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}
			else
			{
				if (badge2 != null)
				{
					TWDModelResult tWDModelResult2 = base.manager.Player.Equipment.ScrapBadge(badge2);
					if (tWDModelResult2 != TWDModelResult.OK)
					{
						return tWDModelResult2;
					}
				}
				List<ActorModel> list = new List<ActorModel>();
				bool flag = false;
				for (int i = 0; i < base.manager.Player.SurvivorContainer.CombatSurvivors.Count; i++)
				{
					SurvivorModel survivorModel = base.manager.Player.SurvivorContainer.CombatSurvivors[i];
					if (survivorModel == this)
					{
						flag = true;
					}
					list.Add(survivorModel);
				}
				base.manager.Player.Equipment.RemoveBadge(badge);
				BadgeContainer.SetBadge(badge);
				EvaluateBadges(new BadgeContext(this, flag ? list : null));
				ConfigureBaseAttributes();
			}
			return TWDModelResult.OK;
		}

		public Cashier GetBadgeReclaimCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.ReclaimBadge);
			int badgeReclaimCost = base.manager.Player.ActivityManager.GetBadgeReclaimCost(base.gameEconomyData.ConfigData);
			cashierItem.SetCost(CurrencyType.Diamonds, badgeReclaimCost);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public override void SetupBadgeBonuses()
		{
			base.SetupBadgeBonuses();
			for (int i = 0; i < ((BadgeContainer != null && BadgeContainer.Badges != null) ? BadgeContainer.Badges.Count : 0); i++)
			{
				BadgeModel badgeModel = BadgeContainer.Badges[i];
				badgeModel.CreateBonusCondition(base.manager.GameEconomyData.GetBadgeBonusDefinition(badgeModel.BonusId));
			}
		}

		public void EvaluateBadges(ConditionContext context)
		{
			if (BadgeContainer.Badges == null || BadgeContainer.Badges.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < BadgeContainer.Badges.Count; i++)
			{
				BadgeModel badgeModel = BadgeContainer.Badges[i];
				FixedPoint? bonus = null;
				if (badgeModel.BonusCondition.Evaluate(context))
				{
					bonus = badgeModel.BonusCondition.BonusValue;
				}
				List<ModelModifier> modifiers = BadgeContainer.CreateBadgeModifiers(badgeModel, bonus);
				AddBadgeModifiers(badgeModel, modifiers);
			}
		}

		public void AddBadgeModifiers(BadgeModel badge, List<ModelModifier> modifiers)
		{
			if (badgeModifiers == null)
			{
				badgeModifiers = new Dictionary<int, List<ModelModifier>>();
			}
			if (badgeModifiers.TryGetValue(badge.SlotIndex, out var value))
			{
				for (int i = 0; i < (value?.Count ?? 0); i++)
				{
					base.Modifiers.RemoveModifier(value[i]);
				}
			}
			for (int j = 0; j < (modifiers?.Count ?? 0); j++)
			{
				base.Modifiers.RegisterModifier(modifiers[j]);
			}
			badgeModifiers[badge.SlotIndex] = modifiers;
		}

		public void ClearBadgeModifiers()
		{
			if (badgeModifiers == null)
			{
				return;
			}
			foreach (KeyValuePair<int, List<ModelModifier>> badgeModifier in badgeModifiers)
			{
				for (int i = 0; i < ((badgeModifier.Value != null) ? badgeModifier.Value.Count : 0); i++)
				{
					base.Modifiers.RemoveModifier(badgeModifier.Value[i]);
				}
			}
			badgeModifiers.Clear();
		}

		public bool CreateBadgeContainerForOldPlayers()
		{
			if (BadgeContainer == null)
			{
				BadgeContainer = new BadgeContainerModel();
				BadgeContainer.SetManager(base.manager);
				BadgeContainer.Initialize();
				return true;
			}
			return false;
		}

		public void ApplyMovementModifier(int newRange)
		{
			if (base.MoveRange > 0)
			{
				FixedPoint fixedPoint = (float)newRange / (float)base.MoveRange;
				MoveRangeOverride = (int)Math.Round((float)(base.MoveRange * fixedPoint), MidpointRounding.AwayFromZero);
			}
			else
			{
				MoveRangeOverride = newRange;
			}
		}

		protected override ActorLevelDefinition GetActorLevelDefinition(string definitionId, int level)
		{
			string definitionId2 = definitionId;
			if (IsHero)
			{
				definitionId2 = "Default" + SurvivorClass;
			}
			return base.GetActorLevelDefinition(definitionId2, level);
		}

		public override int[] GetSPGain(ActorModel attacker, bool shouldCap = false)
		{
			int[] sPGain = base.GetSPGain(attacker, shouldCap);
			if (IsHero)
			{
				sPGain[0] = (int)(sPGain[0] * base.manager.GameEconomyData.ConfigData.HeroSPMultiplier);
			}
			return sPGain;
		}

		public override bool CheckTimedEffectsEndByTraits(Faction activeFaction)
		{
			if (base.CheckTimedEffectsEndByTraits(activeFaction))
			{
				return true;
			}
			FixedPoint value = 0.0;
			if (base.ExclusiveTimedEffect != null && base.ExclusiveTimedEffect.InstigatorFaction == Faction.Walker && base.ExclusiveTimedEffect.Type == TimedEffectType.Struggle && base.manager.Player.AbilityManager.VisitParameter("AbilityModifierGiveDamageOnStruggle", ref value, this) && base.Faction == activeFaction)
			{
				FixedPoint value2 = 0.0;
				FixedPoint value3 = 0.0;
				FixedPoint fixedPoint = new FixedPoint(GetDamageForPreferredWeapon(addEquipmentValue: true, includeFeaturedBonus: true));
				if (base.manager.Player.AbilityManager.VisitParameter("AbilityModifierGiveDamageOnStruggleRoundModifier", ref value3, this) && base.Faction == activeFaction)
				{
					value -= base.ExclusiveTimedEffect.Counter * value3;
					value = FixedPoint.Max(0L, value);
				}
				fixedPoint *= value;
				if (base.manager.Player.AbilityManager.VisitParameter("AbilityModifierGiveDamageOnStruggleVariance", ref value2, this) && base.Faction == activeFaction)
				{
					FixedPoint fixedPoint2 = fixedPoint * value2;
					int min = (int)(fixedPoint - fixedPoint2);
					int max = (int)(fixedPoint + fixedPoint2);
					fixedPoint = base.manager.CombatModel.RollCombatDiceFromRange(RollDiceType.Damage, min, max);
				}
				FixedPoint fixedPoint3 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("AbilityModifierExtrAtorsoAttackDamageBoost", ref value2, this);
				fixedPoint = (1L + fixedPoint3) * fixedPoint;
				ActorModel instigator = base.ExclusiveTimedEffect.Instigator;
				CombatHelpers.ExecuteDamage(base.manager.CombatModel, this, instigator, (int)fixedPoint, 0, DamageType.Melee, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, null, noChargeGain: false, null, isMainTarget: false, isTriggerExtraAttackDamage: true);
				if (instigator.IsDead)
				{
					CombatModel combatModel = base.manager.CombatModel;
					if (combatModel.IsEndlessBattleMission)
					{
						combatModel.EndlessModeCombatModel.HandleKillScoreIncrease();
						combatModel.NotifyChange("EndlessModeScoreChanged");
					}
				}
				return true;
			}
			return false;
		}

		public override void CombatCleanup()
		{
			base.CombatCleanup();
			base.Faction = Faction.Survivor;
		}

		private string CreateIdForAnalytics()
		{
			string hashedId = base.manager.Player.HashedId;
			string text = base.manager.Player.UtcTimeStamp.ToString();
			return ModelHelpers.MD5Sum(StartingLevel + ModelHelpers.GetRarityNameForAnalytics(SurvivorRarityLevel) + SurvivorClass.ToString() + base.ModelId + hashedId + text);
		}

		public SurvivorMockData CreateMockData()
		{
			return new SurvivorMockData
			{
				ActorDefinitionId = base.ActorDefinitionID,
				CharacterPrefabName = base.CharacterPrefab,
				RarityLevel = SurvivorRarityLevel,
				Name = Name,
				SurvivorClass = SurvivorClass,
				AnalyticsId = IdForAnalytics,
				Level = base.Level,
				UpgradeTraitsList = GetUpgradeTraitsList()
			};
		}

		public string GetUpgradeTraitsList()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				if (!upgradeTraitsData.IsLocked)
				{
					stringBuilder.Append(upgradeTraitsData.Identifier);
					stringBuilder.Append(',');
				}
			}
			return stringBuilder.ToString();
		}

		public static bool IsHeroFormActorDefinition(string actorDefinitionId)
		{
			return actorDefinitionId.ToLower().Contains("hero");
		}

		public static ActorGender GetAssetGender(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return ActorGender.NotSpecified;
			}
			return name[0] switch
			{
				'M' => ActorGender.Male,
				'F' => ActorGender.Female,
				_ => ActorGender.NotSpecified,
			};
		}

		public TWDModelResult UnequipBouns(BounsModel bouns)
		{
			if (UsingBounsModel == bouns)
			{
				RemoveTrait(bouns.LevelDefinition.TraitsLevel);
				RemoveTrait(bouns.LevelDefinition.QualityLevel);
				UsingBounsModel = null;
			}
			return TWDModelResult.OK;
		}

		public TWDModelResult EquipBouns(BounsModel bouns)
		{
			if (UsingBounsModel != null && UsingBounsModel != bouns)
			{
				UnequipBouns(UsingBounsModel);
			}
			else if (UsingBounsModel == bouns)
			{
				return TWDModelResult.OK;
			}
			AddTrait(bouns.LevelDefinition.TraitsLevel);
			AddTrait(bouns.LevelDefinition.QualityLevel);
			UsingBounsModel = bouns;
			return TWDModelResult.OK;
		}

		public int GetCommonHealth()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return 0;
			}
			SurvivalManualManager survivalManualManager = base.manager.Player.SurvivalManualManager;
			int num = 0;
			int num2 = 0;
			if (survivalManualManager != null)
			{
				num = survivalManualManager.GetHPClinet(this);
				num2 = survivalManualManager.GetPrivateHpRatioClient(this) + survivalManualManager.GetAttributeHpRatioClient();
			}
			float num3 = (float)GetHitpoints() / 100f * (float)(100 + num2) + (float)num;
			if (base.manager.Player.Tutorial.HasCompletedPart("Phone") && FeaturedDefinition != null)
			{
				num3 += num3 * ((float)FeaturedDefinition.HealthBoostMultiplier / 100f);
			}
			return (int)num3;
		}

		public int GetCommonDamage()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return 0;
			}
			SurvivalManualManager survivalManualManager = base.manager.Player.SurvivalManualManager;
			int num = 0;
			int num2 = 0;
			if (survivalManualManager != null)
			{
				num = survivalManualManager.GetAttackClinet(this);
				num2 = survivalManualManager.GetPrivateAttackRatioClient(this) + survivalManualManager.GetAttributeAttackRatioClient();
			}
			float num3 = (float)GetDamageForPreferredWeapon() / 100f * (float)(100 + num2) + (float)num;
			if (base.manager.Player.Tutorial.HasCompletedPart("Phone") && FeaturedDefinition != null)
			{
				num3 += num3 * ((float)FeaturedDefinition.DamageBoostMultiplier / 100f);
			}
			return (int)num3;
		}



		#region mycode
		public bool ChooseRerolledTrait(string traitIdentifier)
		{
			bool result = false;

			int index = RandomTraitsFromReroll.IndexOf(traitIdentifier);
			if (index != -1)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits.Find((UpgradeTraitsData t) => UpgradeTraitsData.StripTraitLevelIdentifier(t.Identifier) == UpgradeTraitsData.StripTraitLevelIdentifier(TraitToBeRerolledCandidate));
				if (upgradeTraitsData != null)
				{
					int index2 = UpgradeTraits.IndexOf(upgradeTraitsData);
					UpgradeTraitsData upgradeTraitsData2 = GiveUpgradeTrait(traitIdentifier, upgradeTraitsData.RarityLevel, isTactical: false, isLocked: false, upgradeTraitsData.UnlockingLevel, index2);
					if (upgradeTraitsData2 != null)
					{
						UpgradeTraits.Remove(upgradeTraitsData);
						RemoveTrait(upgradeTraitsData.Identifier);
						UnregisterTraitAbilityDependencies(upgradeTraitsData.Identifier);
						AddTrait(upgradeTraitsData2.Identifier);
						ConfigureBaseAttributes();
						if (LastUpgradedTraitId == upgradeTraitsData.Identifier)
						{
							LastUpgradedTraitId = upgradeTraitsData2.Identifier;
						}
						PreviousRandomRolledTraits = null;
						ClearRerolledData();
						result = true;
					}
				}
			}
			else
			{
				PreviousRandomRolledTraits = new List<string>(RandomTraitsFromReroll);
				ClearRerolledData();
				result = true;
			}
			return result;
		}
		#endregion
	}
}
