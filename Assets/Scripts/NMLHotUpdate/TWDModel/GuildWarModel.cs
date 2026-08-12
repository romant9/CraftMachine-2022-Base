using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GuildWarModel : TWDGroupModelChild, ILeaderboardState
	{
		public const int MaxNumberOfBattles = 6;

		private GuildWarDefinition currentWarDefinitionInternal;

		public GuildBattleModel CurrentBattle { get; set; }

		public List<GuildBattleOpponentMatchmakingEntry> NextBattlesOpponentMatchmakingInfo { get; set; }

		public Dictionary<long, List<string>> RegisteredPlayersForBattleSlot { get; set; }

		public Dictionary<string, int> WarTotalVpAccumulatedPerPlayer { get; set; }

		public Dictionary<long, GuildBattleResultInfo> GuildBattleResults { get; set; }

		public List<string> WarParticipants { get; set; }

		public int WarDefinitionId { get; set; }

		public Dictionary<long, GvgBattleEntryMinimized> GvgBattleEntries { get; set; }

		public long timeNextUpdateForGvgBattleEntries { get; set; }

		public bool LeaderboardUpdated { get; set; } = true;

		public string LeaderboardName => Leaderboards.GetLeaderboardNameGuildGlobalWar(WarDefinitionId);

		[JsonIgnore]
		public GuildWarDefinition CurrentWarDefinition
		{
			get
			{
				if (base.gameEconomyData == null)
				{
					base.Debug.LogError("GameEconomyData is null");
					return null;
				}
				if (currentWarDefinitionInternal == null || currentWarDefinitionInternal.Identifier != WarDefinitionId)
				{
					currentWarDefinitionInternal = base.gameEconomyData.FindGuildWarWithId(WarDefinitionId);
				}
				return currentWarDefinitionInternal;
			}
		}

		public void AddBattleEntry(long timeSlot, GvgBattleEntry battleEntry)
		{
			GvgBattleEntries[timeSlot] = new GvgBattleEntryMinimized
			{
				Tier = battleEntry.Tier,
				VictoryPoints = battleEntry.VictoryPoints,
				LastOpponents = battleEntry.LastOpponents
			};
		}

		public void RemoveBattleEntry(long timeSlot)
		{
			GvgBattleEntries.Remove(timeSlot);
		}

		public GuildWarModel()
		{
			WarDefinitionId = -1;
			CurrentBattle = new GuildBattleModel();
			NextBattlesOpponentMatchmakingInfo = new List<GuildBattleOpponentMatchmakingEntry>();
			RegisteredPlayersForBattleSlot = new Dictionary<long, List<string>>();
			WarTotalVpAccumulatedPerPlayer = new Dictionary<string, int>();
			GuildBattleResults = new Dictionary<long, GuildBattleResultInfo>();
			WarParticipants = new List<string>();
			GvgBattleEntries = new Dictionary<long, GvgBattleEntryMinimized>();
			LeaderboardUpdated = true;
		}

		public bool IsCurrentWarOpen(long utcTimeStamp)
		{
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("GameEconomyData is null");
				return false;
			}
			return CurrentWarDefinition?.IsOpen(utcTimeStamp) ?? false;
		}

		public bool IsWarAndBattleOngoing(long utcTimeStamp)
		{
			if (CurrentBattle == null)
			{
				base.Debug.LogError("Current battle is null");
				return false;
			}
			bool num = IsCurrentWarOpen(utcTimeStamp);
			bool flag = CurrentBattle.IsOngoing(utcTimeStamp);
			return num && flag;
		}

		public GuildWarDefinition FindNextGuildWar(long utcTimeStamp)
		{
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("GameEconomyData is null");
				return null;
			}
			if (CurrentWarDefinition == null)
			{
				return base.gameEconomyData.FindNextGuildWar(0L, utcTimeStamp);
			}
			return base.gameEconomyData.FindNextGuildWar(CurrentWarDefinition.EndTimeMilliseconds, utcTimeStamp);
		}

		public GuildWarDefinition FindCurrentOrNextGuildWar(long utcTimeStamp)
		{
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("GameEconomyData is null");
				return null;
			}
			if (CurrentWarDefinition == null)
			{
				return FindNextGuildWar(utcTimeStamp);
			}
			if (CurrentWarDefinition.IsOpen(utcTimeStamp))
			{
				return CurrentWarDefinition;
			}
			return FindNextGuildWar(utcTimeStamp);
		}

		public int GetWarVpTotalForPlayer(string playerHashedId)
		{
			if (WarTotalVpAccumulatedPerPlayer == null)
			{
				base.Debug.LogError("WarTotalVpAccumulatedPerPlayer is null");
				return 0;
			}
			WarTotalVpAccumulatedPerPlayer.TryGetValue(playerHashedId, out var value);
			return value;
		}

		public string AddProgressionToMission(TWDModelManager twdManager, int warId, int sectorId, string uniqueMissionId, string playerId, bool validResult, int vpAmount)
		{
			if (CurrentWarDefinition == null)
			{
				base.Debug.LogError("CurrentWarDefinition is null");
				return "";
			}
			if (CurrentWarDefinition.Identifier != warId)
			{
				return "AddProgressionToMission: War Id does not match, current id: " + CurrentWarDefinition.Identifier + ", param id: " + warId;
			}
			if (CurrentBattle == null)
			{
				base.Debug.LogError("CurrentBattle is null");
				return "";
			}
			if (CurrentBattle.CurrentMapModel == null)
			{
				return "AddProgressionToMission: No map model found!";
			}
			if (CurrentBattle.CurrentMapModel == null)
			{
				base.Debug.LogError("CurrentBattle.CurrentMapModel is null");
				return "";
			}
			if (CurrentBattle.CurrentMapModel.GetSectorModel(sectorId) == null)
			{
				return "AddProgressionToMission: No Sector with id: " + sectorId;
			}
			bool wasPvpCompletion = false;
			if (validResult && !CurrentBattle.CurrentMapModel.AddCompletionToMissionModel(uniqueMissionId, playerId, out wasPvpCompletion))
			{
				return "AddProgressionToMission: Could not find mission! " + sectorId + " " + uniqueMissionId;
			}
			GuildBattleMapSectorModel sectorModel = CurrentBattle.CurrentMapModel.GetSectorModel(sectorId);
			CurrentBattle.UpdateMissionProgressionRewardsForGuildBattle(twdManager, sectorId, uniqueMissionId, playerId, vpAmount);
			if (wasPvpCompletion)
			{
				sectorModel.AddCompletionToArea(CurrentBattle.CurrentMapModel.GetMissionModel(uniqueMissionId).AreaIndex);
			}
			if (CurrentBattle.CompletedSectors == null)
			{
				base.Debug.LogError("CurrentBattle.CompletedSectors is null");
				return "";
			}
			if (sectorModel.IsCompleted() && !CurrentBattle.CompletedSectors.Contains(sectorId))
			{
				CurrentBattle.CompletedSectors.Add(sectorId);
			}
			return "";
		}

		public bool StartWarIfNeeded(int warId, GuildModel guildModel, TWDModelManager twdManager)
		{
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("GameEconomyData is null");
				return false;
			}
			if (!base.gameEconomyData.FindGuildWarWithId(warId).IsOpen(guildModel.TimeStamp))
			{
				base.Debug.LogWarning("StartGuildBattleGroupCommand : War was is not open with id: " + warId);
				return false;
			}
			StartWar(warId, twdManager);
			guildModel.GuildRemotePushNotification?.ScheduledNotifications?.Clear();
			return true;
		}

		private void StartWar(int warId, TWDModelManager twdManager)
		{
			if (WarDefinitionId != warId)
			{
				if (CurrentBattle == null)
				{
					base.Debug.LogError("CurrentBattle is null");
					return;
				}
				if (GuildBattleResults == null)
				{
					base.Debug.LogError("GuildBattleResults is null");
					return;
				}
				if (NextBattlesOpponentMatchmakingInfo == null)
				{
					base.Debug.LogError("NextBattlesOpponentMatchmakingInfo is null");
					return;
				}
				if (RegisteredPlayersForBattleSlot == null)
				{
					base.Debug.LogError("RegisteredPlayersForBattleSlot is null");
					return;
				}
				WarDefinitionId = warId;
				CurrentBattle.WarId = warId;
				WarTotalVpAccumulatedPerPlayer = new Dictionary<string, int>();
				WarParticipants = new List<string>();
				LeaderboardUpdated = true;
				GuildBattleResults.Clear();
				NextBattlesOpponentMatchmakingInfo.Clear();
				RegisteredPlayersForBattleSlot.Clear();
				CurrentBattle.Reset();
				GvgBattleEntries.Clear();
				InitializeBattleSlots(twdManager);
				NotifyChange("GuildWarStarted");
			}
		}

		private void InitializeBattleSlots(TWDModelManager twdManager)
		{
			if (CurrentWarDefinition == null)
			{
				base.Debug.LogError("CurrentWarDefinition is null");
				return;
			}
			if (RegisteredPlayersForBattleSlot == null)
			{
				base.Debug.LogError("CurrentWarDefinition is null");
				return;
			}
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("gameEconomyData is null");
				return;
			}
			if (base.gameEconomyData.GuildWarConfig == null)
			{
				base.Debug.LogError("gameEconomyData.GuildWarConfig is null");
				return;
			}
			long num = CurrentWarDefinition.FirstBattleTimeMilliseconds;
			RegisteredPlayersForBattleSlot.Add(num, new List<string>());
			for (int i = 0; i < 5; i++)
			{
				num += base.gameEconomyData.GuildWarConfig.GuildBattleDurationMilliseconds + base.gameEconomyData.GuildWarConfig.MatchmakingLockdownDurationInMilliseconds;
				RegisteredPlayersForBattleSlot.Add(num, new List<string>());
			}
		}

		public bool StartNewBattle(int randomSeed, GuildModel guild, GuildBattleMatchmakingInfo enemyGuildData, TWDModelManager manager, long timeSlot, bool isFakeBattle = false)
		{
			if (guild == null)
			{
				base.Debug.LogError("guild is null");
				return false;
			}
			if (CurrentBattle == null)
			{
				base.Debug.LogError("CurrentBattle is null");
				return false;
			}
			if (CurrentBattle.RegisteredPlayers == null)
			{
				base.Debug.LogError("CurrentBattle.RegisteredPlayers is null");
				return false;
			}
			CurrentBattle.RegisteredPlayers = GetAllRegisteredPlayersForBattleSlot(timeSlot);
			CurrentBattle.StartBattle(WarDefinitionId, randomSeed, timeSlot, guild.GuildBattleMatchmakingInfo, enemyGuildData, guild.GuildBattleTier, manager, isFakeBattle);
			NotifyChange("GuildBattleStarted");
			return true;
		}

		public bool RegisterPlayerForBattle(string hashedId, out TWDModelResult result, long timeSlot, long utcTimeStamp)
		{
			result = TWDModelResult.Error;
			if (RegisteredPlayersForBattleSlot == null)
			{
				base.Debug.LogError("RegisteredPlayersForBattleSlot is null");
				return false;
			}
			bool flag = false;
			if (CanPlayerRegisterForBattleSlot(timeSlot, hashedId, utcTimeStamp))
			{
				if (RegisteredPlayersForBattleSlot.TryGetValue(timeSlot, out var value))
				{
					if (value == null)
					{
						value = new List<string>();
					}
				}
				else
				{
					value = new List<string>();
					RegisteredPlayersForBattleSlot.Add(timeSlot, value);
				}
				value.Add(hashedId);
				flag = true;
			}
			if (flag)
			{
				NotifyChange("GuildBattlePlayerRegistered");
			}
			result = TWDModelResult.OK;
			return flag;
		}

		public bool ResignPlayerFromBattle(string hashedId, long utcTimeStamp, long timeSlot)
		{
			bool flag = false;
			if (CanPlayerResignFromBattleSlot(timeSlot, hashedId, utcTimeStamp))
			{
				flag = RemovePlayerFromBattleSlot(hashedId, timeSlot);
			}
			if (flag)
			{
				NotifyChange("GuildBattlePlayerResigned", hashedId);
			}
			return flag;
		}

		public int GetRegisteredPlayersCountForBattle(long timeSlot)
		{
			return GetAllRegisteredPlayersForBattleSlot(timeSlot).Count;
		}

		public bool IsPlayerRegisteredForBattle(long timeSlot, string hashedId)
		{
			return GetAllRegisteredPlayersForBattleSlot(timeSlot).Contains(hashedId);
		}

		public bool HasEnoughRegisteredPlayersToStartBattleForTimeSlot(long timeSlot)
		{
			return GetAllRegisteredPlayersForBattleSlot(timeSlot).Count >= base.gameEconomyData.GuildWarConfig.MinPlayersToStartBattle;
		}

		public long GetBattleSlotForTimeStamp(long utcTimeStamp)
		{
			if (CurrentBattle.IsOngoing(utcTimeStamp))
			{
				return CurrentBattle.TimeSlot;
			}
			long num = long.MaxValue;
			foreach (long key in RegisteredPlayersForBattleSlot.Keys)
			{
				if (key >= utcTimeStamp && key < num)
				{
					num = key;
				}
			}
			return num;
		}

		public long GetLockDownTimeForBattleSlot(long timeSlot)
		{
			return timeSlot - base.gameEconomyData.GuildWarConfig.MatchmakingLockdownDurationInMilliseconds;
		}

		public bool IsBattleSlotLocked(long timeSlot, long utcTimestamp)
		{
			if (timeSlot > 0)
			{
				return utcTimestamp >= GetLockDownTimeForBattleSlot(timeSlot);
			}
			return false;
		}

		public override string ToString()
		{
			return $"[GuildWarModel: WarDefinitionId={WarDefinitionId}, gameEconomyData={base.gameEconomyData}]";
		}

		public void EndBattle()
		{
			if (CurrentBattle == null)
			{
				base.Debug.LogError("CurrentBattle is null");
				return;
			}
			CurrentBattle.EndBattle();
			WarParticipants.AddRange(CurrentBattle.RegisteredPlayers);
			RemoveGuildBattleMatchmakingEntry();
		}

		public void SaveGuildBattleInfo()
		{
			GuildBattleResultInfo value = new GuildBattleResultInfo
			{
				EndVictoryPoints = CurrentBattle.EndVictoryPoints,
				EndEnemyVictoryPoints = CurrentBattle.EndEnemyVictoryPoints,
				EnemyLeaderboardInfo = (CurrentBattle.IsFakeBattle ? null : GuildBattleResultInfo.SetCombatParticipants(CurrentBattle.EnemyGuildData.PlayerInfoSnapshot, CurrentBattle.EnemyGuildData.RegisteredPlayersList)),
				RegisteredPlayers = new List<string>(CurrentBattle.RegisteredPlayers),
				EnemyRegisteredPlayers = new List<string>(CurrentBattle.EnemyGuildData.RegisteredPlayersList),
				EnemyGroupId = CurrentBattle.EnemyGuildData.GroupId,
				EnemyGuildName = CurrentBattle.EnemyGuildName,
				GuildTier = CurrentBattle.GuildTier,
				EnemyTier = CurrentBattle.EnemyGuildTier,
				PlayerScores = CurrentBattle.PlayerHighscores,
				BattleResult = CurrentBattle.BattleResult,
				isFakeBattle = CurrentBattle.IsFakeBattle
			};
			GuildBattleResults[CurrentBattle.TimeSlot] = value;
		}

		private void RemoveGuildBattleMatchmakingEntry()
		{
			GuildBattleOpponentMatchmakingEntry guildBattleOpponentMatchmakingEntry = NextBattlesOpponentMatchmakingInfo.Find((GuildBattleOpponentMatchmakingEntry t) => t.StartBattleTimeSlot == CurrentBattle.TimeSlot);
			if (guildBattleOpponentMatchmakingEntry == null)
			{
				base.Debug.LogWarning("No opponentMatchmakingEntry to remove");
			}
			else
			{
				NextBattlesOpponentMatchmakingInfo.Remove(guildBattleOpponentMatchmakingEntry);
			}
		}

		private bool RemovePlayerFromBattleSlot(string hashedId, long timeSlot)
		{
			bool flag = false;
			if (RegisteredPlayersForBattleSlot.ContainsKey(timeSlot))
			{
				flag |= RegisteredPlayersForBattleSlot[timeSlot].Remove(hashedId);
			}
			return flag;
		}

		public List<string> GetAllRegisteredPlayersForBattleSlot(long timeSlot)
		{
			List<string> list = new List<string>();
			if (RegisteredPlayersForBattleSlot.ContainsKey(timeSlot))
			{
				list.AddRange(RegisteredPlayersForBattleSlot[timeSlot]);
			}
			return list;
		}

		public int GetAllValidRegisteredDaysForPlayer(string playerHashedId, long playerTimeStamp)
		{
			if (string.IsNullOrEmpty(playerHashedId))
			{
				base.Debug.LogError("PlayerHashedId is null or empty");
				return int.MaxValue;
			}
			if (RegisteredPlayersForBattleSlot.Count != 6)
			{
				base.Debug.LogError("RegisteredPlayersForBattleSlot doesn't contain 8 battles:" + RegisteredPlayersForBattleSlot.Count);
				return int.MaxValue;
			}
			int num = 0;
			foreach (long key in RegisteredPlayersForBattleSlot.Keys)
			{
				if (DoesBattleSlotCountForPlayer(key, playerHashedId, playerTimeStamp))
				{
					num++;
				}
			}
			return num;
		}

		private bool DoesBattleSlotCountForPlayer(long timeSlot, string hashedId, long playerTimeStamp)
		{
			if (IsPlayerRegisteredForBattle(timeSlot, hashedId))
			{
				if (GuildBattleResults.ContainsKey(timeSlot))
				{
					return true;
				}
				if (CurrentBattle != null && CurrentBattle.IsOngoing(playerTimeStamp) && CurrentBattle.TimeSlot == timeSlot)
				{
					return true;
				}
				if (playerTimeStamp < timeSlot)
				{
					return true;
				}
			}
			return false;
		}

		public bool CanPlayerRegisterForBattleSlot(long timeSlot, string hashedId, long utcTimestamp)
		{
			if (!IsPlayerRegisteredForBattle(timeSlot, hashedId) && GetRegisteredPlayersCountForBattle(timeSlot) < base.gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle)
			{
				return !IsBattleSlotLocked(timeSlot, utcTimestamp);
			}
			return false;
		}

		public bool CanPlayerResignFromBattleSlot(long timeSlot, string hashedId, long utcTimestamp)
		{
			if (IsPlayerRegisteredForBattle(timeSlot, hashedId))
			{
				return !IsBattleSlotLocked(timeSlot, utcTimestamp);
			}
			return false;
		}

		public List<string> GetRegisteredPlayersForCurrentOrNextBattle(long timeSlot)
		{
			if (CurrentBattle != null && CurrentBattle.TimeSlot == timeSlot)
			{
				return CurrentBattle.RegisteredPlayers;
			}
			return GetAllRegisteredPlayersForBattleSlot(timeSlot);
		}

		public List<string> GetWarAndRegisteredParticipants(long timeSlot)
		{
			List<string> list = new List<string>(WarParticipants);
			foreach (string item in GetRegisteredPlayersForCurrentOrNextBattle(timeSlot))
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public int GetWarAndRegisteredCount(long utcTimeStamp)
		{
			int num = WarParticipants.Count;
			long battleSlotForTimeStamp = GetBattleSlotForTimeStamp(utcTimeStamp);
			foreach (KeyValuePair<long, List<string>> item in RegisteredPlayersForBattleSlot)
			{
				if (item.Key >= battleSlotForTimeStamp)
				{
					num = ((CurrentBattle == null || CurrentBattle.TimeSlot != item.Key) ? (num + item.Value.Count) : (num + CurrentBattle.RegisteredPlayers.Count));
				}
			}
			return num;
		}

		public bool SaveOpponentMatchmakingEntry(GuildBattleOpponentMatchmakingEntry entry)
		{
			if (NextBattlesOpponentMatchmakingInfo == null)
			{
				NextBattlesOpponentMatchmakingInfo = new List<GuildBattleOpponentMatchmakingEntry>();
			}
			if (NextBattlesOpponentMatchmakingInfo.Find((GuildBattleOpponentMatchmakingEntry t) => t.StartBattleTimeSlot == entry.StartBattleTimeSlot) == null)
			{
				NextBattlesOpponentMatchmakingInfo.Add(entry);
				return true;
			}
			return false;
		}

		public GuildBattleOpponentMatchmakingEntry GetNextGuildBattleOpponentMatchmakingEntry()
		{
			List<GuildBattleOpponentMatchmakingEntry> nextBattlesOpponentMatchmakingInfo = NextBattlesOpponentMatchmakingInfo;
			if (nextBattlesOpponentMatchmakingInfo != null && nextBattlesOpponentMatchmakingInfo.Count > 0)
			{
				return NextBattlesOpponentMatchmakingInfo[NextBattlesOpponentMatchmakingInfo.Count - 1];
			}
			return null;
		}

		public string GetAllOpponentsGroupIds()
		{
			string text = string.Empty;
			foreach (GuildBattleResultInfo value in GuildBattleResults.Values)
			{
				if (!value.isFakeBattle)
				{
					text = text + value.EnemyGroupId + ",";
				}
			}
			if (!CurrentBattle.IsFakeBattle)
			{
				text += CurrentBattle.EnemyGuildData.GroupId;
			}
			return text;
		}

		public List<long> GetTimeSlotsForGvgBattleEntriesObsolete(GuildModel guildModel, out TWDModelResult result, long currentTimestamp)
		{
			List<long> list = new List<long>();
			result = TWDModelResult.Error;
			foreach (long key in GvgBattleEntries.Keys)
			{
				if (!IsBattleSlotLocked(key, currentTimestamp))
				{
					GvgBattleEntryMinimized battleEntryValue = GvgBattleEntries[key];
					if (IsBattleEntryObsolete(guildModel, battleEntryValue))
					{
						list.Add(key);
					}
				}
			}
			result = TWDModelResult.OK;
			return list;
		}

		public List<long> GetFutureTimeSlots(long currentTimestamp)
		{
			return GvgBattleEntries.Keys.Where((long timeSlot) => !IsBattleSlotLocked(timeSlot, currentTimestamp)).ToList();
		}

		private bool IsBattleEntryObsolete(GuildModel guildModel, GvgBattleEntryMinimized battleEntryValue)
		{
			if (guildModel.GuildBattleTier != battleEntryValue.Tier)
			{
				return true;
			}
			if (guildModel.GvGSeasonModel.CurrentVictoryPoints != battleEntryValue.VictoryPoints)
			{
				return true;
			}
			if (GetAllOpponentsGroupIds() != battleEntryValue.LastOpponents)
			{
				return true;
			}
			return false;
		}

		public bool SaveLeaderboard(IServerService serverService, LeaderboardEntry entry)
		{
			return LeaderboardUpdated = serverService.TrySaveLeaderboardEntry(LeaderboardName, entry);
		}
	}
}
