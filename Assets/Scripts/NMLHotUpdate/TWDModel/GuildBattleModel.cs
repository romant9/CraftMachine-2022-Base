using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;
using TwdCustomMod;

namespace TWDModel
{
	public class GuildBattleModel : TWDGroupModelChild, ILeaderboardState
	{
		public enum GuildBattleState
		{
			NotActive = 0,
			Started = 1,
			Ended = 2
		}

		public enum GuildBattleResult
		{
			NotEnded = 0,
			Victory = 1,
			Defeat = 2,
			Draw = 3
		}

		[Serializable]
		public class LiveMissionData
		{
			public string LastAttackedMissionId;

			public GuildBattleMapMissionModel.MissionState MissionState;
		}

		[Serializable]
		public class GuildBattleIndicatorData
		{
			public int SectorId;

			public int X;

			public int Y;

			public long UtcTimeStamp;

			public string PlayerHashedId;

			public GuildBattleIndicatorData()
			{
			}

			public GuildBattleIndicatorData(int sectorId, int x, int y)
			{
				SectorId = sectorId;
				X = x;
				Y = y;
			}
		}

		public const string GuildRewardTraitBonuseAdded = "GuildRewardTraitBonuseAdded";

		public const string GuildVictoryPointsAdded = "GuildVictoryPointsAdded";

		public const string GuildBattleLiveDataUpdated = "GuildBattleLiveDataUpdated";

		public const string GuildBattleMapIndicatorsUpdated = "GuildBattleMapIndicatorsUpdated";

		public const string GuildBattleScoresUpdated = "GuildBattleScoresUpdated";

		public string BattleId;

		[JsonIgnore]
		public string guildId;

		public bool IsFakeBattle;

		public long LastLeaderboardsUpdateTimestamp;

		public GuildBattleResult BattleResult;

		public List<string> RegisteredPlayers;

		private Dictionary<int, Dictionary<int, Rewards>> selectedRewards;

		private Dictionary<string, GuildBattleReward> orderedGuildBattleRewardsLookup;

		public long TimeSlot { get; set; }

		public int WarId { get; set; }

		public long BattleDurationInMilliseconds { get; set; }

		public int FinalVictoryPoints { get; private set; }

		public int GuildTier { get; private set; }

		public FixedPoint GuildAdjustedLevel { get; set; }

		public FixedPoint GuildActualLevel { get; set; }

		public GuildBattleState CurrentState { get; private set; }

		public GuildBattleMatchmakingInfo EnemyGuildData { get; set; }

		public GuildBattleMapModel CurrentMapModel { get; private set; }

		public List<int> CompletedSectors { get; set; }

		public Dictionary<string, int> NumberOfAttacksPerPlayer { get; private set; }

		public int EndEnemyVictoryPoints { get; private set; }

		public int EndVictoryPoints { get; private set; }

		public Dictionary<string, int> VictoryPointsPerPlayer { get; private set; }

		public Dictionary<int, int> VictoryPointsSectorRewardPerSector { get; private set; }

		public Dictionary<string, LiveMissionData> LiveMissionDataPerPlayer { get; private set; }

		public Dictionary<int, List<string>> CollectedBattleBonusesPerSector { get; set; }

		public Dictionary<string, GuildBattleIndicatorData> GuildBattleEmblemDataPerPlayer { get; set; }

		public Dictionary<string, List<string>> CompletedMissionsPerPlayer { get; set; }

		public bool LeaderboardUpdated { get; set; } = true;

		[JsonIgnore]
		public List<ScoreDataEntry> GuildScores { get; private set; }

		[JsonIgnore]
		public List<ScoreDataEntry> PlayerHighscores { get; private set; }

		[JsonIgnore]
		public Dictionary<string, GuildBattleParticipantInfo> EnemyPlayersInfoList => EnemyGuildData.PlayerInfoSnapshot;

		[JsonIgnore]
		public int EnemyGuildTier => EnemyGuildData.Tier;

		[JsonIgnore]
		public string EnemyGuildName => EnemyGuildData.GuildName;

		[JsonIgnore]
		public long EndBattleTimestamp => TimeSlot + BattleDurationInMilliseconds;

		[JsonIgnore]
		public GuildWarDefinition CurrentWarDefinition => base.gameEconomyData.FindGuildWarWithId(WarId);

		[JsonIgnore]
		public int RegisteredPlayersCount => RegisteredPlayers.Count;

		public string LeaderboardName => Leaderboards.GetGuildBattleLiveScoreLeaderboardName(BattleId, CurrentMapModel.RandomSeed);

		public bool IsOngoing(long utcTimeStamp)
		{
			if (HasStarted())
			{
				return !IsBiggerThanEndBattleTimeStamp(utcTimeStamp);
			}
			return false;
		}

		public bool IsBiggerThanEndBattleTimeStamp(long utcTimeStamp)
		{
			return utcTimeStamp >= EndBattleTimestamp;
		}

		public bool HasStarted()
		{
			return CurrentState == GuildBattleState.Started;
		}

		public bool HasEnded()
		{
			return CurrentState == GuildBattleState.Ended;
		}

