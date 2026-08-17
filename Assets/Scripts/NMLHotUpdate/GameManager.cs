using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using BaseModel;
using BaseModel.ContentTypes;
using Client.Connectivity;
using Fabric;
using Newtonsoft.Json;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using ThinkingAnalytics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TwdCustomMod;

public class GameManager : MonoBehaviour
{
	private class IpApiData
	{
		public string country_code;

		public static IpApiData CreateFromJson(string jsonString)
		{
			return JsonUtility.FromJson<IpApiData>(jsonString);
		}
	}

	[Serializable]
	private class ClientLogRequestBody
	{
		public string userId;

		public string message;

		public string time;

		public string event_type;
	}

	public const string StartupScene = "GameLoader";

	public const string PersistentElementsLevel = "startup";

	public const string SERVER_MESSAGE_PREFIX = "LOC_SERVER_";

	public const string MAINTENANCE_MODE_RESULT_STRING = "LOC_SERVER_WARNING_MAINTENANCE";

	public const string MAINTENANCE_MODE_STARTED_RESULT_STRING = "LOC_SERVER_DISCONNECT_MAINTENANCE";

	public const string SUPPORT_DISCONNECT_RESULT_STRING = "LOC_SERVER_DISCONNECT_SUPPORT";

	public const string DISCONNECT_OTHER_DEVICE = "LOC_SERVER_DISCONNECT_OTHER_DEVICE";

	public const string DISCONNECT_IDLE = "LOC_SERVER_DISCONNECT_IDLE";

	public const string PlayerPrefHashedIdKey = "HashedId";

	public const string ContentBaseUrlKey = "ContentBaseUrl";

	private static string CountryCode = string.Empty;

	public static DevFastTrackType DevFastTrackLoad = DevFastTrackType.None;

	public static bool StartedFromScenario = false;

	protected bool savePending;

	public bool PhoneCallResponseReceived = true;

	private bool defaultContentLoaded;

	private bool dismissPopupOnNextResume;

	public bool VersionUpgradeNeeded;

	public DateTime? VersionValidUntil;

	private string lastGedContentChecksum;

	private string pendingGedContentChecksum;

	private List<AsyncOperation> InitializationOps = new List<AsyncOperation>();

	private float lastOnlineCheckTime;

	protected bool memoryWarningReceivedInSession;

	public static float OfflineBuildSaveTime = 120000f;

	private IEnumerator loginCoroutine;

	private IEnumerator loadGEDCoroutine;

	private IEnumerator loadSceneCoroutine;

	private IEnumerator applicationPauseCoroutine;

	private IEnumerator applicationResumeFromPauseCoroutine;

	private IEnumerator downloadAssetBundleCoroutine;

	private IMessageSerializerFactory serializerFactory = new MessageSerializerFactory();

	private DeserializationWorker<PlayerModel> playerModelDeserializationWorker;

	private bool newPlayerLoadRequested;

	private static float DefaultConnectivityTimeoutInSeconds = 5f;

	private float connectivityTimeout;

	private bool isTimeoutSaved;

	private BuyBundleResultInfoList buyBundleResultInfoList;

	private long lastRequestBuyBundleResultInfoListTimestamp;

	private float offlineBuildSaveTimer;

	[HideInInspector]
	public IAPManager IAPManager;

	public readonly long LoadingScreenPauseTimeout = 30000L;

	public readonly long ForceReloadTimeout = 90000L;

	private CachedLeaderboardsManager cachedLeaderboardsManager;

	public const string userIdKey = "UserId";

	public const string gameEconomyDataKey = "GameEconomyData";

	public const string playerModelKey = "PlayerModel";

	public const string InstallationIdKey = "InstallationId";

	public const string saveVersionKey = "SaveVersion";

	public const string NetworkTimeoutKey = "NetworkTimeout";

	public const string guildModelKey = "OfflineGuildModel";

	public const string localCreateTimeKey = "LocalCreateTime";

	protected const string loadCount = "LoadCount";

	public const string PlayerSelectedLanguage = "PlayerSelectedLanguage";

	public const string PlayerSelectedDisplayMode = "PlayerSelectedDisplayMode";

	public readonly string[] DisplayModeKeyArray = new string[2] { "Popup.Settings.DisplayMode.FULLSCREEN.EPIC", "Popup.Settings.DisplayMode.WINDOWED.EPIC" };

	public const string PlayerSelectedScreenResolution = "PlayerSelectedScreenResolution";

	public static readonly int[] ScreenResolutionWidthArray = new int[11]
	{
		0, 800, 1024, 1280, 1280, 1600, 1680, 1920, 2048, 2560,
		3200
	};

	public static readonly int[] ScreenResolutionHeightArray = new int[11]
	{
		0, 600, 768, 720, 800, 1200, 1050, 1080, 1536, 1600,
		2000
	};

	public const string PlayerSelectedFrameRate = "PlayerSelectedFrameRate";

	public readonly string[] FrameRateArray = new string[4] { "30 FPS", "60 FPS", "90 FPS", "120 FPS" };

	private long deltaTimeOverflow;

	private long pauseTimeMillis;

	private long lastUtcCheck;

	private long timeResumed;

	private const float SecondsToMilliseconds = 1000f;

	private Dictionary<ModelObject, MonoBehaviour> modelViewMap;

	private Dictionary<Type, object> resourceMaps = new Dictionary<Type, object>();

	private OutfitResourcesMap outfitResources;

	private HeroSkinResourcesMap heroSkinResources;

	private RarityColorsResource rarityColorResources;

	private FactionColorsResource factionColorResources;

	private PrefabResource characterTemplate;

	private PrefabResource characterTemplatePortrait;

	private BundleCardsResource bundleCardsResources;

	private BundleRewardCardsResource bundleRewardCardsResources;

	private bool videoRewardCommandPending;

	private bool currentlyLoading;

	private PlayerEmblemResourcesMap _playerEmblemResources;

	private long timeBetweenheartBeatCommands = 120000L;

	private long lastUserActivityTime;

	private long lastCommandSendTime;

	protected LoadVisitParams loadVisitParams;

	private SmartTutorialData smartTutorialData;

	private List<KeyValuePair<int, string>> playerJsonStates = new List<KeyValuePair<int, string>>();

	private bool pushNotificationsEnabled;

	private bool pushNotificationsRequested;

	private static List<byte[]> debugAllocations = null;

	public bool IsPopupActivity;

	private LoginRequest loginRequest;

	private bool waitingGed;

	private bool waitingPlayer;

	private bool waitingLogin;

	private bool waitingPreload;

	private bool loginAborted;

	private long loginTime;

	private List<Type> environmentalActorsIgnoreList = new List<Type> { typeof(DamageVisualizationTask) };

	private Dictionary<Type, Type> visualizationTaskMap;

	private const string EnableNotificationsAfterTutorialPart = "Tutorial_Training_Ground";

	private const int EnableNotificationsAfterTutorialStep = 10;

	public static GameManager Instance { get; private set; }

	public GameState State { get; private set; }

	public GameSettings Settings { get; private set; }

	public bool BundleCheckDone { get; set; }

	public bool ShowTipsDone { get; set; }

	public Metrics.BundleSource BundleSource { get; set; }

	public MapCameraPosition MapCameraPosition { get; protected set; }

	public GameEconomyData gameEconomyData { get; protected set; }

	public MessageSerializer jsonSerializer { get; protected set; }

	public TWDModelManager modelManager { get; protected set; }

	public GuildManager GuildManager { get; protected set; }

	public BannerManager BannerManager { get; protected set; }

	public PlayerHubManager PlayerHubManager { get; protected set; }

	public GuildInviteFlow GuildInviteFlow { get; set; }

	public ITimingManager TimingManager { get; private set; }

	public static bool HasCommandLog
	{
		get
		{
			if (Instance != null && Instance.modelManager != null)
			{
				return Instance.modelManager.CurrentCommandLogEntry != null;
			}
			return false;
		}
	}

	public CommandLog CommandLog
	{
		get
		{
			if (Instance.modelManager == null)
			{
				return null;
			}
			return Instance.modelManager.CommandLog;
		}
	}

	public RollDiceLog RollDiceLog
	{
		get
		{
			if (Instance.modelManager == null)
			{
				return null;
			}
			return Instance.modelManager.RollDiceLog;
		}
	}

	public static bool IsInitialized
	{
		get
		{
			if (Instance != null && Instance.modelManager != null)
			{
				return Instance.playerModel != null;
			}
			return false;
		}
	}

	public bool IsGoreDisabled
	{
		get
		{
			if (playerModel == null || !playerModel.IsGoreDisabled)
			{
				return GameConfiguration.Instance.Config.LowViolence;
			}
			return true;
		}
	}

	public PlayerModel playerModel
	{
		get
		{
			if (modelManager != null)
			{
				return modelManager.Player;
			}
			return null;
		}
	}

	public GuildModel guildModel
	{
		get
		{
			if (IsLoadDataManager)
			{
				return OfflineManager.Instance.CurrentGuildModel;
			}
			if (GuildManager == null)
			{
				return null;
			}
			return GuildManager.Model;
		}
	}

	public BlackboardModel Blackboard => modelManager?.Player?.Blackboard;

	public GameObject CharacterTemplate => characterTemplate.GetPrefab();

	public GameObject CharacterTemplateForPortrait => characterTemplatePortrait.GetPrefab();

	public bool IsModelManagerInitialized => modelManager != null;

	public bool IsReturningFromAds { get; set; }

	public GameCenterManager GameCenterManager { get; private set; }

	public IFriendListManager FriendListManager { get; protected set; }

	public CachedLeaderboardsManager CachedLeaderboardsManager
	{
		get
		{
			if (cachedLeaderboardsManager == null)
			{
				cachedLeaderboardsManager = new CachedLeaderboardsManager();
			}
			return cachedLeaderboardsManager;
		}
	}

	public UnityAdsIds UnityAdsIds { get; private set; }

	public IAttackTargetModel ForceGoThatDetailMap { get; set; }

	public bool HasRatedApp
	{
		get
		{
			return TWDPlayerPrefs.GetInt("RateApp") == 1;
		}
		set
		{
			TWDPlayerPrefs.SetInt("RateApp", value ? 1 : 0);
		}
	}

	public static string ClientVersion => OfflineManager.ClientVersion;

	public static string UserId => TWDPlayerPrefs.GetString("UserId");

	public bool IsGameStarted { get; private set; }

	public bool CurrentlyLoading
	{
		get
		{
			return currentlyLoading;
		}
		private set
		{
			currentlyLoading = value;
		}
	}

	public PlayerEmblemResourcesMap PlayerEmblemResources
	{
		get
		{
			if (_playerEmblemResources == null)
			{
				_playerEmblemResources = UnityUtils.LoadFromAssetBundle<PlayerEmblemResourcesMap>("PlayerEmblemResources", "scriptableobjects");
			}
			return _playerEmblemResources;
		}
	}

	public SmartTutorialData SmartTutorialData
	{
		get
		{
			if (smartTutorialData == null)
			{
				smartTutorialData = UnityUtils.LoadFromAssetBundle<SmartTutorialData>("SmartTutorialData", "scriptableobjects");
			}
			return smartTutorialData;
		}
	}

	public List<SurvivorModel> SurvivorsFromMission { get; set; }

	public bool IsLoggedIn
	{
		get
		{
			if (!(SignalRClient.Instance != null))
			{
				return false;
			}
			if (SignalRClient.Instance.IsConnected)
			{
				return SignalRClient.Instance.CurrentSessionToken != null;
			}
			return false;
		}
	}

	public bool IsConnectedToServer
	{
		get
		{
			if (IsLoadDataManager) return SignalRClient.Instance != null && SignalRClient.Instance.IsConnected;

			if (GameConfiguration.Instance.Config.ConnectedToServer)
			{
				return DevFastTrackLoad == DevFastTrackType.None;
			}
			return false;
		}
	}

	public bool IsLoadLocalGed
	{
		get
		{
			if (GameConfiguration.Instance.Config.LoadLocalGed)
			{
				return DevFastTrackLoad == DevFastTrackType.None;
			}
			return false;
		}
	}

	public LoginRequest LoginRequest => loginRequest;

	public event LoadCompleted OnLoadCompleted;

	public void AddInitializationOp(AsyncOperation op, int priority = 1)
	{
		op.priority = priority;
		InitializationOps.Add(op);
	}

	private IEnumerator WaitForInitializationOps()
	{
		List<AsyncOperation> ops = InitializationOps;
		InitializationOps = new List<AsyncOperation>();
		int num = 0;
		for (int i = 0; i < ops.Count; i++)
		{
			if (!ops[i].isDone)
			{
				num++;
			}
		}
		if (num <= 0)
		{
			yield break;
		}
		for (int j = 0; j < ops.Count; j++)
		{
			while (!ops[j].isDone)
			{
				yield return null;
			}
		}
	}

	private bool IsIPodMusicPlaying()
	{
		return false;
	}

	public static void PauseIPodMusic()
	{
	}

	public static void ResumeIPodMusic()
	{
	}

	public bool ShouldAutoReloadOnError()
	{
		if (timeResumed == 0L)
		{
			return false;
		}
		long num = 120L;
		return DateTime.UtcNow.Ticks / 10000 - timeResumed < num * 1000;
	}

	public void StorePlayerJsonState(int sequenceId)
	{
		string value = jsonSerializer.Serialize(playerModel);
		playerJsonStates.Add(new KeyValuePair<int, string>(sequenceId, value));
		if (playerJsonStates.Count > 10)
		{
			playerJsonStates.RemoveAt(0);
		}
	}

	public string GetPlayerJsonState(int sequenceId)
	{
		if (playerJsonStates != null)
		{
			for (int num = playerJsonStates.Count - 1; num >= 0; num--)
			{
				if (playerJsonStates[num].Key == sequenceId)
				{
					return playerJsonStates[num].Value;
				}
			}
		}
		return null;
	}

	private IEnumerator SendRollDiceSnapshot()
	{
		string snapshotURL = SignalRClient.Instance.CurrentHostPort + "/player/rolldicesnapshot/" + UserId;
		string rollDiceSnapshotData = modelManager.GetRollDiceSnapshotData();
		if (rollDiceSnapshotData == null)
		{
			yield break;
		}
		ClientRollDiceSnapshot value = new ClientRollDiceSnapshot
		{
			RollDiceSnapshot = rollDiceSnapshotData
		};
		string s = modelManager.GetMessageSerializer().SerializeObject(value);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Content-Type", "application/json");
		WWW www = new WWW(snapshotURL, bytes, dictionary);
		float timeout = 20f;
		float startTime = Time.realtimeSinceStartup;
		while (true)
		{
			if (www.isDone)
			{
				if (!string.IsNullOrEmpty(www.error))
				{
					Debug.LogError("Could not send roll dice snapshot data to server '" + snapshotURL + "'. Error = '" + www.error + "'.");
				}
				break;
			}
			if (Time.realtimeSinceStartup - startTime > timeout)
			{
				Debug.LogError("Timeout sending roll dice snapshot data to server '" + snapshotURL + "'.");
				break;
			}
			yield return null;
		}
	}

	private IEnumerator SendModelSnapshotAndReload()
	{
		string snapshotURL = SignalRClient.Instance.CurrentHostPort + "/player/snapshot/" + UserId;
		ModelClientSnapshot modelClientSnapshot = new ModelClientSnapshot();
		try
		{
			modelClientSnapshot.ModelJson = modelManager.SerializeModel();
		}
		catch (Exception ex)
		{
			modelClientSnapshot.ModelJson = "ERROR:" + ex.ToString();
		}
		try
		{
			modelClientSnapshot.ExtraData = modelManager.GetModelSnapshotExtraData();
		}
		catch (Exception ex2)
		{
			modelClientSnapshot.ExtraData = "ERROR:" + ex2.ToString();
		}
		string s = modelManager.GetMessageSerializer().SerializeObject(modelClientSnapshot);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Content-Type", "application/json");
		WWW www = new WWW(snapshotURL, bytes, dictionary);
		float timeout = 20f;
		float startTime = Time.realtimeSinceStartup;
		while (true)
		{
			if (www.isDone)
			{
				if (!string.IsNullOrEmpty(www.error))
				{
					Debug.LogError("Could not send snapshot data to server '" + snapshotURL + "'. Error = '" + www.error + "'.");
				}
				break;
			}
			if (Time.realtimeSinceStartup - startTime > timeout)
			{
				Debug.LogError("Timeout sending snapshot data to server '" + snapshotURL + "'.");
				break;
			}
			yield return null;
		}
		LogReloadEvent("Reason: GameManager.SendModelSnapshotAndReload, desync");
		ReloadGame();
	}

