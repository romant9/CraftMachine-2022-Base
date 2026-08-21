#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
#define MOBILE
#endif
using BaseModel;
using Client.Connectivity;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using PlayEveryWare.EpicOnlineServices;
using Steamworks;
using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TwdCustomMod;
using UnityEngine;
using static OfflineManager;

public class GetPlayerData : MonoBehaviour
{
	public static GetPlayerData Instance { get; private set; }

	public bool waitingLogin { get; private set; }

	public bool waitingPlayer { get; private set; }

	public bool waitingGed { get; set; }

	public bool waitingImages { get; private set; }

	public bool waitingGuild { get; private set; }

	public static string CurrencyCode = string.Empty;

	private LoginRequest loginRequest;

	//public static string UserId => TWDPlayerPrefs.GetString("UserId");

	//old link : "https://twd.drillerservices.com";
	//"https://g02-d05-prd-f35-ng.drillerservices.com:10102" - 23.05.2024
	//https://g02-d05-prd-f04-ng.drillerservices.com:10103 - 24.05.2024
	//public const string Url = "https://g02-d05-prd-f04-ng.drillerservices.com:10103";

	//public const string Url = "https://twd-old.drillerservices.com";
	//public const string DataURL = "https://backup-twd.drillerservices.com";

	public static string userAccountName { get; set; }

	private EOSManager eosManager;
	private SignalRClient signalRC;

	private bool IsSteamInited;
	public UIPopupList SourcesList;

	public GameCenterManager GameCenterManager { get; private set; }

	private ConnectSource connectSource => OfflineManager.Instance.ConnectSourceCurrent;


    private void Awake()
	{
		Instance = this;
		OfflineManager.Instance.OnSourceChangedEvent += OnSourceChangedResult;
	}

	private void OnSourceChangedResult()
	{
		CraftSettings.Instance.ResetEpicLoginToggle.enabled = connectSource == ConnectSource.Epic;

		if (OfflineManager.IsGoogleSource)
		{
			GameCenterManager ??= new GameCenterManager();
		}
		else
		{
			GameCenterManager.Disconnect();
			GameCenterManager = null;
		}
		if (OfflineManager.IsInternetOn && OfflineManager.Instance.ConnectSourceCurrent != ConnectSource.Steam)
		{
			var iapManager = GameManager.Instance.IAPManager;

			if (iapManager != null) Destroy(iapManager.gameObject);
			GameManager.Instance.SetIAPManager();
		}
	}

	private void Start()
	{
#if MOBILE
		//SourcesList.GetComponent<Collider>().enabled = false;
		SourcesList.RemoveItem("Steam");
#endif
		WaitEosManager();
	}

	private void OnDisable()
	{
		if (EOSManager.Instance != null)
		{
			DebugTWD.Log("Epic is not null, ShutDown", DebugType.System);
			//EOSManager.Instance.OnShutdown();
		}
	}

	private async void WaitEosManager()
	{
		while (EOSManager.Instance == null)
		{
			await Task.Yield();
		}
		var statusNet = EOSManager.Instance.GetEOSPlatformInterface().GetNetworkStatus();

		if (statusNet != Epic.OnlineServices.Platform.NetworkStatus.Online)
		{
			return;
		}
		while (EOSLogin.GetAccountUserId() == null)
		{
			await Task.Yield();
		}
		EpicAccountId playerID = EOSLogin.GetAccountUserId();
		var statusLogin = EOSManager.Instance.GetEOSAuthInterface().GetLoginStatus(playerID);
		var token = EOSManager.Instance.GetUserAuthTokenForAccountId(playerID);
		if (token != null)
		{
			var tokenContent = token.Value;
		}
	}

	private IEnumerator DoConnectForLogin()
	{
		var loading = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading) as IngameLoading;
		if (loading != null)
		{
			loading.Open();
			loading.SetText("Загружаем ваш игровой профиль. Ждите...", false);
		}
		//IngameLoading loading = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading) as IngameLoading;
		//loading.SetText("Загружаем ваш игровой профиль. Ждите...", false);
		//loading.Open();

		OfflineManager.Instance.IsPlayerLoaded = false;
		signalRC = GetComponent<SignalRClient>();