		public bool IsVictory()
		{
			return BattleResult == GuildBattleResult.Victory;
		}

		public bool IsDefeat()
		{
			return BattleResult == GuildBattleResult.Defeat;
		}

		public bool IsDraw()
		{
			return BattleResult == GuildBattleResult.Draw;
		}

		public GuildBattleModel()
		{
			WarId = -1;
			EnemyGuildData = new GuildBattleMatchmakingInfo();
			CollectedBattleBonusesPerSector = new Dictionary<int, List<string>>();
			GuildBattleEmblemDataPerPlayer = new Dictionary<string, GuildBattleIndicatorData>();
			CompletedMissionsPerPlayer = new Dictionary<string, List<string>>();
			CurrentMapModel = new GuildBattleMapModel();
			VictoryPointsPerPlayer = new Dictionary<string, int>();
			VictoryPointsSectorRewardPerSector = new Dictionary<int, int>();
			LiveMissionDataPerPlayer = new Dictionary<string, LiveMissionData>();
			CompletedSectors = new List<int>();
			BattleResult = GuildBattleResult.NotEnded;
			CurrentState = GuildBattleState.NotActive;
			RegisteredPlayers = new List<string>();
			NumberOfAttacksPerPlayer = new Dictionary<string, int>();
			selectedRewards = new Dictionary<int, Dictionary<int, Rewards>>();
			orderedGuildBattleRewardsLookup = new Dictionary<string, GuildBattleReward>();
			LeaderboardUpdated = true;
		}

		public int GetTotalVictoryPointsForPlayer(string playerHashedId)
		{
			if (VictoryPointsPerPlayer == null)
			{
				return 0;
			}
			VictoryPointsPerPlayer.TryGetValue(playerHashedId, out var value);
			return value;
		}

		public int CalculateTotalVictoryPoints(bool includeSectorBonus = true)
		{
			if (VictoryPointsPerPlayer == null)
			{
				return 0;
			}
			int num = VictoryPointsPerPlayer.Sum((KeyValuePair<string, int> keyValue) => keyValue.Value);
			if (!includeSectorBonus || VictoryPointsSectorRewardPerSector == null)
			{
				return num;
			}
			return num + VictoryPointsSectorRewardPerSector.Sum((KeyValuePair<int, int> keyValue) => keyValue.Value);
		}

		public override void Start()
		{
			base.Start();
			if (EnemyGuildData != null)
			{
				EnemyGuildData.Start();
				RecalculateBattleDifficultyAndPvpTeams(EnemyGuildData.PlayerInfoSnapshot);
			}
			RewardSetup();
		}

		public void StartBattle(int warId, int randomSeed, long timeSlot, GuildBattleMatchmakingInfo guildData, GuildBattleMatchmakingInfo enemyGuildData, int guildTier, TWDModelManager manager, bool isFakeBattle = false)
		{
			if (CurrentMapModel != null && CurrentMapModel.RandomSeed != -1)
			{
				CurrentMapModel.Reset();
			}
			Reset();
			EnemyGuildData = enemyGuildData;
			EnemyGuildData.Start();
			BattleDurationInMilliseconds = base.gameEconomyData.GuildWarConfig.GuildBattleDurationMilliseconds;
			WarId = warId;
			TimeSlot = timeSlot;
			GuildTier = guildTier;
			CurrentState = GuildBattleState.Started;
			BattleResult = GuildBattleResult.NotEnded;
			Tuple<FixedPoint, FixedPoint> tuple = GvGModelHelper.CalculateGuildLevel(guildData.PlayerInfoSnapshot);
			GuildAdjustedLevel = tuple.First;
			GuildActualLevel = tuple.Second;
			IsFakeBattle = isFakeBattle;
			LeaderboardUpdated = true;
			MapSetup(warId, randomSeed, EnemyPlayersInfoList, manager);
			string groupId = guildData.GroupId;
			string groupId2 = enemyGuildData.GroupId;
			guildId = groupId;
			if (isFakeBattle || groupId.CompareTo(enemyGuildData.GroupId) == 1)
			{
				BattleId = TimeSlot + "_" + groupId + "_" + (isFakeBattle ? "Fake" : groupId2);
			}
			else
			{
				BattleId = TimeSlot + "_" + groupId2 + "_" + groupId;
			}
			if (IsFakeBattle)
			{
				InitializeFakeBattleTargetScore(manager);
			}
		}

		public void SetEndVictoryPoints(int vp, int enemyVp)
		{
			EndEnemyVictoryPoints = enemyVp;
			EndVictoryPoints = CalculateTotalVictoryPoints();
			if (EndVictoryPoints != vp)
			{
				base.Debug.LogWarning("VP points from the leaderboard mismatch from the ones calculated: server = " + EndVictoryPoints + ", calculated = " + FinalVictoryPoints);
				if (vp != 0)
				{
					EndVictoryPoints = vp;
				}
			}
		}

