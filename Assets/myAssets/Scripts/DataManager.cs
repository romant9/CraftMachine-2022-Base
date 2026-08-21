#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
#define MOBILE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWDModel;
using System.IO;
using System.Linq;
using System;
using UnityEngine.Networking;
using PlayEveryWare.EpicOnlineServices;
using System.Net;
using BaseModel;
using Client.Connectivity;
using Supabase.TWD;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using TaskStatus = UnityAuth.TaskStatus;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TwdCustomMod
{
	[DefaultExecutionOrder(-1010)]
	public partial class DataManager : MonoBehaviour
	{
		private class IpApiData
		{
			public string country_code;

			public static IpApiData CreateFromJson(string jsonString)
			{
				return JsonUtility.FromJson<IpApiData>(jsonString);
			}
		}

		public static DataManager Instance;

		public enum Language
		{
			Ru,
			En,
			Es
		}

		public Language language = Language.Ru;

		public ContentSource contentSource = ContentSource.Epic;

		public string languageKey = "ru";

		public static string PlayerConfigPath;

		public static string GameDataFolder
		{
			get
			{
				//return GetFolder("/Download/CraftMachine/");
				//return GetFolder(Application.persistentDataPath + "/CraftMachine/");
				return Application.persistentDataPath + '/';
			}
		}

		//resources
		public static string SettingsJSON
		{
			get { return "Settings/settings"; }
		}

		//ContentCash
		public static string GedPathFolder
		{
			get { return "ged/ged"; }
		}

		public static string LocalizationPathFolder(string langID)
		{
			return $"Localization/SourceFiles/{langID}";
		}

		public static string CustomLocalizationPathFolder
		{
			get { return GetFolder(GameDataFolder + "Localization/"); }
		}

		public static string PlayerSSFolder
		{
			get { return GetFolder(GameDataFolder + "Screenshots/"); }
		}

		public static string PlayerEpicFolder
		{
			get { return GetFolder(GameDataFolder + "EOS/"); }
		}

		public static string PlayerDataFolder
		{
			get { return GetFolder(GameDataFolder + "PlayerData/"); }
		}

		public static string PlayerContentFolder
		{
			get { return GetFolder(GameDataFolder + "Content/"); }
		}

		public static string PlayerRequestFolder
		{
			get { return GetFolder(GameDataFolder + "Request/"); }
		}

		public static string PlayerBadgesFolder
		{
			get { return GetFolder(GameDataFolder + "Badges/"); }
		}

		public static string GetFolder(string path)
		{
			string folder = path;
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			return folder;
		}

		public static string IdentLastFile(string folder)
		{
			DirectoryInfo directory = new DirectoryInfo(folder);
			var files = directory.GetFiles().ToList();
			var nowDate = DateTime.Now;
			if (files == null || files.Count == 0) return null;
			var file = files.Where(file => file.Name.Count() > 10).OrderBy(file => file.LastWriteTime).Last();
			return file.FullName;
		}

		public static int GetCurrencyCount
		{
			get { return (int)CurrencyType.Count; }
		}

		public static int GetCurrencyAmount(CurrencyType currency)
		{
			return Instance.Player != null ? Instance.Player.GetCurrency(currency).Value : 0;
		}

		public GameEconomyData GameData { get; private set; }
		public TWDModelManager ModelManager { get; private set; }
		public PlayerModel Player { get; private set; }

		public GuildManager GuildManager { get; protected set; }
		public PlayerHubManager PlayerHubManager { get; protected set; }
		public Metrics.BundleSource BundleSource { get; set; }

		private CachedLeaderboardsManager cachedLeaderboardsManager;
		public CachedLeaderboardsManager CachedLeaderboardsManager
		{
			get
			{
				cachedLeaderboardsManager ??= new CachedLeaderboardsManager();
				return cachedLeaderboardsManager;
			}
		}
		public event LoadCompleted OnLoadCompleted;
		public void NotifyLoadCompleted()
		{
			DebugTWD.Log("DataManager OnLoadCompleted Invoke");
			OnLoadCompleted?.Invoke();
		}

		public bool IsUseBorder;
		public bool IsUseLastReload;
		public bool IsUseFirstTenLucky;

		public int ResidenceLevel { get; private set; }
		public int RadioTentLevel { get; private set; }

		public SocialPopupGuild guildPopup;
		public SurvivorManagementPopUp SurvivorManagementPopUp; //HUDManager.Instance.IsOpen(UIType.CampSurvivorInfoPopup)
		public ResidenceCraftBadgeTab craftTab;
		public ResidenceBadgeInventoryTab InventoryTab;
		public PortraitManager portraitManager;

		private MessageSerializer jsonSerializer;
		private BadgeCraft badgeCraft;
		private CallCraft callCraft;
		[SerializeField]
		private CraftSettings craftSettings;
		//private SaveLogToSheet saveLogToSheet;
		private GetPlayerData GetData;
		private GWTeamUtils gWTeamUtils;
		private LocalizationManager localizationManager;
		private PlayerRandomValues playerRandomValues;
		private ResidencePopup residencePopup;
		private NewPhonePopup newPhonePopup;
		private SupabaseManager supabaseManager;
		public DatabaseManager DatabaseManager { get; private set; }

		private bool IsDataManagerInitialized;
		public List<BadgeInfo> PlayerBadges { get; private set; }

#if UNITY_EDITOR
		[MenuItem("Tools/Reload Domain")]
		public static void ReloadDomain()
		{
			EditorUtility.RequestScriptReload();
		}
#endif
		//сохранять ли json после авторизации EOS_Login
		//true - Player будет загружаться из файла, false - загружаться сразу из кэша
		public bool DoSavePlayerJson = true;
		//сохранять request при EOS_Login
		public bool DoSaveRequest = false;
		public bool IsNetPath = true;

		public int InitState { get; set; }

		//public static string ClientVersion = "7.13.0.100";
		//public static string ShortVersion = "7.13.0";

		//public const string Url = "https://twd-old.drillerservices.com";
		private static bool connectToBackupServer;
		private static string dataURL;

		public static string DataURL //"https://backup-twd.drillerservices.com";
		{
			get
			{
				if (string.IsNullOrEmpty(dataURL))
				{
					connectToBackupServer = TWDPlayerPrefs.GetInt("ConnectToBackupServer") == 1;
					dataURL = (connectToBackupServer ? Primary_DataURL : Updated_DataURL);
				}

				return dataURL;
			}
			private set
			{
				dataURL = value;
			}
		}
		public static string Primary_DataURL = "https://backup-twd.drillerservices.com";
		public static string Updated_DataURL = "https://twd-old.drillerservices.com";
		public static string Secondary_DataURL = "https://twd.drillerservices.com";

		public static void ToggleBackupServer()
		{
			connectToBackupServer = !connectToBackupServer;
			Debug.LogWarning("Connecting to backup:" + connectToBackupServer);
			TWDPlayerPrefs.SetInt("ConnectToBackupServer", connectToBackupServer ? 1 : 0);
			TWDPlayerPrefs.Save();
		}

		//пользователь вводит в Input
		//private long regState;
		//сохраненный рег
		//private long regCode;

		public bool IsFirstRun { get; private set; }
		public bool TrialModeOver { get; private set; }
		public int TrialModeDays { get; private set; } = 15;

		public DateTime FirstRun { get; set; }
		public int TimesRun { get; set; }
		public int TimesConnect { get; set; }

		public bool IsReged { get; set; }
		public bool IsBlocked { get; set; }

		public bool IsUseGoogleDrive;
		public bool IsUseEOSLogin;

		public string UserWishes { get; set; }
		public string Feedback { get; set; }
		public bool IsFeedbackReaded { get; set; }

		public LocalizationUIUpdater regStateUpdater;
		public UIInput regInput;
		private LocalizationUIUpdater gameStatusUI;

		public static string CountryCode = "null";

		public string HashID;
		public string Pin_HashID;

		public bool IsSaveSettings;

		public bool DoOpenPhonesPopup;

		public const int CoucilLevelBase = 30;
		public const int RadioTentLevelBase = 30;

		public bool IsReconnectPlayerState;
		public bool IsReconnectByCode;

		public bool DoSaveImmediately;

		//подгружать локальный GED при загрузке онлайн
		public bool IsLoadLocalGed;
		public UIToggle IsLoadLocalGedToggle;
		//подгружать гильдию со старта
		public bool IsLoadGuildsFromStart;
		public UIToggle LoadGuildsFromStartToggle;
		//сохранять данные по снаряжению при открытии карточки EquipmentUpgradePopup
		public bool IsSaveEquipmentJson = false;

		public int RadioGoldPrice { get; set; }

		public CurrencyType currencyOverride;
		public bool IsCurrencyOverride { get; set; }

		public static string UserInfo = "null";

		//для скрытых данных гильдии
		public bool ProGuild { get; set; }
		//для скрытых данных линка аккаунтов
		public bool ProLink { get; set; }
		public bool Anonymous { get; set; }

		//я - для отмены режима anonymous
		public bool SuperAdmin = false;
		//сохранен ли hashId как основной в Key_Pin_HashID
		public static bool IsPinId;

		//активировать модуль Mission_HUB
		public static bool IsMissionHubEnabled = true;

		public static bool IsTutorialOn = false;

		public bool ShowDebugMenu;

		//глобальные переменные
		//Где находится папка AssetBundles (в проекте или вне его). Поменять при создании сборки на FALSE
		public static bool IsCustomAssetBundlesPath = false;
		//itemgraphics, hudelements, uimaterials ...
		//false - загрузка ресурсов из бандлов. true - загрузка Resources.Load
		public static bool IsLoadAssetFromResources = true;

		public static bool IsUseMetrics = false;
		//использовать LootModelManager, Cashier
		public static bool IsUseCashier = false;

		public ActivityButton ActivityButton;
		public GameObject ToolBag;
		public LimitedTimeOfferButton LimitedTimeOfferButton;
		//public IAPManager IAPManager { get; set; }
		public bool IsOpenSevenDays;
		public bool IsPlusOneFix;
		public GameObject IsOpenSevenDaysButtons;
		public UIToggle IsOpenSevenDaysToggle;
		public UIToggle IsFreeAllToggle;
		public UIButton FileBrowserBt;
		public UIButtonToggle VpnToggle;
		public UIButtonToggle CopyImageToBufferToggle;
		public bool IsVpnON { get; private set; }
		public GoogleSheetManager GoogleSheetManager;
		public bool IsGedFromGoogle { get; set; }

		public bool IsGedCached => ContentManager.Instance.GetCache("GED") != null;

		public static string GetHashID => Instance.Player?.HashedId ?? UserPrefsKeys.Player_HashID;

		public bool IsInited { get; private set; }

		public bool IsRegPopupOpened => craftSettings.RegPopup != null && craftSettings.RegPopup.gameObject.activeSelf;
		public bool IsLocalPlayer { get; private set; }
		public bool IsCopyImageToBuffer { get; private set; }
		private int regPopupErrorCount;
		[SerializeField]
		private bool IsProUserOverride;

		public void SetLocalPlayer(bool islocal)
		{
			IsLocalPlayer = islocal;
			if (!IsReged) craftSettings.EpicButton.isEnabled = !islocal;
			regPopupErrorCount = 0;
			SupabaseManager.errorsCount = 0;
			SetLocalPlayerResult(islocal);
		}

		private void Awake()
		{
			if (Instance != null)
			{
				DebugTWD.LogError("Multiple DataManager!");
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);

			craftSettings = GetComponent<CraftSettings>();

			InitUISettings();
			currencyOverride = CurrencyType.SurvivalPoints;

			IsCurrencyOverride = currencyOverride != CurrencyType.None;

			RadioGoldPrice = 1;

			//удалить
			if (IsProUserOverride)
			{
				UserPrefsKeys.Player_ProGuild = "true";
				UserPrefsKeys.Player_ProLink = "true";
				UserPrefsKeys.Player_Anonymous = "false";
			}
			//
			if (!TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Player_ProGuild)) UserPrefsKeys.Player_ProGuild = ProGuild.ToString();
			else ProGuild = bool.Parse(UserPrefsKeys.Player_ProGuild);
			if (!TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Player_ProLink)) UserPrefsKeys.Player_ProLink = ProLink.ToString();
			else ProLink = bool.Parse(UserPrefsKeys.Player_ProLink);
			if (!TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Player_Anonymous)) UserPrefsKeys.Player_Anonymous = Anonymous.ToString();
			else Anonymous = bool.Parse(UserPrefsKeys.Player_Anonymous);

			if (!TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Data_Url)) UserPrefsKeys.Data_Url = DataURL;
			else DataURL = UserPrefsKeys.Data_Url;
			TWDPlayerPrefs.Save();
			//
		}

		private void InitUISettings()
		{
			if (!TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Game_Version))
			{
				UserPrefsKeys.Game_Version = OfflineManager.ClientVersion;
				TWDPlayerPrefs.Save();
			}
			else
			{
				var clienVersion = UserPrefsKeys.Game_Version;

				OfflineManager.ClientVersion = clienVersion;
				OfflineManager.ShortVersion = clienVersion[..clienVersion.LastIndexOf('.')];

				DebugTWD.Log("ClientVersion: " + clienVersion);
				DebugTWD.Log("ShortVersion: " + OfflineManager.ShortVersion);
			}

			craftSettings.ClientVersionInput.Set(OfflineManager.ClientVersion);
			IsGedFromGoogle = false;

			if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Data_Url))
			{
				DataURL = UserPrefsKeys.Data_Url;
				craftSettings.ClientDataUrlInput.Set(DataURL);
			}
			if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_LoadGuildsFromStart))
			{
				LoadGuildsFromStartToggle.value = bool.Parse(TWDPlayerPrefs.GetString(UserPrefsKeys.Key_LoadGuildsFromStart));
			}
			if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_LoadLocalGed))
			{
				IsLoadLocalGedToggle.value = bool.Parse(TWDPlayerPrefs.GetString(UserPrefsKeys.Key_LoadLocalGed));
			}
			if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_PlusOneFix))
			{
				IsPlusOneFix = bool.Parse(TWDPlayerPrefs.GetString(UserPrefsKeys.Key_PlusOneFix));
			}
			if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_IsOpenSevenDays))
			{
				IsOpenSevenDaysToggle.value = bool.Parse(TWDPlayerPrefs.GetString(UserPrefsKeys.Key_IsOpenSevenDays));
			}
			if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_IsFreeAll))
			{
				IsFreeAllToggle.value = bool.Parse(TWDPlayerPrefs.GetString(UserPrefsKeys.Key_IsFreeAll));
			}
			if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_IsVPN))
			{
				bool isVpn = bool.Parse(TWDPlayerPrefs.GetString(UserPrefsKeys.Key_IsVPN));
				VpnToggle.SetToggled(isVpn);
			}
			else
			{
				VpnToggle.SetToggled(true);
			}
		}

		void Start()
		{
			Init();
		}

		public void Init()
		{
			jsonSerializer = OfflineManager.JsonSerializer;
			badgeCraft = BadgeCraft.Instance;
			callCraft = CallCraft.Instance;
			craftSettings = CraftSettings.Instance;
			//saveLogToSheet = SaveLogToSheet.Instance;
			GetData = GetPlayerData.Instance;
			gWTeamUtils = GWTeamUtils.Instance;
			localizationManager = LocalizationManager.Instance;
			playerRandomValues = PlayerRandomValues.Instance;
			residencePopup = ResidencePopup.Instance;
			newPhonePopup = NewPhonePopup.Instance;
			supabaseManager = SupabaseManager.Instance;

			gameStatusUI = craftSettings.GameStatus.GetComponent<LocalizationUIUpdater>();

			GameManager.Instance.InitializeResources();

#if GOOGLE_SHEET
			StartCoroutine(GoogleAuthrisationHelper.CheckForRefreshOfToken(true));
#endif
			CraftSettings.Instance.StartSettings();

			if (OfflineManager.ConfigBuildType == OfflineManager.ConfigDataType.Light) FileBrowserBt.gameObject.SetActive(false);

			if (SignalRClient.Instance == null)
			{
				if (GameManager.Instance.TryGetComponent<SignalRClient>(out var signalR))
				{
					signalR.Init();
				}
			}

			SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;

			IsInited = true;

			SetGameStatus();
			//FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders);
		}

		private void OnLocalizationLanguageChanged(string newLanguageID)
		{
			bool isRu = newLanguageID == "ru";
			VpnToggle.transform.parent.gameObject.SetActive(isRu);
		}

		private void BlockedResult()
		{
			string textRu = "Вы заблокированы! Свяжитесь: t.me/BloodyModding";
			string textEn = "You are blocked! Contact via Telegram: t.me/BloodyModding";

			MyTools.UpdateLogPanel(textEn, textRu);
			gameStatusUI.EnCustomText = textEn;
			gameStatusUI.RuCustomText = textRu;
			gameStatusUI.UpdateContent();
		}

		public void SetLocalPlayerResult(bool isLocal)
		{
			gameStatusUI.gameObject.SetActive(true);
			if (isLocal)
			{
				string textRu = "Локальная сессия: ВКЛЮЧЕНА";
				string textEn = "Local Session: ON";

				MyTools.UpdateLogPanel(textEn, textRu);
				gameStatusUI.EnCustomText = textEn;
				gameStatusUI.RuCustomText = textRu;
				gameStatusUI.UpdateContent();
			}
			else
			{
				if (IsBlocked)
				{
					BlockedResult();
				}
				else if (!IsReged)
				{
					TrialOverResult();
				}
				else
				{
					gameStatusUI.gameObject.SetActive(false);
				}
			}			
		}

		private void TrialOverResult()
		{
			DateTime trialExpired = FirstRun.AddDays(TrialModeDays);
			TimeSpan timeLeft = trialExpired - DateTime.Now;
			TrialModeOver = DateTime.Now > trialExpired;

			DebugTWD.Log("Осталось " + timeLeft.Days + " дней");

			if (TrialModeOver)
			{
				gameStatusUI.EnCustomText = "Trial mode expired. Please register. Telegram: t.me/BloodyModding";
				gameStatusUI.RuCustomText = "Пробный режим закончился. Зарегистрируйтесь. Telegram: t.me/BloodyModding";
			}
			else
			{
				gameStatusUI.EnCustomText = "Trial mode. Left " + (timeLeft.Days > 1 ? timeLeft.Days.ToString() + " days" : timeLeft.Hours.ToString() + " hours");
				gameStatusUI.RuCustomText = "Пробный режим. Осталось " + (timeLeft.Days > 1 ? timeLeft.Days.ToString() + " дней" : timeLeft.Hours.ToString() + " часов");
			}
			gameStatusUI.UpdateContent();
		}

		public async void SetGameStatus()
		{
			await UniTask.NextFrame();

			IsFeedbackReaded = true;
			Anonymous = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_Anonymous);
			bool isOffline = true;
			//bool isInternet = await OfflineManager.Instance.CheckIfOnline();
			if (OfflineManager.IsInternetOn && !IsLocalPlayer)
			{
				bool isSignedIn;

				if (OfflineManager.UseSupabase)
				{
					var taskClient = await supabaseManager.SetClient();
					isSignedIn = taskClient.Status == TaskStatus.Success;
					MyTools.UpdateLogPanel(supabaseManager.ErrorText, supabaseManager.ErrorTextRu);

					if (isSignedIn)
					{
						if (DatabaseManager == null || DatabaseManager.SupaClient == null)
						{
							DatabaseManager = new DatabaseManager();
						}

						bool updated = await DatabaseManager.UpdateLastRun();
						if (!updated)
						{
							string textRu = "Необходима авторизация пользователя CraftMachine через email или Google";
							string textEn = "CraftMachine user authentification via email or Google is required";
							MyTools.UpdateLogPanel(textEn, textRu);

							regPopupErrorCount++;
							OpenRegPopup();
							craftSettings.RegPopup.SetState(RegPopup.RegState.Sign);
							return;
						}
						else
						{
							DatabaseManager.IsInited = true;
							isOffline = false;

							var supaUser = DatabaseManager.CurrentCMUser;
							IsReged = supaUser.Regged;
							UserPrefsKeys.Player_Regged = IsReged.ToString();

							IsBlocked = supaUser.Blocked;
							UserPrefsKeys.Player_Blocked = IsBlocked.ToString();

							FirstRun = supaUser.FirstRun;
							UserPrefsKeys.Player_FirstRun = FirstRun.ToString();

							TrialModeDays = supaUser.TrialCount;
							UserPrefsKeys.Player_TrialCount = TrialModeDays;

							ProGuild = supaUser.ProGuild;
							UserPrefsKeys.Player_ProGuild = ProGuild.ToString();

							ProLink = supaUser.ProLink;
							UserPrefsKeys.Player_ProLink = ProLink.ToString();

							TWDPlayerPrefs.Save();

							//var data = await DatabaseManager.GetIDListAsync();
							//Debug.LogWarning(data.First().PlayerName);

							if (IsBlocked)
							{
								BlockedResult();
								return;
							}

							if (!IsReged)
							{
								TrialOverResult();

								if (TrialModeOver)
								{
									var logTrialEn = "The trial period is expired. \nContact the developer via Telegram: t.me/BloodyModding";
									var logTrialRu = "Пробный период завершен. \nСвяжитесь с разработчиком. Telegram: t.me/BloodyModding";

									MyTools.UpdateLogPanel(logTrialEn, logTrialRu);
									DebugTWD.Log(logTrialEn);

									if (!IsLocalPlayer)
									{
										regPopupErrorCount++;
										OpenRegPopup();
										craftSettings.RegPopup.SetState(RegPopup.RegState.Reg);

										while (IsRegPopupOpened)
										{
											await UniTask.Yield();
										}

										if (!IsReged)
										{
											return;
										}
									}
								}
							}

							if (IsFeedbackReaded)
							{
								IsFeedbackReaded = false;
								Feedback = supaUser.Feedback;
								UserPrefsKeys.Player_Feedback = Feedback;
								TWDPlayerPrefs.Save();

								if (!string.IsNullOrEmpty(Feedback))
								{
									craftSettings.SetupGlowFeedback(isOn: true);
								}
							}
						}
					}
					else
					{
						if (taskClient.Status == TaskStatus.NeedAuth)
						{
							string textRu = "Необходима авторизация пользователя CraftMachine через email или Google";
							string textEn = "CraftMachine user authentification via email or Google is required";
							MyTools.UpdateLogPanel(textEn, textRu);

							regPopupErrorCount++;
							OpenRegPopup();
							craftSettings.RegPopup.SetState(RegPopup.RegState.Sign);
						}
						else
						{
							string textRu = "Проблема с подключением к серверу CraftMachine. Попробуйте переключить VPN";
							string textEn = "There is a problem connecting to CraftMachine server. Try switch your VPN";
							MyTools.UpdateLogPanel(textEn, textRu);
						}
						return;
					}
				}
				else
				{
					while (AuthenticationService.Instance == null)
					{
						await Task.Delay(500);
					}
					isSignedIn = AuthenticationService.Instance.IsSignedIn;
				}
			}
			else
			{
				string textRu = "Пожалуйста, включите интернет для проверки вашего доступа";
				string textEn = "Please turn on Internet connection to verify your access";
				MyTools.UpdateLogPanel(textEn, textRu);
			}

			if (isOffline)
			{
				if (!string.IsNullOrEmpty(UserPrefsKeys.Supa_ID))
				{
					ProGuild = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_ProGuild);
					ProLink = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_ProLink);

					IsReged = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_Regged);
					IsBlocked = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_Blocked);

					FirstRun = DateTime.Parse(UserPrefsKeys.Player_FirstRun);

					if (UserPrefsKeys.Player_TrialCount != 0) TrialModeDays = UserPrefsKeys.Player_TrialCount;
					else UserPrefsKeys.Player_TrialCount = TrialModeDays;

					GWTeamUtils.Instance.GuildID = UserPrefsKeys.Player_GuildID;
				}
				else
				{
					string textRu = "Пользователь не найден. Необходима авторизация. Откройте окно REG";
					string textEn = "User not found. Authorization required. Open the REG popup";
					MyTools.UpdateLogPanel(textEn, textRu);
				}
			}

			if (IsBlocked)
			{
				BlockedResult();
				return;
			}

			if (!IsReged && !IsLocalPlayer) TrialOverResult();

			await UniTask.NextFrame();

			LoadSettings();

			craftSettings.CheckInternetStatus();

			CopyImageToBufferToggle.transform.parent.gameObject.SetActive(ProGuild);
			VpnToggle.transform.parent.gameObject.SetActive(language == Language.Ru);

			IsDataManagerInitialized = true;

			DebugTWD.Log("DataManager Initialized");
		}

		public void SetClientVersion(UIInput input)
		{
			if (!IsDataManagerInitialized) return;

			OfflineManager.ClientVersion = input.value;
			DebugTWD.Log("ClientVersion from UIInput: " + input.value);
			UserPrefsKeys.Game_Version = OfflineManager.ClientVersion;
			TWDPlayerPrefs.Save();
		}

		public void SetDataUrl(UIInput input)
		{
			if (!IsDataManagerInitialized) return;

			DataURL = input.value;
			UserPrefsKeys.Data_Url = DataURL;
			TWDPlayerPrefs.Save();
		}

		public void PinCurrentID(UIButtonToggle tg)
		{
			if (Player == null)
			{
				if (tg.IsToggled) tg.SetToggled(false);
				return;
			}

			if (tg.IsToggled)
			{
				UserPrefsKeys.Player_Pin_Name = Player.Name.ToString();
				UserPrefsKeys.Player_Pin_HashID = Player.HashedId.ToString();
				UserPrefsKeys.Player_Pin_EpicAccountID = UserPrefsKeys.Player_EpicAccountID;
				UserPrefsKeys.Player_Pin_GoogleID = UserPrefsKeys.Player_GoogleID;

				craftSettings.Pin_CraftsManLabel.text = Player.Name;
				IsPinId = true;
			}
			else
			{
				TWDPlayerPrefs.DeleteKey(UserPrefsKeys.Key_Player_Pin_Name);
				TWDPlayerPrefs.DeleteKey(UserPrefsKeys.Key_Player_Pin_HashID);
				TWDPlayerPrefs.DeleteKey(UserPrefsKeys.Key_Player_Pin_GoogleID);
				TWDPlayerPrefs.DeleteKey(UserPrefsKeys.Key_Player_Pin_EpicAccountID);

				craftSettings.Pin_CraftsManLabel.text = "";
				IsPinId = false;
			}
			TWDPlayerPrefs.Save();
		}

		public void CheckPinID()
		{
			Pin_HashID = UserPrefsKeys.Player_Pin_HashID;

			if (!string.IsNullOrEmpty(Pin_HashID))
			{
				craftSettings.Pin_CraftsManLabel.text = UserPrefsKeys.Player_Pin_Name;
				IsPinId = true;
				craftSettings.pinToggle.SetToggled(true, false);
			}
			else
			{
				craftSettings.Pin_CraftsManLabel.text = "";
				IsPinId = false;
				craftSettings.pinToggle.SetToggled(false, false);
			}
		}

		public void OpenRegPopup()
		{
			if (OfflineManager.UseSupabase)
			{
                var regPopup = craftSettings.RegPopup;
                regPopup.gameObject.SetActive(true);

                regPopup.SetLocalButton.gameObject.SetActive(true);
                regPopup.ReloadSupaClientButton.gameObject.SetActive(true);
                regPopup.ExtraButtonContainer.Reposition();

                regPopup.GetSessionStatus();
            }
			else
			{
                var regPopup = craftSettings.UnityRegPopup;
                regPopup.gameObject.SetActive(true);
            }		
		}

		public void SetWish(UIInput input)
		{
			UserWishes = input.value;
			DebugTWD.Log("UserWishes : " + UserWishes);
			UserPrefsKeys.Player_Wishes = UserWishes;
			TWDPlayerPrefs.Save();
		}

		public void SendWish()
		{
			UserWishes = craftSettings.WishInput.value;
			craftSettings.FeedbackObject.transform.parent.gameObject.SetActive(false);

			if (string.IsNullOrEmpty(UserWishes)) return;

			if (UserWishes != "null") DebugTWD.Log("UserWishes : " + UserWishes);
			UserPrefsKeys.Player_Wishes = UserWishes;
			TWDPlayerPrefs.Save();

			if (SupabaseManager.IsOnline)
			{
				StartCoroutine(SendWishProcess());
			}
			else
			{
				string log;
				if (language != Language.Ru)
				{
					log = "Can't sent message now. Will try send later.";
				}
				else
				{
					log = "Не могу отправить сообщение. Проверьте интернет";
				}
				MyTools.UpdateLogPanel(log);
				DebugTWD.Log(log);
			}
		}

		public IEnumerator SendWishProcess()
		{
			//saveLogToSheet.IsBuizy = true;
			string hashID = UserPrefsKeys.Player_HashID;
			if (string.IsNullOrEmpty(hashID))
			{
				yield break;
			}

			DebugTWD.Log("Wait for exit : " + hashID);
			//saveLogToSheet.GetUserData(hashID, saveLogToSheet.OnSendMessage);
			//yield return new WaitUntil(() => !saveLogToSheet.IsBuizy);

			string logEn = "Message has been sent";
			string logRu = "Сообщение было отправлено";

			MyTools.UpdateLogPanel(logEn, logRu);
			DebugTWD.Log(logRu);
		}

		private IEnumerator ClearPlayerPrefsC()
		{
			TWDPlayerPrefs.DeleteAll();
			ContentCache.DeleteAll();

			yield return null;
			yield return new WaitForEndOfFrame();

			Application.Quit();
			DebugTWD.Log("EXIT PLAYER DONE");
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#endif
		}

		public void ClearPlayerPrefs()
		{
			StartCoroutine(ClearPlayerPrefsC());
		}

		public static string ClientModelVersion(string version)
		{
			if (string.IsNullOrEmpty(version)) return string.Empty;
			int count = 0;
			int countPoint = 0;
			foreach (var c in version)
			{
				count++;
				if (c == '.')
				{
					countPoint++;
				}
				if (countPoint == 3) break;
			}
			return version.Substring(0, count - 1);
		}

		void Update()
		{
			if (Player != null)
			{
				if (DoSaveImmediately && Player.Camp != null)
				{
					Player.Camp.CampDefenseModel.CreateWalker();
					ActorModel model = Player.Camp.CampDefenseModel.Walkers.Last();
					Helpers.ExecuteCommand(new CampDefenseKillWalkerCommand(model));
					DoSaveImmediately = false;
				}

				if (IsUseCashier)
				{
					CheckForHeartbeat();
				}
			}
		}

		public void LoadGameEconomyData()
		{
			GameData = null;
			string gameEconomyDataJson;

			if (IsGedCached)
			{
				gameEconomyDataJson = ContentManager.Instance.GetCache("GED").GetContentById<string>("GameEconomyData");
			}
			else
			{
				gameEconomyDataJson = ((TextAsset)Resources.Load(GedPathFolder)).text;
			}

			if (string.IsNullOrEmpty(gameEconomyDataJson))
			{
				//string logError = CustomLocalization.GetText("NoGedInCache");
				string logErrorEn = "GameEconomyData is null from Cache";
				string logErrorRu = "GameEconomyData не найдена в Кэше";

				MyTools.UpdateLogPanel(logErrorEn, logErrorRu);
				DebugTWD.LogError(logErrorEn);
				return;
			}
			StartCoroutine(StartGameEconomyData(gameEconomyDataJson));
		}

		public IEnumerator StartGameEconomyData(string gameEconomyDataJson)
		{
			yield return null;

			if (gameEconomyDataJson.Substring(0, 100).IndexOf("\"Version\":2") != -1)
			{
				DebugTWD.Log("Read Version 2");
				GameData = JsonUtility.FromJson<GameEconomyData>(gameEconomyDataJson);
			}
			else
			{
				DebugTWD.Log("Read Version 1");
				GameData = jsonSerializer.Deserialize<GameEconomyData>(gameEconomyDataJson);
			}
			yield return null;

			GameData.Start();

			yield return null;

			GetData.waitingGed = false;

			GameManager.Instance.SetGameEconomyData(GameData);
			if (GameManager.Instance.IsConnectedToServer)
			{
				SingularityMonoBehaviour<LocalizationManager>.Instance.UseOnlyLocalFiles = false;
				SingularityMonoBehaviour<LocalizationManager>.Instance.Load(SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage, forceUpdate: true);
			}

			string accessEn;
			string accessRu;
			if (IsLoadLocalGed || contentSource == ContentSource.Local)
			{
				accessEn = !IsGedCached ? "from Resources" : "from Local Content Cache";
				accessRu = !IsGedCached ? "из ресурсов" : "из локального кэша";
			}
			else
			{
				accessEn = !IsVpnON || IsGedFromGoogle ? "from WEB (by Google-bot)" : "from WEB (direct access)";
				accessRu = !IsVpnON || IsGedFromGoogle ? "по сети (через Google бота)" : "по сети (прямой доступа)";
				ContentManager.Instance.GetCache("GED").SetContent("GameEconomyData", null, null, gameEconomyDataJson);
			}

			OfflineManager.Instance.IsGedLoaded = true;

			TWDPlayerPrefs.SetString(UserPrefsKeys.Key_ContentBaseUrl, GameData.ConfigData.CDNBaseUrl);

			string logEn = "GameEconomyData loading finished " + accessEn;
			string logRu = "GameEconomyData загружена " + accessRu;

			MyTools.UpdateLogPanel(logEn, logRu);
			DebugTWD.Log(logEn);
		}

		public IEnumerator OnRemovePersistentTokenButtonClick()
		{
			//авторизоваться под другим профилем Epic
			craftSettings.CheckInternetStatus();
			if (!OfflineManager.IsInternetOn)
			{
				MyTools.OpenAlert("Пожалуйста включите интернет");
				DebugTWD.Log("Интернет выключен. Break OnRemovePersistentTokenButtonClick", DebugType.Connection);
				yield break;
			}
			if (EOSManager.Instance == null)
			{
				DebugTWD.Log("EOSManager.Instance is null. Break OnRemovePersistentTokenButtonClick", DebugType.Connection);
				MyTools.OpenAlert("Сервис Epic выключен. Для сброса текущего профиля сначала загрузите его онлайн");
				yield break;
			}
			if (EOSManager.Instance.GetEOSPlatformInterface() == null)
			{
				MyTools.OpenAlert("Вы еще не подключались к Эпик. Сброс профиля не требуется");
				DebugTWD.Log("EOSPlatformInterface is null. Break OnRemovePersistentTokenButtonClick", DebugType.Connection);
				yield break;
			}

			EOSManager.Instance.RemovePersistentToken();

			DebugTWD.Log("Reset Epic Persistent auth credentials", DebugType.Connection);
		}

		public void LoadPlayerFromJson()
		{
			//Question
			//string ContentBaseUrl = TWDTWDPlayerPrefs.GetString("ContentBaseUrl", null);
			//if (!string.IsNullOrEmpty(ContentBaseUrl) && craftSettings.IsInternetOn)
			//{
			//    new HTTPRequest(new Uri(ContentBaseUrl), isKeepAlive: true, disableCache: true, delegate
			//    {
			//    }).Send();
			//}

			LoadGameEconomyData();

			if (IsNetPath)
			{
				contentSource = GetPlayerData.GetContentSource(OfflineManager.Instance.ConnectSourceCurrent);
			}
			else
			{
				contentSource = ContentSource.Local;
				StartCoroutine(LoadPlayerContent(null));
			}
		}

		public IEnumerator LoadPlayerContent(string modelJson)
		{
			string path;
			if (contentSource == ContentSource.Local)
			{
				yield return new WaitForSeconds(.5f);

				modelJson = ContentManager.Instance.GetCache("Player").GetContentById<string>("PlayerModel");
				if (string.IsNullOrEmpty(modelJson))
				{
					string logErrorEn = $"Content is null from Cashe. Please Login via Epic once";
					string logErrorRu = $"Данные из локального кэша отсутствуют. Пожалуйста подключитесь с помощью Epic";

					MyTools.UpdateLogPanel(logErrorEn, logErrorRu);
					DebugTWD.LogError(logErrorRu);
					yield break;
				}

				path = language != Language.Ru ? "Cashe" : "Кэша";
			}
			else if (contentSource == ContentSource.Epic)
			{
				path = "Epic Profile";
			}
			else if (contentSource == ContentSource.Steam)
			{
				path = "Steam Profile";
			}
			else
			{
				path = "Google Play";
			}

			yield return new WaitUntil(() => OfflineManager.Instance.IsGedLoaded);

			ModelManager = new TWDModelManager(false);
			ModelManager.SetModelManagerMode(ModelManagerMode.Client);
			ModelManager.SetModelDebug(new UnityModelDebug());
			ModelManager.LoadModel(modelJson, null);

			ModelManager.SetGameEconomyData(GameData);
			ModelManager.Start(1);

			if (IsUseCashier)
			{
				ModelManager.SetModelManagerMode(ModelManagerMode.Client);
				ModelManager.SetModelAnalytics(new AnalyticsClient());
				ModelManager.SetCommandTransport(new TWDModelCommandTransport());
			}

			GameManager.Instance.SetModelManager(ModelManager);

			Player = ModelManager.Player;
			DebugTWD.LogWarning("Base Player Random : " + Player.PlayerRandom.State);

			//Player.Created = DateTime.UtcNow.AddMilliseconds(-ModelManager.Time);
			//Player.SetManager(ModelManager);
			//Player.Initialize();

			OnPlayerLoadEnd();

			yield return null;

			//if (contentSource != ContentSource.Epic && EOSManager.Instance != null)
			//{
			//	GetComponent<EOSManager>().enabled = false;
			//}

			OfflineManager.Instance.IsPlayerLoaded = true;

			CommandHelper.Instance.IsPrelogin = false;

			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);

			CountryCode = Player.Country;

			HashID = Player.HashedId;

			UserPrefsKeys.Player_HashID = HashID;
			UserPrefsKeys.Player_Name = Player.Name;
			TWDPlayerPrefs.Save();

			if (!IsLocalPlayer) DatabaseManager?.UpdateCMUser();
			//DatabaseManager.UpdateTWDAccount();

			DebugTWD.Log("Name and Hash saved for " + Player.Name);

			//Player.Blackboard.CounterValues.TryGetValue("Counter.Residence.0.Level", out int residenceLevel);
			//ResidenceLevel = residenceLevel <= 0 ? 1 : residenceLevel;
			//Debug.Log("Counter.Residence.0.Level " + ResidenceLevel);

			//Player.PhoneCall.SetManager(ModelManager);
			//Player.PhoneCall.Initialize();

			ResidenceLevel = Player.GetBuildingLevel("Residence");
			RadioTentLevel = Player.GetBuildingLevel("RadioTent");

			yield return null;

			PlayerBadges = BadgeUtils.GetAllBadgesCorrect();

			string playerTime = Player.UtcTime.ToLocalTime().ToString(UserPrefsKeys.TimeFormat);
			string logEn = $"Player {Player.Name} loading finished from {path}\nActual profile date: {playerTime}";
			string logRu = $"Данные профиля {Player.Name} загружены из {path}\nАктуальная дата профиля: {playerTime}";
			string randoms = $"\nPlayerRandom: {Player.PlayerRandom.State}\nModelRandom: {ModelManager.Player.LootManager.GetDedicatedRandom("BadgeRandom").State}";
			logEn += randoms;
			logRu += randoms;
			MyTools.UpdateLogPanel(logEn, logRu);
			DebugTWD.Log(logEn);

			StartCoroutine(residencePopup.WaitForPlayer());
			craftSettings.OnLoadPlayer();
			StartCoroutine(badgeCraft.InitData());
			StartCoroutine(callCraft.InitData());
			if (IsLoadGuildsFromStart)
			{
				StartCoroutine(residencePopup.OpenGuildTab(0, 0, false));
				//GWTeamUtils.Instance.GuildID = Player.GuildId;
				//GWTeamUtils.Instance.LoadGuildData(isOpponent : false);
			}

			//CraftSettings.Instance.showDebugPopup.gameObject.SetActive(proUser);
			ActivityButton.gameObject.SetActive(Player != null);
			ToolBag.SetActive(Player != null);

			CraftSettings.Instance.SetMetersCurrency();
			CraftSettings.Instance.ShowMeters(new int[]{ 0, 1 });
			SwitchSevenDaysInternal(IsOpenSevenDays);
		}

		[ContextMenu("UpdateTWDAccount")]
		public void UpdateTWDAccount()
		{
			DatabaseManager?.UpdateTWDAccount();
		}

		public void SwitchSevenDaysInternal(bool isOpenSevenDays)
		{
			IsOpenSevenDays = isOpenSevenDays;

			TWDPlayerPrefs.SetString(UserPrefsKeys.Key_IsOpenSevenDays, IsOpenSevenDays.ToString());
			TWDPlayerPrefs.Save();

			if (Player == null) return;
			if (IsOpenSevenDays)
			{
				if (OfflineManager.IsInternetOn)
				{
					GameManager.Instance.SetIAPManager();
				}
				Player.SevenDayLoginManager?.Tick(0L);
			}

			LimitedTimeOfferButton.gameObject.SetActive(IsOpenSevenDays);

			IsOpenSevenDaysButtons.SetActive(IsOpenSevenDays);

			craftSettings.RepositionSettingsTab();
		}

		public void SavePlayerJsonSwitch(UIToggle tg)
		{
			DoSavePlayerJson = tg.value;
		}

		// OnClick
		public void SwitchNetPath(UIToggle tg)
		{
			IsNetPath = tg.value;

			craftSettings.ContentUrlObject.SetActive(IsNetPath);
			MyTools.ResetLogPanel();
			craftSettings.RepositionSettingsTab();
		}

		// OnClick
		public void SwitchGuildsFromStart(UIToggle tg)
		{
			IsLoadGuildsFromStart = tg.value;
			TWDPlayerPrefs.SetString(UserPrefsKeys.Key_LoadGuildsFromStart, IsLoadGuildsFromStart.ToString());
			TWDPlayerPrefs.Save();
		}

		// OnClick
		public void SwitchPreferLocalGED(UIToggle tg)
		{
			IsLoadLocalGed = tg.value;
			TWDPlayerPrefs.SetString(UserPrefsKeys.Key_LoadLocalGed, IsLoadLocalGed.ToString());
			TWDPlayerPrefs.Save();
		}

		// OnClick
		public void SwitchEOSLogin(UIToggle tg)
		{
			if (OfflineManager.IsGoogleSource) return;

			IsUseEOSLogin = tg.value;

			if (!IsUseEOSLogin) StartCoroutine(OnRemovePersistentTokenButtonClick());
			craftSettings.GameVersionObject.SetActive(IsUseEOSLogin);
			craftSettings.EpicObject.SetActive(IsUseEOSLogin);
			craftSettings.RepositionSettingsTab();
		}

		public void SwitchSaveOnExit(UIToggle tg)
		{
			badgeCraft.IsSaveLog = tg.value;
		}

		// OnClick
		public void SwitchSevenDays(UIToggle tg)
		{
			SwitchSevenDaysInternal(tg.value);
		}

		// OnClick
		public void SwitchFreeAll(UIToggle tg)
		{
			SwitchFreeAllInternal(tg.value);
		}

		// OnClick
		public void SwitchIsVPN(UIButtonToggle tg)
		{
			IsVpnON = tg.IsToggled;
			IsGedFromGoogle = !IsVpnON;
			TWDPlayerPrefs.SetString(UserPrefsKeys.Key_IsVPN, IsVpnON.ToString());
			TWDPlayerPrefs.Save();
		}

		public void SwitchFreeAllInternal(bool isFreeAll)
		{
			OfflineManager.IsFreeAll = isFreeAll;
			TWDPlayerPrefs.SetString(UserPrefsKeys.Key_IsFreeAll, isFreeAll.ToString());
			TWDPlayerPrefs.Save();
		}

		// OnClick
		public void SwitchCopyToBuffer(UIButtonToggle tg)
		{
			IsCopyImageToBuffer = tg.IsToggled;
		}

		// OnClick
		[ContextMenu("ClearBundlePrefs")]
		public void ClearBundlePrefs()
		{
			if (TWDPlayerPrefs.HasKey("BUNDLE_SHOWN_LIST"))
			{
				TWDPlayerPrefs.DeleteKey("BUNDLE_SHOWN_LIST");
			}
			if (TWDPlayerPrefs.HasKey("LIMITED_OFFER_LAST_SHOWN_DATE"))
			{
				TWDPlayerPrefs.DeleteKey("LIMITED_OFFER_LAST_SHOWN_DATE");
			}
			if (TWDPlayerPrefs.HasKey("BUNDLE_LAST_SHOWN_DATE"))
			{
				TWDPlayerPrefs.DeleteKey("BUNDLE_LAST_SHOWN_DATE");
			}
		}

		[ContextMenu("GetBundlePrefsData")]
		public void GetBundlePrefsData()
		{
			string text = "";
			if (TWDPlayerPrefs.HasKey("BUNDLE_SHOWN_LIST"))
			{
				var list = TWDPlayerPrefs.GetString("BUNDLE_SHOWN_LIST");
				if (list.Length > 0)
				{
					Dictionary<string, int> dictionary = jsonSerializer.Deserialize<Dictionary<string, int>>(list);
					string debug = string.Join("\n", dictionary.Keys);
					text += "Просмотренные бандлы :\n" + debug + "\n";
					DebugTWD.Log("BUNDLE_SHOWN_LIST : " + debug, DebugType.System);
				}
			}
			if (TWDPlayerPrefs.HasKey("LIMITED_OFFER_LAST_SHOWN_DATE"))
			{
				var value = TWDPlayerPrefs.GetString("LIMITED_OFFER_LAST_SHOWN_DATE");
				long dateData = System.Convert.ToInt64(value);
				var date = MyTools.ToReadableString(DateTime.Now.Subtract(DateTime.FromBinary(dateData)));
				text += "Прошло времени после просмотра преложения :\n" + date + "\n";
				DebugTWD.Log("LIMITED_OFFER_LAST_SHOWN_DATE : " + date, DebugType.System);
			}
			if (TWDPlayerPrefs.HasKey("BUNDLE_LAST_SHOWN_DATE"))
			{
				var value = TWDPlayerPrefs.GetString("BUNDLE_LAST_SHOWN_DATE");
				long dateData = System.Convert.ToInt64(value);
				var date = MyTools.ToReadableString(DateTime.Now.Subtract(DateTime.FromBinary(dateData)));
				text += "Прошло времени после просмотра бандла :\n" + date;

				DebugTWD.Log("BUNDLE_LAST_SHOWN_DATE : " + date, DebugType.System);
			}
			MyTools.OpenAlert(text);
		}

		[ContextMenu("Debug PlayerRandom")]
		public void DebugPlayerRandom()
		{
			DebugTWD.LogWarning("Current PlayerRandom is : " + Player.PlayerRandom.State);
		}

		public long TimeToConvert;
		[ContextMenu("Convert Time")]
		public void ConvertTime()
		{
			DebugTWD.LogWarning("ConvertTime is : " + MyTools.LongToTime(TimeToConvert));
		}

		[ContextMenu("Debug EOS Data")]
		public void DebugEOSData()
		{
			string Name = Player?.Name ?? UserPrefsKeys.Player_Name;
			DebugTWD.LogWarning("Current Player Name is : " + Name);
			string hashID = GetHashID;
			DebugTWD.LogWarning("Current Player ID is : " + hashID);
			if (GetComponent<EOSManager>().enabled)
			{
				string accName = EOSLogin.GetUserDisplayName();
				DebugTWD.LogWarning("Current Eos Profile Name is : " + accName);

				string EosAccountID = TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Player_EpicAccountID) ? UserPrefsKeys.Player_EpicAccountID : EOSLogin.GetAccountUserId().ToString();
				DebugTWD.LogWarning("Current EOS Account ID is : " + EosAccountID);

				string LoggedInAccountsCount = EOSManager.Instance.GetEOSAuthInterface().GetLoggedInAccountsCount().ToString();
				DebugTWD.LogWarning("LoggedIn Accounts Count is: " + LoggedInAccountsCount);
			}
			string EosLinkID = UserPrefsKeys.Player_GoogleID;
			DebugTWD.LogWarning("Current EOS Link ID is : " + EosLinkID);
		}

		private void OnPlayerLoadEnd()
		{
			playerRandomValues.ReseedRandomStart();
			GWTeamUtils.Instance.Reset();
			SingularityMonoBehaviour<HUDManager>.Instance.Reset();

			if (GuildManager != null)
			{
				GuildManager.Uninitialize();
				GuildManager = null;
			}
			GuildManager = new GuildManager(ModelManager);

			GameManager.Instance.SetGuildManager(GuildManager);

			if (!IsMissionHubEnabled) return;

			if (SingularityMonoBehaviour<GuildWarManager>.Instance != null)
			{
				SingularityMonoBehaviour<GuildWarManager>.Instance.SubscribeToEvents();
			}

			PlayerRandomValues.SetConditionOpened(on: false);

			if (PortraitManager.Instance == null)
			{
				portraitManager.gameObject.SetActive(true);
			}

			GameManager.Instance.SubscribeEvents();

			//Player.SetManagers(ModelManager);

			//if (!OfflineManager.IsLoadFromResources) PortraitManager.Instance.RenderAllPortraits();
		}

		public void EnablePlayerHubManager()
		{
			PlayerHubManager ??= new PlayerHubManager();
			PlayerHubManager.UpdateInfo();
		}

		public void CheckCurrentPlayerButton()
		{
			if (IsBlocked || !IsReged)
			{
				StopAllCoroutines();
				//StartCoroutine(CheckCurrentPlayer());
			}
		}

		public static string SetContentUrl(string driveLink, int maxChar, bool addPrefix)
		{
			string prefix = !addPrefix ? "" : @"https://drive.google.com/uc?export=download&id=";
			if (string.IsNullOrEmpty(driveLink))
			{
				return null;
			}
			if (driveLink.LastIndexOf('/') <= 0)
			{
				if (driveLink.Length <= maxChar) return prefix + driveLink;
				else
				{
					return null;
				}
			}
			var linkSplitted = driveLink.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			string fileId = linkSplitted[linkSplitted.Length - 2];

			return fileId;
		}

		public void SaveSettings(bool IsForExit)
		{
			int lastState = badgeCraft != null && badgeCraft.modelRandomLast != null ? badgeCraft.modelRandomLast.State : TWDPlayerPrefs.GetInt(UserPrefsKeys.Key_LastState, 0);
			if (IsForExit) TWDPlayerPrefs.SetInt(UserPrefsKeys.Key_LastState, lastState);
			TWDPlayerPrefs.Save();
		}

		public void LoadSettings()
		{
			int lastState = TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_LastState) ? TWDPlayerPrefs.GetInt(UserPrefsKeys.Key_LastState) : 0;
			if (lastState > 0) InitState = lastState;
			craftSettings.SetSettingsView();
		}

		public void ChangeServer(UIPopupList list)
		{
			int index = list.items.IndexOf(list.value);
			//index = index >= 0 ? index : 0;
			switch (index)
			{
				case 0:
					DataURL = Primary_DataURL;
					break;
				case 1:
					DataURL = Updated_DataURL;
					break;
				case 2:
					DataURL = Secondary_DataURL;
					break;
			}
			DebugTWD.Log("Switch data server to " + DataURL, DebugType.Load);
			UserPrefsKeys.Data_Url = DataURL;
			UserPrefsKeys.Data_Url_Index = index;
			TWDPlayerPrefs.Save();
		}

		private void OnDisable()
		{
			SaveSettings(IsForExit : true);
			DebugTWD.Log("EXIT PLAYER DONE");
		}

		private IEnumerator ProcessExit()
		{
			SaveSettings(IsForExit: true);
			if (SignalRClient.Instance) SignalRClient.Instance.enabled = false;

			if (!IsSaveSettings)
			{
				Application.Quit();
				yield break;
			}

			if (SupabaseManager.IsOnline)
			{
				//saveLogToSheet.IsBuizy = true;
				string hashID = UserPrefsKeys.Player_HashID;
				DebugTWD.Log("Wait for exit : " + hashID);

				//saveLogToSheet.GetUserData(hashID, saveLogToSheet.OnSetUserData);
				//yield return new WaitUntil(() => !saveLogToSheet.IsBuizy);
				Application.Quit();
				DebugTWD.Log("EXIT PLAYER DONE");
#if UNITY_EDITOR
				UnityEditor.EditorApplication.ExitPlaymode();
#endif
			}
			else
			{
				Application.Quit();
				DebugTWD.Log("EXIT PLAYER DONE");
#if UNITY_EDITOR
				UnityEditor.EditorApplication.ExitPlaymode();
#endif
			}
		}

		public void ExitApp()
		{
			StartCoroutine(ProcessExit());
		}

		private long timeBetweenheartBeatCommands = 120000L;
		private long lastUserActivityTime;
		private long lastCommandSendTime;

		private void CheckForHeartbeat()
		{
			if (SignalRClient.Instance != null && Player != null && timeBetweenheartBeatCommands != -1)
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
				if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
				{
					DebugTWD.Log(num2 + " " + timeBetweenheartBeatCommands);
				}
				if (num2 > timeBetweenheartBeatCommands && lastUserActivityTime > lastCommandSendTime)
				{
					Helpers.ExecuteCommand(new TickModelCommand());
					DebugTWD.Log("heartbreake command");
					lastCommandSendTime = num;
				}
			}
		}

		public void StartNextChallenge()
		{
			if (Player.WeeklyChallenge.CanPlayNextWeeklyChallenge)
			{
				Helpers.ExecuteCommand(new StartChallengeCommand());
			}
		}

		public void StartNextSurvival()
		{
			if (Player.WeeklySurvival.CanPlayNextWeeklySurvival)
			{
				Helpers.ExecuteCommand(new StartSurvivalCommand());
			}
		}

		public void StartNextEndlessCycle()
		{
			EndlessModeManagerModel endlessModeManager = Player.EndlessModeManager;
			if (endlessModeManager.CanStartNewEndlessModeCycle)
			{
				Helpers.ExecuteCommand(new StartEndlessCycleCommand());
			}
			else if (!endlessModeManager.AreEndlessActorsValidAndGenerated)
			{
				Helpers.ExecuteCommand(new ForceGenerateEndlessExpertActorsCommand());
			}
		}

		#region SettingsFromJSON
		//public AppSettings SavedSettings()
		//{
		//    try
		//    {
		//        var settingsJSON = File.ReadAllText(SettingsJSON);
		//        jsonSerializer = new MessageSerializer();
		//        var settingsClass = jsonSerializer.Deserialize<AppSettings>(settingsJSON);
		//        jsonSerializer = null;
		//        return settingsClass;
		//    }
		//    catch
		//    {
		//        return null;
		//    }
		//}

		//public void LoadSettings()
		//{
		//    var settingsJSON = File.ReadAllText(SettingsJSON);
		//    jsonSerializer = new MessageSerializer();
		//    var settingsClass = jsonSerializer.Deserialize<AppSettings>(settingsJSON);

		//    if (settingsClass.lastState > 0) InitState = settingsClass.lastState;
		//    if (!string.IsNullOrEmpty(settingsClass.contentFileID)) ContentFileID = settingsClass.contentFileID;

		//    craftSettings.SetSettingsView();
		//}

		//public void SaveSettings()
		//{
		//    var oldSettings = SavedSettings();
		//    var appSettings = new AppSettings()
		//    {
		//        lastState = badgeCraft.modelRandomLast != null ? badgeCraft.modelRandomLast.State : oldSettings.lastState, // badgeCraft.CurrentState,
		//        contentFileID = !string.IsNullOrEmpty(ContentFileID) ? ContentFileID : oldSettings.contentFileID
		//    };
		//    jsonSerializer = new MessageSerializer();
		//    var settings = jsonSerializer.Serialize(appSettings);

		//    File.WriteAllText(SettingsJSON, settings);
		//}
		#endregion

		#region OldCode
		//public void SetRegCode(UIInput input)
		//{
		//	regState = long.Parse(input.value);
		//	DebugTWD.Log("Reg code: " + regState);

		//	if (regState == regCode || regState == 12475257538)
		//	{
		//		regStateUpdater.GetComponent<UILabel>().color = Color.green;
		//		//uiUpdater.UpdateCustomContent("YourCodeAccepted");
		//		regStateUpdater.EnCustomText = "Your code is accepted. Application is registered.";
		//		regStateUpdater.RuCustomText = "Ваш код принят. Приложение зарегистрировано!";
		//	}
		//	else
		//	{
		//		regStateUpdater.GetComponent<UILabel>().color = Color.red;
		//		//uiUpdater.UpdateCustomContent("YourCodeWrong");
		//		regStateUpdater.EnCustomText = "Your key is wrong!";
		//		regStateUpdater.RuCustomText = "Ваш код неверный!";
		//	}
		//	regStateUpdater.UpdateContent();
		//}

		//public void ReadRegData()
		//{
		//	regStateUpdater.EnCustomText = "";
		//	var label = regStateUpdater.GetComponent<UILabel>();
		//	label.color = Color.green;

		//	if (SupabaseManager.IsOnline)
		//	{
		//		//DatabaseManager.UpdateLastRun();

		//		//saveLogToSheet.GetUserData(hashId, ReacallUserData);
		//	}
		//	else
		//	{
		//		label.color = Color.red;
		//		regStateUpdater.EnCustomText = "Please turn ON Internet!";
		//		regStateUpdater.RuCustomText = "Пожалуйста включите Интернет!";
		//	}
		//	regStateUpdater.UpdateContent();
		//}

		//private void ReacallUserData(GstuSpreadSheet ss)
		//{
		//	if (ss == null || ss.columns["A"] == null)
		//	{
		//		regStateUpdater.GetComponent<UILabel>().color = Color.red;
		//		regStateUpdater.EnCustomText = "Can't recall user reg data from cloud. Please try again!";
		//		regStateUpdater.RuCustomText = "Не могу прочитать рег данные пользователя. Попробуйте еще раз...";
		//		regStateUpdater.UpdateContent();
		//		saveLogToSheet.IsBuizy = false;
		//		return;
		//	}

		//	GSTU_Cell cellUser = saveLogToSheet.CellUser(ss);

		//	if (cellUser == null)
		//	{
		//		regStateUpdater.GetComponent<UILabel>().color = Color.red;
		//		regStateUpdater.EnCustomText = "No user reg data in cloud";
		//		regStateUpdater.RuCustomText = "Нет рег данных пользователя в облаке.";
		//		regStateUpdater.UpdateContent();
		//		saveLogToSheet.IsBuizy = false;
		//		return;
		//	}

		//	saveLogToSheet.IsBuizy = false;

		//	HashID = cellUser.value;

		//	var Name = ss[HashID, "Name"].value;
		//	FirstRun = ss[HashID, "FirstRun"].value;
		//	TimesRun = int.Parse(ss[HashID, "TimesRun"].value) + 1;
		//	DebugTWD.LogWarning("TimesRun: " + TimesRun, DebugType.System);
		//	TimesConnect = int.Parse(ss[HashID, "TimesGetContent"].value);
		//	IsReged = bool.Parse(ss[HashID, "Regged"].value);
		//	IsBlocked = bool.Parse(ss[HashID, "Blocked"].value);
		//	var ver = ss[HashID, "ClientVersion"].value;
		//	if (ver != "1")
		//	{
		//		if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_Version))
		//		{
		//			var clientVersion = TWDPlayerPrefs.GetString(UserPrefsKeys.Key_Version);

		//			if (ver != clientVersion)
		//			{
		//				OfflineManager.ClientVersion = ver;
		//				OfflineManager.ShortVersion = ver[..ver.LastIndexOf('.')];
		//				TWDPlayerPrefs.SetString(UserPrefsKeys.Key_Version, ver);
		//				CraftSettings.Instance.ClientVersionInput.Set(ver);
		//				DebugTWD.Log("New ClientVersion: " + ver);
		//			}
		//		}
		//	}

		//	ProGuild = bool.Parse(ss[HashID, "Pro"].value);
		//	ProLink = bool.Parse(ss[HashID, "ProEos"].value);

		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_ProGuild, ProGuild.ToString());
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_ProLink, ProLink.ToString());

		//	var eosID = ss[HashID, "EosUserId"].value;
		//	var guild = ss[HashID, "Guild"].value;
		//	var code = ss[HashID, "Code"].value;

		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_HashID, HashID);
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_Name, Name);
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_FirstRun, FirstRun);
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_TimesRun, TimesRun.ToString());
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_TimesConnect, TimesConnect.ToString());
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_Regged, IsReged.ToString());
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_Blocked, IsBlocked.ToString());
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_GoogleID, eosID);
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_RegCode, code);

		//	TWDPlayerPrefs.Save();
		//	DebugTWD.Log("Save TWDPlayerPrefs for " + Name);

		//	if (IsReged)
		//	{
		//		regInput.Set(code);
		//	}
		//	regStateUpdater.GetComponent<UILabel>().color = Color.green;
		//	regStateUpdater.EnCustomText = "Reg data for player " + Name + " read and saved";
		//	regStateUpdater.RuCustomText = "Регистрационные данные для игрока " + Name + " прочитаны и сохранены";
		//	regStateUpdater.UpdateContent();

		//	if (IsReged)
		//	{
		//		CloseRegPopup();
		//	}
		//	else
		//	{
		//		var logTrial = "";
		//		if (language != Language.Ru)
		//		{
		//			logTrial = "Your Mod Application is not registered! Enter code";
		//		}
		//		else
		//		{
		//			logTrial = "Ваша версия мода не зарегестрирована! Введите код";
		//		}
		//		MyTools.UpdateLogPanel(logTrial);
		//	}
		//}

		//public void CloseRegPopup()
		//{
		//	string logTrial;
		//	if (regState == regCode || regState == 12475257538 || IsReged)
		//	{
		//		IsReged = true;
		//		TWDPlayerPrefs.SetString(UserPrefsKeys.Player_Regged, "true");
		//		TWDPlayerPrefs.Save();

		//		if (language != Language.Ru)
		//		{
		//			logTrial = "Your Mod Application is registered!";
		//		}
		//		else
		//		{
		//			logTrial = "Ваша версия мода зарегестрирована!";
		//		}
		//		MyTools.UpdateLogPanel(logTrial);
		//		craftSettings.GameStatus.gameObject.SetActive(false);

		//		DebugTWD.Log(logTrial);

		//		if (craftSettings.craftingPanelTween.GetComponent<UIPanel>().alpha == 0)
		//		{
		//			craftSettings.craftingPanelTween.PlayReverse();
		//		}
		//	}
		//	else
		//	{
		//		if (language != Language.Ru)
		//		{
		//			logTrial = "Reg code is wrong. Try Again.";
		//		}
		//		else
		//		{
		//			logTrial = "Введенный код неверный. Попробуй еще.";
		//		}
		//		MyTools.UpdateLogPanel(logTrial);
		//		DebugTWD.LogWarning(logTrial);
		//	}
		//	craftSettings.RegPopup.gameObject.SetActive(false);
		//}

		//public IEnumerator GetPlayerFromDrive(ContentSource contentSource)
		//{
		//	string url = SetContentUrl(ContentFileID, 40, addPrefix : true);

		//	if (string.IsNullOrEmpty(url))
		//	{
		//		DebugTWD.LogWarning("Invalid or empty file ID");
		//		MyTools.UpdateLogPanel("Invalid or empty file ID");
		//		yield break;
		//	}

		//	UnityWebRequest request = UnityWebRequest.Get(url);

		//	yield return request.SendWebRequest();

		//	if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
		//	{
		//		DebugTWD.LogWarning(request.result);
		//		MyTools.UpdateLogPanel(request.result.ToString());
		//	}
		//	else
		//	{
		//		string content = request.downloadHandler.text;

		//		if (DoSavePlayerJson)
		//		{
		//			string path = PlayerContentFolder + "content.json";
		//			SaveToFile(content, path, false);
		//			DebugTWD.Log("Player Content saved to: " + path);
		//		}

		//		StartCoroutine(LoadPlayerContent(content, contentSource));
		//	}

		//	request.Dispose();
		//}


		//private IEnumerator CheckCurrentPlayer()
		//{
		//	if (!craftSettings.IsGoogleSheetConnected && (!IsReged || IsBlocked))
		//	{
		//		MyTools.UpdateLogPanel("Please enable Internet connection to verify your access");
		//		//yield return new WaitUntil(() => craftSettings.IsInternetOn);

		//		float startTime = Time.realtimeSinceStartup;
		//		while (!craftSettings.IsGoogleSheetConnected)
		//		{
		//			if (Time.realtimeSinceStartup - startTime > 60f)
		//			{
		//				MyTools.UpdateLogPanel("You time is done. Good bye");
		//				craftSettings.craftingPanelTween.SetOnFinished(() => { residencePopup.OpenAtTabIndex(0); });
		//				craftSettings.craftingPanelTween.PlayForward();
		//				yield break;
		//			}
		//			yield return null;
		//		}
		//	}

		//	Feedback = "null";

		//	if (craftSettings.IsGoogleSheetConnected)
		//	{
		//		saveLogToSheet.GetUserData(HashID, saveLogToSheet.OnGetUserData);

		//		while (saveLogToSheet.IsBuizy)
		//		{
		//			if (SpreadsheetManager.IsError)
		//			{
		//				if (language == Language.Ru)
		//				{
		//					MyTools.UpdateLogPanel("Не могу подключиться к гугл таблице с регистрационными данными. Пытаюсь переподключиться...");
		//				}
		//				else
		//				{
		//					MyTools.UpdateLogPanel("Unable to Retreive data from google sheet. Trying reconnect...");
		//				}

		//				DebugTWD.Log("try to do twice " + HashID);
		//				saveLogToSheet.GetUserData(HashID, saveLogToSheet.OnGetUserData);
		//				SpreadsheetManager.IsError = false;
		//				yield break;
		//			}
		//			yield return null;
		//		}

		//		yield return new WaitUntil(() => !saveLogToSheet.IsBuizy);

		//		DebugTWD.LogWarning("Получили данные игрока");

		//		string sysName = UserPrefsKeys.UserDeviceName ?? "null";
		//		string socialName = EOSLogin.GetUserDisplayName();

		//		UserInfo = socialName + '\n' + sysName;
		//		UnityEngine.Debug.Log("UserInfo " + UserInfo);

		//		var currentUser = PostGreManager.Instance.CurrentUser;

		//		if (currentUser != null)
		//		{
		//			if (!IsPinId)
		//			{
		//				ProGuild = currentUser.ProGuild;
		//				ProLink = currentUser.ProLink;
		//				TWDPlayerPrefs.SetString(UserPrefsKeys.Player_ProGuild, ProGuild.ToString());
		//				TWDPlayerPrefs.SetString(UserPrefsKeys.Player_ProLink, ProLink.ToString());
		//			}

		//			IsBlocked = currentUser.Blocked;
		//			IsReged = currentUser.Regged;

		//			TimesConnect = TimesConnect >= currentUser.TimesConnect ? TimesConnect : currentUser.TimesConnect;
		//			TimesRun = TimesRun >= currentUser.TimesRun ? TimesRun : currentUser.TimesRun + 1;

		//			if (contentSource != ContentSource.Local && OfflineManager.Instance.IsPlayerLoaded)
		//			{
		//				TimesConnect++;
		//				TWDPlayerPrefs.SetInt(UserPrefsKeys.Player_TimesConnect, TimesConnect);
		//			}
		//			TWDPlayerPrefs.Save();

		//			DebugTWD.LogWarning("TimesRun: " + TimesRun, DebugType.System);

		//			if (DateTime.TryParseExact(currentUser.FirstRun, UserPrefsKeys.TimeFormat, default, default, out DateTime result))
		//			{
		//				if (DateTime.TryParseExact(FirstRun, UserPrefsKeys.TimeFormat, default, default, out DateTime resultLoad))
		//				{
		//					FirstRun = resultLoad < result ? FirstRun : currentUser.FirstRun;
		//				}
		//			}
		//			else
		//			{
		//				FirstRun = currentUser.FirstRun;
		//			}

		//			if (currentUser.Feedback != "null" && IsFeedbackReaded)
		//			{
		//				IsFeedbackReaded = false;
		//				Feedback = currentUser.Feedback;
		//				TWDPlayerPrefs.SetString(UserPrefsKeys.Player_Feedback, Feedback);
		//				TWDPlayerPrefs.Save();
		//			}

		//			if (craftSettings.craftingPanelTween.GetComponent<UIPanel>().alpha == 0 && IsReged)
		//			{
		//				craftSettings.craftingPanelTween.PlayReverse();
		//			}
		//		}
		//	}

		//	if (Feedback != "null")
		//	{
		//		IsFeedbackReaded = false;
		//		craftSettings.SetupGlowFeedback(isOn: true);
		//	}

		//	if (IsSaveSettings)
		//	{
		//		SaveSettings(IsForExit: false);
		//		if (!craftSettings.IsGoogleSheetConnected) yield break;
		//		saveLogToSheet.IsBuizy = true;

		//		SavingUser();

		//		saveLogToSheet.GetUserData(HashID, saveLogToSheet.OnSetUserData);

		//		while (saveLogToSheet.IsBuizy)
		//		{
		//			if (SpreadsheetManager.IsError)
		//			{
		//				DebugTWD.Log("try to do twice");
		//				saveLogToSheet.GetUserData(HashID, saveLogToSheet.OnSetUserData);
		//				SpreadsheetManager.IsError = false;
		//			}
		//			yield return null;
		//		}

		//		yield return new WaitUntil(() => !saveLogToSheet.IsBuizy);

		//		DebugTWD.Log("Записали данные игрока");
		//	}
		//}

		//private IEnumerator CheckSignalRConnect()
		//{
		//	var time = DateTime.Now;
		//	while (DateTime.Now < time + TimeSpan.FromSeconds(20))
		//	{
		//		yield return null;
		//	}
		//	if (saveLogToSheet.IsBuizy)
		//	{
		//		saveLogToSheet.IsBuizy = false;
		//		DebugTWD.Log("Время вышло. Google таблица с рег данными не отвечает. Пропустим этап");
		//	}
		//}

		//public void SaveGuildNameToSheet(string guildId, string guildName)
		//{
		//	var hashID = Player.HashedId;
		//	TWDPlayerPrefs.SetString(UserPrefsKeys.Player_GuildID, guildId);
		//          TWDPlayerPrefs.SetString(UserPrefsKeys.Player_GuildName, guildName);

		//	saveLogToSheet.IsBuizy = true;
		//	saveLogToSheet.GetUserData(hashID, saveLogToSheet.OnSendGuildName);
		//}

		private IEnumerator GetCountryCodeByIP()
		{
			yield return null;
			string text = new WebClient().DownloadString("https://api.ipify.org");
			string uri = "https://ipapi.co/" + text + "/json/";
			using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
			yield return webRequest.SendWebRequest();
			if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
			{
				CountryCode = IpApiData.CreateFromJson(webRequest.downloadHandler.text).country_code;
			}
			else
			{
				CountryCode = "null";
			}
		}
		#endregion

		#region GoogleDriveFiles