		if (connectSource == ConnectSource.Epic)
		{
			eosManager = GetComponent<EOSManager>();
			if (!eosManager.enabled)
			{
				DebugTWD.Log("Epic Init and Enabled", DebugType.System);
				eosManager.enabled = true;
				yield return new WaitForSeconds(.5f);
			}
			if (EOSManager.Instance == null)
			{
				yield return new WaitUntil(() => EOSManager.Instance != null);
			}
		}
		else if (connectSource == ConnectSource.Steam)
		{
			DebugTWD.Log("Init Steam", DebugType.System);
			try
			{
				Steamworks.SteamClient.Init(2936310);
				var playername = SteamClient.Name;
				var playersteamid = SteamClient.SteamId;
				//var appId = SteamClient.AppId;

				DebugTWD.Log("player : " + playername);
				DebugTWD.Log("Id : " + playersteamid);
			}
			catch (System.Exception e)
			{
				DebugTWD.LogException(e);
				yield break;
			}

			IsSteamInited = true;
			//DataManager.Instance.SteamManager.SetActive(true);
			//SteamAPI.Init();
			yield return new WaitForSeconds(.5f);
		}
		else
		{
			DebugTWD.Log("Init GooglePlayGames", DebugType.System);
		}

		if (!signalRC.enabled)
		{
			signalRC.enabled = true;
			if (SignalRClient.Instance == null)
			{
				signalRC.Init();
			}
			yield return new WaitForSeconds(.5f);
		}

