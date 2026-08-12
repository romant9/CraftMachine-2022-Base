using System;
using System.Collections;
using BaseModel;
using Client.Connectivity;
using GooglePlayGames;
using TWDModel;
using UnityEngine;

public class SocialPlatformGooglePlay : SocialPlatform
{
	private const string _autoLogin = "AutoLogin";

	protected bool googlePlayLoggingInProcess;

	public override string Type => "GooglePlay";

	public override AccountType AccountType => AccountType.GooglePlay;

	private void LoginLog(string message)
	{
		if (!BuildConfiguration.Active.Branch.Contains("release"))
		{
			Debug.LogError(message);
		}
	}

	public SocialPlatformGooglePlay()
	{
		PlayGamesPlatform.Activate();
	}

	public override string GetOldGameFoundMessageKey()
	{
		return "Popup.GameCenter.OldGameFoundMessage.GooglePlay";
	}

	public override string GetId()
	{
		return Social.Active.localUser.id;
	}

	public override string GetIdWithPrefix()
	{
		return "GP_" + GetId();
	}

	public void Authenticate(Action<string> authSucceeded, Action authFailed, bool userInitiated)
	{
		googlePlayLoggingInProcess = true;
		SetFlag(AccountType, "AutoLogin", flag: true);
		Social.Active.localUser.Authenticate(delegate(bool success, string error)
		{
			LoginLog(string.Format("Login {0} {1}", AccountType, success ? "Success" : error));
			if (success)
			{
				string id = Social.Active.localUser.id;
				Connect(id);
				googlePlayLoggingInProcess = false;
				authSucceeded(id);
			}
			else
			{
				LoginLog("SocialPlatformGooglePlay: Authenticate auth failed");
				base.Authenticated = false;
				googlePlayLoggingInProcess = false;
				authFailed();
			}
		});
	}

	public override void CheckAuthentication()
	{
		if (base.Authenticated && string.IsNullOrEmpty(GetId()))
		{
			base.Authenticated = false;
		}
	}

	protected override void OnAchievementsChanged()
	{
		SyncAchievements();
	}

	public override void GetFriendListAuto()
	{
		base.GetFriendListAuto();
		base.friendsLoadReady = true;
	}

	public override void ReportProgress(string achievementId, int steps, int total)
	{
		if (!string.IsNullOrEmpty(GetId()))
		{
			ReportProgressInternal(achievementId, steps, total);
		}
	}

	private void ReportProgressInternal(string achievementId, int steps, int total)
	{
		if (steps >= total)
		{
			Social.ReportProgress(achievementId, 100.0, delegate
			{
			});
			SingularityMonoBehaviour<SDKManager>.Instance.FirebaseManager.LogEvent("unlock_achievement", "achievment_unlocked", achievementId);
		}
	}

	public override bool OpenSystemDefaultAchievementsUI()
	{
		Social.ShowAchievementsUI();
		return true;
	}

	public override void GetProfilePicture(string hashedId, string profileId, ProfilePictureLoaded picLoaded)
	{
	}

	public override void ResetAllAchievements()
	{
	}

	protected override void SyncAchievements()
	{
		if (string.IsNullOrEmpty(GetId()))
		{
			return;
		}
		AchievementManager achievementManager = GameManager.Instance.playerModel.AchievementManager;
		if (achievementManager == null)
		{
			return;
		}
		for (int i = 0; i < achievementManager.GED.AchievementDefinitions.Length; i++)
		{
			AchievementDefinition achievementDefinition = achievementManager.GED.AchievementDefinitions[i];
			if (achievementDefinition != null && !string.IsNullOrEmpty(achievementDefinition.GooglePlayID))
			{
				int progress = achievementManager.GetProgress(achievementDefinition);
				if (progress > 0)
				{
					ReportProgressInternal(achievementDefinition.GooglePlayID, progress, 100);
				}
			}
		}
	}

	private void Connect(string user_id)
	{
		LoginLog("SocialPlatformGooglePlay: Connect " + user_id);
		if ((bool)SignalRClient.Instance && SignalRClient.Instance.IsConnected)
		{
			CheckSocialAccountLink();
		}
		base.Authenticated = true;
		NotifyModelOfConnection();
		SyncAchievements();
		AchievementManager achievementManager = GameManager.Instance.playerModel.AchievementManager;
		if (achievementManager != null)
		{
			achievementManager.OnAchievementsChanged += OnAchievementsChanged;
		}
		GameManager.Instance.FriendListManager.UpdateFriends();
	}

