using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BaseModel;
using BaseModel.ContentTypes;
using Newtonsoft.Json;

namespace TWDModel
{
	public class TWDModelManager : ModelManager
	{
		private class Hotfix
		{
			public string Name;

			public Func<bool> Func;

			public Hotfix(string name, Func<bool> func)
			{
				Name = name;
				Func = func;
			}
		}

		public const string PlayerDictionaryKey = "Player";

		public static string Version => OfflineManager.ShortVersion;

		public const long TickLength = 200L;

		protected IMessageSerializer jsonSerializer;

		public bool DisableCombatLoading;

		private bool loadModelFixApplied;

		public MissionData CustomMissionData;

		public string CustomSurvivalMissionConfigName;

		public int CustomSurvivalMissionOrderInSection = -1;

		public int CustomSurvivalMissionOrderNumber = -1;

		public SurvivalMissionConfig.SurvivalObjectiveType CustomSurvivalMissionObjectiveType;

		public string CustomGuildBattleMissionConfigObjectivesString;

		public string CustomGuildBattleMissionConfigEnemiesString;

		protected bool isGuildDirty;

		public List<MatchMakingInfo> LastMatchMakingInfos;

		protected bool desyncDetected;

		public const string OutpostGlobalLeaderboardId = "OutpostGlobal";

		public const string OutpostSeasonLeaderboardIdPrefix = "OutpostSeason_";

		public const string OutpostSeasonLocalLeaderboardIdPrefix = "OutpostSeasonLocal_";

		public const string OutpostGuildLeaderboardIdPrefix = "OutpostGuild_";

		public const string DEBUG_REPORT_MODELLIST = "ModelList";

		private static string[] SEARCH_GROUP_PROPERTIES = new string[21]
		{
			"Id", "Name", "Description", "JoinType", "NumberMembers", "NumberChallengeStarted", "TotalChallengeStars", "PreviousChallengeStars", "CurrentChallengeStars", "GuildMembersPending",
			"GuildMembers", "AverageMemberLevel", "AdCreationTimeStampSeconds", "AdExpireTimeStampSeconds", "AdAvailableTimeSeconds", "IsFull", "CountryCode", "Purpose", "TimeStamp", "TotalAllTimeAccumulatedVp",
			"GuildInfoCurrentVP"
		};

		private static string SEARCH_GROUP_PROPERTIES_TAG = "%selectProperties%";

		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private List<DelayedEventData> collectedEvents;

		private Dictionary<ModelObject, DelayedEventListener> DelayedEventListeners;

		private bool hasDelayedEvents;

		public Dictionary<string, Type> ObjectTypeMapForCreation { get; private set; }

		public GameVersion LoadedModelVersion { get; private set; }

		public bool Disconnected { get; protected set; }

		public CommandLog CommandLog { get; protected set; }

		public CommandLogEntry CurrentCommandLogEntry => CommandLog?.CurrentCommandLogEntry;

		public RollDiceLog RollDiceLog { get; protected set; }

		public PlayerModel Player => root as PlayerModel;

		public BlackboardModel Blackboard => Player.Blackboard;

		public GridModel GridModel => Player.Grid;

		public CombatModel CombatModel => Player?.Combat;

		public CampModel CampModel => Player?.Camp;

		public GameEconomyData GameEconomyData { get; private set; }

		public MediationData MediationData { get; set; }

		[JsonIgnore]
		public bool IsUsingCustomConfigForGuildBattleMission => !string.IsNullOrEmpty(CustomGuildBattleMissionConfigObjectivesString);

		[JsonIgnore]
		public bool IsUsingCustomConfigForSurvivalMission => !string.IsNullOrEmpty(CustomSurvivalMissionConfigName);

		[JsonIgnore]
		public Metrics Metrics { get; set; }

		[JsonIgnore]
		public TdMetrics TdMetrics { get; set; }

		[JsonIgnore]
		public TdMetrics TdUserMetrics { get; set; }

		public MissionData SelectedMissionData
		{
			get
			{
				if (CustomMissionData != null)
				{
					return CustomMissionData;
				}
				return GameEconomyData.GetMissionData(Player.SelectedMissionId);
			}
		}

		public event ModelStartedHandler OnModelStarted;

		public event ActionExecutedEventHandler ActionExecuted;

		public event ActionExecutedEventHandler PreActionExecution;

		public override IMessageSerializer GetMessageSerializer()
		{
			return jsonSerializer;
		}

		public override StorePurchaseInfo GetStorePurchaseInfo(string transactionId)
		{
			return Player.GetPendingPurchase(transactionId);
		}

		public override StorePurchaseInfo GetCurrentPurchaseInfo(string transactionId)
		{
			return Player.CurrentIAP;
		}

		public TWDModelManager()
			: this(false)
		{
			Metrics = new Metrics(this);
		}

		public TWDModelManager(bool useCommandLog = false)
		{
			Metrics = new Metrics(this);
			TdMetrics = new TdMetrics(this);
			TdUserMetrics = new TdMetrics(this);
			jsonSerializer = new MessageSerializer();
			ObjectTypeMapForCreation = new Dictionary<string, Type>();
			if (useCommandLog)
			{
				CommandLog = new CommandLog();
			}
			RollDiceLog = new RollDiceLog();
		}

		public void BindManagerAndPlayer(PlayerModel playerModel)
		{
			root = playerModel;
			playerModel.SetManager(this);
		}

