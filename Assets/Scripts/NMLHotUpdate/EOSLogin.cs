using System;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.UserInfo;
using PlayEveryWare.EpicOnlineServices;

public class EOSLogin
{
	public delegate void OnEpicLoginCallback(ProductUserId productUserId);

	private static OnEpicLoginCallback _EpicLoginCallback = null;

	private static string _AccessToken = "";

	private static string _RefreshToken = "";

	private static EpicAccountId _AccountUserId = null;

	private static ProductUserId _ProductUserId = null;

	private static UserInfoData _UserInfo;

	private static NotifyEventHandle s_notifyConnectAuthExpirationCallbackHandle = null;

	public static string GetAccessToken()
	{
		return _AccessToken;
	}

	public static string GetRefreshToken()
	{
		return _RefreshToken;
	}

	public static EpicAccountId GetAccountUserId()
	{
		return _AccountUserId;
	}

	public static ProductUserId GetProductUserId()
	{
		return _ProductUserId;
	}

	public static string GetUserDisplayName()
	{
		return _UserInfo.DisplayName;
	}

	public static void Login(OnEpicLoginCallback epicLoginCallback)
	{
		_EpicLoginCallback = epicLoginCallback;
		string token = string.Empty;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs == null)
		{
			DebugTWD.LogMycode("if (commandLineArgs == null) return");
			return;
		}

		if (OfflineManager.IsCustomLogin)
		{
			DebugTWD.Log("EosLogin started");
			DebugTWD.LogMycode("if (OfflineManager.IsCustomLogin)");
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				var text = commandLineArgs[i];
				if (text.Contains("accessToken"))
				{
					token = commandLineArgs[i + 1];
					DebugTWD.Log("Token : " + token);
					break;
				}
			}

