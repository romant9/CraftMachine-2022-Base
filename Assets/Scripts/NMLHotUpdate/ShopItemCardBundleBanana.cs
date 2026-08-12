using System;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class ShopItemCardBundleBanana : ShopCardBase<BundleStoreDefinition>
{
	[SerializeField]
	private UIButtonWithLabel button;

	[SerializeField]
	private UILabel itemDesLabel;

	[Tooltip("Image from GED/URL")]
	[SerializeField]
	private UITexture itemDynamicTexture;

	[Tooltip("Image from GED/Dashboard")]
	[SerializeField]
	private UITexture itemDynamicTextureItem;

	[SerializeField]
	private GameObject currencyParent;

	[SerializeField]
	private UILabel salesBadge;

	public const string defaultItemPrefabName = "Bundle_List_Item";

	public const string defaultEquipmentPrefabName = "Bundle_List_Equipment";

	public const string defaultConsumablePrefabName = "Bundle_List_Consumable";

	public override int GetSortValue()
	{
		return 1000;
	}

	public override void AddListeners()
	{
		base.AddListeners();
		if (button != null)
		{
			button.SetClickCallback(OnButtonClicked);
		}
	}

	public override void RemoveListeners()
	{
		base.RemoveListeners();
		if (button != null)
		{
			button.RemoveClickCallback(OnButtonClicked);
		}
	}

	public void OnButtonClicked(UIButtonExtended button)
	{
		if (Helpers.GetFirstPic())
		{
			if (GameManager.Instance.IsConnectedToServer)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
				SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode, waitForResponse: true);
			}
		}
		else
		{
			ShopPopupHelper.OpenWithIndex(2);
		}
	}

	private void OnGetTransferCode(string message)
	{
		if (CheckError(message))
		{
			return;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
		if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			string bananaURL = Helpers.GetBananaURL();
			if (playerModel != null && playerModel.HashedId != null)
			{
				string text = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
				string deviceId = GameManager.Instance.LoginRequest.Device.DeviceId;
				bananaURL = bananaURL + "?id=" + text + "&code=" + transferCode.Code + "&DeviceId=" + deviceId + "&OS=" + Helpers.GetPlatformName(Application.platform);
				Application.OpenURL(bananaURL);
			}
		}
		else
		{
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

	public virtual void OnPoolReturn()
	{
		Clear();
	}

	public override void Clear()
	{
		base.Clear();
		if (button != null)
		{
			button.Clear();
		}
	}

	public override void SetData(BundleStoreDefinition bundleStoreDefinition)
	{
		base.SetData(bundleStoreDefinition);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		string text = LocalizationManager.GetText("Banana.StoreDescription.ItemDescription");
		HelpersUI.SetContentToLabel(itemDesLabel, text);
		string contentPath = "Image/ydlBanana2023";
		if (!string.IsNullOrEmpty(Helpers.GetShopFairBannerUrl()))
		{
			contentPath = Helpers.GetShopFairBannerUrl();
		}
		LoadImageFromCdn.LoadImageToTarget(itemDynamicTextureItem, contentPath);
		string text2 = LocalizationManager.GetText("Banana.StoreDescription.Button");
		button.SetContentToLabelOne(text2);
		HelpersUI.SetContentToLabel(salesBadge, LocalizationManager.GetText("Banana.StoreDescription.Label"));
	}
}
