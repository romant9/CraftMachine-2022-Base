using BaseModel;
using Client.Connectivity;
using NextGames.Sdk.AssetBundleManager;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

public class OfflineManager : MonoBehaviour
{
    //GOOGLE_SHEET
    //UNITY_POSTGRES

    public static OfflineManager Instance;

	public delegate void OnStreamingChanged();
	public event OnStreamingChanged On_StreamingChanged;

	protected static bool IsInited { get; set; } = false;

	public static bool IsLoadDataManager
	{
		get { return ConfigBuildType != ConfigDataType.Game; }
    }

	public GuildModel CurrentGuildModel => GWTeamUtils.Instance.CurrentGuildModel;
	public PlayerModel Player => GameManager.Instance != null ? GameManager.Instance.playerModel : null;
	public GameEconomyData GameEconomyData => GameManager.Instance != null ? GameManager.Instance.gameEconomyData : null;


	private static MessageSerializer jsonSerializer;
	public static MessageSerializer JsonSerializer
	{
		get
		{
			jsonSerializer ??= new MessageSerializer();
			return jsonSerializer;
		}
	}

	public SignalRClientState State { get; set; } = SignalRClientState.Disconnected;
	public List<OfflineCommandItem> OfflineCommandItems { get; set; } = new();
	//Game. Блокировка любого исходящего трафика. Загрузка профиля оффлайн
	public static bool IsPrivateMode { get; set; } = true;
	public static bool IsResetEpicToken { get; set; } = false;
	//Game. Обратимое действие.
	public static bool IsOfflineMode { get; set; } = false;
	public static bool IsDodge { get; set; } = false;
	public static bool IsUsePortraitManager { get; set; } = true;
	public static bool UseSupabase = false;

	public static string MainSceneName = "myTest_66";
	public static string ClientVersion = "7.20.0.100";
	public static string ShortVersion = "7.20.0";

	public string EosAccountID_Custom { get; set; } //"1c6b97e...";
	public string SessionToken = "";
	public string DirectURL = "";

	public static int PortraitSize = 4;
	public bool ReCreatePortraits = true;

	public CallCountMeter callCountMeter { get; set; }
	public Queue queueRandom = new();

	#region Main System Variables
	/// <summary>
	/// Аналог #if !TWDMOD - в частности своя загрузка EOSLogin
	/// </summary>
	public static bool IsCustomLogin = true;

	public static bool IsLoadFromResources
	{
		get { return AssetBundleManager.IsLoadFromResources; }
		set { AssetBundleManager.IsLoadFromResources = value; }
	}

	public static bool IsMd5Bundles
	{
		get { return AssetBundleManager.IsMd5Bundles; }
		set { AssetBundleManager.IsMd5Bundles = value; }
	}

	public static bool IsIgnoreReconnect = true;
	public static bool IsIgnoreResponseNotOK = true;
	public static bool IsUnlockAll = true;
	public static bool IsFreeAll = false;
	public static bool IsFakeExecuteCommands = true;
	public static bool IsUseSendMetrics = false;
	public static bool IsUseChecksum = false;
	public static bool IsNoEffects = true;
	public static bool IsUseServices = false;
	public static bool IsCommandsOrigin = false;
	public static bool IsCollectDebugString = true;
	public static bool IsDebug = true;
	public static bool IsDebugLocalization = false;
	public static bool IsBundlesLoaded = false;

	public static bool IsLoadBundleModular = true;
	public static bool IsTutorialDisable = true;
	//глобально определить тип сборки с миссиями (большой размер) или нет
	public static bool IsMissionModBuild = true; //fasle
	public static bool IsSaveToClipboard = false;

	public static bool IsBatch = false;

	//Для сборки поставить FALSE
	public static bool IsUseMatFix = true;
	public static bool IsFixModelShaders = true;

	public bool IsReturnToResidence { get; set; }
	//переменные для боя
	public static bool IsCombatGridEnabled = false;

	public string testLogMessage;
	public DebugType testLogType;
	public static ConfigDataType ConfigBuildType { get; private set; }

	public int playerRandomCallCountValue { get; set; }
	public int playerRandomCallCountReset { get; set; }
	public int playerRandomCurrentState { get; set; }

