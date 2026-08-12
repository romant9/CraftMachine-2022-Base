using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;

public class GuildWarHelper
{
	public const string Vp_Currency_Icon_Name = "Ui_Icon_Resource_Vp";

	public static long GetTimeLeftToNextWar()
	{
		if (GetGuildWarModel() == null)
		{
			return 0L;
		}
		return GetGuildWarModel().FindNextGuildWar(GameManager.Instance.playerModel.UtcTimeStamp)?.TimeUntilStartMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp) ?? 0;
	}

	public static string GetFormatedTimeLeftToNextWar(bool isGuildMember = true)
	{
		if (isGuildMember)
		{
			return Helpers.FormatTime(GetTimeLeftToNextWar());
		}
		return Helpers.FormatTime(GetTimeLeftToNextWarForNonGuildMember());
	}

	public static string GetFormatedTimeLeftToNextSeason()
	{
		return Helpers.FormatTime(GetTimeLeftToNextSeason());
	}

	public static long GetTimeLeftToNextWarForNonGuildMember()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		if (gameEconomyData.FindGuildWarWithTime(GameManager.Instance.playerModel.UtcTimeStamp) != null)
		{
			return 0L;
		}
		return gameEconomyData.FindNextGuildWar(GameManager.Instance.playerModel.UtcTimeStamp, 0L)?.TimeUntilStartMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp) ?? (-1);
	}

	public static long GetTimeLeftToCurrentWarEnd()
	{
		if (GetGuildWarModel() == null)
		{
			return 0L;
		}
		return GetGuildWarModel().CurrentWarDefinition?.TimeUntilEndMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp) ?? 0;
	}

	public static long GetTimeLeftToCurrentWarEndForNoGuildMember()
	{
		return GameManager.Instance.gameEconomyData.FindGuildWarWithTime(GameManager.Instance.playerModel.UtcTimeStamp)?.TimeUntilEndMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp) ?? 0;
	}

	public static string GetFormatedTimeLeftToCurrentWarEnd(bool isGuildMember = true)
	{
		if (isGuildMember)
		{
			return Helpers.FormatTime(GetTimeLeftToCurrentWarEnd());
		}
		return Helpers.FormatTime(GetTimeLeftToCurrentWarEndForNoGuildMember());
	}

	public static long GetTimeLeftToNextSeason()
	{
		GvGSeasonModel gvGSeasonModel = GetGvGSeasonModel();
		if (gvGSeasonModel == null)
		{
			return 0L;
		}
		return gvGSeasonModel.FindNextSeason(GameManager.Instance.playerModel.UtcTimeStamp)?.TimeUntilStartMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp) ?? 0;
	}

	public static long GetTimeLeftToCurrentSeasonEnd()
	{
		if (GetGvGSeasonModel() == null)
		{
			return 0L;
		}
		return GetGvGSeasonModel().CurrentSeasonDefinition?.TimeUntilEndMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp) ?? 0;
	}

	public static string GetFormatedTimeLeftToCurrentSeasonEnd()
	{
		return Helpers.FormatTime(GetTimeLeftToCurrentSeasonEnd());
	}

	public static long GetTimeLeftToCurrentBattleEnd()
	{
		if (GetCurrentBattle() == null)
		{
			return 0L;
		}
		return GetCurrentBattle().EndBattleTimestamp - GameManager.Instance.playerModel.UtcTimeStamp;
	}

	public static bool IsLastMinuteForBattleEnd()
	{
		return GetTimeLeftToCurrentBattleEnd() / 1000 < 60;
	}

	public static bool IsLastMinuteBeforeBattleStart()
	{
		return GetTimeLeftToNextAvailableBattleStart() / 1000 < 60;
	}

	public static bool IsLockdownTimeForCurrentBattle()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel == null)
		{
			return false;
		}
		return IsLockDownTimeForTimeSlotClientSide(guildWarModel.GetBattleSlotForTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp));
	}

	public static bool IsLockDownTimeForTimeSlotClientSide(long timeslot)
	{
		return GetGuildWarModel()?.IsBattleSlotLocked(timeslot, GameManager.Instance.playerModel.UtcTimeStamp + GameManager.Instance.gameEconomyData.GuildWarConfig.MatchmakingLockdownBufferClientSide) ?? false;
	}

	public static long GetTimeLeftToNextAvailableBattleStart()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			return guildWarModel.GetBattleSlotForTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp) - GameManager.Instance.playerModel.UtcTimeStamp;
		}
		return 0L;
	}

	public static bool CheckIfNextBattleExists()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
			foreach (KeyValuePair<long, List<string>> item in guildWarModel.RegisteredPlayersForBattleSlot)
			{
				if (item.Key > utcTimeStamp)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool HasMatchmakingEntryForTimeSlot(long battleSlot)
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel == null)
		{
			return false;
		}
		foreach (GuildBattleOpponentMatchmakingEntry item in guildWarModel.NextBattlesOpponentMatchmakingInfo)
		{
			if (item.StartBattleTimeSlot == battleSlot)
			{
				return true;
			}
		}
		return false;
	}

	public static GuildBattleOpponentMatchmakingEntry GetNextBattleMatchmakingEntry()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel == null)
		{
			Debug.LogError("Guild war model is null");
		}
		return guildWarModel.GetNextGuildBattleOpponentMatchmakingEntry();
	}

	public static long GetTimeToBattleLockdown(long battleTimeSlot)
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			return guildWarModel.GetLockDownTimeForBattleSlot(battleTimeSlot) - GameManager.Instance.playerModel.UtcTimeStamp;
		}
		return 0L;
	}

	public static long GetTimeLeftToBattle(long battleTimeSlot)
	{
		return battleTimeSlot - GameManager.Instance.playerModel.UtcTimeStamp;
	}

	public static long GetTimeToBattleLockdownClient(long battleTimeSlot)
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			return guildWarModel.GetLockDownTimeForBattleSlot(battleTimeSlot) - GameManager.Instance.gameEconomyData.GuildWarConfig.MatchmakingLockdownBufferClientSide - GameManager.Instance.playerModel.UtcTimeStamp;
		}
		return 0L;
	}

	public static string GetFormatedTimeLeftToNextAvailableBattleStart(bool roundLastMinute = true, bool lastMinuteWarning = true, string customWarningLocalization = null)
	{
		if (roundLastMinute && IsLastMinuteBeforeBattleStart())
		{
			if (lastMinuteWarning)
			{
				if (!string.IsNullOrEmpty(customWarningLocalization))
				{
					return LocalizationManager.GetText(customWarningLocalization);
				}
				return LocalizationManager.GetText("GvG.BattleStarting");
			}
			return "<1 " + LocalizationManager.GetText("Generic.Time.MinuteSmall");
		}
		return Helpers.FormatTimeWithDoubleDigits(GetTimeLeftToNextAvailableBattleStart());
	}

	public static string SetFormatedTime(long timeSlot)
	{
		long num = 60000L;
		if (timeSlot < num)
		{
			return "<1 " + LocalizationManager.GetText("Generic.Time.MinuteSmall");
		}
		return Helpers.FormatTimeWithDoubleDigits(timeSlot);
	}

	public static string GetFormatedTimeLeftToCurrentBattleEnd(bool roundLastMinute = true, bool lastMinuteWarning = true, string customWarningLocalization = null)
	{
		if (roundLastMinute && IsLastMinuteForBattleEnd())
		{
			if (lastMinuteWarning)
			{
				if (!string.IsNullOrEmpty(customWarningLocalization))
				{
					return LocalizationManager.GetText(customWarningLocalization);
				}
				return LocalizationManager.GetText("GvG.BattleEnding");
			}
			return "<1 " + LocalizationManager.GetText("Generic.Time.MinuteSmall");
		}
		return Helpers.FormatTimeWithDoubleDigits(GetTimeLeftToCurrentBattleEnd());
	}

	public static bool IsGuildMember()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			return GameManager.Instance.playerModel.IsGuildMember;
		}
		return false;
	}

	public static bool IsOwnGuild(string groupId)
	{
		if (IsGuildMember())
		{
			return GameManager.Instance.playerModel.GuildId == groupId;
		}
		return false;
	}

	public static GuildWarModel GetGuildWarModel()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			if (IsGuildMember())
			{
				return GameManager.Instance.playerModel.GuildModel.GuildWarModel;
			}
			return null;
		}
		else
		{
			GuildModel guildModel = OfflineManager.Instance.CurrentGuildModel;
			return guildModel?.GuildWarModel ?? null;
		}
	}

	public static GvGSeasonModel GetGvGSeasonModel()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			if (IsGuildMember())
			{
				return GameManager.Instance.playerModel.GuildModel.GvGSeasonModel;
			}
			return null;
		}
		else
		{
			GuildModel guildModel = OfflineManager.Instance.CurrentGuildModel;
			return guildModel?.GvGSeasonModel ?? null;
		}
	}

	public static GuildWarModelPlayer GetGuildWarPlayer()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			return GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer;
		}
		return null;
	}

	public static GvGSeasonModelPlayer GetGvGSeasonModelPlayer()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			return GameManager.Instance.playerModel.GvGSeasonModelPlayer;
		}
		return null;
	}

	public static GuildBattleModel GetCurrentBattle()
	{
		return GetGuildWarModel()?.CurrentBattle;
	}

	public static int GetCurrentWarDefinitionId()
	{
		return GetGuildWarModel()?.WarDefinitionId ?? (-1);
	}

	public static int GetCurrentSeasonDefinitionId()
	{
		return GetGvGSeasonModel()?.SeasonDefinitionId ?? (-1);
	}

	public static GuildBattleMapModel GetCurrentMapModel()
	{
		return GetCurrentBattle()?.CurrentMapModel;
	}

	public static List<string> GetActiveBonusesList()
	{
		GuildBattleModel currentBattle = GetCurrentBattle();
		if (currentBattle != null)
		{
			List<string> list = new List<string>();
			{
				foreach (KeyValuePair<int, List<string>> item in currentBattle.CollectedBattleBonusesPerSector)
				{
					list.AddRange(item.Value);
				}
				return list;
			}
		}
		return null;
	}

	public static bool IsSeasonOngoing()
	{
		GvGSeasonModel gvGSeasonModel = GetGvGSeasonModel();
		long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
		return gvGSeasonModel?.IsCurrentSeasonOpen(utcTimeStamp) ?? false;
	}

	public static int GetWarDayIndexByTimeslot(long timeSlot)
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			int num = 0;
			foreach (KeyValuePair<long, List<string>> item in guildWarModel.RegisteredPlayersForBattleSlot)
			{
				if (item.Key == timeSlot)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public static long GetNextOrCurrentGuildBattleTimeSlot(long utcTimeStamp)
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		long guildBattleDurationMilliseconds = GameManager.Instance.gameEconomyData.GuildWarConfig.GuildBattleDurationMilliseconds;
		long num = long.MaxValue;
		if (guildWarModel != null)
		{
			foreach (KeyValuePair<long, List<string>> item in guildWarModel.RegisteredPlayersForBattleSlot)
			{
				long num2 = item.Key + guildBattleDurationMilliseconds;
				if (num2 > utcTimeStamp && num2 < num)
				{
					num = item.Key;
				}
			}
		}
		return num;
	}

	public static bool IsWarOngoing()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			GuildWarModel guildWarModel = GetGuildWarModel();
			if (IsGuildMember() && IsSeasonOngoing())
			{
				return guildWarModel.IsCurrentWarOpen(GameManager.Instance.playerModel.UtcTimeStamp);
			}
			return false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsWarOngoingForPlayer()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			if (IsWarOngoing())
			{
				return GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.HasWarStarted();
			}
			return false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsBattleOnGoing()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			return GetGuildWarModel()?.CurrentBattle.IsOngoing(GameManager.Instance.playerModel.UtcTimeStamp) ?? false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsBattleOnGoingForPlayer()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			return GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsOngoingForPlayer();
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsWarAndBattleOngoing()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			GuildWarModel guildWarModel = GetGuildWarModel();
			if (IsSeasonOngoing() && guildWarModel != null)
			{
				return guildWarModel.IsWarAndBattleOngoing(GameManager.Instance.playerModel.UtcTimeStamp);
			}
			return false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsWarAndBattleOngoingAndPlayerRegistered()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			GuildWarModel guildWarModel = GetGuildWarModel();
			if (IsSeasonOngoing() && guildWarModel != null)
			{
				if (guildWarModel.IsWarAndBattleOngoing(GameManager.Instance.playerModel.UtcTimeStamp))
				{
					return IsPlayerRegisteredForBattle();
				}
				return false;
			}
			return false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsBattleOngoingAndPlayerRegistered()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			GuildWarModel guildWarModel = GetGuildWarModel();
			if (IsSeasonOngoing() && guildWarModel != null)
			{
				if (guildWarModel.CurrentBattle.IsOngoing(GameManager.Instance.playerModel.UtcTimeStamp))
				{
					return IsPlayerRegisteredForBattle();
				}
				return false;
			}
			return false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsLockedByCouncilLevelOrTutorial()
	{
		if (!IsLockedByCouncilLevel())
		{
			return IsLockedByTutorial();
		}
		return true;
	}

	public static bool IsLockedByTutorial()
	{
		if (!string.IsNullOrEmpty(GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarUnlockAtAfterTutorialPartId))
		{
			return !GameManager.Instance.playerModel.Tutorial.HasCompletedPart(GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarUnlockAtAfterTutorialPartId);
		}
		return false;
	}

	public static bool IsLockedByCouncilLevel()
	{
		return GameManager.Instance.playerModel.Camp.GetCouncilLevel() < GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarUnlockAtCouncilLevel;
	}

	public static int GetRegisteredPlayersCountForBattleTimeSlot()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp);
			return guildWarModel.GetRegisteredPlayersCountForBattle(battleSlotForTimeStamp);
		}
		return 0;
	}

	public static List<string> GetRegisteredPlayersForBattleTimeSlot()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp);
			return guildWarModel.GetAllRegisteredPlayersForBattleSlot(battleSlotForTimeStamp);
		}
		return null;
	}

	public static bool IsPlayerRegisteredForBattle()
	{
		return IsPlayerRegisteredForBattle(GameManager.Instance.playerModel.HashedId);
	}

	public static bool IsPlayerRegisteredForBattle(long timeslot)
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			GuildWarModel guildWarModel = GetGuildWarModel();
			if (guildWarModel.RegisteredPlayersForBattleSlot.ContainsKey(timeslot))
			{
				return guildWarModel.RegisteredPlayersForBattleSlot[timeslot].Contains(GameManager.Instance.playerModel.HashedId);
			}
			return false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsPlayerRegisteredForBattle(string hashedId)
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			GuildWarModel guildWarModel = GetGuildWarModel();
			if (guildWarModel != null)
			{
				long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp);
				return guildWarModel.IsPlayerRegisteredForBattle(battleSlotForTimeStamp, hashedId);
			}
			return false;
		}
		else
		{
			return HelpersModel.IsUnlockPVP;
		}
	}

	public static bool IsCurrentOrNextBattleFull()
	{
		return GetRegisteredPlayersCountForBattleTimeSlot() >= GameManager.Instance.gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
	}

	public static bool IsGuildBattleMapMission()
	{
		GuildWarModelPlayer guildWarPlayer = GetGuildWarPlayer();
		if (guildWarPlayer != null)
		{
			return guildWarPlayer.GuildBattleModel.AttackTargetMissionModel != null;
		}
		return false;
	}

	public static bool CanPlayerRegisterForBattle()
	{
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null && GameManager.Instance.gameEconomyData.GuildWarConfig.MatchmakingVersion >= guildModel.MatchmakingVersion)
		{
			GuildWarModel guildWarModel = guildModel.GvGSeasonModel.GuildWarModel;
			PlayerModel playerModel = GameManager.Instance.playerModel;
			long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp);
			if (!IsLimitRegisted() && !HasJoinedDuringBattle())
			{
				return guildWarModel.CanPlayerRegisterForBattleSlot(battleSlotForTimeStamp, playerModel.HashedId, playerModel.UtcTimeStamp);
			}
			return false;
		}
		return false;
	}

	public static bool IsBattleReadyToEnd()
	{
		if (!IsGuildMember())
		{
			return false;
		}
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel == null)
		{
			return false;
		}
		GuildBattleModel currentBattle = guildWarModel.CurrentBattle;
		if (currentBattle.HasStarted())
		{
			return currentBattle.IsBiggerThanEndBattleTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp);
		}
		return false;
	}

	public static bool HasBattleEnded()
	{
		if (!IsGuildMember())
		{
			return false;
		}
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel == null)
		{
			return false;
		}
		GuildBattleModel currentBattle = guildWarModel.CurrentBattle;
		bool flag = GameManager.Instance.playerModel.UtcTimeStamp >= currentBattle.EndBattleTimestamp;
		if (IsBattleReadyToEnd())
		{
			if (!flag)
			{
				return currentBattle.IsFakeBattle;
			}
			return true;
		}
		return false;
	}

	public static bool CanShowBattleEnd()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool flag = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsOngoingForPlayer();
		bool flag2 = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentGuildBattle();
		bool flag3 = IsBattleReadyToEnd();
		bool flag4 = HasCurrentBattleEnded();
		bool flag5 = false;
		if ((!flag) ? (!playerModel.Blackboard.IsToggleOn("HasSeenGuildBattleEnd")) : flag2)
		{
			return HelpersModel.IsUnlockAllSectors ? false : flag3 || flag4;
		}
		return false;
	}

	public static bool HasCurrentBattleEnded()
	{
		return GetCurrentBattle()?.HasEnded() ?? false;
	}

	public static void SaveSectorProgressionSeen(GuildBattleMapSectorModel sectorModel)
	{
		if (sectorModel != null && GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot != null && !GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot.IsSectorSeen(sectorModel))
		{
			Helpers.ExecuteCommand(new GuildBattleMarkProggressSeenCommand(sectorModel.SectorId));
		}
	}

	public static List<GvGSeasonModel.GuildBattleLogEntry> GetBattleLogForWar(int warId)
	{
		List<GvGSeasonModel.GuildBattleLogEntry> value = new List<GvGSeasonModel.GuildBattleLogEntry>();
		GvGSeasonModel gvGSeasonModel = GetGvGSeasonModel();
		if (gvGSeasonModel != null && gvGSeasonModel.SeasonDefinitionId != -1)
		{
			gvGSeasonModel.BattleLog.TryGetValue(warId, out value);
		}
		return value;
	}

	public static int GetNumberOfWarsForActiveSeason()
	{
		if (!IsSeasonOngoing())
		{
			return 0;
		}
		GvGSeasonDefinition currentSeasonDefinition = GetGvGSeasonModel().CurrentSeasonDefinition;
		return GameManager.Instance.playerModel.gameEconomyData.FindGuildWarDefinitionInSeason(currentSeasonDefinition.Identifier)?.Count ?? 0;
	}

	public static int GetActiveWarWeek()
	{
		if (!IsSeasonOngoing())
		{
			return 0;
		}
		int num = 1;
		GvGSeasonDefinition currentSeasonDefinition = GetGvGSeasonModel().CurrentSeasonDefinition;
		List<GuildWarDefinition> list = GameManager.Instance.playerModel.gameEconomyData.FindGuildWarDefinitionInSeason(currentSeasonDefinition.Identifier);
		long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
		int num2 = list?.Count ?? 0;
		for (int i = 0; i < num2 && list[i].EndTimeMilliseconds < utcTimeStamp; i++)
		{
			if (num >= num2)
			{
				break;
			}
			num++;
		}
		return num;
	}

	public static bool ShowWarIsOnOnMissionHub()
	{
		if (IsGuildMember() || !IsLockedByCouncilLevelOrTutorial())
		{
			return GetTimeLeftToNextWarForNonGuildMember() == 0;
		}
		return false;
	}

	public static void CheckGuildWarFlowPopups()
	{
		if (GameManager.Instance == null || !GuildWarManager.IsGuildAvailable || !GuildWarManager.IsInCamp || TutorialView.Instance.Running || GameManager.Instance.CurrentlyLoading)
		{
			return;
		}
		if (OfflineManager.IsLoadDataManager && DataManager.Instance.guildPopup.gameObject.activeSelf)
		{
			DebugTWD.Log("CheckGuildWarFlowPopups. Проверить!", DebugType.Wars);
			return;
		}
		Queue<UIType> queue = new Queue<UIType>();
		if (CheckEndBattleOnlyEndRewardsPopup())
		{
			ShowEndBattleOnlyRewards();
		}
		else if (!SingularityMonoBehaviour<HUDManager>.Instance.HasFullScreenPopup())
		{
			if (CheckEndBattleFullPopup())
			{
				queue.Enqueue(UIType.GuildBattleEndPopup);
			}
			if (CheckStartWarPopup())
			{
				queue.Enqueue(UIType.StartWarBannerPopup);
			}
			if (CheckEndSeasonPopup())
			{
				queue.Enqueue(UIType.GuildBattleEndSeasonPopup);
			}
			if (CheckEndSeasonRewardsOnly())
			{
				queue.Enqueue(UIType.GuildBattleEndSeasonRewardsOnlyPopup);
			}
			if (CheckStartSeasonPopup())
			{
				queue.Enqueue(UIType.GuildBattleOverviewPopup);
			}
			if (CheckStartBattleNotificationPopup())
			{
				queue.Enqueue(UIType.GuildBattleStartNotificationPopup);
			}
			if (queue.Count > 0)
			{
				UIType uiType = queue.Dequeue();
				SingularityMonoBehaviour<HUDManager>.Instance.Get(uiType).Open();
			}
		}
	}

	private static bool CheckStartSeasonPopup()
	{
		bool result = false;
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleOverviewPopup) && !HasReachedMaxPopupLimit())
		{
			if (GuildBattleOverviewPopup.CanShowSeasonPopup())
			{
				result = true;
			}
			else if ((IsWarOngoing() || GetTimeLeftToNextWar() <= 0) && !HasSeenGvGSeasonStart())
			{
				SetHasSeenGvGSeasonStartFlagAndGiveSeasonStartRewards();
			}
		}
		return result;
	}

	private static bool CheckStartWarPopup()
	{
		bool result = false;
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.StartWarBannerPopup) && !HasReachedMaxPopupLimit() && StartWarBannerPopup.CanShow())
		{
			result = true;
		}
		return result;
	}

	private static bool CheckEndSeasonPopup()
	{
		bool result = false;
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleEndSeasonPopup) && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleEndSeasonRewardsOnlyPopup) && !HasReachedMaxPopupLimit() && GuildBattleEndSeasonPopup.CanShow())
		{
			result = true;
		}
		return result;
	}

	private static bool CheckEndSeasonRewardsOnly()
	{
		bool result = false;
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleEndSeasonRewardsOnlyPopup) && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleEndSeasonPopup) && !HasReachedMaxPopupLimit() && GuildBattleEndSeasonRewardsOnlyPopup.CanShow())
		{
			result = true;
		}
		return result;
	}

	public static bool CheckEndBattleOnlyEndRewardsPopup()
	{
		bool result = false;
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.IAPConfirmPopupNew) && GuildBattleEndPopup.CanShowOnlyRewardsPopup())
		{
			result = true;
		}
		return result;
	}

	public static bool CheckEndBattleFullPopup()
	{
		bool result = false;
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleEndPopup) && GuildBattleEndPopup.CanShowFullPopup())
		{
			result = true;
		}
		return result;
	}

	private static bool CheckStartBattleNotificationPopup()
	{
		bool result = false;
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleStartNotificationPopup) && !HasReachedMaxPopupLimit() && GuildBattleStartNotificationPopup.CanShow())
		{
			result = true;
		}
		return result;
	}

	private static bool HasReachedMaxPopupLimit()
	{
		int num = 0;
		num = (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.StartWarBannerPopup) ? (num + 1) : num);
		num = (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleStartNotificationPopup) ? (num + 1) : num);
		return num > 0;
	}

	public static void ShowEndBattleOnlyRewards()
	{
		List<RewardCurrency> claimableBattleRewardsClientSide = GetClaimableBattleRewardsClientSide();
		if (claimableBattleRewardsClientSide.Count > 0)
		{
			IAPConfirmPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj.OpenForCurrencyList(claimableBattleRewardsClientSide);
			obj.OnClose += OnCloseRewardsOnlyPopupEndBattle;
		}
		else
		{
			SingularityMonoBehaviour<GuildWarManager>.Instance.ResolveEndBattle();
		}
	}

	private static void OnCloseRewardsOnlyPopupEndBattle(HUDElement element, HUDElementConfig config)
	{
		if (element.UIType == UIType.IAPConfirmPopupNew)
		{
			element.OnClose -= OnCloseRewardsOnlyPopupEndBattle;
			SingularityMonoBehaviour<GuildWarManager>.Instance.ResolveEndBattle();
		}
	}

	public static List<RewardCurrency> GetClaimableBattleRewardsClientSide()
	{
		GuildBattleModelPlayer guildBattleModel = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel;
		List<RewardCurrency> claimableBattleRewards = guildBattleModel.GetClaimableBattleRewards();
		bool flag = (GetGvGSeasonModel()?.GetBattleLogEntry(guildBattleModel.CurrentBattleWarId, guildBattleModel.CurrentBattleId))?.IsVictory ?? false;
		if (claimableBattleRewards.Count > 0 && guildBattleModel.GetBattleBonusRewardPointsAmount() > 0)
		{
			int num = guildBattleModel.PersonalRewardPoints;
			for (int i = 0; i < claimableBattleRewards.Count - 1; i++)
			{
				if (claimableBattleRewards[i].CurrencyType == CurrencyType.GuildBattleRP)
				{
					num += claimableBattleRewards[i].Amount;
				}
			}
			if (flag)
			{
				claimableBattleRewards[claimableBattleRewards.Count - 1].Amount = (int)((float)num * guildBattleModel.VictoryRewardPointsMultiplier);
			}
			else
			{
				claimableBattleRewards[claimableBattleRewards.Count - 1].Amount = (int)((float)num * guildBattleModel.DrawRewardPointsMultiplier);
			}
		}
		return claimableBattleRewards;
	}

	public static string GetCurrentOpponentGuildName()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel == null || guildWarModel.CurrentBattle == null)
		{
			return "";
		}
		return guildWarModel.CurrentBattle.EnemyGuildName;
	}

	public static void ShowNotAvailableAlertPopup()
	{
		if (!IsPlayerRegisteredForBattle())
		{
			AlertPopup.ShowPopup(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.NotPartOfBattle.Title"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.NotPartOfBattle.Message"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Button.Ok"));
		}
		else if (!IsBattleOnGoing())
		{
			AlertPopup.ShowPopup(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.BattleNotActive.Title"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.BattleNotActive.Message"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Button.Ok"));
		}
		else
		{
			AlertPopup.ShowPopup(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.BattleEnded.Title"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.BattleEnded.Message"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Button.Ok"));
		}
	}

	public static void ShowNotEnoughPlayersRegisteredForTheBattle()
	{
		AlertPopup.ShowPopup(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.MatchmakingFailed.Title"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Alert.MatchmakingFailed.Message"), SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Button.Ok"));
	}

	public static void SendHasSeenGuildBattleStartFlagCommand()
	{
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenGuildBattleStart"))
		{
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand("HasSeenGuildBattleStart"));
		}
	}

	public static bool HasSeenGvGSeasonStart()
	{
		return GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenSeasonStart");
	}

	public static void SetHasSeenGvGSeasonStartFlagAndGiveSeasonStartRewards()
	{
		if (!IsLockedByCouncilLevel())
		{
			Helpers.ExecuteCommandDelayed(new SetBlackboardToggleCommand("HasSeenSeasonStart"));
		}
	}

	public static void SetHasSeenGvGSeasonEndFlag()
	{
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenSeasonEnd"))
		{
			Helpers.ExecuteCommandDelayed(new SetBlackboardToggleCommand("HasSeenSeasonEnd"));
		}
	}

	public static bool HasSeenGvGSeasonEnd()
	{
		return GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenSeasonEnd");
	}

	public static bool HasJoinedDuringBattle()
	{
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (guildModel == null)
		{
			return false;
		}
		GuildBattleModel currentBattle = guildModel.GuildWarModel.CurrentBattle;
		if (guildModel.GetMemberInfo(GameManager.Instance.playerModel.HashedId) != null)
		{
			long guildJoinedDate = guildModel.GetMemberInfo(GameManager.Instance.playerModel.HashedId).GuildJoinedDate;
			if (currentBattle.HasStarted())
			{
				return currentBattle.TimeSlot < guildJoinedDate;
			}
			return false;
		}
		return false;
	}

	public static bool CanPlayerJoinWar(string playerId)
	{
		if (!IsPlayerRegisted(playerId))
		{
			return !IsLimitRegisted();
		}
		return true;
	}

	public static bool IsPlayerRegisted(string playerId)
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			if (guildWarModel.WarParticipants.Contains(playerId))
			{
				return true;
			}
			long battleSlotForTimeStamp = guildWarModel.GetBattleSlotForTimeStamp(GameManager.Instance.playerModel.UtcTimeStamp);
			return guildWarModel.GetWarAndRegisteredParticipants(battleSlotForTimeStamp).Contains(playerId);
		}
		return true;
	}

	public static bool IsLimitRegisted()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		if (guildWarModel != null)
		{
			return guildWarModel.GetWarAndRegisteredCount(GameManager.Instance.playerModel.UtcTimeStamp) >= GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarRegistrationLimit;
		}
		return false;
	}

	public static bool CheckForGuildShopResetWarning()
	{
		GvGSeasonModelPlayer gvGSeasonModelPlayer = GetGvGSeasonModelPlayer();
		if (gvGSeasonModelPlayer != null && gvGSeasonModelPlayer.IsCurrentSeasonEnded())
		{
			return GameManager.Instance.playerModel.GuildShopModel.HasAnyAffordableItem();
		}
		return false;
	}

	public static bool NeedsBattleHighscoresUpdate()
	{
		GuildWarModel guildWarModel = GetGuildWarModel();
		_ = GetGuildWarPlayer().GuildBattleModel;
		if (IsWarOngoing() && guildWarModel.CurrentBattle.TimeSlot > 0 && !GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenGuildBattleEnd"))
		{
			return true;
		}
		return false;
	}

	public static bool IsNextWarDuringCurrentSeason()
	{
		GuildWarDefinition guildWarDefinition = GameManager.Instance.gameEconomyData.FindNextGuildWar(GameManager.Instance.playerModel.UtcTimeStamp, 0L);
		GvGSeasonModelPlayer gvGSeasonModelPlayer = GetGvGSeasonModelPlayer();
		GvGSeasonDefinition gvGSeasonDefinition = GameManager.Instance.gameEconomyData.FindGvGSeasonDefinition(gvGSeasonModelPlayer.StartedGvGSeasonId);
		if (guildWarDefinition != null && gvGSeasonDefinition != null)
		{
			return guildWarDefinition.StartTimeMilliseconds > gvGSeasonDefinition.EndTimeMilliseconds;
		}
		return true;
	}

	public static List<long> FindBattleSlotsRemovings()
	{
		GvGSeasonModelPlayer gvGSeasonModelPlayer = GetGvGSeasonModelPlayer();
		GuildWarModel guildWarModel = GetGuildWarModel();
		List<long> list = new List<long>();
		if (gvGSeasonModelPlayer != null)
		{
			for (int i = 0; i < gvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots.Count; i++)
			{
				long num = gvGSeasonModelPlayer.GuildWarModelPlayer.RegisteredBattleSlots[i];
				if (!guildWarModel.RegisteredPlayersForBattleSlot.ContainsKey(num))
				{
					continue;
				}
				bool flag = false;
				List<string> list2 = guildWarModel.RegisteredPlayersForBattleSlot[num];
				if (list2 == null)
				{
					continue;
				}
				for (int j = 0; j < list2.Count; j++)
				{
					if (list2[j] != null && GameManager.Instance.playerModel.HashedId == list2[j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(num);
				}
			}
		}
		return list;
	}

	public static bool HasUnseenBattleSlotRemoves()
	{
		if (FindBattleSlotsRemovings().Count > 0)
		{
			return IsGuildMember();
		}
		return false;
	}

	public static bool LeaderboardsAreUpToDate()
	{
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		GvGSeasonModel gvGSeasonModel = GetGvGSeasonModel();
		GuildWarModel guildWarModel = GetGuildWarModel();
		GuildBattleModel guildBattleModel = guildWarModel?.CurrentBattle;
		if ((guildModel == null || guildModel.LeaderboardUpdated) && (gvGSeasonModel == null || gvGSeasonModel.LeaderboardUpdated) && (guildWarModel == null || guildWarModel.LeaderboardUpdated))
		{
			return guildBattleModel?.LeaderboardUpdated ?? true;
		}
		return false;
	}
}
