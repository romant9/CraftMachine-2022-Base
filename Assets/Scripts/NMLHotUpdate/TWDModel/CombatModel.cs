using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class CombatModel : TWDModelObject
	{
		protected int AdditionalInitialWalkers;

		public int AdditionalRaiderLevel;

		public static int DefaultLootKeyAmount = 3;

		public static int MaxLootKeyAmount = 3;

		public CombatHUDStateInfo CombatHUDState;

		public List<SurvivorModel> MissionRoster = new List<SurvivorModel>();

		[JsonIgnore]
		private bool missionLogicFailTurnLimitResolved;

		private List<CombatColliderModel> dynamicVisibilityColliders = new List<CombatColliderModel>();

		private List<CombatColliderModel> dynamicMovementColliders = new List<CombatColliderModel>();

		[JsonIgnore]
		private GridField<bool> blockedCache;

		[JsonIgnore]
		private List<List<GridCoordinate>> colliderAffectedCoords;

		[JsonIgnore]
		private List<bool> prevColliderEnabledStates;

		[JsonIgnore]
		private GridField<byte> traversableCache;

		[JsonIgnore]
		private List<List<(GridCoordinate coord, int neighborIndex)>> colliderAffectedEdges;

		[JsonIgnore]
		private BitArray visibilityCache;

		[JsonIgnore]
		private List<List<(int fromOffset, int toOffset)>> visibilityColliderAffectedPairs;

		[JsonIgnore]
		private List<bool> prevVisibilityColliderEnabledStates;

		[JsonIgnore]
		private BitArray lineMovementBlockedCache;

		private Dictionary<Type, List<TWDModelObject>> modelsByType = new Dictionary<Type, List<TWDModelObject>>();

		public const string actorCreated = "actorCreated";

		public const string actorKilled = "actorKilled";

		public const string modelAdded = "modelAdded";

		public const string modelRemoved = "modelRemoved";

		public const string supportExecuted = "supportExecuted";

		public const string MagazineAreasUpdate = "MagazineAreasUpdate";

		[NonSerialized]
		[JsonIgnore]
		public GridField<ActorModel> Occupiers;

		[JsonIgnore]
		private GridField<int> CoverField;

		[NonSerialized]
		[JsonIgnore]
		public GridField<InteractiveObjectModel> InteractiveObjects;

		[NonSerialized]
		[JsonIgnore]
		public WalkerRandomizer WalkerRandomizer;

		[JsonIgnore]
		public bool ClearRaiderLeaderTraitsPostCombat;

		public const string turnSkippedEvent = "turnSkipped";

		public const string turnEndedEvent = "turnEnded";

		public const string missionCompletedEvent = "missionCompleted";

		public const string suggestedInteractionTargetChanged = "suggestedInteractionTargetChanged";

		public const string turnManagerActorChangedEvent = "turnManagerActorChangedEvent";

		public const string damageDealtEvent = "damageDealt";

		public const string objectBecameVisible = "objectBecameVisible";

		public const string actorBecameVisible = "actorBecameVisible";

		public const string actorBecameHidden = "actorBecameHidden";

		public const string actorWokeUp = "actorWokeUp";

		public const string survivorInjured = "survivorInjured";

		public const string missionLoadedEvent = "missionLoadedEvent";

		public const string exitEnabled = "exitEnabled";

		public const string survivorTurnEnd = "survivorTurnEnd";

		public const string FlushthreatTurn = "FlushthreatTurn";

		public const string DebuffDamagePerRound = "DebuffDamagePerRound";

		public const string redactEndEvent = "redactEndEvent";

		public const string collidersUpdated = "collidersUpdated";

		public const string PvPMissonObjectiveCompleted = "PvPMissonObjectiveCompleted";

		public const string TurnTimerActivated = "TurnTimerActivated";

		public const string MuteStateChanged = "MuteStateChanged";

		public const string actorRemoved = "actorRemoved";

		public const string actorTransformed = "actorTransformed";

		public const string EndlessModeWaveSpawned = "EndlessModeWaveSpawned";

		public const string EndlessModeScoreChanged = "EndlessModeScoreChanged";

		public const string GuildBossPointChanged = "GuildBossPointChanged";

		public const string BattlePassCurrencyEarned = "BattlePassCurrencyEarned";

		public const string EndlessModeMultiplierReduced = "EndlessModeMultiplierReduced";

		private SurvivalMissionConfig survivalMissionConfig;

		private List<ActorModel> DisorientedModels = new List<ActorModel>();

		public Dictionary<int, int> bounsPhonePortraitTurnKilledNum = new Dictionary<int, int>();

		private AssistAttackContainer AssistAttackContainer = new AssistAttackContainer();

		public int DeadlyFocus_TurnsEXAttack;

		public const int DeadlyFocus_MaxTurnsEXAttack = 180;

		[JsonIgnore]
		public AILog AILog { get; private set; }

		public OutpostCombat OutpostCombat { get; set; }

		public EndlessModeCombatModel EndlessModeCombatModel { get; set; }

		public ModelList<TWDModelObject> Models { get; set; }

		public ModelList<ActorModel> Walkers { get; set; }

		public ModelList<ActorModel> Environmentals { get; set; }

		public ModelList<ActorModel> Dormants { get; set; }

		public ModelList<ActorModel> Raiders { get; set; }

		public ModelList<ActorModel> Civilians { get; set; }

		public ModelList<ActorModel> Lures { get; set; }

		public List<string> AppliedBonuses { get; set; }

		public long CombatStartTime { get; set; }

		public int SessionResumeCount { get; set; }

		public string MissionNameEnglish { get; set; }

		public List<int> WalkerLevels { get; set; }

		public List<int> RaiderLevels { get; set; }

		public ModelList<ActorModel> ExtraSurvivors { get; set; }

		[IgnoreModelProperty]
		public ModelList<ActorModel> Perceptors { get; set; }

		[IgnoreModelProperty]
		public ModelList<ActorModel> Survivors { get; set; }

		[IgnoreModelProperty]
		public ModelList<ActorModel> AllActors { get; set; }

		public Dictionary<Faction, List<ActorModel>> AttackedTargetsThisTurn { get; set; }

		public Dictionary<int, ActorModel> SurvivorSlots { get; set; }

		public Dictionary<int, ActorModel> RaiderSlots { get; set; }

		public List<FactionAIController> FactionAIControllers { get; set; }

		public int SpawnedWalkerCount { get; set; }

		public int ThreatIncreasePerTurn { get; private set; }

		public double GuildBossPoint { get; set; }

		public long GuildBossDamage { get; private set; }

		public bool MissionStarted { get; set; }

		public bool MissionStartedChanged { get; set; }

		public bool MissionCompleted { get; set; }

		public bool ResultsResolved { get; set; }

		public ECombatResult MissionResult { get; set; }

		public string CombatFailureReason { get; set; } = "";

		public MissionFactionNames[] FactionNames { get; set; }

		public MissionType MissionType { get; set; }

		public bool IsDeadly { get; set; }

		public int MaxEnemyKillsGivingXP { get; set; }

		public string RunLocationVersion { get; set; }

		public string RunLocationExportHash { get; set; }

		public bool HasPvPRules
		{
			get
			{
				if (PvPMissionType != PvPMissionType.PVPMultiFlag && PvPMissionType != PvPMissionType.PVPMultiLoot && PvPMissionType != PvPMissionType.FakePVPMultiFlag)
				{
					return PvPMissionType == PvPMissionType.FakePVPMultiLoot;
				}
				return true;
			}
		}

		public bool IsPVPMission
		{
			get
			{
				if (PvPMissionType != PvPMissionType.PVPMultiFlag)
				{
					return PvPMissionType == PvPMissionType.PVPMultiLoot;
				}
				return true;
			}
		}

		public bool IsFakePVPMission
		{
			get
			{
				if (PvPMissionType != PvPMissionType.FakePVPMultiFlag)
				{
					return PvPMissionType == PvPMissionType.FakePVPMultiLoot;
				}
				return true;
			}
		}

		public int PvPCollectedLootsCount { get; set; }

		public int PvPCollectedFlagsCount { get; set; }

		[JsonIgnore]
		public bool IsPvPLootCollected
		{
			get
			{
				List<TWDModelObject> models = GetModels<OutpostObjectiveModel>();
				int num = 0;
				for (int i = 0; i < models.Count; i++)
				{
					if (models[i] is OutpostObjectiveModel { OutpostObjectiveType: OutpostObjectiveType.ResourceContainer })
					{
						num++;
					}
				}
				return PvPCollectedLootsCount >= num;
			}
		}

		[JsonIgnore]
		public bool IsPvPFlagCollected
		{
			get
			{
				List<TWDModelObject> models = GetModels<OutpostObjectiveModel>();
				int num = 0;
				for (int i = 0; i < models.Count; i++)
				{
					if (models[i] is OutpostObjectiveModel { OutpostObjectiveType: OutpostObjectiveType.Flag })
					{
						num++;
					}
				}
				return PvPCollectedFlagsCount >= num;
			}
		}

		[JsonIgnore]
		public bool IsPvpDefendersKilled
		{
			get
			{
				if (Raiders != null)
				{
					return Raiders.Count == 0;
				}
				return true;
			}
		}

		public int PvPDefendersKilledCount { get; set; }

		public List<int> PVPKilledDefenderIndices { get; set; }

		public List<int> GuildBattlePVPSurvivorsKilledIndices { get; set; }

		public DropEventDefinition.DropEventTag LootTag { get; set; }

		public long SurvivalPointsAtStart { get; set; }

		public bool RandomSpawningEnabled { get; set; }

		public MissionObjective CurrentMissionObjective { get; set; }

		public ModelList<SurvivalGameModel> SurvivalGameModelList { get; private set; }

		public List<GuardianVowBinding> GuardianVowBindings { get; private set; }

		[JsonIgnore]
		public MapCategory MapCategory
		{
			get
			{
				if (IsPVPMission)
				{
					return MapCategory.Outpost;
				}
				if (IsGuildBattleMission)
				{
					return MapCategory.GuildBattle;
				}
				if (IsWorldBossMission)
				{
					return GetAttackTargetWorldBossMapCategory();
				}
				MapMissionModel attackTargetMissionModel = base.manager.Player.MapContainerModel.AttackTargetMissionModel;
				if (attackTargetMissionModel != null)
				{
					MissionSpawnPointGroup missionSpawnPointGroup = attackTargetMissionModel.MissionSpawnPointGroup;
					if (attackTargetMissionModel.MissionSpawnPointGroup != null)
					{
						return missionSpawnPointGroup.Category;
					}
				}
				return MapCategory.Story;
			}
		}

		[JsonIgnore]
		public AbilityManagerModel AbilityManager => base.manager?.Player?.AbilityManager;

		public GridModel Grid { get; set; }

		public string GridColliderVisibility { get; set; }

		public string GridColliderMovement { get; set; }

		[JsonIgnore]
		public GridColliderData GridColliderData { get; set; }

		public TurnManager TurnManager { get; set; }

		public int BackUpCount { get; set; }

		public PersistentMissionVariableManager PersistentMissionVariableManager { get; set; }

		public ThreatMeterModel ThreatMeter { get; set; }

		[JsonIgnore]
		public int TurnsToWave
		{
			get
			{
				if (IsEndlessBattleMission)
				{
					return EndlessModeCombatModel.TurnsToWave;
				}
				int result = 0;
				if (ThreatMeter != null)
				{
					return ThreatMeter.TurnCounter;
				}
				return result;
			}
		}

		public PvPMissionType PvPMissionType { get; set; }

		public int AfterAlarmTurns { get; set; }

		[JsonIgnore]
		public int MissionLogicFailTurnLimit { get; private set; } = -1;

		public int MaxTime { get; set; }

		public int TurnTimerActivationTurn { get; set; }

		[JsonIgnore]
		public bool OutpostAlarmTriggered => TurnTimerActivationTurn != -1;

		[JsonIgnore]
		public int TurnsToFlee => Math.Max(0, AfterAlarmTurns - (TurnManager.TurnCount - TurnTimerActivationTurn));

		public bool OutOfTurns { get; set; }

		public Dictionary<int, int> Variables { get; set; }

		public int InitialTurnCountToWave { get; set; }

		public int InitialThreatLevel { get; set; }

		public int OptionalLootKeys { get; set; }

		public int InitialLootKeys { get; set; }

		public string CurrentMissionTextID { get; set; }

		public GridCoordinate SuggestedInteractionTargetCoordinate { get; set; }

		[IgnoreModelProperty]
		public ActorModel SuggestedInteractionActor { get; set; }

		public bool SuggestedInteractionIsForced { get; set; }

		public int StaticRewardSuppliesGranted { get; set; }

		public int StaticRewardSurvivalPointsGranted { get; set; }

		[JsonIgnore]
		public EquipmentItemModel StaticRewardStoryMissionEquipment { get; set; }

		public int StaticRewardStoryMissionAmount { get; set; }

		public CurrencyType StaticRewardStoryMissionCurrency { get; set; }

		public int StaticReward2StoryMissionAmount { get; set; }

		public CurrencyType StaticReward2StoryMissionCurrency { get; set; }

		public List<RewardCurrency> StaticRewardStoryMissionCurrencyList { get; set; }

		public int SeasonRewardMissionAmount { get; set; }

		public CurrencyType SeasonRewardMissionCurrency { get; set; }

		[JsonIgnore]
		public List<ActorSpawnPointModel> OrderedSpawnPoints { get; set; }

		[JsonIgnore]
		public bool HasForcedInteractionTarget
		{
			get
			{
				if (SuggestedInteractionTargetCoordinate != GridCoordinate.Invalid)
				{
					return SuggestedInteractionIsForced;
				}
				return false;
			}
		}

		public MissionStatistics MissionStatistics { get; set; }

		[JsonIgnore]
		public GridField<bool> BlockedCache => blockedCache;

		[JsonIgnore]
		public GridField<byte> TraversableCache => traversableCache;

		public string SceneName { get; set; }

		public string BackgroundSceneName { get; set; }

		public string CurrentMissionId { get; set; }

		public ModelList<EquipmentItemModel> CollectedRandomWeapons { get; private set; }

		[IgnoreModelProperty]
		public ModelList<SurvivorModel> RescuedSurvivors { get; set; }

		public int VideoAdsServedInRewardScreen { get; set; }

		public int CurrentTurnFlameTriggerCount { get; set; }

		public bool HasSpentLootKeyCurrency { get; set; }

		public FixedPoint SpMultiplier { get; set; }

		public bool MusicMuteForced { get; set; }

		public string IdForAnalytics { get; set; }

		public SpawnModifierState SpawnModifiers { get; set; }

		public MissionRetryState CombatRetryChoicePendingState { get; set; }

		public ECombatResult PendingCombatResult { get; set; }

		public bool RetryMission { get; set; }

		public CombatSupportManager SupportManager { get; set; }

		public RedactTimedEffect RedactTimedEffect { get; set; }

		[JsonIgnore]
		public bool IsRedacting
		{
			get
			{
				if (RedactTimedEffect != null)
				{
					return RedactTimedEffect.IsValid();
				}
				return false;
			}
		}

		[JsonIgnore]
		public ActorModel ActiveActor
		{
			get
			{
				if (TurnManager != null)
				{
					return TurnManager.ActiveActor;
				}
				return null;
			}
		}

		[IgnoreModelProperty]
		public ActorModel DashSurvivalFlagActor { get; set; }

		[IgnoreModelProperty]
		public ActorModel DashRaiderFlagActor { get; set; }

		public int DebuffQuantunRemove { get; set; }

		public int DebuffQuantunRemoveRaider { get; set; }

		[JsonIgnore]
		public int OutpostObjectivesCompletionPercentage
		{
			get
			{
				int num = 0;
				if (IsPvPLootCollected)
				{
					num += 33;
				}
				if (IsPvPFlagCollected)
				{
					num += 34;
				}
				if (IsPvpDefendersKilled)
				{
					num += 33;
				}
				return num;
			}
		}

		[JsonIgnore]
		public int OutpostObjectivesCompletionCount
		{
			get
			{
				int num = 0;
				if (IsPvPLootCollected)
				{
					num++;
				}
				if (IsPvPFlagCollected)
				{
					num++;
				}
				if (IsPvpDefendersKilled)
				{
					num++;
				}
				return num;
			}
		}

		public string SurvivalMissionConfigName { get; private set; }

		public SurvivalMissionConfig.Type SurvivalMissionConfigType { get; private set; }

		public int SurvivalMissionConfigMissionOrderInSection { get; private set; }

		[JsonIgnore]
		public SurvivalMissionConfig SurvivalMission => survivalMissionConfig;

		[JsonIgnore]
		public bool IsUsingSurvivalMissionConfig => survivalMissionConfig != null;

		[JsonIgnore]
		public bool IsSurvivalMission
		{
			get
			{
				if (survivalMissionConfig != null)
				{
					return survivalMissionConfig.MissionType == SurvivalMissionConfig.Type.Survival;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsGuildBattleMission
		{
			get
			{
				if (survivalMissionConfig != null)
				{
					return survivalMissionConfig.MissionType == SurvivalMissionConfig.Type.GuildBattle;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsWorldBossMission => base.manager?.Player?.GetAttackTargetMissionModel() is WorldBossMissionModel;

		[JsonIgnore]
		public long WorldBossTimeLimitSeconds
		{
			get
			{
				if (!(base.manager.Player.GetAttackTargetMissionModel() is WorldBossMissionModel worldBossMissionModel) || worldBossMissionModel.TimeLimitMs <= 0)
				{
					return 0L;
				}
				return worldBossMissionModel.TimeLimitMs / 1000;
			}
		}

		public bool IsEndlessBattleMission => MapCategory == MapCategory.Endless;

		[JsonIgnore]
		public bool IsGuildBossMission => GetAttackTargetWorldBossMapCategory() == MapCategory.GuildBoss;

		[JsonIgnore]
		public bool IsGuildBossPVEMission => GetAttackTargetWorldBossMapCategory() == MapCategory.GuildBossPVE;

		[JsonIgnore]
		public bool IsGuildBossPVPMission => GetAttackTargetWorldBossMapCategory() == MapCategory.GuildBossPVP;

		[JsonIgnore]
		public bool HasGuildBossRules
		{
			get
			{
				if (!IsGuildBossMission && !IsGuildBossPVEMission)
				{
					return IsGuildBossPVPMission;
				}
				return true;
			}
		}

		public ResurgenceType1Container ResurgenceType1Container { get; private set; }

		public ResurgenceType2Container ResurgenceType2Container { get; private set; }

		public AttackChainContainer AttackChainContainer { get; set; }

		public void AddGuildBossPoint(double point)
		{
			if (point > 0.0)
			{
				GuildBossPoint += point;
				NotifyChange("GuildBossPointChanged", GuildBossPoint);
			}
		}

		public void AddGuildBossDamage(long damage)
		{
			if (damage > 0)
			{
				GuildBossDamage = ((GuildBossDamage > long.MaxValue - damage) ? long.MaxValue : (GuildBossDamage + damage));
			}
		}

		private MapCategory GetAttackTargetWorldBossMapCategory()
		{
			if (base.manager.Player.GetAttackTargetMissionModel() is WorldBossMissionModel worldBossMissionModel)
			{
				if (worldBossMissionModel.WorldBossMissionType == WorldBossMissionType.PVE)
				{
					return MapCategory.GuildBossPVE;
				}
				if (worldBossMissionModel.WorldBossMissionType == WorldBossMissionType.PVP)
				{
					return MapCategory.GuildBossPVP;
				}
				if (worldBossMissionModel.WorldBossMissionType == WorldBossMissionType.BOSS)
				{
					return MapCategory.GuildBoss;
				}
			}
			return MapCategory.None;
		}

		public void AddLootSurvivor(SurvivorModel survivorModel)
		{
			RescuedSurvivors.Add(survivorModel);
		}

		public bool ContainsSurvivorEquipment(EquipmentItemModel item)
		{
			for (int i = 0; i < Survivors.Count; i++)
			{
				if (Survivors[i].EquipmentItems.Contains(item))
				{
					return true;
				}
			}
			for (int j = 0; j < ExtraSurvivors.Count; j++)
			{
				if (ExtraSurvivors[j].EquipmentItems.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		public void SetGridModel(GridModel inGrid)
		{
			Grid = inGrid;
			Grid.SetManager(base.Manager);
		}

		public void SetGridColliderData(string visibilityData, string movementData)
		{
			GridColliderVisibility = visibilityData;
			GridColliderMovement = movementData;
		}

		public void SetModels(ModelList<TWDModelObject> inModels)
		{
			Models = inModels;
			int count = Models.Count;
			for (int i = 0; i < count; i++)
			{
				TWDModelObject tWDModelObject = Models[i];
				if (tWDModelObject != null)
				{
					tWDModelObject.SetManager(base.manager);
					tWDModelObject.Initialize();
				}
				else
				{
					base.manager.Debug.LogError("Null model object in models list for SetModels function!");
				}
			}
			UpdateDynamicColliders();
			UpdateOccupiers();
		}

		public void SetupOutpostCombat(PlayerModel defendingPlayer)
		{
			OutpostCombat = new OutpostCombat();
			OutpostCombat.SetManager(base.manager);
			OutpostCombat.Initialize(defendingPlayer);
		}

		public void SetupEndlessModeCombat()
		{
			if (EndlessModeCombatModel == null)
			{
				EndlessModeCombatModel = new EndlessModeCombatModel();
				EndlessModeCombatModel.SetManager(base.manager);
				EndlessModeCombatModel.Initialize();
			}
			EndlessModeCombatModel.Start();
		}

		public void ApplyDifficultyEffect(IncrementalDifficultyEffectDefinition definition)
		{
			if (SpawnModifiers == null)
			{
				SpawnModifiers = new SpawnModifierState();
			}
			int parameter = definition.Parameter;
			switch (definition.Effect)
			{
			case IncrementalDifficultyEffect.ReduceThreatCounter:
				InitialTurnCountToWave = Math.Max(1, InitialTurnCountToWave - parameter);
				base.Debug.Log("Difficulty adjusted: -" + parameter + " to threat turn count");
				break;
			case IncrementalDifficultyEffect.WalkerMoveRange:
				SpawnModifiers.WalkerMoveRange += parameter;
				break;
			case IncrementalDifficultyEffect.PromoteThreatWalker:
			case IncrementalDifficultyEffect.PromoteThreatWalkerArmored:
			case IncrementalDifficultyEffect.PromoteThreatWalkerTank:
			case IncrementalDifficultyEffect.PromoteThreatWalkerSpiked:
			case IncrementalDifficultyEffect.PromoteThreatWalkerCommonWealth:
			{
				SpawnModifiers.PromoteThreatWalkerCount += parameter;
				WalkerType item = WalkerType.WalkerNormal;
				switch (definition.Effect)
				{
				case IncrementalDifficultyEffect.PromoteThreatWalkerArmored:
					item = WalkerType.WalkerArmored;
					break;
				case IncrementalDifficultyEffect.PromoteThreatWalkerSpiked:
					item = WalkerType.WalkerSpiked;
					break;
				case IncrementalDifficultyEffect.PromoteThreatWalkerTank:
					item = WalkerType.WalkerTank;
					break;
				case IncrementalDifficultyEffect.PromoteThreatWalkerCommonWealth:
					item = WalkerType.WalkerCommonWealth;
					break;
				}
				for (int j = 0; j < parameter; j++)
				{
					SpawnModifiers.PromoteThreatWalkerType.Add(item);
				}
				base.Debug.Log("Difficulty adjusted: promote +" + parameter + " threat walkers");
				break;
			}
			case IncrementalDifficultyEffect.AddThreatWalker:
				InitialThreatLevel += parameter;
				base.Debug.Log("Difficulty adjusted: +" + parameter + " to threat level");
				break;
			case IncrementalDifficultyEffect.PromoteMeleeRaider:
				SpawnModifiers.PromoteMeleeRaiderCount += parameter;
				base.Debug.Log("Difficulty adjusted: promote +" + parameter + " melee raiders");
				break;
			case IncrementalDifficultyEffect.PromoteRangedRaider:
				SpawnModifiers.PromoteRangedRaiderCount += parameter;
				base.Debug.Log("Difficulty adjusted: promote +" + parameter + " ranged raiders");
				break;
			case IncrementalDifficultyEffect.PromoteWalker:
			case IncrementalDifficultyEffect.PromoteWalkerArmored:
			case IncrementalDifficultyEffect.PromoteWalkerTank:
			case IncrementalDifficultyEffect.PromoteWalkerSpiked:
			case IncrementalDifficultyEffect.PromoteWalkerCommonWealth:
			{
				SpawnModifiers.PromoteWalkerCount += parameter;
				WalkerType item = WalkerType.WalkerNormal;
				switch (definition.Effect)
				{
				case IncrementalDifficultyEffect.PromoteWalkerArmored:
					item = WalkerType.WalkerArmored;
					break;
				case IncrementalDifficultyEffect.PromoteWalkerSpiked:
					item = WalkerType.WalkerSpiked;
					break;
				case IncrementalDifficultyEffect.PromoteWalkerTank:
					item = WalkerType.WalkerTank;
					break;
				case IncrementalDifficultyEffect.PromoteWalkerCommonWealth:
					item = WalkerType.WalkerCommonWealth;
					break;
				}
				for (int i = 0; i < parameter; i++)
				{
					SpawnModifiers.PromoteWalkerType.Add(item);
				}
				base.Debug.Log("Difficulty adjusted: promote +" + parameter + " walkers");
				break;
			}
			case IncrementalDifficultyEffect.UpgradePromotedWalker:
				SpawnModifiers.UpgradePromotedWalkerCount += parameter;
				base.Debug.Log("Difficulty adjusted: upgrade " + parameter + " promoted walkers by 1 level");
				break;
			case IncrementalDifficultyEffect.UpgradeWalker:
				SpawnModifiers.UpgradeWalkerCount += parameter;
				base.Debug.Log("Difficulty adjusted: upgrade " + parameter + " normal walkers by 1 level");
				break;
			case IncrementalDifficultyEffect.UpgradeRaider:
				SpawnModifiers.UpgradeRaiderCount += parameter;
				base.Debug.Log("Difficulty adjusted: upgrade " + parameter + " raiders by 1 level");
				break;
			default:
				base.Debug.LogWarning("Unsupported difficulty effect: " + definition.Effect);
				break;
			}
		}

		public void InitializeMission()
		{
			ModelRandom missionRandom = new ModelRandom(base.manager.Player.SelectedMissionRandomSeed);
			CustomizeMissionObjects(missionRandom);
			CustomizeThreatMeter();
			CreateInitialSurvivors();
			CustomizeMissionSpawning();
			CurrentTurnFlameTriggerCount = 0;
		}

		public override void SetManager(ModelManager manager)
		{
			base.Manager = manager;
			GetModelProperties();
			if (modelObjects != null)
			{
				for (int i = 0; i < modelObjects.Count; i++)
				{
					modelObjects[i].SetManager(manager);
				}
			}
		}

		public T GetModel<T>() where T : TWDModelObject
		{
			List<TWDModelObject> models = GetModels<T>();
			if (models != null && models.Count > 0)
			{
				return models[0] as T;
			}
			return null;
		}

		public List<TWDModelObject> GetModels<T>()
		{
			Type typeFromHandle = typeof(T);
			if (modelsByType.ContainsKey(typeFromHandle))
			{
				return modelsByType[typeFromHandle];
			}
			List<TWDModelObject> list = new List<TWDModelObject>();
			for (int i = 0; i < Models.Count; i++)
			{
				if (Models[i].GetType() == typeFromHandle || Models[i].GetType().IsSubclassOf(typeFromHandle))
				{
					list.Add(Models[i]);
				}
			}
			modelsByType.Add(typeFromHandle, list);
			return list;
		}

		public void AddModel(TWDModelObject objectToAdd)
		{
			Models.Add(objectToAdd);
			Type type = objectToAdd.GetType();
			foreach (Type key in modelsByType.Keys)
			{
				if (key == type || type.IsSubclassOf(key))
				{
					modelsByType[key].Add(objectToAdd);
				}
			}
			NotifyChange("modelAdded", objectToAdd);
		}

		public void RemoveModel(TWDModelObject objectToRemove)
		{
			Models.Remove(objectToRemove);
			DestroyModelIfApplicable(objectToRemove);
			Type type = objectToRemove.GetType();
			foreach (Type key in modelsByType.Keys)
			{
				if (key == type || type.IsSubclassOf(key))
				{
					modelsByType[key].Remove(objectToRemove);
				}
			}
			NotifyChange("modelRemoved", objectToRemove);
		}

		public void ClearModels<T>()
		{
			Type typeFromHandle = typeof(T);
			List<TWDModelObject> list = new List<TWDModelObject>();
			for (int i = 0; i < Models.Count; i++)
			{
				if (Models[i].GetType() == typeFromHandle || Models[i].GetType().IsSubclassOf(typeFromHandle))
				{
					list.Add(Models[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Models.Remove(list[j]);
			}
			foreach (Type key in modelsByType.Keys)
			{
				if (key == typeFromHandle || typeFromHandle.IsSubclassOf(key) || key.IsSubclassOf(typeFromHandle))
				{
					modelsByType[key].Clear();
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			CombatHUDState.Reset();
			MissionStarted = false;
			MissionStartedChanged = false;
			MissionCompleted = false;
			ResultsResolved = false;
			CombatFailureReason = "";
			GuildBossPoint = 0.0;
			GuildBossDamage = 0L;
			MissionLogicFailTurnLimit = -1;
			missionLogicFailTurnLimitResolved = false;
			TurnManager = new TurnManager();
			TurnManager.SetManager(base.manager);
			TurnManager.Initialize();
			survivalMissionConfig = null;
			SurvivalMissionConfigName = null;
			SurvivalMissionConfigType = SurvivalMissionConfig.Type.Invalid;
			SurvivalMissionConfigMissionOrderInSection = -1;
			PersistentMissionVariableManager = new PersistentMissionVariableManager();
			PersistentMissionVariableManager.SetManager(base.manager);
			PersistentMissionVariableManager.Initialize();
			MissionFlavorData missionFlavorData = base.gameEconomyData.GetMissionFlavorData(base.manager.Player.SelectedMissionFlavor);
			ThreatMeter = new ThreatMeterModel();
			ThreatMeter.SetManager(base.manager);
			ThreatMeter.Initialize();
			ThreatMeter.ChangeThreatLevel(missionFlavorData.InitialThreat, ThreatInstigator.TurnCount);
			MissionStatistics = new MissionStatistics();
			MissionStatistics.SetManager(base.manager);
			MissionStatistics.Initialize();
			ConfigData configData = base.manager.Player.gameEconomyData.ConfigData;
			ThreatIncreasePerTurn = configData.ThreatIncreasePerTurn;
			MaxTime = configData.MaxMissionTime;
			if (base.manager.Player.GetAttackTargetMissionModel() is MapMissionModel { IsWorldBoss: not false })
			{
				WorldBossConfig worldBossConfig = base.manager.Player.gameEconomyData.WorldBossConfig;
				MaxTime = worldBossConfig.BattleTimeLimit;
			}
			Models = new ModelList<TWDModelObject>();
			AllActors = new ModelList<ActorModel>();
			Survivors = new ModelList<ActorModel>();
			Walkers = new ModelList<ActorModel>();
			Environmentals = new ModelList<ActorModel>();
			Dormants = new ModelList<ActorModel>();
			Raiders = new ModelList<ActorModel>();
			Civilians = new ModelList<ActorModel>();
			Lures = new ModelList<ActorModel>();
			Perceptors = new ModelList<ActorModel>();
			AppliedBonuses = new List<string>();
			SurvivorSlots = new Dictionary<int, ActorModel>();
			RaiderSlots = new Dictionary<int, ActorModel>();
			ExtraSurvivors = new ModelList<ActorModel>();
			PVPKilledDefenderIndices = new List<int>();
			GuildBattlePVPSurvivorsKilledIndices = new List<int>();
			FactionAIControllers = new List<FactionAIController>();
			for (int i = 0; i < Enum.GetValues(typeof(Faction)).Length; i++)
			{
				FactionAIControllers.Add(new FactionAIController((Faction)i, this));
			}
			RescuedSurvivors = new ModelList<SurvivorModel>();
			CollectedRandomWeapons = new ModelList<EquipmentItemModel>();
			base.manager.Player.Blackboard.IncreaseCounter("Counter.NumberMissionPlayed");
			CurrentMissionObjective = new MissionObjective();
			Variables = new Dictionary<int, int>();
			SuggestedInteractionActor = null;
			SuggestedInteractionTargetCoordinate = GridCoordinate.Invalid;
			SuggestedInteractionIsForced = false;
			SurvivalPointsAtStart = base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints).TotalValue;
			ResurgenceType1Container = new ResurgenceType1Container();
			ResurgenceType1Container.ResurgenceType1InfoRecords = new List<ResurgenceType1Info>();
			ResurgenceType2Container = new ResurgenceType2Container();
			ResurgenceType2Container.ResurgenceType2InfoRecords = new List<ResurgenceType2Info>();
			AttackChainContainer = new AttackChainContainer();
			AttackChainContainer.AttackChainSourceInfoRecords = new List<AttackChainSourceInfo>();
			VideoAdsServedInRewardScreen = 0;
			HasSpentLootKeyCurrency = false;
			CurrentTurnFlameTriggerCount = 0;
			if (base.manager.Player.AchievementManager != null)
			{
				base.manager.Player.AchievementManager.MarkChallengeBonusValidity();
			}
			TurnTimerActivationTurn = -1;
			CombatStartTime = base.manager.Player.UtcTimeStamp;
			SessionResumeCount = 0;
			IdForAnalytics = "0";
			SurvivalGameModelList = new ModelList<SurvivalGameModel>();
			GuardianVowBindings = new List<GuardianVowBinding>();
		}

		public int GetOrCreateVariable(int variableHash)
		{
			if (Variables.ContainsKey(variableHash))
			{
				return Variables[variableHash];
			}
			Variables.Add(variableHash, 0);
			return 0;
		}

		public void SetVariable(int variableHash, int value)
		{
			if (Variables.ContainsKey(variableHash))
			{
				Variables[variableHash] = value;
			}
			else
			{
				Variables.Add(variableHash, value);
			}
		}

		public int GetMaximumInitialWalkers()
		{
			MissionGenerationData missionGenerationData = base.gameEconomyData.GetMissionGenerationData(base.manager.Player.SelectedMissionDifficulty);
			MissionFlavorData missionFlavorData = base.gameEconomyData.GetMissionFlavorData(base.manager.Player.SelectedMissionFlavor);
			return (int)((FixedPoint)(Grid.Width * Grid.Height) / (FixedPoint)missionGenerationData.MaxTotalWalkers * missionFlavorData.InitialWalkerAmount + 0.5);
		}

		public int GetRandomWalkerLevel()
		{
			if (WorldBossMissionModel.TryGetEnemyLevel(base.manager.Player.GetAttackTargetMissionModel(), out var enemyLevel))
			{
				return enemyLevel;
			}
			MissionGenerationData missionGenerationData = base.gameEconomyData.GetMissionGenerationData(base.manager.Player.SelectedMissionDifficulty);
			return base.manager.Player.PlayerRandom.GetRandomInRange(missionGenerationData.MinWalkerLevel, missionGenerationData.MaxWalkerLevel);
		}

		private void CustomizeMissionObjects(ModelRandom missionRandom)
		{
			List<TWDModelObject> models = GetModels<LootModel>();
			List<LootModel> list = new List<LootModel>();
			for (int i = 0; i < models.Count; i++)
			{
				LootModel lootModel = models[i] as LootModel;
				lootModel.ContainsKey = false;
				list.Add(lootModel);
			}
			UtilsArray.ShuffleList(list, base.manager.Player.PlayerRandom);
			int val = Math.Max(DefaultLootKeyAmount - InitialLootKeys, 0);
			int num = Math.Min(list.Count, val);
			for (int j = 0; j < num; j++)
			{
				list[j].ContainsKey = true;
			}
		}

		public void ShuffleLootKeys()
		{
			List<TWDModelObject> models = GetModels<LootModel>();
			List<LootModel> list = new List<LootModel>();
			int num = 0;
			for (int i = 0; i < models.Count; i++)
			{
				LootModel lootModel = models[i] as LootModel;
				if (!lootModel.IsOpened)
				{
					lootModel.ContainsKey = false;
					list.Add(lootModel);
				}
				else if (lootModel.ContainsKey)
				{
					num++;
				}
			}
			UtilsArray.ShuffleList(list, base.manager.Player.PlayerRandom);
			int val = Math.Max(DefaultLootKeyAmount - InitialLootKeys - num, 0);
			int num2 = Math.Min(list.Count, val);
			for (int j = 0; j < num2; j++)
			{
				list[j].ContainsKey = true;
			}
		}

		private void CustomizeMissionSpawning()
		{
			if (RandomSpawningEnabled)
			{
				int maximumInitialWalkers = GetMaximumInitialWalkers();
				int num = GetFactionActors(Faction.Walker).Count + GetFactionActors(Faction.Dormant).Count;
				int num2 = maximumInitialWalkers - num;
				if (num2 > 0)
				{
					SpawnRandomWalkers(num2);
				}
			}
			if (AdditionalInitialWalkers > 0)
			{
				SpawnRandomWalkers(AdditionalInitialWalkers);
			}
		}

		public bool IsGuildBattle()
		{
			return SurvivalMissionConfigType == SurvivalMissionConfig.Type.GuildBattle;
		}

		private void CustomizeThreatMeter()
		{
			ThreatMeter.SetupForCombat(this);
		}

		public List<ActorModel> GetStunnedWalkers()
		{
			List<ActorModel> list = new List<ActorModel>();
			foreach (ActorModel walker in Walkers)
			{
				if (walker.IsStunned)
				{
					list.Add(walker);
				}
			}
			return list;
		}

		public List<ActorModel> GetStunnedRaiders()
		{
			List<ActorModel> list = new List<ActorModel>();
			foreach (ActorModel raider in Raiders)
			{
				if (raider.IsStunned)
				{
					list.Add(raider);
				}
			}
			return list;
		}

		private List<CombatStartLocationModel> GetStartLocations()
		{
			List<CombatStartLocationModel> list = new List<CombatStartLocationModel>();
			List<TWDModelObject> models = GetModels<CombatStartLocationModel>();
			for (int i = 0; i < models.Count; i++)
			{
				CombatStartLocationModel item = models[i] as CombatStartLocationModel;
				list.Add(item);
			}
			list.StableSort((CombatStartLocationModel a, CombatStartLocationModel b) => a.Order.CompareTo(b.Order));
			return list;
		}

		private void SpawnRandomWalkers(int count)
		{
			List<CombatStartLocationModel> startLocations = GetStartLocations();
			List<GridCoordinate> list = new List<GridCoordinate>();
			for (int i = 0; i < Grid.Height; i++)
			{
				for (int j = 0; j < Grid.Width; j++)
				{
					GridCoordinate gridCoordinate = new GridCoordinate(j, i);
					if (IsBlocked(gridCoordinate))
					{
						continue;
					}
					bool flag = true;
					for (int k = 0; k < startLocations.Count; k++)
					{
						if (Grid.AreNeighbors(gridCoordinate, startLocations[k].Location))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						list.Add(gridCoordinate);
					}
				}
			}
			List<GridCoordinate> list2 = new List<GridCoordinate>();
			for (int l = 0; l < count; l++)
			{
				int randomInRange = base.manager.Player.PlayerRandom.GetRandomInRange(0, list.Count - 1);
				GridCoordinate item = list[randomInRange];
				list.RemoveAt(randomInRange);
				list2.Add(item);
			}
			for (int m = 0; m < list2.Count; m++)
			{
				if (CreateActor(list2[m], Faction.Walker, GetRandomWalkerLevel(), 0, Enum.GetName(typeof(WalkerType), WalkerType.WalkerNormal)) is WalkerModel walkerModel && walkerModel.IsValid())
				{
					walkerModel.SetupForCombat(this);
					walkerModel.AIDataModel.Alertness = AIAlertness.Idle;
					walkerModel.DormantType = DormantType.DormantNone;
					base.manager.ExecuteAction(new SpawnAction(walkerModel, null, list2[m], null));
				}
			}
		}

		private void ReSolveSurvivalConfigAtStart()
		{
			if (SurvivalMissionConfigType == SurvivalMissionConfig.Type.Survival)
			{
				SurvivalMissionConfig[] survivalMissionConfigs = base.manager.GameEconomyData.SurvivalMissionConfigs;
				for (int i = 0; i < survivalMissionConfigs.Length; i++)
				{
					if (survivalMissionConfigs[i].ConfigName == SurvivalMissionConfigName && survivalMissionConfigs[i].MissionOrderInSection == SurvivalMissionConfigMissionOrderInSection)
					{
						survivalMissionConfig = survivalMissionConfigs[i];
						break;
					}
				}
			}
			else if (SurvivalMissionConfigType == SurvivalMissionConfig.Type.GuildBattle)
			{
				base.manager.Player.GuildBattlePlayer.AttackTargetMission.Setup(base.manager);
				if (base.manager.Player.GetAttackTargetMissionModel() is GuildBattleMapMissionModel guildBattleMapMissionModel)
				{
					Tuple<int, int> configIndexes = new Tuple<int, int>(guildBattleMapMissionModel.MissionConfigIndexObjective, guildBattleMapMissionModel.MissionConfigIndexEnemies);
					if (base.manager.Player.GuildBattlePlayer.AttackTargetMission.IsPvPCombat)
					{
						survivalMissionConfig = GuildBattleMapMissionModel.GenerateSurvivalMissionConfigPVP(SurvivalMissionConfigName, configIndexes, base.manager.GameEconomyData);
					}
					else
					{
						survivalMissionConfig = GuildBattleMapMissionModel.GenerateSurvivalMissionConfig(SurvivalMissionConfigName, configIndexes, base.manager.GameEconomyData);
					}
				}
				else
				{
					base.Debug.LogError("Failed to ReSolveSurvivalConfigAtStart GetAttackTargetMissionModel was NULL");
				}
			}
			if (survivalMissionConfig == null)
			{
				base.Debug.LogError("Failed to find survival mission config in GED, config name '" + SurvivalMissionConfigName + "' and mission order number " + SurvivalMissionConfigMissionOrderInSection);
			}
		}

		private static bool IsSurvivorSpawnPoint(ActorSpawnPointModel spawnPoint)
		{
			return spawnPoint is SurvivorSpawnPointModel;
		}

		private static bool IsCivilianSpawnPoint(ActorSpawnPointModel spawnPoint)
		{
			return spawnPoint is CivilianSpawnPointModel;
		}

		private static bool IsRaiderSpawnPoint(ActorSpawnPointModel spawnPoint)
		{
			return spawnPoint is RaiderSpawnPointModel;
		}

		private static bool IsSpecialWalkerSpawnPoint(ActorSpawnPointModel spawnPoint)
		{
			if (!(spawnPoint is WalkerSpawnPointModel))
			{
				return false;
			}
			WalkerSpawnPointModel walkerSpawnPointModel = (WalkerSpawnPointModel)spawnPoint;
			if (walkerSpawnPointModel.UseOverrideWalkerType)
			{
				return walkerSpawnPointModel.OverrideWalkerType != WalkerType.WalkerNormal;
			}
			return false;
		}

		private void GenerateOrderedSpawnPoints()
		{
			List<TWDModelObject> models = GetModels<ActorSpawnPointModel>();
			DoesSpawnerTypeMatch[] typeMatchSortFuncs = new DoesSpawnerTypeMatch[4] { IsSurvivorSpawnPoint, IsCivilianSpawnPoint, IsRaiderSpawnPoint, IsSpecialWalkerSpawnPoint };
			OrderedSpawnPoints = new List<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				OrderedSpawnPoints.Add((ActorSpawnPointModel)models[i]);
			}
			OrderedSpawnPoints.StableSort(delegate(ActorSpawnPointModel a, ActorSpawnPointModel b)
			{
				int num = typeMatchSortFuncs.Length;
				int num2 = typeMatchSortFuncs.Length;
				for (int j = 0; j < typeMatchSortFuncs.Length; j++)
				{
					if (typeMatchSortFuncs[j](a))
					{
						num = j;
						break;
					}
				}
				for (int k = 0; k < typeMatchSortFuncs.Length; k++)
				{
					if (typeMatchSortFuncs[k](b))
					{
						num2 = k;
						break;
					}
				}
				return num - num2;
			});
		}

		public override void Start()
		{
			DebugTWD.Log("Start Combat Model", DebugType.ActivateObject);

			base.Start();
			if (GuardianVowBindings == null)
			{
				GuardianVowBindings = new List<GuardianVowBinding>();
			}
			if (TurnManager != null)
			{
				TurnManager.FactionChanged -= OnFactionChanged_TickGuardianVowBindings;
				TurnManager.FactionChanged += OnFactionChanged_TickGuardianVowBindings;
			}
			if (survivalMissionConfig == null && !string.IsNullOrEmpty(SurvivalMissionConfigName))
			{
				ReSolveSurvivalConfigAtStart();
			}
			bool isHardScavenge = false;
			if (MapCategory == MapCategory.Grind && base.manager.Player.MapContainerModel.CurrentGrindMissionModel != null)
			{
				isHardScavenge = base.manager.GameEconomyData.GetGrindButtonDefinition(base.manager.Player.MapContainerModel.CurrentGrindMissionModel.GrindButtonDefinitionId).GrindDifficulty == GrindButtonDefinition.Difficulty.Hard;
			}
			WalkerRandomizer = new WalkerRandomizer(base.manager, GetMissionLevel(), MapCategory, isHardScavenge);
			GenerateOrderedSpawnPoints();
			UpdateDynamicColliders();
			ValidateCombatModel();
			ThreatMeter.Changed += OnThreatValueChanged;
			for (int i = 0; i < FactionAIControllers.Count; i++)
			{
				FactionAIControllers[i].SetCombatModel(this);
			}
			if (IsEndlessBattleMission)
			{
				SetupEndlessModeCombat();
			}
			if (OutpostAlarmTriggered)
			{
				for (int j = 0; j < Raiders.Count; j++)
				{
					if (Raiders[j].AIController != null)
					{
						Raiders[j].AIController.Enabled = true;
					}
				}
			}
			if (HelpersModel.IsUnlockAllSectors)
			{
				MissionCompleted = false;
			}
			if (!MissionCompleted)
			{
				TurnManager.ActorChanged += OnTurnManagerActorChanged;
				AbilityManager.AfterEffectApplied += OnAfterAbilityEffectApplied;
				foreach (ActorModel allActor in GetAllActors())
				{
					allActor.Changed -= OnActorChanged;
					allActor.Changed += OnActorChanged;
					allActor.Changed -= OnReceivedChargePoint;
					allActor.Changed += OnReceivedChargePoint;
				}
				UpdateObjectsVisibility();
				UpdateOccupiers();
				UpdateInteractiveObjectsField();
				UpdateCoverField();
				CheckForSpawnpointTrigger();
				if (HasPvPRules)
				{
					SpawnOutpostDefenders();
					UpdateOccupiers();
				}
				UpdateAllActorsVisibility();
				if (base.manager != null && base.manager.Player != null && base.manager.Player.ShouldConsumeMissionCurrency)
				{
					Cashier cashier = null;
					if (base.manager.Player.GetAttackTargetMissionModel() != null)
					{
						if (base.manager.Player.GetAttackTargetMissionModel() is GuildBattleMapMissionModel guildBattleMapMissionModel && base.manager.Player.GuildBattlePlayer.RetryMission)
						{
							cashier = guildBattleMapMissionModel.GetRetryGvGMissionCashier(base.manager);
							base.manager.Player.GuildBattlePlayer.UpdateRetriedMissionAttempts();
							RetryMission = true;
						}
						else
						{
							EndlessModeManagerModel endlessModeManager = base.manager.Player.EndlessModeManager;
							cashier = ((!IsEndlessBattleMission || endlessModeManager == null || endlessModeManager.EndlessModeGameModeType != EndlessModeGameModeType.Expert) ? base.manager.Player.GetAttackTargetMissionModel().GetStartMissionCashier(base.manager) : base.manager.Player.GetAttackTargetMissionModel().GetStartMissionExpertModeCashier(base.manager));
						}
					}
					else if (MapCategory == MapCategory.Outpost)
					{
						cashier = base.manager.Player.OutpostModel.GetRaidCashier();
					}
					if (cashier != null)
					{
						cashier.UseDiamondsAmount = -2;
						if (cashier.Pay(this) == TWDModelResult.OK)
						{
							base.manager.Player.ShouldConsumeMissionCurrency = false;
							base.manager.Player.GuildBattlePlayer.RetryMission = false;
							EndlessModeManagerModel endlessModeManager2 = base.manager.Player.EndlessModeManager;
							if (IsEndlessBattleMission && endlessModeManager2 != null && endlessModeManager2.EndlessModeGameModeType == EndlessModeGameModeType.Expert && cashier.ExchangedDiamonds > 0)
							{
								base.manager.Player.EndlessModeManager.CurrentExpertGoldAttemptCount++;
							}
							if (IsEndlessBattleMission && endlessModeManager2 != null && endlessModeManager2.EndlessModeGameModeType == EndlessModeGameModeType.Normal && cashier.ExchangedDiamonds > 0)
							{
								base.manager.Player.EndlessModeManager.CurrentGoldAttemptCount++;
							}
						}
						else
						{
							base.manager.Debug.LogError("Player cannot pay gas to go to mission. Cost=" + cashier.GetTotalCost(CurrencyType.Gas) + " He has:" + base.manager.Player.GetCurrency(CurrencyType.Gas).Value);
						}
					}
				}
				if (MapCategory == MapCategory.GuildBattle && base.manager.Player.GuildWarModel != null)
				{
					GuildWarModel guildWarModel = base.manager.Player.GuildWarModel;
					if (guildWarModel.CurrentBattle != null)
					{
						foreach (List<string> value in guildWarModel.CurrentBattle.CollectedBattleBonusesPerSector.Values)
						{
							for (int k = 0; k < value.Count; k++)
							{
								string text = value[k];
								AppliedBonuses.Add(text);
								base.manager.Player.AbilityManager.ApplyGuildBattleBuffs(text);
							}
						}
						GuildBattlePVPSurvivorsKilledIndices.AddRange(base.manager.Player.GuildBattlePlayer.AttackTargetMission.KilledPVPSurvivorsIndexes);
						if (guildWarModel.CurrentBattle.IsOngoing(base.manager.Player.UtcTimeStamp))
						{
							UpdateLiveData();
						}
					}
				}
				ActorModel firstSlotSurvivor = ((Survivors != null && Survivors.Count >= 1) ? Survivors[0] : null);
				AddDependentTraits(firstSlotSurvivor, Survivors);
				ActorModel firstSlotSurvivor2 = ((Raiders != null && Raiders.Count >= 1) ? Raiders[0] : null);
				AddDependentTraits(firstSlotSurvivor2, Raiders);
				for (int l = 0; l < ((Survivors != null) ? Survivors.Count : 0); l++)
				{
					if (Survivors[l] is SurvivorModel survivorModel)
					{
						survivorModel.EvaluateBadges(new BadgeContext(survivorModel, Survivors));
					}
				}
				AddForestStalkerTrait();
				AddOneWithTheHerdTrait();
				AddBeatEmUpTrait(firstSlotSurvivor, Survivors);
				AddBeatEmUpTrait(firstSlotSurvivor2, Raiders);
				AddFightingFuryTrait(firstSlotSurvivor, Survivors);
				AddFightingFuryTrait(firstSlotSurvivor2, Raiders);
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffFiringSquad", "FiringSquadLeader", "FiringSquadMember", (SurvivorModel model) => model.IsRangedClass);
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffEmitter", "EmitterCreator");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffHeadshot", "BaseHeadshot");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffRegalAuthority", "BaseRegalAuthority");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffCoupDeGrace", "BaseCoupDeGrace");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffGoodEnough", "LeaderBuffGoodEnoughCrippleBase", null, (SurvivorModel model) => !model.HasAnyLevelTrait("Crippling") || !model.HasAnyLevelTrait("Equipment_Active_Cripple"));
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffGoodEnough", "LeaderBuffGoodEnoughStaggerBase", null, (SurvivorModel model) => !model.HasAnyLevelTrait("Stagger"));
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffMadeToSuffer", "SufferCreator");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffUnleashedFighter", "BaseUnleashedFighter");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffBetterTogether", "BaseBetterTogether");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffKnockKnock", "BaseKnockKnock");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffABTester", "BaseABTester");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffClosingTime", "BaseClosingTime");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffOverload", "BaseOverload", null, (SurvivorModel model) => model.IsMeleeClass);
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffSurvivalGame", "BaseSurvivalGame");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffDeadlyFocus", "BaseDeadlyFocus");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffNoExceptions", "BaseNoExceptions", null, (SurvivorModel model) => model.IsShooterAndHunterClass);
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffShadowedGuard", "BaseShadowedGuard");
				AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders("LeaderBuffDeathsDoor", "BaseDeathsDoor", null, (SurvivorModel model) => model.SurvivorClass == SurvivorClass.Scout);
				if (SupportManager == null)
				{
					InitializeSupportManager();
				}
				AddCommonwealthArmorSupportTrait();
				AddCarolsCookiesSupportTrait();
				AddPastaSupportTrait();
				if (AbilityManager != null)
				{
					AbilityManager.CheckAndAddFeaturedHeroTraits();
					for (int num = 0; num < ((Survivors != null) ? Survivors.Count : 0); num++)
					{
						SurvivorModel survivorModel2 = Survivors[num] as SurvivorModel;
						if (survivorModel2.FeaturedDefinition != null)
						{
							survivorModel2.ConfigureBaseAttributes();
						}
					}
				}
				if (!MissionStarted)
				{
					TriggerStartCombatTraits();
				}
			}
			WalkerRandomizer.IsDisabled = true;
			if (IdForAnalytics == "0")
			{
				IdForAnalytics = CreateIdForAnalytics();
			}
		}

		private void AddForestStalkerTrait()
		{
			if (Survivors.Count <= 0)
			{
				return;
			}
			ActorModel actorModel = Survivors[0];
			if (!actorModel.HasAnyLevelTrait("LeaderBuffForestStalker"))
			{
				return;
			}
			TraitEntry traitAnyLevel = actorModel.TraitContainer.GetTraitAnyLevel("LeaderBuffForestStalker");
			string traitIdentifier = null;
			if (traitAnyLevel != null)
			{
				traitIdentifier = traitAnyLevel.TraitIdentifier;
			}
			for (int i = 1; i < Survivors.Count; i++)
			{
				if (Survivors[i] != null && Survivors[i] is SurvivorModel && ((SurvivorModel)Survivors[i]).IsMeleeClass)
				{
					Survivors[i].AddTemporaryTrait(traitIdentifier, -100.0, null, 0L);
					if (Survivors[i].HasAnyLevelTrait("LeaderBuffOneWithTheHerd"))
					{
						int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitIdentifier);
						Survivors[i].AddTemporaryTrait(UpgradeTraitsData.CompileUpgradeTraitIdentifier("LeaderBuffOneWithTheHerdStalker", traitLevelIdentifier, isLocked: false), default(FixedPoint), null, 0L);
					}
				}
			}
		}

		private void AddOneWithTheHerdTrait()
		{
			if (Survivors.Count <= 0)
			{
				return;
			}
			ActorModel actorModel = Survivors[0];
			if (!actorModel.HasAnyLevelTrait("LeaderBuffOneWithTheHerd"))
			{
				return;
			}
			TraitEntry traitAnyLevel = actorModel.TraitContainer.GetTraitAnyLevel("LeaderBuffOneWithTheHerd");
			string traitIdentifier = null;
			if (traitAnyLevel != null)
			{
				traitIdentifier = traitAnyLevel.TraitIdentifier;
			}
			for (int i = 1; i < Survivors.Count; i++)
			{
				if (Survivors[i] != null && Survivors[i] is SurvivorModel)
				{
					Survivors[i].AddTemporaryTrait(traitIdentifier, -100.0, null, 0L);
					if (Survivors[i].HasAnyLevelTrait("LeaderBuffForestStalker"))
					{
						int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(Survivors[i].TraitContainer.GetTraitAnyLevel("LeaderBuffForestStalker").TraitIdentifier);
						Survivors[i].AddTemporaryTrait(UpgradeTraitsData.CompileUpgradeTraitIdentifier("LeaderBuffOneWithTheHerdStalker", traitLevelIdentifier, isLocked: false), default(FixedPoint), null, 0L);
					}
				}
			}
		}

		private void AddBeatEmUpTrait(ActorModel firstSlotSurvivor, ModelList<ActorModel> survivors)
		{
			if (firstSlotSurvivor == null)
			{
				return;
			}
			if (firstSlotSurvivor.HasAnyLevelTrait("LeaderBuffBeatEmUp"))
			{
				for (int i = 0; i < survivors.Count; i++)
				{
					if (survivors[i] != null && survivors[i] is SurvivorModel && ((SurvivorModel)survivors[i]).IsMeleeClass)
					{
						survivors[i].AddTemporaryTrait("BaseBeatEmUp", default(FixedPoint), null, 0L);
					}
				}
				return;
			}
			for (int j = 1; j < survivors.Count; j++)
			{
				if (survivors[j].HasAnyLevelTrait("LeaderBuffBeatEmUp"))
				{
					survivors[j].AddTemporaryTrait("BaseBeatEmUp", default(FixedPoint), null, 0L);
					break;
				}
			}
		}

		private void AddFightingFuryTrait(ActorModel firstSlotSurvivor, ModelList<ActorModel> survivors)
		{
			if (firstSlotSurvivor == null)
			{
				return;
			}
			if (firstSlotSurvivor.HasAnyLevelTrait("LeaderBuffFightingFury"))
			{
				for (int i = 0; i < survivors.Count; i++)
				{
					if (survivors[i] != null && survivors[i] is SurvivorModel { IsMeleeClass: not false } survivorModel)
					{
						survivorModel.AddTemporaryTrait("BaseFightingFury", default(FixedPoint), null, 0L);
					}
				}
				return;
			}
			for (int j = 0; j < survivors.Count; j++)
			{
				if (survivors[j].HasAnyLevelTrait("LeaderBuffFightingFury"))
				{
					survivors[j].AddTemporaryTrait("FightingFury", default(FixedPoint), null, 0L);
					break;
				}
			}
		}

		private static ActorModel GetActorWithAnyLevelTrait(IEnumerable<ActorModel> actors, string trait)
		{
			foreach (ActorModel actor in actors)
			{
				if (actor.HasAnyLevelTrait(trait))
				{
					return actor;
				}
			}
			return null;
		}

		private void AddGenericLeaderBuffWithBaseTrait(ModelList<ActorModel> survivors, string leaderBuff, string baseTrait, string memberTrait = null, Func<SurvivorModel, bool> additionalCondition = null)
		{
			ActorModel actorWithAnyLevelTrait = GetActorWithAnyLevelTrait(survivors, leaderBuff);
			if (actorWithAnyLevelTrait == null)
			{
				return;
			}
			bool flag = (actorWithAnyLevelTrait as SurvivorModel)?.IsLeader ?? false;
			bool flag2 = !string.IsNullOrEmpty(memberTrait);
			for (int i = 0; i < survivors.Count; i++)
			{
				if (survivors[i] is SurvivorModel survivorModel && (additionalCondition == null || additionalCondition(survivorModel)))
				{
					if (flag2)
					{
						survivorModel.AddTemporaryTrait(memberTrait, default(FixedPoint), null, 0L);
					}
					if (flag || survivorModel.HasAnyLevelTrait(leaderBuff))
					{
						survivorModel.AddTemporaryTrait(baseTrait, default(FixedPoint), null, 0L);
					}
				}
			}
		}

		private void AddGenericLeaderBuffWithBaseTraitToSurvivorsAndRaiders(string leaderBuff, string baseTrait, string memberTrait = null, Func<SurvivorModel, bool> additionalCondition = null)
		{
			ModelList<ActorModel>[] array = new ModelList<ActorModel>[2] { Survivors, Raiders };
			foreach (ModelList<ActorModel> survivors in array)
			{
				AddGenericLeaderBuffWithBaseTrait(survivors, leaderBuff, baseTrait, memberTrait, additionalCondition);
			}
		}

		public void AddCommonwealthArmorSupportTrait()
		{
			CombatSupportModel combatSupportModel = SupportManager.Supports.FirstOrDefault((CombatSupportModel combatSupportModel2) => combatSupportModel2.SupportId == "CommonwealthArmor");
			if (combatSupportModel != null && combatSupportModel.AttachedSurvivor != null)
			{
				combatSupportModel.AttachedSurvivor.AddTemporaryTrait("CommonwealthArmorTrait", default(FixedPoint), null, 0L);
				SurvivorModel attachedSurvivor = combatSupportModel.AttachedSurvivor;
				FixedPoint? chance = combatSupportModel.SupportModel.GetParameter(1) * 0.009999999776482582;
				attachedSurvivor.AddTemporaryTrait("CommonwealthArmorExtraChargeChance", default(FixedPoint), chance, 0L);
			}
		}

		public void AddPastaSupportTrait()
		{
			CombatSupportModel combatSupportModel = SupportManager.Supports.FirstOrDefault((CombatSupportModel combatSupportModel2) => combatSupportModel2.SupportId == "Pasta");
			if (combatSupportModel != null && combatSupportModel.AttachedSurvivor != null)
			{
				combatSupportModel.AttachedSurvivor.AddTemporaryTrait("PastaSupportTrait", default(FixedPoint), null, 0L);
			}
		}

		private void AddCarolsCookiesSupportTrait()
		{
			CombatSupportModel combatSupportModel = SupportManager.Supports.FirstOrDefault((CombatSupportModel combatSupportModel2) => combatSupportModel2.SupportId == "CarolsCookies");
			if (combatSupportModel != null && combatSupportModel.AttachedSurvivor != null)
			{
				combatSupportModel.AttachedSurvivor.AddTemporaryTrait("CarolsCookiesTrait", default(FixedPoint), null, 0L);
			}
		}

		private void TriggerStartCombatTraits()
		{
			base.manager.ExecuteAction(new PostChangeTurnAction());
		}

		private void AddDependentTraits(ActorModel firstSlotSurvivor, ModelList<ActorModel> survivors)
		{
			if (firstSlotSurvivor == null)
			{
				return;
			}
			List<TraitEntry> traits = firstSlotSurvivor.GetTraits();
			List<TraitDefinition> list = new List<TraitDefinition>();
			for (int i = 0; i < traits.Count; i++)
			{
				TraitEntry traitEntry = traits[i];
				TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(traitEntry.TraitIdentifier);
				if (traitDefinition == null || !firstSlotSurvivor.HasTrait(traitEntry.TraitIdentifier) || !traitDefinition.HasTag("FactionBuffTrait") || traitDefinition.DependsOnTraits == null || traitDefinition.DependsOnTraits.Count <= 0)
				{
					continue;
				}
				for (int j = 0; j < traitDefinition.DependsOnTraits.Count; j++)
				{
					string text = traitDefinition.DependsOnTraits[j];
					if (!string.IsNullOrEmpty(text))
					{
						TraitDefinition traitDefinition2 = base.manager.GameEconomyData.GetTraitDefinition(text);
						if (traitDefinition2 != null)
						{
							list.Add(traitDefinition2);
						}
					}
				}
			}
			if (list == null || list.Count <= 0)
			{
				return;
			}
			for (int k = 0; k < survivors.Count; k++)
			{
				ActorModel actorModel = survivors[k];
				for (int l = 0; l < list.Count; l++)
				{
					actorModel.AddTemporaryTrait(list[l].Identifier, default(FixedPoint), null, 0L);
				}
			}
		}

		private void SpawnOutpostDefenders()
		{
			if (OutpostCombat == null)
			{
				return;
			}
			List<TWDModelObject> models = GetModels<PvPDefenderModel>();
			for (int i = 0; i < models.Count; i++)
			{
				PvPDefenderModel pvPDefenderModel = models[i] as PvPDefenderModel;
				if (pvPDefenderModel.DefenderIndex == 0)
				{
					pvPDefenderModel.Spawn();
				}
			}
			for (int j = 0; j < models.Count; j++)
			{
				PvPDefenderModel pvPDefenderModel2 = models[j] as PvPDefenderModel;
				if (pvPDefenderModel2.DefenderIndex != 0)
				{
					pvPDefenderModel2.Spawn();
				}
			}
		}

		public void CheckForSpawnpointTrigger()
		{
			if (IsEndlessBattleMission)
			{
				if (EndlessModeCombatModel.CanSpawnWave())
				{
					NotifyChange("EndlessModeWaveSpawned", EndlessModeCombatModel.GetNextWaveSpawnCount);
					CheckTraitsForThreatValueChange();
				}
				if (EndlessModeCombatModel.IsOverRunByWalkerLevelDifference)
				{
					EndlessModeCombatModel.KillLowLevelWalkers();
				}
				return;
			}
			foreach (ActorSpawnPointModel orderedSpawnPoint in OrderedSpawnPoints)
			{
				if (orderedSpawnPoint != null && orderedSpawnPoint.CanActivate && orderedSpawnPoint.ActivateAtTurn(TurnManager.TurnCount))
				{
					orderedSpawnPoint.Activate(TurnManager.TurnCount == 0);
				}
			}
		}

		private void UpdateLiveData()
		{
			string text = base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel?.Id ?? null;
			base.manager.Player.GuildWarModel.CurrentBattle.LiveMissionDataPerPlayer.TryGetValue(base.manager.Player.HashedId, out var value);
			if (value?.LastAttackedMissionId != text)
			{
				UpdateLiveDataGroupCommand updateLiveDataGroupCommand = new UpdateLiveDataGroupCommand();
				updateLiveDataGroupCommand.Timestamp = base.manager.Player.UtcTimeStamp;
				updateLiveDataGroupCommand.UniqueMissionId = text;
				updateLiveDataGroupCommand.Attacks = base.manager.Player.GetCurrencyAmount(CurrencyType.GvGMissionKey);
				HelpersModel.ExecuteGroupCommand(base.manager, updateLiveDataGroupCommand);
			}
		}

		private static FixedPoint SquaredDistanceToSegment(GridCoordinate coord, GridCoordinate start, GridCoordinate end)
		{
			FixedPoint fixedPoint = start.X;
			FixedPoint fixedPoint2 = start.Y;
			FixedPoint fixedPoint3 = end.X - fixedPoint;
			FixedPoint fixedPoint4 = end.Y - fixedPoint2;
			FixedPoint fixedPoint5 = fixedPoint3 * fixedPoint3 + fixedPoint4 * fixedPoint4;
			FixedPoint fixedPoint6 = coord.X - fixedPoint;
			FixedPoint fixedPoint7 = coord.Y - fixedPoint2;
			if (fixedPoint5 <= 0.0)
			{
				return fixedPoint6 * fixedPoint6 + fixedPoint7 * fixedPoint7;
			}
			FixedPoint fixedPoint8 = fixedPoint3 * fixedPoint6 + fixedPoint4 * fixedPoint7;
			if (fixedPoint8 <= 0.0)
			{
				return fixedPoint6 * fixedPoint6 + fixedPoint7 * fixedPoint7;
			}
			if (fixedPoint8 >= fixedPoint5)
			{
				FixedPoint fixedPoint9 = (FixedPoint)coord.X - (FixedPoint)end.X;
				FixedPoint fixedPoint10 = (FixedPoint)coord.Y - (FixedPoint)end.Y;
				return fixedPoint9 * fixedPoint9 + fixedPoint10 * fixedPoint10;
			}
			FixedPoint fixedPoint11 = fixedPoint6 * fixedPoint6 + fixedPoint7 * fixedPoint7 - fixedPoint8 * fixedPoint8 / fixedPoint5;
			if (!(fixedPoint11 < 0.0))
			{
				return fixedPoint11;
			}
			return 0.0;
		}

		public GridCoordinate GetClosestOnLineCell(ActorModel actor, GridCoordinate from, GridCoordinate to)
		{
			if (actor == null)
			{
				return GridCoordinate.Invalid;
			}
			if (!actor.IsMultiCell)
			{
				return actor.GridCoordinate;
			}
			FixedPoint fixedPoint = 0.25;
			List<GridCoordinate> occupiedCells = actor.GetOccupiedCells();
			GridCoordinate gridCoordinate = GridCoordinate.Invalid;
			int num = int.MaxValue;
			for (int i = 0; i < occupiedCells.Count; i++)
			{
				if (SquaredDistanceToSegment(occupiedCells[i], from, to) <= fixedPoint)
				{
					int num2 = occupiedCells[i].SquaredDistanceTo(from);
					if (gridCoordinate == GridCoordinate.Invalid || num2 < num)
					{
						num = num2;
						gridCoordinate = occupiedCells[i];
					}
				}
			}
			if (gridCoordinate == GridCoordinate.Invalid)
			{
				gridCoordinate = actor.GetClosestOccupiedCell(from);
			}
			return gridCoordinate;
		}

		private int EdgeDistanceFromShooter(ActorModel actor, GridCoordinate from, GridCoordinate to)
		{
			if (!actor.IsMultiCell)
			{
				return actor.GridCoordinate.SquaredDistanceTo(from);
			}
			return GetClosestOnLineCell(actor, from, to).SquaredDistanceTo(from);
		}

		public int CountOccupiedCellsOnShotLine(ActorModel actor, GridCoordinate from, GridCoordinate to, FixedPoint halfWidthSqr)
		{
			if (actor == null)
			{
				return 0;
			}
			if (!actor.IsMultiCell)
			{
				if (!(SquaredDistanceToSegment(actor.GridCoordinate, from, to) <= halfWidthSqr))
				{
					return 0;
				}
				return 1;
			}
			List<GridCoordinate> occupiedCells = actor.GetOccupiedCells();
			if (occupiedCells == null || occupiedCells.Count == 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < occupiedCells.Count; i++)
			{
				if (SquaredDistanceToSegment(occupiedCells[i], from, to) <= halfWidthSqr)
				{
					num++;
				}
			}
			return num;
		}

		public int CountOccupiedCellsInRange(ActorModel actor, GridCoordinate center, int range, bool diagonal = true)
		{
			if (actor == null)
			{
				return 0;
			}
			FixedPoint fixedPoint = ((float)range + (diagonal ? 0.42f : 0f)) * Grid.CellSize.X;
			fixedPoint *= fixedPoint;
			FixedVec3 position = Grid.GetPosition(center);
			if (!actor.IsMultiCell)
			{
				if (!((Grid.GetPosition(actor.GridCoordinate) - position).SqrMagnitude <= fixedPoint))
				{
					return 0;
				}
				return 1;
			}
			List<GridCoordinate> occupiedCells = actor.GetOccupiedCells();
			if (occupiedCells == null || occupiedCells.Count == 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < occupiedCells.Count; i++)
			{
				if ((Grid.GetPosition(occupiedCells[i]) - position).SqrMagnitude <= fixedPoint)
				{
					num++;
				}
			}
			return num;
		}

		public bool IsGridCellPenetrable(GridCoordinate from, GridCoordinate to, GridCoordinate target)
		{
			List<ActorModel> actorsInLine = GetActorsInLine(from, to);
			actorsInLine.Sort((ActorModel x, ActorModel y) => EdgeDistanceFromShooter(x, from, to) - EdgeDistanceFromShooter(y, from, to));
			ActorModel occupier = GetOccupier(target);
			foreach (ActorModel item in actorsInLine)
			{
				if (item == occupier)
				{
					return true;
				}
				if (occupier != null && item != occupier && item.IsImpenetrable)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsDamageAreaBlockTrajectoryPenetrable(ActorModel sourceActor, GridCoordinate from, GridCoordinate to, GridCoordinate target)
		{
			ActorModel occupier = GetOccupier(target);
			if (occupier == null)
			{
				return true;
			}
			List<ActorModel> actorsInLine = GetActorsInLine(from, to, sourceActor);
			if (actorsInLine == null || actorsInLine.Count == 0)
			{
				return true;
			}
			actorsInLine.Sort(delegate(ActorModel x, ActorModel y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x == null)
				{
					return 1;
				}
				return (y == null) ? (-1) : (EdgeDistanceFromShooter(x, from, to) - EdgeDistanceFromShooter(y, from, to));
			});
			for (int num = 0; num < actorsInLine.Count; num++)
			{
				ActorModel actorModel = actorsInLine[num];
				if (actorModel != null && actorModel != sourceActor)
				{
					if (actorModel == occupier)
					{
						return true;
					}
					if (actorModel.HasDamageAreaBlock && CanDamageAreaBlockProtect(actorModel, sourceActor, occupier))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool CanDamageAreaBlockProtect(ActorModel blocker, ActorModel sourceActor, ActorModel targetActor)
		{
			if (blocker != null && sourceActor != null && targetActor != null)
			{
				return blocker.IsEnemy(sourceActor);
			}
			return false;
		}

		public GridCoordinate GetFirstNonPenetrableCoordinate(GridCoordinate from, GridCoordinate to)
		{
			List<ActorModel> actorsInLine = GetActorsInLine(from, to);
			GridCoordinate gridCoordinate = GridCoordinate.Invalid;
			foreach (ActorModel item in actorsInLine)
			{
				if (item.IsImpenetrable)
				{
					GridCoordinate gridCoordinate2 = (item.IsMultiCell ? GetClosestOnLineCell(item, from, to) : item.GridCoordinate);
					if (gridCoordinate == GridCoordinate.Invalid || from.SquaredDistanceTo(gridCoordinate2) < from.SquaredDistanceTo(gridCoordinate))
					{
						gridCoordinate = gridCoordinate2;
					}
				}
			}
			return gridCoordinate;
		}

		public GridCoordinate GetFirstDamageAreaBlockCoordinate(GridCoordinate from, GridCoordinate to, ActorModel sourceActor = null)
		{
			List<ActorModel> actorsInLine = GetActorsInLine(from, to, sourceActor);
			GridCoordinate gridCoordinate = GridCoordinate.Invalid;
			if (actorsInLine == null)
			{
				return gridCoordinate;
			}
			for (int i = 0; i < actorsInLine.Count; i++)
			{
				ActorModel actorModel = actorsInLine[i];
				if (actorModel != null && actorModel != sourceActor && actorModel.HasDamageAreaBlock && (sourceActor == null || actorModel.IsEnemy(sourceActor)))
				{
					GridCoordinate gridCoordinate2 = (actorModel.IsMultiCell ? GetClosestOnLineCell(actorModel, from, to) : actorModel.GridCoordinate);
					if (!(gridCoordinate2 == GridCoordinate.Invalid) && (gridCoordinate == GridCoordinate.Invalid || from.SquaredDistanceTo(gridCoordinate2) < from.SquaredDistanceTo(gridCoordinate)))
					{
						gridCoordinate = gridCoordinate2;
					}
				}
			}
			return gridCoordinate;
		}

		public GridCoordinate GetFirstAimTrajectoryBlockCoordinate(GridCoordinate from, GridCoordinate to, bool includeImpenetrable, ActorModel sourceActor = null)
		{
			GridCoordinate gridCoordinate = (includeImpenetrable ? GetFirstNonPenetrableCoordinate(from, to) : GridCoordinate.Invalid);
			GridCoordinate firstDamageAreaBlockCoordinate = GetFirstDamageAreaBlockCoordinate(from, to, sourceActor);
			if (!gridCoordinate.IsValid)
			{
				return firstDamageAreaBlockCoordinate;
			}
			if (!firstDamageAreaBlockCoordinate.IsValid)
			{
				return gridCoordinate;
			}
			if (from.SquaredDistanceTo(gridCoordinate) > from.SquaredDistanceTo(firstDamageAreaBlockCoordinate))
			{
				return firstDamageAreaBlockCoordinate;
			}
			return gridCoordinate;
		}

		public bool IsGridCellVisible(GridCoordinate from, GridCoordinate to)
		{
			if (visibilityCache != null && Grid != null)
			{
				int width = Grid.Width;
				int num = width * Grid.Height;
				int num2 = from.Y * width + from.X;
				int num3 = to.Y * width + to.X;
				int num4 = num2 * num + num3;
				if (num4 >= 0 && num4 < visibilityCache.Count)
				{
					return visibilityCache[num4];
				}
			}
			return CalculateIsGridCellVisible(from, to);
		}

		public bool IsGridLineMovementBlocked(GridCoordinate from, GridCoordinate to)
		{
			if (lineMovementBlockedCache != null && Grid != null)
			{
				int width = Grid.Width;
				int num = width * Grid.Height;
				int num2 = from.Y * width + from.X;
				int num3 = to.Y * width + to.X;
				int num4 = num2 * num + num3;
				if (num4 >= 0 && num4 < lineMovementBlockedCache.Count)
				{
					return lineMovementBlockedCache[num4];
				}
			}
			return CalculateIsGridLineMovementBlocked(from, to);
		}

		public bool IsGridCellVisibleByAnySurvivor(GridCoordinate targetLocation)
		{
			for (int i = 0; i < Perceptors.Count; i++)
			{
				if (IsGridCellVisible(Perceptors[i].GridCoordinate, targetLocation))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsVisibleFromAnyOccupiedCell(ActorModel observer, GridCoordinate targetCell)
		{
			if (observer == null)
			{
				return false;
			}
			if (!observer.IsMultiCell)
			{
				return IsGridCellVisible(observer.GridCoordinate, targetCell);
			}
			List<GridCoordinate> occupiedCells = observer.GetOccupiedCells();
			for (int i = 0; i < occupiedCells.Count; i++)
			{
				if (IsGridCellVisible(occupiedCells[i], targetCell))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsActorVisibleByAnySurvivor(ActorModel target)
		{
			if (target == null)
			{
				return false;
			}
			if (!target.IsMultiCell)
			{
				return IsGridCellVisibleByAnySurvivor(target.GridCoordinate);
			}
			List<GridCoordinate> occupiedCells = target.GetOccupiedCells();
			for (int i = 0; i < occupiedCells.Count; i++)
			{
				if (IsGridCellVisibleByAnySurvivor(occupiedCells[i]))
				{
					return true;
				}
			}
			return false;
		}

		public void UpdateObjectsVisibility()
		{
			List<TWDModelObject> models = GetModels<InteractiveObjectModel>();
			for (int i = 0; i < models.Count; i++)
			{
				InteractiveObjectModel interactiveObjectModel = models[i] as InteractiveObjectModel;
				if (interactiveObjectModel.IsVisibleToSurvivors)
				{
					continue;
				}
				if (interactiveObjectModel.VisibleInFog)
				{
					interactiveObjectModel.IsVisibleToSurvivors = true;
					NotifyChange("objectBecameVisible", interactiveObjectModel);
					continue;
				}
				for (int j = 0; j < Perceptors.Count; j++)
				{
					bool flag = false;
					foreach (GridCoordinate coordinate in interactiveObjectModel.Location.Coordinates)
					{
						if (IsGridCellVisible(Perceptors[j].GridCoordinate, coordinate))
						{
							flag = true;
							break;
						}
					}
					if (interactiveObjectModel.Placement == Placement.Edge || flag)
					{
						interactiveObjectModel.IsVisibleToSurvivors = true;
						NotifyChange("objectBecameVisible", interactiveObjectModel);
						break;
					}
				}
			}
		}

		public void UpdateAllActorsVisibility()
		{
			List<ActorModel> allActors = GetAllActors();
			for (int i = 0; i < allActors.Count; i++)
			{
				ActorModel actor = allActors[i];
				UpdateActorVisibility(actor);
			}
		}

		public void UpdateActorVisibility(ActorModel actor)
		{
			if (actor.IsFriendlyHuman)
			{
				actor.IsVisibleToSurvivors = true;
				return;
			}
			bool flag = false;
			for (int i = 0; i < Perceptors.Count; i++)
			{
				if (IsGridCellVisible(actor.GridCoordinate, Perceptors[i].GridCoordinate))
				{
					flag = true;
					break;
				}
			}
			if (actor.IsVisibleToSurvivors && !flag)
			{
				actor.IsVisibleToSurvivors = false;
				NotifyChange("actorBecameHidden", actor);
			}
			else if (!actor.IsVisibleToSurvivors && flag)
			{
				actor.IsVisibleToSurvivors = true;
				NotifyChange("actorBecameVisible", actor);
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public void OnExitEnabled()
		{
			NotifyChange("exitEnabled");
		}

		public void OnPostActionExecuted()
		{
			UpdateOccupiers();
			if (!MissionCompleted)
			{
				ECombatResult result = ECombatResult.Failed;
				if (CheckForEndMission(ref result))
				{
					OnMissionComplete(result, casualtiesResolved: false, (result == ECombatResult.Failed) ? ("OnPostActionExecuted_" + GetCurrentMissionFailureReason()) : "");
				}
			}
		}

		public void OnPostCommandExecuted()
		{
			CheckMissionLogic();
		}

		public void CheckMissionLogic()
		{
			if (!MissionStarted)
			{
				return;
			}
			List<TWDModelObject> models = GetModels<NodeGraph>();
			for (int i = 0; i < models.Count; i++)
			{
				if (models[i] is NodeGraph nodeGraph)
				{
					nodeGraph.Update();
				}
			}
			List<TWDModelObject> models2 = GetModels<MissionLogicModel>();
			for (int j = 0; j < models2.Count; j++)
			{
				(models2[j] as MissionLogicModel).CheckConditions();
			}
		}

		public void UpdateOccupiers()
		{
			if (Occupiers == null)
			{
				Occupiers = new GridField<ActorModel>(Grid.Width, Grid.Height, null);
			}
			Occupiers.Clear();
			int num = Enum.GetNames(typeof(Faction)).Length;
			for (int i = 0; i < num; i++)
			{
				Faction faction = (Faction)i;
				if (faction == Faction.Lure || faction == Faction.Any)
				{
					continue;
				}
				foreach (ActorModel factionActor in GetFactionActors(faction))
				{
					List<GridCoordinate> occupiedCells = factionActor.GetOccupiedCells();
					for (int j = 0; j < occupiedCells.Count; j++)
					{
						GridCoordinate coordinate = occupiedCells[j];
						if (Grid.IsCoordinateValid(coordinate))
						{
							Occupiers[coordinate] = factionActor;
						}
					}
				}
			}
		}

		public void UpdateInteractiveObjectsField()
		{
			if (InteractiveObjects == null)
			{
				InteractiveObjects = new GridField<InteractiveObjectModel>(Grid.Width, Grid.Height, null);
			}
			InteractiveObjects.Clear();
			List<TWDModelObject> models = GetModels<InteractiveObjectModel>();
			for (int i = 0; i < models.Count; i++)
			{
				InteractiveObjectModel interactiveObjectModel = models[i] as InteractiveObjectModel;
				if (interactiveObjectModel.Placement == Placement.Cell)
				{
					InteractiveObjects[interactiveObjectModel.Location.Coordinate] = interactiveObjectModel;
				}
			}
		}

		public void UpdateDynamicColliders()
		{
			List<TWDModelObject> list = GetModels<CombatColliderModel>().ToList();
			list.StableSort((TWDModelObject a, TWDModelObject b) => ((CombatColliderModel)a).ViewId.CompareTo(((CombatColliderModel)b).ViewId));
			dynamicVisibilityColliders.Clear();
			dynamicMovementColliders.Clear();
			for (int num = 0; num < list.Count; num++)
			{
				CombatColliderModel combatColliderModel = list[num] as CombatColliderModel;
				if (combatColliderModel.IsDynamic)
				{
					if (combatColliderModel.BlockVision)
					{
						dynamicVisibilityColliders.Add(combatColliderModel);
					}
					if (combatColliderModel.BlockMovement)
					{
						dynamicMovementColliders.Add(combatColliderModel);
					}
				}
			}
			if (GridColliderData == null)
			{
				GridColliderData = new GridColliderData(Grid, dynamicVisibilityColliders.Count, dynamicMovementColliders.Count, GridColliderVisibility, GridColliderMovement);
			}
			UpdateBlockedCache();
			UpdateVisibilityCache();
			UpdateLineMovementBlockedCache();
			UpdateAllActorsVisibility();
			UpdateObjectsVisibility();
			NotifyChange("collidersUpdated");
		}

		private void UpdateBlockedCache()
		{
			if (Grid == null || GridColliderData == null)
			{
				return;
			}
			int width = Grid.Width;
			int height = Grid.Height;
			int count = dynamicMovementColliders.Count;
			if (blockedCache == null || blockedCache.GetWidth() != width || colliderAffectedCoords == null || colliderAffectedCoords.Count != count)
			{
				blockedCache = new GridField<bool>(width, height, defaultValue: false);
				traversableCache = new GridField<byte>(width, height, 0);
				colliderAffectedCoords = new List<List<GridCoordinate>>(count);
				colliderAffectedEdges = new List<List<(GridCoordinate, int)>>(count);
				prevColliderEnabledStates = new List<bool>(count);
				for (int i = 0; i < count; i++)
				{
					colliderAffectedCoords.Add(new List<GridCoordinate>());
					colliderAffectedEdges.Add(new List<(GridCoordinate, int)>());
					prevColliderEnabledStates.Add(dynamicMovementColliders[i].IsEnabled);
				}
				for (int j = 0; j < height; j++)
				{
					for (int k = 0; k < width; k++)
					{
						GridCoordinate gridCoordinate = new GridCoordinate(k, j);
						for (int l = 0; l < count; l++)
						{
							if (GridColliderData.IsBlocked(gridCoordinate, l + 1))
							{
								colliderAffectedCoords[l].Add(gridCoordinate);
							}
						}
						blockedCache[gridCoordinate] = CalculateIsBlocked(gridCoordinate);
						byte b = 0;
						for (int m = 0; m < 8; m++)
						{
							GridCoordinate coordinateNeighbor = Grid.GetCoordinateNeighbor(gridCoordinate, m);
							if (!coordinateNeighbor.IsValid)
							{
								continue;
							}
							for (int n = 0; n < count; n++)
							{
								if (GridColliderData.IsMovementBlocked(gridCoordinate, coordinateNeighbor, n + 1))
								{
									colliderAffectedEdges[n].Add((gridCoordinate, m));
								}
							}
							if (CalculateCanTraverseEdge(gridCoordinate, coordinateNeighbor))
							{
								b |= (byte)(1 << m);
							}
						}
						traversableCache[gridCoordinate] = b;
					}
				}
				return;
			}
			for (int num = 0; num < count; num++)
			{
				bool isEnabled = dynamicMovementColliders[num].IsEnabled;
				bool flag = prevColliderEnabledStates[num];
				if (isEnabled != flag)
				{
					List<GridCoordinate> list = colliderAffectedCoords[num];
					for (int num2 = 0; num2 < list.Count; num2++)
					{
						GridCoordinate gridCoordinate2 = list[num2];
						blockedCache[gridCoordinate2] = CalculateIsBlocked(gridCoordinate2);
					}
					List<(GridCoordinate, int)> list2 = colliderAffectedEdges[num];
					for (int num3 = 0; num3 < list2.Count; num3++)
					{
						(GridCoordinate, int) tuple = list2[num3];
						GridCoordinate item = tuple.Item1;
						int item2 = tuple.Item2;
						GridCoordinate coordinateNeighbor2 = Grid.GetCoordinateNeighbor(item, item2);
						byte b2 = traversableCache[item];
						b2 = ((!CalculateCanTraverseEdge(item, coordinateNeighbor2)) ? ((byte)(b2 & (byte)(~(1 << item2)))) : ((byte)(b2 | (byte)(1 << item2))));
						traversableCache[item] = b2;
					}
					prevColliderEnabledStates[num] = isEnabled;
				}
			}
		}

		private bool CalculateCanTraverseEdge(GridCoordinate from, GridCoordinate to)
		{
			if (GridColliderData.IsMovementBlocked(from, to, 0))
			{
				return false;
			}
			for (int i = 0; i < dynamicMovementColliders.Count; i++)
			{
				if (dynamicMovementColliders[i].IsEnabled && GridColliderData.IsMovementBlocked(from, to, i + 1))
				{
					return false;
				}
			}
			return true;
		}

		private void UpdateVisibilityCache()
		{
			if (Grid == null || GridColliderData == null || !GridColliderData.HasVisibilityData())
			{
				return;
			}
			int width = Grid.Width;
			int height = Grid.Height;
			int num = width * height;
			int count = dynamicVisibilityColliders.Count;
			if (visibilityCache == null || visibilityCache.Count != num * num || visibilityColliderAffectedPairs == null || visibilityColliderAffectedPairs.Count != count)
			{
				visibilityCache = new BitArray(num * num);
				visibilityColliderAffectedPairs = new List<List<(int, int)>>(count);
				prevVisibilityColliderEnabledStates = new List<bool>(count);
				for (int i = 0; i < count; i++)
				{
					visibilityColliderAffectedPairs.Add(new List<(int, int)>());
					prevVisibilityColliderEnabledStates.Add(dynamicVisibilityColliders[i].IsEnabled);
				}
				for (int j = 0; j < height; j++)
				{
					for (int k = 0; k < width; k++)
					{
						GridCoordinate gridCoordinate = new GridCoordinate(k, j);
						int num2 = j * width + k;
						for (int l = 0; l < height; l++)
						{
							for (int m = 0; m < width; m++)
							{
								GridCoordinate to = new GridCoordinate(m, l);
								int num3 = l * width + m;
								int index = num2 * num + num3;
								for (int n = 0; n < count; n++)
								{
									if (GridColliderData.IsVisibilityBlocked(gridCoordinate, to, n + 1))
									{
										visibilityColliderAffectedPairs[n].Add((num2, num3));
									}
								}
								visibilityCache[index] = CalculateIsGridCellVisible(gridCoordinate, to);
							}
						}
					}
				}
				return;
			}
			for (int num4 = 0; num4 < count; num4++)
			{
				bool isEnabled = dynamicVisibilityColliders[num4].IsEnabled;
				bool flag = prevVisibilityColliderEnabledStates[num4];
				if (isEnabled != flag)
				{
					List<(int, int)> list = visibilityColliderAffectedPairs[num4];
					for (int num5 = 0; num5 < list.Count; num5++)
					{
						(int, int) tuple = list[num5];
						int item = tuple.Item1;
						int item2 = tuple.Item2;
						GridCoordinate gridCoordinate2 = new GridCoordinate(item % width, item / width);
						GridCoordinate to2 = new GridCoordinate(item2 % width, item2 / width);
						int index2 = item * num + item2;
						visibilityCache[index2] = CalculateIsGridCellVisible(gridCoordinate2, to2);
					}
					prevVisibilityColliderEnabledStates[num4] = isEnabled;
				}
			}
		}

		private bool CalculateIsGridCellVisible(GridCoordinate from, GridCoordinate to)
		{
			if (GridColliderData.IsVisibilityBlocked(from, to, 0))
			{
				return false;
			}
			for (int i = 0; i < dynamicVisibilityColliders.Count; i++)
			{
				if (dynamicVisibilityColliders[i].IsEnabled && GridColliderData.IsVisibilityBlocked(from, to, i + 1))
				{
					return false;
				}
			}
			return true;
		}

		private void UpdateLineMovementBlockedCache()
		{
			if (Grid == null || blockedCache == null || traversableCache == null)
			{
				return;
			}
			int width = Grid.Width;
			int height = Grid.Height;
			int num = width * height;
			if (lineMovementBlockedCache == null || lineMovementBlockedCache.Count != num * num)
			{
				lineMovementBlockedCache = new BitArray(num * num);
			}
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					GridCoordinate gridCoordinate = new GridCoordinate(j, i);
					int num2 = i * width + j;
					for (int k = 0; k < height; k++)
					{
						for (int l = 0; l < width; l++)
						{
							GridCoordinate to = new GridCoordinate(l, k);
							int num3 = k * width + l;
							int index = num2 * num + num3;
							lineMovementBlockedCache[index] = CalculateIsGridLineMovementBlocked(gridCoordinate, to);
						}
					}
				}
			}
		}

		private bool CalculateIsGridLineMovementBlocked(GridCoordinate from, GridCoordinate to)
		{
			if (!Grid.IsCoordinateValid(from) || !Grid.IsCoordinateValid(to))
			{
				return false;
			}
			if (from == to)
			{
				return IsBlocked(from);
			}
			List<GridCoordinate> lineCoordinates = GridModel.GetLineCoordinates(from, to);
			for (int i = 1; i < lineCoordinates.Count; i++)
			{
				GridCoordinate gridCoordinate = lineCoordinates[i - 1];
				GridCoordinate gridCoordinate2 = lineCoordinates[i];
				if (!CanTraverse(null, gridCoordinate, gridCoordinate2) || IsBlocked(gridCoordinate) || IsBlocked(gridCoordinate2))
				{
					return true;
				}
			}
			return false;
		}

		private bool CalculateIsBlocked(GridCoordinate target)
		{
			if (GridColliderData.IsBlocked(target, 0))
			{
				return true;
			}
			for (int i = 0; i < dynamicMovementColliders.Count; i++)
			{
				if (dynamicMovementColliders[i].IsEnabled && GridColliderData.IsBlocked(target, i + 1))
				{
					return true;
				}
			}
			return false;
		}

		public void Uninitialize()
		{
			if (TurnManager != null)
			{
				TurnManager.ActorChanged -= OnTurnManagerActorChanged;
			}
			if (EndlessModeCombatModel != null && AbilityManager != null)
			{
				AbilityManager.AfterEffectApplied -= OnAfterAbilityEffectApplied;
			}
			while (Survivors.Count > 0)
			{
				UnregisterActor(Survivors[0], updateVisibility: false);
			}
			while (Walkers.Count > 0)
			{
				UnregisterActor(Walkers[0], updateVisibility: false);
			}
			while (Dormants.Count > 0)
			{
				UnregisterActor(Dormants[0], updateVisibility: false);
			}
			while (Civilians.Count > 0)
			{
				UnregisterActor(Civilians[0], updateVisibility: false);
			}
			while (Lures.Count > 0)
			{
				UnregisterActor(Lures[0], updateVisibility: false);
			}
			while (Raiders.Count > 0)
			{
				UnregisterActor(Raiders[0], updateVisibility: false);
			}
			while (Environmentals != null && Environmentals.Count > 0)
			{
				UnregisterActor(Environmentals[0], updateVisibility: false);
			}
			foreach (TWDModelObject model in Models)
			{
				DestroyModelIfApplicable(model);
			}
		}

		private void DestroyModelIfApplicable(TWDModelObject model)
		{
			if (model is IDestructibleCombatModel destructibleCombatModel)
			{
				destructibleCombatModel.Destroy();
			}
		}

		public void ClearSurvivors()
		{
			while (Survivors.Count > 0)
			{
				UnregisterActor(Survivors[0]);
			}
		}

		public void ClearWalkers()
		{
			while (Walkers.Count > 0)
			{
				UnregisterActor(Walkers[0]);
			}
		}

		public void RegisterActor(ActorModel actor)
		{
			AllActors.Add(actor);
			switch (actor.Faction)
			{
			case Faction.Survivor:
				Survivors.Add(actor);
				Perceptors.Add(actor);
				break;
			case Faction.Walker:
				Walkers.Add(actor);
				if (WalkerLevels == null)
				{
					WalkerLevels = new List<int>();
				}
				WalkerLevels.Add(actor.Level);
				break;
			case Faction.Environmental:
				Environmentals.Add(actor);
				break;
			case Faction.Dormant:
				Dormants.Add(actor);
				break;
			case Faction.Raider:
				Raiders.Add(actor);
				if (RaiderLevels == null)
				{
					RaiderLevels = new List<int>();
				}
				RaiderLevels.Add(actor.Level);
				break;
			case Faction.Civilian:
				Civilians.Add(actor);
				break;
			case Faction.Lure:
				Lures.Add(actor);
				break;
			}
			if (actor.Faction == Faction.Survivor)
			{
				UpdateAllActorsVisibility();
				UpdateObjectsVisibility();
			}
			else
			{
				UpdateActorVisibility(actor);
			}
			actor.Changed += OnActorChanged;
			actor.Changed += OnReceivedChargePoint;
			NotifyChange("actorCreated", actor);
		}

		public void RegisterSurvivorAtIndex(ActorModel actor, int index)
		{
			if (actor.Faction == Faction.Survivor && index >= 0)
			{
				AllActors.Add(actor);
				Survivors.Insert(index, actor);
				Perceptors.Add(actor);
				UpdateAllActorsVisibility();
				UpdateObjectsVisibility();
				actor.Changed += OnActorChanged;
				actor.Changed += OnReceivedChargePoint;
				NotifyChange("actorCreated", actor);
			}
		}

		public bool RemoveActor(ActorModel actor, bool updateVisibility = true)
		{
			NotifyChange("actorRemoved", actor);
			return UnregisterActor(actor, updateVisibility);
		}

		public bool UnregisterActor(ActorModel actor, bool updateVisibility = true)
		{
			bool flag = false;
			AllActors.Remove(actor);
			switch (actor.Faction)
			{
			case Faction.Survivor:
				flag = Survivors.Contains(actor);
				Survivors.Remove(actor);
				break;
			case Faction.Walker:
				flag = Walkers.Contains(actor);
				Walkers.Remove(actor);
				break;
			case Faction.Environmental:
				flag = Environmentals.Contains(actor);
				Environmentals.Remove(actor);
				break;
			case Faction.Dormant:
				flag = Dormants.Contains(actor);
				Dormants.Remove(actor);
				break;
			case Faction.Raider:
				flag = Raiders.Contains(actor);
				Raiders.Remove(actor);
				IsLeaderRemovedFromCombat(actor);
				break;
			case Faction.Civilian:
				flag = Civilians.Contains(actor);
				Civilians.Remove(actor);
				break;
			case Faction.Lure:
				flag = Lures.Contains(actor);
				Lures.Remove(actor);
				break;
			}
			if (actor.Faction == Faction.Survivor && updateVisibility)
			{
				UpdateAllActorsVisibility();
				UpdateObjectsVisibility();
			}
			if (flag)
			{
				OnActorRemoved(actor);
			}
			actor.Changed -= OnActorChanged;
			actor.Changed -= OnReceivedChargePoint;
			if (!flag)
			{
				bool flag2 = false;
				foreach (ActorModel item in Survivors.ToList())
				{
					if (item == actor)
					{
						Survivors.Remove(item);
						flag2 = true;
					}
				}
				foreach (ActorModel item2 in Walkers.ToList())
				{
					if (item2 == actor)
					{
						Walkers.Remove(item2);
						flag2 = true;
					}
				}
				foreach (ActorModel item3 in Dormants.ToList())
				{
					if (item3 == actor)
					{
						Dormants.Remove(item3);
						flag2 = true;
					}
				}
				foreach (ActorModel item4 in Raiders.ToList())
				{
					if (item4 == actor)
					{
						Raiders.Remove(item4);
						IsLeaderRemovedFromCombat(item4);
						flag2 = true;
					}
				}
				foreach (ActorModel item5 in Civilians.ToList())
				{
					if (item5 == actor)
					{
						Civilians.Remove(item5);
						flag2 = true;
					}
				}
				foreach (ActorModel item6 in Lures.ToList())
				{
					if (item6 == actor)
					{
						Lures.Remove(item6);
						flag2 = true;
					}
				}
				flag = flag2;
			}
			return flag;
		}

		private void OnActorRemoved(ActorModel actor)
		{
			foreach (ActorModel allActor in GetAllActors())
			{
				if (allActor.ExclusiveTimedEffect != null && allActor.ExclusiveTimedEffect.Target == actor)
				{
					allActor.FinishTimedEffect(interrupted: true);
				}
				if (allActor.AIDataModel != null)
				{
					allActor.AIDataModel.RemoveReferences(actor);
				}
			}
			if (actor == TurnManager.ActiveActor)
			{
				TurnManager.ActiveActor = null;
			}
			UpdateOccupiers();
		}

		private void OnReceivedChargePoint(ModelObject model, string changed, object args)
		{
			if (!(changed == "ActorReceivedChargePoint") || !(model is ActorModel actorModel))
			{
				return;
			}
			int num = (int)args;
			if (num == 0 || !actorModel.HasTraitsThatContains("Equipment_Active_ChargeLoad"))
			{
				return;
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("EquipmentActiveChargeLoadBumpPercent", ref value, actorModel);
			FixedPoint value2 = 0.0;
			if (value != 0.0)
			{
				base.manager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value2, actorModel);
			}
			if (base.manager.Player.RollDice(RollDiceType.ChargeLoad, value, value2) != PlayerRandomChanceResult.Failed)
			{
				FixedPoint value3 = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("EquipmentActiveChargeLoadBumpMaxFloor", ref value3, actorModel);
				if (!(actorModel.ChargeLoadFloor >= value3))
				{
					actorModel.ChargeLoadFloor = FixedPoint.Min(actorModel.ChargeLoadFloor + num, value3);
				}
			}
		}

		private void OnActorChanged(ModelObject model, string changed, object args)
		{
			int num = int.MaxValue;
			if (!IsEndlessBattleMission)
			{
				num = ((base.gameEconomyData != null && base.gameEconomyData.ConfigData != null) ? base.gameEconomyData.ConfigData.MissionMaxEnemiesKillGivingXP : 0);
			}
			if (!(model is ActorModel actorModel))
			{
				return;
			}
			SurvivorModel survivorModel = args as SurvivorModel;
			if (changed == "actorKilledEvent" && actorModel.IsDead)
			{
				NotifyChange("actorKilled", actorModel);
				ClearGuardianVowBindingsByActor(actorModel);
				if (actorModel.HasAnyLevelTrait("LeaderBuffMarkEnemy"))
				{
					RemoveDeBuffMarksFromActors(actorModel);
				}
				UpdateSurvivalGameList();
				DoDeadlyFocusKilledEvent(actorModel, survivorModel);
				DoCitadelKilledEvent(actorModel, survivorModel);
				int[] array = new int[2];
				int num2 = 0;
				if (actorModel.Faction == Faction.Walker && !actorModel.Definition.IsEnvironmental)
				{
					MissionStatistics.AddWalkersKilled(actorModel.Level, actorModel.ActorDefinitionID);
					if (survivorModel != null && survivorModel.Faction == Faction.Survivor)
					{
						survivorModel.Statistics.AddWalkersKilled();
						int num3 = MissionStatistics.WalkersKilled + MissionStatistics.RaidersKilled;
						if (base.manager.Player.Tutorial.HasCompletedPart("InitialCombat"))
						{
							bool shouldCap = num3 > num && num > 0;
							array = actorModel.GetSPGain(survivorModel, shouldCap);
							num2 = actorModel.GetSuppliesGain(survivorModel, array[0] + array[1]);
						}
					}
					if (IsEndlessBattleMission && !actorModel.KilledByLevelDifference)
					{
						WalkerModel item = (WalkerModel)actorModel;
						EndlessModeCombatModel.KilledWalkersInSurvivorTurn.Add(item);
						EndlessModeCombatModel.KilledEnemyInTurn = true;
					}
				}
				else if (actorModel.Faction == Faction.Raider)
				{
					MissionStatistics.AddRaidersKilled();
					if (survivorModel != null && survivorModel.Faction == Faction.Survivor)
					{
						survivorModel.Statistics.AddRaidersKilled();
						bool shouldCap2 = MissionStatistics.WalkersKilled + MissionStatistics.RaidersKilled > num && num > 0;
						array = actorModel.GetSPGain(survivorModel, shouldCap2);
						num2 = actorModel.GetSuppliesGain(survivorModel, array[0] + array[1]);
					}
				}
				if (survivorModel != null && survivorModel.Faction == Faction.Survivor && survivorModel.UserCanControl && !actorModel.Definition.IsEnvironmental)
				{
					SurvivorModel survivorModel2 = survivorModel;
					QuestVariables questVariables = base.manager.Player.DailyQuestManager.StartAction("Kill");
					AbilityModel abilityUnderApplication = base.manager.Player.AbilityManager.AbilityUnderApplication;
					if (abilityUnderApplication != null)
					{
						questVariables.AbilityType = abilityUnderApplication.Definition.Identifier;
					}
					questVariables.TargetType = actorModel.Faction.ToString();
					questVariables.TargetSpecificType = actorModel.ActorDefinitionID;
					questVariables.SurvivorClass.Clear();
					questVariables.Hero.Clear();
					questVariables.SurvivorClass.Add(survivorModel2.SurvivorClass.ToString());
					if (survivorModel2.IsHero || !string.IsNullOrEmpty(survivorModel2.Definition.AltOf))
					{
						questVariables.Hero.Add(survivorModel2.Definition.GetNonAlternativeHeroDefinition());
					}
					base.manager.Player.DailyQuestManager.CommitAction();
				}
				if (base.manager.Player.Tutorial.HasCompletedPart("InitialCombat") && !actorModel.Definition.IsEnvironmental)
				{
					if (array[0] != 0)
					{
						CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints);
						currency.Add(array[0] + array[1]);
						MissionStatistics.AddCollectedSurvivalPoints((int)(array[0] * currency.AddMultiplier));
						MissionStatistics.AddBonusSP((int)(array[1] * currency.AddMultiplier));
						MissionStatistics.AddActualSurvivalPointsAdded(currency.LastAdded);
					}
					if (num2 != 0)
					{
						CurrencyModel currency2 = base.manager.Player.GetCurrency(CurrencyType.Supplies);
						currency2.Add(num2);
						MissionStatistics.AddCollectedSupplies((int)(num2 * currency2.AddMultiplier));
						MissionStatistics.AddActualSuppliesAdded(currency2.LastAdded);
					}
				}
				bool flag = actorModel.MissionFailCondition == MissionFailCondition.FailOnDeath || actorModel.MissionFailCondition == MissionFailCondition.FailOnStruggle;
				int num4;
				if (actorModel.Faction == Faction.Raider || actorModel.Faction == Faction.Walker || actorModel.Faction == Faction.Dormant)
				{
					ActorModel obj = args as ActorModel;
					num4 = ((obj != null && obj.Faction == Faction.Survivor) ? 1 : 0);
				}
				else
				{
					num4 = 0;
				}
				bool flag2 = (byte)num4 != 0;
				if (!(actorModel is TankActorModel) && actorModel.Faction != Faction.Walker && actorModel.Faction != Faction.Environmental && actorModel.Faction != Faction.Dormant && actorModel.Faction != Faction.Lure && !HasPvPRules)
				{
					bool flag3 = false;
					if (IsGuildBattleMission && base.manager.Player.GvGSeasonModelPlayer != null)
					{
						flag3 = base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMission.IsPvPCombat;
					}
					bool flag4 = false;
					if (IsWorldBossMission)
					{
						WorldBossModelManager worldBossModelManager = base.manager.Player.WorldBossModelManager;
						if (worldBossModelManager != null && worldBossModelManager.IsAttackTargetActive)
						{
							flag4 = !worldBossModelManager.AttackTarget.IsPVECapturePoint;
						}
					}
					bool flag5 = false;
					bool flag6 = IsSurvivalMission || IsEndlessBattleMission || flag3 || IsWorldBossMission;
					if (flag6)
					{
						foreach (SurvivorModel item2 in MissionRoster)
						{
							if (!item2.IsDead)
							{
								flag5 = true;
								break;
							}
						}
					}
					if (IsDeadly || actorModel.Faction != Faction.Survivor || (flag6 && flag5))
					{
						if (flag3 && actorModel.Faction != Faction.Survivor)
						{
							SetGuildBattlePVPSurvivorKilled(actorModel);
						}
						if (flag4 && actorModel.Faction != Faction.Survivor)
						{
							SetWorldBossDefenderKilled(actorModel);
						}
						ChangeActorFaction(actorModel, Faction.Lure);
						actorModel.StartTimedEffect(new TimedEffect(TimedEffectType.Lure, 2, 0, actorModel.Faction));
						CollectWalkersToEatLure(actorModel);
					}
					else if (actorModel.Faction == Faction.Survivor)
					{
						flag = true;
					}
				}
				else
				{
					UnregisterActor(actorModel);
					if (HasPvPRules && actorModel.Faction == Faction.Raider)
					{
						SetPvPDefenderKilled(actorModel);
					}
				}
				if (flag2)
				{
					EarnBattleCurrencyFromKill(actorModel);
				}
				if (flag)
				{
					string text = ((actorModel.MissionFailCondition == MissionFailCondition.FailOnDeath) ? "FailOnDeath" : ((actorModel.MissionFailCondition == MissionFailCondition.FailOnStruggle) ? "FailOnStruggleDeath" : "SurvivorDeath"));
					OnMissionComplete(ECombatResult.Failed, casualtiesResolved: false, "OnActorChanged_" + text);
				}
			}
			else if (changed == "actorTimedEffectStart")
			{
				if (args is TimedEffect { Type: TimedEffectType.Struggle })
				{
					if (IsEndlessBattleMission && actorModel.Faction == Faction.Survivor)
					{
						((SurvivorModel)actorModel).SurvivedUntilWave = EndlessModeCombatModel.GetCurrentOverAllWaveIndex;
					}
					if (actorModel.MissionFailCondition == MissionFailCondition.FailOnStruggle)
					{
						OnMissionComplete(ECombatResult.Failed, casualtiesResolved: false, "OnActorChanged_FailOnStruggle");
					}
				}
			}
			else if (changed == "actorExploded" && (actorModel.Faction == Faction.Walker || actorModel.Faction == Faction.Dormant || actorModel.Faction == Faction.Raider))
			{
				ActorModel lastHitAttacker = actorModel.LastHitAttacker;
				if (lastHitAttacker != null && lastHitAttacker.Faction == Faction.Survivor)
				{
					EarnBattleCurrencyFromKill(actorModel);
				}
			}
		}

		private void OnAfterAbilityEffectApplied()
		{
			if (IsEndlessBattleMission && EndlessModeCombatModel != null && EndlessModeCombatModel.KilledWalkersInSurvivorTurn.Count > 0)
			{
				EndlessModeCombatModel.HandleKillScoreIncrease();
				NotifyChange("EndlessModeScoreChanged");
			}
		}

		private void OnTurnManagerActorChanged(ActorModel actor)
		{
			NotifyChange("turnManagerActorChangedEvent");
		}

		public FactionAIController GetFactionAIController(Faction faction)
		{
			for (int i = 0; i < FactionAIControllers.Count; i++)
			{
				FactionAIController factionAIController = FactionAIControllers[i];
				if (factionAIController.Faction == faction)
				{
					return factionAIController;
				}
			}
			return null;
		}

		public List<ActorModel> GetFactionActors(Faction faction, bool isBoss = false)
		{
			List<ActorModel> list = null;
			list = faction switch
			{
				Faction.Any => GetAllActors(),
				Faction.Survivor => Survivors.Models,
				Faction.Walker => Walkers.Models,
				Faction.Environmental => (Environmentals != null) ? Environmentals.Models : new List<ActorModel>(),
				Faction.Dormant => Dormants.Models,
				Faction.Raider => Raiders.Models,
				Faction.Civilian => Civilians.Models,
				Faction.Lure => Lures.Models,
				Faction.Tutorial => new List<ActorModel>(),
				_ => new List<ActorModel>(),
			};
			if (!isBoss)
			{
				return list;
			}
			List<ActorModel> list2 = new List<ActorModel>();
			foreach (ActorModel item in list)
			{
				if (item.IsBoss)
				{
					list2.Add(item);
				}
			}
			return list2;
		}

		public List<ActorModel> GetEnemyFactionsActors(Faction faction)
		{
			List<ActorModel> list = new List<ActorModel>();
			switch (faction)
			{
			case Faction.Survivor:
				list.AddRange(Walkers);
				list.AddRange(Dormants);
				list.AddRange(Raiders);
				list.AddRange(Environmentals);
				break;
			case Faction.Walker:
				list.AddRange(Survivors);
				list.AddRange(Raiders);
				list.AddRange(Civilians);
				list.AddRange(Lures);
				break;
			case Faction.Raider:
				list.AddRange(Survivors);
				list.AddRange(Walkers);
				list.AddRange(Dormants);
				list.AddRange(Civilians);
				list.AddRange(Environmentals);
				break;
			case Faction.Dormant:
				list.AddRange(Survivors);
				list.AddRange(Raiders);
				list.AddRange(Civilians);
				list.AddRange(Lures);
				break;
			case Faction.Civilian:
				list.AddRange(Walkers);
				list.AddRange(Dormants);
				list.AddRange(Raiders);
				break;
			case Faction.Lure:
				list.AddRange(Walkers);
				list.AddRange(Dormants);
				break;
			}
			return list;
		}

		public List<ActorModel> GetAllActors()
		{
			int num = ((Environmentals != null) ? Environmentals.Count : 0);
			int num2 = Survivors.Count + Walkers.Count + Dormants.Count + Raiders.Count + Civilians.Count + Lures.Count + num;
			if (AllActors.Count != num2 && !IsEndlessBattleMission)
			{
				base.Manager.Debug.LogWarning("GetAllActors: Actor count does not match! " + CurrentMissionTextID + " AllActors " + AllActors.Count + " actorSum " + num2);
			}
			return AllActors.Models;
		}

		public List<ActorModel> GetActorsInDiamond(GridCoordinate center, int range, Faction targetFaction = Faction.Any)
		{
			List<ActorModel> list = ((targetFaction == Faction.Any) ? GetAllActors() : GetFactionActors(targetFaction));
			List<ActorModel> list2 = new List<ActorModel>();
			for (int i = 0; i < list.Count; i++)
			{
				ActorModel actorModel = list[i];
				GridCoordinate closestOccupiedCell = actorModel.GetClosestOccupiedCell(center);
				if (Math.Abs(closestOccupiedCell.X - center.X) + Math.Abs(closestOccupiedCell.Y - center.Y) <= range)
				{
					list2.Add(actorModel);
				}
			}
			return list2;
		}

		public List<GridCoordinate> GetDiamondCoordinates(GridCoordinate center, int range)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			for (int i = -range; i <= range; i++)
			{
				int num = range - Math.Abs(i);
				for (int j = -num; j <= num; j++)
				{
					GridCoordinate gridCoordinate = new GridCoordinate(center.X + i, center.Y + j);
					if (Grid.IsCoordinateValid(gridCoordinate) && !IsBlocked(gridCoordinate))
					{
						list.Add(gridCoordinate);
					}
				}
			}
			return list;
		}

		public void RefreshDashTraitFlag()
		{
			if (!(base.manager.Player.GetAttackTargetMissionModel() is MapMissionModel { IsInWeeklySurvival: not false }))
			{
				return;
			}
			for (int i = 0; i < AllActors.Count; i++)
			{
				ActorModel actorModel = AllActors[i];
				if (actorModel.Faction == Faction.Survivor)
				{
					actorModel.dashTraitValidFlag = false;
				}
			}
		}

		public List<InteractiveObjectModel> GetInteractiveObjectsInRange(GridCoordinate location, int range, bool diagonal = true)
		{
			List<TWDModelObject> models = GetModels<InteractiveObjectModel>();
			List<InteractiveObjectModel> list = new List<InteractiveObjectModel>(8);
			FixedPoint fixedPoint = ((float)range + (diagonal ? 0.42f : 0f)) * Grid.CellSize.X;
			fixedPoint *= fixedPoint;
			foreach (InteractiveObjectModel item in models)
			{
				if (!item.Completed && !item.Disabled && !item.HasInteractionStarted && item.Placement == Placement.Cell)
				{
					FixedVec3 position = Grid.GetPosition(item.Location.Coordinate);
					FixedVec3 position2 = Grid.GetPosition(location);
					if ((position - position2).SqrMagnitude <= fixedPoint)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public List<ActorModel> GetActorsInRange(GridCoordinate location, int range, bool diagonal = true, Faction targetFaction = Faction.Any, bool abartFromMe = false)
		{
			List<ActorModel> list = null;
			list = ((targetFaction != Faction.Any) ? GetFactionActors(targetFaction) : GetAllActors());
			List<ActorModel> list2 = new List<ActorModel>(8);
			FixedPoint fixedPoint = ((float)range + (diagonal ? 0.42f : 0f)) * Grid.CellSize.X;
			fixedPoint *= fixedPoint;
			for (int i = 0; i < list.Count; i++)
			{
				ActorModel actorModel = list[i];
				GridCoordinate closestOccupiedCell = actorModel.GetClosestOccupiedCell(location);
				FixedVec3 position = Grid.GetPosition(closestOccupiedCell);
				FixedVec3 position2 = Grid.GetPosition(location);
				if ((position - position2).SqrMagnitude <= fixedPoint && !(closestOccupiedCell.Equals(location) && abartFromMe))
				{
					list2.Add(actorModel);
				}
			}
			return list2;
		}

		public List<ActorModel> GetActorsInLine(GridCoordinate start, GridCoordinate end, ActorModel actorModel = null)
		{
			List<ActorModel> list = new List<ActorModel>();
			FixedPoint lineWidthVisitHalfSqr;
			FixedPoint x0;
			FixedPoint y0;
			FixedPoint x1;
			FixedPoint y1;
			FixedPoint dirX;
			FixedPoint dirY;
			FixedPoint lineLenSqr;
			int minX;
			int maxX;
			int minY;
			int maxY;
			if (start != end)
			{
				FixedPoint value = 1.0;
				AbilityManager.VisitParameter("AbilityModifierIncreaseBulletWidth", ref value, actorModel);
				if (actorModel != null && actorModel.FocusModeState && actorModel.SelectedAbility != null && actorModel.SelectedAbility.IsChargeAttack)
				{
					AbilityManager.VisitParameter("AbilityModifierFocusModeAttackWidth", ref value, actorModel);
				}
				FixedPoint fixedPoint = value * 0.5;
				lineWidthVisitHalfSqr = fixedPoint * fixedPoint;
				x0 = start.X;
				y0 = start.Y;
				x1 = end.X;
				y1 = end.Y;
				dirX = x1 - x0;
				dirY = y1 - y0;
				lineLenSqr = dirX * dirX + dirY * dirY;
				int num = (int)FixedPoint.Ceiling(fixedPoint) + 1;
				minX = Math.Min(start.X, end.X) - num;
				maxX = Math.Max(start.X, end.X) + num;
				minY = Math.Min(start.Y, end.Y) - num;
				maxY = Math.Max(start.Y, end.Y) + num;
				List<ActorModel> allActors = GetAllActors();
				for (int i = 0; i < allActors.Count; i++)
				{
					ActorModel actorModel2 = allActors[i];
					bool flag;
					if (actorModel2.IsMultiCell)
					{
						flag = false;
						List<GridCoordinate> occupiedCells = actorModel2.GetOccupiedCells();
						for (int j = 0; j < occupiedCells.Count; j++)
						{
							if (CoordOnLine(occupiedCells[j]))
							{
								flag = true;
								break;
							}
						}
					}
					else
					{
						flag = CoordOnLine(actorModel2.GridCoordinate);
					}
					if (flag)
					{
						list.Add(actorModel2);
					}
				}
			}
			else
			{
				ActorModel occupier = GetOccupier(start);
				if (occupier != null)
				{
					list.Add(occupier);
				}
			}
			return list;
			bool CoordOnLine(GridCoordinate coord)
			{
				if (coord.X < minX || coord.X > maxX || coord.Y < minY || coord.Y > maxY)
				{
					return false;
				}
				FixedPoint fixedPoint2 = coord.X - x0;
				FixedPoint fixedPoint3 = coord.Y - y0;
				FixedPoint fixedPoint4 = dirX * fixedPoint2 + dirY * fixedPoint3;
				FixedPoint fixedPoint5;
				if (fixedPoint4 <= 0.0)
				{
					fixedPoint5 = fixedPoint2 * fixedPoint2 + fixedPoint3 * fixedPoint3;
				}
				else if (fixedPoint4 >= lineLenSqr)
				{
					FixedPoint fixedPoint6 = coord.X - x1;
					FixedPoint fixedPoint7 = coord.Y - y1;
					fixedPoint5 = fixedPoint6 * fixedPoint6 + fixedPoint7 * fixedPoint7;
				}
				else
				{
					fixedPoint5 = fixedPoint2 * fixedPoint2 + fixedPoint3 * fixedPoint3 - fixedPoint4 * fixedPoint4 / lineLenSqr;
					if (fixedPoint5 < 0.0)
					{
						fixedPoint5 = 0.0;
					}
				}
				return fixedPoint5 <= lineWidthVisitHalfSqr;
			}
		}

		public ActorModel GetActorOfFactionAt(Faction faction, GridCoordinate position)
		{
			foreach (ActorModel factionActor in GetFactionActors(faction))
			{
				if (factionActor.GridCoordinate == position)
				{
					return factionActor;
				}
			}
			return null;
		}

		public List<ActorModel> GetActorsWithTag(int tag, Faction faction = Faction.Any, bool isBoss = false)
		{
			List<ActorModel> list = new List<ActorModel>();
			foreach (ActorModel allActor in GetAllActors())
			{
				if (allActor.ActorTag == tag && (allActor.Faction == faction || faction == Faction.Any) && (!isBoss || allActor.IsBoss))
				{
					list.Add(allActor);
				}
			}
			return list;
		}

		public string GetFactionName(Faction faction)
		{
			if (FactionNames != null)
			{
				for (int i = 0; i < FactionNames.Length; i++)
				{
					if (FactionNames[i].Faction == faction && !string.IsNullOrEmpty(FactionNames[i].Name))
					{
						return FactionNames[i].Name;
					}
				}
			}
			return Enum.GetName(typeof(Faction), faction);
		}

		public List<GridCoordinate> GetAbilityTargetsInRange(AbilityModel ability, ActorModel sourceActor, GridCoordinate sourceCell, bool acceptInteractiveObjects = false)
		{
			FixedPoint range = ability.Definition.AbilityRange;
			if (!ability.IsConsumableAbility)
			{
				CombatHelpers.CalculateRangeExtension(ref range, sourceActor, AbilityManager);
			}
			return GetAbilityTargetsInRange(ability, sourceActor, sourceCell, range, acceptInteractiveObjects);
		}

		public List<GridCoordinate> GetAbilityTargetsInRange(AbilityModel ability, ActorModel sourceActor, GridCoordinate sourceCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			bool abilityTargetDiagonal = ability.Definition.AbilityTargetDiagonal;
			bool hasFriendlyFire = ability.Definition.HasFriendlyFire;
			if (acceptInteractiveObjects)
			{
				List<InteractiveObjectModel> interactiveObjectsInRange = GetInteractiveObjectsInRange(sourceCell, (int)preComputedRange, abilityTargetDiagonal);
				for (int i = 0; i < interactiveObjectsInRange.Count; i++)
				{
					InteractiveObjectModel interactiveObjectModel = interactiveObjectsInRange[i];
					if (interactiveObjectModel.InteractBy == InteractBy.Shoot)
					{
						GridCoordinate coordinate = interactiveObjectModel.Location.Coordinate;
						bool flag = false;
						if (ability.Definition.RequiresLineOfSight && !IsGridCellVisible(sourceCell, coordinate))
						{
							flag = true;
						}
						else if (ability.Definition.RequiresLineOfMovement && IsGridLineMovementBlocked(sourceCell, coordinate))
						{
							flag = true;
						}
						if (!flag)
						{
							list.Add(coordinate);
						}
					}
				}
			}
			FixedPoint fixedPoint = (preComputedRange + (abilityTargetDiagonal ? 0.42f : 0f)) * Grid.CellSize.X;
			fixedPoint *= fixedPoint;
			FixedVec3 position = Grid.GetPosition(sourceCell);
			List<ActorModel> allActors = GetAllActors();
			for (int j = 0; j < allActors.Count; j++)
			{
				ActorModel actorModel = allActors[j];
				if (!ability.IsTargetValid(sourceActor, actorModel, hasFriendlyFire))
				{
					continue;
				}
				GridCoordinate closestOccupiedCell = actorModel.GetClosestOccupiedCell(sourceCell);
				FixedVec3 position2 = Grid.GetPosition(closestOccupiedCell);
				if ((position - position2).SqrMagnitude < fixedPoint)
				{
					bool flag2 = false;
					if (ability.Definition.RequiresLineOfSight && !IsGridCellVisible(sourceCell, closestOccupiedCell))
					{
						flag2 = true;
					}
					else if (ability.Definition.RequiresLineOfMovement && IsGridLineMovementBlocked(sourceCell, closestOccupiedCell))
					{
						flag2 = true;
					}
					if (!flag2)
					{
						list.Add(closestOccupiedCell);
					}
				}
			}
			return list;
		}

		public AbilityResult IsAbilityTargetValid(AbilityModel ability, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			FixedPoint range = ability.Definition.AbilityRange;
			if (!ability.IsConsumableAbility)
			{
				CombatHelpers.CalculateRangeExtension(ref range, sourceActor, AbilityManager);
			}
			return IsAbilityTargetValidInternal(ability, sourceActor, sourceCell, targetCell, range, acceptInteractiveObjects);
		}

		public AbilityResult IsAbilityTargetValid(AbilityModel ability, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			return IsAbilityTargetValidInternal(ability, sourceActor, sourceCell, targetCell, preComputedRange, acceptInteractiveObjects);
		}

		private AbilityResult IsAbilityTargetValidInternal(AbilityModel ability, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint range, bool acceptInteractiveObjects)
		{
			bool abilityTargetDiagonal = ability.Definition.AbilityTargetDiagonal;
			if (acceptInteractiveObjects)
			{
				InteractiveObjectModel interactiveObject = GetInteractiveObject(targetCell);
				if (interactiveObject != null && interactiveObject.InteractBy == InteractBy.Shoot)
				{
					GridCoordinate coordinate = interactiveObject.Location.Coordinate;
					if (ability.Definition.RequiresLineOfSight && !IsGridCellVisible(sourceCell, coordinate))
					{
						return AbilityResult.FailedVisibilityBlocked;
					}
					if (ability.Definition.RequiresLineOfMovement && IsGridLineMovementBlocked(sourceCell, coordinate))
					{
						return AbilityResult.FailedMovementBlocked;
					}
					return AbilityResult.Success;
				}
			}
			if (ability.Definition.RequiresLineOfSight && !IsGridCellVisible(sourceCell, targetCell))
			{
				return AbilityResult.FailedVisibilityBlocked;
			}
			if (ability.Definition.RequiresLineOfMovement && IsGridLineMovementBlocked(sourceCell, targetCell))
			{
				return AbilityResult.FailedMovementBlocked;
			}
			FixedPoint fixedPoint = (range + (abilityTargetDiagonal ? 0.42f : 0f)) * Grid.CellSize.X;
			FixedPoint fixedPoint2 = fixedPoint * fixedPoint;
			FixedVec3 position = Grid.GetPosition(sourceCell);
			FixedVec3 position2 = Grid.GetPosition(targetCell);
			if ((position - position2).SqrMagnitude >= fixedPoint2)
			{
				return AbilityResult.FailedOutOfRange;
			}
			ActorModel occupier = GetOccupier(targetCell);
			if (occupier != null)
			{
				if (ability.IsTargetValid(sourceActor, occupier))
				{
					return AbilityResult.Success;
				}
			}
			else if (ability.Definition.TriggerType == AbilityTriggerType.GridOrTarget)
			{
				if (AbilityManager.HasAnyValidActorToBeTargetted(ability, sourceActor, sourceCell, targetCell, range, ability.Definition.RequiresLineOfSight, ability.Definition.RequiresLineOfMovement))
				{
					return AbilityResult.Success;
				}
				return AbilityResult.FailedNoValidTarget;
			}
			return AbilityResult.FailedNoValidTarget;
		}

		public bool IsActorWithinMoveRangeForAbility(AbilityModel ability, ActorModel sourceActor, GridCoordinate targetPosition)
		{
			if (ability.Definition.IsPerformedAfterPlayerMove)
			{
				return FindPath(sourceActor, sourceActor.GridCoordinate, targetPosition).Count <= sourceActor.MoveRange;
			}
			return true;
		}

		public GridCoordinate GetClosestEmptyGridToThrowLocation(GridCoordinate sourceLocation, GridCoordinate targetLocation, bool requiresLineOfSight, float radius = -1f)
		{
			GridCoordinate result = GridCoordinate.Invalid;
			if (sourceLocation != targetLocation)
			{
				FixedVec3 position = Grid.GetPosition(sourceLocation);
				FixedVec3 fixedVec = Grid.GetPosition(targetLocation) - position;
				FixedPoint magnitude = fixedVec.Magnitude;
				FixedVec3 fixedVec2 = fixedVec / magnitude;
				FixedPoint fixedPoint = ((radius > -1f) ? FixedPoint.Min(magnitude, radius) : magnitude);
				FixedVec3 position2 = position + fixedVec2 * fixedPoint;
				List<GridCoordinate> allCellsInterceptedByLineSegment = GetAllCellsInterceptedByLineSegment(sourceLocation, Grid.GetCoordinate(position2));
				FixedPoint fixedPoint2 = FixedPoint.MaxValue;
				foreach (GridCoordinate item in allCellsInterceptedByLineSegment)
				{
					bool flag = GetOccupier(item) != null;
					bool flag2 = IsGridCellVisibleByAnySurvivor(targetLocation);
					bool flag3 = !requiresLineOfSight || IsGridCellVisible(sourceLocation, targetLocation);
					if (!IsBlocked(item) && !flag && flag2 && flag3)
					{
						FixedPoint fixedPoint3 = targetLocation.DistanceTo(item);
						if (fixedPoint3 < fixedPoint2)
						{
							fixedPoint2 = fixedPoint3;
							result = item;
						}
					}
				}
			}
			return result;
		}

		public List<GridCoordinate> GetAllCellsInterceptedByLineSegment(GridCoordinate lineOrigin, GridCoordinate lineEnd)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			if (lineOrigin != lineEnd)
			{
				List<GridCoordinate> list2 = new List<GridCoordinate>();
				int num = Math.Min(lineOrigin.X, lineEnd.X);
				int num2 = Math.Max(lineOrigin.X, lineEnd.X);
				int num3 = Math.Min(lineOrigin.Y, lineEnd.Y);
				int num4 = Math.Max(lineOrigin.Y, lineEnd.Y);
				for (int i = num; i <= num2; i++)
				{
					for (int j = num3; j <= num4; j++)
					{
						list2.Add(new GridCoordinate(i, j));
					}
				}
				FixedVec3 position = Grid.GetPosition(lineOrigin);
				FixedVec3 rayDir = Grid.GetPosition(lineEnd) - position;
				foreach (GridCoordinate item in list2)
				{
					FixedVec3 position2 = Grid.GetPosition(item);
					FixedPoint fixedPoint = Grid.GetCellSize().X * 0.5;
					FixedPoint fixedPoint2 = Grid.GetCellSize().Y * 0.5;
					FixedVec3 fixedVec = new FixedVec3(-fixedPoint, 0.0, -fixedPoint2);
					FixedVec3 fixedVec2 = new FixedVec3(fixedPoint, 0.0, fixedPoint2);
					FixedPoint tmin = 0.0;
					FixedPoint tmax = 0.0;
					if (GeometryMath.IntersectAABB(position, rayDir, position2 + fixedVec, position2 + fixedVec2, out tmin, out tmax))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public List<ActivatedObjectModel> GetNoiseActivatedObjectsInRange(GridCoordinate location, double range)
		{
			List<ActivatedObjectModel> list = new List<ActivatedObjectModel>();
			GridCoordinate other = new GridCoordinate(location.X + (int)range, location.Y + (int)range);
			foreach (ActivatedObjectModel model in GetModels<ActivatedObjectModel>())
			{
				if (model.ActivationType == ActivatedObjectType.Threat && location.SquaredDistanceTo(model.Location.Coordinate) <= location.SquaredDistanceTo(other))
				{
					list.Add(model);
				}
			}
			return list;
		}

		public List<ActivatedObjectModel> GetNoiseActivatedObjects()
		{
			List<ActivatedObjectModel> list = new List<ActivatedObjectModel>();
			foreach (ActivatedObjectModel model in GetModels<ActivatedObjectModel>())
			{
				if (model.ActivationType == ActivatedObjectType.Threat)
				{
					list.Add(model);
				}
			}
			return list;
		}

		public bool CheckForEndMission(ref ECombatResult result)
		{
			if (MissionCompleted)
			{
				return false;
			}
			bool flag = false;
			OutOfTurns = AfterAlarmTurns > 0 && TurnTimerActivationTurn > 0 && TurnsToFlee == 0;
			int survivorsIncapacitated = 0;
			int survivorsInExits = 0;
			GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
			if (IsGuildBattleMission && GuildBattlePVPSurvivorsKilledIndices.Count >= 3)
			{
				flag = true;
				result = ECombatResult.Successful;
			}
			else if (HasPvPRules)
			{
				flag = survivorsIncapacitated == Survivors.Count || (IsPVPMission && OutOfTurns) || (IsPvpDefendersKilled && IsPvPFlagCollected && IsPvPLootCollected);
				result = GetPvpResult(survivorsIncapacitated, Survivors.Count);
			}
			else if (IsEndlessBattleMission)
			{
				EndlessModeManagerModel endlessModeManager = base.manager.Player.EndlessModeManager;
				if (endlessModeManager != null)
				{
					int num = 0;
					if (endlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert)
					{
						num = endlessModeManager.CurrentEndlessModeCalendarDefinition.MaxWalkerAmountExpert;
					}
					else if (endlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Normal)
					{
						num = endlessModeManager.CurrentEndlessModeCalendarDefinition.MaxWalkerAmount;
					}
					bool num2 = GetFactionActors(Faction.Walker).Count >= num;
					flag = num2 || survivorsIncapacitated == Survivors.Count;
					result = ((!flag) ? ECombatResult.Successful : ECombatResult.Failed);
					if (num2)
					{
						EndlessModeCombatModel.SetSurvivorsSurvivedWaveCount();
						EndlessModeCombatModel.DefeatedByOverrun = true;
					}
				}
			}
			else
			{
				flag = survivorsInExits == Survivors.Count || survivorsIncapacitated == Survivors.Count || OutOfTurns;
				result = ((survivorsInExits > 0) ? ECombatResult.Successful : ECombatResult.Failed);
			}
			return flag;
		}

		private string GetCurrentMissionFailureReason()
		{
			int num = 0;
			int num2 = 0;
			int num3 = ((Survivors != null) ? Survivors.Count : 0);
			for (int i = 0; i < num3; i++)
			{
				if (Survivors[i] is SurvivorModel survivorModel)
				{
					num2++;
					if (survivorModel.IsStruggling || !survivorModel.UserCanControl)
					{
						num++;
					}
				}
			}
			if (num2 > 0 && num >= num2)
			{
				return "AllSurvivorsIncapacitated";
			}
			if (OutOfTurns)
			{
				return "OutOfTurns";
			}
			if (HasPvPRules)
			{
				if (0 + (IsPvPFlagCollected ? 1 : 0) + (IsPvPLootCollected ? 1 : 0) + (IsPvpDefendersKilled ? 1 : 0) == 0)
				{
					return "NoPvpObjectivesCompleted";
				}
				return "PvpFailed";
			}
			if (!IsGuildBattleMission)
			{
				return "MissionFailedUnknown";
			}
			return "GuildBattleMissionFailedUnknown";
		}

		public void GetSurvivorStatus(out int survivorsIncapacitated, out int survivorsInExits)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < ((Survivors != null) ? Survivors.Count : 0); i++)
			{
				SurvivorModel survivorModel = (SurvivorModel)Survivors[i];
				if (survivorModel.IsStruggling || !survivorModel.UserCanControl)
				{
					num++;
				}
				else if (IsActorInValidExit(survivorModel))
				{
					num2++;
				}
			}
			survivorsIncapacitated = num;
			survivorsInExits = num2;
		}

		public ECombatResult GetPvpResult(int survivorsIncapacitated, int survivorsTotal)
		{
			int num = (IsPvPFlagCollected ? 1 : 0);
			num += (IsPvPLootCollected ? 1 : 0);
			num += (IsPvpDefendersKilled ? 1 : 0);
			if (survivorsIncapacitated != survivorsTotal)
			{
				switch (num)
				{
				case 0:
					break;
				case 1:
					return ECombatResult.Draw;
				default:
					return ECombatResult.Successful;
				}
			}
			return ECombatResult.Failed;
		}

		public bool IsActorInValidExit(ActorModel actor)
		{
			List<TWDModelObject> models = GetModels<CombatExitModel>();
			for (int i = 0; i < models.Count; i++)
			{
				CombatExitModel combatExitModel = (CombatExitModel)models[i];
				if (combatExitModel.Enabled && combatExitModel.IsActorInExit(actor))
				{
					return true;
				}
			}
			return false;
		}

		public bool PushActor(ActorModel actor, GridPath path)
		{
			if (actor != null && (actor.IsStruggling || actor.IsEatingLure || actor.IsInteractingWithObject))
			{
				actor.FinishTimedEffect(interrupted: true);
			}
			return MoveActor(actor, path);
		}

		public bool MoveActor(ActorModel actor, GridPath path)
		{
			if (actor is TankActorModel)
			{
				return false;
			}
			if (actor.IsValid() && path.IsValid)
			{
				DebugTWD.Log("MoveActor " + actor.Name, DebugType.Wars);

				if (path.Count > 2 && GetOccupier(path.End) != null)
				{
					path.RemoveLast();
					return MoveActor(actor, path);
				}
				GridCoordinate gridCoordinate = actor.GridCoordinate;
				actor.GridCoordinate = path.End;
				foreach (ActorModel enemyFactionsActor in GetEnemyFactionsActors(actor.Faction))
				{
					if (HasPvPRules && actor.Faction == Faction.Raider && enemyFactionsActor.IsWalker)
					{
						continue;
					}
					GridCoordinate other = new GridCoordinate(enemyFactionsActor.GridCoordinate.X + enemyFactionsActor.ActivationRange, enemyFactionsActor.GridCoordinate.Y);
					foreach (GridCoordinate item in path.Path)
					{
						int num = enemyFactionsActor.GridCoordinate.SquaredDistanceTo(item);
						int num2 = enemyFactionsActor.GridCoordinate.SquaredDistanceTo(other);
						if (num <= num2 && IsGridCellVisible(item, enemyFactionsActor.GridCoordinate))
						{
							enemyFactionsActor.AIController?.SeeEnemy(actor);
						}
					}
				}
				if (actor.Faction == Faction.Survivor)
				{
					UpdateAllActorsVisibility();
					UpdateObjectsVisibility();
				}
				else
				{
					UpdateActorVisibility(actor);
				}
				if (gridCoordinate != actor.GridCoordinate && actor.IsInFortifications)
				{
					actor.EndFortifications(interrupted: true);
				}
				return true;
			}
			base.Debug.Log("MoveActor failed -> actor.IsValid() == " + actor.IsValid() + " path.IsValid == " + path.IsValid);
			return false;
		}

		public bool AttackInteractiveObject(ActorModel actor, InteractiveObjectModel target)
		{
			if (actor.IsValid() && target.IsValid() && !target.Completed && !target.Disabled)
			{
				target.OnAttacked(actor);
				return true;
			}
			return false;
		}

		public static GridCoordinate GetInteractionDirectionNeighbor(GridCoordinate origin, CombatModel combat, ActorModel movingActor, Direction direction, bool edgeCheck)
		{
			if (direction != Direction.Any)
			{
				GridModel grid = combat.Grid;
				int index = (int)direction * 2;
				GridCoordinate coordinateNeighbor = grid.GetCoordinateNeighbor(origin, index);
				if (grid.IsCoordinateValid(coordinateNeighbor) && (combat.GetOccupier(coordinateNeighbor) == null || combat.GetOccupier(coordinateNeighbor) == movingActor) && (!edgeCheck || combat.CanTraverse(movingActor, coordinateNeighbor, origin)))
				{
					return coordinateNeighbor;
				}
			}
			return GridCoordinate.Invalid;
		}

		public bool CanUseInteractiveObject(ActorModel actor, InteractiveObjectModel interactiveObject)
		{
			if (actor == null || interactiveObject == null)
			{
				return false;
			}
			if (actor.IsValid() && interactiveObject.IsValid() && interactiveObject.CanBeInteracted && interactiveObject.InteractBy == InteractBy.Use)
			{
				if (interactiveObject.Placement == Placement.Cell)
				{
					List<GridCoordinate> coordinates = interactiveObject.Location.Coordinates;
					for (int i = 0; i < coordinates.Count; i++)
					{
						GridCoordinate gridCoordinate = coordinates[i];
						GridCoordinate interactionDirectionNeighbor = GetInteractionDirectionNeighbor(gridCoordinate, this, actor, interactiveObject.InteractionDirection, interactiveObject.Placement == Placement.Cell);
						if (!interactionDirectionNeighbor.IsValid)
						{
							for (int j = 0; j < 8; j++)
							{
								GridCoordinate coordinateNeighbor = Grid.GetCoordinateNeighbor(gridCoordinate, j);
								if (Grid.IsCoordinateValid(coordinateNeighbor) && (GetOccupier(coordinateNeighbor) == null || GetOccupier(coordinateNeighbor) == actor) && (interactiveObject.Placement != Placement.Cell || CanTraverse(actor, coordinateNeighbor, gridCoordinate)) && coordinateNeighbor == actor.GridCoordinate)
								{
									return true;
								}
							}
						}
						if (interactionDirectionNeighbor == actor.GridCoordinate)
						{
							return true;
						}
					}
				}
				else
				{
					List<int> edges = interactiveObject.Location.Edges;
					for (int k = 0; k < edges.Count; k++)
					{
						int edgeId = edges[k];
						Grid.GetCoordinatesFromEdge(edgeId, out var a, out var b);
						if (a == actor.GridCoordinate || b == actor.GridCoordinate)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public MovableModel GetMovableForInteractiveObject(InteractiveObjectModel target)
		{
			if (target == null)
			{
				return null;
			}
			for (int i = 0; i < target.receivers.Count; i++)
			{
				if (target.receivers[i] is MovableModel result)
				{
					return result;
				}
			}
			return null;
		}

		public bool UseInteractiveObject(ActorModel actor, InteractiveObjectModel target)
		{
			if (actor.IsValid() && target.IsValid() && target.CanBeInteracted)
			{
				MovableModel movableForInteractiveObject = GetMovableForInteractiveObject(target);
				if (movableForInteractiveObject != null && !movableForInteractiveObject.CheckClearance())
				{
					return false;
				}
				target.Interactor = actor;
				actor.UseInteractiveObject(target);
				return true;
			}
			return false;
		}

		public bool StruggleActor(ActorModel attacker, ActorModel target)
		{
			if (target.IsSurvivalGameNoDead())
			{
				return false;
			}
			if (attacker.IsValid() && target.IsValid() && AreActorsAdjacent(attacker, target))
			{
				FixedPoint value = target.Definition.InitialStruggleTurns;
				AbilityManager.VisitParameter("AbilityModifierIncreaseStruggleTurns", ref value, target);
				AbilityManager.VisitParameter("AbilityModifierIncreaseStruggleTurns", ref value, attacker);
				value = Math.Max(1, (int)value);
				attacker.StartStruggle(target, (int)value);
				return true;
			}
			return false;
		}

		public bool BleedOutActor(ActorModel attacker, ActorModel target, bool giveFullHealth = true)
		{
			if ((attacker == null || (attacker != null && attacker.IsValid())) && target.IsValid())
			{
				target.StartBleedOut(attacker, giveFullHealth);
				return true;
			}
			return false;
		}

		public bool BurningOutActor(ActorModel target, bool onRedHealthBar, int burnTurns = 0)
		{
			if (target.IsValid())
			{
				target.StartBurningOut(onRedHealthBar, burnTurns);
				return true;
			}
			return false;
		}

		public bool SkinnedActor(ActorModel target, int turns)
		{
			if (target.IsValid())
			{
				target.StartSkinned(turns);
				return true;
			}
			return false;
		}

		public bool CreateNoise(GridCoordinate source, int range)
		{
			if (base.manager.GridModel.IsCoordinateValid(source))
			{
				foreach (ActorModel item in GetActorsInRange(source, range))
				{
					item.AIController.HeardNoise(source);
				}
				return true;
			}
			return false;
		}

		public void ActorAttacked(ActorModel attacker, ActorModel target)
		{
			if (attacker == null || target == null)
			{
				return;
			}
			List<ActorModel> enemyFactionsActors = GetEnemyFactionsActors(attacker.Faction);
			for (int i = 0; i < enemyFactionsActors.Count; i++)
			{
				ActorModel actorModel = enemyFactionsActors[i];
				AIController aIController = actorModel.AIController;
				if (aIController.AIDataModel.Alertness < AIAlertness.Homing && IsVisibleFromAnyOccupiedCell(actorModel, attacker.GridCoordinate) && AIBehaviorHelpers.IsTargetInActivationRange(actorModel, this, attacker) && !attacker.IsCamouflaged)
				{
					aIController.AttackTarget(target);
				}
			}
		}

		public bool ChangeThreatLevel(int value, ThreatInstigator instigator)
		{
			ThreatMeter.ChangeThreatLevel(value, instigator);
			return true;
		}

		public void NewTurn()
		{
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(Dormants);
			foreach (ActorModel item in list)
			{
				if ((item.AIController as DormantController).HasWoken)
				{
					item.NotifyChange("actorWokeUp");
					ChangeActorFaction(item, Faction.Walker);
				}
			}
			List<TWDModelObject> models = GetModels<ActivatedObjectModel>();
			UtilsArray.ShuffleList(models, base.manager.Player.PlayerRandom);
			foreach (ActivatedObjectModel item2 in models)
			{
				item2.CheckAction();
			}
			foreach (ActorSpawnPointModel orderedSpawnPoint in OrderedSpawnPoints)
			{
				orderedSpawnPoint.CheckSpawn();
			}
			if (!MissionCompleted)
			{
				ECombatResult result = ECombatResult.Failed;
				if (CheckForEndMission(ref result))
				{
					OnMissionComplete(result, casualtiesResolved: false, (result == ECombatResult.Failed) ? ("NewTurn_" + GetCurrentMissionFailureReason()) : "");
				}
			}
			NotifyChange("turnEnded");
			CheckMissionLogic();
			if (ThreatMeter != null && CombatHUDState.ShowThreatState && !IsEndlessBattleMission && ThreatMeter.TurnCounter <= 0)
			{
				FinishRedactTimedEffect();
			}
			for (int i = 0; i < SurvivalGameModelList.Count; i++)
			{
				SurvivalGameModelList[i].TurnChange();
			}
			UpdateSurvivalGameList();
			if (DebuffQuantunRemove > 0)
			{
				int debuffQuantunRemove = DebuffQuantunRemove - 1;
				DebuffQuantunRemove = debuffQuantunRemove;
				int num = 0;
				int debuffQuantunRemove2 = 0;
				IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider != null)
				{
					List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
					if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.DebuffQuantunRemove) != null)
					{
						num = (int)ChallengeDebufHelps.GetDebufTotalSecondParam(challengeDebuffs, ChallengeDebuffType.DebuffQuantunRemove);
						debuffQuantunRemove2 = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffQuantunRemove);
					}
				}
				int num2 = 0;
				List<ActorModel> allActors = GetAllActors();
				for (int j = 0; j < allActors.Count; j++)
				{
					if (num2 >= num)
					{
						break;
					}
					if (allActors[j].IsQuantuned && allActors[j].IsWalker && DebuffQuantunRemove <= 0)
					{
						allActors[j].CombatCleanQuanTunTimedEffect();
						num2++;
					}
				}
				if (DebuffQuantunRemove <= 0)
				{
					DebuffQuantunRemove = debuffQuantunRemove2;
				}
			}
			else
			{
				DebuffQuantunRemove = 0;
			}
			DeadlyFocus_TurnsEXAttack = 0;
			List<ActorModel> allActors2 = GetAllActors();
			for (int k = 0; k < allActors2.Count; k++)
			{
				allActors2[k]?.SetDeadlyFocusAI();
			}
			if (DebuffQuantunRemoveRaider > 0)
			{
				int debuffQuantunRemove = DebuffQuantunRemoveRaider - 1;
				DebuffQuantunRemoveRaider = debuffQuantunRemove;
				int num3 = 0;
				int debuffQuantunRemoveRaider = 0;
				IChallengeDebuffProvider challengeDebuffProvider2 = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider2 != null)
				{
					List<DifficultyIncrementalDebuff> challengeDebuffs2 = challengeDebuffProvider2.GetChallengeDebuffs();
					if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs2, ChallengeDebuffType.DebuffQuantunRemoveRaider) != null)
					{
						num3 = (int)ChallengeDebufHelps.GetDebufTotalSecondParam(challengeDebuffs2, ChallengeDebuffType.DebuffQuantunRemoveRaider);
						debuffQuantunRemoveRaider = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs2, ChallengeDebuffType.DebuffQuantunRemoveRaider);
					}
				}
				int num4 = 0;
				List<ActorModel> allActors3 = GetAllActors();
				for (int l = 0; l < allActors3.Count; l++)
				{
					if (num4 >= num3)
					{
						break;
					}
					if (allActors3[l].IsQuantuned && DebuffQuantunRemoveRaider <= 0 && allActors3[l].IsRaider)
					{
						allActors3[l].CombatCleanQuanTunTimedEffect();
						num4++;
					}
				}
				if (DebuffQuantunRemoveRaider <= 0)
				{
					DebuffQuantunRemoveRaider = debuffQuantunRemoveRaider;
				}
			}
			else
			{
				DebuffQuantunRemoveRaider = 0;
			}
		}

		public void RefreshCitadelTraits()
		{
			List<ActorModel> list = new List<ActorModel>(Raiders.Models);
			list.AddRange(Survivors.Models);
			for (int i = 0; i < list.Count; i++)
			{
				ActorModel actorModel = list[i];
				if (actorModel != null && actorModel.HasAnyLevelTrait("LeaderBuffCitadel"))
				{
					actorModel.ExecuteCitadelTrait();
				}
			}
		}

		public void SurvivorTurnEnd()
		{
			for (int i = 0; i < Survivors.Count; i++)
			{
				Survivors[i].EquipWeaponEquipment();
				Survivors[i].UnequipConsumableEquipment();
			}
			MissionStatistics.AddTurn();
			ThreatMeter.UpdateTurnCount();
			NotifyChange("survivorTurnEnd");
		}

		public void CollectWalkersToEatLure(ActorModel lure, int turns = 2)
		{
			List<ActorModel> closestWalkersToLure = CombatHelpers.GetClosestWalkersToLure(this, lure.GridCoordinate, preview: false);
			for (int i = 0; i < closestWalkersToLure.Count; i++)
			{
				ActorModel actorModel = closestWalkersToLure[i];
				if (!actorModel.IsDead && !actorModel.AIController.IsActorIncapacitated)
				{
					actorModel.StartEatLure(lure, turns);
				}
			}
		}

		public TWDModelResult FleeStep1(List<SurvivorModel> survivors)
		{
			TWDModelResult result = TWDModelResult.OK;
			if (!CheckMatchAgainstCombatTeam(base.manager.Player.SurvivorContainer.CombatSurvivors, survivors))
			{
				base.manager.Debug.LogError("CombatModel: FleeStep1, survivors passed to flee do not match against the combat team. Possible client hack.");
				return TWDModelResult.Error;
			}
			for (int i = 0; i < survivors.Count; i++)
			{
				SurvivorModel survivorModel = survivors[i];
				survivorModel.StrugglesLeft = 0;
				survivorModel.SetHitPoints(0, survivorModel.MaxHitPoints);
				survivorModel.MinHitpoints = 0;
			}
			MissionCompleted = true;
			ResolveCasualties();
			return result;
		}

		private bool IsAllMissionRosterOfClass(List<SurvivorClass> requiredClasses)
		{
			if (MissionRoster == null || MissionRoster.Count == 0 || requiredClasses == null)
			{
				return false;
			}
			if (requiredClasses.Count != MissionRoster.Count)
			{
				return false;
			}
			List<SurvivorClass> list = new List<SurvivorClass>(requiredClasses);
			foreach (SurvivorModel item in MissionRoster)
			{
				if (!list.Remove(item.SurvivorClass))
				{
					return false;
				}
			}
			return list.Count == 0;
		}

		private bool CheckMatchAgainstCombatTeam(List<SurvivorModel> combatSurvivors, List<SurvivorModel> survivorsToCheck)
		{
			bool flag = true;
			for (int i = 0; i < (survivorsToCheck?.Count ?? 0); i++)
			{
				SurvivorModel survivorModel = survivorsToCheck[i];
				bool flag2 = false;
				for (int j = 0; j < (combatSurvivors?.Count ?? 0); j++)
				{
					if (survivorModel == combatSurvivors[j])
					{
						flag2 = true;
						break;
					}
				}
				flag = flag && flag2;
				if (!flag)
				{
					return flag;
				}
			}
			return flag;
		}

		public TWDModelResult FleeStep2()
		{
			if (!MissionCompleted)
			{
				base.manager.Debug.LogError("CombatModel: FleeStep2 without MissionCompleted=true. Possible client hack.");
				return TWDModelResult.Error;
			}
			ECombatResult eCombatResult = ECombatResult.Flee;
			if (IsPVPMission)
			{
				int survivorsIncapacitated = 0;
				int survivorsInExits = 0;
				GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
				eCombatResult = GetPvpResult(survivorsIncapacitated, Survivors.Count);
			}
			OnMissionComplete(eCombatResult, casualtiesResolved: true, (eCombatResult == ECombatResult.Failed) ? ("FleeStep2_FleeFailed_" + GetCurrentMissionFailureReason()) : "");
			return TWDModelResult.OK;
		}

		private void OnMissionComplete(ECombatResult combatResult, bool casualtiesResolved = false, string failureReason = "")
		{
			WorldBossAttackTargetData worldBossAttackTargetData = base.manager?.Player?.WorldBossModelManager?.AttackTarget;
			if (worldBossAttackTargetData != null && worldBossAttackTargetData.IsActive)
			{
				base.manager.Debug.LogInfo($"WorldBossSettleTrace Stage=OnMissionComplete.Entry Outcome=Observed GroupId={base.manager.Player.GuildId} PlayerHashedId={base.manager.Player.HashedId} SeasonId={worldBossAttackTargetData.SeasonId} CycleId={worldBossAttackTargetData.CycleId} CapturePoint={worldBossAttackTargetData.CapturePoint} Cell={worldBossAttackTargetData.Cell} CombatResult={combatResult} ResultsResolved={ResultsResolved} RetryState={CombatRetryChoicePendingState} HasServerService={base.manager.ServerService != null}");
			}
			if (combatResult == ECombatResult.Failed && string.IsNullOrEmpty(CombatFailureReason))
			{
				CombatFailureReason = failureReason ?? "";
			}
			if (ShouldOfferRetry(combatResult) && CombatRetryChoicePendingState != MissionRetryState.Resolved)
			{
				TurnManager.Paused = true;
				MissionCompleted = false;
				CombatRetryChoicePendingState = MissionRetryState.Pending;
				PendingCombatResult = combatResult;
				if (!casualtiesResolved)
				{
					ResolveCasualties();
				}
				NotifyChange("missionCompleted", combatResult);
				return;
			}
			if (!ResultsResolved)
			{
				TurnManager.Paused = true;
				MissionResult = combatResult;
				MissionCompleted = true;
				ResultsResolved = true;
				AttackedTargetsThisTurn = null;
				CasualtyReport casualtyReport = null;
				if (!casualtiesResolved)
				{
					casualtyReport = ResolveCasualties();
				}
				if (IsSurvivalMission)
				{
					SurvivalCombatHelper.SavePlayerCharacterStates(this, base.manager.Player.SurvivorContainer.SurvivalCharacters);
					if (combatResult == ECombatResult.Successful)
					{
						base.manager.Player.SavedSurvivalMissionData.ClearSavedState();
					}
					else
					{
						base.manager.Player.SavedSurvivalMissionData.DoesSavedMissionDataExist = true;
						SurvivalCombatHelper.IncreaseFailureCount(this);
						SurvivalCombatHelper.SavePersistentVariables(this, base.manager.Player.SavedSurvivalMissionData);
						SurvivalCombatHelper.SaveEnemyCounts(this, base.manager.Player.SavedSurvivalMissionData);
					}
				}
				if (IsEndlessBattleMission)
				{
					EndlessModeCombatModel.EndlessModeManager.HandlePostMissionLogic();
				}
				PersistentMissionVariableManager.Clear();
				MapMissionModel mapMissionModel = base.manager.Player.GetAttackTargetMissionModel() as MapMissionModel;
				GuildBattleMapMissionModel guildBattleMapMissionModel = base.manager.Player.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
				WorldBossMissionModel worldBossMissionModel = base.manager.Player.GetAttackTargetMissionModel() as WorldBossMissionModel;
				MissionStatistics.SetCombatResult(combatResult, IsDeadly, (mapMissionModel != null && (mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge)) ? true : false);
				base.manager.Player.ReportMissionStatistics(MissionStatistics, casualtyReport);
				if (mapMissionModel != null)
				{
					mapMissionModel.LatestRunResult = combatResult;
					if (combatResult == ECombatResult.Successful && mapMissionModel != null && mapMissionModel.Stars != null)
					{
						mapMissionModel.GiveStars();
					}
					if (combatResult == ECombatResult.Successful && mapMissionModel != null && mapMissionModel.IsInWeeklySurvival)
					{
						mapMissionModel.GiveSurvivalCompletions();
					}
					if (mapMissionModel != null && mapMissionModel.IsInApocalyptiWeeklyChallenge)
					{
						base.manager.Player.WeeklyChallengeClassTeamActivity?.ClearLastBattleReward();
					}
					if (combatResult == ECombatResult.Successful && mapMissionModel != null && mapMissionModel.IsInApocalyptiWeeklyChallenge)
					{
						WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivity = base.manager.Player.WeeklyChallengeClassTeamActivity;
						if (weeklyChallengeClassTeamActivity != null && weeklyChallengeClassTeamActivity.IsActive)
						{
							ClassTeamDefinition currentDefinition = weeklyChallengeClassTeamActivity.CurrentDefinition;
							if (currentDefinition != null && currentDefinition.RewardsObj != null && IsAllMissionRosterOfClass(currentDefinition.GetClasses()))
							{
								if (mapMissionModel.IsMasterMission)
								{
									MapMissionGroupModel mapMissionGroupModel = base.manager.Player.ApocalypseWeeklyChallenge.GetMapMissionGroupModel();
									if (mapMissionGroupModel != null)
									{
										int num = 0;
										for (int i = 0; i < mapMissionGroupModel.Missions.Count; i++)
										{
											MapMissionModel mapMissionModel2 = mapMissionGroupModel.Missions[i];
											if (mapMissionModel2 != null && !mapMissionModel2.IsMasterMission && !mapMissionModel2.ClassTeamRewardGiven)
											{
												mapMissionModel2.ClassTeamRewardGiven = true;
												num++;
											}
										}
										for (int j = 0; j < num; j++)
										{
											currentDefinition.RewardsObj.Give(base.manager);
											base.manager.TdMetrics.SetEventType("Class_Team_Challenge_Reward").AddProperty("reward_info", currentDefinition.Reward).Send();
											base.manager.Metrics.AddClassTeamReward(currentDefinition.Reward).Send();
										}
										weeklyChallengeClassTeamActivity.RecordBattleReward(currentDefinition.RewardsObj, num);
									}
								}
								else if (!mapMissionModel.ClassTeamRewardGiven)
								{
									mapMissionModel.ClassTeamRewardGiven = true;
									currentDefinition.RewardsObj.Give(base.manager);
									weeklyChallengeClassTeamActivity.RecordBattleReward(currentDefinition.RewardsObj, 1);
									base.manager.TdMetrics.SetEventType("Class_Team_Challenge_Reward").AddProperty("reward_info", currentDefinition.Reward).Send();
									base.manager.Metrics.AddClassTeamReward(currentDefinition.Reward).Send();
								}
							}
						}
					}
					base.manager.Metrics.ResetTdEvent().AddOriginalEventType();
					if (base.manager.Player.Combat != null && base.manager.Player.Combat.HasPvPRules)
					{
						base.manager.Metrics.AddEnd().AddMission().AddMissionResult(combatResult)
							.AddMissionType()
							.AddOutpostTutorial(OutpostTutorialStateForAnalytics.FirstAttackDone)
							.Send();
					}
					else
					{
						base.manager.Metrics.AddEnd().AddMission().AddMissionResult(combatResult)
							.AddMissionType()
							.Send();
					}
					base.manager.Metrics.TdEventType = "End_Mission_MissionResult";
					base.manager.Metrics.TdEventPropertyTypes = new List<string>
					{
						"Mission", "MissionResult", "GvG", "PvP", "Grind", "Challenge", "ApocalypticChallenge", "Distance", "Season", "Endless",
						"Story"
					};
					base.manager.Metrics.SendTdEvent();
				}
				else if (guildBattleMapMissionModel != null)
				{
					if (base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentGuildBattle())
					{
						List<int> savedData = (base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMission.IsPvPCombat ? GuildBattleCombatHelper.CreateSaveData(this, guildBattleMapMissionModel) : null);
						guildBattleMapMissionModel.SendGuildProgressToGuild(combatResult, base.manager, RetryMission, savedData);
					}
					else if (base.manager != null && base.manager.Debug != null)
					{
						GuildBattleModelPlayer guildBattleModelPlayer = null;
						if (base.manager.Player != null && base.manager.Player.GvGSeasonModelPlayer != null && base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer != null)
						{
							guildBattleModelPlayer = base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel;
						}
						GuildBattleModel guildBattleModel = null;
						if (base.manager.Player != null && base.manager.Player.GuildWarModel != null)
						{
							guildBattleModel = base.manager.Player.GuildWarModel.CurrentBattle;
						}
						string text = ((guildBattleMapMissionModel != null) ? guildBattleMapMissionModel.Id : "null");
						string text2 = ((guildBattleModelPlayer != null && guildBattleModelPlayer.CurrentBattleId != null) ? guildBattleModelPlayer.CurrentBattleId : "null");
						string text3 = ((guildBattleModel != null && guildBattleModel.BattleId != null) ? guildBattleModel.BattleId : "null");
						string text4 = ((base.manager.Player != null) ? base.manager.Player.IsGuildMember.ToString() : "null");
						string text5 = ((base.manager.Player != null) ? base.manager.Player.UtcTimeStamp.ToString() : "null");
						string text6 = ((guildBattleModel != null) ? guildBattleModel.EndBattleTimestamp.ToString() : "null");
						string text7 = ((guildBattleModel != null) ? guildBattleModel.HasEnded().ToString() : "null");
						base.manager.Debug.LogInfo("OnMissionComplete GuildBattle skipped SendGuildProgressToGuild, IsCurrentGuildBattle=false. combatResult=" + combatResult.ToString() + ", missionId=" + text + ", playerCurrentBattleId=" + text2 + ", guildCurrentBattleId=" + text3 + ", IsGuildMember=" + text4 + ", currentTime=" + text5 + ", battleEndTime=" + text6 + ", battleHasEnded=" + text7);
					}
					if (combatResult == ECombatResult.Successful)
					{
						base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GiveMissionPersonalRewards(guildBattleMapMissionModel);
						base.manager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.AddPersonalMissionProgression();
					}
					base.manager.Metrics.ResetTdEvent().AddOriginalEventType();
					base.manager.Metrics.AddEnd().AddMission().AddMissionResult(combatResult)
						.AddGvG()
						.AddGvGBattle()
						.AddCombatFailureReason(CombatFailureReason)
						.AddGvGPvPInfoIfNeeded()
						.Send();
					base.manager.Metrics.TdEventType = "End_Mission_MissionResult";
					base.manager.Metrics.TdEventPropertyTypes = new List<string> { "Mission", "MissionResult", "GvG", "GvGBattle" };
					base.manager.Metrics.SendTdEvent();
				}
				else if (worldBossMissionModel != null)
				{
					base.manager.Debug.LogInfo($"WorldBossSettleTrace Stage=OnMissionComplete.WorldBossRoute Outcome=Selected GroupId={base.manager.Player.GuildId} PlayerHashedId={base.manager.Player.HashedId} SeasonId={(base.manager.Player.WorldBossModelManager?.AttackTarget?.SeasonId).GetValueOrDefault()} CycleId={(base.manager.Player.WorldBossModelManager?.AttackTarget?.CycleId).GetValueOrDefault()} CapturePoint={base.manager.Player.WorldBossModelManager?.AttackTarget?.CapturePoint ?? string.Empty} Cell={base.manager.Player.WorldBossModelManager?.AttackTarget?.Cell ?? string.Empty} CombatResult={combatResult} HasServerService={base.manager.ServerService != null}");
					WorldBossModelManager worldBossModelManager = base.manager.Player.WorldBossModelManager;
					WorldBossAttackTargetData worldBossAttackTargetData2 = worldBossModelManager?.AttackTarget;
					if (worldBossAttackTargetData2 != null && worldBossAttackTargetData2.IsActive)
					{
						bool flag = combatResult == ECombatResult.Successful;
						bool isTimeout = !flag && worldBossMissionModel.IsBattleTimedOut(base.manager.Player.UtcTimeStamp);
						worldBossAttackTargetData2.SetResult(flag, isTimeout);
						if (worldBossMissionModel.CapturePoint == "BOSS")
						{
							worldBossAttackTargetData2.SetBossScore((long)Math.Round(GuildBossPoint));
							worldBossAttackTargetData2.SetBossDamage(GuildBossDamage);
						}
						WorldBossCombatHelper.SettleCombatResult(base.manager, flag, isTimeout);
						int currentBattleDifficulty = worldBossModelManager.GetCurrentBattleDifficulty();
						WorldBossCycleDefinition worldBossCycleDefinition = base.manager.Player.gameEconomyData.FindWorldBossCycleDefinition(worldBossAttackTargetData2.SeasonId, worldBossAttackTargetData2.CycleId);
						int battleScoreChange = WorldBossCombatHelper.GetBattleScoreChange(worldBossMissionModel.WorldBossMissionType, base.manager.Player.gameEconomyData.WorldBossConfig, flag, isTimeout, worldBossAttackTargetData2.BossScore);
						string heroWeaponUse = WorldBossCombatHelper.BuildHeroWeaponUse(MissionRoster);
						base.manager.Metrics.ResetTdEvent().AddOriginalEventType();
						base.manager.Metrics.AddEnd().AddMission().AddMissionResult(combatResult)
							.AddWorldBossBattleResult(battleScoreChange, heroWeaponUse, worldBossCycleDefinition?.ID ?? 0, currentBattleDifficulty)
							.Send();
						base.manager.Metrics.TdEventType = "End_Mission_MissionResult";
						base.manager.Metrics.TdEventPropertyTypes = new List<string> { "Mission", "MissionResult", "WorldBoss" };
						base.manager.Metrics.SendTdEvent();
					}
				}
				else
				{
					bool flag2 = true;
					if (OutpostCombat != null && OutpostCombat.DefendingSurvivors != null && OutpostCombat.DefendingSurvivors.Count > 0)
					{
						for (int k = 0; k < OutpostCombat.DefendingSurvivors.Count; k++)
						{
							SurvivorModel survivorModel = OutpostCombat.DefendingSurvivors[k];
							if (survivorModel == null || survivorModel.manager == null)
							{
								base.manager.Debug.LogWarning("OnMissionComplete for Outpost, either OutpostCombat.DefendingSurvivor[" + k + "] or model.manager is NULL, not sending analytics.");
								flag2 = false;
								break;
							}
						}
					}
					if (flag2)
					{
						base.manager.Metrics.ResetTdEvent().AddOriginalEventType();
						base.manager.Metrics.AddEnd().AddMission().AddMissionResult(combatResult)
							.AddPvp()
							.AddPvpAttacker()
							.AddPvpDefender(OutpostCombat)
							.Send();
						base.manager.Metrics.TdEventType = "End_Mission_MissionResult";
						base.manager.Metrics.TdEventPropertyTypes = new List<string> { "Mission", "MissionResult", "PvP" };
						base.manager.Metrics.SendTdEvent();
					}
				}
				for (int l = 0; l < MissionRoster.Count; l++)
				{
					SurvivorModel survivorModel2 = MissionRoster[l];
					if (survivorModel2 == null)
					{
						continue;
					}
					EquipmentItemModel weaponEquipment = survivorModel2.GetWeaponEquipment();
					EquipmentItemModel equipmentOfCategory = survivorModel2.GetEquipmentOfCategory(EquipmentCategory.Armor);
					if (weaponEquipment == null || equipmentOfCategory == null)
					{
						continue;
					}
					base.manager.Metrics.AddEnd().AddMission().AddMissionType();
					if (guildBattleMapMissionModel != null)
					{
						base.manager.Metrics.AddGvGBattle();
						if (l == 0)
						{
							base.manager.Metrics.AddLeaderEvent(survivorModel2);
						}
					}
					base.manager.Metrics.TdEventType = "Hero_Use_State";
					base.manager.Metrics.TdEventPropertyTypes = new List<string> { "Survivor", "SupportUnit", "SurvivorResul", "SupportResult", "Equipment" };
					SupportManager.TryGetSupport(l, out var combatSupportModel);
					base.manager.Metrics.AddSurvivor(survivorModel2).AddWinAndLose(combatResult).AddMissionCategory(survivorModel2)
						.AddSupportUnit(combatSupportModel?.SupportModel)
						.AddSurvivorResult(survivorModel2)
						.AddSupportResult(combatSupportModel)
						.AddEquipmentWeapon(weaponEquipment)
						.AddEquipmentArmor(equipmentOfCategory)
						.AddBadgeList(survivorModel2.BadgeContainer.Badges, survivorModel2, MissionRoster)
						.Send();
				}
				if (combatResult == ECombatResult.Successful)
				{
					GrantStaticGrindMissionRewards();
					Metrics.MetricsResourcesData metricsResourcesData = new Metrics.MetricsResourcesData();
					GrantGuaranteedMissionRewards(ref metricsResourcesData);
					if (!GrantSeasonTrialMissionRewards(ref metricsResourcesData))
					{
						base.Debug.LogError("Season reward supports currently only one currency");
					}
					if (metricsResourcesData.HasResources())
					{
						base.manager.Metrics.AddFind().AddResources(metricsResourcesData).AddMission()
							.AddMissionType()
							.AddStaticReward()
							.Send();
					}
					if (mapMissionModel != null && mapMissionModel.MissionSpawnPointGroup != null && mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season)
					{
						mapMissionModel.CompletionTimes++;
					}
					mapMissionModel?.UpdateMapState();
					base.manager.Player.DailyQuestManager.StartAction("CompleteMission");
					base.manager.Player.DailyQuestManager.CommitAction();
					base.manager.Player.NotifyMissionCompleted();
				}
				else
				{
					base.manager.Player.RFMGiftManager.TriggerRFMEvent(RFMEvent.missionFail);
				}
				SendAnalyticsFromCollectedRewards();
				NotifyChange("missionCompleted", combatResult);
			}
			for (int m = 0; m < MissionRoster.Count; m++)
			{
				SurvivorModel survivorModel3 = MissionRoster[m];
				survivorModel3.ClearEquipmentActiveKingFactor();
				survivorModel3.ClearRandomStatusNumberOfAttacks();
				survivorModel3.RandomStatusTraitIdentifier = null;
				survivorModel3.ResetAttributeGreene();
				survivorModel3.ResetParryRiposteIncreaseStorey();
				survivorModel3.HelpreHandActorModel = null;
				survivorModel3.GuardActorModel = null;
				survivorModel3.RemoveTrait("ShadowedGuard_StateRef");
			}
			SurvivalGameModelList.Clear();
		}

		private void SendAnalyticsFromCollectedRewards()
		{
			base.manager.Metrics.AddFind();
			if (MissionStatistics.ActualSuppliesAdded > 0 || MissionStatistics.GetSuppliesOverflow() > 0)
			{
				base.manager.Metrics.PushResource(CurrencyType.Supplies, MissionStatistics.ActualSuppliesAdded, MissionStatistics.GetSuppliesOverflow());
			}
			if (MissionStatistics.ActualSurvivalPointsAdded > 0 || MissionStatistics.GetSPOverflow() > 0)
			{
				base.manager.Metrics.PushResource(CurrencyType.SurvivalPoints, MissionStatistics.ActualSurvivalPointsAdded, MissionStatistics.GetSPOverflow());
			}
			BattlePassModel battlePass = base.manager.Player.BattlePass;
			if (MissionStatistics.BattlePassCurrencyEarned > 0 && battlePass.IsSeasonActive)
			{
				base.manager.Metrics.PushResource(CurrencyType.BattlePassPoints, MissionStatistics.BattlePassCurrencyEarned);
				base.manager.Metrics.AddBattlePass(battlePass).AddBattlePassEnemiesKilledProperty(MissionStatistics.BattlePassCurrencyEarned);
			}
			if (base.manager.Metrics.metricsResourcesData.HasResources())
			{
				base.manager.Metrics.AddResources().AddMission().AddMissionType()
					.AddWalkersKilled()
					.Send();
			}
			base.manager.Metrics.Reset();
		}

		private void ValidateCombatModel()
		{
			UpdateOccupiers();
			List<ActorModel> allActors = GetAllActors();
			for (int i = 0; i < allActors.Count; i++)
			{
				ActorModel actorModel = allActors[i];
				for (int j = 1; j < allActors.Count; j++)
				{
					ActorModel actorModel2 = allActors[j];
					if (actorModel == actorModel2 || !(actorModel.GridCoordinate == actorModel2.GridCoordinate) || actorModel.IsDead || actorModel2.IsDead)
					{
						continue;
					}
					GridCoordinate gridCoordinate = GridCoordinate.Invalid;
					for (int k = 0; k < 8; k++)
					{
						GridCoordinate coordinateNeighbor = Grid.GetCoordinateNeighbor(actorModel.GridCoordinate, k);
						if (coordinateNeighbor.IsValid && !IsBlocked(coordinateNeighbor) && GetOccupier(coordinateNeighbor) == null)
						{
							gridCoordinate = coordinateNeighbor;
						}
					}
					if (gridCoordinate == GridCoordinate.Invalid)
					{
						FixedPoint fixedPoint = FixedPoint.MaxValue;
						for (int l = 0; l < Grid.NumCells; l++)
						{
							GridCoordinate coordinate = Grid.GetCoordinate(l);
							if (coordinate.IsValid && !IsBlocked(coordinate) && GetOccupier(coordinate) == null && coordinate.DistanceTo(actorModel.GridCoordinate) < fixedPoint)
							{
								fixedPoint = coordinate.DistanceTo(actorModel.GridCoordinate);
								gridCoordinate = coordinate;
							}
						}
					}
					if (gridCoordinate != GridCoordinate.Invalid)
					{
						if (actorModel.Faction == Faction.Survivor)
						{
							actorModel2.GridCoordinate = gridCoordinate;
							actorModel2.FinishTimedEffect(interrupted: true);
							base.manager.Debug.LogWarning("CombatModel validation error: Actors [" + actorModel.ToString() + "] and [" + actorModel2.ToString() + "] occupied the same cell! " + actorModel2.ToString() + " was moved.");
						}
						else
						{
							actorModel.GridCoordinate = gridCoordinate;
							actorModel.FinishTimedEffect(interrupted: true);
							base.manager.Debug.LogWarning("CombatModel validation error: Actors [" + actorModel.ToString() + "] and [" + actorModel2.ToString() + "] occupied the same cell! " + actorModel.ToString() + " was moved.");
						}
					}
					else
					{
						base.manager.Debug.LogWarning("CombatModel validation error: Actors [" + actorModel.ToString() + "] and [" + actorModel2.ToString() + "] are occupying the same cell and cannot find a free cell where to move the other one!");
					}
					UpdateOccupiers();
				}
				TimedEffect exclusiveTimedEffect = actorModel.ExclusiveTimedEffect;
				if (exclusiveTimedEffect != null)
				{
					ActorModel actorModel3 = exclusiveTimedEffect.Target as ActorModel;
					GridCoordinate gridCoordinate2 = actorModel3?.GridCoordinate ?? exclusiveTimedEffect.TargetCoordinate;
					if (gridCoordinate2 != GridCoordinate.Invalid && (!((actorModel3 != null) ? AreActorsAdjacent(actorModel, actorModel3) : Grid.AreNeighbors(actorModel.GridCoordinate, gridCoordinate2)) || (actorModel3 != null && GetOccupier(gridCoordinate2) != actorModel3)))
					{
						base.manager.Debug.LogWarning("CombatModel validation error: Actor [" + actorModel.ToString() + "] has an invalid TimedEffect: " + Enum.GetName(typeof(TimedEffectType), exclusiveTimedEffect.Type) + ". The effect is finished.");
						actorModel.FinishTimedEffect(interrupted: true);
					}
				}
			}
		}

		private string GetMissionNameEnglishForAnalytics(MapMissionModel attackTargetMissionModel, MapCategory category)
		{
			string result = "";
			if (attackTargetMissionModel == null)
			{
				result = (string.IsNullOrEmpty(SceneName) ? "outpost" : SceneName);
			}
			else
			{
				switch (category)
				{
				case MapCategory.ApocalypticChallenge:
				{
					MapMissionGroupModel missionGroupModelThatContains4 = base.manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains4 != null && missionGroupModelThatContains4.MissionSpawnPointGroup != null)
					{
						result = $"A_{missionGroupModelThatContains4.MissionSpawnPointGroup.DisplayName}_M_{MissionNameEnglish}";
					}
					break;
				}
				case MapCategory.Challenge:
				{
					MapMissionGroupModel missionGroupModelThatContains3 = base.manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains3 != null && missionGroupModelThatContains3.MissionSpawnPointGroup != null)
					{
						result = $"C_{missionGroupModelThatContains3.MissionSpawnPointGroup.DisplayName}_M_{MissionNameEnglish}";
					}
					break;
				}
				case MapCategory.Survival:
				{
					MapMissionGroupModel missionGroupModelThatContains2 = base.manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains2 != null && missionGroupModelThatContains2.MissionSpawnPointGroup != null)
					{
						result = $"S_{missionGroupModelThatContains2.MissionSpawnPointGroup.DisplayName}_M_{MissionNameEnglish}";
					}
					break;
				}
				case MapCategory.Story:
					result = string.Format("E{0}M{1}_{2}", (base.manager.Player.MapContainerModel.GetEpisodeIndex(attackTargetMissionModel) + 1).ToString("D2"), (base.manager.Player.MapContainerModel.GetMissionIndex(attackTargetMissionModel) + 1).ToString("D2"), MissionNameEnglish);
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
					MapMissionGroupModel missionGroupModelThatContains = base.manager.Player.MapContainerModel.GetMissionGroupModelThatContains(attackTargetMissionModel);
					if (missionGroupModelThatContains != null && missionGroupModelThatContains.MissionSpawnPointGroup != null)
					{
						result = string.Format("{0}_{1}_M_{2}", category switch
						{
							MapCategory.GuildBossPVP => "GB_PVP",
							MapCategory.GuildBossPVE => "GB_PVE",
							_ => "GB",
						}, missionGroupModelThatContains.MissionSpawnPointGroup.DisplayName, MissionNameEnglish);
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

		public TWDModelResult SkipTurn()
		{
			if (TurnManager.CanSwitchActiveActor && TurnManager.ActiveFaction == Faction.Survivor)
			{
				foreach (ActorModel survivor in Survivors)
				{
					HealActorStatus(survivor);
					survivor.EquipWeaponEquipment();
					survivor.UnequipConsumableEquipment();
					if (!survivor.TurnComplete)
					{
						survivor.EndAction();
					}
				}
			}
			return TWDModelResult.OK;
		}

		public TWDModelResult HealActorStatus(ActorModel actor)
		{
			if (!actor.AbilityCompleted && !actor.SecondMoveCompleted)
			{
				bool flag = false;
				if (actor.HasTrait("Bleeding"))
				{
					actor.RemoveTrait("Bleeding");
					flag = true;
				}
				if (actor.HasTrait("Burning"))
				{
					actor.RemoveTrait("Burning");
					flag = true;
				}
				if (flag)
				{
					actor.EndAbilityAction(allowSecondMove: false, 0, resetMoveCompleted: false, clearInvisibility: false);
				}
			}
			return TWDModelResult.OK;
		}

		public TWDModelResult StartCombat()
		{
			if (!MissionStarted)
			{
				CombatFailureReason = "";
				MissionStarted = true;
				MissionStartedChanged = true;
				TutorialModel tutorial = base.manager.Player.Tutorial;
				if (tutorial != null && tutorial.CurrentPartId != null && (tutorial.CurrentPartId == "Tutorial" || tutorial.CurrentPartId == "Tutorial_Training_Ground" || tutorial.CurrentPartId == "Phone"))
				{
					tutorial.SetPartCompleted(tutorial.CurrentPartId);
				}
				base.manager.Player.LootManager.AddCombatFoundKey(InitialLootKeys);
				for (int i = 0; i < InitialLootKeys; i++)
				{
					base.manager.CombatModel.MissionStatistics.AddCollectedLoot();
				}
				RefreshDashTraitFlag();
				IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider != null)
				{
					List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
					int debuffQuantunRemove = 0;
					int debuffQuantunRemoveRaider = 0;
					if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.DebuffQuantunRemove) != null)
					{
						debuffQuantunRemove = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffQuantunRemove);
					}
					else if (ChallengeDebufHelps.GetDebufConfig(challengeDebuffs, ChallengeDebuffType.DebuffQuantunRemoveRaider) != null)
					{
						debuffQuantunRemoveRaider = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffQuantunRemoveRaider);
					}
					DebuffQuantunRemove = debuffQuantunRemove;
					DebuffQuantunRemoveRaider = debuffQuantunRemoveRaider;
				}
				GetOrResolveMissionLogicFailTurnLimit();
			}
			else
			{
				SessionResumeCount++;
				MissionStartedChanged = false;
			}
			if (Survivors != null && Survivors.Count > 0 && SurvivorSlots != null && SurvivorSlots.Count == 0)
			{
				for (int j = 0; j < Survivors.Count; j++)
				{
					SurvivorSlots.Add(j + 1, Survivors[j]);
				}
			}
			if (Raiders != null && Raiders.Count > 0 && Raiders != null && Raiders.Count == 0)
			{
				for (int k = 0; k < Raiders.Count; k++)
				{
					RaiderSlots.Add(k + 1, Raiders[k]);
				}
			}
			NotifyChange("missionLoadedEvent");
			return TWDModelResult.OK;
		}

		public TWDModelResult EndCombat(bool forceFailure = false)
		{
			ECombatResult endCombatResult = GetEndCombatResult(forceFailure);
			ThreatMeter.Changed -= OnThreatValueChanged;
			OnMissionComplete(endCombatResult, casualtiesResolved: false, (endCombatResult == ECombatResult.Failed) ? ("EndCombat_" + GetCurrentMissionFailureReason()) : "");
			return TWDModelResult.OK;
		}

		public ECombatResult GetEndCombatResult(bool forceFailure)
		{
			if (forceFailure)
			{
				return ECombatResult.Failed;
			}
			ECombatResult result = ECombatResult.Failed;
			if (IsPVPMission)
			{
				int survivorsIncapacitated = 0;
				int survivorsInExits = 0;
				GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
				result = GetPvpResult(survivorsIncapacitated, Survivors.Count);
			}
			return result;
		}

		public GridPath FindPath(ActorModel mover, GridCoordinate from, GridCoordinate to)
		{
			GridPath gridPath = GridPath.Create();
			if (Grid.AreNeighbors(from, to) && GetOccupier(to) != null && CanTraverse(null, from, to))
			{
				gridPath.Invalidate();
				return gridPath;
			}
			FixedPoint fixedPoint = ((mover.Faction == Faction.Survivor) ? 1.5f : 1f);
			GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(this, to, new DistanceFieldOptions((float)fixedPoint, mover, mover));
			GridField<bool> gridField2 = new GridField<bool>(base.manager.GridModel.Width, base.manager.GridModel.Height, defaultValue: false);
			Queue<GridCoordinate> queue = new Queue<GridCoordinate>();
			queue.Enqueue(from);
			gridField2[from] = true;
			gridPath.AddNode(from);
			bool flag = GetOccupier(to) != null || IsBlocked(to);
			while (queue.Count > 0)
			{
				GridCoordinate gridCoordinate = queue.Dequeue();
				bool flag2 = gridCoordinate != from && (IsBlocked(gridCoordinate) || GetOccupier(gridCoordinate) != null);
				GridCoordinate gridCoordinate2 = GridCoordinate.Invalid;
				FixedPoint fixedPoint2 = FixedPoint.MinValue;
				foreach (GridCoordinate item in Grid.Neighbors(gridCoordinate))
				{
					if (!gridField2[item] && CanTraverse(mover, gridCoordinate, item) && !IsBlocked(item))
					{
						FixedPoint fixedPoint3 = gridField[gridCoordinate] - gridField[item];
						bool flag3 = flag2 && !gridCoordinate2.IsValid;
						if ((fixedPoint3 > 0.0 && fixedPoint3 > fixedPoint2) || flag3)
						{
							fixedPoint2 = fixedPoint3;
							gridCoordinate2 = item;
						}
						gridField2[item] = true;
					}
				}
				if (Grid.IsCoordinateValid(gridCoordinate2))
				{
					gridPath.AddNode(gridCoordinate2);
					if (gridCoordinate2 == to || (flag && gridField[gridCoordinate2] < 1.5 && GetOccupier(gridCoordinate2) == null))
					{
						break;
					}
					queue.Enqueue(gridCoordinate2);
				}
			}
			if (flag)
			{
				bool num = Grid.AreNeighbors(gridPath.End, to);
				bool flag4 = CanTraverse(null, gridPath.End, to);
				if (!num || !flag4)
				{
					gridPath.Invalidate();
				}
			}
			else if (!gridPath.EndsAt(to))
			{
				gridPath.Invalidate();
			}
			return gridPath;
		}

		public bool CanTraverse(ActorModel mover, GridCoordinate fromCoordinate, GridCoordinate toCoordinate, float range = 0f)
		{
			if (!Grid.IsCoordinateValid(fromCoordinate) || !Grid.IsCoordinateValid(toCoordinate))
			{
				return false;
			}
			if (fromCoordinate == toCoordinate)
			{
				return true;
			}
			if (range > 0f)
			{
				range *= (float)Grid.CellSize.X;
				FixedPoint fixedPoint = range * range;
				FixedVec3 position = Grid.GetPosition(fromCoordinate);
				FixedVec3 position2 = Grid.GetPosition(toCoordinate);
				if ((position - position2).SqrMagnitude > fixedPoint)
				{
					return false;
				}
			}
			int coordinateNeighborIndex = Grid.GetCoordinateNeighborIndex(fromCoordinate, toCoordinate);
			if (coordinateNeighborIndex == -1)
			{
				return false;
			}
			if (traversableCache != null)
			{
				if ((traversableCache[fromCoordinate] & (1 << coordinateNeighborIndex)) == 0)
				{
					return false;
				}
			}
			else if (!CalculateCanTraverseEdge(fromCoordinate, toCoordinate))
			{
				return false;
			}
			if (mover != null)
			{
				ActorModel occupier = GetOccupier(toCoordinate);
				if (occupier != null)
				{
					return !occupier.IsEnemy(mover);
				}
				return true;
			}
			return true;
		}

		public bool IsBlocked(GridCoordinate target)
		{
			if (!Grid.IsCoordinateValid(target))
			{
				return true;
			}
			if (blockedCache != null)
			{
				return blockedCache[target];
			}
			return CalculateIsBlocked(target);
		}

		public ActorModel GetOccupier(GridCoordinate coordinate)
		{
			if (Occupiers != null)
			{
				return Occupiers[coordinate];
			}
			return null;
		}

		public GridCoordinate ResolveMultiCellTargetCell(GridCoordinate sourceCell, GridCoordinate targetCell)
		{
			ActorModel occupier = GetOccupier(targetCell);
			if (occupier != null && occupier.IsMultiCell)
			{
				return occupier.GetClosestOccupiedCell(sourceCell);
			}
			return targetCell;
		}

		public bool AreActorsAdjacent(ActorModel a, ActorModel b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			if (a.IsMultiCell || b.IsMultiCell)
			{
				List<GridCoordinate> occupiedCells = a.GetOccupiedCells();
				List<GridCoordinate> occupiedCells2 = b.GetOccupiedCells();
				for (int i = 0; i < occupiedCells.Count; i++)
				{
					for (int j = 0; j < occupiedCells2.Count; j++)
					{
						if (Grid.AreNeighbors(occupiedCells[i], occupiedCells2[j]))
						{
							return true;
						}
					}
				}
				return false;
			}
			return Grid.AreNeighbors(a.GridCoordinate, b.GridCoordinate);
		}

		public InteractiveObjectModel GetInteractiveObject(GridCoordinate coordinate, bool onlyUsable = true)
		{
			if (InteractiveObjects != null)
			{
				InteractiveObjectModel interactiveObjectModel = InteractiveObjects[coordinate];
				if (interactiveObjectModel != null && (!onlyUsable || (!interactiveObjectModel.HasInteractionStarted && !interactiveObjectModel.HasBeenActivated)))
				{
					return interactiveObjectModel;
				}
			}
			return null;
		}

		public bool IsInteractiveObjectCoordinate(GridCoordinate coordinate)
		{
			foreach (InteractiveObjectModel model in GetModels<InteractiveObjectModel>())
			{
				if (model.Placement == Placement.Cell && model.Location.Contains(coordinate))
				{
					return true;
				}
				if (model.Placement != Placement.Edge)
				{
					continue;
				}
				foreach (int edge in model.Location.Edges)
				{
					Grid.GetCoordinatesFromEdge(edge, out var a, out var b);
					if (a == coordinate || b == coordinate)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int GetCoveredDirections(GridCoordinate coverCoordinate)
		{
			int num = 0;
			foreach (CoverModel model in GetModels<CoverModel>())
			{
				if (model.IsActive && model.CoverCoordinates.Contains(coverCoordinate))
				{
					CoverDirection direction = model.GetDirection(coverCoordinate);
					if (direction != CoverDirection.None)
					{
						num |= 1 << (int)direction;
					}
				}
			}
			return num;
		}

		public List<CoverDirection> GetCoverDirections(GridCoordinate coverCoordinate)
		{
			List<CoverDirection> list = null;
			if (GetCoveredDirections(coverCoordinate) != 0)
			{
				list = new List<CoverDirection>();
				if (CoordinateCoversDirection(coverCoordinate, CoverDirection.Top))
				{
					list.Add(CoverDirection.Top);
				}
				if (CoordinateCoversDirection(coverCoordinate, CoverDirection.Right))
				{
					list.Add(CoverDirection.Right);
				}
				if (CoordinateCoversDirection(coverCoordinate, CoverDirection.Left))
				{
					list.Add(CoverDirection.Left);
				}
				if (CoordinateCoversDirection(coverCoordinate, CoverDirection.Bottom))
				{
					list.Add(CoverDirection.Bottom);
				}
			}
			return list;
		}

		public void UpdateCoverField()
		{
			if (CoverField == null)
			{
				CoverField = new GridField<int>(Grid.Width, Grid.Height, 0);
			}
			else
			{
				CoverField.Clear();
			}
			for (int i = 0; i < Grid.NumCells; i++)
			{
				CoverField[i] = GetCoveredDirections(Grid.GetCoordinate(i));
			}
		}

		public bool IsInCover(GridCoordinate coverCoordinate, GridCoordinate fromCoordinate)
		{
			if (CoverField == null)
			{
				return false;
			}
			int num = CoverField[coverCoordinate];
			if (num == 0)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((num & (1 << i)) != 0)
				{
					FixedPoint coverAngle = base.manager.GameEconomyData.ConfigData.CoverAngle;
					FixedPoint fixedPoint = FixedPoint.DegToRad(coverAngle);
					GridCoordinate coordinateNeighbor = Grid.GetCoordinateNeighbor(coverCoordinate, (i * 2 + 2) % 8);
					if (coordinateNeighbor.IsValid && (CoverField[coordinateNeighbor] & (1 << i)) != 0)
					{
						fixedPoint = FixedPoint.PI / 2.0;
					}
					FixedPoint fixedPoint2 = FixedPoint.DegToRad(coverAngle);
					GridCoordinate coordinateNeighbor2 = Grid.GetCoordinateNeighbor(coverCoordinate, (i * 2 + 6) % 8);
					if (coordinateNeighbor2.IsValid && (CoverField[coordinateNeighbor2] & (1 << i)) != 0)
					{
						fixedPoint2 = FixedPoint.PI / 2.0;
					}
					FixedVec3 fixedVec = default(FixedVec3);
					switch (i)
					{
					case 0:
						fixedVec = new FixedVec3(0.0, 0.0, 1.0);
						break;
					case 1:
						fixedVec = new FixedVec3(1.0, 0.0, 0.0);
						break;
					case 2:
						fixedVec = new FixedVec3(0.0, 0.0, -1.0);
						break;
					case 3:
						fixedVec = new FixedVec3(-1.0, 0.0, 0.0);
						break;
					}
					FixedVec3 position = Grid.GetPosition(coverCoordinate);
					FixedVec3 fixedVec2 = FixedVec3.Normalize(Grid.GetPosition(fromCoordinate) - position);
					FixedPoint radians = fixedPoint;
					FixedVec3 b = FixedVec3.Cross(fixedVec, new FixedVec3(0.0, 1.0, 0.0));
					if (FixedVec3.Dot(fixedVec2, b) >= 0.0)
					{
						radians = fixedPoint2;
					}
					if (FixedVec3.Dot(fixedVec, fixedVec2) > FixedPoint.Cos(radians))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasCover(GridCoordinate coordinate)
		{
			return GetCoveredDirections(coordinate) != 0;
		}

		public bool IsCoverFlanked(GridCoordinate coordinate, ActorModel actor)
		{
			if (HasCover(coordinate))
			{
				if (actor.AIController.IsActorIncapacitated)
				{
					return true;
				}
				List<ActorModel> enemyFactionsActors = GetEnemyFactionsActors(actor.Faction);
				for (int i = 0; i < enemyFactionsActors.Count; i++)
				{
					ActorModel actorModel = enemyFactionsActors[i];
					if (actorModel.IsWalker || actorModel.AIController.IsActorIncapacitated)
					{
						continue;
					}
					bool flag;
					if (!actorModel.IsMultiCell)
					{
						flag = IsGridCellVisible(coordinate, actorModel.GridCoordinate) && !IsInCover(coordinate, actorModel.GridCoordinate);
					}
					else
					{
						flag = false;
						List<GridCoordinate> occupiedCells = actorModel.GetOccupiedCells();
						for (int j = 0; j < occupiedCells.Count; j++)
						{
							if (IsGridCellVisible(coordinate, occupiedCells[j]) && !IsInCover(coordinate, occupiedCells[j]))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsCoverFlankedAfterMove(GridCoordinate coverCoordinate, ActorModel coveredActor, GridCoordinate moveCoordinate, ActorModel movingActor)
		{
			if (!movingActor.IsEnemy(coveredActor))
			{
				return IsCoverFlanked(coverCoordinate, coveredActor);
			}
			if (HasCover(coverCoordinate))
			{
				if (coveredActor.AIController.IsActorIncapacitated)
				{
					return true;
				}
				List<ActorModel> enemyFactionsActors = GetEnemyFactionsActors(coveredActor.Faction);
				for (int i = 0; i < enemyFactionsActors.Count; i++)
				{
					ActorModel actorModel = enemyFactionsActors[i];
					if (actorModel.IsWalker || actorModel.IsEnvironmental || actorModel.AIController.IsActorIncapacitated)
					{
						continue;
					}
					if (actorModel == movingActor)
					{
						if (IsGridCellVisible(coverCoordinate, moveCoordinate) && !IsInCover(coverCoordinate, moveCoordinate))
						{
							return true;
						}
						continue;
					}
					if (!actorModel.IsMultiCell)
					{
						if (IsGridCellVisible(coverCoordinate, actorModel.GridCoordinate) && !IsInCover(coverCoordinate, actorModel.GridCoordinate))
						{
							return true;
						}
						continue;
					}
					List<GridCoordinate> occupiedCells = actorModel.GetOccupiedCells();
					for (int j = 0; j < occupiedCells.Count; j++)
					{
						if (IsGridCellVisible(coverCoordinate, occupiedCells[j]) && !IsInCover(coverCoordinate, occupiedCells[j]))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool CoordinateCoversDirection(GridCoordinate coordinate, CoverDirection direction)
		{
			if (direction == CoverDirection.None || !coordinate.IsValid)
			{
				return false;
			}
			int coveredDirections = GetCoveredDirections(coordinate);
			if (coveredDirections == 0)
			{
				return false;
			}
			return (coveredDirections & (1 << (int)direction)) != 0;
		}

		public int GetActiveWalkerCount()
		{
			int num = 0;
			List<ActorModel> factionActors = GetFactionActors(Faction.Walker);
			for (int i = 0; i < factionActors.Count; i++)
			{
				if (!factionActors[i].IsDead)
				{
					num++;
				}
			}
			return num;
		}

		public bool SpawnCivilian(GridCoordinate spawnPointCoordinate, int actorTag, string actorClassID = null, string actorID = null)
		{
			return CreateActor(spawnPointCoordinate, Faction.Civilian, GetRandomWalkerLevel(), actorTag, null, null, -1, (actorClassID != null) ? actorClassID : "DefaultScout", (actorID != null) ? actorID : "DefaultScout") != null;
		}

		public void ActivateDormants()
		{
			foreach (ActorModel factionActor in GetFactionActors(Faction.Dormant))
			{
				if (factionActor.AIController is DormantController dormantController)
				{
					dormantController.ForceWakeUp = true;
				}
			}
		}

		public void SetWalkersAlertness(AIAlertness alertness)
		{
			foreach (ActorModel factionActor in GetFactionActors(Faction.Walker))
			{
				AIController aIController = factionActor.AIController;
				if (aIController != null && aIController.AIDataModel.Alertness < alertness)
				{
					aIController.AIDataModel.Alertness = alertness;
				}
			}
			foreach (ActorSpawnPointModel model in GetModels<ActorSpawnPointModel>())
			{
				model.Alertness = alertness;
			}
		}

		public void SetPvPFlagCollected()
		{
			PvPCollectedFlagsCount++;
			if (IsPvPFlagCollected)
			{
				int survivorsIncapacitated = 0;
				int survivorsInExits = 0;
				GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
				NotifyChange("PvPMissonObjectiveCompleted", GetPvpResult(survivorsIncapacitated, Survivors.Count));
				CurrentMissionObjective.NotifyChange("Status", false);
			}
		}

		public void SetPvPLootCollected()
		{
			PvPCollectedLootsCount++;
			if (IsPvPLootCollected)
			{
				int survivorsIncapacitated = 0;
				int survivorsInExits = 0;
				GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
				NotifyChange("PvPMissonObjectiveCompleted", GetPvpResult(survivorsIncapacitated, Survivors.Count));
				CurrentMissionObjective.NotifyChange("Status", false);
			}
		}

		public void SetGuildBattlePVPSurvivorKilled(ActorModel actor)
		{
			if (actor is SurvivorModel survivorModel && !GuildBattlePVPSurvivorsKilledIndices.Contains(survivorModel.GuildBattlePvPSurvivorIndex))
			{
				GuildBattlePVPSurvivorsKilledIndices.Add(survivorModel.GuildBattlePvPSurvivorIndex);
			}
		}

		public void SetWorldBossDefenderKilled(ActorModel actor)
		{
			if (actor is SurvivorModel)
			{
				WorldBossModelManager worldBossModelManager = ((base.manager.Player != null) ? base.manager.Player.WorldBossModelManager : null);
				if (worldBossModelManager != null && worldBossModelManager.IsAttackTargetActive)
				{
					worldBossModelManager.AttackTarget.AddKilledDefenders(1);
				}
			}
		}

		public void SetPvPDefenderKilled(ActorModel actor)
		{
			PvPDefendersKilledCount++;
			if (actor is SurvivorModel survivorModel)
			{
				PVPKilledDefenderIndices.Add(survivorModel.PvPDefenderIndex);
			}
			if (Raiders != null && Raiders.Count == 0)
			{
				int survivorsIncapacitated = 0;
				int survivorsInExits = 0;
				GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
				NotifyChange("PvPMissonObjectiveCompleted", GetPvpResult(survivorsIncapacitated, Survivors.Count));
				CurrentMissionObjective.NotifyChange("Status", false);
			}
		}

		public void ActivateMaxTurnTimer()
		{
			if (TurnTimerActivationTurn >= 0)
			{
				return;
			}
			TurnTimerActivationTurn = TurnManager.TurnCount;
			if (HasPvPRules)
			{
				List<ActorModel> factionActors = GetFactionActors(Faction.Raider);
				for (int i = 0; i < factionActors.Count; i++)
				{
					ActorModel actorModel = factionActors[i];
					actorModel.AIController.Enabled = true;
					actorModel.AIController.AIDataModel.Alertness = AIAlertness.Idle;
					actorModel.HadActionPointsAtEndOfTurn = false;
				}
			}
			NotifyChange("TurnTimerActivated");
		}

		public int RollCombatDice(RollDiceType rollType, int max)
		{
			int num = base.manager.Player.PlayerRandom.Next(max + 1);
			if (base.manager.CurrentCommandLogEntry != null)
			{
				base.manager.CurrentCommandLogEntry.RollDice(num, max, rollType);
			}
			return num;
		}

		public int RollCombatDiceFromRange(RollDiceType rollType, int min, int max)
		{
			int randomInRange = base.manager.Player.PlayerRandom.GetRandomInRange(min, max);
			if (base.manager.CurrentCommandLogEntry != null)
			{
				base.manager.CurrentCommandLogEntry.RollDice(randomInRange, min, max, rollType);
			}
			return randomInRange;
		}

		public ActorModel CreateActor(GridCoordinate coordinate, Faction faction, int level, int actorTag, string weaponID, string armorID, int equipmentRarityLevel = -1, string actorClassID = null, string actorID = null, ActorGender gender = ActorGender.NotSpecified, WalkerVisualization walkerVisualVariation = WalkerVisualization.Normal, RaiderVisualization raiderVisualVariation = RaiderVisualization.Normal)
		{
			if (faction == Faction.Walker || faction == Faction.Dormant || faction == Faction.Environmental)
			{
				actorID = actorClassID;
			}
			if (actorID == null || actorClassID == null)
			{
				return null;
			}
			ActorDefinition actorDefinition = base.manager.GameEconomyData.GetActorDefinition(actorID);
			ActorModel actorModel;
			if (faction == Faction.Raider && actorDefinition != null && actorDefinition.BossType == BossType.BossTank)
			{
				TankActorModel tankActorModel = new TankActorModel();
				tankActorModel.SetFootprint(ActorFootprint.CreateFromActorDefinition(actorDefinition));
				tankActorModel.Facing = actorDefinition?.InitialFacingDirection ?? FacingDirection.North;
				actorModel = tankActorModel;
			}
			else
			{
				actorModel = ActorModel.Create(faction);
			}
			actorModel.ActorDefinitionID = actorID;
			actorModel.ActorTag = actorTag;
			actorModel.Faction = faction;
			actorModel.GridCoordinate = coordinate;
			actorModel.Level = level;
			actorModel.SetManager(base.manager);
			actorModel.Initialize();
			actorModel.CharacterPrefab = actorModel.Definition.VisualAsset;
			actorModel.OutfitDefinitionID = actorModel.Definition.OutfitDefinitionID;
			switch (faction)
			{
			case Faction.Walker:
			case Faction.Dormant:
			case Faction.Environmental:
			{
				List<EquipmentSetupData> initialEquipmentsData3 = actorModel.Definition.InitialEquipmentsData;
				if (initialEquipmentsData3 != null && initialEquipmentsData3.Count > 0)
				{
					foreach (EquipmentSetupData item in initialEquipmentsData3)
					{
						EquipmentItemModel equipmentItem5 = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(item.ID, item.RarityLevel, actorModel.Level);
						actorModel.Equip(equipmentItem5);
					}
				}
				if (actorModel is WalkerModel)
				{
					(actorModel as WalkerModel).VisualVariation = walkerVisualVariation;
				}
				break;
			}
			case Faction.Lure:
			{
				TimedEffect timedEffect = new TimedEffect(TimedEffectType.Lure, level, 0, Faction.Survivor);
				timedEffect.SetManager(base.manager);
				timedEffect.Initialize();
				actorModel.StartTimedEffect(timedEffect);
				break;
			}
			case Faction.Civilian:
			{
				if (gender != ActorGender.NotSpecified)
				{
					actorModel.Gender = gender;
				}
				else if (actorModel.Definition.Gender == ActorGender.NotSpecified)
				{
					actorModel.Gender = ((base.manager.Player.PlayerRandom.GetRandomInRange(0, 1) != 0) ? ActorGender.Female : ActorGender.Male);
				}
				else
				{
					actorModel.Gender = actorModel.Definition.Gender;
				}
				if (!actorModel.Definition.ID.ToLower().Contains("unique"))
				{
					(actorModel as CivilianModel).DisplayName = base.manager.Player.SurvivorContainer.GetRandomSurvivorName(gender, base.manager.Player.PlayerRandom);
				}
				List<EquipmentSetupData> initialEquipmentsData2 = actorModel.Definition.InitialEquipmentsData;
				if (initialEquipmentsData2 == null || initialEquipmentsData2.Count <= 0)
				{
					break;
				}
				foreach (EquipmentSetupData item2 in initialEquipmentsData2)
				{
					EquipmentItemModel equipmentItem4 = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(item2.ID, item2.RarityLevel, actorModel.Level);
					actorModel.Equip(equipmentItem4);
				}
				break;
			}
			case Faction.Raider:
				if (gender != ActorGender.NotSpecified)
				{
					actorModel.Gender = gender;
				}
				else if (actorModel.Definition.Gender == ActorGender.NotSpecified)
				{
					actorModel.Gender = ((base.manager.Player.PlayerRandom.GetRandomInRange(0, 1) != 0) ? ActorGender.Female : ActorGender.Male);
				}
				else
				{
					actorModel.Gender = actorModel.Definition.Gender;
				}
				if (!actorModel.Definition.ID.ToLower().Contains("unique"))
				{
					(actorModel as RaiderModel).DisplayName = base.manager.Player.SurvivorContainer.GetRandomSurvivorName(gender, base.manager.Player.PlayerRandom);
				}
				if (!string.IsNullOrEmpty(weaponID) && !string.IsNullOrEmpty(armorID))
				{
					EquipmentItemModel equipmentItem = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(weaponID, equipmentRarityLevel, actorModel.Level);
					actorModel.Equip(equipmentItem);
					EquipmentItemModel equipmentItem2 = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(armorID, equipmentRarityLevel, actorModel.Level);
					actorModel.Equip(equipmentItem2);
				}
				else
				{
					List<EquipmentSetupData> initialEquipmentsData = actorModel.Definition.InitialEquipmentsData;
					if (initialEquipmentsData != null && initialEquipmentsData.Count > 0)
					{
						foreach (EquipmentSetupData item3 in initialEquipmentsData)
						{
							EquipmentItemModel equipmentItem3 = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(item3.ID, item3.RarityLevel, actorModel.Level);
							actorModel.Equip(equipmentItem3);
						}
					}
				}
				if (actorModel is RaiderModel)
				{
					(actorModel as RaiderModel).VisualVariation = raiderVisualVariation;
				}
				break;
			}
			actorModel.Start();
			RegisterActor(actorModel);
			return actorModel;
		}

		public ActorModel CreateActor(GridCoordinate coordinate, Faction faction, int level, int actorTag, string actorClassID = null, string actorID = null, ActorGender gender = ActorGender.NotSpecified, WalkerVisualization walkerVisualVariation = WalkerVisualization.Normal, RaiderVisualization raiderVisualVariation = RaiderVisualization.Normal)
		{
			return CreateActor(coordinate, faction, level, actorTag, null, null, -1, actorClassID, actorID, gender, walkerVisualVariation, raiderVisualVariation);
		}

		private AbilityModel CreateSurvivorAbility(string Identifier, int maxUses)
		{
			AbilityModel abilityModel = new AbilityModel();
			abilityModel.SetManager(base.manager);
			abilityModel.DefinitionID = Identifier;
			abilityModel.TotalUses = 0;
			abilityModel.MaxUses = maxUses;
			return abilityModel;
		}

		private void CreateInitialSurvivors()
		{
			MissionRoster.Clear();
			foreach (SurvivorModel combatSurvivor in base.manager.Player.SurvivorContainer.CombatSurvivors)
			{
				MissionRoster.Add(combatSurvivor);
				combatSurvivor.ResetMissionSpecifcStatistics();
			}
			if (!base.manager.Player.Tutorial.StaticTutorialComplete && CurrentMissionTextID == "S01E01M04AlongTheTracks")
			{
				int num = 1;
				for (int i = 0; i < MissionRoster.Count; i++)
				{
					if (MissionRoster[i].SurvivorClass == SurvivorClass.Bruiser)
					{
						num = i;
					}
				}
				if (num != 1)
				{
					SurvivorModel value = MissionRoster[1];
					MissionRoster[1] = MissionRoster[num];
					MissionRoster[num] = value;
				}
			}
			List<CombatStartLocationModel> startLocations = GetStartLocations();
			if (startLocations.Count <= 0)
			{
				return;
			}
			GridModel gridModel = base.manager.GridModel;
			int num2 = Math.Min(MissionRoster.Count, startLocations.Count);
			List<CombatStartLocationModel> list = new List<CombatStartLocationModel>();
			for (int j = 0; j < startLocations.Count; j++)
			{
				GridCoordinate location = startLocations[j].Location;
				if (gridModel.IsCoordinateValid(location) && !IsBlocked(location) && !list.Contains(startLocations[j]))
				{
					list.Add(startLocations[j]);
				}
			}
			for (int k = 0; k < startLocations.Count; k++)
			{
				GridCoordinate location2 = startLocations[k].Location;
				for (int l = 0; l < 8; l++)
				{
					if (list.Count >= num2)
					{
						break;
					}
					GridCoordinate coordinateNeighbor = gridModel.GetCoordinateNeighbor(location2, l);
					if (gridModel.IsCoordinateValid(coordinateNeighbor) && !IsBlocked(coordinateNeighbor) && !list.Contains(startLocations[k]))
					{
						list.Add(startLocations[k]);
					}
				}
			}
			list.StableSort((CombatStartLocationModel a, CombatStartLocationModel b) => a.Order.CompareTo(b.Order));
			for (int num3 = 0; num3 < num2; num3++)
			{
				SurvivorModel survivorModel = MissionRoster[num3];
				survivorModel.GridCoordinate = list[num3].Location;
				survivorModel.ActorTag = list[num3].ActorTagHash;

				if (OfflineManager.IsLoadDataManager && StartGWBattle.Instance.IsAIForSurvivors)
				{
					survivorModel.AIController.Enabled = true;
					survivorModel.AIDataModel.Alertness = AIAlertness.Wandering;
					survivorModel.AIDataModel.Mode = AIMode.Defending;
				}

				survivorModel.SetupForCombat(this);
				RegisterActor(survivorModel);
			}
			while (MissionRoster.Count > num2)
			{
				MissionRoster.RemoveAt(MissionRoster.Count - 1);
			}
		}

		public void AddExtraCombatSurvivor(SurvivorModel survivor, GridCoordinate coordinate)
		{
			ExtraSurvivors.Add(survivor);
			MissionRoster.Add(survivor);
			survivor.GridCoordinate = coordinate;
			if (OfflineManager.IsLoadDataManager && StartGWBattle.Instance.IsAIForSurvivors)
			{
				survivor.AIController.Enabled = true;
			}
			survivor.SetupForCombat(this);
			RegisterActor(survivor);
			UpdateOccupiers();
		}

		public void AddExtraCombatSurvivor(SurvivorModel survivor, GridCoordinate coordinate, int index)
		{
			ExtraSurvivors.Add(survivor);
			MissionRoster.Add(survivor);
			survivor.GridCoordinate = coordinate;
			if (OfflineManager.IsLoadDataManager && StartGWBattle.Instance.IsAIForSurvivors)
			{
				survivor.AIController.Enabled = true;
			}
			survivor.SetupForCombat(this);
			RegisterSurvivorAtIndex(survivor, index);
			UpdateOccupiers();
		}

		public void ChangeActorFaction(ActorModel actor, Faction faction)
		{
			if (actor.Faction != faction)
			{
				UnregisterActor(actor);
				actor.ChangeFaction(faction);
				RegisterActor(actor);
				UpdateOccupiers();
			}
		}

		private CasualtyReport ResolveCasualties()
		{
			CasualtyReport casualtyReport = new CasualtyReport();
			bool isSurvivalMission = IsSurvivalMission;
			foreach (SurvivorModel item in MissionRoster)
			{
				if (base.manager.Player.SurvivorContainer.Survivors.Contains(item))
				{
					if (isSurvivalMission && item.Faction == Faction.Lure)
					{
						ChangeActorFaction(item, Faction.Survivor);
					}
					ResolveCasualty(item, casualtyReport);
				}
				item.FinishTimedEffect(interrupted: true);
			}
			return casualtyReport;
		}

		public void ResolveCasualty(SurvivorModel survivor, CasualtyReport casualtyReport)
		{
			PlayerModel player = base.manager.Player;
			bool flag = false;
			if (!IsDeadly)
			{
				if (survivor.IsDead)
				{
					survivor.SetFaction(Faction.Survivor);
					survivor.SetHitPoints(1, survivor.MaxHitPoints);
					survivor.MinHitpoints = 1;
					survivor.CombatEndCondition = CombatEndCondition.Incapacitated;
				}
				else if (survivor.IsStruggling)
				{
					survivor.CombatEndCondition = CombatEndCondition.Incapacitated;
				}
			}
			else if (survivor.IsDead || survivor.IsStruggling)
			{
				player.SurvivorContainer.SurvivorDied(survivor);
				flag = true;
				survivor.SetHitPoints(0, survivor.MaxHitPoints);
				survivor.MinHitpoints = 0;
				survivor.CombatEndCondition = CombatEndCondition.Dead;
				MissionStatistics.AddSurvivorDied();
			}
			if (!flag)
			{
				InjuryType injuryType;
				InjuryType previousCombatInjuryType;
				if (HasPvPRules && !IsPVPMission)
				{
					injuryType = InjuryType.None;
					previousCombatInjuryType = injuryType;
				}
				else if (IsSurvivalMission || IsEndlessBattleMission)
				{
					if (survivor.CombatEndCondition == CombatEndCondition.Incapacitated)
					{
						injuryType = InjuryType.None;
						previousCombatInjuryType = (survivor.PreviousCombatInjuryType = InjuryType.OutOfAction);
					}
					else
					{
						injuryType = InjuryType.None;
						previousCombatInjuryType = InjuryType.None;
					}
				}
				else
				{
					injuryType = survivor.GetInjuryType();
					previousCombatInjuryType = injuryType;
				}
				if (injuryType != InjuryType.None && ((PlayerModel)base.manager.GetPlayer()).Camp != null)
				{
					if (survivor.CombatEndCondition == CombatEndCondition.None)
					{
						survivor.CombatEndCondition = CombatEndCondition.Injured;
					}
					if (((PlayerModel)base.manager.GetPlayer()).Camp.GetBuilding("MedicTent") is MedicTentModel medicTentModel)
					{
						FixedPoint healingTimeModifier = GetHealingTimeModifier();
						survivor.InjuryType = injuryType;
						survivor.PreviousCombatInjuryType = previousCombatInjuryType;
						int missionLevel = (IsWorldBossMission ? player.Level : GetMissionLevel());
						medicTentModel.NewSurvivorInjured(survivor, missionLevel, healingTimeModifier);
					}
				}
				else if (survivor.CombatEndCondition == CombatEndCondition.None)
				{
					survivor.CombatEndCondition = CombatEndCondition.NotInjured;
				}
			}
			if (casualtyReport == null)
			{
				return;
			}
			if (flag)
			{
				casualtyReport.CasualtyCount++;
				casualtyReport.NoDamage = false;
				casualtyReport.NoStruggle = false;
				return;
			}
			if (survivor.InjuryType != InjuryType.None)
			{
				casualtyReport.NoDamage = false;
			}
			if (survivor.StrugglesLeft == 0)
			{
				casualtyReport.NoStruggle = false;
			}
		}

		private int GetMissionLevel()
		{
			int num = 1;
			if (IsPVPMission)
			{
				return ((PlayerModel)base.manager.GetPlayer()).Level;
			}
			return base.manager.Player.SelectedMissionDifficulty;
		}

		public bool GetPvPDefenderKilled(int defenderIndex)
		{
			return PVPKilledDefenderIndices.Contains(defenderIndex);
		}

		public FixedPoint GetHealingTimeModifier()
		{
			MapCategory category = MapCategory.None;
			if (base.manager.Player.GetAttackTargetMissionModel() is GuildBattleMapMissionModel || IsWorldBossMission)
			{
				return base.manager.GameEconomyData.ConfigData.GetHealingTimeModifier(MapCategory.GuildBattle);
			}
			if (IsPVPMission)
			{
				category = MapCategory.Outpost;
			}
			else
			{
				MapContainerModel mapContainerModel = base.manager.Player.MapContainerModel;
				if (mapContainerModel.AttackTargetMissionModel != null)
				{
					category = ((mapContainerModel.AttackTargetMissionModel.MissionSpawnPointGroup != null) ? mapContainerModel.AttackTargetMissionModel.MissionSpawnPointGroup.Category : MapCategory.None);
				}
			}
			return base.manager.GameEconomyData.ConfigData.GetHealingTimeModifier(category);
		}

		public void ClearExtraSurvivors()
		{
			ExtraSurvivors.Clear();
		}

		public bool HasCivilianDied()
		{
			return false;
		}

		public void SetSuggestedInteractionTarget(ActorModel actor, GridCoordinate coordinate, bool forced = true)
		{
			SuggestedInteractionActor = actor;
			SuggestedInteractionTargetCoordinate = coordinate;
			SuggestedInteractionIsForced = forced;
			NotifyChange("suggestedInteractionTargetChanged");
		}

		public void ClearSuggestedInteractionTarget()
		{
			SuggestedInteractionActor = null;
			SuggestedInteractionTargetCoordinate = GridCoordinate.Invalid;
			NotifyChange("suggestedInteractionTargetChanged");
		}

		public void ForceEndMissionVictory()
		{
			base.manager.Player.LootManager.AvailableKeys = 3;
			if (base.manager.Player.LootManager.LootKeysSources != null)
			{
				base.manager.Player.LootManager.LootKeysSources.Clear();
				for (int i = 0; i < base.manager.Player.LootManager.AvailableKeys; i++)
				{
					base.manager.Player.LootManager.LootKeysSources.Add(LootKeySource.Combat);
				}
			}
			if (HasPvPRules)
			{
				PvPDefendersKilledCount = 3;
				while (Raiders.Count > 0)
				{
					UnregisterActor(Raiders[0], updateVisibility: false);
				}
				if (PvPMissionType == PvPMissionType.FakePVPMultiFlag || PvPMissionType == PvPMissionType.PVPMultiFlag)
				{
					PvPCollectedFlagsCount = 3;
					PvPCollectedLootsCount = 1;
				}
				else if (PvPMissionType == PvPMissionType.FakePVPMultiLoot || PvPMissionType == PvPMissionType.PVPMultiLoot)
				{
					PvPCollectedFlagsCount = 1;
					PvPCollectedLootsCount = 3;
				}
			}
			OnMissionComplete(ECombatResult.Successful);
		}

		public void ForceEndMissionFailure()
		{
			OnMissionComplete(ECombatResult.Failed, casualtiesResolved: false, "ForceEndMissionFailure_ForcedFailure");
		}

		public void ProceedEndCombat()
		{
			CombatRetryChoicePendingState = MissionRetryState.Resolved;
			OnMissionComplete(PendingCombatResult, casualtiesResolved: true, (PendingCombatResult == ECombatResult.Failed) ? ("ProceedEndCombat_" + GetCurrentMissionFailureReason()) : "");
		}

		public bool IsTargetNextToAlly(ActorModel source, ActorModel target)
		{
			SurvivorModel survivorModel = source as SurvivorModel;
			int num = 0;
			while (survivorModel != null && target != null && !target.Definition.IsEnvironmental && num < base.manager.Player.SurvivorContainer.CombatSurvivors.Count)
			{
				SurvivorModel survivorModel2 = base.manager.Player.SurvivorContainer.CombatSurvivors[num];
				if (Grid.AreNeighbors(target.GridCoordinate, survivorModel2.GridCoordinate) && survivorModel2 != survivorModel)
				{
					return true;
				}
				num++;
			}
			return false;
		}

		public bool IsTargetNextToActorWithTrait(ActorModel target, string traitId)
		{
			int num = 0;
			while (target != null && num < base.manager.Player.SurvivorContainer.CombatSurvivors.Count)
			{
				SurvivorModel survivorModel = base.manager.Player.SurvivorContainer.CombatSurvivors[num];
				if (Grid.AreNeighbors(target.GridCoordinate, survivorModel.GridCoordinate) && survivorModel.HasAnyLevelTrait(traitId))
				{
					return true;
				}
				num++;
			}
			return false;
		}

		private void GrantStaticGrindMissionRewards()
		{
			if (!IsPVPMission && LootTag != DropEventDefinition.DropEventTag.None && base.manager != null && base.manager.Player.MapContainerModel.AttackTargetMissionModel != null)
			{
				DropCurrenciesStaticDefinition dropCurrencyStaticDefinition = base.manager.GameEconomyData.GetDropCurrencyStaticDefinition(LootTag, base.manager.Player.MapContainerModel.AttackTargetMissionModel.MissionLevel);
				dropCurrencyStaticDefinition = base.manager.Player.ActivityManager.ModifyActivityDefinition(dropCurrencyStaticDefinition);
				int num = ((dropCurrencyStaticDefinition.MaxSupplies > 0) ? base.manager.Player.PlayerRandom.GetRandomInRange(dropCurrencyStaticDefinition.MinSupplies, dropCurrencyStaticDefinition.MaxSupplies) : 0);
				int num2 = ((dropCurrencyStaticDefinition.MaxSurvivalPoints > 0) ? base.manager.Player.PlayerRandom.GetRandomInRange(dropCurrencyStaticDefinition.MinSurvivalPoints, dropCurrencyStaticDefinition.MaxSurvivalPoints) : 0);
				StaticRewardSuppliesGranted = num;
				StaticRewardSurvivalPointsGranted = num2;
				if (num > 0)
				{
					CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.Supplies);
					currency.Add(num);
					StaticRewardSuppliesGranted = (int)(num * currency.AddMultiplier);
					base.manager.Metrics.PushResource(CurrencyType.Supplies, currency.LastAdded, (num != currency.LastAdded) ? (num - currency.LastAdded) : 0);
				}
				if (num2 > 0)
				{
					CurrencyModel currency2 = base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints);
					currency2.Add(num2);
					StaticRewardSurvivalPointsGranted = (int)(num2 * currency2.AddMultiplier);
					base.manager.Metrics.PushResource(CurrencyType.SurvivalPoints, currency2.LastAdded, (num2 != currency2.LastAdded) ? (num2 - currency2.LastAdded) : 0);
				}
				if (base.manager.Metrics.metricsResourcesData.HasResources())
				{
					base.manager.Metrics.AddFind().AddResources(base.manager.Metrics.metricsResourcesData).AddMission()
						.AddMissionType()
						.AddStaticReward()
						.Send();
				}
				base.manager.Metrics.Reset();
			}
		}

		private bool GrantSeasonTrialMissionRewards(ref Metrics.MetricsResourcesData metricsResourcesData)
		{
			if (base.manager.Player.MapContainerModel.AttackTargetMissionModel != null && base.manager.Player.MapContainerModel.AttackTargetMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && base.manager.Player.MapContainerModel.AttackTargetMissionModel.IsLastInGroup && base.manager.Player.MapContainerModel.AttackTargetMissionModel.CompletionTimes == 4)
			{
				MissionHighlight isFeaturedData = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(base.manager.Player.MapContainerModel.AttackTargetMissionModel.MissionSpawnPointGroupId).IsFeaturedData;
				if (isFeaturedData != null && isFeaturedData.CompletionRewards != null)
				{
					List<RewardCurrency> allRewardCurrencies = isFeaturedData.CompletionRewards.GetAllRewardCurrencies();
					if (allRewardCurrencies != null && allRewardCurrencies.Count == 1)
					{
						SeasonRewardMissionAmount = allRewardCurrencies[0].Amount;
						SeasonRewardMissionCurrency = allRewardCurrencies[0].CurrencyType;
						isFeaturedData.CompletionRewards.Give(base.manager);
						CurrencyModel currency = base.manager.Player.GetCurrency(allRewardCurrencies[0].CurrencyType);
						metricsResourcesData.SetOrAdd(allRewardCurrencies[0].CurrencyType, allRewardCurrencies[0].Amount, (allRewardCurrencies[0].Amount != currency.LastAdded) ? (allRewardCurrencies[0].Amount - currency.LastAdded) : 0);
						return true;
					}
					return false;
				}
			}
			return true;
		}

		private void GrantGuaranteedMissionRewards(ref Metrics.MetricsResourcesData metricsResourcesData)
		{
			if (IsPVPMission || base.manager == null || base.manager.Player.MapContainerModel.AttackTargetMissionModel == null)
			{
				return;
			}
			Rewards storyMissionRewards = base.manager.Player.MapContainerModel.AttackTargetMissionModel.GetStoryMissionRewards();
			StaticRewardStoryMissionCurrencyList = new List<RewardCurrency>();
			bool flag = true;
			if (storyMissionRewards == null)
			{
				return;
			}
			for (int i = 0; i < storyMissionRewards.Count; i++)
			{
				IReward rewardAt = storyMissionRewards.GetRewardAt(i);
				if (rewardAt == null)
				{
					continue;
				}
				object obj = rewardAt.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom });
				if (rewardAt is RewardCurrency)
				{
					RewardCurrency rewardCurrency = rewardAt as RewardCurrency;
					if (flag)
					{
						flag = false;
						StaticRewardStoryMissionCurrency = rewardCurrency.CurrencyType;
						StaticRewardStoryMissionAmount = rewardCurrency.Amount;
					}
					else
					{
						StaticReward2StoryMissionCurrency = rewardCurrency.CurrencyType;
						StaticReward2StoryMissionAmount = rewardCurrency.Amount;
					}
					metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount());
					StaticRewardStoryMissionCurrencyList.Add(rewardCurrency);
				}
				else if (rewardAt is RewardEquipment rewardEquipment)
				{
					StaticRewardStoryMissionEquipment = obj as EquipmentItemModel;
					base.manager.Metrics.AddFind().AddEquipment(StaticRewardStoryMissionEquipment, "Equipment", rewardEquipment?.Amount ?? 1).AddMission()
						.AddMissionType()
						.AddStaticReward()
						.Send();
				}
			}
		}

		private void OnThreatValueChanged(ModelObject model, string changed, object args)
		{
			if (changed == "waveTriggered")
			{
				CheckTraitsForThreatValueChange();
			}
		}

		public long GetWorldBossCombatTimeLeftSeconds(long nowUtcMs)
		{
			if (!(base.manager.Player.GetAttackTargetMissionModel() is WorldBossMissionModel worldBossMissionModel) || worldBossMissionModel.BattleStartUtcMs <= 0 || worldBossMissionModel.TimeLimitMs <= 0)
			{
				return 0L;
			}
			long num = worldBossMissionModel.BattleStartUtcMs + worldBossMissionModel.TimeLimitMs - nowUtcMs;
			if (num <= 0)
			{
				return 0L;
			}
			return (num + 999) / 1000;
		}

		public bool IsWorldBossCombatTimedOut(long nowUtcMs)
		{
			if (base.manager.Player.GetAttackTargetMissionModel() is WorldBossMissionModel worldBossMissionModel)
			{
				return worldBossMissionModel.IsBattleTimedOut(nowUtcMs);
			}
			return false;
		}

		public int GetOrResolveMissionLogicFailTurnLimit()
		{
			if (!missionLogicFailTurnLimitResolved)
			{
				MissionLogicFailTurnLimit = FindMissionLogicFailTurnLimit();
				missionLogicFailTurnLimitResolved = true;
			}
			return MissionLogicFailTurnLimit;
		}

		private int FindMissionLogicFailTurnLimit()
		{
			List<TWDModelObject> models = GetModels<NodeGraph>();
			if (models == null || models.Count == 0)
			{
				return -1;
			}
			int num = -1;
			for (int i = 0; i < models.Count; i++)
			{
				if (!(models[i] is NodeGraph { Nodes: not null } nodeGraph))
				{
					continue;
				}
				for (int j = 0; j < nodeGraph.Nodes.Count; j++)
				{
					if (nodeGraph.Nodes[j] is AtTurnNode { TurnToCheck: >=0 } atTurnNode && DoesNodeReachMissionFailure(nodeGraph, atTurnNode) && (num < 0 || atTurnNode.TurnToCheck < num))
					{
						num = atTurnNode.TurnToCheck;
					}
				}
			}
			return num;
		}

		private static bool DoesNodeReachMissionFailure(NodeGraph graph, NodeBase start)
		{
			if (graph == null || start == null || start.nodeConnections == null)
			{
				return false;
			}
			HashSet<int> hashSet = new HashSet<int>();
			Queue<NodeBase> queue = new Queue<NodeBase>();
			queue.Enqueue(start);
			hashSet.Add(start.guidHash);
			while (queue.Count > 0)
			{
				NodeBase nodeBase = queue.Dequeue();
				if (nodeBase.nodeConnections == null)
				{
					continue;
				}
				for (int i = 0; i < nodeBase.nodeConnections.Count; i++)
				{
					NodeConnection nodeConnection = nodeBase.nodeConnections[i];
					if (nodeConnection != null && graph.GetNode(nodeConnection.TargetGuidHash) is NodeBase nodeBase2)
					{
						if (nodeConnection.InputPinId == "Failure" && nodeBase2 is MissionCompletedNode)
						{
							return true;
						}
						if (hashSet.Add(nodeBase2.guidHash))
						{
							queue.Enqueue(nodeBase2);
						}
					}
				}
			}
			return false;
		}

		public void ApplySurvivalConfig(SurvivalMissionConfig survivalMissionConfig, SurvivalSavedMissionModel savedMissionData)
		{
			if (survivalMissionConfig == null)
			{
				base.Debug.LogError("Null survival mission config given!");
			}
			else
			{
				ApplySurvivalConfigInternal(survivalMissionConfig.ConfigName, survivalMissionConfig, savedMissionData);
			}
		}

		public void ApplySurvivalConfig(string missionConfigName, SurvivalMissionConfig survivalMissionConfig, int survivalMissionObjectiveIndex, int survivalMissionEnemyIndex)
		{
			if (survivalMissionConfig == null)
			{
				base.Debug.LogError("Null survival mission config given!");
			}
			else
			{
				ApplySurvivalConfigInternal(missionConfigName, survivalMissionConfig, null);
			}
		}

		public void ApplySurvivalConfigInternal(string missionConfigName, SurvivalMissionConfig survivalMissionConfig, SurvivalSavedMissionModel savedMissionData)
		{
			if (survivalMissionConfig == null)
			{
				base.Debug.LogError("Null survival mission config given!");
				return;
			}
			this.survivalMissionConfig = survivalMissionConfig;
			SurvivalMissionConfigName = missionConfigName;
			SurvivalMissionConfigType = survivalMissionConfig.MissionType;
			SurvivalMissionConfigMissionOrderInSection = survivalMissionConfig.MissionOrderInSection;
			if (survivalMissionConfig.ThreatFrequency == 0)
			{
				base.Debug.LogError("Survival mission with ThreatFrequency=0 encountered. Use ThreatFrequency=999 instead.");
			}
			InitialTurnCountToWave = survivalMissionConfig.ThreatFrequency;
			InitialThreatLevel = survivalMissionConfig.ThreatStart;
			SurvivalCombatHelper.ApplyOpponentLevel(this);
			SurvivalCombatHelper.ApplySavedEnemyCounts(this, savedMissionData);
			GuildBattleCombatHelper.ApplySavedEnemyCounts(this, savedMissionData);
			SurvivalCombatHelper.ApplySurvivalMissionConfigToPersistentVariables(this, savedMissionData);
		}

		private void CheckTraitsForThreatValueChange()
		{
			for (int i = 0; i < ((Survivors != null) ? Survivors.Count : 0); i++)
			{
				ActorModel actorModel = Survivors[i];
				FixedPoint value = 0.0;
				if (base.manager.Player.AbilityManager.VisitParameter("AbilityModifierIncreaseExtraChargePointChanceAtThreatWave", ref value, actorModel) && value > 0.0)
				{
					PlayerRandomChanceResult playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.GainChargePoint, value);
					if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && actorModel.ChargeMeter != null)
					{
						actorModel.AddChargePoints(1);
						actorModel.NotifyChange("AbilityVisited", new object[2]
						{
							"LeaderBuffBringThemOn",
							playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
				}
			}
		}

		public void AddAttackedTarget(ActorModel source, ActorModel target)
		{
			if (AttackedTargetsThisTurn == null)
			{
				AttackedTargetsThisTurn = new Dictionary<Faction, List<ActorModel>>();
			}
			if (!AttackedTargetsThisTurn.ContainsKey(source.Faction))
			{
				AttackedTargetsThisTurn.Add(source.Faction, new List<ActorModel>());
			}
			if (!AttackedTargetsThisTurn[source.Faction].Contains(target))
			{
				AttackedTargetsThisTurn[source.Faction].Add(target);
			}
			target.OneTurnAttackedTimes += (FixedPoint)1L;
		}

		public bool HasActorBeenAttackedByFaction(ActorModel target, Faction attackerFaction)
		{
			if (AttackedTargetsThisTurn != null && AttackedTargetsThisTurn.ContainsKey(attackerFaction))
			{
				return AttackedTargetsThisTurn[attackerFaction].Contains(target);
			}
			return false;
		}

		public void ClearAttackedTargets(Faction faction)
		{
			if (AttackedTargetsThisTurn != null && AttackedTargetsThisTurn.ContainsKey(faction))
			{
				AttackedTargetsThisTurn[faction].Clear();
			}
		}

		private string CreateIdForAnalytics()
		{
			string hashedId = base.manager.Player.HashedId;
			string text = base.manager.Player.UtcTimeStamp.ToString();
			return ModelHelpers.MD5Sum(CombatStartTime.ToString() + base.ModelId + hashedId + text);
		}

		public bool IsConsumableInCooldown(EquipmentModel.ConsumableType consumableType)
		{
			return GetCooldown(consumableType) > 0;
		}

		public int GetCooldown(EquipmentModel.ConsumableType consumableType)
		{
			AbilityDefinition abilityDefinition;
			int lastTurnForConsumable;
			switch (consumableType)
			{
			case EquipmentModel.ConsumableType.Grenade:
				abilityDefinition = base.manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityMolotovConsumable");
				lastTurnForConsumable = base.manager.CombatModel.MissionStatistics.GetLastTurnForConsumable("Weapon_Throwable_Grenade_Consumable");
				break;
			case EquipmentModel.ConsumableType.MedKit:
				abilityDefinition = base.manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityMedkitConsumable");
				lastTurnForConsumable = base.manager.CombatModel.MissionStatistics.GetLastTurnForConsumable("Medkit_Consumable");
				break;
			case EquipmentModel.ConsumableType.Flare:
				abilityDefinition = base.manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityFlareConsumable");
				lastTurnForConsumable = base.manager.CombatModel.MissionStatistics.GetLastTurnForConsumable("Weapon_Throwable_Flare_Consumable");
				break;
			case EquipmentModel.ConsumableType.BlastGrenade:
				abilityDefinition = base.manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityPushGrenadeConsumable");
				lastTurnForConsumable = base.manager.CombatModel.MissionStatistics.GetLastTurnForConsumable("Weapon_Throwable_Blast_Grenade_Consumable");
				break;
			default:
				abilityDefinition = base.manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityGoreConsumable");
				lastTurnForConsumable = base.manager.CombatModel.MissionStatistics.GetLastTurnForConsumable("Gore_Consumable");
				break;
			}
			int num = 0;
			int num2 = 0;
			IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
			if (challengeDebuffProvider != null)
			{
				List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
				num += (int)ChallengeDebufHelps.GetDebufTotalSecondParam(challengeDebuffs, ChallengeDebuffType.ToolCooldown);
				num2 += (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.ToolCooldown);
				if (challengeDebuffProvider.IsInApocalyptiWeeklyChallenge)
				{
					num -= (int)base.manager.Player.ApocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.ToolCoolDown);
				}
			}
			int turnCount = base.manager.CombatModel.TurnManager.TurnCount;
			if (abilityDefinition.InitialCooldown + num2 > turnCount)
			{
				return abilityDefinition.InitialCooldown + num2 - turnCount;
			}
			if (lastTurnForConsumable == -1)
			{
				return 0;
			}
			if (lastTurnForConsumable + abilityDefinition.CooldownAfterUse + num > turnCount)
			{
				return lastTurnForConsumable + abilityDefinition.CooldownAfterUse + num - turnCount;
			}
			return 0;
		}

		private void IsLeaderRemovedFromCombat(ActorModel actorModel)
		{
			if (actorModel.LeaderTraitModifiers == null || base.manager?.Player?.AbilityManager == null)
			{
				return;
			}
			for (int i = 0; i < actorModel.LeaderTraitModifiers.Count; i++)
			{
				ModelModifier modifier = actorModel.LeaderTraitModifiers[i];
				if (base.manager.Player.AbilityManager.HasFactionModifier(actorModel.Faction, modifier))
				{
					ClearRaiderLeaderTraitsPostCombat = true;
					break;
				}
			}
		}

		private bool ShouldOfferRetry(ECombatResult combatResult)
		{
			if (combatResult != ECombatResult.Successful && base.manager.Player.GetAttackTargetMissionModel() is GuildBattleMapMissionModel guildBattleMapMissionModel)
			{
				long utcTimeStamp = base.manager.Player.UtcTimeStamp + 5000;
				if (base.manager.Player.GuildBattlePlayer.IsCurrentGuildBattle() && base.manager.Player.GuildWarModel.CurrentBattle.IsOngoing(utcTimeStamp) && base.manager.Player.GuildBattlePlayer.CanRetryMission() && guildBattleMapMissionModel.GetRetryGvGMissionCashier(base.manager).CanAfford())
				{
					return true;
				}
			}
			return false;
		}

		public void RemoveDeBuffMarksFromActors(ActorModel actor)
		{
			foreach (ActorModel allActor in GetAllActors())
			{
				if (allActor != null && allActor.HasAnyLevelTrait("DebuffMarkEnemy"))
				{
					TraitEntry traitAnyLevel = allActor.TraitContainer.GetTraitAnyLevel("DebuffMarkEnemy");
					if (traitAnyLevel != null && traitAnyLevel.Tag == actor.Faction.ToString())
					{
						allActor.RemoveAnyLevelTrait("DebuffMarkEnemy");
					}
				}
			}
		}

		public void InitializeSupportManager()
		{
			SupportManager = new CombatSupportManager();
			SupportManager.SetManager(base.manager);
			SupportManager.InitializeEquippedSupports();
		}

		private void EarnBattleCurrencyFromKill(ActorModel killedActor)
		{
			int num = base.manager.Player.BattlePass.AttemptToEarnCurrencyThroughKill(1);
			MissionStatistics.AddBattlePassCurrency(num);
			if (num > 0)
			{
				NotifyChange("BattlePassCurrencyEarned", new object[2] { num, killedActor });
			}
		}

		public void AddDisorientedModel(ActorModel model)
		{
			if (!DisorientedModels.Contains(model))
			{
				DisorientedModels.Add(model);
			}
		}

		public void RemoveDisorientedModel(ActorModel model)
		{
			if (DisorientedModels.Contains(model))
			{
				DisorientedModels.Remove(model);
			}
		}

		public bool IsDisorientedModel(ActorModel model)
		{
			if (DisorientedModels.Contains(model))
			{
				return true;
			}
			return false;
		}

		public void ClearDisorientedModel()
		{
			DisorientedModels.Clear();
		}

		public void AddPhonePortraitKilledNum()
		{
			if (!bounsPhonePortraitTurnKilledNum.ContainsKey(TurnManager.TurnCount))
			{
				bounsPhonePortraitTurnKilledNum.Add(TurnManager.TurnCount, 1);
			}
			else
			{
				bounsPhonePortraitTurnKilledNum[TurnManager.TurnCount]++;
			}
		}

		public int getPhonePortraitKilledNum()
		{
			if (!bounsPhonePortraitTurnKilledNum.ContainsKey(TurnManager.TurnCount))
			{
				return 0;
			}
			return bounsPhonePortraitTurnKilledNum[TurnManager.TurnCount];
		}

		public void CheckAndClearExpiredPhonePortraitKilledNum()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, int> item in bounsPhonePortraitTurnKilledNum)
			{
				if (TurnManager.TurnCount > item.Key)
				{
					list.Add(item.Key);
				}
			}
			foreach (int item2 in list)
			{
				bounsPhonePortraitTurnKilledNum.Remove(item2);
			}
		}

		public void AddAssistAttackRecord(ActorModel source)
		{
			AssistAttackContainer.AddRecord(source, TurnManager.TurnCount);
		}

		public bool CanAssistAttackTargetThisTurn(ActorModel source)
		{
			return AssistAttackContainer.CanAssistAttackTargetThisTurn(source, TurnManager.TurnCount);
		}

		public List<ActorModel> GetOneGridWalkRaiderModels(ActorModel source)
		{
			List<ActorModel> list = new List<ActorModel>();
			List<ActorModel> actorsInRange = base.manager.CombatModel.GetActorsInRange(source.GridCoordinate, 1);
			for (int i = 0; i < actorsInRange.Count; i++)
			{
				ActorModel actorModel = actorsInRange[i];
				if ((actorModel.IsWalker || actorModel.IsRaider) && source != actorModel)
				{
					list.Add(actorModel);
				}
			}
			return list;
		}

		private void FinishRedactTimedEffect()
		{
			if (RedactTimedEffect == null)
			{
				NotifyChange("redactEndEvent");
				return;
			}
			RedactTimedEffect = null;
			UpdateModelObjects();
			NotifyChange("redactEndEvent");
		}

		public bool StartRedactTimedEffect(ActorModel instigator)
		{
			bool result = false;
			if (instigator == null)
			{
				return false;
			}
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffRedactReduceWalkerHpRatio", ref value, instigator);
			FixedPoint value2 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffRedactIncreaseHumanDamage", ref value2, instigator);
			FixedPoint value3 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffRedactMaxLayers", ref value3, instigator);
			FixedPoint value4 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffRedactReduceWalkerHpChance", ref value4, instigator);
			if (RedactTimedEffect != null)
			{
				int num = RedactTimedEffect.Layers + 1;
				if (num <= value3)
				{
					RedactTimedEffect.Layers = Math.Min(num, (int)value3);
					RedactTimedEffect.ReducedHpRatio += (int)value;
					RedactTimedEffect.IncreaseDamageRatio += (int)value2;
					result = true;
				}
			}
			else
			{
				RedactTimedEffect = new RedactTimedEffect((int)value2, value4, (int)value);
				result = true;
			}
			NotifyChange("redactEndEvent");
			return result;
		}

		public void SetNewSurvivalGame(ActorModel leader, ActorModel enemy)
		{
			if (enemy.IsSurvivalGameEnemy())
			{
				SurvivalGameModel survivalGameModel = SurvivalGameModelList.Find((SurvivalGameModel t) => t.EnemyActor == enemy);
				survivalGameModel.End();
				SurvivalGameModelList.Remove(survivalGameModel);
			}
			SurvivalGameModel survivalGameModel2 = new SurvivalGameModel();
			survivalGameModel2.SetManager(base.manager);
			survivalGameModel2.Initialize();
			survivalGameModel2.SetNewSelected(leader, enemy);
			SurvivalGameModelList.Add(survivalGameModel2);
		}

		private void UpdateSurvivalGameList()
		{
			if (SurvivalGameModelList.Count > 0)
			{
				for (int i = 0; i < SurvivalGameModelList.Count; i++)
				{
					SurvivalGameModelList[i].UpdateData();
				}
			}
			SurvivalGameModelList.RemoveAll((SurvivalGameModel t) => t.LeftCount <= 0);
			SurvivalGameModelList.RemoveAll((SurvivalGameModel t) => t.EnemyActor.IsDead);
			SurvivalGameModelList.RemoveAll((SurvivalGameModel t) => t.LeaderActor.IsDead);
		}

		public ActorModel GetActorByActorDefinitionID(string actorDefinitionID)
		{
			if (string.IsNullOrEmpty(actorDefinitionID))
			{
				return null;
			}
			List<ActorModel> allActors = GetAllActors();
			for (int i = 0; i < allActors.Count; i++)
			{
				if (allActors[i] != null && allActors[i].ActorDefinitionID == actorDefinitionID)
				{
					return allActors[i];
				}
			}
			return null;
		}

		public GuardianVowBinding GetGuardianVowBindingByGuardian(ActorModel guardian)
		{
			if (guardian == null || GuardianVowBindings == null)
			{
				return null;
			}
			for (int i = 0; i < GuardianVowBindings.Count; i++)
			{
				if (GuardianVowBindings[i].GuardianActorDefinitionID == guardian.ActorDefinitionID)
				{
					return GuardianVowBindings[i];
				}
			}
			return null;
		}

		public GuardianVowBinding GetGuardianVowBindingBySovereign(ActorModel sovereign)
		{
			if (sovereign == null || GuardianVowBindings == null)
			{
				return null;
			}
			for (int i = 0; i < GuardianVowBindings.Count; i++)
			{
				if (GuardianVowBindings[i].SovereignActorDefinitionID == sovereign.ActorDefinitionID)
				{
					return GuardianVowBindings[i];
				}
			}
			return null;
		}

		public GuardianVowBinding GetGuardianVowBindingByActor(ActorModel actor)
		{
			if (actor == null || GuardianVowBindings == null)
			{
				return null;
			}
			for (int i = 0; i < GuardianVowBindings.Count; i++)
			{
				GuardianVowBinding guardianVowBinding = GuardianVowBindings[i];
				if (guardianVowBinding.GuardianActorDefinitionID == actor.ActorDefinitionID || guardianVowBinding.SovereignActorDefinitionID == actor.ActorDefinitionID)
				{
					return guardianVowBinding;
				}
			}
			return null;
		}

		public GuardianVowBinding BindGuardianVow(ActorModel guardian, ActorModel sovereign, int durationTurns, int chargeAttackMaxTimes, FixedPoint chargeGain)
		{
			if (guardian == null || sovereign == null)
			{
				return null;
			}
			if (GuardianVowBindings == null)
			{
				GuardianVowBindings = new List<GuardianVowBinding>();
			}
			HashSet<ActorModel> hashSet = new HashSet<ActorModel>();
			for (int num = GuardianVowBindings.Count - 1; num >= 0; num--)
			{
				GuardianVowBinding guardianVowBinding = GuardianVowBindings[num];
				ActorModel actorByActorDefinitionID = GetActorByActorDefinitionID(guardianVowBinding.GuardianActorDefinitionID);
				ActorModel actorByActorDefinitionID2 = GetActorByActorDefinitionID(guardianVowBinding.SovereignActorDefinitionID);
				GuardianVowBindings.RemoveAt(num);
				if (actorByActorDefinitionID != null && actorByActorDefinitionID != guardian && actorByActorDefinitionID != sovereign)
				{
					hashSet.Add(actorByActorDefinitionID);
				}
				if (actorByActorDefinitionID2 != null && actorByActorDefinitionID2 != guardian && actorByActorDefinitionID2 != sovereign)
				{
					hashSet.Add(actorByActorDefinitionID2);
				}
			}
			GuardianVowBinding guardianVowBinding2 = new GuardianVowBinding
			{
				GuardianActorDefinitionID = guardian.ActorDefinitionID,
				SovereignActorDefinitionID = sovereign.ActorDefinitionID,
				LeftTurns = durationTurns,
				PursuitTriggeredCount = 0,
				ChargeAttackMaxTimes = chargeAttackMaxTimes,
				ChargeGain = chargeGain
			};
			GuardianVowBindings.Add(guardianVowBinding2);
			foreach (ActorModel item in hashSet)
			{
				item.NotifyChange("UpdateGuardianVowEvent");
			}
			guardian.NotifyChange("UpdateGuardianVowEvent");
			sovereign.NotifyChange("UpdateGuardianVowEvent");
			return guardianVowBinding2;
		}

		private void OnFactionChanged_TickGuardianVowBindings(Faction currentFaction, Faction newFaction)
		{
			if (newFaction != Faction.Survivor || GuardianVowBindings == null || GuardianVowBindings.Count == 0)
			{
				return;
			}
			for (int num = GuardianVowBindings.Count - 1; num >= 0; num--)
			{
				GuardianVowBinding guardianVowBinding = GuardianVowBindings[num];
				ActorModel actorByActorDefinitionID = GetActorByActorDefinitionID(guardianVowBinding.GuardianActorDefinitionID);
				ActorModel actorByActorDefinitionID2 = GetActorByActorDefinitionID(guardianVowBinding.SovereignActorDefinitionID);
				guardianVowBinding.LeftTurns--;
				if (guardianVowBinding.LeftTurns <= 0)
				{
					GuardianVowBindings.RemoveAt(num);
				}
				else
				{
					guardianVowBinding.ChargeRefreshUsedThisTurn = 0;
					if (guardianVowBinding.ChargeGain > 0L && actorByActorDefinitionID2 != null && !actorByActorDefinitionID2.IsDead)
					{
						actorByActorDefinitionID2.AddChargePoints((int)guardianVowBinding.ChargeGain);
					}
				}
				actorByActorDefinitionID?.NotifyChange("UpdateGuardianVowEvent");
				actorByActorDefinitionID2?.NotifyChange("UpdateGuardianVowEvent");
			}
		}

		public int ClearGuardianVowBindingsByActor(ActorModel actor)
		{
			if (actor == null || GuardianVowBindings == null || GuardianVowBindings.Count == 0)
			{
				return 0;
			}
			int num = 0;
			for (int num2 = GuardianVowBindings.Count - 1; num2 >= 0; num2--)
			{
				GuardianVowBinding guardianVowBinding = GuardianVowBindings[num2];
				if (!(guardianVowBinding.GuardianActorDefinitionID != actor.ActorDefinitionID) || !(guardianVowBinding.SovereignActorDefinitionID != actor.ActorDefinitionID))
				{
					ActorModel actorModel = null;
					actorModel = ((!(guardianVowBinding.GuardianActorDefinitionID == actor.ActorDefinitionID)) ? GetActorByActorDefinitionID(guardianVowBinding.GuardianActorDefinitionID) : GetActorByActorDefinitionID(guardianVowBinding.SovereignActorDefinitionID));
					GuardianVowBindings.RemoveAt(num2);
					num++;
					actorModel?.NotifyChange("UpdateGuardianVowEvent");
				}
			}
			if (num > 0)
			{
				actor.NotifyChange("UpdateGuardianVowEvent");
			}
			return num;
		}

		public void HealSurvivalGameList(ActorModel leaderActor)
		{
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffSurvivalGame_HealPer", ref value, leaderActor);
			ModelList<ActorModel> modelList = null;
			if (leaderActor.Faction == Faction.Raider)
			{
				modelList = Raiders;
			}
			if (leaderActor.Faction == Faction.Survivor)
			{
				modelList = Survivors;
			}
			if (modelList == null || modelList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < modelList.Count; i++)
			{
				if (!modelList[i].IsDead && modelList[i].HasAnyLevelTrait("BaseSurvivalGame"))
				{
					int amountHealed = (int)(modelList[i].MaxHitPoints * value);
					base.manager.ExecuteAction(new HealAction(leaderActor, modelList[i], amountHealed));
				}
			}
		}

		private void DoDeadlyFocusKilledEvent(ActorModel killedActor, ActorModel attackActor)
		{
			if ((killedActor.DeadlyFocusLeftCount_SourceRaider > 0 || killedActor.DeadlyFocusLeftCount_SourceSurvivor > 0) && killedActor != null)
			{
				if (killedActor.DeadlyFocusLeftCount_SourceRaider > 0)
				{
					DeadlyFocus_KilledTrans(killedActor, Faction.Raider);
				}
				if (killedActor.DeadlyFocusLeftCount_SourceSurvivor > 0)
				{
					DeadlyFocus_KilledTrans(killedActor, Faction.Survivor);
				}
				if (attackActor != null)
				{
					DeadlyFocus_KilledDmg(killedActor, attackActor);
				}
			}
		}

		private void DeadlyFocus_KilledTrans(ActorModel killedActor, Faction buffFaction)
		{
			ActorModel leaderBuffDeadlyFocusMan = CombatHelpers.GetLeaderBuffDeadlyFocusMan(base.manager.CombatModel, buffFaction);
			if (leaderBuffDeadlyFocusMan == null)
			{
				return;
			}
			int leaderBuffDeadlyFocusLevel = CombatHelpers.GetLeaderBuffDeadlyFocusLevel(base.manager.CombatModel, buffFaction);
			FixedPoint value = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_LevelReq_KilledTransDis", ref value, leaderBuffDeadlyFocusMan);
			if (leaderBuffDeadlyFocusLevel + 1 < (int)value)
			{
				return;
			}
			FixedPoint value2 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_KilledTransDis", ref value2, leaderBuffDeadlyFocusMan);
			FixedPoint value3 = 0.0;
			base.manager.Player.AbilityManager.VisitParameter("LeaderBuffDeadlyFocus_BuffMaxTurns", ref value3, leaderBuffDeadlyFocusMan);
			List<ActorModel> enemiesByDistanceAndFaction = killedActor.GridCoordinate.GetEnemiesByDistanceAndFaction(killedActor.GridCoordinate, base.manager.CombatModel, (int)value2, buffFaction);
			if (enemiesByDistanceAndFaction == null || enemiesByDistanceAndFaction.Count <= 0)
			{
				return;
			}
			enemiesByDistanceAndFaction.RemoveAll((ActorModel t) => t.Faction != Faction.Raider && t.Faction != Faction.Survivor && t.Faction != Faction.Walker);
			enemiesByDistanceAndFaction.RemoveAll((ActorModel t) => !base.manager.CombatModel.IsActorVisibleByAnySurvivor(t));
			enemiesByDistanceAndFaction.Remove(killedActor);
			switch (buffFaction)
			{
			case Faction.Raider:
				enemiesByDistanceAndFaction.OrderBy((ActorModel t) => t.DeadlyFocusLeftCount_SourceRaider);
				if (enemiesByDistanceAndFaction != null && enemiesByDistanceAndFaction.Count > 0)
				{
					enemiesByDistanceAndFaction[0].DeadlyFocusLeftCount_SourceRaider = (int)value3;
					enemiesByDistanceAndFaction[0].NotifyChange("AbilityVisited", new object[2] { "LeaderBuffDeadlyFocus", false });
					enemiesByDistanceAndFaction[0].NotifyChange("UpdateDeadlyFocus");
				}
				break;
			case Faction.Survivor:
				enemiesByDistanceAndFaction.OrderBy((ActorModel t) => t.DeadlyFocusLeftCount_SourceSurvivor);
				if (enemiesByDistanceAndFaction != null && enemiesByDistanceAndFaction.Count > 0)
				{
					enemiesByDistanceAndFaction[0].DeadlyFocusLeftCount_SourceSurvivor = (int)value3;
					enemiesByDistanceAndFaction[0].NotifyChange("AbilityVisited", new object[2] { "LeaderBuffDeadlyFocus", false });
					enemiesByDistanceAndFaction[0].NotifyChange("UpdateDeadlyFocus");
				}
				break;
			}
		}

		private void DeadlyFocus_KilledDmg(ActorModel killedActor, ActorModel attackActor)
		{
			switch (attackActor.Faction)
			{
			case Faction.Raider:
				if (killedActor.DeadlyFocusLeftCount_SourceRaider > 0)
				{
					attackActor.AddDeadlyFocus_TotalEXDamageMultiplier();
				}
				break;
			case Faction.Survivor:
				if (killedActor.DeadlyFocusLeftCount_SourceSurvivor > 0)
				{
					attackActor.AddDeadlyFocus_TotalEXDamageMultiplier();
				}
				break;
			}
		}

		public void AddShadowedGuard(ActorModel ownActorModel, ActorModel actorModel)
		{
			AbilityManagerModel abilityManager = base.manager.Player.AbilityManager;
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("LeaderBuffShadowedGuard_Charge_MaxNum", ref value, ownActorModel);
			ShadowedGuardSkill shadowedGuardSkill = ownActorModel.CommandSkillModelManager?.GetActorCommandSkill<ShadowedGuardSkill>(CommandSkillType.CommandSkillShadowedGuard);
			if (shadowedGuardSkill == null || shadowedGuardSkill.LeftCooldownTurns > 0 || value > ownActorModel.ChargeNum)
			{
				return;
			}
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("LeaderBuffShadowedGuard_Hp_PerReduce", ref value2, ownActorModel);
			value2 *= (FixedPoint)actorModel.Hitpoints;
			FixedPoint value3 = 0.0;
			abilityManager.VisitParameter("LeaderBuffShadowedGuard_Hp_PreChange", ref value3, ownActorModel);
			value3 *= value2;
			actorModel.ShadowedGuard_Atk = (int)value3;
			if (value2 > 0L && !actorModel.IsStruggling)
			{
				CombatHelpers.ExecuteDamage(this, null, actorModel, (int)value2, 0, DamageType.ShadowedGuard, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				FixedPoint value4 = 0.0;
				int leaderBuffShadowedGuardLevel = CombatHelpers.GetLeaderBuffShadowedGuardLevel(base.manager.CombatModel, ownActorModel.Faction);
				abilityManager.VisitParameter("LeaderBuffShadowedGuard_Level_Recover", ref value4, ownActorModel);
				if (leaderBuffShadowedGuardLevel + 1 >= (int)value4)
				{
					actorModel.ShadowedGuard_DelHP = (int)value2;
				}
			}
			else
			{
				actorModel.ShadowedGuard_DelHP = 0;
			}
			FixedPoint value5 = 0.0;
			abilityManager.VisitParameter("LeaderBuffShadowedGuard_MaxTurns", ref value5, ownActorModel);
			actorModel.ShadowedGuard_LeftCount = (int)value5;
		}

		public void AddShadowedGuardRefTrait(ActorModel ownActorModel, ActorModel actor)
		{
			int leaderBuffShadowedGuardLevel = CombatHelpers.GetLeaderBuffShadowedGuardLevel(base.manager.CombatModel, ownActorModel.Faction);
			FixedPoint value = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffShadowedGuard_Level_Resist", ref value, ownActorModel);
			if (leaderBuffShadowedGuardLevel + 1 >= (int)value)
			{
				actor.AddTrait("ShadowedGuard_StateRef");
			}
		}

		public List<GridCoordinate> GetMagazineGridCoordinates()
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			MagazineAreasManager model = GetModel<MagazineAreasManager>();
			if (model == null)
			{
				return list;
			}
			if (model.ExistedMagazineAreas == null || model.ExistedMagazineAreas.Count == 0)
			{
				return list;
			}
			foreach (MagazineArea existedMagazineArea in model.ExistedMagazineAreas)
			{
				list.Add(existedMagazineArea.EffectiveAreaGridCoordinate);
			}
			return list;
		}

		public void DoCitadelKilledEvent(ActorModel killedActor, ActorModel attackActor)
		{
			if (killedActor != null && killedActor.HasAnyLevelTrait("LeaderBuffCitadel"))
			{
				List<ActorModel> allActors = GetAllActors();
				for (int i = 0; i < allActors.Count; i++)
				{
					allActors[i]?.CleanAllCitadelTraits();
				}
			}
		}
	}
}
