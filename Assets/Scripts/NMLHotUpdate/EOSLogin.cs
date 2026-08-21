using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.UserInfo;
using PlayEveryWare.EpicOnlineServices;
using System;
using CopyIdTokenOptions = Epic.OnlineServices.Auth.CopyIdTokenOptions;

public class EOSLogin
{
	public delegate void OnEpicLoginCallback(ProductUserId productUserId);

	private static OnEpicLoginCallback _EpicLoginCallback = null;

	private static string _AccessToken = "";

	private static string _RefreshToken = "";

	private static EpicAccountId _AccountUserId = null;

	private static ProductUserId _ProductUserId = null;

	private static UserInfoData _UserInfo;

	private static AuthInterface _AuthInterface;

	private const string EpicIdTokenKey = "EpicIdToken";

	private static NotifyEventHandle s_notifyConnectAuthExpirationCallbackHandle = null;

	public static AuthInterface GetAuthInterface()
	{
		if (_AuthInterface == null)
		{
			_AuthInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
		}
		return _AuthInterface;
	}

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
				var savedToken = TWDPlayerPrefs.GetString(EpicIdTokenKey);
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
					LoginWithPersistentMode();
				}				
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
		});
	}

	private static void SetupAccessTokenAndRefreshToken(EpicAccountId accountId)
	{
		_AccountUserId = accountId;
		_AuthInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
		CopyUserAuthTokenOptions options = default;
		_AuthInterface.CopyUserAuthToken(ref options, accountId, out var outUserAuthToken);
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

	public static void SaveEpicToken()
	{
		string idToken = GetIdToken();
		if (idToken != null)
		{
			HttpHelper.ParsePayload(idToken);
			TWDPlayerPrefs.SetString(EpicIdTokenKey, idToken);
			TWDPlayerPrefs.Save();
		}
	}

	public static string GetIdToken()
	{
		if (_AuthInterface == null)
		{
			Debug.LogError("[EOS] Auth not initialized. Call InitializeAuth first.");
			return null;
		}

		var options = new CopyIdTokenOptions
		{
			AccountId = _AccountUserId
		};

		var tokenResult = _AuthInterface.CopyIdToken(ref options, out var idToken);
		if (tokenResult == Result.Success)
		{
			if (idToken.HasValue && !string.IsNullOrEmpty(idToken.Value.JsonWebToken))
			{
				Debug.Log($"[EOS] IdToken retrieved");
				return idToken.Value.JsonWebToken;
			}
			else
			{
				Debug.LogWarning("[EOS] CopyIdToken succeeded but token is empty.");
				return null;
			}
		}
		else
		{
			Debug.LogError("[EOS] CopyIdToken failed.");
			return null;
		}
	}
}
