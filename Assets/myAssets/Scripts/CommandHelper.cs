using BaseModel;
using Client.Connectivity;
using Epic.OnlineServices;
using NextGames.Sdk.AssetBundleManager;
using PlayEveryWare.EpicOnlineServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TwdCustomMod;
using TWDModel;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static GuildManager;

public class CommandHelper : MonoBehaviour
{
	//Scripting Define:
	//TWDMOD
	//Fix_615
	//Fix_Mission
	//AMPLIFY_SHADER_EDITOR
	//FIX_NO_SERVICES - нет покупок, туториалов, видео
	//TWDMOD_TEMP
	//FIX_NO_SOUND
	//FIX_NO_EFFECTS - нет анимации, нет частиц
	//ENABLE_DEBUG_EOSMANAGER
	//ATMOSPHERIC_HEIGHT_FOG


	//мой код:
	//mycode
	//myparams

	//global::Debug.Log - сослаться на глобальный класс

	public static CommandHelper Instance;

	public const string GlobalPath = @"e:\Unity Projects\TWD\GameData\Bloodymary\";
	public const string PlayerFolder = @"";

	//все типы лога. Используются для обычного лога в редакторе
	public List<DebugType> CurrentDebugTypes = new List<DebugType>() { DebugType.All };
	//все типы лога, отображаемые в logPopup
	public List<DebugType> LogUserTypesAll = new List<DebugType>() { DebugType.All };

	//просто для мониторинга
	public List<DebugType> LogUserTypesSelected = new List<DebugType>() { DebugType.All };

	private MessageSerializer JsonSerializer => OfflineManager.JsonSerializer;

	public bool LoadCustomCommand = false;
	public bool IsSaveDataToDisk = false;

	public string codeInput;

	public string customUserEosID;
	public string customUserHashID;
	public bool IsUseCustomID;
	public bool IsPrelogin;

	private void Awake()
	{
		Instance = this;
	}

	public void SetDataID(bool isUse, string hashId = "", string eosId = "")
	{
		IsUseCustomID = isUse;
		if (IsUseCustomID)
		{
			customUserHashID = hashId;
			customUserEosID = eosId;
		}
		else
		{
			customUserHashID = "";
			customUserEosID = "";
		}
	}

	void Start()
	{
	}

	void Update()
	{
		if (LoadCustomCommand)
		{
			LoadCustomCommand = false;
			//LoadAsset();
			//OpenMissionHub();
			//SaveModelMessages();
		}
		if (IsSaveDataToDisk)
		{
			IsSaveDataToDisk = false;
		}
		LogUserTypesSelected = DebugTWD.LogUserTypesSelected;
	}

	public void OpenMissionHub()
	{
		EventManager.NotifyClick(EventManager.EventTypeClick.MissionHub);
        HelpersModel.IsUnlockPVP = true;
		MissionHubPopup.OpenPopup();
		ResidencePopup.Instance.gameObject.SetActive(false);
	}

	public void LoadAsset()
	{
		//string url = string.Format("file://{0}/{1}/{2}", Application.streamingAssetsPath, "AssetBundles", "AssetBundleData.json").Replace('\\', '/');
		//StartCoroutine(AssetBundleCollection.DownloadCollection(url, OnCollectionDownloaded, OnError));
		//GameObject gameObject = UnityUtils.LoadFromAssetBundle(assetName, bundleName) as GameObject;

		StartCoroutine(DownloadAssets());
	}

	private IEnumerator DownloadAssets()
	{
		StartCoroutine(AssetBundleController.Instance.DownloadAssets());
		while (AssetBundleController.Instance.LoadingAssetBundles || !AssetBundleController.Instance.AssetBundlesInitializedAndLoaded)
		{
			yield return null;
		}

		string name = "BuildingShadow";
		string bundle = "buildingsprefabs_dependencies";
		var objects = AssetBundleManager.Instance.LoadAllAssets(bundle);

	}