	public override void Disconnect()
	{
		PlayGamesPlatform.Instance.SignOut();
		base.Authenticated = false;
		if (GameManager.Instance.playerModel != null)
		{
            AchievementManager achievementManager = GameManager.Instance.playerModel.AchievementManager;
            if (achievementManager != null)
            {
                achievementManager.OnAchievementsChanged -= OnAchievementsChanged;
            }
        }
		
		LoginLog("SocialPlatformGooglePlay: Disconnect");
	}

	public override IEnumerator ToggleConnect_Coroutine(bool connect, Action uiCallback)
	{
		LoginLog("SocialPlatformGooglePlay: ToggleConnect_Coroutine, googlePlayLoggingInProcess=" + googlePlayLoggingInProcess + ", Authenticated=" + base.Authenticated + ", connect=" + connect);
		if (googlePlayLoggingInProcess && string.IsNullOrEmpty(GetId()))
		{
			LoginLog("SocialPlatformGooglePlay: Waiting for callback");
			uiCallback();
		}
		else if (connect)
		{
			Authenticate(delegate
			{
				LoginLog("SocialPlatformGooglePlay: ToggleConnect_Coroutine callback: authentication successful!");
				base.HasDeclinedGameCenter = false;
				uiCallback();
			}, delegate
			{
				LoginLog("SocialPlatformGooglePlay: ToggleConnect_Coroutine callback: authentication failed!");
				uiCallback();
			}, userInitiated: true);
		}
		else
		{
			Disconnect();
			uiCallback();
			googlePlayLoggingInProcess = false;
		}
		yield break;
	}

	public override void PromptGameCenterConnect(bool comingFromSettings = false)
	{
		LoginLog("SocialPlatformGooglePlay: PromptGameCenterConnect");
		string id = Social.Active.localUser.id;
		bool flag = (!GetFlag(AccountType, "AutoLogin") && !base.HasDeclinedGameCenter) || comingFromSettings;
		LoginLog("SocialPlatformGooglePlay: gpUserId " + id);
		LoginLog("SocialPlatformGooglePlay: GetFlag " + GetFlag(AccountType, "AutoLogin"));
		LoginLog("SocialPlatformGooglePlay: GetKey " + GetKey(AccountType, "AutoLogin"));
		LoginLog("SocialPlatformGooglePlay: canAskLogin " + flag);
		LoginLog("SocialPlatformGooglePlay: comingFromSettings " + comingFromSettings);
		if (flag && string.IsNullOrEmpty(id) && (bool)SingularityMonoBehaviour<HUDManager>.Instance && !googlePlayLoggingInProcess)
		{
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent(LocalizationManager.GetText("Popup.GameCenter.GooglePlayLoginTitle"), LocalizationManager.GetText("Popup.GameCenter.GooglePlayLoginMessage"));
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Yes"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.No"));
			obj.SetCallbacks(delegate
			{
				Authenticate(delegate(string s)
				{
					LoginLog("SocialPlatformGooglePlay: PromptGameCenterConnect Auth succeeded => " + s);
					base.HasDeclinedGameCenter = false;
				}, delegate
				{
					LoginLog("SocialPlatformGooglePlay: PromptGameCenterConnect Auth failed");
					base.HasDeclinedGameCenter = false;
				}, userInitiated: true);
			}, delegate
			{
				base.HasDeclinedGameCenter = true;
				LoginLog("SocialPlatformGooglePlay: PromptGameCenterConnect Auth cancelled");
				Disconnect();
			});
			obj.Open();
			obj.EnableCloseArea(enable: false);
		}
		else if (!string.IsNullOrEmpty(id))
		{
			LoginLog("SocialPlatformGooglePlay: PromptGameCenterConnect already logged connect gpuserid: " + id);
			Connect(id);
		}
		else if (!base.HasDeclinedGameCenter && GetFlag(AccountType, "AutoLogin"))
		{
			LoginLog("SocialPlatformGooglePlay: Auto login triggered");
			Authenticate(delegate(string s)
			{
				LoginLog("SocialPlatformGooglePlay: PromptGameCenterConnect Auth succeeded => " + s);
				base.HasDeclinedGameCenter = false;
			}, delegate
			{
				LoginLog("SocialPlatformGooglePlay: PromptGameCenterConnect Auth failed");
			}, userInitiated: true);
		}
	}

	private bool GetFlag(AccountType type, string id)
	{
		return PlayerPrefs.GetInt(GetKey(type, id), 0) == 1;
	}

	private void SetFlag(AccountType type, string id, bool flag)
	{
		if (GetFlag(type, id) != flag)
		{
			PlayerPrefs.SetInt(GetKey(type, id), flag ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	private string GetKey(AccountType type, string id)
	{
		return $"SocialManager.{type}.{id}";
	}

	public override string GetUsername()
	{
		return Social.localUser.userName;
	}
}