			if (!string.IsNullOrEmpty(token))
			{
				EOSManager.Instance.StartLoginWithLoginTypeAndToken(LoginCredentialType.ExchangeCode, null, token, delegate (Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo)
				{
					DebugTWD.Log("ResultCode: " + callbackInfo.ResultCode);
					if (callbackInfo.ResultCode != Result.Success)
					{
						LoginWithPersistentMode();
					}
					else
					{
						StartConnectLoginWithLoginCallbackInfo(callbackInfo);
					}
				});
			}
			else
			{
				DebugTWD.Log("Token is null. Try LoginWithPersistentMode");
				LoginWithPersistentMode();
			}
		}
		else
		{
			foreach (string text in commandLineArgs)
			{
				if (text.Contains("AUTH_PASSWORD"))
				{
					string[] array = text.Split('=');
					if (array.Length >= 2)
					{
						token = array[1];
					}
				}
			}
			EOSManager.Instance.StartLoginWithLoginTypeAndToken(LoginCredentialType.ExchangeCode, null, token, delegate (Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo)
			{
				if (callbackInfo.ResultCode != Result.Success)
				{
					LoginWithPersistentMode();
				}
				else
				{
					StartConnectLoginWithLoginCallbackInfo(callbackInfo);
				}
			});
		}
	}

	private static void LoginWithPersistentMode()
	{
		EOSManager.Instance.StartPersistentLogin(delegate(Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo)
		{
			if (callbackInfo.ResultCode != Result.Success)
			{
				LoginWithLoginTypeAndToken();
			}
			else
			{
				StartConnectLoginWithLoginCallbackInfo(callbackInfo);
			}
		});
	}

	private static void LoginWithLoginTypeAndToken()
	{
		EOSManager.Instance.StartLoginWithLoginTypeAndToken(LoginCredentialType.AccountPortal, ExternalCredentialType.Epic, null, null, delegate(Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo)
		{
			if (callbackInfo.ResultCode != Result.Success)
			{
				Debug.LogError("Login Failed With AccountPortal Login");
				if (_EpicLoginCallback != null)
				{
					_EpicLoginCallback(null);
				}
			}
			else
			{
				StartConnectLoginWithLoginCallbackInfo(callbackInfo);
			}
		});
	}

	private static void StartConnectLoginWithLoginCallbackInfo(Epic.OnlineServices.Auth.LoginCallbackInfo loginCallbackInfo)
	{
		_AccountUserId = loginCallbackInfo.LocalUserId;
		EOSManager.Instance.StartConnectLoginWithEpicAccount(loginCallbackInfo.LocalUserId, delegate(Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo)
		{
			if (OfflineManager.IsCustomLogin)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsCustomLogin)");
				SetupAccessTokenAndRefreshToken(_AccountUserId);
				QueryUserInfoById(_AccountUserId);
			}
			else
			{
				if (connectLoginCallbackInfo.ResultCode == Result.Success)
				{
					SetupAccessTokenAndRefreshToken(_AccountUserId);
					SetupTokenExpirationListener();
					_ProductUserId = connectLoginCallbackInfo.LocalUserId;
					QueryUserInfoById(_AccountUserId);
				}
				else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
				{
					EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, delegate (CreateUserCallbackInfo createUserCallbackInfo)
					{
						if (createUserCallbackInfo.ResultCode == Result.Success)
						{
							EOSManager.Instance.StartConnectLoginWithEpicAccount(loginCallbackInfo.LocalUserId, delegate (Epic.OnlineServices.Connect.LoginCallbackInfo retryConnectLoginCallbackInfo)
							{
								if (retryConnectLoginCallbackInfo.ResultCode == Result.Success)
								{
									SetupAccessTokenAndRefreshToken(_AccountUserId);
									SetupTokenExpirationListener();
									_ProductUserId = retryConnectLoginCallbackInfo.LocalUserId;
									QueryUserInfoById(_AccountUserId);
								}
								else
								{
									Debug.LogError("Connect Login Retry Failed");
									if (_EpicLoginCallback != null)
									{
										_EpicLoginCallback(null);
									}
								}
							});
						}
						else
						{
							Debug.LogError("Create Connect User Failed");
							if (_EpicLoginCallback != null)
							{
								_EpicLoginCallback(null);
							}
						}
					});
				}
				else
				{
					Debug.LogError("Connect Login Failed");
					if (_EpicLoginCallback != null)
					{
						_EpicLoginCallback(null);
					}
				}
			}
		});
	}

	private static void SetupAccessTokenAndRefreshToken(EpicAccountId accountId)
	{
		AuthInterface authInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
		CopyUserAuthTokenOptions options = default(CopyUserAuthTokenOptions);
		authInterface.CopyUserAuthToken(ref options, accountId, out var outUserAuthToken);
		_AccessToken = outUserAuthToken.Value.AccessToken;
		_RefreshToken = outUserAuthToken.Value.RefreshToken;
	}

	private static void SetupTokenExpirationListener()
	{
		if (s_notifyConnectAuthExpirationCallbackHandle != null)
		{
			return;
		}
		ConnectInterface connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
		AddNotifyAuthExpirationOptions options = default(AddNotifyAuthExpirationOptions);
		s_notifyConnectAuthExpirationCallbackHandle = new NotifyEventHandle(connectInterface.AddNotifyAuthExpiration(ref options, null, delegate
		{
			EOSManager.Instance.StartConnectLoginWithEpicAccount(_AccountUserId, delegate(Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo)
			{
				if (connectLoginCallbackInfo.ResultCode == Result.Success)
				{
					SetupAccessTokenAndRefreshToken(_AccountUserId);
				}
				else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
				{
					EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, delegate(CreateUserCallbackInfo createUserCallbackInfo)
					{
						if (createUserCallbackInfo.ResultCode == Result.Success)
						{
							EOSManager.Instance.StartConnectLoginWithEpicAccount(_AccountUserId, delegate(Epic.OnlineServices.Connect.LoginCallbackInfo retryConnectLoginCallbackInfo)
							{
								if (retryConnectLoginCallbackInfo.ResultCode == Result.Success)
								{
									SetupAccessTokenAndRefreshToken(_AccountUserId);
								}
								else
								{
									Debug.LogError("Refresh Token Failed 1");
								}
							});
						}
						else
						{
							Debug.LogError("Refresh Token Failed 2");
						}
					});
				}
				else
				{
					Debug.LogError("Refresh Token Failed 3");
				}
			});
		}), delegate(ulong handle)
		{
			EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface()?.RemoveNotifyAuthExpiration(handle);
		});
	}

	private static void QueryUserInfoById(EpicAccountId UserId)
	{
		if ((object)UserId == null || !UserId.IsValid())
		{
			Debug.LogError("UserInfo (QueryUserInfoById): Invalid UserId");
			if (_EpicLoginCallback != null)
			{
				_EpicLoginCallback(null);
			}
		}
		else
		{
			QueryUserInfoOptions options = new QueryUserInfoOptions
			{
				LocalUserId = EOSManager.Instance.GetLocalUserId(),
				TargetUserId = UserId
			};
			(EOSManager.Instance?.GetEOSPlatformInterface()?.GetUserInfoInterface()).QueryUserInfo(ref options, null, OnQueryUserInfoIdCompleted);
		}
	}

	private static void OnQueryUserInfoIdCompleted(ref QueryUserInfoCallbackInfo data)
	{
		if (data.ResultCode != Result.Success)
		{
			Debug.LogErrorFormat("UserData (OnQueryUserInfoCompleted): Error calling QueryUserInfo: {0}", data.ResultCode);
			if (_EpicLoginCallback != null)
			{
				_EpicLoginCallback(null);
			}
			return;
		}
		CopyUserInfoOptions options = new CopyUserInfoOptions
		{
			LocalUserId = data.LocalUserId,
			TargetUserId = data.TargetUserId
		};
		UserInfoData? outUserInfo;
		Result result = (EOSManager.Instance?.GetEOSPlatformInterface()?.GetUserInfoInterface()).CopyUserInfo(ref options, out outUserInfo);
		if (result != Result.Success)
		{
			Debug.LogErrorFormat("UserData (OnQueryUserInfoCompleted): CopyUserInfo error: {0}", result);
			if (_EpicLoginCallback != null)
			{
				_EpicLoginCallback(null);
			}
		}
		else
		{
			_UserInfo = outUserInfo.Value;
			if (_EpicLoginCallback != null)
			{
				_EpicLoginCallback(_ProductUserId);
			}
		}
	}
}
