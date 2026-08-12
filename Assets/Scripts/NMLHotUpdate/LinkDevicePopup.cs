using BaseModel;
using Client.Connectivity;
using PlayEveryWare.EpicOnlineServices;
using Supabase.TWD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class LinkDevicePopup : HUDElement
{
	public enum LinkDeviceStates
	{
		MainMenu = 0,
		OldDeviceMenu = 1,
		NewDeviceMenu = 2,
		Info = 3,
		GetCode = 4,
		EnterCode = 5,
		Confirmation = 6,
		Confirmed = 7
	}

	private const int codeLenght = 8;

	[SerializeField]
	private UIButton backButton;

	[Header("Containers")]
	[SerializeField]
	private GameObject mainMenuContainer;

	[SerializeField]
	private GameObject infoContainer;

	[SerializeField]
	private GameObject getCodeContainer;

	[SerializeField]
	private GameObject setCodeContainer;

	[SerializeField]
	private GameObject confirmationContainer;

	[Header("Get Code")]
	[SerializeField]
	private UILabel codeLabel;

	[SerializeField]
	private UILabel codeTimerLabel;

	[Header("Set Code")]
	[SerializeField]
	private UIInput codeInput;

	[SerializeField]
	private UIButton enterCodeOkButton;

	[Header("Info")]
	[SerializeField]
	private UIButton infoOkButton;

	[SerializeField]
	private UILabel infoTitleLabel;

	[SerializeField]
	private UILabel infoMessageLabel;

	[Header("Confirmation")]
	[SerializeField]
	private UILabel playerLevelLabel;

	[SerializeField]
	private UILabel playerNameLabel;

	private LinkDeviceStates linkDeviceState;

	private string code;

	private DateTime codeTimer1970;

	private long codeTimerExpiration;

	private string infoTitleTextId;

	private string infoMessageTextId;

	private Callback infoOkButtonCallback;

	private TransferResult transferResult;

	private string confirmationPlayerName = "";

	private string confirmationPlayerLevel = "";

	private Task getPlayerListTask;

	public override void Open()
	{
		base.Open();
		code = "";
		SetState(LinkDeviceStates.MainMenu);

		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			savedLocKey = proDescrLabel.LocalizationKey;

			if (DataManager.Instance.ProLink)
			{
				ProToggle.gameObject.SetActive(true);
				currentPlayerNameLabel.text = "---";
			}
			string textValue = "";
			string playerName = UserPrefsKeys.Player_Name;
			lastEosId = UserPrefsKeys.Player_GoogleID;

			if (!string.IsNullOrEmpty(lastEosId) && !string.IsNullOrEmpty(playerName))
			{
				switch (DataManager.Instance.language)
				{
					case DataManager.Language.Ru:
						textValue = "Последний используемый профиль: ";
						break;
					case DataManager.Language.En:
						textValue = "Last used profile: ";
						break;
					case DataManager.Language.Es:
						textValue = "?ltimo perfil utilizado: ";
						break;
					default: break;
				}
				textValue += playerName;
            }
			else
			{
				switch (DataManager.Instance.language)
				{
					case DataManager.Language.Ru:
						textValue = "Сперва нужно загрузить основной аккаунт";
						break;
					case DataManager.Language.En:
						textValue = "First you need to download the main account";
						break;
					case DataManager.Language.Es:
						textValue = "Primero necesitas descargar la cuenta principal";
						break;
					default: break;
				}
			}
			LastPlayerName.text = playerName;

			if (DataManager.IsPinId)
			{
				string homeName = UserPrefsKeys.Player_Pin_Name;

				if (homeName != playerName)
				{
					BtLinkHome.GetComponentInChildren<UILabel>().text = "Link [b]" + homeName;
					BtLinkHome.gameObject.SetActive(true);
					BtLinkHome.transform.parent.GetComponent<UIGrid>().Reposition();

					BtLinkHome.onClick.Clear();
					BtLinkHome.onClick.Add(new EventDelegate(delegate { LinkHome(); }));
					return;
				}
			}
			BtLinkHome.gameObject.SetActive(false);
			BtLinkHome.transform.parent.GetComponent<UIGrid>().Reposition();
            GetPlayerList();
        }
	}

	private void GetPlayerList()
	{
        playersIDDataList = new();

        getPlayerListTask = Task.Run(async () =>
		{
			if (SupabaseManager.IsOnline)
			{
				playersIDDataList = await DataManager.Instance.DatabaseManager.GetIDListAsync();
			}
		});      
    }

	public override void OnClickClose()
	{
		if (linkDeviceState != LinkDeviceStates.Confirmed)
		{
			Close();
		}
		else
		{
			OnReloadAccount();
		}
	}

	private void SetState(LinkDeviceStates newState)
	{
		linkDeviceState = newState;
		UpdateUI();
	}

	private void ShowInfo(string titleTextId, string messageTextId, Callback infoOkButtonCallback)
	{
		this.infoOkButtonCallback = infoOkButtonCallback;
		linkDeviceState = LinkDeviceStates.Info;
		infoTitleTextId = titleTextId;
		infoMessageTextId = messageTextId;
		UpdateUI();
	}

	public void OnOldDevice()
	{
		if (IsLoadDataManager && string.IsNullOrEmpty(lastEosId))
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && string.IsNullOrEmpty(lastEosId)) return");
			return;
		}

		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		SignalRClient.Instance.RequestCommand("GetTransferCode", OnGetTransferCode, waitForResponse: true);
	}

	private void OnGetTransferCode(string message)
	{
		if (!CheckError(message))
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
			TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
			if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
			{
				code = transferCode.Code;
				codeTimer1970 = new DateTime(1970, 1, 1);
				codeTimerExpiration = transferCode.Expiration;
			}
			else if (CheckError(""))
			{
				return;
			}
			SetState(LinkDeviceStates.GetCode);
		}
	}

	public void OnNewDevice()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			if (string.IsNullOrEmpty(lastEosId)) return;

			if (CommandHelper.Instance.IsUseCustomID)
			{
				DebugTWD.Log("Грузим кастомный профиль вместо текущего", DebugType.Connection);
				StartCoroutine(CheckSignalRConnect());
				StartCoroutine(Request());
			}
			else
			{
				DebugTWD.Log("Линкуемся по коду", DebugType.Connection);
				SetState(LinkDeviceStates.EnterCode);
			}
			return;
		}
		SetState(LinkDeviceStates.EnterCode);
	}

	public override void UpdateUI()
	{
		mainMenuContainer.SetActive(value: false);
		getCodeContainer.SetActive(value: false);
		setCodeContainer.SetActive(value: false);
		infoContainer.SetActive(value: false);
		mainMenuContainer.SetActive(value: false);
		confirmationContainer.SetActive(value: false);
		switch (linkDeviceState)
		{
		case LinkDeviceStates.MainMenu:
			backButton.gameObject.SetActive(value: false);
			mainMenuContainer.SetActive(value: true);
			break;
		case LinkDeviceStates.EnterCode:
			backButton.gameObject.SetActive(value: false);
			setCodeContainer.SetActive(value: true);
			break;
		case LinkDeviceStates.GetCode:
			UpdateGetCodeUI();
			break;
		case LinkDeviceStates.Info:
			UpdateInfoUI();
			break;
		case LinkDeviceStates.Confirmation:
			UpdateConfirmationUI();
			break;
		case LinkDeviceStates.Confirmed:
			infoContainer.SetActive(value: true);
			break;
		case LinkDeviceStates.OldDeviceMenu:
		case LinkDeviceStates.NewDeviceMenu:
			break;
		}
	}

	private void UpdateGetCodeUI()
	{
		getCodeContainer.SetActive(value: true);
		backButton.gameObject.SetActive(value: true);
		codeLabel.text = code.Substring(0, 4) + " " + code.Substring(4, 4);
	}

	private void UpdateInfoUI()
	{
		infoContainer.SetActive(value: true);
		backButton.gameObject.SetActive(value: false);
		infoTitleLabel.text = LocalizationManager.GetText(infoTitleTextId);
		infoMessageLabel.text = LocalizationManager.GetText(infoMessageTextId);
	}

	private void UpdateConfirmationUI()
	{
		confirmationContainer.SetActive(value: true);
		playerLevelLabel.text = LocalizationManager.GetText("Generic.Level{Level}", confirmationPlayerLevel);
		playerNameLabel.text = confirmationPlayerName;
	}

	public override void Update()
	{
		if (linkDeviceState == LinkDeviceStates.GetCode)
		{
			long num = (long)DateTime.UtcNow.Subtract(codeTimer1970).TotalSeconds;
			long num2 = codeTimerExpiration - num;
			codeTimerLabel.text = Helpers.FormatTime(num2 * 1000);
			if (num2 <= 0)
			{
				OnCodeExpired();
			}
		}
		else if (linkDeviceState == LinkDeviceStates.EnterCode)
		{
			enterCodeOkButton.isEnabled = codeInput.value.Length == 8;
		}
	}

	private void OnCodeExpired()
	{
		ShowInfo("Popup.LinkDevice.GetCode.ExpiredTitle", "Popup.LinkDevice.GetCode.ExpiredMessage", Close);
	}

	public void OnCodeEntered()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		string input = codeInput.value.ToLower();
		input = Regex.Replace(input, "\\s+", "");
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			InputCode = input;
			StartCoroutine(CheckSignalRConnect());
			StartCoroutine(Request());
			return;
		}
		SignalRClient.Instance.RequestCommand("UseTransferCode", input, OnUseTransferCode, waitForResponse: true);
	}

	private void OnUseTransferCode(string message)
	{
		if (!CheckError(message))
		{
			DebugTWD.Log("OnUseTransferCode message: " + message, DebugType.SignalR);

			transferResult = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferResult>(message);
			if (transferResult == null || transferResult.State == TransferResultState.Error)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
				ShowInfo("Error.Error", "Popup.LinkDevice.EnterCode.Error", Close);
			}
			else if (transferResult.State == TransferResultState.Success)
			{
				Helpers.ExecuteCommand(new LinkDeviceUseCodeCommand());
				SignalRClient.Instance.RequestCommand("GetPlayerDataSubsetByHashedId", transferResult.PlayerHashedId, OnConfirmationGotPlayer, waitForResponse: true);
			}
			else if (transferResult.State == TransferResultState.CodeExpired)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
				OnCodeExpired();
			}
			else
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
				ShowInfo("Error.Error", "Popup.LinkDevice.EnterCode.Error", Close);
			}
		}
	}

	private void OnConfirmationGotPlayer(string message)
	{
		DebugTWD.Log("OnConfirmationGotPlayer message: " + message, DebugType.SignalR);

		if (!CheckError(message))
		{
			IDictionary<string, object> dictionary = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<IDictionary<string, object>>(message);
			dictionary.TryGetValue("Level", out var value);
			dictionary.TryGetValue("Nickname", out var value2);
			confirmationPlayerLevel = value.ToString();
			confirmationPlayerName = value2 as string;
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);

            if (IsLoadDataManager)
            {
                CommandHelper.Instance.IsPrelogin = false;
                SignalRClient.Instance.IsOnlyGedData = false;
                SignalRClient.Instance.IsOnlyGetPlayersData = true;
            }

            SetState(LinkDeviceStates.Confirmation);
		}
	}

	public void OnBackButton()
	{
		if (linkDeviceState == LinkDeviceStates.GetCode || linkDeviceState == LinkDeviceStates.EnterCode)
		{
			SetState(LinkDeviceStates.MainMenu);
		}
	}

	public void OnInfoOkButton()
	{
		if (infoOkButtonCallback != null)
		{
			infoOkButtonCallback();
		}
	}

	public void OnConfirmationNo()
	{
		ShowInfo("Popup.LinkDevice.LinkCancelledTitle", "Popup.LinkDevice.LinkCancelledMessage", Close);
	}

	[ContextMenu("UnlinkAccount")]
	public void OnConfirmationYes()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			UnlinkAccount(OnUnlinkSuccess, OnUnlinkFailure);
			return;
		}

		if (OfflineManager.IsGoogleSource)
		{
			if (GameManager.Instance.GameCenterManager.Authenticated && !GameManager.Instance.GameCenterManager.HasDeclinedGameCenter)
			{
				GameManager.Instance.GameCenterManager.UnlinkAccount(OnUnlinkSuccess, OnUnlinkFailure);
			}
			else
			{
				OnUnlinkSuccess();
			}
		}
		else
		{
			GameManager.Instance.IAPManager.UnlinkAccount(OnUnlinkSuccess, OnUnlinkFailure);
		}
	}

	public void OnUnlinkSuccess()
	{
		SetState(LinkDeviceStates.Confirmed);
		ShowInfo("Popup.LinkDevice.LinkSuccessTitle", "Popup.LinkDevice.LinkSuccessMessage", OnReloadAccount);
	}

	public void OnUnlinkFailure()
	{
		SetState(LinkDeviceStates.Confirmed);
		OnReloadAccount();
	}

	private void OnReloadAccount()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			if (CommandHelper.Instance.IsUseCustomID)
			{
				if (DataManager.Instance.Player != null) Helpers.ExecuteCommand(new LinkDeviceFinishedCommand(CommandHelper.Instance.customUserEosID, PlayerPrefs.GetString(UserPrefsKeys.Player_GoogleID)));
				LoadNewAccount(CommandHelper.Instance.customUserEosID, "LinkDevice");
			}
			else
			{
				DebugTWD.LogMycode("СРОЧНО МЕНЯТЬ");

				Helpers.ExecuteCommand(new LinkDeviceFinishedCommand(transferResult.PlayerId, GameManager.UserId));
				LoadNewAccount(transferResult.PlayerId, "LinkDevice");
			}
		}
		else
		{
			Helpers.ExecuteCommand(new LinkDeviceFinishedCommand(transferResult.PlayerId, GameManager.UserId));
			GameManager.Instance.LoadNewAccount(transferResult.PlayerId, "LinkDevice");
		}
	}

	private bool CheckError(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			Close();
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}


	#region myparams
	public const string Url = @"https://twd.drillerservices.com";

	public string InputCode = "";

	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public GameObject PlayersDataBlock;
	public UIToggle ProToggle;
	public GameObject PlayersDataScrollContainer;
	public UIScrollView PlayersDataScrollView;
	public UITable PlayersDataTable;
	public GameObject playerPrefab;
	public UILabel currentPlayerNameLabel;
	public LocalizationUIUpdater proDescrLabel;
	public UIToggle AnonymousToggle;
	public UILabel LastPlayerName;
	public string lastEosId;
	public string lastGoogleId;

	public UIButton BtLinkHome;

	private Coroutine routine;
	private string savedLocKey;

	private List<PlayersIDData> playersIDDataList;

	#endregion

	#region mycode
	public void SetInitState(LinkDeviceStates newState)
	{
		linkDeviceState = newState;
	}

	private IEnumerator CheckSignalRConnect()
	{
		var time = DateTime.Now;
		while (DateTime.Now < time + TimeSpan.FromSeconds(20))
		{
			if (GameManager.Instance.IsConnectedToServer)
			{
				yield break;
			}
			yield return null;
		}

		if (SignalRClient.Instance == null || !SignalRClient.Instance.IsConnected)
		{
			DebugTWD.Log("Время вышло. SignalRClient не отвечает");
			StopCoroutine(Request());
		}
	}

	private IEnumerator Request()
	{
		if (DataManager.Instance.Player == null)
		{
			DebugTWD.LogWarning("Is Prelogin");
			CommandHelper.Instance.IsPrelogin = true;
            CommandHelper.Instance.IsUseCustomID = true;
            GetPlayerData.Instance.OnClickConnectEpic();
			yield return new WaitUntil(() => !CommandHelper.Instance.IsPrelogin);
			SignalRClient.Instance.RequestCommand("GetPlayerDataSubsetByHashedId", CommandHelper.Instance.customUserHashID, OnConfirmationGotPlayer, waitForResponse: true);
		}
		else
		{
			DebugTWD.LogWarning("Is Main Login");

			SignalRClient SignalRC = DataManager.Instance.GetComponent<SignalRClient>();
			if (SignalRC && !SignalRC.enabled)
			{
				SignalRC.enabled = true;
				yield return new WaitUntil(() => SignalRClient.Instance != null);
			}

			if (!SignalRClient.Instance.IsConnected)
			{
				DebugTWD.LogWarning("SignalR is not Connected");
				SignalRClient.Instance.Connect(DataManager.DataURL, null);
				yield return new WaitUntil(() => SignalRClient.Instance.IsConnected);
			}

			if (CommandHelper.Instance.IsUseCustomID)
			{
				Helpers.ExecuteCommand(new LinkDeviceUseCodeCommand());
				SignalRClient.Instance.RequestCommand("GetPlayerDataSubsetByHashedId", CommandHelper.Instance.customUserHashID, OnConfirmationGotPlayer, waitForResponse: true);
			}
			else
			{
				SignalRClient.Instance.RequestCommand("UseTransferCode", InputCode, OnUseTransferCode, waitForResponse: true);
			}
		}
	}

	public void UnlinkAccount(Action successCallback = null, Action failureCallback = null)
	{
		if (OfflineManager.IsGoogleSource)
		{
			string googlePlayId = Social.Active.localUser.id ?? lastEosId;

			DebugTWD.Log("Try UnlinkAccount " + googlePlayId);
			if (!string.IsNullOrEmpty(googlePlayId))
			{
				SignalRClient.Instance.RequestCommand("UnlinkAccountAsync", googlePlayId, AccountType.GooglePlay.ToString(), delegate (string message)
				{
					if (SignalRClient.Instance.HasError)
					{
						Debug.LogError("UnlinkAccountAsync failed: " + message);
						SignalRClient.Instance.ClearError();
						failureCallback?.Invoke();
					}
					else
					{
						successCallback?.Invoke();
					}
				}, null, waitForResponse: true);
			}
			else
			{
				failureCallback?.Invoke();
			}
		}
		else
		{
			string EosAccountID = EOSLogin.GetAccountUserId()?.ToString() ?? lastEosId;

			DebugTWD.Log("Try UnlinkAccount " + EosAccountID);
			if (string.IsNullOrEmpty(EosAccountID))
			{
				failureCallback?.Invoke();
				return;
			}
			SignalRClient.Instance.RequestCommand("UnlinkAccountAsync", EosAccountID, AccountType.WindowsEditor.ToString(), delegate (string message)
			{
				if (SignalRClient.Instance.HasError)
				{
					DebugTWD.LogError("UnlinkAccountAsync failed: " + message);
					SignalRClient.Instance.ClearError();
					failureCallback?.Invoke();
				}
				else
				{
					successCallback?.Invoke();
				}
			}, null, waitForResponse: true);
		}
	}

	public void LoadNewAccount(string userId, string type)
	{
        var oldID = UserPrefsKeys.Player_GoogleID; //G02-D05-a47227b5-7221-48d0-b374-936db93636f9
		DebugTWD.Log("Old UserID " + oldID);

        DebugTWD.Log("Try Load new UserID " + userId);

		if (DataManager.Instance.Player != null)
		{
			Helpers.ExecuteCommand(new LoadNewAccountCommand
			{
				Type = type,
				UserId = userId
			});
		}

		UserPrefsKeys.Player_GoogleID = userId; //G02-D05-db8da192-6732-4544-8911-760fc1e09b40 bloody
		TWDPlayerPrefs.SetString("UserId", userId);
		PlayerPrefs.Save();

		StartCoroutine(WaitCommandQueueAndReloadRoutine());
	}

	private IEnumerator WaitCommandQueueAndReloadRoutine()
	{
		while (SignalRClient.Instance.IsWaitingForResponse)
		{
			yield return null;
		}

		if (!OfflineManager.IsGoogleSource)
		{
			var eosManager = DataManager.Instance.GetComponent<EOSManager>();
			if (!eosManager.enabled)
			{
				eosManager.enabled = true;
				yield return new WaitForSeconds(.5f);
			}
		}

		DataManager.Instance.IsReconnectByCode = true;
		DataManager.Instance.IsReconnectPlayerState = false;

		GetPlayerData.Instance.OnClickConnectEpic();
		Close();
	}

	public void GetIDList(UIToggle tg)
	{
        if (!SupabaseManager.IsOnline) return;
        GetIDListInternal(tg.value);
	}

    private async void GetIDListInternal(bool isOpen)
    {
        if (isOpen)
        {
            if (playersIDDataList.Count == 0)
            {
                playersIDDataList = await DataManager.Instance.DatabaseManager.GetIDListAsync();
            }

            if (playersIDDataList.Count > 0)
            {
                AnonymousToggle.Set(TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_Anonymous));

                PlayersDataBlock.SetActive(true);
                currentPlayerNameLabel.text = "---";
            }
        }
        else
        {
            playersIDDataList = new();
            PlayersDataBlock.SetActive(false);
            CommandHelper.Instance.SetDataID(false);
        }
    }

    public void OnOpenPlayersIDDropdown()
	{
		if (playersIDDataList.Count > 0)
		{
			playersIDDataList = playersIDDataList.Where(x => !string.IsNullOrEmpty(x.PlayerName)).ToList();
			playersIDDataList.StableSort((PlayersIDData a, PlayersIDData b) => a.PlayerName.CompareTo(b.PlayerName));

			if (PlayersDataScrollView.transform.childCount > 0)
			{
				Helpers.DestroyAllChildren(PlayersDataScrollView.gameObject);
			}

			foreach (var itemId in playersIDDataList)
			{
				GameObject gameObject = Helpers.InstantiateToParent(playerPrefab, PlayersDataScrollView.gameObject);
				gameObject.GetComponentsInChildren<UILabel>()[0].text = itemId.PlayerName;
				gameObject.GetComponentsInChildren<UILabel>()[1].text = itemId.Level.ToString();
				gameObject.GetComponent<UIButtonExtended>().SetClickCallback(OnClickPlayerRow);
			}
		}

		PlayersDataScrollContainer.SetActive(!PlayersDataScrollContainer.activeSelf);
		PlayersDataScrollView.ResetPosition();
		PlayersDataTable.repositionNow = true;
	}

	private void OnClickPlayerRow(UIButtonExtended bt)
	{
		if (playersIDDataList.Count > 0)
		{
			var name = bt.GetComponentsInChildren<UILabel>()[0].text;
			var level = bt.GetComponentsInChildren<UILabel>()[1].text;

			var data = playersIDDataList.FirstOrDefault(x => x.PlayerName == name && x.Level.ToString() == level);
			if (data != null)
			{
				CommandHelper.Instance.SetDataID(true, data.GameID, data.EosID);
				currentPlayerNameLabel.text = name;

				proDescrLabel.IsCustomTranslate = true;
				proDescrLabel.RuCustomText = "Профиль игрока " + name + " может быть перенесен на ваше устройство";
				proDescrLabel.EnCustomText = "The player profile " + name + " can be transferred to your device";

				PlayersDataScrollContainer.SetActive(false);
			}
			else
			{
				proDescrLabel.UpdateContent();
			}
		}
	}

	public void SetAnonymous(UIToggle tg)
	{
		DataManager.Instance.Anonymous = tg.value;
        UserPrefsKeys.Player_Anonymous = tg.value.ToString();
		PlayerPrefs.Save();
	}

	public void LinkHome()
	{
		string homeEosId = UserPrefsKeys.Player_Pin_GoogleID;
		string homeHash = UserPrefsKeys.Player_Pin_HashID;
		string homeName = UserPrefsKeys.Player_Pin_Name;

		CommandHelper.Instance.SetDataID(true, homeHash, homeEosId);

		DebugTWD.Log("Грузим родной " + homeName + " профиль вместо текущего", DebugType.Connection);
		StartCoroutine(CheckSignalRConnect());
		StartCoroutine(Request());
	}

	public void CopyCode()
	{
		var code = codeLabel.text;
		MyTools.CopyToClipboard(code);
		DebugTWD.Log("Скопировали код " + code);
	}

	private void OnDisable()
	{
		if (IsLoadDataManager)
		{
			proDescrLabel.LocalizationKey = savedLocKey;
		}
	}
	#endregion
}
