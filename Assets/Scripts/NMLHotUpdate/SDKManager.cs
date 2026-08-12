using System;
using System.Collections.Generic;
using BaseModel;
using BaseModel.ContentTypes;
using Decagames.Externals.SingularSDK;
using Decagames.Externals.ThinkingAnalytics;
using Firebase.Analytics;
using NextGames.Externals;
using NextGames.Externals.Core;
using NextGames.Externals.FirebaseSdk;
using TWD.Externals;
using TWDModel;
using ThinkingAnalytics;
using UnityEngine;

public class SDKManager : SingularityMonoBehaviour<SDKManager>
{
	private const string PlayerLevelAttribute = "playerLevel";

	private const string PlayerNameAttribute = "playerName";

	private const string CouncilLevelAttribute = "councilLevel";

	private const string PlayerHashedId = "playerHashedId";

	private const string InstallationId = "installationId";

	private Action<string> _onPushTokenReceived;

	private string _pushToken = "";

	private ExternalManager _externalManager;

	private DateTime _initTime;

	private ExternalThinkingAnalyticsConfiguration _externalThinkingAnalyticsConfiguration;

	private DeepLinkActionResolver _deepLinkActionResolver;

	private UnityMainThreadDispatcher _unityMainThreadDispatcher;

	public ExternalManager ExternalManager => _externalManager;

	public FirebaseManager FirebaseManager => _externalManager?.GetService<FirebaseManager>();

	public ZendeskManager ZendeskManager => ZendeskManager.GetInstance();

	public SingularSDKManager SingularSDKManager => _externalManager?.GetService<SingularSDKManager>();

	public ThinkingAnalyticsManager ThinkingAnalyticsManager => _externalManager?.GetService<ThinkingAnalyticsManager>();

	public ExternalAnalytics ExternalAnalytics => _externalManager?.GetService<ExternalAnalytics>();

	public NextActivity NextActivity => _externalManager?.GetService<NextActivity>();

	public SKAdNetworkController SkAdNetworkController => _externalManager?.SKAdNetwork;

	public UnityMainThreadDispatcher UnityMainThreadDispatcher => _unityMainThreadDispatcher;

	public event ServiceTokensSetDelegate OnServiceTokensSet;

	protected override void AwakeInternal()
	{
		base.AwakeInternal();
		AddExternalManager();
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	public void OnPlayerLoaded()
	{
		GameManager gameManager = GameManager.Instance;
		if (!(gameManager == null))
		{
			SetUserIdService(TWDPlayerPrefs.GetString("HashedId"));
			ZendeskManager.SetUserName(gameManager.playerModel.Name);
			new Dictionary<string, object>
			{
				{
					"councilLevel",
					gameManager.playerModel.Camp.GetCouncilLevel()
				},
				{
					"playerLevel",
					gameManager.playerModel.Level
				},
				{
					"playerName",
					gameManager.playerModel.Name
				},
				{
					"playerHashedId",
					TWDPlayerPrefs.GetString("HashedId")
				},
				{
					"installationId",
					TWDPlayerPrefs.GetString("InstallationId")
				}
			};
		}
	}

	private void InitThinkingAnalytics()
	{
		LoginRequest loginRequest = GameManager.Instance.LoginRequest;
		ThinkingAnalyticsAPI.Identify(loginRequest.InstallationId);
		SetAccountId(loginRequest.Identification);
		Debug.LogError("--------------InstallationId:" + loginRequest.InstallationId + "--------Identification:" + loginRequest.Identification + "--------------");
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("client_version", loginRequest.ClientVersion);
		dictionary.Add("country_code", loginRequest.Device.CountryCode);
		dictionary.Add("device", loginRequest.Device.Device);
		dictionary.Add("graphics_device_name", loginRequest.Device.GraphicsDeviceName);
		dictionary.Add("graphics_device_version", loginRequest.Device.GraphicsDeviceVersion);
		dictionary.Add("wifi", loginRequest.Device.Wifi);
		dictionary.Add("device_id", loginRequest.Device.DeviceId);
		dictionary.Add("channel", "epic");
		ThinkingAnalyticsManager.SetSuperProperties(dictionary);
		ThinkingAnalyticsAPI.SetDynamicSuperProperties(new DynamicProp());
		ThinkingAnalyticsManager.EnableAutoTrack(AUTO_TRACK_EVENTS.APP_START | AUTO_TRACK_EVENTS.APP_END | AUTO_TRACK_EVENTS.APP_CRASH | AUTO_TRACK_EVENTS.APP_INSTALL);
		TDFirstEvent analyticsEvent = new TDFirstEvent("device_activate", new Dictionary<string, object> {
		{
			"device_activate_time",
			DateTime.UtcNow
		} });
		ThinkingAnalyticsManager.SendEvent(analyticsEvent);
		Loading((DateTime.UtcNow - _initTime).TotalSeconds);
		Channel();
	}

	public void SetAccountId(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			ThinkingAnalyticsAPI.Login(id);
		}
	}