		public void EndBattle()
		{
			CurrentState = GuildBattleState.Ended;
			if (EndVictoryPoints > EndEnemyVictoryPoints)
			{
				BattleResult = GuildBattleResult.Victory;
			}
			else if (EndVictoryPoints < EndEnemyVictoryPoints)
			{
				BattleResult = GuildBattleResult.Defeat;
			}
			else
			{
				BattleResult = GuildBattleResult.Draw;
			}
			FinalVictoryPoints = EndVictoryPoints;
			if (IsVictory())
			{
				FinalVictoryPoints += (int)((float)EndVictoryPoints * GetGuildBattleVictoryPointsMultiplier());
			}
			if (IsDraw())
			{
				FinalVictoryPoints += (int)((float)EndVictoryPoints * GetGuildBattleDrawPointsMultiplier());
			}
		}

		public void Reset()
		{
			CollectedBattleBonusesPerSector.Clear();
			GuildBattleEmblemDataPerPlayer.Clear();
			CompletedMissionsPerPlayer.Clear();
			CompletedSectors.Clear();
			VictoryPointsPerPlayer.Clear();
			LiveMissionDataPerPlayer.Clear();
			VictoryPointsSectorRewardPerSector.Clear();
			selectedRewards.Clear();
			orderedGuildBattleRewardsLookup.Clear();
			NumberOfAttacksPerPlayer.Clear();
			FinalVictoryPoints = 0;
			EndVictoryPoints = 0;
			EndEnemyVictoryPoints = 0;
			TimeSlot = 0L;
			CurrentState = GuildBattleState.NotActive;
			BattleResult = GuildBattleResult.NotEnded;
			GuildTier = 0;
			IsFakeBattle = false;
			GuildScores = null;
			PlayerHighscores = null;
			LeaderboardUpdated = true;
		}

		private void MapSetup(int warId, int randomSeed, Dictionary<string, GuildBattleParticipantInfo> players, TWDModelManager manager)
		{
			if (randomSeed != -1 && warId != -1)
			{
				CurrentMapModel = new GuildBattleMapModel();
				CurrentMapModel.SetPlayerOwnerAndGameEconomyData(base.gameEconomyData, base.root, manager.Player);
				CurrentMapModel.SetupMap(randomSeed, warId);
				RecalculateBattleDifficultyAndPvpTeams(players, manager);
				CurrentMapModel.SetupMissionAndPvpPlacement();
				RewardSetup();
			}
		}

		private void RecalculateBattleDifficultyAndPvpTeams(Dictionary<string, GuildBattleParticipantInfo> players, TWDModelManager debugManager = null)
		{
			CurrentMapModel.AssingPVPTeams(players, debugManager);
			ParsePersistentPvpPlacement();
		}

		private void ParsePersistentPvpPlacement()
		{
			foreach (KeyValuePair<string, string> item in CurrentMapModel.PvpTeamsIndexPerMission)
			{
				GuildBattleMapModel.ParsePvpTeamIndexId(item.Value, out var sectorId, out var index);
				if (CurrentMapModel.PVPTeamsListPerSector.TryGetValue(sectorId, out var value) && value.Count > index)
				{
					value[index].MissionId = item.Key;
				}
			}
		}

		public GuildBattleParticipantInfo GetCurrentGuildBattlePlayerInfo(GuildBattlePvpTeam team)
		{
			return GetCurrentGuildBattlePlayerInfo(team.OwnerHashedPlayerId);
		}

		public GuildBattleParticipantInfo GetCurrentGuildBattlePlayerInfo(string hashedId)
		{
			EnemyPlayersInfoList.TryGetValue(hashedId, out var value);
			return value;
		}

		private void RewardSetup()
		{
			if (CurrentMapModel != null && CurrentMapModel.RandomSeed != -1)
			{
				SetupOrderedRewardsLookup();
				SetupAndSelectRewards();
			}
		}

