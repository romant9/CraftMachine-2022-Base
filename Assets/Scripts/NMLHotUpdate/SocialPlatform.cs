using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public abstract class SocialPlatform
{
	public class AccountData
	{
		public string id { get; set; }

		public string playerid { get; set; }

		public string name { get; set; }

		public string level { get; set; }
	}

	public const string SocialAccountNameKey = "SocialAccountName";

	protected const string forceSocialKey = "ForceSocial";

	protected bool warnOnGameCenterConnectError;

	protected MessageSerializer serializer = new MessageSerializer();

	public virtual string Type => "";

	public abstract AccountType AccountType { get; }

	public bool ForceSocialKey
	{
		get
		{
			return TWDPlayerPrefs.GetInt("ForceSocial") == 1;
		}
		set
		{
			TWDPlayerPrefs.SetInt("ForceSocial", value ? 1 : 0);
		}
	}

	public bool Authenticated { get; set; }

	public List<Friend> Friends { get; protected set; }

	public bool friendsLoadReady { get; protected set; }

	public string serverPlayerId { get; protected set; }

	public AccountData PlayerAccountData { get; set; }

	public bool AnotherGameFoundOnServer
	{
		get
		{
			if (!string.IsNullOrEmpty(serverPlayerId))
			{
				return serverPlayerId.CompareTo(GameManager.UserId) != 0;
			}
			return false;
		}
	}

	public bool HasDeclinedGameCenter
	{
		get
		{
			return TWDPlayerPrefs.GetInt("DeclinedGameCenter") == 1;
		}
		set
		{
			TWDPlayerPrefs.SetInt("DeclinedGameCenter", value ? 1 : 0);
		}
	}

	public abstract string GetId();

	public abstract string GetUsername();

	public abstract string GetIdWithPrefix();

	public abstract bool OpenSystemDefaultAchievementsUI();

	public abstract void GetProfilePicture(string hashedId, string profileId, ProfilePictureLoaded picLoaded);

	public abstract void ResetAllAchievements();

	public abstract void ReportProgress(string achievementId, int steps, int total);

	public abstract void PromptGameCenterConnect(bool comingFromSettings = false);

	public abstract void CheckAuthentication();

	public abstract void Disconnect();

	public abstract string GetOldGameFoundMessageKey();

	public abstract IEnumerator ToggleConnect_Coroutine(bool connect, Action uiCallback);

	protected abstract void SyncAchievements();

	protected abstract void OnAchievementsChanged();

	protected AccountInfo CreateLinkSocialAccountParameters()
	{
		AccountInfo accountInfo = new AccountInfo();
		Dictionary<string, string> data = new Dictionary<string, string> {
		{
			"SocialAccountName",
			GetUsername()
		} };
		accountInfo.AccountId = GetId();
		accountInfo.Type = AccountType;
		accountInfo.Data = data;
		return accountInfo;
	}

	public virtual void GetFriendListAuto()
	{
		Friends = new List<Friend>();
	}

	public virtual void PromptGameCenterConnectFromSettings()
	{
	}

	protected bool CheckForSavedGame(bool force = false)
	{
		if (!force && HasDeclinedGameCenter)
		{
			return false;
		}
		if (AnotherGameFoundOnServer)
		{
			OpenAccountSyncPrompt(force);
			return true;
		}
		return false;
	}

	public virtual void OpenAccountSyncPrompt(bool force)
	{
		AccountSyncPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AccountSyncPopup) as AccountSyncPopup;
		obj.SetupWithAccount(PlayerAccountData);
		obj.SetCallbacks(delegate
		{
			GameManager.Instance.LoadNewAccount(serverPlayerId, Type);
		}, delegate
		{
			OpenAccountSyncDiscardPrompt(force);
		});
		obj.Open();
	}

	private void OpenAccountSyncDiscardPrompt(bool force)
	{
		ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		string text = "Apple";
		obj.SetContent(LocalizationManager.GetText("Popup.AccountConfirmation.AdditionalCheck.Title"), LocalizationManager.GetText("Popup.AccountConfirmation.AdditionalCheck." + text));
		obj.SetCallbacks(delegate
		{
			HasDeclinedGameCenter = true;
		}, delegate
		{
			CheckForSavedGame(force);
		});
		obj.Open();
		obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Back"));
		obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Yes"));
	}

	protected void NotifyModelOfConnection()
	{
		if (Authenticated && !GameManager.Instance.Blackboard.IsToggleOn("Toggle.GameCenterConnected"))
		{
			Helpers.ExecuteCommand(new SocialPlatformConnected
			{
				GCConnected = true
			});
		}
	}

	protected void CheckSocialAccountLink()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.RequestCommand("GetAccount", GetId(), AccountType.ToString(), OnGetAccount, null, waitForResponse: true);
		}
	}

	protected void OnGetAccount(string message)
	{
		if (SignalRClient.Instance.HasError)
		{
			SignalRClient.Instance.ClearError();
			Debug.LogWarning("GetAccount failed - proceeding with current game");
			return;
		}
		if (message == null)
		{
			PlayerAccountData = null;
		}
		else
		{
			PlayerAccountData = GameManager.Instance.jsonSerializer.DeserializeObject<AccountData>(message);
		}
		if (PlayerAccountData != null)
		{
			serverPlayerId = PlayerAccountData.playerid;
			CheckForSavedGame();
		}
		else
		{
			LinkAccount();
		}
	}

	protected void LinkAccount()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			AccountInfo accountInfo = CreateLinkSocialAccountParameters();
			if (!string.IsNullOrEmpty(accountInfo.AccountId))
			{
				SignalRClient.Instance.RequestCommand("LinkAccount", serializer.Serialize(accountInfo), OnLinkAccount, waitForResponse: true);
			}
			else
			{
				Debug.LogWarning("AccountID was empty when trying to link account. AccountType: " + AccountType.ToString() + " Authenticated: " + Authenticated);
			}
		}
	}

	protected void OnLinkAccount(string message)
	{
		if (SignalRClient.Instance.HasError)
		{
			Debug.LogError("SocialPlatform.LinkAccount failed");
			SignalRClient.Instance.ClearError();
		}
		else
		{
			GameManager.Instance.RequestPltv();
		}
	}

	public void UnlinkAccount(Action successCallback = null, Action failureCallback = null)
	{
		SignalRClient.Instance.RequestCommand("UnlinkAccountAsync", GetId(), AccountType.ToString(), delegate(string message)
		{
			serverPlayerId = "";
			PlayerAccountData = null;
			if (SignalRClient.Instance.HasError)
			{
				Debug.LogError("UnlinkAccountAsync failed: " + message);
				SignalRClient.Instance.ClearError();
				if (failureCallback != null)
				{
					failureCallback();
				}
			}
			else if (successCallback != null)
			{
				successCallback();
			}
		}, null, waitForResponse: true);
	}
}
