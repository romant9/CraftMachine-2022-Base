using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossPVPDetailPopup : HUDElement
{
	[SerializeField]
	private GameObject loadingContainer;

	[Header("Bundle Items List")]
	[SerializeField]
	private NUIScrollableList scrollableList;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private GameObject titleBG;

	[SerializeField]
	private UILabel descLabel;

	[SerializeField]
	private GameObject LeftContainer;

	[SerializeField]
	private GameObject RightContainer;

	[SerializeField]
	private UIButton refreshButton;

	[SerializeField]
	private UILabel refreshButtonLabel;

	[SerializeField]
	private UISprite CaptureSprite;

	private WorldBossGuildFullSnapshot worldBossGuildFullSnapshot;

	private WorldBossModelManager worldBossModelManager;

	private WorldBossCapturePointView capturePointView;

	private Coroutine restoreScrollCoroutine;

	private bool shouldShowLoadingOnRequest;

	private readonly List<GameObject> contentsHiddenForLoading = new List<GameObject>();

	private const float RefreshCooldownSeconds = 10f;

	private const string RefreshButtonLocalizationKey = "World.Boss.Cell.Refresh";

	private float refreshCooldownRemaining;

	private bool isProtectionCountdownActive;

	private long protectionRemainingMs;

	private bool isRightOwnershipCountdownActive;

	private bool isRightOccupationCountdownActive;

	private long rightOwnershipRemainingMs;

	private long rightOwnershipTotalMs;

	private long rightOccupationRemainingMs;

	private const int RightBlockIconCount = 9;

	private const int MajorityCellThreshold = 5;

	private static readonly Color BlueColor = Helpers.HexToColor("#3F65B1");

	private static readonly Color RedColor = Helpers.HexToColor("#882F28");

	private static readonly Color GrayColor = Helpers.HexToColor("#8d8d8d");

	private static readonly Color LightBlueColor = Helpers.HexToColor("#1E2942");

	private static readonly Color LightRedColor = Helpers.HexToColor("#401A16");

	private static readonly Color UnoccupiedStatusUpColor = Helpers.HexToColor("#8d8d8d");

	private static readonly Color UnoccupiedStatusDownColor = Helpers.HexToColor("#676666");

	private static readonly Color BuffActiveDescColor = Helpers.HexToColor("#B4E52F");

	private static readonly Color BuffInactiveDescColor = Helpers.HexToColor("#8d8d8d");

	public const string defaultItemPrefabName = "WorldBossPVP_List_Item";

	private static readonly string[] LoadingHiddenRootNames = new string[6] { "Overlay", "Close_Button", "Bg", "LeftContent", "RightContent", "UpdateBtn" };

	public string CapturePoint { get; private set; }

	public static void OpenPopup(string capturePoint)
	{
		WorldBossPVPDetailPopup worldBossPVPDetailPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVPDetailPopup) as WorldBossPVPDetailPopup;
		if (!(worldBossPVPDetailPopup == null))
		{
			worldBossPVPDetailPopup.CapturePoint = capturePoint;
			worldBossPVPDetailPopup.Open();
		}
	}

	public override void Open()
	{
		refreshCooldownRemaining = 0f;
		shouldShowLoadingOnRequest = true;
		SetLoadingVisible(visible: true);
		base.Open();
		ResetRefreshButtonLabel();
		GetWorldBossFullSnapshot();
	}

	public override void Close()
	{
		RestoreContentHiddenForLoading();
		Helpers.GameObjectSetActive(loadingContainer, value: false);
		base.Close();
	}

	public void GetWorldBossFullSnapshot()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.WorldBossModelManager == null)
		{
			SetLoadingVisible(visible: false);
			return;
		}
		worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
		WorldBossGetSnapshotRequest value = ((!worldBossModelManager.IsOffSeason()) ? new WorldBossGetSnapshotRequest
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
		string arg = GameManager.Instance.jsonSerializer.Serialize(value);
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
		Dictionary<string, WorldBossCapturePointView> allCapturePointStates = GameManager.Instance.playerModel.WorldBossModelManager.GetAllCapturePointStates();
		if (allCapturePointStates != null && allCapturePointStates.ContainsKey(CapturePoint))
		{
			capturePointView = allCapturePointStates[CapturePoint];
		}
		SetLoadingVisible(visible: false);
		UpdateUI();
	}

	public void Awake()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage += OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnWorldBossFullSnapshotChanged(string message, string type)
	{
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
			if (shouldShowLoadingOnRequest)
			{
				HideAllContentForLoading();
				Helpers.GameObjectSetActive(loadingContainer, value: true);
			}
		}
		else
		{
			shouldShowLoadingOnRequest = false;
			RestoreContentHiddenForLoading();
			Helpers.GameObjectSetActive(loadingContainer, value: false);
		}
	}

	private void HideAllContentForLoading()
	{
		if (contentsHiddenForLoading.Count > 0)
		{
			return;
		}
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			if ((!(loadingContainer != null) || !(gameObject == loadingContainer)) && gameObject.activeSelf)
			{
				contentsHiddenForLoading.Add(gameObject);
				Helpers.GameObjectSetActive(gameObject, value: false);
			}
		}
	}

	private void RestoreContentHiddenForLoading()
	{
		for (int i = 0; i < contentsHiddenForLoading.Count; i++)
		{
			Helpers.GameObjectSetActive(contentsHiddenForLoading[i], value: true);
		}
		contentsHiddenForLoading.Clear();
		for (int j = 0; j < LoadingHiddenRootNames.Length; j++)
		{
			Transform transform = base.transform.Find(LoadingHiddenRootNames[j]);
			if (transform != null)
			{
				Helpers.GameObjectSetActive(transform.gameObject, value: true);
			}
		}
	}

	public void OnRefreshButtonClicked()
	{
		if (!(refreshCooldownRemaining > 0f))
		{
			GetWorldBossFullSnapshot();
			StartRefreshCooldown();
		}
	}

	private void StartRefreshCooldown()
	{
		refreshCooldownRemaining = 10f;
		if (refreshButton != null)
		{
			refreshButton.isEnabled = false;
		}
		UpdateRefreshButtonLabel();
	}

	private void EndRefreshCooldown()
	{
		ResetRefreshButtonLabel();
	}

	private void ResetRefreshButtonLabel()
	{
		if (refreshButton != null)
		{
			refreshButton.isEnabled = true;
		}
		if (refreshButtonLabel != null)
		{
			refreshButtonLabel.text = LocalizationManager.GetText("World.Boss.Cell.Refresh");
		}
	}

	private void UpdateRefreshButtonLabel()
	{
		if (!(refreshButtonLabel == null))
		{
			int num = Mathf.CeilToInt(refreshCooldownRemaining);
			refreshButtonLabel.text = num + "s";
		}
	}

	private void OnDestroy()
	{
		if (restoreScrollCoroutine != null)
		{
			StopCoroutine(restoreScrollCoroutine);
			restoreScrollCoroutine = null;
		}
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
		}
	}

	public override void UpdateUI()
	{
		if (!IsPopupAlive())
		{
			return;
		}
		base.UpdateUI();
		if (scrollableList == null || string.IsNullOrEmpty(CapturePoint))
		{
			return;
		}
		bool flag = scrollableList.currentItemsCount > 0;
		Vector3 savedPanelLocalPosition = (flag ? scrollableList.GetCurrentScrollPanelLocalPosition() : Vector3.zero);
		scrollableList.Clear();
		List<WorldBossCellDefinition> worldBossCellDefinitionsByCapturePoint = GameManager.Instance.gameEconomyData.GetWorldBossCellDefinitionsByCapturePoint(CapturePoint);
		WorldBossCapturePointSnapshot capturePointSnapshot = FindCapturePointSnapshot(CapturePoint);
		SortCellDefinitions(worldBossCellDefinitionsByCapturePoint, capturePointSnapshot);
		string myColorFlag = CheckMyColorFlag();
		UpdateCaptureSprite(CapturePoint);
		for (int i = 0; i < worldBossCellDefinitionsByCapturePoint.Count; i += 3)
		{
			WorldBossPVPItem worldBossPVPItem = scrollableList.InstantiateAdd("WorldBossPVP_List_Item") as WorldBossPVPItem;
			if (!(worldBossPVPItem == null))
			{
				List<WorldBossPVPCellSlotData> list = new List<WorldBossPVPCellSlotData>(3) { CreateSlotData(worldBossCellDefinitionsByCapturePoint[i], capturePointSnapshot, myColorFlag) };
				if (i + 1 < worldBossCellDefinitionsByCapturePoint.Count)
				{
					list.Add(CreateSlotData(worldBossCellDefinitionsByCapturePoint[i + 1], capturePointSnapshot, myColorFlag));
				}
				if (i + 2 < worldBossCellDefinitionsByCapturePoint.Count)
				{
					list.Add(CreateSlotData(worldBossCellDefinitionsByCapturePoint[i + 2], capturePointSnapshot, myColorFlag));
				}
				worldBossPVPItem.SetData(list.ToArray());
			}
		}
		scrollableList.SortAndRepositionItems();
		if (restoreScrollCoroutine != null)
		{
			StopCoroutine(restoreScrollCoroutine);
		}
		restoreScrollCoroutine = StartCoroutine(RelayoutScrollViewAfterEnable(flag, savedPanelLocalPosition));
		UpdateLeftContainer();
		UpdateRightContainer();
	}

	private void UpdateRightContainer()
	{
		ResolveRightContainer();
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		if (worldBossModelManager != null && !(RightContainer == null) && !string.IsNullOrEmpty(CapturePoint))
		{
			WorldBossMatchSnapshot worldBossMatchSnapshot = worldBossGuildFullSnapshot?.Match ?? worldBossModelManager.WorldBossGuildFullSnapshot?.Match;
			if (worldBossMatchSnapshot != null)
			{
				Transform transform = RightContainer.transform;
				Transform obj = transform.Find("BlueContent");
				Transform transform2 = transform.Find("RedContent");
				string text = GameManager.Instance?.playerModel?.GuildId;
				bool flag = !string.IsNullOrEmpty(text) && text == worldBossMatchSnapshot.GroupIdA;
				Helpers.GameObjectSetActive(obj?.Find("TitleBG")?.gameObject, flag);
				Helpers.GameObjectSetActive(transform2?.Find("TitleBG")?.gameObject, !flag);
				WorldBossCellBarView capturePointCellBar = worldBossModelManager.GetCapturePointCellBar(CapturePoint);
				int num = (flag ? capturePointCellBar.MineOccupied : capturePointCellBar.EnemyOccupied);
				int num2 = (flag ? capturePointCellBar.EnemyOccupied : capturePointCellBar.MineOccupied);
				UpdateSideBlockContent(obj, num, BlueColor);
				UpdateSideBlockContent(transform2, num2, RedColor);
				UpdateRightCaptureState(transform, num, num2, worldBossMatchSnapshot);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (refreshCooldownRemaining > 0f)
		{
			refreshCooldownRemaining -= Time.deltaTime;
			if (refreshCooldownRemaining <= 0f)
			{
				refreshCooldownRemaining = 0f;
				EndRefreshCooldown();
			}
			else
			{
				UpdateRefreshButtonLabel();
			}
		}
		if (isProtectionCountdownActive)
		{
			UpdateProtectionCountdownLabel();
		}
		if (isRightOwnershipCountdownActive)
		{
			UpdateRightOwnershipCountdown();
		}
		if (isRightOccupationCountdownActive)
		{
			UpdateRightOccupationCountdown();
		}
	}

	private void UpdateLeftContainer()
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		if (worldBossModelManager != null && !string.IsNullOrEmpty(CapturePoint))
		{
			Dictionary<string, WorldBossCapturePointView> allCapturePointStates = worldBossModelManager.GetAllCapturePointStates();
			if (allCapturePointStates != null && allCapturePointStates.TryGetValue(CapturePoint, out var value))
			{
				capturePointView = value;
			}
			WorldBossBattlegroundDefinition battlegroundDefinition = GetBattlegroundDefinition(CapturePoint);
			if (battlegroundDefinition != null)
			{
				HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText(battlegroundDefinition.BuildingName));
				HelpersUI.SetContentToLabel(descLabel, LocalizationManager.GetText(battlegroundDefinition.BuildingDoneDesc));
			}
			WorldBossBuildingBuffView buildingBuff = FindBuildingBuff(CapturePoint);
			UpdateLeftContainerBuffDesc(battlegroundDefinition, buildingBuff);
			UpdateLeftContainerOccupationInfo(capturePointView);
		}
	}

	private void UpdateLeftContainerBuffDesc(WorldBossBattlegroundDefinition definition, WorldBossBuildingBuffView buildingBuff)
	{
		Transform transform = LeftContainer?.transform;
		if (!(transform == null))
		{
			Color color = ((buildingBuff != null && buildingBuff.IsActive) ? BuffActiveDescColor : BuffInactiveDescColor);
			SetLabelText(transform.Find("DescNow")?.GetComponent<UILabel>(), WorldBossBuildingBuffDescHelper.FormatBuffDescNow(definition, buildingBuff), color);
			Transform transform2 = transform.Find("DescNext");
			if (WorldBossBuildingBuffDescHelper.ShouldShowDescNext(buildingBuff))
			{
				Helpers.GameObjectSetActive(transform2?.gameObject, value: true);
				SetLabelText(transform2?.GetComponent<UILabel>(), WorldBossBuildingBuffDescHelper.FormatBuffDescNext(definition, buildingBuff), BuffInactiveDescColor);
			}
			else
			{
				Helpers.GameObjectSetActive(transform2?.gameObject, value: false);
			}
		}
	}

	private void UpdateLeftContainerOccupationInfo(WorldBossCapturePointView view)
	{
		Transform transform = LeftContainer?.transform;
		if (transform == null)
		{
			return;
		}
		bool num = view != null && (view.State == WorldBossCapturePointState.PvpOccupiedByOwn || view.State == WorldBossCapturePointState.PvpOccupiedByEnemy || !string.IsNullOrEmpty(view.GroupId));
		Transform transform2 = transform.Find("StatusUp");
		Transform transform3 = transform.Find("StatusDown");
		Helpers.GameObjectSetActive(transform2?.gameObject, value: true);
		Helpers.GameObjectSetActive(transform3?.gameObject, value: true);
		if (!num)
		{
			isProtectionCountdownActive = false;
			SetSpriteColor((titleBG != null) ? titleBG.transform : null, GrayColor);
			SetSpriteColor(transform2, UnoccupiedStatusUpColor);
			string text = LocalizationManager.GetText("World.Boss.PVP.BuildingCon");
			SetLabelText(transform2?.Find("Name")?.GetComponent<UILabel>(), text, Color.black);
			string text2 = LocalizationManager.GetText("World.Boss.PVP.BuildingUnoccupied");
			SetSpriteColor(transform3, UnoccupiedStatusDownColor);
			SetLabelText(transform3?.Find("Left")?.GetComponent<UILabel>(), text2, Color.white);
			return;
		}
		WorldBossMatchSnapshot worldBossMatchSnapshot = worldBossModelManager.WorldBossGuildFullSnapshot?.Match;
		int num2;
		Color color;
		if (worldBossMatchSnapshot != null && !string.IsNullOrEmpty(view.GroupId))
		{
			num2 = ((view.GroupId == worldBossMatchSnapshot.GroupIdA) ? 1 : 0);
			if (num2 != 0)
			{
				color = BlueColor;
				goto IL_018c;
			}
		}
		else
		{
			num2 = 0;
		}
		color = RedColor;
		goto IL_018c;
		IL_018c:
		Color color2 = color;
		Color color3 = ((num2 != 0) ? LightBlueColor : LightRedColor);
		string guildNameByGroupId = worldBossModelManager.GetGuildNameByGroupId(view.GroupId);
		SetSpriteColor((titleBG != null) ? titleBG.transform : null, color2);
		SetSpriteColor(transform2, color2);
		string text3 = LocalizationManager.GetText("World.Boss.PVP.BuildingOccupied", guildNameByGroupId);
		SetLabelText(transform2?.Find("Name")?.GetComponent<UILabel>(), text3, Color.white);
		SetSpriteColor(transform3, color3);
		protectionRemainingMs = ResolveProtectionRemainingMs(view);
		isProtectionCountdownActive = protectionRemainingMs > 0;
		if (isProtectionCountdownActive)
		{
			SetStatusDownLeftText(LocalizationManager.GetText("World.Boss.PVP.ProtectionDuration", Helpers.FormatTimeNoZero(protectionRemainingMs)));
		}
		else
		{
			SetStatusDownLeftText(LocalizationManager.GetText("World.Boss.PVP.ProtectionInvalid"));
		}
	}

	private void UpdateProtectionCountdownLabel()
	{
		protectionRemainingMs = ResolveProtectionRemainingMs(capturePointView);
		if (protectionRemainingMs > 0)
		{
			SetStatusDownLeftText(LocalizationManager.GetText("World.Boss.PVP.ProtectionDuration", Helpers.FormatTimeNoZero(protectionRemainingMs)));
		}
		else
		{
			isProtectionCountdownActive = false;
			SetStatusDownLeftText(LocalizationManager.GetText("World.Boss.PVP.ProtectionInvalid"));
		}
	}

	private long ResolveProtectionRemainingMs(WorldBossCapturePointView view)
	{
		long num = worldBossModelManager?.GetCapturePointProtectionRemainingMs(CapturePoint) ?? 0;
		if (num > 0)
		{
			return num;
		}
		if (view == null || view.ProtectionEndUtcMs <= 0)
		{
			return 0L;
		}
		long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
		if (view.ProtectionEndUtcMs <= valueOrDefault)
		{
			return 0L;
		}
		return view.ProtectionEndUtcMs - valueOrDefault;
	}

	private void SetStatusDownLeftText(string text)
	{
		SetLabelText(LeftContainer?.transform.Find("StatusDown/Left")?.GetComponent<UILabel>(), text, Color.white);
	}

	private WorldBossBattlegroundDefinition GetBattlegroundDefinition(string capturePoint)
	{
		int difficultyLevel = worldBossModelManager?.GetCurrentBattleDifficulty() ?? 0;
		return GameManager.Instance?.gameEconomyData?.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, difficultyLevel);
	}

	private WorldBossBuildingBuffView FindBuildingBuff(string capturePoint)
	{
		foreach (WorldBossBuildingBuffView myBuildingBuff in worldBossModelManager.GetMyBuildingBuffs())
		{
			if (myBuildingBuff != null && myBuildingBuff.CapturePoint == capturePoint)
			{
				return myBuildingBuff;
			}
		}
		return null;
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

	private void ResolveRightContainer()
	{
		if (!(RightContainer != null))
		{
			Transform transform = base.transform.Find("RightContent");
			if (transform != null)
			{
				RightContainer = transform.gameObject;
			}
		}
	}

	private static void UpdateSideBlockContent(Transform sideContent, int occupiedCount, Color blockColor)
	{
		if (sideContent == null)
		{
			return;
		}
		int num = Mathf.Clamp(occupiedCount, 0, 9);
		string text = LocalizationManager.GetText("World.Boss.PVP.GuildOccupyCount", num.ToString());
		SetLabelText(sideContent.Find("Num")?.GetComponent<UILabel>(), text, Color.white);
		Transform transform = sideContent.Find("BlockUp");
		if (transform == null)
		{
			return;
		}
		for (int i = 1; i <= 9; i++)
		{
			Transform transform2 = transform.Find("Icon" + i);
			if (!(transform2 == null))
			{
				Helpers.GameObjectSetActive(transform2.gameObject, i <= num);
				SetSpriteColor(transform2, blockColor);
			}
		}
	}

	private void UpdateRightCaptureState(Transform rightRoot, int blueCount, int redCount, WorldBossMatchSnapshot match)
	{
		isRightOwnershipCountdownActive = false;
		isRightOccupationCountdownActive = false;
		Transform transform = rightRoot.Find("NoOneState");
		Transform transform2 = rightRoot.Find("OneHalfState");
		Transform transform3 = rightRoot.Find("OccupationState");
		bool num = ResolveProtectionRemainingMs(capturePointView) > 0;
		long capturePointOwnershipCountdownMs = worldBossModelManager.GetCapturePointOwnershipCountdownMs(CapturePoint);
		bool flag = blueCount >= 5;
		bool flag2 = redCount >= 5;
		bool flag3 = capturePointOwnershipCountdownMs > 0 || flag || flag2;
		if (num)
		{
			Helpers.GameObjectSetActive(transform?.gameObject, value: false);
			Helpers.GameObjectSetActive(transform2?.gameObject, value: false);
			Helpers.GameObjectSetActive(transform3?.gameObject, value: true);
			string text = capturePointView?.GroupId;
			bool flag4 = match != null && !string.IsNullOrEmpty(text) && text == match.GroupIdA;
			SetSpriteName(transform3?.Find("Icon"), flag4 ? "UI_Shield_blue" : "UI_Shield_red");
			rightOccupationRemainingMs = ResolveProtectionRemainingMs(capturePointView);
			isRightOccupationCountdownActive = rightOccupationRemainingMs > 0;
			UpdateRightOccupationCountdown();
		}
		else if (!flag3)
		{
			Helpers.GameObjectSetActive(transform?.gameObject, value: true);
			Helpers.GameObjectSetActive(transform2?.gameObject, value: false);
			Helpers.GameObjectSetActive(transform3?.gameObject, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(transform?.gameObject, value: false);
			Helpers.GameObjectSetActive(transform2?.gameObject, value: true);
			Helpers.GameObjectSetActive(transform3?.gameObject, value: false);
			bool flag5 = ResolveMajorityIsBlue(match, blueCount, redCount, flag, flag2);
			SetSpriteName(transform2?.Find("Icon"), flag5 ? "UI_WB_PVPDetail_Blue" : "UI_WB_PVPDetail_Red");
			rightOwnershipRemainingMs = capturePointOwnershipCountdownMs;
			int valueOrDefault = (GameManager.Instance?.gameEconomyData?.WorldBossConfig?.BeforeProtection).GetValueOrDefault();
			rightOwnershipTotalMs = ((valueOrDefault > 0) ? ((long)valueOrDefault * 1000L) : 600000);
			isRightOwnershipCountdownActive = rightOwnershipRemainingMs > 0;
			UpdateRightOwnershipCountdown();
		}
	}

	private bool ResolveMajorityIsBlue(WorldBossMatchSnapshot match, int blueCount, int redCount, bool blueHasMajority, bool redHasMajority)
	{
		string text = worldBossModelManager?.GetCapturePointOwnershipCountdownGroupId(CapturePoint);
		if (match != null && !string.IsNullOrEmpty(text))
		{
			return text == match.GroupIdA;
		}
		if (blueHasMajority && !redHasMajority)
		{
			return true;
		}
		if (redHasMajority && !blueHasMajority)
		{
			return false;
		}
		return blueCount >= redCount;
	}

	private void UpdateRightOwnershipCountdown()
	{
		Transform transform = RightContainer?.transform.Find("OneHalfState");
		if (transform == null)
		{
			return;
		}
		if (rightOwnershipRemainingMs > 0)
		{
			rightOwnershipRemainingMs = worldBossModelManager?.GetCapturePointOwnershipCountdownMs(CapturePoint) ?? 0;
		}
		SetLabelText(transform.Find("TimeLeft")?.GetComponent<UILabel>(), FormatMinutesSeconds(rightOwnershipRemainingMs), Color.white);
		UIProgressBar uIProgressBar = transform.Find("Progress Bar")?.GetComponent<UIProgressBar>();
		if (uIProgressBar != null && rightOwnershipTotalMs > 0)
		{
			uIProgressBar.value = Mathf.Clamp01((float)rightOwnershipRemainingMs / (float)rightOwnershipTotalMs);
		}
		if (rightOwnershipRemainingMs <= 0)
		{
			bool num = isRightOwnershipCountdownActive;
			isRightOwnershipCountdownActive = false;
			if (num)
			{
				UpdateRightContainer();
			}
		}
	}

	private void UpdateRightOccupationCountdown()
	{
		Transform transform = RightContainer?.transform.Find("OccupationState");
		if (transform == null)
		{
			return;
		}
		if (rightOccupationRemainingMs > 0)
		{
			long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
			if (capturePointView != null && capturePointView.ProtectionEndUtcMs > valueOrDefault)
			{
				rightOccupationRemainingMs = capturePointView.ProtectionEndUtcMs - valueOrDefault;
			}
			else
			{
				rightOccupationRemainingMs = worldBossModelManager?.GetCapturePointProtectionRemainingMs(CapturePoint) ?? 0;
			}
		}
		if (rightOccupationRemainingMs <= 0)
		{
			bool num = isRightOccupationCountdownActive;
			isRightOccupationCountdownActive = false;
			if (num)
			{
				UpdateRightContainer();
			}
		}
		else
		{
			SetLabelText(transform.Find("TimeLeft")?.GetComponent<UILabel>(), LocalizationManager.GetText("World.Boss.PVP.OccupiedwithProtection", Helpers.FormatTimeNoZero(rightOccupationRemainingMs)), Color.white);
		}
	}

	private static void SetSpriteName(Transform target, string spriteName)
	{
		if (!(target == null) && !string.IsNullOrEmpty(spriteName))
		{
			UISprite component = target.GetComponent<UISprite>();
			if (component != null)
			{
				component.spriteName = spriteName;
			}
		}
	}

	private static string FormatMinutesSeconds(long milliSeconds)
	{
		milliSeconds = Math.Max(0L, milliSeconds);
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 60;
		int num3 = num % 60;
		return $"{num2:00}:{num3:00}";
	}

	private static WorldBossPVPCellSlotData CreateSlotData(WorldBossCellDefinition cellDefinition, WorldBossCapturePointSnapshot capturePointSnapshot, string myColorFlag)
	{
		return new WorldBossPVPCellSlotData
		{
			MyColorFlag = myColorFlag,
			CellDefinition = cellDefinition,
			CellStateSnapshot = FindCellState(capturePointSnapshot, cellDefinition)
		};
	}

	private IEnumerator RelayoutScrollViewAfterEnable(bool preserveScrollPosition, Vector3 savedPanelLocalPosition)
	{
		yield return null;
		restoreScrollCoroutine = null;
		if (!(this == null) && !(scrollableList == null))
		{
			scrollableList.SortAndRepositionItems();
			if (preserveScrollPosition)
			{
				scrollableList.RestoreScrollPanelLocalPosition(savedPanelLocalPosition);
			}
			else
			{
				scrollableList.ResetScrollPosition();
			}
		}
	}

	private WorldBossCapturePointSnapshot FindCapturePointSnapshot(string capturePoint)
	{
		if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(capturePoint))
		{
			return null;
		}
		for (int i = 0; i < worldBossGuildFullSnapshot.CapturePoints.Count; i++)
		{
			WorldBossCapturePointSnapshot worldBossCapturePointSnapshot = worldBossGuildFullSnapshot.CapturePoints[i];
			if (worldBossCapturePointSnapshot != null && worldBossCapturePointSnapshot.CapturePoint == capturePoint)
			{
				return worldBossCapturePointSnapshot;
			}
		}
		return null;
	}

	private static void SortCellDefinitions(List<WorldBossCellDefinition> cellDefinitions, WorldBossCapturePointSnapshot capturePointSnapshot)
	{
		if (cellDefinitions != null && cellDefinitions.Count > 1)
		{
			cellDefinitions.Sort(delegate(WorldBossCellDefinition a, WorldBossCellDefinition b)
			{
				int num = GetCellDisplaySortOrder(GetCellStatus(capturePointSnapshot, a)).CompareTo(GetCellDisplaySortOrder(GetCellStatus(capturePointSnapshot, b)));
				return (num != 0) ? num : GetCellDefinitionSortId(a).CompareTo(GetCellDefinitionSortId(b));
			});
		}
	}

	private static int GetCellStatus(WorldBossCapturePointSnapshot capturePointSnapshot, WorldBossCellDefinition cellDefinition)
	{
		return FindCellState(capturePointSnapshot, cellDefinition)?.Status ?? 0;
	}

	private static int GetCellDisplaySortOrder(int status)
	{
		return status switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			_ => 0, 
		};
	}

	private static int GetCellDefinitionSortId(WorldBossCellDefinition cellDefinition)
	{
		if (cellDefinition == null || string.IsNullOrEmpty(cellDefinition.Cell))
		{
			return int.MaxValue;
		}
		string cell = cellDefinition.Cell;
		int num = cell.LastIndexOf('-');
		if (num >= 0 && num + 1 < cell.Length && int.TryParse(cell.Substring(num + 1), out var result))
		{
			return result;
		}
		return int.MaxValue;
	}

	private static WorldBossCellStateSnapshot FindCellState(WorldBossCapturePointSnapshot capturePointSnapshot, WorldBossCellDefinition cellDefinition)
	{
		if (capturePointSnapshot?.CellStates == null || cellDefinition == null)
		{
			return null;
		}
		for (int i = 0; i < capturePointSnapshot.CellStates.Count; i++)
		{
			WorldBossCellStateSnapshot worldBossCellStateSnapshot = capturePointSnapshot.CellStates[i];
			if (worldBossCellStateSnapshot != null && worldBossCellStateSnapshot.Cell == cellDefinition.Cell)
			{
				return worldBossCellStateSnapshot;
			}
		}
		return null;
	}

	private string CheckMyColorFlag()
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

	private void UpdateCaptureSprite(string capturePoint)
	{
		if (!(CaptureSprite == null))
		{
			string capturePointSpriteName = WorldBossCaptureBase.GetCapturePointSpriteName(capturePoint);
			if (!string.IsNullOrEmpty(capturePointSpriteName))
			{
				CaptureSprite.spriteName = capturePointSpriteName;
			}
		}
	}
}
