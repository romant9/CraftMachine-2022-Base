using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BaseModel;
using Client.Connectivity;
using Fabric;
using TWDModel;
using UnityEngine;

public class LoadingScreenHUD : SingularityMonoBehaviour<LoadingScreenHUD>
{
	private enum ErrorState
	{
		Connectivity = 0,
		Maintenance = 1,
		NewVersion = 2,
		UnderAttack = 3,
		PlayerLoadLock = 4,
		Banned = 5,
		OtherLock = 6
	}

	[SerializeField]
	private UILabel tipLabel;

	[SerializeField]
	private UILabel loadingLabel;

	[SerializeField]
	private UITexture loadingTexture;

	[SerializeField]
	private UIProgressBar progressBar;

	[SerializeField]
	private UILabel progressPercentLabel;

	[SerializeField]
	private UIButton retryConnectButton;

	[SerializeField]
	private UIButton helpButton;

	[SerializeField]
	private GameObject epicLoginContainer;

	[Header("Version Mismatch popup")]
	[SerializeField]
	private GameObject versionMismatchContainer;

	[SerializeField]
	private UILabel versionMismatchLabel;

	[SerializeField]
	[Header("Error popup")]
	private GameObject errorPopupContainer;

	[SerializeField]
	private UILabel errorPopupLabel;

	[SerializeField]
	private UIButton errorPopupReloadButton;

	[SerializeField]
	[Header("Banned popup")]
	private GameObject bannedPopupContainer;

	[SerializeField]
	private UILabel bannedPopupTitleLabel;

	[SerializeField]
	private UILabel bannedPopupReasonLabel;

	[SerializeField]
	private UILabel bannedPopupTimeLabel;

	[SerializeField]
	private GdprFlowHandler gdprFlowHandler;

	[SerializeField]
	private float progressSmoothTime = 0.65f;

	[SerializeField]
	private float progressMaxChangePerSecond = 0.5f;

	[SerializeField]
	private float stallLabelCreepPercentPerSecond = 0.12f;

	[SerializeField]
	private float stallLabelCreepStartSeconds = 0.35f;

	[Tooltip("Clamp delta time used only for progress smoothing. After a hitch, unscaledDeltaTime can be huge and would teleport the bar in one frame.")]
	[SerializeField]
	private float progressSmoothDeltaTimeCap = 0.04f;

	[Tooltip("If a frame's unscaledDeltaTime is this large or more, skip advancing the smoothed bar this frame (main-thread stall just ended). 0 = never skip.")]
	[SerializeField]
	private float progressHitchHoldSkipSeconds = 0.22f;

	private int currentLoadingStep;

	private const int totalLoadingSteps = 14;

	private bool playerLocked;

	private bool themeStarted;

	private ErrorState errorState;

	private string bannedMessage;

	public const int defaultSleepTimeOut = -2;

	public const int HotUpdateKeepAwakeSleepTimeout = -1;

	private float peakUnifiedProgress;

	private float smoothDisplayValue;

	private float smoothVelocity;

	private float lastPeakChangeRealtime;

	private float labelStallCreep;

	private void OnEnable()
	{
		EventManager.OnEvent += OnEvent;
		currentLoadingStep = 0;
		themeStarted = false;
		errorState = ErrorState.Connectivity;
		ResetProgressVisualState();
		HideNetworkError();
	}

	private void ResetProgressVisualState()
	{
		peakUnifiedProgress = 0f;
		smoothDisplayValue = 0f;
		smoothVelocity = 0f;
		lastPeakChangeRealtime = Time.realtimeSinceStartup;
		labelStallCreep = 0f;
		if (progressBar != null)
		{
			progressBar.value = 0f;
		}
		UpdatePercentLabelImmediate(0f);
	}

