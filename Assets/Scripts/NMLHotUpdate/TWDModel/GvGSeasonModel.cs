using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GvGSeasonModel : TWDGroupModelChild, ILeaderboardState
	{
		[Serializable]
		public class GuildBattleLogEntry
		{
			public string BattleId;

			public long EndedTimeStamp;

			public string OpponentGuildName;

			public List<int> CompletedSectors;

			public int VictoryPoints;

			public int Result;

			[JsonIgnore]
			public bool IsVictory => Result == 1;

			[JsonIgnore]
			public bool IsDraw => Result == 3;

			[JsonIgnore]
			public bool Ended => Result != 0;
		}

		[Serializable]
		public class GvGSeasonStats
		{
			public int CurrentTier;

			public int CurrentVictoryPoints;

			public int PreviousVictoryPoints;

			public int LastSeasonTier;

			public int WonBattles;

			public int LostBattles;

			public int DrawBattles;

			public void UpdateWithResult(GuildBattleModel.GuildBattleResult battleResult, int battleVictoryPoints)
			{
				PreviousVictoryPoints = CurrentVictoryPoints;
				CurrentVictoryPoints += battleVictoryPoints;
				switch (battleResult)
				{
				case GuildBattleModel.GuildBattleResult.Victory:
					WonBattles++;
					break;
				case GuildBattleModel.GuildBattleResult.Defeat:
					LostBattles++;
					break;
				case GuildBattleModel.GuildBattleResult.Draw:
					DrawBattles++;
					break;
				}
			}
		}

		public GvGSeasonStats CurrentSeasonStats;

		public Dictionary<int, List<GuildBattleLogEntry>> BattleLog;

		private GvGSeasonDefinition currentSeasonDefinitionInternal;

		public int SeasonDefinitionId { get; set; }

		public GuildWarModel GuildWarModel { get; set; }

		public Dictionary<string, int> SeasonTotalVpAccumulatedPerPlayer { get; set; }

		public bool LeaderboardUpdated { get; set; } = true;

		public string LeaderboardName => Leaderboards.GetLeaderboardNameGuildGlobalSeason(SeasonDefinitionId);

		[JsonIgnore]
		public int CurrentVictoryPoints => CurrentSeasonStats.CurrentVictoryPoints;

		[JsonIgnore]
		public int PreviousVictoryPoints => CurrentSeasonStats.PreviousVictoryPoints;

		[JsonIgnore]
		public int CurrentTier => CurrentSeasonStats.CurrentTier;

		[JsonIgnore]
		public int CurrentSeasonVictories => CurrentSeasonStats.WonBattles;

		[JsonIgnore]
		public int CurrentSeasonDefeats => CurrentSeasonStats.LostBattles;

		[JsonIgnore]
		public GvGSeasonDefinition CurrentSeasonDefinition
		{
			get
			{
				if (base.gameEconomyData == null)
				{
					base.Debug.LogWarning("GameEconomyData is null");
					return null;
				}
				if (currentSeasonDefinitionInternal == null || currentSeasonDefinitionInternal.Identifier != SeasonDefinitionId)
				{
					currentSeasonDefinitionInternal = base.gameEconomyData.FindGvGSeasonDefinition(SeasonDefinitionId);
				}
				return currentSeasonDefinitionInternal;
			}
		}

		public GvGSeasonModel()
		{
			SeasonDefinitionId = -1;
			GuildWarModel = new GuildWarModel();
			BattleLog = new Dictionary<int, List<GuildBattleLogEntry>>();
			CurrentSeasonStats = new GvGSeasonStats();
			SeasonTotalVpAccumulatedPerPlayer = new Dictionary<string, int>();
			LeaderboardUpdated = true;
		}

		public override void Start()
		{
			base.Start();
			if (CurrentTier == 0)
			{
				if (base.gameEconomyData == null)
				{
					base.Debug.LogError("GameEconomyData is null");
				}
				else if (base.gameEconomyData.GuildWarConfig == null)
				{
					base.Debug.LogError("GuildWarConfig is null");
				}
				else
				{
					CurrentSeasonStats.CurrentTier = base.gameEconomyData.GuildWarConfig.GuildBattleMinimumTier;
				}
			}
		}

		public bool IsCurrentSeasonOpen(long utcTimeStamp)
		{
			if (CurrentSeasonDefinition == null)
			{
				return false;
			}
			return CurrentSeasonDefinition.IsOpen(utcTimeStamp);
		}

		public GvGSeasonDefinition FindCurrentOrNextSeason(long utcTimeStamp)
		{
			if (CurrentSeasonDefinition == null)
			{
				return FindNextSeason(utcTimeStamp);
			}
			if (!CurrentSeasonDefinition.IsOpen(utcTimeStamp))
			{
				return FindNextSeason(utcTimeStamp);
			}
			return CurrentSeasonDefinition;
		}

		public GvGSeasonDefinition FindNextSeason(long utcTimeStamp)
		{
			if (base.gameEconomyData == null)
			{
				base.Debug.LogWarning("GameEconomyData is null");
				return null;
			}
			return base.gameEconomyData.FindNextGvGSeason(CurrentSeasonDefinition?.EndTimeMilliseconds ?? 0, utcTimeStamp);
		}

		public void AddToBattleLog(GuildBattleModel battle)
		{
			if (battle == null)
			{
				base.Debug.LogError("Trying to add null battle to BattleLog");
				return;
			}
			if (BattleLog == null)
			{
				base.Debug.LogError("BattleLog is null when trying to calculate score for the season");
				return;
			}
			if (!BattleLog.TryGetValue(battle.WarId, out var value))
			{
				value = new List<GuildBattleLogEntry>();
				BattleLog.Add(battle.WarId, value);
			}
			GuildBattleLogEntry guildBattleLogEntry = CreateBattleLogEntry(battle);
			if (guildBattleLogEntry == null)
			{
				base.Debug.LogError("LogEntry created from battle is null.");
			}
			value.Add(guildBattleLogEntry);
		}

		private GuildBattleLogEntry CreateBattleLogEntry(GuildBattleModel battle)
		{
			if (battle == null)
			{
				base.Debug.LogError("Trying to create a BattleLogEntry from a null battle");
				return null;
			}
			GuildBattleLogEntry guildBattleLogEntry = new GuildBattleLogEntry
			{
				BattleId = battle.BattleId,
				OpponentGuildName = battle.EnemyGuildName,
				EndedTimeStamp = battle.EndBattleTimestamp,
				VictoryPoints = battle.FinalVictoryPoints,
				Result = (int)battle.BattleResult,
				CompletedSectors = new List<int>(battle.CompletedSectors?.Count ?? 0)
			};
			if (battle.CompletedSectors == null)
			{
				base.Debug.LogError("CompletedSectors list from battle is null");
				return guildBattleLogEntry;
			}
			foreach (int completedSector in battle.CompletedSectors)
			{
				guildBattleLogEntry.CompletedSectors.Add(completedSector);
			}
			return guildBattleLogEntry;
		}

		public GuildBattleLogEntry GetBattleLogEntry(int warId, string battleId)
		{
			if (BattleLog == null)
			{
				base.Debug.LogError("BattleLog is null when trying to calculate score for the season");
				return null;
			}
			if (!BattleLog.TryGetValue(warId, out var value))
			{
				base.Debug.LogWarning("BattleLog does not contain warId " + warId);
				return null;
			}
			if (value == null)
			{
				base.Debug.LogWarning("BattleList retrieved from BattleLog is null");
				return null;
			}
			GuildBattleLogEntry guildBattleLogEntry = value.Find((GuildBattleLogEntry x) => x.BattleId == battleId);
			if (guildBattleLogEntry == null)
			{
				base.Debug.LogWarning("No battle log entry found for warId. WarId : " + warId + ", battleId: " + battleId);
			}
			return guildBattleLogEntry;
		}

		public GuildBattleLogEntry GetBattleLogEntryAfterTimeStamp(int warId, long timeStamp)
		{
			if (BattleLog == null)
			{
				base.Debug.LogError("BattleLog is null when trying to calculate score for the season");
				return null;
			}
			if (!BattleLog.TryGetValue(warId, out var value))
			{
				base.Debug.LogWarning("BattleLog does not contain warId " + warId);
				return null;
			}
			if (value == null)
			{
				base.Debug.LogWarning("BattleList retrieved from BattleLog is null");
				return null;
			}
			foreach (GuildBattleLogEntry item in value)
			{
				if (item.EndedTimeStamp > timeStamp)
				{
					return item;
				}
			}
			base.Debug.LogWarning("No battle log entry had a timeStamp bigger than " + timeStamp);
			return null;
		}

		public int CalculateBattleLogTotalScoreForWar(int warId)
		{
			if (BattleLog == null)
			{
				base.Debug.LogError("BattleLog is null when trying to calculate score for the season");
				return 0;
			}
			if (!BattleLog.TryGetValue(warId, out var value))
			{
				base.Debug.LogWarning("BattleLog does not contain warId " + warId);
				return 0;
			}
			if (value == null)
			{
				base.Debug.LogWarning("BattleList retrieved from BattleLog is null");
				return 0;
			}
			int num = 0;
			foreach (GuildBattleLogEntry item in value)
			{
				if (item == null)
				{
					base.Debug.LogWarning("Null battleLogEntry found");
				}
				else
				{
					num += item.VictoryPoints;
				}
			}
			return num;
		}

		public int CalculateBattleLogTotalScoreForSeason()
		{
			if (BattleLog == null)
			{
				base.Debug.LogError("BattleLog is null when trying to calculate score for the season");
				return 0;
			}
			int num = 0;
			foreach (int key in BattleLog.Keys)
			{
				num += CalculateBattleLogTotalScoreForWar(key);
			}
			return num;
		}

		public void EndSeason()
		{
			Reset();
			SeasonDefinitionId = -1;
		}

		private void Reset()
		{
			if (BattleLog == null)
			{
				base.Debug.LogError("BattleLog is null when resetting guildModel");
			}
			else
			{
				BattleLog.Clear();
			}
			GvGSeasonStats currentSeasonStats = CurrentSeasonStats;
			CurrentSeasonStats = new GvGSeasonStats();
			SeasonTotalVpAccumulatedPerPlayer = new Dictionary<string, int>();
			if (base.gameEconomyData?.GuildWarConfig == null)
			{
				base.Debug.LogError("Cant access GuildWarConfig. Reference is null.");
				return;
			}
			CurrentSeasonStats.CurrentTier = base.gameEconomyData.GuildWarConfig.GuildBattleMinimumTier;
			if (currentSeasonStats != null)
			{
				CurrentSeasonStats.LastSeasonTier = currentSeasonStats.CurrentTier;
				FixedPoint fixedPoint = currentSeasonStats.CurrentVictoryPoints;
				fixedPoint *= base.gameEconomyData.GuildWarConfig.SeasonResetPercentage;
				if (fixedPoint > base.gameEconomyData.GuildWarConfig.MaxSeasonStartVictoryPoints)
				{
					fixedPoint = base.gameEconomyData.GuildWarConfig.MaxSeasonStartVictoryPoints;
				}
				CurrentSeasonStats.CurrentVictoryPoints = (int)fixedPoint;
				CheckForTierIncrease();
				LeaderboardUpdated = true;
			}
		}

		public bool StartSeason(int seasonId, GuildModel guildModel)
		{
			if (guildModel == null)
			{
				base.Debug.LogWarning("Can not start a season for a nu ll GuildModel");
				return false;
			}
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("GameEconomyData is null");
				return false;
			}
			GvGSeasonDefinition gvGSeasonDefinition = base.gameEconomyData.FindGvGSeasonDefinition(seasonId);
			if (gvGSeasonDefinition == null)
			{
				return false;
			}
			if (gvGSeasonDefinition.IsOpen(guildModel.TimeStamp))
			{
				guildModel.GuildBattleMatchmakingInfo.ResetParticipants();
				EndSeason();
				guildModel.GuildBattleMatchmakingInfo.UpdateInfoOnEndBattle(CurrentTier, CurrentVictoryPoints);
				SeasonDefinitionId = seasonId;
				return true;
			}
			return false;
		}

		public void CheckForTierIncrease()
		{
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("GameEconomyData is null");
			}
			else if (CurrentSeasonStats == null)
			{
				base.Debug.LogError("Current season stats is null");
			}
			else
			{
				if (CurrentTier <= 1)
				{
					return;
				}
				GuildTierDefinition guildTierDefinition = base.gameEconomyData.GetGuildTierDefinition(CurrentTier - 1);
				if (guildTierDefinition == null)
				{
					base.Debug.LogError("Next tier definition is null");
					return;
				}
				while (CurrentTier > 1 && guildTierDefinition.VictoryPointsRequired <= CurrentVictoryPoints)
				{
					CurrentSeasonStats.CurrentTier--;
					guildTierDefinition = base.gameEconomyData.GetGuildTierDefinition(CurrentTier - 1);
					if (guildTierDefinition == null)
					{
						base.Debug.LogError("Next tier definition is null");
						break;
					}
				}
			}
		}

		public void UpdateStatsFromLastBattle()
		{
			if (GuildWarModel == null)
			{
				base.Debug.LogError("GuildWarModel is null");
				return;
			}
			if (GuildWarModel.CurrentBattle == null)
			{
				base.Debug.LogError("GuildWarModel current battle is null");
				return;
			}
			GuildBattleModel currentBattle = GuildWarModel.CurrentBattle;
			if (CurrentSeasonStats == null)
			{
				base.Debug.LogError("Current season stats is null");
				return;
			}
			CurrentSeasonStats.UpdateWithResult(currentBattle.BattleResult, currentBattle.FinalVictoryPoints);
			CheckForTierIncrease();
		}

		public int GetSeasonVpTotalForPlayer(string playerHashedId)
		{
			if (SeasonTotalVpAccumulatedPerPlayer == null)
			{
				base.Debug.LogWarning("SeasonTotalVpAccumulatedPerPlayer is null");
				return 0;
			}
			if (SeasonTotalVpAccumulatedPerPlayer.TryGetValue(playerHashedId, out var value))
			{
				base.Debug.LogWarning("Player not found when looking for total Vp accumulated in season");
			}
			return value;
		}

		public bool SaveLeaderboard(IServerService serverService, LeaderboardEntry entry)
		{
			return LeaderboardUpdated = serverService.TrySaveLeaderboardEntry(LeaderboardName, entry);
		}
	}
}