		public override Dictionary<string, string> GetPlayerStateForModelAnalytics(string type = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (Player == null)
			{
				base.Debug.LogError("GetPlayerStateForModelAnalytics() -> Player null, can't fetch player state!");
				return null;
			}
			if (string.IsNullOrEmpty(type) || type.IndexOf("mission") > -1)
			{
				if (CombatModel == null)
				{
					dictionary.Add("mission", "CAMP");
				}
				else
				{
					string value = ((SelectedMissionData != null) ? SelectedMissionData.Id : "SelectedMissionData is NULL");
					dictionary.Add("mission", value);
					dictionary.Add("turn_count", (CombatModel.TurnManager != null) ? CombatModel.TurnManager.TurnCount.ToString() : "0");
					dictionary.Add("out_of_turns", CombatModel.OutOfTurns ? "1" : "0");
					dictionary.Add("after_alarm_turns", CombatModel.AfterAlarmTurns.ToString());
					dictionary.Add("turn_timer_activation_turn", CombatModel.TurnTimerActivationTurn.ToString());
					dictionary.Add("turns_to_flee", (CombatModel.TurnManager != null) ? CombatModel.TurnsToFlee.ToString() : "0");
					dictionary.Add("combat_failure_reason", GetCombatFailureReason());
					if (CombatModel.MissionRoster != null)
					{
						int num = 0;
						int num2 = 0;
						int num3 = 0;
						int num4 = 0;
						int num5 = 0;
						for (int i = 0; i < CombatModel.MissionRoster.Count; i++)
						{
							SurvivorModel survivorModel = CombatModel.MissionRoster[i];
							if (survivorModel != null)
							{
								string text = "survivor_" + (i + 1);
								dictionary.Add(text + "_level", survivorModel.Level.ToString());
								dictionary.Add(text + "_class", survivorModel.SurvivorClass.ToString());
								dictionary.Add(text + "_name", survivorModel.Name ?? "");
								dictionary.Add(text + "_hitpoints", survivorModel.Hitpoints.ToString());
								dictionary.Add(text + "_max_hitpoints", survivorModel.MaxHitPoints.ToString());
								dictionary.Add(text + "_min_hitpoints", survivorModel.MinHitpoints.ToString());
								dictionary.Add(text + "_struggles_left", survivorModel.StrugglesLeft.ToString());
								dictionary.Add(text + "_is_dead", survivorModel.IsDead ? "1" : "0");
								dictionary.Add(text + "_is_struggling", survivorModel.IsStruggling ? "1" : "0");
								dictionary.Add(text + "_is_bleeding_out", survivorModel.IsBleedingOut ? "1" : "0");
								dictionary.Add(text + "_user_can_control", survivorModel.UserCanControl ? "1" : "0");
								dictionary.Add(text + "_user_can_control_false_reason", survivorModel.UserCanControlFalseReason ?? "");
								dictionary.Add(text + "_is_incapacitated", (survivorModel.IsStruggling || !survivorModel.UserCanControl) ? "1" : "0");
								dictionary.Add(text + "_on_red_health_bar", survivorModel.OnRedHealthBar ? "1" : "0");
								dictionary.Add(text + "_combat_end_condition", survivorModel.CombatEndCondition.ToString());
								dictionary.Add(text + "_injury_type_stored", survivorModel.InjuryType.ToString());
								InjuryType injuryType = ((survivorModel.gameEconomyData != null && survivorModel.gameEconomyData.ConfigData != null) ? survivorModel.GetInjuryType() : survivorModel.InjuryType);
								dictionary.Add(text + "_injury_type_current", injuryType.ToString());
								int num6 = ((survivorModel.MaxHitPoints > 0) ? ((survivorModel.MinHitpoints + survivorModel.MaxHitPoints * survivorModel.StrugglesLeft) * 100 / (survivorModel.MaxHitPoints * 2)) : 0);
								dictionary.Add(text + "_injury_health_percent", num6.ToString());
								TimedEffect exclusiveTimedEffect = survivorModel.ExclusiveTimedEffect;
								dictionary.Add(text + "_timed_effect", (exclusiveTimedEffect != null) ? exclusiveTimedEffect.Type.ToString() : "None");
								dictionary.Add(text + "_timed_effect_duration", (exclusiveTimedEffect != null) ? exclusiveTimedEffect.Duration.ToString() : "0");
								dictionary.Add(text + "_timed_effect_counter", (exclusiveTimedEffect != null) ? exclusiveTimedEffect.Counter.ToString() : "0");
								if (survivorModel.Statistics != null && survivorModel.Statistics.NumberOfChargeAbilitiesUsedInMission > 0)
								{
									num5 += survivorModel.Statistics.NumberOfChargeAbilitiesUsedInMission;
								}
								num4 += (survivorModel.IsStruggling ? 1 : 0);
								switch (injuryType)
								{
								case InjuryType.Critical:
									num3++;
									break;
								case InjuryType.Major:
									num2++;
									break;
								case InjuryType.Minor:
									num++;
									break;
								}
							}
						}
						dictionary.Add("injuries_critical", num3.ToString());
						dictionary.Add("injuries_major", num2.ToString());
						dictionary.Add("injuries_minor", num.ToString());
						dictionary.Add("struggles", num4.ToString());
						dictionary.Add("charge_uses", num5.ToString());
					}
				}
			}
			dictionary.Add("council_level", CampModel.GetBuildingLevel("Council").ToString());
			dictionary.Add("player_level", Player.Level.ToString());
			dictionary.Add("player_name", Player.Name);
			if (!string.IsNullOrEmpty(Player.GuildId))
			{
				dictionary.Add("guild_id", Player.GuildId);
			}
			if (string.IsNullOrEmpty(type) || type.IndexOf("currency") > -1)
			{
				dictionary.Add("gold_state", Player.GetCurrency(CurrencyType.Diamonds).Value.ToString());
				dictionary.Add("bought_gold_state", Player.GetCurrency(CurrencyType.Diamonds).Bought.ToString());
				dictionary.Add("free_gold_state", (Player.GetCurrency(CurrencyType.Diamonds).Value - Player.GetCurrency(CurrencyType.Diamonds).Bought).ToString());
				dictionary.Add("xp_state", Player.GetCurrency(CurrencyType.SurvivalPoints).TotalValue.ToString());
				dictionary.Add("supplies_state", Player.GetCurrency(CurrencyType.Supplies).TotalValue.ToString());
				dictionary.Add("trade_goods_state", Player.GetCurrency(CurrencyType.Outpost).Value.ToString());
				dictionary.Add("radio_phone_state", Player.GetCurrency(CurrencyType.Phone).Value.ToString());
				dictionary.Add("gvg_gas_state", Player.GetCurrency(CurrencyType.GvGGas).Value.ToString());
				dictionary.Add("battle_pass_state", Player.GetCurrency(CurrencyType.BattlePass).Value.ToString());
				dictionary.Add("reward_points_state", Player.GetCurrency(CurrencyType.GuildBattleRP).Value.ToString());
				dictionary.Add("loot_key_state", Player.GetCurrency(CurrencyType.LootKeys).Value.ToString());
				foreach (CurrencyType value2 in Enum.GetValues(typeof(CurrencyType)))
				{
					string text2 = value2.ToString();
					if (new CurrencyType[2]
					{
						CurrencyType.BattlePassPoints,
						CurrencyType.FreeGuildGiftPerk
					}.Contains(value2) || text2.IndexOf("token", StringComparison.InvariantCultureIgnoreCase) > 0)
					{
						dictionary.Add(text2.ToLowerInvariant().Replace("token", "") + "_token_state", Player.GetCurrency(value2).Value.ToString());
					}
					else if (text2.StartsWith("Metal", StringComparison.InvariantCultureIgnoreCase) || text2.StartsWith("Badge", StringComparison.InvariantCultureIgnoreCase) || text2.StartsWith("Cloth", StringComparison.InvariantCultureIgnoreCase) || text2.StartsWith("Chemicals", StringComparison.InvariantCultureIgnoreCase) || text2.StartsWith("Food", StringComparison.InvariantCultureIgnoreCase))
					{
						dictionary.Add(text2 + "_state", Player.GetCurrency(value2).Value.ToString());
					}
				}
			}
			if (string.IsNullOrEmpty(type) || type.IndexOf("iap") > -1)
			{
				dictionary.Add("total_usd_spent", Player.TotalUSDSpent.ToString());
			}
			dictionary.Add("GDPR_dataDeletion_isOn", (Player.MarkedForDeletion > 0) ? "1" : "0");
			dictionary.Add("GDPR_adConsent_isOn", Player.HasAcceptedGdprAction("TargetedAdsConsent") ? "1" : "0");
			dictionary.Add("Is_60FPS", Player.Blackboard.IsToggleOn("Toggle.Toggle60FPSModeEnabled") ? "1" : "0");
			dictionary.Add("Language", Player.Language);
			dictionary.Add("EventTreemap", SendSurvivorsToAnalytics());
			dictionary.Add("GrenadeAmount", Player.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.Grenade).Count.ToString());
			dictionary.Add("MedkitAmount", Player.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.MedKit).Count.ToString());
			dictionary.Add("FlareAmount", Player.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.Flare).Count.ToString());
			dictionary.Add("BlastGrenadeAmount", Player.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.BlastGrenade).Count.ToString());
			dictionary.Add("GoreAmount", Player.Equipment.GetConsumablesOfType(EquipmentModel.ConsumableType.Gore).Count.ToString());
			return dictionary;
		}

		private string GetCombatFailureReason()
		{
			CombatModel combatModel = CombatModel;
			if (combatModel == null || !combatModel.IsGuildBattleMission)
			{
				return "";
			}
			if (combatModel.MissionResult != ECombatResult.Failed && (combatModel.CombatRetryChoicePendingState != MissionRetryState.Pending || combatModel.PendingCombatResult != ECombatResult.Failed))
			{
				return "";
			}
			if (!string.IsNullOrEmpty(combatModel.CombatFailureReason))
			{
				return combatModel.CombatFailureReason;
			}
			int num = 0;
			int num2 = 0;
			int num3 = ((combatModel.Survivors != null) ? combatModel.Survivors.Count : 0);
			for (int i = 0; i < num3; i++)
			{
				if (combatModel.Survivors[i] is SurvivorModel survivorModel)
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
			if (combatModel.OutOfTurns)
			{
				return "OutOfTurns";
			}
			if (combatModel.HasPvPRules)
			{
				if (0 + (combatModel.IsPvPFlagCollected ? 1 : 0) + (combatModel.IsPvPLootCollected ? 1 : 0) + (combatModel.IsPvpDefendersKilled ? 1 : 0) == 0)
				{
					return "NoPvpObjectivesCompleted";
				}
				return "PvpFailed";
			}
			return "GuildBattleMissionFailedUnknown";
		}

		private string SendSurvivorsToAnalytics()
		{
			List<object> list = new List<object>();
			foreach (SurvivorModel survivor in Player.SurvivorContainer.Survivors)
			{
				object item = new
				{
					Hero = survivor.ActorDefinitionID,
					Level = survivor.Level,
					Rarity = survivor.SurvivorRarityLevel
				};
				list.Add(item);
			}
			return JsonConvert.SerializeObject(new
			{
				SurvivorInventory = list
			});
		}

		public override MatchMakingInfo GetMatchMakingInfo(IPlayerModel playerModel = null)
		{
			PlayerModel playerModel2 = playerModel as PlayerModel;
			if (playerModel == null)
			{
				playerModel2 = Player;
			}
			if (playerModel2 == null)
			{
				base.Debug.LogError("GetMatchMakingInfo() -> Could not cast given player model to PlayerModel (TWD specific type)!");
				return null;
			}
			long utcTimeStamp = Player.UtcTimeStamp;
			MatchMakingInfo matchMakingInfo = new MatchMakingInfo();
			if (playerModel2.HasValidOutpost)
			{
				IMessageSerializer messageSerializer = GetMessageSerializer();
				string text = messageSerializer.SerializeObject(playerModel2.OutpostModel.StoredLevelModel);
				if (text != null)
				{
					OutpostLevelModel outpostLevelModel = messageSerializer.DeserializeObject<OutpostLevelModel>(text);
					outpostLevelModel.RemoveDefenderHotspots();
					outpostLevelModel.RemoveWalkerHotspots();
					BuildingModel building = playerModel2.Camp.GetBuilding("Outpost");
					CurrencyModel currency = playerModel2.GetCurrency(CurrencyType.Outpost);
					MatchInfo matchInfo = new MatchInfo(tradeGoodsAmount: currency?.Value ?? 0, tradeGoodsCapacity: currency?.Max ?? 0, tierId: (playerModel2.CurrentOutpostTier != null) ? playerModel2.CurrentOutpostTier.Id : "", outpostLevelModel: outpostLevelModel, rankingScore: playerModel2.RankingScore);
					matchInfo.UtcTime = utcTimeStamp;
					matchInfo.DefendingOutpostLevel = Blackboard.GetCounter(BuildingModel.GetBuildingLevelBlackboardKey(building.TypeName, building.TypeIndex));
					matchInfo.DefendingOutpostPower = playerModel2.OutpostPower;
					matchInfo.DefendingOutpostWalkerPower = playerModel2.OutpostWalkerPower;
					matchInfo.DefendingPlayerLevel = playerModel2.Level;
					for (int i = 0; i < playerModel2.SurvivorContainer.OutpostDefendingSurvivors.Count; i++)
					{
						SurvivorModel survivorModel = playerModel2.SurvivorContainer.OutpostDefendingSurvivors[i];
						matchInfo.DefendingSurvivorLevels.Add(survivorModel.Level);
						matchInfo.DefendingSurvivorNames.Add(survivorModel.SurvivorName);
						matchInfo.DefendingSurvivorClasses.Add(survivorModel.SurvivorClass);
						matchInfo.DefendingSurvivorRarityLevels.Add(survivorModel.SurvivorRarityLevel);
					}
					matchMakingInfo.PlayerInformation = matchInfo.GetJson(GetMessageSerializer());
				}
				matchMakingInfo.Rating = playerModel2.Level;
				matchMakingInfo.SecondaryRating = playerModel2.RankingScore;
				matchMakingInfo.Priority = 0;
				long num = playerModel2.ShieldTimeStamp / 1000;
				matchMakingInfo.Availability = ((num > 0) ? num : 0);
			}
			else
			{
				matchMakingInfo.Availability = -1L;
			}
			matchMakingInfo.Version = GameEconomyData.ConfigData.MatchMakingVersion;
			return matchMakingInfo;
		}

		public override MatchMakingSearchParameters GetMatchMakingSearchParameters()
		{
			MatchMakingSearchParameters matchMakingSearchParameters = new MatchMakingSearchParameters();
			List<int> outpostMatchMakingLevelRange = GameEconomyData.ConfigData.OutpostMatchMakingLevelRange;
			List<int> outpostMatchMakingInfluenceRange = GameEconomyData.ConfigData.OutpostMatchMakingInfluenceRange;
			if (Player.HasValidOutpost && outpostMatchMakingLevelRange.Count > 1 && outpostMatchMakingInfluenceRange.Count > 1)
			{
				matchMakingSearchParameters.MinRating = Player.Level + outpostMatchMakingLevelRange[0];
				matchMakingSearchParameters.MaxRating = Player.Level + outpostMatchMakingLevelRange[1];
				matchMakingSearchParameters.MinSecondaryRating = Player.RankingScore + outpostMatchMakingInfluenceRange[0];
				matchMakingSearchParameters.MaxSecondaryRating = Player.RankingScore + outpostMatchMakingInfluenceRange[1];
				matchMakingSearchParameters.ExcludedPlayerIds = Player.ExcludedMatchMakingTargets;
			}
			return matchMakingSearchParameters;
		}

		public void UpdateMatchMakingAvailability(string hashedId, long shieldTimeStamp)
		{
			if (base.ServerService != null)
			{
				Dictionary<MatchMakingValue, object> dictionary = new Dictionary<MatchMakingValue, object>();
				dictionary.Add(MatchMakingValue.MatchMakingAvailabilityUnixTime, shieldTimeStamp / 1000);
				base.Debug.Log("SaveMatchMakingInfo " + hashedId + " " + shieldTimeStamp / 1000);
				if (!base.ServerService.SaveMatchMakingInfo(hashedId, dictionary))
				{
					base.Debug.LogWarning("SaveMatchMakingInfo " + hashedId + " " + dictionary[MatchMakingValue.MatchMakingAvailabilityUnixTime]?.ToString() + " failed!");
				}
			}
		}

		public override long GetMarkForDeletionUnixSeconds()
		{
			if (Player.MarkedForDeletion > 0)
			{
				return (Player.Created - Epoch).Ticks / 10000000 + Player.MarkedForDeletion / 1000;
			}
			return 0L;
		}

		public bool ExecuteAction(ModelAction action)
		{
			if (IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				ExecuteCount++;
				var typeString = action.GetType().Name;
				var actor = Player.Combat.ActiveActor?.Name;

				if (!string.IsNullOrEmpty(actor) && !actor.ToLower().Contains("walker"))
				{
					DebugTWD.Log("ExecuteAction: " + typeString + " (" + ExecuteCount + ")" + " for " + Player.Combat.ActiveActor?.Name ?? "", DebugType.Action);
				}
			}
			bool flag = true;
			if (CurrentCommandLogEntry != null)
			{
				CurrentCommandLogEntry.StartExecuteAction(action);
			}
			List<ModelAction> additionalActions = new List<ModelAction>();
			Player.AbilityManager.VisitActions(action, null, additionalActions);
			MapMissionDebuffHelper.CanUseDebuffMission(this)?.VisitActions(action, null, additionalActions);
			try
			{
				this.PreActionExecution?.Invoke(action);
				if (action.CanExecute())
				{
					bool flag2 = action.Execute(this);
					flag = flag && flag2;
					if (!flag)
					{
						base.Debug.LogError("TWDModelManager.ExecuteAction failed for action '" + action.GetType().Name + "'!");
					}
					else
					{
						if (CombatModel != null)
						{
							CombatModel.OnPostActionExecuted();
						}
						this.ActionExecuted?.Invoke(action);
					}
				}
			}
			catch (Exception exception)
			{
				string text = FormatException(exception);
				base.Debug.LogError("TWDModelManager.ExecuteAction exception: " + text);
				flag = false;
			}
			Dictionary<Type, List<ModelAction>> groupedActions = GetGroupedActions(additionalActions);
			FilterAndSortGroupedActions(ref groupedActions, ref additionalActions);
			foreach (List<ModelAction> value in groupedActions.Values)
			{
				for (int i = 0; i < value.Count; i++)
				{
					ModelAction action2 = value[i];
					bool flag3 = ExecuteAction(action2);
					flag = flag && flag3;
				}
			}
			additionalActions.StableSort((ModelAction a, ModelAction b) => a.SortOrder().CompareTo(b.SortOrder()));
			for (int num = 0; num < additionalActions.Count; num++)
			{
				ModelAction action3 = additionalActions[num];
				bool flag4 = ExecuteAction(action3);
				flag = flag && flag4;
			}
			if (CurrentCommandLogEntry != null)
			{
				CurrentCommandLogEntry.EndExecuteAction(flag);
			}
			return flag;
		}

		private Dictionary<Type, List<ModelAction>> GetGroupedActions(List<ModelAction> additionalActions)
		{
			Dictionary<Type, List<ModelAction>> dictionary = new Dictionary<Type, List<ModelAction>>();
			for (int i = 0; i < (additionalActions?.Count ?? 0); i++)
			{
				ModelAction modelAction = additionalActions[i];
				List<ModelAction> value = null;
				if (modelAction.HasOrderWhenGrouped())
				{
					if (!dictionary.TryGetValue(modelAction.GetType(), out value))
					{
						value = new List<ModelAction>();
						dictionary.Add(modelAction.GetType(), value);
					}
					value.Add(modelAction);
				}
			}
			return dictionary;
		}

		private void FilterAndSortGroupedActions(ref Dictionary<Type, List<ModelAction>> groupedActions, ref List<ModelAction> additionalActions)
		{
			foreach (List<ModelAction> value in groupedActions.Values)
			{
				value.StableSort((ModelAction a, ModelAction b) => a.SortOrder().CompareTo(b.SortOrder()));
				for (int num = 0; num < (value?.Count ?? 0); num++)
				{
					if (additionalActions.Contains(value[num]))
					{
						additionalActions.Remove(value[num]);
					}
				}
			}
		}

		protected override void OnBeforeCommandExecution(ModelCommand command)
		{
			if (CommandLog != null)
			{
				IModelObject model = GetModel(command.ModelId);
				CommandLog.StartCommandExecution(command, model);
			}
			if (command is AbilityCommand)
			{
				RollDiceLog.ClearRollDiceLogEntries();
			}
		}

		protected override void OnCommandExecuted(ModelCommand command)
		{
			DispatchDelayedEvents();
			if (CombatModel != null)
			{
				CombatModel.OnPostCommandExecuted();
				DispatchDelayedEvents();
				if ((command is EndSurvivorTurnCommand && !CombatModel.MissionCompleted) || (command is StartCombatCommand && CombatModel.MissionStartedChanged))
				{
					if (IsLoadDataManager || OfflineManager.IsOfflineMode)
					{
						DebugTWD.LogMycode("if (IsLoadDataManager || OfflineManager.IsOfflineMod || OfflineManager.IsPrivateMode)");
						DebugTWD.LogWarning("RecordCombatStatus: Ignore. Проверить", DebugType.Command);
					}
					else
					{
						Player.RecordCombatStatus();
					}
				}
			}
			if (CommandLog != null)
			{
				CommandLog.EndCommandExecution(success: true);
			}
			if (Player != null && Player.AchievementManager != null)
			{
				Player.AchievementManager.CheckAchievements();
			}
			if (Player != null && Player.Camp != null && Player.Camp.CampDefenseModel != null)
			{
				Player.Camp.CampDefenseModel.CheckSpawns();
			}
		}

		private long CalculateCombatOccupancyHash(CombatModel combatModel)
		{
			long num = 0L;
			if (combatModel.Occupiers != null)
			{
				GridField<ActorModel> occupiers = combatModel.Occupiers;
				for (int i = 0; i < occupiers.Length; i++)
				{
					if (occupiers[i] != null)
					{
						num = num * 314159 + i;
					}
				}
			}
			return num;
		}

		protected override void AddModelState(ModelCommand command)
		{
			base.AddModelState(command);
			command.ModelState.Add("PlayerRandomState", Player.PlayerRandom.State.ToString());
			command.ModelState.Add("PlayerRandomCallCount", Player.PlayerRandom.CallCount.ToString());
			command.ModelState.Add("PlayerRandomInitialSeed", Player.PlayerRandom.InitialSeed.ToString());
			if (Player.Combat != null)
			{
				command.ModelState.Add("CombatOccupancyHash", CalculateCombatOccupancyHash(Player.Combat).ToString());
			}
		}

		protected string GetCommandDebugInfo(ModelCommand command)
		{
			string text = jsonSerializer.SerializeObject(command);
			if (command is AbilityCommand)
			{
				AbilityCommand abilityCommand = command as AbilityCommand;
				ActorModel model = GetModel<ActorModel>(abilityCommand.ModelId);
				SurvivorModel survivorModel = model as SurvivorModel;
				AbilityModel model2 = GetModel<AbilityModel>(abilityCommand.AbilityId);
				string text2 = ((survivorModel != null) ? survivorModel.SurvivorName : model.ActorDefinitionID);
				if (model != null && model2 != null)
				{
					text = text + " Ability " + model2.DefinitionID + " Actor " + text2;
				}
			}
			return text;
		}

		public delegate void OnCommanError(int code);
		public OnCommanError OnCommanErrorResult;
		protected override IModelCommandRespond CheckModelState(ModelCommand command)
		{
			if (desyncDetected)
			{
				if (IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)
				{
					DebugTWD.LogMycode("if (IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)");
					DebugTWD.LogWarning("ModelManager.ExecuteCommand Error" + command.GetType().FullName, DebugType.CommandError);
					desyncDetected = false;
					var respond = new NGModelCommandRespond(command, TWDModelResult.Error);
					OnCommanErrorResult?.Invoke(respond == null ? -1 : respond.Code);
					if (CallCountBase.Instance)
					{
						CallCountBase.Instance.Show_Command_Error(respond);
					}
					return new NGModelCommandRespond(command, TWDModelResult.OK);
				}
				return new NGModelCommandRespond(command, TWDModelResult.Error);
			}
			IModelCommandRespond modelCommandRespond = base.CheckModelState(command);
			OnCommanErrorResult?.Invoke(modelCommandRespond.Code);
			if (modelCommandRespond.Code != 0)
			{
				if (IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)
				{
					DebugTWD.LogMycode("if (IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)");
					DebugTWD.LogWarning("Desync error: Client / Server model list out of sync! Command: " + command.GetType().FullName + ", Code: " + modelCommandRespond.Code, DebugType.CommandError);
					desyncDetected = false;
					if (CallCountBase.Instance)
					{
						CallCountBase.Instance.Show_Command_Error(modelCommandRespond);
					}
					return new NGModelCommandRespond(command, TWDModelResult.OK);
				}
				desyncDetected = true;
				base.Debug.LogError("Desync error: Client / Server model list out of sync! Command: " + command?.ToString() + " " + GetCommandDebugInfo(command) + " Hotfixed: " + Player.ModelHotfixWasApplied + " Model count: " + int.Parse(command.ModelState["ModelCount"]) + "/" + GetDebugModelsCount() + " Model hash: " + long.Parse(command.ModelState["ModelHashCode"]) + "/" + GetDebugModelsHashCode() + " Last visit: " + Player.LastVisitDebugInfo);
				return new NGModelCommandRespond(command, TWDModelResult.ModelListMismatch);
			}
			int num = int.Parse(command.ModelState["PlayerRandomState"]);
			int num2 = int.Parse(command.ModelState["PlayerRandomCallCount"]);
			int num3 = int.Parse(command.ModelState["PlayerRandomInitialSeed"]);
			if (num != Player.PlayerRandom.State || num2 != Player.PlayerRandom.CallCount || num3 != Player.PlayerRandom.InitialSeed)
			{
				if (OfflineManager.IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)
				{
					DebugTWD.LogMycode("if (IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)");
					DebugTWD.LogWarning("Desync error: Client / Server random out of sync! Command: " + command.GetType().FullName + ", Code: " + modelCommandRespond.Code, DebugType.CommandError);
					desyncDetected = false;
					if (CallCountBase.Instance)
					{
						CallCountBase.Instance.Show_Command_Error(new NGModelCommandRespond(command, TWDModelResult.PlayerRandomMismatch));
					}
					return new NGModelCommandRespond(command, TWDModelResult.OK);
				}
				desyncDetected = true;
				base.Debug.LogError("Desync error: Client / Server random out of sync! Command: " + command?.ToString() + " " + GetCommandDebugInfo(command) + " Hotfixed: " + Player.ModelHotfixWasApplied + " State: " + num + "/" + Player.PlayerRandom.State + " Call count: " + num2 + " / " + Player.PlayerRandom.CallCount + " Initial seed: " + num3 + "/" + Player.PlayerRandom.InitialSeed + " Last visit:" + Player.LastVisitDebugInfo);
				var respond = new NGModelCommandRespond(command, TWDModelResult.PlayerRandomMismatch);
				OnCommanErrorResult?.Invoke(respond == null ? -1 : respond.Code);
				return respond;
			}
			if (Player.Combat != null && command.ModelState.ContainsKey("CombatOccupancyHash"))
			{
				long num4 = long.Parse(command.ModelState["CombatOccupancyHash"]);
				if (num4 != CalculateCombatOccupancyHash(Player.Combat))
				{
					if (IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)
					{
						DebugTWD.LogMycode("if (IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)");
						DebugTWD.LogWarning("Desync error: Client / Server combat occupancy map out of sync! Command: " + command.GetType().FullName + ", Code: " + modelCommandRespond.Code, DebugType.CommandError);
						desyncDetected = false;
						if (CallCountBase.Instance)
						{
							CallCountBase.Instance.Show_Command_Error(new NGModelCommandRespond(command, TWDModelResult.PlayerRandomMismatch));
						}
						return new NGModelCommandRespond(command, TWDModelResult.CombatOccupancyMismatch);
					}
					desyncDetected = true;
					base.Debug.LogError("Desync error: Client / Server combat occupancy map out of sync! Command: " + command?.ToString() + " " + GetCommandDebugInfo(command) + " Hotfixed: " + Player.ModelHotfixWasApplied + " Occupancy hash: " + num4 + "/" + CalculateCombatOccupancyHash(Player.Combat) + " Last visit:" + Player.LastVisitDebugInfo);
					var respond = new NGModelCommandRespond(command, TWDModelResult.CombatOccupancyMismatch);
					OnCommanErrorResult?.Invoke(respond == null ? -1 : respond.Code);
					return respond;
				}
			}
			return modelCommandRespond;
		}

		public string GetDebugJSON()
		{
			return jsonSerializer.Serialize(root, indent: true).Replace(", Assembly-CSharp", ", Driller.Games.WalkingDead");
		}

		public string GetDebugModelsList()
		{
			string text = "";
			foreach (KeyValuePair<int, ModelObject> model in models)
			{
				string text2 = model.Value.GetType().Name;
				if (text2 != null && text2.Length > 0)
				{
					int num = text2.LastIndexOf(".");
					if (num >= 0 && num < text2.Length - 1)
					{
						text2 = text2.Substring(num + 1);
					}
				}
				text = text + model.Key + "=" + text2 + "\n";
			}
			return text;
		}

		public override IPlayerModel CreateModel()
		{
			root = new PlayerModel();
			PlayerModel obj = root as PlayerModel;
			obj.Version = Version;
			obj.Blackboard = new BlackboardModel();
			return Player;
		}

		public override IPlayerModel LoadModel(string data, LoginRequest loginRequest)
		{
			DebugTWD.LogWarning("Internal load Player data", DebugType.Load);

			root = jsonSerializer.Deserialize<PlayerModel>(data);
			base.Debug.Log("Player " + Player.Name + ", lifeTime " + Player.LifeTime);
			LoadedModelVersion = new GameVersion(Player.Version);
			if (loginRequest != null && loginRequest.Device != null)
			{
				Player.Country = loginRequest.Device.CountryCode;
				DebugTWD.LogWarning("Player Country is " + Player.Country, DebugType.Load);
			}
			if (loginRequest != null && loginRequest.PcPlatform != null)
			{
				Player.PcPlatform = loginRequest.PcPlatform;
				DebugTWD.LogWarning("Player PcPlatform DataValue is " + Player.PcPlatform.Data.First().Value, DebugType.Load);
				DebugTWD.LogWarning("Player PcPlatform PcAccountId is " + Player.PcPlatform.PcAccountId, DebugType.Load);
			}
			return Player;
		}

		public void LoadModel(PlayerModel deserializedModel)
		{
			root = deserializedModel;
			base.Debug.Log("Player " + Player.Name + ", lifeTime " + Player.LifeTime);
			LoadedModelVersion = new GameVersion(Player.Version);
		}

		public override void StartModel(long time, bool isDev = false)
		{
			base.Time = time / 200 * 200;
			if (Player.PlayerRandom == null)
			{
				InitialPlayerData.CreatePlayerModel(this);
			}
			if (DisableCombatLoading || !Player.CombatTutorialCompleted)
			{
				((PlayerModel)root).DeleteCombatModel(!Player.CombatTutorialCompleted);
			}
			else if (Player.CombatTutorialCompleted && !Player.Tutorial.HasCompletedPart("Tutorial") && Player.Combat != null)
			{
				((PlayerModel)root).DeleteCombatModel(notify: false);
			}
			if (Player.Version != GetVersion() || (Player.HasValidOutpost && Player.OutpostModel.PublishedLevelDataVersion != GameEconomyData.ConfigData.OutpostLevelDataVersion))
			{
				new TWDModelMigrations(this).Migrate(Player);
			}
			else if (isDev && new GameVersion(Player.Version).CompareTo(new GameVersion("6.1.0")) > 0)
			{
				new TWDModelMigrations(this).MigrateDev(Player);
			}
			if (base.VisitMode == VisitMode.None)
			{
				Player.Blackboard.IncreaseCounter("Counter.SessionPlayed");
			}
			if (Player.Version == "7.9.0" && !Player.ResetCombat790_01)
			{
				Player.DeleteCombatModel(notify: false);
				Player.ResetCombat790_01 = true;
			}
			List<Hotfix> list = new List<Hotfix>();
			list.Add(new Hotfix("LoadingScreenCombatWipe", FixLoadingScreenCombatMission));
			list.Add(new Hotfix("SurvivorSlotsMax", FixSurvivorSlotsMaximumLevel));
			list.Add(new Hotfix("FixInvalidUpgradingSurvivor", FixInvalidUpgradingSurvivor));
			list.Add(new Hotfix("EquippedItems", FixEquippedItems));
			list.Add(new Hotfix("FixLastOpenedGiftLootRemoved", FixLastOpenedGiftLootRemoved));
			list.Add(new Hotfix("AssignEquipmentForSurvivorsWithoutEquipment", AssignEquipmentForSurvivorsWithoutEquipment));
			list.Add(new Hotfix("FixRetiredSurvivorStuckInTeam", FixRetiredSurvivorStuckInTeam));
			list.Add(new Hotfix("FixInvalidAttackTarget", FixInvalidAttackTarget));
			list.Add(new Hotfix("FixBrokenChallengeMissionModels", FixBrokenChallengeMissionModels));
			list.Add(new Hotfix("FixInitiatedBundles", FixInitiatedBundles));
			list.Add(new Hotfix("FixInvalidGrindMissionReference", FixInvalidGrindMissionReference));
			list.Add(new Hotfix("FixStuckInLootScreenTutorial", FixStuckInLootScreenTutorial));
			list.Add(new Hotfix("FixSurvivorBadgeMigration270", FixSurvivorBadgeMigration270));
			list.Add(new Hotfix("FixReapplyGoreSettings", FixReapplyGoreSettings));
			list.Add(new Hotfix("FixIncorrectSeasonMissionS8E1", FixIncorrectSeasonMissionS8E1));
			list.Add(new Hotfix("FixAddForestStalkerTraitToEmptySurvivors", FixAddForestStalkerTraitToEmptySurvivors));
			list.Add(new Hotfix("RemoveNextActorOverride", RemoveNextActorOverride));
			list.Add(new Hotfix("FixStuckPlayersDueToBrokenChallenge196", FixStuckPlayersDueToBrokenChallenge196));
			list.Add(new Hotfix("FixOutpostBrokenPlayersFromEmptyCombatColliderModel", FixOutpostBrokenPlayersFromEmptyCombatColliderModel));
			list.Add(new Hotfix("FixConsumablesNotShowingOnTradeGoodsShop", FixConsumablesNotShowingOnTradeGoodsShop));
			ExecuteHotfixes(list);
			base.StartModel(base.Time);
			List<Hotfix> list2 = new List<Hotfix>();
			list2.Add(new Hotfix("FixNullReferenceInCheckingDailyQuestAction", FixNullReferenceInCheckingDailyQuestAction));
			list2.Add(new Hotfix("FixInvalidStruggles", FixInvalidTimedEffects));
			list2.Add(new Hotfix("InvalidUpgradingItem", FixInvalidUpgradingItem));
			list2.Add(new Hotfix("CompletedChallengeMission", FixCompletedChallengeMission));
			list2.Add(new Hotfix("InvalidRescuedSurvivor", FixInvalidRescuedSurvivor));
			list2.Add(new Hotfix("CinemaCrashAtStartup", FixCinemaCrashAtStartup));
			list2.Add(new Hotfix("CombatSurvivorsNullElement", FixCombatSurvivorsNullElement));
			list2.Add(new Hotfix("ChallengeMissionNoManager", ChallengeMissionNoManager));
			list2.Add(new Hotfix("PurchasedBundles", FixPurchasedBundles));
			list2.Add(new Hotfix("AssignEquipmentForSurvivorsWithoutEquipmentPart2", AssignEquipmentForSurvivorsWithoutEquipmentPart2));
			list2.Add(new Hotfix("FixOutpostTutorialState", FixOutpostTutorialState));
			list2.Add(new Hotfix("FixZeroRewardMultiplier", FixZeroRewardMultiplier));
			list2.Add(new Hotfix("FixIncorrect270UpdateChallenge", FixIncorrect270UpdateChallenge));
			list2.Add(new Hotfix("FixIncorrectSeason8MissionIds", FixIncorrectSeason8MissionIds));
			list2.Add(new Hotfix("FixPlayersStuckInStartSeasonPopupLoop", FixPlayersStuckInStartSeasonPopupLoop));
			list2.Add(new Hotfix("FixMixedCurrencyModelOrders", FixMixedCurrencyModelOrders));
			list2.Add(new Hotfix("FixMissingSpawnPointGroups", FixMissingSpawnPointGroups));
			list2.Add(new Hotfix("FixMissingHeroSkinInformation", FixMissingHeroSkinInformation));
			list2.Add(new Hotfix("FixMissingDefaultHeroSkinInformation", FixMissingDefaultHeroSkinInformation));
			list2.Add(new Hotfix("FixApocalypseWeeklyChallengeMap", FixApocalypseWeeklyChallengeMap));
			ExecuteHotfixes(list2);
			long deltaTime = base.Time - Player.LifeTime;
			root.Tick(deltaTime);
			NotifyModelStarted();
			DateTime dateTime = Player.Created.ToUniversalTime().AddMilliseconds(Player.LifeTime);
			DateTime dateTime2 = DateTime.UtcNow.AddMinutes(5.0);
			DateTime dateTime3 = DateTime.UtcNow.AddMinutes(-5.0);
			if (dateTime > dateTime2 || dateTime < dateTime3)
			{
				double totalMinutes = (dateTime - dateTime2).TotalMinutes;
				double totalMinutes2 = (dateTime - dateTime3).TotalMinutes;
				base.Debug.LogWarning("TWDModelManager.LoadModel Player creation time problem, diff more than 5 minutes. Created='" + Player.Created.ToUniversalTime().ToString() + "'. Lifetime='" + Player.LifeTime + "', lifetimeDT='" + Player.Created.ToUniversalTime().AddMilliseconds(Player.LifeTime).ToString() + "', now='" + DateTime.UtcNow.ToString() + "', utcPlus1DiffMinutes='" + totalMinutes + "', utcMinus1DiffMinutes='" + totalMinutes2 + "'");
			}
			if (Player.Combat != null && Player.Combat.OutpostCombat != null)
			{
				long num = Player.UtcTimeStamp - Player.Combat.CombatStartTime;
				long num2 = GameEconomyData.ConfigData.OutpostResumeTimeLimit;
				if (num > num2)
				{
					Player.Combat.EndCombat();
					Player.ResolvePvPResult();
					DeleteCombatModel();
					Player.CombatAutoResolved = true;
				}
			}
			Player.BeginSession();
			if (base.Time != Player.LifeTime)
			{
				base.Debug.LogError("TWDModelManager.LoadModel Time mismatch Time='" + base.Time + "', Lifetime='" + Player.LifeTime + "'");
			}
		}

		public void SetModelHotfixApplied()
		{
			loadModelFixApplied = true;
			Player.ModelHotfixWasApplied = true;
		}

		private void ExecuteHotfixes(List<Hotfix> fixes)
		{
			for (int i = 0; i < fixes.Count; i++)
			{
				if ((GameEconomyData.ConfigData.DisabledHotfixes == null || !GameEconomyData.ConfigData.DisabledHotfixes.Contains(fixes[i].Name)) && fixes[i].Func())
				{
					base.Debug.LogWarning("Hotfix " + fixes[i].Name + " applied");
					SetModelHotfixApplied();
				}
			}
		}

		private bool ChallengeMissionNoManager()
		{
			if (Player != null && GameEconomyData != null && Player.MapContainerModel != null && Player.MapContainerModel.AttackTargetMissionModel != null)
			{
				MapMissionModel attackTargetMissionModel = Player.MapContainerModel.AttackTargetMissionModel;
				if (attackTargetMissionModel.manager == null && (GameEconomyData.IsEpisodeWeeklyChallenge(attackTargetMissionModel.MissionSpawnPointGroupId) || GameEconomyData.IsEpisodeApocalypticWeeklyChallenge(attackTargetMissionModel.MissionSpawnPointGroupId)))
				{
					MissionSpawnPointGroup spawnPointGroup = GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(attackTargetMissionModel.MissionSpawnPointGroupId);
					if (spawnPointGroup != null)
					{
						MissionSpawnPoint spawnPointByMissionId = spawnPointGroup.GetSpawnPointByMissionId(attackTargetMissionModel.MissionId);
						if (spawnPointByMissionId != null)
						{
							MapMissionModel missionModelForSpawnPoint = Player.MapContainerModel.GetMissionModelForSpawnPoint(spawnPointByMissionId);
							if (missionModelForSpawnPoint != null)
							{
								Player.MapContainerModel.SetAttackedMissionForHotfix(missionModelForSpawnPoint);
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private bool FixOutpostTutorialState()
		{
			bool result = false;
			if (Player != null && Player.Camp != null && Player.Camp.Buildings != null && Player.OutpostTutorialState == OutpostTutorialState.Done && (Player.Camp.GetBuildingLevel("Outpost") <= 0 || Player.Camp.GetBuildingLevel("Cage") <= 0))
			{
				Player.OutpostTutorialState = OutpostTutorialState.None;
				base.Debug.Log("Fixe broken OutpostTutorialState to None");
				result = true;
			}
			return result;
		}

		private bool FixZeroRewardMultiplier()
		{
			bool result = false;
			if (Player != null)
			{
				LootEntry lootEntry = null;
				if (Player.WeeklyChallenge != null && Player.WeeklyChallenge.Rewards != null)
				{
					for (int i = 0; i < Player.WeeklyChallenge.Rewards.Count; i++)
					{
						lootEntry = Player.WeeklyChallenge.Rewards[i];
						if (MigrateLootEntryMultiplier(ref lootEntry))
						{
							result = true;
						}
					}
				}
				if (Player.PendingGuildGiftsLootToOpen != null)
				{
					for (int j = 0; j < Player.PendingGuildGiftsLootToOpen.Count; j++)
					{
						lootEntry = Player.PendingGuildGiftsLootToOpen[j];
						if (MigrateLootEntryMultiplier(ref lootEntry))
						{
							result = true;
						}
					}
				}
				if (Player.LootBoxesToOpen != null)
				{
					for (int k = 0; k < Player.LootBoxesToOpen.Count; k++)
					{
						lootEntry = Player.LootBoxesToOpen[k];
						if (MigrateLootEntryMultiplier(ref lootEntry))
						{
							result = true;
						}
					}
				}
				if (Player.BundleManager != null)
				{
					lootEntry = Player.BundleManager.IAPBonusGiftLootEntry;
					if (MigrateLootEntryMultiplier(ref lootEntry))
					{
						result = true;
					}
					for (int l = 0; l < Player.BundleManager.WebShopLootEntrys.Count; l++)
					{
						lootEntry = Player.BundleManager.WebShopLootEntrys[l];
						if (MigrateLootEntryMultiplier(ref lootEntry))
						{
							result = true;
						}
					}
				}
				if (Player.LootManager != null)
				{
					if (Player.LootManager.PendingTradeCrates != null)
					{
						for (int m = 0; m < Player.LootManager.PendingTradeCrates.Count; m++)
						{
							lootEntry = Player.LootManager.PendingTradeCrates[m];
							if (MigrateLootEntryMultiplier(ref lootEntry))
							{
								result = true;
							}
						}
					}
					if (Player.LootManager.Loots != null)
					{
						for (int n = 0; n < Player.LootManager.Loots.Count; n++)
						{
							lootEntry = Player.LootManager.Loots[n];
							if (MigrateLootEntryMultiplier(ref lootEntry))
							{
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private bool MigrateLootEntryMultiplier(ref LootEntry entry)
		{
			if (entry != null && entry.ChallengeRoundCompletionRewardMultiplier == 0)
			{
				entry.ChallengeRoundCompletionRewardMultiplier = 1;
				return true;
			}
			return false;
		}

		private bool FixPlayersStuckInStartSeasonPopupLoop()
		{
			bool flag = false;
			if (Player != null && !Player.Blackboard.IsToggleOn("HasSeenSeasonStart"))
			{
				try
				{
					GvGModelHelper.CreateEnemyPlayerData(Player, Player.gameEconomyData);
				}
				catch (Exception)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Player.Blackboard.SetToggle("HasSeenSeasonStart");
			}
			return flag;
		}

		private bool FixIncorrectSeasonMissionS8E1()
		{
			if (Player != null && Player.Combat != null && Player.Combat.CurrentMissionId != null && Player.Combat.SceneName != null && Player.Combat.CurrentMissionId == "PVE-G02-000000003f7d879c32d7042a6120cb6c26d5b30b" && Player.Combat.SceneName == "Town_Downtown_001_S8_01A")
			{
				Player.DeleteCombatModel(notify: false);
				return true;
			}
			return false;
		}

		private bool FixStuckPlayersDueToBrokenChallenge196()
		{
			if (Player != null && Player.Combat != null && Player.WeeklyChallenge != null && (Player.WeeklyChallenge.Id == 196 || Player.WeeklyChallenge.Id == 198) && Player.Combat.CurrentMissionId == "PVE-G02-000000007a6dacc4fe35cea5e731fd6f44313555" && Player.Combat.SceneName == "Indoors_Cells_001_Challenge_01")
			{
				Player.DeleteCombatModel(notify: false);
				base.Debug.Log("Player fixed from challenge id 196");
				return true;
			}
			return false;
		}

		private bool FixOutpostBrokenPlayersFromEmptyCombatColliderModel()
		{
			if (Player != null && Player.Combat != null && Player.Combat.GetModels<CombatColliderModel>().ToList().Count == 0)
			{
				Player.DeleteCombatModel(notify: false);
				return true;
			}
			return false;
		}

		private bool FixConsumablesNotShowingOnTradeGoodsShop()
		{
			if (Player != null && Blackboard != null && !Blackboard.IsUnlocked("Unlock.Survivor." + SurvivorClass.None))
			{
				Blackboard.Unlock("Unlock.Survivor." + SurvivorClass.None);
				return true;
			}
			return false;
		}

		private bool FixMixedCurrencyModelOrders()
		{
			bool result = false;
			if (Player != null && Player.Currencies != null)
			{
				List<CurrencyType> list = Player.Currencies.Select((CurrencyModel x) => x.Type).ToList();
				List<CurrencyType> second = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>().ToList()
					.Intersect(list)
					.ToList();
				if (!list.SequenceEqual(second))
				{
					Player.Currencies.Models.StableSort((CurrencyModel a, CurrencyModel b) => (a.Type > b.Type) ? 1 : (-1));
					result = true;
				}
			}
			return result;
		}

		private bool FixMissingSpawnPointGroups()
		{
			bool result = false;
			foreach (MissionSpawnPointGroup missionSpawnPointGroup2 in Player.manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				Player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup2);
			}
			MapContainerModel mapContainerModel = Player.MapContainerModel;
			for (int i = 0; i < mapContainerModel.MapMissionGroups.Count; i++)
			{
				MissionSpawnPointGroup missionSpawnPointGroup = Player.MapContainerModel.MapMissionGroups[i].MissionSpawnPointGroup;
				if (missionSpawnPointGroup.Category != MapCategory.Endless && missionSpawnPointGroup.Category != MapCategory.Season)
				{
					continue;
				}
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(missionSpawnPointGroup);
				if (missionGroupModelForSpawnPointGroup == null)
				{
					return false;
				}
				for (int j = 0; j < missionSpawnPointGroup.MissionSpawnPoints.Count; j++)
				{
					MissionSpawnPoint missionSpawnPoint = missionGroupModelForSpawnPointGroup.MissionSpawnPointGroup.MissionSpawnPoints[j];
					MapMissionModel missionModelForSpawnPoint = mapContainerModel.GetMissionModelForSpawnPoint(missionSpawnPoint);
					MapMissionGroupModel missionGroupModelForSpawnPointGroup2 = mapContainerModel.GetMissionGroupModelForSpawnPointGroup(missionSpawnPoint.OwningGroup);
					if (missionModelForSpawnPoint == null)
					{
						missionModelForSpawnPoint = Player.MapContainerModel.CreateMissionModel(missionSpawnPoint);
						missionGroupModelForSpawnPointGroup2.AddMission(missionModelForSpawnPoint);
						result = true;
					}
				}
			}
			return result;
		}

		private bool FixMissingHeroSkinInformation()
		{
			bool result = false;
			if (Player.SurvivorContainer.HeroSkinsOwned == null || Player.SurvivorContainer.HeroSkinsOwned.Count <= 0)
			{
				foreach (SurvivorModel survivor in Player.SurvivorContainer.Survivors)
				{
					foreach (HeroSkinDefinition item in Player.manager.GameEconomyData.HeroSkinDefinitions.Where((HeroSkinDefinition x) => x.HeroID == survivor.ActorDefinitionID && x.AvailableOnHeroPurchased))
					{
						Player.SurvivorContainer.AddHeroSkin(item.ID);
						result = true;
					}
				}
			}
			return result;
		}

		private bool FixMissingDefaultHeroSkinInformation()
		{
			bool result = false;
			foreach (HeroSkinDefinition item in Player.manager.GameEconomyData.HeroSkinDefinitions.Where((HeroSkinDefinition x) => x.AvailableOnHeroPurchased))
			{
				if (Player.SurvivorContainer.HeroSkinsOwned == null)
				{
					Player.SurvivorContainer.HeroSkinsOwned = new List<string>();
				}
				if (!Player.SurvivorContainer.HeroSkinsOwned.Contains(item.ID))
				{
					Player.SurvivorContainer.AddHeroSkin(item.ID);
					result = true;
				}
			}
			return result;
		}

		private bool FixIncorrectSeason8MissionIds()
		{
			bool flag = false;
			if (Player != null && Player.MapContainerModel != null)
			{
				List<string> list = new List<string> { "PVE-G02-000000003f7d879c32d7042a6120cb6c26d5b30b", "PVE-G02-0000000075e9affdd9e75de9b01e02150b7f37e0", "PVE-G02-0000000017ddc34ed9153e8358c462439c8c9d29", "PVE-G02-00000000653cfa573961101982ab34c60259768d", "PVE-G02-00000000314da454173a76c5a1744c3d2d2163c0", "PVE-G02-00000000669d7be6affae20db32019c62ec0cd7d" };
				List<int> list2 = new List<int> { 20000, 20100, 20200, 20300, 20400, 20500, 20600, 20700 };
				for (int i = 0; i < (list2?.Count ?? 0); i++)
				{
					MapMissionGroupModel missionGroupModelForSpawnPointGroup = Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(list2[i]);
					if (missionGroupModelForSpawnPointGroup == null)
					{
						continue;
					}
					for (int j = 0; j < ((missionGroupModelForSpawnPointGroup.Missions != null) ? missionGroupModelForSpawnPointGroup.Missions.Count : 0); j++)
					{
						MapMissionModel mapMissionModel = missionGroupModelForSpawnPointGroup.Missions[j];
						if (mapMissionModel != null && !string.IsNullOrEmpty(mapMissionModel.MissionId) && list.Contains(mapMissionModel.MissionId))
						{
							missionGroupModelForSpawnPointGroup.RemoveMissions();
							base.Debug.Log($"Removed mission in Season 8 SpawnPointGroup: {list2[i].ToString()}, because of MissionId: {mapMissionModel.MissionId}");
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				Player.MapContainerModel.SpawnSeasonEpisodes();
			}
			return flag;
		}

		private bool FixApocalypseWeeklyChallengeMap()
		{
			MapMissionGroupModel mapMissionGroupModel = Player.ApocalypseWeeklyChallenge.GetMapMissionGroupModel();
			if (mapMissionGroupModel != null)
			{
				foreach (MapMissionModel mission in mapMissionGroupModel.Missions)
				{
					if (mission.ChallengeId < 0)
					{
						Player.ApocalypseWeeklyChallenge.Reset(Player.WeeklyChallenge.Id);
						return true;
					}
				}
			}
			return false;
		}

		private bool FixIncorrect270UpdateChallenge()
		{
			bool result = false;
			bool flag = false;
			if (Player != null && Player.WeeklyChallenge != null && Player.WeeklyChallenge.Id == 116)
			{
				MissionSpawnPointGroup spawnPointGroup = GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(-1533515109);
				if (Player.MapContainerModel != null)
				{
					MapMissionGroupModel missionGroupModelForSpawnPointGroup = Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup);
					if (missionGroupModelForSpawnPointGroup != null)
					{
						for (int i = 0; i < ((missionGroupModelForSpawnPointGroup.Missions != null) ? missionGroupModelForSpawnPointGroup.Missions.Count : 0); i++)
						{
							if (missionGroupModelForSpawnPointGroup.Missions[i] != null && missionGroupModelForSpawnPointGroup.Missions[i].ChallengeId == 116)
							{
								missionGroupModelForSpawnPointGroup.Missions[i].ChallengeId = -1;
								flag = true;
							}
						}
					}
					if (flag)
					{
						Player.WeeklyChallenge.Reset(Player.WeeklyChallenge.Id);
						Player.ApocalypseWeeklyChallenge.Reset(Player.WeeklyChallenge.Id);
						result = true;
					}
				}
			}
			return result;
		}

		private bool FixSurvivorBadgeMigration270()
		{
			bool result = false;
			if (Player != null)
			{
				result = Migration270.RunSurvivorBadgeMigration(Player);
			}
			return result;
		}

		private bool FixReapplyGoreSettings()
		{
			bool result = false;
			if (Player != null && Player.Country != null && GameEconomyData.ConfigData.GoreDisabledCountryCodes.Contains(Player.Country.ToLowerInvariant()) && !Player.Blackboard.IsToggleOn("Toggle.GoreDisabled"))
			{
				Player.Blackboard.SetToggle("Toggle.GoreDisabled");
				result = true;
			}
			return result;
		}

		private bool AssignEquipmentForSurvivorsWithoutEquipmentPart2()
		{
			int num = 0;
			if (Player != null && Player.SurvivorContainer != null && Player.SurvivorContainer.Survivors != null && Player.Blackboard.IsToggleOn("PendingEmptyEquipmentHotfixEquip"))
			{
				ModelList<SurvivorModel> survivors = Player.SurvivorContainer.Survivors;
				for (int i = 0; i < survivors.Count; i++)
				{
					SurvivorModel survivorModel = survivors[i];
					if (survivorModel == null)
					{
						continue;
					}
					foreach (EquipmentItemModel item in new List<EquipmentItemModel>(survivorModel.EquipmentItems))
					{
						survivorModel.Unequip(item);
						survivorModel.Equip(item);
						num++;
					}
				}
				Player.Blackboard.ClearToggle("PendingEmptyEquipmentHotfixEquip");
			}
			return num > 0;
		}

		private bool AssignEquipmentForSurvivorsWithoutEquipment()
		{
			int num = 0;
			if (Player != null && Player.Blackboard != null && Player.Equipment != null && Player.SurvivorContainer != null && Player.SurvivorContainer.Survivors != null)
			{
				ModelList<SurvivorModel> survivors = Player.SurvivorContainer.Survivors;
				for (int i = 0; i < survivors.Count; i++)
				{
					SurvivorModel survivorModel = survivors[i];
					if (survivorModel == null)
					{
						continue;
					}
					if (survivorModel.manager == null)
					{
						survivorModel.SetManager(this);
					}
					if (survivorModel.TraitContainer != null && survivorModel.TraitContainer.manager == null)
					{
						survivorModel.TraitContainer.SetManager(this);
					}
					EquipmentItemModel weaponEquipment = survivorModel.GetWeaponEquipment();
					EquipmentItemModel equipmentOfCategory = survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor);
					if (weaponEquipment != null && equipmentOfCategory != null)
					{
						continue;
					}
					base.Debug.Log("Survivor has invalid equips " + survivorModel.Name.ToString());
					if (weaponEquipment == null)
					{
						int num2 = 0;
						EquipmentItemModel equipmentItemModel = null;
						List<EquipmentItemModel> list = new List<EquipmentItemModel>();
						if (Player.Equipment.MeleeWeapons != null)
						{
							list.AddRange(Player.Equipment.MeleeWeapons);
						}
						if (Player.Equipment.RangeWeapons != null)
						{
							list.AddRange(Player.Equipment.RangeWeapons);
						}
						for (int j = 0; j < list.Count; j++)
						{
							EquipmentItemModel equipmentItemModel2 = list[j];
							if (equipmentItemModel2.manager == null)
							{
								equipmentItemModel2.SetManager(this);
							}
							if (equipmentItemModel2.IsWeaponEquipment && survivorModel.CanEquip(equipmentItemModel2) && equipmentItemModel2.Owner == null && equipmentItemModel2.CanBeManipulated())
							{
								int damageForLevel = equipmentItemModel2.GetDamageForLevel(equipmentItemModel2.Level);
								if (damageForLevel > num2)
								{
									num2 = damageForLevel;
									equipmentItemModel = equipmentItemModel2;
								}
							}
						}
						if (equipmentItemModel != null)
						{
							survivorModel.EquipmentItems.Add(equipmentItemModel);
							equipmentItemModel.Owner = survivorModel;
							num++;
							base.Debug.Log("New weapon given to survivor " + equipmentItemModel.Definition.ID);
						}
						else
						{
							base.Debug.LogError("Could not assign weapon for empty weapon on survivor hotfix");
						}
					}
					if (equipmentOfCategory != null || Player.Equipment.Armors == null)
					{
						continue;
					}
					int num3 = 0;
					EquipmentItemModel equipmentItemModel3 = null;
					for (int k = 0; k < Player.Equipment.Armors.Count; k++)
					{
						EquipmentItemModel equipmentItemModel4 = Player.Equipment.Armors[k];
						if (equipmentItemModel4.manager == null)
						{
							equipmentItemModel4.SetManager(this);
						}
						if (equipmentItemModel4.Definition != null && equipmentItemModel4.Definition.Category == EquipmentCategory.Armor && survivorModel.CanEquip(equipmentItemModel4) && equipmentItemModel4.Owner == null && equipmentItemModel4.CanBeManipulated())
						{
							int defenseForLevel = equipmentItemModel4.GetDefenseForLevel(equipmentItemModel4.Level);
							if (defenseForLevel > num3)
							{
								num3 = defenseForLevel;
								equipmentItemModel3 = equipmentItemModel4;
							}
						}
					}
					if (equipmentItemModel3 != null)
					{
						survivorModel.EquipmentItems.Add(equipmentItemModel3);
						equipmentItemModel3.Owner = survivorModel;
						num++;
						base.Debug.Log("New armor given to survivor " + equipmentItemModel3.Definition.ID);
					}
					else
					{
						base.Debug.LogError("Could not assign armor for empty armor on survivor hotfix");
					}
				}
				if (num > 0 && !Player.Blackboard.IsToggleOn("PendingEmptyEquipmentHotfixEquip"))
				{
					Player.Blackboard.SetToggle("PendingEmptyEquipmentHotfixEquip");
				}
			}
			return num > 0;
		}

		private bool FixPurchasedBundles()
		{
			bool flag = false;
			if (Player != null && Player.BundleManager != null && Player.BundleManager.BoughtIAPsHotfixAppliedTimes > 1)
			{
				return false;
			}
			if (Player.BundleManager.BoughtIAPsHotfixAppliedTimes == 1)
			{
				if (Player.BundleManager.InitiatedLimitedBundles != null && Player.BundleManager.InitiatedLimitedBundles.Count > 0)
				{
					Player.BundleManager.InitiatedLimitedBundles.Clear();
					flag = true;
				}
				if (flag)
				{
					Player.BundleManager.BoughtIAPsHotfixAppliedTimes++;
				}
				return flag;
			}
			PlayerModel player = Player;
			if (player.BoughtIAPs != null)
			{
				for (int i = 0; i < player.BoughtIAPs.Count; i++)
				{
					string text = player.BoughtIAPs[i];
					int num = ((player.BoughtIAPsQuantity == null || player.BoughtIAPsQuantity.Count <= i) ? 1 : player.BoughtIAPsQuantity[i]);
					if (player.BundleManager != null && player.BundleManager.BoughtBundles != null)
					{
						if (player.BundleManager.BoughtBundlesAmount == null)
						{
							player.BundleManager.BoughtBundlesAmount = new Dictionary<string, int>();
						}
						if (player.BundleManager.BoughtBundlesLastPurchaseTime == null)
						{
							player.BundleManager.BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
						}
						if (!player.BundleManager.BoughtBundles.Contains(text))
						{
							player.BundleManager.BoughtBundles.Add(text);
							flag = true;
						}
						if (!player.BundleManager.BoughtBundlesAmount.ContainsKey(text))
						{
							player.BundleManager.BoughtBundlesAmount.Add(text, num);
							flag = true;
						}
						else
						{
							player.BundleManager.BoughtBundlesAmount[text] = player.BundleManager.BoughtBundlesAmount[text] + num;
							flag = true;
						}
						if (!player.BundleManager.BoughtBundlesLastPurchaseTime.ContainsKey(text))
						{
							player.BundleManager.BoughtBundlesLastPurchaseTime.Add(text, player.UtcTimeStamp);
							flag = true;
						}
					}
				}
				if (player.BundleManager != null && player.BundleManager.BoughtBundles != null)
				{
					for (int j = 0; j < player.BundleManager.BoughtBundles.Count; j++)
					{
						string key = player.BundleManager.BoughtBundles[j];
						if (player.BundleManager.BoughtBundlesAmount == null)
						{
							player.BundleManager.BoughtBundlesAmount = new Dictionary<string, int>();
						}
						if (player.BundleManager.BoughtBundlesLastPurchaseTime == null)
						{
							player.BundleManager.BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
						}
						if (!player.BundleManager.BoughtBundlesAmount.ContainsKey(key))
						{
							player.BundleManager.BoughtBundlesAmount.Add(key, 1);
							flag = true;
						}
						if (!player.BundleManager.BoughtBundlesLastPurchaseTime.ContainsKey(key))
						{
							player.BundleManager.BoughtBundlesLastPurchaseTime.Add(key, player.UtcTimeStamp);
							flag = true;
						}
					}
				}
			}
			if (Player.BundleManager != null && Player.BundleManager.InitiatedLimitedBundles != null && Player.BundleManager.InitiatedLimitedBundles.Count > 0)
			{
				Player.BundleManager.InitiatedLimitedBundles.Clear();
				flag = true;
			}
			if (flag)
			{
				Player.BundleManager.BoughtIAPsHotfixAppliedTimes++;
			}
			return flag;
		}

		private bool FixLoadingScreenCombatMission()
		{
			if (Player != null && Player.Combat != null && GameEconomyData != null && Player.Combat.OutpostCombat == null)
			{
				MissionData missionData = GameEconomyData.GetMissionData(Player.SelectedMissionId);
				if (missionData == null || missionData.RunLocationName != Player.Combat.SceneName)
				{
					Player.DeleteCombatModel(notify: false);
					return true;
				}
			}
			return false;
		}

		private bool FixCombatSurvivorsNullElement()
		{
			if (Player != null && Player.SurvivorContainer != null && Player.SurvivorContainer.CombatSurvivors != null && Player.SurvivorContainer.CombatSurvivors.Contains(null))
			{
				Player.SurvivorContainer.CombatSurvivors.Remove(null);
				return true;
			}
			return false;
		}

		private bool FixLastOpenedGiftLootRemoved()
		{
			if (Player != null && Player.LastOpenedGuildGiftLoot != null)
			{
				Player.LastOpenedGuildGiftLoot = null;
				return true;
			}
			return false;
		}

		private bool FixInvalidGrindMissionReference()
		{
			if (Player != null && Player.MapContainerModel != null && Player.MapContainerModel.AttackTargetMissionModel != null && Player.MapContainerModel.CurrentGrindMissionModel != null && Player.Combat != null)
			{
				MapMissionModel attackTargetMissionModel = Player.MapContainerModel.AttackTargetMissionModel;
				MissionSpawnPointGroup spawnPointGroup = GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(attackTargetMissionModel.MissionSpawnPointGroupId);
				if (spawnPointGroup != null && spawnPointGroup.Category == MapCategory.Grind && attackTargetMissionModel.MissionId != Player.MapContainerModel.CurrentGrindMissionModel.MissionId)
				{
					Player.DeleteCombatModel();
					return true;
				}
			}
			return false;
		}

		private bool FixInvalidAttackTarget()
		{
			bool result = false;
			if (Player != null && Player.MapContainerModel != null && Player.MapContainerModel.AttackTargetMissionModel != null && Player.Combat != null && Player.Combat.OutpostCombat != null && Player.MapContainerModel.AttackTargetMissionModel.MissionId != Player.Combat.CurrentMissionId)
			{
				Player.MapContainerModel.ClearMissionModelReferences();
				result = true;
			}
			return result;
		}

		private bool FixBrokenChallengeMissionModels()
		{
			bool result = false;
			if (Player != null && Player.MapContainerModel != null && Player.MapContainerModel.MapMissionGroups != null)
			{
				ModelList<MapMissionGroupModel> mapMissionGroups = Player.MapContainerModel.MapMissionGroups;
				for (int i = 0; i < mapMissionGroups.Count; i++)
				{
					MapMissionGroupModel mapMissionGroupModel = mapMissionGroups[i];
					if (mapMissionGroupModel == null || mapMissionGroupModel.Missions == null)
					{
						continue;
					}
					MissionSpawnPointGroup spawnPointGroup = GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(mapMissionGroupModel.MissionSpawnPointGroupId);
					if (spawnPointGroup == null || (spawnPointGroup.Category != MapCategory.Challenge && spawnPointGroup.Category != MapCategory.ApocalypticChallenge))
					{
						continue;
					}
					for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
					{
						MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[j];
						if (mapMissionModel.Stars == null || mapMissionModel.Stars.Stars == null || mapMissionModel.Stars.Stars.Length != 3)
						{
							mapMissionModel.Stars = new MapMissionStars();
							mapMissionModel.Stars.Initialize();
							base.Debug.LogWarning("Fixing stars on broken mission=" + mapMissionModel.MissionId + " from map=" + ((spawnPointGroup == null) ? "null" : spawnPointGroup.DisplayName));
							result = true;
						}
					}
				}
			}
			return result;
		}

		private bool FixNullReferenceInCheckingDailyQuestAction()
		{
			bool result = false;
			if (Player.Camp.GetBuilding("Workshop") is WorkshopBuildingModel { UpgradedUnseenModel: { manager: null } } workshopBuildingModel)
			{
				base.Debug.LogWarning("Workshop null ref exception fix daily quests");
				workshopBuildingModel.MarkModelUpgradeAsSeenHack();
				result = true;
			}
			if (Player.Camp.GetBuilding("TrainingGround") is TrainingGroundBuildingModel { UpgradedUnseenModel: { manager: null } } trainingGroundBuildingModel)
			{
				base.Debug.LogWarning("TrainingGroundTypeName null ref exception fix daily quests");
				trainingGroundBuildingModel.MarkModelUpgradeAsSeenHack();
				result = true;
			}
			return result;
		}

		private bool FixInvalidTimedEffects()
		{
			bool result = false;
			if (Player != null && Player.Combat != null)
			{
				foreach (ActorModel allActor in Player.Combat.AllActors)
				{
					if (allActor.ExclusiveTimedEffect != null && allActor.ExclusiveTimedEffect.Instigator != null && allActor.ExclusiveTimedEffect.Instigator.manager == null)
					{
						result = true;
						allActor.ExclusiveTimedEffect = null;
					}
					if (allActor.PendingExclusiveTimedEffect != null && allActor.PendingExclusiveTimedEffect.Instigator != null && allActor.PendingExclusiveTimedEffect.Instigator.manager == null)
					{
						result = true;
						allActor.PendingExclusiveTimedEffect = null;
					}
				}
			}
			return result;
		}

		private bool FixStuckInLootScreenTutorial()
		{
			bool result = false;
			if (Player != null && Player.Combat != null && Player.Combat.MissionCompleted && Player.Combat.MissionResult == ECombatResult.Successful && Player.Tutorial.CurrentPartId != null && !Player.Tutorial.CurrentPartId.StartsWith("RewardsScreen"))
			{
				Player.Tutorial.SetPartCompleted(Player.Tutorial.CurrentPartId);
				result = true;
			}
			return result;
		}

		private bool FixInitiatedBundles()
		{
			if (Player != null && Player.BundleManager != null && Player.BundleManager.InitiatedLimitedBundles == null)
			{
				Player.BundleManager.InitiatedLimitedBundles = new List<LimitedBundleData>();
				return true;
			}
			return false;
		}

		private bool FixRetiredSurvivorStuckInTeam()
		{
			bool result = false;
			if (Player != null && Player.SurvivorContainer != null)
			{
				SurvivorContainerModel survivorContainer = Player.SurvivorContainer;
				if (survivorContainer.CombatSurvivors != null)
				{
					for (int i = 0; i < survivorContainer.CombatSurvivors.Count; i++)
					{
						if (!survivorContainer.Survivors.Contains(survivorContainer.CombatSurvivors[i]))
						{
							survivorContainer.CombatSurvivors.RemoveAt(i);
							result = true;
						}
					}
				}
				if (survivorContainer.OutpostDefendingSurvivors != null)
				{
					for (int j = 0; j < survivorContainer.OutpostDefendingSurvivors.Count; j++)
					{
						if (!survivorContainer.Survivors.Contains(survivorContainer.OutpostDefendingSurvivors[j]))
						{
							survivorContainer.OutpostDefendingSurvivors.RemoveAt(j);
							result = true;
						}
					}
				}
			}
			return result;
		}

		private bool FixAddForestStalkerTraitToEmptySurvivors()
		{
			if (Player != null && Player.Combat != null && Player.Combat.Survivors != null && Player.Combat.Survivors.Count == 0 && Player.Combat.MissionRoster != null && Player.Combat.MissionCompleted)
			{
				for (int i = 0; i < Player.Combat.MissionRoster.Count; i++)
				{
					Player.Combat.Survivors.Add(Player.Combat.MissionRoster[i]);
					Player.Combat.AllActors.Add(Player.Combat.MissionRoster[i]);
				}
				if (Player.Combat.MissionRoster.Count > 0)
				{
					base.Debug.LogWarning("FixAddForestStalkerTraitToEmptySurvivors: Repaired");
					return true;
				}
			}
			return false;
		}

		private bool RemoveNextActorOverride()
		{
			if (Player != null && Player.Combat != null && Player.Combat.TurnManager != null && Player.Combat.TurnManager.NextActorOverride != null)
			{
				Player.Combat.TurnManager.NextActorOverride = null;
				base.Debug.LogWarning("RemoveNextActorOvverride: Repaired");
				return true;
			}
			return false;
		}

		private bool FixEquippedItems()
		{
			int num = 0;
			if (Player != null && Player.SurvivorContainer != null)
			{
				foreach (SurvivorModel survivor in Player.SurvivorContainer.Survivors)
				{
					foreach (EquipmentItemModel equipmentItem in survivor.EquipmentItems)
					{
						equipmentItem.SetManager(this);
						if (!Player.Equipment.Contains(equipmentItem))
						{
							Player.Equipment.AddEquipment(equipmentItem);
							base.Debug.Log($"Player.SurvivorContainer adds item {equipmentItem.EquipmentDefinitionIdentifier}");
							num++;
						}
					}
				}
			}
			if (Player != null && Player.Combat != null && Player.Combat.Survivors != null)
			{
				foreach (SurvivorModel survivor2 in Player.Combat.Survivors)
				{
					foreach (EquipmentItemModel equipmentItem2 in survivor2.EquipmentItems)
					{
						equipmentItem2.SetManager(this);
						if (!Player.Equipment.Contains(equipmentItem2))
						{
							Player.Equipment.AddEquipment(equipmentItem2);
							base.Debug.Log($"Player.Combat adds item {equipmentItem2.EquipmentDefinitionIdentifier}");
							num++;
						}
					}
				}
			}
			if (Player != null && Player.PhoneCall != null && Player.PhoneCall.Loot != null && Player.PhoneCall.Loot.GeneratedSurvivor != null)
			{
				foreach (EquipmentItemModel equipmentItem3 in Player.PhoneCall.Loot.GeneratedSurvivor.EquipmentItems)
				{
					equipmentItem3.SetManager(this);
					if (!Player.Equipment.Contains(equipmentItem3))
					{
						Player.Equipment.AddEquipment(equipmentItem3);
						base.Debug.Log($"PhoneCall.Loot adds item {equipmentItem3.EquipmentDefinitionIdentifier}");
						num++;
					}
				}
			}
			if (Player != null && Player.PhoneCall != null && Player.PhoneCall.LootsList != null)
			{
				for (int i = 0; i < Player.PhoneCall.LootsList.Count; i++)
				{
					if (Player.PhoneCall.LootsList[i].GeneratedSurvivor == null)
					{
						continue;
					}
					ModelList<EquipmentItemModel> equipmentItems = Player.PhoneCall.LootsList[i].GeneratedSurvivor.EquipmentItems;
					if (equipmentItems == null)
					{
						continue;
					}
					foreach (EquipmentItemModel item in equipmentItems)
					{
						item.SetManager(this);
						if (!Player.Equipment.Contains(item))
						{
							Player.Equipment.AddEquipment(item);
							base.Debug.Log($"PhoneCall.LootList adds item {item.EquipmentDefinitionIdentifier}");
							num++;
						}
					}
				}
			}
			if (num > 0)
			{
				base.Debug.LogWarning("FixEquippedItems: Repaired " + num + " items! Inventory size " + (Player.Equipment.MeleeWeapons.Count + Player.Equipment.RangeWeapons.Count + Player.Equipment.Armors.Count));
				return true;
			}
			return false;
		}

		private bool FixCompletedChallengeMission()
		{
			if (Player != null && Player.MapContainerModel != null && Player.MapContainerModel.MapMissionGroups != null)
			{
				foreach (MapMissionGroupModel mapMissionGroup in Player.MapContainerModel.MapMissionGroups)
				{
					if ((!mapMissionGroup.IsWeeklyChallenge && !mapMissionGroup.IsInApocalyptiWeeklyChallenge) || mapMissionGroup.Missions == null)
					{
						continue;
					}
					foreach (MapMissionModel mission in mapMissionGroup.Missions)
					{
						if (mission.State == MapMissionState.Completed)
						{
							mission.State = MapMissionState.Unlocked;
						}
					}
				}
			}
			return false;
		}

		private bool FixSurvivorSlotsMaximumLevel()
		{
			if (Player != null && Player.SurvivorContainer != null && Player.SurvivorContainer.SurvivorSlotsUpgradeLevel > GameEconomyData.GetMaxSurvivorSlotsLevel())
			{
				Player.SurvivorContainer.SurvivorSlotsUpgradeLevel = GameEconomyData.GetMaxSurvivorSlotsLevel();
				return true;
			}
			return false;
		}

		private bool FixInvalidUpgradingSurvivor()
		{
			bool result = false;
			if (Player.Camp != null && Player.Camp.GetBuilding("TrainingGround") is TrainingGroundBuildingModel { UpgradingModel: not null, UpgradingModel: SurvivorModel upgradingModel } && !Player.SurvivorContainer.ContainsSurvivor(upgradingModel))
			{
				upgradingModel.MissionFailCondition = MissionFailCondition.None;
				Player.SurvivorContainer.Survivors.Add(upgradingModel);
				result = true;
				base.Debug.LogWarning("Survivor " + upgradingModel.Name + " does not exist in survivor container! Adding survivor.");
			}
			return result;
		}

		private bool FixInvalidUpgradingItem()
		{
			if (Player.Camp.GetBuilding("Workshop") is WorkshopBuildingModel workshopBuildingModel)
			{
				bool result = false;
				EquipmentItemModel upgradingEquipment = workshopBuildingModel.UpgradingEquipment;
				if (upgradingEquipment != null && upgradingEquipment.manager == null)
				{
					base.Debug.LogWarning("Workshop item " + upgradingEquipment?.ToString() + " does not exist in inventory! Removing.");
					workshopBuildingModel.ResetUpgradingModel();
					result = true;
				}
				TWDModelObject upgradedUnseenModel = workshopBuildingModel.UpgradedUnseenModel;
				if (upgradedUnseenModel != null && upgradedUnseenModel.manager == null)
				{
					base.Debug.LogWarning("Workshop item " + upgradedUnseenModel?.ToString() + " does not exist in inventory! Removing.");
					workshopBuildingModel.MarkModelUpgradeAsSeen();
					result = true;
				}
				return result;
			}
			return false;
		}

		private bool FixInvalidRescuedSurvivor()
		{
			bool flag = false;
			if (Player != null && Player.Combat != null && Player.Combat.MissionCompleted && Player.Combat.Survivors != null && Player.SurvivorContainer != null)
			{
				foreach (ActorModel survivor in Player.Combat.Survivors)
				{
					if (survivor is SurvivorModel)
					{
						SurvivorModel survivorModel = survivor as SurvivorModel;
						if (!Player.SurvivorContainer.ContainsSurvivor(survivorModel) && (Player.Combat.ExtraSurvivors == null || !Player.Combat.ExtraSurvivors.Contains(survivorModel)))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				Player.DeleteCombatModel(notify: false);
			}
			return flag;
		}

		private bool FixCinemaCrashAtStartup()
		{
			int num = 3;
			int num2 = 3;
			if (GameEconomyData != null && GameEconomyData.ConfigData != null)
			{
				num = GameEconomyData.ConfigData.VideoAdLimit;
				num2 = GameEconomyData.ConfigData.VideoAdLimitRewardScreen;
			}
			bool result = false;
			if (Player.PendingVideoAdReward)
			{
				if (Player.VideoAdsServed >= num)
				{
					Player.VideoAdsServed = num - 1;
					result = true;
				}
				if (Player.VideoAdsServedRewardScreen >= num2)
				{
					Player.VideoAdsServedRewardScreen = num2 - 1;
					result = true;
				}
			}
			return result;
		}

		private bool FixChargeEquipmentsTimedAction()
		{
			if (Player != null && Player.Equipment != null)
			{
				foreach (EquipmentItemModel allEquipment in Player.Equipment.GetAllEquipments())
				{
					if (allEquipment != null && allEquipment.ChargeEquipment != null && allEquipment.ChargeEquipment.TimedActionModel != null)
					{
						allEquipment.NullifyChargeEquipmentTimedActionModel();
					}
				}
				return true;
			}
			return false;
		}

		public override ModelManager CreateManager()
		{
			return new TWDModelManager(false);
		}

		public override IPlayerModel GetPlayer()
		{
			return Player;
		}

		public override IPlayerModel GetVisitPlayer()
		{
			return null;
		}

		public override string SerializeModel()
		{
			return jsonSerializer.Serialize(root);
		}

		public override void SetGameEconomyData(IGameEconomyData data)
		{
			GameEconomyData = data as GameEconomyData;
		}

		public override bool Disconnect(long time)
		{
			if (Disconnected)
			{
				return true;
			}
			Metrics.SendWalkersTapMetric();
			if (base.Mode == ModelManagerMode.Server)
			{
				CombatModel combat = Player.Combat;
				if (combat != null && combat.IsPVPMission)
				{
					base.ServerService.FreeVisit();
				}
				Player.EndSession();
				isDirty = true;
			}
			time = time / 200 * 200;
			base.Debug.Log("DrillModelManager.Disconnect " + time);
			if (isDirty)
			{
				Save(SaveType.Player);
			}
			if (isGuildDirty)
			{
				SaveGuild();
			}
			Disconnected = true;
			return true;
		}

		protected void SaveGuild()
		{
			string guildId = Player.GuildId;
			if (!string.IsNullOrEmpty(guildId) && GetGroupModel(guildId) != null && base.ServerService != null)
			{
				base.ServerService.SaveGroupModel(guildId);
			}
		}

		public override void LoadVisitModel(string modelJson, long time, VisitMode visitMode)
		{
			if (visitMode != VisitMode.PVP)
			{
				throw new Exception("Not supported VisitMode " + visitMode);
			}
			PlayerModel playerModel = jsonSerializer.Deserialize<PlayerModel>(modelJson);
			if (playerModel == null)
			{
				throw new Exception("Failed to deserialize PvP visit model, player json could not be deserialized!");
			}
			if (playerModel.OutpostModel == null)
			{
				throw new Exception("Player being attacked hashed id = '" + playerModel.HashedId + "' does not have OutpostModel!");
			}
			RunLocationModel outpostRunLocation = playerModel.OutpostModel.OutpostRunLocation;
			if (outpostRunLocation == null)
			{
				throw new Exception("Failed to get outpost from attack target, hashed id = '" + playerModel.HashedId + "'.");
			}
			if (playerModel.SurvivorContainer.OutpostDefendingSurvivors == null || playerModel.SurvivorContainer.OutpostDefendingSurvivors.Count == 0)
			{
				throw new Exception("There are no defending survivors in the outpost for the player with hashed id = '" + playerModel.HashedId + "'.");
			}
			if (Player.Combat != null && Player.Combat.OutpostCombat != null && !Player.Combat.OutpostCombat.CombatStarted && Player.Combat.OutpostCombat.DefenderHashedId == playerModel.HashedId)
			{
				base.Debug.LogWarning("Trying to re-apply outpost run location. Ignored.");
			}
			else
			{
				ApplyRunLocation(visitMode, outpostRunLocation, playerModel);
			}
		}

		public void ApplyRunLocation(VisitMode visitMode, RunLocationModel runLocation, PlayerModel defendingPlayer)
		{
			runLocation.SetManager(this);
			if (!runLocation.IsValid())
			{
				throw new Exception("Run location failed validation");
			}
			if (visitMode == VisitMode.ScoutPVE)
			{
				base.Debug.Log("Loaded outpost template " + runLocation.DisplayName + " storing it to outpost template cache with id = '" + Player.SelectedOutpostTemplateDefinitionId + "'.");
				Player.SetOutpostTemplate(Player.SelectedOutpostTemplateDefinitionId, runLocation);
			}
			else
			{
				if (visitMode == VisitMode.PVP && !Player.OutpostModel.MatchMakingPaid)
				{
					throw new Exception("Outpost match has not been paid.");
				}
				CombatModel combat = Player.Combat;
				if (combat != null && combat.CombatRetryChoicePendingState == MissionRetryState.Pending)
				{
					throw new Exception("[ApplyRunLocation] The flow is invalid");
				}
				DebugTWD.Log("SetRunLocation ", DebugType.Load);
				Player.SetRunLocation(runLocation, defendingPlayer);
			}
			if (visitMode == VisitMode.PVE)
			{
				Player.Combat.RunLocationVersion = runLocation.VersionInfo;
			}
			if (base.ServerService != null && !IsLoadDataManager && !OfflineManager.IsOfflineMode)
			{
				DebugTWD.Log("ServerService.Save. Проверить", DebugType.Load);
				base.ServerService.Save(SaveType.Player);
			}
			else
			{
				DebugTWD.Log("ServerService.Save Ignore. Проверить", DebugType.Load);
				DebugTWD.LogMycode("if (base.ServerService != null && !OfflineManager.IsPrivateMode && !OfflineManager.IsOfflineMod)");
			}
		}

		public bool DeleteCombatModel()
		{
			try
			{
				Player.DeleteCombatModel();
			}
			catch (Exception exception)
			{
				string text = FormatException(exception);
				base.Debug.LogError("Failed to delete combat model: " + text);
				return false;
			}
			ClearDelayedEvents();
			return true;
		}

		public override string GetVersion()
		{
			return Version;
		}

		public override void TickModel(long deltaTime)
		{
			if (deltaTime == 0L || base.TickModelSuspended)
			{
				return;
			}
			if (deltaTime < 0 || deltaTime % 200 != 0L)
			{
				base.Debug.LogError("ModelManager.TickModel invalid deltaTime " + deltaTime);
				return;
			}
			long num = deltaTime;
			while (deltaTime > 0)
			{
				base.TickModel(num);
				deltaTime -= num;
			}
			DispatchDelayedEvents();
		}

		public override bool SetMatchData(string parameters, List<MatchMakingInfo> matchInfos)
		{
			LastMatchMakingInfos = matchInfos.ToList();
			return true;
		}

		public MatchMakingInfo GetMatchMakingInfo(string hashedId)
		{
			for (int i = 0; i < ((LastMatchMakingInfos != null) ? LastMatchMakingInfos.Count : 0); i++)
			{
				if (LastMatchMakingInfos[i].PlayerHashedId == hashedId)
				{
					return LastMatchMakingInfos[i];
				}
			}
			return null;
		}

		public int GetNextModelId()
		{
			return nextModelId;
		}

		public List<string> GetModelListReport()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < modelIds.Count; i++)
			{
				ModelObject modelObject = models[modelIds[i]];
				Type type = modelObject.GetType();
				if (type.IsGenericType)
				{
					list.Add(modelObject.ModelId + "-" + type.Name + type.GetGenericArguments()[0].Name);
				}
				else
				{
					list.Add(modelObject.ModelId + "-" + type.Name);
				}
			}
			return list;
		}

		public void SendMetricsEvent(string eventType, Dictionary<string, string> properties)
		{
			if (HelpersModel.IsOffThinkingAnalytics)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager && !OfflineManager.IsUseSendMetrics || OfflineManager.IsOffAnalyticsManager) return");
				return;
			}
			if (base.Analytics != null)
			{
				base.Analytics.CreateEvent(eventType, properties);
			}
		}

		public void SendTdMetricsEvent(string eventType, Dictionary<string, object> properties)
		{
		}

		public void SendTdUserMetricsEvent(string eventType, Dictionary<string, object> properties)
		{
		}

		public override string GetModelSnapshotExtraData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<string> modelListReport = GetModelListReport();
			for (int i = 0; i < modelListReport.Count; i++)
			{
				stringBuilder.AppendLine(modelListReport[i]);
			}
			return stringBuilder.ToString();
		}

		public override string GetRollDiceSnapshotData()
		{
			return null;
		}

		public List<TWDModelObject> GetModels<T>() where T : TWDModelObject
		{
			List<TWDModelObject> list = new List<TWDModelObject>();
			Type typeFromHandle = typeof(T);
			foreach (ModelObject value in models.Values)
			{
				if (typeFromHandle.IsAssignableFrom(value.GetType()))
				{
					list.Add(value as TWDModelObject);
				}
			}
			return list;
		}

		public void DeregisterUnreferencedModels()
		{
			if (root != null)
			{
				Dictionary<int, ModelObject> dictionary = new Dictionary<int, ModelObject>();
				TWDModelObject.AddRecursively(dictionary, root);
				List<ModelObject> list = new List<ModelObject>();
				foreach (KeyValuePair<int, ModelObject> model in models)
				{
					if (!dictionary.ContainsKey(model.Key))
					{
						list.Add(model.Value);
					}
				}
				foreach (ModelObject item in list)
				{
					DeregisterModel(item);
				}
				if (list.Count > 0)
				{
					base.Debug.Log("Deregistered " + list.Count + " models.");
				}
			}
			else
			{
				models.Clear();
				modelIds.Clear();
			}
		}

		public override bool ExecuteLoadQueueMessage(string payload)
		{
			if (base.ServerService != null)
			{
				base.Debug.Log("ExecuteLoadQueueMessage(" + payload + ") for player = " + Player.HashedId);
			}
			LoadQueueMessageContainer loadQueueMessageContainer = jsonSerializer.Deserialize<LoadQueueMessageContainer>(payload);
			if (loadQueueMessageContainer != null && loadQueueMessageContainer.Message != null)
			{
				return loadQueueMessageContainer.Message.Execute(this);
			}
			return true;
		}

		public override bool ExecuteSaveQueueMessage(string payload)
		{
			if (base.ServerService != null)
			{
				base.Debug.Log("ExecuteSaveQueueMessage(" + payload + ") for player = " + Player.HashedId);
			}
			LoadQueueMessageContainer loadQueueMessageContainer = jsonSerializer.Deserialize<LoadQueueMessageContainer>(payload);
			if (loadQueueMessageContainer != null && loadQueueMessageContainer.Message != null)
			{
				return loadQueueMessageContainer.Message.Execute(this);
			}
			return true;
		}

		public override bool ExecuteBananaQueueMessage(string payload, ref BananaBuyBundleRPCCommand bananaBuyBundleRpcCommand)
		{
			if (base.ServerService != null)
			{
				base.Debug.Log("ExecuteBananaQueueMessage(" + payload + ") for player = " + Player.HashedId);
			}
			LoadQueueMessageContainer loadQueueMessageContainer = jsonSerializer.Deserialize<LoadQueueMessageContainer>(payload);
			if (loadQueueMessageContainer == null || loadQueueMessageContainer.Message == null)
			{
				return false;
			}
			if (!(loadQueueMessageContainer.Message is BuyBundleLoadQueueMessage buyBundleLoadQueueMessage))
			{
				return false;
			}
			bool num = loadQueueMessageContainer.Message.Execute(this);
			if (num)
			{
				bananaBuyBundleRpcCommand = new BananaBuyBundleRPCCommand
				{
					BundleId = buyBundleLoadQueueMessage.BundleId,
					PaidPrice = buyBundleLoadQueueMessage.PaidPrice,
					PurchaseSource = buyBundleLoadQueueMessage.PurchaseSource,
					SupportEntityTimestamp = buyBundleLoadQueueMessage.SupportGivenTimestamp
				};
			}
			return num;
		}

		public void AddQueueMessage(string playerHashedId, LoadQueueMessage message, QueueMessageKind messageKind = QueueMessageKind.Load)
		{
			if (message != null && playerHashedId != null && base.ServerService != null)
			{
				LoadQueueMessageContainer value = new LoadQueueMessageContainer
				{
					Message = message,
					ModelVersion = Version
				};
				string text = jsonSerializer.Serialize(value);
				if (text != null && text.Length > 0)
				{
					base.ServerService.AddPlayerQueueMessage(playerHashedId, messageKind, text);
				}
			}
		}

		public override GroupModelBase CreateGroupModel(string id)
		{
			return new GuildModel(id);
		}

		public override void LoadGroupModel(string json, bool forceSync = false)
		{
			GuildModel guildModel = GetMessageSerializer().DeserializeObject<GuildModel>(json);
			if (!groupModels.ContainsKey(guildModel.Id))
			{
				groupModels.Add(guildModel.Id, guildModel);
				if (guildModel.Version != GetVersion())
				{
					new TWDGuildMigrations(this).Migrate(guildModel);
				}
				guildModel.Start();
				guildModel.StartGroupChildren(Player, Player.gameEconomyData);
			}
			else if (forceSync)
			{
				base.Debug.Log("LoadGroupModel force update = " + guildModel.Id + " - with sequence = " + guildModel.SequenceId);
				groupModels[guildModel.Id] = guildModel;
				guildModel.Start();
				guildModel.StartGroupChildren(Player, Player.gameEconomyData);
			}
		}

		public override GroupCommandBase ExecuteGroupCommand(GroupCommandBase command)
		{
			GetGroupModel(command.GroupId).LifeTime = command.Time;
			if (command.SequenceId > 0)
			{
				GetGroupModel(command.GroupId).SequenceId = command.SequenceId;
			}
			bool flag = command is SyncGroupCommand;
			isGuildDirty = !flag;
			command.Execute(this);
			if (flag)
			{
				PostSyncGroupCommand();
			}
			return command;
		}

		protected void PostSyncGroupCommand()
		{
			if (Player != null && Player.WeeklyChallenge != null)
			{
				Player.WeeklyChallenge.UpdateGuildChallenge(updateStars: true);
			}
		}

		public void RemoveGroupModel(string id)
		{
			if (groupModels.ContainsKey(id))
			{
				groupModels.Remove(id);
			}
		}

		public override GroupModelBase GetGroupModelInfo(string groupModelJson)
		{
			GuildModel guildModel = GetMessageSerializer().DeserializeObject<GuildModel>(groupModelJson);
			guildModel.ChatMessages.Clear();
			return guildModel;
		}

		public override IGuildBattleMatchmakingInfoBase GetGuildBattleMatchmakingInfo(string groupModelJson)
		{
			return GetMessageSerializer().DeserializeObject<GuildModelMatchmakingWrapper>(groupModelJson).GuildBattleMatchmakingInfo;
		}

		public void SendGroupCommand(GroupCommandBase groupCommand)
		{
			if (base.ServerService != null&& Player.HasGuild)
			{
				groupCommand.GroupId = Player.GuildId;
				groupCommand.SenderId = Player.HashedId;
				JsonCommand jsonCommand = new JsonCommand();
				jsonCommand.Type = groupCommand.GetType().FullName;
				jsonCommand.Command = GetMessageSerializer().SerializeObject(groupCommand);
				base.ServerService.SendGroupCommand(Player.GuildId, jsonCommand);
			}
		}

		public bool RemovePlayerFromGuild()
		{
			if (string.IsNullOrEmpty(Player.GuildId))
			{
				RemoveGroupModel(Player.GuildId);
				Player.GuildId = "";
				return true;
			}
			return false;
		}

		public override List<GroupInfo> GetGroupInfo()
		{
			List<GroupInfo> list = new List<GroupInfo>();
			if (Player != null && Player.IsGuildMember)
			{
				GroupInfo groupInfo = new GroupInfo();
				groupInfo.GroupId = Player.GuildId;
				groupInfo.Name = Player.GuildModel.Name;
				list.Add(groupInfo);
			}
			return list;
		}

		public override string GetBattlePassRealBundleId(string bundleId, string PurchaseSource = "")
		{
			if (PurchaseSource == "tradefair")
			{
				TradefairBundleStoreDefinition bundleTradefairDefinition = Player.gameEconomyData.GetBundleTradefairDefinition(bundleId);
				if (bundleTradefairDefinition != null)
				{
					TradefairBundleContentDefinition tradefairBundleContentDefinition = Player.gameEconomyData.GetTradefairBundleContentDefinition(bundleTradefairDefinition.BundleIdentifier);
					if (tradefairBundleContentDefinition != null)
					{
						return tradefairBundleContentDefinition.Identifier;
					}
				}
			}
			BundleStoreDefinition bundleStoreDefinition = Player.gameEconomyData.GetBundleStoreDefinition(bundleId);
			if (bundleStoreDefinition == null && Player.gameEconomyData.GetCustomBundleDefinition(bundleId) != null)
			{
				return bundleId;
			}
			BundleContentDefinition bundleContentDefinition = Player.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
			if (bundleContentDefinition.BundleType == BundleType.NormalBP && Player.BattlePass.PremiumActive)
			{
				return Player.gameEconomyData.ConfigData.NormalBPcompensate;
			}
			if (bundleContentDefinition.BundleType == BundleType.BeginerBP && Player.BattlePass.PremiumActive)
			{
				return Player.gameEconomyData.ConfigData.BeginerBPcompensate;
			}
			return bundleId;
		}

		public override bool BananaBuyBundle(string bundleId, double paidPrice, long supportEntityTimestamp, string PurchaseSource = "")
		{
			base.Debug.LogInfo("BananaBuyBundle buy bundle, order.PurchaseSource:" + PurchaseSource + ",order.BundleId :" + bundleId);
			if (Player != null && Player.BundleManager != null && Player.gameEconomyData != null)
			{
				bool flag = false;
				bool flag2 = false;
				if (!string.IsNullOrEmpty(PurchaseSource) && PurchaseSource == "tradefair")
				{
					TradefairBundleStoreDefinition bundleTradefairDefinition = Player.gameEconomyData.GetBundleTradefairDefinition(bundleId);
					flag = Player.TradefairManager.BuyBundle(bundleTradefairDefinition, TradeFairPurchaseType.TradeFairXSolla);
					flag2 = true;
				}
				else
				{
					BundleStoreDefinition bundleStoreDefinition = Player.gameEconomyData.GetBundleStoreDefinition(bundleId);
					flag = Player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, Metrics.BundleSource.Banana, 0L);
				}
				if (!flag)
				{
					return false;
				}
				if (flag2)
				{
					if (Player.WebShopBuyedTradeFairBundleIds == null)
					{
						Player.WebShopBuyedTradeFairBundleIds = new List<string>();
					}
					Player.WebShopBuyedTradeFairBundleIds.Add(bundleId);
				}
				else
				{
					if (Player.WebShopBuyedBundleIds == null)
					{
						Player.WebShopBuyedBundleIds = new List<string>();
					}
					Player.WebShopBuyedBundleIds.Add(bundleId);
					if (paidPrice > 0.0)
					{
						if (Player.WebshopBuyedBundleSingularSyncDatas == null)
						{
							Player.WebshopBuyedBundleSingularSyncDatas = new List<WebshopBuyedBundleSingularSyncData>();
						}
						Player.WebshopBuyedBundleSingularSyncDatas.Add(new WebshopBuyedBundleSingularSyncData
						{
							BundleId = bundleId,
							PaidPrice = paidPrice,
							BuyTime = supportEntityTimestamp
						});
					}
				}
				Save(SaveType.Player);
				return true;
			}
			return false;
		}

		public override bool BuySubscription(string subscriptionId, int platform, long expiryTimeMillis, int giveExtraReward)
		{
			if (Player != null && Player.BundleManager != null && Player.gameEconomyData != null)
			{
				if (Player.SubscriptionBuyedBundleIds == null)
				{
					Player.SubscriptionBuyedBundleIds = new List<string>();
				}
				if (giveExtraReward == 1)
				{
					BundleStoreDefinition bundleStoreDefinition = Player.gameEconomyData.GetBundleStoreDefinition(subscriptionId);
					if (!Player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, Metrics.BundleSource.Subscription, 0L))
					{
						return false;
					}
					Player.SubscriptionBuyedBundleIds.Add(subscriptionId);
				}
				Player.SubscriptionManager.SyncSubscriptionExpireDictionary(subscriptionId, expiryTimeMillis);
				if (giveExtraReward == 0 && Player.SubscriptionManager.IsSubscriptionActive)
				{
					Player.EndlessModeManager.UseSubscriptionConfig = true;
					if (!Player.EndlessModeManager.SubscriptionGivedToken)
					{
						CurrencyModel currency = Player.GetCurrency(CurrencyType.EndlessPassToken);
						int val = Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxPasses - currency.Value;
						int num = Math.Min(Player.gameEconomyData.EndlessModeConfig.SubscriptionPassesGivenPerRefresh - Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh, val);
						currency.Add(num);
						Player.EndlessModeManager.SubscriptionGivedToken = true;
						Player.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassToken, Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh, num).AddEndlessSubscriptionAdd()
							.Send();
						CurrencyModel currency2 = Player.GetCurrency(CurrencyType.EndlessPassExpertToken);
						int val2 = Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxExpertPasses - currency2.Value;
						int num2 = Math.Min(Player.gameEconomyData.EndlessModeConfig.SubscriptionExpertPassesGivenPerRefresh - Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, val2);
						currency2.Add(num2);
						Player.EndlessModeManager.SubscriptionGivedToken = true;
						Player.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassExpertToken, Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, num2).AddEndlessSubscriptionAdd()
							.Send();
					}
				}
				Save(SaveType.Player);
				return true;
			}
			return false;
		}

		public override bool SubscriptionSync()
		{
			if (Player != null)
			{
				return true;
			}
			return false;
		}

		public static string GetOutpostLocalLeaderboardId(string seasonId, string country)
		{
			return "OutpostSeasonLocal_" + country.ToUpperInvariant() + "_" + seasonId;
		}

		private void SetOutpostLeaderboardEntry(string leaderboardId, PlayerModel player, string tierId = "")
		{
			SetOutpostLeaderboardEntry(leaderboardId, player.HashedId, player.Name, player.RankingScore, player.Level, player.OutpostLevel, tierId);
		}

		private void SetOutpostLeaderboardEntryForDefender(string leaderboardId, OutpostCombat combat, int newRankingScore, string tierId = "")
		{
			SetOutpostLeaderboardEntry(leaderboardId, combat.DefenderHashedId, combat.DefenderName, newRankingScore, combat.DefenderPlayerLevel, combat.DefenderOutpostLevel, tierId);
		}

		private void SetOutpostLeaderboardEntry(string leaderboardId, string playerHashedId, string playerName, int playerRankingScore, int playerLevel, int outpostLevel, string tierId = "")
		{
			if (base.ServerService != null)
			{
				Leaderboards.OutpostLeaderboardDetails outpostLeaderboardDetails = new Leaderboards.OutpostLeaderboardDetails();
				outpostLeaderboardDetails.Name = playerName;
				outpostLeaderboardDetails.Level = playerLevel;
				outpostLeaderboardDetails.OutpostLevel = outpostLevel;
				outpostLeaderboardDetails.OutpostTierId = tierId;
				LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
				leaderboardEntry.Id = playerHashedId;
				leaderboardEntry.Score = playerRankingScore;
				leaderboardEntry.Details = jsonSerializer.SerializeObject(outpostLeaderboardDetails);
				base.ServerService.SaveLeaderboardEntry(leaderboardId, leaderboardEntry);
			}
		}

		public void UpdateOutpostLeaderboardEntry()
		{
			OutpostSeason outpostSeason = GameEconomyData.GetOutpostSeason(Player.UtcTimeStamp);
			if (outpostSeason != null)
			{
				OutpostTier outpostInfluenceTier = GameEconomyData.GetOutpostInfluenceTier(Player.RankingScore, outpostSeason.TierSetId);
				SetOutpostLeaderboardEntry("OutpostSeason_" + outpostSeason.Id, Player, outpostInfluenceTier.Id);
				if (Player.Country != null)
				{
					SetOutpostLeaderboardEntry(GetOutpostLocalLeaderboardId(outpostSeason.Id.ToString() ?? "", Player.Country), Player, outpostInfluenceTier.Id);
				}
			}
			else
			{
				SetOutpostLeaderboardEntry("OutpostGlobal", Player);
			}
			if (Player.GuildId != null)
			{
				SetOutpostLeaderboardEntry("OutpostGuild_" + Player.GuildId, Player);
			}
		}

		public void UpdateOutpostLeaderboardEntryForDefender(OutpostCombat outpostCombat, int rankingScoreChange)
		{
			int num = outpostCombat.DefenderInitialRankingScore + rankingScoreChange;
			OutpostSeason outpostSeason = GameEconomyData.GetOutpostSeason(Player.UtcTimeStamp);
			if (outpostSeason != null)
			{
				OutpostTier outpostInfluenceTier = GameEconomyData.GetOutpostInfluenceTier(num, outpostSeason.TierSetId);
				SetOutpostLeaderboardEntryForDefender("OutpostSeason_" + outpostSeason.Id, outpostCombat, num, outpostInfluenceTier.Id);
				if (outpostCombat.DefenderCountry != null)
				{
					SetOutpostLeaderboardEntryForDefender(GetOutpostLocalLeaderboardId(outpostSeason.Id.ToString() ?? "", outpostCombat.DefenderCountry), outpostCombat, num, outpostInfluenceTier.Id);
				}
			}
			else
			{
				SetOutpostLeaderboardEntryForDefender("OutpostGlobal", outpostCombat, num);
			}
			if (outpostCombat.DefenderGuildId != null)
			{
				SetOutpostLeaderboardEntryForDefender("OutpostGuild_" + outpostCombat.DefenderGuildId, outpostCombat, num);
			}
		}

		public override string GetDebugInfo(string debugReportKey)
		{
			if (debugReportKey == "ModelList")
			{
				return GetDebugModelsList();
			}
			if (debugReportKey == "LoadModelFixApplied")
			{
				return loadModelFixApplied.ToString();
			}
			return "";
		}

		private void NotifyModelStarted()
		{
			this.OnModelStarted?.Invoke();
		}

		public override List<string> GetSupportedModelFixes()
		{
			return new List<string> { "ResetCombat", "GiveBundle", "GiveRewards" };
		}

		public override bool ApplyModelFix(string key, string parameter)
		{
			switch (key)
			{
			case "ResetCombat":
				if (Player.Combat != null)
				{
					Player.DeleteCombatModel(notify: false);
					return true;
				}
				break;
			case "GiveBundle":
				if (Player != null && Player.BundleManager != null && parameter != null)
				{
					return Player.BundleManager.BuyBundle(GameEconomyData.GetBundleStoreDefinition(parameter), givenBySupport: true, Metrics.BundleSource.Support, 0L);
				}
				base.Debug.LogError("Bundle reward failed, missing bundle id or invalid player");
				break;
			case "GiveRewards":
				if (Player != null && Player.BundleManager != null && parameter != null)
				{
					return Player.BundleManager.GiveRewardsGivenBySupport(parameter, 0L);
				}
				base.Debug.LogError("Reward failed, missing rewards string or invalid player");
				break;
			}
			return false;
		}

		public override object RunModelTestMethod(string methodName, string parameter = null)
		{
			return string.Empty;
		}

		public override List<string> GetSupportGetMethods()
		{
			return new List<string>();
		}

		public override object RunSupportGetMethod(string methodName, string parameter = null)
		{
			return null;
		}

		public override List<string> GetSupportGiveMethods()
		{
			return new List<string>();
		}

		public override object RunSupportGiveMethod(string methodName, string content, string modelVersion, string supportEntityGUID)
		{
			return null;
		}

		private static long DateTimeToUnixTimeSeconds(DateTime dateTime)
		{
			return (dateTime.ToUniversalTime() - Epoch).Ticks / 10000000;
		}

		public override string BuildSearchGroupQuery(Dictionary<string, string> clientParameters)
		{
			string value = (clientParameters.ContainsKey("keyword") ? clientParameters["keyword"] : string.Empty);
			string text = (clientParameters.ContainsKey("queryType") ? clientParameters["queryType"].ToLowerInvariant() : string.Empty);
			int num = 0;
			if (clientParameters.ContainsKey("queryIteration"))
			{
				string text2 = clientParameters["queryIteration"];
				try
				{
					num = int.Parse(text2);
				}
				catch (FormatException ex)
				{
					base.Debug.LogWarning("Invalid value received from client for queryIteration '" + text2 + "', int parse error: " + ex.Message);
				}
			}
			bool flag = text == "ads" || (clientParameters.ContainsKey("adsOnly") && (clientParameters["adsOnly"].ToLowerInvariant() == "1" || clientParameters["adsOnly"].ToLowerInvariant() == "true"));
			bool flag2 = text == "suggestionsnew";
			bool flag3 = text == "suggestionssimilarlevel";
			bool flag4 = text == "suggestionssamecountry";
			bool flag5 = text == "suggestionsfallback";
			bool flag6 = text == "suggestion";
			bool flag7 = Player.Level <= GameEconomyData.ConfigData.GuildSuggestionsEarlyLevelLimit;
			bool flag8 = GameEconomyData.ConfigData.GuildSuggestionsLargeCountries.Contains(Player.Country.ToLowerInvariant());
			if (Player != null)
			{
				if (!clientParameters.ContainsKey("playerLevel"))
				{
					clientParameters.Add("playerLevel", Player.Level.ToString());
				}
				if (!clientParameters.ContainsKey("playerCountry") && Player.Country != null)
				{
					clientParameters.Add("playerCountry", Player.Country.ToLowerInvariant());
				}
				int num2 = ((Player.Camp != null && Player.Camp.GetBuilding("Council") != null) ? Player.Camp.GetBuilding("Council").Level : 0);
				if (!clientParameters.ContainsKey("playerCouncilLevel"))
				{
					clientParameters.Add("playerCouncilLevel", num2.ToString());
				}
				if (!clientParameters.ContainsKey("playerUTCTime"))
				{
					clientParameters.Add("playerUTCTime", (Player.UtcTimeStamp / 1000).ToString());
				}
				DateTime dateTime = DateTime.UtcNow.AddDays(-7.0);
				int guildSuggestionsRecentDays = GameEconomyData.ConfigData.GuildSuggestionsRecentDays;
				DateTime dateTime2 = DateTime.UtcNow.AddDays(guildSuggestionsRecentDays * -1);
				if (!clientParameters.ContainsKey("weekAgoISO8601"))
				{
					clientParameters.Add("weekAgoISO8601", dateTime.ToString("s") + "Z");
				}
				if (!clientParameters.ContainsKey("weekAgoEpoch"))
				{
					clientParameters.Add("weekAgoEpoch", DateTimeToUnixTimeSeconds(dateTime).ToString());
				}
				if (!clientParameters.ContainsKey("recentISO8601"))
				{
					clientParameters.Add("recentISO8601", dateTime2.ToString("s") + "Z");
				}
				if (!clientParameters.ContainsKey("recentEpoch"))
				{
					clientParameters.Add("recentEpoch", DateTimeToUnixTimeSeconds(dateTime2).ToString());
				}
				if (!clientParameters.ContainsKey("lowLevelLimit"))
				{
					clientParameters.Add("lowLevelLimit", Math.Max(Player.Level - GameEconomyData.ConfigData.GuildSuggestionsSimilarLevelThreshold, 0).ToString());
				}
				if (!clientParameters.ContainsKey("highLevelLimit"))
				{
					clientParameters.Add("highLevelLimit", (Player.Level + GameEconomyData.ConfigData.GuildSuggestionsSimilarLevelThreshold).ToString());
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < SEARCH_GROUP_PROPERTIES.Length; i++)
			{
				stringBuilder.AppendFormat("g.{0}", SEARCH_GROUP_PROPERTIES[i]);
				if (i < SEARCH_GROUP_PROPERTIES.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			string text3 = "";
			if (GameEconomyData != null && GameEconomyData.ConfigData != null)
			{
				if (flag)
				{
					text3 = GameEconomyData.ConfigData.GuildAdSearchQuery;
				}
				else if (flag2)
				{
					text3 = GameEconomyData.ConfigData.GuildSuggestionsNewQuery;
				}
				else if (flag4)
				{
					text3 = ((!flag8) ? GameEconomyData.ConfigData.GuildSuggestionsSameCountryQuery : GameEconomyData.ConfigData.GuildSuggestionsSameLargeCountryQuery);
				}
				else if (flag3)
				{
					text3 = ((!flag7) ? GameEconomyData.ConfigData.GuildSuggestionsSimilarLevelQuery : GameEconomyData.ConfigData.GuildSuggestionsEarlyLevelQuery);
				}
				else if (flag5)
				{
					text3 = GameEconomyData.ConfigData.GuildSuggestionsFallbackQuery;
				}
				else if (!flag6)
				{
					text3 = (string.IsNullOrEmpty(value) ? GameEconomyData.ConfigData.GuildEmptySearchQuery : GameEconomyData.ConfigData.GuildDefaultSearchQuery);
				}
				else
				{
					switch (num)
					{
					case 1:
						text3 = ((!flag7) ? ((!flag8) ? GameEconomyData.ConfigData.GuildSuggestionPopupLateLevelSmallCountryQuery1 : GameEconomyData.ConfigData.GuildSuggestionPopupLateLevelLargeCountryQuery1) : ((!flag8) ? GameEconomyData.ConfigData.GuildSuggestionPopupEarlyLevelSmallCountryQuery1 : GameEconomyData.ConfigData.GuildSuggestionPopupEarlyLevelLargeCountryQuery1));
						break;
					case 2:
						text3 = ((!flag7) ? ((!flag8) ? GameEconomyData.ConfigData.GuildSuggestionPopupLateLevelSmallCountryQuery2 : GameEconomyData.ConfigData.GuildSuggestionPopupLateLevelLargeCountryQuery2) : ((!flag8) ? GameEconomyData.ConfigData.GuildSuggestionPopupEarlyLevelSmallCountryQuery2 : GameEconomyData.ConfigData.GuildSuggestionPopupEarlyLevelLargeCountryQuery2));
						break;
					}
					if (string.IsNullOrEmpty(text3))
					{
						text3 = GameEconomyData.ConfigData.GuildSuggestionsFallbackQuery;
					}
				}
			}
			if (!string.IsNullOrEmpty(text3))
			{
				stringBuilder2.Append(" ");
				string pattern = "%\\w+%";
				string text4 = text3;
				foreach (Match item in Regex.Matches(text3, pattern))
				{
					string key = item.Value.Replace("%", "");
					if (clientParameters.ContainsKey(key))
					{
						text4 = text4.Replace(item.Value, clientParameters[key]);
					}
				}
				text4 = text4.Replace(SEARCH_GROUP_PROPERTIES_TAG, stringBuilder.ToString());
				stringBuilder2.Append(text4);
			}
			string text5 = stringBuilder2.ToString().Trim();
			base.Debug.Log("Searching with query: " + text5);
			return text5;
		}

		public void RegisterDelayedEventListener(ModelObject modelObjectToListen, ModelChangeEventHandler eventHandler)
		{
			if (DelayedEventListeners == null)
			{
				DelayedEventListeners = new Dictionary<ModelObject, DelayedEventListener>();
			}
			if (modelObjectToListen == null)
			{
				base.Debug.LogError("Registering delayed listener to NULL object failed");
				return;
			}
			if (!DelayedEventListeners.ContainsKey(modelObjectToListen))
			{
				modelObjectToListen.Changed += DelayedEventListener_ModelObjectChanged;
				DelayedEventListeners.Add(modelObjectToListen, new DelayedEventListener());
			}
			DelayedEventListener delayedEventListener = DelayedEventListeners[modelObjectToListen];
			delayedEventListener.listeners += eventHandler;
			delayedEventListener.listenerCount++;
		}

		public void UnregisterDelayedEventListener(ModelObject modelObjectToListen, ModelChangeEventHandler eventHandler)
		{
			if (DelayedEventListeners != null && DelayedEventListeners.ContainsKey(modelObjectToListen))
			{
				DelayedEventListener delayedEventListener = DelayedEventListeners[modelObjectToListen];
				delayedEventListener.listeners -= eventHandler;
				delayedEventListener.listenerCount--;
				if (delayedEventListener.listenerCount <= 0)
				{
					DelayedEventListeners.Remove(modelObjectToListen);
					modelObjectToListen.Changed -= DelayedEventListener_ModelObjectChanged;
				}
			}
		}

		private void DelayedEventListener_ModelObjectChanged(ModelObject m, string changed, object args)
		{
			if (DelayedEventListeners == null || !DelayedEventListeners.ContainsKey(m))
			{
				return;
			}
			if (DelayedEventListeners.ContainsKey(m))
			{
				if (collectedEvents == null)
				{
					collectedEvents = new List<DelayedEventData>();
				}
				collectedEvents.Add(new DelayedEventData(m, changed, args));
			}
			hasDelayedEvents = true;
		}

		public void ClearCollectedDelayedEvents()
		{
			if (DelayedEventListeners != null && collectedEvents != null)
			{
				collectedEvents.Clear();
			}
		}

		private void DispatchDelayedEvents()
		{
			int num = 0;
			while (hasDelayedEvents && DelayedEventListeners != null)
			{
				List<DelayedEventData> list = new List<DelayedEventData>(collectedEvents);
				collectedEvents.Clear();
				hasDelayedEvents = false;
				for (int i = 0; i < list.Count; i++)
				{
					if (DelayedEventListeners.TryGetValue(list[i].model, out var value))
					{
						value.Dispatch(list[i]);
					}
				}
				if (++num > 100)
				{
					base.Debug.LogError("TWDModelManager.DispatchDelayedEvents infinite loop detected");
					break;
				}
			}
		}

		public void ClearDelayedEvents()
		{
			if (DelayedEventListeners != null)
			{
				DelayedEventListeners.Clear();
			}
			if (collectedEvents != null)
			{
				collectedEvents.Clear();
			}
			hasDelayedEvents = false;
		}



		#region myparams
		private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
		public int ExecuteCount { get; private set; }
		#endregion

		#region mycode
		public void Start(long time)
		{
			base.Time = time / 200 * 200;
			base.StartModel(base.Time);
		}
		#endregion
	}
}