		private void SetupAndSelectRewards()
		{
			ModelRandom modelRandom = new ModelRandom(CurrentMapModel.RandomSeed);
			int num = base.gameEconomyData.GuildBattleSectorDefinitions.Length;
			int sectorId = CurrentMapModel.Sectors[0].SectorId;
			List<int> list = new List<int>();
			Dictionary<int, Rewards> dictionary = new Dictionary<int, Rewards>();
			for (int i = 0; i < num; i++)
			{
				if (CurrentMapModel.Sectors.Count == i)
				{
					base.Debug.LogError($"CurrentMapModel has {CurrentMapModel.Sectors.Count} sectors while the definitions length is {num}");
					break;
				}
				GuildBattleMapSectorModel guildBattleMapSectorModel = CurrentMapModel.Sectors[i];
				if (sectorId != guildBattleMapSectorModel.SectorId)
				{
					sectorId = guildBattleMapSectorModel.SectorId;
					list.Clear();
				}
				string key = $"{GuildBattleReward.GuildRewardType.SectorCompletion}_{sectorId}_{CurrentWarDefinition.RewardSetName}";
				if (!orderedGuildBattleRewardsLookup.TryGetValue(key, out var value))
				{
					continue;
				}
				FixedPoint[] rewardPoolWeights = value.GetRewardPoolWeights(list);
				if (rewardPoolWeights != null)
				{
					int num2 = modelRandom.WeightedRandom(rewardPoolWeights);
					if (value.RewardsPoolParsed[num2].Unique)
					{
						list.Add(num2);
					}
					string reward = value.RewardsPoolParsed[num2].Reward;
					if (!string.IsNullOrEmpty(reward))
					{
						Rewards value2 = new Rewards(reward, null, i, EquipmentSource.MissionLoot);
						dictionary.Add(guildBattleMapSectorModel.SectorId, value2);
					}
				}
			}
			selectedRewards.Add(3, dictionary);
			Dictionary<int, Rewards> dictionary2 = new Dictionary<int, Rewards>();
			for (int j = 0; j < 4; j++)
			{
				string key2 = $"{GuildBattleReward.GuildRewardType.MissionCompletion}_{j}_{CurrentWarDefinition.RewardSetName}";
				if (orderedGuildBattleRewardsLookup.TryGetValue(key2, out var value3))
				{
					FixedPoint[] rewardPoolWeights2 = value3.GetRewardPoolWeights();
					if (rewardPoolWeights2 != null)
					{
						int index = modelRandom.WeightedRandom(rewardPoolWeights2);
						Rewards value4 = new Rewards(value3.RewardsPoolParsed[index].Reward, null, j, EquipmentSource.MissionLoot);
						dictionary2.Add(j, value4);
					}
				}
			}
			selectedRewards.Add(5, dictionary2);
		}

		private void SetupOrderedRewardsLookup()
		{
			if (orderedGuildBattleRewardsLookup == null || orderedGuildBattleRewardsLookup.Count != 0)
			{
				return;
			}
			for (int i = 0; i < base.gameEconomyData.GuildBattleRewardDefinitions.Length; i++)
			{
				GuildBattleReward guildBattleReward = base.gameEconomyData.GuildBattleRewardDefinitions[i];
				if (guildBattleReward != null)
				{
					string key = string.Format("{0}_{1}_{2}", guildBattleReward.RewardType, guildBattleReward.SectorId, guildBattleReward.SetName ?? "");
					if (!orderedGuildBattleRewardsLookup.ContainsKey(key) || orderedGuildBattleRewardsLookup[key] == null)
					{
						orderedGuildBattleRewardsLookup[key] = guildBattleReward;
					}
				}
			}
		}

		public void UpdateMissionProgressionRewardsForGuildBattle(TWDModelManager twdManager, int sectorId, string uniqueMissionId, string playerId, int vpAmount)
		{
			AddVictoryMissionPointsToGuild(twdManager, sectorId, uniqueMissionId, playerId, vpAmount);
			AddTraitBonusRewardToGuild(twdManager, sectorId);
		}

		public void AddTraitBonusRewardToGuild(TWDModelManager manager, int sectorId)
		{
			GuildBattleMapSectorModel sectorModel = CurrentMapModel.GetSectorModel(sectorId);
			if (sectorModel.IsCompleted() && !CollectedBattleBonusesPerSector.ContainsKey(sectorId))
			{
				List<string> guildSectorBattleTraitBonus = GetGuildSectorBattleTraitBonus(sectorModel.SectorId);
				if (guildSectorBattleTraitBonus != null && guildSectorBattleTraitBonus.Count > 0)
				{
					CollectedBattleBonusesPerSector.Add(sectorId, guildSectorBattleTraitBonus);
					if (manager.ServerService != null)
					{
						manager.Metrics.AddFind().AddBattleBonus(guildSectorBattleTraitBonus[0]).AddSector(sectorModel)
							.AddGvG()
							.AddGvGBattle()
							.Send();
					}
				}
			}
			NotifyChange("GuildRewardTraitBonuseAdded", this);
		}

		public void AddVictoryMissionPointsToGuild(TWDModelManager twdManager, int sectorId, string uniqueMissionId, string playerId, int vpAmount)
		{
			GuildBattleMapSectorModel sectorModel = CurrentMapModel.GetSectorModel(sectorId);
			bool flag = true;
			if (CompletedMissionsPerPlayer.TryGetValue(playerId, out var value))
			{
				flag = !value.Contains(uniqueMissionId);
				if (!flag)
				{
					base.Debug.LogWarning("Player " + playerId + " has already completed mission " + uniqueMissionId);
				}
			}
			else
			{
				CompletedMissionsPerPlayer[playerId] = new List<string>();
			}
			if (flag)
			{
				twdManager.Player.GuildModel.AccumulatePlayerTotalVp(twdManager, playerId, vpAmount);
				CompletedMissionsPerPlayer[playerId].Add(uniqueMissionId);
			}
			if (sectorModel.IsCompleted() && !VictoryPointsSectorRewardPerSector.ContainsKey(sectorId))
			{
				int num = GetGuildSectorBattleVictoryPoints(sectorId);
				RewardGuildBattleVP bonusVPRewardFromSector = GetBonusVPRewardFromSector(sectorId);
				if (bonusVPRewardFromSector != null)
				{
					num += bonusVPRewardFromSector.Amount;
				}
				VictoryPointsSectorRewardPerSector.Add(sectorId, num);
				if (twdManager.ServerService != null && twdManager.Player.HashedId == playerId)
				{
					twdManager.Metrics.AddFind().AddGuildVictoryPointsResources(num).AddSector(sectorModel)
						.AddGvG()
						.AddGvGBattle()
						.Send();
				}
			}
			NotifyChange("GuildVictoryPointsAdded", this);
		}