	//проверка через webRequest к google
	public static bool IsInternetOn { get; set; }
	//включать или нет постоянную провеку интернета
	public static bool CheckTimerIsActive = true;
	//переменные для вызовов
	public static bool IsNoAddRewards = true;
	public static LanguageType Language = LanguageType.Ru;

	public UITextList LogPanelList;
	public UITextList StatPanelList;
	public PostProcessingProfile PostProfile
	{
		get { return (PostProcessingProfile)Resources.Load("PPProfile"); }
	}

	public bool IsReconnectPlayerState { get; set; }
	public bool IsReconnectByCode { get; set; }
	public bool IsPlayerLoaded { get; set; }
	public bool IsGedLoaded { get; set; }

	public enum ConnectSource
	{
		Epic,
		Google,
		Steam
	}

	public ConnectSource ConnectSourceCurrent { get; set; } = ConnectSource.Epic;
	public static bool IsGoogleSource => Instance.ConnectSourceCurrent == ConnectSource.Google;
	public event OnSourceChanged OnSourceChangedEvent;
	public delegate void OnSourceChanged();

	public CallCountBase CallCountBasePopup
	{
		get
		{
			if (CallCountBase.Instance == null)
			{
				var go = GameObject.Find("CallCount_On_Top");
				if (go != null)
				{
					return go.GetComponent<CallCountBase>();
				}
				else return null;
			}
			else
			{
				return CallCountBase.Instance;
			}
		}
		set { }
	}
	#endregion

	public enum ConfigDataType
	{
		Pro,
		Light,
		Game
	}

	public enum LanguageType
	{
		Ru,
		En,
		Es
	}

	private void Awake()
	{
		if (IsInited || !this.enabled) return;

		Init();
	}

	public void Start()
	{
		InternetCheckAccess(true);

		MainSceneName = SceneManager.GetSceneByBuildIndex(0).name;

		//client = new HttpClient();
		//client.Timeout = TimeSpan.FromSeconds(10);
	}

	private void OnDestroy()
	{
		IsInited = false;
	}

	public void Init()
	{
		DebugTWD.LogMycode("OfflineManager Awake");

		this.enabled = true;
		IsInited = true;
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);