	private void Awake()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("GameManager Awake with DataManager");
			if (Instance != null)
			{
				Destroy(base.gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(base.gameObject);
			jsonSerializer = new MessageSerializer();
			CreateVisualizationTaskMaps();
			modelViewMap = new Dictionary<ModelObject, MonoBehaviour>();
			Settings = new GameSettings();
			return;
		}
		Application.lowMemory -= OnMemoryWarning;
		Application.lowMemory += OnMemoryWarning;
		offlineBuildSaveTimer = OfflineBuildSaveTime;
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Application.logMessageReceived += OnLogCallback;
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		BuildConfigurationManager.Instance.Load();
		string item = "scene_audiosetup";
		string scenarioName = "audio_setup";
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle(new List<string> { item }, delegate
		{
			AssetBundleManager.Instance.LoadScene(scenarioName, LoadSceneMode.Additive);
		});
		jsonSerializer = new MessageSerializer();
		CreateVisualizationTaskMaps();
		modelViewMap = new Dictionary<ModelObject, MonoBehaviour>();
		Settings = new GameSettings();
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnServerMessage += HandleOnServerMessage;
			SignalRClient.Instance.OnCommandCompletedMessage += HandleOnCommandCompletedMessage;
			SignalRClient.Instance.OnBananaMessage += HandleOnBananaMessage;
			SignalRClient.Instance.OnBuySubscriptionMessage += HandleOBuySubscriptionMessage;
		}
		else
		{
			Debug.LogError("Could not register to SignalR client");
		}
		if (GameConfiguration.Instance.Config.ShowDebugMenu && TutorialView.Instance != null && TutorialView.Instance.StartupSetting != TutorialView.StartupSettingType.Normal)
		{
			ResetGameData();
		}
		CreateInstallationId();
		AnalyticsManager.instance.CreateEvent("Connectivity_ApplicationLaunched").AddProperty("VersionNumber", BuildConfigurationManager.Instance.VersionNumber).Send();
		Application.deepLinkActivated += OpenedWithDeepLink;
		if (!string.IsNullOrEmpty(Application.absoluteURL))
		{
			OpenedWithDeepLink(Application.absoluteURL);
		}
		StartCoroutine(GetCountryCodeByIP());
	}

	private IEnumerator DelayToUnloadSceneBundle(string scenarioABName)
	{
		yield return null;
		SingularityMonoBehaviour<AssetBundleController>.Instance.UnloadAssetBundleWithRealDependencies(scenarioABName);
	}

	private IEnumerator GetCountryCodeByIP()
	{
		yield return null;
		string text = new WebClient().DownloadString("https://api.ipify.org");
		string uri = "https://ipapi.co/" + text + "/json/";
		using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
		yield return webRequest.SendWebRequest();
		if (!webRequest.isNetworkError && !webRequest.isHttpError)
		{
			CountryCode = IpApiData.CreateFromJson(webRequest.downloadHandler.text).country_code;
		}
	}

	public void OnMemoryWarning()
	{
		if (!memoryWarningReceivedInSession)
		{
			AnalyticsManager.instance.CreateEvent("Performance_Memory_ReceivedWarning").Send();
			Debug.LogWarning("Received memory warning");
			memoryWarningReceivedInSession = true;
		}
		UIDrawCall.ReleaseInactive();
		Helpers.ClearUnusedMemory(gcCollect: true);
	}

	private IEnumerator SendClientLog(ClientLogRequestBody body)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest("https://serverlist.drillerservices.com/collect/collectLog", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));
		unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		unityWebRequest.SetRequestHeader("Content-Type", "application/json");
		unityWebRequest.SetRequestHeader("Authorization", "Bearer 9c5efb2fNML55cbNML4f0fNMLbe3dNML54a9352c066d");
		yield return unityWebRequest.SendWebRequest();
	}

	private void OnLogCallback(string message, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Warning || type == LogType.Exception || type == LogType.Assert)
		{
			string value = type.ToString();
			if (HelpersModel.IsOffThinkingAnalytics)
			{
				DebugTWD.LogMycode("if (HelpersModel.IsOffThinkingAnalytics) return");
				DebugTWD.LogWarning("OnLogCallback of type: " + value + "\nmessage: " + message + "\nstackTrace: " + stackTrace);
				return;
			}
			AnalyticsEvent analyticsEvent = AnalyticsManager.instance.CreateEvent("LogEntry").AddProperty("Type", value).AddProperty("Message", message);
			if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
			{
				analyticsEvent.AddProperty("StackTrace", stackTrace);
			}
			analyticsEvent.Send();
			if (!OfflineManager.IsLoadDataManager && (type == LogType.Error || type == LogType.Exception || type == LogType.Assert))
			{
				ClientLogRequestBody clientLogRequestBody = new ClientLogRequestBody();
				clientLogRequestBody.userId = TWDPlayerPrefs.GetString("UserId");
				clientLogRequestBody.message = message + "\n" + stackTrace;
				clientLogRequestBody.time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
				clientLogRequestBody.event_type = "log_error_entry";
				StartCoroutine(SendClientLog(clientLogRequestBody));
			}
		}
		if (type != LogType.Exception)
		{
			return;
		}
		bool flag = false;
		if (TutorialView.Instance != null && TutorialView.Instance.Running)
		{
			TutorialView.Instance.Stop();
		}
		else if (!flag && gameEconomyData != null && gameEconomyData.ConfigData.IgnoreRuntimeExceptions)
		{
			return;
		}
		if (flag && SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			if (!stackTrace.Contains("UnityEngine.InternalStaticBatchingUtility.CombineGameObjects"))
			{
				ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
				if (confirmationPopup != null)
				{
					confirmationPopup.SetContent("", message + "\n" + stackTrace);
					confirmationPopup.SetDebugText();
					confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Reload"));
					confirmationPopup.SetCallbacks(OnErrorReload);
					confirmationPopup.Open();
				}
			}
		}
		else
		{
			LogReloadEvent("Reason: GameManager.OnLogCallback");
			OnErrorReload();
		}
	}

	private void OnErrorReload()
	{
		ReloadGame();
	}

	private void ShowLoadError()
	{
		ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		obj.SetContent("Error", "Failed to load save game. Progress will be reset.");
		obj.SetOkButtonLabel("Reset game");
		obj.SetCallbacks(OnErrorReset);
		obj.Open();
	}

	private void OnErrorReset()
	{
		ResetGame();
	}

	private bool ExecuteGuildInvite(string deeplink)
	{
		string text = "://";
		int num = deeplink.IndexOf(text);
		if (num > -1)
		{
			string text2 = deeplink.Substring(0, num);
			if (text2 == GameConfiguration.Instance.Config.BundleURLScheme || (GameManager.ActiveBranch.Contains("release") && text2 == "twdnomansland"))
			{
				string[] array = deeplink.Substring(num + text.Length).Split('?');
				string text3 = array[0].ToLower();
				if (array.Length >= 2)
				{
					string[] array2 = array[1].Split('&');
					if (text3 == "guildinvite")
					{
						for (int i = 0; i < array2.Length; i++)
						{
							string[] array3 = array2[i].Split('=');
							if (array3.Length < 2)
							{
								continue;
							}
							string text4 = array3[0].ToLower();
							if (text4 == "g")
							{
								if (GuildInviteFlow == null)
								{
									GuildInviteFlow = new GuildInviteFlow();
								}
								GuildInviteFlow.GuildToJoinId = array3[1];
							}
							else if (text4 == "p")
							{
								if (GuildInviteFlow == null)
								{
									GuildInviteFlow = new GuildInviteFlow();
								}
								GuildInviteFlow.InviterHashedId = array3[1];
							}
						}
					}
				}
				return true;
			}
		}
		return false;
	}

	private bool ExecuteLoadPlayer(string deeplink)
	{
		string text = "://";
		int num = deeplink.IndexOf(text);
		string text2 = "";
		if (num > -1 && deeplink.Substring(0, num) == GameConfiguration.Instance.Config.BundleURLScheme)
		{
			string[] array = deeplink.Substring(num + text.Length).Split('?');
			string text3 = array[0].ToLower();
			if (array.Length >= 2)
			{
				string[] array2 = array[1].Split('&');
				if (text3 == "loadplayer")
				{
					for (int i = 0; i < array2.Length; i++)
					{
						string[] array3 = array2[i].Split('=');
						if (array3.Length >= 2 && array3[0].ToLower() == "p")
						{
							text2 = array3[1];
						}
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(text2))
		{
			TWDPlayerPrefs.SetString("UserId", text2.Trim());
			TWDPlayerPrefs.Save();
			newPlayerLoadRequested = true;
			if ((CampView.Instance != null && CampView.Instance.enabled) || (CombatView.Instance != null && CombatView.Instance.enabled))
			{
				AlertPopup.ShowPopupGetText("Generic.Info", "Load new player: " + text2, "Button.Reload", ReloadGame, AlertPopup.Priority.Critical);
			}
			else
			{
				ReloadGame();
			}
			return true;
		}
		return false;
	}

	public bool HandleNativeDeeplinkUrl(string deeplink)
	{
		if (deeplink.Contains("guildinvite"))
		{
			return ExecuteGuildInvite(deeplink);
		}
		if (deeplink.Contains("loadplayer"))
		{
			return ExecuteLoadPlayer(deeplink);
		}
		Debug.LogError("Deeplink url action not recognised" + deeplink);
		return false;
	}

	public void OpenedWithDeepLink(string deeplink)
	{
		if (deeplink.Contains("guildinvite"))
		{
			ExecuteGuildInvite(deeplink);
		}
		if (deeplink.Contains("loadplayer"))
		{
			ExecuteLoadPlayer(deeplink);
		}
		Debug.LogError("Deeplink url action not recognised" + deeplink);
	}

	private IEnumerator Start()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			yield break;
		}
		while (!defaultContentLoaded)
		{
			yield return null;
		}
		StartCoroutine(WaitToEnableNotificationsDuringTutorial());
		NotificationsSetup(schedule: false);
		EventManager.OnEvent += OnEvent;
		LoadResourceMap<EquipmentResourceEntry>("EquipmentsResources");
		LoadResourceMap<AbilityResourceEntry>("AbilitiesResources");
		LoadResourceMap<ActorResourceEntry>("ActorsResources");
		LoadResourceMap<InhabitantResourceEntry>("InhabitantsResources");
		LoadResourceMap<CharacterResourceEntry>("CharacterResources");
		outfitResources = UnityUtils.LoadFromAssetBundle<OutfitResourcesMap>("OutfitResources", "scriptableobjects");
		heroSkinResources = UnityUtils.LoadFromAssetBundle<HeroSkinResourcesMap>("HeroSkinResources", "scriptableobjects");
		rarityColorResources = UnityUtils.LoadFromAssetBundle<RarityColorsResource>("RarityColorsResources", "scriptableobjects");
		factionColorResources = UnityUtils.LoadFromAssetBundle<FactionColorsResource>("FactionColorsResources", "scriptableobjects");
		bundleCardsResources = UnityUtils.LoadFromAssetBundle<BundleCardsResource>("BundleCardsResources", "scriptableobjects");
		bundleRewardCardsResources = UnityUtils.LoadFromAssetBundle<BundleRewardCardsResource>("BundleRewardCardsResources", "scriptableobjects");
		characterTemplate = UnityUtils.LoadFromAssetBundle<PrefabResource>("CharacterTemplate", "scriptableobjects");
		if (characterTemplate == null)
		{
			Debug.LogError("Could not load the modular character template!");
		}
		characterTemplatePortrait = UnityUtils.LoadFromAssetBundle<PrefabResource>("CharacterTemplatePortrait", "scriptableobjects");
		if (characterTemplatePortrait == null)
		{
			Debug.LogError("Could not load the modular character portrait template!");
		}
		TimingManager = new CoroutineTimingManager();
	}

	public OutfitResourceEntry GetOutfitResourceEntry(string id)
	{
		if (outfitResources != null && outfitResources.Outfits != null)
		{
			for (int i = 0; i < outfitResources.Outfits.Count; i++)
			{
				OutfitResourceEntry outfitResourceEntry = outfitResources.Outfits[i];
				if (outfitResourceEntry.OutfitDefinitionID == id)
				{
					return outfitResourceEntry;
				}
			}
		}
		return null;
	}

	public HeroSkinResourceEntry GetHeroSkinResourceEntry(string heroId)
	{
		if (heroSkinResources != null && heroSkinResources.Skins != null)
		{
			for (int i = 0; i < heroSkinResources.Skins.Count; i++)
			{
				HeroSkinResourceEntry heroSkinResourceEntry = heroSkinResources.Skins[i];
				if (heroSkinResourceEntry.HeroDefinitionID == heroId)
				{
					return heroSkinResourceEntry;
				}
			}
		}
		return null;
	}

	public HeroSkinInfo GetHeroSkinInfoEntry(string skinPrefabId)
	{
		HeroSkinInfo heroSkinInfo = null;
		foreach (HeroSkinResourceEntry skin in heroSkinResources.Skins)
		{
			heroSkinInfo = skin.HeroSkins.FirstOrDefault((HeroSkinInfo x) => x.PrefabId == skinPrefabId);
			if (heroSkinInfo != null)
			{
				break;
			}
		}
		return heroSkinInfo;
	}

	private void HandleOnCommandCompletedMessage(int code, int sequenceId)
	{
		if (sequenceId == -1)
		{
			if (code == 85702)
			{
				Debug.LogError("Server RPC error:" + code);
				LogReloadEvent("Reason: GameManager.HandleOnCommandCompletedMessage, Server RPC error: " + code);
				OnErrorReload();
			}
			return;
		}
		if (sequenceId >= 0 && CommandLog != null)
		{
			CommandLog.ServerCommandResponse(sequenceId, code);
		}
		TWDModelResult tWDModelResult;
		switch (code)
		{
		case 37:
		{
			string[] obj = new string[5]
			{
				"Command execution skipped on server with code = ",
				code.ToString(),
				" (",
				null,
				null
			};
			tWDModelResult = (TWDModelResult)code;
			obj[3] = tWDModelResult.ToString();
			obj[4] = ").";
			Debug.LogWarning(string.Concat(obj));
			return;
		}
		case 0:
		case 42:
			return;
		}
		string[] obj2 = new string[5]
		{
			"Command execution failed on server with code = ",
			code.ToString(),
			" (",
			null,
			null
		};
		tWDModelResult = (TWDModelResult)code;
		obj2[3] = tWDModelResult.ToString();
		obj2[4] = "), reloading game.";
		Debug.LogError(string.Concat(obj2));
		TWDModelResult tWDModelResult2 = (TWDModelResult)code;
		if ((tWDModelResult2 == TWDModelResult.ModelListMismatch || tWDModelResult2 == TWDModelResult.CombatOccupancyMismatch || tWDModelResult2 == TWDModelResult.PlayerRandomMismatch) && Instance.modelManager.GameEconomyData.ConfigData.DebugPostLevel >= 1)
		{
			if (tWDModelResult2 == TWDModelResult.PlayerRandomMismatch)
			{
				StartCoroutine(SendRollDiceSnapshot());
			}
			StartCoroutine(SendModelSnapshotAndReload());
		}
		else
		{
			LogReloadEvent("Reason: GameManager.HandleOnCommandCompletedMessage, Command execution failed on server with code = " + code);
			ReloadGame();
		}
	}

	private void HandleOnBananaMessage(string message, string type)
	{
	}

	private void HandleOBuySubscriptionMessage(string message, string type)
	{
		if (!(type == "BuySubscription"))
		{
			return;
		}
		SubscriptionRPCCommand subscriptionRPCCommand = JsonConvert.DeserializeObject<SubscriptionRPCCommand>(message);
		if (subscriptionRPCCommand == null)
		{
			return;
		}
		if (modelManager.BuySubscription(subscriptionRPCCommand.SubscriptionId, subscriptionRPCCommand.Platform, subscriptionRPCCommand.ExpiryTimeMillis, subscriptionRPCCommand.GiveExtraReward))
		{
			BundleStoreDefinition bundleStoreDefinition = Instance.gameEconomyData.GetBundleStoreDefinition(subscriptionRPCCommand.SubscriptionId);
			BundleContentDefinition bundleContentDefinition = Instance.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
			UIEvent.Send("OnBundleBought", bundleStoreDefinition);
			if (CampView.Instance != null && subscriptionRPCCommand.GiveExtraReward == 1)
			{
				IAPConfirmPopupNew.OpenWithSubscriptionContent(bundleStoreDefinition, bundleContentDefinition, givenBySupport: false);
				Helpers.ExecuteCommand(new SubscriptionBundleViewedCommand());
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
				}
			}
		}
		else
		{
			Debug.LogError("SubscriptionBuyBundle execution failed BundleIdentifier: (" + message + "), reloading game.");
			ReloadGame();
		}
	}

	private void HandleOnServerMessage(string message, string type)
	{
		switch (type)
		{
		case "disconnect":
			if (message == "LOC_SERVER_DISCONNECT_IDLE")
			{
				dismissPopupOnNextResume = true;
			}
			LogReloadEvent("Reason: GameManager.HandleOnServerMessage, Disconnect");
			AlertPopup.ShowPopupGetText("Generic.Info", message, "Button.Reload", ReloadGame, AlertPopup.Priority.Critical);
			return;
		case "warning":
			Debug.LogWarning("Server warning:" + message);
			AlertPopup.ShowPopupGetText("Generic.Info", message, "Button.Ok", null);
			return;
		case "timeout":
			if (SignalRClient.Instance.HasError)
			{
				Debug.LogError("Server timeout:" + message);
				ShowConnectionLost();
				return;
			}
			break;
		}
		if (type == "error" && SignalRClient.Instance.HasError)
		{
			Debug.LogError("Server error:" + message);
			LogReloadEvent("Reason: GameManager.HandleOnServerMessage, Server error:" + message);
			OnErrorReload();
		}
	}

	public void ShowConnectionLost()
	{
		if (IsOfflineMode) return;
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
			dismissPopupOnNextResume = true;
			if (ShouldAutoReloadOnError())
			{
				AnalyticsManager.instance.CreateEvent("Connectivity_AutoReloadConnectionLost").Send();
				OnErrorReload();
			}
			else
			{
				LogReloadEvent("Reason: GameManager.ShowConnectionLost");
				AlertPopup.ShowPopupGetText("Error.ConnectionLost.Title", "Error.ConnectionLost.Message", "Error.ConnectionLost.Button", OnErrorReload, AlertPopup.Priority.Critical);
			}
		}
		else if (SingularityMonoBehaviour<LoadingScreenHUD>.Instance != null)
		{
			AnalyticsManager.instance.CreateEvent("Connectivity_ConnectionLost").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError();
		}
	}

	private void OnDestroy()
	{
		EventManager.OnEvent -= OnEvent;
		Application.logMessageReceived -= OnLogCallback;
		AnalyticsManager.instance.Deinit();
	}

	protected void checkRegisterPushNotification()
	{
		if (!AllowNotifications() || pushNotificationsEnabled || pushNotificationsRequested)
		{
			return;
		}
		SingularityMonoBehaviour<SDKManager>.Instance.AddPushTokenListener(delegate(string s)
		{
			SingularityMonoBehaviour<SDKManager>.Instance.ExternalManager.LogDebug($"checkRegisterPushNotification SDKManager Registering device PushToken: {s}");
			SignalRClient.Instance.RequestCommand("EnablePush", s, delegate
			{
				pushNotificationsEnabled = true;
			}, waitForResponse: true);
		});
		pushNotificationsRequested = true;
	}

	private void CheckForAirplaneMode()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!(realtimeSinceStartup > lastOnlineCheckTime + 1f))
		{
			return;
		}
		float networkTimeout = GetNetworkTimeout();
		if (IsConnectedToServer && UnityUtils.InternetReachability == NetworkReachability.NotReachable && !IsOfflineMode)
		{
			if (connectivityTimeout >= networkTimeout)
			{
				Debug.LogWarning($"Airplane mode enabled or no connectivity (with allowed timeout of {networkTimeout} s) - reloading game");
				connectivityTimeout = 0f;
				LogReloadEvent("Reason: GameManager.CheckForAirplaneMode, No connectivity with allowed timeout");
				ReloadGame();
			}
			else
			{
				connectivityTimeout += Time.deltaTime;
			}
		}
		else
		{
			connectivityTimeout = 0f;
		}
		lastOnlineCheckTime = realtimeSinceStartup;
	}

	private float GetNetworkTimeout()
	{
		if (gameEconomyData != null)
		{
			float num = gameEconomyData.ConfigData.NetworkConnectivityTimeout;
			if (isTimeoutSaved)
			{
				return num;
			}
			isTimeoutSaved = true;
			TWDPlayerPrefs.SetFloat("NetworkTimeout", num);
			return num;
		}
		return TWDPlayerPrefs.GetFloat("NetworkTimeout", DefaultConnectivityTimeoutInSeconds);
	}

	private void CheckForHeartbeat()
	{
		if (!(SignalRClient.Instance == null) && IsLoggedIn && timeBetweenheartBeatCommands != -1)
		{
			long num = DateTime.UtcNow.Ticks / 10000;
			if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
			{
				lastUserActivityTime = num;
			}
			lastCommandSendTime = Math.Max(lastCommandSendTime, SignalRClient.Instance.Statistics.LastSendTimeTicks / 10000);
			long num2 = num - lastCommandSendTime;
			if (num2 < 0)
			{
				lastCommandSendTime = num;
			}
			if (num2 > timeBetweenheartBeatCommands && lastUserActivityTime > lastCommandSendTime)
			{
				Helpers.ExecuteCommand(new TickModelCommand());
				lastCommandSendTime = num;
			}
		}
	}

	private void Update()
	{
		if (!IsShowModInPlayer) return;

		UpdateGesture();
		SwitchMod();
		GetRandomChange();
		RedrawMenu();

		if (IsLoadDataManager) return;

		CheckForHelpShiftGesture();
		CheckForAirplaneMode();
		checkRegisterPushNotification();
		AnalyticsManager.instance.Update();

		if (!IsGameStarted)
		{
			return;
		}
		if (lastUtcCheck == 0L)
		{
			lastUtcCheck = DateTime.UtcNow.Ticks / 10000;
		}
		long num = DateTime.UtcNow.Ticks / 10000;
		long num2 = Math.Abs(num - lastUtcCheck);
		deltaTimeOverflow += num2;
		lastUtcCheck = num;
		if (deltaTimeOverflow >= 200)
		{
			long num3 = deltaTimeOverflow / 200 * 200;
			modelManager.TickModel(num3);
			deltaTimeOverflow -= num3;
		}
		if (!IsConnectedToServer)
		{
			offlineBuildSaveTimer -= num2;
			if (offlineBuildSaveTimer <= 0f)
			{
				SaveLocalPlayer();
				offlineBuildSaveTimer = OfflineBuildSaveTime;
			}
		}
		if (Input.GetKeyUp(KeyCode.Escape) && !CurrentlyLoading)
		{
			HandleBackButton();
		}
		CheckForHeartbeat();
	}

	public static string GetCountryCode()
	{
		return CountryCode;
	}

	private static bool IsOnWifi()
	{
		return SingularityMonoBehaviour<SDKManager>.Instance.NextActivity.IsOnWifi();
	}

	public static bool CanOpenURLScheme(string urlScheme)
	{
		return false;
	}

	public static bool NativeIOSCrash()
	{
		return false;
	}

	public void HandleBackButton()
	{
		if (State == GameState.None || TutorialView.Instance == null)
		{
			return;
		}
		HUDElement popupOnTop = SingularityMonoBehaviour<HUDManager>.Instance.GetPopupOnTop();
		if (popupOnTop != null)
		{
			if (BackButtonShouldClosePopup())
			{
				popupOnTop.OnBackButtonClicked();
			}
		}
		else if (BackButtonShouldExitCombat())
		{
			CombatView.Instance.CombatHUD.OnGoToMenu(null);
		}
		else if (BackButtonShouldUnselectBuilding())
		{
			if (CampView.Instance.CampViewBuildings.SelectedBuilding.IsTemporary)
			{
				UIEvent.Send("OnBuildingMoveCancelled");
			}
			else if (CampView.Instance.CampViewBuildings.Moving)
			{
				UIEvent.Send("OnBuildingMoveCancelled", CampView.Instance.CampViewBuildings.SelectedBuilding.Model);
			}
			else
			{
				CampView.Instance.CampViewBuildings.UnselectBuilding();
			}
		}
		else if (BackButtonShouldPromptForQuit())
		{
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.ConfirmApplicationQuit.Title"), LocalizationManager.GetText("Popup.ConfirmApplicationQuit.Message"), LocalizationManager.GetText("Popup.ConfirmApplicationQuit.Confirm"), ApplicationQuit, LocalizationManager.GetText("Popup.ConfirmApplicationQuit.Cancel"));
		}
	}

	private bool BackButtonShouldUnselectBuilding()
	{
		if (CampView.Instance != null && !TutorialView.Instance.IsTutorialUIEnabled() && (!TutorialView.Instance.Running || (TutorialView.Instance.Running && TutorialView.Instance.IsSuggesting)) && CampView.Instance.IsShown)
		{
			return CampView.Instance.CampViewBuildings.SelectedBuilding != null;
		}
		return false;
	}

	private bool BackButtonShouldPromptForQuit()
	{
		if (CampView.Instance != null && CampView.Instance.IsShown)
		{
			return CampView.Instance.CampViewBuildings.SelectedBuilding == null;
		}
		return false;
	}

	private bool BackButtonShouldExitCombat()
	{
		if (State == GameState.Combat && !TutorialView.Instance.IsTutorialUIEnabled() && !TutorialView.Instance.Running && CombatView.Instance != null && CombatView.Instance.CombatHUD != null)
		{
			return CombatView.Instance.CombatHUD.IsMenuButtonActive();
		}
		return false;
	}

	private bool BackButtonShouldClosePopup()
	{
		if (TutorialView.Instance != null)
		{
			return !TutorialView.Instance.IsWaitingForClick;
		}
		return true;
	}

	public bool IsSocialEnabled()
	{
		ConfigData configData = gameEconomyData.ConfigData;
		if (!configData.SocialFeaturesEnabled)
		{
			return false;
		}
		if (configData.SocialFeaturesCountryFilter == null || configData.SocialFeaturesCountryFilter.Count == 0)
		{
			return true;
		}
		if (playerModel != null && playerModel.HasSeenSocial)
		{
			return true;
		}
		string countryCode = GetCountryCode();
		if (countryCode == null)
		{
			return true;
		}
		countryCode = countryCode.ToLower();
		for (int i = 0; i < configData.SocialFeaturesCountryFilter.Count; i++)
		{
			if (configData.SocialFeaturesCountryFilter[i].ToLower().Trim() == countryCode)
			{
				Helpers.ExecuteCommand(new HasSeenSocialCommand());
				return true;
			}
		}
		if (modelManager.Player.HasGuild)
		{
			return true;
		}
		return false;
	}

	private void NotifyLoadCompleted()
	{
		this.OnLoadCompleted?.Invoke();
	}

	public void ResetGame()
	{
		ResetGameData();
		LogReloadEvent("Reason: GameManager.ResetGame");
		ReloadGame();
	}

	public void ReloadGame()
	{
		Screen.sleepTimeout = -2;
		SignalRClient instance = SignalRClient.Instance;
		if (instance != null)
		{
			instance.Disconnect();
			instance.ClearError();
		}
		State = GameState.None;
		dismissPopupOnNextResume = false;
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.DeleteAll();
		}
		ClearModelViewMapForReload();
		TooltipManager.DestroyAllAndClear();
		if (TutorialView.Instance != null && TutorialView.Instance.Running)
		{
			TutorialView.Instance.Stop();
		}
		if (SingularityMonoBehaviour<FullscreenActorOverlay>.Instance != null)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.close();
		}
		LoadingScreenCombat.Remove();
		GoToLoaderScene();
	}

	private void ClearModelViewMapForReload()
	{
		if (modelViewMap != null && modelViewMap.Count != 0)
		{
			modelViewMap.Clear();
		}
	}

	public void ReloadGameDelayed()
	{
		StartCoroutine(ReloadGameDelayedRoutine());
	}

	private IEnumerator ReloadGameDelayedRoutine()
	{
		yield return null;
		Instance.ReloadGame();
	}

	public void WaitCommandQueueAndReload()
	{
		StartCoroutine(WaitCommandQueueAndReloadRoutine());
	}

	private IEnumerator WaitCommandQueueAndReloadRoutine()
	{
		while (SignalRClient.Instance.IsWaitingForResponse)
		{
			yield return null;
		}
		Instance.ReloadGame();
	}

	public void GiveSurvivors(int count)
	{
		for (int i = 0; i < 10; i++)
		{
			SurvivorModel survivor = playerModel.SurvivorContainer.CreateRandomSurvivor(2);
			if (playerModel.SurvivorContainer.CanAddSurvivor())
			{
				playerModel.SurvivorContainer.AddSurvivor(survivor);
				continue;
			}
			break;
		}
	}

	public void GiveSurvivor(SurvivorClass requestedClass, string requestedAssetName)
	{
		SurvivorModel survivor = playerModel.SurvivorContainer.CreateRandomSurvivor(1, 1, 1, 0, requestedClass, requestedAssetName);
		if (playerModel.SurvivorContainer.CanAddSurvivor())
		{
			playerModel.SurvivorContainer.AddSurvivor(survivor);
		}
	}

	public void GiveRandomEquipment()
	{
		EquipmentItemModel equipmentItemModel = playerModel.Equipment.GenerateRandomEquipment();
		if (equipmentItemModel != null)
		{
			playerModel.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Debug);
		}
	}

	public void GiveRandomHero()
	{
		List<ActorDefinition> actorDefinitions = playerModel.gameEconomyData.ActorDefinitions;
		List<string> list = new List<string>();
		for (int i = 0; i < (actorDefinitions?.Count ?? 0); i++)
		{
			if (actorDefinitions[i].ID.Contains("Hero_"))
			{
				list.Add(actorDefinitions[i].ID);
			}
		}
		for (int j = 0; j < ((playerModel.SurvivorContainer.Survivors != null) ? playerModel.SurvivorContainer.Survivors.Count : 0); j++)
		{
			if (playerModel.SurvivorContainer.Survivors[j].IsHero)
			{
				list.Remove(playerModel.SurvivorContainer.Survivors[j].ActorDefinitionID);
			}
		}
		if (list.Count > 0)
		{
			int index = new System.Random().Next(0, list.Count - 1);
			SurvivorModel survivor = playerModel.SurvivorContainer.CreateHero(list[index]);
			if (!playerModel.SurvivorContainer.ContainsSurvivor(survivor))
			{
				playerModel.SurvivorContainer.AddSurvivor(survivor);
			}
		}
	}

	public void GiveFirstTierEquipments()
	{
		EquipmentCategory[] array = new EquipmentCategory[3]
		{
			EquipmentCategory.MeleeWeapon,
			EquipmentCategory.RangeWeapon,
			EquipmentCategory.Armor
		};
		for (int i = 0; i < 3; i++)
		{
			EquipmentCategory category = array[i];
			for (int j = 0; j < 5; j++)
			{
				EquipmentItemModel equipmentItemModel = Instance.playerModel.Equipment.GenerateRandomEquipment(category, 1);
				if (equipmentItemModel != null)
				{
					Instance.playerModel.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Debug);
				}
			}
		}
	}

	public void PrintAllPossibleWeaponsGivenOnCurrentLevel()
	{
	}

	public void PrintAllPossibleWeaponsByTier()
	{
	}

	public void PrintEquipmentsStats()
	{
	}

	public static void DebugAllocateMemory(int megaBytes)
	{
		if (debugAllocations == null)
		{
			debugAllocations = new List<byte[]>();
		}
		byte[] array = new byte[megaBytes * 1024 * 1024];
		for (int i = 0; i < megaBytes * 1024 * 1024; i++)
		{
			array[i] = (byte)(i & 0xFF);
		}
		debugAllocations.Add(array);
	}

	public void PrintRadioCallSimulation(int count)
	{
		PrintRadioCallSimulationImpl(count, standardCalls: true, 3, 999, 0);
	}

	public void PrintRadioCallSimulationForSpecialCallSlot(int count, int slot, int rerollingAggressiveness)
	{
		PrintRadioCallSimulationImpl(count, standardCalls: false, slot, slot, rerollingAggressiveness);
	}

	private bool WouldPlayerRerollCallLoot(LootEntry loot, int rerollingAggressiveness)
	{
		if (rerollingAggressiveness == 0)
		{
			return false;
		}
		if (loot.RewardedCurrency == CurrencyType.None)
		{
			if (loot.RewardedRarityLevel < 3 || ((loot.GeneratedSurvivor.SurvivorClass == SurvivorClass.Scout || loot.GeneratedSurvivor.SurvivorClass == SurvivorClass.Bruiser) && loot.RewardedRarityLevel == 3 && rerollingAggressiveness >= 2))
			{
				return true;
			}
		}
		else if (loot.RewardedAmount < 32 || (loot.RewardedAmount <= 32 && rerollingAggressiveness >= 2))
		{
			return true;
		}
		return false;
	}

	private void PrintRadioCallSimulationImpl(int count, bool standardCalls, int startSlotNumber, int endSlotNumberInclusive, int rerollingAggressiveness)
	{
	}

	public void PrintGenerateBadgesDebugOld()
	{
	}

	public void PrintGenerateBadgesDebug()
	{
	}

	private List<CurrencyType> CreateListOfCurrencies(System.Random random, List<CurrencyType> baseComponents, int[] rarities)
	{
		List<CurrencyType> list = new List<CurrencyType>();
		list.Add(ComponentHelper.GetCurrencyFromBaseAndRarity(CurrencyType.Badge0, rarities[random.Next(5)]));
		for (int i = 0; i < 4; i++)
		{
			list.Add(ComponentHelper.GetCurrencyFromBaseAndRarity(baseComponents[random.Next(1, 5)], rarities[random.Next(5)]));
		}
		return list;
	}

	public void SetMapCameraPosition(Vector3 cameraPosition, float cameraDistance, MapCameraSaveReason saveReason)
	{
		MapCameraPosition = new MapCameraPosition();
		MapCameraPosition.SavedCameraTarget = cameraPosition;
		MapCameraPosition.SavedCameraDistance = cameraDistance;
		MapCameraPosition.MapCameraSaveReason = saveReason;
	}

	public void ClearMapCameraPosition()
	{
		MapCameraPosition = null;
	}

	public static void ResetGameData()
	{
		string value = TWDPlayerPrefs.GetString("InstallationId");
		int num = PlayerPrefs.GetInt("IDFAPopupAnswer", -1);
		TWDPlayerPrefs.DeleteAll();
		TWDPlayerPrefs.SetString("InstallationId", value);
		if (num != -1)
		{
			PlayerPrefs.SetInt("IDFAPopupAnswer", num);
		}
	}

	private string getDeviceId()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}

	private LoginRequest InitializeLoginRequest()
	{
		string advertisingIdentifier = "";
		string deviceId = getDeviceId();
		string deviceModelId = "";
		int licenseValidationStatus = 0;
		DeviceInfo device = new DeviceInfo
		{
			CountryCode = GetCountryCode(),
			Device = SystemInfo.deviceModel,
			Platform = Application.platform.ToString(),
			OsVersion = SystemInfo.operatingSystem,
			AdvertisingIdentifier = advertisingIdentifier,
			GraphicsDeviceName = SystemInfo.graphicsDeviceName,
			GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion,
			DeviceId = deviceId,
			DeviceModelId = deviceModelId,
			TotalMemory = SystemInfo.systemMemorySize + SystemInfo.graphicsMemorySize,
			GraphicsMemory = SystemInfo.graphicsMemorySize
		};
		BaseModel.TDPresetProperties tDPresetProperties = new BaseModel.TDPresetProperties
		{
			AppVersion = "default",
			BundleId = "default",
			Carrier = "default",
			DeviceId = "default",
			DeviceModel = "default",
			Manufacturer = "default",
			NetworkType = "default",
			OS = "default",
			OSVersion = "default",
			ScreenHeight = 0.0,
			ScreenWidth = 0.0,
			SystemLanguage = "default",
			ZoneOffset = 0.0,
			InstallTime = "default",
			Disk = "default",
			Ram = "default",
			Fps = 0.0,
			Simulator = false
		};
		if (OfflineManager.Instance.ConnectSourceCurrent == OfflineManager.ConnectSource.Epic)
			tDPresetProperties.Channel = "epic";

		if (SingularityMonoBehaviour<SDKManager>.Instance != null)
		{
			ThinkingAnalytics.TDPresetProperties tdPresetProperties = SingularityMonoBehaviour<SDKManager>.Instance.GetTdPresetProperties();
			if (tdPresetProperties != null)
			{
				tDPresetProperties.AppVersion = tdPresetProperties.AppVersion;
				tDPresetProperties.BundleId = tdPresetProperties.BundleId;
				tDPresetProperties.Carrier = tdPresetProperties.Carrier;
				tDPresetProperties.DeviceId = tdPresetProperties.DeviceId;
				tDPresetProperties.DeviceModel = tdPresetProperties.DeviceModel;
				tDPresetProperties.Manufacturer = tdPresetProperties.Manufacturer;
				tDPresetProperties.NetworkType = tdPresetProperties.NetworkType;
				tDPresetProperties.OS = tdPresetProperties.OS;
				tDPresetProperties.OSVersion = tdPresetProperties.OSVersion;
				tDPresetProperties.ScreenHeight = tdPresetProperties.ScreenHeight;
				tDPresetProperties.ScreenWidth = tdPresetProperties.ScreenWidth;
				tDPresetProperties.SystemLanguage = tdPresetProperties.SystemLanguage;
				tDPresetProperties.ZoneOffset = tdPresetProperties.ZoneOffset;
				tDPresetProperties.InstallTime = tdPresetProperties.InstallTime;
				tDPresetProperties.Disk = tdPresetProperties.Disk;
				tDPresetProperties.Ram = tdPresetProperties.Ram;
				tDPresetProperties.Fps = tdPresetProperties.Fps;
				tDPresetProperties.Simulator = tdPresetProperties.Simulator;
				tDPresetProperties.Channel = GetPlayerData.GetChannel(OfflineManager.Instance.ConnectSourceCurrent);
			}
		}

		var request = new LoginRequest
		{
			ClientVersion = ClientVersion,
			ClientModelVersion = OfflineManager.ShortVersion,
			HotFixVersion = GameStart.hotfixVersion,
			InstallationId = TWDPlayerPrefs.GetString("InstallationId"),
			BuildId = BuildConfigurationManager.Instance.BuildId,
			InstallDateStamp = 0L,
			LicenseValidationStatus = licenseValidationStatus,
			Device = device,
			TDPresetProperties = tDPresetProperties
		};

		if (OfflineManager.Instance.ConnectSourceCurrent == OfflineManager.ConnectSource.Epic)
		{
			Dictionary<string, string> dictionary = new()
			{
				["SocialAccountName"] = EOSLogin.GetUserDisplayName()
			};

			request.PcPlatform = new PcPlatform
			{
				Data = dictionary,
				PcPlatformType = AccountType.WindowsEditor,
				PcAccountId = EOSLogin.GetAccountUserId().ToString(),
				PcAccessToken = EOSLogin.GetAccessToken(),
				PcRefreshToken = EOSLogin.GetRefreshToken()
			};
		}

		return request;
	}

	public IEnumerator Login()
	{
		if (SingularityMonoBehaviour<GdprFlowHandler>.Instance != null)
		{
			Startup.LogStartupEvent("Start_GDPR");
			yield return SingularityMonoBehaviour<GdprFlowHandler>.Instance.HandlePreLogin();
		}
		if (loginRequest == null)
		{
			loginRequest = InitializeLoginRequest();
		}
		loginRequest.Identification = UserId;
		loginRequest.ModelChecksum = "";
		loginRequest.CurrentDateStamp = Helpers.DateTimeToUnixTime(DateTime.UtcNow);
		loginRequest.InstallLaunchCount = 1L;
		loginRequest.LastSessionDateStamp = 0L;
		loginRequest.Device.Wifi = IsOnWifi();
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		Startup.LogStartupEvent("Login");
		LoadingScreenHUD.SetLoadingMessage(LoadingMessageType.Login);
		GameManager gameManager = this;
		GameManager gameManager2 = this;
		bool isGameStarted = false;
		gameManager2.loginAborted = false;
		gameManager.IsGameStarted = isGameStarted;
		GameManager gameManager3 = this;
		GameManager gameManager4 = this;
		GameManager gameManager5 = this;
		GameManager gameManager6 = this;
		isGameStarted = true;
		gameManager6.waitingLogin = true;
		gameManager3.waitingPreload = (gameManager4.waitingGed = (gameManager5.waitingPlayer = isGameStarted));
		if (IsConnectedToServer)
		{
			Debug.LogError("Login");
			SignalRClient.Instance.RequestCommand("Login", jsonSerializer.Serialize(loginRequest), OnLogin, waitForResponse: true);
		}
		else
		{
			Debug.LogError("OfflineLogin");
			OfflineLogin();
		}
		StartCoroutine(PreloadPrefabs());
		float startTime = Time.realtimeSinceStartup;
		while (waitingPreload || waitingGed || waitingPlayer || waitingLogin || !playerModelDeserializationWorker.Ready)
		{
			if (loginAborted || SignalRClient.Instance.HasError)
			{
				yield break;
			}
			if (Time.realtimeSinceStartup - startTime > 60f)
			{
				Debug.LogWarning("Login timeout");
				AnalyticsManager.instance.CreateEvent("GameLoad_Login_Timeout").Send();
				SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError();
				yield break;
			}
			yield return null;
		}
		modelManager.LoadModel(playerModelDeserializationWorker.Result);
		playerModelDeserializationWorker = null;
		Startup.LogStartupEvent("PlayerDeserialized");
		yield return null;
		loginCoroutine = null;
		StartModelManager();
		if (!IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (!IsLoadDataManager)");
			if (SingularityMonoBehaviour<GdprFlowHandler>.Instance != null)
			{
				yield return SingularityMonoBehaviour<GdprFlowHandler>.Instance.HandlePostLogin(playerModel);
				Startup.LogStartupEvent("End_GDPR");
			}
			if (SingularityMonoBehaviour<GdprFlowHandler>.Instance != null)
			{
				yield return SingularityMonoBehaviour<GdprFlowHandler>.Instance.HandlePostLoginIDFAScreen();
			}
		}
		LoginCompleted();
		if (!IsLoadDataManager)
		{
			if (Helpers.ExecuteCommand(new LoginCompleterePortingTdAnalyticsCommand()) != TWDModelResult.OK)
			{
				Debug.LogError("LoginCompleterePortingTdAnalyticsCommand is not the current state");
			}
			AnalyticsManager.instance.CreateEvent("Connectivity_GameLoaded").Send();
		}
	}

	public void RequestPltv()
	{
	}

	private void OnPltvValueData(string message)
	{
		if (SignalRClient.Instance.HasError)
		{
			SignalRClient.Instance.ClearError();
			return;
		}
		PltvResponse pltvResponse = modelManager.GetMessageSerializer().DeserializeObject<PltvResponse>(message);
		SingularityMonoBehaviour<SDKManager>.Instance.SkAdNetworkController.UpdateConversionValueClick(pltvResponse.Value);
		Helpers.ExecuteCommand(new SendPLTVValueMetricCommand(pltvResponse.Value));
	}

	private void OnLogin(string loginResponseJson)
	{
		DebugTWD.Log("OnLogin response: " + '\n' + loginResponseJson, DebugType.Connection);

		if (!waitingLogin)
		{
			Debug.LogError("Unexpected OnLogin " + loginResponseJson);
			return;
		}
		if (string.IsNullOrEmpty(loginResponseJson) || SignalRClient.Instance.HasError)
		{
			Debug.LogError("GameLoad_Login_LoadError ");
			AnalyticsManager.instance.CreateEvent("GameLoad_Login_LoadError").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError();
			loginAborted = true;
			return;
		}
		LoginResponse loginResponse = jsonSerializer.Deserialize<LoginResponse>(loginResponseJson);
		if (loginResponse.State == GameHostState.Redirect)
		{
			Debug.LogError("Login Redirect " + loginResponse.Address);
			Startup.ConnectionUrl = loginResponse.Address;
			LogReloadEvent("Reason: GameManager.OnLogin, Login Redirect");
			ReloadGame();
			loginAborted = true;
			return;
		}
		if (loginResponse.State == GameHostState.NoNewConnections)
		{
			Debug.LogError("Login NoNewConnections " + loginResponse.Address);
			LogReloadEvent("Reason: GameManager.OnLogin, Login NoNewConnections");
			ReloadGame();
			loginAborted = true;
			return;
		}
		if (loginResponse.State == GameHostState.Maintenance)
		{
			Debug.LogError("Connectivity_InitFailed_Maintenance " + loginResponse);
			AnalyticsManager.instance.CreateEvent("Connectivity_InitFailed_Maintenance").Send();
			long endTime = 3600000L;
			if (loginResponse.Maintenance != null)
			{
				endTime = loginResponse.Maintenance.EndingTimeStamp - Helpers.DateTimeToUnixTime(DateTime.UtcNow) * 1000;
			}
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.ShowMaintenanceBreak(endTime);
			loginAborted = true;
			return;
		}
		if (loginResponse.State == GameHostState.Upgrade || loginResponse.State == GameHostState.UpgradedClientUsed)
		{
			Debug.LogError("Connectivity_VersionMismatch " + loginResponse);
			AnalyticsManager.instance.CreateEvent("Connectivity_VersionMismatch").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.ShowVersionMismatch();
			loginAborted = true;
			return;
		}
		if (loginResponse.LockState != null && loginResponse.LockState.IsLocked)
		{
			Debug.LogError("GameLoad_Login_Locked " + loginResponse);
			AnalyticsManager.instance.CreateEvent("GameLoad_Login_Locked").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.ShowPlayerLocked(loginResponse.LockState);
			loginAborted = true;
			return;
		}
		if (loginResponse.State == GameHostState.Erased)
		{
			Debug.LogError("GameLoad_Login_Erased " + loginResponse);
			AnalyticsManager.instance.CreateEvent("GameLoad_Login_Erased").Send();
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.ShowPlayerLocked(new LockRespond
			{
				Status = LockRespond.LockStatus.PlayerDisabled,
				Reason = "Erased"
			});
			loginAborted = true;
			return;
		}
		string identification = loginResponse.Identification;
		if (!string.IsNullOrEmpty(identification) && identification != loginRequest.Identification)
		{
			DebugTWD.Log("UserId first identification : " + identification, DebugType.Connection);

			TWDPlayerPrefs.SetString("UserId", identification);
			TWDPlayerPrefs.Save();
			AnalyticsManager.instance.CreateEvent("Connectivity_NewUser").Send();
			SingularityMonoBehaviour<SDKManager>.Instance.SetAccountId(identification);
			SingularityMonoBehaviour<SDKManager>.Instance.SetSingularAccountId(identification);
		}
		SignalRClient.Instance.SetSessionToken(loginResponse.SessionToken);
		SignalRClient.Instance.SetDirectUrl(loginResponse.Address);
		Debug.LogError("OnLogin " + loginResponseJson);
		waitingLogin = false;
	}

	public void OnLoadGed(string url, string checksum)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log($"OnLoadGed begin: {url}", DebugType.SignalR);
			if (OfflineManager.Instance.IsGedLoaded) return;
			GetPlayerData.Instance.OnLoadGed(url);
			return;
		}
		if (!waitingGed)
		{
			Debug.LogError("Unexpected OnLoadGed");
			return;
		}
		_ = deltaTimeOverflow;
		_ = 0;
		deltaTimeOverflow = 0L;
		string extractedChecksum = null;
		TryExtractChecksumFromUrl(url, out extractedChecksum, "GameLoad_GameEconomyData", "GED");
		pendingGedContentChecksum = extractedChecksum;
		Startup.LogStartupEvent("OnGEDURL");
		LoadingScreenHUD.SetLoadingMessage(LoadingMessageType.LoadGameEconomyData);
		if (CanReuseStartedGameEconomyData(extractedChecksum))
		{
			Helpers.StartCoroutine(this, ReuseStartedGameEconomyData(), ref loadGEDCoroutine);
		}
		else
		{
			ContentManager.Instance.GetCDNContent<string>(url, "GED", "GameEconomyData", OnGameEconomyData, extractedChecksum);
		}
	}

	public void OnLoadPlayer(long time, string json, string checksum)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("OnLoadPlayer begin", DebugType.SignalR);
			GetPlayerData.Instance.OnLoadPlayer(json);
			waitingPlayer = false;
			return;
		}
		if (!waitingPlayer)
		{
			Debug.LogError("Unexpected OnLoadPlayer " + checksum);
			return;
		}
		if (string.IsNullOrEmpty(json))
		{
			json = ContentManager.Instance.GetCache("Player").GetContentById<string>("PlayerModel");
			if (string.IsNullOrEmpty(json))
			{
				Debug.LogError("No player json!");
				AnalyticsManager.instance.CreateEvent("GameLoad_PlayerModel_LoadError").Send();
				SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError();
				loginAborted = true;
				return;
			}
		}
		loginTime = time;
		Startup.LogStartupEvent("PlayerModelReceived");
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		if (modelManager != null && playerModel != null)
		{
			playerModel.Changed -= OnPlayerChange;
		}
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		if (modelManager != null)
		{
			modelManager.ActionExecuted -= OnActionExecuted;
		}
		modelManager = new TWDModelManager(Debug.isDebugBuild || OfflineManager.IsDebug);
		modelManager.SetModelManagerMode(ModelManagerMode.Client);
		modelManager.SetModelDebug(new UnityModelDebug());
		modelManager.SetModelAnalytics(new AnalyticsClient());
		modelManager.SetContentService(ContentManager.Instance);
		if (IsConnectedToServer)
		{
			modelManager.SetCommandTransport(new TWDModelCommandTransport());
		}
		playerModelDeserializationWorker = new DeserializationWorker<PlayerModel>(serializerFactory.CreateSerializer(SerializerType.NewtonSoft), json);
		playerModelDeserializationWorker.Start();
		waitingPlayer = false;
	}

	public static bool TryExtractChecksumFromUrl(string url, out string extractedChecksum, string analyticsEventPrefix, string analyticsPropertyPrefix)
	{
		extractedChecksum = null;
		if (string.IsNullOrEmpty(url))
		{
			return false;
		}
		int num = url.LastIndexOf('/');
		if (num < 0)
		{
			Debug.LogWarning("Invalid content url: " + url);
			AnalyticsManager.instance.CreateEvent(analyticsEventPrefix + "_InvalidGedUrl").AddProperty(analyticsPropertyPrefix + "Url", url).Send();
			return false;
		}
		num++;
		int num2 = url.IndexOf('.', num);
		int length = ((num2 >= 0) ? num2 : url.Length) - num;
		extractedChecksum = url.Substring(num, length);
		if (!ContentCache.CheckIsValidChecksum(extractedChecksum))
		{
			Debug.LogWarning("Invalid content checksum " + extractedChecksum + " as extracted from url " + url);
			AnalyticsManager.instance.CreateEvent(analyticsEventPrefix + "_InvalidGedChecksum").AddProperty(analyticsPropertyPrefix + "Checksum", extractedChecksum).Send();
			extractedChecksum = null;
			return false;
		}
		return true;
	}

	public void ShowNativeIDFAPopup(int position)
	{
		Helpers.ExecuteCommand(new SendIDFAMetricCommand("show", position, isNativePopup: true));
		PlayerPrefs.SetInt("IDFAPopupAnswer", 1);
		PlayerPrefs.Save();
	}

	private void OfflineLogin()
	{
		CreateLocalData();
		DebugTWD.Log("OnLoadGed begin", DebugType.SignalR);

		OnLoadGed("mock", "");
		long result = GetTime();
		string text = TWDPlayerPrefs.GetString("LocalCreateTime");
		if (!string.IsNullOrEmpty(text))
		{
			long.TryParse(text, out result);
		}
		long time = (GetTime() - result) * 1000;
		OnLoadPlayer(time, "", "");
		LoginResponse loginResponse = new LoginResponse();
		loginResponse.Identification = SystemInfo.deviceUniqueIdentifier;
		OnLogin(jsonSerializer.Serialize(loginResponse));
	}

	public void LoadGame(bool skipDefaultContentLoading = false)
	{
		if (skipDefaultContentLoading)
		{
			defaultContentLoaded = true;
		}
		Helpers.StartCoroutine(this, DownloadAssets(), ref downloadAssetBundleCoroutine);
	}

	private IEnumerator DownloadAssets()
	{
		if (!SingularityMonoBehaviour<AssetBundleController>.Instance.AssetBundlesInitializedAndLoaded)
		{
			StartCoroutine(SingularityMonoBehaviour<AssetBundleController>.Instance.DownloadAssets());
			while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles || !SingularityMonoBehaviour<AssetBundleController>.Instance.AssetBundlesInitializedAndLoaded)
			{
				yield return null;
			}
		}
		if (!SingularityMonoBehaviour<AssetBundleController>.Instance.AdditiveSceneLoaded)
		{
			AddInitializationOp(AssetBundleManager.Instance.LoadSceneAsync("PhotoBooth", LoadSceneMode.Additive));
			SingularityMonoBehaviour<AssetBundleController>.Instance.AdditiveSceneLoaded = true;
		}
		downloadAssetBundleCoroutine = null;
		if (!OfflineManager.IsCustomLogin)
		{
			Helpers.StartCoroutine(this, Login(), ref loginCoroutine);
		}
	}

	public void CreateInstallationId()
	{
		string value = TWDPlayerPrefs.GetString("InstallationId");
		if (string.IsNullOrEmpty(value))
		{
			value = Guid.NewGuid().ToString();
			TWDPlayerPrefs.SetString("InstallationId", value);
			TWDPlayerPrefs.Save();
		}
	}

	private GameEconomyData ProcessReceivedGED(string gameEconomyDataJson)
	{
		Stopwatch stopwatch = new Stopwatch();
		Stopwatch stopwatch2 = new Stopwatch();
		Stopwatch stopwatch3 = new Stopwatch();
		if (0 == 0)
		{
			return jsonSerializer.Deserialize<GameEconomyData>(gameEconomyDataJson);
		}
		stopwatch.Start();
		GameEconomyData gameEconomyData = jsonSerializer.Deserialize<GameEconomyData>(gameEconomyDataJson);
		stopwatch.Stop();
		stopwatch2.Start();
		string text = JsonUtility.ToJson(gameEconomyData);
		stopwatch2.Stop();
		stopwatch3.Start();
		GameEconomyData gameEconomyData2 = JsonUtility.FromJson<GameEconomyData>(text);
		stopwatch3.Stop();
		File.WriteAllText(Application.dataPath + "/Debug/_Original.txt", gameEconomyDataJson);
		File.WriteAllText(Application.dataPath + "/Debug/_Reserialized.txt", text);
		return gameEconomyData2;
	}

	private bool CanReuseStartedGameEconomyData(string checksum)
	{
		if (gameEconomyData != null && gameEconomyData.Started && !string.IsNullOrEmpty(checksum) && !string.IsNullOrEmpty(lastGedContentChecksum))
		{
			return lastGedContentChecksum == checksum;
		}
		return false;
	}

	private IEnumerator ReuseStartedGameEconomyData()
	{
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		Startup.LogStartupEvent("OnGEDReceived");
		Startup.LogStartupEvent("OnGEDDeserialized");
		AnalyticsManager.instance.BlackListString = gameEconomyData.ConfigData.ClientAnalyticsBlackList;
		yield return null;
		Startup.LogStartupEvent("OnGEDStarted");
		TWDPlayerPrefs.SetString("ContentBaseUrl", CdnUrlHelper.RewriteCdnUrl(gameEconomyData.ConfigData.CDNBaseUrl));
		SetVersionValidUntil();
		StartCoroutine(LoadDefaultContentPackAndPlayer());
		SetupLocalization();
		if (!gameEconomyData.GetFeature("HeartbeatCommand").Enabled)
		{
			timeBetweenheartBeatCommands = -1L;
		}
		waitingGed = false;
		loadGEDCoroutine = null;
	}

	private void OnGameEconomyData(string gameEconomyDataJson)
	{
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		if (string.IsNullOrEmpty(gameEconomyDataJson))
		{
			if (!loginAborted)
			{
				Debug.LogWarning("OnGameEconomyData null");
				AnalyticsManager.instance.CreateEvent("GameLoad_GameEconomyData_DownloadError").Send();
				SingularityMonoBehaviour<LoadingScreenHUD>.Instance.HandleNetworkError();
				loginAborted = true;
			}
		}
		else
		{
			Startup.LogStartupEvent("OnGEDReceived");
			Helpers.StartCoroutine(this, StartGameEconomyData(gameEconomyDataJson), ref loadGEDCoroutine);
		}
	}

	private IEnumerator StartGameEconomyData(string gameEconomyDataJson)
	{
		yield return null;
		if (gameEconomyDataJson.Substring(0, 100).IndexOf("\"Version\":2") != -1)
		{
			gameEconomyData = JsonUtility.FromJson<GameEconomyData>(gameEconomyDataJson);
		}
		else
		{
			gameEconomyData = jsonSerializer.Deserialize<GameEconomyData>(gameEconomyDataJson);
		}
		Startup.LogStartupEvent("OnGEDDeserialized");
		AnalyticsManager.instance.BlackListString = gameEconomyData.ConfigData.ClientAnalyticsBlackList;
		yield return null;
		gameEconomyData.Start();
		Startup.LogStartupEvent("OnGEDStarted");
		TWDPlayerPrefs.SetString("ContentBaseUrl", CdnUrlHelper.RewriteCdnUrl(gameEconomyData.ConfigData.CDNBaseUrl));
		SetVersionValidUntil();
		StartCoroutine(LoadDefaultContentPackAndPlayer());
		SetupLocalization();
		if (!gameEconomyData.GetFeature("HeartbeatCommand").Enabled)
		{
			timeBetweenheartBeatCommands = -1L;
		}
		if (!string.IsNullOrEmpty(pendingGedContentChecksum))
		{
			lastGedContentChecksum = pendingGedContentChecksum;
		}
		waitingGed = false;
		loadGEDCoroutine = null;
	}

	private void SetVersionValidUntil()
	{
		if (!string.IsNullOrEmpty(gameEconomyData.ConfigData.VersionValidUntil))
		{
			VersionValidUntil = GameEconomyData.ParseDateTime(gameEconomyData.ConfigData.VersionValidUntil);
			VersionUpgradeNeeded = true;
		}
	}

	public void SetupLocalization()
	{
		bool flag = !IsConnectedToServer || GameConfiguration.Instance.Config.UseOnlyLocalLocalizations || gameEconomyData.ConfigData.UseOnlyLocalLocalizationFiles;
		if (!flag)
		{
			SingularityMonoBehaviour<LocalizationManager>.Instance.UseOnlyLocalFiles = flag;
			SingularityMonoBehaviour<LocalizationManager>.Instance.Load(SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage, forceUpdate: true);
		}
	}

	private IEnumerator PreloadPrefabs()
	{
		Startup.LogStartupEvent("PreloadStarted");
		string text = TWDPlayerPrefs.GetString("PreloadPrefabs");
		if (!string.IsNullOrEmpty(text))
		{
			string[] prefabs = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < prefabs.Length; i++)
			{
				if (i % 4 == 0)
				{
					yield return null;
				}
				UnityUtils.PreloadAsset(prefabs[i], "scriptableobjects");
			}
		}
		UnityUtils.PreloadAsset("InhabitantsResources", "scriptableobjects");
		yield return null;
		UnityUtils.PreloadAsset("ActorsResources", "scriptableobjects");
		UnityUtils.PreloadAsset("CharacterTemplate", "scriptableobjects");
		Startup.LogStartupEvent("PreloadFinished");
		waitingPreload = false;
	}

	private IEnumerator LoadDefaultContentPackAndPlayer()
	{
		if (!defaultContentLoaded)
		{
			LoadPersistentElementsScene();
			yield return null;
			defaultContentLoaded = true;
		}
	}

	public long GetTime()
	{
		return DateTime.Now.Ticks / 10000000;
	}

	private void StartModelManager()
	{
		foreach (SurvivorModel survivor in playerModel.SurvivorContainer.Survivors)
		{
			foreach (EquipmentItemModel model in survivor.EquipmentItems.Models)
			{
				model.Owner = survivor;
			}
		}
		modelManager.SetGameEconomyData(gameEconomyData);
		modelManager.ModelStateCheckEnabled = gameEconomyData.ConfigData.EnableCheckPlayerModelCommand;
		modelManager.StartModel(loginTime, BuildConfiguration.Active.Branch.Equals("develop"));
		if (playerModel == null)
		{
			AnalyticsManager.instance.CreateEvent("GameLoad_PlayerModel_StartError").Send();
			ShowLoadError();
			return;
		}
		Startup.LogStartupEvent("PlayerStarted");
		if (playerModel.HashedId != TWDPlayerPrefs.GetString("HashedId"))
		{
			TWDPlayerPrefs.SetString("HashedId", playerModel.HashedId);
		}
		modelManager.ActionExecuted += OnActionExecuted;
	}

	private void LoginCompleted()
	{
		if (SingularityMonoBehaviour<SDKManager>.Instance != null)
		{
			SingularityMonoBehaviour<SDKManager>.Instance.InitializeExternalSdks();
		}
		if (GuildManager != null)
		{
			GuildManager.Uninitialize();
			GuildManager = null;
		}
		GuildManager = new GuildManager(modelManager);
		if (SingularityMonoBehaviour<GuildWarManager>.Instance != null)
		{
			SingularityMonoBehaviour<GuildWarManager>.Instance.SubscribeToEvents();
		}
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		if (IAPManager == null)
		{
			IAPManager = base.gameObject.GetComponent<IAPManager>();
			if (IAPManager == null)
			{
				IAPManager = base.gameObject.AddComponent<IAPManager>();
			}
		}
		if (gameEconomyData != null && !IsLoadDataManager)
		{
			IAPManager.PopulateProductList(gameEconomyData.GetInAppPurchaseProductIdList());
		}
		if (GameCenterManager == null)
		{
			GameCenterManager = new GameCenterManager();
		}
		if (FriendListManager == null)
		{
			FriendListManager = new FriendListManager(GameCenterManager);
		}
		if (SingularityMonoBehaviour<VideoAdManager>.Instance != null)
		{
			SingularityMonoBehaviour<VideoAdManager>.Instance.OnVideoClose += OnVideoWatched;
		}
		if (BannerManager == null && GameConfiguration.Instance.Config.OnlineLevel != BuildGameConfiguration.OnlineLevelType.Offline)
		{
			BannerManager = new BannerManager();
			BannerManager.UpdateBannerInfo();
		}
		if (PlayerHubManager == null)
		{
			PlayerHubManager = new PlayerHubManager();
		}
		if (PlayerHubManager != null)
		{
			PlayerHubManager.UpdateInfo();
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.SubsrcibeToPlayerModel();
		}
		Fabric.EventManager.Instance.PostEvent("volume/sound_effects", EventAction.SetVolume, Instance.Settings.SoundFxVolume);
		Fabric.EventManager.Instance.PostEvent("volume/music", EventAction.SetVolume, Instance.Settings.MusicVolume);
		string countryCode = GetCountryCode();
		if (playerModel.Country != countryCode && !string.IsNullOrEmpty(countryCode))
		{
			Helpers.ExecuteCommand(new SetPlayerCountryCommand
			{
				Country = countryCode,
				OldCountry = playerModel.Country
			});
		}
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		playerModel.Changed -= OnPlayerChange;
		playerModel.Changed += OnPlayerChange;
		IsGameStarted = true;
		if (DevFastTrackLoad == DevFastTrackType.Camp || DevFastTrackLoad == DevFastTrackType.Map)
		{
			SetState(GameState.Camp);
		}
		if (TutorialView.Instance != null)
		{
			TutorialView.Instance.Initialize(playerModel.Tutorial);
		}
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		if (DevFastTrackLoad != DevFastTrackType.None || TutorialView.Instance == null || TutorialView.Instance.StartupSetting != TutorialView.StartupSettingType.Normal || !TutorialView.Instance.IsInInitialPart)
		{
			if (DevFastTrackLoad == DevFastTrackType.Combat)
			{
				LoadVisitModel(VisitMode.PVE, default(MapMissionParameters));
			}
			else if (DevFastTrackLoad == DevFastTrackType.Map)
			{
				InitializePreExistingViews();
			}
			else
			{
				EnableAssetLoaderUI(enable: true);
				Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
			}
		}
		else if (TutorialView.Instance.IsInInitialPart)
		{
			TutorialView.Instance.InitializeForCombat();
		}
		else if (!TutorialView.Instance.StartPart("InitialCombat"))
		{
			Debug.LogError("Could not start initial tutorial 'InitialCombat'!");
		}
		LoadAdsIds();
		SingularityMonoBehaviour<HUDManager>.Instance.Reset();
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		Helpers.ExecuteCommand(new ReseedRandomCommand());
		SingularityMonoBehaviour<SDKManager>.Instance.OnServiceTokensSet += delegate(IDictionary<string, string> tokens)
		{
			AnalyticsEvent analyticsEvent = AnalyticsManager.instance.CreateEvent("PlayerTokens").AddProperty("DeviceToken", getDeviceId());
			foreach (KeyValuePair<string, string> token in tokens)
			{
				analyticsEvent.AddProperty(token.Key, token.Value);
			}
			analyticsEvent.Send();
		};
		SingularityMonoBehaviour<SDKManager>.Instance.OnPlayerLoaded();
		if (string.IsNullOrEmpty(playerModel.Language))
		{
			SetPlayerPickedLanguage(SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage);
		}
		RequestPltv();
		SetPlayerPickedFrameRate(TWDPlayerPrefs.GetInt("PlayerSelectedFrameRate", 1));
		if (Instance.Settings.VSync)
		{
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
		if (!playerModel.Blackboard.ToggleValues.ContainsKey("Toggle.ToggleCombatGridEnabled"))
		{
			if (gameEconomyData.ConfigData.CombatGridStateByDefault)
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("Toggle.ToggleCombatGridEnabled"));
			}
			else
			{
				Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("Toggle.ToggleCombatGridEnabled"));
			}
		}
		if (gameEconomyData != null && TutorialView.Instance.IsInInitialPart)
		{
			PortraitManager.Instance.PreRenderInitialSurvivors();
		}
		Helpers.SetConditionOpened(on: false);

		modelManager.OnCommanErrorResult -= Show_Command_Error;
		modelManager.OnCommanErrorResult += Show_Command_Error;
	}

	private void LoadAdsIds()
	{
		if (!IsConnectedToServer)
		{
			LoadMediationData();
		}
		else
		{
			ContentManager.Instance.LoadContent(typeof(UnityAdsIds).Name, OnAdsIdsLoaded);
		}
	}

	private void OnAdsIdsLoaded(string transactionId, bool successfull)
	{
		if (successfull)
		{
			string content = modelManager.ContentService.GetContent(transactionId);
			List<UnityAdsIds> list = modelManager.GetMessageSerializer().DeserializeObject<List<UnityAdsIds>>(content);
			if (list.Count > 0)
			{
				UnityAdsIds = list[0];
			}
		}
		LoadMediationData();
	}

	private void LoadMediationData()
	{
		if (OfflineManager.IsUseServices && IsConnectedToServer)
		{
			Startup.LogStartupEvent("RequestMediationData");
			ContentManager.Instance.LoadContent(typeof(MediationData).Name, OnMediationContent);
		}
	}

	private void OnMediationContent(string transactionId, bool successful)
	{
		if (successful)
		{
			SingularityMonoBehaviour<VideoAdManager>.Instance.Init();
			Helpers.ExecuteCommand(new ApplyMediationDataCommand(transactionId));
		}
	}

	public void SetState(GameState state)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.OnStateChange(state, State);
		if (state != GameState.None)
		{
			SingularityMonoBehaviour<ObjectPoolManager>.Instance.DestroyAllObjects();
			UIDrawCall.ReleaseInactive();
			Helpers.ClearUnusedMemory();
		}
		State = state;
	}

	private void CreateVisualizationTaskMaps()
	{
		visualizationTaskMap = new Dictionary<Type, Type>();
		Type[] exportedTypes = Assembly.GetExecutingAssembly().GetExportedTypes();
		foreach (KeyValuePair<Type, Type> item in new Dictionary<Type, Type> { [typeof(VisualizationTask)] = typeof(ModelAction) })
		{
			Type[] array = exportedTypes;
			foreach (Type type in array)
			{
				if (!type.IsSubclassOf(item.Key))
				{
					continue;
				}
				Type type2 = null;
				ConstructorInfo[] constructors = type.GetConstructors();
				for (int j = 0; j < constructors.Length; j++)
				{
					ParameterInfo[] parameters = constructors[j].GetParameters();
					foreach (ParameterInfo parameterInfo in parameters)
					{
						if (parameterInfo.ParameterType.IsSubclassOf(item.Value))
						{
							type2 = parameterInfo.ParameterType;
						}
					}
				}
				if (type2 != null)
				{
					visualizationTaskMap.Add(type2, type);
				}
			}
		}
	}

	public void SetPlayerPickedLanguage(string languageID)
	{
		if (SingularityMonoBehaviour<LocalizationManager>.Instance != null && SingularityMonoBehaviour<LocalizationManager>.Instance.SupportedLanguages.Contains(languageID))
		{
			SingularityMonoBehaviour<LocalizationManager>.Instance.Load(languageID);
			TWDPlayerPrefs.SetString("PlayerSelectedLanguage", languageID);
			Helpers.ExecuteCommand(new SetPlayerLanguageCommand
			{
				Language = languageID
			});
			SingularityMonoBehaviour<SDKManager>.Instance.OnLanguageChanged(languageID);
		}
	}

	public void SetPlayerPickedDisplayMode(int displayModeKeyIndex)
	{
		int num = TWDPlayerPrefs.GetInt("PlayerSelectedScreenResolution");
		Screen.SetResolution((num == 0) ? Display.main.systemWidth : ScreenResolutionWidthArray[num], (num == 0) ? Display.main.systemHeight : ScreenResolutionHeightArray[num], (displayModeKeyIndex == 0) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
		TWDPlayerPrefs.SetInt("PlayerSelectedDisplayMode", displayModeKeyIndex);
	}

	public void SetPlayerPickedScreenResolution(int screenResolutionKeyIndex)
	{
		int num = TWDPlayerPrefs.GetInt("PlayerSelectedDisplayMode");
		Screen.SetResolution((screenResolutionKeyIndex == 0) ? Display.main.systemWidth : ScreenResolutionWidthArray[screenResolutionKeyIndex], (screenResolutionKeyIndex == 0) ? Display.main.systemHeight : ScreenResolutionHeightArray[screenResolutionKeyIndex], (num == 0) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
		TWDPlayerPrefs.SetInt("PlayerSelectedScreenResolution", screenResolutionKeyIndex);
	}

	public void SetPlayerPickedFrameRate(int frameRateIndex)
	{
		switch (frameRateIndex)
		{
		case 0:
			Application.targetFrameRate = 30;
			break;
		case 1:
			Application.targetFrameRate = 60;
			break;
		case 2:
			Application.targetFrameRate = 90;
			break;
		case 3:
			Application.targetFrameRate = 120;
			break;
		}
		TWDPlayerPrefs.SetInt("PlayerSelectedFrameRate", frameRateIndex);
	}

	public Type GetEffectVisualizationType(Type effectType)
	{
		if (visualizationTaskMap.ContainsKey(effectType))
		{
			return visualizationTaskMap[effectType];
		}
		return null;
	}

	private void OnActionExecuted(ModelAction action)
	{
		DebugTWD.Log("OnActionExecuted " + action.GetType().ToString(), DebugType.Action);

		if (VisualizationQueue.Instance == null || !visualizationTaskMap.ContainsKey(action.GetType()))
		{
			return;
		}
		VisualizationTask visualizationTask = Activator.CreateInstance(visualizationTaskMap[action.GetType()], action) as VisualizationTask;
		if (visualizationTask is ActorVisualizationTask)
		{
			for (int i = 0; i < environmentalActorsIgnoreList.Count; i++)
			{
				if (visualizationTask.GetType() == environmentalActorsIgnoreList[i])
				{
					ActorVisualizationTask actorVisualizationTask = visualizationTask as DamageVisualizationTask;
					if (actorVisualizationTask != null && actorVisualizationTask.Actor != null && actorVisualizationTask.Actor.IsEnvironmental)
					{
						return;
					}
				}
			}
		}
		if (visualizationTask != null)
		{
			VisualizationQueue.Instance.Add(visualizationTask);
		}
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer && changed != "shield")
		{
			savePending = true;
		}
		switch (changed)
		{
		case "CombatModelDeleted":
			Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
			break;
		case "NewMap":
			Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
			break;
		case "SetAchievement":
		{
			string achievementId = args as string;
			GameCenterManager.ReportProgress(achievementId, 1, 1);
			break;
		}
		case "level":
			RequestPltv();
			break;
		}
	}

	public void Backup()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.DeleteAll();
		TooltipManager.DestroyAllAndClear();
		SingularityMonoBehaviour<ObjectPoolManager>.Instance.DestroyAllObjects();
		UIDrawCall.ReleaseInactive();
		Helpers.ClearUnusedMemory();
		Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
	}

	private void CreateLocalData()
	{
		if (TWDPlayerPrefs.GetString("SaveVersion") != OfflineManager.ShortVersion)
		{
			Debug.LogWarning("Save version is different from current version");
		}
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			gameEconomyData = DataManager.Instance.GameData;
			gameEconomyData.Start();
			modelManager = DataManager.Instance.ModelManager;
			DebugTWD.Log(playerModel.Name);
			TWDPlayerPrefs.SetString("LocalCreateTime", (GetTime() - playerModel.LifeTime / 1000).ToString());
			return;
		}
		MockData component = GetComponent<MockData>();
		string gameEconomyJSON = component.GetGameEconomyJSON();
		ContentManager.Instance.GetCache("GED").SetContent("GameEconomyData", "mock", null, gameEconomyJSON);
		ContentCache cache = ContentManager.Instance.GetCache("Player");
		if (cache.GetContentById<string>("PlayerModel") == null)
		{
			gameEconomyData = jsonSerializer.Deserialize<GameEconomyData>(gameEconomyJSON);
			gameEconomyData.Start();
			string text = component.GetDemoPlayerJSON();
			if (text == null)
			{
				TWDModelManager tWDModelManager = new TWDModelManager(Debug.isDebugBuild);
				tWDModelManager.SetModelManagerMode(ModelManagerMode.Client);
				tWDModelManager.SetModelDebug(new UnityModelDebug());
				tWDModelManager.SetModelAnalytics(null);
				tWDModelManager.CreateModel();
				tWDModelManager.Player.Created = DateTime.UtcNow;
				tWDModelManager.SetGameEconomyData(gameEconomyData);
				text = tWDModelManager.SerializeModel();
			}
			PlayerModel playerModel = jsonSerializer.Deserialize<PlayerModel>(text);
			cache.SetContent("PlayerModel", null, null, text);
			TWDPlayerPrefs.SetString("LocalCreateTime", (GetTime() - playerModel.LifeTime / 1000).ToString());
		}
	}

	private void InitializeRunScene()
	{
		_ = modelManager.Player.Combat;
	}

	public void LoadVisitModel(VisitMode visitMode, MapMissionParameters missionInfo, string sceneToUnload = null)
	{
		if (DevFastTrackLoad == DevFastTrackType.None)
		{
			if (visitMode == VisitMode.PVP && CampView.Instance != null)
			{
				CampView.Instance.SetEnabled(enabled: false);
			}
			CurrentlyLoading = true;
			TransitionScreenHUD transitionScreenHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Transition) as TransitionScreenHUD;
			if (IsLoadDataManager || TutorialView.Instance.Model.HasCompletedPart("InitialCombat"))
			{
				if (missionInfo.IsSurvival)
				{
					bool flag = false;
					if (modelManager != null && modelManager.Player != null && modelManager.Player.WeeklySurvival != null && modelManager.Player.WeeklySurvival.CurrentDifficulty > SurvivalDifficulty.Normal)
					{
						flag = true;
					}
					transitionScreenHUD.SceneToLoadAfterInAnimation = (flag ? "LoadingScreenCombat_Distance" : "LoadingScreenCombat_Distance_Normal_Mode");
				}
				else if (missionInfo.GuildBattleState == GuildBattleMapMissionModel.MissionState.PVE)
				{
					transitionScreenHUD.SceneToLoadAfterInAnimation = "LoadingScreenCombat_GVG_Normal_Mission";
				}
				else if (missionInfo.GuildBattleState == GuildBattleMapMissionModel.MissionState.PVP)
				{
					DebugTWD.LogWarning("Exception for LoadVisitModel scene", DebugType.Wars);

					transitionScreenHUD.SceneToLoadAfterInAnimation = "LoadingScreenCombat_GVG";
				}
				else if (visitMode == VisitMode.PVP)
				{
					transitionScreenHUD.SceneToLoadAfterInAnimation = "LoadingScreenCombat_Outpost";
				}
				else
				{
					transitionScreenHUD.SceneToLoadAfterInAnimation = "LoadingScreenCombat_Mission";
				}
			}
			if (sceneToUnload != null)
			{
				transitionScreenHUD.SceneToUnload = sceneToUnload;
			}
			transitionScreenHUD.AnimationInCallback = delegate
			{
				OnTransitionFinished(visitMode, missionInfo);
			};
			transitionScreenHUD.Open();
			Instance.SetState(GameState.Transition);
		}
		else
		{
			OnTransitionFinished(visitMode, missionInfo);
		}
	}

	public void DebugLoadVisitModel(string missionId)
	{
		loadVisitParams = new LoadVisitParams();
		loadVisitParams.VisitMode = VisitMode.PVE;
		Helpers.ExecuteCommand(new SetSelectedMissionCommand(new MapMissionParameters
		{
			MissionId = missionId,
			MissionLevel = modelManager.Player.Level,
			MissionFlavor = "Default"
		})
		{
			ShuffleLoot = true
		});
		StartCoroutine(FakeVisitModel());
	}

	public void LoadVisitModel(string missionId)
	{
		MapMissionParameters missionInfo = new MapMissionParameters
		{
			MissionId = missionId,
			MissionLevel = 1
		};
		LoadVisitModel(VisitMode.PVE, missionInfo);
	}

	private void OnTransitionFinished(VisitMode visitMode, MapMissionParameters missionParameters)
	{
		loadVisitParams = new LoadVisitParams();
		loadVisitParams.VisitMode = visitMode;
		if (DevFastTrackLoad == DevFastTrackType.None)
		{
			MissionData missionData = gameEconomyData.GetMissionData(missionParameters.MissionId);
			loadVisitParams.Parameters = missionParameters.MissionId;
			Helpers.ExecuteCommand(new SetSelectedMissionCommand(missionParameters));
			if (visitMode != VisitMode.PVE)
			{
			}
		}
		if (!IsConnectedToServer)
		{
			if (IsLoadDataManager)
			{
				if (string.IsNullOrEmpty(CustomMissionContent))
				{
					MissionData selectedMissionData = modelManager.SelectedMissionData;
					if (selectedMissionData != null && selectedMissionData.RunLocationName != null)
					{
						TextAsset textAsset = UnityUtils.LoadAsset<TextAsset>("run_locations/" + selectedMissionData.RunLocationName);
						if (textAsset == null)
						{
							DebugTWD.LogError("Failed to load run location for " + selectedMissionData.RunLocationName);
							CustomMissionContent = string.Empty;
						}
						else
						{
							CustomMissionContent = textAsset.text;
							modelManager.ApplyRunLocation(runLocation: jsonSerializer.DeserializeObject<RunLocationModel>(CustomMissionContent), visitMode: loadVisitParams.VisitMode, defendingPlayer: null);
							Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
						}
						return;
					}
				}
				else
				{
					modelManager.ApplyRunLocation(runLocation: jsonSerializer.DeserializeObject<RunLocationModel>(CustomMissionContent), visitMode: loadVisitParams.VisitMode, defendingPlayer: null);
					Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
					return;
				}
				if (PlayerPrefs.HasKey(UserPrefsKeys.Key_RunLocationID))
				{
					CustomMissionContent = ContentManager.Instance.GetContent(PlayerPrefs.GetString(UserPrefsKeys.Key_RunLocationID));
					modelManager.ApplyRunLocation(runLocation: jsonSerializer.DeserializeObject<RunLocationModel>(CustomMissionContent), visitMode: loadVisitParams.VisitMode, defendingPlayer: null);
					return;
				}
			}
			else
			{
				StartCoroutine(FakeVisitModel());
				return;
			}
		}
		if (loadVisitParams.VisitMode == VisitMode.PVE || loadVisitParams.VisitMode == VisitMode.ScoutPVE)
		{
			ContentManager.Instance.LoadContent("RunLocation/" + loadVisitParams.Parameters, OnRunLocationContent, 1);
			return;
		}
		modelManager.SetPaused(paused: true);
		SignalRClient.Instance.RequestCommand("loadVisit", jsonSerializer.Serialize(loadVisitParams), OnVisitModel, waitForResponse: true);
	}

	private void OnRunLocationContent(string transactionId, bool loaded)
	{
		if (!loaded)
		{
			if (OfflineManager.IsIgnoreReconnect)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsIgnoreReconnect) return");
				DebugTWD.LogError("Ignore ReloadGame, OnRunLocationContent, transactionId: " + transactionId, DebugType.System);
				return;
			}
			LogReloadEvent("Reason: GameManager.OnRunLocationContent, not loaded: " + transactionId);
			ReloadGame();
		}
		else
		{
			if (!IsLoadDataManager && !OfflineManager.IsPrivateMode)
			{
				if (Helpers.ExecuteCommand(new ApplyRunLocationCommand(transactionId, loadVisitParams.VisitMode)) != TWDModelResult.OK && !IsOfflineMode)
				{
					LogReloadEvent("Reason: GameManager.OnRunLocationContent, result error: " + transactionId);
					ReloadGame();
				}
				else if (loadVisitParams.VisitMode == VisitMode.ScoutPVE)
				{
					EventManager.NotifyEvent(EventManager.EventType.OutpostTemplateLoaded);
				}
				else
				{
					Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
				}
			}
			else
			{
				DebugTWD.LogMycode("if (!IsLoadDataManager && !OfflineManager.IsPrivateMode)");
				string content = ContentManager.Instance.GetContent(transactionId);

				if (!string.IsNullOrEmpty(content))
				{
					DebugTWD.Log("Проверить и сравнить 2: " + content, DebugType.Load);
					PlayerPrefs.SetString(UserPrefsKeys.Key_RunLocationID, transactionId);
					modelManager.ApplyRunLocation(runLocation: jsonSerializer.DeserializeObject<List<RunLocationModel>>(content)[0], visitMode: loadVisitParams.VisitMode, defendingPlayer: null);
				}
				else
				{
					MissionData selectedMissionData = modelManager.SelectedMissionData;
					if (selectedMissionData != null && selectedMissionData.RunLocationName != null)
					{
						TextAsset textAsset = UnityUtils.LoadAsset<TextAsset>("run_locations/" + selectedMissionData.RunLocationName);
						if (textAsset == null)
						{
							Debug.LogError("Failed to load run location for " + selectedMissionData.RunLocationName);
							CustomMissionContent = string.Empty;
							return;
						}
						else
						{
							CustomMissionContent = textAsset.text;
						}

						modelManager.ApplyRunLocation(runLocation: jsonSerializer.DeserializeObject<RunLocationModel>(CustomMissionContent), visitMode: loadVisitParams.VisitMode, defendingPlayer: null);
					}
					else
					{
						DebugTWD.Log("selectedMissionData is null", DebugType.Load);
					}
				}
			}
		}
	}

	private IEnumerator FakeVisitModel()
	{
		if (loadVisitParams.VisitMode == VisitMode.PVE || loadVisitParams.VisitMode == VisitMode.ScoutPVE)
		{
			string content = GetComponent<MockData>().GetRunJSON();
			MissionData selectedMissionData = modelManager.SelectedMissionData;
			if (selectedMissionData != null && selectedMissionData.RunLocationName != null)
			{
				TextAsset textAsset = UnityUtils.LoadAsset<TextAsset>("run_locations/" + selectedMissionData.RunLocationName);
				if (textAsset == null)
				{
					Debug.LogError("Failed to load run location for " + selectedMissionData.RunLocationName);
					CustomMissionContent = string.Empty;
				}
				else
				{
					content = textAsset.text;
					CustomMissionContent = content;
				}
			}
			ContentManager.Instance.FakeContent(content, OnRunLocationContent);
		}
		else
		{
			string content = ((loadVisitParams.VisitMode != VisitMode.PVP) ? ContentManager.Instance.GetCache("Player").GetContentById<string>("PlayerModel") : modelManager.SerializeModel());
			ModelRespond modelRespond = new ModelRespond();
			modelRespond.ModelJson = content;
			OnVisitModel(jsonSerializer.Serialize(modelRespond));
		}
		yield break;
	}

	private void OnVisitModel(string modelRespondJson)
	{
		if (string.IsNullOrEmpty(modelRespondJson))
		{
			modelManager.SetPaused(paused: false);
			CurrentlyLoading = false;
			if (loadVisitParams.VisitMode == VisitMode.ScoutPVE)
			{
				EventManager.NotifyEvent(EventManager.EventType.OutpostTemplateLoadFailed);
			}
		}
		else
		{
			ModelRespond modelRespond = jsonSerializer.Deserialize<ModelRespond>(modelRespondJson);
			LoadVisit(modelRespond.ModelJson, modelRespond.Time);
		}
	}

	private void LoadVisit(string modelJson, long visitTime)
	{
		bool flag;
		try
		{
			modelManager.LoadVisitModel(modelJson, visitTime, loadVisitParams.VisitMode);
			flag = true;
		}
		catch (Exception ex)
		{
			DebugTWD.LogError("Ошибка : " + ex.Message, DebugType.Error);
			flag = false;
		}
		modelManager.SetPaused(paused: false);
		if (flag && loadVisitParams.VisitMode == VisitMode.PVP)
		{
			Helpers.ExecuteCommand(new StartOutpostAttackCommand());
			if (IsConnectedToServer)
			{
				SignalRClient.Instance.RequestCommand("UnLockPlayer", modelManager.Player.Combat.OutpostCombat.DefenderHashedId, delegate
				{
				}, waitForResponse: true);
			}
		}
		if (loadVisitParams.VisitMode == VisitMode.PVP)
		{
			LoadingScreenCombat loadingScreenCombat = UnityEngine.Object.FindObjectOfType<LoadingScreenCombat>();
			if (loadingScreenCombat != null)
			{
				loadingScreenCombat.ShowDefenders();
			}
		}
		if (loadVisitParams.VisitMode == VisitMode.ScoutPVE)
		{
			EventManager.NotifyEvent(EventManager.EventType.OutpostTemplateLoaded);
		}
		else
		{
			Helpers.StartCoroutine(this, LoadScene(), ref loadSceneCoroutine);
		}
	}

	private IEnumerator LoadScene()
	{
		if (SingularityMonoBehaviour<LocalizationManager>.Instance.ShouldWaitForLocalizations())
		{
			Startup.LogStartupEvent("LocalizationWaitStart");
			Stopwatch localizationLoadTimer = new Stopwatch();
			localizationLoadTimer.Start();
			yield return StartCoroutine(SingularityMonoBehaviour<LocalizationManager>.Instance.WaitForLocalizations());
			localizationLoadTimer.Stop();
			Startup.LogStartupEvent("LocalizationWaitEnd");
		}
		Startup.LogStartupEvent("SceneLoadStarting");
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		LoadingScreenHUD.SetLoadingMessage(LoadingMessageType.LoadScene);
		yield return null;
		string baseLevelName = IsLoadDataManager ? OfflineManager.MainSceneName : "camp";
		string scenarioName = null;
		if (playerModel.Combat != null)
		{
			DebugTWD.Log("Load SceneBundle And Dependencies: " + scenarioName, DebugType.Load);
			baseLevelName = playerModel.Combat.BackgroundSceneName;
			scenarioName = playerModel.Combat.SceneName;
			SingularityMonoBehaviour<AssetBundleController>.Instance.LoadSceneBundleAndDependencies(scenarioName);
		}
		else
		{
			SingularityMonoBehaviour<AssetBundleController>.Instance.LoadCampOnlyAssetBundles();
		}
		while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles)
		{
			yield return null;
		}
		EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
		if (DevFastTrackLoad == DevFastTrackType.None)
		{
			DebugTWD.Log("Load Scene Base: " + baseLevelName, DebugType.Load);
			LightmapSettings.lightmaps = new LightmapData[0];
			AssetBundleManager.Instance.LoadScene(baseLevelName);
			EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
			yield return null;
			Startup.LogStartupEvent("SceneCampLoaded");
			RenderSettings.ambientIntensity = 0f;
			if (scenarioName != null)
			{
				DebugTWD.Log("Load Scene Scenario: " + scenarioName, DebugType.Load);
				LightmapData[] backgroundLightMaps = LightmapSettings.lightmaps;
				LightProbes backgroundLightProbes = LightmapSettings.lightProbes;
				LightmapsMode backgroundLightMapsMode = LightmapSettings.lightmapsMode;
				AssetBundleManager.Instance.LoadScene(scenarioName, LoadSceneMode.Additive);
				EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
				yield return null;
				LightmapSettings.lightmaps = backgroundLightMaps;
				LightmapSettings.lightProbes = backgroundLightProbes;
				LightmapSettings.lightmapsMode = backgroundLightMapsMode;
			}
		}
		AssetBundleManager.Instance.LoadScene("RewardScreen_Nineboxes", LoadSceneMode.Additive);
		Startup.LogStartupEvent("SceneRewardLoaded");
		if (playerModel.Combat == null)
		{
			Startup.LogStartupEvent("SceneMapLoaded");
			EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
			if (IsLoadDataManager && OfflineManager.Instance.IsReturnToResidence)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager && OfflineManager.Instance.IsReturnToResidence) return");
				OfflineManager.Instance.IsReturnToResidence = false;
				yield break;
			}
			AssetBundleManager.Instance.LoadScene("Camp_Background_" + playerModel.CampMover.BackgroundName, LoadSceneMode.Additive);
			Startup.LogStartupEvent("SceneCampBgLoaded");
			EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
			yield return null;
		}
		else
		{
			MissionData missionData = gameEconomyData.GetMissionData(playerModel.Combat.CurrentMissionId);
			if (missionData == null)
			{
				Debug.LogWarning("NULL missionData for saved mission id " + playerModel.Combat.CurrentMissionId + ", selecting first mission.");
			}
			string text = missionData?.MissionName;
			Scenario scenario = UnityEngine.Object.FindObjectOfType<Scenario>();
			Scenario component = scenario.GetComponent<Scenario>();
			bool flag = false;
			MissionSettings[] componentsInChildren = scenario.GetComponentsInChildren<MissionSettings>(includeInactive: true);
			foreach (MissionSettings missionSettings in componentsInChildren)
			{
				if (missionSettings.name == text || string.IsNullOrEmpty(text))
				{
					missionSettings.gameObject.SetActive(value: true);
					flag = true;
				}
				else
				{
					missionSettings.gameObject.SetActive(value: false);
				}
			}
			if (!flag)
			{
				Debug.LogError("Mission '" + text + "' not found in scenario!");
				yield break;
			}
			if (!string.IsNullOrEmpty(component.ExportHash) && !string.IsNullOrEmpty(playerModel.Combat.RunLocationExportHash) && component.ExportHash != playerModel.Combat.RunLocationExportHash)
			{
				Debug.LogWarning("Scenario version mismatch: Client version " + component.ExportVersion + " Exported version " + playerModel.Combat.RunLocationVersion);
			}
			EventManager.NotifyEvent(EventManager.EventType.LoadingStepComplete);
			loadSceneCoroutine = null;
		}
		WaitForInitializationOps();
		if (TutorialView.Instance.StartupSetting == TutorialView.StartupSettingType.SkipToEarlyGame || TutorialView.Instance.StartupSetting == TutorialView.StartupSettingType.SkipToLateGame || TutorialView.Instance.StartupSetting == TutorialView.StartupSettingType.SkipToMaxCouncil)
		{
			bool cheating = true;
			var definition = Instance.gameEconomyData.OutpostTemplateDefinitions[0];
			DebugTWD.LogWarning("Load LocationModel: " + definition.MissionID, DebugType.Load);

			RunLocationLoader.LoadLocationModel(definition, delegate
			{
				cheating = false;
			}, delegate
			{
				cheating = false;
			});
			while (cheating)
			{
				yield return null;
			}
		}
		Startup.LogStartupEvent("ViewInitStarting");
		InitializePreExistingViews();
		Startup.LogStartupEvent("ViewInitialized");
		NotifyLoadCompleted();
		if (!string.IsNullOrEmpty(scenarioName))
		{
			SingularityMonoBehaviour<AssetBundleController>.Instance.UnloadSceneBundle(scenarioName);
			SingularityMonoBehaviour<AssetBundleController>.Instance.UnloadCampOnlyAssetBundles();
		}
		UnityUtils.ReleasePreloadedAssets();
		EventManager.NotifyEvent(EventManager.EventType.StateTransitionCompleted);
		int value = TWDPlayerPrefs.GetInt("LoadCount") + 1;
		TWDPlayerPrefs.SetInt("LoadCount", value);
		if (playerModel.Combat != null)
		{
			if (LoadingScreenCombat.Active)
			{
				LoadingScreenCombat.HideCombatScene();
			}
			else
			{
				if (CombatView.Instance != null)
				{
					CombatView.Instance.CombatWasResumed = true;
				}
				EventManager.NotifyEvent(EventManager.EventType.CombatStart);
			}
		}
		CurrentlyLoading = false;
		if (videoRewardCommandPending)
		{
			OnVideoWatched(completely: true);
			videoRewardCommandPending = false;
		}
		yield return null;
		if (SingularityMonoBehaviour<LoadingScreenHUD>.Instance != null)
		{
			SingularityMonoBehaviour<LoadingScreenHUD>.Instance.Completed();
		}
		EnableAssetLoaderUI(enable: false);
		if (playerModel.Combat == null)
		{
			Screen.sleepTimeout = -2;
		}

		LoadingScreenHUD.SetLoadingMessage(LoadingMessageType.Default);

		if (OfflineManager.IsFixModelShaders)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsFixModelShaders)");
			FixShadersMesh();
			FixShadersSkinned();
		}

		Startup.LogStartupEvent("SceneLoadFinished");
	}

	private IEnumerator WaitToEnableNotificationsDuringTutorial()
	{
		while (!IsGameStarted || (!modelManager.Player.Tutorial.HasCompletedPart("Tutorial_Training_Ground") && modelManager.Player.Tutorial.CurrentPartId != "Tutorial_Training_Ground"))
		{
			yield return new WaitForSeconds(1f);
		}
		while (!IsGameStarted || (!modelManager.Player.Tutorial.HasCompletedPart("Tutorial_Training_Ground") && modelManager.Player.Tutorial.CurrentPartId == "Tutorial_Training_Ground" && modelManager.Player.Tutorial.CurrentStep <= 10))
		{
			yield return new WaitForSeconds(1f);
		}
		while (CampView.Instance == null || !CampView.Instance.IsShown || SingularityMonoBehaviour<HUDManager>.Instance == null || SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen > 0)
		{
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(1f);
	}

	private bool AllowNotifications()
	{
		if (IsGameStarted && modelManager.Player.Tutorial.HasCompletedPart("Tutorial_Training_Ground"))
		{
			return true;
		}
		return false;
	}

	private void InitializePreExistingViews<T, U>(bool destroyViewWithoutModel = true) where T : TWDModelObjectWithViewId where U : ModelView<T>
	{
		List<TWDModelObject> models = playerModel.manager.GetModels<T>();
		U[] array = UnityEngine.Object.FindObjectsOfType<U>();
		for (int i = 0; i < array.Length; i++)
		{
			bool flag = false;
			for (int j = 0; j < models.Count; j++)
			{
				if (array[i].ViewId == (models[j] as T).ViewId)
				{
					array[i].Initialize(models[j]);
					flag = true;
					break;
				}
			}
			if (!flag && destroyViewWithoutModel)
			{
				array[i].gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(array[i].gameObject);
			}
		}
	}

	private void InitializePreExistingView<T, U>() where T : TWDModelObject where U : ModelView<T>
	{
		List<TWDModelObject> models = playerModel.manager.GetModels<T>();
		U[] array = UnityEngine.Object.FindObjectsOfType<U>();
		if (models.Count == 1 && array.Length == 1)
		{
			array[0].Initialize(models[0]);
		}
		else if (models.Count > 0 || array.Length != 0)
		{
			Debug.LogError("Expecting only one instance of a model " + typeof(T).Name + " and found " + models.Count + " models and one instance of view " + typeof(U).Name + " and found " + array.Length + "!");
		}
	}

	private void InitializePreExistingViews()
	{
		if (playerModel.Combat != null)
		{
			OutpostSliceView[] componentsInChildren = UnityEngine.Object.FindObjectOfType<MissionSettings>().GetComponentsInChildren<OutpostSliceView>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetActive(value: true);
			}
			InitializePreExistingViews<OutpostSliceModel, OutpostSliceView>();
			InitializePreExistingViews<DoorModel, DoorView>();
			InitializePreExistingViews<AnimatedPropModel, AnimatedPropView>();
			InitializePreExistingViews<ExplosiveModel, ExplosiveView>();
			InitializePreExistingViews<InteractiveObjectModel, InteractiveObjectView>();
			InitializePreExistingViews<LevelLoopingSoundModel, LevelLoopingSoundView>();
			InitializePreExistingViews<TriggerModel, TriggerView>();
			InitializePreExistingViews<CombatColliderModel, CombatColliderView>();
			InitializePreExistingViews<ActorSpawnPointModel, ActorSpawnPointView>();
			InitializePreExistingViews<LootModel, LootView>();
			InitializePreExistingViews<CombatDialogPlayerModel, CombatDialogPlayerView>();
			InitializePreExistingViews<SetMissionObjectiveModel, SetMissionObjectiveView>();
			InitializePreExistingView<CombatExitModel, CombatExitView>();
			InitializePreExistingView<CombatModel, CombatView>();
			InitializePreExistingViews<MovableModel, MovableView>();
			NodeGraphWrapper[] array = UnityEngine.Object.FindObjectsOfType<NodeGraphWrapper>();
			foreach (NodeGraphWrapper nodeGraphWrapper in array)
			{
				List<TWDModelObject> models = playerModel.Combat.GetModels<NodeGraph>();
				bool flag = false;
				foreach (NodeGraph item in models)
				{
					if (item.GuidHash == nodeGraphWrapper.GuidHash)
					{
						nodeGraphWrapper.BindToModels(item);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Debug.LogError("Could not find node graph for wrapper '" + nodeGraphWrapper.gameObject.name + "', re-export level to synchronize unity scene and json data.");
				}
			}
		}
		else
		{
			InitializePreExistingView<CampModel, CampView>();
			InitializePreExistingView<CampDefenseModel, CampDefenseView>();
		}
	}

	public void ReturnFromVisit()
	{
		PlayerModel playerModel = Instance.playerModel;

		if (playerModel.Combat != null && MovementBaseModified)
		{
			MovementBaseModified = false;
			ActivateLongTurn(false);
			SavedDef = new();
		}
		bool wasOutpost = false;
		if (playerModel.Combat != null && playerModel.Combat.OutpostCombat != null)
		{
			wasOutpost = true;
			if (PortraitManager.Instance != null)
			{
				List<SurvivorModel> models = playerModel.Combat.OutpostCombat.DefendingSurvivors.Models;
				for (int i = 0; i < models.Count; i++)
				{
					PortraitRenderSource info = PortraitRenderSource.fromActorModel(models[i]);
					if (PortraitManager.Instance.GetPortrait(info) != null)
					{
						PortraitManager.Instance.RemovePortrait(info);
					}
				}
			}
		}
		ForceGoThatDetailMap = null;
		if (playerModel.Combat != null && playerModel.Combat.IsGuildBattleMission && GuildWarHelper.GetGuildWarModel() != null)
		{
			ForceGoThatDetailMap = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMissionModel;
		}
		else
		{
			ForceGoThatDetailMap = playerModel.MapContainerModel.AttackTargetMissionGroupModel;
		}
		if (playerModel.Combat != null)
		{
			SingularityMonoBehaviour<AssetBundleController>.Instance.UnloadSceneBundleDependencies(playerModel.Combat.SceneName);
		}
		TransitionScreenHUD obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Transition) as TransitionScreenHUD;

		if (IsLoadDataManager && OfflineManager.Instance.IsReturnToResidence)
		{
			obj.SceneToLoadAfterInAnimation = OfflineManager.MainSceneName;
		}

		obj.AnimationInCallback = delegate
		{
			Helpers.ExecuteCommand(new ReturnFromVisitCommand());
			if (wasOutpost)
			{
				CampHUD.TryOpenOutpostTutorial(CampHUD.OpenOutpostPopupAfterChecks);
				OutpostPopup outpostPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OutpostPopup) as OutpostPopup;
				if (outpostPopup.IsOpen)
				{
					UIToggleMenu componentInChildren = outpostPopup.GetComponentInChildren<UIToggleMenu>();
					if (componentInChildren != null)
					{
						componentInChildren.OpenContentByIndex(1);
						StartCoroutine(outpostPopup.Reward());
					}
				}
			}
		};
		obj.Open();
		SingularityMonoBehaviour<HUDManager>.Instance.Reset();
	}

	public List<T> GetViews<T>() where T : ModelViewBase
	{
		List<T> list = new List<T>();
		foreach (MonoBehaviour value in modelViewMap.Values)
		{
			T val = value as T;
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public ModelView<T> GetViewForModel<T>(T model) where T : ModelObject
	{
		if (modelViewMap.ContainsKey(model))
		{
			MonoBehaviour monoBehaviour = modelViewMap[model];
			ModelView<T> modelView = monoBehaviour as ModelView<T>;
			if (monoBehaviour != null && modelView == null)
			{
				Debug.LogError("GameManager::GetViewForModel<T>() -> Trying to get view for model " + model.ToString() + " but cannot cast type " + monoBehaviour.GetType().Name + " to requested type ModelView<" + typeof(T).Name + ">");
			}
			return modelView;
		}
		return null;
	}

	public void RegisterViewWithModel<T>(T model, ModelView<T> view) where T : ModelObject
	{
		if (!modelViewMap.ContainsKey(model))
		{
			modelViewMap.Add(model, view);
		}
		else
		{
			modelViewMap[model] = view;
		}
	}

	public bool UnregisterViewWithModel<T>(T model) where T : ModelObject
	{
		if (modelViewMap.ContainsKey(model))
		{
			UnityEngine.Object.Destroy(modelViewMap[model].gameObject);
			modelViewMap.Remove(model);
			return true;
		}
		return false;
	}

	public void SaveLocalPlayer()
	{
		DebugTWD.Log("SaveLocalPlayer - проверить");
		if (IsLoadDataManager || OfflineManager.IsPrivateMode) return;

		string content = modelManager.SerializeModel();
		ContentManager.Instance.GetCache("Player").SetContent("PlayerModel", null, null, content);
	}

	private void NotificationsSetup(bool schedule)
	{
		DebugTWD.Log("NotificationsSetup - проверить");
		if (IsLoadDataManager || OfflineManager.IsPrivateMode) return;
		if (!(!AllowNotifications() | (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)))
		{
			if (schedule)
			{
				ScheduleNotifications();
			}
			else
			{
				NotificationUtils.CancelAllLocalNotifications();
			}
		}
	}

	private void OnApplicationQuit()
	{
		if (Helpers.IsInEditor && modelManager != null)
		{
			SaveLocalPlayer();
		}
	}

	private void ApplicationQuit()
	{
		NotificationsSetup(schedule: true);
		Application.Quit();
	}

	private IEnumerator OnApplicationFocus(bool focus)
	{
		if (OfflineManager.IsLoadDataManager) yield break;
		yield return null;
		if (focus)
		{
			Instance.SendWebShopRequest();
		}
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		if (OfflineManager.IsLoadDataManager) yield break;
		if (FabricManager.Instance != null)
		{
			FabricManager.Instance.Pause(pause);
		}
		if (pause)
		{
			modelManager?.Metrics?.SendWalkersTapMetric();
		}
		else
		{
			Instance.SendWebShopRequest();
			timeResumed = DateTime.UtcNow.Ticks / 10000;
			yield return null;
			yield return null;
			if (PortraitManager.Instance != null)
			{
				StartCoroutine(PortraitManager.Instance.Refresh());
			}
		}
		if (!IsModelManagerInitialized)
		{
			if (pause)
			{
				pauseTimeMillis = DateTime.UtcNow.Ticks / 10000;
			}
			else if (((pauseTimeMillis > 0) ? (DateTime.UtcNow.Ticks / 10000 - pauseTimeMillis) : 0) >= LoadingScreenPauseTimeout && LoadingScreenPauseTimeout != -1)
			{
				LogReloadEvent("Reason: GameManager.OnApplicationPause, Pause: " + pause + ", Application resumed during initialization excided timout");
				ReloadGame();
			}
			yield break;
		}
		NotificationsSetup(pause);
		if (pause)
		{
			pauseTimeMillis = DateTime.UtcNow.Ticks / 10000;
		}
		else
		{
			yield return null;
			yield return null;
			Helpers.StartCoroutine(this, ResumeFromPause(), ref applicationResumeFromPauseCoroutine);
		}
		applicationPauseCoroutine = null;
	}

	private void ScheduleNotifications()
	{
		NotificationUtils.CancelAllLocalNotifications();
		for (int i = 0; i < modelManager.CampModel.Buildings.Count; i++)
		{
			BuildingModel buildingModel = modelManager.CampModel.Buildings[i];
			if (buildingModel != null && buildingModel.IsUpgrading)
			{
				NotificationUtils.ScheduleNotificationForBuilding(buildingModel);
			}
			if (buildingModel is WorkshopBuildingModel)
			{
				WorkshopBuildingModel workshopBuildingModel = buildingModel as WorkshopBuildingModel;
				if (workshopBuildingModel.UpgradingEquipment != null)
				{
					NotificationUtils.ScheduletNotificationForEquipment(workshopBuildingModel.UpgradingEquipment);
				}
			}
			if (buildingModel is TrainingGroundBuildingModel)
			{
				TrainingGroundBuildingModel trainingGroundBuildingModel = buildingModel as TrainingGroundBuildingModel;
				if (trainingGroundBuildingModel.UpgradingSurvivor != null)
				{
					NotificationUtils.ScheduleNotificationForSurvivor(trainingGroundBuildingModel.UpgradingSurvivor);
				}
			}
		}
		NotificationUtils.ScheduleDailyLoginNotifications();
		NotificationUtils.AddReminderNotifications();
		NotificationUtils.AddGasFullNotification();
		if (gameEconomyData.GetFeature("FreeCallNotification").Enabled)
		{
			NotificationUtils.AddFreeCallNotification();
		}
		if (gameEconomyData.GetFeature("UseLootKeysInPostCombatScreenNotification").Enabled)
		{
			NotificationUtils.AddLootKeyRefreshNotification();
		}
		if (gameEconomyData.GetFeature("AdsRefreshedNotification").Enabled)
		{
			NotificationUtils.AddAdsRefreshedNotification();
		}
		NotificationUtils.AddOutpostSeasonEnding();
		NotificationUtils.AddChallengeNotifications();
		NotificationUtils.AddTradeGoodShopNotification();
		NotificationUtils.AddGuildBattleNotifications();
		NotificationUtils.AddBlackMarketNotifications();
		NotificationUtils.AddBattlePassNotifications();
		NotificationUtils.ScheduleSavedNotifications();
	}

	private IEnumerator ResumeFromPause()
	{
		long deltaTime = ((pauseTimeMillis > 0) ? (DateTime.UtcNow.Ticks / 10000 - pauseTimeMillis) : 0);
		yield return null;
		bool flag = false;
		if (deltaTime > 30000)
		{
			if (dismissPopupOnNextResume)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
				LogReloadEvent($"Reason: GameManager.ResumeFromPause, pause too long. Pause duration : {deltaTime} ms, Time limit : 30000 ms");
				ReloadGame();
				flag = true;
			}
			memoryWarningReceivedInSession = false;
		}
		pauseTimeMillis = 0L;
		if (!flag)
		{
			long forceReloadTImeout = ForceReloadTimeout;
			if (modelManager != null && modelManager.GameEconomyData != null && modelManager.GameEconomyData.ConfigData != null && modelManager.GameEconomyData.ConfigData.ForceReloadTimeout != 0L)
			{
				forceReloadTImeout = modelManager.GameEconomyData.ConfigData.ForceReloadTimeout;
			}
			if (deltaTime >= forceReloadTImeout && !IsReturningFromAds)
			{
				if (SingularityMonoBehaviour<VideoAdManager>.Instance.IsPlaying)
				{
					while (SingularityMonoBehaviour<VideoAdManager>.Instance.IsPlaying)
					{
						yield return null;
					}
					while (SignalRClient.Instance.IsWaitingForResponse)
					{
						yield return null;
					}
				}
				LogReloadEvent($"Reason: GameManager.ResumeFromPause, pause too long, force reload. Pause duration : {deltaTime} ms, Time limit : {forceReloadTImeout} ms");
				ReloadGame();
				yield break;
			}
			if (GuildInviteFlow != null)
			{
				GuildInviteFlow.StartJoinGuildAfterResumeGame();
			}
		}
		IsReturningFromAds = false;
		applicationResumeFromPauseCoroutine = null;
	}

	public void GoToLoaderScene()
	{
		Helpers.StopCoroutine(this, ref loginCoroutine);
		Helpers.StopCoroutine(this, ref loadGEDCoroutine);
		Helpers.StopCoroutine(this, ref applicationPauseCoroutine);
		Helpers.StopCoroutine(this, ref applicationResumeFromPauseCoroutine);
		Helpers.StopCoroutine(this, ref downloadAssetBundleCoroutine);
		loginAborted = true;
		string item = "scene_gameloader";
		string scenarioName = "GameLoader";
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle(new List<string> { item }, delegate
		{
			AssetBundleManager.Instance.LoadScene(scenarioName);
			if (newPlayerLoadRequested)
			{
				TutorialView.Instance.StartupSetting = TutorialView.StartupSettingType.Normal;
				newPlayerLoadRequested = false;
			}
			EnableAssetLoaderUI(enable: true);
			if (SingularityMonoBehaviour<GdprFlowHandler>.Instance != null)
			{
				SingularityMonoBehaviour<GdprFlowHandler>.Instance.Reset();
			}
		});
	}

	public void LoadPersistentElementsScene()
	{
		string item = "scene_startup";
		string scenarioName = "startup";
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle(new List<string> { item }, delegate
		{
			AssetBundleManager.Instance.LoadScene(scenarioName);
		});
	}

	public void EnableAssetLoaderUI(bool enable)
	{
		if (SingularityMonoBehaviour<AssetLoaderRoot>.Instance != null)
		{
			SingularityMonoBehaviour<AssetLoaderRoot>.Instance.Show(enable);
		}
	}

	public void SetInputEnabled(bool isEnabled)
	{
		if (PlayerInputManager.Instance != null)
		{
			PlayerInputManager.Instance.IsEnabled = isEnabled;
		}
		Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Transform item in array[i].transform)
			{
				GameObject gameObject = item.gameObject;
				if (gameObject.tag == "InputDisable")
				{
					gameObject.SetActive(!isEnabled);
				}
			}
		}
		UICamera[] array2 = UnityEngine.Object.FindObjectsOfType<UICamera>();
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j].enabled = isEnabled;
		}
	}

	public bool AllowCameraMove()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen > 0)
		{
			return false;
		}
		if (UICamera.isOverUI && NGUITools.FindInParents<PreventCameraAction>(HelpersUI.GetTouchedUIObject()) != null)
		{
			return false;
		}
		return true;
	}

	private void LoadResourceMap<T>(string name)
	{
		UnityEngine.Object obj = UnityUtils.LoadFromAssetBundle(name, "scriptableobjects");
		if (obj == null)
		{
			Debug.LogError("Failed to load resource map '" + name + "'");
		}
		else
		{
			resourceMaps.Add(typeof(T), obj);
		}
	}

	public bool HasResources<T>(string identifier) where T : ResourceEntry
	{
		return GetResources<T>(identifier) != null;
	}

	public T GetResources<T>(string identifier) where T : ResourceEntry
	{
		Type typeFromHandle = typeof(T);
		if (resourceMaps.ContainsKey(typeFromHandle))
		{
			return (resourceMaps[typeFromHandle] as ResourcesMap<T>).GetResources(identifier);
		}
		return null;
	}

	public ColorEntry GetRarityColorData(int rarityLevel)
	{
		return rarityColorResources.GetRarityColor(rarityLevel);
	}

	public string GetEquipmentBackgroundRaritySprite(int rarity)
	{
		return HelpersGfx.GetEquipmentRaritySprite(rarity);
	}

	public FactionColorEntry GetFactionColorData(Faction faction)
	{
		return factionColorResources.GetFactionColor(faction);
	}

	public GameObject GetBundleCard(string cardIdentifier)
	{
		return bundleCardsResources.GetBundleCardPrefab(cardIdentifier);
	}

	public GameObject GetBundleRewardCard(string cardIdentifier)
	{
		return bundleRewardCardsResources.GetBundleRewardCardPrefab(cardIdentifier);
	}

	public GameObject GetBundleRewardCardByType(RewardType rewardType)
	{
		Dictionary<RewardType, string> dictionary = new Dictionary<RewardType, string>();
		dictionary.Add(RewardType.Currency, "Bundle_Card");
		dictionary.Add(RewardType.Outfit, "Bundle_Card_Outfit");
		dictionary.Add(RewardType.Equipment, "Bundle_Card_Weapon");
		dictionary.Add(RewardType.RandomEquipment, "Bundle_Card_Weapon");
		dictionary.Add(RewardType.SurvivorSlot, "Bundle_Card");
		dictionary.Add(RewardType.TimedBonus, "Bundle_Card");
		if (dictionary.ContainsKey(rewardType))
		{
			return bundleRewardCardsResources.GetBundleRewardCardPrefab(dictionary[rewardType]);
		}
		return bundleRewardCardsResources.GetBundleRewardCardPrefab("Bundle_Card_Label");
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		switch (eventType)
		{
		case EventManager.EventType.StateTransitionCompleted:
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.Transition);
			break;
		case EventManager.EventType.CinematicWatched:
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.SetMute(mute: false, "all");
				SingularityMonoBehaviour<AudioManager>.Instance.SetMute(!Settings.SoundFxOn, "ambience");
				SingularityMonoBehaviour<AudioManager>.Instance.SetMute(!Settings.MusicOn, "music");
			}
			if (TutorialView.Instance.Running)
			{
				Instance.GameCenterManager.PromptGameCenterConnect();
			}
			break;
		}
	}

	private void OnVideoWatched(bool completely)
	{
		if (!OfflineManager.IsUseServices)
		{
			Debug.Log("OnVideoWatched: return");
			return;
		}

		if (SingularityMonoBehaviour<VideoAdManager>.Instance != null)
		{
			SingularityMonoBehaviour<VideoAdManager>.Instance.RestoreAudio();
		}
		if (CurrentlyLoading)
		{
			videoRewardCommandPending = completely;
		}
		else if (!(SingularityMonoBehaviour<VideoAdManager>.Instance == null))
		{
			if (completely)
			{
				SingularityMonoBehaviour<SDKManager>.Instance.AdWatched();
			}
			EventManager.NotifyEvent(EventManager.EventType.VideoWatched, completely);
		}
	}

	public void StartNextChallenge()
	{
		if (modelManager.Player.WeeklyChallenge.CanPlayNextWeeklyChallenge)
		{
			Helpers.ExecuteCommand(new StartChallengeCommand());
		}
	}

	public void StartNextSurvival()
	{
		if (modelManager.Player.WeeklySurvival.CanPlayNextWeeklySurvival)
		{
			Helpers.ExecuteCommand(new StartSurvivalCommand());
		}
	}

	public void StartNextEndlessCycle()
	{
		EndlessModeManagerModel endlessModeManager = modelManager.Player.EndlessModeManager;
		if (endlessModeManager.CanStartNewEndlessModeCycle)
		{
			Helpers.ExecuteCommand(new StartEndlessCycleCommand());
		}
		else if (!endlessModeManager.AreEndlessActorsValidAndGenerated)
		{
			Helpers.ExecuteCommand(new ForceGenerateEndlessExpertActorsCommand());
		}
	}

	public void LoadNewAccount(string userId, string type)
	{
		Helpers.ExecuteCommand(new LoadNewAccountCommand
		{
			Type = type,
			UserId = userId
		});
		TWDPlayerPrefs.SetString("UserId", userId);
		TWDPlayerPrefs.Save();
		Instance.WaitCommandQueueAndReload();
	}

	public string GetFilteredText(string text)
	{
		return playerModel.ValidateStringsAgainstProfanity(text);
	}

	public static void LogReloadEvent(string message)
	{
		if (!OfflineManager.IsUseServices)
		{
			Debug.Log("LogReloadEvent: return!");
			return;
		}

		if (AnalyticsManager.instance != null)
		{
			AnalyticsManager.instance.CreateEvent("Connectivity_AutoReloadNetworkError").AddProperty("Message", message).Send();
			SingularityMonoBehaviour<SDKManager>.Instance.Reload("Connectivity_AutoReloadNetworkError", message);
		}
	}

	public void UpdateTargeteAdsConsent(bool consent, string dialogueName)
	{
		if (!OfflineManager.IsUseServices)
		{
			Debug.Log("UpdateTargeteAdsConsent: return!");
			return;
		}

		long timeStamp = (long)(DateTime.UtcNow - Helpers.UnixEpoch).TotalSeconds * 1000;
		Helpers.ExecuteCommand(new SetGdprStateCommand("TargetedAdsConsent", consent, timeStamp));
		SingularityMonoBehaviour<VideoAdManager>.Instance.ClearAdAvailable();
		Helpers.ExecuteCommand(new SendGdprMetricCommand(SendGdprMetricCommand.MetricType.End_GDPR)
		{
			DialogueName = dialogueName,
			DialogueDecision = (consent ? "1" : "0")
		});
	}

	public void AskForAdConsent(AdUsage adType, Action acceptedCallback, Action noAdsCallback)
	{
		ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		obj.SetContent(LocalizationManager.GetText("Popup.TOS.AdConsent.Title"), LocalizationManager.GetText("Popup.TOS.AdConsent.Content"));
		obj.SetOkButtonLabel(LocalizationManager.GetText("Popup.TOS.Button.Accept"));
		obj.SetCancelButtonLabel(LocalizationManager.GetText("Popup.TOS.Button.Decline"));
		obj.SetCallbacks(delegate
		{
			UpdateTargeteAdsConsent(consent: true, "Ad_Consent");
			ShowAdsLoading(adType, acceptedCallback, noAdsCallback);
		});
		obj.Open();
		obj.EnableCloseArea(enable: true);
	}

	public void ShowAdsLoading(AdUsage adType, Action adsAvailableCallback, Action noAdsCallback)
	{
		if (!OfflineManager.IsUseServices)
		{
			Debug.Log("ShowAdsLoading: return!");
			return;
		}

		ConnectingShopPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConnectingShopPopup) as ConnectingShopPopup;
		obj.SetMessageLocalizationKey("LOADING");
		obj.Open();
		float num = gameEconomyData.ConfigData.LoadingAdsPopupTimeout;
		if (num == 0f)
		{
			num = 5f;
		}
		StartCoroutine(WaitForAdsLoading(adType, adsAvailableCallback, noAdsCallback, num));
	}

	private IEnumerator WaitForAdsLoading(AdUsage adType, Action adsAvailableCallback, Action noAdsCallback, float timeout = 5f)
	{
		while (!SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(adType) && timeout > 0f)
		{
			timeout -= Time.deltaTime;
			yield return null;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.ConnectingShopPopup);
		if (SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(adType))
		{
			adsAvailableCallback();
			yield break;
		}
		OnNoAdsAfterLoading();
		noAdsCallback?.Invoke();
	}

	private void OnNoAdsAfterLoading()
	{
		if (!playerModel.AdsCompensationReceived)
		{
			if (Helpers.ExecuteCommand(new GiveAdsCompensationCommand()) == TWDModelResult.OK)
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				iAPConfirmPopupNew.ShowShopWhenClosed = false;
				Rewards adsCompensationRewards = gameEconomyData.GetAdsCompensationRewards();
				if (adsCompensationRewards.GetRewardsOfType(RewardType.Currency).Count > 0)
				{
					iAPConfirmPopupNew.OpenForCurrency(adsCompensationRewards.GetRewardsOfType(RewardType.Currency)[0] as RewardCurrency);
					iAPConfirmPopupNew.SetOkButtonLabel(LocalizationManager.GetText("Popup.TOS.PrivacyPolicy.Content.Ok"));
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.Ads.NoAdsNewuser.Title"), LocalizationManager.GetText("Popup.Ads.NoAdsNewuser.Content"));
				}
			}
			else
			{
				AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Ads.NoAdsOlduser.Title"), LocalizationManager.GetText("Popup.Ads.NoAdsOlduser.Content"), LocalizationManager.GetText("Button.Ok"));
			}
		}
		else
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Ads.NoAdsOlduser.Title"), LocalizationManager.GetText("Popup.Ads.NoAdsOlduser.Content"), LocalizationManager.GetText("Button.Ok"));
		}
	}

	public bool CheckConnectionReachability(bool showPopup = true, string commandLogName = "")
	{
		if (!IsConnectedToServer || IsOfflineMode)
		{
			return true;
		}
		if (SignalRClient.Instance.State != SignalRClientState.Connected || Application.internetReachability == NetworkReachability.NotReachable)
		{
			if (showPopup && gameEconomyData.GetFeature("LostConnectionBlocker").Enabled)
			{
				LostConnectionAlertPopup.ShowPopup();
			}
			if (!string.IsNullOrEmpty(commandLogName))
			{
				Debug.LogWarning("Client executed command before sending - " + commandLogName + "  with network state : " + SignalRClient.Instance.State.ToString() + ", NetworkReachability : " + NetworkReachability.NotReachable);
			}
			return false;
		}
		return true;
	}

	public bool IsIDFACheckEnabled()
	{
		GameEconomyData obj = gameEconomyData;
		if (obj != null && obj.GetFeature("IOSIDFAEnabled").Enabled)
		{
			return Helpers.GetIOSOSVersion() >= "14.5";
		}
		return false;
	}

	private void DispatchHighScoresList(EventManager.EventType eventType, string result)
	{
		IEnumerable<LeaderboardEntry> enumerable = Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(result);
		HighScores highScores = new HighScores
		{
			Scores = new List<ScoreEntry>()
		};
		int num = 1;
		foreach (LeaderboardEntry item in enumerable)
		{
			Leaderboards.ChallengeLeaderboardDetails challengeLeaderboardDetails = jsonSerializer.Deserialize<Leaderboards.ChallengeLeaderboardDetails>(item.Details);
			ScoreEntry scoreEntry = new ScoreEntry();
			scoreEntry.Position = num++;
			scoreEntry.Score = (int)item.Score;
			scoreEntry.Nickname = challengeLeaderboardDetails.Name;
			scoreEntry.Country = challengeLeaderboardDetails.Country;
			scoreEntry.GroupId = challengeLeaderboardDetails.GroupId;
			scoreEntry.GroupName = challengeLeaderboardDetails.GroupName;
			scoreEntry.Level = challengeLeaderboardDetails.Level;
			highScores.Scores.Add(scoreEntry);
		}
		EventManager.NotifyEvent(eventType, highScores);
	}

	public void GetHighScoreFriendsList()
	{
		string friends = FriendListManager.GetFriends();
		if (string.IsNullOrEmpty(friends))
		{
			EventManager.NotifyEvent(EventManager.EventType.SocialScoreLoaded);
		}
		else
		{
			SignalRClient.Instance.RequestCommand("GetHighScoresBySocialIds", friends, "100", OnHighScoreListFriends, null, waitForResponse: true);
		}
	}

	protected void OnHighScoreListFriends(string result)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(result))
		{
			Debug.LogError("GetHighScoresFriends failed");
			SignalRClient.Instance.ClearError();
			return;
		}
		ScoreEntry[] array = Instance.jsonSerializer.DeserializeObject<ScoreEntry[]>(result);
		HighScores highScores = new HighScores();
		highScores.Scores = new List<ScoreEntry>();
		int num = 1;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Nickname != "" && array[i].Nickname != "-")
			{
				array[i].Position = num;
				num++;
				highScores.Scores.Add(array[i]);
			}
		}
		PlayerModel playerModel = Instance.playerModel;
		ScoreEntry[] array2 = array;
		foreach (ScoreEntry scoreEntry in array2)
		{
			if (scoreEntry.HashedId == playerModel.HashedId)
			{
				scoreEntry.Nickname = playerModel.Name;
			}
		}
		EventManager.NotifyEvent(EventManager.EventType.SocialScoreLoaded, highScores);
	}

	public bool ShouldAskForAdConsent()
	{
		if (!HasAnsweredTargetedAdsConsentQuestion())
		{
			return playerModel.LifeTimeInDays >= gameEconomyData.ConfigData.NoAdsWaitingDays;
		}
		return false;
	}

	public bool HasAnsweredTargetedAdsConsentQuestion()
	{
		return playerModel.HasAcceptedGdprAction("TargetedAdsConsent");
	}

	private void CheckForHelpShiftGesture()
	{
	}

	public void SendRequest(string url, RequestMethod method = RequestMethod.GET, string bodyJson = "", Action<string> onSuccess = null, Action<string> onError = null)
	{
		StartCoroutine(SendRequestCoroutine(url, method, bodyJson, onSuccess, onError));
	}

	public void SendWebShopRequest()
	{
		if (IsLoadDataManager || !IsConnectedToServer || Instance.gameEconomyData == null || playerModel == null || Helpers.DateTimeToUnixTime(DateTime.UtcNow) < lastRequestBuyBundleResultInfoListTimestamp + 10)
		{
			return;
		}
		lastRequestBuyBundleResultInfoListTimestamp = Helpers.DateTimeToUnixTime(DateTime.UtcNow);
		string bodyJson = JsonConvert.SerializeObject(new BuyBundleRequestInfo
		{
			HashId = playerModel.HashedId
		});
		Instance.SendRequest(Helpers.GetBananaURL() + "api/requestpaiedbundles", RequestMethod.POST, bodyJson, delegate(string result)
		{
			buyBundleResultInfoList = JsonConvert.DeserializeObject<BuyBundleResultInfoList>(result);
			if (buyBundleResultInfoList != null && buyBundleResultInfoList.BuyBundleResultList != null && buyBundleResultInfoList.BuyBundleResultList.Count > 0)
			{
				if (Helpers.ExecuteCommand(new BuyWebshopAndTradefairBundleCommand
				{
					buyBundleResultInfo = buyBundleResultInfoList
				}) == TWDModelResult.BuyWebshopAndTradefairBundleCommandSuc)
				{
					if (buyBundleResultInfoList != null && buyBundleResultInfoList.BuyBundleResultList != null && buyBundleResultInfoList.BuyBundleResultList.Count > 0)
					{
						string pCPlatformType = playerModel.gameEconomyData.ConfigData.GetPCPlatformType(0);
						for (int i = 0; i < buyBundleResultInfoList.BuyBundleResultList.Count; i++)
						{
							string identifier = buyBundleResultInfoList.BuyBundleResultList[i].BundleId;
							if (buyBundleResultInfoList.BuyBundleResultList[i].PurchaseSource == "tradefair" || buyBundleResultInfoList.BuyBundleResultList[i].PurchaseSource == pCPlatformType)
							{
								TradefairBundleStoreDefinition bundleTradefairDefinition = Instance.gameEconomyData.GetBundleTradefairDefinition(identifier);
								TradefairBundleContentDefinition tradefairBundleContentDefinition = Instance.gameEconomyData.GetTradefairBundleContentDefinition(bundleTradefairDefinition.BundleIdentifier);
								IAPConfirmBananaPopupNew.OpenWithTradeFairBundleContentLogin(bundleTradefairDefinition, tradefairBundleContentDefinition, givenBySupport: false, isLast: false);
							}
							else
							{
								if (buyBundleResultInfoList.BuyBundleResultList[i].IsFreeDailyBundle)
								{
									identifier = buyBundleResultInfoList.BuyBundleResultList[i].RandomResultBundleId;
								}
								BundleStoreDefinition bundleStoreDefinition = Instance.gameEconomyData.GetBundleStoreDefinition(identifier);
								BundleContentDefinition bundleContentDefinition = Instance.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
								IAPConfirmBananaPopupNew.OpenWithBundleContentLogin(bundleStoreDefinition, bundleContentDefinition, givenBySupport: false, isLast: false);
								ShowWebShopExtraRewardByBundleDefinition(bundleStoreDefinition, bundleContentDefinition, buyBundleResultInfoList.BuyBundleResultList[i].PurchaseSource);
							}
						}
						SingularityMonoBehaviour<SDKManager>.Instance.SentWebShopData(buyBundleResultInfoList.BuyBundleResultList);
						buyBundleResultInfoList = null;
					}
				}
				else
				{
					buyBundleResultInfoList = null;
				}
			}
		}, delegate
		{
			buyBundleResultInfoList = null;
		});
	}

	private IEnumerator SendRequestCoroutine(string url, RequestMethod method, string bodyJson, Action<string> onSuccess, Action<string> onError)
	{
		UnityWebRequest webRequest = CreateWebRequest(url, method, bodyJson);
		_ = Time.time;
		yield return webRequest.SendWebRequest();
		if (webRequest.result == UnityWebRequest.Result.Success)
		{
			onSuccess?.Invoke(webRequest.downloadHandler.text);
		}
		else
		{
			string text = "Error: " + webRequest.error;
			if (webRequest.responseCode != 0L)
			{
				text += $", Status: {webRequest.responseCode}";
			}
			onError?.Invoke(text);
		}
		webRequest.Dispose();
	}

	private UnityWebRequest CreateWebRequest(string url, RequestMethod method, string bodyJson)
	{
		UnityWebRequest unityWebRequest;
		if (method != RequestMethod.GET && method == RequestMethod.POST)
		{
			unityWebRequest = new UnityWebRequest(url, method.ToString());
			if (!string.IsNullOrEmpty(bodyJson))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(bodyJson);
				unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
			}
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.SetRequestHeader("Content-Type", "application/json");
		}
		else
		{
			unityWebRequest = UnityWebRequest.Get(url);
		}
		return unityWebRequest;
	}

	private void ShowWebShopExtraRewardByBundleDefinition(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition, string purchaseSource)
	{
		if (purchaseSource == null || !purchaseSource.Equals("IAPBundle") || !bundleStoreDefinition.BundleIdentifier.EndsWith("_WB") || !gameEconomyData.ConfigData.IsPriceRangeEnabled)
		{
			return;
		}
		string identifier = bundleContentDefinition.Identifier.Substring(0, bundleContentDefinition.Identifier.Length - 3);
		BundleContentDefinition bundleContentDefinition2 = gameEconomyData.GetBundleContentDefinition(identifier);
		InAppPurchaseProductApple inAppPurchaseProduct = gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition2.IAPProduct);
		if (gameEconomyData.ConfigData.IsPriceInRange(inAppPurchaseProduct.PriceUSD))
		{
			Rewards extraGiftRewards = gameEconomyData.ConfigData.GetExtraGiftRewards();
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.OpenForRewards(extraGiftRewards.RewardsList);
				iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			}
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private string CustomMissionContent { get; set; }
	#endregion

	#region mycode
	public void SetGameEconomyData(GameEconomyData data)
	{
		gameEconomyData = data;
	}

	public void SetModelManager(TWDModelManager manager)
	{
		modelManager = manager;
	}

	public void SetGuildManager(GuildManager guildManager)
	{
		GuildManager = guildManager;
	}

	public void SetGameState(GameState state)
	{
		State = state;
	}

	public void SetIAPManager()
	{
		if (IAPManager == null)
		{
			IAPManager = base.gameObject.GetComponent<IAPManager>();
			if (IAPManager == null)
			{
				IAPManager = base.gameObject.AddComponent<IAPManager>();
			}
		}
		if (OfflineManager.Instance.State == SignalRClientState.Connected) IAPManager.PopulateProductList(gameEconomyData.GetInAppPurchaseProductIdList());
		CampManager.Instance.CheckBundleTrigger();
	}

	public void InitializeResources()
	{
		resourceMaps = new Dictionary<Type, object>();
		InitializationOps = new List<AsyncOperation>();

		NotificationUtils.CancelAllLocalNotifications();
		EventManager.OnEvent += OnEvent;
		LoadResourceMap<EquipmentResourceEntry>("EquipmentsResources");
		LoadResourceMap<AbilityResourceEntry>("AbilitiesResources");
		LoadResourceMap<ActorResourceEntry>("ActorsResources");
		LoadResourceMap<InhabitantResourceEntry>("InhabitantsResources");
		LoadResourceMap<CharacterResourceEntry>("CharacterResources");

		outfitResources = UnityUtils.LoadFromAssetBundle<OutfitResourcesMap>("OutfitResources", "scriptableobjects");
		heroSkinResources = UnityUtils.LoadFromAssetBundle<HeroSkinResourcesMap>("HeroSkinResources", "scriptableobjects");
		rarityColorResources = UnityUtils.LoadFromAssetBundle<RarityColorsResource>("RarityColorsResources", "scriptableobjects");
		factionColorResources = UnityUtils.LoadFromAssetBundle<FactionColorsResource>("FactionColorsResources", "scriptableobjects");
		bundleCardsResources = UnityUtils.LoadFromAssetBundle<BundleCardsResource>("BundleCardsResources", "scriptableobjects");
		bundleRewardCardsResources = UnityUtils.LoadFromAssetBundle<BundleRewardCardsResource>("BundleRewardCardsResources", "scriptableobjects");

		characterTemplate = UnityUtils.LoadFromAssetBundle<PrefabResource>("CharacterTemplate", "scriptableobjects");
		if (characterTemplate == null)
		{
			Debug.LogError("Could not load the modular character template!");
		}
		characterTemplatePortrait = UnityUtils.LoadFromAssetBundle<PrefabResource>("CharacterTemplatePortrait", "scriptableobjects");
		if (characterTemplatePortrait == null)
		{
			Debug.LogError("Could not load the modular character portrait template!");
		}
		TimingManager = new CoroutineTimingManager();
		BundleSource = Metrics.BundleSource.Auto;
	}

	public GameObject GetCustomPrefab(string prefabName)
	{
		characterTemplate.PrefabName = prefabName;
		characterTemplate.IsCustom = true;
		return characterTemplate.GetPrefab();
	}

	public static void FixShadersFromMatList(List<Material> mats)
	{
		foreach (var mat in mats)
		{
			string shaderOldName = mat.shader.name;

			if (shaderOldName.Contains("HOVL")) shaderOldName = "Mobile/Particles/Additive";
			mat.shader = Shader.Find(shaderOldName);
		}
	}

	public static void FixShadersMeshFromParent(Transform root, bool includeInactive = true)
	{
		var renderers = root.GetComponentsInChildren<Renderer>(includeInactive).ToList();
		for (int i = 0; i < renderers.Count; i++)
		{
			var renderer = renderers[i];

			var mats = renderer.materials;
			foreach (var mat in mats)
			{
				string shaderOldName = mat.shader.name;
				mat.shader = Shader.Find(shaderOldName);
			}
		}
	}

	public static void FixShadersMesh(bool includeInactive = true)
	{
		var renderers = FindObjectsOfType<Renderer>(includeInactive).ToList();
		for (int i = 0; i < renderers.Count; i++)
		{
			var renderer = renderers[i];

			var mats = renderer.materials;
			foreach (var mat in mats)
			{
				string shaderOldName = mat.shader.name;
				mat.shader = Shader.Find(shaderOldName);

				if (mat.shader.name == "TWD/Transparent/Diffuse Transparent Vertexcolor Dual" || mat.shader.name == "TWD/Transparent/Diffuse Tint Transparent Dual")
				{
					mat.shader = Shader.Find("Unlit/Transparent Masked");
				}

				if (mat.shader.name == "TWD/MaterialCapture Bumped Alphatest Dual")
				{
					mat.SetFloat("_MatcapBlendFactor", 0);
				}
				//"Hovl/Particles/Add_CenterGlow"
			}
		}

		var particles = FindObjectsOfType<ParticleSystem>(includeInactive).ToList();
		for (int i = 0; i < particles.Count; i++)
		{
			var part = particles[i].GetComponent<ParticleSystemRenderer>();

			if (part.material.name.StartsWith("GlowDot"))
			{
				var tex = Resources.Load<Texture>("Game/BuildingAppereanceGlow_copy");
				if (tex != null)
				{
					part.material.SetTexture("_MainTex", tex);

				}
			}
		}
	}
	public static void FixShadersSkinned(bool includeInactive = true)
	{
		var renderers = FindObjectsOfType<SkinnedMeshRenderer>(includeInactive).ToList();
		for (int i = 0; i < renderers.Count; i++)
		{
			var renderer = renderers[i];

			var mats = renderer.materials;
			foreach (var mat in mats)
			{
				string shaderOldName = mat.shader.name;
				mat.shader = Shader.Find(shaderOldName);
			}
		}
	}

	public void SubscribeEvents()
	{
		if (modelManager != null)
		{
			modelManager.ActionExecuted -= OnActionExecuted;
			modelManager.ActionExecuted += OnActionExecuted;

			if (playerModel != null)
			{
				playerModel.Changed -= OnPlayerChange;
				playerModel.Changed += OnPlayerChange;
			}
		}
	}

	public void MainLogin()
	{
		Helpers.StartCoroutine(this, Login(), ref loginCoroutine);
	}


	public int ModelRandomNew;
	public int ModelRandomPlayerNew;
	public void SetNewPlayerRandomValue()
	{
		playerModel?.PlayerRandom.SetNewState(ModelRandomPlayerNew);
	}

	public void SetNewRandomValue()
	{
		playerModel?.LootManager.GetDedicatedRandom("BadgeRandom").SetNewState(ModelRandomNew);
	}
	#endregion


	#region myparams
	// меняем вручную перед билдом
	public bool IsShowModInPlayer = false;
	private bool IsActivateMod;
	private bool MovementBaseModified;
	private List<Rect> ListRect = new List<Rect>();
	private List<Vector4> ListRectMetric = new List<Vector4>();

	private Rect RectFromVec4(Vector4 vec)
	{
		return new Rect(vec.x, vec.y, vec.z, vec.w);
	}

	private Vector4 Vector4FromRect(Rect rect)
	{
		return new Vector4(rect.x, rect.y, rect.width, rect.height);
	}

	private List<int> PlayerRandomState = new List<int>();
	private int playerRandomCallCount = 0;
	private int playerRandomTimes = 0;
	private float mult = 1f;
	private float oldMult = 1f;

	private Dictionary<int, float> SavedDef = new Dictionary<int, float>();

	public bool IsDodge { get; private set; }
	public bool IsUnlockPVP { get; private set; }
	public bool IsUnlockAllSectors { get; private set; }
	public bool IsOfflineMode { get; set; }
	public bool IsWriteData { get; set; }
	public bool IsOffThinkingAnalytics { get; set; }

	private Texture2D BackTexture
	{
		get
		{
			if (backTexture == null)
			{
				CreateGUITexture();
			}
			return backTexture;
		}
	}
	private Texture2D backTexture;

	private enum LanguageCode { EN, RU }
	private LanguageCode Language = LanguageCode.RU;
	private Color originalColor;
	private int command_errors_count;
	public Color CommandErrorColor { get; set; } = Color.green;

	//private Rect windowRect = new Rect(50, 50, 270, 590);
	private Rect windowRect = new Rect(50, 50, 20, 20);
	private float windowHeight = 590;
	private bool showDropdown = false;
	private int selectedIndex = -1;
	private Vector2 scrollPosition;
	private List<string> branchOptions = new List<string> { "release", "test", "feature", "offline" };
	private float listHeight = 140;
	private GUIStyle bigButtonStyle;

	//for circle gestures
	private List<Vector2> lineList = new();
	private bool done = false;
	private Vector2 vCenter;
	private Vector2 vRadius;
	private float fAngle;

	internal List<OfflineCommandItem> OfflineCommandItems { get; private set; } = new();
	public string SessionToken = "";
	public string DirectURL = "";

	public static BuildConfiguration ActiveConfiguration => IsCustomConfiguration ? GetActiveConfiguration : BuildConfigurationManager.Instance.ActiveConfiguration;

	public static bool IsCustomConfiguration { get { return PlayerPrefs.HasKey("ActiveBranch"); } }
	public static string ActiveBranch
	{
		get
		{
			if (PlayerPrefs.HasKey("ActiveBranch"))
				return PlayerPrefs.GetString("ActiveBranch");
			return BuildConfiguration.Active.Branch;
		}
	}

	private static BuildConfigurationData _buildConfigurationData;

	public static BuildConfiguration GetActiveConfiguration
	{
		get
		{
			if (_buildConfigurationData == null)
			{
				_buildConfigurationData = Resources.Load<BuildConfigurationData>("BuildConfigurationData");
			}
			if (_buildConfigurationData == null)
			{
				Debug.LogError("No build configuration");
				return null;
			}
			if (_buildConfigurationData.BuildConfigurations.Count == 0)
			{
				Debug.LogError("No build configurations");
				return null;
			}
			return _buildConfigurationData.BuildConfigurations.FirstOrDefault((BuildConfiguration bc) => bc.Branch == ActiveBranch);
		}
	}

	internal class OfflineCommandItem
	{
		public string Arg { get; set; }
		public string Type { get; set; }
		public IModelCommand Command { get; set; }
		public bool WaitForResponse { get; set; }
	}
	#endregion

	#region mycode
	private void SavePvpTeamsListPerSector()
	{
		Dictionary<int, List<GuildBattlePvpTeam>> teams = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PVPTeamsListPerSector;
		var jsonString = jsonSerializer.Serialize(teams);
		string path = @"e:\Unity Projects\TWD\Projects\Resources\JSON\PvpTeamsListPerSector_origin.json";
		File.WriteAllText(path, jsonString);
	}

	private void OnGUI()
	{
		if (!IsShowModInPlayer) return;
		if (!IsActivateMod)
		{
			DrawToggle();
		}
		else
		{
			DrawMod();
		}
	}

	private void DrawMod()
	{
		originalColor = GUI.backgroundColor;

		GUI.skin.label.onHover.textColor = Color.white;
		GUI.skin.button.onNormal.textColor = Color.green;

		GUI.skin.label.fontSize = Mathf.RoundToInt(14 * mult);
		GUI.skin.toggle.fontSize = Mathf.RoundToInt(12 * mult);
		GUI.skin.box.fontSize = Mathf.RoundToInt(14 * mult);
		GUI.skin.toggle.stretchWidth = true;
		GUI.skin.toggle.stretchHeight = true;
		GUI.skin.toggle.contentOffset = new Vector2(5, 2);
		GUI.skin.button.fontSize = Mathf.RoundToInt(14 * mult);

		bigButtonStyle = new GUIStyle(GUI.skin.button);
		bigButtonStyle.fontSize = Mathf.RoundToInt(18 * mult);

		windowRect = GUI.Window(0, windowRect, DoMyWindow, "");
	}

	private void DrawToggle()
	{
		windowRect = GUI.Window(0, windowRect, DoMyWindowToggle, "");
	}

	void DoMyWindow(int windowID)
	{
		if (GUI.Button(new Rect(0, 0, 30 * mult, 30 * mult), "-", bigButtonStyle))
		{
			mult -= .2f;
		}
		if (GUI.Button(new Rect(30 * mult, 0, 30 * mult, 30 * mult), "+", bigButtonStyle))
		{
			mult += .2f;
		}
		if (GUI.Button(new Rect(240 * mult, 0, 30 * mult, 30 * mult), "X", bigButtonStyle))
		{
			CloseModUI();
		}
		GUI.BeginGroup(new Rect(0, 0, 270 * mult, windowHeight * mult));

		//GUI.Box(new Rect(30 * mult, 30 * mult, 270 * mult, windowHeight * mult), "NML Trainer v0.3 by Bloodymary");
		GUI.Label(new Rect(30 * mult, 35 * mult, 240 * mult, 30 * mult), "NML Trainer v0.3 by Bloodymary");
		if (ListRect.Count == 0)
		{
			ListRect = new List<Rect>();
			ListRectMetric = new List<Vector4>();
			int prelastIndex = 13;
			int secondIndex = 0;
			for (int i = 0; i <= prelastIndex; i++)
			{
				if (i > prelastIndex - 2)
				{
					secondIndex++;
				}
				var rect1 = new Rect(10 * mult, (60 + i * 35 - 10 * secondIndex) * mult, 248 * mult, (secondIndex > 0 ? 20 : 30) * mult);
				ListRect.Add(rect1);
				ListRectMetric.Add(Vector4FromRect(rect1));
			}
			Rect rect2 = ListRect[prelastIndex];
			rect2.height = 120 * mult;
			ListRectMetric[prelastIndex] = Vector4FromRect(rect2);
			ListRect[prelastIndex] = rect2;

			var rect3 = new Rect(10 * mult, (60 + prelastIndex * 35 + 100) * mult, 248 * mult, 30 * mult);
			ListRect.Add(rect3);
			ListRectMetric.Add(Vector4FromRect(rect3));
		}

		string toggleTextOfflineRu = IsOfflineMode ? "Режим OFFLINE: ВКЛ" : "Режим OFFLINE: ВЫКЛ";
		string toggleTextOfflineEn = IsOfflineMode ? "Offline mode: ON" : "Offline mode: OFF";
		string toggleTextOffline = Language == LanguageCode.RU ? toggleTextOfflineRu : toggleTextOfflineEn;
		if (GUI.Toggle(ListRect[0], IsOfflineMode, toggleTextOffline, "button") != IsOfflineMode)
		{
			if (SignalRClient.Instance != null)
			{
				IsOfflineMode = !IsOfflineMode;
				HelpersModel.IsOfflineMode = IsOfflineMode;

				if (IsOfflineMode)
				{
					SetOfflineMode();
				}
				else
				{
					SetOnlineMode();
				}
			}
		}

		string toggleTextWriteDataRu = IsWriteData ? "Сохранять данные в JSON" : "НЕ сохранять в JSON";
		string toggleTextWriteDataEn = IsWriteData ? "Save data to JSON" : "Dont save JSON";
		string toggleTextWriteData = Language == LanguageCode.RU ? toggleTextWriteDataRu : toggleTextWriteDataEn;
		if (GUI.Toggle(ListRect[1], IsWriteData, toggleTextWriteData, "button") != IsWriteData)
		{
			IsWriteData = !IsWriteData;
		}

		string toggleTextAnalyticsRu = IsOffThinkingAnalytics ? "Отправка аналитики: ВЫКЛ" : "Отправка аналитики: ВКЛ";
		string toggleTextAnalyticsEn = IsOffThinkingAnalytics ? "Send Analytics: OFF" : "Send Analytics: ON";
		string toggleTextAnalyticsAll = Language == LanguageCode.RU ? toggleTextAnalyticsRu : toggleTextAnalyticsEn;
		if (GUI.Toggle(ListRect[2], IsOffThinkingAnalytics, toggleTextAnalyticsAll, "button") != IsOffThinkingAnalytics)
		{
			if (SignalRClient.Instance != null)
			{
				IsOffThinkingAnalytics = !IsOffThinkingAnalytics;
				HelpersModel.IsOffThinkingAnalytics = IsOffThinkingAnalytics;
			}
		}

		string toggleTextLongMoveRu = MovementBaseModified ? "Длинный ход: ВКЛ" : "Длинный ход: ВЫКЛ";
		string toggleTextLongMoveEn = MovementBaseModified ? "Long movement: ON" : "Long movement: OFF";
		string toggleTextLongMove = Language == LanguageCode.RU ? toggleTextLongMoveRu : toggleTextLongMoveEn;
		if (GUI.Toggle(ListRect[3], MovementBaseModified, toggleTextLongMove, "button") != MovementBaseModified)
		{
			MovementBaseModified = !MovementBaseModified;
			ActivateLongTurn(MovementBaseModified);
		}

		string toggleTextDodgeRu = IsDodge ? "Уворот от всего: ВКЛ" : "Уворот от всего: ВЫКЛ";
		string toggleTextDodgeEn = IsDodge ? "Dodge full: ON" : "Dodge full: OFF";
		string toggleTextDodge = Language == LanguageCode.RU ? toggleTextDodgeRu : toggleTextDodgeEn;
		if (GUI.Toggle(ListRect[4], IsDodge, toggleTextDodge, "button") != IsDodge)
		{
			IsDodge = !IsDodge;
			HelpersModel.IsDodge = IsDodge;
		}

		string toggleTextIsFakeBattleRu = IsUnlockPVP ? "Разблокировать PVP: ВКЛ" : "Разблокировать PVP: ВЫКЛ";
		string toggleTextIsFakeBattleEn = IsUnlockPVP ? "Unlock all PVP: ON" : "Unlock all PVP: OFF";
		string toggleTextIsFakeBattle = Language == LanguageCode.RU ? toggleTextIsFakeBattleRu : toggleTextIsFakeBattleEn;
		if (GUI.Toggle(ListRect[5], IsUnlockPVP, toggleTextIsFakeBattle, "button") != IsUnlockPVP)
		{
			IsUnlockPVP = !IsUnlockPVP;
			HelpersModel.IsUnlockPVP = IsUnlockPVP;

			if (IsUnlockPVP == true && IsOfflineMode == false)
			{
				IsOfflineMode = true;
				HelpersModel.IsOfflineMode = true;
				SetOfflineMode();
			}
		}

		string toggleTextIsUnlockAllRu = IsUnlockAllSectors ? "Разблокировать сектора: ВКЛ" : "Разблокировать сектора: ВЫКЛ";
		string toggleTextIsUnlockAllEn = IsUnlockAllSectors ? "Unlock all sectors: ON" : "Unlock all sectors: OFF";
		string toggleTextIsUnlockAll = Language == LanguageCode.RU ? toggleTextIsUnlockAllRu : toggleTextIsUnlockAllEn;
		if (GUI.Toggle(ListRect[6], IsUnlockAllSectors, toggleTextIsUnlockAll, "button") != IsUnlockAllSectors)
		{
			IsUnlockAllSectors = !IsUnlockAllSectors;
			HelpersModel.IsUnlockAllSectors = IsUnlockAllSectors;

			if (IsUnlockAllSectors == true && IsOfflineMode == false)
			{
				IsOfflineMode = true;
				HelpersModel.IsOfflineMode = true;
				SetOfflineMode();
			}
		}

		string endTurnString = Language == LanguageCode.RU ? "Конец Хода" : "End of turn";
		if (GUI.Button(ListRect[7], endTurnString))
		{
			OnClickSkipTurnOverride();
		}

		string restoreTurnString = Language == LanguageCode.RU ? "Восстановить ход" : "Restore turn";
		if (GUI.Button(ListRect[8], restoreTurnString))
		{
			OnClickNewTurnOverride();
		}

		string reloadGameString = Language == LanguageCode.RU ? "Перезапустить игру" : "Reload game";
		if (GUI.Button(ListRect[9], reloadGameString))
		{
			ReloadGameHud();
		}

		string resetRandomString = Language == LanguageCode.RU ? "Сбросить счетчик рандома" : "Reset random counter";
		if (GUI.Button(ListRect[10], resetRandomString))
		{
			PlayerRandomState.Clear();
			playerRandomCallCount = 0;
			playerRandomTimes = 0;
		}

		GUI.Label(ListRect[11], "PlayerRandom CallCount : " + playerRandomCallCount.ToString());
		GUI.Label(ListRect[12], "PlayerRandom Times : " + playerRandomTimes.ToString());
		//PlayerRandomState = new() { 111111, 111111, 111111, 111111, 111111, 1111111 };
		GUI.Label(ListRect[13], "PlayerRandom State : \n" + string.Join("\n", PlayerRandomState));

		//ShowDropDown(ListRect[13]);

		GUI.EndGroup();
		var rectError = new Rect(5 * mult, 35 * mult, 20 * mult, 20 * mult);
		DrawBox(rectError, "", CommandErrorColor);
		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}

	private bool isShowButton;
	void DoMyWindowToggle(int windowID)
	{
		if (isShowButton)
		{
			var rectToggle = new Rect(0, 0, 20 * mult, 20 * mult);

			GUI.skin.button.stretchWidth = true;
			GUI.skin.button.stretchHeight = true;

			if (GUI.Button(rectToggle, ""))
			{
				OpenModUI();
				isShowButton = false;
			}
		}
		if (!IsActivateMod) GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}

	public static void WriteDataToDisk(string name, string message)
	{
		var pathDir = Application.persistentDataPath + '\\' + "SavedData\\";
		if (!Directory.Exists(pathDir))
		{
			Directory.CreateDirectory(pathDir);
		}
		File.WriteAllText(pathDir + name + ".json", message);
	}

	private void RedrawMenu()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetKeyDown(KeyCode.Equals))
			{
				mult += .2f;
			}
			if (Input.GetKeyDown(KeyCode.Minus))
			{
				mult -= .2f;
			}
		}

		if (oldMult != mult)
		{
			var x = windowRect.x;
			var y = windowRect.y;
			oldMult = mult;

			if (IsActivateMod)
			{
				windowRect = new Rect(x, y, 270 * mult, windowHeight * mult);
			}
			else
			{
				windowRect = new Rect(x, y, 20 * mult, 20 * mult);
			}

			if (ListRect.Count > 0)
			{
				for (int i = 0; i < ListRect.Count; i++)
				{
					var metric = ListRectMetric[i];
					ListRect[i] = new Rect(metric.x * mult, metric.y * mult, metric.z * mult, metric.w * mult);
				}
			}
		}
	}

	public void Show_Command_Error(int code)
	{
		if (code == -1 || code == 0 || code == 42)
		{
			command_errors_count = 0;
			CommandErrorColor = Color.green;
		}
		else
		{
			command_errors_count++;
			CommandErrorColor = Color.red;
		}
	}

	private void CreateGUITexture()
	{
		backTexture = new Texture2D(1, 1);
		backTexture.SetPixel(0, 0, Color.white);
		backTexture.Apply();
	}

	public void DrawBox(Rect position, string text, Color color)
	{
		GUIStyle colorBoxStyle = new GUIStyle(GUI.skin.box);
		colorBoxStyle.hover.textColor = Color.white;

		colorBoxStyle.normal.background = BackTexture;

		GUI.backgroundColor = color;
		GUI.Box(position, text, colorBoxStyle);

		GUI.backgroundColor = originalColor;
		colorBoxStyle.normal.background = null;
	}

	void ShowDropDown(Rect dropdownRect)
	{
		string buttonText = (selectedIndex == -1) ? "Выберите конфигурацию..." : branchOptions[selectedIndex];

		if (GUI.Button(dropdownRect, buttonText))
		{
			showDropdown = !showDropdown;

			if (showDropdown)
			{
				windowHeight += 120;
			}
			else
			{
				windowHeight -= 120;
			}
			windowRect = new Rect(windowRect.x, windowRect.y, 270 * mult, windowHeight * mult);
		}
		if (showDropdown)
		{
			// Define the rectangle for the scrollable area, positioned below the button
			Rect listRect = new Rect(10, dropdownRect.y + dropdownRect.height - 10 * mult, dropdownRect.width, listHeight * mult);

			// Begin the scroll view
			scrollPosition = GUI.BeginScrollView(listRect, scrollPosition, new Rect(0, 0, dropdownRect.width, branchOptions.Count * 30 * mult), false, false);

			// Use a vertical layout group for the list items
			GUILayout.BeginVertical();
			for (int i = 0; i < branchOptions.Count; i++)
			{
				// Create a button for each option
				if (GUILayout.Button(branchOptions[i], GUILayout.Height(25 * mult)))
				{
					selectedIndex = i;
					TWDPlayerPrefs.SetString("ActiveBranch", branchOptions[i]);
					TWDPlayerPrefs.Save();

					// Hide the list
					showDropdown = false;
					windowHeight -= 100;
					windowRect = new Rect(windowRect.x, windowRect.y, 270 * mult, windowHeight * mult);
				}
			}
			GUILayout.EndVertical();
			GUI.EndScrollView();
		}
	}

	private void ReloadGameHud()
	{
		IsOfflineMode = false;
		HelpersModel.IsOfflineMode = false;
		IsDodge = false;
		HelpersModel.IsDodge = false;
		IsUnlockPVP = false;
		HelpersModel.IsUnlockPVP = false;

		if (MovementBaseModified)
		{
			MovementBaseModified = false;
			ActivateLongTurn(false);
			SavedDef = new();
		}
		OfflineCommandItems = new();
		SessionToken = "";
		DirectURL = "";
		command_errors_count = 0;
		CommandErrorColor = Color.green;

		ReloadGameDelayed();
	}

	//ход 4 клетки для всех
	void ActivateLongTurn(bool isChange)
	{
		if (playerModel == null || playerModel.Combat == null) return;
		var teamAlt = playerModel.Combat.Survivors;
		var SurvivorUpgradeDefinitions = gameEconomyData.SurvivorUpgradeDefinitions.ToList();

		foreach (ActorModel actor in teamAlt)
		{
			var actorDef = gameEconomyData.GetActorDefinition(actor.ActorDefinitionID);
			var survivor = (SurvivorModel)actor;
			SurvivorUpgradeDefinition originDef = gameEconomyData.GetSurvivorsUpgradeDefinition(SurvivorClasFromStrings(actorDef.Class), actor.Level);

			if (isChange)
			{
				float originValue = originDef.MovementBase;
				var index = SurvivorUpgradeDefinitions.IndexOf(originDef);
				if (!SavedDef.ContainsKey(index) && originValue != 10)
				{
					SavedDef.Add(SurvivorUpgradeDefinitions.IndexOf(originDef), originDef.MovementBase);
				}
				originDef.MovementBase = 10;
				Debug.Log("Увеличили для " + actor.Name + " с " + originValue + " до " + originDef.MovementBase);
				survivor.ConfigureBaseAttributes();
			}
			else
			{
				if (SavedDef.Count > 0)
				{
					int index = SurvivorUpgradeDefinitions.IndexOf(originDef);
					var modifiedValue = originDef.MovementBase;
					originDef.MovementBase = SavedDef[index];
					Debug.Log("Вернули для " + actor.Name + " с " + modifiedValue + " до " + originDef.MovementBase);
					survivor.ConfigureBaseAttributes();
				}
			}
		}
	}

	public void OnClickSkipTurnOverride()
	{
		if (playerModel == null || playerModel.Combat == null) return;

		if (MovementBaseModified)
		{
			MovementBaseModified = false;
			ActivateLongTurn(false);
		}

		var teamAlt = playerModel.Combat.Survivors;
		foreach (ActorModel actor in teamAlt)
		{
			var survivor = (SurvivorModel)actor;
			survivor.EndMovement();
			Debug.Log("End Movement for " + survivor.Name);
		}
	}

	public void OnClickNewTurnOverride()
	{
		if (!PlayerInputManager.Instance) return;

		var survivor = PlayerInputManager.Instance.ControlledActor;
		survivor.MoveRangeConsumed = 0;
		survivor.TurnState = TurnState.Idle;
		survivor.MoveCompleted = false;
		survivor.SecondMoveCompleted = false;
		survivor.AbilityCompleted = false;

		survivor.AllowSecondMoveAfterAbility = false;
		survivor.AdditionalMoveRange = 0;
		survivor.OverwatchedOnTurn = false;

		Debug.Log("New Turn for " + survivor.Name);
	}

	private SurvivorClass SurvivorClasFromStrings(string sclass)
	{
		switch (sclass)
		{
			case "Scout": return SurvivorClass.Scout;
			case "Bruiser": return SurvivorClass.Bruiser;
			case "Warrior": return SurvivorClass.Warrior;
			case "Assault": return SurvivorClass.Assault;
			case "Hunter": return SurvivorClass.Hunter;
			case "Shooter": return SurvivorClass.Shooter;
			default: return SurvivorClass.Scout;
		}
	}

	private void SwitchMod()
	{
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F5))
		{
			IsActivateMod = !IsActivateMod;

			if (IsActivateMod)
			{
				InitSettings();
			}
			Debug.Log(IsActivateMod ? "Show Menu" : "Hide Menu");
		}
	}

	private void OpenModUI()
	{
		IsActivateMod = true;
		var x = windowRect.x;
		var y = windowRect.y;
		windowRect = new Rect(x, y, 270 * mult, windowHeight * mult);

		InitSettings();
		Debug.Log("Show Menu");
	}

	private void CloseModUI()
	{
		IsActivateMod = false;
		var x = windowRect.x;
		var y = windowRect.y;
		windowRect = new Rect(x, y, 20 * mult, 20 * mult);

		Debug.Log("Hide Menu");
	}

	private void InitSettings()
	{
		IsOfflineMode = HelpersModel.IsOfflineMode;
		IsOffThinkingAnalytics = HelpersModel.IsOffThinkingAnalytics;

		if (LocalizationManager.Instance)
		{
			if (LocalizationManager.Instance.CurrentLanguage == "ru")
				Language = LanguageCode.RU;
			else
				Language = LanguageCode.EN;
		}
		selectedIndex = branchOptions.IndexOf(ActiveBranch);
	}

	private void GetRandomChange()
	{
		if (playerModel == null) return;
		var callCount = playerModel.PlayerRandom.CallCount;

		//1-2 миссии-лагерь
		//1-2-3 - зомби
		//1 - снаряжение-лагерь
		if (callCount != playerRandomCallCount || PlayerRandomState.Count == 0 || playerModel.PlayerRandom.State != PlayerRandomState.Last())
		{
			if (PlayerRandomState.Count > 6)
			{
				PlayerRandomState.Clear();
			}
			PlayerRandomState.Add(playerModel.PlayerRandom.State);
			playerRandomCallCount = callCount;
			playerRandomTimes++;
		}
	}

	[ContextMenu("Set Online Mode")]
	public void SetOnlineMode()
	{
		IsOfflineMode = false;
		HelpersModel.IsOfflineMode = false;

		Debug.Log("Set Online Mode");
		SignalRClient.Instance.SetSessionToken(SessionToken);
		SignalRClient.Instance.SetDirectUrl(DirectURL);

		if (OfflineCommandItems.Count > 0)
		{
			foreach (var cmd in OfflineCommandItems)
			{
				SignalRClient.Instance.RequestCommand("Command", cmd.Arg, cmd.Type, null, cmd.Command, cmd.WaitForResponse);
			}
			OfflineCommandItems = new();
		}
	}

	[ContextMenu("Set Offline Mode")]
	public void SetOfflineMode()
	{
		OfflineCommandItems = new();
		SessionToken = SignalRClient.Instance.CurrentSessionToken;
		DirectURL = SignalRClient.Instance.CurrentHostPort;
		SignalRClient.Instance.SetSessionToken(null);
		Debug.Log("Set Offline Mode");

		IsOfflineMode = true;
		HelpersModel.IsOfflineMode = true;
	}

	[ContextMenu("ExecuteError")]
	public void ExecuteError()
	{
		Show_Command_Error(37);
	}

	void UpdateGesture()
	{
		if (IsActivateMod) return;
		Vector2 tmp;
#if UNITY_ANDROID
		if (Input.touchCount > 0)
		{
			tmp = Input.GetTouch(0).position;
			tmp.y = Screen.height - tmp.y;
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				lineList.Clear();
				done = false;
			}
			lineList.Add(tmp);
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				done = true;
				ComputeGestureStats();
			}
		}
