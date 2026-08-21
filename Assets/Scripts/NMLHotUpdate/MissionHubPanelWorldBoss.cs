using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class MissionHubPanelWorldBoss : MissionHubGameModePanel
{
	public enum WorldBossState
	{
		None = 0,
		SeasonOpenAndCycleOpenNotSigned = 1,
		SeasonOpenAndCycleOpen = 2,
		SeasonOpenAndCycleClosedNotSignedSelfSigned = 3,
		SeasonOpenAndCycleClosedSigned = 4,
		SeasonOpenAndCycleClosedNotSigned = 5,
		SeasonOpenAndCycleClosedUnregistered = 6,
		LockedByCouncilLevel = 7,
		NotInGuild = 8
	}

	[SerializeField]
	public GameObject ActiveFx;

	[SerializeField]
	public UILabel middleLabel;

	[SerializeField]
	public UILabel bottomLabel;

	[SerializeField]
	public UIButton signButton;

	[SerializeField]
	public GameObject atwarFlag;

	[SerializeField]
	public GameObject bg;

	[SerializeField]
	private GameObject NoSignGM;

	private const string SystemId = "SystemBase.WorldBoss";

	private const int UnlockCouncilLevelFallback = 14;

	private const string TitleLocalizationKeyFallback = "Popup.MissionHub.WorldBoss.Title";

	private WorldBossModelManager worldBossModelManager;

	private WorldBossState currentWorldBossState;

	private bool hasRequestedCycleBoundaryRefresh;

	private bool hasReceivedInitialSnapshot;

	private bool hasTrackedCycleOpen;

	private bool wasCycleOpen;

	private readonly WorldBossBaseSnapshotHelper worldBossBaseSnapshotHelper = new WorldBossBaseSnapshotHelper();

	public override void Awake()
	{
		base.Awake();
		DebugIdString = "MissionHubPanelWorldBoss";
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossBaseSnapshotMessage -= HandleOnWorldBossBaseSnapshotMessage;
			SignalRClient.Instance.OnWorldBossBaseSnapshotMessage += HandleOnWorldBossBaseSnapshotMessage;
		}
	}

	private void OnDestroy()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossBaseSnapshotMessage -= HandleOnWorldBossBaseSnapshotMessage;
		}
	}

	private void HandleOnWorldBossBaseSnapshotMessage(string message, string type)
	{
		if (IsPanelAlive())
		{
			UpdateUI();
		}
	}

	private void OnWorldBossBaseSnapshotAsync(string responseJson)
	{
		hasReceivedInitialSnapshot = true;
		if (!IsPanelAlive())
		{
			return;
		}
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(responseJson))
		{
			SignalRClient.Instance.ClearError();
			return;
		}
		WorldBossGuildBaseSnapshot snapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildBaseSnapshot>(responseJson);
		worldBossBaseSnapshotHelper.SetSnapshot(snapshot);
		WorldBossCycleSettlementSnapshot latestPendingSettlement = worldBossBaseSnapshotHelper.GetLatestPendingSettlement(worldBossModelManager);
		if (latestPendingSettlement != null)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossEndPopup) as WorldBossEndPopup)?.OpenForSettlement(latestPendingSettlement);
		}
		else if (worldBossModelManager.GetCurrentCycleId() > 0 && worldBossModelManager.ShouldShowOpeningPopup(worldBossBaseSnapshotHelper.GuildBaseState))
		{
			WorldBossOverviewPopup.OpenPopup();
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand(worldBossModelManager.GetOpeningPopupSeenToggleKey()));
		}
		UpdateWorldBossBaseSnapshot();
	}

	public void UpdateWorldBossBaseSnapshot()
	{
		WorldBossGuildBaseState guildBaseState = worldBossBaseSnapshotHelper.GuildBaseState;
		if (!IsPanelAlive() || guildBaseState == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(ActiveFx, value: false);
		Helpers.GameObjectSetActive(bottomLabel, value: false);
		Helpers.GameObjectSetActive(signButton, value: false);
		Helpers.GameObjectSetActive(atwarFlag, value: false);
		Helpers.GameObjectSetActive(NoSignGM, value: false);
		Helpers.GameObjectSetActive(middleLabel, value: true);
		middleLabel.text = "";
		if (worldBossModelManager.GetCurrentSeason() != null)
		{
			HelpersUI.SetContentToLabel(middleLabel, LocalizationManager.GetText(worldBossModelManager.GetCurrentSeason().SeasonTitle));
		}
		_ = worldBossModelManager.GetCurrentSeason().SeasonPic;
		if (worldBossModelManager.IsSeasonOpen() && worldBossModelManager.IsCycleOpen())
		{
			timeLabelLocalisation = "World.Boss.Countdown";
			gameModeTimeLeft = worldBossModelManager.GetTimeUntilCycleEndMs();
			if (guildBaseState.Status == WorldBossCycleStatus.None || guildBaseState.Status == WorldBossCycleStatus.SigningUp)
			{
				Helpers.GameObjectSetActive(NoSignGM, value: true);
				currentWorldBossState = WorldBossState.SeasonOpenAndCycleOpenNotSigned;
			}
			else
			{
				Helpers.GameObjectSetActive(NoSignGM, value: false);
				Helpers.GameObjectSetActive(atwarFlag, value: true);
				Helpers.GameObjectSetActive(bottomLabel, value: true);
				bottomLabel.text = LocalizationManager.GetText("WorldBoss.TheWarisOn");
				HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText(GetTitleLocalizationKey()));
				currentWorldBossState = WorldBossState.SeasonOpenAndCycleOpen;
				Helpers.GameObjectSetActive(ActiveFx, worldBossModelManager.ShouldShowGoldLight());
			}
		}
		else if (worldBossModelManager.IsOffSeason() && worldBossModelManager.IsCurrentCycleSignUpOpen() && (guildBaseState.Status == WorldBossCycleStatus.None || guildBaseState.Status == WorldBossCycleStatus.SigningUp))
		{
			timeLabelLocalisation = "World.Boss.Countdown";
			gameModeTimeLeft = worldBossModelManager.GetTimeUntilSignUpDeadlineMs();
			Debug.LogError("报名窗口开放");
			if (IsPlayerSignedUp(guildBaseState))
			{
				Debug.LogError("报名窗口开放自己已经报名");
				ShowSignedUpBottomUi();
				currentWorldBossState = WorldBossState.SeasonOpenAndCycleClosedNotSignedSelfSigned;
			}
			else
			{
				Debug.LogError("报名窗口开放自己未报名");
				ShowSignButtonBottomUi();
				currentWorldBossState = WorldBossState.SeasonOpenAndCycleClosedNotSigned;
			}
		}
		else if (worldBossModelManager.IsCurrentCycleDifficultySelectionOpen() && (guildBaseState.Status == WorldBossCycleStatus.DifficultySelected || guildBaseState.Status == WorldBossCycleStatus.SignedUp))
		{
			timeLabelLocalisation = "World.Boss.NextCycle.Countdown";
			gameModeTimeLeft = worldBossModelManager.GetTimeUntilNextCycleStartMs();
			Debug.LogError("休赛期难度选择窗口开放已报名");
			Helpers.GameObjectSetActive(bottomLabel, value: true);
			bottomLabel.text = LocalizationManager.GetText("WorldBoss.GetReadyfornextwar");
			currentWorldBossState = WorldBossState.SeasonOpenAndCycleClosedSigned;
		}
		else if (worldBossModelManager.IsOffSeason() && (guildBaseState.Status == WorldBossCycleStatus.DifficultySelected || guildBaseState.Status == WorldBossCycleStatus.SignedUp))
		{
			timeLabelLocalisation = "World.Boss.NextCycle.Countdown";
			gameModeTimeLeft = worldBossModelManager.GetTimeUntilNextCycleStartMs();
			Debug.LogError("休赛期报名、选择窗口开关已报名");
			Helpers.GameObjectSetActive(bottomLabel, value: true);
			bottomLabel.text = LocalizationManager.GetText("WorldBoss.GetReadyfornextwar");
			currentWorldBossState = WorldBossState.SeasonOpenAndCycleClosedSigned;
		}
		else if (worldBossModelManager.IsOffSeason() && (guildBaseState.Status == WorldBossCycleStatus.None || guildBaseState.Status == WorldBossCycleStatus.SigningUp))
		{
			timeLabelLocalisation = "World.Boss.NextCycle.Countdown";
			gameModeTimeLeft = worldBossModelManager.GetTimeUntilNextCycleStartMs();
			Helpers.GameObjectSetActive(NoSignGM, value: true);
			currentWorldBossState = WorldBossState.SeasonOpenAndCycleClosedUnregistered;
		}
		else
		{
			Debug.LogError("休赛期难度选择窗口未开放未报名窗口或者根本没有公会报名");
		}
		hasRequestedCycleBoundaryRefresh = false;
	}

	public override void Update()
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		if (worldBossModelManager != null && !base.isLocked)
		{
			if (worldBossModelManager.IsCycleOpen())
			{
				timeLabelLocalisation = "World.Boss.Countdown";
				gameModeTimeLeft = worldBossModelManager.GetTimeUntilCycleEndMs();
			}
			else if (worldBossModelManager.IsCurrentCycleSignUpOpen())
			{
				timeLabelLocalisation = "World.Boss.Countdown";
				gameModeTimeLeft = worldBossModelManager.GetTimeUntilSignUpDeadlineMs();
			}
			else
			{
				timeLabelLocalisation = "World.Boss.NextCycle.Countdown";
				gameModeTimeLeft = worldBossModelManager.GetTimeUntilNextCycleStartMs();
			}
		}
		if (timerLabel != null && !string.IsNullOrEmpty(timeLabelLocalisation) && gameModeTimeLeft > 0)
		{
			SetContentToTimerLabel(LocalizationManager.GetText(timeLabelLocalisation, MissionHubGameModePanel.FormatTimeLeft(gameModeTimeLeft)));
		}
		Helpers.GameObjectSetActive(timerGameobject, !base.isLocked && gameModeTimeLeft > 0);
		if (signButton != null && signButton.gameObject.activeSelf && (worldBossModelManager == null || !worldBossModelManager.IsCurrentCycleSignUpOpen()))
		{
			Helpers.GameObjectSetActive(signButton, value: false);
		}
		RefreshWhenCycleOpenStateChanged();
		RefreshWhenCycleBoundaryReached();
	}

	private void RefreshWhenCycleOpenStateChanged()
	{
		if (hasReceivedInitialSnapshot && worldBossModelManager != null && !base.isLocked)
		{
			bool flag = worldBossModelManager.IsCycleOpen();
			if (!hasTrackedCycleOpen)
			{
				hasTrackedCycleOpen = true;
				wasCycleOpen = flag;
			}
			else if (wasCycleOpen != flag)
			{
				wasCycleOpen = flag;
				hasRequestedCycleBoundaryRefresh = true;
				RefreshWorldBossStateAfterCycleChange();
			}
		}
	}

	private void RefreshWhenCycleBoundaryReached()
	{
		if (!hasRequestedCycleBoundaryRefresh && worldBossModelManager != null && !string.IsNullOrEmpty(timeLabelLocalisation) && gameModeTimeLeft <= 0)
		{
			hasRequestedCycleBoundaryRefresh = true;
			RefreshWorldBossStateAfterCycleChange();
		}
	}

	private void RefreshWorldBossStateAfterCycleChange()
	{
		if (worldBossBaseSnapshotHelper.GuildBaseState != null)
		{
			UpdateWorldBossBaseSnapshot();
		}
		GetWorldBossBaseSnapshot();
	}

	public override void UpdateUI()
	{
		currentWorldBossState = WorldBossState.None;
		base.UpdateUI();
		worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
		Helpers.GameObjectSetActive(ActiveFx, value: false);
		Helpers.GameObjectSetActive(middleLabel, value: false);
		Helpers.GameObjectSetActive(bottomLabel, value: false);
		Helpers.GameObjectSetActive(signButton, value: false);
		Helpers.GameObjectSetActive(timerGameobject, !base.isLocked);
		Helpers.GameObjectSetActive(atwarFlag, value: false);
		Helpers.GameObjectSetActive(NoSignGM, value: false);
		HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("Popup.MissionHub.WorldBoss.Title"));
		if (worldBossModelManager == null)
		{
			hasReceivedInitialSnapshot = true;
		}
		else if (worldBossModelManager.GetUnlockState() == WorldBossUnlockState.LevelNotReached)
		{
			hasReceivedInitialSnapshot = true;
			currentWorldBossState = WorldBossState.LockedByCouncilLevel;
			UpdateLockedState(locked: true);
			HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.SurvivalDifficulty.HardSurvivalUnlockAtLevel{CouncilLevel}", GetUnlockCouncilLevel()));
		}
		else if (worldBossModelManager.GetUnlockState() == WorldBossUnlockState.NotInGuild)
		{
			hasReceivedInitialSnapshot = true;
			currentWorldBossState = WorldBossState.NotInGuild;
			UpdateLockedState(locked: true);
			HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.GuildWarJoinGuild"));
		}
		else
		{
			UpdateLockedState(locked: false);
			GetWorldBossBaseSnapshot();
		}
	}

	public void GetWorldBossBaseSnapshot()
	{
		WorldBossGetSnapshotRequest worldBossGetSnapshotRequest = null;
		worldBossGetSnapshotRequest = ((!worldBossModelManager.IsOffSeason()) ? new WorldBossGetSnapshotRequest
		{
			GroupId = GameManager.Instance.playerModel.GuildId,
			SeasonId = worldBossModelManager.GetCurrentSeasonId(),
			CycleId = worldBossModelManager.GetCurrentCycleId()
		} : new WorldBossGetSnapshotRequest
		{
			GroupId = GameManager.Instance.playerModel.GuildId,
			SeasonId = worldBossModelManager.GetCurrentSeasonId(),
			CycleId = worldBossModelManager.GetNextCycleId()
		});
		if (worldBossModelManager.TryGetLatestUnshownSettlementTarget(out var seasonId, out var cycleId))
		{
			worldBossGetSnapshotRequest.SettlementSeasonId = seasonId;
			worldBossGetSnapshotRequest.SettlementCycleId = cycleId;
		}
		string arg = GameManager.Instance.jsonSerializer.Serialize(worldBossGetSnapshotRequest);
		SignalRClient.Instance.RequestCommand("WorldBossBaseSnapshot", arg, OnWorldBossBaseSnapshotAsync, waitForResponse: true);
	}

	public void OnSignButtonClicked()
	{
		if (hasReceivedInitialSnapshot)
		{
			if (worldBossModelManager == null || !worldBossModelManager.IsCurrentCycleSignUpOpen())
			{
				HUDNotification.Info(LocalizationManager.GetText("World.Boss.SignInEnd.Tips"));
				Helpers.GameObjectSetActive(signButton, value: false);
			}
			else if (currentWorldBossState == WorldBossState.SeasonOpenAndCycleClosedNotSigned && Helpers.ExecuteCommand(new WorldBossSignUpCycleCommand(worldBossModelManager.GetCurrentSeasonId(), worldBossModelManager.GetNextCycleId())) == TWDModelResult.OK)
			{
				HUDNotification.Info(LocalizationManager.GetText("WorldBoss.SignInScceed"));
				currentWorldBossState = WorldBossState.SeasonOpenAndCycleClosedNotSignedSelfSigned;
				ShowSignedUpBottomUi();
			}
		}
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		if (!hasReceivedInitialSnapshot)
		{
			return;
		}
		if (currentWorldBossState == WorldBossState.LockedByCouncilLevel)
		{
			WorldBossOverviewPopup.OpenPopup(WorldBossOverviewPopup.OpenReason.LockedByCouncilLevel);
		}
		else if (currentWorldBossState == WorldBossState.NotInGuild)
		{
			WorldBossOverviewPopup.OpenPopup(WorldBossOverviewPopup.OpenReason.NotInGuild);
		}
		else if (currentWorldBossState == WorldBossState.SeasonOpenAndCycleOpenNotSigned || currentWorldBossState == WorldBossState.SeasonOpenAndCycleClosedUnregistered)
		{
			HUDNotification.Info(LocalizationManager.GetText("WorldBoss.Unregistered.Tips"));
		}
		else if (currentWorldBossState == WorldBossState.SeasonOpenAndCycleOpen)
		{
			if (Helpers.ExecuteCommand(new EnterWorldBossCommand(worldBossModelManager.GetCurrentSeasonId(), worldBossModelManager.GetCurrentCycleId())) == TWDModelResult.OK)
			{
				MissionHubNavigation.OpenWorldBoss();
			}
		}
		else if (currentWorldBossState == WorldBossState.SeasonOpenAndCycleClosedNotSignedSelfSigned || currentWorldBossState == WorldBossState.SeasonOpenAndCycleClosedNotSigned)
		{
			if (worldBossModelManager != null && worldBossModelManager.IsCurrentCycleSignUpOpen())
			{
				ShowGuildNeedSignTips();
			}
			else
			{
				HUDNotification.Info(LocalizationManager.GetText("World.Boss.SignInEnd.Tips"));
			}
		}
		else
		{
			WorldBossOffSeasonPopup.OpenPopup(currentWorldBossState);
		}
	}

	private static int GetUnlockCouncilLevel()
	{
		return (GameManager.Instance?.gameEconomyData?.GetSystemOpenById("SystemBase.WorldBoss"))?.OpenCampLv ?? 14;
	}

	private static string GetTitleLocalizationKey()
	{
		SystemOpen systemOpen = GameManager.Instance?.gameEconomyData?.GetSystemOpenById("SystemBase.WorldBoss");
		if (systemOpen == null)
		{
			return "Popup.MissionHub.WorldBoss.Title";
		}
		return systemOpen.SystemName;
	}

	private bool IsPanelAlive()
	{
		if (this != null)
		{
			return middleLabel != null;
		}
		return false;
	}

	private void ShowGuildNeedSignTips()
	{
		HUDNotification.Info(LocalizationManager.GetText("WorldBoss.GuildNeedSign.Tips", GetSignedUpMemberCount(), GetSignUpNumNeed()));
	}

	private void SetSignedNumLabel()
	{
		bottomLabel.text = LocalizationManager.GetText("WorldBoss.SignedNum", GetSignedUpMemberCount(), GetSignUpNumNeed());
	}

	private void ShowSignedUpBottomUi()
	{
		Helpers.GameObjectSetActive(signButton, value: false);
		Helpers.GameObjectSetActive(bottomLabel, value: true);
		SetSignedNumLabel();
	}

	private void ShowSignButtonBottomUi()
	{
		Helpers.GameObjectSetActive(bottomLabel, value: false);
		Helpers.GameObjectSetActive(signButton, value: true);
	}

	private bool IsPlayerSignedUp(WorldBossGuildBaseState guildBaseState)
	{
		if (guildBaseState?.SignedUpMemberIds != null)
		{
			return guildBaseState.SignedUpMemberIds.Contains(GameManager.Instance.playerModel.HashedId);
		}
		return false;
	}

	private int GetSignedUpMemberCount()
	{
		int num = (worldBossBaseSnapshotHelper.GuildBaseState?.SignedUpMemberIds)?.Count ?? 0;
		if (currentWorldBossState == WorldBossState.SeasonOpenAndCycleClosedNotSignedSelfSigned && !IsPlayerSignedUp(worldBossBaseSnapshotHelper.GuildBaseState))
		{
			num++;
		}
		return num;
	}

	private static int GetSignUpNumNeed()
	{
		return (GameManager.Instance?.gameEconomyData?.WorldBossConfig?.SignUpNumNeed).GetValueOrDefault();
	}
}
