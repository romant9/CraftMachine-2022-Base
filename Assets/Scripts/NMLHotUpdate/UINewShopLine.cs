using System;
using System.Collections.Generic;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class UINewShopLine : ShopCardBase<UINewShopLineData>
{
	[SerializeField]
	private GameObject BannerContent;

	[SerializeField]
	private GameObject EquipContent;

	[SerializeField]
	private List<UINewShopItem> cards;

	[SerializeField]
	private UISprite BannerIcon;

	[SerializeField]
	private UITexture DynamicBannerIcon;

	[SerializeField]
	private UIButtonWithLabel BannerButton;

	private UINewShopLineData data;

	private string confGoTo => GameManager.Instance.gameEconomyData?.ConfigData?.BananaEnterButtonGoTo;

	public override void AddListeners()
	{
		base.AddListeners();
		UIEvent.OnUIEvent -= UIEvent_OnUIEvent;
		UIEvent.OnUIEvent += UIEvent_OnUIEvent;
	}

	public override void RemoveListeners()
	{
		base.RemoveListeners();
		UIEvent.OnUIEvent -= UIEvent_OnUIEvent;
	}

	public override void Clear()
	{
		base.Clear();
	}

	private void UIEvent_OnUIEvent(string type, object parameter)
	{
		_ = type == "OnPopUpClose";
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (data != null)
		{
			Helpers.GameObjectSetActive(BannerContent, value: false);
			Helpers.GameObjectSetActive(EquipContent, value: false);
			if (data.IsBanner())
			{
				Helpers.GameObjectSetActive(BannerContent, value: true);
				UpdateUIBanner();
			}
			else
			{
				Helpers.GameObjectSetActive(EquipContent, value: true);
				UpdateUIEquip();
			}
		}
	}

	public override void SetData(UINewShopLineData newData)
	{
		base.SetData(data);
		data = newData;
		UpdateUI();
	}

	public void OnBannerButtonClicked(UIButtonExtended button)
	{
		if (data == null)
		{
			return;
		}
		switch (data.GetShopType())
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
			if (confGoTo == "GoldRadioCall")
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopup).Close();
				NewPhonePopup.OpenRadiophoneFeaturePopup();
				(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup).OnClickGoldRadio();
			}
			else if (Helpers.IsPCPlatform())
			{
				OnBannerButtonClicked_BundleStore();
			}
			else
			{
				OnBannerButtonClicked_Tradefair();
			}
			break;
		case UINewShopLineData.NewShopItemType.Tradefair:
			OnBannerButtonClicked_Tradefair();
			break;
		}
	}

	public void OnBannerButtonClicked_BundleStore()
	{
		UIEvent.Send("NewShopSetRightVisibleEvent", true);
		string bundleButtonJump = GameManager.Instance.gameEconomyData.ConfigData.BundleButtonJump;
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(bundleButtonJump);
		if (bundleStoreDefinition != null && GameManager.Instance.playerModel.BundleManager.CanBuyBundle(bundleStoreDefinition))
		{
			UINewShopItemData uINewShopItemData = new UINewShopItemData();
			uINewShopItemData.storeDefinition = bundleStoreDefinition;
			UIEvent.Send("NewShopSelectedEvent", uINewShopItemData);
		}
		else
		{
			UIEvent.Send("NewShopSelectedFirstEvent");
		}
	}

	public void OnBannerButtonClicked_Tradefair()
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

	private void UpdateUIBanner()
	{
		if (data == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(BannerIcon, value: false);
		Helpers.GameObjectSetActive(DynamicBannerIcon, value: false);
		switch (data.BannerType)
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
			if (confGoTo == "GoldRadioCall")
			{
				BannerButton.SetContentToLabelOne(LocalizationManager.GetText("BananaEnterButton.GoldRadioCall"));
			}
			else
			{
				BannerButton.SetContentToLabelOne(LocalizationManager.GetText("ShopUI.FrontPage.BannerButton"));
			}
			if (Helpers.IsPCPlatform())
			{
				UpdateUIBanner_BundleStore();
			}
			else
			{
				UpdateUIBanner_Tradefair();
			}
			break;
		case UINewShopLineData.NewShopItemType.Tradefair:
			BannerButton.SetContentToLabelOne(LocalizationManager.GetText("Banana.StoreDescription.Button"));
			UpdateUIBanner_Tradefair();
			break;
		}
	}

	private void UpdateUIBanner_BundleStore()
	{
		string text = "";
		text = Helpers.GetShopBundleBannerUrl();
		if (string.IsNullOrEmpty(text))
		{
			BannerIcon.spriteName = Helpers.GetShopBundleBannerUrlINPACK();
			Helpers.GameObjectSetActive(BannerIcon, value: true);
		}
		else
		{
			LoadImageFromCdn.LoadImageToTarget(DynamicBannerIcon, text);
			Helpers.GameObjectSetActive(DynamicBannerIcon, value: true);
		}
	}

	private void UpdateUIBanner_Tradefair()
	{
		string text = "";
		text = Helpers.GetShopFairBannerUrl();
		if (string.IsNullOrEmpty(text))
		{
			BannerIcon.spriteName = Helpers.GetShopFairBannerUrlINPACK();
			Helpers.GameObjectSetActive(BannerIcon, value: true);
		}
		else
		{
			LoadImageFromCdn.LoadImageToTarget(DynamicBannerIcon, text);
			Helpers.GameObjectSetActive(DynamicBannerIcon, value: true);
		}
	}

	private void UpdateUIEquip()
	{
		List<BundleStoreDefinition> bundleStores = data.GetBundleStores();
		List<TradefairBundleStoreDefinition> tradefairs = data.GetTradefairs();
		List<GoldShopDefinition> componentItems = data.GetComponentItems();
		if (data == null || cards == null || cards.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < cards.Count; i++)
		{
			UINewShopItemData uINewShopItemData = new UINewShopItemData();
			if (bundleStores.Count > i)
			{
				uINewShopItemData.storeDefinition = bundleStores[i];
			}
			if (tradefairs.Count > i)
			{
				uINewShopItemData.tradefairDefinition = tradefairs[i];
			}
			if (componentItems.Count > i)
			{
				uINewShopItemData.goldShopDefinition = componentItems[i];
			}
			cards[i].SetData(uINewShopItemData);
		}
	}
}
