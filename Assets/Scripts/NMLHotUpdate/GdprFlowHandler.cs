using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GdprFlowHandler : SingularityMonoBehaviour<GdprFlowHandler>
{
	private enum PopupState
	{
		TermsChangedStart = 0,
		Declined = 1,
		MarkedForDeletion = 2,
		NewUserAcceptTOS = 3,
		CookieConsent = 4,
		PrivacyPolicy = 5,
		DeletedUser = 6,
		IDFAConsent = 7
	}

	public const string GDPR_KEY_NEWUSER_TOS = "NewUserTOSAccepted";

	public const string GDPR_KEY_NEWUSER_TOS_TIMESTAMP = "TOSAcceptedTimestamp";

	public const string GDPR_KEY_COOKIECONSENT = "CookieConsent";

	public const string GDPR_KEY_TARGETEDADSCONSENT = "TargetedAdsConsent";

	public const string GDPR_KEY_PRIVACYPOLICYCHANGED = "PrivacyPolicyChanged";

	public const string GDPR_KEY_TOS_CHANGED = "TermsOfServiceChanged";

	public const string HELPSHIFT_INTERNAL_URL = "helpshift-ng://";

	[Header("GDPR notification popup")]
	[SerializeField]
	private GameObject gdprNotificationContainer;

	[SerializeField]
	private GameObject gdprTOSChangedContainer;

	[SerializeField]
	private GameObject gdprTOSDeclainedContainer;

	[SerializeField]
	private GameObject gdprTOSMarkedDeletionContainer;

	[SerializeField]
	private GameObject gdpTOSNewUserContainer;

	[SerializeField]
	private GameObject gdprCookieConsentContainer;

	[SerializeField]
	private GameObject gdprPrivacyPolicyContainer;

	[SerializeField]
	private GameObject gdprTOSPlayerDeletedContainer;

	[SerializeField]
	private IDFARequestPopup FallBackIDFARequestPopup;

	[SerializeField]
	private IDFARequestPopup CurrentIDFARequestPopup;

	[SerializeField]
	private UILabel markedDeletionLabel;

	[SerializeField]
	private LabelWithURLHandler dataDeletionHandler;

	private GdprLoginStateMachine loginStateMachine;

	private PopupState state;

	private DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

	private Dictionary<PopupState, string> stateToAnalyticsNames = new Dictionary<PopupState, string>
	{
		{
			PopupState.CookieConsent,
			"CookieConsent"
		},
		{
			PopupState.Declined,
			"ToS_Declined"
		},
		{
			PopupState.MarkedForDeletion,
			"Notification_DataDeletion"
		},
		{
			PopupState.PrivacyPolicy,
			"Notification_Change_PP"
		},
		{
			PopupState.TermsChangedStart,
			"Accept_ToS"
		},
		{
			PopupState.DeletedUser,
			"Player_Deleted"
		},
		{
			PopupState.IDFAConsent,
			"IDFA_Consent"
		}
	};

	public bool DataDeletionRequested { get; protected set; }

	public bool AcceptedTOS { get; protected set; }

	public bool CookieConsentSeen { get; protected set; }

	public bool AcceptedConsent { get; protected set; }

	public bool TosPrivacyPolicySeen { get; protected set; }

	public bool TOSTermsChangedSeen { get; protected set; }

	public bool IDFARequestSeen { get; protected set; }

	public void ShowNewUserTOS()
	{
		Startup.LogStartupEvent("Open_GDPR_TermsOfService");
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: true);
		SetState(PopupState.NewUserAcceptTOS);
	}

	public void ShowCookieConsent()
	{
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: true);
		SetState(PopupState.CookieConsent);
	}

	public void ShowTermsChangedDialog()
	{
		Startup.LogStartupEvent("Open_GDPR_TermsOfService");
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: true);
		SetState(PopupState.TermsChangedStart);
	}

	public void ShowMarkedForDeletion()
	{
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: true);
		SetState(PopupState.MarkedForDeletion);
	}

	public void ShowPrivacyPolicyDialog()
	{
		Startup.LogStartupEvent("Open_GDPR_PrivacyPolicy");
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: true);
		SetState(PopupState.PrivacyPolicy);
	}

	public void ShowPlayerDeleted()
	{
		Startup.LogStartupEvent("Open_GDPR_PrivacyPolicy");
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: true);
		SetState(PopupState.DeletedUser);
	}

	public void ShowIDFARequestPopup()
	{
		Startup.LogStartupEvent("Open_IDFA_Popup");
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: false);
		SetState(PopupState.IDFAConsent);
	}

	private void OnEnable()
	{
		if (dataDeletionHandler != null)
		{
			dataDeletionHandler.OnUrlClicked -= OnLaunchHelpShift;
			dataDeletionHandler.OnUrlClicked += OnLaunchHelpShift;
		}
	}

	private void OnDisable()
	{
		if (dataDeletionHandler != null)
		{
			dataDeletionHandler.OnUrlClicked -= OnLaunchHelpShift;
		}
	}

	public void Start()
	{
		loginStateMachine = new GdprLoginStateMachine(!string.IsNullOrEmpty(GameManager.UserId));
	}

	public void Reset()
	{
		loginStateMachine = new GdprLoginStateMachine(!string.IsNullOrEmpty(GameManager.UserId));
		DataDeletionRequested = false;
		AcceptedTOS = false;
		CookieConsentSeen = false;
		AcceptedConsent = false;
		TosPrivacyPolicySeen = false;
		TOSTermsChangedSeen = false;
		IDFARequestSeen = false;
	}

	public IEnumerator HandlePreLogin()
	{
		yield return loginStateMachine.ProcessEvent(GdprLoginStateMachine.Events.PreLogin);
	}

	public IEnumerator HandlePostLogin(PlayerModel player)
	{
		if (player != null && player.MarkedForDeletion > 0)
		{
			loginStateMachine.State = GdprLoginStateMachine.States.MarkedForDeletion;
		}
		if (AcceptedTOS)
		{
			long timeStamp = (long)(DateTime.UtcNow - Helpers.UnixEpoch).TotalSeconds * 1000;
			if (GameManager.Instance.gameEconomyData.ConfigData.GdprTosChanged)
			{
				Helpers.ExecuteCommand(new SetGdprStateCommand("TermsOfServiceChanged", accepted: true, timeStamp));
			}
			if (AcceptedTOS && GameManager.Instance.gameEconomyData.ConfigData.GdprPrivacyPolicyChanged)
			{
				Helpers.ExecuteCommand(new SetGdprStateCommand("PrivacyPolicyChanged", accepted: true, timeStamp));
			}
		}
		yield return loginStateMachine.ProcessEvent(GdprLoginStateMachine.Events.Login);
	}

	public IEnumerator HandlePostLoginIDFAScreen()
	{
		yield return loginStateMachine.ProcessEvent(GdprLoginStateMachine.Events.Login);
	}

	public void SetDataToBeDeleted()
	{
		Helpers.ExecuteCommand(new SetMarkedForDeletionCommand(marked: true));
		if (GameManager.Instance.IsGameStarted)
		{
			GameManager.Instance.WaitCommandQueueAndReload();
		}
		else
		{
			SetState(PopupState.MarkedForDeletion);
		}
	}

	public void OnAcceptNewUserTOS()
	{
		long num = (long)(DateTime.UtcNow - origin).TotalSeconds * 1000;
		TWDPlayerPrefs.SetInt("NewUserTOSAccepted", 1);
		TWDPlayerPrefs.SetString("TOSAcceptedTimestamp", num.ToString());
		TWDPlayerPrefs.Save();
		AcceptedTOS = true;
		Helpers.GameObjectSetActive(gdpTOSNewUserContainer, value: false);
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: false);
	}

	public void OnAcceptConsent()
	{
		SetGdprState("CookieConsent", accepted: true, gdprCookieConsentContainer);
		TWDPlayerPrefs.SetInt("CookieConsent", 1);
		TWDPlayerPrefs.Save();
		CookieConsentSeen = true;
	}

	public void OnDeclineConsent()
	{
		SetGdprState("CookieConsent", accepted: false, gdprCookieConsentContainer);
		TWDPlayerPrefs.SetInt("CookieConsent", -1);
		TWDPlayerPrefs.Save();
		CookieConsentSeen = true;
	}

	public void OnAcceptPrivacyPolicyChanged()
	{
		SetGdprState("PrivacyPolicyChanged", accepted: true, gdprPrivacyPolicyContainer);
		TosPrivacyPolicySeen = true;
	}

	public void OnAcceptTOSTermsChanged()
	{
		SetGdprState("TermsOfServiceChanged", accepted: true, gdprTOSChangedContainer);
		TOSTermsChangedSeen = true;
	}

	public void OnDeclineTOSTermsChanged()
	{
		SetGdprState("TermsOfServiceChanged", accepted: false, null);
		SendEndGdprMetrics(GetAnalyticsDialogueName(), accepted: false);
		SetState(PopupState.Declined);
	}

	public void OnIDFARequstSeen()
	{
		IDFARequestSeen = true;
	}

	public void OnIDFARequestAccept()
	{
		GameManager.Instance.ShowNativeIDFAPopup(1);
	}

	public void OnBackToTermsChangedStart()
	{
		SendEndGdprMetrics(GetAnalyticsDialogueName(), accepted: false);
		SetState(PopupState.TermsChangedStart);
	}

	public void OnCancelDataDeletion()
	{
		Helpers.ExecuteCommand(new SetMarkedForDeletionCommand());
		SendEndGdprMetrics(GetAnalyticsDialogueName(), accepted: false);
		if (GameManager.Instance.playerModel.HasTakenGdprAction("TermsOfServiceChanged") && !GameManager.Instance.playerModel.HasAcceptedGdprAction("TermsOfServiceChanged"))
		{
			SetState(PopupState.TermsChangedStart);
			return;
		}
		TOSTermsChangedSeen = true;
		Helpers.GameObjectSetActive(gdprNotificationContainer, value: false);
	}

	private void SetState(PopupState state)
	{
		this.state = state;
		UpdateUI();
	}

	private void UpdateUI()
	{
		IDFARequestPopup currentIDFAPopup = GetCurrentIDFAPopup();
		Helpers.GameObjectSetActive(gdprTOSChangedContainer, value: false);
		Helpers.GameObjectSetActive(gdprTOSDeclainedContainer, value: false);
		Helpers.GameObjectSetActive(gdprTOSMarkedDeletionContainer, value: false);
		Helpers.GameObjectSetActive(gdpTOSNewUserContainer, value: false);
		Helpers.GameObjectSetActive(gdprCookieConsentContainer, value: false);
		Helpers.GameObjectSetActive(gdprPrivacyPolicyContainer, value: false);
		Helpers.GameObjectSetActive(gdprPrivacyPolicyContainer, value: false);
		Helpers.GameObjectSetActive(gdprTOSPlayerDeletedContainer, value: false);
		Helpers.GameObjectSetActive(FallBackIDFARequestPopup.gameObject, value: false);
		Helpers.GameObjectSetActive(CurrentIDFARequestPopup.gameObject, value: false);
		switch (state)
		{
		case PopupState.TermsChangedStart:
			SendStartGdprMetrics(GetAnalyticsDialogueName());
			Helpers.GameObjectSetActive(gdprTOSChangedContainer, value: true);
			break;
		case PopupState.Declined:
			SendStartGdprMetrics(GetAnalyticsDialogueName());
			Helpers.GameObjectSetActive(gdprTOSDeclainedContainer, value: true);
			break;
		case PopupState.MarkedForDeletion:
			SendStartGdprMetrics(GetAnalyticsDialogueName());
			HelpersUI.SetContentToLabel(markedDeletionLabel, LocalizationManager.GetText("Popup.TOS.ChangedTerms.MarkedDeletion.Content{Parameter}", GetDateTimeForDeletion().ToString("MMM ddd d HH:mm yyyy")));
			Helpers.GameObjectSetActive(gdprTOSMarkedDeletionContainer, value: true);
			break;
		case PopupState.NewUserAcceptTOS:
			Helpers.GameObjectSetActive(gdpTOSNewUserContainer, value: true);
			break;
		case PopupState.DeletedUser:
			Helpers.GameObjectSetActive(gdprTOSPlayerDeletedContainer, value: true);
			break;
		case PopupState.CookieConsent:
			SendStartGdprMetrics(GetAnalyticsDialogueName());
			Helpers.GameObjectSetActive(gdprCookieConsentContainer, value: true);
			break;
		case PopupState.PrivacyPolicy:
			SendStartGdprMetrics(GetAnalyticsDialogueName());
			Helpers.GameObjectSetActive(gdprPrivacyPolicyContainer, value: true);
			break;
		case PopupState.IDFAConsent:
			SendStartGdprMetrics(GetAnalyticsDialogueName());
			currentIDFAPopup.Initialize(1);
			Helpers.GameObjectSetActive(currentIDFAPopup.gameObject, value: true);
			break;
		}
	}

	private DateTime GetDateTimeForDeletion()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		long num = (long)((playerModel.Created.ToUniversalTime() - origin).TotalSeconds * 1000.0) + playerModel.MarkedForDeletion;
		return origin + TimeSpan.FromMilliseconds(num);
	}

	private void SetGdprState(string stateName, bool accepted, GameObject goToInactivate)
	{
		long timeStamp = (long)(DateTime.UtcNow - origin).TotalSeconds * 1000;
		Helpers.ExecuteCommand(new SetGdprStateCommand(stateName, accepted, timeStamp));
		SendEndGdprMetrics(GetAnalyticsDialogueName(), accepted);
		if (goToInactivate != null)
		{
			Helpers.GameObjectSetActive(goToInactivate, value: false);
			Helpers.GameObjectSetActive(gdprNotificationContainer, value: false);
		}
	}

	private string GetAnalyticsDialogueName()
	{
		string value = null;
		if (stateToAnalyticsNames.TryGetValue(state, out value))
		{
			return value;
		}
		return "";
	}

	private void SendStartGdprMetrics(string metricsDialogueName)
	{
		Helpers.ExecuteCommand(new SendGdprMetricCommand(SendGdprMetricCommand.MetricType.Start_GDPR)
		{
			DialogueName = metricsDialogueName
		});
	}

	private void SendEndGdprMetrics(string metricsDialogueName, bool accepted)
	{
		Helpers.ExecuteCommand(new SendGdprMetricCommand(SendGdprMetricCommand.MetricType.End_GDPR)
		{
			DialogueName = metricsDialogueName,
			DialogueDecision = (accepted ? "1" : "0")
		});
	}

	private void OnLaunchHelpShift(string url)
	{
		string launchDetails = null;
		if (TryParseHelpshiftLink(url, out launchDetails))
		{
			DataDeletionRequested = true;
			SingularityMonoBehaviour<SDKManager>.Instance.LaunchFromURLInternal(launchDetails);
		}
	}

	private bool TryParseHelpshiftLink(string url, out string launchDetails)
	{
		launchDetails = null;
		if (url != null && url.StartsWith("helpshift-ng://"))
		{
			launchDetails = url.Substring("helpshift-ng://".Length);
			return true;
		}
		return false;
	}

	public IDFARequestPopup GetCurrentIDFAPopup()
	{
		if (GameManager.Instance.gameEconomyData != null)
		{
			if (!GameManager.Instance.gameEconomyData.ConfigData.IDFAVariantPrefab)
			{
				return FallBackIDFARequestPopup;
			}
			return CurrentIDFARequestPopup;
		}
		return null;
	}
}
