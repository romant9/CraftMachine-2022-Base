using System.Collections;
using TWDModel;
using UnityEngine;

public class UINewShopItem : MonoBehaviour
{
	[SerializeField]
	private EquipmentTokenButton equipmentTokenButton;

	[SerializeField]
	private EquipmentButton equipmentButton;

	[SerializeField]
	private EquipmentRandomButton equipmentRandomButton;

	[SerializeField]
	private UITexture DynamicIcon;

	[SerializeField]
	private GameObject DefaultIcon;

	[SerializeField]
	private GameObject DefaultIconGold;

	[SerializeField]
	private UISprite itemSprite;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private GameObject SelectIcon;

	[SerializeField]
	private GameObject newContainer;

	[SerializeField]
	private UILabel newContainerTxt;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel NumsTxt;

	[SerializeField]
	private UILabel timeLeftTxt;

	[SerializeField]
	private GameObject timeContainer;

	[SerializeField]
	private UILabel priceTxt;

	[SerializeField]
	private UISprite priceIconSprite;

	[SerializeField]
	private UIAtlas uiCurrencyAtlas;

	[SerializeField]
	private UIAtlas uiShopAtlas;

	[SerializeField]
	private UIAtlas uiCampAtlas;

	[SerializeField]
	private GameObject soldOutContainer;

	[Header("ContentButtons")]
	[SerializeField]
	private UIButton ContentTokenButton;

	[SerializeField]
	private UIButton ContentEquipButton;

	private UINewShopItemData bindData;

	private ItemBundleType dataType;

	private bool LastTimeHasData;

	private bool canBuyBundle = true;

