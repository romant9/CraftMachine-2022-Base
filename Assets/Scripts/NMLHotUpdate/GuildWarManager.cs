using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class GuildWarManager : SingularityMonoBehaviour<GuildWarManager>
{
	public class CommandRequestStatus
	{
		public readonly long SendTimestamp;

		public readonly Type CommandType;

		public CommandRequestStatus(Type type)
		{
			if (GameManager.Instance != null && GameManager.Instance.playerModel != null && !HelpersModel.IsOfflineMode)
			{
				SendTimestamp = GameManager.Instance.playerModel.UtcTimeStamp;
			}
			CommandType = type;
		}
	}

	public CommandRequestStatus GroupCommandRequestStatus;

	private int groupCommandRequestRetryCount;

	private string lastGroupCommand;

	private const int maxRetryCount = 5;

	private int refreshRate = 1;

	private float refreshTimer;

	private const int waitTimeoutMilliseconds = 30000;

	private bool lockdownTimeEventSendFlag;

	private long NextHighscoresRequestTimestamp;

	private const long HighscoresChangedNotificationDebounceMilliseconds = 5000L;

	private long lastHighscoresChangedNotificationTimestamp;

	private string lastHighscoresChangedNotificationBattleId;

	private GuildBattleVisualConfig guildBattleVisualConfig;

	public static bool IsConnectedToServer
	{
		get
		{
			if (SignalRClient.Instance != null)
			{
				if (!SignalRClient.Instance.IsConnected)
				{
					return !GameConfiguration.Instance.Config.ConnectedToServer;
				}
				return true;
			}
			return false;
		}
	}

	public static bool IsGuildAvailable
	{
		get
		{
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
				return GWTeamUtils.Instance.GuildModel != null && DataManager.Instance.Player.IsGuildMember;
			}
			if (GameManager.Instance != null && GameManager.Instance.GuildManager != null)
			{
				bool num = GameManager.Instance.IsSocialEnabled();
				bool flag = !GameManager.Instance.GuildManager.GuildOffline && !GameManager.Instance.GuildManager.IsLoading;
				bool flag2 = GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.GuildModel != null;
				return num && flag && flag2;
			}
			return false;
		}
	}

	public static bool AreGuildsLoaded
	{
		get
		{
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
				return GWTeamUtils.Instance.GuildModel != null;
			}
			if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
			{
				if (GameManager.Instance.playerModel.IsGuildMember)
				{
					if (GameManager.Instance.GuildManager != null)
					{
						bool num = GameManager.Instance.IsSocialEnabled();
						bool flag = !GameManager.Instance.GuildManager.GuildOffline && !GameManager.Instance.GuildManager.IsLoading;
						return num && flag;
					}
					return false;
				}
				return true;
			}
			return true;
		}
	}

	public static bool AreGuildsReady
	{
		get
		{
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
				return GWTeamUtils.Instance.GuildModel != null;
			}
			if (GameManager.Instance.GuildManager != null)
			{
				return !GameManager.Instance.GuildManager.IsBusy;
			}
			return false;
		}
	}

	public static bool IsInCamp => !CampManager.IsInstanceNull;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public GuildBattleVisualConfig GuildBattleVisualConfig
	{
		get
		{
			if (guildBattleVisualConfig == null)
			{
				guildBattleVisualConfig = UnityUtils.LoadFromAssetBundle<GuildBattleVisualConfig>("GuildBattleVisualConfig", "scriptableobjects");
			}
			return guildBattleVisualConfig;
		}
	}

	private int playerSpecificDelay => GvGModelHelper.NotificationDelayInMilliseconds(playerModel.HashedId, 60);

	protected override void AwakeInternal()
	{
		base.AwakeInternal();
		lockdownTimeEventSendFlag = false;
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			DebugTWD.Log("GuildWarManager. AwakeInternal return");
			return;
		}
		GameManager.Instance.OnLoadCompleted += OnLoadCompleted;
		SubscribeToEvents();
	}

	private void OnDestroy()
	{
		if (!(GameManager.Instance != null))
		{
			return;
		}
		if (IsLoadDataManager && !IsGuildSubscribed)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && !IsGuildSubscribed) return");
			return;
		}
		GameManager.Instance.OnLoadCompleted -= OnLoadCompleted;
		if (GameManager.Instance.GuildManager != null)
		{
			GameManager.Instance.GuildManager.OnLoadGroupCompleted -= OnLoadGroupCompleted;
			GameManager.Instance.GuildManager.OnSocialMessageReceived -= OnSocialMessageReceived;
			GameManager.Instance.GuildManager.OnGroupCommandReceived -= OnGroupCommandReceived;
		}
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnGuildBattleHighScoresMessage -= OnGuildBattleHighScoresMessageReceived;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnGuildWarChange;
			if (guildWarModel.CurrentBattle != null)
			{
				guildWarModel.CurrentBattle.Changed -= OnGuildBattleChange;
			}
		}
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed -= OnGuildWarPlayerChange;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
	}

	public void OnLoadCompleted()
	{
		DebugTWD.Log("GuildWarManager. OnLoadCompleted", DebugType.Guild);
		SubscribeToEvents();
		UpdateGuildWarForPlayer();
		SetupAttackTargetMissionData();
	}

	public void SubscribeToEvents()
	{
		IsGuildSubscribed = false;
		IsPlayerSubscribed = false;

		if (!(GameManager.Instance != null))
		{
			return;
		}
		if (GameManager.Instance.GuildManager != null)
		{
			GameManager.Instance.GuildManager.OnLoadGroupCompleted -= OnLoadGroupCompleted;
			GameManager.Instance.GuildManager.OnLoadGroupCompleted += OnLoadGroupCompleted;
			GameManager.Instance.GuildManager.OnSocialMessageReceived -= OnSocialMessageReceived;
			GameManager.Instance.GuildManager.OnSocialMessageReceived += OnSocialMessageReceived;
			GameManager.Instance.GuildManager.OnGroupCommandReceived -= OnGroupCommandReceived;
			GameManager.Instance.GuildManager.OnGroupCommandReceived += OnGroupCommandReceived;
		}
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnGuildBattleHighScoresMessage -= OnGuildBattleHighScoresMessageReceived;
			SignalRClient.Instance.OnGuildBattleHighScoresMessage += OnGuildBattleHighScoresMessageReceived;
		}
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.GvGSeasonModelPlayer != null)
		{
			GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed -= OnGuildWarPlayerChange;
			GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed += OnGuildWarPlayerChange;
			GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
			GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed += OnGuildBattlePlayerChange;
			IsPlayerSubscribed = true;
		}
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.manager == null || GameManager.Instance.playerModel.GuildModel == null)
		{
			return;
		}
		GuildWarModel guildWarModel = GameManager.Instance.playerModel.GuildModel.GuildWarModel;
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnGuildWarChange;
			guildWarModel.Changed += OnGuildWarChange;
			if (guildWarModel.CurrentBattle != null)
			{
				guildWarModel.CurrentBattle.Changed -= OnGuildBattleChange;
				guildWarModel.CurrentBattle.Changed += OnGuildBattleChange;
			}
			IsGuildSubscribed = true;
		}
	}

	private void OnLoadGroupCompleted(bool success)
	{
		if (!success)
		{
			return;
		}
		SubscribeToEvents();
		UpdateGuildWarForPlayer();
		SetupAttackTargetMissionData();
		CheckIfGuildHasSavedInMatchmaking();
		if (IsGuildAvailable)
		{
			if (GuildWarHelper.NeedsBattleHighscoresUpdate())
			{
				RequestBattleHighscoresUpdate(forceBroadcast: true);
			}
			EventManager.NotifyEvent(EventManager.EventType.GroupModelLoaded);
			GroupCommandRequestStatus = null;
			groupCommandRequestRetryCount = 0;
			lastGroupCommand = string.Empty;
		}
	}

	private void OnGuildBattleHighScoresMessageReceived(string message, string type)
	{
		if (!(type != "GuildBattleHighScoresChanged") && !string.IsNullOrEmpty(message) && GameManager.Instance?.playerModel?.manager != null)
		{
			GuildBattleHighscoresChangedNotification guildBattleHighscoresChangedNotification;
			try
			{
				guildBattleHighscoresChangedNotification = GameManager.Instance.playerModel.manager.GetMessageSerializer().DeserializeObject<GuildBattleHighscoresChangedNotification>(message);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Failed to parse guild battle highscore notification: " + ex.Message);
				return;
			}
			if (ShouldRefreshHighscoresFromNotification(guildBattleHighscoresChangedNotification))
			{
				lastHighscoresChangedNotificationBattleId = guildBattleHighscoresChangedNotification.BattleId;
				lastHighscoresChangedNotificationTimestamp = playerModel.UtcTimeStamp;
				Helpers.ExecuteCommandDelayed(new RefreshGuildBattleHighscoresCommand(forceBroadcast: true));
			}
		}
	}

	private bool ShouldRefreshHighscoresFromNotification(GuildBattleHighscoresChangedNotification notification)
	{
		if (notification == null || !IsGuildAvailable || playerModel == null || playerModel.GuildModel == null)
		{
			return false;
		}
		GuildBattleModel guildBattleModel = GuildWarHelper.GetGuildWarModel()?.CurrentBattle;
		if (guildBattleModel == null || guildBattleModel.EnemyGuildData == null)
		{
			return false;
		}
		if (notification.TargetGuildId != playerModel.GuildId || notification.SourceGuildId != guildBattleModel.EnemyGuildData.GroupId || notification.BattleId != guildBattleModel.BattleId || notification.WarId != guildBattleModel.WarId)
		{
			return false;
		}
		if (lastHighscoresChangedNotificationBattleId == notification.BattleId && playerModel.UtcTimeStamp - lastHighscoresChangedNotificationTimestamp < 5000)
		{
			return false;
		}
		return true;
	}

	private void OnGroupCommandReceived(GroupCommandBase type, string memberInfo)
	{
		if (GroupCommandRequestStatus != null)
		{
			bool flag = GroupCommandRequestStatus.CommandType == typeof(RegisterForGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(ResignFromGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(RemovePlayerFromGuildBattleGroupCommand);
			if (type.GetType() == GroupCommandRequestStatus.CommandType && (!flag || memberInfo == playerModel.HashedId))
			{
				GroupCommandRequestStatus = null;
				groupCommandRequestRetryCount = 0;
				lastGroupCommand = string.Empty;
			}
		}
	}

	private void OnSocialMessageReceived(GuildManager.SocialMessageType type, string memberInfo)
	{
		switch (type)
		{
		case GuildManager.SocialMessageType.Guild:
			if (!string.IsNullOrEmpty(memberInfo))
			{
				UpdateGuildBattleSnapshot(memberInfo);
			}
			UpdateGuildWarForPlayer();
			break;
		case GuildManager.SocialMessageType.GuildWar:
			UpdateGuildWarForPlayer();
			break;
		}
	}

	private void OnGuildWarChange(TWDGroupModelChild modelObject, string changed, object args)
	{
		switch (changed)
		{
		case "GuildBattleEnded":
			GuildWarHelper.CheckGuildWarFlowPopups();
			break;
		case "GuildBattleStarted":
			GuildWarHelper.CheckGuildWarFlowPopups();
			break;
		case "GuildBattlePlayerResigned":
			if (args as string == playerModel.HashedId && GuildWarHelper.HasUnseenBattleSlotRemoves())
			{
				playerModel.manager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildRemovedFromBattleSlot, 0, LocalizationManager.GetText("GvG.Alert.RemovedFromBattle.Title"), 0);
			}
			break;
		}
	}

	private void OnGuildBattlePlayerChange(ModelObject model, string changed, object args)
	{
		if (changed == "GuildBattleStarted")
		{
			GuildWarHelper.CheckGuildWarFlowPopups();
		}
	}

	private void OnGuildWarPlayerChange(ModelObject model, string changed, object args)
	{
		if (changed == "GuildWarStarted")
		{
			Helpers.ExecuteCommandDelayed(new RestockGuildShopCommand(onNewTier: false, onNewWar: true));
		}
	}

	private void OnGuildBattleChange(TWDGroupModelChild model, string changed, object args)
	{
		if (changed == "GuildBattleScoresUpdated")
		{
			UpdateNextHighscoresRequestTimestamp();
		}
	}

	private void UpdateClient()
	{
		if (string.IsNullOrEmpty(GameManager.Instance.playerModel.GuildId) && GuildWarHelper.CheckEndBattleOnlyEndRewardsPopup())
		{
			GuildWarHelper.ShowEndBattleOnlyRewards();
		}
		else
		{
			if (!playerModel.IsGuildMember)
			{
				return;
			}
			if (GuildWarHelper.IsWarOngoing())
			{
				if (GuildWarHelper.IsBattleOnGoing())
				{
					lockdownTimeEventSendFlag = false;
					if (GuildWarHelper.IsPlayerRegisteredForBattle() && (!GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsOngoingForPlayer() || !GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentGuildBattle()))
					{
						UpdateGuildWarForPlayer();
					}
				}
				else if (!lockdownTimeEventSendFlag && GuildWarHelper.IsLockdownTimeForCurrentBattle())
				{
					lockdownTimeEventSendFlag = true;
					EventManager.NotifyEvent(EventManager.EventType.GuildBattleLockdownTimeEvent);
				}
				if (!GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.HasWarStarted())
				{
					UpdateGuildWarForPlayer();
				}
			}
			if (GuildWarHelper.IsSeasonOngoing() && !GameManager.Instance.playerModel.GvGSeasonModelPlayer.HasGvGSeasonStarted())
			{
				UpdateGuildWarForPlayer();
			}
			if (!IsLoadDataManager) GuildWarHelper.CheckGuildWarFlowPopups();
		}
	}

	private void UpdateGroupModel()
	{
		if ((GroupCommandRequestStatus != null && GroupCommandRequestPending()) || !playerModel.IsGuildMember)
		{
			return;
		}
		if (GuildWarHelper.HasBattleEnded())
		{
			SendBattleEndCommand();
			return;
		}
		if (!GuildWarHelper.LeaderboardsAreUpToDate())
		{
			long num = (playerModel.GuildWarModel.CurrentBattle.LeaderboardUpdated ? UtilsDateTime.HourInMilliseconds : (UtilsDateTime.HourInMilliseconds / 2));
			if (playerModel.GuildModel.LastGvGLeaderboardUpdateTime + playerSpecificDelay + num < playerModel.UtcTimeStamp && Helpers.ExecuteCommand(new UpdateGvGLeaderboardsCommand()) == TWDModelResult.OK)
			{
				GroupCommandRequestStatus = new CommandRequestStatus(typeof(UpdateLeaderboardSaveStateGroupCommand));
				return;
			}
		}
		if (GuildWarHelper.IsWarOngoing())
		{
			GuildBattleOpponentMatchmakingEntry nextBattleMatchmakingEntry = GuildWarHelper.GetNextBattleMatchmakingEntry();
			if (nextBattleMatchmakingEntry != null && !GuildWarHelper.GetCurrentBattle().IsOngoing(nextBattleMatchmakingEntry.StartBattleTimeSlot) && nextBattleMatchmakingEntry.StartBattleTimeSlot <= GameManager.Instance.playerModel.UtcTimeStamp)
			{
				if (!GuildWarHelper.GetGuildWarModel().HasEnoughRegisteredPlayersToStartBattleForTimeSlot(nextBattleMatchmakingEntry.StartBattleTimeSlot))
				{
					return;
				}
				Helpers.ExecuteCommandDelayed(new StartGuildBattleCommand
				{
					Timeslot = nextBattleMatchmakingEntry.StartBattleTimeSlot,
					WarDefinitionId = GuildWarHelper.GetGuildWarModel().WarDefinitionId
				}, delegate(bool success)
				{
					if (success)
					{
						GroupCommandRequestStatus = new CommandRequestStatus(typeof(StartGvgBattleGroupCommand));
					}
				});
				return;
			}
			if (GameManager.Instance.gameEconomyData.GetFeature("GvGClientSideOpponentRequest").Enabled)
			{
				if (GuildWarHelper.IsBattleOnGoing() || GroupCommandRequestStatus?.CommandType == typeof(SetNextBattleMatchmakingInfoGroupCommand))
				{
					return;
				}
				GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
				GvGSeasonModel gvGSeasonModel = GuildWarHelper.GetGvGSeasonModel();
				long nextBattleTimeslot = GuildWarHelper.GetNextOrCurrentGuildBattleTimeSlot(playerModel.UtcTimeStamp);
				long lockDownTimeForBattleSlot = guildWarModel.GetLockDownTimeForBattleSlot(nextBattleTimeslot);
				if (nextBattleTimeslot < long.MaxValue && playerModel.UtcTimeStamp > lockDownTimeForBattleSlot)
				{
					if (guildWarModel.NextBattlesOpponentMatchmakingInfo.Exists((GuildBattleOpponentMatchmakingEntry m) => m.StartBattleTimeSlot == nextBattleTimeslot) || (gvGSeasonModel.BattleLog.ContainsKey(guildWarModel.WarDefinitionId) && gvGSeasonModel.BattleLog[guildWarModel.WarDefinitionId].Exists((GvGSeasonModel.GuildBattleLogEntry b) => b.EndedTimeStamp > nextBattleTimeslot)) || !guildWarModel.HasEnoughRegisteredPlayersToStartBattleForTimeSlot(nextBattleTimeslot) || playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.LastOpponentRequestTime + UtilsDateTime.HourInMilliseconds > playerModel.UtcTimeStamp)
					{
						return;
					}
					long num2 = playerModel.UtcTimeStamp - lockDownTimeForBattleSlot;
					int num3 = (int)((float)GameManager.Instance.gameEconomyData.GuildWarConfig.MatchmakingLockdownDurationInMilliseconds * 0.2f);
					if (num2 > num3)
					{
						if (HelpersModel.IsOfflineMode)
						{
							Helpers.ExecuteCommand(new RequestMatchmakingEntryCommand(nextBattleTimeslot));
							GroupCommandRequestStatus = new CommandRequestStatus(typeof(SetNextBattleMatchmakingInfoGroupCommand));
							return;
						}
						GroupCommandRequestStatus = new CommandRequestStatus(typeof(SetNextBattleMatchmakingInfoGroupCommand));
						SignalRClient.Instance.RequestCommand("SyncGroup", GameManager.Instance.guildModel.Id, delegate
						{
							if (SignalRClient.Instance.HasError)
							{
								SignalRClient.Instance.ClearError();
								GroupCommandRequestStatus = null;
							}
							else if (Helpers.ExecuteCommand(new RequestMatchmakingEntryCommand(nextBattleTimeslot)) == TWDModelResult.OK)
							{
								GroupCommandRequestStatus = new CommandRequestStatus(typeof(SetNextBattleMatchmakingInfoGroupCommand));
							}
						}, waitForResponse: true);
					}
				}
			}
		}
		if (!GuildWarHelper.IsSeasonOngoing() && GuildWarHelper.GetGvGSeasonModel() != null)
		{
			GvGSeasonDefinition gvGSeasonDefinition = GuildWarHelper.GetGvGSeasonModel().FindNextSeason(GameManager.Instance.playerModel.UtcTimeStamp);
			if (gvGSeasonDefinition != null && gvGSeasonDefinition.IsOpen(GameManager.Instance.playerModel.UtcTimeStamp))
			{
				StartSeason();
			}
		}
		else if (GuildWarHelper.IsSeasonOngoing() && !GuildWarHelper.GetGuildWarModel().IsCurrentWarOpen(GameManager.Instance.playerModel.UtcTimeStamp))
		{
			GuildWarDefinition guildWarDefinition = GuildWarHelper.GetGuildWarModel().FindNextGuildWar(GameManager.Instance.playerModel.UtcTimeStamp);
			if (guildWarDefinition != null && guildWarDefinition.IsOpen(GameManager.Instance.playerModel.UtcTimeStamp))
			{
				StartWar();
			}
		}
	}

	private void Update()
	{
		if (IsLoadDataManager) return;

		refreshTimer -= Time.deltaTime;
		if (refreshTimer < 0f && IsInCamp && playerModel != null && playerModel.manager != null && AreGuildsLoaded && AreGuildsReady && IsConnectedToServer)
		{
			UpdateGroupModel();
			UpdateClient();
			refreshTimer = refreshRate;
		}
	}

	private void StartSeason()
	{
		GvGSeasonModel gvGSeasonModel = GuildWarHelper.GetGvGSeasonModel();
		if (gvGSeasonModel == null)
		{
			return;
		}
		int identifier = gvGSeasonModel.FindNextSeason(GameManager.Instance.playerModel.UtcTimeStamp).Identifier;
		SubscribeToEvents();
		Helpers.ExecuteCommandDelayed(new StartGvGSeasonCommand(identifier), delegate(bool success)
		{
			if (success)
			{
				GroupCommandRequestStatus = new CommandRequestStatus(typeof(StartGvGSeasonGroupCommand));
			}
		});
	}

	private void StartWar()
	{
		GvGSeasonModel gvGSeasonModel = GuildWarHelper.GetGvGSeasonModel();
		GuildWarDefinition guildWarDefinition = GuildWarHelper.GetGuildWarModel().FindCurrentOrNextGuildWar(playerModel.UtcTimeStamp);
		GvGSeasonDefinition gvGSeasonDefinition = gvGSeasonModel.FindCurrentOrNextSeason(playerModel.UtcTimeStamp);
		if (guildWarDefinition == null || gvGSeasonDefinition == null)
		{
			return;
		}
		Helpers.ExecuteCommandDelayed(new StartGuildWarCommand(gvGSeasonDefinition.Identifier, guildWarDefinition.Identifier), delegate(bool success)
		{
			if (success)
			{
				GroupCommandRequestStatus = new CommandRequestStatus(typeof(StartGuildWarGroupCommand));
			}
		});
	}

	public void UpdateBattleTimestamps()
	{
		if (!AreGuildsLoaded || !AreGuildsReady)
		{
			Debug.LogWarning("UpdateBattleTimestamps: Guilds not loaded yet!");
		}
		else
		{
			if (!GameManager.Instance.playerModel.IsGuildMember || !GameManager.Instance.guildModel.GvGSeasonModel.IsCurrentSeasonOpen(GameManager.Instance.playerModel.UtcTimeStamp) || !GameManager.Instance.guildModel.GvGSeasonModel.GuildWarModel.IsCurrentWarOpen(GameManager.Instance.playerModel.UtcTimeStamp) || GroupCommandRequestStatus?.CommandType == typeof(UpdateGvgBattleEntriesGroupCommand))
			{
				return;
			}
			long num = playerModel.GuildWarModel.timeNextUpdateForGvgBattleEntries;
			if (GuildWarHelper.HasCurrentBattleEnded())
			{
				num = Math.Max(playerModel.GuildWarModel.CurrentBattle.EndBattleTimestamp, num);
			}
			else if (GuildWarHelper.IsBattleOnGoing())
			{
				num = Math.Max(playerModel.GuildWarModel.CurrentBattle.TimeSlot, num);
			}
			if (num + playerSpecificDelay <= GameManager.Instance.playerModel.UtcTimeStamp)
			{
				TWDModelResult result;
				if (GroupCommandRequestStatus != null)
				{
					Debug.LogError("UpdateBattleTimestamps: Cannot start register still waiting for response!");
				}
				else if (GameManager.Instance.guildModel.GuildWarModel.GetTimeSlotsForGvgBattleEntriesObsolete(GameManager.Instance.guildModel, out result, GameManager.Instance.playerModel.UtcTimeStamp).Any() && Helpers.ExecuteCommand(new UpdateGvgBattleEntriesCommand()) == TWDModelResult.OK)
				{
					GroupCommandRequestStatus = new CommandRequestStatus(typeof(UpdateGvgBattleEntriesGroupCommand));
				}
			}
		}
	}

	public void RegisterToGuildBattle(long timeSlot)
	{
		if (!AreGuildsLoaded || !AreGuildsReady)
		{
			Debug.LogWarning("RegisterForGuildBattleCommand: Cannot start register still waiting for guild load!");
			return;
		}
		if (GroupCommandRequestStatus != null && (GroupCommandRequestStatus.CommandType == typeof(RegisterForGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(ResignFromGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(RemovePlayerFromGuildBattleGroupCommand)))
		{
			Debug.LogError("RegisterForGuildBattleCommand: Cannot start register still waiting for response!");
			return;
		}
		GvGSeasonModel gvGSeasonModel = GuildWarHelper.GetGvGSeasonModel();
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		string hashedId = GameManager.Instance.playerModel.HashedId;
		if (GuildWarHelper.IsLimitRegisted() || !guildWarModel.CanPlayerRegisterForBattleSlot(timeSlot, playerModel.HashedId, playerModel.UtcTimeStamp))
		{
			return;
		}
		GvGSeasonDefinition gvGSeasonDefinition = gvGSeasonModel.FindCurrentOrNextSeason(playerModel.UtcTimeStamp);
		GuildWarDefinition guildWarDefinition = guildWarModel.FindCurrentOrNextGuildWar(playerModel.UtcTimeStamp);
		if (guildWarDefinition != null && gvGSeasonDefinition != null)
		{
			if (Helpers.ExecuteCommand(new RegisterForGuildBattleCommand(gvGSeasonDefinition.Identifier, guildWarDefinition.Identifier, timeSlot)) == TWDModelResult.OK)
			{
				GroupCommandRequestStatus = new CommandRequestStatus(typeof(RegisterForGuildBattleGroupCommand));
				UpdateGuildBattleSnapshot(hashedId);
			}
		}
		else
		{
			Debug.LogWarning("RegisterToNextGuildBattle: No Wars Found!");
		}
	}

	public void ResignFromGuildBattle(long timeSlot)
	{
		if (!AreGuildsLoaded || !AreGuildsReady)
		{
			Debug.LogWarning("ResignFromGuildBattle: Cannot start resign still waiting for guild load!");
			return;
		}
		if (GroupCommandRequestStatus != null && (GroupCommandRequestStatus.CommandType == typeof(RegisterForGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(ResignFromGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(RemovePlayerFromGuildBattleGroupCommand)))
		{
			Debug.LogError("ResignFromGuildBattle: Cannot start resign still waiting for response!");
			return;
		}
		GvGSeasonModel gvGSeasonModel = GuildWarHelper.GetGvGSeasonModel();
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (!guildWarModel.CanPlayerResignFromBattleSlot(timeSlot, playerModel.HashedId, playerModel.UtcTimeStamp))
		{
			return;
		}
		GvGSeasonDefinition gvGSeasonDefinition = gvGSeasonModel.FindCurrentOrNextSeason(playerModel.UtcTimeStamp);
		GuildWarDefinition guildWarDefinition = guildWarModel.FindCurrentOrNextGuildWar(playerModel.UtcTimeStamp);
		if (guildWarDefinition != null && gvGSeasonDefinition != null)
		{
			if (Helpers.ExecuteCommand(new ResignFromGuildBattleCommand(gvGSeasonDefinition.Identifier, guildWarDefinition.Identifier, timeSlot)) == TWDModelResult.OK)
			{
				GroupCommandRequestStatus = new CommandRequestStatus(typeof(ResignFromGuildBattleGroupCommand));
			}
		}
		else
		{
			Debug.LogWarning("ResignFromGuildBattle: No Wars Found!");
		}
	}

	public void RemovePlayerFromGuildBattle(long timeSlot, string hashId)
	{
		if (!AreGuildsLoaded || !AreGuildsReady)
		{
			Debug.LogWarning("RemovePlayerFromGuildBattle: Cannot start resign still waiting for guild load!");
			return;
		}
		if (GroupCommandRequestStatus != null && (GroupCommandRequestStatus.CommandType == typeof(RegisterForGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(ResignFromGuildBattleGroupCommand) || GroupCommandRequestStatus.CommandType == typeof(RemovePlayerFromGuildBattleGroupCommand)))
		{
			Debug.LogError("RemovePlayerFromGuildBattle: Cannot start resign still waiting for response!");
			return;
		}
		GvGSeasonModel gvGSeasonModel = GuildWarHelper.GetGvGSeasonModel();
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (!guildWarModel.CanPlayerResignFromBattleSlot(timeSlot, hashId, playerModel.UtcTimeStamp))
		{
			return;
		}
		GvGSeasonDefinition gvGSeasonDefinition = gvGSeasonModel.FindCurrentOrNextSeason(playerModel.UtcTimeStamp);
		GuildWarDefinition guildWarDefinition = guildWarModel.FindCurrentOrNextGuildWar(playerModel.UtcTimeStamp);
		if (guildWarDefinition != null && gvGSeasonDefinition != null)
		{
			NotificationHubSendPushRequest notificationHubSendPushRequest = new NotificationHubSendPushRequest
			{
				AndroidTitle = "Guild Wars",
				IosBadgeNumber = 1,
				Message = "Your Guild Wars Battle sign-up has been removed!",
				ScheduledTimeEpochSeconds = 0L,
				HashedIds = new List<string> { hashId }
			};
			if (Helpers.ExecuteCommand(new RemovePlayerFromGuildBattleCommand(gvGSeasonDefinition.Identifier, guildWarDefinition.Identifier, timeSlot, hashId, notificationHubSendPushRequest)) == TWDModelResult.OK)
			{
				GroupCommandRequestStatus = new CommandRequestStatus(typeof(RemovePlayerFromGuildBattleGroupCommand));
			}
		}
		else
		{
			Debug.LogWarning("RemovePlayerFromGuildBattle: No Wars Found!");
		}
	}

	public void CheckGuildWarStatus()
	{
		UpdateGuildWarForPlayer();
	}

	public void UpdateGuildBattleSnapshot(string senderHashId)
	{
		if (!GuildWarHelper.IsLockedByCouncilLevelOrTutorial() && playerModel != null && playerModel.HashedId == senderHashId && playerModel.IsGuildMember)
		{
			GuildManager.CheckGvGDefenders(playerModel);
			GuildBattleParticipantInfo playerInfo = GvGModelHelper.CreateEnemyPlayerData(playerModel, playerModel.gameEconomyData);
			GuildModel guildModel = playerModel.GuildModel;
			if (guildModel.GuildBattleMatchmakingInfo.ShouldUpdateGuildBattlePlayerSnapshot(playerInfo) || guildModel.MatchmakingVersion < playerModel.gameEconomyData.GuildWarConfig.MatchmakingVersion)
			{
				Helpers.ExecuteCommandDelayed(new UpdateGuildWarPlayerSnapshotCommand(playerInfo));
			}
		}
	}

	private void UpdateGuildWarForPlayer()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
			DebugTWD.LogWarning("UpdateGuildWarForPlayer, Ignore and return. Позже нужно поизучать, что оставить!", DebugType.Wars);
			return;
		}
		if (playerModel == null || !playerModel.IsGuildMember || !playerModel.GvGSeasonModel.IsCurrentSeasonOpen(playerModel.UtcTimeStamp))
		{
			return;
		}
		if (!playerModel.GvGSeasonModelPlayer.HasGvGSeasonStarted())
		{
			Helpers.ExecuteCommandDelayed(new StartGvGSeasonForPlayerCommand(), delegate(bool success)
			{
				if (success)
				{
					GuildWarHelper.CheckGuildWarFlowPopups();
				}
			});
			UpdateGuildBattleSnapshot(playerModel.HashedId);
		}
		if (!playerModel.GuildWarModel.IsCurrentWarOpen(playerModel.UtcTimeStamp))
		{
			return;
		}
		if (playerModel.GuildWarModel.IsCurrentWarOpen(playerModel.UtcTimeStamp) && !playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.HasWarStarted())
		{
			Helpers.ExecuteCommandDelayed(new StartGuildWarForPlayerCommand(), delegate(bool success)
			{
				if (success)
				{
					GuildWarHelper.CheckGuildWarFlowPopups();
				}
			});
		}
		if (!GuildWarHelper.IsPlayerRegisteredForBattle())
		{
			if (GuildWarHelper.IsBattleOnGoing() && playerModel.Blackboard.IsToggleOn("HasSeenGuildBattleEnd"))
			{
				Helpers.ExecuteCommandDelayed(new ClearBlackboardToggleCommand("HasSeenGuildBattleEnd"));
			}
		}
		else
		{
			if (!playerModel.GuildWarModel.CurrentBattle.IsOngoing(playerModel.UtcTimeStamp) || playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentBattleActiveForPlayer() || !IsInCamp)
			{
				return;
			}
			if (playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsOngoingForPlayer())
			{
				if (GameManager.Instance.gameEconomyData.GetFeature("ForceResolveBattleForPlayer").Enabled)
				{
					Debug.LogWarning("Battle has not been resolved for the player, so we resolve it");
					ResolveEndBattle();
				}
				return;
			}
			Helpers.ExecuteCommandDelayed(new StartGuildBattleForPlayerCommand(), delegate(bool success)
			{
				if (success)
				{
					GuildWarHelper.CheckGuildWarFlowPopups();
					GameManager.Instance.GuildManager.UpdateGvGRelatedInfo();
				}
			});
		}
	}

	public void ResolveEndBattle()
	{
		Helpers.ExecuteCommandDelayed(new ResolveEndBattleCommand(), delegate
		{
			GameManager.Instance.GuildManager.UpdateGvGRelatedInfo();
			UpdateGuildWarForPlayer();
		});
	}

	public void ShowSectorCompleteVPReward(int sectorId, GameObject collectAnimVP)
	{
		int guildSectorBattleVictoryPoints = GuildWarHelper.GetGuildWarModel().CurrentBattle.GetGuildSectorBattleVictoryPoints(sectorId);
		GameObject gameObject = null;
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		if (guildBattleMapPopup != null)
		{
			gameObject = guildBattleMapPopup.GetViewInstance().FindButtonWithId(sectorId.ToString()).gameObject;
		}
		if (gameObject != null)
		{
			PlayVPRewardAnimation(guildSectorBattleVictoryPoints, collectAnimVP, gameObject.transform);
		}
	}

	public void PlayVPRewardAnimation(int amount, GameObject sourcePrefab, Transform from, Transform to = null, bool useCameraOffsetPos = true)
	{
		if (to == null)
		{
			CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
			if (campHUD != null)
			{
				to = campHUD.GuildBattleProgressBarGetter;
			}
		}
		if (from != null && to != null)
		{
			StartCoroutine(PlayAnimationAfterLoading(delegate
			{
				PrepareVPAnimation(amount, sourcePrefab, from, to, useCameraOffsetPos);
			}));
		}
	}

	public void ClaimSectorReward(int sectorId)
	{
		RewardCurrency sectorReward = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.GetSectorRewardForCurrentBattle(sectorId, claimableOnly: true) as RewardCurrency;
		if (sectorReward == null)
		{
			return;
		}
		Helpers.ExecuteCommandDelayed(new ClaimGuildBattleSectorRewardCommand(sectorId), delegate(bool success)
		{
			if (success)
			{
				GameObject sectorGameObject = null;
				GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
				if (guildBattleMapPopup != null)
				{
					sectorGameObject = guildBattleMapPopup.GetViewInstance().FindButtonWithId(sectorId.ToString()).gameObject;
				}
				StartCoroutine(PlayAnimationAfterLoading(delegate
				{
					StartRPAnimation(sectorReward.CurrencyType, sectorGameObject, sectorReward.Amount);
				}));
			}
		});
	}

	private void StartRPAnimation(CurrencyType currencyType, GameObject gameObjectToFollow, int amount, AnimComplete animComplete = null)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null && campHUD.GetComponent<BuildingsHUD>() != null)
		{
			campHUD.GetComponent<BuildingsHUD>().CreateCollectAnim(currencyType, gameObjectToFollow, amount, null, BuildingsHUD.CollectSoundTrigger.OnStart, gameObjectToFollow);
		}
	}

	private IEnumerator PlayAnimationAfterLoading(Action action)
	{
		GuildBattleMapPopup mapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		while (mapPopup == null || mapPopup.GetViewInstance() == null)
		{
			if (mapPopup == null)
			{
				yield break;
			}
			yield return null;
		}
		while (mapPopup == null || mapPopup.GetViewInstance().GetCamera() == null)
		{
			if (mapPopup == null)
			{
				yield break;
			}
			yield return null;
		}
		yield return null;
		while (mapPopup == null || mapPopup.GetViewInstance().IsTweening())
		{
			if (mapPopup == null)
			{
				yield break;
			}
			yield return null;
		}
		action();
	}

	private void PrepareVPAnimation(int amount, GameObject sourcePrefab, Transform from, Transform to, bool useCameraOffsetPos)
	{
		if (from == null || to == null)
		{
			return;
		}
		int num = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 4 : 8);
		CollectAnimation collectAnimation = null;
		for (int i = 0; i < num; i++)
		{
			collectAnimation = Helpers.InstantiateToParent(sourcePrefab, to.gameObject).GetComponent<CollectAnimation>();
			if (collectAnimation != null)
			{
				collectAnimation.transform.position = from.position;
				CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
				if (campHUD != null)
				{
					AnimComplete animComplete = campHUD.GuildBattleProgressBarGetter.GetComponent<UIGuildBattleVictoryPointsProgressBar>().CurrencyTweenAnimationComplete;
					collectAnimation.StartAnimation(amount, GetGvGMapFlyingCurrencyTargetPosition(to, useCameraOffsetPos), animComplete, i == 0);
				}
				else
				{
					collectAnimation.StartAnimation(amount, GetGvGMapFlyingCurrencyTargetPosition(to, useCameraOffsetPos));
				}
			}
		}
	}

	public Vector3 GetGvGMapFlyingCurrencyTargetPosition(Transform target, bool useCameraOffsetPosition)
	{
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		if (guildBattleMapPopup != null && target != null)
		{
			GuildBattleMapView viewInstance = guildBattleMapPopup.GetViewInstance();
			if (viewInstance != null)
			{
				float num = (useCameraOffsetPosition ? viewInstance.GetCamera().orthographicSize : 1f);
				Vector3 result = target.position * num;
				if (useCameraOffsetPosition)
				{
					result += viewInstance.GetCamera().transform.position;
				}
				return result;
			}
		}
		return Vector3.zero;
	}

	public void GuildBattleBuffAnimationSeen(string bonusName, int stackedBuffNum)
	{
		Helpers.ExecuteCommandDelayed(new GuildBattleBuffAnimationSeenCommand(bonusName, stackedBuffNum));
	}

	private void SendBattleEndCommand()
	{
		Helpers.ExecuteCommandDelayed(new EndGuildBattleCommand(), delegate(bool success)
		{
			if (success)
			{
				GroupCommandRequestStatus = new CommandRequestStatus(typeof(EndGuildBattleGroupCommand));
				GuildWarHelper.CheckGuildWarFlowPopups();
			}
		});
	}

	private void SetupAttackTargetMissionData()
	{
		GameManager.Instance.playerModel.GuildBattlePlayer?.AttackTargetMission.Setup(GameManager.Instance.modelManager);
	}

	private void CheckIfGuildHasSavedInMatchmaking()
	{
		if (!GuildWarHelper.IsWarOngoing())
		{
			return;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		foreach (KeyValuePair<long, List<string>> item in guildWarModel.RegisteredPlayersForBattleSlot)
		{
			if (guildWarModel.CurrentBattle.TimeSlot != item.Key && !guildWarModel.GuildBattleResults.ContainsKey(item.Key) && !guildWarModel.IsBattleSlotLocked(item.Key, playerModel.UtcTimeStamp) && item.Value.Count == GameManager.Instance.gameEconomyData.GuildWarConfig.MinPlayersToStartBattle)
			{
				Helpers.ExecuteCommandDelayed(new CheckGuildJoinedStateCommand());
				break;
			}
		}
	}

	private bool GroupCommandRequestPending()
	{
		if (GameManager.Instance.playerModel.UtcTimeStamp - GroupCommandRequestStatus.SendTimestamp < 30000)
		{
			return true;
		}
		if (GroupCommandRequestStatus.CommandType.ToString() == lastGroupCommand)
		{
			groupCommandRequestRetryCount++;
		}
		else
		{
			lastGroupCommand = GroupCommandRequestStatus.CommandType.ToString();
			groupCommandRequestRetryCount = 1;
		}
		Debug.LogWarning("Did not receive response from social in time for group command : " + GroupCommandRequestStatus.CommandType?.ToString() + ". Retrying.");
		GroupCommandRequestStatus = null;
		if (groupCommandRequestRetryCount >= 5)
		{
			SignalRClient.Instance.RequestCommand("SyncGroup", GameManager.Instance.guildModel.Id, delegate
			{
				groupCommandRequestRetryCount = 0;
			}, waitForResponse: false);
			return true;
		}
		return false;
	}

	public void RequestBattleHighscoresUpdate(bool forceBroadcast = false)
	{
		if (IsGuildAvailable && GuildWarHelper.GetGuildWarModel()?.CurrentBattle?.HasStarted() == true && GameManager.Instance.playerModel.UtcTimeStamp > NextHighscoresRequestTimestamp)
		{
			UpdateNextHighscoresRequestTimestamp();
			Helpers.ExecuteCommandDelayed(new RequestGuildBattleHighscoresCommand(forceBroadcast));
		}
	}

	public void UpdateNextHighscoresRequestTimestamp()
	{
		long num = (long)((float)GameManager.Instance.gameEconomyData.GuildWarConfig.BattleLeaderboardsCacheDurationInMilliseconds * UnityEngine.Random.Range(1f, 2f));
		NextHighscoresRequestTimestamp = GuildWarHelper.GetGuildWarModel().CurrentBattle.LastLeaderboardsUpdateTimestamp + num;
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private bool IsPlayerSubscribed;
	private bool IsGuildSubscribed;
	#endregion
}