#else
		tmp = Input.mousePosition;
		tmp.y = Screen.height - tmp.y;
		if (Input.GetMouseButtonDown(0))
		{
			lineList.Clear();
			done = false;
			lineList.Add(tmp);
		}
		if (Input.GetMouseButtonUp(0))
		{
			lineList.Add(tmp);
			done = true;
			ComputeGestureStats();
		}
		if (Input.GetMouseButton(0))
		{
			lineList.Add(tmp);
		}
#endif
	}

	private void ComputeGestureStats()
	{
		if (lineList.Count > 1)
		{
			vCenter = Vector2.zero;
			for (int i = 0; i < lineList.Count; i++)
			{
				vCenter += lineList[i];
			}

			vCenter /= lineList.Count;

			vRadius = Vector2.zero;
			vRadius.x = 123456.0f;

			Vector2 tmp;
			float len;
			for (int i = 0; i < lineList.Count; i++)
			{
				tmp = lineList[i] - vCenter;
				len = tmp.magnitude;

				vRadius.x = (len < vRadius.x) ? len : vRadius.x;
				vRadius.y = (len > vRadius.y) ? len : vRadius.y;
			}

			Vector2 a = lineList[0] - vCenter;
			fAngle = 0;
			for (int i = 1; i < lineList.Count; i++)
			{
				Vector2 b = lineList[i] - vCenter;

				fAngle += Vector2.Angle(a, b);
				a = b;
			}
			if (fAngle > 360)
			{
				var delta = vRadius.x > vRadius.y ? vRadius.x : vRadius.y;
				var rectDelta = (new Vector2(windowRect.x, windowRect.y) - vCenter).magnitude;
				if (!IsActivateMod && rectDelta < delta)
				{
					isShowButton = true;
				}
			}
		}
	}
	#endregion
}