		if (CommandHelper.Instance.IsPrelogin)
		{
			signalRC.IsOnlyGedData = false;

            signalRC.IsOnlyGetImagesData = true;
            signalRC.IsOnlyGetPlayersData = true;
		}
		else
		{
            signalRC.IsOnlyGedData = !CommandHelper.Instance.IsUseCustomID;

            signalRC.IsOnlyGetImagesData = true;
            signalRC.IsOnlyGetPlayersData = true;
		}
		try
		{
			SignalRClient.Instance.Connect(DataManager.DataURL, OnSignalRConnectForLogin);
		}
		catch (WebException ex)
		{
			DebugTWD.LogError("Ошибка подключения:\n" + ex.Message);
		}
	}

	public IEnumerator DoConnectForGuild()
	{
		signalRC = GetComponent<SignalRClient>();

		if (signalRC.enabled && signalRC.IsConnected) yield break;

		if (!signalRC.enabled || SignalRClient.Instance == null)
		{
			signalRC.enabled = true;
			yield return new WaitUntil(() => SignalRClient.Instance != null);

			if (connectSource == ConnectSource.Epic)
			{
				eosManager = GetComponent<EOSManager>();
				if (!eosManager.enabled || EOSManager.Instance == null)
				{
					eosManager.enabled = true;
					yield return new WaitUntil(() => EOSManager.Instance != null);
				}
			}
			else if (connectSource == ConnectSource.Steam)
			{
				DebugTWD.Log("Init Steam", DebugType.System);

				try
				{
					Steamworks.SteamClient.Init(2936310);
					var playername = SteamClient.Name;
					var playersteamid = SteamClient.SteamId;
					//var appId = SteamClient.AppId;

					DebugTWD.Log("player : " + playername);
					DebugTWD.Log("Id : " + playersteamid);
					//Debug.Log("appId : " + appId);
				}
				catch (System.Exception e)
				{
					DebugTWD.LogException(e);
					yield break;
				}

				IsSteamInited = true;
				//DataManager.Instance.SteamManager.SetActive(true);
				//SteamAPI.Init();
				yield return new WaitForSeconds(.5f);
			}
			else
			{
				DebugTWD.Log("Init GooglePlayGames", DebugType.System);
			}
		}

		if (OfflineManager.Instance.IsGedLoaded)
		{
			signalRC.IsOnlyGedData = false;
			signalRC.IsOnlyGetPlayersData = false;
		}
		signalRC.IsOnlyGetImagesData = true;

		signalRC.Connect(DataManager.DataURL, OnSignalRConnectForLogin);
	}

	private void OnSignalRConnectForLogin(string status)
	{
		DebugTWD.Log("SignalR Connection status : " + status);
		if (status == "connected")
		{
			string textRu = "Успешно подключились к игровому серверу!";
			string textEn = "Successfully connected the game server!";
			MyTools.UpdateLogPanel(textEn, textRu);

			if (connectSource == ConnectSource.Epic)
			{
				EOSLogin.Login(LoginCallback);
			}
			else
			{
				StartCoroutine(MainLogin());
			}
		}
		else
		{
			string textRu = "Не могу подключиться! Переключаю игровой сервер. Попробуйте переподключиться.";
			string textEn = "Couldn't connect! The game server has been switched. Try reconecting.";
			MyTools.UpdateLogPanel(textEn, textRu);

			DebugTWD.LogWarning("SignalR Disconnected");
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);

			DataManager.ToggleBackupServer();
		}
	}

	private void LoginCallback(ProductUserId productUserId)
	{
		if (productUserId != null)
		{
			DebugTWD.Log("productUserId : " + productUserId.ToString());
			EOSLogin.SaveEpicToken();
		}
		else
		{
			DebugTWD.Log("productUserId is NULL");
		}
		StartCoroutine(MainLogin());
	}

	void Update()
	{
		if (IsSteamInited)
		{
			Steamworks.SteamClient.RunCallbacks();
		}
	}

	//OnClick альтернативно
	public void OnClickConnectEpic()
	{
		CraftSettings.Instance.CheckInternetStatus();
		if (!OfflineManager.IsInternetOn || (DataManager.Instance.IsLocalPlayer && !DataManager.Instance.IsReged))
		{
			string textRu = "Пользователь не зарегистрирован. Загрузка аккаунта в локальной сесии запрещена!";
			string textEn = "User is not registered/ Loading the account in the local session is prohibited!";

			MyTools.UpdateLogPanel(textEn, textRu);
            return;
		}

		if (DataManager.Instance.IsReconnectPlayerState || DataManager.Instance.IsReconnectByCode)
		{
			BadgeCraft.Instance.Reset();
			CallCraft.Instance.Reset();
			GWTeamUtils.Instance.Reset();
		}
		StartCoroutine(DoConnectForLogin());
	}

	public void OnClickGetGuild()
	{
		waitingGuild = true;

		CraftSettings.Instance.CheckInternetStatus();
		if (!OfflineManager.IsInternetOn || (DataManager.Instance.IsLocalPlayer && !DataManager.Instance.IsReged))
		{
			string textRu = "Пользователь не зарегистрирован. Загрузка гильдий в локальной сесии запрещена!";
			string textEn = "User is not registered/ Loading the guilds in the local session is prohibited!";

			MyTools.UpdateLogPanel(textEn, textRu);
			waitingGuild = false;
			return;
		}
		StartCoroutine(DoConnectForGuild());
	}

	public void OnClickGetData()
	{
		StartCoroutine(DoConnectForLogin());
	}

	public IEnumerator MainLogin()
	{
		DebugTWD.Log("MainLogin");
		loginRequest = InitializeLoginRequest();

		waitingGed = true;
		waitingPlayer = true;

		var userID = UserPrefsKeys.UserId;
		if (!string.IsNullOrEmpty(userID))
		{
			loginRequest.Identification = userID; //G02-D05-db8da192...
			DebugTWD.Log("PlayerPrefs GoogleID is " + userID);
		}

		var customEosID = CommandHelper.Instance.customUserEosID;
        if (CommandHelper.Instance.IsUseCustomID && !string.IsNullOrEmpty(customEosID) && !CommandHelper.Instance.IsPrelogin)
		{
			loginRequest.Identification = customEosID;
			DebugTWD.Log("CustomUserEosID is " + customEosID);
			CommandHelper.Instance.IsUseCustomID = false;
		}

		loginRequest.ModelChecksum = "";
		loginRequest.CurrentDateStamp = Helpers.DateTimeToUnixTime(DateTime.UtcNow);
		loginRequest.InstallLaunchCount = 1L;
		loginRequest.LastSessionDateStamp = 0L;

		string request = JsonSerializer.Serialize(loginRequest, true);

		if (DataManager.Instance.DoSaveRequest)
		{
			string path = DataManager.PlayerContentFolder + "request.json";
			DebugTWD.Log("Save request to: " + path, DebugType.Connection);
			MyTools.SaveToFile(request, path, append: false);
		}

		waitingLogin = true;

		DebugTWD.Log("Login request :\n" + request);
		DebugTWD.Log("Identification request is " + loginRequest.Identification); //G02-D05-db8da192...
		DebugTWD.Log("PcPlatform.PcAccountId is " + loginRequest.PcPlatform.PcAccountId); //1c6b97e39423...

		SignalRClient.Instance.RequestCommand("Login", request, OnLogin, waitForResponse: false);

		float startTime = Time.realtimeSinceStartup;
		while (waitingLogin)
		{
			if (Time.realtimeSinceStartup - startTime > 30f)
			{
				DebugTWD.LogWarning("Login timeout");
				waitingLogin = false;
				waitingGuild = false;
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
				yield break;
			}
			yield return null;
		}
	}

	public void OnLoadGed(string url)
	{
		if (DataManager.Instance.IsGedCached && DataManager.Instance.IsLoadLocalGed)
		{
			DataManager.Instance.LoadGameEconomyData();
		}
		else
		{
			//GetGedAsync(url);
			if (!DataManager.Instance.IsVpnON)
			{
				GetGedFromGoogle(url);          
			}
			else
			{
				TryExtractChecksumFromUrl(url, out string extractedChecksum);
				ContentManager.Instance.GetCDNContent<string>(url, "GED", "GameEconomyData", OnGameEconomyData, extractedChecksum);
			}
		}
	}

	public void GetGedFromGoogle(string url)
	{
		DataManager.Instance.GoogleSheetManager.GetGedFromGoogle(url, OnGameEconomyData);
	}

	private async void GetGedAsync(string url, Action<string> contentCallback)
	{
        using HttpClient _httpClient = new HttpClient();
        var ged = await _httpClient.GetStringAsync(url);
		DebugTWD.Log("Cкачали ged");
		contentCallback(ged);
	}

    public static bool TryExtractChecksumFromUrl(string url, out string extractedChecksum)
	{
		extractedChecksum = null;
		if (string.IsNullOrEmpty(url))
		{
			return false;
		}
		int num = url.LastIndexOf('/');
		if (num < 0)
		{
			DebugTWD.LogWarning("Invalid content url: " + url);
			return false;
		}
		num++;
		int num2 = url.IndexOf('.', num);
		int length = ((num2 >= 0) ? num2 : url.Length) - num;
		extractedChecksum = url.Substring(num, length);
		if (!ContentCache.CheckIsValidChecksum(extractedChecksum))
		{
			DebugTWD.LogWarning("Invalid content checksum " + extractedChecksum + " as extracted from url " + url);
			extractedChecksum = null;
			return false;
		}
		return true;
	}

	private void OnGameEconomyData(string gameEconomyDataJson)
	{
		if (string.IsNullOrEmpty(gameEconomyDataJson))
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);

			string logEn = "Cant't load GED from server. Try load it locally (Switch Local GED)";
			string logRu = "Не могу скачать GED с сервера. Попробуйте загрузить его локально (переключите локальный GED)";
			MyTools.UpdateLogPanel(logEn, logRu);
			DebugTWD.Log(logEn);
		}
		else
		{
			waitingGed = true;
			StartCoroutine(DataManager.Instance.StartGameEconomyData(gameEconomyDataJson));
		}
	}

	public void OnLoadPlayer(string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			json = ContentManager.Instance.GetCache("Player").GetContentById<string>("PlayerModel");
			DataManager.Instance.contentSource = ContentSource.Local;
		}
		else
		{
			ContentManager.Instance.GetCache("Player").SetContent("PlayerModel", null, null, json);
			DataManager.Instance.contentSource = GetContentSource(connectSource);
		}

		StartCoroutine(DataManager.Instance.LoadPlayerContent(json));
	}

	public IEnumerator SetEosManagerDisable(int time)
	{
		yield return new WaitForSeconds(time);
		if (connectSource == ConnectSource.Epic) eosManager.enabled = false;
		else if (SteamClient.IsLoggedOn) SteamClient.Shutdown();
		DebugTWD.LogWarning("EosManager set Disable!");
	}

	private void OnLogin(string loginResponseJson)
	{
		//{"Identification":"G02-D05-a11478e6-a47b-4ef9-9824-b32bceab0d6b","Address":"https://g02-d05-prd-f01-ng.drillerservices.com:10103","State":0,"SessionToken":"9d6753d7f7b74ec4aebbd5d093b3e01d","LockState":null,"Maintenance":null,"currency":null}

		DebugTWD.Log("Login Response :\n" + loginResponseJson);

		if (string.IsNullOrEmpty(loginResponseJson))
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
			return;
		}
		LoginResponse loginResponse = JsonSerializer.Deserialize<LoginResponse>(loginResponseJson);

		if (DataManager.Instance.DoSaveRequest)
		{
			string path = DataManager.PlayerContentFolder + "response.json";
			DebugTWD.Log("Save response to: " + path, DebugType.Connection);
			MyTools.SaveToFile(loginResponseJson, path, append: false);
		}
		if (connectSource == ConnectSource.Steam)
		{
			DebugTWD.Log("Connect to Steam", DebugType.Connection);

			//CurrencyCode = loginResponse.currency;
			//SteamStore.InitializeProducts();
		}

		string identification = loginResponse.Identification;
		if (!string.IsNullOrEmpty(identification))// && identification == loginRequest.Identification)
		{
			TWDPlayerPrefs.SetString("UserId", identification);

			UserPrefsKeys.Player_GoogleID = identification;
			UserPrefsKeys.Player_EpicAccountID = loginRequest.PcPlatform.PcAccountId;
			UserPrefsKeys.UserAccountName = userAccountName;
			PlayerPrefs.Save();
		}
		DebugTWD.Log("UserAccountName : " + userAccountName, DebugType.Connection);
		DebugTWD.Log("Identification Response: " + identification, DebugType.Connection);

		//var direct = "https://backup-twd.drillerservices.com"; //loginResponse.Address

		SignalRClient.Instance.SetSessionToken(loginResponse.SessionToken); //da780ad0201646b2ad320673d054d801
        SignalRClient.Instance.SetDirectUrl(loginResponse.Address);//https://g02-d05-prd-f05-ng.drillerservices.com:10100

        waitingLogin = false;
		waitingGuild = false;

		CommandHelper.Instance.IsPrelogin = false;
	}

	public void CreateInstallationId()
	{
		if (string.IsNullOrEmpty(UserPrefsKeys.InstallationID))
		{
			UserPrefsKeys.InstallationID = Guid.NewGuid().ToString();
			TWDPlayerPrefs.Save();
		}
	}

	private LoginRequest InitializeLoginRequest()
	{
		CreateInstallationId();
		DeviceInfo device = new DeviceInfo
		{
			CountryCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
			Device = SystemInfo.deviceModel,
			//Platform = "WindowsPlayer",
			Platform = Application.platform.ToString(),
			OsVersion = SystemInfo.operatingSystem,
			AdvertisingIdentifier = "",
			GraphicsDeviceName = SystemInfo.graphicsDeviceName,
			GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion,
			DeviceId = SystemInfo.deviceUniqueIdentifier,
			DeviceModelId = "",
			TotalMemory = SystemInfo.systemMemorySize + SystemInfo.graphicsMemorySize,
			GraphicsMemory = SystemInfo.graphicsMemorySize
		};
		BaseModel.TDPresetProperties tDPresetProperties = new TDPresetProperties
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
		tDPresetProperties.Channel = GetChannel(connectSource);
#if !TWDMOD
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
				tDPresetProperties.Channel = "epic";
			}
		}