	private void Start()
	{
		InitializeTips();
		HideHelpButton();
		string path = Application.persistentDataPath + "/GameAssets/loadingscreen";
		bool flag = false;
		if (File.Exists(path))
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
			if (assetBundle != null)
			{
				loadingTexture.mainTexture = assetBundle.LoadAsset<Texture>("Loading_Screen");
				assetBundle.Unload(unloadAllLoadedObjects: false);
				flag = true;
			}
		}
		if (!flag)
		{
			loadingTexture.mainTexture = Resources.Load<Texture>("UI/Textures/Loading_Screen");
		}
	}

	private void Update()
	{
		TickSmoothedProgress();
	}

	public static void SetLoadingMessage(LoadingMessageType message)
	{
		if (SingularityMonoBehaviour<LoadingScreenHUD>.Instance != null)
		{
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.SetLoadingMessageInternal(message);
		}
	}

	private void SetLoadingMessageInternal(LoadingMessageType message)
	{
		string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LOADING");
		switch (message)
		{
		case LoadingMessageType.InitRequest:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.Step1");
			break;
		case LoadingMessageType.SignalRConnect:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.Step2");
			break;
		case LoadingMessageType.Login:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.Step3");
			break;
		case LoadingMessageType.LoadGameEconomyData:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.Step3");
			break;
		case LoadingMessageType.LoadPlayer:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.Step4");
			break;
		case LoadingMessageType.LoadScene:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.Step5");
			break;
		case LoadingMessageType.AssetLoading:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.AssetLoading");
			break;
		case LoadingMessageType.DownloadAssets:
			localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("LoadingScreen.LoadingProgressBar.AssetDownloading");
			break;
		}
		loadingLabel.text = localizedText;
	}

	private void OnDisable()
	{
		EventManager.OnEvent -= OnEvent;
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.LoadingStepComplete)
		{
			IncrementStep();
		}
	}

	private void InitializeTips()
	{
		List<string> list = new List<string>();
		string text = "LOADING_TIPS";
		string text2 = text + "0";
		string text3 = "";
		int num = 0;
		while (true)
		{
			text2 = text + num;
			if (!SingularityMonoBehaviour<LocalizationManager>.Instance.LocalizationExists(text2))
			{
				break;
			}
			text3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(text2);
			list.Add(text3);
			num++;
		}
		if (list.Count > 0)
		{
			string text4 = list[UnityEngine.Random.Range(0, list.Count)];
			tipLabel.text = text4;
		}
	}

	private void IncrementStep()
	{
		currentLoadingStep++;
		ApplyStepToPeakProgress();
		if (!themeStarted && SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StopAllSounds(1f);
			themeStarted = true;
			if (TWDPlayerPrefs.GetInt("LoadCount") > 0)
			{
				Fabric.EventManager.Instance.PostEvent("volume/sound_effects", EventAction.SetVolume, GameManager.Instance.Settings.SoundFxVolume);
				Fabric.EventManager.Instance.PostEvent("volume/music", EventAction.SetVolume, GameManager.Instance.Settings.MusicVolume);
				SingularityMonoBehaviour<AudioManager>.Instance.RequestMusicStateChange(MusicState.Theme);
			}
		}
	}

	public void Completed()
	{
		peakUnifiedProgress = 1f;
		smoothDisplayValue = 1f;
		smoothVelocity = 0f;
		lastPeakChangeRealtime = Time.realtimeSinceStartup;
		labelStallCreep = 0f;
		if (progressBar != null)
		{
			progressBar.value = 1f;
		}
		UpdatePercentLabelImmediate(100f);
	}

	private void ApplyStepToPeakProgress()
	{
		float b = Mathf.Min(13f / 14f, (float)currentLoadingStep / 14f);
		float num = Mathf.Max(peakUnifiedProgress, b);
		if (num > peakUnifiedProgress + 1E-05f)
		{
			lastPeakChangeRealtime = Time.realtimeSinceStartup;
			labelStallCreep = 0f;
		}
		peakUnifiedProgress = num;
		float num2 = peakUnifiedProgress - 1f / 7f;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		if (smoothDisplayValue < num2)
		{
			smoothDisplayValue = Mathf.Lerp(smoothDisplayValue, num2, 0.5f);
			smoothVelocity = 0f;
			if (progressBar != null)
			{
				progressBar.value = smoothDisplayValue;
				UpdatePercentLabelImmediate(smoothDisplayValue * 100f);
			}
		}
	}

	private void TickSmoothedProgress()
	{
		if (progressBar == null)
		{
			return;
		}
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (!(progressHitchHoldSkipSeconds > 0f) || !(unscaledDeltaTime >= progressHitchHoldSkipSeconds))
		{
			float num = ((progressSmoothDeltaTimeCap > 0f) ? Mathf.Min(unscaledDeltaTime, progressSmoothDeltaTimeCap) : unscaledDeltaTime);
			float target = Mathf.Clamp01(peakUnifiedProgress);
			float maxSpeed = ((progressMaxChangePerSecond > 0.0001f) ? progressMaxChangePerSecond : float.PositiveInfinity);
			smoothDisplayValue = Mathf.SmoothDamp(smoothDisplayValue, target, ref smoothVelocity, progressSmoothTime, maxSpeed, num);
			progressBar.value = smoothDisplayValue;
			float num2 = smoothDisplayValue * 100f;
			float num3 = peakUnifiedProgress * 100f;
			float num4 = Time.realtimeSinceStartup - lastPeakChangeRealtime;
			float num5 = num2;
			if (num4 > stallLabelCreepStartSeconds && num3 - num5 > 0.12f)
			{
				labelStallCreep += num * stallLabelCreepPercentPerSecond;
				num5 = Mathf.Min(num3 - 0.06f, num2 + labelStallCreep);
			}
			else
			{
				labelStallCreep = 0f;
			}
			UpdatePercentLabelImmediate(num5);
		}
	}

	private void UpdatePercentLabelImmediate(float percentForLabel)
	{
		if (!(progressPercentLabel == null))
		{
			percentForLabel = Mathf.Clamp(percentForLabel, 0f, 100f);
			progressPercentLabel.text = percentForLabel.ToString("F1", CultureInfo.InvariantCulture) + "%";
		}
	}

	public static void BeginHotUpdateKeepAwake()
	{
		Screen.sleepTimeout = -1;
	}

	private void ShowNetworkError(string text)
	{
		if (HelpersModel.IsOfflineMode) return;

		string text2 = "";
		text2 = ((UnityUtils.InternetReachability != NetworkReachability.NotReachable) ? LocalizationManager.GetText("Error.CannotConnect") : LocalizationManager.GetText("Error.NoInternet"));
		if (text != null && GameConfiguration.Instance.Config.ShowDebugMenu)
		{
			text2 = text;
		}
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		}
		ShowErrorPopup(text2);
	}

	public void HandleNetworkError(string text = null)
	{
		if (OfflineManager.IsIgnoreReconnect || HelpersModel.IsOfflineMode)
		{
			MyTools.OpenAlert(LocalizationManager.GetText("Error.NoConnectivity"));
			return;
		}
		if (UnityUtils.InternetReachability == NetworkReachability.NotReachable)
		{
			AnalyticsManager.instance.CreateEvent("Connectivity_noDataConnection").Send();
			ShowNetworkError(LocalizationManager.GetText("Error.NoConnectivity"));
		}
		else if (GameManager.Instance != null && GameManager.Instance.ShouldAutoReloadOnError())
		{
			GameManager.LogReloadEvent("Reason: GameManager.HandleNetworkError, error " + text);
			GameManager.Instance.GoToLoaderScene();
		}
		else
		{
			ShowNetworkError(text);
		}
	}

	public void ShowPlayerLocked(LockRespond lockRespond)
	{
		if (lockRespond.Status == LockRespond.LockStatus.LoadLocked)
		{
			errorState = ErrorState.PlayerLoadLock;
		}
		else if (lockRespond.Status == LockRespond.LockStatus.Locked)
		{
			errorState = ErrorState.UnderAttack;
		}
		else if (lockRespond.Status == LockRespond.LockStatus.Banned)
		{
			errorState = ErrorState.Banned;
		}
		else
		{
			if (lockRespond.Status == LockRespond.LockStatus.PlayerDisabled)
			{
				ShowPlayerErasedPopup();
				return;
			}
			errorState = ErrorState.OtherLock;
		}
		if (lockRespond.Status == LockRespond.LockStatus.LoadLocked)
		{
			ShowErrorPopup(LocalizationManager.GetText("Error.PlayerLoadLocked"));
			return;
		}
		if (lockRespond.Status == LockRespond.LockStatus.Banned)
		{
			bannedMessage = lockRespond.Reason;
			ShowBannedPopup(lockRespond.LockedUntil);
			return;
		}
		ShowErrorPopup("", showReloadButton: false);
		int num = 60;
		float num2 = (float)(lockRespond.LockedUntil - DateTime.UtcNow).TotalSeconds + 1f;
		if (num2 < 0f || num2 > (float)num)
		{
			Debug.LogWarning("Capping locked time, locked until " + lockRespond.LockedUntil.ToString() + ", device now " + DateTime.UtcNow);
			num2 = num;
		}
		StopAllCoroutines();
		StartCoroutine(WaitPlayerLocked((int)num2));
	}

	private IEnumerator WaitPlayerLocked(int waitSeconds)
	{
		float endTime = Time.realtimeSinceStartup + (float)waitSeconds;
		float retryTime = Time.realtimeSinceStartup + 60f;
		int lastSeconds = 0;
		playerLocked = true;
		while (waitSeconds > 0)
		{
			waitSeconds = (int)(endTime - Time.realtimeSinceStartup);
			if (waitSeconds != lastSeconds)
			{
				lastSeconds = waitSeconds;
				if (errorState == ErrorState.UnderAttack)
				{
					UpdateErrorPopupText(LocalizationManager.GetText("Error.PlayerLockedUnderAttack", waitSeconds));
				}
				else
				{
					UpdateErrorPopupText(LocalizationManager.GetText("Error.PlayerLocked", waitSeconds));
				}
			}
			if (Time.realtimeSinceStartup > retryTime)
			{
				break;
			}
			yield return null;
		}
		playerLocked = false;
		OnRetryConnect();
	}

	public void ShowMaintenanceBreak(long endTime)
	{
		errorState = ErrorState.Maintenance;
		if (endTime < 0)
		{
			ShowErrorPopup(LocalizationManager.GetText("Popup.Loading.MaintenanceStartedEndsSoon"));
			return;
		}
		ShowErrorPopup(LocalizationManager.GetText("Popup.Loading.MaintenanceStarted{Time}", Helpers.FormatTime(endTime)));
	}

	public void AssetsDownloading(float value)
	{
		float t = Mathf.Clamp01(value);
		float b = (float)currentLoadingStep / 14f;
		float a = Mathf.Max(peakUnifiedProgress, b);
		float b2 = 13f / 14f;
		float b3 = Mathf.Lerp(a, Mathf.Max(a, b2), t);
		float num = Mathf.Max(peakUnifiedProgress, b3);
		if (num > peakUnifiedProgress + 1E-05f)
		{
			lastPeakChangeRealtime = Time.realtimeSinceStartup;
			labelStallCreep = 0f;
		}
		peakUnifiedProgress = num;
	}

	private void ShowHelpButton()
	{
		if (helpButton != null)
		{
			helpButton.gameObject.SetActive(value: true);
		}
	}

	private void HideHelpButton()
	{
		if (helpButton != null)
		{
			helpButton.gameObject.SetActive(value: false);
		}
	}

	private void NetworkError()
	{
		currentLoadingStep = 0;
		ResetProgressVisualState();
		Screen.sleepTimeout = -2;
		retryConnectButton.gameObject.SetActive(value: true);
		SignalRClient signalRClient = SignalRClient.Instance;
		signalRClient.Disconnect();
		signalRClient.ClearError();
		ShowHelpButton();
	}

	public void HideNetworkError()
	{
		Screen.sleepTimeout = -2;
		retryConnectButton.gameObject.SetActive(value: false);
		loadingLabel.gameObject.SetActive(value: true);
		List<string> list = new List<string>();
		string text = "LOADING_TIPS";
		string text2 = text + "0";
		string text3 = "";
		int num = 0;
		while (true)
		{
			text2 = text + num;
			if (SingularityMonoBehaviour<LocalizationManager>.Instance == null || !SingularityMonoBehaviour<LocalizationManager>.Instance.LocalizationExists(text2))
			{
				break;
			}
			text3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(text2);
			list.Add(text3);
			num++;
		}
		if (list.Count > 0)
		{
			string text4 = list[UnityEngine.Random.Range(0, list.Count)];
			tipLabel.text = text4;
		}
		HideHelpButton();
		errorPopupContainer.SetActive(value: false);
		errorState = ErrorState.Connectivity;
	}

	public void OnRetryConnect()
	{
		if (!playerLocked)
		{
			HideNetworkError();
			if (GameManager.Instance != null)
			{
				GameManager.LogReloadEvent("Reason: LoadingScreenHUD.OnRetryConnect");
				GameManager.Instance.GoToLoaderScene();
			}
		}
	}

	public void ShowEpicLoginFailed()
	{
		epicLoginContainer.SetActive(value: true);
	}

	public void ShowVersionMismatch()
	{
		errorState = ErrorState.NewVersion;
		versionMismatchContainer.SetActive(value: true);
	}

	public void ShowWarningPopup(string text)
	{
		errorPopupContainer.SetActive(value: true);
		errorPopupReloadButton.gameObject.SetActive(value: false);
		errorPopupLabel.text = text;
	}

	public void ShowBannedPopup(DateTime dateTime)
	{
		bannedPopupContainer.SetActive(value: true);
		tipLabel.gameObject.SetActive(value: false);
		loadingLabel.gameObject.SetActive(value: false);
		bannedPopupTitleLabel.text = LocalizationManager.GetText("Popup.Loading.Banned.Title");
		bannedPopupReasonLabel.text = bannedMessage;
		bannedPopupTimeLabel.text = LocalizationManager.GetText("Popup.Loading.Banned.Time{Time}", dateTime.ToString("MMMM dd, yyyy"));
		NetworkError();
		HideHelpButton();
	}

	public void ShowPlayerErasedPopup()
	{
		tipLabel.gameObject.SetActive(value: false);
		loadingLabel.gameObject.SetActive(value: false);
		gdprFlowHandler.ShowPlayerDeleted();
		NetworkError();
		HideHelpButton();
		StopAllCoroutines();
	}

	public void OnBannedOk()
	{
		bannedPopupContainer.SetActive(value: false);
	}

	public void ErasedOk()
	{
		HideNetworkError();
		GameManager.Instance.ResetGame();
	}

	public void ShowErrorPopup(string text, bool showReloadButton = true)
	{
		errorPopupContainer.SetActive(value: true);
		errorPopupReloadButton.gameObject.SetActive(showReloadButton);
		errorPopupLabel.text = text;
		NetworkError();
	}

	private void UpdateErrorPopupText(string text)
	{
		errorPopupLabel.text = text;
	}

	public void OnRetryEpicLogin()
	{
		epicLoginContainer.SetActive(value: false);
		Startup.LoginEpic();
	}

	public void OnGetNewVersion()
	{
		Application.Quit();
	}

	public void OnHelpShift()
	{
		if (errorState == ErrorState.Maintenance || errorState == ErrorState.NewVersion)
		{
			_ = 1;
		}
		else
			_ = errorState == ErrorState.UnderAttack;
		if (errorState != ErrorState.Maintenance && errorState != ErrorState.NewVersion)
		{
			_ = errorState;
			_ = 3;
		}
		SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.ShowFAQs();
	}
}
