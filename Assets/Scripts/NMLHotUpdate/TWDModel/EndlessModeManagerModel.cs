using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class EndlessModeManagerModel : TWDModelObject
	{
		public Dictionary<int, long> OverallScoreHistoryLog;

		public readonly HashSet<int> EndlessParticipationLog = new HashSet<int>();

		public readonly HashSet<int> EndlessExpertParticipationLog = new HashSet<int>();

		public bool EndlessExpertParticipationLogFixApplied;

		public readonly HashSet<int> EndlessRewardsClaimedLog = new HashSet<int>();

		private EndlessModeConfig endlessModeConfig;

		private const int ExpertModeRandomSeed = 1321616566;

		public const int ScoreOverflowLimit = 1000000;

		[JsonIgnore]
		public PlayerModel PlayerModel { get; set; }

		public int Id { get; set; }

		public EndlessModeZoneModel EndlessModeZoneModel { get; set; }

		public List<string> PendingAttemptRewards { get; set; }

		public List<string> PendingAttemptRegularRewards { get; set; }

		public long LastEndlessPassClaimTimeStamp { get; set; }

		private long CheckForEndlessPasses { get; set; }

		public bool PendingLeaderBoardUpdate { get; set; }

		public long OverAllScore { get; set; }

		public long PreviousOverAllScore { get; set; }

		public int CurrentGoldAttemptCount { get; set; }

		public int CurrentExpertGoldAttemptCount { get; set; }

		public bool UseSubscriptionConfig { get; set; }

		public bool SubscriptionGivedToken { get; set; }

		public List<EndlessModeExpertModeHeroDefinition> CurrentExpertModeHeroes { get; set; }

		public EndlessModeGameModeType EndlessModeGameModeType { get; set; }

		[JsonIgnore]
		public long NextEndlessPassClaimTimeStamp => GetNextEndlessBattlePassTimeStamp();

		[JsonIgnore]
		public EndlessModeCalendarDefinition CurrentEndlessModeCalendarDefinition => base.gameEconomyData.GetEndlessModeCalendarDefinitionById(Id);

		[JsonIgnore]
		public EndlessModeConfig EndlessModeConfig
		{
			get
			{
				if (endlessModeConfig == null)
				{
					endlessModeConfig = base.manager.Player.gameEconomyData.EndlessModeConfig;
				}
				return endlessModeConfig;
			}
		}

		[JsonIgnore]
		public EndlessModeCalendarDefinition GetActiveEndlessMode => base.gameEconomyData.GetCurrentEndlessCalendarDefinition(base.manager.Player.UtcTimeStamp);

		[JsonIgnore]
		public bool CanStartNewEndlessModeCycle
		{
			get
			{
				if (GetActiveEndlessMode != null && IsNextEndlessCycle && !DoWeHaveRewardsUnclaimed())
				{
					return base.manager.CombatModel == null;
				}
				return false;
			}
		}

		private bool IsNextEndlessCycle => CurrentEndlessModeCalendarDefinition != PlayerModel.gameEconomyData.GetCurrentEndlessCalendarDefinition(PlayerModel.UtcTimeStamp);

		[JsonIgnore]
		public string CurrentEndlessModeSpawnName => CurrentEndlessModeCalendarDefinition.SpawnSetupID;

		[JsonIgnore]
		public string CurrentExpertEndlessModeSpawnName => CurrentEndlessModeCalendarDefinition.SpawnSetIDExpert;

		[JsonIgnore]
		public string CurrentLeaderBoardName
		{
			get
			{
				if (EndlessModeZoneModel != null && EndlessModeZoneModel.FeatureEnabled)
				{
					int zoneIdById = EndlessModeZoneModel.GetZoneIdById(Id);
					return Leaderboards.GetEndlessModeLeaderboardNameWithZoneId(CurrentEndlessModeCalendarDefinition.Identifier - 1, zoneIdById);
				}
				return Leaderboards.GetEndlessModeLeaderboardName(CurrentEndlessModeCalendarDefinition.Identifier);
			}
		}

		[JsonIgnore]
		public bool IsLockedByCouncilLevel => PlayerModel.CouncilLevel < EndlessModeConfig.CouncilLockLevel;

		private bool CompletedEndlessModeFTUE
		{
			get
			{
				if (PlayerModel.Blackboard.IsToggleOn("ToggleEndlessModeIntroductionPopup"))
				{
					return PlayerModel.Blackboard.IsToggleOn("TootleEndlessModeFTUEHubReturnTutorial");
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool AreEndlessActorsValidAndGenerated
		{
			get
			{
				foreach (EndlessModeExpertModeHeroDefinition currentExpertModeHero in CurrentExpertModeHeroes)
				{
					if (base.manager.GameEconomyData.GetActorDefinition(currentExpertModeHero?.HeroDefinitionID) == null)
					{
						return false;
					}
				}
				return CurrentExpertModeHeroes.Count == PlayerModel.gameEconomyData.EndlessModeConfig.ExpertModeHeroAmount;
			}
		}

		public List<int> ClaimedNormalProgressRewardIndex { get; set; }

		public List<EndlessModeAttemptData> EndlessAttemptData { get; set; }

		public List<EndlessModeAttemptData> EndlessNormalAttemptData { get; set; }

		public List<EndlessModeAttemptData> EndlessExpertAttemptData { get; set; }

		[JsonIgnore]
		public List<EndlessModeNormalRewardDefiniton> GetOrderedEndlessModeNormalRewardsDefinitions => base.manager.GameEconomyData.EndlessModeNormalRewardDefinitons.OrderBy((EndlessModeNormalRewardDefiniton x) => x.Score).ToList();

		public Dictionary<SurvivorClass, List<EndlessModeAttemptData>> EndlessExpertLeaderSurvivorClassAttemptData { get; set; }

		public Dictionary<int, HashSet<SurvivorClass>> EndlessExpertLeaderSurvivorClassRewardsClaimedLog { get; set; }

		public Dictionary<int, bool> EndlessExpertLeaderSurvivorClassParticipationLog { get; set; }

		public HashSet<SurvivorClass> PendingLeaderSurvivorClassUpdate { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			ClaimedNormalProgressRewardIndex = new List<int>();
			EndlessAttemptData = new List<EndlessModeAttemptData>();
			EndlessNormalAttemptData = new List<EndlessModeAttemptData>();
			EndlessExpertAttemptData = new List<EndlessModeAttemptData>();
			LastEndlessPassClaimTimeStamp = base.manager.Player.UtcTimeStamp;
			OverallScoreHistoryLog = new Dictionary<int, long>();
			CurrentExpertModeHeroes = new List<EndlessModeExpertModeHeroDefinition>();
			EndlessModeZoneModel = new EndlessModeZoneModel
			{
				Id2ZoneIdDict = new Dictionary<int, int>()
			};
			InitLeaderSurvivorClassAttempts();
			UpdateLastClaimedEndlessPassTimeStamp();
		}

		public override void Start()
		{
			base.Start();
			PlayerModel = base.manager.Player;
			CheckForEndlessPasses = UtilsDateTime.MinuteInMilliseconds;
			if (EndlessModeZoneModel == null)
			{
				EndlessModeZoneModel = new EndlessModeZoneModel
				{
					Id2ZoneIdDict = new Dictionary<int, int>()
				};
			}
			if (ClaimedNormalProgressRewardIndex == null)
			{
				ClaimedNormalProgressRewardIndex = new List<int>();
			}
			if (EndlessNormalAttemptData == null)
			{
				EndlessNormalAttemptData = new List<EndlessModeAttemptData>();
			}
			if (EndlessExpertAttemptData == null)
			{
				EndlessExpertAttemptData = new List<EndlessModeAttemptData>();
			}
			InitLeaderSurvivorClassAttempts();
			if (!EndlessExpertParticipationLogFixApplied)
			{
				EndlessExpertParticipationLogFixApplied = true;
				if (CurrentEndlessModeCalendarDefinition != null)
				{
					EndlessExpertParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier - 1);
					EndlessExpertParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
				}
			}
			if (!IsNextEndlessCycle)
			{
				if (PendingLeaderBoardUpdate)
				{
					bool flag = UpdateLeaderBoardEntry();
					PendingLeaderBoardUpdate = !flag;
				}
				RetryPendingLeaderSurvivorClassUpdate();
			}
			else if (DoWeHaveRewardsUnclaimed() && base.manager.ServerService != null)
			{
				string hashedId = PlayerModel.HashedId;
				string leaderboard = Leaderboards.GetEndlessModeLeaderboardName(CurrentEndlessModeCalendarDefinition.Identifier - 1);
				if (EndlessModeZoneModel.FeatureEnabled)
				{
					int zoneIdById = EndlessModeZoneModel.GetZoneIdById(Id);
					leaderboard = Leaderboards.GetEndlessModeLeaderboardNameWithZoneId(CurrentEndlessModeCalendarDefinition.Identifier - 1, zoneIdById);
				}
				if (base.manager.ServerService.GetLeaderboardPosition(leaderboard, hashedId) == null)
				{
					EndlessRewardsClaimedLog.Add(CurrentEndlessModeCalendarDefinition.Identifier - 1);
					base.manager.SetModelHotfixApplied();
				}
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			CheckForEndlessPasses -= deltaTime;
			if (CheckForEndlessPasses <= 0)
			{
				CheckForNextEndlessPass();
				CheckForEndlessPasses = UtilsDateTime.MinuteInMilliseconds;
			}
		}

		public void StartNewEndlessCycle(int id)
		{
			if (Id != 0)
			{
				OverallScoreHistoryLog[Id] = OverAllScore;
			}
			Id = id;
			if (base.manager.GameEconomyData.ConfigData.LastStandWarZoneSwitch)
			{
				LastStandWarZone lastStandWarZoneByCouncilLevel = base.manager.GameEconomyData.GetLastStandWarZoneByCouncilLevel(base.manager.Player.CouncilLevel);
				if (lastStandWarZoneByCouncilLevel != null && EndlessModeZoneModel != null)
				{
					EndlessModeZoneModel.FeatureEnabled = true;
					EndlessModeZoneModel.Id2ZoneIdDict.Add(Id, lastStandWarZoneByCouncilLevel.Id);
				}
			}
			PreviousOverAllScore = (EndlessParticipationLog.Contains(Id - 1) ? OverAllScore : 0);
			OverAllScore = 0L;
			ClaimedNormalProgressRewardIndex.Clear();
			EndlessAttemptData.Clear();
			EndlessNormalAttemptData.Clear();
			EndlessExpertAttemptData.Clear();
			GenerateNewExpertModeActors();
			ResetLeaderSurvivorClassAttempts();
			SubscriptionGivedToken = false;
			UseSubscriptionConfig = false;
			if (base.manager.Player.SubscriptionManager.IsSubscriptionActive)
			{
				UseSubscriptionConfig = true;
				CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.EndlessPassToken);
				int val = base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxPasses - currency.Value;
				int num = Math.Min(base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionPassesGivenPerRefresh - base.manager.Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh, val);
				currency.Add(num);
				SubscriptionGivedToken = true;
				base.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassToken, EndlessModeConfig.PassesGivenPerRefresh, num).AddEndlessSubscriptionAdd()
					.Send();
				CurrencyModel currency2 = base.manager.Player.GetCurrency(CurrencyType.EndlessPassExpertToken);
				int val2 = base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxExpertPasses - currency2.Value;
				int num2 = Math.Min(base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionExpertPassesGivenPerRefresh - base.manager.Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, val2);
				currency2.Add(num2);
				SubscriptionGivedToken = true;
				base.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassExpertToken, EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, num2).AddEndlessSubscriptionAdd()
					.Send();
			}
			else
			{
				CurrencyModel currency3 = base.manager.Player.GetCurrency(CurrencyType.EndlessPassToken);
				if (currency3.Value > base.manager.Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh)
				{
					currency3.SetValue(base.manager.Player.gameEconomyData.EndlessModeConfig.PassesGivenPerRefresh);
				}
				CurrencyModel currency4 = base.manager.Player.GetCurrency(CurrencyType.EndlessPassExpertToken);
				if (currency4.Value > base.manager.Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh)
				{
					currency4.SetValue(base.manager.Player.gameEconomyData.EndlessModeConfig.EndlessExpertPassesGivenPerRefresh);
				}
			}
		}

		private void CheckForNextEndlessPass()
		{
			int num = CalculateAccumulatedEndlessPassTokens();
			if (num > 0)
			{
				int claimableCount = num * EndlessModeConfig.PassesGivenPerRefresh;
				AddAndCalculateEndlessPasses(claimableCount);
				int claimableCount2 = num * EndlessModeConfig.EndlessExpertPassesGivenPerRefresh;
				AddAndCalculateEndlessExpertPasses(claimableCount2);
				if (CompletedEndlessModeFTUE)
				{
					PlayerModel.Blackboard.ClearToggle("Toggle.ToggleEndlessModeHighlightExpired");
				}
			}
		}

		private List<string> GeneratePostMissionRewards(int waveCount, List<string> setIds)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < waveCount; i++)
			{
				List<EndlessModeWaveReward> possibleRewardsForWave = GetPossibleRewardsForWave(i, setIds);
				list.Add(GetRandomRewardFromCollection(possibleRewardsForWave)?.Reward);
			}
			if (!list.Any((string x) => !string.IsNullOrEmpty(x)))
			{
				return null;
			}
			return list;
		}

		private List<string> GeneratePostMissionRegularRewards(int waveCount, List<string> setIds)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < waveCount; i++)
			{
				list.Add(GetRegularRewardsForWave(i, setIds)?.Reward);
			}
			if (!list.Any((string x) => !string.IsNullOrEmpty(x)))
			{
				return null;
			}
			return list;
		}

		private List<EndlessModeWaveReward> GetPossibleRewardsForWave(int wave, List<string> setIds)
		{
			return PlayerModel.gameEconomyData.EndlessModeWaveRewards.Where((EndlessModeWaveReward x) => setIds.Contains(x.ID) && wave >= x.WaveNumberMin && wave <= x.WaveNumberMax).ToList();
		}

		private EndlessModeWaveRegularReward GetRegularRewardsForWave(int wave, List<string> setIds)
		{
			string firstSetId = setIds[0];
			return PlayerModel.gameEconomyData.EndlessModeWaveRegularRewards.FirstOrDefault((EndlessModeWaveRegularReward x) => firstSetId == x.ID && wave == x.WaveNumber);
		}

		private EndlessModeWaveReward GetRandomRewardFromCollection(List<EndlessModeWaveReward> possibleRewards)
		{
			int num = possibleRewards.Sum((EndlessModeWaveReward x) => x.Weight);
			if (num == 0)
			{
				return null;
			}
			int randomInRange = PlayerModel.PlayerRandom.GetRandomInRange(1, num);
			int num2 = 0;
			foreach (EndlessModeWaveReward possibleReward in possibleRewards)
			{
				num2 += possibleReward.Weight;
				if (num2 >= randomInRange)
				{
					return possibleReward;
				}
			}
			return null;
		}

		public TWDModelResult GiveAttemptRegularRewards(out Rewards regularRewards)
		{
			regularRewards = new Rewards();
			if (PendingAttemptRegularRewards == null || PendingAttemptRegularRewards.Count == 0)
			{
				return TWDModelResult.OK;
			}
			for (int i = 0; i < PendingAttemptRegularRewards.Count; i++)
			{
				if (PendingAttemptRegularRewards[i] == null)
				{
					continue;
				}
				Rewards rewards = new Rewards(PendingAttemptRegularRewards[i]);
				if (rewards.RewardsList.Count != 0)
				{
					List<object> list = rewards.Give(base.manager);
					IReward rewardAt = rewards.GetRewardAt(0);
					base.manager.Metrics.AddFind();
					if (list[0] is EquipmentItemModel equipment)
					{
						base.manager.Metrics.AddEquipment(equipment, "Equipment", (rewardAt as RewardEquipment)?.Amount ?? 1);
					}
					else
					{
						base.manager.Metrics.AddReward(rewardAt);
					}
					base.manager.Metrics.AddMission().AddEndless(EndlessModeGameModeType.ToString()).AddPerformanceRewards(i)
						.Send();
					regularRewards.RewardsList.Add(rewardAt);
				}
			}
			PendingAttemptRegularRewards = null;
			if (regularRewards.RewardsList.Count != 0)
			{
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}

		public TWDModelResult GiveAttemptRewards(out Rewards rewards)
		{
			rewards = new Rewards();
			if (PendingAttemptRewards == null || PendingAttemptRewards.Count == 0)
			{
				return TWDModelResult.OK;
			}
			for (int i = 0; i < PendingAttemptRewards.Count; i++)
			{
				if (PendingAttemptRewards[i] == null)
				{
					continue;
				}
				Rewards rewards2 = new Rewards(PendingAttemptRewards[i]);
				if (rewards2.RewardsList.Count != 0)
				{
					List<object> list = rewards2.Give(base.manager);
					IReward rewardAt = rewards2.GetRewardAt(0);
					base.manager.Metrics.AddFind();
					if (list[0] is EquipmentItemModel equipment)
					{
						base.manager.Metrics.AddEquipment(equipment, "Equipment", (rewardAt as RewardEquipment)?.Amount ?? 1);
					}
					else
					{
						base.manager.Metrics.AddReward(rewardAt);
					}
					base.manager.Metrics.AddMission().AddEndless(EndlessModeGameModeType.ToString()).AddPerformanceRewards(i)
						.Send();
					rewards.RewardsList.Add(rewardAt);
				}
			}
			PendingAttemptRewards = null;
			if (rewards.RewardsList.Count != 0)
			{
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}

		public TWDModelResult GiveLeaderBoardRewards(out Rewards rewards, long position, long entryCount, string setId, int leaderboardId)
		{
			EndlessModeLeaderBoardReward endlessModeLeaderBoardReward = base.manager.GameEconomyData.GetEndlessModeLeaderBoardReward(setId, position, entryCount);
			string rewardsString = endlessModeLeaderBoardReward?.Rewards ?? string.Empty;
			rewards = new Rewards(rewardsString);
			if (rewards.RewardsList.Count == 0)
			{
				return TWDModelResult.Error;
			}
			List<object> list = rewards.Give(base.manager);
			for (int i = 0; i < rewards.RewardsList.Count; i++)
			{
				IReward reward = rewards.RewardsList[i];
				base.manager.Metrics.AddFind();
				if (list[i] is EquipmentItemModel equipment)
				{
					base.manager.Metrics.AddEquipment(equipment, "Equipment", (reward as RewardEquipment)?.Amount ?? 1);
				}
				else
				{
					base.manager.Metrics.AddReward(reward);
				}
				string text = ((base.manager.GameEconomyData.GetLeaderBoardRewardTypeByPosition(position, setId) == EndlessModeLeaderBoardRewardType.Percentage) ? "%" : "");
				base.manager.Player.EndlessModeManager.OverallScoreHistoryLog.TryGetValue(leaderboardId, out var value);
				base.manager.Metrics.AddEndlessCycle(leaderboardId, endlessModeLeaderBoardReward?.RewardBracket + text, value).Send();
			}
			return TWDModelResult.OK;
		}

		public void HandlePostMissionLogic()
		{
			if (PlayerModel.Combat.EndlessModeCombatModel.CombatResolved)
			{
				return;
			}
			EndlessModeAttemptData currentAttemptData = GetCurrentAttemptData();
			if (!currentAttemptData.Expired && currentAttemptData.Score > 0 && !IsNextEndlessCycle)
			{
				CommonAddEndlessAttemptData(currentAttemptData);
				EndlessParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
				if (currentAttemptData.GameModeType == EndlessModeGameModeType.Expert)
				{
					EndlessExpertParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
				}
				ReOrderAttemptDataByScores(EndlessModeReOrderType.All);
				if (ShouldUpdateLeaderBoardEntryForExpertMode(currentAttemptData))
				{
					OverAllScore = GetOverAllScoreForFinalScoreExpert();
					bool flag = UpdateLeaderBoardEntry();
					PendingLeaderBoardUpdate = !flag;
				}
			}
			int getCurrentOverAllWaveIndex = PlayerModel.Combat.EndlessModeCombatModel.GetCurrentOverAllWaveIndex;
			PendingAttemptRewards = GeneratePostMissionRewards(getCurrentOverAllWaveIndex, GetPostMissionRewardSetIdsByGameType());
			PendingAttemptRegularRewards = GeneratePostMissionRegularRewards(getCurrentOverAllWaveIndex, GetPostMissionRewardSetIdsByGameType());
			PlayerModel.Combat.EndlessModeCombatModel.CombatResolved = true;
		}

		public bool UpdateLeaderBoardEntry()
		{
			if (base.manager.ServerService != null && CurrentEndlessModeCalendarDefinition != null)
			{
				LeaderboardEntry entry = Leaderboards.CreateEndlessModeLeaderBoardEntry(base.manager.Player);
				string currentLeaderBoardName = CurrentLeaderBoardName;
				return base.manager.ServerService.TrySaveLeaderboardEntry(currentLeaderBoardName, entry);
			}
			return false;
		}

		private EndlessModeAttemptData GetCurrentAttemptData()
		{
			CombatModel combat = PlayerModel.Combat;
			if (combat != null)
			{
				List<SurvivorMockData> list = new List<SurvivorMockData>();
				List<SurvivorSupportData> list2 = new List<SurvivorSupportData>();
				for (int i = 0; i < combat.MissionRoster.Count; i++)
				{
					SurvivorModel survivorModel = combat.MissionRoster[i];
					if (survivorModel != null)
					{
						list.Add(survivorModel.CreateMockData());
					}
				}
				for (int j = 0; j < combat.SupportManager.Supports.Count; j++)
				{
					CombatSupportModel combatSupportModel = combat.SupportManager.Supports[j];
					if (combatSupportModel != null)
					{
						list2.Add(new SurvivorSupportData(combatSupportModel.SupportId, combatSupportModel.SlotIndex, combatSupportModel.SupportModel.Level - 1));
					}
				}
				return new EndlessModeAttemptData
				{
					WaveCount = combat.EndlessModeCombatModel.GetCurrentOverAllWaveIndex,
					MaxMultiplier = combat.EndlessModeCombatModel.MaxMultiplierReached,
					TimeStamp = PlayerModel.UtcTimeStamp,
					WalkersKilled = combat.MissionStatistics.WalkersKilled,
					Score = GetOverAllGameScore(combat.EndlessModeCombatModel.CurrentScore),
					SurvivorMockData = list,
					SurvivorSupportData = list2,
					Expired = IsNextEndlessCycle,
					GameModeType = EndlessModeGameModeType,
					IsScan = false
				};
			}
			return null;
		}

		public bool DoWeHaveRewardsUnclaimed()
		{
			if (IsLockedByCouncilLevel || CurrentEndlessModeCalendarDefinition == null)
			{
				return false;
			}
			int item = CurrentEndlessModeCalendarDefinition.Identifier - 1;
			bool flag = EndlessParticipationLog.Contains(item);
			bool flag2 = EndlessExpertParticipationLog.Contains(item);
			bool flag3 = !EndlessRewardsClaimedLog.Contains(item);
			return PlayerModel.UtcTimeStamp > CurrentEndlessModeCalendarDefinition.StartTimeMilliseconds + EndlessModeConfig.DelayUntilRewardsAreClaimed && flag && flag2 && flag3;
		}

		public bool DoWeHaveSurvivorClassRewardsUnclaimed()
		{
			if (!base.manager.GameEconomyData.ConfigData.EndlessExpertClassLeaderboardSwitch)
			{
				return false;
			}
			if (IsLockedByCouncilLevel || CurrentEndlessModeCalendarDefinition == null)
			{
				return false;
			}
			int num = CurrentEndlessModeCalendarDefinition.Identifier - 1;
			bool num2 = EndlessParticipationLog.Contains(num);
			bool flag = EndlessExpertParticipationLog.Contains(num);
			bool value = default(bool);
			bool flag2 = EndlessExpertLeaderSurvivorClassParticipationLog != null && EndlessExpertLeaderSurvivorClassParticipationLog.TryGetValue(num, out value) && value;
			bool flag3 = PlayerModel.UtcTimeStamp > CurrentEndlessModeCalendarDefinition.StartTimeMilliseconds + EndlessModeConfig.DelayUntilRewardsAreClaimed;
			bool flag4 = true;
			if (EndlessExpertLeaderSurvivorClassRewardsClaimedLog != null && EndlessExpertLeaderSurvivorClassRewardsClaimedLog.TryGetValue(num, out var value2) && value2 != null && value2.Count > 0)
			{
				flag4 = false;
			}
			return num2 && flag3 && flag && flag2 && flag4;
		}

		public bool AreWeInLockdownTimerBeforeRewardsAreGiven()
		{
			if (IsLockedByCouncilLevel || CurrentEndlessModeCalendarDefinition == null)
			{
				return false;
			}
			int num = CurrentEndlessModeCalendarDefinition.Identifier - 1;
			bool flag = EndlessParticipationLog.Contains(num);
			bool flag2 = EndlessExpertParticipationLog.Contains(num);
			bool flag3 = !EndlessRewardsClaimedLog.Contains(num);
			bool flag4 = false;
			if (EndlessExpertLeaderSurvivorClassAttemptData != null && EndlessExpertLeaderSurvivorClassAttemptData.Count > 0)
			{
				HashSet<SurvivorClass> hashSet = new HashSet<SurvivorClass>(EndlessExpertLeaderSurvivorClassAttemptData.Keys.Where((SurvivorClass c) => c != SurvivorClass.None));
				bool flag5 = false;
				if (hashSet.Count > 0 && EndlessExpertLeaderSurvivorClassRewardsClaimedLog != null && EndlessExpertLeaderSurvivorClassRewardsClaimedLog.TryGetValue(num, out var claimedClasses) && claimedClasses != null)
				{
					flag5 = hashSet.All((SurvivorClass c) => claimedClasses.Contains(c));
				}
				flag4 = !flag5;
			}
			if (PlayerModel.UtcTimeStamp > CurrentEndlessModeCalendarDefinition.StartTimeMilliseconds && PlayerModel.UtcTimeStamp < CurrentEndlessModeCalendarDefinition.StartTimeMilliseconds + EndlessModeConfig.DelayUntilRewardsAreClaimed && flag && flag2)
			{
				return flag3 || flag4;
			}
			return false;
		}

		public long GetMillisecondsUntilRewardsCanBeClaimed()
		{
			return CurrentEndlessModeCalendarDefinition.StartTimeMilliseconds + EndlessModeConfig.DelayUntilRewardsAreClaimed - PlayerModel.UtcTimeStamp;
		}

		public void UpdateLastClaimedEndlessPassTimeStamp()
		{
			DateTime playerDateTime = GetPlayerDateTime();
			DayOfWeek dayOfWeek = playerDateTime.DayOfWeek;
			DayOfWeek lastEndlessPassClaimDay = GetLastEndlessPassClaimDay(dayOfWeek);
			long lastEndlessPassClaimTimeStamp = new DateTime(playerDateTime.Year, playerDateTime.Month, playerDateTime.Day).TotalMilliseconds() + UtilsDateTime.HourInMilliseconds * EndlessModeConfig.PassRefresh - GetDaysBetweenTwoWeekDays(lastEndlessPassClaimDay, dayOfWeek) * UtilsDateTime.DayInMilliseconds;
			LastEndlessPassClaimTimeStamp = lastEndlessPassClaimTimeStamp;
		}

		private DayOfWeek GetLastEndlessPassClaimDay(DayOfWeek currentDay)
		{
			DateTime playerDateTime = GetPlayerDateTime();
			List<DayOfWeek> validRefreshDays = EndlessModeConfig.GetValidRefreshDays();
			validRefreshDays.OrderBy((DayOfWeek x) => (int)(x - 1 + 7) % 7);
			int orderedWeekDay = GetOrderedWeekDay(currentDay);
			int orderedWeekDay2 = GetOrderedWeekDay(validRefreshDays.LastOrDefault());
			int orderedWeekDay3 = GetOrderedWeekDay(validRefreshDays.FirstOrDefault());
			if (orderedWeekDay > orderedWeekDay2 || (orderedWeekDay == orderedWeekDay2 && playerDateTime.Hour >= EndlessModeConfig.PassRefresh))
			{
				return validRefreshDays.LastOrDefault();
			}
			if (orderedWeekDay3 == orderedWeekDay && playerDateTime.Hour < EndlessModeConfig.PassRefresh)
			{
				return validRefreshDays.LastOrDefault();
			}
			if (validRefreshDays.Contains(currentDay))
			{
				int num = validRefreshDays.FindIndex((DayOfWeek x) => x == currentDay);
				if (playerDateTime.Hour >= EndlessModeConfig.PassRefresh)
				{
					return validRefreshDays[num];
				}
				return validRefreshDays[num - 1];
			}
			for (int num2 = 0; num2 < validRefreshDays.Count; num2++)
			{
				if (validRefreshDays[num2] > currentDay)
				{
					return validRefreshDays[num2 - 1];
				}
			}
			return DayOfWeek.Monday;
		}

		private void AddEndlessPasses(int count)
		{
			CurrencyModel currency = PlayerModel.GetCurrency(CurrencyType.EndlessPassToken);
			int value = currency.Value;
			if (EndlessModeConfig.MaxPasses - currency.Value > 0)
			{
				int val = EndlessModeConfig.MaxPasses - currency.Value;
				currency.Add(Math.Min(count, val));
			}
			int actualAmountAdded = currency.Value - value;
			base.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassToken, EndlessModeConfig.PassesGivenPerRefresh, actualAmountAdded).AddEndlessRefresh()
				.Send();
		}

		private void AddEndlessExpertPasses(int count)
		{
			CurrencyModel currency = PlayerModel.GetCurrency(CurrencyType.EndlessPassExpertToken);
			int value = currency.Value;
			if (EndlessModeConfig.MaxEndlessPassExpertToken - currency.Value > 0)
			{
				int val = EndlessModeConfig.MaxEndlessPassExpertToken - currency.Value;
				currency.Add(Math.Min(count, val));
			}
			int actualAmountAdded = currency.Value - value;
			base.manager.Metrics.AddFind().AddResources(CurrencyType.EndlessPassExpertToken, EndlessModeConfig.EndlessExpertPassesGivenPerRefresh, actualAmountAdded).AddEndlessRefresh()
				.Send();
		}

		private DateTime GetPlayerDateTime()
		{
			DateTime utcTime = base.manager.Player.UtcTime;
			return new DateTime(utcTime.Year, utcTime.Month, utcTime.Day, utcTime.Hour, 0, 0);
		}

		private int GetDaysBetweenTwoWeekDays(DayOfWeek d1, DayOfWeek d2)
		{
			return (7 + (d2 - d1)) % 7;
		}

		private int GetOrderedWeekDay(DayOfWeek dayOfWeek)
		{
			return (int)(dayOfWeek + 6) % 7;
		}

		private int CalculateAccumulatedEndlessPassTokens()
		{
			int num = 0;
			int num2 = (int)((PlayerModel.UtcTimeStamp - LastEndlessPassClaimTimeStamp) / UtilsDateTime.DayInMilliseconds);
			long num3 = LastEndlessPassClaimTimeStamp;
			List<DayOfWeek> validRefreshDays = EndlessModeConfig.GetValidRefreshDays();
			for (int i = 0; i < num2; i++)
			{
				num3 += UtilsDateTime.DayInMilliseconds;
				DateTime dateTime = UtilsDateTime.MillisecondsToDateTime(num3);
				DayOfWeek dayOfWeek = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).DayOfWeek;
				if (validRefreshDays.Contains(dayOfWeek))
				{
					num++;
				}
			}
			return num;
		}

		private long GetNextEndlessBattlePassTimeStamp()
		{
			DateTime playerDateTime = GetPlayerDateTime();
			DayOfWeek dayOfWeek = playerDateTime.DayOfWeek;
			List<DayOfWeek> validRefreshDays = EndlessModeConfig.GetValidRefreshDays();
			DayOfWeek[] array = (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek));
			DateTime dateTime = new DateTime(playerDateTime.Year, playerDateTime.Month, playerDateTime.Day);
			foreach (DayOfWeek dayOfWeek2 in array)
			{
				if (validRefreshDays.Contains(dayOfWeek2))
				{
					if (dayOfWeek2 == dayOfWeek && playerDateTime.Hour < EndlessModeConfig.PassRefresh)
					{
						return dateTime.TotalMilliseconds() + UtilsDateTime.HourInMilliseconds * EndlessModeConfig.PassRefresh;
					}
					if (dayOfWeek2 > dayOfWeek)
					{
						return dateTime.TotalMilliseconds() + UtilsDateTime.HourInMilliseconds * EndlessModeConfig.PassRefresh + (dayOfWeek2 - dayOfWeek) * UtilsDateTime.DayInMilliseconds;
					}
				}
			}
			validRefreshDays.Sort();
			DayOfWeek d = validRefreshDays.FirstOrDefault();
			return dateTime.TotalMilliseconds() + UtilsDateTime.HourInMilliseconds * EndlessModeConfig.PassRefresh + GetDaysBetweenTwoWeekDays(dayOfWeek, d) * UtilsDateTime.DayInMilliseconds;
		}

		private void AddAndCalculateEndlessPasses(int claimableCount)
		{
			UpdateLastClaimedEndlessPassTimeStamp();
			AddEndlessPasses(claimableCount);
			CurrentGoldAttemptCount = 0;
			if (CompletedEndlessModeFTUE)
			{
				PlayerModel.Blackboard.ClearToggle("Toggle.ToggleEndlessModeHighlightExpired");
			}
		}

		private void AddAndCalculateEndlessExpertPasses(int claimableCount)
		{
			UpdateLastClaimedEndlessPassTimeStamp();
			AddEndlessExpertPasses(claimableCount);
			CurrentExpertGoldAttemptCount = 0;
			if (CompletedEndlessModeFTUE)
			{
				PlayerModel.Blackboard.ClearToggle("Toggle.ToggleEndlessModeHighlightExpired");
			}
		}

		private List<string> GetPostMissionRewardSetIdsByGameType()
		{
			List<string> list = new List<string>();
			if (EndlessModeGameModeType == EndlessModeGameModeType.Normal)
			{
				list.AddRange(CurrentEndlessModeCalendarDefinition.GetNormalWaveRewardSetIDs);
			}
			if (EndlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				list.AddRange(CurrentEndlessModeCalendarDefinition.GetExpertModeWaveRewardSetIDs);
			}
			return list;
		}

		public long GetOverAllGameScore(long score)
		{
			if (EndlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				if (score >= 1000000)
				{
					return GetOverflowScoreMultiplied(score, (double)EndlessModeConfig.ExpertModeTotalScoreMultiplier);
				}
				return (long)FixedPoint.Ceiling(score * EndlessModeConfig.ExpertModeTotalScoreMultiplier);
			}
			return score;
		}

		public long GetOverflowScoreMultiplied(long score, double multiplier)
		{
			int num = GetDecimalCount(multiplier) * 10;
			if (num == 0)
			{
				return score * (int)multiplier;
			}
			double num2 = score / 1000000;
			long num3 = (long)((double)score - 1000000.0 * num2);
			int num4 = (int)(multiplier * (double)num);
			num2 *= (double)num4;
			num2 /= (double)num;
			long num5 = (long)(num2 * 1000000.0);
			num3 *= num4;
			num3 = (long)FixedPoint.Ceiling((float)num3 / (float)num);
			return num5 + num3;
		}

		private int GetDecimalCount(double val)
		{
			int num = 0;
			while (val != Math.Floor(val))
			{
				val = (val - Math.Floor(val)) * 10.0;
				num++;
			}
			return num;
		}

		private void GenerateNewExpertModeActors()
		{
			CurrentExpertModeHeroes.Clear();
			List<EndlessModeExpertModeHeroDefinition> list = base.manager.Player.gameEconomyData.EndlessModeExpertModeHeroDefinitions.Where((EndlessModeExpertModeHeroDefinition x) => x.SetID == CurrentEndlessModeCalendarDefinition.ExpertModeHeroSetID).ToList();
			for (int num = 0; num < EndlessModeConfig.ExpertModeHeroAmount; num++)
			{
				EndlessModeExpertModeHeroDefinition randomExpertActorFromCollection = GetRandomExpertActorFromCollection(list);
				CurrentExpertModeHeroes.Add(randomExpertActorFromCollection);
				list.Remove(randomExpertActorFromCollection);
			}
		}

		private EndlessModeExpertModeHeroDefinition GetRandomExpertActorFromCollection(List<EndlessModeExpertModeHeroDefinition> possibleActors)
		{
			int num = possibleActors.Sum((EndlessModeExpertModeHeroDefinition x) => x.Weight);
			if (num == 0)
			{
				return null;
			}
			int randomInRange = new ModelRandom(1321616566).GetRandomInRange(1, num);
			int num2 = 0;
			foreach (EndlessModeExpertModeHeroDefinition possibleActor in possibleActors)
			{
				num2 += possibleActor.Weight;
				if (num2 >= randomInRange)
				{
					return possibleActor;
				}
			}
			return null;
		}

		public List<ActorDefinition> GetExpertModeActorDefinitions()
		{
			List<ActorDefinition> list = new List<ActorDefinition>();
			foreach (EndlessModeExpertModeHeroDefinition currentExpertModeHero in CurrentExpertModeHeroes)
			{
				ActorDefinition actorDefinition = base.manager.GameEconomyData.GetActorDefinition(currentExpertModeHero.HeroDefinitionID);
				if (actorDefinition != null)
				{
					list.Add(actorDefinition);
				}
			}
			return list;
		}

		public bool HasValidCombatActorsForExpertMode()
		{
			List<ActorDefinition> expertModeActorDefinitions = GetExpertModeActorDefinitions();
			int num = 0;
			foreach (SurvivorModel combatSurvivor in base.manager.Player.SurvivorContainer.CombatSurvivors)
			{
				if (!combatSurvivor.ActorDefinitionID.Contains("Hero"))
				{
					num++;
					continue;
				}
				for (int i = 0; i < expertModeActorDefinitions.Count; i++)
				{
					string iD = expertModeActorDefinitions[i].ID;
					if (combatSurvivor.ActorDefinitionID == iD)
					{
						num++;
					}
				}
			}
			return num == base.manager.Player.SurvivorContainer.CombatSurvivors.Count;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void ForceRegenerateExpertActors()
		{
			GenerateNewExpertModeActors();
		}

		public int GetMaxPasses()
		{
			if (UseSubscriptionConfig)
			{
				return base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxPasses;
			}
			return base.manager.Player.gameEconomyData.EndlessModeConfig.MaxPasses;
		}

		public int GetMaxExpertPasses()
		{
			if (UseSubscriptionConfig)
			{
				return base.manager.Player.gameEconomyData.EndlessModeConfig.SubscriptionMaxExpertPasses;
			}
			return base.manager.Player.gameEconomyData.EndlessModeConfig.MaxEndlessPassExpertToken;
		}

		public List<DifficultyIncrementalDebuff> GetEndlessModeExpertDebuffConfigs()
		{
			Dictionary<int, EndlessModeExpertDebuffConfig> endlessModeExpertDebuffConfigById = base.manager.Player.gameEconomyData.EndlessModeExpertDebuffConfigById;
			int num = 0;
			if (PlayerModel?.Combat?.EndlessModeCombatModel != null)
			{
				num = PlayerModel.Combat.EndlessModeCombatModel.CurrentWaveIndex;
			}
			if (num <= 0)
			{
				return new List<DifficultyIncrementalDebuff>();
			}
			return endlessModeExpertDebuffConfigById[num].EndLessDebuffs;
		}

		private void CommonAddEndlessAttemptData(EndlessModeAttemptData endlessModeAttemptData)
		{
			EndlessAttemptData.Add(endlessModeAttemptData);
			if (endlessModeAttemptData.GameModeType == EndlessModeGameModeType.Normal)
			{
				EndlessNormalAttemptData.Add(endlessModeAttemptData);
			}
			if (endlessModeAttemptData.GameModeType == EndlessModeGameModeType.Expert)
			{
				EndlessExpertAttemptData.Add(endlessModeAttemptData);
				TryAddExpertLeaderClassAttempt(endlessModeAttemptData);
			}
		}

		public bool CheckCanScanNormal()
		{
			long num = base.manager.GameEconomyData.EndlessModeNormalRewardDefinitons.Max((EndlessModeNormalRewardDefiniton reward) => reward.Score);
			if (base.manager.Player.EndlessModeManager.GetOverAllScoreForFinalScoreNormal() < num)
			{
				return true;
			}
			if (EndlessNormalAttemptData.Count > 0)
			{
				return !IsNextEndlessCycle;
			}
			return false;
		}

		public bool CheckCanScanExpert()
		{
			if (EndlessExpertAttemptData.Count > 0)
			{
				return !IsNextEndlessCycle;
			}
			return false;
		}

		public void DebugAddAttempDataNormal(int wave, int score, int walkerKilled)
		{
			EndlessModeAttemptData endlessModeAttemptData = EndlessNormalAttemptData.Where((EndlessModeAttemptData x) => !x.IsScan).Max();
			EndlessModeAttemptData endlessModeAttemptData2 = new EndlessModeAttemptData
			{
				WaveCount = wave,
				MaxMultiplier = endlessModeAttemptData.MaxMultiplier,
				TimeStamp = PlayerModel.UtcTimeStamp,
				WalkersKilled = walkerKilled,
				Score = score,
				SurvivorMockData = endlessModeAttemptData.SurvivorMockData,
				SurvivorSupportData = endlessModeAttemptData.SurvivorSupportData,
				Expired = endlessModeAttemptData.Expired,
				GameModeType = endlessModeAttemptData.GameModeType,
				IsScan = false
			};
			CommonAddEndlessAttemptData(endlessModeAttemptData2);
			EndlessParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
			ReOrderAttemptDataByScores(EndlessModeReOrderType.Normal);
		}

		public void DebugAddAttempDataExpert(int wave, int score, int walkerKilled)
		{
			EndlessModeAttemptData endlessModeAttemptData = EndlessExpertAttemptData.Where((EndlessModeAttemptData x) => !x.IsScan).Max();
			EndlessModeAttemptData endlessModeAttemptData2 = new EndlessModeAttemptData
			{
				WaveCount = wave,
				MaxMultiplier = endlessModeAttemptData.MaxMultiplier,
				TimeStamp = PlayerModel.UtcTimeStamp,
				WalkersKilled = walkerKilled,
				Score = score,
				SurvivorMockData = endlessModeAttemptData.SurvivorMockData,
				SurvivorSupportData = endlessModeAttemptData.SurvivorSupportData,
				Expired = endlessModeAttemptData.Expired,
				GameModeType = endlessModeAttemptData.GameModeType,
				IsScan = false
			};
			CommonAddEndlessAttemptData(endlessModeAttemptData2);
			EndlessParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
			EndlessExpertParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
			ReOrderAttemptDataByScores(EndlessModeReOrderType.Expert);
			if (ShouldUpdateLeaderBoardEntryForExpertMode(endlessModeAttemptData2))
			{
				OverAllScore = GetOverAllScoreForFinalScoreExpert();
				bool flag = UpdateLeaderBoardEntry();
				PendingLeaderBoardUpdate = !flag;
			}
		}

		private bool ShouldUpdateLeaderBoardEntryForExpertMode(EndlessModeAttemptData endlessModeAttemptData)
		{
			if (endlessModeAttemptData.GameModeType == EndlessModeGameModeType.Normal)
			{
				return false;
			}
			return OverAllScore < GetOverAllScoreForFinalScoreExpert();
		}

		public int GetExpertModeAttemptEntryCount()
		{
			int attemptsToSumForFinalScoreExpert = EndlessModeConfig.AttemptsToSumForFinalScoreExpert;
			return (from x in EndlessExpertAttemptData.Take(attemptsToSumForFinalScoreExpert)
				where x.GameModeType == EndlessModeGameModeType.Expert
				select x).ToList().Count;
		}

		private void ReOrderAttemptDataByScores(EndlessModeReOrderType endlessModeReOrderType)
		{
			EndlessAttemptData.StableSort(delegate(EndlessModeAttemptData entry1, EndlessModeAttemptData entry2)
			{
				long score = entry1.Score;
				long score2 = entry2.Score;
				return (score <= score2) ? 1 : (-1);
			});
			if (endlessModeReOrderType == EndlessModeReOrderType.All || endlessModeReOrderType == EndlessModeReOrderType.Normal)
			{
				EndlessNormalAttemptData.StableSort(delegate(EndlessModeAttemptData entry1, EndlessModeAttemptData entry2)
				{
					long score = entry1.Score;
					long score2 = entry2.Score;
					return (score <= score2) ? 1 : (-1);
				});
			}
			if (endlessModeReOrderType == EndlessModeReOrderType.All || endlessModeReOrderType == EndlessModeReOrderType.Expert)
			{
				EndlessExpertAttemptData.StableSort(delegate(EndlessModeAttemptData entry1, EndlessModeAttemptData entry2)
				{
					long score = entry1.Score;
					long score2 = entry2.Score;
					return (score <= score2) ? 1 : (-1);
				});
			}
		}

		public void ScanNormal()
		{
			EndlessModeAttemptData endlessModeAttemptData = EndlessNormalAttemptData.Where((EndlessModeAttemptData x) => !x.IsScan).Max();
			EndlessModeAttemptData endlessModeAttemptData2 = new EndlessModeAttemptData
			{
				WaveCount = endlessModeAttemptData.WaveCount,
				MaxMultiplier = endlessModeAttemptData.MaxMultiplier,
				TimeStamp = PlayerModel.UtcTimeStamp,
				WalkersKilled = endlessModeAttemptData.WalkersKilled,
				Score = endlessModeAttemptData.Score,
				SurvivorMockData = endlessModeAttemptData.SurvivorMockData,
				SurvivorSupportData = endlessModeAttemptData.SurvivorSupportData,
				Expired = endlessModeAttemptData.Expired,
				GameModeType = endlessModeAttemptData.GameModeType,
				IsScan = true
			};
			CommonAddEndlessAttemptData(endlessModeAttemptData2);
			EndlessParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
			ReOrderAttemptDataByScores(EndlessModeReOrderType.Normal);
			int waveCount = endlessModeAttemptData2.WaveCount;
			PendingAttemptRewards = GeneratePostMissionRewards(waveCount, GetPostMissionRewardSetIdsByGameType());
			PendingAttemptRegularRewards = GeneratePostMissionRegularRewards(waveCount, GetPostMissionRewardSetIdsByGameType());
		}

		public void ScanExpert()
		{
			EndlessModeAttemptData endlessModeAttemptData = EndlessExpertAttemptData.Where((EndlessModeAttemptData x) => !x.IsScan).Max();
			EndlessModeAttemptData endlessModeAttemptData2 = new EndlessModeAttemptData
			{
				WaveCount = endlessModeAttemptData.WaveCount,
				MaxMultiplier = endlessModeAttemptData.MaxMultiplier,
				TimeStamp = PlayerModel.UtcTimeStamp,
				WalkersKilled = endlessModeAttemptData.WalkersKilled,
				Score = endlessModeAttemptData.Score,
				SurvivorMockData = endlessModeAttemptData.SurvivorMockData,
				SurvivorSupportData = endlessModeAttemptData.SurvivorSupportData,
				Expired = endlessModeAttemptData.Expired,
				GameModeType = endlessModeAttemptData.GameModeType,
				IsScan = true
			};
			CommonAddEndlessAttemptData(endlessModeAttemptData2);
			EndlessParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
			EndlessExpertParticipationLog.Add(CurrentEndlessModeCalendarDefinition.Identifier);
			ReOrderAttemptDataByScores(EndlessModeReOrderType.Expert);
			if (ShouldUpdateLeaderBoardEntryForExpertMode(endlessModeAttemptData2))
			{
				OverAllScore = GetOverAllScoreForFinalScoreExpert();
				bool flag = UpdateLeaderBoardEntry();
				PendingLeaderBoardUpdate = !flag;
			}
			int waveCount = endlessModeAttemptData2.WaveCount;
			PendingAttemptRewards = GeneratePostMissionRewards(waveCount, GetPostMissionRewardSetIdsByGameType());
			PendingAttemptRegularRewards = GeneratePostMissionRegularRewards(waveCount, GetPostMissionRewardSetIdsByGameType());
		}

		private long GetOverAllScoreForFinalScoreExpert()
		{
			long num = 0L;
			for (int i = 0; i < Math.Min(EndlessExpertAttemptData.Count, EndlessModeConfig.AttemptsToSumForFinalScoreExpert); i++)
			{
				if (!EndlessExpertAttemptData[i].Expired)
				{
					long score = EndlessExpertAttemptData[i].Score;
					num += score;
				}
			}
			return num;
		}

		public long GetOverAllScoreForFinalScoreNormal()
		{
			long num = 0L;
			for (int i = 0; i < Math.Min(EndlessNormalAttemptData.Count, EndlessModeConfig.AttemptsToSumForFinalScoreNormal); i++)
			{
				if (!EndlessNormalAttemptData[i].Expired)
				{
					long score = EndlessNormalAttemptData[i].Score;
					num += score;
				}
			}
			return num;
		}

		public TWDModelResult GiveAttemptNormalProgressRewards(out Rewards progressRewards)
		{
			progressRewards = new Rewards();
			List<EndlessModeNormalRewardDefiniton> getOrderedEndlessModeNormalRewardsDefinitions = GetOrderedEndlessModeNormalRewardsDefinitions;
			if (getOrderedEndlessModeNormalRewardsDefinitions == null || getOrderedEndlessModeNormalRewardsDefinitions.Count == 0)
			{
				return TWDModelResult.Error;
			}
			long overAllScoreForFinalScoreNormal = GetOverAllScoreForFinalScoreNormal();
			foreach (EndlessModeNormalRewardDefiniton item in getOrderedEndlessModeNormalRewardsDefinitions)
			{
				if (item.Score > overAllScoreForFinalScoreNormal)
				{
					break;
				}
				if (ClaimedNormalProgressRewardIndex.Contains(item.RewardIndex))
				{
					continue;
				}
				Rewards rewards = new Rewards(item.Rewards);
				List<object> list = rewards.Give(base.manager);
				for (int i = 0; i < rewards.RewardsList.Count; i++)
				{
					IReward reward = rewards.RewardsList[i];
					base.manager.Metrics.ResourceChangeObtainReason = "LastStandScoreRewardNormal";
					base.manager.Metrics.AddFind();
					if (list[i] is EquipmentItemModel equipment)
					{
						base.manager.Metrics.AddEquipment(equipment, "Equipment", (reward as RewardEquipment)?.Amount ?? 1);
					}
					else
					{
						base.manager.Metrics.AddReward(reward);
					}
					base.manager.Metrics.AddMission().AddEndless(EndlessModeGameModeType.ToString()).AddEndlessModeNormalProgressReward(item.RewardIndex)
						.Send();
					progressRewards.RewardsList.Add(reward);
				}
				ClaimedNormalProgressRewardIndex.Add(item.RewardIndex);
			}
			if (progressRewards.RewardsList.Count != 0)
			{
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}

		private void InitLeaderSurvivorClassAttempts()
		{
			if (EndlessExpertLeaderSurvivorClassAttemptData == null)
			{
				EndlessExpertLeaderSurvivorClassAttemptData = new Dictionary<SurvivorClass, List<EndlessModeAttemptData>>();
			}
			if (EndlessExpertLeaderSurvivorClassRewardsClaimedLog == null)
			{
				EndlessExpertLeaderSurvivorClassRewardsClaimedLog = new Dictionary<int, HashSet<SurvivorClass>>();
			}
			if (EndlessExpertLeaderSurvivorClassParticipationLog == null)
			{
				EndlessExpertLeaderSurvivorClassParticipationLog = new Dictionary<int, bool>();
			}
			if (PendingLeaderSurvivorClassUpdate == null)
			{
				PendingLeaderSurvivorClassUpdate = new HashSet<SurvivorClass>();
			}
		}

		private void RetryPendingLeaderSurvivorClassUpdate()
		{
			if (!base.manager.GameEconomyData.ConfigData.EndlessExpertClassLeaderboardSwitch || PendingLeaderSurvivorClassUpdate == null || PendingLeaderSurvivorClassUpdate.Count == 0 || base.manager.ServerService == null || CurrentEndlessModeCalendarDefinition == null || EndlessExpertLeaderSurvivorClassAttemptData == null)
			{
				return;
			}
			foreach (SurvivorClass item in PendingLeaderSurvivorClassUpdate.ToList())
			{
				if (!EndlessExpertLeaderSurvivorClassAttemptData.TryGetValue(item, out var value) || value == null || value.Count == 0)
				{
					PendingLeaderSurvivorClassUpdate.Remove(item);
					continue;
				}
				EndlessModeAttemptData endlessModeAttemptData = value.OrderByDescending((EndlessModeAttemptData a) => a.Score).FirstOrDefault();
				if (endlessModeAttemptData == null || endlessModeAttemptData.Score <= 0)
				{
					PendingLeaderSurvivorClassUpdate.Remove(item);
				}
				else if (UpdateLeaderSurvivorClassEntry(item, endlessModeAttemptData))
				{
					PendingLeaderSurvivorClassUpdate.Remove(item);
				}
			}
		}

		private bool UpdateLeaderSurvivorClassEntry(SurvivorClass survivorClass, EndlessModeAttemptData attemptData)
		{
			if (base.manager.ServerService == null || CurrentEndlessModeCalendarDefinition == null || base.manager.Player == null)
			{
				return false;
			}
			LeaderboardEntry leaderboardEntry = Leaderboards.CreateEndlessModeLeaderBoardEntryByLeaderSurvivorClass(base.manager.Player, survivorClass, attemptData, attemptData.Score);
			if (leaderboardEntry == null)
			{
				return false;
			}
			string endlessModeLeaderboardNameByClass = Leaderboards.GetEndlessModeLeaderboardNameByClass(CurrentEndlessModeCalendarDefinition.Identifier, survivorClass);
			try
			{
				return base.manager.ServerService.TrySaveLeaderboardEntry(endlessModeLeaderboardNameByClass, leaderboardEntry);
			}
			catch
			{
				return false;
			}
		}

		private void ResetLeaderSurvivorClassAttempts()
		{
			EndlessExpertLeaderSurvivorClassAttemptData?.Clear();
		}

		private void TryAddExpertLeaderClassAttempt(EndlessModeAttemptData endlessModeExpertAttemptData)
		{
			if (!base.manager.GameEconomyData.ConfigData.EndlessExpertClassLeaderboardSwitch || endlessModeExpertAttemptData == null || endlessModeExpertAttemptData.SurvivorMockData == null || endlessModeExpertAttemptData.SurvivorMockData.Count == 0 || endlessModeExpertAttemptData.Expired || endlessModeExpertAttemptData.Score <= 0 || endlessModeExpertAttemptData.GameModeType != EndlessModeGameModeType.Expert)
			{
				return;
			}
			SurvivorMockData survivorMockData = endlessModeExpertAttemptData.SurvivorMockData[0];
			if (survivorMockData == null)
			{
				return;
			}
			SurvivorClass survivorClass = survivorMockData.SurvivorClass;
			if (survivorClass == SurvivorClass.None)
			{
				return;
			}
			if (PendingLeaderSurvivorClassUpdate == null)
			{
				PendingLeaderSurvivorClassUpdate = new HashSet<SurvivorClass>();
			}
			if (EndlessExpertLeaderSurvivorClassAttemptData == null)
			{
				EndlessExpertLeaderSurvivorClassAttemptData = new Dictionary<SurvivorClass, List<EndlessModeAttemptData>>();
			}
			if (!EndlessExpertLeaderSurvivorClassAttemptData.TryGetValue(survivorClass, out var value))
			{
				value = new List<EndlessModeAttemptData>();
				EndlessExpertLeaderSurvivorClassAttemptData[survivorClass] = value;
			}
			long num = ((value.Count == 0) ? 0 : (from a in value
				where a != null
				select a.Score).DefaultIfEmpty(0L).Max());
			value.Add(endlessModeExpertAttemptData);
			if (CurrentEndlessModeCalendarDefinition != null)
			{
				if (EndlessExpertLeaderSurvivorClassParticipationLog == null)
				{
					EndlessExpertLeaderSurvivorClassParticipationLog = new Dictionary<int, bool>();
				}
				EndlessExpertLeaderSurvivorClassParticipationLog[CurrentEndlessModeCalendarDefinition.Identifier] = true;
			}
			if (endlessModeExpertAttemptData.Score > num)
			{
				if (!UpdateLeaderSurvivorClassEntry(survivorClass, endlessModeExpertAttemptData))
				{
					PendingLeaderSurvivorClassUpdate.Add(survivorClass);
				}
				else
				{
					PendingLeaderSurvivorClassUpdate.Remove(survivorClass);
				}
			}
		}

		public TWDModelResult GiveExpertLeaderSurvivorClassLeaderBoardRewards(out Rewards rewards, List<SurvivorClassLeaderboardInfo> survivorClassLeaderboardInfos, string setId, int leaderboardId)
		{
			string empty = string.Empty;
			rewards = new Rewards(empty);
			if (survivorClassLeaderboardInfos == null || survivorClassLeaderboardInfos.Count == 0)
			{
				return TWDModelResult.Error;
			}
			if (base.manager == null || base.manager.GameEconomyData == null)
			{
				return TWDModelResult.Error;
			}
			List<string> list = new List<string>();
			List<SurvivorClass> list2 = new List<SurvivorClass>();
			foreach (SurvivorClassLeaderboardInfo survivorClassLeaderboardInfo in survivorClassLeaderboardInfos)
			{
				if (survivorClassLeaderboardInfo != null)
				{
					EndlessModeLeaderSurvivorClassLeaderBoardReward endlessModeLeaderSurvivorClassLeaderBoardReward = base.manager.GameEconomyData.GetEndlessModeLeaderSurvivorClassLeaderBoardReward(setId, survivorClassLeaderboardInfo.LeaderBoardPosition, survivorClassLeaderboardInfo.LeaderBoardEntryCount, survivorClassLeaderboardInfo.SurvivorClass);
					if (!string.IsNullOrEmpty(endlessModeLeaderSurvivorClassLeaderBoardReward?.Rewards))
					{
						string rewards2 = endlessModeLeaderSurvivorClassLeaderBoardReward.Rewards;
						list.Add(rewards2);
						list2.Add(survivorClassLeaderboardInfo.SurvivorClass);
					}
				}
			}
			empty = string.Join(";", list.Select((string r) => r.Trim(';')));
			rewards = new Rewards(empty);
			if (rewards.RewardsList.Count == 0)
			{
				return TWDModelResult.Error;
			}
			List<object> list3 = rewards.Give(base.manager);
			if (base.manager.Metrics != null && list3 != null)
			{
				for (int num = 0; num < rewards.RewardsList.Count && num < list3.Count; num++)
				{
					IReward reward = rewards.RewardsList[num];
					base.manager.Metrics.AddFind();
					if (list3[num] is EquipmentItemModel equipment)
					{
						base.manager.Metrics.AddEquipment(equipment, "Equipment", (reward as RewardEquipment)?.Amount ?? 1);
					}
					else
					{
						base.manager.Metrics.AddReward(reward);
					}
					base.manager.Metrics.Send();
				}
			}
			if (EndlessExpertLeaderSurvivorClassRewardsClaimedLog == null)
			{
				EndlessExpertLeaderSurvivorClassRewardsClaimedLog = new Dictionary<int, HashSet<SurvivorClass>>();
			}
			if (!EndlessExpertLeaderSurvivorClassRewardsClaimedLog.TryGetValue(leaderboardId, out var value))
			{
				value = new HashSet<SurvivorClass>();
				EndlessExpertLeaderSurvivorClassRewardsClaimedLog[leaderboardId] = value;
			}
			foreach (SurvivorClass item in list2)
			{
				value.Add(item);
			}
			if (base.manager.Metrics != null)
			{
				foreach (SurvivorClassLeaderboardInfo survivorClassLeaderboardInfo2 in survivorClassLeaderboardInfos)
				{
					base.manager.Metrics.AddFind();
					base.manager.Metrics.AddEndlessLeaderSurvivorClassCycle(leaderboardId, survivorClassLeaderboardInfo2.SurvivorClass, survivorClassLeaderboardInfo2.LeaderBoardPosition, survivorClassLeaderboardInfo2.LeaderBoardEntryCount).Send();
				}
			}
			return TWDModelResult.OK;
		}
	}
}
