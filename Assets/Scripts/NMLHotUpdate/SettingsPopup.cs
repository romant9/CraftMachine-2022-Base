using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using Fabric;
using TWDModel;
using UnityEngine;

public class SettingsPopup : HUDElement
{
	[SerializeField]
	[Header("Game Settings")]
	private UIToggle goreToggle;

	[SerializeField]
	private UIToggle vsyncToggle;

	[SerializeField]
	private UISlider soundFxSlider;

	[SerializeField]
	private GameObject soundFxOnGo;

	[SerializeField]
	private GameObject soundFxOffGo;

	[SerializeField]
	private UISlider MusicSlider;

	[SerializeField]
	private GameObject MusicOnGo;

	[SerializeField]
	private GameObject MusicOffGo;

	[SerializeField]
	private UIToggle autoCoverToggle;

	[SerializeField]
	private GameObject framteRatePrefab;

	[SerializeField]
	private GameObject framteRateScrollContainer;

	[SerializeField]
	private UIScrollView framteRateScrollView;

	[SerializeField]
	private UITable framteRateTable;

	[SerializeField]
	private UILabel currentFramteRate;

	private List<GameObject> framteRates;

	[SerializeField]
	private UIToggle combatGridToggle;

	[SerializeField]
	private UIToggle combatCameraToggle;

	[SerializeField]
	private UIToggle blackMarketNotificationsToggle;

	[SerializeField]
	private GameObject linkDeviceButton;

	[SerializeField]
	private GameObject languagePrefab;

	[SerializeField]
	private GameObject languageScrollContainer;

	[SerializeField]
	private UIScrollView languageScrollView;

	[SerializeField]
	private UITable languageTable;

	[SerializeField]
	private UILabel currentLanguage;

	private List<GameObject> languages;

	[SerializeField]
	private GameObject displayModePrefab;

	[SerializeField]
	private GameObject displayModeScrollContainer;

	[SerializeField]
	private UIScrollView displayModeScrollView;

	[SerializeField]
	private UITable displayModeTable;

	[SerializeField]
	private UILabel currentDisplayMode;

	private List<GameObject> displayModes;

	[SerializeField]
	private GameObject screenResolutionPrefab;

	[SerializeField]
	private GameObject screenResolutionScrollContainer;

	[SerializeField]
	private UIScrollView screenResolutionScrollView;

	[SerializeField]
	private UITable screenResolutionTable;

	[SerializeField]
	private UILabel currentScreenResolution;

	private List<GameObject> screenResolutions;

	[SerializeField]
	private GameObject autoScrapPrefab;

	[SerializeField]
	private GameObject autoScrapScrollContainer;

	[SerializeField]
	private UIScrollView autoScrapScrollView;

	[SerializeField]
	private UITable autoScrapTable;

	[SerializeField]
	private UILabel currentAutoScrap;

	private List<GameObject> autoScrapS;

	private List<string> availableautoScrapS = new List<string> { "Popup.Settings.AutoScrap.Off", "Popup.Settings.AutoScrap.ThreeStars", "Popup.Settings.AutoScrap.FourStars", "Popup.Settings.AutoScrap.FiveStars" };

	[SerializeField]
	private UIToggle targetedAdsToggle;

	[Header("Account Linking - General")]
	[SerializeField]
	private GameObject accountLinkingInfo;

	[SerializeField]
	private UILabel accountLinkingTitleLabel;

	[SerializeField]
	private UILabel accountLinkingStatusLabel;

	[SerializeField]
	private GameObject accountLinkingInfoIconSaved;

	[SerializeField]
	private GameObject accountLinkingInfoIconNotSaved;

	[Header("Account Linking - Game Center")]
	[SerializeField]
	private GameObject gameCenterContainer;

	[SerializeField]
	private UIButton gameCenterButton;

	[SerializeField]
	private UILabel gameCenterButtonLabel;

	[SerializeField]
	private GameObject gameCenterButtonBackground;

	[Header("Account Linking - Google Play")]
	[SerializeField]
	private GameObject googlePlayContainer;

	[SerializeField]
	private UILabel googlePlayButtonLabel;

	[SerializeField]
	private GameObject googlePlayButtonBackgroundGreen;

	[SerializeField]
	private GameObject googlePlayButtonBackgroundGrey;

	[SerializeField]
	[Tooltip("Number of response received from the helpshift.")]
	private ThingsToDoIndicator helpNotification;

	[SerializeField]
	private GameObject giftCodeRedeemButton;