		private List<IReward> GetGuildBattleRewards(GuildBattleReward.GuildRewardType rewardType, int control)
		{
			List<IReward> list = new List<IReward>();
			if (selectedRewards != null && selectedRewards.TryGetValue((int)rewardType, out var value) && value.TryGetValue(control, out var value2))
			{
				list.AddRange(value2.RewardsList);
			}
			return list;
		}

		public int GetPersonalGuildBattleMissionRewardPoints(int sectorId, bool isPvP, int column)
		{
			int num = -1;
			GuildBattleSectorDefinition guildBattleSectorDefinition = base.gameEconomyData.FindMissionSectorDefinition(sectorId);
			if (guildBattleSectorDefinition != null)
			{
				int difficultyOffset = (isPvP ? guildBattleSectorDefinition.PVPModifierPerArea[column] : guildBattleSectorDefinition.ColumnsDifficulty[column]);
				num = base.gameEconomyData.GetGuildBattleMissionRewardRP(difficultyOffset, isPvP);
			}
			if (num == -1 || num == 0)
			{
				base.Debug.LogWarning("No reward for Mission Completions, fallback to default value");
				return 2;
			}
			return num;
		}

		public IReward GetPersonalGuildBattleSectorCompletionBonus(int sectorId)
		{
			List<IReward> guildBattleRewards = GetGuildBattleRewards(GuildBattleReward.GuildRewardType.SectorCompletion, sectorId);
			if (guildBattleRewards != null && guildBattleRewards.Count > 0)
			{
				for (int i = 0; i < guildBattleRewards.Count; i++)
				{
					if (guildBattleRewards[i] is RewardCurrency rewardCurrency && (rewardCurrency.CurrencyType == CurrencyType.GuildBattleRP || rewardCurrency.CurrencyType == CurrencyType.GvGGas))
					{
						return rewardCurrency;
					}
				}
			}
			return null;
		}

		public RewardGuildBattleVP GetBonusVPRewardFromSector(int sectorId)
		{
			List<IReward> guildBattleRewards = GetGuildBattleRewards(GuildBattleReward.GuildRewardType.SectorCompletion, sectorId);
			if (guildBattleRewards != null && guildBattleRewards.Count > 0)
			{
				for (int i = 0; i < guildBattleRewards.Count; i++)
				{
					if (guildBattleRewards[i] is RewardGuildBattleVP result)
					{
						return result;
					}
				}
			}
			return null;
		}

		public float GetGuildBattleVictoryRewardPointsMultiplier()
		{
			return base.gameEconomyData.GetGuildBattleVictoryRewardPointsMultiplierForTier(GuildTier);
		}

		public float GetGuildBattleVictoryPointsMultiplier()
		{
			return base.gameEconomyData.GetGuildBattleVictoryPointsMultiplierForTier(GuildTier);
		}

		public float GetGuildBattleDrawPointsMultiplier()
		{
			return base.gameEconomyData.GetGuildBattleDrawPointsMultiplierForTier(GuildTier);
		}

		public float GetGuildBattleDrawRewardPointsMultiplier()
		{
			return base.gameEconomyData.GetGuildBattleDrawRewardPointsMultiplierForTier(GuildTier);
		}

		public int GetBattleWonBonusVictoryPoints()
		{
			float guildBattleVictoryPointsMultiplierForTier = base.gameEconomyData.GetGuildBattleVictoryPointsMultiplierForTier(GuildTier);
			return (int)((float)EndVictoryPoints * guildBattleVictoryPointsMultiplierForTier);
		}

		public int GetBattleDrawPoints()
		{
			float guildBattleDrawPointsMultiplierForTier = base.gameEconomyData.GetGuildBattleDrawPointsMultiplierForTier(GuildTier);
			return (int)((float)EndVictoryPoints * guildBattleDrawPointsMultiplierForTier);
		}

		public int GetGuildBattleMissionVictoryPoints(int sectorId, bool isPvP, int column)
		{
			int num = -1;
			GuildBattleSectorDefinition guildBattleSectorDefinition = base.gameEconomyData.FindMissionSectorDefinition(sectorId);
			if (guildBattleSectorDefinition != null)
			{
				int difficultyOffset = (isPvP ? guildBattleSectorDefinition.PVPModifierPerArea[column] : guildBattleSectorDefinition.ColumnsDifficulty[column]);
				num = base.gameEconomyData.GetGuildBattleMissionRewardVP(difficultyOffset, isPvP);
			}
			if (num == -1 || num == 0)
			{
				base.Debug.LogWarning("No reward for Mission Completions, fallback to default value 2");
				return 2;
			}
			return num;
		}