	// OnClick
	public void LinkByCode()
	{
		if (OfflineManager.IsInternetOn)
		{
			LinkDevicePopup linkDevicePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LinkDevicePopup, MyTools.GetParent()) as LinkDevicePopup;
			if (linkDevicePopup)
			{
				linkDevicePopup.transform.localScale = Vector3.one * 1.25f;
				linkDevicePopup.Open();
			}
		}
	}

	//Outpost бои
	private List<MatchMakingInfo> matches;
	public void QueryMatchMakingOutpost()
	{
		if (!GameManager.Instance.IsConnectedToServer)
		{
			DebugTWD.LogError("No connection, aborted.");
			return;
		}

		GetMatchParams getMatchParams = new GetMatchParams();
		getMatchParams.Count = 20;
		getMatchParams.Parameters = "";
		getMatchParams.Version = DataManager.Instance.GameData.ConfigData.MatchMakingVersion;

		SignalRClient.Instance.RequestCommand("GetMatch", JsonSerializer.Serialize(getMatchParams), OnMatchDataLoaded, waitForResponse: true);
	}
	private void OnMatchDataLoaded(string response)
	{
		if (string.IsNullOrEmpty(response))
		{
			DebugTWD.LogError("No response, aborted.");
			return;
		}
		matches = JsonSerializer.Deserialize<List<MatchMakingInfo>>(response);
		DataManager.Instance.ModelManager.SetMatchData("", matches);
		matches = DataManager.Instance.ModelManager.LastMatchMakingInfos;
		if (matches != null && matches.Count > 0)
		{
			for (int num = matches.Count - 1; num >= 0; num--)
			{
				MatchMakingInfo matchMakingInfo = matches[num];
				if (matchMakingInfo == null || matchMakingInfo.PlayerInformation == null || matchMakingInfo.PlayerInformation.Length == 0 || matchMakingInfo.PlayerHashedId == null || matchMakingInfo.PlayerHashedId.Length == 0)
				{
					matches.RemoveAt(num);
				}
			}
		}
		if (matches == null || matches.Count == 0 || matches[0].PlayerHashedId == "")
		{
			DebugTWD.LogError("No matches, aborted.");
		}

		int playerLevel = DataManager.Instance.Player.Level;
		int playerInfluence = DataManager.Instance.Player.RankingScore;
		FixedPoint influenceWeight = DataManager.Instance.GameData.ConfigData.InfluenceWeightOnMatchMakingSort;

		matches.Sort(delegate (MatchMakingInfo a, MatchMakingInfo b)
		{
			int num2 = Mathf.Abs(a.Rating - playerLevel);
			int num3 = Mathf.Abs(a.SecondaryRating - playerInfluence);
			int num4 = Mathf.Abs(b.Rating - playerLevel);
			int num5 = Mathf.Abs(b.SecondaryRating - playerInfluence);
			int num6 = (int)(num2 + num3 * influenceWeight);
			int num7 = (int)(num4 + num5 * influenceWeight);
			return num6 - num7;
		});

		if (IsSaveDataToDisk)
		{
			string data = JsonSerializer.Serialize(matches);
			var path = GlobalPath + PlayerFolder + "matches.json";
			MyTools.SaveToFile(data, path, append: false);
			DebugTWD.Log("matches saved");
		}
	}
	//

	//CTRL + K + S - окружить регионом

	//Получение данных о гильдии
	//Используется в GWTeamUtils
	//SignalRClient.Instance.RequestCommand("GetGroupInfo", id, OnGuildReceived, waitForResponse: true);
	//
	//Используется для генерации ссылки для открытия WebShop
	//Используется в CampHUD
	//SignalRClient.Instance.RequestCommand("GetWebShopLoginCode", OnGetTransferCode, waitForResponse: true);
	//
	//хз. Покупка бандла
	//Используется в EpicIAPImplementation
	//SignalRClient.Instance.RequestCommand("ValidateAndApplyReceipt", JsonConvert.SerializeObject(currentPurchase), OnValidateReceipt, waitForResponse: true);
	//
	//Получение массива ScoreDataEntry
	//Используется в FriendsScoreDataProvider
	//SignalRClient.Instance.RequestCommand("GetHighScoresBySocialIds", friends, "100", OnHighScoreListFriends, null, waitForResponse: true);
	//
	//Загрузка карты (локации) PVE или PVP
	//Используется в GameManager
	//SignalRClient.Instance.RequestCommand("loadVisit", jsonSerializer.Serialize(loadVisitParams), OnVisitModel, waitForResponse: true);
	//
	//Список очков ВГ всех гильдий. формирует лист ScoreDataEntry
	//Используется в
	//GuildBattleGuildLeaderboardDataProvider,
	//GuildBattleLiveLeaderboardScoreDataProvider,
	//GuildBattlePlayersScoreDataProvider
	//GuildLeaderboardScoreDataProvider
	//OutpostLeaderboardListPanel
	//PlayerEndlessModeScoreDataProvider
	//PlayerLeaderboardScoreDataProvider
	//SignalRClient.Instance.RequestCommand("GetLeaderboard", leaderboardName, max, OnLeaderboardData, null, waitForResponse: true);
	//
	//Поиск гильдий
	//Используется в GuildManager
	//SignalRClient.Instance.RequestCommand("SearchGroups", arg, OnSearchGuilds, waitForResponse: false);
	//
	//Используется в LeaderboardPositionProvider
	//SignalRClient.Instance.RequestCommand("GetLeaderboardPosition", endlessModeLeaderboardName, hashedId, OnGetCurrentLeaderBoardRanking, null, waitForResponse: true);
	//
	//Интересная штука. Изучить.
	//Используется в SocialPlatform и GameCenterManager
	//SignalRClient.Instance.RequestCommand("GetAccount", GetId(), AccountType.ToString(), OnGetAccount, null, waitForResponse: true);
	//SignalRClient.Instance.RequestCommand("LinkAccount", serializer.Serialize(accountInfo), OnLinkAccount, waitForResponse: true);
	//SignalRClient.Instance.RequestCommand("UnlinkAccountAsync", GetId(), AccountType.ToString(), delegate(string message)

	//когда появляется новый герой - проверить CurrencyModel.cs


	//создание MockData из survivorModel
	private SurvivorMockData CreateSurvivorMockData(SurvivorModel survivorModel)
	{
		SurvivorMockData survivorMockData = survivorModel.CreateMockData();
		survivorMockData.AdjustedLevel = (int)GvGModelHelper.GetAdjustedLevelForSurvivor(survivorModel, DataManager.Instance.GameData);
		survivorMockData.TotalDamage = survivorModel.GetHitpoints();
		survivorMockData.OwnerHashedPlayerId = DataManager.Instance.Player.HashedId;
		survivorMockData.MockWeapon = survivorModel.GetWeaponEquipment().CreateMockData();
		survivorMockData.MockArmor = survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor).CreateMockData();
		return survivorMockData;
	}
	//

	private TransferResult transferResult;
	private string confirmationPlayerName = "";
	private string confirmationPlayerLevel = "";

	public string ActionID1;
	public string command1;

	[ContextMenu("Start Action 1")]
	public void StartAction1()
	{
		SignalRClient.Instance.RequestCommand(command1, ActionID1, OnConfirmationGotPlayer1, waitForResponse: true);
	}
	private void OnConfirmationGotPlayer1(string message)
	{
		IDictionary<string, object> dictionary = JsonSerializer.DeserializeObject<IDictionary<string, object>>(message);
		dictionary.TryGetValue("Level", out var value);
		dictionary.TryGetValue("Nickname", out var value2);
		confirmationPlayerLevel = value.ToString();
		confirmationPlayerName = value2 as string;
		DebugTWD.LogWarning($"{confirmationPlayerName} {confirmationPlayerLevel} ready to load");
		DebugTWD.LogWarning("далее отображаем инфу о новом игроке, делая confirmation, или сразу загружаем");
	}

    [ContextMenu("Lod Groups")]
    public void LodGroups()
	{
        var arg = "[\"" + GWTeamUtils.Instance.CustomGuildID + "\"]"; //354c053cba634e7ba9f7c30d2218cf7b
        SignalRClient.Instance.RequestCommand("LoadGroups", arg, OnGroupsAsync, waitForResponse: true);
    }

    protected void OnGroupsAsync(string groupsRespondJson)
    {
        if (SignalRClient.Instance.Statistics.LastErrorType == ErrorType.GuildsOffline || string.IsNullOrEmpty(groupsRespondJson))
        {
            Debug.LogError("LoadGroupsAsync failed: " + SignalRClient.Instance.LastErrorMessage);
            SignalRClient.Instance.ClearError();
            return;
        }
        List<string> list = OfflineManager.JsonSerializer.DeserializeObject<List<string>>(groupsRespondJson);
        if (list.Count == 0)
        {
            return;
        }
        var guildsToLoad = new List<GuildData>();
        for (int i = 0; i < list.Count; i++)
        {
            guildsToLoad.Add(new GuildData
            {
                Id = list[i],
                Json = null
            });
        }
    }
    //линковка по коду
    //1
    public void OnCodeEntered(string code)
	{
		//code = "введенный код, типа Y6TR86BR";
		SignalRClient.Instance.RequestCommand("UseTransferCode", code, OnUseTransferCode, waitForResponse: true);
	}
	//2
	private void OnUseTransferCode(string message)
	{
		transferResult = JsonSerializer.DeserializeObject<TransferResult>(message);
		if (transferResult == null || transferResult.State == TransferResultState.Error)
		{
			DebugTWD.LogWarning("ошибка");
		}
		else if (transferResult.State == TransferResultState.Success)
		{
			Helpers.ExecuteCommand(new LinkDeviceUseCodeCommand());
			SignalRClient.Instance.RequestCommand("GetPlayerDataSubsetByHashedId", transferResult.PlayerHashedId, OnConfirmationGotPlayer, waitForResponse: true);
		}
		else if (transferResult.State == TransferResultState.CodeExpired)
		{
			DebugTWD.LogWarning("ошибка");
		}
		else
		{
			DebugTWD.LogWarning("ошибка");
		}
	}
	//3
	private void OnConfirmationGotPlayer(string message)
	{
		IDictionary<string, object> dictionary = JsonSerializer.DeserializeObject<IDictionary<string, object>>(message);
		dictionary.TryGetValue("Level", out var value);
		dictionary.TryGetValue("Nickname", out var value2);
		confirmationPlayerLevel = value.ToString();
		confirmationPlayerName = value2 as string;
		DebugTWD.LogWarning($"{confirmationPlayerName} {confirmationPlayerLevel} ready to load");
		DebugTWD.LogWarning("далее отображаем инфу о новом игроке, делая confirmation, или сразу загружаем");
		UnlinkAccount(OnReloadAccount, OnReloadAccount);
	}
	//4
	public void UnlinkAccount(Action<bool> successCallback = null, Action<bool> failureCallback = null)
	{
		string userID = EOSLogin.GetAccountUserId().ToString();
		DebugTWD.LogWarning($"Unlink account {userID}");
		SignalRClient.Instance.RequestCommand("UnlinkAccountAsync", userID, AccountType.WindowsEditor.ToString(), delegate (string message)
		{
			if (SignalRClient.Instance.HasError)
			{
				DebugTWD.LogError("UnlinkAccountAsync failed: " + message);
				SignalRClient.Instance.ClearError();
				if (failureCallback != null)
				{
					failureCallback(false);
				}
			}
			else if (successCallback != null)
			{
				successCallback(true);
			}
		}, null, waitForResponse: true);
	}
	//5
	private void OnReloadAccount(bool result)
	{
		if (result)
		{
			Helpers.ExecuteCommand(new LinkDeviceFinishedCommand(transferResult.PlayerId, DataManager.Instance.Player.HashedId));
			LoadNewAccount(transferResult.PlayerId, "LinkDevice");
		}
		else
		{
			DebugTWD.LogWarning("ошибка");
		}
	}
	//6
	public void LoadNewAccount(string userId, string type)
	{
		Helpers.ExecuteCommand(new LoadNewAccountCommand
		{
			Type = type,
			UserId = userId
		});
		//PlayerPrefs.SetString(UserPrefsKeys.Key_HashID, userId);
		//PlayerPrefs.Save();
		DebugTWD.LogWarning("А теперь перезагрузка");
	}
	//

	[ContextMenu("Restart Epic")]
	public IEnumerator RestartEpic()
	{
		if (EOSManager.Instance != null)
		{
			var PlatformInterface = EOSManager.Instance.GetEOSPlatformInterface();
			if (PlatformInterface != null)
			{
				var result = PlatformInterface.CheckForLauncherAndRestart();
				yield return result.IsOperationComplete();
				DebugTWD.Log("Restart EOS succesfull : " + result);
			}
		}
	}

	[ContextMenu("Get Epic Info")]
	public void GetEpicInfo()
	{
		if (EOSManager.Instance != null)
		{
			var PlatformInterface = EOSManager.Instance.GetEOSPlatformInterface();

			if (PlatformInterface != null)
			{
				var EOSAuthInterface = PlatformInterface.GetAuthInterface();
				var acc = EOSAuthInterface.GetLoggedInAccountByIndex(0);
				var count = EOSAuthInterface.GetLoggedInAccountsCount();

				DebugTWD.Log("accs : " + acc.ToString() + " | " + count);

				//EOSAuthInterface.DeletePersistentAuth();
			}
		}
		//EOSManager.Instance.RemovePersistentToken();
	}
}