	private bool UsingGameCenter = true;

	private float _timeUntilNextCheckUnreadMessageCount;

	private const float _checkUnreadMessageCountIntervalSeconds = 1f;

	private bool gameSaved => true;

	private bool connected => true;

	private bool hasOtherGame => false;

	public void Awake()
	{
		languages = new List<GameObject>();
		displayModes = new List<GameObject>();
		screenResolutions = new List<GameObject>();
		framteRates = new List<GameObject>();
		autoScrapS = new List<GameObject>();
	}

	public override void Start()
	{
		soundFxSlider.value = GameManager.Instance.Settings.SoundFxVolume;
		soundFxSlider.onDragFinished = OnSoundFxSliderDragFinished;
		if (GameManager.Instance.Settings.SoundFxVolume > 0f)
		{
			soundFxOnGo.SetActive(value: true);
			soundFxOffGo.SetActive(value: false);
		}
		else
		{
			soundFxOnGo.SetActive(value: false);
			soundFxOffGo.SetActive(value: true);
		}
		MusicSlider.value = GameManager.Instance.Settings.MusicVolume;
		if (GameManager.Instance.Settings.MusicVolume > 0f)
		{
			MusicOnGo.SetActive(value: true);
			MusicOffGo.SetActive(value: false);
		}
		else
		{
			MusicOnGo.SetActive(value: false);
			MusicOffGo.SetActive(value: true);
		}
		goreToggle.value = !GameManager.Instance.IsGoreDisabled;
		vsyncToggle.value = GameManager.Instance.Settings.VSync;
		targetedAdsToggle.value = GameManager.Instance.playerModel.HasAcceptedGdprAction("TargetedAdsConsent");
		autoCoverToggle.value = !GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.AutoCoverDisabled");
		combatGridToggle.value = GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleCombatGridEnabled");
		combatCameraToggle.value = GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleCombatCameraEnabled");
		blackMarketNotificationsToggle.value = GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.BlackMarketNotifications");
		if (GameConfiguration.Instance.Config.LowViolence)
		{
			Helpers.GameObjectSetActive(goreToggle, value: false);
		}
		InvokeRepeating("UpdateAccountState", 1f, 1f);
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
		linkDeviceButton.SetActive(!GameManager.Instance.gameEconomyData.ConfigData.DisableLinkDevice);
		languageScrollContainer.SetActive(value: false);
		displayModeScrollContainer.SetActive(value: false);
		screenResolutionScrollContainer.SetActive(value: false);
		framteRateScrollContainer.SetActive(value: false);
		autoScrapScrollContainer.SetActive(value: false);
	}

