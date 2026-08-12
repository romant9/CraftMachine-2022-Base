using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class CampView : ModelView<CampModel>
{
	[SerializeField]
	[Tooltip("Grid mesh which shows valid positions for placing a building.")]
	private CampValidAreaGridVisualization validAreaGrid;

	[SerializeField]
	[Tooltip("Time for the grid to fade in/out.")]
	private float validAreaGridFadeTime;

	[SerializeField]
	[Tooltip("Ad Tower Position")]
	public GameObject AdTowerPosition;

	public CameraCampController CameraController;

	private Camera ActiveCamera;

	[Tooltip("The transform that will contain all the buildings.")]
	public Transform BuildingsContainer;

	[Tooltip("The collider used to check which building you are clicking.")]
	[SerializeField]
	private BoxCollider buildingsGroundCollider;

	[SerializeField]
	private float OutpostSeasonUpdateIntervalSeconds = 60f;

	[SerializeField]
	private float OutpostTierUpdateIntervalSeconds = 5f;

	private float updateOutpostSeasonTimer;

	private float updateOutpostTierTimer;

	private float lastTradeShopRefresh;

	private Camera mainCamera;

	private static CampView instance;

	[SerializeField]
	[Tooltip("The transform that translates ground space to grid space.")]
	private Transform gridSpace;

	private CampHUD hud;

	private BuildingsHUD buildingsHud;

	private Vector3 touchDownPosition;

	private CampModel campModel;

	private BuildingView temporaryBuildingToConstruct;

	private float previousCarMoveTime;

	private IEnumerator mFadeCoroutine;

	public CampViewBuildings CampViewBuildings { get; private set; }

	public CampViewActors CampViewActors { get; private set; }

	public BuildableBuildings BuildableBuildings { get; private set; }

	public bool SelectEnabled { get; set; }

	public bool ShowGuildInfoPendingOnJoin { get; set; }

	public CampHUD Hud => hud;

	public BuildingsHUD BuildingsHud => buildingsHud;

	public ActorHUD ActorHUD { get; private set; }

	public static CampView Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<CampView>();
			}
			return instance;
		}
	}

	public bool IsShown => base.gameObject.activeInHierarchy;

	private void Awake()
	{
		CampViewBuildings = GetComponent<CampViewBuildings>();
		CampViewActors = GetComponent<CampViewActors>();
		BuildableBuildings = new BuildableBuildings();
		BuildableBuildings.Update();
		ActiveCamera = CameraController.GetComponent<Camera>();
		mainCamera = Camera.main;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		SingularityMonoBehaviour<HUDManager>.Instance.OnAnyHudElementClosed += OnAnyHudElementClosed;
		Helpers.ClearUnusedMemory();
		SingularityMonoBehaviour<AudioManager>.Instance.UnloadAudio("CombatSfx");
		SingularityMonoBehaviour<AudioManager>.Instance.LoadAudio("CampSfx");
		HideValidAreaGrid();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		SingularityMonoBehaviour<HUDManager>.Instance.OnAnyHudElementClosed -= OnAnyHudElementClosed;
		GameManager.Instance.GuildManager.SuggestionLogic.OnStop();
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnBuildingMoveConfirmed":
			if (parameter is BuildingModel)
			{
				CampViewBuildings.NewBuildingLocationConfirmed();
			}
			else
			{
				CampViewBuildings.RequestNewBuildingCreation();
			}
			break;
		case "OnBuildingMoveCancelled":
			if (parameter is BuildingModel)
			{
				CampViewBuildings.CancelBuildingMove();
				break;
			}
			CampViewBuildings.UnselectBuilding();
			ClearTemporaryBuilding();
			break;
		case "SocialGuildJoined":
			if (ShowGuildInfoPendingOnJoin)
			{
				ShowGuildInfoPendingOnJoin = false;
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialPopupGuild).Open();
			}
			break;
		}
	}

	public void ClearTemporaryBuilding()
	{
		if (temporaryBuildingToConstruct != null)
		{
			CampViewBuildings.RemoveBuildingView(temporaryBuildingToConstruct);
			temporaryBuildingToConstruct = null;
		}
	}

	public void CancelBuildingPlacement()
	{
		if (!(CampViewBuildings == null))
		{
			if (CampViewBuildings.SelectedBuilding != null && CampViewBuildings.SelectedBuilding.Model == null)
			{
				CampViewBuildings.CancelBuildingMove();
				CampViewBuildings.UnselectBuilding();
				ClearTemporaryBuilding();
			}
			else if (CampViewBuildings.SelectedBuilding != null && CampViewBuildings.SelectedBuilding.Model != null && CampViewBuildings.Moving)
			{
				CampViewBuildings.CancelBuildingMove();
			}
		}
	}

	public BuildingView CreateBuildingViewWithoutModel(string buildingTypeName, int level, GridPosition startPosition)
	{
		GameObject gameObject = new GameObject("FakeBuildingView_" + buildingTypeName);
		gameObject.transform.parent = Instance.BuildingsContainer;
		BuildingView buildingView = CreateBuildingViewComponentFromType(buildingTypeName, gameObject);
		GameEconomyData gameEconomyData = GameManager.Instance.playerModel.gameEconomyData;
		BuildingType buildingType = gameEconomyData.GetBuildingType(buildingTypeName);
		GridSize abuildingSize = new GridSize((int)Mathf.Ceil((float)gameEconomyData.ScaleToGrid(buildingType.Size.X) * 0.5f) * 2, (int)Mathf.Ceil((float)gameEconomyData.ScaleToGrid(buildingType.Size.Y) * 0.5f) * 2);
		buildingView.SetupVisualsWithoutModel(buildingTypeName, level, abuildingSize, startPosition, buildingType.CanMove);
		buildingView.Initialize(null);
		AddBuilding(buildingView);
		return buildingView;
	}

	private BuildingView CreateBuildingViewComponentFromType(string buildingType, GameObject targetObject)
	{
		BuildingView buildingView = null;
		switch (buildingType)
		{
		case "Workshop":
			return targetObject.AddComponent<WorkshopView>();
		case "TrainingGround":
			return targetObject.AddComponent<TrainingGroundView>();
		case "Cage":
			return targetObject.AddComponent<CageView>();
		case "MedicTent":
			return targetObject.AddComponent<MedicTentView>();
		case "BuffBuildingCriticalChance":
		case "BuffBuildingDamage":
		case "BuffBuildingHealth":
			return targetObject.AddComponent<BuffBuildingView>();
		default:
			if (base.Model.gameEconomyData.GetBuildingType(buildingType).Category == BuildingCategory.Vegetation)
			{
				return targetObject.AddComponent<VegetationView>();
			}
			return targetObject.AddComponent<BuildingView>();
		}
	}

	public BuildingView GetOwnedBuildingByType(string buildingType)
	{
		if (CampViewBuildings != null && CampViewBuildings.Buildings != null)
		{
			for (int i = 0; i < CampViewBuildings.Buildings.Count; i++)
			{
				if (CampViewBuildings.Buildings[i].BuildingType == buildingType && CampViewBuildings.Buildings[i].BuildingLevel > 0)
				{
					return CampViewBuildings.Buildings[i];
				}
			}
		}
		else
		{
			Debug.LogWarning("Could not get BuildingView of type: " + buildingType + ". Something NULL");
		}
		return null;
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		hud = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		hud.Open();
		hud.SetSettingsNotifications(0);
		buildingsHud = hud.GetComponent<BuildingsHUD>();
		ActorHUD = hud.GetComponent<ActorHUD>();
		SelectEnabled = true;
		campModel = model as CampModel;
		InstantiateCampBuildings();
		if (TutorialView.Instance.Model != null)
		{
			hud.SetupTutorialHUD(TutorialView.Instance.Model.GetCurrentStepDefinition);
		}
		else
		{
			hud.SetupTutorialHUD();
		}
		TutorialView.Instance.StartupSetting = TutorialView.StartupSettingType.Normal;
		CampViewBuildings.Initialize();
		campModel.Changed += OnModelChange;
		if (validAreaGrid != null)
		{
			validAreaGrid.Initialize(campModel);
		}
		UpdateGridSpaceTranslation();
		CampViewActors.Initialize();
		if (buildingsHud != null)
		{
			buildingsHud.RefreshExpansionIndicator(campModel);
		}
		TutorialView.Instance.InitializeForCamp();
		ShowUpgradesCompletedNotification();
		if (!GameManager.Instance.GameCenterManager.Authenticated)
		{
			GameManager.Instance.GameCenterManager.PromptGameCenterConnect();
		}
		TryStartGuildInviteFlow();
		DisplayBanner();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.LootManager != null && playerModel.LootManager.PendingTradeCrates != null && playerModel.LootManager.PendingTradeCrates.Count > 0)
		{
			OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				openLootInUi.OpenForModel(playerModel.LootManager);
			}
		}
		if (playerModel != null && playerModel.BundleManager != null && (playerModel.BundleManager.IAPBonusGiftLootEntry != null || playerModel.BundleManager.WebShopLootEntrys.Count > 0 || playerModel.BundleManager.ShareRewardEntrys.Count > 0) && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OpenLootInUi) == null)
		{
			OpenLootInUi openLootInUi2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi2 != null)
			{
				openLootInUi2.OpenForModel(GameManager.Instance.playerModel.BundleManager);
			}
		}
		if (playerModel?.SubscriptionBuyedBundleIds != null && playerModel.SubscriptionBuyedBundleIds.Count > 0)
		{
			for (int i = 0; i < playerModel.SubscriptionBuyedBundleIds.Count; i++)
			{
				string identifier = playerModel.SubscriptionBuyedBundleIds[i];
				BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(identifier);
				BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
				UIEvent.Send("OnBundleBought", bundleStoreDefinition);
				IAPConfirmPopupNew.OpenWithSubscriptionContent(bundleStoreDefinition, bundleContentDefinition, givenBySupport: false);
				Helpers.ExecuteCommand(new SubscriptionBundleViewedCommand());
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
				}
			}
		}
		CheckBananaPop();
		DisplayUpdateInfo();
	}

	public void CheckBananaPop()
	{
		if (Helpers.GetOpenBananaButtonOnApp())
		{
			TWDModelResult tWDModelResult = Helpers.ExecuteCommand(new BananaPopupCommand(Helpers.GetShopRoleType()));
			CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
			if (campHUD != null)
			{
				_ = campHUD.GetBananaButton().activeSelf;
			}
		}
	}

	private void TryStartGuildInviteFlow()
	{
		if (GameManager.Instance.GuildInviteFlow == null)
		{
			GuildInviteFlow.TryRestoreDeeplink();
		}
		if (GameManager.Instance.GuildInviteFlow != null)
		{
			GameManager.Instance.GuildInviteFlow.StartJoinGuildFlow();
		}
	}

	public void ShowUpgradesCompletedNotification()
	{
		if (TutorialView.Instance != null && TutorialView.Instance.Running)
		{
			if (base.Model != null && base.Model.NotificationQueue != null)
			{
				base.Model.NotificationQueue.Clear();
			}
		}
		else
		{
			if (base.Model == null || base.Model.Manager == null || base.Model.Manager.VisitMode != VisitMode.None || base.Model.NotificationQueue == null)
			{
				return;
			}
			string text = "";
			string text2 = "";
			string title = "";
			for (int i = 0; i < base.Model.NotificationQueue.Count; i++)
			{
				NotificationQueueItem notificationQueueItem = base.Model.NotificationQueue[i];
				if (notificationQueueItem == null)
				{
					continue;
				}
				string text3 = notificationQueueItem.Name;
				string text4 = null;
				if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.Building)
				{
					text4 = "Notification.BuildingUpgraded{Name}";
					text3 = HelpersLocalization.GetBuildingName(text3);
					BuildingView buildingView = CampViewBuildings.FindBuildingView(notificationQueueItem.ModelId);
					if (buildingView != null)
					{
						buildingView.ShowCompleteEffect();
					}
				}
				else if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.Survivor)
				{
					text4 = "Notification.SurvivorUpgraded{Name}";
				}
				else if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.Walker)
				{
					text4 = "Notification.WalkerUpgraded";
				}
				else if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.Equipment)
				{
					text4 = "Notification.EquipmentUpgraded{Name}";
					text3 = HelpersLocalization.GetEquipmentName(text3);
				}
				else if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildKickedOut || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildPromotedLeader || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildRequestAccepted || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildRequestRefused || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildPromotedLeaderByDemotion)
				{
					text2 = text2 + LocalizationManager.GetText("Notification." + notificationQueueItem.NotificationType) + "\n";
				}
				else if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildDemoted || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildPromoted)
				{
					GuildModel guildModel = GameManager.Instance.guildModel;
					if (guildModel != null)
					{
						GuildMemberRole valueOrDefault = guildModel.GetMemberRole(GameManager.Instance.playerModel.HashedId).GetValueOrDefault();
						text2 = LocalizationManager.GetText("Notification." + notificationQueueItem.NotificationType.ToString() + valueOrDefault) + "\n";
					}
				}
				else if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildRemovedFromBattleSlot)
				{
					text2 = LocalizationManager.GetText("GvG.Alert.RemovedFromBattle.Message");
					title = notificationQueueItem.Name;
					Helpers.ExecuteCommand(new GvgMarkUnseenBattleSignUpRemove());
				}
				else if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.LaunchTutorial)
				{
					LaunchTutorial(notificationQueueItem);
				}
				else
				{
					text4 = "Notification." + notificationQueueItem.NotificationType;
				}
				if (text4 != null)
				{
					text += LocalizationManager.GetText(text4, text3, notificationQueueItem.Level.ToString());
					if (i < base.Model.NotificationQueue.Count - 1)
					{
						text += "\n";
					}
				}
			}
			if (base.Model != null && base.Model.NotificationQueue != null)
			{
				base.Model.NotificationQueue.Clear();
			}
			if (text != "")
			{
				HUDNotification.Info(text);
			}
			if (text2 != "")
			{
				AlertPopup.ShowPopup(title, text2, LocalizationManager.GetText("Button.Ok"));
			}
		}
	}

	private void LaunchTutorial(NotificationQueueItem item)
	{
		if (item.Name == "RadioTent")
		{
			if (item.Level == 1)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.BuiltRadioTent.1" });
			}
		}
		else
		{
			if (!(item.Name == "Council"))
			{
				return;
			}
			if (item.Level == 3)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.UpgradedCouncilLvl3.1", "Tutorial.UpgradedCouncilLvl3.2" }, TutorialView.Instance.StartNextTutorial);
			}
			if (item.Level == GameManager.Instance.gameEconomyData.ConfigData.ChallengesUnlockAtCouncilLevel)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.ChallengeUnlock.1" }, delegate
				{
					if (!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("ChallengeMode"))
					{
						TutorialView.Instance.StartPart("ChallengeMode");
					}
				});
			}
			else if (item.Level == GameManager.Instance.gameEconomyData.ConfigData.SurvivalUnlockAtCouncilLevel)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.SurvivalUnlock.1" }, delegate
				{
					if (!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("SurvivalMode"))
					{
						TutorialView.Instance.StartPart("SurvivalMode");
					}
				});
			}
			else if (item.Level == GameManager.Instance.gameEconomyData.ConfigData.SurvivalHardUnlockAtCouncilLevel)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.SurvivalHardUnlock.1" });
			}
			else if (item.Level == GameManager.Instance.gameEconomyData.ConfigData.SurvivalNightmareUnlockAtCouncilLevel)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.SurvivalNightmareUnlock.1" });
			}
			else if (item.Level == GameManager.Instance.gameEconomyData.EndlessModeConfig.CouncilLockLevel)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.EndlessUnlock.1" }, delegate
				{
					if (!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("EndlessMode"))
					{
						TutorialView.Instance.StartPart("EndlessMode");
					}
				});
			}
			else if (item.Level == GameManager.Instance.gameEconomyData.EndlessModeConfig.ExpertModeCouncilLockLevel)
			{
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.EndlessExpertUnlock.1" });
			}
			else
			{
				if (item.Level != GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarUnlockAtCouncilLevel)
				{
					return;
				}
				ShowDialog("Portrait_Info", new List<string> { "Tutorial.GuildBattleUnlock.1" }, delegate
				{
					if (!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("GuildBattleMode"))
					{
						TutorialView.Instance.StartPart("GuildBattleMode");
					}
				});
			}
		}
	}

	public void ShowDialog(string portraitId, List<string> localizationKeys, Callback callback = null)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < localizationKeys.Count; i++)
		{
			string text = localizationKeys[i];
			list.Add("Dialog," + portraitId + "," + text + ((i >= localizationKeys.Count - 1) ? ",hide" : ""));
		}
		TutorialView.Instance.StartCutScene(list, callback);
	}

	private void OnAnyHudElementClosed(HUDElement element, HUDElementConfig hudElementConfig)
	{
		if (hudElementConfig != null && hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Dialog)
		{
			ShowUpgradesCompletedNotification();
		}
	}

	public void InstantiateCampBuildings()
	{
		ModelList<BuildingModel> buildings = campModel.Buildings;
		for (int i = 0; i < buildings.Count; i++)
		{
			InstantiateCampBuilding(buildings[i]);
		}
		HashSet<string> hashSet = new HashSet<string>();
		string text = "";
		for (int j = 0; j < buildings.Count; j++)
		{
			text = ((!SingularityMonoBehaviour<AssetBundleController>.Instance.IsAssetExistedInAssetBundle("scriptableobjects", buildings[j].TypeName + "_level" + buildings[j].Level)) ? ("Buildings/" + buildings[j].TypeName + "_Level" + buildings[j].Level) : ("Buildings/" + buildings[j].TypeName + "_level" + buildings[j].Level));
			if (!hashSet.Contains(text))
			{
				hashSet.Add(text);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in hashSet)
		{
			stringBuilder.Append(item + ",");
		}
		TWDPlayerPrefs.SetString("PreloadPrefabs", stringBuilder.ToString());
	}

	public void SetRequestedConstructionBuilding(BuildingView building)
	{
		temporaryBuildingToConstruct = building;
		CampViewBuildings.SelectBuilding(temporaryBuildingToConstruct.gameObject);
		CampViewBuildings.PrepareCampForMovingBuilding();
	}

	public GameObject InstantiateCampBuilding(BuildingModel building)
	{
		GameObject gameObject = new GameObject("BuildingView_" + building.TypeName + " - " + building.ModelId);
		gameObject.transform.parent = Instance.BuildingsContainer;
		BuildingView buildingView = CreateBuildingViewComponentFromType(building.TypeName, gameObject);
		buildingView.Initialize(building);
		AddBuilding(buildingView);
		return gameObject;
	}

	private IEnumerator FadeTo(Renderer R, bool show, float time)
	{
		if ((bool)R)
		{
			Material mat = R.material;
			int prop = Shader.PropertyToID("_Color");
			Color currentColor = mat.GetColor(prop);
			float startAlpha = currentColor.a;
			float targetAlpha = (show ? 1f : 0f);
			float cumulTime = 0f;
			if (show)
			{
				R.enabled = true;
			}
			for (; cumulTime < time; cumulTime += Time.deltaTime)
			{
				currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, cumulTime / time);
				mat.SetColor(prop, currentColor);
				yield return null;
			}
			if (!show)
			{
				R.enabled = false;
			}
		}
	}

	public void ShowValidAreaGrid()
	{
		GameObject mesh = validAreaGrid.GetMesh();
		if (!mesh)
		{
			return;
		}
		Renderer component = mesh.GetComponent<Renderer>();
		if (base.gameObject.activeInHierarchy)
		{
			if (mFadeCoroutine != null)
			{
				StopCoroutine(mFadeCoroutine);
			}
			mFadeCoroutine = FadeTo(mesh.GetComponent<Renderer>(), show: true, validAreaGridFadeTime);
			StartCoroutine(mFadeCoroutine);
		}
		else if ((bool)component)
		{
			Material material = component.material;
			int nameID = Shader.PropertyToID("_Color");
			Color color = material.GetColor(nameID);
			color.a = 1f;
			material.SetColor(nameID, color);
			component.enabled = true;
		}
	}

	public void HideValidAreaGrid()
	{
		GameObject mesh = validAreaGrid.GetMesh();
		if (!mesh)
		{
			return;
		}
		Renderer component = mesh.GetComponent<Renderer>();
		if (base.gameObject.activeInHierarchy)
		{
			if (mFadeCoroutine != null)
			{
				StopCoroutine(mFadeCoroutine);
			}
			mFadeCoroutine = FadeTo(mesh.GetComponent<Renderer>(), show: false, validAreaGridFadeTime);
			StartCoroutine(mFadeCoroutine);
		}
		else if ((bool)component)
		{
			Material material = component.material;
			int nameID = Shader.PropertyToID("_Color");
			Color color = material.GetColor(nameID);
			color.a = 0f;
			material.SetColor(nameID, color);
			component.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (campModel != null)
		{
			campModel.Changed -= OnModelChange;
		}
		instance = null;
	}

	public void SetEnabled(bool enabled)
	{
		if (!enabled && CampViewActors != null)
		{
			CampViewActors.ResetActors();
		}
		base.gameObject.SetActive(enabled);
		SetEnabledCameraAndUI(enabled);
		if (enabled && GameManager.Instance.Blackboard.IsToggleOn("Toggle.CampMoved"))
		{
			float distance = CameraController.Distance;
			CameraController.Reset(TransformGridToWorldPosition(new GridPosition(base.Model.GridWidth / 2, base.Model.GridHeight / 2)), CameraController.MaxDistance);
			CameraController.StartPan(CameraController.TargetPosition, distance, 2f);
		}
		if (!enabled)
		{
			CampViewBuildings.UnselectBuilding();
		}
		if (enabled)
		{
			if (GameManager.Instance.playerModel.HasLootBoxesToOpen)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(GameManager.Instance.playerModel);
			}
			StartCoroutine(ShowCampNotifications());
			StartCoroutine(PreloadCampUIPrefabs());
			updateOutpostSeasonTimer = OutpostSeasonUpdateIntervalSeconds;
			updateOutpostTierTimer = OutpostTierUpdateIntervalSeconds;
		}
	}

	public void SetEnabledCameraAndUI(bool enabled)
	{
		CameraController.SetEnabled(enabled);
		if (Hud != null)
		{
			Hud.ShowCamp(enabled);
		}
	}

	private IEnumerator ShowCampNotification(string title, string message, UserNotificationViewedCommand.NotificationType type)
	{
		AlertPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
		if (confirmationPopup == null)
		{
			yield break;
		}
		confirmationPopup.SetContent(title, message);
		confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
		confirmationPopup.SetCallbacks(delegate
		{
			Helpers.ExecuteCommand(new UserNotificationViewedCommand(type));
			if (confirmationPopup != null)
			{
				confirmationPopup.Close();
			}
		});
		confirmationPopup.Open();
		while (confirmationPopup != null && confirmationPopup.IsOpen)
		{
			yield return null;
		}
	}

	private IEnumerator PreloadCampUIPrefabs()
	{
		UIType[] uiTypeList = new UIType[3]
		{
			UIType.CampTrainingGrounds,
			UIType.CampWorkshopPopup,
			UIType.CampCampMapHud
		};
		for (int i = 0; i < uiTypeList.Length; i++)
		{
			yield return new WaitForSeconds(1f);
			SingularityMonoBehaviour<HUDManager>.Instance.Load(uiTypeList[i]);
		}
	}

	private IEnumerator ShowCampNotifications()
	{
		yield return null;
		if (!TutorialView.Instance.Running)
		{
			if (GameManager.Instance.playerModel.ScrappedExcessItems)
			{
				string text = LocalizationManager.GetText("Popup.ScrappedExcessItems.Title");
				string text2 = LocalizationManager.GetText("Popup.ScrappedExcessItems.Message{maxItemCount}", GameManager.Instance.playerModel.gameEconomyData.ConfigData.MaxItemCount);
				yield return StartCoroutine(ShowCampNotification(text, text2, UserNotificationViewedCommand.NotificationType.AutoScrap));
			}
			if (GameManager.Instance.playerModel.MigratedAchievementRewards != null)
			{
				string rewardsCurrencyDescription = HelpersLocalization.GetRewardsCurrencyDescription(GameManager.Instance.playerModel.MigratedAchievementRewards);
				string text3 = LocalizationManager.GetText("Popup.AchievementsRemoved.Title");
				string text4 = LocalizationManager.GetText("Popup.AchievementsRemoved.Message{rewards}", rewardsCurrencyDescription);
				yield return StartCoroutine(ShowCampNotification(text3, text4, UserNotificationViewedCommand.NotificationType.AchievementMigration));
			}
			if (GameManager.Instance.playerModel.CombatAutoResolved)
			{
				string text5 = LocalizationManager.GetText("Popup.CombatTimedOut.Title");
				string text6 = LocalizationManager.GetText("Popup.CombatTimedOut.Message");
				yield return StartCoroutine(ShowCampNotification(text5, text6, UserNotificationViewedCommand.NotificationType.CombatAutoResolved));
			}
			if (GameManager.Instance.GuildManager != null)
			{
				yield return StartCoroutine(GameManager.Instance.GuildManager.SuggestionLogic.GuildSuggestionCheck(this));
			}
		}
	}

	private void Update()
	{
		if (base.IsInitialized && ActiveCamera.enabled)
		{
			UpdateSelection();
			if (!CameraController.Lerping && !CameraController.Scrolled)
			{
				_ = CameraController.Zoomed;
			}
			UpdateGuildNotifications();
			UpdateOutpostUnlockTutorial();
			UpdateOutpostSeasonAndTier();
			UpdateTradeGoodShop();
			CheckBlackMarketIsInitialized();
			CheckHillTopStoreIsInitialized();
		}
	}

	private void CheckHillTopStoreIsInitialized()
	{
		if (!GameManager.Instance.playerModel.HillTopStore.ContentInitialized)
		{
			Helpers.ExecuteCommand(new InitializeHillTopStoreCommand());
		}
	}

	private void CheckBlackMarketIsInitialized()
	{
		if (!GameManager.Instance.playerModel.BlackMarket.ContentInitialized)
		{
			Helpers.ExecuteCommand(new InitializeBlackMarketCommand());
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand
			{
				BlackboardToggle = "Toggle.BlackMarketNotifications"
			});
		}
	}

	private void UpdateTradeGoodShop()
	{
		if (GameManager.Instance.playerModel.GetTimeLeftToTradeShopRefresh() <= 0 && (lastTradeShopRefresh == 0f || Time.realtimeSinceStartup - lastTradeShopRefresh > 60f))
		{
			Helpers.ExecuteCommand(new RefreshTradeShopCommand());
			lastTradeShopRefresh = Time.realtimeSinceStartup;
		}
	}

	private void UpdateOutpostSeasonAndTier()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.OutpostEnabled || GameManager.Instance.gameEconomyData.ConfigData.DisableOutpostSeasons || !GameManager.Instance.playerModel.HasValidOutpost || TutorialView.Instance.Running || SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen > 0)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool flag = false;
		updateOutpostSeasonTimer += Time.deltaTime;
		if (updateOutpostSeasonTimer > OutpostSeasonUpdateIntervalSeconds)
		{
			updateOutpostSeasonTimer = 0f;
			if (playerModel.HasOutpostSeasonChanged() || playerModel.OutpostSeasonChanged)
			{
				Helpers.ExecuteCommand(new UpdateOutpostSeasonCommand());
				flag = playerModel.PreviousOutpostSeasonId != playerModel.CurrentOutpostSeasonId && playerModel.PreviousOutpostSeasonId != -1;
				SeasonChangePopup seasonChangePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostSeasonChangePopup) as SeasonChangePopup;
				if (seasonChangePopup != null)
				{
					seasonChangePopup.Open();
				}
			}
		}
		updateOutpostTierTimer += Time.deltaTime;
		if (!(updateOutpostTierTimer > OutpostTierUpdateIntervalSeconds))
		{
			return;
		}
		updateOutpostTierTimer = 0f;
		OutpostSeason outpostSeasonById = playerModel.gameEconomyData.GetOutpostSeasonById(playerModel.CurrentOutpostSeasonId);
		if (outpostSeasonById != null && !flag)
		{
			OutpostTier outpostInfluenceTier = playerModel.gameEconomyData.GetOutpostInfluenceTier(playerModel.RankingScore, outpostSeasonById.TierSetId);
			if (!playerModel.LastKnownOutpostTierId.Equals(outpostInfluenceTier.Id, StringComparison.InvariantCultureIgnoreCase))
			{
				Helpers.ExecuteCommand(new UpdateLastKnownOutpostTierCommand());
				string text = LocalizationManager.GetText(outpostInfluenceTier.LocalizationKey);
				hud.ShowOutpostNotification(LocalizationManager.GetText("Outpost.Notification.TierChange{NewTier}", text));
			}
		}
	}

	private void UpdateOutpostUnlockTutorial()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.OutpostEnabled || TutorialView.Instance.Running)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (!base.gameObject.activeSelf || !(TutorialView.Instance != null))
		{
			return;
		}
		if (playerModel.OutpostTutorialState != OutpostTutorialState.Done)
		{
			if (GameManager.Instance.playerModel.IsOutpostUnlocked)
			{
				if (playerModel.OutpostTutorialState == OutpostTutorialState.None)
				{
					ShowDialog("Portrait_Daryl", new List<string> { "Tutorial.OutpostTutorial.Initial.1", "Tutorial.OutpostTutorial.Initial.2" }, Instance.OutpostTutorialUnlockDialogOver);
					Helpers.ExecuteCommand(new OutpostTutorialProgressCommand(OutpostTutorialState.WaitingTutorialMissionCompletion));
					hud.OutpostTutorialProgressChanged();
				}
				else if (playerModel.OutpostTutorialState == OutpostTutorialState.WaitingTutorialMissionCompletion)
				{
					string outpostTutorialMissionId = GameManager.Instance.gameEconomyData.ConfigData.OutpostTutorialMissionId;
					if (outpostTutorialMissionId != null)
					{
						MapMissionModel missionModelForSpawnPoint = GameManager.Instance.playerModel.MapContainerModel.GetMissionModelForSpawnPoint(GameManager.Instance.gameEconomyData.MissionSpawnPointData.FindFirstSpawnPointByMissionId(outpostTutorialMissionId));
						if (missionModelForSpawnPoint != null && missionModelForSpawnPoint.IsCompleted)
						{
							ShowDialog("Portrait_Daryl", new List<string> { "Tutorial.OutpostTutorial.MissionCompleted.1", "Tutorial.OutpostTutorial.BuildOutpost.1" }, OutpostTutorialBuildOutpostAndCageDialogOver);
							Helpers.ExecuteCommand(new OutpostTutorialProgressCommand(OutpostTutorialState.WaitingForBuildings));
							BuildableBuildings.Update();
							hud.UpdateIndicators();
						}
					}
				}
				else if (playerModel.OutpostTutorialState == OutpostTutorialState.WaitingForBuildings && GameManager.Instance.playerModel.OutpostLevel > 0 && GameManager.Instance.playerModel.WalkerPitLevel > 0)
				{
					Helpers.ExecuteCommand(new OutpostTutorialProgressCommand(OutpostTutorialState.Done));
					SingularityMonoBehaviour<SDKManager>.Instance.OutpostBuilt();
				}
			}
			if (playerModel.OutpostTutorialState != OutpostTutorialState.None)
			{
				TutorialView.Instance.ShowButtonSuggest("OutpostButton", playerModel.OutpostTutorialState < OutpostTutorialState.WaitingForBuildings);
			}
		}
		else if (!TutorialView.Instance.IsDialogPlaying && !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.OutpostGiftSurvivorsGiven"))
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostUnlockReward).Open();
		}
	}

	public void OutpostTutorialBuildOutpostAndCageDialogOver()
	{
		CampHUD.ShowOutpostBuildMenuSuggestion();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampBuildMenu) as BuildMenu).Open();
	}

	public void OutpostTutorialSurvivorsGivenDialogOver()
	{
		hud.ShowOutpostMenuSuggetion(show: true);
	}

	public void OutpostTutorialUnlockDialogOver()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.OutpostTutorialState != OutpostTutorialState.None && playerModel.OutpostTutorialState != OutpostTutorialState.Done)
		{
			if (Instance != null && Instance.Model != null && TutorialView.Instance != null)
			{
				TutorialView.Instance.ShowButtonSuggest("MissionHub", show: true);
			}
			if (!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("OutpostMode"))
			{
				TutorialView.Instance.StartPart("OutpostMode");
			}
		}
	}

	private void UpdateGuildNotifications()
	{
		List<NotificationQueueItem> list = null;
		for (int i = 0; i < base.Model.NotificationQueue.Count; i++)
		{
			NotificationQueueItem notificationQueueItem = base.Model.NotificationQueue[i];
			if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildDemoted || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildKickedOut || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildPromoted || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildPromotedLeader || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildRequestAccepted || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildRequestRefused || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildPromotedLeaderByDemotion || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildDemotedDueInactivity)
			{
				string empty = string.Empty;
				if (notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildDemoted || notificationQueueItem.NotificationType == NotificationQueueItem.Type.GuildPromoted)
				{
					GuildMemberRole valueOrDefault = GameManager.Instance.guildModel.GetMemberRole(GameManager.Instance.playerModel.HashedId).GetValueOrDefault();
					empty = LocalizationManager.GetText("Notification." + notificationQueueItem.NotificationType.ToString() + valueOrDefault) + "\n";
				}
				else
				{
					empty = LocalizationManager.GetText("Notification." + notificationQueueItem.NotificationType) + "\n";
				}
				AlertPopup.ShowPopup(GameManager.Instance.GetFilteredText(notificationQueueItem.Name), empty, LocalizationManager.GetText("Button.Ok"));
				if (list == null)
				{
					list = new List<NotificationQueueItem>();
				}
				list.Add(notificationQueueItem);
			}
		}
		if (list != null)
		{
			for (int j = 0; j < list.Count; j++)
			{
				NotificationQueueItem item = list[j];
				base.Model.NotificationQueue.Remove(item);
			}
		}
	}

	private void UpdateSelection()
	{
		if (UICamera.isOverUI || !SelectEnabled)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (!CampViewBuildings.Moving)
			{
				touchDownPosition = Input.mousePosition;
				CampViewBuildings.StartBuildingMove();
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			Vector3 vector = touchDownPosition - mousePosition;
			if (!(temporaryBuildingToConstruct == null) || !(vector.magnitude < 10f))
			{
				return;
			}
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			if (mainCamera != null && Physics.Raycast(mainCamera.ScreenPointToRay(touchDownPosition), out var hitInfo, float.PositiveInfinity, 1))
			{
				if (hitInfo.collider.tag == "Building")
				{
					CampViewBuildings.SelectBuilding(hitInfo.collider.gameObject);
				}
				else if (TutorialView.Instance.MoveBuildingAllowed())
				{
					CampViewBuildings.UnselectBuilding();
				}
			}
			else if (!TutorialView.Instance.Running)
			{
				CampViewBuildings.UnselectBuilding();
			}
		}
		else if (Input.GetMouseButton(0) && CampViewBuildings.Moving)
		{
			CampViewBuildings.UpdateBuildingMove();
		}
	}

	public void AddBuilding(BuildingView buildingView)
	{
		CampViewBuildings.Buildings.Add(buildingView);
	}

	public void UpdateGridSpaceTranslation()
	{
		gridSpace.transform.localPosition = campModel.Grid.Position.ToVector3();
		BuildingsContainer.transform.localPosition = campModel.Grid.Position.ToVector3();
		Vector3 one = Vector3.one;
		one.x = (float)base.Model.Grid.CellSize.X;
		one.z = (float)base.Model.Grid.CellSize.Y;
		gridSpace.transform.localScale = one;
	}

	public GridPosition TransformGroundToGridPosition(Vector3 groundPosition)
	{
		Vector3 position = base.transform.TransformPoint(groundPosition);
		Vector3 vector = gridSpace.InverseTransformPoint(position);
		return new GridPosition(vector.x, vector.z);
	}

	public GridPosition TransformScreenToGridPosition(Vector2 screenPosition, bool floor)
	{
		Vector3 groundPosition = TransformScreenToGroundPosition(screenPosition);
		GridPosition gridPosition = TransformGroundToGridPosition(groundPosition);
		if (floor)
		{
			gridPosition.X = FixedPoint.Floor(gridPosition.X);
			gridPosition.Y = FixedPoint.Floor(gridPosition.Y);
		}
		return gridPosition;
	}

	public Vector3 TransformScreenToGroundPosition(Vector2 screenPosition)
	{
		Ray ray = mainCamera.ScreenPointToRay(screenPosition);
		if (!buildingsGroundCollider.Raycast(ray, out var hitInfo, 2000f))
		{
			Ray ray2 = ray;
			Debug.LogError("GetGroundPosition failed" + ray2.ToString());
			return Vector3.zero;
		}
		return buildingsGroundCollider.transform.InverseTransformPoint(hitInfo.point);
	}

	public Vector3 TransformGridToGroundPosition(GridPosition gridPosition)
	{
		Vector3 position = gridSpace.TransformPoint(new Vector3((float)gridPosition.X, 0f, (float)gridPosition.Y));
		return base.transform.InverseTransformPoint(position);
	}

	public Vector3 TransformGridToWorldPosition(GridPosition gridPosition)
	{
		return gridSpace.TransformPoint(new Vector3((float)gridPosition.X, 0f, (float)gridPosition.Y));
	}

	protected void OnModelChange(ModelObject m, string changed, object args)
	{
		switch (changed)
		{
		case "EventAddBuilding":
		{
			if (temporaryBuildingToConstruct != null)
			{
				CampViewBuildings.RemoveBuildingView(temporaryBuildingToConstruct);
			}
			temporaryBuildingToConstruct = null;
			BuildingModel buildingModel3 = args as BuildingModel;
			GameObject buildingGameObject = InstantiateCampBuilding(buildingModel3);
			if (!(buildingModel3 is VegetationModel))
			{
				CampViewBuildings.SelectBuilding(buildingGameObject, forcedSelection: true);
			}
			if (buildingModel3.TypeName == "Cage")
			{
				CampManager.Instance.CampBackground.ShowWalkerPitFill(show: false);
			}
			break;
		}
		case "EventLevelUpBuilding":
			ShowUpgradesCompletedNotification();
			if (args is BuildingModel { TypeName: "Council" } buildingModel2)
			{
				SingularityMonoBehaviour<SDKManager>.Instance.SentCouncilLevelData(buildingModel2.Level);
				GuildManager.CheckGvGDefenders(GameManager.Instance.playerModel);
				if (hud != null)
				{
					hud.UpdateSubscription();
					UIEvent.Send("ActivityIconRefreshEvent");
				}
				if (buildingsHud != null && campModel != null)
				{
					buildingsHud.RefreshExpansionIndicator(campModel);
				}
			}
			break;
		case "RemoveBuilding":
		{
			BuildingModel buildingModel = args as BuildingModel;
			if (buildingModel != null && buildingModel.BuildingType.Category == BuildingCategory.Vegetation)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/forest_cut");
			}
			CampViewBuildings.RemoveBuilding(buildingModel);
			if (CampViewBuildings.SelectedBuilding != null && buildingModel == CampViewBuildings.SelectedBuilding.Model)
			{
				CampViewBuildings.CancelBuildingMove();
			}
			EventManager.NotifyEvent(EventManager.EventType.CampVisualizationChanged);
			break;
		}
		case "MoveCampNotification":
			TutorialView.Instance.Say("Portrait_StoryTeller", "Popup.MoveCamp.FoundStartNewRoad");
			break;
		}
	}

	public void EnableCampControls(bool enable)
	{
		SelectEnabled = enable;
		CameraController.ScrollEnabled = enable;
	}

	public void EnableCameraControl(bool enable)
	{
		CameraController.ScrollEnabled = enable;
	}

	public void DisplayUpdateInfo()
	{
		if (TutorialView.Instance != null && TutorialView.Instance.Running)
		{
			return;
		}
		GameManager gameManager = GameManager.Instance;
		if (gameManager.playerModel.Tutorial.HasCompletedPart("EndTutorial"))
		{
			bool num = gameManager.gameEconomyData.GetFeature("UpdatePopup").Enabled;
			bool flag = !gameManager.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown");
			bool flag2 = !string.IsNullOrEmpty(gameManager.gameEconomyData.ConfigData.UpdateGift) && !gameManager.playerModel.Blackboard.IsToggleOn("Toggle.ToggleUpdateGiftReceived");
			if (num && (flag || flag2) && !TutorialView.WasStartedWithSkipCheat)
			{
				StartCoroutine(UpdateInfoDisplayLoop());
			}
		}
	}

	private IEnumerator UpdateInfoDisplayLoop()
	{
		yield return new WaitForSeconds(5f);
		float timeStarted = Time.time;
		while (Time.time - timeStarted < 300f)
		{
			if (SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen == 0 && !TutorialView.Instance.Running)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.UpdateInfoPopup).Open();
				break;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void DisplayBanner()
	{
		if (GameManager.Instance.BannerManager != null && GameManager.Instance.BannerManager.CanShowBannerEarlyCheck())
		{
			GameManager.Instance.BannerManager.LoadBannerImage();
			StartCoroutine(BannerDisplayLoop());
		}
	}

	private IEnumerator BannerDisplayLoop()
	{
		yield return new WaitForSeconds(5f);
		float timeStarted = Time.time;
		while (Time.time - timeStarted < 300f)
		{
			if (GameManager.Instance.BannerManager.CanShowBanner() && SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen == 0)
			{
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BannerAdPopup) as BannerAdPopup).Open();
				break;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void OnGUI()
	{
	}
}
