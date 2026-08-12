using System;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class BananaPopup : HUDElement
{
	[SerializeField]
	private GameObject container;

	[SerializeField]
	private GameObject closeButton;

	[SerializeField]
	private UITexture BananaBGTexture;

	[Tooltip("Ok button inside the timer continer used when popup is blocked by time")]
	[SerializeField]
	private UIButton okButton;

	public override void Open()
	{
		base.Open();
		okButton.enabled = true;
		UpdateUI();
	}

	public override void Update()
	{
		base.Update();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		string contentPath = "Image/ydlBanana2023";
		if (!string.IsNullOrEmpty(Helpers.GetBananaPopupImage()))
		{
			contentPath = Helpers.GetBananaPopupImage();
		}
		LoadImageFromCdn.LoadImageToTarget(BananaBGTexture, contentPath);
	}

	public override void Close()
	{
		base.Close();
	}

	public void Clear()
	{
	}

	public void OnClickOkButton()
	{
		if (GameManager.Instance.gameEconomyData?.ConfigData == null)
		{
			return;
		}
		if (Helpers.GetClickInternal())
		{
			if (GameManager.Instance.IsConnectedToServer)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
				SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode, waitForResponse: true);
				okButton.enabled = false;
			}
		}
		else
		{
			Close();
			ShopPopupHelper.OpenWithIndex(2);
		}
	}

	private void OnGetTransferCode(string message)
	{
		if (CheckError(message))
		{
			return;
		}
		TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
		if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			string bananaURL = Helpers.GetBananaURL();
			if (playerModel != null && playerModel.HashedId != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
				string text = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
				string deviceId = GameManager.Instance.LoginRequest.Device.DeviceId;
				bananaURL = bananaURL + "?id=" + text + "&code=" + transferCode.Code + "&DeviceId=" + deviceId + "&OS=" + Helpers.GetPlatformName(Application.platform);
				okButton.enabled = true;
				Application.OpenURL(bananaURL);
			}
		}
		else
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
			CheckError("");
		}
	}

	private bool CheckError(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}
}
