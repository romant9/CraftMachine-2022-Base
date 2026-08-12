using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NextGames.Externals.Core;
using Singular;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Decagames.Externals.SingularSDK
{
	public class SingularSDKManager : ExternalSdk
	{
		public delegate void ShortLinkCallback(string data, string error);

		private enum NSType
		{
			STRING = 0,
			INT = 1,
			LONG = 2,
			FLOAT = 3,
			DOUBLE = 4,
			NULL = 5,
			ARRAY = 6,
			DICTIONARY = 7
		}

		private class SingularConfig
		{
			private Dictionary<string, object> _configValues;

			public SingularConfig()
			{
				_configValues = new Dictionary<string, object>();
			}

			public void SetValue(string key, object value)
			{
				if (key != null && !(key.Trim() == "") && value != null)
				{
					_configValues[key] = value;
				}
			}

			public string ToJsonString()
			{
				return JsonConvert.SerializeObject(_configValues);
			}
		}

		private class SingularGlobalProperty
		{
			public string Key { get; set; }

			public string Value { get; set; }

			public bool OverrideExisting { get; set; }

			public SingularGlobalProperty(string key, string value, bool overrideExisting)
			{
				Key = key;
				Value = value;
				OverrideExisting = overrideExisting;
			}
		}

		private const string Tag = "SingularSDK";

		private List<string> _blackListedEvents;

		private List<Regex> _blackListedPatternEvents;

		public string SingularAPIKey = "deca_live_operations_gmbh_5c5283da";

		public string SingularAPISecret = "a739f84c6816e1c98ce3ee14e41f121e";

		public bool InitializeOnAwake = true;

		public bool enableLogging = true;

		public int logLevel = 3;

		private static SingularSDKManager instance = null;

		private const string UNITY_WRAPPER_NAME = "Unity";

		private const string UNITY_VERSION = "5.3.1";

		[Obsolete]
		public bool autoIAPComplete;

		public bool clipboardAttribution;

		public bool SKANEnabled = true;

		public bool manualSKANConversionManagement;

		public int waitForTrackingAuthorizationWithTimeoutInterval;

		public int enableODMWithTimeoutInterval = 5;

		public static string fcmDeviceToken = null;

		public string facebookAppId;

		public bool collectOAID;

		public bool limitedIdentifiersEnabled;

		private static string imei;

		private Dictionary<string, SingularGlobalProperty> globalProperties = new Dictionary<string, SingularGlobalProperty>();

		private static bool? limitDataSharing = null;

		private static string customUserId;

		public long ddlTimeoutSec;

		public long sessionTimeoutSec;

		public long shortlinkResolveTimeout;

		public static bool enableDeferredDeepLinks = true;

		public static string openUri;

		private static ShortLinkCallback shortLinkCallback;

		private const long DEFAULT_SHORT_LINKS_TIMEOUT = 10L;

		private const long DEFAULT_DDL_TIMEOUT = 60L;

		private SingularLinkParams resolvedSingularLinkParams;

		private int resolvedSingularLinkTime;

		private static int cachedDDLMessageTime;

		private static string cachedDDLMessage;

		public static bool endSessionOnGoingToBackground = false;

		public static bool restartSessionOnReturningToForeground = false;

		public static bool batchEvents = false;

		private const string ADMON_REVENUE_EVENT_NAME = "__ADMON_USER_LEVEL_REVENUE__";

		public static string CustomSdid;

		public static SingularLinkHandler registeredSingularLinkHandler = null;

		public static SingularDeferredDeepLinkHandler registeredDDLHandler = null;

		public static SingularConversionValueUpdatedHandler registeredConversionValueUpdatedHandler = null;

		public static SingularConversionValuesUpdatedHandler registeredConversionValuesUpdatedHandler = null;

		public static SingularDeviceAttributionCallbackHandler registeredDeviceAttributionCallbackHandler = null;

		public static SingularSdidAccessorHandler registeredSdidAccessorHandler = null;

		private const string androidNativeMethodName_Revenue = "revenue";

		private const string androidNativeMethodName_CustomRevenue = "customRevenue";

		public static bool Initialized { get; private set; } = false;

		public event OnDeepLinkAction OnDeepLinkAction;

		private void SetListeners()
		{
			LogDebug("SingularSDK externalAnalytics set listeners");
			if (!(base._externalAnalytics == null))
			{
				base._externalAnalytics.SetTutorialCompletedListener(TutorialCompleted);
				base._externalAnalytics.SetLanguageAction(OnLanguageChanged);
				base._externalAnalytics.SetJoinedGuildListener(JoinedGuild);
				base._externalAnalytics.SetLevelUpAction(LevelUp);
				base._externalAnalytics.SetEventListener(SendEventListener);
				base._externalAnalytics.SetPurchaseListener(OnPurchase);
			}
		}

		private void RemoveListeners()
		{
			LogDebug("SingularSDK externalAnalytics remove listeners");
			if (!(base._externalAnalytics == null))
			{
				base._externalAnalytics.RemoveTutorialCompletedListener(TutorialCompleted);
				base._externalAnalytics.RemoveLanguageAction(OnLanguageChanged);
				base._externalAnalytics.RemoveJoinedGuildListener(JoinedGuild);
				base._externalAnalytics.RemoveLevelUpAction(LevelUp);
				base._externalAnalytics.RemoveEventListener(SendEventListener);
				base._externalAnalytics.RemovePurchaseListener(OnPurchase);
			}
		}

		private void Awake()
		{
			instance = this;
		}

		public void OnEnable()
		{
			if (_serviceStarted)
			{
				SetListeners();
			}
		}

		public void OnDisable()
		{
			RemoveListeners();
		}

		private void Initialize()
		{
			LogDebug("SingularSDK: Initialize()");
			InitExternalSingularSDK();
		}

		public void SetBlacklistedEvents(List<string> eventsToBlacklist)
		{
			_blackListedEvents = eventsToBlacklist;
		}

		public void SetBlacklistEventsPatterns(List<Regex> patternEventsToBlacklist)
		{
			_blackListedPatternEvents = patternEventsToBlacklist;
		}

		public new void StartService(ExternalManager ext)
		{
			InitCommon(ext);
			LogDebug("SingularSDK StartService");
			Initialize();
			ext.AddPushTokenListener(base.SetPushToken);
			if (base._externalAnalytics != null)
			{
				SetListeners();
			}
		}

		private void TutorialCompleted()
		{
			SendEvent(ExternalAnalytics.TutorialCompletedEventName);
		}

		private void OnLanguageChanged(string language)
		{
		}

		private void JoinedGuild(string guildid)
		{
			SendEvent(ExternalAnalytics.JoinedGuildEventName);
		}

		private void LevelUp(int level)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("singular_level", level.ToString());
			SendEvent(ExternalAnalytics.LevelUpEventName, dictionary);
		}

		private void OnPurchase(float revenue, string productId, string transactionId, int count)
		{
			TrackPurchase(revenue, productId, transactionId, count);
		}

		private void PurchaseEvent(float revenueUsd, string productId, string transactionId, string eventName, int count)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("singular_currency", "USD");
			dictionary.Add("singular_revenue", revenueUsd.ToString());
			dictionary.Add("productId", productId);
			dictionary.Add("transactionId", transactionId);
			dictionary.Add("count", count.ToString());
			SendEvent(eventName, dictionary);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("productId", productId);
			dictionary2.Add("price", revenueUsd.ToString());
			CustomRevenue("total_revenue", "USD", revenueUsd, dictionary2);
		}

		private void TrackPurchase(float revenueUsd, string productId, string transactionId, int count)
		{
			PurchaseEvent(revenueUsd, productId, transactionId, "singular_purchase", count);
		}

		private void SendEventListener(string eventName, Dictionary<string, string> eventData)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, string> eventDatum in eventData)
			{
				dictionary[eventDatum.Key] = eventDatum.Value;
			}
			SendEvent(eventName, dictionary);
		}

		public void SendEvent(string eventName, Dictionary<string, object> callBackParameters = null, Dictionary<string, string> partnerParameters = null)
		{
			try
			{
				if (callBackParameters == null)
				{
					callBackParameters = new Dictionary<string, object>();
				}
				if (_blackListedEvents != null && _blackListedEvents.Contains(eventName))
				{
					LogDebug("SingularSDKwill ignore blacklisted event:" + eventName);
					return;
				}
				if (_blackListedPatternEvents != null)
				{
					foreach (Regex blackListedPatternEvent in _blackListedPatternEvents)
					{
						if (blackListedPatternEvent.IsMatch(eventName))
						{
							LogDebug("SingularSDKwill ignore blacklisted prefixed event:" + eventName);
							return;
						}
					}
				}
				LogDebug("SingularSDK: Sending event: " + eventName);
				Event(callBackParameters, eventName);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("{0} sent event fail:{1}", "SingularSDK", arg));
			}
		}

		private void InitExternalSingularSDK()
		{
			if (!BuildConfiguration.Active.Branch.Contains("release"))
			{
				SingularUnityLogger.EnableLogging(enableLogging);
				SingularUnityLogger.SetLogLevel(logLevel);
				SingularUnityLogger.LogDebug($"SingularSDK Awake, InitializeOnAwake={InitializeOnAwake}");
			}
			if (!Application.isEditor && !(instance == null) && InitializeOnAwake)
			{
				SingularUnityLogger.LogDebug("Awake : calling Singular Init");
				InitializeSingularSDK();
			}
		}

		public static void InitializeSingularSDK()
		{
			if (Initialized)
			{
				return;
			}
			if (!instance)
			{
				SingularUnityLogger.LogError("SingularSDK InitializeSingularSDK, no instance available - cannot initialize");
				return;
			}
			SingularUnityLogger.LogDebug($"SingularSDK InitializeSingularSDK, APIKey={instance.SingularAPIKey}");
			if (!Application.isEditor)
			{
				BuildSingularConfig();
				Initialized = true;
			}
		}

		public static void createReferrerShortLink(string baseLink, string referrerName, string referrerId, Dictionary<string, string> passthroughParams, ShortLinkCallback completionHandler)
		{
			shortLinkCallback = completionHandler;
		}

		private static SingularConfig BuildSingularConfig()
		{
			SingularConfig singularConfig = new SingularConfig();
			singularConfig.SetValue("apiKey", instance.SingularAPIKey);
			singularConfig.SetValue("secret", instance.SingularAPISecret);
			singularConfig.SetValue("shortlinkResolveTimeout", (instance.shortlinkResolveTimeout == 0L) ? 10 : instance.shortlinkResolveTimeout);
			singularConfig.SetValue("globalProperties", instance.globalProperties);
			singularConfig.SetValue("sessionTimeoutSec", instance.sessionTimeoutSec);
			singularConfig.SetValue("customSdid", CustomSdid);
			return singularConfig;
		}

		public void Update()
		{
		}

		private static bool StartSingularSession(SingularConfig config)
		{
			_ = Application.isEditor;
			return false;
		}

		public static bool StartSingularSessionWithLaunchOptions(string key, string secret, Dictionary<string, object> options)
		{
			_ = Application.isEditor;
			return false;
		}

		public static bool StartSingularSessionWithLaunchURL(string key, string secret, string url)
		{
			_ = Application.isEditor;
			return false;
		}

		public static void RestartSingularSession(string key, string secret)
		{
			_ = Application.isEditor;
		}

		public static void EndSingularSession()
		{
			_ = Application.isEditor;
		}

		public static void Event(string name)
		{
			if (Initialized)
			{
				_ = Application.isEditor;
			}
		}

		public static void Event(Dictionary<string, object> args, string name)
		{
			if (Initialized)
			{
				_ = Application.isEditor;
			}
		}

		public static void Event(string name, params object[] args)
		{
			if (Initialized)
			{
				_ = Application.isEditor;
			}
		}

		public static void SetDeviceCustomUserId(string customUserId)
		{
			if (!Application.isEditor)
			{
				_ = Initialized;
			}
		}

		public static void SetAge(int age)
		{
			if (Initialized && Mathf.Clamp(age, 0, 100) != age)
			{
				SingularUnityLogger.LogDebug("Age " + age + "is not between 0 and 100");
			}
		}

		public static void SetGender(string gender)
		{
			if (Initialized && gender != "m" && gender != "f")
			{
				SingularUnityLogger.LogDebug("gender " + gender + "is not m or f");
			}
		}

		public static void SetAllowAutoIAPComplete(bool allowed)
		{
		}

		private void OnApplicationPause(bool paused)
		{
			if (Initialized)
			{
				_ = (bool)instance;
			}
		}

		private void OnApplicationQuit()
		{
			if (!Application.isEditor)
			{
				_ = Initialized;
			}
		}

		public static void SetDeferredDeepLinkHandler(SingularDeferredDeepLinkHandler ddlHandler)
		{
			if (!instance)
			{
				SingularUnityLogger.LogError("SingularSDK SetDeferredDeepLinkHandler, no instance available - cannot set deferred deeplink handler!");
			}
			else if (!Application.isEditor)
			{
				registeredDDLHandler = ddlHandler;
				if ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - cachedDDLMessageTime < instance.ddlTimeoutSec && cachedDDLMessage != null)
				{
					registeredDDLHandler.OnDeferredDeepLink(cachedDDLMessage);
				}
			}
		}

		public void DeepLinkHandler(string message)
		{
			SingularUnityLogger.LogDebug($"SingularSDK DeepLinkHandler called! message='{message}'");
			if (!Application.isEditor)
			{
				if (message == "")
				{
					message = null;
				}
				if (registeredDDLHandler != null)
				{
					registeredDDLHandler.OnDeferredDeepLink(message);
					return;
				}
				cachedDDLMessage = message;
				cachedDDLMessageTime = CurrentTimeSec();
			}
		}

		private static int CurrentTimeSec()
		{
			return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static void SetSingularLinkHandler(SingularLinkHandler handler)
		{
			if (!Application.isEditor)
			{
				registeredSingularLinkHandler = handler;
				if (instance != null)
				{
					instance.ResolveSingularLink();
				}
			}
		}

		public static void SetSingularDeviceAttributionCallbackHandler(SingularDeviceAttributionCallbackHandler handler)
		{
			if (!Application.isEditor)
			{
				registeredDeviceAttributionCallbackHandler = handler;
			}
		}

		private void SingularLinkHandlerResolved(string handlerParamsJson)
		{
			instance.resolvedSingularLinkParams = JsonConvert.DeserializeObject<SingularLinkParams>(handlerParamsJson);
			instance.resolvedSingularLinkTime = CurrentTimeSec();
			ResolveSingularLink();
		}

		private void SingularDeviceAttributionCallback(string handlerParamsJson)
		{
			SingularUnityLogger.LogDebug($"SingularSDK SingularDeviceAttributionCallback called! message='{handlerParamsJson}'");
			if (registeredDeviceAttributionCallbackHandler != null && handlerParamsJson != null)
			{
				Dictionary<string, object> attributionInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(handlerParamsJson);
				registeredDeviceAttributionCallbackHandler.OnSingularDeviceAttributionCallback(attributionInfo);
			}
		}

		private void ShortLinkResolved(string json)
		{
			ShortLinkParams shortLinkParams = JsonConvert.DeserializeObject<ShortLinkParams>(json);
			if (shortLinkCallback != null)
			{
				shortLinkCallback(string.IsNullOrEmpty(shortLinkParams.Data) ? null : shortLinkParams.Data, string.IsNullOrEmpty(shortLinkParams.Error) ? null : shortLinkParams.Error);
				shortLinkCallback = null;
			}
		}

		public static void SetConversionValueUpdatedHandler(SingularConversionValueUpdatedHandler handler)
		{
		}

		public static void SetConversionValuesUpdatedHandler(SingularConversionValuesUpdatedHandler handler)
		{
		}

		private void ConversionValueUpdated(string value)
		{
		}

		private void ConversionValuesUpdated(string json)
		{
		}

		private void ResolveSingularLink()
		{
			if (instance.resolvedSingularLinkParams == null)
			{
				return;
			}
			if (registeredSingularLinkHandler != null)
			{
				registeredSingularLinkHandler.OnSingularLinkResolved(instance.resolvedSingularLinkParams);
				instance.resolvedSingularLinkParams = null;
			}
			else if (registeredDDLHandler != null)
			{
				if (ddlTimeoutSec <= 0)
				{
					ddlTimeoutSec = 60L;
				}
				if (CurrentTimeSec() - instance.resolvedSingularLinkTime <= ddlTimeoutSec)
				{
					registeredDDLHandler.OnDeferredDeepLink(instance.resolvedSingularLinkParams.Deeplink);
				}
				instance.resolvedSingularLinkParams = null;
			}
		}

		public static void RegisterDeviceTokenForUninstall(string APNSToken)
		{
		}

		public static string GetAPID()
		{
			return null;
		}

		public static string GetIDFA()
		{
			return null;
		}

		public static void SetSingularSdidAccessorHandler(SingularSdidAccessorHandler handler)
		{
			if (!Application.isEditor)
			{
				registeredSdidAccessorHandler = handler;
			}
		}

		private void SingularDidSetSdid(string result)
		{
			if (!Application.isEditor && registeredSdidAccessorHandler != null)
			{
				registeredSdidAccessorHandler.DidSetSdid(result);
			}
		}

		private void SingularSdidReceived(string result)
		{
			if (!Application.isEditor && registeredSdidAccessorHandler != null)
			{
				registeredSdidAccessorHandler.SdidReceived(result);
			}
		}

		public static void InAppPurchase(IEnumerable<Product> products, Dictionary<string, object> attributes, bool isRestored = false)
		{
			InAppPurchase("__iap__", products, attributes, isRestored);
		}

		public static void InAppPurchase(string eventName, IEnumerable<Product> products, Dictionary<string, object> attributes, bool isRestored = false)
		{
			foreach (Product product in products)
			{
				InAppPurchase(eventName, product, attributes, isRestored);
			}
		}

		public static void InAppPurchase(Product product, Dictionary<string, object> attributes, bool isRestored = false)
		{
			InAppPurchase("__iap__", product, attributes, isRestored);
		}

		public static void InAppPurchase(string eventName, Product product, Dictionary<string, object> attributes, bool isRestored = false)
		{
			if (!Application.isEditor && product != null)
			{
				double amount = (double)product.metadata.localizedPrice;
				if (isRestored)
				{
					amount = 0.0;
				}
				if (!product.hasReceipt)
				{
					CustomRevenue(eventName, product.metadata.isoCurrencyCode, amount);
				}
				else
				{
					Event(null, eventName);
				}
			}
		}

		public static void Revenue(string currency, double amount)
		{
			_ = Application.isEditor;
		}

		public static void CustomRevenue(string eventName, string currency, double amount)
		{
			_ = Application.isEditor;
		}

		public static void Revenue(string currency, double amount, string receipt, string signature)
		{
			_ = Application.isEditor;
		}

		public static void CustomRevenue(string eventName, string currency, double amount, string receipt, string signature)
		{
			_ = Application.isEditor;
		}

		public static void Revenue(string currency, double amount, string productSKU, string productName, string productCategory, int productQuantity, double productPrice)
		{
			_ = Application.isEditor;
		}

		public static void CustomRevenue(string eventName, string currency, double amount, string productSKU, string productName, string productCategory, int productQuantity, double productPrice)
		{
			_ = Application.isEditor;
		}

		public static void Revenue(string currency, double amount, Dictionary<string, object> attributes)
		{
			if (Application.isEditor)
			{
				return;
			}
			try
			{
				JsonConvert.SerializeObject(attributes);
			}
			catch (Exception)
			{
			}
		}

		public static void CustomRevenue(string eventName, string currency, double amount, Dictionary<string, object> attributes)
		{
			if (Application.isEditor)
			{
				return;
			}
			try
			{
				JsonConvert.SerializeObject(attributes);
			}
			catch (Exception)
			{
			}
		}

		public static void RegisterTokenForUninstall(string token)
		{
		}

		public static void SetFCMDeviceToken(string fcmDeviceToken)
		{
			_ = Application.isEditor;
		}

		public static void SetGCMDeviceToken(string gcmDeviceToken)
		{
			_ = Application.isEditor;
		}

		public static void SetCustomUserId(string userId)
		{
			_ = Application.isEditor;
		}

		public static void UnsetCustomUserId()
		{
			_ = Application.isEditor;
		}

		public static void TrackingOptIn()
		{
			_ = Application.isEditor;
		}

		public static void TrackingUnder13()
		{
			_ = Application.isEditor;
		}

		public static void StopAllTracking()
		{
			_ = Application.isEditor;
		}

		public static void ResumeAllTracking()
		{
			_ = Application.isEditor;
		}

		public static bool IsAllTrackingStopped()
		{
			if (Application.isEditor)
			{
				return false;
			}
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				_ = Application.platform;
				_ = 11;
			}
			return false;
		}

		public static void LimitDataSharing(bool limitDataSharingValue)
		{
			_ = Application.isEditor;
		}

		public static bool GetLimitDataSharing()
		{
			_ = Application.isEditor;
			return false;
		}

		public static void AdRevenue(SingularAdData adData)
		{
			try
			{
				if (Initialized && adData != null && adData.HasRequiredParams())
				{
					Event(adData, "__ADMON_USER_LEVEL_REVENUE__");
				}
			}
			catch (Exception)
			{
			}
		}

		public static Dictionary<string, string> GetGlobalProperties()
		{
			if (Application.isEditor)
			{
				return null;
			}
			string text = null;
			if (text == null || text.Trim() == "")
			{
				return null;
			}
			return JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
		}

		public static bool SetGlobalProperty(string key, string value, bool overrideExisting)
		{
			if (Application.isEditor)
			{
				return false;
			}
			if (key == null || key.Trim() == string.Empty)
			{
				return false;
			}
			if (!Initialized)
			{
				instance.globalProperties[key] = new SingularGlobalProperty(key, value, overrideExisting);
				return true;
			}
			return false;
		}

		public static void UnsetGlobalProperty(string key)
		{
			if (!Application.isEditor)
			{
				_ = Initialized;
			}
		}

		public static void ClearGlobalProperties()
		{
			_ = Application.isEditor;
		}

		public static void SkanRegisterAppForAdNetworkAttribution()
		{
		}

		public static bool SkanUpdateConversionValue(int conversionValue)
		{
			return false;
		}

		public static void SkanUpdateConversionValue(int conversionValue, int coarse, bool _lock)
		{
		}

		public static int? SkanGetConversionValue()
		{
			return null;
		}
	}
}