	private void OnEnable()
	{
		Helpers.GameObjectSetActive(timeContainer, value: false);
		Helpers.GameObjectSetActive(soldOutContainer, value: false);
		ContentTokenButton.onClick.Add(new EventDelegate(OnClicked));
		ContentEquipButton.onClick.Add(new EventDelegate(OnClicked));
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		ContentTokenButton.onClick.Remove(new EventDelegate(OnClicked));
		ContentEquipButton.onClick.Remove(new EventDelegate(OnClicked));
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "NewShopSelectedEvent" && parameter is UINewShopItemData uINewShopItemData)
		{
			SetSelect(uINewShopItemData);
		}
	}

	public void Update()
	{
		if (bindData == null || bindData.IsEmptyData())
		{
			return;
		}
		canBuyBundle = true;
		bool flag = false;
		LimitedBundleData limitedBundleData = null;
		switch (bindData.GetShopType())
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
			flag = bindData.storeDefinition.ShowTimerInCard;
			limitedBundleData = GameManager.Instance.playerModel.BundleManager.GetInitiatedLimitedBundle(bindData.storeDefinition.BundleIdentifier);
			break;
		case UINewShopLineData.NewShopItemType.Tradefair:
			flag = bindData.tradefairDefinition.ShowTimerInCard;
			limitedBundleData = GameManager.Instance.playerModel.TradefairManager.GetInitiatedLimitedBundle(bindData.tradefairDefinition.BundleIdentifier);
			break;
		case UINewShopLineData.NewShopItemType.Gold:
			flag = bindData.goldShopDefinition.ShowTimerInCard;
			limitedBundleData = GameManager.Instance.playerModel.GoldShopDefinitionManager.GetInitiatedLimitedBundle(bindData.goldShopDefinition.ItemId);
			canBuyBundle = GameManager.Instance.playerModel.GoldShopDefinitionManager.CanBuyBundle(bindData.goldShopDefinition);
			break;
		}
		if (limitedBundleData == null && LastTimeHasData)
		{
			StartCoroutine(FreshShopCoroutine());
		}
		LastTimeHasData = limitedBundleData != null;
		if (canBuyBundle)
		{
			Helpers.GameObjectSetActive(soldOutContainer, value: false);
			Helpers.GameObjectSetActive(timeContainer, value: false);
			if (timeLeftTxt != null && flag && limitedBundleData != null)
			{
				Helpers.GameObjectSetActive(timeContainer, value: true);
				HelpersUI.SetContentToLabel(timeLeftTxt, Helpers.FormatTimeNoZero(limitedBundleData.Timer));
			}
		}
		else
		{
			SetSelectState(selected: false);
			Helpers.GameObjectSetActive(soldOutContainer, value: true);
		}
	}

	private IEnumerator FreshShopCoroutine()
	{
		yield return null;
		UIEvent.Send("OnRequestShopUpdate");
	}

	public void SetData(UINewShopItemData newShopItem)
	{
		if (newShopItem == null || newShopItem.IsEmptyData())
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		bindData = newShopItem;
		UpdateUI();
		SetSelectState(selected: false);
	}

	private void UpdateUI()
	{
		Helpers.GameObjectSetActive(equipmentTokenButton, value: false);
		Helpers.GameObjectSetActive(equipmentButton, value: false);
		Helpers.GameObjectSetActive(equipmentRandomButton, value: false);
		Helpers.GameObjectSetActive(DynamicIcon, value: false);
		Helpers.GameObjectSetActive(DefaultIcon, value: false);
		Helpers.GameObjectSetActive(DefaultIconGold, value: false);
		Helpers.GameObjectSetActive(consumableTexture, value: false);
		Helpers.GameObjectSetActive(itemSprite, value: false);
		Helpers.GameObjectSetActive(newContainer, value: false);
		Helpers.GameObjectSetActive(timeLeftTxt, value: false);
		Helpers.GameObjectSetActive(NumsTxt, value: false);
		Helpers.GameObjectSetActive(priceIconSprite, value: false);
		IReward reward = null;
		RewardTimedBonus rewardTimedBonus = null;
		string text = "";
		string localImageName = "";
		string overallImagePath = "";
		int cardImageRatio = 100;
		switch (bindData.GetShopType())
		{
		case UINewShopLineData.NewShopItemType.BundleStore:
		{
			BundleContentDefinition bundleContentDefinition2 = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bindData.storeDefinition.BundleIdentifier);
			if (bundleContentDefinition2 != null && bundleContentDefinition2.RewardEntries != null)
			{
				if (bundleContentDefinition2.RewardEntries.RewardsList.Count == 1)
				{
					rewardTimedBonus = bundleContentDefinition2.RewardEntries.RewardsList[0] as RewardTimedBonus;
				}
				reward = bundleContentDefinition2.RewardEntries.RewardsList[0];
			}
			if (!string.IsNullOrEmpty(bindData.storeDefinition.FrontPageLabelLocalization))
			{
				Helpers.GameObjectSetActive(newContainer, value: true);
				newContainerTxt.text = LocalizationManager.GetText(bindData.storeDefinition.FrontPageLabelLocalization);
			}
			HelpersUI.SetContentToLabel(content: (!string.IsNullOrEmpty(bindData.storeDefinition.OverrideTitleLocalization)) ? LocalizationManager.GetText("IAPCard.ItemName." + bindData.storeDefinition.OverrideTitleLocalization) : ((rewardTimedBonus == null) ? LocalizationManager.GetText("IAPCard.ItemName." + bindData.storeDefinition.BundleIdentifier) : HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration)), label: title);
			if (priceTxt != null)
			{
				InAppPurchaseProductApple inAppPurchaseProduct2 = GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition2.IAPProduct);
				string text5 = LocalizationManager.GetText("Generic.Free");
				if (inAppPurchaseProduct2 != null)
				{
					text5 = ((inAppPurchaseProduct2.PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(bundleContentDefinition2.IAPProduct) : LocalizationManager.GetText("Generic.Free"));
				}
				priceTxt.text = text5;
			}
			overallImagePath = bindData.storeDefinition.OverallImagePath;
			cardImageRatio = bindData.storeDefinition.CardImageRatio;
			localImageName = bindData.storeDefinition.LocalImageName;
			break;
		}
		case UINewShopLineData.NewShopItemType.Tradefair:
		{
			TradefairBundleContentDefinition tradefairBundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetTradefairBundleContentDefinition(bindData.tradefairDefinition.BundleIdentifier);
			if (tradefairBundleContentDefinition != null && tradefairBundleContentDefinition.RewardEntries != null)
			{
				if (tradefairBundleContentDefinition.RewardEntries.RewardsList.Count == 1)
				{
					rewardTimedBonus = tradefairBundleContentDefinition.RewardEntries.RewardsList[0] as RewardTimedBonus;
				}
				reward = tradefairBundleContentDefinition.RewardEntries.RewardsList[0];
			}
			if (!string.IsNullOrEmpty(bindData.tradefairDefinition.FrontPageLabelLocalization))
			{
				Helpers.GameObjectSetActive(newContainer, value: true);
				newContainerTxt.text = LocalizationManager.GetText(bindData.tradefairDefinition.FrontPageLabelLocalization);
			}
			HelpersUI.SetContentToLabel(content: (!string.IsNullOrEmpty(bindData.tradefairDefinition.OverrideTitleLocalization)) ? LocalizationManager.GetText("IAPCard.ItemName." + bindData.tradefairDefinition.OverrideTitleLocalization) : ((rewardTimedBonus == null) ? LocalizationManager.GetText("IAPCard.ItemName." + bindData.tradefairDefinition.BundleIdentifier) : HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration)), label: title);
			if (bindData.tradefairDefinition.BundleIdentifier == "TWD_BOX_OF_FAIRCOINS")
			{
				BundleContentDefinition bundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(bindData.tradefairDefinition.BundleIdentifier);
				if (priceTxt != null)
				{
					InAppPurchaseProductApple inAppPurchaseProduct = GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct);
					string text3 = LocalizationManager.GetText("Generic.Free");
					if (inAppPurchaseProduct != null)
					{
						text3 = ((inAppPurchaseProduct.PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(bundleContentDefinition.IAPProduct) : LocalizationManager.GetText("Generic.Free"));
					}
					priceTxt.text = text3;
				}
			}
			else if (priceTxt != null)
			{
				int iAPProduct = tradefairBundleContentDefinition.IAPProduct;
				string text4 = (((float)iAPProduct > 0f) ? (iAPProduct.ToString() ?? "") : LocalizationManager.GetText("Generic.Free"));
				Helpers.GameObjectSetActive(priceIconSprite, iAPProduct > 0);
				if (tradefairBundleContentDefinition.PayBanana && GameManager.Instance.gameEconomyData.ConfigData.PayBananaSwitch && Helpers.GetShopRoleType() == ShopRoleType.DefaultType)
				{
					text4 = "$" + tradefairBundleContentDefinition.ShowPrice;
					Helpers.GameObjectSetActive(priceIconSprite, value: false);
				}
				priceTxt.text = text4;
				priceIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Fairmoney);
			}
			overallImagePath = bindData.tradefairDefinition.OverallImagePath;
			cardImageRatio = bindData.tradefairDefinition.CardImageRatio;
			localImageName = bindData.tradefairDefinition.LocalImageName;
			break;
		}
		case UINewShopLineData.NewShopItemType.Gold:
		{
			GoldShopDefinition goldShopDefinition = GameManager.Instance.playerModel.gameEconomyData.GetGoldShopDefinition(bindData.goldShopDefinition.ItemId);
			if (goldShopDefinition != null && goldShopDefinition.RewardEntries != null)
			{
				if (goldShopDefinition.RewardEntries.RewardsList.Count == 1)
				{
					rewardTimedBonus = goldShopDefinition.RewardEntries.RewardsList[0] as RewardTimedBonus;
				}
				reward = goldShopDefinition.RewardEntries.RewardsList[0];
			}
			HelpersUI.SetContentToLabel(content: string.IsNullOrEmpty(goldShopDefinition.OverrideTitleLocalization) ? LocalizationManager.GetText("GoldShopItem." + goldShopDefinition.ItemId + ".Name") : LocalizationManager.GetText("IAPCard.ItemName." + goldShopDefinition.OverrideTitleLocalization), label: title);
			int numsForIReward = Helpers.GetNumsForIReward(reward);
			if (numsForIReward > 0)
			{
				Helpers.GameObjectSetActive(NumsTxt, value: true);
				HelpersUI.SetContentToLabel(NumsTxt, "x" + numsForIReward);
			}
			if (priceTxt != null)
			{
				int price = goldShopDefinition.Price;
				string text2 = (((float)price > 0f) ? (price.ToString() ?? "") : LocalizationManager.GetText("Generic.Free"));
				priceTxt.text = text2;
				priceIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Diamonds);
				Helpers.GameObjectSetActive(priceIconSprite, price > 0);
			}
			overallImagePath = goldShopDefinition.CardImageContentPathItem;
			cardImageRatio = goldShopDefinition.CardImageRatio;
			localImageName = goldShopDefinition.LocalImageName;
			break;
		}
		}
		UpdateIcon(reward, localImageName, overallImagePath, cardImageRatio);
	}

	public void OnClicked()
	{
		if (canBuyBundle)
		{
			UIEvent.Send("NewShopSetRightVisibleEvent", true);
			UIEvent.Send("NewShopSelectedEvent", bindData);
		}
	}

	private void SetSelectState(bool selected)
	{
		Helpers.GameObjectSetActive(SelectIcon, selected);
	}

	private void SetSelect(UINewShopItemData newSelect)
	{
		if (newSelect != null && bindData != null && bindData.GetDataID() == newSelect.GetDataID())
		{
			SetSelectState(selected: true);
		}
		else
		{
			SetSelectState(selected: false);
		}
	}

	private void UpdateIcon(IReward firstReward, string LocalImageName, string OverallImagePath, int CardImageRatio)
	{
		if (firstReward == null)
		{
			UpdateIconUseParam(LocalImageName, OverallImagePath, CardImageRatio);
		}
		else
		{
			UpdateIconFirstReward(firstReward, LocalImageName, OverallImagePath, CardImageRatio);
		}
	}

	private void UpdateIconFirstReward(IReward firstReward, string LocalImageName, string OverallImagePath, int CardImageRatio)
	{
		if (firstReward == null)
		{
			return;
		}
		if (!(firstReward is RewardEquipToken upForTrade))
		{
			if (!(firstReward is RewardEquipment rewardEquipment))
			{
				if (firstReward is RewardRandomEquipment reward)
				{
					Helpers.GameObjectSetActive(equipmentRandomButton, value: true);
					equipmentRandomButton.Setup(reward);
				}
				else if (string.IsNullOrEmpty(LocalImageName) && string.IsNullOrEmpty(OverallImagePath))
				{
					string spriteName = "";
					UIAtlas iconNameForIReward = HelpersGfx.GetIconNameForIReward(firstReward, out spriteName, uiCurrencyAtlas, uiShopAtlas, uiCampAtlas);
					if (!HelpersUI.SetSpriteAndAtlas(itemSprite, spriteName, iconNameForIReward))
					{
						UpdateIconUseParam(LocalImageName, OverallImagePath, CardImageRatio);
					}
				}
				else
				{
					UpdateIconUseParam(LocalImageName, OverallImagePath, CardImageRatio);
				}
			}
			else if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
			{
				Helpers.GameObjectSetActive(consumableTexture, value: true);
				consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			}
			else
			{
				Helpers.GameObjectSetActive(equipmentButton, value: true);
				EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
				bool flag = equipmentDefinition?.TraitsOverride != null && equipmentDefinition.TraitsOverride.Count > 0;
				equipmentButton.Setup(rewardEquipment, allowClick: true, !flag);
			}
		}
		else
		{
			dataType = ItemBundleType.token;
			if (equipmentTokenButton != null)
			{
				Helpers.GameObjectSetActive(equipmentTokenButton, value: true);
				equipmentTokenButton.SetUpForTrade(upForTrade);
			}
		}
	}

	private void UpdateIconUseParam(string LocalImageName, string OverallImagePath, int CardImageRatio)
	{
		if (!string.IsNullOrEmpty(LocalImageName))
		{
			UpdateLocalIcon(LocalImageName);
		}
		else if (!string.IsNullOrEmpty(OverallImagePath))
		{
			UpdateCdnIcon(OverallImagePath, CardImageRatio);
		}
		else if (string.IsNullOrEmpty(LocalImageName) && string.IsNullOrEmpty(OverallImagePath))
		{
			if (bindData.GetShopType() == UINewShopLineData.NewShopItemType.Gold)
			{
				Helpers.GameObjectSetActive(DefaultIconGold, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(DefaultIcon, value: true);
			}
		}
	}

	private void UpdateLocalIcon(string LocalImageName)
	{
		Texture texture = (Texture)UnityUtils.LoadFromAssetBundle(LocalImageName, "itemgraphics");
		if (texture != null)
		{
			Helpers.GameObjectSetActive(DynamicIcon, value: true);
			DynamicIcon.mainTexture = texture;
		}
		else if (bindData.GetShopType() == UINewShopLineData.NewShopItemType.Gold)
		{
			Helpers.GameObjectSetActive(DefaultIconGold, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(DefaultIcon, value: true);
		}
	}

	private void UpdateCdnIcon(string OverallImagePath, int CardImageRatio)
	{
		Helpers.GameObjectSetActive(DynamicIcon, value: true);
		UITextureSnapToHeight uITextureSnapToHeight = Helpers.AddComponent<UITextureSnapToHeight>(DynamicIcon.gameObject);
		if (uITextureSnapToHeight != null)
		{
			uITextureSnapToHeight.SetCustomAspect(CardImageRatio);
		}
		LoadImageFromCdn.LoadImageToTarget(DynamicIcon, OverallImagePath);
	}
}
