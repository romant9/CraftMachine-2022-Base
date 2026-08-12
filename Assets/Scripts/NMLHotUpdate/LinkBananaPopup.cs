using System;
using BaseModel;
using UnityEngine;

public class LinkBananaPopup : HUDElement
{
	private enum LinkDeviceStates
	{
		Info = 0,
		GetCode = 1,
		Confirmed = 2
	}

	[Header("Containers")]
	[SerializeField]
	private GameObject infoContainer;

	[SerializeField]
	private GameObject getCodeContainer;

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

	private string code = "";

	private DateTime codeTimer1970;

	private long codeTimerExpiration;

	private string infoTitleTextId;

	private string infoMessageTextId;

	private Callback infoOkButtonCallback;

	private TransferResult transferResult;

	private string confirmationPlayerName = "";

	private string confirmationPlayerLevel = "";

	public string Code
	{
		get
		{
			return code;
		}
		set
		{
			code = value;
		}
	}

	public DateTime CodeTimer1970
	{
		get
		{
			return codeTimer1970;
		}
		set
		{
			codeTimer1970 = value;
		}
	}

	public long CodeTimerExpiration
	{
		get
		{
			return codeTimerExpiration;
		}
		set
		{
			codeTimerExpiration = value;
		}
	}

	public override void Open()
	{
		base.Open();
		SetState(LinkDeviceStates.GetCode);
	}

	public override void OnClickClose()
	{
		Close();
	}

	public void OnClickEditCopy()
	{
		string text = codeLabel.text;
		if (text != null)
		{
			GUIUtility.systemCopyBuffer = text;
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

	public override void UpdateUI()
	{
		getCodeContainer.SetActive(value: false);
		infoContainer.SetActive(value: false);
		switch (linkDeviceState)
		{
		case LinkDeviceStates.GetCode:
			UpdateGetCodeUI();
			break;
		case LinkDeviceStates.Info:
			UpdateInfoUI();
			break;
		}
	}

	private void UpdateGetCodeUI()
	{
		getCodeContainer.SetActive(value: true);
		codeLabel.text = code.Substring(0, 4) + code.Substring(4, 4);
	}

	private void UpdateInfoUI()
	{
		infoContainer.SetActive(value: true);
		infoTitleLabel.text = LocalizationManager.GetText(infoTitleTextId);
		infoMessageLabel.text = LocalizationManager.GetText(infoMessageTextId);
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
	}

	private void OnCodeExpired()
	{
		ShowInfo("Popup.LinkDevice.GetCode.ExpiredTitle", "Popup.GenerateVerifyCode.Description", Close);
	}

	public void OnInfoOkButton()
	{
		if (infoOkButtonCallback != null)
		{
			infoOkButtonCallback();
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
}
