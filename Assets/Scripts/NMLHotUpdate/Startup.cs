using System;
using System.Collections;
using System.Net;
using System.Text;
using BestHTTP;
using Client.Connectivity;
using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;

public class Startup : MonoBehaviour
{
	public GUISkin guiSkin;

	[Tooltip("Texture limit in editor - set to 0 for full-res and 1 for half-res textures")]
	public int EditorMasterTextureLimit;

	private string statusMessage;

	private LoadingScreenHUD loadingScreen;

	private bool pendingForStartupChoice;

	private bool startupModeOffline;

	private BuildGameConfiguration.OnlineLevelType originalOnlineLevel;

	private bool connectToBackupServer;

	private bool connectedOnAwake;

	[SerializeField]
	private bool showStartupMenuInEditor;

	[SerializeField]
	private CustomLogger additionalCustomLogger;

	private static StringBuilder startupEventLog;

	private static float previousStartupEventTime;

	public static string ConnectionUrl;

	private const string CDNBaseUrlKey = "CDNBaseURL";

	private void Awake()
	{
		LogStartupEvent("Awake");
		additionalCustomLogger.enabled = GameConfiguration.Instance.Config.AdditionalCustomLogging;
		Screen.sleepTimeout = -2;
		Shader.EnableKeyword("SEPARATE_ALPHA_OFF");
		Shader.DisableKeyword("SEPARATE_ALPHA_ON");
		InternalProfiler.initProfiler();
		Application.runInBackground = true;
		Application.targetFrameRate = 30;
		int num = TWDPlayerPrefs.GetInt("PlayerSelectedDisplayMode");
		int num2 = TWDPlayerPrefs.GetInt("PlayerSelectedScreenResolution");
		Screen.SetResolution((num2 == 0) ? Display.main.systemWidth : GameManager.ScreenResolutionWidthArray[num2], (num2 == 0) ? Display.main.systemHeight : GameManager.ScreenResolutionHeightArray[num2], (num == 0) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
		Shader.globalMaximumLOD = 1000;
		if (Application.isEditor)
		{
			QualitySettings.globalTextureMipmapLimit = EditorMasterTextureLimit;
		}
		else if (PlatformInfo.HasFlag(PlatformFlag.SDResolution))
		{
			QualitySettings.globalTextureMipmapLimit = 1;
		}
		if (UnityUtils.InternetReachability != NetworkReachability.NotReachable && GameConfiguration.Instance.Config.OnlineLevel != BuildGameConfiguration.OnlineLevelType.Offline && !GameConfiguration.Instance.Config.ShowStartupMenu)
		{
			connectedOnAwake = true;
			StartCoroutine(DoConnect());
		}
	}

	private IEnumerator DoConnect()
	{
		if (SignalRClient.Instance == null)
		{
			if (GameManager.Instance.TryGetComponent<SignalRClient>(out var signalR))
			{
				signalR.Init();
				yield return new WaitUntil(() => SignalRClient.Instance != null);
			}
		}
		if (SignalRClient.Instance.IsConnected)
		{
			SignalRClient.Instance.Disconnect();
			yield return new WaitForSeconds(0.5f);
		}
		statusMessage = "Connecting to backend...";
		if (string.IsNullOrEmpty(ConnectionUrl))
		{
			connectToBackupServer = TWDPlayerPrefs.GetInt("ConnectToBackupServer") == 1;
			ConnectionUrl = (connectToBackupServer ? GameConfiguration.Instance.Config.SecondaryConnectionUrl : GameConfiguration.Instance.Config.ConnectionUrl);
		}
		try
		{
			LogStartupEvent("Connect");
			SignalRClient.Instance.Connect(ConnectionUrl, OnConnect);
		}
		catch (WebException ex)
		{
			Debug.LogException(ex);
			AnalyticsManager.instance.CreateEvent("Connectivity_InitFailed_SRConnectFailed").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError("Connect error: " + ex.Message);
		}
		LogStartupEvent("ConnectInvoked");
		ConnectionUrl = null;
	}

	public void Start()
	{
		StartCoroutine(loader());
	}

	private IEnumerator loader()
	{
		AnalyticsManager.instance.CreateEvent("Connectivity_GameLaunched").Send();
		Screen.sleepTimeout = -2;
		Shader.EnableKeyword("SEPARATE_ALPHA_OFF");
		Shader.DisableKeyword("SEPARATE_ALPHA_ON");
		ContentManager.Instance.Reset();
		if (PortraitManager.Instance != null)
		{
			PortraitManager.Instance.OnReload();
		}
		if (GameConfiguration.Instance.Config.ShowStartupMenu && (!Application.isEditor || showStartupMenuInEditor))
		{
			pendingForStartupChoice = true;
		}
		if (!GameConfiguration.Instance.Config.ShowStartupMenu && !Application.isEditor)
		{
			TutorialView.Instance.StartupSetting = TutorialView.StartupSettingType.Normal;
		}
		originalOnlineLevel = GameConfiguration.Instance.Config.OnlineLevel;
		if (guiSkin == null)
		{
			guiSkin = Resources.Load("DebugMenu") as GUISkin;
		}
		while (pendingForStartupChoice)
		{
			yield return new WaitForSeconds(1f);
		}
		LogStartupEvent("Start");
		Time.timeScale = 1f;
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		if (GameConfiguration.Instance.Config.OnlineLevel == BuildGameConfiguration.OnlineLevelType.Offline)
		{
			LoginEpic();
			yield break;
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		if (UnityUtils.InternetReachability == NetworkReachability.NotReachable)
		{
			AnalyticsManager.instance.CreateEvent("Connectivity_noDataConnectionStartup").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError();
			yield break;
		}
		LogStartupEvent("StartComplete");
		LoadingScreenHUD.SetLoadingMessage(LoadingMessageType.SignalRConnect);
		if (!connectedOnAwake)
		{
			StartCoroutine(DoConnect());
		}
		string url = TWDPlayerPrefs.GetString("ContentBaseUrl", CdnUrlHelper.GetDefaultCdnBaseUrl());
		url = CdnUrlHelper.RewriteCdnUrl(url);
		if (!string.IsNullOrEmpty(url))
		{
			new HTTPRequest(new Uri(url), isKeepAlive: true, disableCache: true, delegate
			{
			}).Send();
		}
	}

	private void ToggleBackupServer()
	{
		connectToBackupServer = !connectToBackupServer;
		Debug.LogWarning("Connecting to backup:" + connectToBackupServer);
		TWDPlayerPrefs.SetInt("ConnectToBackupServer", connectToBackupServer ? 1 : 0);
		TWDPlayerPrefs.Save();
	}

	private void OnConnect(string status)
	{
		if (status == "connected")
		{
			LogStartupEvent("Connected");
			if (OfflineManager.IsResetEpicToken) StartCoroutine(StartupLoginReset());
			else LoginEpic();
		}
		else
		{
			AnalyticsManager.instance.CreateEvent("Connectivity_InitFailed_SignalRConnectionFailed").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError("SignalR connect failed");
			ToggleBackupServer();
		}
	}

	public static void LoginEpic()
	{
		EOSLogin.Login(delegate(ProductUserId productUserId)
		{
			if (productUserId != null)
			{
				if (OfflineManager.IsIgnoreResponseNotOK) DebugTWD.Log("Epic productUserId : " + productUserId.ToString());
				else GameManager.Instance.LoadGame();
			}
			else
			{
				if (OfflineManager.IsIgnoreResponseNotOK) DebugTWD.LogWarning("Ignore! Epic productUserId is NULL", DebugType.Connection);
				else SingularityMonoBehaviour<LoadingScreenHUD>.Instance.ShowEpicLoginFailed();
			}
			if (OfflineManager.IsIgnoreResponseNotOK)
			{
				GameManager.Instance.MainLogin();
				GameManager.Instance.LoadGame();
			}
		});
	}

	private void LateUpdate()
	{
		if (SignalRClient.Instance.HasError)
		{
			statusMessage = SignalRClient.Instance.LastErrorMessage;
		}
	}

	public static void LogStartupEvent(string message)
	{
		if (startupEventLog == null)
		{
			startupEventLog = new StringBuilder();
			startupEventLog.AppendLine("Time, DeltaTime, Event");
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - previousStartupEventTime;
		previousStartupEventTime = realtimeSinceStartup;
		startupEventLog.AppendLine($"{realtimeSinceStartup * 1000f:0}, {num * 1000f:0}, {message}");
		AnalyticsManager.instance.CreateEvent("Connectivity_StartupEvent").AddProperty("Message", message).Send();
	}

	public static string GetStartupEvents()
	{
		if (startupEventLog == null)
		{
			return "";
		}
		return startupEventLog.ToString();
	}



	#region mycode
	public static IEnumerator StartupLoginReset()
	{
		if (EOSManager.Instance.GetEOSPlatformInterface() != null)
		{
			if (OfflineManager.IsResetEpicToken)
			{
				OfflineManager.Instance.SetResetEpicTokenValue(false);
				DebugTWD.Log("Old AccountUserId is " + EOSLogin.GetAccountUserId().ToString(), DebugType.Connection);

				EOSManager.Instance.RemovePersistentToken();
				yield return new WaitUntil(() => !string.IsNullOrEmpty(EOSLogin.GetUserDisplayName()));
				DebugTWD.Log("New AccountUserId is " + EOSLogin.GetAccountUserId().ToString(), DebugType.Connection);
			}
			LoginEpic();
		}
		OfflineManager.IsResetEpicToken = false;
	}
	#endregion
}