using System;
using System.Collections.Generic;
using System.Globalization;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class WorldBossModelManager : TWDModelObject
	{
		private sealed class PendingParticipation
		{
			public int SequenceId;

			public int SeasonId;

			public int CycleId;
		}

		private sealed class PendingWorldBossAttack
		{
			public int SequenceId;

			public List<WorldBossFatigueChargeSnapshot> FatigueSnapshots;
		}

		public sealed class WorldBossFatigueChargeSnapshot
		{
			public string SurvivorId { get; set; }

			public bool ExistedBefore { get; set; }

			public int ChargesBefore { get; set; }

			public long BaseUtcMsBefore { get; set; }
		}

		public const string WorldBossTankBossCapturePoint = "BOSS";

		private const long DayMilliseconds = 86400000L;

		private const long HourMilliseconds = 3600000L;

		private const int MaxPendingParticipations = 8;

		private List<PendingParticipation> pendingParticipations;

		private const int MaxPendingWorldBossAttacks = 8;

		private List<PendingWorldBossAttack> pendingWorldBossAttacks;

		[JsonIgnore]
		public WorldBossGuildFullSnapshot WorldBossGuildFullSnapshot;

		public const string SystemId = "SystemBase.WorldBoss";

		public const string OpeningPopupSeenTogglePrefix = "Toggle.WorldBossPopupSeen.";

		public WorldBossAttackTargetData AttackTarget;

		private const int CellStatusEmpty = 0;

		private const int CellStatusFighting = 1;

		private const int CellStatusOccupied = 2;

		private const string TowerACapturePoint = "TOWER-A";

		private const string TowerBCapturePoint = "TOWER-B";

		private const string DepotCapturePoint = "DEPOT";

		public const string BossCapturePoint = "BOSS";

		private const double MsPerHour = 3600000.0;

		private long Now => (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();

		private GameEconomyData Ged => base.manager?.GameEconomyData;

		public List<WorldBossSeasonCycleRecord> ParticipatedCycleHistory { get; set; }

		public List<WorldBossSeasonCycleRecord> ShownSettlementHistory { get; set; }

		[JsonIgnore]
		public bool IsAttackTargetActive
		{
			get
			{
				if (AttackTarget != null)
				{
					return AttackTarget.IsActive;
				}
				return false;
			}
		}

		public static bool TryGetDailyRefreshStartUtcMs(long utcTimeStamp, int dailyRefreshUtcHour, out long refreshStartUtcMs)
		{
			refreshStartUtcMs = 0L;
			if (utcTimeStamp < 0 || dailyRefreshUtcHour < 0 || dailyRefreshUtcHour >= 24)
			{
				return false;
			}
			long num = utcTimeStamp / 86400000 * 86400000 + (long)dailyRefreshUtcHour * 3600000L;
			refreshStartUtcMs = ((utcTimeStamp >= num) ? num : (num - 86400000));
			return true;
		}

		public bool RecordParticipation(int seasonId, int cycleId)
		{
			if (seasonId <= 0 || cycleId <= 0)
			{
				return false;
			}
			ParticipatedCycleHistory = ParticipatedCycleHistory ?? new List<WorldBossSeasonCycleRecord>();
			if (ContainsCycle(ParticipatedCycleHistory, seasonId, cycleId))
			{
				return false;
			}
			ParticipatedCycleHistory.Add(new WorldBossSeasonCycleRecord(seasonId, cycleId));
			return true;
		}

		public void TrackOptimisticParticipation(int sequenceId, int seasonId, int cycleId)
		{
			if (seasonId > 0 && cycleId > 0)
			{
				pendingParticipations = pendingParticipations ?? new List<PendingParticipation>();
				while (pendingParticipations.Count >= 8)
				{
					pendingParticipations.RemoveAt(0);
				}
				pendingParticipations.Add(new PendingParticipation
				{
					SequenceId = sequenceId,
					SeasonId = seasonId,
					CycleId = cycleId
				});
			}
		}

		public bool RollbackOptimisticParticipation(int sequenceId)
		{
			if (pendingParticipations == null)
			{
				return false;
			}
			for (int i = 0; i < pendingParticipations.Count; i++)
			{
				PendingParticipation pending = pendingParticipations[i];
				if (pending != null && pending.SequenceId == sequenceId)
				{
					pendingParticipations.RemoveAt(i);
					if (ParticipatedCycleHistory == null)
					{
						return false;
					}
					return ParticipatedCycleHistory.RemoveAll((WorldBossSeasonCycleRecord record) => record != null && record.SeasonId == pending.SeasonId && record.CycleId == pending.CycleId) > 0;
				}
			}
			return false;
		}

		public void TrackOptimisticAttackCell(int sequenceId, List<WorldBossFatigueChargeSnapshot> fatigueSnapshots)
		{
			if (sequenceId >= 0)
			{
				pendingWorldBossAttacks = pendingWorldBossAttacks ?? new List<PendingWorldBossAttack>();
				while (pendingWorldBossAttacks.Count >= 8)
				{
					pendingWorldBossAttacks.RemoveAt(0);
				}
				pendingWorldBossAttacks.Add(new PendingWorldBossAttack
				{
					SequenceId = sequenceId,
					FatigueSnapshots = fatigueSnapshots
				});
			}
		}

		public bool RollbackOptimisticAttackCell(int sequenceId)
		{
			if (pendingWorldBossAttacks == null)
			{
				return false;
			}
			for (int i = 0; i < pendingWorldBossAttacks.Count; i++)
			{
				PendingWorldBossAttack pendingWorldBossAttack = pendingWorldBossAttacks[i];
				if (pendingWorldBossAttack != null && pendingWorldBossAttack.SequenceId == sequenceId)
				{
					pendingWorldBossAttacks.RemoveAt(i);
					RollbackFatigueCharges(pendingWorldBossAttack.FatigueSnapshots);
					AttackTarget = null;
					return true;
				}
			}
			return false;
		}

		private void RollbackFatigueCharges(List<WorldBossFatigueChargeSnapshot> snapshots)
		{
			if (snapshots == null || snapshots.Count == 0)
			{
				return;
			}
			Dictionary<string, WorldBossHeroFatigueEntry> dictionary = (base.manager?.Player?.WorldBossHeroFatigue)?.Entries;
			if (dictionary == null)
			{
				return;
			}
			foreach (WorldBossFatigueChargeSnapshot snapshot in snapshots)
			{
				if (snapshot != null && !string.IsNullOrEmpty(snapshot.SurvivorId))
				{
					if (snapshot.ExistedBefore)
					{
						dictionary[snapshot.SurvivorId] = new WorldBossHeroFatigueEntry(snapshot.ChargesBefore, snapshot.BaseUtcMsBefore);
					}
					else
					{
						dictionary.Remove(snapshot.SurvivorId);
					}
				}
			}
		}

		public bool TryGetLatestUnshownSettlementTarget(out int seasonId, out int cycleId)
		{
			return TryGetLatestUnshownSettlementTarget(GetCurrentSeasonId(), out seasonId, out cycleId);
		}

		public bool TryGetLatestUnshownSettlementTarget(int currentSeasonId, out int seasonId, out int cycleId)
		{
			seasonId = 0;
			cycleId = 0;
			if (currentSeasonId <= 0 || ParticipatedCycleHistory == null)
			{
				return false;
			}
			long num = long.MinValue;
			foreach (WorldBossSeasonCycleRecord item in ParticipatedCycleHistory)
			{
				if (item == null || item.SeasonId != currentSeasonId || IsSettlementShown(item.SeasonId, item.CycleId))
				{
					continue;
				}
				WorldBossCycleDefinition worldBossCycleDefinition = Ged?.FindWorldBossCycleDefinition(item.SeasonId, item.CycleId);
				if (worldBossCycleDefinition == null || worldBossCycleDefinition.EndTimeMilliseconds <= Now)
				{
					long num2 = worldBossCycleDefinition?.EndTimeMilliseconds ?? item.CycleId;
					if (cycleId == 0 || num2 > num || (num2 == num && item.CycleId > cycleId))
					{
						seasonId = item.SeasonId;
						cycleId = item.CycleId;
						num = num2;
					}
				}
			}
			if (seasonId > 0)
			{
				return cycleId > 0;
			}
			return false;
		}

		public bool IsSettlementShown(int seasonId, int cycleId)
		{
			return ContainsCycle(ShownSettlementHistory, seasonId, cycleId);
		}

		public void MarkSettlementShown(int seasonId, int cycleId)
		{
			if (seasonId > 0 && cycleId > 0)
			{
				ShownSettlementHistory = ShownSettlementHistory ?? new List<WorldBossSeasonCycleRecord>();
				if (!ContainsCycle(ShownSettlementHistory, seasonId, cycleId))
				{
					ShownSettlementHistory.Add(new WorldBossSeasonCycleRecord(seasonId, cycleId));
				}
			}
		}

		private static bool ContainsCycle(List<WorldBossSeasonCycleRecord> history, int seasonId, int cycleId)
		{
			if (history == null)
			{
				return false;
			}
			foreach (WorldBossSeasonCycleRecord item in history)
			{
				if (item != null && item.SeasonId == seasonId && item.CycleId == cycleId)
				{
					return true;
				}
			}
			return false;
		}

		public WorldBossUnlockState GetUnlockState()
		{
			PlayerModel playerModel = base.manager?.Player;
			if (playerModel == null)
			{
				return WorldBossUnlockState.LevelNotReached;
			}
			int num = Ged?.GetSystemOpenById("SystemBase.WorldBoss")?.OpenCampLv ?? 14;
			if (playerModel.CouncilLevel < num)
			{
				return WorldBossUnlockState.LevelNotReached;
			}
			if (!playerModel.IsGuildMember)
			{
				return WorldBossUnlockState.NotInGuild;
			}
			return WorldBossUnlockState.Unlocked;
		}

		public bool ShouldShowGoldLight()
		{
			PlayerModel playerModel = base.manager?.Player;
			if (playerModel == null)
			{
				return false;
			}
			if (GetUnlockState() != WorldBossUnlockState.Unlocked || !IsCycleOpen())
			{
				return false;
			}
			if (GetCurrentSeasonId() == playerModel.WorldBossLastEnteredSeasonId)
			{
				return GetCurrentCycleId() != playerModel.WorldBossLastEnteredCycleId;
			}
			return true;
		}

		public static string GetOpeningPopupSeenToggleKey(int seasonId, int cycleId)
		{
			return "Toggle.WorldBossPopupSeen." + seasonId + "_" + cycleId;
		}

		public string GetOpeningPopupSeenToggleKey()
		{
			return GetOpeningPopupSeenToggleKey(GetCurrentSeasonId(), GetCurrentCycleId());
		}

		public bool IsGuildSignedUpForCurrentCycle(WorldBossGuildBaseState baseState)
		{
			if (baseState == null)
			{
				return false;
			}
			if (baseState.SeasonId != GetCurrentSeasonId())
			{
				return false;
			}
			if (baseState.CycleId != GetCurrentCycleId())
			{
				return false;
			}
			return baseState.Status >= WorldBossCycleStatus.SignedUp;
		}

		public bool ShouldShowOpeningPopup(WorldBossGuildBaseState baseState)
		{
			PlayerModel playerModel = base.manager?.Player;
			if (playerModel?.Blackboard == null)
			{
				return false;
			}
			if (GetUnlockState() != WorldBossUnlockState.Unlocked || !IsCycleOpen())
			{
				return false;
			}
			if (!IsGuildSignedUpForCurrentCycle(baseState))
			{
				return false;
			}
			return !playerModel.Blackboard.IsToggleOn(GetOpeningPopupSeenToggleKey());
		}

		public WorldBossSeasonDefinition GetCurrentSeason()
		{
			return Ged?.FindWorldBossSeasonWithTime(Now);
		}

		public bool IsSeasonOpen()
		{
			return GetCurrentSeason() != null;
		}

		public bool IsSeasonOpen(int season)
		{
			return (Ged?.FindWorldBossSeasonDefinition(season))?.IsOpen(Now) ?? false;
		}

		public int GetCurrentSeasonId()
		{
			return GetCurrentSeason()?.Season ?? 0;
		}

		public long GetTimeUntilSeasonEndMs()
		{
			return GetCurrentSeason()?.TimeUntilEndMilliseconds(Now) ?? 0;
		}

		public long GetTimeUntilSeasonStartMs(int season)
		{
			return (Ged?.FindWorldBossSeasonDefinition(season))?.TimeUntilStartMilliseconds(Now) ?? 0;
		}

		public WorldBossCycleDefinition GetCurrentCycle()
		{
			WorldBossSeasonDefinition currentSeason = GetCurrentSeason();
			if (currentSeason == null)
			{
				return null;
			}
			return Ged?.FindWorldBossCycleWithTime(Now, currentSeason.Season);
		}

		public bool IsCycleOpen()
		{
			return GetCurrentCycle() != null;
		}

		public bool IsCycleOpen(int seasonId, int cycleId)
		{
			return (Ged?.FindWorldBossCycleDefinition(seasonId, cycleId))?.IsOpen(Now) ?? false;
		}

		public int GetCurrentCycleId()
		{
			return GetCurrentCycle()?.Cycle ?? 0;
		}

		public int GetNextCycleId()
		{
			return GetNextCycle()?.Cycle ?? 0;
		}

		public long GetTimeUntilCycleEndMs()
		{
			return GetCurrentCycle()?.TimeUntilEndMilliseconds(Now) ?? 0;
		}

		public long GetTimeUntilCycleStartMs(int seasonId, int cycleId)
		{
			return (Ged?.FindWorldBossCycleDefinition(seasonId, cycleId))?.TimeUntilStartMilliseconds(Now) ?? 0;
		}

		public WorldBossCycleDefinition GetNextCycle()
		{
			WorldBossSeasonDefinition currentSeason = GetCurrentSeason();
			if (currentSeason == null)
			{
				return null;
			}
			long currentEndTime = (Ged?.FindWorldBossCycleWithTime(Now, currentSeason.Season))?.EndTimeMilliseconds ?? Now;
			return Ged?.FindNextWorldBossCycle(currentEndTime, currentSeason.Season);
		}

		public bool IsOffSeason()
		{
			if (!IsCycleOpen())
			{
				return GetNextCycle() != null;
			}
			return false;
		}

		public long GetTimeUntilNextCycleStartMs()
		{
			return GetNextCycle()?.TimeUntilStartMilliseconds(Now) ?? 0;
		}

		public long GetTimeUntilSignUpDeadlineMs()
		{
			WorldBossCycleDefinition openingOrCurrentCycle = GetOpeningOrCurrentCycle();
			if (openingOrCurrentCycle == null)
			{
				return 0L;
			}
			long num = openingOrCurrentCycle.SignUpDeadlineMilliseconds - Now;
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		public bool IsCurrentCycleSignUpOpen()
		{
			return GetOpeningOrCurrentCycle()?.IsSignUpOpen(Now) ?? false;
		}

		public bool IsCurrentCycleDifficultySelectionOpen()
		{
			return GetOpeningOrCurrentCycle()?.IsDifficultySelectionOpen(Now) ?? false;
		}

		private WorldBossCycleDefinition GetOpeningOrCurrentCycle()
		{
			return GetCurrentCycle() ?? GetNextCycle();
		}

		public int GetHeroChargeLimit()
		{
			return (Ged?.WorldBossConfig?.DailyHeroBattleLimit).GetValueOrDefault();
		}

		public long GetHeroRecoverMs()
		{
			int valueOrDefault = (Ged?.WorldBossConfig?.HeroBattleRecoverDuration).GetValueOrDefault();
			if (valueOrDefault <= 0)
			{
				return 0L;
			}
			long num = (long)valueOrDefault * 1000L;
			double myDepotFatigueRecoverySpeedupPercent = GetMyDepotFatigueRecoverySpeedupPercent();
			if (myDepotFatigueRecoverySpeedupPercent <= 0.0 || double.IsNaN(myDepotFatigueRecoverySpeedupPercent) || double.IsInfinity(myDepotFatigueRecoverySpeedupPercent))
			{
				return num;
			}
			double num2 = (double)num * 100.0 / (100.0 + myDepotFatigueRecoverySpeedupPercent);
			if (!(num2 <= 1.0))
			{
				return (long)Math.Ceiling(num2);
			}
			return 1L;
		}

		public bool ClearHeroFatigueIfOutdated(int seasonId, int cycleId)
		{
			if (seasonId <= 0 || cycleId <= 0)
			{
				return false;
			}
			PlayerModel playerModel = base.manager?.Player;
			WorldBossHeroFatigueState worldBossHeroFatigueState = playerModel?.WorldBossHeroFatigue;
			if (worldBossHeroFatigueState == null || worldBossHeroFatigueState.IsForCycle(seasonId, cycleId))
			{
				return false;
			}
			playerModel.WorldBossHeroFatigue = null;
			return true;
		}

		private WorldBossHeroFatigueEntry GetFatigueEntry(int seasonId, int cycleId, string survivorId)
		{
			WorldBossHeroFatigueState worldBossHeroFatigueState = base.manager?.Player?.WorldBossHeroFatigue;
			if (worldBossHeroFatigueState == null || !worldBossHeroFatigueState.IsForCycle(seasonId, cycleId) || worldBossHeroFatigueState.Entries == null || string.IsNullOrEmpty(survivorId))
			{
				return null;
			}
			if (!worldBossHeroFatigueState.Entries.TryGetValue(survivorId, out var value))
			{
				return null;
			}
			return value;
		}

		public int GetHeroCharges(string survivorId)
		{
			return GetHeroCharges(GetCurrentSeasonId(), GetCurrentCycleId(), survivorId);
		}

		public int GetHeroCharges(int seasonId, int cycleId, string survivorId)
		{
			int heroChargeLimit = GetHeroChargeLimit();
			return GetFatigueEntry(seasonId, cycleId, survivorId)?.GetCurrentCharges(heroChargeLimit, GetHeroRecoverMs(), Now) ?? heroChargeLimit;
		}

		public long GetHeroNextRecoverRemainingMs(string survivorId)
		{
			return GetFatigueEntry(GetCurrentSeasonId(), GetCurrentCycleId(), survivorId)?.GetNextRecoverRemainingMs(GetHeroChargeLimit(), GetHeroRecoverMs(), Now) ?? 0;
		}

		public bool IsHeroDispatched(string survivorId)
		{
			return !string.IsNullOrEmpty(GetSurvivorDispatchedCapturePoint(survivorId));
		}

		public string GetSurvivorDispatchedCapturePoint(string survivorId)
		{
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			string text = base.manager?.Player?.HashedId;
			if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(text) || string.IsNullOrEmpty(survivorId))
			{
				return string.Empty;
			}
			foreach (WorldBossCapturePointSnapshot capturePoint in worldBossGuildFullSnapshot.CapturePoints)
			{
				if (capturePoint?.CellStates == null || IsPveType(capturePoint.CapturePointType))
				{
					continue;
				}
				foreach (WorldBossCellStateSnapshot cellState in capturePoint.CellStates)
				{
					if (cellState != null && !(cellState.OccupyingPlayerHashedId != text) && cellState.OccupyingSurvivorIds != null && cellState.OccupyingSurvivorIds.Contains(survivorId))
					{
						return capturePoint.CapturePoint ?? string.Empty;
					}
				}
			}
			return string.Empty;
		}

		public bool IsHeroReturning(string survivorId)
		{
			return GetSurvivorReturnRemainingMs(survivorId) > 0;
		}

		public bool CanHeroBattle(string survivorId)
		{
			return CanHeroBattle(GetCurrentSeasonId(), GetCurrentCycleId(), survivorId);
		}

		public bool CanHeroBattle(int seasonId, int cycleId, string survivorId)
		{
			if (IsHeroDispatched(survivorId) || IsHeroReturning(survivorId))
			{
				return false;
			}
			if (GetHeroChargeLimit() <= 0)
			{
				return true;
			}
			return GetHeroCharges(seasonId, cycleId, survivorId) > 0;
		}

		public WorldBossSurvivorStatusView GetSurvivorStatus(string survivorId)
		{
			WorldBossSurvivorStatusView worldBossSurvivorStatusView = new WorldBossSurvivorStatusView();
			if (string.IsNullOrEmpty(survivorId))
			{
				return worldBossSurvivorStatusView;
			}
			worldBossSurvivorStatusView.DispatchedCapturePoint = GetSurvivorDispatchedCapturePoint(survivorId);
			worldBossSurvivorStatusView.IsDispatched = !string.IsNullOrEmpty(worldBossSurvivorStatusView.DispatchedCapturePoint);
			worldBossSurvivorStatusView.ReturnRemainingMs = GetSurvivorReturnRemainingMs(survivorId);
			worldBossSurvivorStatusView.IsReturning = worldBossSurvivorStatusView.ReturnRemainingMs > 0;
			return worldBossSurvivorStatusView;
		}

		public long GetSurvivorReturnRemainingMs(string survivorId)
		{
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			string text = base.manager?.Player?.HashedId;
			if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(text) || string.IsNullOrEmpty(survivorId))
			{
				return 0L;
			}
			long num = 0L;
			foreach (WorldBossCapturePointSnapshot capturePoint in worldBossGuildFullSnapshot.CapturePoints)
			{
				if (capturePoint?.ReturningTeams == null)
				{
					continue;
				}
				foreach (WorldBossReturningTeamModel returningTeam in capturePoint.ReturningTeams)
				{
					if (returningTeam?.SurvivorIds != null && !(returningTeam.PlayerHashedId != text) && returningTeam.SurvivorIds.Contains(survivorId))
					{
						long returningTeamRemainingMs = GetReturningTeamRemainingMs(returningTeam);
						if (returningTeamRemainingMs > num)
						{
							num = returningTeamRemainingMs;
						}
					}
				}
			}
			return num;
		}

		public WorldBossReturningTeamModel FindMyReturningTeam(string capturePoint, string returningTeamId)
		{
			PlayerModel playerModel = base.manager?.Player;
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			if (playerModel == null || worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(returningTeamId))
			{
				return null;
			}
			string hashedId = playerModel.HashedId;
			foreach (WorldBossCapturePointSnapshot capturePoint2 in worldBossGuildFullSnapshot.CapturePoints)
			{
				if (capturePoint2?.ReturningTeams == null || (!string.IsNullOrEmpty(capturePoint) && capturePoint2.CapturePoint != capturePoint))
				{
					continue;
				}
				foreach (WorldBossReturningTeamModel returningTeam in capturePoint2.ReturningTeams)
				{
					if (returningTeam != null && returningTeam.Id == returningTeamId && returningTeam.PlayerHashedId == hashedId)
					{
						return returningTeam;
					}
				}
			}
			return null;
		}

		public long GetReturningTeamRemainingMs(WorldBossReturningTeamModel team)
		{
			if (team == null)
			{
				return 0L;
			}
			long num = team.ReturnEndUtcMs - Now;
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		public int GetInstantReturnGoldCost(WorldBossReturningTeamModel team)
		{
			long returningTeamRemainingMs = GetReturningTeamRemainingMs(team);
			if (returningTeamRemainingMs <= 0)
			{
				return 0;
			}
			int valueOrDefault = (Ged?.WorldBossConfig?.WithdrawGoldCost).GetValueOrDefault();
			if (valueOrDefault <= 0)
			{
				return 0;
			}
			long num = ((returningTeamRemainingMs + 999) / 1000 + valueOrDefault - 1) / valueOrDefault;
			if (num >= 1)
			{
				return (int)num;
			}
			return 1;
		}

		public int GetInstantReturnGoldCost(string capturePoint, string returningTeamId)
		{
			return GetInstantReturnGoldCost(FindMyReturningTeam(capturePoint, returningTeamId));
		}

		public List<WorldBossReturningTeamView> GetMyReturningTeams()
		{
			List<WorldBossReturningTeamView> list = new List<WorldBossReturningTeamView>();
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			string text = base.manager?.Player?.HashedId;
			if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(text))
			{
				return list;
			}
			foreach (WorldBossCapturePointSnapshot capturePoint in worldBossGuildFullSnapshot.CapturePoints)
			{
				if (capturePoint?.ReturningTeams == null)
				{
					continue;
				}
				foreach (WorldBossReturningTeamModel returningTeam in capturePoint.ReturningTeams)
				{
					if (returningTeam != null && !(returningTeam.PlayerHashedId != text))
					{
						long returningTeamRemainingMs = GetReturningTeamRemainingMs(returningTeam);
						if (returningTeamRemainingMs > 0)
						{
							list.Add(new WorldBossReturningTeamView
							{
								ReturningTeamId = returningTeam.Id,
								CapturePoint = capturePoint.CapturePoint,
								SurvivorIds = (returningTeam.SurvivorIds ?? new List<string>()),
								StartUtcMs = returningTeam.StartUtcMs,
								ReturnEndUtcMs = returningTeam.ReturnEndUtcMs,
								RemainingMs = returningTeamRemainingMs,
								InstantReturnGoldCost = GetInstantReturnGoldCost(returningTeam)
							});
						}
					}
				}
			}
			return list;
		}

		public List<WorldBossDispatchedTeamView> GetMyDispatchedTeams()
		{
			List<WorldBossDispatchedTeamView> list = new List<WorldBossDispatchedTeamView>();
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			string text = base.manager?.Player?.HashedId;
			if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(text))
			{
				return list;
			}
			long now = Now;
			foreach (WorldBossCapturePointSnapshot capturePoint in worldBossGuildFullSnapshot.CapturePoints)
			{
				if (capturePoint?.CellStates == null || IsPveType(capturePoint.CapturePointType))
				{
					continue;
				}
				foreach (WorldBossCellStateSnapshot cellState in capturePoint.CellStates)
				{
					if (cellState != null && !(cellState.OccupyingPlayerHashedId != text))
					{
						long num = ((cellState.OccupiedAtUtcMs > 0) ? (now - cellState.OccupiedAtUtcMs) : 0);
						list.Add(new WorldBossDispatchedTeamView
						{
							CapturePoint = cellState.CapturePoint,
							Cell = cellState.Cell,
							SurvivorIds = (cellState.OccupyingSurvivorIds ?? new List<string>()),
							DefenderRemainingDurability = cellState.DefenderRemainingDurability,
							OccupiedAtUtcMs = cellState.OccupiedAtUtcMs,
							DispatchedMs = ((num > 0) ? num : 0)
						});
					}
				}
			}
			return list;
		}

		public int GetMyDispatchedTeamCount()
		{
			return GetMyDispatchedTeams().Count;
		}

		public HashSet<string> GetMyDeployedSurvivorIds()
		{
			HashSet<string> hashSet = new HashSet<string>();
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			string text = base.manager?.Player?.HashedId;
			if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(text))
			{
				return hashSet;
			}
			foreach (WorldBossCapturePointSnapshot capturePoint in worldBossGuildFullSnapshot.CapturePoints)
			{
				if (capturePoint?.CellStates == null || IsPveType(capturePoint.CapturePointType))
				{
					continue;
				}
				foreach (WorldBossCellStateSnapshot cellState in capturePoint.CellStates)
				{
					if (cellState?.OccupyingSurvivorIds == null || cellState.OccupyingPlayerHashedId != text)
					{
						continue;
					}
					foreach (string occupyingSurvivorId in cellState.OccupyingSurvivorIds)
					{
						if (!string.IsNullOrEmpty(occupyingSurvivorId))
						{
							hashSet.Add(occupyingSurvivorId);
						}
					}
				}
			}
			return hashSet;
		}

		public HashSet<string> GetMyReturningSurvivorIds()
		{
			HashSet<string> hashSet = new HashSet<string>();
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			string text = base.manager?.Player?.HashedId;
			if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(text))
			{
				return hashSet;
			}
			foreach (WorldBossCapturePointSnapshot capturePoint in worldBossGuildFullSnapshot.CapturePoints)
			{
				if (capturePoint?.ReturningTeams == null)
				{
					continue;
				}
				foreach (WorldBossReturningTeamModel returningTeam in capturePoint.ReturningTeams)
				{
					if (returningTeam?.SurvivorIds == null || returningTeam.PlayerHashedId != text || GetReturningTeamRemainingMs(returningTeam) <= 0)
					{
						continue;
					}
					foreach (string survivorId in returningTeam.SurvivorIds)
					{
						if (!string.IsNullOrEmpty(survivorId))
						{
							hashSet.Add(survivorId);
						}
					}
				}
			}
			return hashSet;
		}

		public int GetDispatchTeamLimit()
		{
			return (Ged?.WorldBossConfig?.TeamLimit).GetValueOrDefault();
		}

		public long GetCellRemainingLockMs(WorldBossCellStateSnapshot cell)
		{
			if (cell == null || cell.Status != 1 || cell.LastBattleStartUtcMs <= 0)
			{
				return 0L;
			}
			long num = cell.LastBattleStartUtcMs + cell.LastBattleTimeLimitMs - Now;
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		public long GetCellRemainingLockMs(string capturePoint, string cell)
		{
			List<WorldBossCapturePointSnapshot> list = WorldBossGuildFullSnapshot?.CapturePoints;
			if (list == null || string.IsNullOrEmpty(capturePoint) || string.IsNullOrEmpty(cell))
			{
				return 0L;
			}
			foreach (WorldBossCapturePointSnapshot item in list)
			{
				if (item?.CellStates == null || item.CapturePoint != capturePoint)
				{
					continue;
				}
				foreach (WorldBossCellStateSnapshot cellState in item.CellStates)
				{
					if (cellState != null && cellState.Cell == cell)
					{
						return GetCellRemainingLockMs(cellState);
					}
				}
			}
			return 0L;
		}

		public string GetCellOccupierName(WorldBossCellStateSnapshot cell)
		{
			return cell?.OccupyingPlayerName ?? string.Empty;
		}

		public PlayerEmblem GetCellOccupierEmblem(WorldBossCellStateSnapshot cell)
		{
			return ParseEmblem(cell?.OccupyingPlayerEmblem);
		}

		public string GetCellOccupierName(string capturePoint, string cell)
		{
			return GetCellOccupierName(FindCellSnapshot(capturePoint, cell));
		}

		public PlayerEmblem GetCellOccupierEmblem(string capturePoint, string cell)
		{
			return GetCellOccupierEmblem(FindCellSnapshot(capturePoint, cell));
		}

		private WorldBossCellStateSnapshot FindCellSnapshot(string capturePoint, string cell)
		{
			List<WorldBossCapturePointSnapshot> list = WorldBossGuildFullSnapshot?.CapturePoints;
			if (list == null || string.IsNullOrEmpty(capturePoint) || string.IsNullOrEmpty(cell))
			{
				return null;
			}
			foreach (WorldBossCapturePointSnapshot item in list)
			{
				if (item?.CellStates == null || item.CapturePoint != capturePoint)
				{
					continue;
				}
				foreach (WorldBossCellStateSnapshot cellState in item.CellStates)
				{
					if (cellState != null && cellState.Cell == cell)
					{
						return cellState;
					}
				}
			}
			return null;
		}

		public static PlayerEmblem ParseEmblem(string serialized)
		{
			if (string.IsNullOrEmpty(serialized))
			{
				return null;
			}
			string[] array = serialized.Split(';');
			if (array.Length < 3)
			{
				return null;
			}
			if (int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2) && int.TryParse(array[2], out var result3))
			{
				return new PlayerEmblem
				{
					IconIndex = result,
					BorderIndex = result2,
					ColorIndex = result3
				};
			}
			return null;
		}

		public long GetMyGuildScore()
		{
			return (WorldBossGuildFullSnapshot?.GuildFullState?.GuildScore).GetValueOrDefault();
		}

		public long GetOpponentGuildScore()
		{
			return WorldBossGuildFullSnapshot?.OpponentGuildScore ?? 0;
		}

		public WorldBossDifficultyDefinition GetSettlementRewardDefinition(int seasonId, int difficulty)
		{
			return Ged?.FindWorldBossDifficultyDefinition(seasonId, difficulty);
		}

		public void SetAttackCellTarget(int seasonId, int cycleId, string capturePoint, string cell, List<string> participantSurvivorIds)
		{
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = Ged?.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, WorldBossGuildFullSnapshot.Match.BattleDifficulty);
			bool isPVECapturePoint = worldBossBattlegroundDefinition?.IsPVECapturePointType() ?? false;
			int num = (Ged?.WorldBossConfig?.BattleTimeLimit).GetValueOrDefault() * 1000;
			WorldBossMissionType worldBossMissionType = ResolveMissionTypeForCell(worldBossBattlegroundDefinition, capturePoint, cell);
			WorldBossMissionModel worldBossMissionModel = WorldBossMissionModel.Create(worldBossBattlegroundDefinition, capturePoint, cell, Ged, worldBossMissionType);
			worldBossMissionModel.BattleStartUtcMs = Now;
			worldBossMissionModel.TimeLimitMs = num;
			base.manager.Player.ResetIAttackTargetMapMission();
			(AttackTarget ?? (AttackTarget = new WorldBossAttackTargetData())).AttackCell(worldBossMissionModel, seasonId, cycleId, capturePoint, cell, isPVECapturePoint, participantSurvivorIds, num);
			base.manager.Player.ShouldConsumeMissionCurrency = true;
			if (worldBossMissionType == WorldBossMissionType.PVP)
			{
				AttackTarget.DefenderTeam = ResolveDefenderTeam(capturePoint, cell);
			}
		}

		public void SetAttackTankTarget(int seasonId, int cycleId, string bossBattleId = null)
		{
			WorldBossBattlegroundDefinition def = Ged?.FindWorldBossBattlegroundDefinitionByCapturePoint("BOSS", WorldBossGuildFullSnapshot.Match.BattleDifficulty);
			int num = (Ged?.WorldBossConfig?.BattleTimeLimit).GetValueOrDefault() * 1000;
			WorldBossMissionType worldBossMissionType = WorldBossMissionType.BOSS;
			WorldBossMissionModel worldBossMissionModel = WorldBossMissionModel.Create(def, "BOSS", "", Ged, worldBossMissionType);
			worldBossMissionModel.BattleStartUtcMs = Now;
			worldBossMissionModel.TimeLimitMs = num;
			base.manager.Player.ResetIAttackTargetMapMission();
			(AttackTarget ?? (AttackTarget = new WorldBossAttackTargetData())).AttackCell(worldBossMissionModel, seasonId, cycleId, "BOSS", "", isPVECapturePoint: true, null, num, bossBattleId);
			base.manager.Player.ShouldConsumeMissionCurrency = true;
		}

		public WorldBossMissionType ResolveMissionTypeForCell(WorldBossBattlegroundDefinition definition, string capturePoint, string cell)
		{
			if (definition == null)
			{
				return WorldBossMissionType.PVE;
			}
			if (definition.IsPVECapturePointType())
			{
				return WorldBossMissionType.PVE;
			}
			if (definition.IsBOSSCapturePointType())
			{
				return WorldBossMissionType.BOSS;
			}
			if (!HasEnemyDefenderSnapshot(capturePoint, cell))
			{
				return WorldBossMissionType.PVE;
			}
			return WorldBossMissionType.PVP;
		}

		private bool HasEnemyDefenderSnapshot(string capturePoint, string cell)
		{
			WorldBossCellStateSnapshot worldBossCellStateSnapshot = FindCellSnapshot(capturePoint, cell);
			if (worldBossCellStateSnapshot == null || worldBossCellStateSnapshot.Status != 2 || !worldBossCellStateSnapshot.HasDefender || string.IsNullOrEmpty(worldBossCellStateSnapshot.OccupyingGroupId))
			{
				return false;
			}
			string text = WorldBossGuildFullSnapshot?.GuildFullState?.GroupId;
			if (string.IsNullOrEmpty(text) || string.Equals(worldBossCellStateSnapshot.OccupyingGroupId, text, StringComparison.Ordinal))
			{
				return false;
			}
			WorldBossCellDefenderSnapshot worldBossCellDefenderSnapshot = FindDefenderSnapshot(capturePoint, cell);
			if (worldBossCellDefenderSnapshot != null && string.Equals(worldBossCellDefenderSnapshot.OccupyingGroupId, worldBossCellStateSnapshot.OccupyingGroupId, StringComparison.Ordinal))
			{
				return !string.IsNullOrEmpty(worldBossCellDefenderSnapshot.DefenderInfo);
			}
			return false;
		}

		private List<SurvivorMockData> ResolveDefenderTeam(string capturePoint, string cell)
		{
			if (base.manager == null)
			{
				return null;
			}
			WorldBossCellDefenderSnapshot worldBossCellDefenderSnapshot = FindDefenderSnapshot(capturePoint, cell);
			if (string.IsNullOrEmpty(worldBossCellDefenderSnapshot?.DefenderInfo))
			{
				return null;
			}
			return base.manager.GetMessageSerializer().Deserialize<GuildBattleParticipantInfo>(worldBossCellDefenderSnapshot.DefenderInfo)?.SelectedSurvivors;
		}

		private WorldBossCellDefenderSnapshot FindDefenderSnapshot(string capturePoint, string cell)
		{
			List<WorldBossCapturePointSnapshot> list = WorldBossGuildFullSnapshot?.CapturePoints;
			if (list == null || string.IsNullOrEmpty(capturePoint) || string.IsNullOrEmpty(cell))
			{
				return null;
			}
			foreach (WorldBossCapturePointSnapshot item in list)
			{
				if (item?.CapturePoint != capturePoint || item.Defenders == null)
				{
					continue;
				}
				foreach (WorldBossCellDefenderSnapshot defender in item.Defenders)
				{
					if (defender != null && defender.Cell == cell)
					{
						return defender;
					}
				}
				break;
			}
			return null;
		}

		public GuildBattlePvpTeam GetCurrentDefenderTeam()
		{
			List<SurvivorMockData> list = AttackTarget?.DefenderTeam;
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			return new GuildBattlePvpTeam(list);
		}

		public void ClearAttackTarget()
		{
			base.manager.Player.ResetIAttackTargetMapMission();
			AttackTarget?.Clear();
		}

		public Dictionary<string, WorldBossCapturePointView> GetAllCapturePointStates()
		{
			Dictionary<string, WorldBossCapturePointView> dictionary = new Dictionary<string, WorldBossCapturePointView>();
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			GameEconomyData ged = Ged;
			if (worldBossGuildFullSnapshot == null || ged == null)
			{
				return dictionary;
			}
			string text = worldBossGuildFullSnapshot.GuildFullState?.GroupId;
			string text2 = worldBossGuildFullSnapshot.GuildFullState?.OpponentGroupId;
			int difficulty = ((worldBossGuildFullSnapshot.Match != null && worldBossGuildFullSnapshot.Match.BattleDifficulty > 0) ? worldBossGuildFullSnapshot.Match.BattleDifficulty : (worldBossGuildFullSnapshot.GuildFullState?.Difficulty ?? 0));
			WorldBossBattlegroundDefinition[] array = ged.FindWorldBossBattlegroundDefinitionsByDifficulty(difficulty);
			if (array == null || array.Length == 0)
			{
				return dictionary;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (WorldBossBattlegroundDefinition worldBossBattlegroundDefinition in array)
			{
				if (worldBossBattlegroundDefinition == null)
				{
					continue;
				}
				foreach (string item in SplitAfter(worldBossBattlegroundDefinition.After))
				{
					hashSet.Add(item);
				}
			}
			foreach (WorldBossBattlegroundDefinition worldBossBattlegroundDefinition2 in array)
			{
				if (worldBossBattlegroundDefinition2 == null || string.IsNullOrEmpty(worldBossBattlegroundDefinition2.CapturePoint))
				{
					continue;
				}
				bool flag = IsCapturePointUnlockedForGroup(worldBossBattlegroundDefinition2, array, hashSet, text);
				bool flag2 = !string.IsNullOrEmpty(text2) && IsCapturePointUnlockedForGroup(worldBossBattlegroundDefinition2, array, hashSet, text2);
				WorldBossCapturePointView worldBossCapturePointView = new WorldBossCapturePointView();
				if (IsPveType(worldBossBattlegroundDefinition2.CapturePointType))
				{
					WorldBossCapturePointSnapshot worldBossCapturePointSnapshot = FindPveShard(worldBossBattlegroundDefinition2.CapturePoint);
					worldBossCapturePointView.IsPve = true;
					worldBossCapturePointView.GroupId = worldBossCapturePointSnapshot?.GroupId;
					worldBossCapturePointView.TotalCells = ged.GetWorldBossCapturePointTotalCells(worldBossBattlegroundDefinition2.CapturePoint);
					worldBossCapturePointView.ClearedCells = CountClearedCells(worldBossCapturePointSnapshot);
					worldBossCapturePointView.IsInBattle = IsPveInBattle(worldBossCapturePointSnapshot);
					if (!flag && !flag2 && worldBossCapturePointSnapshot == null)
					{
						worldBossCapturePointView.State = WorldBossCapturePointState.Locked;
					}
					else if (worldBossCapturePointView.TotalCells > 0 && worldBossCapturePointView.ClearedCells >= worldBossCapturePointView.TotalCells)
					{
						worldBossCapturePointView.State = WorldBossCapturePointState.PveCleared;
					}
					else
					{
						worldBossCapturePointView.State = WorldBossCapturePointState.PveInProgress;
					}
				}
				else
				{
					worldBossCapturePointView.IsPve = false;
					WorldBossCapturePointStateModel worldBossCapturePointStateModel = FindPvpShard(worldBossBattlegroundDefinition2.CapturePoint)?.OwnershipState;
					bool flag3 = worldBossCapturePointStateModel != null && !string.IsNullOrEmpty(worldBossCapturePointStateModel.OwnerGroupId);
					worldBossCapturePointView.GroupId = (flag3 ? worldBossCapturePointStateModel.OwnerGroupId : null);
					if (flag3 && worldBossCapturePointStateModel.OwnerGroupId == text)
					{
						flag = true;
					}
					if (flag3 && !string.IsNullOrEmpty(text2) && worldBossCapturePointStateModel.OwnerGroupId == text2)
					{
						flag2 = true;
					}
					worldBossCapturePointView.MyUnlocked = flag;
					worldBossCapturePointView.OpponentUnlocked = flag2;
					if (flag3 && worldBossCapturePointStateModel.OwnerGroupId == text)
					{
						worldBossCapturePointView.State = WorldBossCapturePointState.PvpOccupiedByOwn;
						worldBossCapturePointView.ProtectionEndUtcMs = (worldBossCapturePointStateModel.IsProtected ? worldBossCapturePointStateModel.ProtectionEndUtcMs : 0);
					}
					else if (!flag)
					{
						worldBossCapturePointView.State = WorldBossCapturePointState.Locked;
					}
					else if (flag3)
					{
						worldBossCapturePointView.State = WorldBossCapturePointState.PvpOccupiedByEnemy;
						worldBossCapturePointView.ProtectionEndUtcMs = (worldBossCapturePointStateModel.IsProtected ? worldBossCapturePointStateModel.ProtectionEndUtcMs : 0);
					}
					else
					{
						worldBossCapturePointView.State = WorldBossCapturePointState.PvpUnoccupied;
					}
				}
				dictionary[worldBossBattlegroundDefinition2.CapturePoint] = worldBossCapturePointView;
			}
			return dictionary;
		}

		public WorldBossCellBarView GetCapturePointCellBar(string capturePoint)
		{
			WorldBossCellBarView worldBossCellBarView = new WorldBossCellBarView();
			if (Ged == null || string.IsNullOrEmpty(capturePoint))
			{
				return worldBossCellBarView;
			}
			WorldBossCapturePointSnapshot worldBossCapturePointSnapshot = FindPvpShard(capturePoint);
			if (worldBossCapturePointSnapshot == null)
			{
				return worldBossCellBarView;
			}
			worldBossCellBarView.TotalCells = Ged.GetWorldBossCapturePointTotalCells(capturePoint);
			string text = WorldBossGuildFullSnapshot?.GuildFullState?.GroupId;
			string text2 = WorldBossGuildFullSnapshot?.GuildFullState?.OpponentGroupId;
			if (worldBossCapturePointSnapshot.CellStates == null)
			{
				return worldBossCellBarView;
			}
			foreach (WorldBossCellStateSnapshot cellState in worldBossCapturePointSnapshot.CellStates)
			{
				if (cellState != null && cellState.Status == 2 && !string.IsNullOrEmpty(cellState.OccupyingGroupId))
				{
					if (!string.IsNullOrEmpty(text) && cellState.OccupyingGroupId == text)
					{
						worldBossCellBarView.MineOccupied++;
					}
					else if (!string.IsNullOrEmpty(text2) && cellState.OccupyingGroupId == text2)
					{
						worldBossCellBarView.EnemyOccupied++;
					}
				}
			}
			if (worldBossCellBarView.TotalCells > 0 && worldBossCellBarView.MineOccupied + worldBossCellBarView.EnemyOccupied > worldBossCellBarView.TotalCells)
			{
				if (worldBossCellBarView.MineOccupied > worldBossCellBarView.TotalCells)
				{
					worldBossCellBarView.MineOccupied = worldBossCellBarView.TotalCells;
				}
				worldBossCellBarView.EnemyOccupied = Math.Max(0, worldBossCellBarView.TotalCells - worldBossCellBarView.MineOccupied);
			}
			return worldBossCellBarView;
		}

		public string GetGuildNameByGroupId(string groupId)
		{
			if (string.IsNullOrEmpty(groupId))
			{
				return string.Empty;
			}
			WorldBossMatchSnapshot worldBossMatchSnapshot = WorldBossGuildFullSnapshot?.Match;
			if (worldBossMatchSnapshot == null)
			{
				return string.Empty;
			}
			if (groupId == worldBossMatchSnapshot.GroupIdA)
			{
				return worldBossMatchSnapshot.GroupNameA ?? string.Empty;
			}
			if (groupId == worldBossMatchSnapshot.GroupIdB)
			{
				return worldBossMatchSnapshot.GroupNameB ?? string.Empty;
			}
			return string.Empty;
		}

		public long GetCapturePointProtectionRemainingMs(string capturePoint)
		{
			WorldBossCapturePointStateModel worldBossCapturePointStateModel = FindPvpShard(capturePoint)?.OwnershipState;
			if (worldBossCapturePointStateModel == null || !worldBossCapturePointStateModel.IsProtected)
			{
				return 0L;
			}
			long num = worldBossCapturePointStateModel.ProtectionEndUtcMs - Now;
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		public long GetCapturePointOwnershipCountdownMs(string capturePoint)
		{
			WorldBossCapturePointStateModel worldBossCapturePointStateModel = FindPvpShard(capturePoint)?.OwnershipState;
			if (worldBossCapturePointStateModel == null || worldBossCapturePointStateModel.IsProtected)
			{
				return 0L;
			}
			if (worldBossCapturePointStateModel.ProtectionCountdownSinceUtcMs <= 0)
			{
				return 0L;
			}
			int valueOrDefault = (Ged?.WorldBossConfig?.BeforeProtection).GetValueOrDefault();
			if (valueOrDefault <= 0)
			{
				return 0L;
			}
			long num = worldBossCapturePointStateModel.ProtectionCountdownSinceUtcMs + (long)valueOrDefault * 1000L - Now;
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		public string GetCapturePointOwnershipCountdownGroupId(string capturePoint)
		{
			WorldBossCapturePointStateModel worldBossCapturePointStateModel = FindPvpShard(capturePoint)?.OwnershipState;
			if (worldBossCapturePointStateModel == null || worldBossCapturePointStateModel.IsProtected)
			{
				return string.Empty;
			}
			if (worldBossCapturePointStateModel.ProtectionCountdownSinceUtcMs <= 0)
			{
				return string.Empty;
			}
			return worldBossCapturePointStateModel.OwnerGroupId ?? string.Empty;
		}

		public WorldBossTowerATierView GetTowerAScoreTier(string capturePoint)
		{
			WorldBossTowerATierView worldBossTowerATierView = new WorldBossTowerATierView();
			WorldBossConfig worldBossConfig = Ged?.WorldBossConfig;
			if (worldBossConfig == null || capturePoint != "TOWER-A")
			{
				return worldBossTowerATierView;
			}
			WorldBossCapturePointStateModel worldBossCapturePointStateModel = FindPvpShard(capturePoint)?.OwnershipState;
			if (worldBossCapturePointStateModel == null || string.IsNullOrEmpty(worldBossCapturePointStateModel.OwnerGroupId))
			{
				return worldBossTowerATierView;
			}
			worldBossTowerATierView.CurrentScorePerMinute = ParsePerMinuteScore(ResolveTieredEffect(worldBossCapturePointStateModel, worldBossConfig.TowerA, worldBossConfig.TowerAEff));
			if (TryResolveNextTier(worldBossCapturePointStateModel, worldBossConfig.TowerA, worldBossConfig.TowerAEff, out var nextThresholdHours, out var nextEffect))
			{
				worldBossTowerATierView.HasNextTier = true;
				worldBossTowerATierView.NextThresholdHours = nextThresholdHours;
				worldBossTowerATierView.NextScorePerMinute = ParsePerMinuteScore(nextEffect);
			}
			return worldBossTowerATierView;
		}

		private static bool IsPveType(string capturePointType)
		{
			if (!string.IsNullOrEmpty(capturePointType))
			{
				return capturePointType.Equals("PVE", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private static IEnumerable<string> SplitAfter(string after)
		{
			if (string.IsNullOrEmpty(after))
			{
				yield break;
			}
			string[] parts = after.Split(';', ',');
			for (int i = 0; i < parts.Length; i++)
			{
				string text = ((parts[i] != null) ? parts[i].Trim() : null);
				if (!string.IsNullOrEmpty(text))
				{
					yield return text;
				}
			}
		}

		private static bool AfterContains(string after, string target)
		{
			foreach (string item in SplitAfter(after))
			{
				if (string.Equals(item, target, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsCapturePointUnlockedForGroup(WorldBossBattlegroundDefinition def, WorldBossBattlegroundDefinition[] groupDefs, HashSet<string> referenced, string groupId)
		{
			if (!referenced.Contains(def.CapturePoint))
			{
				return true;
			}
			if (string.IsNullOrEmpty(groupId))
			{
				return false;
			}
			foreach (WorldBossBattlegroundDefinition worldBossBattlegroundDefinition in groupDefs)
			{
				if (worldBossBattlegroundDefinition != null && worldBossBattlegroundDefinition != def && AfterContains(worldBossBattlegroundDefinition.After, def.CapturePoint) && IsClearedForUnlockByGroup(worldBossBattlegroundDefinition, groupId))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsCapturePointUnlockedForMyGroup(string capturePoint)
		{
			if (string.IsNullOrEmpty(capturePoint))
			{
				return true;
			}
			WorldBossGuildFullSnapshot worldBossGuildFullSnapshot = WorldBossGuildFullSnapshot;
			GameEconomyData ged = Ged;
			if (worldBossGuildFullSnapshot == null || ged == null)
			{
				return true;
			}
			WorldBossBattlegroundDefinition[] array = ged.FindWorldBossBattlegroundDefinitionsByDifficulty(GetCurrentBattleDifficulty());
			if (array == null || array.Length == 0)
			{
				return true;
			}
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = null;
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (WorldBossBattlegroundDefinition worldBossBattlegroundDefinition2 in array)
			{
				if (worldBossBattlegroundDefinition2 == null)
				{
					continue;
				}
				if (worldBossBattlegroundDefinition == null && string.Equals(worldBossBattlegroundDefinition2.CapturePoint, capturePoint, StringComparison.OrdinalIgnoreCase))
				{
					worldBossBattlegroundDefinition = worldBossBattlegroundDefinition2;
				}
				foreach (string item in SplitAfter(worldBossBattlegroundDefinition2.After))
				{
					hashSet.Add(item);
				}
			}
			if (worldBossBattlegroundDefinition == null || IsPveType(worldBossBattlegroundDefinition.CapturePointType))
			{
				return true;
			}
			return IsCapturePointUnlockedForGroup(worldBossBattlegroundDefinition, array, hashSet, worldBossGuildFullSnapshot.GuildFullState?.GroupId);
		}

		private bool IsClearedForUnlockByGroup(WorldBossBattlegroundDefinition def, string groupId)
		{
			if (!IsPveType(def.CapturePointType))
			{
				return false;
			}
			int num = Ged?.GetWorldBossCapturePointTotalCells(def.CapturePoint) ?? 0;
			if (num > 0)
			{
				return CountClearedCells(FindPveShardForGroup(def.CapturePoint, groupId)) >= num;
			}
			return false;
		}

		private WorldBossCapturePointSnapshot FindPveShard(string capturePoint)
		{
			List<WorldBossCapturePointSnapshot> list = WorldBossGuildFullSnapshot?.CapturePoints;
			if (list == null)
			{
				return null;
			}
			foreach (WorldBossCapturePointSnapshot item in list)
			{
				if (item != null && !(item.CapturePoint != capturePoint) && IsPveType(item.CapturePointType))
				{
					return item;
				}
			}
			return null;
		}

		private WorldBossCapturePointSnapshot FindPvpShard(string capturePoint)
		{
			List<WorldBossCapturePointSnapshot> list = WorldBossGuildFullSnapshot?.CapturePoints;
			if (list == null)
			{
				return null;
			}
			foreach (WorldBossCapturePointSnapshot item in list)
			{
				if (item != null && !(item.CapturePoint != capturePoint) && !IsPveType(item.CapturePointType))
				{
					return item;
				}
			}
			return null;
		}

		private WorldBossCapturePointSnapshot FindPveShardForGroup(string capturePoint, string groupId)
		{
			List<WorldBossCapturePointSnapshot> list = WorldBossGuildFullSnapshot?.CapturePoints;
			if (list == null || string.IsNullOrEmpty(groupId))
			{
				return null;
			}
			foreach (WorldBossCapturePointSnapshot item in list)
			{
				if (item != null && !(item.CapturePoint != capturePoint) && IsPveType(item.CapturePointType) && item.GroupId == groupId)
				{
					return item;
				}
			}
			return null;
		}

		private static int CountClearedCells(WorldBossCapturePointSnapshot shard)
		{
			if (shard?.CellStates == null)
			{
				return 0;
			}
			int num = 0;
			foreach (WorldBossCellStateSnapshot cellState in shard.CellStates)
			{
				if (cellState != null && (cellState.PveCleared || cellState.Status == 2))
				{
					num++;
				}
			}
			return num;
		}

		private static bool IsPveInBattle(WorldBossCapturePointSnapshot shard)
		{
			if (shard?.CellStates == null || shard.CellStates.Count == 0)
			{
				return false;
			}
			foreach (WorldBossCellStateSnapshot cellState in shard.CellStates)
			{
				if (cellState != null && cellState.Status == 1)
				{
					return true;
				}
			}
			return false;
		}

		public WorldBossCellEnterAction GetCellEnterAction(string capturePoint, string cell)
		{
			if (string.IsNullOrEmpty(capturePoint) || string.IsNullOrEmpty(cell))
			{
				return WorldBossCellEnterAction.FightPve;
			}
			WorldBossCellStateSnapshot worldBossCellStateSnapshot = FindCellSnapshot(capturePoint, cell);
			int num = worldBossCellStateSnapshot?.Status ?? 0;
			if (num == 1)
			{
				return WorldBossCellEnterAction.Blocked;
			}
			bool flag = (Ged?.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, GetCurrentBattleDifficulty()))?.IsPVECapturePointType() ?? false;
			string text = WorldBossGuildFullSnapshot?.GuildFullState?.GroupId;
			if (!flag && !IsCapturePointUnlockedForMyGroup(capturePoint))
			{
				return WorldBossCellEnterAction.Blocked;
			}
			if (num == 2)
			{
				if (!flag && !string.IsNullOrEmpty(worldBossCellStateSnapshot?.OccupyingGroupId) && !string.IsNullOrEmpty(text) && worldBossCellStateSnapshot.OccupyingGroupId != text)
				{
					return WorldBossCellEnterAction.FightPvpDefender;
				}
				return WorldBossCellEnterAction.Blocked;
			}
			bool flag2 = worldBossCellStateSnapshot?.PveCleared ?? false;
			bool flag3 = worldBossCellStateSnapshot != null && worldBossCellStateSnapshot.RemainingDurability <= 0 && worldBossCellStateSnapshot.BattleCount > 0;
			if (flag)
			{
				if (!(flag2 || flag3))
				{
					return WorldBossCellEnterAction.FightPve;
				}
				return WorldBossCellEnterAction.Blocked;
			}
			int num2;
			if (!flag)
			{
				GameEconomyData ged = Ged;
				num2 = ((ged != null && ged.FindWorldBossCellDefinition(capturePoint, cell)?.HaveBattle == false) ? 1 : 0);
			}
			else
			{
				num2 = 0;
			}
			if (((uint)num2 | (flag2 ? 1u : 0u) | (flag3 ? 1u : 0u)) == 0)
			{
				return WorldBossCellEnterAction.FightPve;
			}
			return WorldBossCellEnterAction.DirectOccupy;
		}

		public WorldBossGuildBuffStateModel GetMyGuildBuffState()
		{
			WorldBossGuildBuffStateModel worldBossGuildBuffStateModel = new WorldBossGuildBuffStateModel
			{
				GroupId = WorldBossGuildFullSnapshot?.GuildFullState?.GroupId,
				UpdatedUtcMs = Now
			};
			WorldBossConfig worldBossConfig = Ged?.WorldBossConfig;
			string groupId = worldBossGuildBuffStateModel.GroupId;
			if (worldBossConfig == null || string.IsNullOrEmpty(groupId))
			{
				return worldBossGuildBuffStateModel;
			}
			if (IsCapturePointOwnedByMe("TOWER-A", groupId, out var ownership))
			{
				worldBossGuildBuffStateModel.OwnedTowerA = true;
				worldBossGuildBuffStateModel.TowerAEffect = ResolveTieredEffect(ownership, worldBossConfig.TowerA, worldBossConfig.TowerAEff);
			}
			if (IsCapturePointOwnedByMe("TOWER-B", groupId, out var ownership2))
			{
				worldBossGuildBuffStateModel.OwnedTowerB = true;
				worldBossGuildBuffStateModel.TowerBEffect = ResolveTieredEffect(ownership2, worldBossConfig.TowerB, worldBossConfig.TowerBEff);
			}
			if (IsCapturePointOwnedByMe("DEPOT", groupId, out var ownership3))
			{
				worldBossGuildBuffStateModel.OwnedDepot = true;
				worldBossGuildBuffStateModel.DepotEffect = ResolveTieredEffect(ownership3, worldBossConfig.Depot, worldBossConfig.DepotEff);
				worldBossGuildBuffStateModel.DepotBossBattleTimeEffect = ((worldBossConfig.DepotEffBossBattleTime > 0) ? worldBossConfig.DepotEffBossBattleTime : 0);
			}
			return worldBossGuildBuffStateModel;
		}

		public int GetMyActiveBuffCount()
		{
			int num = 0;
			foreach (WorldBossBuildingBuffView myBuildingBuff in GetMyBuildingBuffs())
			{
				if (myBuildingBuff.IsActive)
				{
					num++;
				}
			}
			return num;
		}

		public List<WorldBossBuildingBuffView> GetMyBuildingBuffs()
		{
			List<WorldBossBuildingBuffView> list = new List<WorldBossBuildingBuffView>();
			WorldBossConfig worldBossConfig = Ged?.WorldBossConfig;
			if (worldBossConfig == null)
			{
				return list;
			}
			list.Add(BuildMyBuildingBuffView("TOWER-A", worldBossConfig.TowerA, worldBossConfig.TowerAEff, 0L));
			list.Add(BuildMyBuildingBuffView("TOWER-B", worldBossConfig.TowerB, worldBossConfig.TowerBEff, 0L));
			int num = worldBossConfig.DepotEffBossBattleTime;
			if (num < 0)
			{
				num = 0;
			}
			list.Add(BuildMyBuildingBuffView("DEPOT", worldBossConfig.Depot, worldBossConfig.DepotEff, num));
			return list;
		}

		public long GetMyTowerAScorePerMinute()
		{
			WorldBossGuildBuffStateModel myGuildBuffState = GetMyGuildBuffState();
			if (!myGuildBuffState.OwnedTowerA)
			{
				return 0L;
			}
			return ParsePerMinuteScore(myGuildBuffState.TowerAEffect);
		}

		public double GetMyTowerBBossScoreMultiplier()
		{
			WorldBossGuildBuffStateModel myGuildBuffState = GetMyGuildBuffState();
			if (!myGuildBuffState.OwnedTowerB)
			{
				return 1.0;
			}
			if (!double.TryParse(myGuildBuffState.TowerBEffect, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !(result > 0.0))
			{
				return 1.0;
			}
			return result;
		}

		public double GetMyDepotFatigueRecoverySpeedupPercent()
		{
			WorldBossGuildBuffStateModel myGuildBuffState = GetMyGuildBuffState();
			if (!myGuildBuffState.OwnedDepot)
			{
				return 0.0;
			}
			if (!double.TryParse(myGuildBuffState.DepotEffect, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !(result > 0.0))
			{
				return 0.0;
			}
			return result;
		}

		public long GetMyDepotExtraBossBattleTimes()
		{
			WorldBossGuildBuffStateModel myGuildBuffState = GetMyGuildBuffState();
			if (!myGuildBuffState.OwnedDepot)
			{
				return 0L;
			}
			return (myGuildBuffState.DepotBossBattleTimeEffect > 0) ? myGuildBuffState.DepotBossBattleTimeEffect : 0;
		}

		public long GetDailyBossBattleLimit()
		{
			long num = (Ged?.WorldBossConfig?.DailyBossBattleTime).GetValueOrDefault();
			if (num < 0)
			{
				num = 0L;
			}
			return num + GetMyDepotExtraBossBattleTimes();
		}

		public long GetUsedBossBattleTimes()
		{
			PlayerModel playerModel = base.manager?.Player;
			int dailyRefreshUtcHour = Ged?.WorldBossConfig?.DailyRefresh ?? (-1);
			if (playerModel == null || !TryGetDailyRefreshStartUtcMs(playerModel.UtcTimeStamp, dailyRefreshUtcHour, out var refreshStartUtcMs) || playerModel.WorldBossDailyBattleRefreshUtcMs != refreshStartUtcMs)
			{
				return 0L;
			}
			return (playerModel.WorldBossDailyBattleCount > 0) ? playerModel.WorldBossDailyBattleCount : 0;
		}

		public long GetBossBattleBaseScore()
		{
			long valueOrDefault = (Ged?.WorldBossConfig?.PlayerScoreBossBattle).GetValueOrDefault();
			if (valueOrDefault <= 0)
			{
				return 0L;
			}
			return valueOrDefault;
		}

		public int GetCurrentBattleDifficulty()
		{
			if (WorldBossGuildFullSnapshot?.Match == null || WorldBossGuildFullSnapshot.Match.BattleDifficulty <= 0)
			{
				return (WorldBossGuildFullSnapshot?.GuildFullState?.Difficulty).GetValueOrDefault();
			}
			return WorldBossGuildFullSnapshot.Match.BattleDifficulty;
		}

		public ActorDefinition GetBossActorDefinition(string capturePoint)
		{
			if (Ged == null || string.IsNullOrEmpty(capturePoint))
			{
				return null;
			}
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = Ged.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, GetCurrentBattleDifficulty());
			if (worldBossBattlegroundDefinition == null || string.IsNullOrEmpty(worldBossBattlegroundDefinition.EnemyActorId))
			{
				return null;
			}
			return Ged.GetActorDefinition(worldBossBattlegroundDefinition.EnemyActorId);
		}

		public int GetBossDefense(string capturePoint)
		{
			if (Ged == null || string.IsNullOrEmpty(capturePoint))
			{
				return 0;
			}
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = Ged.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, GetCurrentBattleDifficulty());
			if (worldBossBattlegroundDefinition == null || string.IsNullOrEmpty(worldBossBattlegroundDefinition.EnemyActorId))
			{
				return 0;
			}
			int enemyLevel = worldBossBattlegroundDefinition.EnemyLevel;
			return Ged.GetActorLevelDefinition(worldBossBattlegroundDefinition.EnemyActorId, enemyLevel)?.GuildBossDefense ?? 0;
		}

		public long GetRemainingBossBattleTimes()
		{
			long num = GetDailyBossBattleLimit() - GetUsedBossBattleTimes();
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		private static long ParsePerMinuteScore(string effect)
		{
			if (!double.TryParse(effect, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !(result > 0.0))
			{
				return 0L;
			}
			return (long)Math.Floor(result);
		}

		private bool IsCapturePointOwnedByMe(string capturePoint, string myGroupId, out WorldBossCapturePointStateModel ownership)
		{
			ownership = FindPvpShard(capturePoint)?.OwnershipState;
			if (ownership != null && !string.IsNullOrEmpty(ownership.OwnerGroupId))
			{
				return ownership.OwnerGroupId == myGroupId;
			}
			return false;
		}

		private WorldBossBuildingBuffView BuildMyBuildingBuffView(string capturePoint, string thresholdsConfig, string effectsConfig, long extraBossBattleTimes = 0L)
		{
			string myGroupId = WorldBossGuildFullSnapshot?.GuildFullState?.GroupId;
			WorldBossCapturePointStateModel ownership;
			bool flag = IsCapturePointOwnedByMe(capturePoint, myGroupId, out ownership);
			WorldBossBuildingBuffView worldBossBuildingBuffView = new WorldBossBuildingBuffView
			{
				CapturePoint = capturePoint,
				IsOccupiedByMe = flag,
				ExtraBossBattleTimes = extraBossBattleTimes
			};
			double currentThresholdHours = 0.0;
			string currentEffect = "0";
			bool flag2 = flag && TryResolveCurrentTier(ownership, thresholdsConfig, effectsConfig, out currentThresholdHours, out currentEffect);
			if (flag2)
			{
				worldBossBuildingBuffView.CurrentThresholdHours = currentThresholdHours;
				worldBossBuildingBuffView.CurrentValue = currentEffect;
			}
			double nextThresholdHours;
			string nextEffect;
			bool flag3 = (flag2 ? TryResolveNextTier(ownership, thresholdsConfig, effectsConfig, out nextThresholdHours, out nextEffect) : TryResolveFirstTier(thresholdsConfig, effectsConfig, out nextThresholdHours, out nextEffect));
			if (flag3)
			{
				worldBossBuildingBuffView.NextThresholdHours = nextThresholdHours;
				worldBossBuildingBuffView.NextValue = nextEffect;
			}
			worldBossBuildingBuffView.IsActive = flag2 || (flag && capturePoint == "DEPOT" && extraBossBattleTimes > 0);
			worldBossBuildingBuffView.IsMaxTier = worldBossBuildingBuffView.IsActive && !flag3;
			return worldBossBuildingBuffView;
		}

		private bool TryResolveCurrentTier(WorldBossCapturePointStateModel ownership, string thresholdsConfig, string effectsConfig, out double currentThresholdHours, out string currentEffect)
		{
			currentThresholdHours = 0.0;
			currentEffect = "0";
			if (ownership == null || ownership.MajoritySinceUtcMs <= 0)
			{
				return false;
			}
			double num = (double)Math.Max(0L, Now - ownership.MajoritySinceUtcMs) / 3600000.0;
			List<string> list = SplitSemicolon(thresholdsConfig);
			List<string> list2 = SplitSemicolon(effectsConfig);
			int num2 = Math.Min(list.Count, list2.Count);
			bool result = false;
			double num3 = double.NegativeInfinity;
			for (int i = 0; i < num2; i++)
			{
				if (double.TryParse(list[i], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2) && num >= result2 && result2 > num3)
				{
					num3 = result2;
					currentThresholdHours = result2;
					currentEffect = (string.IsNullOrEmpty(list2[i]) ? "0" : list2[i]);
					result = true;
				}
			}
			return result;
		}

		private static bool TryResolveFirstTier(string thresholdsConfig, string effectsConfig, out double firstThresholdHours, out string firstEffect)
		{
			firstThresholdHours = 0.0;
			firstEffect = "0";
			List<string> list = SplitSemicolon(thresholdsConfig);
			List<string> list2 = SplitSemicolon(effectsConfig);
			int num = Math.Min(list.Count, list2.Count);
			bool result = false;
			double num2 = double.PositiveInfinity;
			for (int i = 0; i < num; i++)
			{
				if (double.TryParse(list[i], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2) && result2 < num2)
				{
					num2 = result2;
					firstThresholdHours = result2;
					firstEffect = (string.IsNullOrEmpty(list2[i]) ? "0" : list2[i]);
					result = true;
				}
			}
			return result;
		}

		private string ResolveTieredEffect(WorldBossCapturePointStateModel ownership, string thresholdsConfig, string effectsConfig)
		{
			if (ownership == null)
			{
				return "0";
			}
			long majoritySinceUtcMs = ownership.MajoritySinceUtcMs;
			if (majoritySinceUtcMs <= 0)
			{
				return "0";
			}
			long num = Now - majoritySinceUtcMs;
			if (num <= 0)
			{
				return "0";
			}
			double num2 = (double)num / 3600000.0;
			List<string> list = SplitSemicolon(thresholdsConfig);
			List<string> list2 = SplitSemicolon(effectsConfig);
			int num3 = Math.Min(list.Count, list2.Count);
			string result = "0";
			double num4 = double.NegativeInfinity;
			for (int i = 0; i < num3; i++)
			{
				if (double.TryParse(list[i], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2) && num2 >= result2 && result2 > num4)
				{
					num4 = result2;
					result = (string.IsNullOrEmpty(list2[i]) ? "0" : list2[i]);
				}
			}
			return result;
		}

		private bool TryResolveNextTier(WorldBossCapturePointStateModel ownership, string thresholdsConfig, string effectsConfig, out double nextThresholdHours, out string nextEffect)
		{
			nextThresholdHours = 0.0;
			nextEffect = "0";
			if (ownership == null)
			{
				return false;
			}
			long majoritySinceUtcMs = ownership.MajoritySinceUtcMs;
			double num = ((majoritySinceUtcMs > 0) ? ((double)Math.Max(0L, Now - majoritySinceUtcMs) / 3600000.0) : 0.0);
			List<string> list = SplitSemicolon(thresholdsConfig);
			List<string> list2 = SplitSemicolon(effectsConfig);
			int num2 = Math.Min(list.Count, list2.Count);
			bool result = false;
			double num3 = double.PositiveInfinity;
			for (int i = 0; i < num2; i++)
			{
				if (double.TryParse(list[i], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2) && result2 > num && result2 < num3)
				{
					num3 = result2;
					nextThresholdHours = result2;
					nextEffect = (string.IsNullOrEmpty(list2[i]) ? "0" : list2[i]);
					result = true;
				}
			}
			return result;
		}

		private static List<string> SplitSemicolon(string config)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(config))
			{
				return list;
			}
			string[] array = config.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				list.Add((array[i] != null) ? array[i].Trim() : string.Empty);
			}
			return list;
		}

		public override void Initialize()
		{
			base.Initialize();
			ParticipatedCycleHistory = ParticipatedCycleHistory ?? new List<WorldBossSeasonCycleRecord>();
			ShownSettlementHistory = ShownSettlementHistory ?? new List<WorldBossSeasonCycleRecord>();
			if (AttackTarget == null)
			{
				AttackTarget = new WorldBossAttackTargetData();
			}
			AttackTarget.MissionModel?.Restore(Ged);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