		public int GetGuildSectorBattleVictoryPoints(int sectorId)
		{
			int num = -1;
			num = base.gameEconomyData.GetGuildBattleSectorRewardVP(sectorId);
			if (num == -1 || num == 0)
			{
				base.Debug.LogWarning("No reward for Sector Completions, fallback to default value 2");
				return 2;
			}
			return num;
		}

		public List<string> GetGuildSectorBattleTraitBonus(int sectorId)
		{
			List<IReward> guildBattleRewards = GetGuildBattleRewards(GuildBattleReward.GuildRewardType.SectorCompletion, sectorId);
			List<IReward> list = null;
			if (guildBattleRewards != null && guildBattleRewards.Count > 0)
			{
				list = guildBattleRewards.FindAll((IReward x) => x is RewardTraitBonus);
			}
			if (list == null)
			{
				base.Debug.LogWarning("No bonus for Sector Completions");
				return null;
			}
			List<string> list2 = new List<string>();
			for (int num = 0; num < list.Count; num++)
			{
				if (list[num] is RewardTraitBonus rewardTraitBonus)
				{
					list2.Add(rewardTraitBonus.TraitId);
				}
			}
			return list2;
		}

		public bool UpdateLiveData(string uniqueMissionId, string playerHashedId)
		{
			LiveMissionData value = null;
			if (!LiveMissionDataPerPlayer.TryGetValue(playerHashedId, out value))
			{
				value = new LiveMissionData();
				LiveMissionDataPerPlayer.Add(playerHashedId, value);
			}
			if (!string.IsNullOrEmpty(value.LastAttackedMissionId) && string.Equals(value.LastAttackedMissionId, uniqueMissionId, StringComparison.Ordinal))
			{
				return false;
			}
			if (uniqueMissionId != null)
			{
				GuildBattleMapMissionModel missionModel = CurrentMapModel.GetMissionModel(uniqueMissionId);
				if (missionModel != null)
				{
					if (CurrentMapModel.PvpTeamsIndexPerMission.ContainsKey(uniqueMissionId) && missionModel.IsMissionPveComplete())
					{
						value.MissionState = GuildBattleMapMissionModel.MissionState.PVP;
						missionModel.AddPvpParticipant(playerHashedId);
					}
					else
					{
						value.MissionState = GuildBattleMapMissionModel.MissionState.PVE;
					}
				}
			}
			value.LastAttackedMissionId = uniqueMissionId;
			NotifyChange("GuildBattleLiveDataUpdated");
			return true;
		}

		public bool IsPlayerRegistered(string hashedId)
		{
			return RegisteredPlayers.Contains(hashedId);
		}

		public bool CanPlayerRegister(string hashedId, long utcTimestamp)
		{
			if (IsOngoing(utcTimestamp) && !IsPlayerRegistered(hashedId) && RegisteredPlayersCount < base.gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle)
			{
				return true;
			}
			return false;
		}

		public bool IsBattleActiveForTimeSlot(long timeSlot, long utcTimestamp)
		{
			if (IsOngoing(utcTimestamp))
			{
				return TimeSlot == timeSlot;
			}
			return false;
		}

		public void UpdateIndicatorState(GuildBattleIndicatorData data)
		{
			if (GuildBattleEmblemDataPerPlayer != null)
			{
				if (!GuildBattleEmblemDataPerPlayer.ContainsKey(data.PlayerHashedId))
				{
					GuildBattleEmblemDataPerPlayer.Add(data.PlayerHashedId, data);
				}
				else
				{
					GuildBattleEmblemDataPerPlayer[data.PlayerHashedId] = data;
				}
				NotifyChange("GuildBattleMapIndicatorsUpdated", data);
			}
		}

		public bool CanPlayerAddNewMapNotification(string playerHashedId, long utcTimestamp, out long milliSecondsLeft)
		{
			milliSecondsLeft = 0L;
			if (GuildBattleEmblemDataPerPlayer == null)
			{
				return false;
			}
			if (GuildBattleEmblemDataPerPlayer.TryGetValue(playerHashedId, out var value))
			{
				milliSecondsLeft = Math.Max(value.UtcTimeStamp + 5000 - utcTimestamp, 0L);
				return milliSecondsLeft <= 0;
			}
			return true;
		}

		public void UpdateMemberAttackAttempts(string playerId, int numberOfAttacks)
		{
			NumberOfAttacksPerPlayer[playerId] = numberOfAttacks;
		}