#if !MOBILE
		//        private void UpdateAuth()
		//        {
		//            AuthController.CancelAuth();

		//            CraftSettings.UpdateLogPanel("Cancel Auth");

		//            request = GoogleDriveAbout.Get();
		//            request.Fields = new List<string> { "user" };
		//            request.Send().OnDone += OnUpdateAuth;
		//        }

		//        private void OnUpdateAuth(UnityGoogleDrive.Data.About response)
		//        {
		//            if (response != null)
		//            {
		//                string userName = response.User.DisplayName;
		//                string userMail = response.User.EmailAddress;
		//                CraftSettings.UpdateLogPanel("Hello " + userName);
		//                GetFile();
		//            }
		//        }

		//        public void GetFile()
		//        {
		//            if (string.IsNullOrEmpty(ContentFileID))
		//            {
		//                string log = "fileId is empty";
		//                CraftSettings.UpdateLogPanel(log);
		//                Debug.Log(log);
		//                return;
		//            }

		//            Debug.Log(ContentFileID);
		//            requestGet = GoogleDriveFiles.Get(ContentFileID);
		//            requestGet.Fields = new List<string> { "name, size, createdTime, parents" };
		//            requestGet.Send().OnDone += OnFileGet;
		//        }

		//        public void OnFileGet(UnityGoogleDrive.Data.File file)
		//        {
		//            result = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}\n Parents: {3}",
		//                file.Name,
		//                file.Size * .000001f,
		//                file.CreatedTime,
		//                file.Parents);

		//            Debug.Log("Google Drive file Info : " + result);

		//            GFileParents = file.Parents;

		//            DownloadFile();
		//        }

		//        public void DownloadFile()
		//        {
		//            if (string.IsNullOrEmpty(ContentFileID))
		//            {
		//                Debug.Log("FileId is empty");
		//                return;
		//            }
		//            requestGet = GoogleDriveFiles.Download(ContentFileID);
		//            //request.Fields = new List<string> { "name, size, createdTime" };
		//            requestGet.Send().OnDone += OnDownloadContent;
		//        }

		//        private void OnDownloadContent(UnityGoogleDrive.Data.File file)
		//        {
		//            var content = System.Text.Encoding.UTF8.GetString(file.Content);
		//            StartCoroutine(LoadPlayerContent(content));
		//        }

		//        public void UploadLog(string jsonContent)
		//        {
		//            if (string.IsNullOrEmpty(jsonContent))
		//            {
		//                Debug.Log("Badge Log is empty");
		//                return;
		//            }
		//            if (GFileParents == null || GFileParents.Count == 0)
		//            {
		//                Debug.Log("content.json parent folder is unknown");
		//                return;
		//            }
		//            //var content = File.ReadAllBytes(UploadFilePath);
		//            var content = Encoding.UTF8.GetBytes(jsonContent);
		//            var file = new UnityGoogleDrive.Data.File { Name = "BadgesCraft" + badgeCraft.timeStampStatic + ".json", Content = content };
		//            //file.Parents = new List<string> { "appDataFolder" };
		//            file.Parents = GFileParents;
		//            requestSet = GoogleDriveFiles.Create(file);
		//            requestSet.Fields = new List<string> { "id", "name", "size", "createdTime" };
		//            requestSet.Send().OnDone += OnUploadLog;
		//        }

		//        private void OnUploadLog(UnityGoogleDrive.Data.File file)
		//        {
		//            result = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}\nID: {3}",
		//                file.Name,
		//                file.Size * .000001f,
		//                file.CreatedTime,
		//                file.Id);
		//            Debug.Log(result);
		//        }

		//        public static void MoveFileToDownloadsFolder()
		//        {

		//            string logfilename = "/content.json";
		//            string sourcePath = Application.persistentDataPath + logfilename;

		//            // Get the path to the Downloads folder based on the platform
		//            string downloadsPath = "";
		//#if UNITY_ANDROID && !UNITY_EDITOR
		//                downloadsPath = "/storage/emulated/0/Download/" +  "CraftMachine" + logfilename;
		//#else
		//    downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CraftMachine");
		//#endif
		//            if (File.Exists(downloadsPath))
		//            {
		//                CraftSettings.UpdateLogPanel("File moved to Downloads folder: " + sourcePath);

		//                File.Copy(downloadsPath, sourcePath, true);
		//                Debug.Log("File moved to Downloads folder: " + sourcePath);
		//            }
		//            else
		//            {
		//                CraftSettings.UpdateLogPanel("File not found: " + downloadsPath);

		//                Debug.LogError("File not found: " + downloadsPath);
		//            }
		//        }
#endif
		#endregion

	}

	public enum ContentSource
	{
		Epic,
		Steam,
		Google,
		Local
	}
}
