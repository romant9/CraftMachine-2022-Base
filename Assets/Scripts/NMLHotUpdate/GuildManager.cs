using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class GuildManager
{
	public enum SocialMessageType
	{
		Guild = 0,
		GuildWar = 1
	}

	public delegate void LoadGroupCompleted(bool success);

	public delegate void SocialMessageReceived(SocialMessageType type, string memberInfo);

	public delegate void GroupCommandReceived(GroupCommandBase type, string memberInfo);

	public class GuildData
	{
		public string Id;

		public string Json;
	}

	private TWDModelManager modelManager;

	private GuildModel processGuildModel;

	private const long SEARCH_WAIT_TIMEOUT = 30000L;

	private bool loadingGuildModel;

	private bool waitingSyncResponse;

	private IEnumerator syncGroupProcessContainer;

	private List<GuildModel> cachedDefaultSearchResults;

	private List<string> lastSearchResultsLevel;

	private List<string> lastSearchResultsCountry;

	private List<string> lastSearchResultsNew;

	private List<string> lastSearchResultsKeyword;

	private List<string> lastSearchResultsAds;

	private List<string> lastSearchResultsFallback;

	private string lastSearchTerm = "";

	private string pendingJoinGuildId;

	private GuildModel pendingCreateGuildModel;

	private string lastViewedGuildAdId;

	private string lastEventSearchId;

	private List<GuildModel> lastSearchModels;

	private string lastSuggestModelId;

	private long lastSearchStartTS;

	private GuildSearchEventData suggestEventData;

	private bool hubDisconnected;

	private long lastSyncModelSequenceId;

	private long lastLoadGroupSyncRequestedTimestamp;

	private float loadGroupRequestTimeoutMilliseconds = 300000f;

	private Queue<GroupCommandBase> queuedMessagedDuringLoad;

	private const string PromotedDate = "PromotedDate";

	private const string DemotedDate = "DemotedDate";

	private List<GuildData> guildsToLoad;

	public bool IsLoading => loadingGuildModel;

	public bool IsWaitingForSyncResponse => waitingSyncResponse;

	public bool IsBusy
	{
		get
		{
			if (!IsLoading && !IsWaitingForSyncResponse)
			{
				return hubDisconnected;
			}
			return true;
		}
	}

	public GuildSuggestionLogic SuggestionLogic { get; private set; }

	public bool GuildOffline { get; private set; }

	public GuildModel Model
	{
		get
		{
			if (modelManager.Player != null && modelManager.Player.HasGuild)
			{
				return modelManager.GetGroupModel(modelManager.Player.GuildId) as GuildModel;
			}
			return null;
		}
	}

	public event LoadGroupCompleted OnLoadGroupCompleted;

	public event SocialMessageReceived OnSocialMessageReceived;

	public event GroupCommandReceived OnGroupCommandReceived;

	public event Action<List<GuildModel>> guildSearchFinishedEvent;

	public event Action<string> guildSearchFailedEvent;

	public event Action<GuildModel> guildSuggestFinishedEvent;

	public GuildManager(TWDModelManager manager)
	{
		modelManager = manager;
		SuggestionLogic = new GuildSuggestionLogic(this, manager);
		queuedMessagedDuringLoad = new Queue<GroupCommandBase>();
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.OnSocialMessage += OnSocialMessage;
			SignalRClient.Instance.OnServerMessage += OnServerMessage;
			SignalRClient.Instance.OnHubConnectionMessage += OnHubConnectionMessage;
			StartLoadingGuildModel();
		}
	}

	public void Uninitialize()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnSocialMessage -= OnSocialMessage;
			SignalRClient.Instance.OnServerMessage -= OnServerMessage;
			SignalRClient.Instance.OnHubConnectionMessage -= OnHubConnectionMessage;
		}
	}

	public void NotifyGuildAdViewed(string advertisementId)
	{
		if (advertisementId != null && advertisementId != lastViewedGuildAdId)
		{
			Helpers.ExecuteCommand(new SendMetricCommand(SendMetricCommand.MetricType.GuildAdViewed)
			{
				IdParameter = advertisementId
			});
			lastViewedGuildAdId = advertisementId;
		}
	}

	private bool StartLoadingGuildModel(bool checkTimeout = false)
	{
		if (!GameManager.Instance.IsSocialEnabled())
		{
			return false;
		}
		string arg = "";
		if (modelManager.Player != null && modelManager.Player.GuildId != null)
		{
			arg = "[\"" + modelManager.Player.GuildId + "\"]";
		}
		if (checkTimeout && (float)(GameManager.Instance.playerModel.UtcTimeStamp - lastLoadGroupSyncRequestedTimestamp) < loadGroupRequestTimeoutMilliseconds)
		{
			return false;
		}
		Debug.LogWarning("Client requested LoadGroups, last time called " + lastLoadGroupSyncRequestedTimestamp);
		lastLoadGroupSyncRequestedTimestamp = GameManager.Instance.playerModel.UtcTimeStamp;
		SignalRClient.Instance.RequestCommand("LoadGroups", arg, OnGroupsAsync, waitForResponse: true);
		GuildOffline = false;
		loadingGuildModel = true;
		pendingJoinGuildId = null;
		pendingCreateGuildModel = null;
		processGuildModel = null;
		return true;
	}

	private void OnHubConnectionMessage(string message, string type)
	{
		switch (type)
		{
		case "error":
			hubDisconnected = true;
			break;
		case "connected":
			if (hubDisconnected && Model != null && GameManager.Instance.gameEconomyData != null && GameManager.Instance.gameEconomyData.GetFeature("ForceGroupSyncOnDisconnect").Enabled && StartLoadingGuildModel(checkTimeout: true))
			{
				lastSyncModelSequenceId = Model.SequenceId;
			}
			hubDisconnected = false;
			break;
		case "disconnected":
			hubDisconnected = true;
			break;
		}
	}

	public void OnServerMessage(string message, string type)
	{
		if (type == "timeout" && message == "CreateGroup")
		{
			processGuildModel = null;
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
		}
	}

	public void OnSocialMessage(string message, string type)
	{
		if (SignalRClient.Instance.Statistics.LastErrorType == ErrorType.GuildsOffline || string.IsNullOrEmpty(message))
		{
			SignalRClient.Instance.ClearError();
			loadingGuildModel = false;
			GuildOffline = true;
			SendOnLoadGroupCompleted(success: false);
		}
		else if (type == "SocialGroupLoaded")
		{
			OnGroupData(message);
		}
		else
		{
			if (type == "SocialGroupLoadFailed")
			{
				return;
			}
			_ = type == "SocialCommand";
			IMessageSerializer messageSerializer = modelManager.GetMessageSerializer();
			JsonCommand jsonCommand = messageSerializer.DeserializeObject<JsonCommand>(message);
			Type type2 = Type.GetType(jsonCommand.Type, throwOnError: false);
			if (type2 == null)
			{
				Debug.LogError("Group command type '" + jsonCommand.Type + "' not found");
				return;
			}
			if (!typeof(GroupCommandBase).IsAssignableFrom(type2))
			{
				Debug.LogError("Group command type '" + jsonCommand.Type + "' does not inherit GroupCommandBase");
				return;
			}
			if (!(messageSerializer.DeserializeObject(type2, jsonCommand.Command) is GroupCommandBase groupCommandBase))
			{
				Debug.LogError("Failed to parse group command " + jsonCommand.Command);
				return;
			}
			if (type == "SocialGroupRemoved" && groupCommandBase is LeftGuildGroupCommand)
			{
				OnLeftGuild(groupCommandBase as LeftGuildGroupCommand);
				return;
			}
			if (Model == null && !IsLoading)
			{
				StartLoadingGuildModel();
				return;
			}
			if (Model == null || groupCommandBase.GroupId != Model.Id)
			{
				if (Model != null)
				{
					_ = Model.Id;
				}
				return;
			}
			if ((IsLoading || IsBusy) && type2 != typeof(SyncGroupCommand))
			{
				Debug.LogWarning("Missed social message (doing load) added to queue: " + type + " - " + groupCommandBase.GetType());
				queuedMessagedDuringLoad.Enqueue(groupCommandBase);
				return;
			}
			if (groupCommandBase.SequenceId > Model.SequenceId + 1 && type2 != typeof(SyncGroupCommand))
			{
				Debug.LogWarning("Social message received out of order. Command with sequence : " + groupCommandBase.SequenceId + ", client expecting " + (Model.SequenceId + 1) + ". Triggering group sync.");
				if (GameManager.Instance.gameEconomyData != null && GameManager.Instance.gameEconomyData.GetFeature("ForceGroupSyncOnOutOfOrder").Enabled && StartLoadingGuildModel(checkTimeout: true))
				{
					return;
				}
			}
			OnSocialMessageReceivedInternal(groupCommandBase);
		}
	}

	private void OnSocialMessageReceivedInternal(GroupCommandBase groupCommand)
	{
		if (groupCommand is LeftGuildGroupCommand)
		{
			OnLeftGuild(groupCommand as LeftGuildGroupCommand);
			return;
		}
		modelManager.ExecuteGroupCommand(groupCommand);
		lastSyncModelSequenceId = ((Model == null) ? 0 : Model.SequenceId);
		if (this.OnGroupCommandReceived != null)
		{
			this.OnGroupCommandReceived(groupCommand, groupCommand.SenderId);
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (groupCommand is LeaveGuildGroupCommand)
		{
			LeaveGuildGroupCommand command = groupCommand as LeaveGuildGroupCommand;
			OnLeaveGuild(command);
		}
		else if (groupCommand is AcceptMembershipGroupCommand)
		{
			AcceptMembershipGroupCommand acceptMembershipGroupCommand = groupCommand as AcceptMembershipGroupCommand;
			if (playerModel.HashedId == acceptMembershipGroupCommand.MemberId)
			{
				playerModel.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildRequestAccepted, 0, Model.Name, 0);
				UIEvent.Send("SocialMembershipAccepted");
				CheckChallenge();
				UpdateGvGRelatedInfo();
			}
			NotifySocialMessageReceived(SocialMessageType.Guild, acceptMembershipGroupCommand.MemberId);
		}
		else if (groupCommand is AddGuildMemberGroupCommand)
		{
			AddGuildMemberGroupCommand addGuildMemberGroupCommand = groupCommand as AddGuildMemberGroupCommand;
			OnAddGuildMember(groupCommand as AddGuildMemberGroupCommand);
			NotifySocialMessageReceived(SocialMessageType.Guild, addGuildMemberGroupCommand.MemberId);
		}
		else if (groupCommand is UpdateMemberInfoGroupCommand)
		{
			UpdateMemberInfoGroupCommand updateMemberInfoGroupCommand = groupCommand as UpdateMemberInfoGroupCommand;
			NotifySocialMessageReceived(SocialMessageType.Guild, updateMemberInfoGroupCommand.SenderId);
		}
		else if (groupCommand is ChangeGuildNameGroupCommand)
		{
			ChangeGuildNameGroupCommand changeGuildNameGroupCommand = groupCommand as ChangeGuildNameGroupCommand;
			NotifySocialMessageReceived(SocialMessageType.Guild, changeGuildNameGroupCommand.SenderId);
		}
		else if (groupCommand is ModifyMemberRoleGroupCommand)
		{
			ModifyMemberRoleGroupCommand modifyMemberRoleGroupCommand = groupCommand as ModifyMemberRoleGroupCommand;
			if (playerModel.HashedId == modifyMemberRoleGroupCommand.MemberId && modifyMemberRoleGroupCommand.IsPromotion)
			{
				playerModel.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildPromoted, 0, Model.Name, 0);
			}
			else if (playerModel.HashedId == modifyMemberRoleGroupCommand.MemberId)
			{
				playerModel.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildDemoted, 0, Model.Name, 0);
			}
			NotifySocialMessageReceived(SocialMessageType.Guild, modifyMemberRoleGroupCommand.MemberId);
		}
		else if (groupCommand is PromoteMemberToLeaderGroupCommand)
		{
			PromoteMemberToLeaderGroupCommand promoteMemberToLeaderGroupCommand = groupCommand as PromoteMemberToLeaderGroupCommand;
			if (playerModel.HashedId == promoteMemberToLeaderGroupCommand.MemberId)
			{
				playerModel.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildPromotedLeader, 0, Model.Name, 0);
			}
			NotifySocialMessageReceived(SocialMessageType.Guild, promoteMemberToLeaderGroupCommand.MemberId);
		}
		else if (groupCommand is DemoteInactiveLeaderGroupCommand)
		{
			CheckIfNewLeader();
		}
		else if (groupCommand is AddChallengeStarsGroupCommand && !IsWaitingForSyncResponse)
		{
			CheckChallenge();
		}
		else if (groupCommand is CreateGuildGroupCommand)
		{
			CreateGuildGroupCommand createGuildGroupCommand = groupCommand as CreateGuildGroupCommand;
			CheckChallenge();
			if (playerModel.WeeklyChallenge != null)
			{
				Helpers.ExecuteCommand(new UpdateGuildChallengeCommand());
			}
			CheckForGuildShopInitialization();
			SetMemberLastActiveDate();
			UpdateGvGRelatedInfo();
			NotifySocialMessageReceived(SocialMessageType.Guild, createGuildGroupCommand.Leader.MemberId);
		}
		else if (groupCommand is SyncGroupCommand)
		{
			OnSyncGroup(groupCommand as SyncGroupCommand);
		}
		else if (groupCommand is GiveGuildGiftGroupCommand && !IsWaitingForSyncResponse)
		{
			CheckForGuildGifts();
		}
		else if (groupCommand is RegisterForGuildBattleGroupCommand && GuildWarHelper.IsWarOngoing())
		{
			NotifySocialMessageReceived(SocialMessageType.GuildWar, groupCommand.SenderId);
		}
		else if (groupCommand is StartGuildWarGroupCommand && GuildWarHelper.IsWarOngoing())
		{
			NotifySocialMessageReceived(SocialMessageType.GuildWar, groupCommand.SenderId);
		}
		else if (groupCommand is StartGvgBattleGroupCommand && GuildWarHelper.IsWarAndBattleOngoing())
		{
			NotifySocialMessageReceived(SocialMessageType.GuildWar, groupCommand.SenderId);
		}
		else if (groupCommand is EndGuildBattleGroupCommand)
		{
			CheckForNewGuildTier();
		}
	}

	private void OnSyncGroup(SyncGroupCommand command)
	{
		waitingSyncResponse = false;
		if (modelManager.GameEconomyData.GetFeature("RetrySyncGroupTimeout").Enabled)
		{
			Helpers.StopCoroutine(GameManager.Instance, ref syncGroupProcessContainer);
		}
		bool flag = !SignalRClient.Instance.HasError;
		if (flag)
		{
			ExecuteQueuedGroupCommands();
		}
		GuildMemberInfo memberInfo = Model.GetMemberInfo(modelManager.Player.HashedId);
		if (memberInfo == null && Model.GetMemberPendingInfo(modelManager.Player.HashedId) == null)
		{
			modelManager.Debug.LogWarning("Leaving guild, player is not an member\n" + Model.Id);
			ForceLeaveGuild();
			SetPlayerGuild("");
			return;
		}
		SendOnLoadGroupCompleted(flag);
		if (memberInfo != null && (memberInfo.PlayerLevel != modelManager.Player.Level || memberInfo.Name != modelManager.Player.Name))
		{
			Helpers.ExecuteCommand(new UpdateMemberInfoCommand());
		}
		CheckForNewGuildTier();
		CheckChallenge();
		CheckForGuildGifts();
		SetMemberLastActiveDate();
		UpdateGvGRelatedInfo();
		CheckForLeaderInactivity();
		CheckIfNewLeader();
		CheckIfDemoted();
		CheckGvGDefenders(modelManager.Player);
		if (!OfflineManager.IsLoadDataManager) CheckForUnSeenRemovedBattleSlots();
	}

	public static void CheckGvGDefenders(PlayerModel playerModel)
	{
		if (GuildWarHelper.IsLockedByCouncilLevel())
		{
			return;
		}
		if (IsGvgDefendersInitialized(playerModel))
		{
			CheckIfGvgDefendersAreObsolete(playerModel);
			return;
		}
		Helpers.ExecuteCommandDelayed(new InitializeGvgDefendersCommand(), delegate(bool result)
		{
			if (!result)
			{
				Debug.LogError("Something went wrong when initialising gvg defenders");
			}
			else
			{
				CheckIfGvgDefendersAreObsolete(playerModel);
			}
		});
	}

	private static bool IsGvgDefendersInitialized(PlayerModel player)
	{
		if (player.GvGDefenders != null && player.GvGDefenders.Count == 9)
		{
			return player.GvGDefenders.Count((SurvivorMockData x) => string.IsNullOrEmpty(x.AnalyticsId) || x.AnalyticsId == "0") == 0;
		}
		return false;
	}

	private static void CheckIfGvgDefendersAreObsolete(PlayerModel player)
	{
		foreach (SurvivorMockData survivorMockData in player.GvGDefenders)
		{
			if (!player.SurvivorContainer.Survivors.Any((SurvivorModel x) => x.IdForAnalytics == survivorMockData.AnalyticsId))
			{
				Helpers.ExecuteCommandDelayed(new FixObsoleteGvgDefendersCommand());
				break;
			}
		}
	}

	public void CheckForNewGuildTier()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.IsGuildMember && playerModel.GuildModel.GuildBattleTier < playerModel.GuildShopModel.HighestTierUnlocked && playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.HasParticipatedInBattle())
		{
			Helpers.ExecuteCommand(new RestockGuildShopCommand(onNewTier: true, onNewWar: false));
		}
	}

	private void CheckChallenge()
	{
		GameManager.Instance.StartNextChallenge();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.IsGuildMember && playerModel.WeeklyChallenge != null && playerModel.WeeklyChallenge.ShouldUpdateGuildRewards())
		{
			Helpers.ExecuteCommand(new UpdateChallengeRewardsCommand());
		}
	}

	private void CheckForGuildShopInitialization()
	{
		if (!GameManager.Instance.playerModel.GuildShopModel.InitializedThisSeason)
		{
			Helpers.ExecuteCommandDelayed(new RestockGuildShopCommand(onNewTier: true, onNewWar: false));
		}
	}

	private void CheckForGuildGifts()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.gameEconomyData.ConfigData.GuildGiftsEnabled && playerModel.IsGuildMember && playerModel.GuildModel != null)
		{
			Helpers.ExecuteCommand(new FetchGuildGiftsCommand
			{
				GuildId = playerModel.GuildModel.Id
			});
			UIEvent.Send("OnGuildGiftReceived");
		}
	}

	private void SetMemberLastActiveDate()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.IsGuildMember && playerModel.GuildModel != null)
		{
			Helpers.ExecuteCommand(new SetMemberLastActiveDateCommand
			{
				MemberId = playerModel.HashedId,
				MemberUTCTimestamp = playerModel.UtcTimeStamp
			});
		}
	}

	public void UpdateGvGRelatedInfo()
	{
		if (NeedsUpdate())
		{
			Helpers.ExecuteCommand(new SetMemberGvGInfoCommand());
		}
	}

	private bool NeedsUpdate()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.IsGuildMember && playerModel.GuildModel != null)
		{
			GuildMemberInfo memberInfo = playerModel.GuildModel.GetMemberInfo(playerModel.HashedId);
			if (memberInfo != null)
			{
				return memberInfo.TotalVP != playerModel.CalculateLifeTimeGvGVpAccumulated();
			}
			return false;
		}
		return false;
	}

	private void CheckForLeaderInactivity()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildModel guildModel = playerModel.GuildModel;
		if (playerModel.IsGuildMember && guildModel != null)
		{
			GuildMemberInfo leaderMemberInfo = guildModel.GetLeaderMemberInfo();
			if (leaderMemberInfo == null)
			{
				Helpers.ExecuteCommand(new DemoteInactiveLeaderCommand());
			}
			if (!(leaderMemberInfo.MemberId == playerModel.HashedId) && playerModel.UtcTimeStamp - leaderMemberInfo.LastActiveDate > GameManager.Instance.gameEconomyData.ConfigData.LeaderInactivityTimeThreshold)
			{
				Helpers.ExecuteCommand(new DemoteInactiveLeaderCommand());
			}
		}
	}

	private void CheckIfNewLeader()
	{
		ChatMessage chatMessage = Model.ChatMessages.FindLast((ChatMessage m) => m.NotificationType == ChatNotificationType.MemberPromotedToLeader);
		if (chatMessage != null && chatMessage.PlayerId == modelManager.Player.HashedId && TWDPlayerPrefs.GetString("PromotedDate") != chatMessage.Time.ToString())
		{
			TWDPlayerPrefs.SetString("PromotedDate", chatMessage.Time.ToString());
			modelManager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildPromotedLeaderByDemotion, 0, Model.Name, 0);
		}
	}

	private void CheckIfDemoted()
	{
		string hashedId = modelManager.Player.HashedId;
		ChatMessage chatMessage = Model.ChatMessages.FindLast((ChatMessage m) => m.NotificationType == ChatNotificationType.LeaderDemoted && m.PlayerId == hashedId);
		if (chatMessage != null && TWDPlayerPrefs.GetString("DemotedDate") != chatMessage.Time.ToString())
		{
			TWDPlayerPrefs.SetString("DemotedDate", chatMessage.Time.ToString());
			modelManager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildDemotedDueInactivity, 0, Model.Name, 0);
		}
	}

	private void OnAddGuildMember(AddGuildMemberGroupCommand command)
	{
		if (command.MemberId == modelManager.Player.HashedId)
		{
			SingularityMonoBehaviour<SDKManager>.Instance.ExternalAnalytics.JoinedGuild(command.GroupId);
			GameManager.Instance.RequestPltv();
			UIEvent.Send("SocialGuildJoined", command.GroupId);
			CheckForGuildShopInitialization();
			UpdateGvGRelatedInfo();
		}
	}

	private void OnLeftGuild(LeftGuildGroupCommand command)
	{
		string guildId = modelManager.Player.GuildId;
		if (command.GroupId != guildId)
		{
			if (!string.IsNullOrEmpty(guildId))
			{
				Debug.LogWarning("Received LeftGuildGroupCommand from wrong guild " + command.GroupId + " " + guildId);
			}
			return;
		}
		if (command.LeaverId != modelManager.Player.HashedId)
		{
			Debug.LogError("Received LeftGuildGroupCommand for wrong player " + command.LeaverId + " " + modelManager.Player.HashedId);
			return;
		}
		if (modelManager.GetGroupModel(command.GroupId) != null)
		{
			modelManager.ExecuteGroupCommand(command);
		}
		if (command.LeaveType == GuildLeaveType.Kick || command.LeaveType == GuildLeaveType.KickAndSoftBan)
		{
			modelManager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildKickedOut, 0, LocalizationManager.GetText("Popup.Social.Tab.Guild"), 0);
		}
		else if (command.LeaveType == GuildLeaveType.RejectRequest)
		{
			modelManager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildRequestRefused, 0, LocalizationManager.GetText("Popup.Social.Tab.Guild"), 0);
		}
		SetPlayerGuild("");
		if (pendingCreateGuildModel != null)
		{
			CreateGuild(pendingCreateGuildModel);
		}
		else if (!string.IsNullOrEmpty(pendingJoinGuildId))
		{
			JoinGuild(pendingJoinGuildId);
		}
	}

	private void OnLeaveGuild(LeaveGuildGroupCommand command)
	{
		if (command.SenderId != modelManager.Player.HashedId)
		{
			return;
		}
		if (Model != null && Model.GetActiveMembers() == 0)
		{
			foreach (GuildMemberInfo item in Model.GuildMembersPending)
			{
				RefuseGuildMember(item.MemberId);
			}
			Helpers.ExecuteCommand(new DisbandGuildCommand
			{
				GuildId = command.GroupId
			});
			return;
		}
		Helpers.ExecuteCommand(new LeftGuildCommand
		{
			GuildId = command.GroupId,
			LeaveType = command.LeaveType,
			LeaverId = command.LeaverId
		});
		if (Model == null)
		{
			Debug.LogWarning("Guild model is NULL at OnLeaveGuild");
		}
	}

	protected void OnGroupsAsync(string groupsRespondJson)
	{
		if (SignalRClient.Instance.Statistics.LastErrorType == ErrorType.GuildsOffline || string.IsNullOrEmpty(groupsRespondJson))
		{
			Debug.LogError("LoadGroupsAsync failed: " + SignalRClient.Instance.LastErrorMessage);
			SignalRClient.Instance.ClearError();
			loadingGuildModel = false;
			GuildOffline = true;
			SendOnLoadGroupCompleted(success: false);
			return;
		}
		List<string> list = modelManager.GetMessageSerializer().DeserializeObject<List<string>>(groupsRespondJson);
		if (list.Count == 0)
		{
			loadingGuildModel = false;
			SendOnLoadGroupCompleted(success: false);
			return;
		}
		guildsToLoad = new List<GuildData>();
		for (int i = 0; i < list.Count; i++)
		{
			guildsToLoad.Add(new GuildData
			{
				Id = list[i],
				Json = null
			});
		}
	}

	protected void OnGroupData(string groupJson)
	{
		GuildModel guildModel = modelManager.GetMessageSerializer().DeserializeObject<GuildModel>(groupJson);
		if (guildsToLoad == null || guildsToLoad.Count == 0)
		{
			Debug.LogError("Received guild data while not loading " + guildModel.Id);
			loadingGuildModel = false;
			SendOnLoadGroupCompleted(success: false);
			return;
		}
		bool flag = true;
		bool flag2 = false;
		for (int i = 0; i < guildsToLoad.Count; i++)
		{
			GuildData guildData = guildsToLoad[i];
			if (guildData.Id == guildModel.Id)
			{
				if (guildData.Json != null)
				{
					Debug.LogError("Guild data already received " + guildModel.Id);
				}
				flag2 = true;
				guildData.Json = groupJson;
			}
			if (guildData.Json == null)
			{
				flag = false;
			}
		}
		if (!flag2)
		{
			Debug.LogError("Unknown guild data received " + guildModel.Id);
		}
		bool forceSync = guildModel.SequenceId != lastSyncModelSequenceId;
		if (flag)
		{
			LoadGroups(forceSync);
		}
	}

	protected void LoadGroups(bool forceSync)
	{
		for (int i = 0; i < guildsToLoad.Count; i++)
		{
			modelManager.LoadGroupModel(guildsToLoad[i].Json, forceSync);
		}
		string text = null;
		if (guildsToLoad.Count == 1)
		{
			text = guildsToLoad[0].Id;
			if (modelManager.GetGroupModel(text).SequenceId == 0L)
			{
				Debug.LogError("Leaving ghost guild!");
				ForceLeaveGuild(text);
				text = null;
			}
		}
		else if (guildsToLoad.Count > 1)
		{
			Debug.LogError("Invalid guild count " + guildsToLoad.Count);
			string text2 = null;
			string text3 = null;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int j = 0; j < guildsToLoad.Count; j++)
			{
				if (!(modelManager.GetGroupModel(guildsToLoad[j].Id) is GuildModel guildModel))
				{
					continue;
				}
				if (guildModel.GetMemberInfo(modelManager.Player.HashedId) != null)
				{
					if (text2 == null)
					{
						text2 = guildModel.Id;
					}
					else
					{
						list.Add(guildModel.Id);
					}
				}
				else if (guildModel.GetMemberPendingInfo(modelManager.Player.HashedId) != null)
				{
					if (text3 == null)
					{
						text3 = guildModel.Id;
					}
					else
					{
						list.Add(guildModel.Id);
					}
				}
				else
				{
					list2.Add(guildModel.Id);
				}
			}
			if (text2 != null)
			{
				if (text3 != null)
				{
					list.Add(text3);
				}
			}
			else
			{
				text2 = text3;
			}
			if (text2 != null)
			{
				text = text2;
			}
			for (int k = 0; k < list.Count; k++)
			{
				LeaveGuild(list[k]);
			}
			for (int l = 0; l < list2.Count; l++)
			{
				ForceLeaveGuild(list2[l]);
			}
		}
		if (text != null)
		{
			SetPlayerGuild(text);
			if (modelManager.GameEconomyData.GetFeature("RetrySyncGroupTimeout").Enabled)
			{
				Helpers.StartCoroutine(GameManager.Instance, SyncGroupProcess(text), ref syncGroupProcessContainer);
			}
			else
			{
				SignalRClient.Instance.RequestCommand("SyncGroup", text, OnSyncGroupAsync, waitForResponse: true);
				waitingSyncResponse = true;
			}
		}
		guildsToLoad = null;
	}

	private IEnumerator SyncGroupProcess(string loadedGuild)
	{
		waitingSyncResponse = true;
		while (waitingSyncResponse)
		{
			SignalRClient.Instance.RequestCommand("SyncGroup", loadedGuild, OnSyncGroupAsync, waitForResponse: true);
			yield return new WaitForSeconds(30f);
		}
	}

	protected void OnSyncGroupAsync(string groupsRespondJson)
	{
		loadingGuildModel = false;
		if (SignalRClient.Instance.Statistics.LastErrorType == ErrorType.GuildsOffline)
		{
			Debug.LogError("SyncGroupAsync failed: " + SignalRClient.Instance.LastErrorMessage);
			GuildOffline = true;
			SignalRClient.Instance.ClearError();
		}
	}

	private void ExecuteQueuedGroupCommands()
	{
		while (queuedMessagedDuringLoad.Count > 0)
		{
			GroupCommandBase groupCommand = queuedMessagedDuringLoad.Dequeue();
			OnSocialMessageReceivedInternal(groupCommand);
		}
	}

	public Cashier GetCreateGuildCashier()
	{
		return Cashier.CreateOneItemCashier(modelManager, PurchaseType.GuildCreate, CurrencyType.SurvivalPoints, 300);
	}

	public Cashier GetChangeGuildNameCashier()
	{
		return Cashier.CreateOneItemCashier(modelManager, PurchaseType.TradeCrate, CurrencyType.Diamonds, GameManager.Instance.gameEconomyData.ConfigData.GuildNameChangeCost);
	}

	public bool CreateGuild(GuildModel guild)
	{
		if (!GameManager.Instance.IsConnectedToServer)
		{
			Debug.LogError("Not connected to a server");
			return false;
		}
		pendingCreateGuildModel = null;
		if (Model != null)
		{
			pendingCreateGuildModel = guild;
			LeaveGuild(modelManager.Player.GuildId);
			return true;
		}
		if (processGuildModel != null)
		{
			Debug.LogError("Guild creation already in progress");
			return false;
		}
		processGuildModel = guild;
		SignalRClient.Instance.RequestCommand("CreateGroup", "", OnGroupCreated, waitForResponse: true);
		return true;
	}

	protected void OnGroupCreated(string groupsRespondJson)
	{
		Log("OnGroupCreated: " + groupsRespondJson);
		if (SignalRClient.Instance.HasError)
		{
			Debug.LogError("Create Group failed : " + SignalRClient.Instance.LastErrorMessage);
			processGuildModel = null;
		}
		else if (!string.IsNullOrEmpty(groupsRespondJson))
		{
			GroupModelBase groupModelBase = modelManager.GetMessageSerializer().DeserializeObject<GroupModelBase>(groupsRespondJson);
			if (groupModelBase == null)
			{
				Debug.LogError("Unable to load group model");
				processGuildModel = null;
				return;
			}
			modelManager.LoadGroupModel(groupsRespondJson);
			SetPlayerGuild(groupModelBase.Id);
			GuildMemberInfo guildMemberInfo = new GuildMemberInfo();
			guildMemberInfo.Name = modelManager.Player.Name;
			guildMemberInfo.MemberId = modelManager.Player.HashedId;
			guildMemberInfo.PlayerLevel = modelManager.Player.Level;
			guildMemberInfo.Role = GuildMemberRole.Leader;
			guildMemberInfo.State = GuildMemberState.Normal;
			guildMemberInfo.PlayerEmblem = modelManager.Player.PlayerEmblem;
			guildMemberInfo.GuildId = groupModelBase.Id;
			processGuildModel.Id = groupModelBase.Id;
			CreateGuildCommand command = new CreateGuildCommand
			{
				GuildData = processGuildModel,
				GuildLeader = guildMemberInfo,
				GuildLeaderCountryCode = GameManager.GetCountryCode()
			};
			processGuildModel = null;
			Helpers.ExecuteCommand(command);
		}
	}

	public bool SuggestGuild(int iteration)
	{
		if (!GameManager.Instance.IsConnectedToServer)
		{
			return false;
		}
		lastSuggestModelId = null;
		lastEventSearchId = null;
		if (iteration <= 1 || lastSearchStartTS <= 0)
		{
			lastSearchStartTS = GameManager.Instance.playerModel.UtcTimeStamp;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["queryType"] = "suggestion";
		dictionary["queryIteration"] = iteration.ToString();
		AddRandomBucketParameters(dictionary);
		string arg = modelManager.GetMessageSerializer().SerializeObject(dictionary);
		SignalRClient.Instance.RequestCommand("SearchGroups", arg, OnSearchGuilds, waitForResponse: false);
		return true;
	}

	public bool SearchGuilds(string keyword)
	{
		if (!GameManager.Instance.IsConnectedToServer)
		{
			return false;
		}
		if (string.IsNullOrEmpty(keyword) && cachedDefaultSearchResults != null)
		{
			SignalRClient.Instance.StartCoroutine(SendCachedSearchResults());
		}
		else
		{
			lastSearchStartTS = GameManager.Instance.playerModel.UtcTimeStamp;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["keyword"] = keyword;
			AddRandomBucketParameters(dictionary);
			lastSearchTerm = keyword;
			if (string.IsNullOrEmpty(keyword))
			{
				if (!string.IsNullOrEmpty(GameManager.Instance.gameEconomyData.ConfigData.GuildSuggestionsNewQuery))
				{
					dictionary["queryType"] = "suggestionsnew";
					string arg = modelManager.GetMessageSerializer().SerializeObject(dictionary);
					lastSearchResultsNew = null;
					SignalRClient.Instance.RequestCommand("SearchGroups", arg, OnSearchGuilds, waitForResponse: false);
				}
				else
				{
					lastSearchResultsNew = new List<string>();
				}
				if (!string.IsNullOrEmpty(GameManager.Instance.gameEconomyData.ConfigData.GuildSuggestionsSameCountryQuery))
				{
					dictionary["queryType"] = "suggestionssamecountry";
					string arg = modelManager.GetMessageSerializer().SerializeObject(dictionary);
					lastSearchResultsCountry = null;
					SignalRClient.Instance.RequestCommand("SearchGroups", arg, OnSearchGuilds, waitForResponse: false);
				}
				else
				{
					lastSearchResultsCountry = new List<string>();
				}
				if (!string.IsNullOrEmpty(GameManager.Instance.gameEconomyData.ConfigData.GuildSuggestionsSimilarLevelQuery))
				{
					dictionary["queryType"] = "suggestionssimilarlevel";
					string arg = modelManager.GetMessageSerializer().SerializeObject(dictionary);
					lastSearchResultsLevel = null;
					SignalRClient.Instance.RequestCommand("SearchGroups", arg, OnSearchGuilds, waitForResponse: false);
				}
				else
				{
					lastSearchResultsLevel = new List<string>();
				}
				if (GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled)
				{
					dictionary.Clear();
					dictionary["keyword"] = keyword;
					dictionary["queryType"] = "ads";
					dictionary["adBucket"] = UnityEngine.Random.Range(0, GameManager.Instance.gameEconomyData.ConfigData.GuildAdBucketCount).ToString();
					string arg = modelManager.GetMessageSerializer().SerializeObject(dictionary);
					lastSearchResultsAds = null;
					SignalRClient.Instance.RequestCommand("SearchGroups", arg, OnSearchGuilds, waitForResponse: false);
				}
				SignalRClient.Instance.StartCoroutine(SendCombinedSearchResults());
			}
			else
			{
				if (keyword.IndexOf('*') > -1)
				{
					Debug.LogWarning("Query string includes bad character \"*\"");
					this.guildSearchFailedEvent("Query string includes bad character \"*\"");
					return false;
				}
				string arg2 = modelManager.GetMessageSerializer().SerializeObject(dictionary);
				lastSearchResultsKeyword = null;
				SignalRClient.Instance.RequestCommand("SearchGroups", arg2, OnSearchGuilds, waitForResponse: false);
				SignalRClient.Instance.StartCoroutine(SendKeywordSearchResults());
			}
		}
		return true;
	}

	private bool HasSearchTimeouted()
	{
		if (lastSearchStartTS > 0)
		{
			return GameManager.Instance.playerModel.UtcTimeStamp - lastSearchStartTS > 30000;
		}
		return false;
	}

	private void AddRandomBucketParameters(Dictionary<string, string> parameters)
	{
		string text = UnityEngine.Random.Range(0, 4095).ToString("x3");
		parameters["idBucket16"] = text.Substring(0, 1);
		parameters["idBucket256"] = text.Substring(0, 2);
		parameters["idBucket4096"] = text;
	}

	private void SendTimeoutEvent(GuildSearchInfo.SearchType searchType)
	{
		GuildSearchEventData guildSearchEventData = new GuildSearchEventData(searchType, lastSearchTerm, modelManager.Player);
		guildSearchEventData.SetSearchDuration(lastSearchStartTS);
		lastSearchStartTS = 0L;
		Helpers.ExecuteCommand(guildSearchEventData.ToSendMetricCommand());
	}

	private void SendSuggestGuildEvent(GuildModel guild)
	{
		if (suggestEventData != null)
		{
			suggestEventData.SetSearchDuration(lastSearchStartTS);
			lastSearchStartTS = 0L;
			Helpers.ExecuteCommand(suggestEventData.ToSendMetricCommand());
			suggestEventData = null;
		}
	}

	protected IEnumerator SendCachedSearchResults()
	{
		yield return null;
		if (this.guildSearchFinishedEvent != null)
		{
			this.guildSearchFinishedEvent(cachedDefaultSearchResults);
		}
	}

	protected IEnumerator SendCombinedSearchResults()
	{
		while (lastSearchResultsLevel == null || lastSearchResultsCountry == null || lastSearchResultsNew == null || (GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled && lastSearchResultsAds == null))
		{
			if (HasSearchTimeouted())
			{
				SendTimeoutEvent(GuildSearchInfo.SearchType.Suggestions);
				yield break;
			}
			yield return null;
		}
		GuildSearchEventData eventData = new GuildSearchEventData(GuildSearchInfo.SearchType.Suggestions, lastSearchTerm, modelManager.Player);
		List<GuildSearchResult> combinedSearchResults = CombineLastSearchGuildsResponses(eventData);
		if (combinedSearchResults.Count < 15)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["keyword"] = lastSearchTerm;
			AddRandomBucketParameters(dictionary);
			dictionary["queryType"] = "suggestionsfallback";
			string arg = modelManager.GetMessageSerializer().SerializeObject(dictionary);
			lastSearchResultsFallback = null;
			SignalRClient.Instance.RequestCommand("SearchGroups", arg, OnSearchGuilds, waitForResponse: false);
			while (lastSearchResultsFallback == null)
			{
				if (HasSearchTimeouted())
				{
					SendTimeoutEvent(GuildSearchInfo.SearchType.Suggestions);
					yield break;
				}
				yield return null;
			}
			List<string> list = SelectRandomSample(15 - combinedSearchResults.Count, lastSearchResultsFallback);
			foreach (string item in list)
			{
				combinedSearchResults.Add(new GuildSearchResult(item, GuildSearchResult.Source.Fallback));
			}
			eventData.SetGuildCounts(GuildSearchResult.Source.Fallback, lastSearchResultsFallback.Count, list.Count);
		}
		if (GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled)
		{
			AddAds(combinedSearchResults, eventData);
		}
		_ = eventData.Info.GuildCountsQueried;
		DeserializeResultModels(combinedSearchResults);
		SetSearchResultsOrder(combinedSearchResults);
		eventData.FillSelected(combinedSearchResults);
		List<GuildModel> list2 = new List<GuildModel>();
		foreach (GuildSearchResult item2 in combinedSearchResults)
		{
			list2.Add(item2.model);
		}
		if (this.guildSearchFinishedEvent != null)
		{
			this.guildSearchFinishedEvent(list2);
		}
		if (cachedDefaultSearchResults == null && string.IsNullOrEmpty(lastSearchTerm) && list2.Count > 0)
		{
			cachedDefaultSearchResults = list2;
		}
		eventData.SetSearchDuration(lastSearchStartTS);
		lastSearchStartTS = 0L;
		lastSearchModels = list2;
		lastEventSearchId = eventData.Info.SearchId;
		Helpers.ExecuteCommand(eventData.ToSendMetricCommand());
		lastSearchResultsLevel = null;
		lastSearchResultsCountry = null;
		lastSearchResultsNew = null;
		lastSearchResultsFallback = null;
		lastSearchResultsAds = null;
	}

	protected IEnumerator SendKeywordSearchResults()
	{
		while (lastSearchResultsKeyword == null)
		{
			if (HasSearchTimeouted())
			{
				SendTimeoutEvent(GuildSearchInfo.SearchType.KeywordSearch);
				yield break;
			}
			yield return null;
		}
		GuildSearchEventData guildSearchEventData = new GuildSearchEventData(GuildSearchInfo.SearchType.KeywordSearch, lastSearchTerm, modelManager.Player);
		List<string> list = SelectRandomSample(15, lastSearchResultsKeyword);
		List<GuildSearchResult> list2 = new List<GuildSearchResult>();
		foreach (string item in list)
		{
			list2.Add(new GuildSearchResult(item, GuildSearchResult.Source.Keyword));
		}
		guildSearchEventData.SetGuildCounts(GuildSearchResult.Source.Keyword, lastSearchResultsKeyword.Count, list.Count);
		DeserializeResultModels(list2);
		SetSearchResultsOrder(list2);
		guildSearchEventData.FillSelected(list2);
		List<GuildModel> list3 = new List<GuildModel>();
		foreach (GuildSearchResult item2 in list2)
		{
			list3.Add(item2.model);
		}
		if (this.guildSearchFinishedEvent != null)
		{
			this.guildSearchFinishedEvent(list3);
		}
		guildSearchEventData.SetSearchDuration(lastSearchStartTS);
		lastSearchStartTS = 0L;
		lastSearchModels = list3;
		lastEventSearchId = guildSearchEventData.Info.SearchId;
		Helpers.ExecuteCommand(guildSearchEventData.ToSendMetricCommand());
		lastSearchResultsKeyword = null;
	}

	private List<string> SelectRandomSample(int count, List<string> aList)
	{
		List<string> list = new List<string>();
		if (aList == null || aList.Count == 0)
		{
			return list;
		}
		List<string> list2 = new List<string>(aList);
		for (int i = 0; i < count; i++)
		{
			if (list2.Count <= 0)
			{
				break;
			}
			int index = UnityEngine.Random.Range(0, list2.Count - 1);
			list.Add(list2[index]);
			list2.RemoveAt(index);
		}
		return list;
	}

	private List<GuildSearchResult> CombineLastSearchGuildsResponses(GuildSearchEventData eventData)
	{
		List<GuildSearchResult> list = new List<GuildSearchResult>();
		List<string> list2 = SelectRandomSample(5, lastSearchResultsNew);
		foreach (string item in list2)
		{
			list.Add(new GuildSearchResult(item, GuildSearchResult.Source.New));
		}
		eventData.SetGuildCounts(GuildSearchResult.Source.New, lastSearchResultsNew.Count, list2.Count);
		list2 = SelectRandomSample(5, lastSearchResultsCountry);
		foreach (string item2 in list2)
		{
			list.Add(new GuildSearchResult(item2, GuildSearchResult.Source.SameCountry));
		}
		eventData.SetGuildCounts(GuildSearchResult.Source.SameCountry, lastSearchResultsCountry.Count, list2.Count);
		list2 = SelectRandomSample(15 - list.Count, lastSearchResultsLevel);
		foreach (string item3 in list2)
		{
			list.Add(new GuildSearchResult(item3, GuildSearchResult.Source.NearLevel));
		}
		eventData.SetGuildCounts(GuildSearchResult.Source.NearLevel, lastSearchResultsLevel.Count, list2.Count);
		return list;
	}

	private void AddAds(List<GuildSearchResult> results, GuildSearchEventData eventData)
	{
		if (results == null || lastSearchResultsAds == null || lastSearchResultsAds.Count == 0)
		{
			eventData.SetGuildCounts(GuildSearchResult.Source.Ad, 0, 0);
			return;
		}
		if (lastSearchResultsAds.Count == 0)
		{
			if (string.IsNullOrEmpty(lastSearchTerm))
			{
				Debug.LogWarning("No ad results in default group search");
			}
		}
		else
		{
			int index = UnityEngine.Random.Range(0, lastSearchResultsAds.Count - 1);
			if (lastSearchResultsAds[index] == null)
			{
				Debug.LogWarning("Invalid entry in SearchGroup response");
			}
			else
			{
				string modelJson = lastSearchResultsAds[index];
				results.Add(new GuildSearchResult(modelJson, GuildSearchResult.Source.Ad));
			}
		}
		eventData.SetGuildCounts(GuildSearchResult.Source.Ad, lastSearchResultsAds.Count, 1);
	}

	private void SetSearchResultsOrder(List<GuildSearchResult> results)
	{
		Helpers.RandomShuffle(results);
		if (!GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled)
		{
			return;
		}
		results.StableSort(delegate(GuildSearchResult a, GuildSearchResult b)
		{
			GuildModel model = a.model;
			GuildModel model2 = b.model;
			if (a == null || b == null || model == null || model2 == null)
			{
				return 0;
			}
			bool flag = model.AdAvailableTimeSeconds > 0 && modelManager.Player != null && modelManager.Player.UtcTimeStamp / 1000 < model.AdExpireTimeStampSeconds;
			bool flag2 = model2.AdAvailableTimeSeconds > 0 && modelManager.Player != null && modelManager.Player.UtcTimeStamp / 1000 < model2.AdExpireTimeStampSeconds;
			if (flag && !flag2)
			{
				return 1;
			}
			if (!flag && flag2)
			{
				return -1;
			}
			return (flag && flag2) ? ((model.AdExpireTimeStampSeconds < model2.AdExpireTimeStampSeconds) ? 1 : (-1)) : 0;
		});
	}

	private void DeserializeResultModels(List<GuildSearchResult> results)
	{
		IMessageSerializer messageSerializer = modelManager.GetMessageSerializer();
		foreach (GuildSearchResult result in results)
		{
			result.DeserializeModel(messageSerializer);
		}
	}

	private void SetSuggestGuildIterationResults(GuildSearchEventData eventData, int queryIteration, int queriedCount, GuildModel guild)
	{
		int num = queryIteration - 1;
		GuildSearchResult.Source source = GuildSearchResult.Source.Undefined;
		if (Enum.IsDefined(typeof(GuildSearchResult.Source), num))
		{
			source = (GuildSearchResult.Source)num;
		}
		int selectedCount = 0;
		if (guild != null)
		{
			selectedCount = 1;
			List<GuildSearchResult> list = new List<GuildSearchResult>(1);
			GuildSearchResult guildSearchResult = new GuildSearchResult(null, source);
			guildSearchResult.model = guild;
			list.Add(guildSearchResult);
			suggestEventData.FillSelected(list);
		}
		if (queriedCount > 0)
		{
			eventData.SetGuildCounts(source, queriedCount, selectedCount);
		}
		else
		{
			eventData.SetGuildCounts(source, 0, selectedCount);
		}
	}

	protected void OnSuggestGuildResults(string queryType, int queryIteration, List<string> resultList)
	{
		if (resultList != null)
		{
			if (suggestEventData == null)
			{
				suggestEventData = new GuildSearchEventData(GuildSearchInfo.SearchType.SuggestionPopup, null, modelManager.Player);
			}
			if (resultList.Count > 0)
			{
				IMessageSerializer messageSerializer = modelManager.GetMessageSerializer();
				string value = SelectRandomSample(1, resultList)[0];
				GuildModel guildModel = messageSerializer.DeserializeObject<GuildModel>(value);
				lastSuggestModelId = guildModel.Id;
				lastEventSearchId = suggestEventData.Info.SearchId;
				if (this.guildSuggestFinishedEvent != null)
				{
					this.guildSuggestFinishedEvent(guildModel);
				}
				SetSuggestGuildIterationResults(suggestEventData, queryIteration, resultList.Count, guildModel);
				SendSuggestGuildEvent(guildModel);
				return;
			}
			SetSuggestGuildIterationResults(suggestEventData, queryIteration, resultList.Count, null);
			int guildSuggestionPopupQueryIterationsMax = modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupQueryIterationsMax;
			if (queryIteration < guildSuggestionPopupQueryIterationsMax)
			{
				if (!SuggestGuild(queryIteration + 1))
				{
					if (this.guildSearchFailedEvent != null)
					{
						this.guildSearchFailedEvent("Failed to start SuggestGuild query for iteration " + (queryIteration + 1));
					}
					SendSuggestGuildEvent(null);
				}
			}
			else
			{
				if (this.guildSearchFailedEvent != null)
				{
					this.guildSearchFailedEvent("No results for SuggestGuild query after max iteration " + queryIteration);
				}
				SendSuggestGuildEvent(null);
			}
		}
		else
		{
			if (this.guildSuggestFinishedEvent != null)
			{
				this.guildSuggestFinishedEvent(null);
			}
			SendSuggestGuildEvent(null);
		}
	}

	protected void OnSearchGuilds(string searchResponseJson)
	{
		List<string> list = new List<string>();
		string text = null;
		string text2 = null;
		int queryIteration = 0;
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(searchResponseJson))
		{
			if (this.guildSearchFailedEvent != null)
			{
				this.guildSearchFailedEvent(SignalRClient.Instance.LastErrorMessage);
			}
		}
		else
		{
			IMessageSerializer messageSerializer = modelManager.GetMessageSerializer();
			GroupSearchResult groupSearchResult = messageSerializer.DeserializeObject<GroupSearchResult>(searchResponseJson);
			if (groupSearchResult == null || groupSearchResult.Models == null)
			{
				Debug.LogWarning("Invalid SearchGroup response");
				if (this.guildSearchFailedEvent != null)
				{
					this.guildSearchFailedEvent("Invalid data");
				}
				return;
			}
			if (string.IsNullOrEmpty(groupSearchResult.Parameters))
			{
				Debug.LogWarning("Missing search parameters in SearchGroup response");
				this.guildSearchFailedEvent("Missing search parameters");
				return;
			}
			Dictionary<string, string> dictionary = messageSerializer.DeserializeObject<Dictionary<string, string>>(groupSearchResult.Parameters);
			if (dictionary == null && dictionary.Count == 0)
			{
				Debug.LogWarning("Missing search parameters in SearchGroup response");
				this.guildSearchFailedEvent("Missing search parameters");
				return;
			}
			if (dictionary.ContainsKey("queryType"))
			{
				text = dictionary["queryType"];
			}
			if (dictionary.ContainsKey("queryIteration"))
			{
				string text3 = dictionary["queryIteration"];
				try
				{
					queryIteration = int.Parse(text3);
				}
				catch (FormatException ex)
				{
					Debug.LogWarning("Invalid value for queryIteration '" + text3 + "', int parse error: " + ex.Message);
					queryIteration = modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupQueryIterationsMax + 1;
				}
			}
			if (dictionary.ContainsKey("keyword"))
			{
				text2 = dictionary["keyword"];
			}
			string[] models = groupSearchResult.Models;
			foreach (string text4 in models)
			{
				if (text4 == null)
				{
					Debug.LogWarning("Invalid entry in SearchGroup response, queryType = " + text + ", keyword = " + text2);
				}
				else
				{
					list.Add(text4);
				}
			}
		}
		switch (text)
		{
		case "suggestionsnew":
			lastSearchResultsNew = list;
			return;
		case "suggestionssamecountry":
			lastSearchResultsCountry = list;
			return;
		case "suggestionssimilarlevel":
			lastSearchResultsLevel = list;
			return;
		case "suggestionsfallback":
			lastSearchResultsFallback = list;
			return;
		case "ads":
			lastSearchResultsAds = list;
			return;
		case "suggestion":
			OnSuggestGuildResults(text, queryIteration, list);
			return;
		}
		if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			lastSearchResultsKeyword = list;
		}
		else
		{
			Debug.LogWarning("Unknown SearchGroup response, queryType = " + text + ", keyword = " + text2);
		}
	}

	private int GetLastSearchPositionForGuild(string guildId)
	{
		if (lastSearchModels != null)
		{
			for (int i = 0; i < lastSearchModels.Count; i++)
			{
				if (lastSearchModels[i].Id == guildId)
				{
					return i;
				}
			}
		}
		if (lastSuggestModelId == guildId)
		{
			return 0;
		}
		return -1;
	}

	public bool JoinGuild(string guildId)
	{
		if (!GameManager.Instance.IsConnectedToServer)
		{
			return false;
		}
		pendingJoinGuildId = null;
		if (Model != null)
		{
			pendingJoinGuildId = guildId;
			LeaveGuild(modelManager.Player.GuildId);
			return true;
		}
		JoinGuildCommand joinGuildCommand = new JoinGuildCommand();
		joinGuildCommand.GuildId = guildId;
		joinGuildCommand.JoinerName = modelManager.Player.Name;
		joinGuildCommand.JoinerLevel = modelManager.Player.Level;
		joinGuildCommand.JoinerPlayerEmblem = modelManager.Player.PlayerEmblem;
		joinGuildCommand.AutomaticallyJoinIfOpen = true;
		joinGuildCommand.totalVpPoints = modelManager.Player.CalculateLifeTimeGvGVpAccumulated();
		int lastSearchPositionForGuild = GetLastSearchPositionForGuild(guildId);
		if (lastSearchPositionForGuild >= 0)
		{
			joinGuildCommand.SearchId = lastEventSearchId;
			joinGuildCommand.SearchPosition = lastSearchPositionForGuild;
		}
		Helpers.ExecuteCommand(joinGuildCommand);
		return true;
	}

	protected void OnJoinGuild(string groupsRespondJson)
	{
	}

	public bool CheckCanLeaveGuild()
	{
		if (Model != null && Model.GetLeaderMemberInfo().MemberId == GameManager.Instance.playerModel.HashedId && Model.NumberMembers > 1)
		{
			return false;
		}
		return true;
	}

	public void LeaveGuild(string guildId = null)
	{
		LeaveGuildCommand leaveGuildCommand = new LeaveGuildCommand();
		if (guildId != null)
		{
			leaveGuildCommand.GuildId = guildId;
		}
		else
		{
			leaveGuildCommand.GuildId = modelManager.Player.GuildId;
		}
		leaveGuildCommand.LeaverId = modelManager.Player.HashedId;
		leaveGuildCommand.LeaveType = GuildLeaveType.MemberLeave;
		Helpers.ExecuteCommand(leaveGuildCommand);
	}

	private void ForceLeaveGuild(string guildId = null)
	{
		LeftGuildCommand leftGuildCommand = new LeftGuildCommand();
		if (guildId != null)
		{
			leftGuildCommand.GuildId = guildId;
		}
		else
		{
			leftGuildCommand.GuildId = modelManager.Player.GuildId;
		}
		leftGuildCommand.LeaverId = modelManager.Player.HashedId;
		leftGuildCommand.LeaveType = GuildLeaveType.MemberLeave;
		Helpers.ExecuteCommand(leftGuildCommand);
	}

	public void ModifyGuild(string description, GuildJoinType joinType, string purpose)
	{
		Helpers.ExecuteCommand(new ModifyGuildCommand
		{
			Description = description,
			JoinType = joinType,
			Purpose = purpose
		});
	}

	public void ModifyGuildMemberRole(string memberId, GuildMemberRole newRole)
	{
		Helpers.ExecuteCommand(new ModifyMemberRoleCommand(memberId)
		{
			NewRole = newRole
		});
	}

	public void PromoteMemberToLeader(string memberId)
	{
		Helpers.ExecuteCommand(new PromoteMemberToLeaderCommand(memberId));
	}

	public void AcceptGuildMember(string memberId)
	{
		Helpers.ExecuteCommand(new AcceptMembershipCommand(memberId));
	}

	public void RefuseGuildMember(string memberId)
	{
		Helpers.ExecuteCommand(new LeaveGuildCommand
		{
			GuildId = modelManager.Player.GuildId,
			LeaverId = memberId,
			LeaveType = GuildLeaveType.RejectRequest
		});
	}

	public void KickOutGuildMember(string memberId, bool softBan)
	{
		Helpers.ExecuteCommand(new LeaveGuildCommand
		{
			GuildId = modelManager.Player.GuildId,
			LeaverId = memberId,
			LeaveType = (softBan ? GuildLeaveType.KickAndSoftBan : GuildLeaveType.Kick)
		});
	}

	public void SendChatMessage(string message)
	{
		if (!string.IsNullOrEmpty(message))
		{
			Helpers.ExecuteCommand(new ChatMessageCommand
			{
				Message = message
			});
		}
	}

	private void SetPlayerGuild(string guildId)
	{
		Helpers.ExecuteCommand(new SetGuildCommand(modelManager.Player)
		{
			GuildId = guildId
		});
	}

	private void NotifySocialMessageReceived(SocialMessageType type, string memberInfo)
	{
		this.OnSocialMessageReceived?.Invoke(type, memberInfo);
	}

	private void Log(string s)
	{
	}

	public void SendOnLoadGroupCompleted(bool success)
	{
		if (this.OnLoadGroupCompleted != null)
		{
			this.OnLoadGroupCompleted(success);
		}
	}

	public static bool CanCreateGuildMapIndicator(out long milliSecondsLeft)
	{
		milliSecondsLeft = 0L;
		if (!GameManager.Instance.playerModel.IsGuildMember)
		{
			return false;
		}
		return GameManager.Instance.playerModel.GuildWarModel.CurrentBattle?.CanPlayerAddNewMapNotification(GameManager.Instance.playerModel.HashedId, GameManager.Instance.playerModel.GuildModel.TimeStamp, out milliSecondsLeft) ?? false;
	}

	public static GuildBattleModel.GuildBattleIndicatorData CreateAndSendGuildMapIndicator(int sectorId, Vector2 position)
	{
		if (!CanCreateGuildMapIndicator(out var _))
		{
			return null;
		}
		if (Helpers.ExecuteCommand(new GuildBattleCreateIndicatorCommand(new GuildBattleModel.GuildBattleIndicatorData(sectorId, (int)position.x, (int)position.y))) == TWDModelResult.OK)
		{
			return new GuildBattleModel.GuildBattleIndicatorData(sectorId, (int)position.x, (int)position.y)
			{
				PlayerHashedId = GameManager.Instance.playerModel.HashedId,
				UtcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp
			};
		}
		return null;
	}

	private void CheckForUnSeenRemovedBattleSlots()
	{
		if (GuildWarHelper.HasUnseenBattleSlotRemoves())
		{
			modelManager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.GuildRemovedFromBattleSlot, 0, LocalizationManager.GetText("GvG.Alert.RemovedFromBattle.Title"), 0);
		}
	}

	public static void ShowGuildJoinResultMessage(bool immediateJoin, bool banned)
	{
		HUDNotification.Info(LocalizationManager.GetText((!immediateJoin) ? "Popup.Guild.MemberRequestSent" : (banned ? "Popup.Guild.JoinFailedBanned" : "Popup.Guild.OpenGuildJoined")));
	}

	public static async Task<GuildModel> GetGuild(string guildId)
	{
		TaskCompletionSource<GuildModel> completion = new TaskCompletionSource<GuildModel>();
		SignalRClient.Instance.RequestCommand("GetGroupInfo", guildId, delegate(string message)
		{
			if (string.IsNullOrEmpty(message) || message == "null")
			{
				completion.SetResult(null);
			}
			else
			{
				completion.SetResult(GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<GuildModel>(message));
			}
		}, waitForResponse: true);
		return await completion.Task;
	}
}