	public void SetSingularAccountId(string id)
	{
		Debug.LogError("singular account id set =====" + id);
		if (!string.IsNullOrEmpty(id))
		{
			SingularSDKManager.SetCustomUserId(id);
		}
	}

	public void InitializeExternalSdks()
	{
		if (_externalManager != null)
		{
			_externalManager.AddService<FirebaseManager>();
			_externalManager.AddService<SingularSDKManager>();
			_externalManager.AddService<ThinkingAnalyticsManager>();
			_externalManager.GetService<ExternalAnalytics>().LanguageChanged("en");
			_deepLinkActionResolver = base.gameObject.GetComponent<DeepLinkActionResolver>();
			_unityMainThreadDispatcher = base.gameObject.GetComponent<UnityMainThreadDispatcher>();
			StartServicesSdks();
			InitThinkingAnalytics();
		}
		else
		{
			Debug.LogError("ExternalManager not initialised");
		}
	}

	private void StartServicesSdks()
	{
		_deepLinkActionResolver.Initialize();
		FirebaseManager.StartService(_externalManager);
		SingularSDKManager.StartService(_externalManager);
		ThinkingAnalyticsManager.StartService(_externalManager);
	}

	public void AddPushTokenListener(Action<string> onPushTokenReceived)
	{
		_onPushTokenReceived = (Action<string>)Delegate.Remove(_onPushTokenReceived, onPushTokenReceived);
		_onPushTokenReceived = (Action<string>)Delegate.Combine(_onPushTokenReceived, onPushTokenReceived);
		string pushToken = _externalManager.PushToken;
		if (!string.IsNullOrEmpty(pushToken))
		{
			_onPushTokenReceived?.Invoke(pushToken);
		}
	}

	public void OnPushTokenReceived(string pushToken)
	{
		if (!string.IsNullOrEmpty(pushToken))
		{
			_onPushTokenReceived?.Invoke(pushToken);
		}
	}

	private void SetUserIdService(string id)
	{
		SetAppsFlyerAgain();
		ZendeskManager.SetUserToken(id);
		FirebaseManager.SetUserToken(id);
		this.OnServiceTokensSet?.Invoke(new Dictionary<string, string>
		{
			{ "LeanplumToken", id },
			{ "HelpshiftToken", id },
			{ "FirebaseToken", id },
			{ "AppsflyerToken", id }
		});
	}

	public void SetUserName(string name)
	{
		if (!name.Equals("Unknown"))
		{
			ZendeskManager.SetUserName(name);
			ZendeskManager.LogUserData();
		}
	}

