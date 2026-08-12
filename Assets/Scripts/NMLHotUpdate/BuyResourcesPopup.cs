using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BuyResourcesPopup : ConfirmationPopup
{
	[SerializeField]
	private PayButton payButton;

	[SerializeField]
	private UIButton yesButton;

	[SerializeField]
	private GameObject outfitContainer;

	[SerializeField]
	private UILabel outfitName;

	[SerializeField]
	private GameObject outfitDefault;

	[SerializeField]
	private UISprite outfitIcon;

	[SerializeField]
	private GameObject currencyContainer;

	[SerializeField]
	private UILabel rewardCurrencyAmount;

	[SerializeField]
	private UILabel rewardCurrencyDescription;

	[SerializeField]
	private UISprite rewardCurrencyIcon;

	[SerializeField]
	private UISprite rewardCurrencyIconShop;

	[SerializeField]
	private GameObject consumableContainer;

	[SerializeField]
	private UILabel rewardConsumableAmount;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UILabel rewardConsumableDescription;

	[SerializeField]
	private GameObject crateContainer;

	[SerializeField]
	private UILabel crateTitle;

	[SerializeField]
	private UILabel crateDesc;

	[SerializeField]
	private UITexture crateTexture;

	[SerializeField]
	private GameObject rewardDraw3Stars;

	[SerializeField]
	private GameObject rewardDraw4Stars;

	[SerializeField]
	private GameObject rewardDraw5Stars;

	[Header("EquipmentToken")]
	[SerializeField]
	private GameObject equipmentTokenCardContainer;

	[SerializeField]
	private GameObject equipmentTokenCardPrefab;

	[SerializeField]
	private UILabel equipTokenDescription;

	private EquipmentTokenButton equipmentTokenButton;

	[SerializeField]
	private HUDMeter diamondMeter;

	private Cashier cashier;

	private UIButton payUIButton;

	public static int DefaultDepth = 101;

	public static int TradeShopDepth = 150;

	public static int BadgeRerollDepth = 105;

	private float timeLastOpen;

	private float timeBeforeClosing = 0.2f;

	private bool CanClosePopup => Time.realtimeSinceStartup > timeLastOpen + timeBeforeClosing;

	public override void Open()
	{
		if (diamondMeter != null)
		{
			diamondMeter.SetCurrencyType(CurrencyType.Diamonds);
			diamondMeter.SetValue(GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Diamonds));
		}
		SetPayButtonEnabled(enable: true);
		base.Open();
	}

	public virtual void SetContent(string title, string info, int amount, CurrencyType currencyType = CurrencyType.Diamonds)
	{
		SetContent(title, info);
		SetButtonActive();
		if (outfitContainer != null)
		{
			outfitContainer.SetActive(value: false);
		}
		if (currencyContainer != null)
		{
			currencyContainer.SetActive(value: false);
		}
		if (crateContainer != null)
		{
			crateContainer.SetActive(value: false);
		}
		if (consumableContainer != null)
		{
			consumableContainer.SetActive(value: false);
		}
		if (equipmentTokenCardContainer != null && equipmentTokenCardContainer.transform.parent != null)
		{
			Helpers.GameObjectSetActive(equipmentTokenCardContainer.transform.parent.gameObject, value: false);
		}
		timeLastOpen = Time.realtimeSinceStartup;
		cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.None, currencyType, amount);
		payButton.UpdateUI(cashier);
	}

	public void SetConfirmContent(string title, string info, int amount, CurrencyType currencyType = CurrencyType.Diamonds, IReward reward = null)
	{
		SetContent(title, info, amount, currencyType);
		if (reward == null)
		{
			return;
		}
		if (!(reward is RewardHeroSkin reward2))
		{
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (!(reward is RewardMissingTokens rewardMissingTokens))
				{
					if (!(reward is RewardTimedBonus rewardTimedBonus))
					{
						if (!(reward is RewardTradeCrate rewardTradeCrate))
						{
							if (!(reward is RewardOutfit rewardOutfit))
							{
								if (!(reward is RewardEquipment rewardEquipment))
								{
									if (!(reward is RewardEquipToken rewardEquipToken))
									{
										return;
									}
									EquipTokenItemModel equipTokenItemModel = rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager);
									if (equipTokenItemModel != null)
									{
										Helpers.GameObjectSetActive(equipmentTokenCardContainer.transform.parent.gameObject, value: true);
										if (equipmentTokenButton == null)
										{
											equipmentTokenButton = Helpers.InstantiateWithComponent<EquipmentTokenButton>(equipmentTokenCardPrefab, equipmentTokenCardContainer);
										}
										if (equipmentTokenButton != null)
										{
											equipmentTokenButton.SetUpForReward(equipTokenItemModel);
										}
										HelpersUI.SetContentToLabel(equipTokenDescription, HelpersLocalization.GetEquipmentTokenName(equipTokenItemModel));
									}
								}
								else if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
								{
									HelpersUI.SetContentToLabel(rewardConsumableDescription, HelpersLocalization.GetEquipmentName(rewardEquipment.EquipmentId));
									Helpers.GameObjectSetActive(consumableContainer, value: true);
									consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
									rewardConsumableAmount.text = rewardEquipment.Amount.ToString();
								}
							}
							else
							{
								if (!(outfitContainer != null))
								{
									return;
								}
								outfitContainer.SetActive(value: true);
								OutfitDefinition outfitDefinition = GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]);
								if (outfitDefinition == null)
								{
									return;
								}
								outfitName.text = LocalizationManager.GetText("Bundle.Outfit.Description{Parameter}", LocalizationManager.GetText(outfitDefinition.TitleLocalizationKey));
								if (outfitIcon != null && outfitDefault != null)
								{
									if (string.IsNullOrEmpty(outfitDefinition.BundleSprite))
									{
										outfitDefault.gameObject.SetActive(value: true);
										outfitIcon.gameObject.SetActive(value: false);
									}
									else
									{
										outfitDefault.gameObject.SetActive(value: false);
										outfitIcon.gameObject.SetActive(value: true);
										outfitIcon.spriteName = outfitDefinition.BundleSprite;
									}
								}
							}
						}
						else
						{
							if (!(crateContainer != null))
							{
								return;
							}
							crateContainer.SetActive(value: true);
							Dictionary<string, GameObject> obj = new Dictionary<string, GameObject>
							{
								{ "TradeCrateGearLow", rewardDraw3Stars },
								{ "TradeCrateGearMid", rewardDraw4Stars },
								{ "TradeCrateGearHigh", rewardDraw5Stars }
							};
							if (crateTitle != null)
							{
								crateTitle.text = HelpersLocalization.GetTradeCrateName(rewardTradeCrate.TradeCrateId);
							}
							foreach (KeyValuePair<string, GameObject> item in obj)
							{
								if (item.Value != null)
								{
									item.Value.SetActive(rewardTradeCrate.TradeCrateId == item.Key);
								}
							}
							if (crateDesc != null)
							{
								crateDesc.text = LocalizationManager.GetText("TradeItems.Card.Content." + rewardTradeCrate.TradeCrateId);
							}
							if (crateTexture != null)
							{
								crateTexture.material = HelpersGfx.GetTradeCrateMaterial(rewardTradeCrate.TradeCrateId);
							}
						}
					}
					else if (currencyContainer != null)
					{
						currencyContainer.SetActive(value: true);
						Helpers.GameObjectSetActive(rewardCurrencyIconShop, value: true);
						Helpers.GameObjectSetActive(rewardCurrencyIcon, value: false);
						Helpers.GameObjectSetActive(rewardCurrencyAmount, value: false);
						if (rewardCurrencyDescription != null)
						{
							rewardCurrencyDescription.text = HelpersLocalization.GetBundleTitleForIReward(rewardTimedBonus);
						}
						if (rewardCurrencyIconShop != null)
						{
							rewardCurrencyIconShop.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
						}
					}
				}
				else if (currencyContainer != null)
				{
					currencyContainer.SetActive(value: true);
					Helpers.GameObjectSetActive(rewardCurrencyIconShop, value: false);
					PlayerModel playerModel = GameManager.Instance.playerModel;
					if (rewardCurrencyDescription != null)
					{
						rewardCurrencyDescription.text = HelpersLocalization.GetCurrencyName(rewardMissingTokens.RewardCurrencyType);
					}
					if (rewardCurrencyIcon != null)
					{
						rewardCurrencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardMissingTokens.RewardCurrencyType, playerModel);
					}
					if (rewardCurrencyAmount != null)
					{
						rewardCurrencyAmount.text = rewardMissingTokens.GetTokenAmount(GameManager.Instance.modelManager).ToString();
					}
				}
			}
			else
			{
				if (!(currencyContainer != null))
				{
					return;
				}
				currencyContainer.SetActive(value: true);
				Helpers.GameObjectSetActive(rewardCurrencyIconShop, value: false);
				Helpers.GameObjectSetActive(rewardCurrencyIcon, value: true);
				PlayerModel playerModel2 = GameManager.Instance.playerModel;
				if (rewardCurrencyDescription != null)
				{
					rewardCurrencyDescription.text = HelpersLocalization.GetCurrencyName(rewardCurrency.CurrencyType);
					if (rewardCurrency.CurrencyType == CurrencyType.HillTopCoin)
					{
						rewardCurrencyDescription.text = HelpersLocalization.GetCurrencyName(CurrencyType.Fairmoney);
					}
				}
				if (rewardCurrencyIcon != null)
				{
					rewardCurrencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType, playerModel2);
				}
				if (rewardCurrencyAmount != null)
				{
					rewardCurrencyAmount.text = rewardCurrency.Amount.ToString();
				}
			}
		}
		else if (currencyType == CurrencyType.HillTopCoin)
		{
			currencyContainer.SetActive(value: true);
			Helpers.GameObjectSetActive(rewardCurrencyIconShop, value: true);
			Helpers.GameObjectSetActive(rewardCurrencyIcon, value: false);
			_ = GameManager.Instance.playerModel;
			if (rewardCurrencyDescription != null)
			{
				rewardCurrencyDescription.text = HelpersLocalization.GetBundleTitleForIReward(reward);
			}
			if (rewardCurrencyIcon != null)
			{
				string spriteName = "";
				HelpersGfx.GetIconNameForIReward(reward2, out spriteName, null, null, null);
				HelpersUI.SetSprite(rewardCurrencyIconShop, spriteName);
			}
			if (rewardCurrencyAmount != null)
			{
				rewardCurrencyAmount.text = "";
			}
		}
	}

	public void SetYesContent(string title, string info, int amount, CurrencyType currencyType = CurrencyType.Diamonds, IReward reward = null)
	{
		SetConfirmContent(title, info, amount, currencyType, reward);
		SetButtonActive(isYesActive: true);
	}

	private void SetButtonActive(bool isYesActive = false)
	{
		if (payButton != null)
		{
			Helpers.GameObjectSetActive(payButton, !isYesActive);
		}
		if (yesButton != null)
		{
			Helpers.GameObjectSetActive(yesButton, isYesActive);
		}
	}

	public void YesPressed()
	{
		if (CanClosePopup)
		{
			base.OkPressed();
		}
	}

	public override void OkPressed()
	{
		if (CanClosePopup)
		{
			SetPayButtonEnabled(enable: false);
			if (BuyEnergyPopup.IsPayOnlyMissionCostActive)
			{
				Helpers.ExecuteCommand(new SetBlackboardToggleCommand("BuyJustEnoughGasForMission"));
			}
			else if (GameManager.Instance.playerModel.Blackboard.IsToggleOn("BuyJustEnoughGasForMission"))
			{
				Helpers.ExecuteCommand(new ClearBlackboardToggleCommand("BuyJustEnoughGasForMission"));
			}
			if (cashier.CanAfford() || (GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Diamonds) >= BuyEnergyPopup.MissionCostGold && BuyEnergyPopup.IsPayOnlyMissionCostActive))
			{
				base.OkPressed();
			}
			else
			{
				SetPayButtonEnabled(enable: true);
				okCallback?.Invoke();
			}
			BuyEnergyPopup.IsPayOnlyMissionCostActive = false;
			if (diamondMeter != null)
			{
				diamondMeter.SetValue(GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Diamonds));
			}
		}
	}

	private void SetPayButtonEnabled(bool enable)
	{
		if (payUIButton == null)
		{
			payUIButton = payButton.GetComponent<UIButton>();
		}
		HelpersUI.SetButtonState(payUIButton, (!enable) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
	}
}