	public override void UpdateUI()
	{
		if (languageScrollView != null)
		{
			for (int i = 0; i < languages.Count; i++)
			{
				UnityEngine.Object.Destroy(languages[i]);
			}
			languages.Clear();
			List<string> uISupportedLanguages = GameManager.Instance.gameEconomyData.ConfigData.UISupportedLanguages;
			languageScrollView.ResetPosition();
			for (int j = 0; j < uISupportedLanguages.Count; j++)
			{
				GameObject gameObject = Helpers.InstantiateToParent(languagePrefab, languageScrollView.gameObject);
				gameObject.GetComponent<LanguageItem>().SetKey(this, uISupportedLanguages[j]);
				languages.Add(gameObject);
			}
			languageScrollView.ResetPosition();
			languageTable.repositionNow = true;
		}
		currentLanguage.text = LocalizationManager.GetText("LanguageName." + SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage.ToLower());
		if (autoScrapScrollView != null)
		{
			for (int k = 0; k < autoScrapS.Count; k++)
			{
				UnityEngine.Object.Destroy(autoScrapS[k]);
			}
			autoScrapS.Clear();
			autoScrapScrollView.ResetPosition();
			for (int l = 0; l < availableautoScrapS.Count; l++)
			{
				GameObject gameObject2 = Helpers.InstantiateToParent(autoScrapPrefab, autoScrapScrollView.gameObject);
				gameObject2.GetComponent<AutoScrapItem>().SetKey(this, availableautoScrapS[l]);
				autoScrapS.Add(gameObject2);
			}
			autoScrapScrollView.ResetPosition();
			autoScrapTable.repositionNow = true;
			AutoScrapEquipmentType isEquipmentAutoScrap = GameManager.Instance.playerModel.IsEquipmentAutoScrap;
			string text = "";
			text = isEquipmentAutoScrap switch
			{
				AutoScrapEquipmentType.None => availableautoScrapS[0],
				AutoScrapEquipmentType.ThreeStar => availableautoScrapS[1],
				AutoScrapEquipmentType.FourStar => availableautoScrapS[2],
				AutoScrapEquipmentType.FiveStar => availableautoScrapS[3],
				_ => availableautoScrapS[0],
			};
			currentAutoScrap.text = LocalizationManager.GetText(text);
		}
		if (displayModeScrollView != null)
		{
			for (int m = 0; m < displayModes.Count; m++)
			{
				UnityEngine.Object.Destroy(displayModes[m]);
			}
			displayModes.Clear();
			displayModeScrollView.ResetPosition();
			for (int n = 0; n < GameManager.Instance.DisplayModeKeyArray.Length; n++)
			{
				GameObject gameObject3 = Helpers.InstantiateToParent(displayModePrefab, displayModeScrollView.gameObject);
				gameObject3.GetComponent<DisplayModeItem>().SetKey(this, n);
				displayModes.Add(gameObject3);
			}
			displayModeScrollView.ResetPosition();
			displayModeTable.repositionNow = true;
		}
		currentDisplayMode.text = LocalizationManager.GetText(GameManager.Instance.DisplayModeKeyArray[TWDPlayerPrefs.GetInt("PlayerSelectedDisplayMode")]);
		if (screenResolutionScrollView != null)
		{
			for (int num = 0; num < screenResolutions.Count; num++)
			{
				UnityEngine.Object.Destroy(screenResolutions[num]);
			}
			screenResolutions.Clear();
			screenResolutionScrollView.ResetPosition();
			for (int num2 = 0; num2 < GameManager.ScreenResolutionWidthArray.Length; num2++)
			{
				GameObject gameObject4 = Helpers.InstantiateToParent(screenResolutionPrefab, screenResolutionScrollView.gameObject);
				gameObject4.GetComponent<ScreenResolutionItem>().SetKey(this, num2);
				screenResolutions.Add(gameObject4);
			}
			screenResolutionScrollView.ResetPosition();
			screenResolutionTable.repositionNow = true;
		}
		int num3 = TWDPlayerPrefs.GetInt("PlayerSelectedScreenResolution");
		if (num3 == 0)
		{
			currentScreenResolution.text = LocalizationManager.GetText("Popup.Settings.ScreenResolution.Default.EPIC");
		}
		else
		{
			currentScreenResolution.text = GameManager.ScreenResolutionWidthArray[num3] + "X" + GameManager.ScreenResolutionHeightArray[num3];
		}
		if (framteRateScrollView != null)
		{
			for (int num4 = 0; num4 < framteRates.Count; num4++)
			{
				UnityEngine.Object.Destroy(framteRates[num4]);
			}
			framteRates.Clear();
			framteRateScrollView.ResetPosition();
			for (int num5 = 0; num5 < GameManager.Instance.FrameRateArray.Length; num5++)
			{
				GameObject gameObject5 = Helpers.InstantiateToParent(framteRatePrefab, framteRateScrollView.gameObject);
				gameObject5.GetComponent<FrameRateItem>().SetKey(this, num5);
				framteRates.Add(gameObject5);
			}
			framteRateScrollView.ResetPosition();
			framteRateTable.repositionNow = true;
		}
		currentFramteRate.text = GameManager.Instance.FrameRateArray[TWDPlayerPrefs.GetInt("PlayerSelectedFrameRate", 1)];
		UpdateAccountState();
		giftCodeRedeemButton.SetActive(GameManager.Instance.gameEconomyData.GetFeature("GiftCodes").IsEnabledForThisClient());
	}

	private void OnLanguageChanged()
	{
		UpdateUI();
	}