#endif
		userAccountName = GetPlayerName(connectSource); //BloodymaryHere

		PcPlatform pcPlatform = new ();
		if (connectSource == ConnectSource.Epic)
		{
			pcPlatform.PcPlatformType = AccountType.WindowsEditor;
			pcPlatform.PcAccountId = EOSLogin.GetAccountUserId().ToString(); //1c6b
			pcPlatform.PcAccessToken = EOSLogin.GetAccessToken();
			pcPlatform.PcRefreshToken = EOSLogin.GetRefreshToken();
		}
		else if (connectSource == ConnectSource.Steam)
		{
			pcPlatform.PcPlatformType = AccountType.Steam;
			pcPlatform.PcAccountId = SteamClient.SteamId.ToString();
			pcPlatform.PcAccessToken = "";
			pcPlatform.PcRefreshToken = "";
		}
		else if (connectSource == ConnectSource.Google)
		{

		}

		pcPlatform.Data = new()
		{
			{ "SocialAccountName", userAccountName }
		};

		return new LoginRequest
		{
			ClientVersion = OfflineManager.ClientVersion,
			ClientModelVersion = DataManager.ClientModelVersion(OfflineManager.ClientVersion),
			InstallationId = UserPrefsKeys.InstallationID,
			BuildId = "Local build",
			InstallDateStamp = 0L,
			LicenseValidationStatus = 0,
			Device = device,
			TDPresetProperties = tDPresetProperties,
			// для Google PсPlatform рекомендуется отключить
			PcPlatform = pcPlatform
		};
	}

	public static string GetPlayerName(ConnectSource source)
	{
		return source switch
		{
			ConnectSource.Epic => EOSLogin.GetUserDisplayName(),
			ConnectSource.Google => Social.Active.localUser.userName,
			ConnectSource.Steam => SteamClient.Name,
			_ => string.Empty
		};
	}

	public static string GetChannel(ConnectSource source)
	{
		return source switch
		{
			ConnectSource.Epic => "epic",
			ConnectSource.Google => "googleplay",
			ConnectSource.Steam => "steam",
			_ => string.Empty
		};
	}

	public static ContentSource GetContentSource(ConnectSource source)
	{
		return source switch
		{
			ConnectSource.Epic => ContentSource.Epic,
			ConnectSource.Google => ContentSource.Google,
			ConnectSource.Steam => ContentSource.Steam,
			_ => ContentSource.Local
		};
	}
}