	public void LaunchFromURLInternal(string url)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = url.Split('&');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Contains("="))
			{
				string[] array2 = array[i].Split('=');
				if (array2 != null && array2.Length == 2)
				{
					dictionary[array2[0]] = array2[1];
				}
			}
		}
		if (dictionary.ContainsKey("faq"))
		{
			ZendeskManager.ShowFAQs(null, (!dictionary.ContainsKey("tags")) ? null : new string[1] { dictionary["tags"] });
		}
	}

	public void OnLanguageChanged(string language)
	{
		ExternalAnalytics.LanguageChanged(language);
	}

	public void ShowFAQs()
	{
		ZendeskManager.ShowFAQs();
	}

	public string GetATTAnswerByValue(int value)
	{
		return value switch
		{
			0 => "not determined",
			1 => "restricted",
			2 => "denied",
			3 => "authorised",
			_ => string.Empty,
		};
	}

	public void SentCouncilLevelData(int level)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("singular_level", level.ToString());
		SingularSDKManager.SendEvent("council_level", dictionary);
		GameManager.Instance.RequestPltv();
	}

	public void SentWebShopData(List<WebshopBuyedBundleSingularSyncData> webData)
	{
		if (webData == null || webData.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < webData.Count; i++)
		{
			WebshopBuyedBundleSingularSyncData webshopBuyedBundleSingularSyncData = webData[i];
			if (webshopBuyedBundleSingularSyncData != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("BundleId", webshopBuyedBundleSingularSyncData.BundleId);
				dictionary.Add("PaidPrice", webshopBuyedBundleSingularSyncData.PaidPrice);
				dictionary.Add("BuyTime", webshopBuyedBundleSingularSyncData.BuyTime);
				SingularSDKManager.SendEvent("webshop_bundle", dictionary);
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("productId", webshopBuyedBundleSingularSyncData.BundleId);
				dictionary2.Add("price", webshopBuyedBundleSingularSyncData.PaidPrice);
				SingularSDKManager.CustomRevenue("total_revenue", "USD", webshopBuyedBundleSingularSyncData.PaidPrice, dictionary2);
			}
		}
	}

	public void SentWebShopData(List<BuyBundleResultInfo> webData)
	{
		if (webData == null || webData.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < webData.Count; i++)
		{
			BuyBundleResultInfo buyBundleResultInfo = webData[i];
			if (buyBundleResultInfo != null && !(buyBundleResultInfo.BundleId == "TWD_DAILYFREE_BUNDLE1_WB"))
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("BundleId", buyBundleResultInfo.BundleId);
				dictionary.Add("PaidPrice", buyBundleResultInfo.PayPrice);
				dictionary.Add("BuyTime", buyBundleResultInfo.BuyTime);
				SingularSDKManager.SendEvent("webshop_bundle", dictionary);
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("productId", buyBundleResultInfo.BundleId);
				dictionary2.Add("price", buyBundleResultInfo.PayPrice);
				SingularSDKManager.CustomRevenue("total_revenue", "USD", buyBundleResultInfo.PayPrice, dictionary2);
			}
		}
	}

	public void AdWatched()
	{
		SingularSDKManager.SendEvent("ad_watched");
		GameManager.Instance.RequestPltv();
	}

	public void OutpostBuilt()
	{
		SingularSDKManager.SendEvent("build_outpost");
		GameManager.Instance.RequestPltv();
	}

	public void PlayStoryMission(string episode, string missionNumber, string difficulty)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Episode_Number", episode);
		dictionary.Add("Episode_Difficulty", difficulty);
		dictionary.Add("Mission_Number", missionNumber);
		SingularSDKManager.SendEvent("chapter_mission", dictionary);
		GameManager.Instance.RequestPltv();
	}

	public void StoryMissionCompleted(int chapter, int missionNumber, int difficulty)
	{
		if (difficulty <= 1 && missionNumber <= 1 && (chapter == 2 || chapter == 6 || chapter == 7 || chapter == 9 || chapter == 10 || chapter == 11))
		{
			SingularSDKManager.SendEvent($"chapter{chapter}_mission{missionNumber}");
			GameManager.Instance.RequestPltv();
		}
	}

	private void AddExternalManager()
	{
		_externalManager = base.gameObject.AddComponent<ExternalManager>();
		_externalManager.Initialize();
		_externalManager.AddPushTokenListener(OnPushTokenReceived);
		LoadThinkingAnalyticsAPIConfiguration();
	}

	public void ShowDataRequest()
	{
		ZendeskManager.ShowFAQs();
	}

	private void LoadThinkingAnalyticsAPIConfiguration()
	{
		base.gameObject.AddComponent<ThinkingAnalyticsAPI>();
		_externalThinkingAnalyticsConfiguration = Resources.Load<ExternalThinkingAnalyticsConfiguration>("ExternalThinkingAnalyticsData");
		if (_externalThinkingAnalyticsConfiguration == null)
		{
			throw new InvalidOperationException("External ThinkingAnalytics Configuration file is null");
		}
		string appId = _externalThinkingAnalyticsConfiguration.releaseAppId;
		if (!GameManager.ActiveBranch.Contains("release"))
		{
			appId = _externalThinkingAnalyticsConfiguration.testAppId;
		}
		ThinkingAnalyticsAPI.Token token = new ThinkingAnalyticsAPI.Token(appId, _externalThinkingAnalyticsConfiguration.serverUrl);
		token.timeZone = ThinkingAnalyticsAPI.TATimeZone.UTC;
		ThinkingAnalyticsAPI.StartThinkingAnalytics(token);
		if (!OfflineManager.IsCustomLogin) ThinkingAnalyticsAPI.CalibrateTimeWithNtp("time.apple.com");
		ThinkingAnalyticsAPI.TimeEvent("loading");
		ThinkingAnalyticsAPI.TimeEvent("logout_client");
		_initTime = DateTime.UtcNow;
		SetAppsFlyer();
	}

	private void SetAppsFlyer()
	{
		try
		{
			string distinctId = ThinkingAnalyticsAPI.GetDistinctId();
			new Dictionary<string, string>().Add("ta_distinct_id", distinctId);
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager SetAppsFlyer fail:{arg}");
		}
	}

	private void SetAppsFlyerAgain()
	{
		try
		{
			string identification = GameManager.Instance.LoginRequest.Identification;
			string distinctId = ThinkingAnalyticsAPI.GetDistinctId();
			new Dictionary<string, string>
			{
				{ "ta_distinct_id", distinctId },
				{ "ta_account_id", identification }
			};
			SetSingularAccountId(identification);
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager SetAppsFlyerAgain fail:{arg}");
		}
	}

	public ThinkingAnalytics.TDPresetProperties GetTdPresetProperties()
	{
		return ThinkingAnalyticsAPI.GetPresetProperties();
	}

	public void LoginClient()
	{
		ThinkingAnalyticsManager.SendEvent("login_client");
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			Channel();
		}
	}

	private string GetConversionData(string label)
	{
		string result = "";
		try
		{
			string.IsNullOrEmpty(PlayerPrefs.GetString("Walking_Dead_ConversionData"));
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager GetConversionData fail:{arg}");
		}
		return result;
	}

	private void Channel()
	{
		try
		{
			string conversionData = GetConversionData("af_status");
			int num = PlayerPrefs.GetInt("Walking_Dead_af_status", 0);
			if (!string.IsNullOrEmpty(conversionData) && num == 0)
			{
				UserSetOnce(new Dictionary<string, object> { { "af_status", conversionData } });
				PlayerPrefs.SetInt("Walking_Dead_af_status", 1);
			}
			string conversionData2 = GetConversionData("media_source");
			int num2 = PlayerPrefs.GetInt("Walking_Dead_media_source", 0);
			if (!string.IsNullOrEmpty(conversionData2) && num2 == 0)
			{
				UserSetOnce(new Dictionary<string, object> { { "media_source", conversionData2 } });
				PlayerPrefs.SetInt("Walking_Dead_media_source", 1);
			}
			string conversionData3 = GetConversionData("campaign");
			int num3 = PlayerPrefs.GetInt("Walking_Dead_campaign", 0);
			if (!string.IsNullOrEmpty(conversionData3) && num3 == 0)
			{
				UserSetOnce(new Dictionary<string, object> { { "campaign", conversionData3 } });
				PlayerPrefs.SetInt("Walking_Dead_campaign", 1);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager Channel fail:{arg}");
		}
	}

	public void AdsWatchClient(AdUsage adUsage, string status)
	{
		try
		{
			ThinkingAnalyticsManager.SendEvent("ads_watch_client", new Dictionary<string, object>
			{
				{
					"ads_usage",
					adUsage.ToString()
				},
				{ "ads_status", status }
			});
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager AdsWatchClient Fail:{arg}");
		}
	}

	private void LogoutClient(double duration)
	{
		if (ThinkingAnalyticsManager == null)
		{
			return;
		}
		try
		{
			ThinkingAnalyticsManager.SendEvent("logout_client", new Dictionary<string, object> { { "duration", duration } });
		}
		catch (Exception arg)
		{
			Debug.LogError($"LogoutClient fail:{arg}");
		}
	}

	public void Loading(double duration)
	{
		ThinkingAnalyticsManager.SendEvent("loading", new Dictionary<string, object>
		{
			{ "loading_id", "gameload" },
			{ "duration", duration }
		});
	}

	public void InterfaceResult(string popupId)
	{
		if ((bool)ThinkingAnalyticsManager)
		{
			ThinkingAnalyticsManager.SendEvent("interface_result", new Dictionary<string, object> { { "interface_id", popupId } });
		}
	}

	public void Recharge(StorePurchaseInfo info)
	{
		if (info == null)
		{
			return;
		}
		try
		{
			BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(info.BundleId);
			BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(info.BundleId);
			new MessageSerializer();
			string value = "";
			if (bundleStoreDefinition.ShopTabIndex == 0)
			{
				value = "bundle";
			}
			if (bundleStoreDefinition.ShopTabIndex == 1)
			{
				value = "resource";
			}
			ThinkingAnalyticsManager.SendEvent("recharge", new Dictionary<string, object>
			{
				{
					"product_value",
					Mathf.FloorToInt(info.Product.PriceUSD * 100f + 0.5f)
				},
				{
					"currency_type",
					info.Product.CurrencyCode
				},
				{
					"pay_amount",
					info.Product.Price
				},
				{
					"pay_amount_usd",
					info.Product.PriceUSD
				},
				{ "product_type", value },
				{ "purchase_channel", info.Store },
				{ "bundle_Id", info.BundleId },
				{
					"product_id",
					info.Product.ProductIdentifier
				},
				{
					"product_detail",
					bundleContentDefinition.RewardEntries.RewardResources
				}
			});
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager Recharge Fail:{arg}");
		}
	}

	public void AdsRevenue(Dictionary<string, object> properties)
	{
		try
		{
			if (properties != null)
			{
				ThinkingAnalyticsManager.SendEvent("ads_revenue", properties);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager IronSourceImpression Fail:{arg}");
		}
	}

	public void TutorialClient(TutorialModel model, string action)
	{
		if (model == null)
		{
			return;
		}
		try
		{
			ThinkingAnalyticsManager.SendEvent("tutorial_client", new Dictionary<string, object>
			{
				{ "part_id", model.CurrentPartId },
				{ "step", model.CurrentStep },
				{ "action", action }
			});
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager TutorialClient Fail:{arg}");
		}
	}

	public void Reload(string reloadId, string message)
	{
		try
		{
			if (ThinkingAnalyticsManager != null)
			{
				ThinkingAnalyticsManager.SendEvent("reload", new Dictionary<string, object>
				{
					{ "reload_id", reloadId },
					{ "message", message }
				});
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager Reload Fail:{arg}");
		}
	}

	public void ActivityJump(string buttonId, int panelType, string functionValue)
	{
		try
		{
			if (ThinkingAnalyticsManager != null)
			{
				ThinkingAnalyticsManager.SendEvent("ActivityNotice_SystemJump", new Dictionary<string, object>
				{
					{ "button_id", buttonId },
					{ "panel_type", panelType },
					{ "function_value", functionValue }
				});
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"SDKManager Reload Fail:{arg}");
		}
	}

	public void UserSetOnce(Dictionary<string, object> data)
	{
		if (data != null)
		{
			ThinkingAnalyticsAPI.UserSetOnce(data);
		}
	}

	public void ReportPurchaseEvent(StorePurchaseInfo purchaseInfo)
	{
		if (!Helpers.IsPCPlatform() && purchaseInfo != null && purchaseInfo.Product != null && purchaseInfo.Transaction != null)
		{
			Parameter[] parameters = new Parameter[4]
			{
				new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"),
				new Parameter(FirebaseAnalytics.ParameterValue, purchaseInfo.Product.PriceUSD),
				new Parameter(FirebaseAnalytics.ParameterTransactionID, purchaseInfo.Transaction.TransactionIdentifier),
				new Parameter(FirebaseAnalytics.ParameterItemID, purchaseInfo.BundleId)
			};
			FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase, parameters);
		}
	}

	public void SendTestFirebase()
	{
		FirebaseAnalytics.LogEvent("in_app_purchase", new Parameter("value", 0.01), new Parameter("currency", "USD"), new Parameter("product_id", "editor_test_product"), new Parameter("debug", "unity_editor"), new Parameter("platform", "editor"));
		FirebaseAnalytics.LogEvent("ad_impression", new Parameter("value", 0.02), new Parameter("currency", "USD"), new Parameter("ad_platform", "admob"), new Parameter("ad_format", "banner"), new Parameter("debug", "unity_editor"));
	}
}
