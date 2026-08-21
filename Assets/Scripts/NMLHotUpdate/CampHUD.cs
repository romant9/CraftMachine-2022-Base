using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CampHUD : HUDElement
{
	[Header("CampHUD")]
	[SerializeField]
	[Tooltip("Container for the non-HUD camp UI elements.")]
	private GameObject campUIContainer;

	[SerializeField]
	[Tooltip("Container for the camp hud.")]
	private GameObject campHudContainer;

	[SerializeField]
	[Tooltip("Container for the currency hud.")]
	private UIPanel campCurrencyHud;

	[Header("Global Elements")]
	[SerializeField]
	[Tooltip("Global Elements")]
	private GameObject globalElementsContainer;

	[SerializeField]
	private GameObject playerHubButton;

	[SerializeField]
	private GameObject challengeButton;

	[SerializeField]
	private GameObject challengeLockButton;

	[SerializeField]
	private UILabel challengeButtonLabel;

	[SerializeField]
	private ThingsToDoIndicator playerHubNotifications;

	[SerializeField]
	private GameObject socialButton;

	[SerializeField]
	private GameObject gvgChatButton;

	[SerializeField]
	private GameObject highScoreButton;

	[SerializeField]
	private GameObject campButtonTopRight;

	[Header("Top Left")]
	[SerializeField]
	private GameObject googlePlayButton;

	[SerializeField]
	private GameObject dailyQuestButton;

	[SerializeField]
	private GameObject playerLevel;

	[Header("Top Right")]
	[SerializeField]
	[Tooltip("Number of response received from the sharediscourd.")]
	private UILabel shareDiscourdNotificationLabel;

	[SerializeField]
	private ThingsToDoIndicator sharedDiscourdFreeContainer;

	[Header("Currency Meters")]
	[SerializeField]
	private HUDMeter suppliesMeter;

	[SerializeField]
	private HUDMeter survivalPointsMeter;

	[SerializeField]
	private HUDMeter diamondsMeter;

	[SerializeField]
	private HUDMeter tokensMeter;

	[SerializeField]
	private HUDMeter outpostTokensMeter;

	[SerializeField]
	private HUDMeter gvgMissionKeyMeter;

	[SerializeField]
	private HUDMeter gvgGasMeter;

	[SerializeField]
	private HUDMeter rewardPointsMeter;

	[SerializeField]
	private HUDMeter blackMarketTokensMeter;

	[SerializeField]
	private HUDMeter tradeFairTokensMeter;

	[SerializeField]
	private HUDMeter bluePrintTokensMeter;

	[SerializeField]
	private HUDMeter apocalypticEquipTokensMeter;

	[SerializeField]
	private HUDMeter survivalManualEXTokensMeter;

	[SerializeField]
	private HUDMeter endlessModeTokensMeter;

	[SerializeField]
	private HUDMeter endlessModeExpertTokensMeter;

	[SerializeField]
	private HUDMeter traitRerollTokensMeter;

	[SerializeField]
	private HUDMeter equipmentUpgradeTokensMeter;

	[SerializeField]
	private HUDMeter hillTopCoinMeter;

	[SerializeField]
	private HUDMeter SPTraitsUpgradeTokensMeter;

	[SerializeField]
	private HUDMeter worldBossExchangeCoinMeter;

	private List<HUDMeter> meters = new List<HUDMeter>();

	[Header("Camp Navigation")]
	[SerializeField]
	private UIButton mapButton;

	[SerializeField]
	private GameObject mapButtonIcon;

	[SerializeField]
	private GameObject buildMenuButton;

	[SerializeField]
	private GameObject TopLeftContainer;

	[SerializeField]
	private GameObject timedOfferContainer;

	[SerializeField]
	private GameObject newBieOfferContainer;

	[SerializeField]
	private GameObject DailyLoginCalendarContainer;

	[SerializeField]
	private GameObject achievementButton;

	[SerializeField]
	private UILabel achievementNotificationLabel;

	private float achievementNotificationDelay;

	[SerializeField]
	private ThingsToDoIndicator buildMenuButtonToDoIndicator;

	[SerializeField]
	private GameObject teamManagementButton;

	[SerializeField]
	private GameObject workshopButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon shopButton;

	[SerializeField]
	private GameObject BananaButton;

	[SerializeField]
	private UITexture BananaButtonTexture;

	[SerializeField]
	private GameObject cinemaAdsButton;

	[Header("Toolbag")]
	[SerializeField]
	private GameObject consumablesButton;

	[SerializeField]
	private ThingsToDoIndicator consumablesNotification;

	[SerializeField]
	private GameObject consumableNotificationContainer;

	[Header("Phone")]
	[SerializeField]
	private GameObject phoneButton;

	[SerializeField]
	private ThingsToDoIndicator phoneNumberNotification;

	[SerializeField]
	private GameObject specialNotificationContainer;

	[SerializeField]
	private UILabel specialNotificationLabel;

	[SerializeField]
	private GameObject settingsButton;

	[SerializeField]
	[Tooltip("When you collect some currency where does the flying currency icon flies to?")]
	private Transform[] collectAnimationDestinationForCurrencies;

	[SerializeField]
	private Transform collectAnimationTokenDestination;

	[SerializeField]
	private Transform collectAnimationComponentDestination;

	[SerializeField]
	private Transform collectAnimationGoldPhoneDestination;

	[SerializeField]
	[Tooltip("When you collect xp where does the flying xp icon flies to?")]
	private Transform collectAnimationDestinationForXp;

	[SerializeField]
	private Transform collectAnimationSuppliesDestination;

	[SerializeField]
	private Transform collectAnimationFairMoneyDestination;

	[SerializeField]
	private Transform collectAnimationBluePrintDestination;

	[SerializeField]
	private Transform collectAnimationWorkShopDestination;

	[SerializeField]
	private Transform collectAnimationHillTopDestination;

	[SerializeField]
	private Transform collectAnimationWorldBossExchangeDestination;

	[SerializeField]
	private Transform collectAnimationEnergyDestination;

	[SerializeField]
	private Transform collectAnimationGuildBattleRPDestination;

	[SerializeField]
	private Transform claimGuildGiftAnimationDestination;

	[SerializeField]
	private Transform claimBattleCurrencyAnimationDestination;

	[SerializeField]
	private Transform collectAnimationSPTraitsUpgradeTokensDestination;

	[SerializeField]
	[Tooltip("Number of response received from the helpshift.")]
	private ThingsToDoIndicator settingsNotifications;

	[SerializeField]
	private GameObject updateButton;

	[SerializeField]
	private UILabel updateTimerLabel;

	[Header("New GoldRadio Banner Notification")]
	[SerializeField]
	private GameObject newGoldRadioBannerNotificationContainer;

	[Header("HUD Top Right Elements")]
	[SerializeField]
	private GameObject sharedTopRightHudPanelContainer;

	[SerializeField]
	private GameObject campTopRightHudPanelContainer;

	[SerializeField]
	private GameObject gvgTopRightHudPanelContainer;

	[Header("Guild War")]
	[SerializeField]
	private GameObject guildWarMapIcon;

	[SerializeField]
	private GameObject guildBattleProgressBar;

	[SerializeField]
	private GameObject topRightGridContainer;

	[SerializeField]
	private UITable bottomLeftTable;

	[SerializeField]
	private int topRightGridPositionOffsetX = -100;

	private Vector3 topRightGridOriginLocalPosition = Vector3.zero;

	private bool subscribedToModelChange;

	private bool subscribedToGroupModelChange;

	private PlayerModel playerModel;

	private int lastTimeLeftUpdate;

	private static Callback AccessChallengesCallback;

	private static Callback AccessSurvivalCallback;

	private static Callback AccessOutpostsCallback;

	private static Callback AccessGvGBattleCallback;

	private Vector3 endlessMissionVector3;

	private Vector3 endlessVector3;

	private static Callback AccessEndlessModeCallback;

	private float _timeUntilNextCheckUnreadMessageCount;

	private const float _checkUnreadMessageCountIntervalSeconds = 1f;

	[SerializeField]
	public GameObject ItemListButton;

	[SerializeField]
	private GameObject unlockedEffect;

	[Tooltip("回归入口")]
	[SerializeField]
	private GameObject ReturnLogin;

	[SerializeField]
	private GameObject returnLoginFreeTag;

	public bool PauseCurrencyMeters { get; set; }

	public Transform GuildBattleProgressBarGetter => guildBattleProgressBar.transform;

	public Transform CollectAnimationDestinationForXp => collectAnimationDestinationForXp;

	private bool ShouldLoadAssetBundles
	{
		get
		{
			TrainingGroundBuildingModel obj = playerModel.Camp.GetBuilding("TrainingGround") as TrainingGroundBuildingModel;
			bool hasPendingSurvivor = playerModel.PhoneCall.HasPendingSurvivor;
			bool flag = obj?.UpgradedUnseenModel != null;
			bool valueOrDefault = obj?.UpgradingSurvivor?.TimedActionModel.IsActionUnderway() == true;
			bool flag2 = playerModel.Tutorial.CurrentPartId == "HeroUnlock";
			return hasPendingSurvivor || flag || valueOrDefault || flag2;
		}
	}

	public static CampHUD OpenHudPostCombat()
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			campHUD.Open();
			campHUD.HideAll();
			CurrencyHudSetActive(enable: true);
			campHUD.SetupTutorialHUD(GameManager.Instance.playerModel.Tutorial.GetCurrentStepDefinition);
		}
		return campHUD;
	}

	public static void CurrencyHudSetActive(bool enable)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.GetCurrencyHudUIPanel(), enable);
		}
	}

	public static void SetBlackMarketHudCurrencyVisibility(bool visibility)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.blackMarketTokensMeter, visibility);
			Helpers.GameObjectSetActive(campHUD.tradeFairTokensMeter, !visibility);
			if (visibility)
			{
				campHUD.blackMarketTokensMeter.SetValue(GameManager.Instance.playerModel.GetCurrency(CurrencyType.BlackMarketToken).Value);
			}
			else
			{
				campHUD.tradeFairTokensMeter.SetValue(GameManager.Instance.playerModel.GetCurrency(CurrencyType.Fairmoney).Value);
			}
			WorkshopPopup workshopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampWorkshopPopup) as WorkshopPopup;
			if (workshopPopup != null && workshopPopup.gameObject.activeInHierarchy)
			{
				Helpers.GameObjectSetActive(campHUD.tradeFairTokensMeter, value: false);
			}
		}
	}

	public static void SetBluePrintHudCurrencyVisibility(bool visibility)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.bluePrintTokensMeter, visibility);
			Helpers.GameObjectSetActive(campHUD.tokensMeter, !visibility);
			if (visibility)
			{
				campHUD.bluePrintTokensMeter.SetValue(GameManager.Instance.playerModel.GetCurrency(CurrencyType.BulePrintToken).Value);
			}
		}
	}

	public static void SetSPTraitsUpgradeTokensHudCurrencyVisibility(bool visibility)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.SPTraitsUpgradeTokensMeter, visibility);
			if (visibility)
			{
				campHUD.bluePrintTokensMeter.SetValue(GameManager.Instance.playerModel.GetCurrency(CurrencyType.SPTraitsUpgradeToken).Value);
			}
		}
	}

	public static void SetHillTopCoinHudCurrencyVisibility(bool visibility)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.hillTopCoinMeter, visibility);
			if (visibility)
			{
				campHUD.hillTopCoinMeter.SetValue(GameManager.Instance.playerModel.GetCurrency(CurrencyType.HillTopCoin).Value);
			}
		}
	}

	public static void SetSurvivalManualEXTokensCurrencyVisibility(bool visibility)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.survivalManualEXTokensMeter, visibility);
			if (visibility)
			{
				campHUD.survivalManualEXTokensMeter.SetValue(GameManager.Instance.playerModel.GetCurrency(CurrencyType.EXToken).Value);
			}
		}
	}

	public static void SetTopLeftContainerVisibility(bool visibility)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.TopLeftContainer, visibility);
		}
	}

	public static void SetTradeFairHudCurrencyVisibility(bool visibility)
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			Helpers.GameObjectSetActive(campHUD.tradeFairTokensMeter, visibility);
			if (visibility)
			{
				campHUD.tradeFairTokensMeter.SetValue(GameManager.Instance.playerModel.GetCurrency(CurrencyType.Fairmoney).Value);
			}
			WorkshopPopup workshopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampWorkshopPopup) as WorkshopPopup;
			if (workshopPopup != null && workshopPopup.gameObject.activeInHierarchy)
			{
				Helpers.GameObjectSetActive(campHUD.tradeFairTokensMeter, value: false);
			}
			TutorialModel tutorial = GameManager.Instance.playerModel.Tutorial;
			if (tutorial != null && !tutorial.StaticTutorialComplete && tutorial.CurrentPartId == "Phone")
			{
				Helpers.GameObjectSetActive(campHUD.tradeFairTokensMeter, value: false);
			}
		}
	}

	public UIPanel GetCurrencyHudUIPanel()
	{
		return campCurrencyHud;
	}

	private void Awake()
	{
		endlessMissionVector3 = new Vector3(-609f, -50f, 0f);
		endlessVector3 = new Vector3(-755f, -50f, 0f);
	}

	public override void Start()
	{
		base.Start();
		PauseCurrencyMeters = false;
		GameManager instance = GameManager.Instance;
		if (instance != null)
		{
			playerModel = instance.playerModel;
			playerModel.Changed += OnPlayerChange;
			playerModel.Camp.Changed += OnCampChange;
			if (playerModel.PhoneCall != null)
			{
				playerModel.PhoneCall.Changed += OnPhoneCallChange;
			}
			SubscribeToEvents();
			EventManager.OnEvent += OnEvent;
			instance.GuildManager.OnLoadGroupCompleted += OnLoadGroupCompleted;
			UpdateCurrencies();
			UpdateDiscord();
			UpdateGoldRadioBannerNotification();
			UpdateBananaButton();
			RecordTopRightGridPosition();
			UpdateItemListUI();
		}
		Helpers.GameObjectSetActive(googlePlayButton, value: false);
		SetSettingsNotifications(0);
	}

	public GameObject GetBananaButton()
	{
		return BananaButton;
	}

	private void UpdateBananaButton()
	{
		if (Helpers.GetBananaButtonSwitch() && shopButton.gameObject.activeSelf)
		{
			string contentPath = "Image/ydlBanana2023";
			if (!string.IsNullOrEmpty(Helpers.GetBananaEnterButtonIcon()))
			{
				contentPath = Helpers.GetBananaEnterButtonIcon();
			}
			LoadImageFromCdn.LoadImageToTarget(BananaButtonTexture, contentPath);
		}
		BananaButton.SetActive(value: false);
	}

	public void OnclickBananaButton()
	{
		BananaPopup bananaPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BananaPopup) as BananaPopup;
		if (bananaPopup != null)
		{
			bananaPopup.Open();
		}
	}

	public void UpdateDiscord()
	{
		Dictionary<ShareType, ShareModel> obtainedRewards = playerModel.ShareManagerModel.ObtainedRewards;
		bool active = false;
		if (obtainedRewards != null && obtainedRewards.Count == 0)
		{
			active = true;
			shareDiscourdNotificationLabel.text = LocalizationManager.GetText("Generic.Free");
		}
		sharedDiscourdFreeContainer.gameObject.SetActive(active);
	}

	public void UpdateGoldRadioBannerNotification()
	{
		if (playerModel != null && playerModel.EquipPrizeWheelModel != null)
		{
			bool flag = playerModel.EquipPrizeWheelModel.ShouldShowGoldRadioPoolRedDot();
			Helpers.GameObjectSetActive(newGoldRadioBannerNotificationContainer, flag);
			if (!flag)
			{
				SetPhonesNumber();
			}
		}
	}

	private void SubscribeToEvents()
	{
		if (playerModel != null)
		{
			if (!subscribedToModelChange)
			{
				playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed += OnGuildWarModelPlayerChanged;
				subscribedToModelChange = true;
			}
			if (!subscribedToGroupModelChange && playerModel.GuildWarModel != null)
			{
				playerModel.GuildWarModel.Changed += OnGuildWarChanged;
				subscribedToGroupModelChange = true;
			}
		}
	}

	private void UpdateGooglePlayButton()
	{
		if (GameManager.Instance != null && TutorialView.Instance != null)
		{
			bool value = !TutorialView.Instance.RunningButNotSuggesting && !GameManager.Instance.GameCenterManager.Authenticated;
			Helpers.GameObjectSetActive(googlePlayButton, value);
		}
	}

	public GameObject GetTeamManagementButton()
	{
		return teamManagementButton;
	}

	public HUDMeter GetHUDMeter(CurrencyType currencyType)
	{
		for (int i = 0; i < meters.Count; i++)
		{
			if (meters[i].CurrencyType == currencyType)
			{
				return meters[i];
			}
		}
		return null;
	}

	private void OnDestroy()
	{
		if (playerModel != null)
		{
			playerModel.Changed -= OnPlayerChange;
			if (playerModel.Camp != null)
			{
				playerModel.Camp.Changed -= OnCampChange;
			}
			if (playerModel.PhoneCall != null)
			{
				playerModel.PhoneCall.Changed -= OnPhoneCallChange;
			}
			playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed -= OnGuildWarModelPlayerChanged;
			if (playerModel.GuildWarModel != null)
			{
				playerModel.GuildWarModel.Changed -= OnGuildWarChanged;
			}
		}
		GameManager.Instance.GuildManager.OnLoadGroupCompleted -= OnLoadGroupCompleted;
		EventManager.OnEvent -= OnEvent;
		subscribedToGroupModelChange = false;
		subscribedToModelChange = false;
	}

	private void OnLoadGroupCompleted(bool success)
	{
		if (success)
		{
			SubscribeToEvents();
			SingularityMonoBehaviour<GuildWarManager>.Instance.UpdateBattleTimestamps();
		}
	}

	private void OnEnable()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HUDNotification).Open();
		UIEvent.OnUIEvent += OnUIEvent;
		meters.Clear();
		if (GameManager.Instance != null)
		{
			suppliesMeter.SetCurrencyType(CurrencyType.Supplies, formatValue: true);
			survivalPointsMeter.SetCurrencyType(CurrencyType.SurvivalPoints, formatValue: true);
			diamondsMeter.SetCurrencyType(CurrencyType.Diamonds);
			tokensMeter.SetCurrencyType(CurrencyType.ReplayToken, formatValue: true);
			outpostTokensMeter.SetCurrencyType(CurrencyType.Outpost, formatValue: true);
			gvgMissionKeyMeter.SetCurrencyType(CurrencyType.GvGMissionKey, formatValue: true);
			gvgGasMeter.SetCurrencyType(CurrencyType.GvGGas, formatValue: true);
			rewardPointsMeter.SetCurrencyType(CurrencyType.GuildBattleRP, formatValue: true);
			equipmentUpgradeTokensMeter.SetCurrencyType(CurrencyType.EquipmentUpgradeToken, formatValue: true);
			traitRerollTokensMeter.SetCurrencyType(CurrencyType.TraitRerollToken);
			blackMarketTokensMeter.SetCurrencyType(CurrencyType.BlackMarketToken);
			tradeFairTokensMeter.SetCurrencyType(CurrencyType.Fairmoney);
			apocalypticEquipTokensMeter.SetCurrencyType(CurrencyType.ApocalypticEquipToken);
			survivalManualEXTokensMeter.SetCurrencyType(CurrencyType.EXToken);
			endlessModeTokensMeter.SetCurrencyType(CurrencyType.EndlessPassToken);
			endlessModeExpertTokensMeter.SetCurrencyType(CurrencyType.EndlessPassExpertToken);
			hillTopCoinMeter.SetCurrencyType(CurrencyType.HillTopCoin);
			bluePrintTokensMeter.SetCurrencyType(CurrencyType.BulePrintToken);
			SPTraitsUpgradeTokensMeter.SetCurrencyType(CurrencyType.SPTraitsUpgradeToken);
			worldBossExchangeCoinMeter.SetCurrencyType(CurrencyType.WorldBossExchangeCoin);
		}
		meters.Add(suppliesMeter);
		meters.Add(survivalPointsMeter);
		meters.Add(diamondsMeter);
		meters.Add(tokensMeter);
		meters.Add(outpostTokensMeter);
		meters.Add(gvgMissionKeyMeter);
		meters.Add(gvgGasMeter);
		meters.Add(rewardPointsMeter);
		meters.Add(equipmentUpgradeTokensMeter);
		meters.Add(traitRerollTokensMeter);
		meters.Add(blackMarketTokensMeter);
		meters.Add(tradeFairTokensMeter);
		meters.Add(apocalypticEquipTokensMeter);
		meters.Add(survivalManualEXTokensMeter);
		meters.Add(endlessModeTokensMeter);
		meters.Add(endlessModeExpertTokensMeter);
		meters.Add(hillTopCoinMeter);
		meters.Add(bluePrintTokensMeter);
		meters.Add(SPTraitsUpgradeTokensMeter);
		meters.Add(worldBossExchangeCoinMeter);
		if (updateTimerLabel != null && GameManager.Instance.VersionValidUntil.HasValue && GameManager.Instance.VersionUpgradeNeeded)
		{
			Helpers.GameObjectSetActive(updateButton, value: true);
			updateTimerLabel.text = Helpers.FormatTimeNoZero((long)(GameManager.Instance.VersionValidUntil.Value - DateTime.UtcNow).TotalMilliseconds);
		}
		else
		{
			Helpers.GameObjectSetActive(updateButton, value: false);
		}
		SetupTutorialHUD(TutorialView.Instance.Model.GetCurrentStepDefinition);
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
		SingularityMonoBehaviour<GuildWarManager>.Instance.UpdateBattleTimestamps();
		Helpers.GameObjectSetActive(endlessModeTokensMeter.gameObject, value: false);
		Helpers.GameObjectSetActive(endlessModeExpertTokensMeter.gameObject, value: false);
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		if (AccessChallengesCallback != null)
		{
			AccessChallengesCallback = null;
		}
		if (AccessOutpostsCallback != null)
		{
			AccessOutpostsCallback = null;
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	public override void Update()
	{
		base.Update();
		CheckAchievementNotifications();
		UpdateTimeLeftToUpdate();
		UpdateTopRightGrid();
		UpdateSubscription();
		_timeUntilNextCheckUnreadMessageCount -= Time.deltaTime;
		if (!(_timeUntilNextCheckUnreadMessageCount <= 0f))
		{
			return;
		}
		_timeUntilNextCheckUnreadMessageCount = 1f;
		if (CampView.Instance != null && GameManager.Instance.PlayerHubManager != null)
		{
			int num = 0;
			if (SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager != null)
			{
				num = SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount;
			}
			playerHubNotifications.SetNumber(GameManager.Instance.PlayerHubManager.GetUnreadNewsNumber() + num + GameManager.Instance.PlayerHubManager.ActivityRedDotNum);
		}
		SetSettingsNotifications(SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount);
		ShowReturnLogin();
	}

	private void UpdateTimeLeftToUpdate()
	{
		if (updateTimerLabel != null && GameManager.Instance.VersionValidUntil.HasValue && GameManager.Instance.VersionUpgradeNeeded)
		{
			int num = Helpers.ConvertToSecondsNoZero((long)(GameManager.Instance.VersionValidUntil.Value - DateTime.UtcNow).TotalMilliseconds);
			if (num != lastTimeLeftUpdate)
			{
				lastTimeLeftUpdate = num;
				updateTimerLabel.text = Helpers.FormatTimeNoZero((long)num * 1000L);
			}
		}
	}

	private void CheckAchievementNotifications()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen != 0 || OfflineManager.IsTutorialDisable)
		{
			return;
		}
		achievementNotificationDelay -= Time.deltaTime;
		if (!(achievementNotificationDelay > 0f) && achievementButton.activeSelf && !IsTweenGroupEnabled(achievementButton, 0))
		{
			if (GameManager.Instance.playerModel.AchievementManager.HasNewAchievement)
			{
				Helpers.ExecuteCommand(new ChangeAchievementViewState(ViewStateChangeScope.AllAchievements, AchievementViewState.NewNotificationShown));
			}
			else if (GameManager.Instance.playerModel.AchievementManager.HasNewQuest)
			{
				ShowAchievementNotification(LocalizationManager.GetText("AchievementButton.Notification.NewQuest"));
				Helpers.ExecuteCommand(new ChangeAchievementViewState(ViewStateChangeScope.AllDailyQuests, AchievementViewState.NewNotificationShown));
			}
			else if (GameManager.Instance.playerModel.AchievementManager.HasAchievementCompleted)
			{
				ShowAchievementNotification(LocalizationManager.GetText("AchievementButton.Notification.AchievementCompleted"));
				Helpers.ExecuteCommand(new ChangeAchievementViewState(ViewStateChangeScope.AllAchievements, AchievementViewState.CompletedNotificationShown));
			}
			else if (GameManager.Instance.playerModel.AchievementManager.HasQuestCompleted)
			{
				ShowAchievementNotification(LocalizationManager.GetText("AchievementButton.Notification.QuestCompleted"));
				Helpers.ExecuteCommand(new ChangeAchievementViewState(ViewStateChangeScope.AllDailyQuests, AchievementViewState.CompletedNotificationShown));
			}
		}
	}

	public void ShowCamp(bool show)
	{
		PauseCurrencyMeters = false;
		mapButton.SetState(UIButtonColor.State.Normal, true);
		campUIContainer.SetActive(show);
		campHudContainer.SetActive(show);
		UpdateIndicators();
		GameManager.Instance.playerModel.Camp.InCamp = show;
		mapButton.SetState(UIButtonColor.State.Normal, true);
		if (show)
		{
			EndTweenGroupForGameObject(achievementButton, 0);
			achievementNotificationDelay = 2f;
			if (BuildConfiguration.Active.Branch != "develop" && GameManager.Instance.gameEconomyData.ConfigData.AskForGore && !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.GoreUsed"))
			{
				GoreSettingPopup goreSettingPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GoreSettingPopup) as GoreSettingPopup;
				if (goreSettingPopup != null)
				{
					goreSettingPopup.Open();
				}
			}
		}
		UpdateGenericElementsAfterChange();
		UpdateMapButtonVisualization();
		UpdateSubscription();
		ShowMissionHighLight();
		ShowReturnLogin();
		UIEvent.Send("ActivityIconRefreshEvent");
		if (show)
		{
			StartCoroutine(AskForIDFAConsent());
			Helpers.ReturnCamp();
		}
	}

	[ContextMenu("UpdateGenericElementsAfterChange")]
	public void UpdateGenericElementsAfterChange()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = false;
		bool flag10 = !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.CombatEndFlowThreeByThree) && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.RadioSelectSurvivorPopup);
		bool flag11 = false;
		bool flag12 = false;
		bool flag13 = false;
		bool flag14 = false;
		bool flag15 = GameManager.Instance.playerModel.Name != null || GameManager.Instance.playerModel.CouncilLevel >= GameManager.Instance.playerModel.gameEconomyData.ConfigData.GuildUnlockAtCouncilLevel;
		DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp;
		ScavengePopup scavengePopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ScavengePopup) as ScavengePopup;
		MissionHubPopup missionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.MissionHubPopup) as MissionHubPopup;
		OutpostPopup outpostPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OutpostPopup) as OutpostPopup;
		ActivityPopup activityPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ActivityPopup) as ActivityPopup;
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		GuildBattleEndPopup guildBattleEndPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleEndPopup) as GuildBattleEndPopup;
		GuildBattleSelectMissionPopup guildBattleSelectMissionPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleSelectMissionPopup) as GuildBattleSelectMissionPopup;
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ShopPopup) as ShopPopup;
		EndlessMissionHubPopup endlessMissionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.EndlessMissionHubPopup) as EndlessMissionHubPopup;
		EndlessNormalMissionHubPopup endlessNormalMissionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.EndlessNormalMissionHubPopup) as EndlessNormalMissionHubPopup;
		SocialPopupGuild socialPopupGuild = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.SocialPopupGuild) as SocialPopupGuild;
		bool flag16 = GameManager.Instance.IsConnectedToServer && GameManager.Instance.IsSocialEnabled();
		bool flag17 = GameManager.Instance.playerModel.Tutorial != null && GameManager.Instance.playerModel.Tutorial.StaticTutorialComplete;
		if (detailMapPopUp != null && detailMapPopUp.CurrentMap != null)
		{
			flag = detailMapPopUp.IsOpen;
			flag6 = detailMapPopUp.CurrentMap.IsWeeklyChallenge;
			flag7 = detailMapPopUp.CurrentMap.IsInApocalyptiWeeklyChallenge;
		}
		if (scavengePopup != null)
		{
			flag2 = scavengePopup.IsOpen;
		}
		if (missionHubPopup != null)
		{
			flag3 = missionHubPopup.IsOpen;
		}
		if (outpostPopup != null)
		{
			flag4 = outpostPopup.IsOpen;
		}
		if (activityPopup != null)
		{
			flag5 = activityPopup.IsOpen;
		}
		if (guildBattleMapPopup != null)
		{
			flag8 = guildBattleMapPopup.IsOpen;
		}
		if (guildBattleEndPopup != null)
		{
			flag8 |= guildBattleEndPopup.IsOpen;
		}
		if (guildBattleSelectMissionPopup != null)
		{
			flag9 = guildBattleSelectMissionPopup.IsOpen;
		}
		if (socialPopupGuild != null)
		{
			flag11 = socialPopupGuild.SelectedTab == 2 || socialPopupGuild.SelectedTab == 4;
		}
		if (shopPopup != null)
		{
			flag12 = shopPopup.IsOpen;
		}
		if (endlessMissionHubPopup != null)
		{
			flag13 = endlessMissionHubPopup.IsOpen;
		}
		if (endlessNormalMissionHubPopup != null)
		{
			flag14 = endlessNormalMissionHubPopup.IsOpen;
		}
		if (campUIContainer != null)
		{
			Helpers.GameObjectSetActive(playerHubButton, (campUIContainer.activeSelf || (flag && detailMapPopUp.MapCategory != MapCategory.Season) || flag2) && flag17 && !flag4 && !flag5);
			if (flag && (flag6 || flag7))
			{
				Helpers.GameObjectSetActive(playerHubButton, value: false);
			}
			Helpers.GameObjectSetActive(socialButton, (campUIContainer.activeSelf || flag6 || flag8) && flag17 && flag16 && !flag5 && !flag4 && !flag9 && flag15);
			Helpers.GameObjectSetActive(highScoreButton, (flag6 || flag7 || flag8) && flag17 && !flag9);
			Helpers.GameObjectSetActive(challengeButton, (flag6 || flag7) && flag17 && !flag9);
			if (!flag6)
			{
				challengeButton.GetComponent<UISprite>().color = Color.white;
			}
			else
			{
				challengeButton.GetComponent<UISprite>().color = Color.red;
			}
			SetChallengeButtonTxt((!flag6) ? LocalizationManager.GetText("Popup.MissionHub.Challenge.Title") : LocalizationManager.GetText("WeeklyChallenge.Difficulty_Apocalyptic.Title"));
			Helpers.GameObjectSetActive(campButtonTopRight, flag3 || (flag && detailMapPopUp.MapCategory != MapCategory.Season) || flag2 || flag4);
			Helpers.GameObjectSetActive(topRightGridContainer, campUIContainer.activeSelf && !flag4 && !flag9 && !flag && !flag2);
		}
		TutorialModel tutorial = GameManager.Instance.playerModel.Tutorial;
		bool value = true;
		if (tutorial != null && !tutorial.StaticTutorialComplete)
		{
			value = tutorial.ShowDailyQuestHud;
		}
		if (!flag10)
		{
			value = false;
		}
		Helpers.GameObjectSetActive(dailyQuestButton, value);
		Helpers.GameObjectSetActive(playerLevel, flag10);
		Helpers.GameObjectSetActive(gvgChatButton, flag8 && !flag9);
		Helpers.GameObjectSetActive(gvgTopRightHudPanelContainer, (flag8 || flag11) && !flag12);
		Helpers.GameObjectSetActive(sharedTopRightHudPanelContainer, value: true);
		Helpers.GameObjectSetActive(campTopRightHudPanelContainer, (!flag8 && !flag11) || flag12);
		Helpers.GameObjectSetActive(endlessModeTokensMeter, flag13 || flag3 || flag14);
		if (flag13 || flag14)
		{
			endlessModeTokensMeter.transform.localPosition = endlessVector3;
		}
		else
		{
			endlessModeTokensMeter.transform.localPosition = endlessMissionVector3;
		}
		Helpers.GameObjectSetActive(endlessModeExpertTokensMeter, flag13 || flag14);
		bool activeInHierarchy = (SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossMainPopup) as WorldBossMainPopup).gameObject.activeInHierarchy;
		Helpers.GameObjectSetActive(tokensMeter, !activeInHierarchy && !flag13 && !flag14);
		Helpers.GameObjectSetActive(SPTraitsUpgradeTokensMeter, !activeInHierarchy);
		SurvivorManagementPopUp survivorManagementPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds) as SurvivorManagementPopUp;
		WorkshopPopup workshopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampWorkshopPopup) as WorkshopPopup;
		Helpers.GameObjectSetActive(outpostTokensMeter, flag17 && !(survivorManagementPopUp.gameObject.activeInHierarchy || workshopPopup.gameObject.activeInHierarchy || activeInHierarchy) && !endlessModeTokensMeter.gameObject.activeInHierarchy);
		Helpers.GameObjectSetActive(traitRerollTokensMeter, survivorManagementPopUp.gameObject.activeInHierarchy);
		Helpers.GameObjectSetActive(equipmentUpgradeTokensMeter, workshopPopup.gameObject.activeInHierarchy);
		Helpers.GameObjectSetActive(apocalypticEquipTokensMeter, workshopPopup.gameObject.activeInHierarchy);
		Helpers.GameObjectSetActive(worldBossExchangeCoinMeter, activeInHierarchy);
		if (activeInHierarchy)
		{
			Helpers.GameObjectSetActive(endlessModeTokensMeter, value: false);
		}
		UpdateGuildBattleVisualization();
	}

	public void HideAll()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}

	public void UpdateIndicators()
	{
		if (CampView.Instance != null)
		{
			buildMenuButtonToDoIndicator.SetNumber(CampView.Instance.BuildableBuildings.NumberBuildableBuildings);
			CampView.Instance.ShowUpgradesCompletedNotification();
			if (GameManager.Instance.PlayerHubManager != null)
			{
				int num = 0;
				if (SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager != null)
				{
					num = SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount;
				}
				playerHubNotifications.SetNumber(GameManager.Instance.PlayerHubManager.GetUnreadNewsNumber() + num + GameManager.Instance.PlayerHubManager.ActivityRedDotNum);
			}
		}
		UpdateShopButtonIcon();
		SetConsumablesNumber();
	}

	public void UpdateShopButtonIcon()
	{
		if (shopButton != null)
		{
			string spriteName = "Ui_Icon_Shop";
			if (ShopPopupHelper.ContainsAnyFreeItems())
			{
				spriteName = "Ui_Icon_Gift";
			}
			shopButton.SetContentToIconOne(spriteName);
		}
	}

	public void OnGoToMission()
	{
	}

	public void OnClickCamp()
	{
		CampManager.Instance.GoToCamp();
		EventManager.NotifyClick("Camp");
	}

	public void OnGoToMap()
	{
		if (TutorialView.Allowed("MissionHub") && GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.FirstQuestAccepted)
		{
			if ((bool)mapButton && (bool)mapButton)
			{
				mapButton.SetState(UIButtonColor.State.Disabled, true);
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/view_change");
			EventManager.NotifyClick(EventManager.EventTypeClick.MissionHub);
			if ((bool)CampView.Instance && (bool)CampView.Instance.CampViewBuildings)
			{
				CampView.Instance.CampViewBuildings.UnselectBuilding();
			}
			MissionHubPopup.OpenPopup();
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
		}
	}

	public void ShowUpgradeAvailableInformation()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		OptionalUpdatePopup.OpenUpdateGiftContent();
	}

	public void OpenBuildMenu()
	{
		if (TutorialView.Allowed("BuildMenu"))
		{
			EventManager.NotifyClick("BuildMenu");
			BuildMenu buildMenu = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampBuildMenu) as BuildMenu;
			if (buildMenu != null)
			{
				buildMenu.Open();
			}
		}
	}

	public void OpenShop()
	{
		if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
		{
			EventManager.NotifyClick(EventManager.EventTypeClick.Shop);
			ShopPopupHelper.OpenWithIndex(0);
		}
	}

	public void UpdateCurrencies()
	{
		if (!PauseCurrencyMeters)
		{
			suppliesMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.Supplies).Max);
			suppliesMeter.SetValue(playerModel.GetCurrency(CurrencyType.Supplies).TotalValue);
			survivalPointsMeter.TimedBonusModel = playerModel.GetTimedBonus(TimedBonusType.DoubleXp);
			survivalPointsMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.SurvivalPoints).Max);
			survivalPointsMeter.SetValue(playerModel.GetCurrency(CurrencyType.SurvivalPoints).TotalValue);
			diamondsMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.Diamonds).Max);
			diamondsMeter.SetValue(playerModel.GetCurrency(CurrencyType.Diamonds).Value);
			tokensMeter.TimedBonusModel = playerModel.GetTimedBonus(TimedBonusType.UnlimitedGas);
			tokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.ReplayToken).Max);
			tokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.ReplayToken).Value);
			gvgMissionKeyMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.GvGMissionKey).Max);
			gvgMissionKeyMeter.SetValue(playerModel.GetCurrency(CurrencyType.GvGMissionKey).Value);
			gvgGasMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.GvGGas).Max);
			gvgGasMeter.SetValue(playerModel.GetCurrency(CurrencyType.GvGGas).Value);
			rewardPointsMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.GuildBattleRP).Max);
			rewardPointsMeter.SetValue(playerModel.GetCurrency(CurrencyType.GuildBattleRP).Value);
			outpostTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.Outpost).Max);
			outpostTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.Outpost).Value);
			equipmentUpgradeTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.EquipmentUpgradeToken).Max);
			equipmentUpgradeTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.EquipmentUpgradeToken).Value);
			traitRerollTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.TraitRerollToken).Max);
			traitRerollTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.TraitRerollToken).Value);
			blackMarketTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.BlackMarketToken).Max);
			blackMarketTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.BlackMarketToken).Value);
			tradeFairTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.Fairmoney).Max);
			tradeFairTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.Fairmoney).Value);
			apocalypticEquipTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.ApocalypticEquipToken).Max);
			apocalypticEquipTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.ApocalypticEquipToken).Value);
			survivalManualEXTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.EXToken).Max);
			survivalManualEXTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.EXToken).Value);
			endlessModeTokensMeter.SetMaxValue(playerModel.EndlessModeManager.GetMaxPasses());
			endlessModeTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.EndlessPassToken).Value);
			endlessModeExpertTokensMeter.SetMaxValue(playerModel.EndlessModeManager.GetMaxExpertPasses());
			endlessModeExpertTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.EndlessPassExpertToken).Value);
			hillTopCoinMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.HillTopCoin).Max);
			hillTopCoinMeter.SetValue(playerModel.GetCurrency(CurrencyType.HillTopCoin).Value);
			bluePrintTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.BulePrintToken).Max);
			bluePrintTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.BulePrintToken).Value);
			SPTraitsUpgradeTokensMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.SPTraitsUpgradeToken).Max);
			SPTraitsUpgradeTokensMeter.SetValue(playerModel.GetCurrency(CurrencyType.SPTraitsUpgradeToken).Value);
			worldBossExchangeCoinMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.WorldBossExchangeCoin).Max);
			worldBossExchangeCoinMeter.SetValue(playerModel.GetCurrency(CurrencyType.WorldBossExchangeCoin).Value);
			SetPhonesNumber();
		}
	}

	public void UpdateCurrencyConvertDiamonds()
	{
		diamondsMeter.SetMaxValue(playerModel.GetCurrency(CurrencyType.Diamonds).Max);
		diamondsMeter.SetValue(playerModel.GetCurrency(CurrencyType.Diamonds).Value);
	}

	public void SetConsumablesNumber()
	{
		int num = TWDPlayerPrefs.GetInt("NewSpeedUpTokenAcquiredAmount");
		int num2 = TWDPlayerPrefs.GetInt("NewConsumablesAcquiredAmount");
		if (num == 0 && num2 == 0 && (bool)consumableNotificationContainer)
		{
			consumableNotificationContainer.SetActive(value: false);
		}
		if (num > 0 || num2 > 0)
		{
			consumableNotificationContainer.SetActive(value: true);
			consumablesNotification.SetNumber(num + num2);
		}
	}

	public void SetPhonesNumber()
	{
		if (GameManager.Instance.playerModel.EquipPrizeWheelModel.ShouldShowGoldRadioPoolRedDot())
		{
			phoneNumberNotification.SetNumber(0);
			specialNotificationContainer.SetActive(value: false);
		}
		else if (GameManager.Instance.playerModel.PhoneCall.HasPendingSurvivor)
		{
			phoneNumberNotification.SetNumber(0);
			specialNotificationLabel.text = LocalizationManager.GetText("Hud.Button.Survivor");
			specialNotificationContainer.SetActive(value: true);
		}
		else if (GameManager.Instance.playerModel.PhoneCall.HasFreeCall())
		{
			phoneNumberNotification.SetNumber(0);
			specialNotificationLabel.text = LocalizationManager.GetText("Generic.Free");
			specialNotificationContainer.SetActive(value: true);
		}
		else
		{
			phoneNumberNotification.SetNumber(GameManager.Instance.playerModel.GetCurrency(CurrencyType.Phone).Value);
			specialNotificationContainer.SetActive(value: false);
		}
	}

	public void AddToMeter(CurrencyType currency, int amount)
	{
		HUDMeter hUDMeter = GetHUDMeter(currency);
		if (hUDMeter != null)
		{
			long value = Math.Min(hUDMeter.Value + amount, GameManager.Instance.playerModel.GetCurrency(currency).TotalValue);
			hUDMeter.SetValue(value);
		}
	}

	public void SetupTutorialHUD(TutorialStepDefinition tutorialStep = null)
	{
		TutorialModel tutorial = GameManager.Instance.playerModel.Tutorial;
		if (tutorial != null && !tutorial.StaticTutorialComplete)
		{
			Helpers.GameObjectSetActive(suppliesMeter, tutorial.ShowSuppliesHud);
			Helpers.GameObjectSetActive(survivalPointsMeter, TutorialView.Instance.Model.CurrentPartId != "Tutorial");
			Helpers.GameObjectSetActive(diamondsMeter, tutorial.ShowDiamondsHud);
			Helpers.GameObjectSetActive(SPTraitsUpgradeTokensMeter, tutorial.ShowDiamondsHud && TutorialView.Instance.Model.CurrentPartId != "Phone");
			Helpers.GameObjectSetActive(tokensMeter, tutorial.ShowGasHud);
			Helpers.GameObjectSetActive(apocalypticEquipTokensMeter, value: false);
			Helpers.GameObjectSetActive(survivalManualEXTokensMeter, value: false);
			Helpers.GameObjectSetActive(outpostTokensMeter, value: false);
			Helpers.GameObjectSetActive(equipmentUpgradeTokensMeter, value: false);
			Helpers.GameObjectSetActive(traitRerollTokensMeter, value: false);
			Helpers.GameObjectSetActive(worldBossExchangeCoinMeter, value: false);
			Helpers.GameObjectSetActive(settingsButton, tutorial.ShowDiamondsHud);
			Helpers.GameObjectSetActive(achievementButton, tutorial.ShowDiamondsHud);
			Helpers.GameObjectSetActive(teamManagementButton, tutorial.ShowDiamondsHud);
			Helpers.GameObjectSetActive(consumablesButton, tutorial.ShowDiamondsHud);
			Helpers.GameObjectSetActive(workshopButton, value: false);
			Helpers.GameObjectSetActive(playerHubButton, value: false);
			Helpers.GameObjectSetActive(gvgChatButton, value: false);
			Helpers.GameObjectSetActive(cinemaAdsButton, value: false);
			Helpers.GameObjectSetActive(endlessModeTokensMeter, value: false);
			Helpers.GameObjectSetActive(endlessModeExpertTokensMeter, value: false);
			showPhoneButtonInTutorial();
			Helpers.GameObjectSetActive(shopButton, tutorial.ShowDiamondsHud);
			Helpers.GameObjectSetActive(specialNotificationContainer, value: false);
			Helpers.GameObjectSetActive(BananaButton, value: false);
			if (tutorial.ShowDiamondsHud)
			{
				UpdateBananaButton();
			}
			Helpers.GameObjectSetActive(topRightGridContainer, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(phoneButton, value: true);
			SetPhonesNumber();
			Helpers.GameObjectSetActive(suppliesMeter, value: true);
			Helpers.GameObjectSetActive(survivalPointsMeter, value: true);
			Helpers.GameObjectSetActive(diamondsMeter, value: true);
			Helpers.GameObjectSetActive(tokensMeter, value: true);
			Helpers.GameObjectSetActive(endlessModeTokensMeter, value: true);
			Helpers.GameObjectSetActive(endlessModeExpertTokensMeter, value: false);
			Helpers.GameObjectSetActive(apocalypticEquipTokensMeter, value: false);
			Helpers.GameObjectSetActive(survivalManualEXTokensMeter, value: false);
			Helpers.GameObjectSetActive(equipmentUpgradeTokensMeter, value: false);
			Helpers.GameObjectSetActive(traitRerollTokensMeter, value: false);
			Helpers.GameObjectSetActive(worldBossExchangeCoinMeter, value: false);
			Helpers.GameObjectSetActive(settingsButton, value: true);
			Helpers.GameObjectSetActive(consumablesButton, value: true);
			Helpers.GameObjectSetActive(teamManagementButton, value: true);
			Helpers.GameObjectSetActive(shopButton, value: true);
			Helpers.GameObjectSetActive(cinemaAdsButton, value: false);
			SetSPTraitsUpgradeTokensHudCurrencyVisibility(visibility: true);
			UpdateBananaButton();
			if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Camp != null)
			{
				BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("Workshop");
				Helpers.GameObjectSetActive(workshopButton, building != null);
				Helpers.GameObjectSetActive(outpostTokensMeter, GameManager.Instance.playerModel.GetCurrency(CurrencyType.Outpost).Max > 0);
			}
			else
			{
				Helpers.GameObjectSetActive(workshopButton, value: false);
				Helpers.GameObjectSetActive(outpostTokensMeter, value: false);
			}
			Helpers.GameObjectSetActive(achievementButton, GameManager.Instance.playerModel.gameEconomyData.ConfigData.EnableAchievements);
			Helpers.GameObjectSetActive(topRightGridContainer, value: true);
		}
		if (tutorialStep == null)
		{
			Helpers.GameObjectSetActive(mapButton, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(mapButton, tutorialStep.ShowMapButton);
		}
		UpdateGenericElementsAfterChange();
		Helpers.GameObjectSetActive(buildMenuButton, value: true);
		SetSettingsNotifications(SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount);
	}

	public void EnableAllButtons(bool enable)
	{
		if ((bool)mapButton)
		{
			mapButton.gameObject.SetActive(enable);
		}
		if ((bool)buildMenuButton)
		{
			buildMenuButton.SetActive(enable);
		}
		Helpers.GameObjectSetActive(shopButton, enable);
		if (campUIContainer != null)
		{
			campUIContainer.SetActive(enable && GameManager.Instance.playerModel.Camp.InCamp);
		}
		if (TutorialView.Instance.RunningButNotSuggesting)
		{
			SetupTutorialHUD(TutorialView.Instance.Model.GetCurrentStepDefinition);
		}
		if (timedOfferContainer != null)
		{
			timedOfferContainer.SetActive(enable);
		}
		if (newBieOfferContainer != null)
		{
			newBieOfferContainer.SetActive(enable);
		}
		if ((bool)DailyLoginCalendarContainer)
		{
			DailyLoginCalendarContainer.SetActive(enable);
		}
		if ((bool)cinemaAdsButton)
		{
			cinemaAdsButton.SetActive(value: false);
		}
		ShowGenericElement(enable);
		if (enable)
		{
			UpdateGenericElementsAfterChange();
		}
	}

	public async void ShowInfo(int mmNum)
	{
		HUDNotification.Info(LocalizationManager.GetText("Notification.EquipmentAutoScrap"));
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
		switch (changed)
		{
		case "SpeedUpTokenAcquired":
		{
			int num2 = TWDPlayerPrefs.GetInt("NewSpeedUpTokenAcquiredAmount");
			TWDPlayerPrefs.SetInt("NewSpeedUpTokenAcquiredAmount", num2 + 1);
			break;
		}
		case "SpeedUpTokenUsed":
			TWDPlayerPrefs.SetInt("NewSpeedUpTokenAcquiredAmount", 0);
			break;
		case "ConsumableAcquired":
		{
			int num = TWDPlayerPrefs.GetInt("NewConsumablesAcquiredAmount");
			TWDPlayerPrefs.SetInt("NewConsumablesAcquiredAmount", num + 1);
			break;
		}
		}
		if (changed == "AutoScrapEquipmentMessage")
		{
			ShowInfo(2000);
			PauseCurrencyMeters = false;
			UpdateCurrencies();
		}
		switch (changed)
		{
		case "currencyChangedEvent":
			UpdateCurrencies();
			break;
		case "CurrencyConvertToDiamondsEvent":
			UpdateCurrencyConvertDiamonds();
			break;
		case "guildChanged":
			SetupTutorialHUD(TutorialView.Instance.Model.GetCurrentStepDefinition);
			break;
		case "name":
		case "TradeShopSlotBought":
		case "TradeShopRefreshed":
		case "TradeShopItemBought":
			UpdateIndicators();
			break;
		}
	}

	private void OnCampChange(ModelObject m, string changed, object args)
	{
		if (changed == "EventLevelUpBuilding")
		{
			SetupTutorialHUD();
		}
	}

	private void OnPhoneCallChange(ModelObject m, string changed, object args)
	{
		if (changed == "EventNewFreeCallAvailable" || changed == "CallMade")
		{
			SetupTutorialHUD();
		}
		if (changed == "EventPendingSurvivorCleared")
		{
			SetPhonesNumber();
		}
	}

	private void OnGuildWarModelPlayerChanged(ModelObject m, string changed, object args)
	{
		if (changed == "GuildWarStarted")
		{
			UpdateMapButtonVisualization();
		}
	}

	private void OnGuildWarChanged(TWDGroupModelChild twdGroupModelChild, string changed, object args)
	{
		if (changed == "GuildBattleEnded" || changed == "GuildBattleStarted")
		{
			UpdateGuildBattleVisualization();
		}
	}

	public void OnClickDiamondsPlus()
	{
		OpenDialogByItem("Diamonds");
	}

	public void OnClickGuildPetrol()
	{
		GuildShopPopup.OpenGuildShop();
	}

	public void OnClickBluePrint()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopup).Close();
		NewPhonePopup.OpenRadiophoneFeaturePopup();
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup).OnClickWeapon();
	}

	public void OnClickTradeFairPlus()
	{
		if ((!(SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatEndFlowThreeByThree) != null) || !SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatEndFlowThreeByThree).IsOpen) && TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()) && GameManager.Instance.gameEconomyData?.ConfigData != null)
		{
			ShopPopupHelper.OpenWithIndex(2);
		}
	}

	private void OnGetTransferCode(string message)
	{
		if (CheckError(message))
		{
			return;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
		if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			string bananaURL = Helpers.GetBananaURL();
			if (playerModel != null && playerModel.HashedId != null)
			{
				string text = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
				string deviceId = GameManager.Instance.LoginRequest.Device.DeviceId;
				bananaURL = bananaURL + "?id=" + text + "&code=" + transferCode.Code + "&DeviceId=" + deviceId + "&OS=" + Helpers.GetPlatformName(Application.platform);
				Application.OpenURL(bananaURL);
			}
		}
		else
		{
			CheckError("");
		}
	}

	private bool CheckError(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}

	public void OnClickSettings()
	{
		if (TutorialView.Instance.Allow("Settings"))
		{
			SettingsPopup settingsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SettingsPopup) as SettingsPopup;
			if (settingsPopup != null)
			{
				settingsPopup.Open();
				settingsPopup.SetHelpNotification(SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount);
			}
		}
	}

	public void OnClickPhone()
	{
		NewPhonePopup.OpenRadiophoneFeaturePopup();
	}

	public void OnClickTrainingGround()
	{
		HandleClickTrainingGround();
	}

	public static void HandleClickTrainingGround()
	{
		if (TutorialView.Allowed("TentsButton"))
		{
			TrainingGroundView trainingGroundView = CampView.Instance.CampViewBuildings.FindBuildingViewOfType<TrainingGroundView>() as TrainingGroundView;
			if (trainingGroundView != null && trainingGroundView.GetDoneIndicator() != null && (TutorialView.Instance == null || TutorialView.Instance.Model == null || (TutorialView.Instance.Model.CurrentPartId != "HeroUnlock" && TutorialView.Instance.Model.CurrentPartId != "HeroPromote")))
			{
				trainingGroundView.GetDoneIndicator().OnClickUpgradedSurvivor();
			}
			else
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds).Open();
			}
			EventManager.NotifyClick("TentsButton");
		}
	}

	public void OnClickConsumables()
	{
		HUDManager.TryOpenPopup(UIType.ConsumablesCampPopup);
	}

	public void OnClickWorkshop()
	{
		if (TutorialView.Allowed("WorkshopButton"))
		{
			WorkshopView workshopView = CampView.Instance.CampViewBuildings.FindBuildingViewOfType<WorkshopView>() as WorkshopView;
			if (workshopView != null && workshopView.GetDoneIndicator() != null)
			{
				workshopView.GetDoneIndicator().OnClickUpgradedSurvivor();
			}
			else
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampWorkshopPopup).Open();
			}
			EventManager.NotifyClick("WorkshopButton");
		}
	}

	private void showPhoneButtonInTutorial()
	{
		TutorialModel tutorial = GameManager.Instance.playerModel.Tutorial;
		bool active = true;
		if (tutorial != null)
		{
			active = tutorial.HasCompletedPart("Tutorial_Training_Ground") || tutorial.HasCompletedPart("RewardsScreen2");
		}
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.RadioSelectSurvivorPopup);
		if (noCreation != null && noCreation.IsOpen)
		{
			active = false;
		}
		phoneButton.SetActive(active);
	}

	public void OnClickHighscore()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleMapPopup))
		{
			HUDManager.TryOpenPopup(UIType.GuildBattleHighscorePopup);
			return;
		}
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HighscorePopup);
		if (hUDElement != null)
		{
			hUDElement.Open();
		}
	}

	public void OnClickGuild()
	{
		OpenGuildOrChallenge(UIType.SocialPopupGuild);
	}

	public static void TryToAccessChallenges(Callback successCallback)
	{
		AccessChallengesCallback = successCallback;
		if (GameManager.Instance.GuildManager.IsBusy)
		{
			AlertPopup.ShowPopup("", LocalizationManager.GetText("Popup.SocialLoading.Message"), LocalizationManager.GetText("Button.Ok"));
		}
		else if (WeeklyChallengeHelper.IsLockedByCouncilLevelOrTutorial())
		{
			AccessChallengesCallback = null;
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Challenge, locked: true);
		}
		else if (!GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled)
		{
			AccessChallengesCallback = null;
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Alert.NotAvailableTitle"), LocalizationManager.GetText("Popup.Alert.NotAvailableMessage"), LocalizationManager.GetText("Button.Ok"));
		}
		else if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.ChallengeUnlockedSeen"))
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Challenge, locked: false, CheckNameBeforeChallenges);
		}
		else
		{
			CheckNameBeforeChallenges();
		}
	}

	public static void TryToAccessSurvival(Callback successCallback)
	{
		AccessSurvivalCallback = successCallback;
		if (WeeklySurvivalHelper.IsLockedByCouncilLevelOrTutorial())
		{
			AccessSurvivalCallback = null;
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Survival, locked: true);
		}
		else if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.SurvivalUnlockedSeen"))
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Survival, locked: false, CheckNameBeforeSurvival);
		}
		else
		{
			CheckNameBeforeSurvival();
		}
	}

	public static void TryToAccessGuildBattle(Callback successCallback, bool isSpectator)
	{
		AccessGvGBattleCallback = successCallback;
		if (!GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled)
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Alert.NotAvailableTitle"), LocalizationManager.GetText("Popup.Alert.NotAvailableMessage"), LocalizationManager.GetText("Button.Ok"));
			return;
		}
		if (GameManager.Instance.GuildManager.IsBusy)
		{
			AlertPopup.ShowPopup("", LocalizationManager.GetText("Popup.SocialLoading.Message"), LocalizationManager.GetText("Button.Ok"));
			return;
		}
		if (GuildWarHelper.IsLockedByCouncilLevelOrTutorial())
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.GuildBattle, locked: true);
			AccessGvGBattleCallback = null;
			return;
		}
		if (GuildWarHelper.IsBattleOnGoingForPlayer() || GuildWarHelper.CanShowBattleEnd())
		{
			CheckNameBeforeGuildBattle();
			return;
		}
		if (!GuildWarHelper.IsWarOngoing())
		{
			AccessGvGBattleCallback = null;
			HUDManager.TryOpenPopup(UIType.GuildBattleOverviewPopup);
			return;
		}
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.BattlePass);
		GuildWarHelper.GetRegisteredPlayersCountForBattleTimeSlot();
		_ = gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
		if (currency.Value == 0 && GuildWarHelper.IsBattleOnGoing() && !isSpectator)
		{
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GvG.OutOfBattlePasses.Title"), LocalizationManager.GetText("Popup.GvG.OutOfBattlePasses"), LocalizationManager.GetText("Button.Spectate"), CheckNameBeforeGuildBattle, LocalizationManager.GetText("Button.Back"));
		}
		else
		{
			CheckNameBeforeGuildBattle();
		}
	}

	public static void TryToAccessEndlessMode(Callback successCallback)
	{
		AccessEndlessModeCallback = successCallback;
		if (EndlessModeHelpers.IsLockedByCouncilLevel())
		{
			AccessEndlessModeCallback = null;
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.EndlessMode, locked: true);
		}
		else if (!EndlessModeHelpers.IsEndlessModeActive())
		{
			AccessEndlessModeCallback = null;
		}
		else
		{
			CheckNameBeforeEndless();
		}
		if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.EndlessModeUnlockedSeen") && EndlessModeHelpers.IsEndlessModeActive())
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.EndlessMode, locked: false);
		}
	}

	private static void CheckNameBeforeChallenges()
	{
		OpenIfPlayerHasName(UIType.None, AccessChallengesSuccess, AccessChallengesCancel);
	}

	private static void CheckNameBeforeOutposts()
	{
		OpenIfPlayerHasName(UIType.None, AccessOutpostsSuccess, AccessOutpostsCancel);
	}

	private static void CheckNameBeforeSurvival()
	{
		OpenIfPlayerHasName(UIType.None, AccessSurvivalSuccess, AccessSurvivalCancel);
	}

	private static void CheckNameBeforeGuildBattle()
	{
		OpenIfPlayerHasName(UIType.None, AccessGvGBattleSuccess, AccessGvGBattleCancel);
	}

	private static void CheckNameBeforeEndless()
	{
		OpenIfPlayerHasName(UIType.None, AccessEndlessModeSuccess, AccessEndlessModeCancel);
	}

	private static void AccessChallengesSuccess(UIType popupType)
	{
		ProcessAccessSuccessInternal(AccessChallengesCallback);
	}

	private static void AccessChallengesCancel(UIType popupType)
	{
		ProcessAccessCancelInternal(AccessChallengesCallback);
	}

	private static void AccessOutpostsSuccess(UIType popupType)
	{
		ProcessAccessSuccessInternal(AccessOutpostsCallback);
	}

	private static void AccessOutpostsCancel(UIType popupType)
	{
		ProcessAccessCancelInternal(AccessOutpostsCallback);
	}

	private static void AccessSurvivalSuccess(UIType popupType)
	{
		ProcessAccessSuccessInternal(AccessSurvivalCallback);
	}

	private static void AccessSurvivalCancel(UIType popupType)
	{
		ProcessAccessCancelInternal(AccessSurvivalCallback);
	}

	private static void AccessGvGBattleSuccess(UIType popupType)
	{
		ProcessAccessSuccessInternal(AccessGvGBattleCallback);
	}

	private static void AccessGvGBattleCancel(UIType popupType)
	{
		ProcessAccessCancelInternal(AccessGvGBattleCallback);
	}

	private static void AccessEndlessModeSuccess(UIType popupType)
	{
		ProcessAccessSuccessInternal(AccessEndlessModeCallback);
	}

	private static void AccessEndlessModeCancel(UIType popupType)
	{
		ProcessAccessCancelInternal(AccessEndlessModeCallback);
	}

	private static void ProcessAccessCancelInternal(Callback callback)
	{
		if (callback != null)
		{
			callback = null;
		}
	}

	private static void ProcessAccessSuccessInternal(Callback callback)
	{
		if (callback != null)
		{
			callback();
			callback = null;
		}
		else
		{
			Debug.LogError("Could not access. Callback was NULL!");
		}
	}

	public void OutpostTutorialProgressChanged()
	{
		SetupTutorialHUD(TutorialView.Instance.Model.GetCurrentStepDefinition);
	}

	public void OnClickOutpostManagement()
	{
		TryOpenOutpostTutorial(OpenOutpostPopupAfterChecks);
	}

	public static void OpenOutpostPopupAfterChecks()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopup);
		if (hUDElement != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			hUDElement.Open();
		}
	}

	public static void TryOpenOutpostTutorial(Callback successCallback)
	{
		AccessOutpostsCallback = successCallback;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.OutpostTutorialState == OutpostTutorialState.None)
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Outpost, locked: true);
			AccessOutpostsCallback = null;
		}
		else if (playerModel.OutpostTutorialState != OutpostTutorialState.Done && playerModel.OutpostTutorialState == OutpostTutorialState.WaitingTutorialMissionCompletion)
		{
			if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.ToggleOutpostUnlockedSeen"))
			{
				FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Outpost, locked: false, PlayOutopstTutorialMission);
			}
			else
			{
				PlayOutopstTutorialMission();
			}
			AccessOutpostsCallback = null;
		}
		else if (OutpostPopup.HasBuildingAndCorrectLevelToEdit() && !GameManager.Instance.Blackboard.IsToggleOn("Toggle.ToggleOutpostEditUnlockedSeen"))
		{
			FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.OutpostEdit, locked: false, TryOpenOutpostPopup);
		}
		else
		{
			TryOpenOutpostPopup();
		}
	}

	private static void PlayOutopstTutorialMission()
	{
		MapMissionModel mapMissionModel = null;
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		MissionSpawnPoint missionSpawnPoint = gameEconomyData.MissionSpawnPointData.FindFirstSpawnPointByMissionId(gameEconomyData.ConfigData.OutpostTutorialMissionId);
		if (missionSpawnPoint != null)
		{
			mapMissionModel = GameManager.Instance.playerModel.MapContainerModel.GetMissionModelForSpawnPoint(missionSpawnPoint);
		}
		if (mapMissionModel != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
			obj.SurvivorType = SurvivorContainerModel.SurvivorType.Combat;
			obj.OpenForModel(mapMissionModel);
			EventManager.NotifyClick("SelectTeam");
		}
	}

	private static void TryOpenOutpostPopup()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.OutpostTutorialState == OutpostTutorialState.None)
		{
			AccessOutpostsCallback = null;
			return;
		}
		if (playerModel.OutpostTutorialState == OutpostTutorialState.Done)
		{
			HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OutpostPopup);
			if (noCreation == null || !noCreation.IsOpen)
			{
				CheckNameBeforeOutposts();
			}
			return;
		}
		AccessOutpostsCallback = null;
		bool flag = GameManager.Instance.playerModel.OutpostLevel > 0;
		bool flag2 = GameManager.Instance.playerModel.WalkerPitLevel > 0;
		if (!flag || !flag2)
		{
			ShowOutpostBuildMenuSuggestion();
			if (!flag)
			{
				CampView.Instance.ShowDialog("Portrait_Daryl", new List<string> { "Tutorial.OutpostTutorial.BuildOutpost.1" }, CampView.Instance.OutpostTutorialBuildOutpostAndCageDialogOver);
			}
			else
			{
				CampView.Instance.ShowDialog("Portrait_Daryl", new List<string> { "Tutorial.OutpostTutorial.BuildWalkerPit.1" }, CampView.Instance.OutpostTutorialBuildOutpostAndCageDialogOver);
			}
		}
	}

	public static void ShowOutpostBuildMenuSuggestion()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.OutpostTutorialState != OutpostTutorialState.None && playerModel.OutpostTutorialState != OutpostTutorialState.Done && CampView.Instance != null && CampView.Instance.Model != null)
		{
			List<string> availableBuildingsToBuild = CampView.Instance.Model.GetAvailableBuildingsToBuild();
			TutorialView.Instance.ShowButtonSuggest("BuildMenu", playerModel.OutpostTutorialState == OutpostTutorialState.WaitingForBuildings && (availableBuildingsToBuild.Contains("Outpost") || availableBuildingsToBuild.Contains("Cage")));
		}
	}

	public void ShowOutpostMenuSuggetion(bool show)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.OutpostTutorialState != OutpostTutorialState.None && playerModel.OutpostTutorialState != OutpostTutorialState.Done && CampView.Instance != null && CampView.Instance.Model != null)
		{
			TutorialView.Instance.ShowButtonSuggest("MissionHub", show);
		}
	}

	public void OnClickGooglePlay()
	{
		GameManager.Instance.GameCenterManager.PromptGameCenterConnect(comingFromSettings: true);
	}

	public void OnClickAchievement()
	{
		NewBieQuestsPopup newBieQuestsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewBieQuestsPopup) as NewBieQuestsPopup;
		if (newBieQuestsPopup != null && newBieQuestsPopup.IsOpen)
		{
			newBieQuestsPopup.Close();
		}
		if (!DailyQuestModel.GetIsSupported(GameManager.Instance.gameEconomyData))
		{
			AchievementPopup.OpenAchievement();
		}
		else
		{
			QuestsPopup.OpenQuestsPopup();
		}
	}

	public void OnClickPlayerHub()
	{
		GameManager.Instance.PlayerHubManager.OpenNewsletter();
		UpdateIndicators();
	}

	public void OnClickChallenge()
	{
		if (WeeklyChallengeHelper.IsNormalChallenge)
		{
			if (!GameManager.Instance.gameEconomyData.ConfigData.ApocalypticChallengeSwitch)
			{
				HUDNotification.Info(LocalizationManager.GetText("Tips.ChallengeMode.SwitchOff"));
			}
			else
			{
				MissionHubNavigation.TryOpenApocalypticChallengeMap();
			}
		}
		else if (!GameManager.Instance.gameEconomyData.ConfigData.ChallengeNormalSwitch)
		{
			HUDNotification.Info(LocalizationManager.GetText("Tips.ChallengeMode.SwitchOff"));
		}
		else
		{
			MissionHubNavigation.TryOpenChallengeMap();
		}
	}

	public void OnClickChallengeLock()
	{
		HUDNotification.Info(LocalizationManager.GetText("Tips.ApocalpticChallengeMode.Locked"));
	}

	public void ShowChallengeLock(bool flag)
	{
		if (challengeLockButton != null)
		{
			challengeLockButton.gameObject.SetActive(flag);
		}
	}

	public void SetChallengeButtonTxt(string txt)
	{
		challengeButtonLabel.text = txt;
	}

	public void OnClickNewsLetter()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BannerAdPopup).Open();
	}

	public void OnClickUpdateInfo()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.UpdateInfoPopup).Open();
	}

	public static void OpenGuildOrChallenge(UIType popupType)
	{
		if (GameManager.Instance.GuildManager.GuildOffline)
		{
			AlertPopup.ShowPopupGetText("Generic.Info", "Error.GuildLoadTimeout", "Button.Ok", null);
		}
		else if (GameManager.Instance.GuildManager.IsBusy)
		{
			AlertPopup.ShowPopup("", LocalizationManager.GetText("Popup.SocialLoading.Message"), LocalizationManager.GetText("Button.Ok"));
		}
		else
		{
			OpenIfPlayerHasName(popupType, OnNameSubmitComplete);
		}
	}

	public static void OpenIfPlayerHasName(UIType popupType, EnterNamePopup.Callback OnSubmitComplete, EnterNamePopup.Callback OnCancel = null)
	{
		if (string.IsNullOrEmpty(GameManager.Instance.playerModel.Name))
		{
			EnterNamePopup enterNamePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialEnterName) as EnterNamePopup;
			if (enterNamePopup != null)
			{
				enterNamePopup.PopupToOpenOnConfirm = popupType;
				enterNamePopup.OnSubmitCallback = OnSubmitComplete;
				enterNamePopup.OnCancelCallback = OnCancel;
				enterNamePopup.Open();
			}
		}
		else if (popupType != UIType.None)
		{
			OnSubmitComplete(popupType);
		}
		else
		{
			OnSubmitComplete?.Invoke(UIType.None);
		}
	}

	private static void OnNameSubmitComplete(UIType popupType)
	{
		if (popupType != UIType.None)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(popupType).Open();
		}
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		switch (eventType)
		{
		case EventManager.EventType.CampMovedVisited:
			SetupTutorialHUD();
			break;
		case EventManager.EventType.CampVisualizationChanged:
			UpdateIndicators();
			break;
		case EventManager.EventType.TutorialPartOver:
			StartCoroutine(AskForIDFAConsent());
			break;
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnBuildingMoveCancelled":
		case "OnBuildingMoveEnded":
			EnableAllButtons(enable: true);
			ShowOutpostBuildMenuSuggestion();
			break;
		case "OnBuildingConstructionStartPlacing":
		case "OnBuildingMoveStarted":
			EnableAllButtons(enable: false);
			break;
		case "OnPopUpOpen":
			UpdateGenericElementsAfterChange();
			if (CampView.Instance != null && !TutorialView.Instance.RunningButNotSuggesting)
			{
				CampView.Instance.CancelBuildingPlacement();
			}
			break;
		case "OnPopUpClose":
			UpdateGenericElementsAfterChange();
			break;
		case "GuildTabChanged":
			UpdateGenericElementsAfterChange();
			break;
		case "OnBundleBought":
			UpdateIndicators();
			break;
		case "SubscriptionEndEvent":
			UpdateSubscription();
			break;
		case "CampTopRightFreshEvent":
			FreshTopRightGrid();
			break;
		case "CampBottomLeftFreshEvent":
			FreshBottomLeftGrid();
			break;
		case "MarkGoldRadioBanner":
			UpdateGoldRadioBannerNotification();
			break;
		case "OnBuildingMoveConfirmed":
			break;
		}
	}

	public Transform GetCollectAnimationDestination(CurrencyType currencyType)
	{
		if (GameManager.Instance.gameEconomyData.IsToken(currencyType) || GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(currencyType) || currencyType == CurrencyType.BounsItem)
		{
			return collectAnimationTokenDestination;
		}
		if (ComponentHelper.IsComponentCurrency(currencyType))
		{
			return collectAnimationComponentDestination;
		}
		switch (currencyType)
		{
		case CurrencyType.CampaignToken:
			return collectAnimationSuppliesDestination;
		case CurrencyType.GuildBattleRP:
			return collectAnimationGuildBattleRPDestination;
		case CurrencyType.GvGGas:
			return collectAnimationEnergyDestination;
		case CurrencyType.FreeGuildGiftPerk:
			return claimGuildGiftAnimationDestination;
		case CurrencyType.BattlePassPoints:
			return claimBattleCurrencyAnimationDestination;
		case CurrencyType.Fairmoney:
			return collectAnimationFairMoneyDestination;
		case CurrencyType.BulePrintToken:
			return collectAnimationBluePrintDestination;
		case CurrencyType.ApocalypticEquipToken:
			return collectAnimationWorkShopDestination;
		case CurrencyType.HillTopCoin:
			return collectAnimationHillTopDestination;
		case CurrencyType.WorldBossExchangeCoin:
			return collectAnimationWorldBossExchangeDestination;
		case CurrencyType.GoldRadio:
			return collectAnimationGoldPhoneDestination;
		case CurrencyType.SPTraitsUpgradeToken:
			return collectAnimationSPTraitsUpgradeTokensDestination;
		default:
		{
			int num = Math.Min((int)currencyType, collectAnimationDestinationForCurrencies.Length - 1);
			return collectAnimationDestinationForCurrencies[num];
		}
		}
	}

	public void ShowOutpostNotification(string text)
	{
	}

	public void ShowAchievementNotification(string text)
	{
		if (achievementNotificationLabel != null)
		{
			achievementNotificationLabel.text = text;
		}
		UITweener[] componentsInChildren = achievementButton.GetComponentsInChildren<UITweener>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].tweenGroup == 0)
			{
				componentsInChildren[i].enabled = true;
			}
		}
		TweenManager.PlayTweenGroup(achievementButton, 0);
	}

	public static void EndTweenGroupForGameObject(GameObject button, int tweenGroup)
	{
		UITweener[] componentsInChildren = button.GetComponentsInChildren<UITweener>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].tweenGroup == tweenGroup && componentsInChildren[i].enabled)
			{
				componentsInChildren[i].ResetToEnd();
				componentsInChildren[i].enabled = false;
			}
		}
	}

	public static bool IsTweenGroupEnabled(GameObject button, int tweenGroup)
	{
		UITweener[] componentsInChildren = button.GetComponentsInChildren<UITweener>(includeInactive: false);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].tweenGroup == tweenGroup && componentsInChildren[i].enabled)
			{
				return true;
			}
		}
		return false;
	}

	public static void PlayTweenGroupInGameObject(GameObject button, int tweenGroup, EventDelegate.Callback callback = null)
	{
		if (!(button != null))
		{
			return;
		}
		UITweener[] componentsInChildren = button.GetComponentsInChildren<UITweener>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].tweenGroup == tweenGroup)
			{
				componentsInChildren[i].enabled = true;
			}
		}
		TweenManager.PlayTweenGroup(button, tweenGroup, forward: true, callback);
	}

	public void SetSettingsNotifications(int amount)
	{
		amount = ((amount >= 0) ? amount : 0);
		settingsNotifications.SetNumber(amount);
	}

	public void ShowcampHudContainer(bool show)
	{
		campHudContainer.SetActive(show);
	}

	public bool GetCampHudContainerShowState()
	{
		return campHudContainer.activeInHierarchy;
	}

	public void ShowcampUiContainer(bool show)
	{
		campUIContainer.SetActive(show);
	}

	public void ShowGenericElement(bool show)
	{
		Helpers.GameObjectSetActive(globalElementsContainer, show);
	}

	public static CampHUD Get()
	{
		return SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
	}

	private void UpdateMapButtonVisualization()
	{
		if (GuildWarHelper.IsWarOngoingForPlayer() || GuildWarHelper.IsBattleOnGoing())
		{
			Helpers.GameObjectSetActive(mapButtonIcon, value: false);
			Helpers.GameObjectSetActive(guildWarMapIcon.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(mapButtonIcon, value: true);
			Helpers.GameObjectSetActive(guildWarMapIcon.gameObject, value: false);
		}
	}

	private void UpdateGuildBattleVisualization()
	{
		if (GuildWarHelper.IsBattleOnGoing())
		{
			Helpers.GameObjectSetActive(guildBattleProgressBar, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(guildBattleProgressBar, value: false);
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		SetPhonesNumber();
	}

	private IEnumerator AskForIDFAConsent()
	{
		if (Application.platform != RuntimePlatform.Android && GameManager.Instance.IsIDFACheckEnabled())
		{
			yield return null;
			PlayerModel playerModel = GameManager.Instance.playerModel;
			if (SingularityMonoBehaviour<HUDManager>.Instance.OpenPopups.Count <= 0 && playerModel.Tutorial.CurrentPartId == "Tutorial_Training_Ground" && playerModel.Tutorial.CurrentStep == playerModel.Tutorial.CurrentPartDefinition.Steps.Count - 1)
			{
				ShowIDFARequestPopup();
			}
		}
	}

	private void ShowIDFARequestPopup()
	{
		if (!SingularityMonoBehaviour<SDKManager>.Instance.SkAdNetworkController.HasAnsweredIDFAPopup())
		{
			GameObject prefabVariant = SingularityMonoBehaviour<GdprFlowHandler>.Instance.GetCurrentIDFAPopup().gameObject;
			IDFARequestPopup iDFARequestPopup = (IDFARequestPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IDFARequestPopup, null, createIfNotExist: true, prefabVariant);
			if (iDFARequestPopup != null)
			{
				iDFARequestPopup.Initialize(2);
				iDFARequestPopup.Open();
			}
		}
	}

	public void OnClickThreeDay()
	{
	}

	public void OnClickActiveFoundation()
	{
	}

	public void OnClickSubscription()
	{
	}

	private void RecordTopRightGridPosition()
	{
		if (topRightGridContainer != null)
		{
			topRightGridOriginLocalPosition = topRightGridContainer.transform.localPosition;
		}
	}

	private void UpdateTopRightGrid()
	{
		if (!(topRightGridContainer == null))
		{
			if (BananaButton != null && BananaButton.activeInHierarchy)
			{
				Vector3 localPosition = new Vector3(topRightGridOriginLocalPosition.x + (float)topRightGridPositionOffsetX, topRightGridOriginLocalPosition.y, topRightGridOriginLocalPosition.z);
				topRightGridContainer.transform.localPosition = localPosition;
			}
			else
			{
				topRightGridContainer.transform.localPosition = topRightGridOriginLocalPosition;
			}
		}
	}

	public void UpdateSubscription()
	{
	}

	private void FreshTopRightGrid()
	{
		if (topRightGridContainer != null)
		{
			UITable component = topRightGridContainer.GetComponent<UITable>();
			if (component != null)
			{
				component.Reposition();
			}
		}
	}

	private void FreshBottomLeftGrid()
	{
		if (bottomLeftTable != null)
		{
			bottomLeftTable.Reposition();
		}
	}

	public void OnClickItemList()
	{
		ItemListPopup itemListPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ItemListPopup) as ItemListPopup;
		if (itemListPopup != null)
		{
			itemListPopup.Open();
		}
		UpdateItemListUI();
	}

	public void UpdateItemListUI()
	{
		if (!(ItemListButton == null))
		{
			Transform transform = ItemListButton.transform.Find("Notice");
			Helpers.GameObjectSetActive(transform.gameObject, value: false);
			if (!Helpers.IsItemListOpened())
			{
				Helpers.GameObjectSetActive(transform.gameObject, value: true);
			}
		}
	}

	public Vector3 GetDiamondsMeterV()
	{
		return new Vector3(diamondsMeter.transform.position.x, diamondsMeter.transform.position.y, diamondsMeter.transform.position.z);
	}

	public void OnClickSuppliesPlus()
	{
		OpenDialogByItem("Supplies");
	}

	public void OnClickSurvivalPointsPlus()
	{
		OpenDialogByItem("SurvivalPoints");
	}

	public void OnClickReplayTokenPlus()
	{
		OpenDialogByItem("ReplayToken");
	}

	public void OnClickOutpostPlus()
	{
		OpenDialogByItem("Outpost");
	}

	public void OnClickBlackMarketTokenPlus()
	{
		OpenDialogByItem("BlackMarketToken");
	}

	public void OnClickEquipmentUpgradeTokenPlus()
	{
		OpenDialogByItem("EquipmentUpgradeToken");
	}

	public void OnClickTraitRerollTokenPlus()
	{
		OpenDialogByItem("TraitRerollToken");
	}

	public void OnClickApocalypticEquipTokenPlus()
	{
		OpenDialogByItem("ApocalypticEquipToken");
	}

	public void OnClickEquipTraitsRemodelTokenPlus()
	{
		OpenDialogByItem("EquipTraitsRemodelToken");
	}

	public void OpenDialogByItem(string itemName)
	{
		if (GameManager.Instance.gameEconomyData == null || (SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatEndFlowThreeByThree) != null && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatEndFlowThreeByThree).IsOpen))
		{
			return;
		}
		switch (GameManager.Instance.gameEconomyData.GetItemGetDefinition(itemName))
		{
		case ItemGetType.Resources:
			if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
				ShopPopupHelper.OpenWithIndex(2);
			}
			break;
		case ItemGetType.Scavenge:
			if (!SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.ScavengePopup))
			{
				if (GameManager.Instance.playerModel.Tutorial.HasCompletedPart("EndTutorial"))
				{
					MissionHubNavigation.OpenScavenge();
				}
				else if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					ShopPopupHelper.OpenWithIndex(2);
				}
			}
			break;
		case ItemGetType.Missions:
			switch (itemName)
			{
			case "Outpost":
				if (!GameManager.Instance.playerModel.IsOutpostUnlocked)
				{
					if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
					{
						SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
						ShopPopupHelper.OpenWithIndex(2);
					}
				}
				else if (!SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.MissionHubPopup))
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					OnGoToMap();
				}
				break;
			case "EquipmentUpgradeToken":
			case "TraitRerollToken":
				if (!GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled || WeeklyChallengeHelper.IsLockedByCouncilLevelOrTutorial())
				{
					if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
					{
						SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
						ShopPopupHelper.OpenWithIndex(2);
					}
				}
				else if (!SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.MissionHubPopup))
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					OnGoToMap();
				}
				break;
			case "ApocalypticEquipToken":
				if (EndlessModeHelpers.IsLockedByCouncilLevel())
				{
					if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
					{
						SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
						ShopPopupHelper.OpenWithIndex(2);
					}
				}
				else if (!SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.MissionHubPopup))
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					OnGoToMap();
				}
				break;
			default:
				if (!SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.MissionHubPopup))
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					OnGoToMap();
				}
				break;
			}
			break;
		case ItemGetType.BlackMarket:
			if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
				ShopPopupHelper.OpenWithIndex(4);
			}
			break;
		case ItemGetType.None:
			break;
		}
	}

	private void ShowMissionHighLight()
	{
		Helpers.GameObjectSetActive(unlockedEffect, FeatureUIHighlights.IsActive(FeatureUIHighlights.FeaturesIds.SeasonModeUnlocked));
	}

	public void OnClickSurvivalManual()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		ShopPopupHelper.OpenWithIndex(0);
	}

	private void ShowReturnLogin()
	{
		ReturnActivityManager returnActivityManager = GameManager.Instance?.playerModel?.ReturnActivityManager;
		bool flag = returnActivityManager != null && (returnActivityManager.IsReturnActivityAvailable() || returnActivityManager.IsReturnExchangeAvailable());
		Helpers.GameObjectSetActive(ReturnLogin, flag);
		Helpers.GameObjectSetActive(returnLoginFreeTag, flag && returnActivityManager.HasRedDot);
		CheckReturnLogin();
	}

	private void CheckReturnLogin()
	{
		if (!(ReturnLogin == null) && ReturnLogin.activeInHierarchy)
		{
			ReturnLoginModel returnLoginModel = GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnLogin;
			if (!TutorialView.Instance.Running && returnLoginModel != null && returnLoginModel.ShouldPopupOnCurrentLogin)
			{
				OnClickReturnLogin();
				Helpers.ExecuteCommand(new MarkReturnLoginPopupShownCommand());
			}
		}
	}

	public void OnClickReturnLogin()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ReturnLoginPopup)?.Open();
	}
}