		private void InitializeFakeBattleTargetScore(TWDModelManager manager)
		{
			IServerService serverService = manager.ServerService;
			if (serverService != null)
			{
				FakeBattleDefinition fakeBattleDefinition = manager.GameEconomyData.FindFakeBattleDefinition(GuildTier);
				if (fakeBattleDefinition != null)
				{
					int targetScore = fakeBattleDefinition.TargetScore;
					LeaderboardEntry leaderboardEntry = new LeaderboardEntry
					{
						Id = EnemyGuildData.GroupId,
						Tags = null,
						ScoreAt = (long)(DateTime.UtcNow - TWDModelManager.Epoch).TotalSeconds,
						Score = targetScore
					};
					Leaderboards.GuildBattleLiveScoreLeaderboardDetails value = new Leaderboards.GuildBattleLiveScoreLeaderboardDetails(EnemyGuildData.GroupId, EnemyGuildData.GuildName);
					leaderboardEntry.Details = manager.GetMessageSerializer().SerializeObject(value);
					serverService.SaveLeaderboardEntry(Leaderboards.GetGuildBattleLiveScoreLeaderboardName(BattleId, CurrentMapModel.RandomSeed), leaderboardEntry);
					leaderboardEntry = new LeaderboardEntry();
					string playerHashedId = (leaderboardEntry.Id = "Fake_Player_Scorer");
					leaderboardEntry.Tags = null;
					leaderboardEntry.ScoreAt = (long)(DateTime.UtcNow - TWDModelManager.Epoch).TotalSeconds;
					leaderboardEntry.Score = targetScore;
					Leaderboards.GuildBattlePlayersScoreLeaderboardDetails value2 = new Leaderboards.GuildBattlePlayersScoreLeaderboardDetails(fakeBattleDefinition.OpponentName, playerHashedId, EnemyGuildData.GuildName, null);
					leaderboardEntry.Details = manager.GetMessageSerializer().SerializeObject(value2);
					serverService.SaveLeaderboardEntry(BattleId, leaderboardEntry);
				}
			}
		}

		public void FetchBattleHighscores(long timeStamp, TWDModelManager manager, bool forceBroadcast, bool forceUpdate, bool updateGuildBattleResults, bool requireUpdate = false)
		{
			IServerService serverService = manager.ServerService;
			if (!HelpersModel.IsOfflineMode && serverService == null)
			{
				return;
			}
			bool flag = timeStamp >= LastLeaderboardsUpdateTimestamp + manager.GameEconomyData.GuildWarConfig.BattleLeaderboardsCacheDurationInMilliseconds;
			if (GuildScores == null || PlayerHighscores == null)
			{
				forceUpdate = true;
			}
			if (!flag && !forceBroadcast && !forceUpdate)
			{
				return;
			}
			if (flag || forceUpdate || requireUpdate)
			{
				if (HelpersModel.IsOfflineMode)
				{
					GuildScores = GetGuildScoresFromGuildData(manager);
					PlayerHighscores = GetPlayerScoresFromGuildData(manager);
				}
				else
				{
					IMessageSerializer messageSerializer = manager.GetMessageSerializer();
					GuildScores = GuildBattleLiveScoreDataEntry.ParseLeaderboardData(manager.ServerService.GetLeaderboard(Leaderboards.GetGuildBattleLiveScoreLeaderboardName(BattleId, CurrentMapModel.RandomSeed), "2"), messageSerializer);
					PlayerHighscores = GuildBattlePlayersScoreDataEntry.ParseLeaderboardData(manager.ServerService.GetLeaderboard(BattleId, "22"), messageSerializer);
					if (PlayerHighscores != null && RegisteredPlayers != null && RegisteredPlayers.Count > 0)
					{
						for (int num = PlayerHighscores.Count - 1; num >= 0; num--)
						{
							ScoreDataEntry scoreDataEntry = PlayerHighscores[num];
							if (NotInPvP(scoreDataEntry.Id) && !string.Equals(scoreDataEntry.Id, "Fake_Player_Scorer"))
							{
								manager.GvGLogWarning("FetchBattleHighscores:" + BattleId + "#" + scoreDataEntry.Id + "#" + EnemyGuildData?.GroupId + "#" + guildId);
							}
						}
					}
				}
			}
			UpdateGuildBattleHighscoresGroupCommand command = new UpdateGuildBattleHighscoresGroupCommand(timeStamp, GuildScores, PlayerHighscores, updateGuildBattleResults && HasEnded());
			HelpersModel.ExecuteGroupCommand(manager, command);
		}

		public bool GetBattleScoresFromLeaderboard(TWDModelManager manager, ref int vp, ref int opponentVP)
		{
			List<LeaderboardEntry> leaderboard = manager.ServerService.GetLeaderboard(Leaderboards.GetGuildBattleLiveScoreLeaderboardName(BattleId, CurrentMapModel.RandomSeed), "2");
			if (leaderboard != null)
			{
				IMessageSerializer messageSerializer = manager.GetMessageSerializer();
				List<ScoreDataEntry> list = GuildBattleLiveScoreDataEntry.ParseLeaderboardData(leaderboard, messageSerializer);
				if (IsFakeBattle)
				{
					opponentVP = base.gameEconomyData.FindFakeBattleDefinition(GuildTier).TargetScore;
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Id == manager.Player.GuildId)
					{
						vp = (int)Math.Min(list[i].Score, 2147483647L);
					}
					else if (!IsFakeBattle)
					{
						opponentVP = (int)Math.Min(list[i].Score, 2147483647L);
					}
				}
				return true;
			}
			return false;
		}

