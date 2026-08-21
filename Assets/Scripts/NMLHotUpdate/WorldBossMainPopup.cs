using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossMainPopup : HUDElement
{
	[SerializeField]
	private GameObject loadingContainer;

	[SerializeField]
	private GameObject leftContent;

	[SerializeField]
	private GameObject rightContent;

	[SerializeField]
	private GameObject middleContent;

	[SerializeField]
	private GameObject bottomContent;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private UILabel difficultyLabel;

	[SerializeField]
	private UISprite difficultyIcon;

	[SerializeField]
	private WorldBossCaptureData[] worldBossCaptures;

	[SerializeField]
	private UILabel difficultyLabel2;

	[SerializeField]
	private UILabel teamTitleLabel;

	[SerializeField]
	private GameObject team1Container;

	[SerializeField]
	private GameObject team1Active1Icon;

	[SerializeField]
	private GameObject team1Active2Icon;

	[SerializeField]
	private GameObject team2Container;

	[SerializeField]
	private GameObject team2DesGM;

	[SerializeField]
	private GameObject buff1Container;

	[SerializeField]
	private GameObject buff2Container;

	[SerializeField]
	private UILabel buffTitleLabel;

	[SerializeField]
	private GameObject teamStateContainer;

	[SerializeField]
	private GameObject blueFlag;

	[SerializeField]
	private UILabel blueScoreLabel;

	[SerializeField]
	private UILabel blueNameLabel;

	[SerializeField]
	private GameObject redFlag;

	[SerializeField]
	private UILabel redScoreLabel;

	[SerializeField]
	private UILabel redNameLabel;

	private WorldBossGuildFullSnapshot worldBossGuildFullSnapshot;

	private const string SystemId = "SystemBase.WorldBoss";

	private const string TitleLocalizationKeyFallback = "Popup.MissionHub.WorldBoss.Title";

	protected long gameModeTimeLeft = -1000L;

	private const long delayOnCompleteMillisec = -1000L;

	private WorldBossModelManager worldBossModelManager;

	protected string timeLabelLocalisation = "";

	private Dictionary<string, WorldBossCaptureData> _worldBossCaptures;

	private readonly long[] _dispatchedTeamOccupiedAtUtcMs = new long[2] { -1L, -1L };

	private const string HeroTokenNamePrefix = "HeroToken";

	private const int DurabilityBlockNum = 10;

	private const string DurabilityBlockNamePrefix = "Icon";

	private bool pendingDifficultyIconRefresh;

	private bool shouldShowLoadingOnRequest;

	private static readonly Color DurabilityColorGreen = Helpers.HexToColor("#a0c92f");

	private static readonly Color DurabilityColorYellow = Helpers.HexToColor("#f7c225");

	private static readonly Color DurabilityColorRed = Helpers.HexToColor("#fd3535");

	private static readonly Color BuffActiveIconColor = Helpers.HexToColor("#FFFFFF");

	private static readonly Color BuffInactiveIconColor = Helpers.HexToColor("#616060");

	private static readonly Color BuffActiveTitleColor = Helpers.HexToColor("#FFFFFF");

	private static readonly Color BuffInactiveTitleColor = Helpers.HexToColor("#8d8d8d");

	private static readonly Color BuffActiveDescColor = Helpers.HexToColor("#B4E52F");

	private static readonly Color BuffInactiveDescColor = Helpers.HexToColor("#8d8d8d");

	public override void Open()
	{
		_worldBossCaptures = worldBossCaptures.ToDictionary((WorldBossCaptureData x) => x.des, (WorldBossCaptureData x) => x);
		pendingDifficultyIconRefresh = true;
		shouldShowLoadingOnRequest = true;
		SetLoadingVisible(visible: true);
		base.Open();
		worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		worldBossGuildFullSnapshot = worldBossModelManager?.WorldBossGuildFullSnapshot;
		RefreshDifficultyIconOnce();
		GetWorldBossFullSnapshot();
	}

	public override void OnClickClose()
	{
		UITypeOpenOnClose = UIType.MissionHubPopup;
		base.OnClickClose();
	}

	public void OnclickTeam1()
	{
		Helpers.GameObjectSetActive(team1Container, value: false);
		Helpers.GameObjectSetActive(team2Container, value: true);
		Helpers.GameObjectSetActive(buff1Container, value: true);
		Helpers.GameObjectSetActive(buff2Container, value: false);
		SetLocalPosY(buff1Container, -125f);
		SetLocalPosY(buffTitleLabel.gameObject, -125f);
	}

	public void OnclickTeam2()
	{
		Helpers.GameObjectSetActive(team1Container, value: true);
		Helpers.GameObjectSetActive(team2Container, value: false);
		Helpers.GameObjectSetActive(buff1Container, value: true);
		Helpers.GameObjectSetActive(buff2Container, value: false);
		SetLocalPosY(buff1Container, 125f);
		SetLocalPosY(buffTitleLabel.gameObject, 125f);
	}

	public void OnclickBuff1()
	{
		if (team2Container != null && team2Container.activeInHierarchy)
		{
			SetLocalPosY(buff1Container, 125f);
			SetLocalPosY(buffTitleLabel.gameObject, 125f);
		}
		Helpers.GameObjectSetActive(buff1Container, value: false);
		Helpers.GameObjectSetActive(buff2Container, value: true);
		Helpers.GameObjectSetActive(team1Container, value: true);
		Helpers.GameObjectSetActive(team2Container, value: false);
	}

	public void OnclickBuff2()
	{
		Helpers.GameObjectSetActive(buff1Container, value: true);
		Helpers.GameObjectSetActive(buff2Container, value: false);
		Helpers.GameObjectSetActive(team1Container, value: true);
		Helpers.GameObjectSetActive(team2Container, value: false);
	}

	public string CheckRedAndBlueFlag()
	{
		if (worldBossGuildFullSnapshot == null)
		{
			return "Red";
		}
		if (!(worldBossGuildFullSnapshot.Match.GroupIdA == GameManager.Instance.playerModel.GuildId))
		{
			return "Red";
		}
		return "Blue";
	}

	public void Awake()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage += OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnDestroy()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
		}
	}

	public void OnclickDifficulty()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossDifficultyPopup).Open();
	}

	private void OnWorldBossFullSnapshotChanged(string message, string type)
	{
		Debug.LogError("OnWorldBossFullSnapshotChanged: " + message);
		if (IsPopupAlive())
		{
			GetWorldBossFullSnapshot();
		}
	}

	private bool IsPopupAlive()
	{
		if (this != null && base.gameObject != null)
		{
			return base.IsOpen;
		}
		return false;
	}

	private void SetLoadingVisible(bool visible)
	{
		if (visible)
		{
			if (!shouldShowLoadingOnRequest)
			{
				return;
			}
		}
		else
		{
			shouldShowLoadingOnRequest = false;
		}
		Helpers.GameObjectSetActive(loadingContainer, visible);
		Helpers.GameObjectSetActive(leftContent, !visible);
		Helpers.GameObjectSetActive(rightContent, !visible);
		Helpers.GameObjectSetActive(middleContent, !visible);
		Helpers.GameObjectSetActive(bottomContent, !visible);
	}

	private void GetWorldBossFullSnapshot()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.WorldBossModelManager == null)
		{
			SetLoadingVisible(visible: false);
			return;
		}
		WorldBossGetSnapshotRequest worldBossGetSnapshotRequest = null;
		worldBossGetSnapshotRequest = ((!GameManager.Instance.playerModel.WorldBossModelManager.IsOffSeason()) ? new WorldBossGetSnapshotRequest
		{
			GroupId = GameManager.Instance.playerModel.GuildId,
			SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
			CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentCycleId()
		} : new WorldBossGetSnapshotRequest
		{
			GroupId = GameManager.Instance.playerModel.GuildId,
			SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
			CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetNextCycleId()
		});
		string arg = GameManager.Instance.jsonSerializer.Serialize(worldBossGetSnapshotRequest);
		SetLoadingVisible(visible: true);
		SignalRClient.Instance.RequestCommand("WorldBossFullSnapshot", arg, OnWorldBossFullSnapshotAsync, waitForResponse: true);
	}

	private void OnWorldBossFullSnapshotAsync(string responseJson)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(responseJson))
		{
			SignalRClient.Instance.ClearError();
			SetLoadingVisible(visible: false);
			return;
		}
		worldBossGuildFullSnapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildFullSnapshot>(responseJson);
		if (worldBossGuildFullSnapshot == null)
		{
			SetLoadingVisible(visible: false);
			return;
		}
		GameManager.Instance.modelManager.SetWorldBossGuildFullSnapshot(worldBossGuildFullSnapshot);
		if (!IsPopupAlive())
		{
			SetLoadingVisible(visible: false);
			return;
		}
		RefreshCapturePoints();
		UpdateUI();
		RefreshDifficultyIconOnce();
		SetLoadingVisible(visible: false);
	}

	private void RefreshCapturePoints()
	{
		if (worldBossGuildFullSnapshot?.GuildFullState == null || _worldBossCaptures == null)
		{
			return;
		}
		Dictionary<string, WorldBossCapturePointView> allCapturePointStates = GameManager.Instance.playerModel.WorldBossModelManager.GetAllCapturePointStates();
		WorldBossBattlegroundDefinition[] array = GameManager.Instance.gameEconomyData.FindWorldBossBattlegroundDefinitionsByDifficulty(worldBossGuildFullSnapshot.GuildFullState.Difficulty);
		Debug.LogError("worldBossGuildFullSnapshot.GuildFullState.Difficulty: " + worldBossGuildFullSnapshot.GuildFullState.Difficulty);
		if (array == null)
		{
			DebugLogError("battlegroundDefinitions is null");
			return;
		}
		WorldBossBattlegroundDefinition[] array2 = array;
		foreach (WorldBossBattlegroundDefinition worldBossBattlegroundDefinition in array2)
		{
			if (worldBossBattlegroundDefinition == null || string.IsNullOrEmpty(worldBossBattlegroundDefinition.CapturePoint))
			{
				continue;
			}
			string capturePoint = worldBossBattlegroundDefinition.CapturePoint;
			if (_worldBossCaptures.TryGetValue(capturePoint, out var value) && !(value?.capture == null))
			{
				WorldBossCaptureOwner worldBossCaptureOwner = WorldBossCaptureOwner.None;
				if (worldBossBattlegroundDefinition.CapturePointType == "TOWER" || worldBossBattlegroundDefinition.CapturePointType == "DEPOT")
				{
					worldBossCaptureOwner = WorldBossCaptureOwner.PVP;
				}
				else if (worldBossBattlegroundDefinition.CapturePointType == "BOSS")
				{
					worldBossCaptureOwner = WorldBossCaptureOwner.BOSS;
				}
				else if (allCapturePointStates[capturePoint] != null && allCapturePointStates[capturePoint].GroupId != null)
				{
					worldBossCaptureOwner = ((allCapturePointStates[capturePoint].GroupId == GameManager.Instance.playerModel.GuildId) ? WorldBossCaptureOwner.MyPVE : WorldBossCaptureOwner.OtherPVE);
				}
				else
				{
					string value2 = CheckRedAndBlueFlag();
					worldBossCaptureOwner = ((worldBossBattlegroundDefinition.CapturePoint.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >= 0) ? WorldBossCaptureOwner.MyPVE : WorldBossCaptureOwner.OtherPVE);
				}
				value.capture.SetData(new WorldBossCaptureDataClient
				{
					definition = worldBossBattlegroundDefinition,
					view = allCapturePointStates[capturePoint],
					owner = worldBossCaptureOwner
				});
				value.capture.UpdateUI();
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (gameModeTimeLeft > -1000)
		{
			gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (timeLabel != null)
			{
				SetContentToTimerLabel(string.Concat(LocalizationManager.GetText(timeLabelLocalisation, FormatTimeLeft(gameModeTimeLeft))));
			}
			if (gameModeTimeLeft <= -1000)
			{
				gameModeTimeLeft = -1001L;
				CloseOnActivityEnded();
			}
		}
	}

	private void CloseOnActivityEnded()
	{
		CloseWorldBossDetailPopupIfOpen(UIType.WorldBossPVEDetailBackPopup);
		CloseWorldBossDetailPopupIfOpen(UIType.WorldBossPVPDetailPopup);
		UITypeOpenOnClose = UIType.MissionHubPopup;
		Close();
	}

	private static void CloseWorldBossDetailPopupIfOpen(UIType uiType)
	{
		HUDElement hUDElement = ((SingularityMonoBehaviour<HUDManager>.Instance != null) ? SingularityMonoBehaviour<HUDManager>.Instance.Get(uiType, null, createIfNotExist: false) : null);
		if (hUDElement != null && hUDElement.IsOpen)
		{
			hUDElement.Close();
		}
	}

	protected static string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	public virtual void SetContentToTimerLabel(string value)
	{
		HelpersUI.SetContentToLabel(timeLabel, value);
	}

	public override void UpdateUI()
	{
		if (IsPopupAlive())
		{
			base.UpdateUI();
			timeLabelLocalisation = "World.Boss.Countdown";
			HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText(GetTitleLocalizationKey()));
			worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
			gameModeTimeLeft = worldBossModelManager.GetTimeUntilCycleEndMs();
			UpdateTeamInfo();
			UpdateGroupInfo();
			UpdateBuffInfo();
		}
	}

	private void RefreshDifficultyIconOnce()
	{
		if (!pendingDifficultyIconRefresh)
		{
			return;
		}
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		if (worldBossGuildFullSnapshot?.GuildFullState == null || worldBossModelManager == null)
		{
			return;
		}
		WorldBossDifficultyDefinition worldBossDifficultyDefinition = GameManager.Instance.gameEconomyData.FindWorldBossDifficultyDefinition(worldBossModelManager.GetCurrentSeasonId(), worldBossGuildFullSnapshot.GuildFullState.Difficulty);
		if (worldBossDifficultyDefinition != null)
		{
			if (difficultyIcon != null)
			{
				difficultyIcon.spriteName = "UI_WB_Diffic_" + worldBossDifficultyDefinition.DifficultyClass;
			}
			if (difficultyLabel != null)
			{
				HelpersUI.SetContentToLabel(difficultyLabel, LocalizationManager.GetText(worldBossDifficultyDefinition.Localization));
			}
			pendingDifficultyIconRefresh = false;
		}
	}

	public void UpdateBuffInfo()
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		if (worldBossModelManager == null || buff1Container == null || buff2Container == null)
		{
			return;
		}
		List<WorldBossBuildingBuffView> myBuildingBuffs = worldBossModelManager.GetMyBuildingBuffs();
		int myActiveBuffCount = worldBossModelManager.GetMyActiveBuffCount();
		HelpersUI.SetContentToLabel(buffTitleLabel, LocalizationManager.GetText("World.Boss.ActiveBonuses.Outside", myActiveBuffCount.ToString()));
		Transform transform = buff1Container.transform.Find("Content");
		Transform transform2 = buff2Container.transform;
		foreach (WorldBossBuildingBuffView item in myBuildingBuffs)
		{
			if (item == null || string.IsNullOrEmpty(item.CapturePoint))
			{
				continue;
			}
			Color color = (item.IsActive ? BuffActiveIconColor : BuffInactiveIconColor);
			string n = "Icon_" + item.CapturePoint;
			SetSpriteColor(transform?.Find(n), color);
			string buffContentName = GetBuffContentName(item.CapturePoint);
			Transform transform3 = transform2?.Find(buffContentName);
			if (!(transform3 == null))
			{
				SetSpriteColor(transform3.Find(n), color);
				WorldBossBattlegroundDefinition buffDefinition = GetBuffDefinition(item.CapturePoint);
				string buffBuildingName = GetBuffBuildingName(buffDefinition, item.CapturePoint);
				Color color2 = (item.IsActive ? BuffActiveTitleColor : BuffInactiveTitleColor);
				SetLabelText(transform3.Find("Name")?.GetComponent<UILabel>(), LocalizationManager.GetText(buffBuildingName), color2);
				Color color3 = (item.IsActive ? BuffActiveDescColor : BuffInactiveDescColor);
				SetLabelText(transform3.Find("DescNow")?.GetComponent<UILabel>(), WorldBossBuildingBuffDescHelper.FormatBuffDescNow(buffDefinition, item), color3);
				Transform transform4 = transform3.Find("DescNext");
				if (WorldBossBuildingBuffDescHelper.ShouldShowDescNext(item))
				{
					Helpers.GameObjectSetActive(transform4?.gameObject, value: true);
					SetLabelText(transform4?.GetComponent<UILabel>(), WorldBossBuildingBuffDescHelper.FormatBuffDescNext(buffDefinition, item), BuffInactiveDescColor);
				}
				else
				{
					Helpers.GameObjectSetActive(transform4?.gameObject, value: false);
				}
			}
		}
	}

	public void OpneRetreat1ConfirmPopup()
	{
		OpenRetreatConfirmPopup(0);
	}

	public void OpneRetreat2ConfirmPopup()
	{
		OpenRetreatConfirmPopup(1);
	}

	private void OpenRetreatConfirmPopup(int teamIndex)
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		List<WorldBossDispatchedTeamView> list = worldBossModelManager?.GetMyDispatchedTeams();
		int num = list?.Count ?? 0;
		if (list != null && teamIndex >= 0 && teamIndex < num)
		{
			WorldBossRetreatConfirmPopup worldBossRetreatConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRetreatConfirmPopup) as WorldBossRetreatConfirmPopup;
			if (!(worldBossRetreatConfirmPopup == null))
			{
				worldBossRetreatConfirmPopup.SetTeam(list[teamIndex]);
				worldBossRetreatConfirmPopup.Open();
			}
		}
	}

	private WorldBossBattlegroundDefinition GetBuffDefinition(string capturePoint)
	{
		int difficultyLevel = worldBossModelManager?.GetCurrentBattleDifficulty() ?? 0;
		return GameManager.Instance?.gameEconomyData?.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, difficultyLevel);
	}

	private static string GetBuffBuildingName(WorldBossBattlegroundDefinition definition, string capturePoint)
	{
		if (definition != null && !string.IsNullOrEmpty(definition.BuildingName))
		{
			return definition.BuildingName;
		}
		return capturePoint;
	}

	private static string GetBuffContentName(string capturePoint)
	{
		return capturePoint switch
		{
			"TOWER-A" => "TOWER-AContent", 
			"TOWER-B" => "TOWER-BContent", 
			"DEPOT" => "DEPOTContent", 
			_ => capturePoint + "Content", 
		};
	}

	private static void SetLabelText(UILabel label, string text, Color color)
	{
		if (!(label == null))
		{
			HelpersUI.SetContentToLabel(label, text);
			label.color = color;
		}
	}

	private static void SetSpriteColor(Transform target, Color color)
	{
		if (!(target == null))
		{
			UISprite component = target.GetComponent<UISprite>();
			if (component != null)
			{
				component.color = color;
			}
		}
	}

	public void UpdateGroupInfo()
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		if (worldBossModelManager == null)
		{
			Helpers.GameObjectSetActive(blueFlag, value: false);
			Helpers.GameObjectSetActive(redFlag, value: false);
			return;
		}
		WorldBossMatchSnapshot worldBossMatchSnapshot = worldBossModelManager.WorldBossGuildFullSnapshot?.Match;
		if (worldBossMatchSnapshot == null)
		{
			Helpers.GameObjectSetActive(blueFlag, value: false);
			Helpers.GameObjectSetActive(redFlag, value: false);
			return;
		}
		string text = GameManager.Instance?.playerModel?.GuildId;
		bool flag = !string.IsNullOrEmpty(text) && text == worldBossMatchSnapshot.GroupIdA;
		bool flag2 = !string.IsNullOrEmpty(text) && text == worldBossMatchSnapshot.GroupIdB;
		Helpers.GameObjectSetActive(blueFlag, flag);
		Helpers.GameObjectSetActive(redFlag, flag2);
		long myGuildScore = worldBossModelManager.GetMyGuildScore();
		long opponentGuildScore = worldBossModelManager.GetOpponentGuildScore();
		long value = (flag ? myGuildScore : opponentGuildScore);
		long value2 = (flag2 ? myGuildScore : opponentGuildScore);
		if (blueNameLabel != null)
		{
			HelpersUI.SetContentToLabel(blueNameLabel, worldBossMatchSnapshot.GroupNameA);
		}
		if (redNameLabel != null)
		{
			HelpersUI.SetContentToLabel(redNameLabel, worldBossMatchSnapshot.GroupNameB);
		}
		if (blueScoreLabel != null)
		{
			HelpersUI.SetContentToLabel(blueScoreLabel, Helpers.FormatNumber(value));
		}
		if (redScoreLabel != null)
		{
			HelpersUI.SetContentToLabel(redScoreLabel, Helpers.FormatNumber(value2));
		}
	}

	public void OpenTeamSetPopup()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRetreatPopup).Open();
	}

	public void UpdateTeamInfo()
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		int num = worldBossModelManager?.GetMyDispatchedTeamCount() ?? 0;
		int num2 = worldBossModelManager?.GetDispatchTeamLimit() ?? 0;
		List<WorldBossDispatchedTeamView> list = worldBossModelManager?.GetMyDispatchedTeams() ?? new List<WorldBossDispatchedTeamView>();
		HelpersUI.SetContentToLabel(teamTitleLabel, LocalizationManager.GetText("World.Boss.DeployedTeams.Outside", num, num2));
		Helpers.GameObjectSetActive(team1Active1Icon, num >= 1);
		Helpers.GameObjectSetActive(team1Active2Icon, num >= 2);
		SetDispatchedTeamSlotState("TeamInfo1", num >= 1);
		SetDispatchedTeamSlotState("TeamInfo2", num >= 2);
		_dispatchedTeamOccupiedAtUtcMs[0] = -1L;
		_dispatchedTeamOccupiedAtUtcMs[1] = -1L;
		if (list.Count >= 1)
		{
			FillDispatchedTeamInfo("TeamInfo1", list[0], 0);
		}
		if (list.Count >= 2)
		{
			FillDispatchedTeamInfo("TeamInfo2", list[1], 1);
		}
	}

	private void FillDispatchedTeamInfo(string teamInfoName, WorldBossDispatchedTeamView team, int slotIndex)
	{
		if (!(team2DesGM == null) && team != null)
		{
			Transform transform = team2DesGM.transform.Find(teamInfoName + "/HaveTeamContent");
			if (!(transform == null))
			{
				UpdateDispatchedTeamHeroTokens(transform, team.SurvivorIds);
				UILabel label = transform.Find("TeamZone")?.GetComponent<UILabel>();
				string text = LocalizationManager.GetText(GetCapturePointDisplayName(team.CapturePoint));
				HelpersUI.SetContentToLabel(label, text);
				UpdateDispatchedTeamZoneIcon(transform, team.CapturePoint);
				_dispatchedTeamOccupiedAtUtcMs[slotIndex] = team.OccupiedAtUtcMs;
				UpdateDispatchedTeamTimeLabel(transform, slotIndex, GetSynchronizedUtcNowMs());
				UpdateDispatchedTeamDurabilityBlocks(transform, team.DefenderRemainingDurability);
			}
		}
	}

	private void UpdateDispatchedTeamDurabilityBlocks(Transform haveTeamContent, int defenderRemainingDurability)
	{
		Transform transform = haveTeamContent?.Find("BlockUp");
		if (transform == null)
		{
			return;
		}
		int num = Mathf.Clamp(defenderRemainingDurability, 0, 10);
		Color dispatchedTeamDurabilityColor = GetDispatchedTeamDurabilityColor(num);
		for (int i = 0; i < 10; i++)
		{
			int num2 = i + 1;
			Transform transform2 = transform.Find("Icon" + num2);
			if (transform2 == null)
			{
				continue;
			}
			bool flag = num2 <= num;
			Helpers.GameObjectSetActive(transform2.gameObject, flag);
			if (flag)
			{
				UIButton component = transform2.GetComponent<UIButton>();
				if (component != null)
				{
					component.defaultColor = dispatchedTeamDurabilityColor;
					component.hover = dispatchedTeamDurabilityColor;
					component.pressed = dispatchedTeamDurabilityColor;
					component.disabledColor = dispatchedTeamDurabilityColor;
					component.enabled = false;
				}
				TweenColor component2 = transform2.GetComponent<TweenColor>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
				UISprite component3 = transform2.GetComponent<UISprite>();
				if (component3 != null)
				{
					component3.color = dispatchedTeamDurabilityColor;
				}
			}
		}
	}

	private static Color GetDispatchedTeamDurabilityColor(int durabilityLine)
	{
		if (durabilityLine >= 7)
		{
			return DurabilityColorGreen;
		}
		if (durabilityLine >= 4)
		{
			return DurabilityColorYellow;
		}
		return DurabilityColorRed;
	}

	private void UpdateDispatchedTeamHeroTokens(Transform haveTeamContent, List<string> survivorAnalyticsIds)
	{
		for (int i = 0; i < 3; i++)
		{
			Transform transform = haveTeamContent.Find("HeroToken" + (i + 1));
			if (transform == null)
			{
				continue;
			}
			bool flag = survivorAnalyticsIds != null && i < survivorAnalyticsIds.Count && !string.IsNullOrEmpty(survivorAnalyticsIds[i]);
			Helpers.GameObjectSetActive(transform.gameObject, flag);
			if (flag)
			{
				SurvivorModel survivorByAnalyticsId = GetSurvivorByAnalyticsId(survivorAnalyticsIds[i]);
				UISprite component = transform.GetComponent<UISprite>();
				if (survivorByAnalyticsId != null && !(component == null))
				{
					component.spriteName = HelpersGfx.GetCurrencyIconName(SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivorByAnalyticsId));
				}
			}
		}
	}

	private void UpdateDispatchedTeamTimers()
	{
		if (team2DesGM == null)
		{
			return;
		}
		long synchronizedUtcNowMs = GetSynchronizedUtcNowMs();
		for (int i = 0; i < _dispatchedTeamOccupiedAtUtcMs.Length; i++)
		{
			if (_dispatchedTeamOccupiedAtUtcMs[i] > 0)
			{
				string text = ((i == 0) ? "TeamInfo1" : "TeamInfo2");
				Transform transform = team2DesGM.transform.Find(text + "/HaveTeamContent");
				if (!(transform == null) && transform.gameObject.activeInHierarchy)
				{
					UpdateDispatchedTeamTimeLabel(transform, i, synchronizedUtcNowMs);
				}
			}
		}
	}

	private void UpdateDispatchedTeamTimeLabel(Transform haveTeamContent, int slotIndex, long now)
	{
		if (haveTeamContent == null || slotIndex < 0 || slotIndex >= _dispatchedTeamOccupiedAtUtcMs.Length)
		{
			return;
		}
		UILabel uILabel = haveTeamContent.Find("TeamTime")?.GetComponent<UILabel>();
		if (!(uILabel == null))
		{
			long num = _dispatchedTeamOccupiedAtUtcMs[slotIndex];
			if (num <= 0)
			{
				HelpersUI.SetContentToLabel(uILabel, "00:00:00");
				return;
			}
			long milliSeconds = ((now > num) ? (now - num) : 0);
			HelpersUI.SetContentToLabel(uILabel, Helpers.FormatTimeAsHms(milliSeconds));
		}
	}

	private static long GetSynchronizedUtcNowMs()
	{
		return (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault() / 1000 * 1000;
	}

	private string GetCapturePointDisplayName(string capturePoint)
	{
		if (string.IsNullOrEmpty(capturePoint))
		{
			return string.Empty;
		}
		int difficultyLevel = worldBossModelManager?.GetCurrentBattleDifficulty() ?? 0;
		WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = GameManager.Instance?.gameEconomyData?.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, difficultyLevel);
		if (worldBossBattlegroundDefinition != null && !string.IsNullOrEmpty(worldBossBattlegroundDefinition.BuildingName))
		{
			return worldBossBattlegroundDefinition.BuildingName;
		}
		return capturePoint;
	}

	private static void UpdateDispatchedTeamZoneIcon(Transform haveTeamContent, string capturePoint)
	{
		if (haveTeamContent == null || string.IsNullOrEmpty(capturePoint))
		{
			return;
		}
		UISprite uISprite = haveTeamContent.Find("TeamZoneIcon")?.GetComponent<UISprite>();
		if (!(uISprite == null))
		{
			switch (capturePoint)
			{
			case "TOWER-A":
			case "TOWER-B":
				uISprite.spriteName = "Ui_Icon_Outpost";
				break;
			case "DEPOT":
				uISprite.spriteName = "Ui_Icon_Tents";
				break;
			}
		}
	}

	private static SurvivorModel GetSurvivorByAnalyticsId(string analyticsId)
	{
		if (string.IsNullOrEmpty(analyticsId))
		{
			return null;
		}
		ModelList<SurvivorModel> modelList = GameManager.Instance?.playerModel?.SurvivorContainer?.Survivors;
		if (modelList == null)
		{
			return null;
		}
		for (int i = 0; i < modelList.Count; i++)
		{
			if (modelList[i].IdForAnalytics == analyticsId)
			{
				return modelList[i];
			}
		}
		return null;
	}

	private void SetDispatchedTeamSlotState(string teamInfoName, bool hasTeam)
	{
		if (!(team2DesGM == null))
		{
			Transform transform = team2DesGM.transform.Find(teamInfoName);
			if (!(transform == null))
			{
				Helpers.GameObjectSetActive(transform.gameObject, value: true);
				Transform transform2 = transform.Find("HaveTeamContent");
				Transform obj = transform.Find("HaveNoTeamContent");
				Helpers.GameObjectSetActive(transform2?.gameObject, hasTeam);
				Helpers.GameObjectSetActive(obj?.gameObject, !hasTeam);
			}
		}
	}

	public void ShowTeamState()
	{
		Helpers.GameObjectSetActive(teamStateContainer, value: true);
	}

	public void HideTeamState()
	{
		Helpers.GameObjectSetActive(teamStateContainer, value: false);
	}

	public void OnclickTip()
	{
	}

	private string GetTitleLocalizationKey()
	{
		WorldBossSeasonDefinition worldBossSeasonDefinition = GameManager.Instance?.playerModel?.WorldBossModelManager?.GetCurrentSeason();
		if (worldBossSeasonDefinition != null)
		{
			return worldBossSeasonDefinition.SeasonTitle;
		}
		return "World.Boss.Title";
	}

	public void OnClickTest()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVEDetailBackPopup).Open();
	}

	public void OnClickPVPTest()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVPDetailPopup).Open();
	}

	public void OnClickStartTest()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossStartPopup).Open();
	}

	public void OnClickEndTest()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossEndPopup).Open();
	}

	private static void SetLocalPosY(GameObject go, float y)
	{
		if (!(go == null))
		{
			Vector3 localPosition = go.transform.localPosition;
			localPosition.y += y;
			go.transform.localPosition = localPosition;
		}
	}
}
