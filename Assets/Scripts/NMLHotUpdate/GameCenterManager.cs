using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;

public class GameCenterManager
{
	private SocialPlatform platformImplementation;

	public bool Authenticated
	{
		get
		{
			if (platformImplementation == null)
			{
				return false;
			}
			return platformImplementation.Authenticated;
		}
	}

	public bool AnotherGameFoundOnServer
	{
		get
		{
			if (platformImplementation == null)
			{
				return false;
			}
			return platformImplementation.AnotherGameFoundOnServer;
		}
	}

	public bool HasDeclinedGameCenter
	{
		get
		{
			if (platformImplementation == null)
			{
				return false;
			}
			return platformImplementation.HasDeclinedGameCenter;
		}
	}

	public List<Friend> Friends
	{
		get
		{
			if (platformImplementation == null)
			{
				return null;
			}
			return platformImplementation.Friends;
		}
	}

	public bool friendsLoadReady
	{
		get
		{
			if (platformImplementation == null)
			{
				return false;
			}
			return platformImplementation.friendsLoadReady;
		}
	}

	public GameCenterManager()
	{
		platformImplementation = new SocialPlatformGooglePlay();
	}

	public void CheckAuthentication()
	{
		if (platformImplementation != null)
		{
			platformImplementation.CheckAuthentication();
		}
	}

	public string GetId()
	{
		if (platformImplementation != null)
		{
			return platformImplementation.GetId();
		}
		return string.Empty;
	}

	public AccountType GetAccountType()
	{
		if (platformImplementation != null)
		{
			return platformImplementation.AccountType;
		}
		return AccountType.GooglePlay;
	}

	public string GetIdWithPrefix()
	{
		if (platformImplementation != null)
		{
			return platformImplementation.GetIdWithPrefix();
		}
		return string.Empty;
	}

	public void GetFriendListAuto()
	{
		if (platformImplementation != null)
		{
			platformImplementation.GetFriendListAuto();
		}
	}

	public void ReportProgress(string achievementId, int steps, int total)
	{
		if (platformImplementation != null)
		{
			platformImplementation.ReportProgress(achievementId, steps, total);
		}
	}

	public bool OpenSystemDefaultAchievementsUI()
	{
		if (platformImplementation != null)
		{
			return platformImplementation.OpenSystemDefaultAchievementsUI();
		}
		return false;
	}

	public void GetProfilePicture(string hashedId, string profileId, ProfilePictureLoaded picLoaded)
	{
		if (platformImplementation != null)
		{
			platformImplementation.GetProfilePicture(hashedId, profileId, picLoaded);
		}
	}

	public void ResetAllAchievements()
	{
		if (platformImplementation != null)
		{
			platformImplementation.ResetAllAchievements();
		}
	}

	public void PromptGameCenterConnect(bool comingFromSettings = false)
	{
		if (platformImplementation != null)
		{
			platformImplementation.PromptGameCenterConnect(comingFromSettings);
		}
	}

	public void PromptGameCenterRestore(bool comingFromSettings = false)
	{
		if (platformImplementation != null)
		{
			platformImplementation.OpenAccountSyncPrompt(comingFromSettings);
		}
	}

	public IEnumerator ToggleConnect_Coroutine(bool connect, Action uiCallback)
	{
		if (platformImplementation != null)
		{
			return platformImplementation.ToggleConnect_Coroutine(connect, uiCallback);
		}
		return null;
	}

	public void Disconnect()
	{
		if (platformImplementation != null)
		{
			platformImplementation.Disconnect();
		}
	}

	public void SetForceSocialKey(bool force)
	{
		if (platformImplementation != null)
		{
			platformImplementation.ForceSocialKey = force;
		}
	}

	public void UnlinkAccount(Action successCallback = null, Action failureCallback = null)
	{
		if (platformImplementation != null)
		{
			platformImplementation.UnlinkAccount(successCallback, failureCallback);
		}
		else
		{
			failureCallback?.Invoke();
		}
	}
}