		SwitchConfigMode(ConfigDataType.Light);
	}

	public static void SwitchConfigMode(ConfigDataType mode)
	{
		switch (mode)
		{
			case ConfigDataType.Pro:
				SetModProSettings();
				break;
			case ConfigDataType.Light:
				SetModLightSettings();
				break;
			case ConfigDataType.Game:
				SetModGameSettings();
				break;
		}
	}

	public void ActivateMethods()
	{
		if (!SignalRClient.Instance)
		{
			DataManager.Instance.GetComponent<SignalRClient>().Init();
		}

		if (!IsLoadFromResources)
		{
			UnityUtils.PreloadAsset("InhabitantsResources", "scriptableobjects");
			UnityUtils.PreloadAsset("ActorsResources", "scriptableobjects");
		}

		if (!DataManager.Instance.IsInited) DataManager.Instance.Init();

		IsBundlesLoaded = true;
	}

	[ContextMenu("Switch Config Pro")]
	public static void SetModProSettings()
	{
		ConfigBuildType = ConfigDataType.Pro;
		IsCustomLogin = true;

		IsPrivateMode = true;
		HelpersModel.IsOffThinkingAnalytics = true;
		IsOfflineMode = true;
		HelpersModel.IsOfflineMode = true;

		IsUsePortraitManager = true;
		IsLoadFromResources = false;
		IsIgnoreReconnect = true;
		IsIgnoreResponseNotOK = true;
		IsUnlockAll = true;
		//IsFreeAll = true;

		IsFakeExecuteCommands = true;
		IsUseSendMetrics = false;
		IsUseChecksum = false;
		IsNoEffects = true;
		IsUseServices = false;
		IsCommandsOrigin = false;
		IsCollectDebugString = true;
		IsDebug = true;
		IsMd5Bundles = true;
		IsLoadBundleModular = true;
		IsTutorialDisable = true;
		IsMissionModBuild = true;

		DebugTWD.Log("Сборка PRO для запуска миссий.", DebugType.System);
	}

	[ContextMenu("Switch Config Game")]
	public static void SetModGameSettings()
	{
		ConfigBuildType = ConfigDataType.Game;
		IsCustomLogin = true;
		IsPrivateMode = true;
		HelpersModel.IsOffThinkingAnalytics = false;
		IsOfflineMode = false;
		HelpersModel.IsOfflineMode = false;

		IsUsePortraitManager = true;
		IsLoadFromResources = false;
		IsIgnoreReconnect = true;
		IsIgnoreResponseNotOK = true;
		IsUnlockAll = true;
		//IsFreeAll = true;

		IsFakeExecuteCommands = false;
		IsUseSendMetrics = true;
		IsUseChecksum = true;
		IsNoEffects = true;
		IsUseServices = true;
		IsCommandsOrigin = true;
		IsCollectDebugString = true;
		IsDebug = true;
		IsMd5Bundles = true;
		IsLoadBundleModular = true;
		IsTutorialDisable = false;
		IsMissionModBuild = true;

		if (TWDPlayerPrefs.HasKey("IsOffThinkingAnalytics"))
		{
			if (bool.TryParse(TWDPlayerPrefs.GetString("IsOffThinkingAnalytics"), out bool result))
			{
				HelpersModel.IsOffThinkingAnalytics = result;
			}
		}
		else
		{
			HelpersModel.IsOffThinkingAnalytics = false;
		}
		DebugTWD.Log("Сборка для запуска игры.", DebugType.System);
	}

	[ContextMenu("Switch Config Light")]
	public static void SetModLightSettings()
	{
		ConfigBuildType = ConfigDataType.Light;
		IsCustomLogin = true;

		IsPrivateMode = true;
		HelpersModel.IsOffThinkingAnalytics = true;
		IsOfflineMode = true;
		HelpersModel.IsOfflineMode = true;

		IsUsePortraitManager = false;
		IsLoadFromResources = true;
		IsIgnoreReconnect = true;
		IsIgnoreResponseNotOK = true;
		IsUnlockAll = true;
		//IsFreeAll = true;

		IsFakeExecuteCommands = true;
		IsUseSendMetrics = false;
		IsUseChecksum = false;
		IsNoEffects = true;
		IsUseServices = false;
		IsCommandsOrigin = false;
		IsCollectDebugString = true;

		IsDebug = Instance != null && Instance.isDebug;

		IsMd5Bundles = false;
		IsLoadBundleModular = false;
		IsTutorialDisable = true;
		IsMissionModBuild = false;

		DebugTWD.Log("Сборка LIGHT для запуска крафта.", DebugType.System);
	}

	[SerializeField]
	private bool isDebug;

	private void OnValidate()
	{
		IsDebug = Instance != null && Instance.isDebug;
	}

	private void Update()
	{
		if (SignalRClient.Instance)
		{
			if (State != SignalRClient.Instance.State)
			{
				State = SignalRClient.Instance.State;
				CallCountBasePopup?.SetSignalRUI(GameManager.Instance.IsLoggedIn);
			}

			if (CallCountBasePopup != null && CallCountBasePopup.SignalRStatusLabelOn != GameManager.Instance.IsLoggedIn)
			{
				State = SignalRClient.Instance.State;
				CallCountBasePopup?.SetSignalRUI(GameManager.Instance.IsLoggedIn);
			}
		}
	}

	private void FixedUpdate()
	{
		if (callCountMeter != null && Player != null && playerRandomCurrentState != Player.PlayerRandom.State)
		{
			playerRandomCurrentState = Player.PlayerRandom.State;
			playerRandomCallCountValue = Player.PlayerRandom.CallCount - playerRandomCallCountReset;

			if (queueRandom.Count > 3)
			{
				queueRandom.Dequeue();
			}
			queueRandom.Enqueue(playerRandomCurrentState);
			callCountMeter.SetValueImmediate(playerRandomCallCountValue);
		}
	}

	public void SetPrivate(bool isPrivate)
	{
		if (isPrivate)
		{
			HelpersModel.IsOffThinkingAnalytics = true;
			IsPrivateMode = true;
			IsOfflineMode = true;
		}
		else
		{
			HelpersModel.IsOffThinkingAnalytics = false;
			IsPrivateMode = false;
			IsOfflineMode = false;
		}
	}

	[ContextMenu("Reset Random Counter")]
	public void ResetRandomCounter()
	{
		if (Player != null)
		{
			playerRandomCallCountReset = Player.PlayerRandom.CallCount;
			playerRandomCallCountValue = 0;
			callCountMeter.SetValueImmediate(playerRandomCallCountValue);
		}
	}

	public void SetResetEpicTokenValue(bool value)
	{
		IsResetEpicToken = value;
	}

	public void SwitchAnalytics(bool isOn)
	{
		HelpersModel.IsOffThinkingAnalytics = !isOn;
		TWDPlayerPrefs.SetString("IsOffAnalyticsManager", HelpersModel.IsOffThinkingAnalytics.ToString());
		TWDPlayerPrefs.Save();
	}

	public void SwitchDodge(bool isOn)
	{
		IsDodge = isOn;
	}

	public void ShowModPopup()
	{
		if (IsLoadDataManager)
		{
			ShowRandomValuesPopup();
		}
		else
		{
			ShowModdingPopup();
		}
	}
	public void ShowRandomValuesPopup(GameObject parent = null)
	{
		if (Player == null) return;
		RandomValuesPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RandomValuesPopup, parent) as RandomValuesPopup;
		if (obj != null)
		{
			if (obj.IsOpen)
			{
				obj.OnClickClose();
			}
			else
			{
				obj.transform.localScale = parent == null ? Vector3.one : Vector3.one * .7f;
				obj.Open();
			}
		}
	}

	public void ShowModdingPopup()
	{
		if (GameManager.Instance == null || Player == null) return;

		ModdingPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ModdingPopup) as ModdingPopup;
		if (obj != null)
		{
			if (obj.IsOpen)
			{
				obj.OnClickClose();
			}
			else
			{
				obj.Open();
			}
		}
	}

	public void SaveSignalRData(string token, string url)
	{
		SessionToken = token;
		DirectURL = url;
	}

	public void SetStreamingPath()
	{
		FileBrowser.ShowLoadDialog(DefineStreamingDirectory, OnCancelDirectoryDialog, FileBrowser.PickMode.Folders, initialPath: Application.streamingAssetsPath);
		UICamera.ignoreAllEvents = true;
	}

	private void DefineStreamingDirectory(string[] paths)
	{
		if (paths != null && paths.Length > 0)
		{
			var streamingPath = paths.First();
			AssetBundleManager.StreamingAssetsPath = streamingPath;
			TWDPlayerPrefs.SetString(UserPrefsKeys.Key_StreamingAssetsPath, streamingPath);

			DebugTWD.Log("FileBrowser Defined SteramingAssets: " + string.Join(", ", paths), DebugType.Load);
			On_StreamingChanged?.Invoke();
		}
		UICamera.ignoreAllEvents = false;
	}

	private void OnCancelDirectoryDialog()
	{
		UICamera.ignoreAllEvents = false;
	}

	[ContextMenu("Set Online Mode")]
	public void SetOnlineMode()
	{
		if (IsOfflineMode && SignalRClient.Instance != null)
		{
			IsOfflineMode = false;
			DebugTWD.Log("Set Online Mode", DebugType.SignalR);
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
			//StartCoroutine(GameManager.Instance.SendModelSnapshotAndReload());
		}
	}

	[ContextMenu("Set Offline Mode")]
	public void SetOfflineMode()
	{
		if (!IsOfflineMode && SessionToken != SignalRClient.Instance.CurrentSessionToken)
		{
			OfflineCommandItems = new();
			SaveSignalRData(SignalRClient.Instance.CurrentSessionToken, SignalRClient.Instance.GetDirectUrl());
			SignalRClient.Instance.SetSessionToken(null);
			DebugTWD.Log("Set Offline Mode", DebugType.SignalR);
			IsOfflineMode = true;
		}
	}

	[ContextMenu("Reload Game")]
	public void ReloadGame()
	{
		OfflineCommandItems = new();
		SaveSignalRData(null, null);
		GameManager.Instance.ReloadGame();
	}

	[ContextMenu("Set Test Log String")]
	public void SetTestLogString()
	{
		DebugTWD.Log(testLogMessage, testLogType);
	}

	public void SetConnectSource(UIPopupList list)
	{
		int index = list.items.IndexOf(list.value);
		ConnectSourceCurrent = (ConnectSource)index;
		DebugTWD.Log("Connection source set to " + ConnectSourceCurrent);

		OnSourceChangedEvent?.Invoke();
	}

	#region InternetCheck
	private Coroutine TimedContentCheckCoroutine;

	public void InternetCheckAccess(bool isActive)
	{
		CheckTimerIsActive = isActive;

		if (isActive == true)
		{
			if (TimedContentCheckCoroutine != null)
			{
				StopCoroutine(TimedContentCheckCoroutine);
			}
			TimedContentCheckCoroutine = StartCoroutine(InternetAccessCheck());
		}
		else TimedContentCheckCoroutine = null;
	}

	private IEnumerator InternetAccessCheck()
	{
		while (CheckTimerIsActive)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable)
			{
				SetInternetAccess(true);

				//var task = CheckIfOnline();
				//while (!task.IsCompleted)
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(20f);
			}
			else
			{
				SetInternetAccess(false);
				yield return new WaitForSeconds(20f);
			}
			yield return null;
		}
	}

	public async Task<bool> CheckIfOnline()
	{
		var url = "https://www.google.com/";
		//Uri.TryCreate(url.Trim(), UriKind.Absolute, out Uri result);

		using HttpClient client = new HttpClient();
		client.Timeout = TimeSpan.FromSeconds(10);
		bool isSuccess = true;

		try
		{
			var result = await client.GetAsync(url);
			result.EnsureSuccessStatusCode();
		}
		catch (Exception ex)
		{
			Debug.LogError($"Ошибка HTTP запроса {url}: {ex.Message}");
			isSuccess = false;
		}
		SetInternetAccess(isSuccess);
		return isSuccess;

		//string HtmlLookUpResult_Content = string.Empty;
		//HttpWebRequest HtmlLookUpResult_Request = (HttpWebRequest)WebRequest.Create(result);
		//try
		//{
		//	using (HttpWebResponse HtmlLookUpResult_Response = (HttpWebResponse)HtmlLookUpResult_Request.GetResponse())
		//	{
		//		bool HtmlLookUpResult_isSuccess = (int)HtmlLookUpResult_Response.StatusCode < 299 && (int)HtmlLookUpResult_Response.StatusCode >= 200;
		//		if (HtmlLookUpResult_isSuccess)
		//		{
		//			using (StreamReader HtmlLookUpResult_Reader = new StreamReader(HtmlLookUpResult_Response.GetResponseStream()))
		//			{
		//				var HtmlLookUpResult_Chars = new char[1];
		//				HtmlLookUpResult_Reader.Read(HtmlLookUpResult_Chars, 0, 1);
		//				HtmlLookUpResult_Content += HtmlLookUpResult_Chars[0];
		//			}
		//		}
		//	}
		//}
		//catch
		//{
		//	HtmlLookUpResult_Content = null;
		//}
		//SetInternetAccess(!string.IsNullOrEmpty(HtmlLookUpResult_Content));
	}

	private void SetInternetAccess(bool isOn)
	{
		if (IsInternetOn != isOn)
		{
			if (isOn == true)
			{
				//Internet is Active.
				DebugTWD.Log("Enabling Internet related content.");
				CallCountBasePopup?.SetInternetUI(true);
			}
			else
			{
				//Internet is NOT Active.
				DebugTWD.Log("Disabling Internet related content.");
				CallCountBasePopup?.SetInternetUI(false);
			}
			IsInternetOn = isOn;
		}
	}
	#endregion
}

public class OfflineCommandItem
{
	public string Arg { get; set; }
	public string Type { get; set; }
	public IModelCommand Command { get; set; }
	public bool WaitForResponse { get; set; }

	public OfflineCommandItem(string arg, string type, IModelCommand cmd, bool wait)
	{
		Arg = arg;
		Type = type;
		Command = cmd;
		WaitForResponse = wait;
	}

	public OfflineCommandItem()
	{
	}
}