	private void UpdateAccountState()
	{
		if (gameCenterContainer != null)
		{
			gameCenterContainer.gameObject.SetActive(UsingGameCenter);
			gameCenterButton.isEnabled = !gameSaved || !connected || hasOtherGame;
			if (gameCenterButtonBackground != null)
			{
				gameCenterButtonBackground.SetActive(gameCenterButton.isEnabled);
			}
		}
		if (gameCenterButtonLabel != null)
		{
			if (!connected)
			{
				gameCenterButtonLabel.text = LocalizationManager.GetText("Text.General.Connect");
			}
			else if (gameSaved && !hasOtherGame)
			{
				gameCenterButtonLabel.text = LocalizationManager.GetText("Text.General.Connected");
			}
			else
			{
				gameCenterButtonLabel.text = LocalizationManager.GetText("Popup.GameSaveInfo.RestoreGame");
			}
		}
		if (googlePlayContainer != null)
		{
			googlePlayContainer.gameObject.SetActive(!UsingGameCenter);
		}
		if (googlePlayButtonLabel != null)
		{
			if (!connected)
			{
				googlePlayButtonLabel.text = LocalizationManager.GetText("Text.General.Connect");
			}
			else if (gameSaved && !hasOtherGame)
			{
				googlePlayButtonLabel.text = LocalizationManager.GetText("Text.General.Disconnect");
			}
			else if (!gameSaved || hasOtherGame)
			{
				googlePlayButtonLabel.text = LocalizationManager.GetText("Popup.GameSaveInfo.RestoreGame");
			}
		}
		if (googlePlayButtonBackgroundGrey != null)
		{
			googlePlayButtonBackgroundGrey.gameObject.SetActive(connected && gameSaved && !hasOtherGame);
		}
		if (googlePlayButtonBackgroundGreen != null)
		{
			googlePlayButtonBackgroundGreen.SetActive(!connected || !gameSaved || hasOtherGame);
		}
		if (accountLinkingInfo != null)
		{
			accountLinkingInfo.gameObject.SetActive(value: true);
		}
		if (accountLinkingTitleLabel != null)
		{
			accountLinkingTitleLabel.text = LocalizationManager.GetText(UsingGameCenter ? "Popup.Settings.AccountInfo.UseGameCenterInfo" : "Popup.Settings.AccountInfo.UseGooglePlayInfo");
		}
		bool flag = gameSaved && !hasOtherGame;
		if (accountLinkingStatusLabel != null)
		{
			accountLinkingStatusLabel.text = LocalizationManager.GetText(flag ? "Popup.Settings.AccountInfo.GameSaved" : "Popup.Settings.AccountInfo.GameNotSaved");
		}
		if (accountLinkingInfoIconSaved != null)
		{
			accountLinkingInfoIconSaved.gameObject.SetActive(flag);
		}
		if (accountLinkingInfoIconNotSaved != null)
		{
			accountLinkingInfoIconNotSaved.gameObject.SetActive(!flag);
		}
	}

	public void ToggleLanguageScroll()
	{
		languageScrollContainer.SetActive(!languageScrollContainer.activeSelf);
		languageScrollView.ResetPosition();
		if (languageScrollContainer.activeSelf)
		{
			displayModeScrollContainer.SetActive(value: false);
			screenResolutionScrollContainer.SetActive(value: false);
			framteRateScrollContainer.SetActive(value: false);
		}
	}

	public void ToggleAutoScrapScroll()
	{
		autoScrapScrollContainer.SetActive(!autoScrapScrollContainer.activeSelf);
		autoScrapScrollView.ResetPosition();
	}

	public void ToggleDisplayMode()
	{
		displayModeScrollContainer.SetActive(!displayModeScrollContainer.activeSelf);
		displayModeScrollView.ResetPosition();
		if (displayModeScrollContainer.activeSelf)
		{
			languageScrollContainer.SetActive(value: false);
			screenResolutionScrollContainer.SetActive(value: false);
			framteRateScrollContainer.SetActive(value: false);
		}
	}

	public void ToggleScreenResolution()
	{
		screenResolutionScrollContainer.SetActive(!screenResolutionScrollContainer.activeSelf);
		screenResolutionScrollView.ResetPosition();
		if (screenResolutionScrollContainer.activeSelf)
		{
			languageScrollContainer.SetActive(value: false);
			displayModeScrollContainer.SetActive(value: false);
			framteRateScrollContainer.SetActive(value: false);
		}
	}

	public void ToggleFrameRate()
	{
		framteRateScrollContainer.SetActive(!framteRateScrollContainer.activeSelf);
		framteRateScrollView.ResetPosition();
		if (framteRateScrollContainer.activeSelf)
		{
			languageScrollContainer.SetActive(value: false);
			displayModeScrollContainer.SetActive(value: false);
			screenResolutionScrollContainer.SetActive(value: false);
		}
	}

	public void OnSoundFxSlider()
	{
		GameManager.Instance.Settings.SoundFxVolume = soundFxSlider.value;
		Fabric.EventManager.Instance.PostEvent("volume/sound_effects", EventAction.SetVolume, GameManager.Instance.Settings.SoundFxVolume);
		if (GameManager.Instance.Settings.SoundFxVolume > 0f)
		{
			soundFxOnGo.SetActive(value: true);
			soundFxOffGo.SetActive(value: false);
		}
		else
		{
			soundFxOnGo.SetActive(value: false);
			soundFxOffGo.SetActive(value: true);
		}
	}