		private List<ScoreDataEntry> GetPlayerScoresFromGuildData(TWDModelManager manager)
		{
			List<ScoreDataEntry> list = new List<ScoreDataEntry>();
			foreach (KeyValuePair<string, int> item in VictoryPointsPerPlayer)
			{
				list.Add(new GuildBattlePlayersScoreDataEntry(manager.Player.Name, manager.Player.HashedId, manager.Player.GuildModel.Id, manager.Player.PlayerEmblem, item.Value));
			}
			list.Add(new GuildBattlePlayersScoreDataEntry(BattleId, manager.Player.HashedId, manager.Player.GuildModel.Id, null, VictoryPointsSectorRewardPerSector.Sum((KeyValuePair<int, int> keyValue) => keyValue.Value)));
			return list;
		}

		public List<ScoreDataEntry> GetGuildScoresFromGuildData(TWDModelManager manager)
		{
			List<ScoreDataEntry> list = new List<ScoreDataEntry>();
			list.Add(new GuildBattleLiveScoreDataEntry(manager.Player.GuildModel.Id, manager.Player.GuildModel.Name, CalculateTotalVictoryPoints()));

			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
				int enemyPoints = GWTeamUtils.Instance.OpponentGuilModel?.GuildWarModel.CurrentBattle.CalculateTotalVictoryPoints() ?? 0;
				list.Add(new GuildBattleLiveScoreDataEntry(EnemyGuildData.GroupId, EnemyGuildData.GuildName, enemyPoints));
				DebugTWD.Log("Custom GetGuildScoresFromGuildData", DebugType.Wars);
			}
			else
			{
				FakeBattleDefinition fakeBattleDefinition = manager.GameEconomyData.FindFakeBattleDefinition(GuildTier);
				if (fakeBattleDefinition != null)
				{
					list.Add(new GuildBattleLiveScoreDataEntry(EnemyGuildData.GroupId, EnemyGuildData.GuildName, fakeBattleDefinition.TargetScore));
				}
			}
			return list;
		}

		public void UpdateGuildBattleHighScores(List<ScoreDataEntry> guildScores, List<ScoreDataEntry> playerHighscores, long lastLeaderboardsUpdateTimestamp)
		{
			LastLeaderboardsUpdateTimestamp = lastLeaderboardsUpdateTimestamp;
			GuildScores = guildScores;
			PlayerHighscores = playerHighscores;
			NotifyChange("GuildBattleScoresUpdated");
		}

		public bool SaveLeaderboard(IServerService serverService, LeaderboardEntry entry)
		{
			return LeaderboardUpdated = serverService.TrySaveLeaderboardEntry(LeaderboardName, entry);
		}

		public bool NotInPvP(string hashId)
		{
			List<string> registeredPlayers = RegisteredPlayers;
			if (registeredPlayers == null || registeredPlayers.Count != 0)
			{
				GuildBattleMatchmakingInfo enemyGuildData = EnemyGuildData;
				if (enemyGuildData == null || enemyGuildData.RegisteredPlayersList?.Count != 0)
				{
					if (!RegisteredPlayers.Contains(hashId) && !EnemyGuildData.RegisteredPlayersList.Contains(hashId) && hashId != EnemyGuildData.GroupId)
					{
						return hashId != guildId;
					}
					return false;
				}
			}
			return false;
		}


		#region mycode
		public List<GuildBattlePlayersScoreDataEntry> GetPlayerScoresFromGuildDataCustom(GuildModel guild)
		{
			List<GuildBattlePlayersScoreDataEntry> list = new List<GuildBattlePlayersScoreDataEntry>();
			var playerGuildId = guild.Id;

			foreach (KeyValuePair<string, int> item in VictoryPointsPerPlayer)
			{
				var playerId = item.Key;

				PlayerEmblem playerEmblem = null;

				string playerName = guild.GuildBattleMatchmakingInfo.GetParticipantInfo(playerId)?.Name ?? null;//GetMemberInfo(playerId).Name;
				if (playerName == null)
				{
					List<ScoreDataEntry> scores = GuildWarHelper.GetGuildWarModel().GuildBattleResults.Last().Value.PlayerScores;

					var score = scores.FirstOrDefault(x => x.Id == playerId);
					if (score != null)
					{
						playerName = score.Name;
					}
				}
				else
				{
					playerEmblem = guild.GuildBattleMatchmakingInfo.GetParticipantInfo(playerId).PlayerEmblem;//guild.GetMemberInfo(playerId).PlayerEmblem;
				}
				list.Add(new GuildBattlePlayersScoreDataEntry(playerName, playerId, playerGuildId, playerEmblem, item.Value));
			}
			list.Add(new GuildBattlePlayersScoreDataEntry(BattleId, playerGuildId, playerGuildId, null, VictoryPointsSectorRewardPerSector.Sum((KeyValuePair<int, int> keyValue) => keyValue.Value)));
			return list;
		}

		public List<ScoreDataEntry> UpdateGuildScrores()
		{
			return OfflineManager.IsLoadDataManager ?
			GetGuildScoresFromGuildData(GameManager.Instance.modelManager) :
			GuildScores;
		}
		#endregion
	}
}