	private void OnSoundFxSliderDragFinished()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/sound_effect_try");
	}

	public void OnMusicSlider()
	{
		GameManager.Instance.Settings.MusicVolume = MusicSlider.value;
		Fabric.EventManager.Instance.PostEvent("volume/music", EventAction.SetVolume, GameManager.Instance.Settings.MusicVolume);
		if (GameManager.Instance.Settings.MusicVolume > 0f)
		{
			MusicOnGo.SetActive(value: true);
			MusicOffGo.SetActive(value: false);
		}
		else
		{
			MusicOnGo.SetActive(value: false);
			MusicOffGo.SetActive(value: true);
		}
	}

	public void OnGoreToggle()
	{
		bool flag = !GameManager.Instance.IsGoreDisabled;
		if (goreToggle.value != flag)
		{
			Helpers.ExecuteCommand(new ChangeGoreSettingCommand(goreToggle.value));
		}
	}

	public void OnVsyncToggle()
	{
		GameManager.Instance.Settings.VSync = vsyncToggle.value;
		if (GameManager.Instance.Settings.VSync)
		{
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
	}

	public void SetLanguage(string key)
	{
		GameManager.Instance.SetPlayerPickedLanguage(key);
		languageScrollContainer.SetActive(value: false);
		UpdateUI();
	}

	public void SetAutoScrap(string key)
	{
		AutoScrapEquipmentType isEquipmentAutoScrap = ((!(key == availableautoScrapS[0])) ? ((key == availableautoScrapS[1]) ? AutoScrapEquipmentType.ThreeStar : ((!(key == availableautoScrapS[2])) ? AutoScrapEquipmentType.FiveStar : AutoScrapEquipmentType.FourStar)) : AutoScrapEquipmentType.None);
		if (Helpers.ExecuteCommand(new SettingAutoScrapEquipmentCommand(isEquipmentAutoScrap)) == TWDModelResult.OK)
		{
			autoScrapScrollContainer.SetActive(value: false);
			UpdateUI();
		}
	}

	public void SetDisplayMode(int keyIndex)
	{
		GameManager.Instance.SetPlayerPickedDisplayMode(keyIndex);
		displayModeScrollContainer.SetActive(value: false);
		UpdateUI();
	}

	public void SetScreenResolution(int keyIndex)
	{
		GameManager.Instance.SetPlayerPickedScreenResolution(keyIndex);
		screenResolutionScrollContainer.SetActive(value: false);
		UpdateUI();
	}

	public void SetFrameRate(int keyIndex)
	{
		GameManager.Instance.SetPlayerPickedFrameRate(keyIndex);
		framteRateScrollContainer.SetActive(value: false);
		UpdateUI();
	}

	public void OnLinkDevice()
	{
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LinkDevicePopup).Open();
	}

	public void OnBananaCode()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode, waitForResponse: true);
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
			LinkBananaPopup linkBananaPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LinkBananaPopup) as LinkBananaPopup;
			if (linkBananaPopup != null)
			{
				Close();
				linkBananaPopup.Code = transferCode.Code;
				linkBananaPopup.CodeTimer1970 = new DateTime(1970, 1, 1);
				linkBananaPopup.CodeTimerExpiration = transferCode.Expiration;
				linkBananaPopup.Open();
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
			Close();
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}

	public void OnHeplShift()
	{
		SingularityMonoBehaviour<SDKManager>.Instance.ShowFAQs();
	}

	public void SetHelpNotification(int amount)
	{
		helpNotification.SetNumber(Mathf.Max(0, amount));
	}

	private new void Update()
	{
		_timeUntilNextCheckUnreadMessageCount -= Time.deltaTime;
		if (_timeUntilNextCheckUnreadMessageCount <= 0f)
		{
			_timeUntilNextCheckUnreadMessageCount = 1f;
			SetHelpNotification(SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount);
		}
	}

	public void OnCredits()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CreditsPopup).Open();
	}

	public void OnQuit()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.QuitPopup).Open();
	}

	public void OnSocialPageTwitter()
	{
		if (GameManager.CanOpenURLScheme("twitter://"))
		{
			Application.OpenURL("twitter:///user?screen_name=twdnomansland");
		}
		else
		{
			Application.OpenURL("https://twitter.com/twdnomansland");
		}
	}

	public void OnSocialPageInstagram()
	{
		if (GameManager.CanOpenURLScheme("instagram://"))
		{
			Application.OpenURL("instagram://user?username=twdnomansland");
		}
		else
		{
			Application.OpenURL("https://www.instagram.com/twdnomansland");
		}
	}

	public void OnGooglePlayAchievements()
	{
		GameManager.Instance.GameCenterManager.OpenSystemDefaultAchievementsUI();
	}

	private IEnumerator UpdateUIDelayed_Coroutine()
	{
		yield return null;
		UpdateUI();
	}

	public void OnAccountButtonPressed()
	{
		if (!connected)
		{
			StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(connect: true, delegate
			{
				UpdateAccountState();
			}));
			return;
		}
		if ((!gameSaved || hasOtherGame) && GameManager.Instance != null && GameManager.Instance.GameCenterManager != null)
		{
			GameManager.Instance.GameCenterManager.PromptGameCenterRestore(comingFromSettings: true);
		}
		if (!UsingGameCenter && gameSaved && !hasOtherGame && GameManager.Instance != null && GameManager.Instance.GameCenterManager != null)
		{
			StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(connect: false, delegate
			{
				UpdateAccountState();
			}));
		}
	}

	public void OnAccountInfo()
	{
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GameSaveInfoPopup) as GameSaveInfoPopup).Open();
		Close();
	}

	public void OnTargetedAdsToggle()
	{
		bool flag = GameManager.Instance.playerModel.HasAcceptedGdprAction("TargetedAdsConsent");
		if (targetedAdsToggle.value != flag)
		{
			GameManager.Instance.UpdateTargeteAdsConsent(targetedAdsToggle.value, "Settings_Ads_Enabled");
		}
	}

	public void OnAutoCoverToggle()
	{
		if (!autoCoverToggle.value)
		{
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.AutoCoverDisabled"));
		}
		else if (GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.AutoCoverDisabled"))
		{
			Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("Toggle.AutoCoverDisabled"));
		}
	}

	public void OnBatterySaverToggle()
	{
		bool flag = true;
		if (flag != GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.Toggle60FPSModeEnabled"))
		{
			if (flag)
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.Toggle60FPSModeEnabled"));
				Application.targetFrameRate = 60;
			}
			else
			{
				Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("Toggle.Toggle60FPSModeEnabled"));
				Application.targetFrameRate = 30;
			}
		}
	}

	public void OnCombatGridToggle()
	{
		bool value = combatGridToggle.value;
		if (value != GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleCombatGridEnabled"))
		{
			if (value)
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleCombatGridEnabled"));
			}
			else
			{
				Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("Toggle.ToggleCombatGridEnabled"));
			}
		}
	}

	public void OnCombatCameraToggle()
	{
		bool value = combatCameraToggle.value;
		if (value != GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleCombatCameraEnabled"))
		{
			if (value)
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleCombatCameraEnabled"));
			}
			else
			{
				Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("Toggle.ToggleCombatCameraEnabled"));
			}
		}
	}

	public void OnBlackMarketNotificationsToggle()
	{
		bool value = blackMarketNotificationsToggle.value;
		if (value != GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.BlackMarketNotifications"))
		{
			if (value)
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.BlackMarketNotifications"));
			}
			else
			{
				Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("Toggle.BlackMarketNotifications"));
			}
		}
	}

	public void OnTermsOfServiceClick()
	{
		SendGdprMetricLinkCommand("Settings_Open_Link", "Terms_of_Service");
		Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.TermsOfServiceURL);
	}

	public void OnPrivacyPolicyClick()
	{
		SendGdprMetricLinkCommand("Settings_Open_Link", "Privacy_Policy");
		Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.PrivacyPolicyURL);
	}

	public void OnFairPlayPolicyClick()
	{
		SendGdprMetricLinkCommand("Settings_Open_Link", "Fair_Play_Policy");
		Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.FairPlayPolicyURL);
	}

	public void OnMyDataClick()
	{
	}

	public void OnDeleteMyAccountClick()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AccountDeletionPopup);
		if (hUDElement != null)
		{
			hUDElement.Open();
			Close();
		}
	}

	private void SendGdprMetricLinkCommand(string dialogueName, string linkName)
	{
		Helpers.ExecuteCommand(new SendGdprMetricCommand(SendGdprMetricCommand.MetricType.Open_GDPR_Link)
		{
			DialogueName = dialogueName,
			LinkName = linkName
		});
	}

	public void OnRedeemGiftCode()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RedeemCodePopup).Open();
	}
}
