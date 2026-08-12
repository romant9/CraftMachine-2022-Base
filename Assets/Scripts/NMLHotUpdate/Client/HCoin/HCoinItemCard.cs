using System;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

namespace Client.HCoin
{
	public class HCoinItemCard : ShopCardBase<HillTopStoreDefinition>
	{
		[Header("Name")]
		[SerializeField]
		private UILabel itemNameLabel;

		[Header("Reward")]
		[SerializeField]
		private UISprite rewardSprite;

		[SerializeField]
		private UILabel rewardLabel;

		[SerializeField]
		private UITexture rewardConsumableTexture;

		[SerializeField]
		private UIAtlas uiShopAtlas;

		[SerializeField]
		private UIAtlas uiShopSurvivorTokensAtlas;

		[Header("Price")]
		[SerializeField]
		private UILabel priceLabel;

		[SerializeField]
		private UISprite priceSprite;

		[SerializeField]
		private UILabel freeLabel;

		[SerializeField]
		private ColorAsset enoughCurrencyColor;

		[SerializeField]
		private ColorAsset notEnoughCurrencyColor;

		[Header("Equipment")]
		[SerializeField]
		private GameObject equipmentCardContainer;

		[SerializeField]
		private GameObject equipmentCardPrefab;

		[SerializeField]
		private GameObject equipmentRandomCardPrefab;

		[Header("EquipmentToken")]
		[SerializeField]
		private GameObject equipmentTokenCardContainer;

		[SerializeField]
		private GameObject equipmentTokenCardPrefab;

		private EquipmentButton equipmentButton;

		private EquipmentTokenButton equipmentTokenButton;

		private EquipmentRandomButton equipmentRandomButton;

		[Header("Buy button")]
		[SerializeField]
		private UIButtonWithLabel buyButton;

		private IReward reward;

		private int priceToPay;

		private CurrencyType currencyToPay;

		private string equipmentName;

		private EquipmentUpgradePopup equipmentUpgradePopup;

		private int convertedAmount;

		[Tooltip("Tween group will be called on the whole card when the item is purchased and should be removed from the store after the purchase")]
		[SerializeField]
		private int ItemBoughtAndRemovedTweenGroup = 5;

		[Tooltip("Tween group will be called on the itemDynamicTextureHero and itemDynamicTextureItem when the images are loaded")]
		[SerializeField]
		private int ImageLoadCompleteTweenGroup = 10;

		[SerializeField]
		private UITexture heroTex;

		[SerializeField]
		private UITexture heroImage;

		private HillTopStoreDefinition definition => GetData();

		public override void SetData(HillTopStoreDefinition data)
		{
			base.SetData(data);
			Rewards rewards = new Rewards(definition.Reward);
			reward = rewards.RewardsList[0];
			UpdateUI();
		}

		public override void UpdateUI()
		{
			if (GameManager.Instance.playerModel.HillTopStore.CanPurchaseItem(definition))
			{
				UpdateName();
				UpdatePrice();
				UpdateSprite();
				UpdateBuyButton();
			}
		}

		public virtual void OnCompleteBoughtTween()
		{
			HCoinShopController.Instance.ShowFor(HCoinShopController.Instance.ActiveHero);
		}

		private void UpdateBuyButton()
		{
			priceToPay = definition.Score;
			currencyToPay = CurrencyType.HillTopCoin;
			GameManager.Instance.playerModel.GetCurrencyAmount(currencyToPay);
			HelpersUI.SetButtonState(buyButton, UIButtonColor.State.Normal);
		}

		private void UpdateName()
		{
			string text = HelpersLocalization.GetRewardLocalizedName(reward, 0);
			if (text == "Reward Type: HeroSkin")
			{
				text = HelpersLocalization.GetBundleTitleForIReward(reward);
			}
			HelpersUI.SetContentToLabel(itemNameLabel, text);
		}

		private void UpdatePrice()
		{
			if (!GameManager.Instance.playerModel.HillTopStore.CanPurchaseItem(definition))
			{
				freeLabel.gameObject.SetActive(value: true);
				priceLabel.gameObject.SetActive(value: false);
				return;
			}
			CurrencyType currencyType = CurrencyType.HillTopCoin;
			priceSprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyType);
			int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyType);
			int score = definition.Score;
			if (score <= 0)
			{
				freeLabel.gameObject.SetActive(value: true);
				priceLabel.gameObject.SetActive(value: false);
				return;
			}
			freeLabel.gameObject.SetActive(value: false);
			priceLabel.gameObject.SetActive(value: true);
			HelpersUI.SetContentToLabel(priceLabel, score.ToString());
			Color color = ((score <= currencyAmount) ? enoughCurrencyColor.Color : notEnoughCurrencyColor.Color);
			HelpersUI.SetColor(priceLabel, color);
		}

		private void UpdateSprite()
		{
			rewardSprite.gameObject.SetActive(value: false);
			rewardLabel.gameObject.SetActive(value: false);
			rewardConsumableTexture.gameObject.SetActive(value: false);
			equipmentCardContainer.SetActive(value: false);
			equipmentTokenCardContainer.SetActive(value: false);
			heroTex.gameObject.SetActive(value: false);
			heroImage.gameObject.SetActive(value: false);
			IReward reward = this.reward;
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (!(reward is RewardEquipment rewardEquipment))
				{
					if (!(reward is RewardRandomEquipment rewardRandomEquipment))
					{
						if (!(reward is RewardEquipToken rewardEquipToken))
						{
							if (!(reward is RewardTimedBonus rewardTimedBonus))
							{
								if (!(reward is RewardMissingTokens rewardMissingTokens))
								{
									if (reward is RewardHeroSkin)
									{
										heroTex.gameObject.SetActive(value: true);
										string contentPath = "Image/Lucky_box";
										if (definition.ImagePath != null)
										{
											contentPath = definition.ImagePath;
										}
										LoadImageFromCdn.LoadImageToTarget(heroTex, contentPath, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
									}
								}
								else
								{
									HelpersGfx.SetShopAtlasToSprite(rewardMissingTokens.RewardCurrencyType, rewardSprite, uiShopAtlas, uiShopSurvivorTokensAtlas);
									rewardSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardMissingTokens.RewardCurrencyType, GameManager.Instance.playerModel);
									rewardSprite.gameObject.SetActive(value: true);
									int tokenAmount = rewardMissingTokens.GetTokenAmount(GameManager.Instance.modelManager);
									rewardLabel.text = ((tokenAmount < 0) ? string.Empty : tokenAmount.ToString());
									rewardLabel.gameObject.SetActive(value: true);
								}
							}
							else
							{
								rewardSprite.atlas = uiShopAtlas;
								rewardSprite.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
								rewardSprite.gameObject.SetActive(value: true);
							}
						}
						else
						{
							Helpers.DestroyOrCache(equipmentButton);
							Helpers.DestroyOrCache(equipmentRandomButton);
							equipmentTokenCardContainer.SetActive(value: true);
							if (equipmentTokenButton == null)
							{
								equipmentTokenButton = Helpers.InstantiateWithComponent<EquipmentTokenButton>(equipmentTokenCardPrefab, equipmentTokenCardContainer);
							}
							if (equipmentTokenButton != null)
							{
								equipmentTokenButton.SetUpForTrade(rewardEquipToken);
								rewardLabel.text = rewardEquipToken.RewardAmount.ToString();
								rewardLabel.gameObject.SetActive(value: true);
							}
						}
					}
					else
					{
						Helpers.DestroyOrCache(equipmentButton);
						Helpers.DestroyOrCache(equipmentTokenButton);
						equipmentCardContainer.SetActive(value: true);
						if (equipmentRandomButton == null)
						{
							equipmentRandomButton = Helpers.InstantiateWithComponent<EquipmentRandomButton>(equipmentRandomCardPrefab, equipmentCardContainer);
						}
						equipmentRandomButton.Setup(rewardRandomEquipment);
					}
					return;
				}
				if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
				{
					rewardSprite.gameObject.SetActive(value: false);
					rewardConsumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
					rewardConsumableTexture.gameObject.SetActive(value: true);
					rewardLabel.text = rewardEquipment.Amount.ToString();
					rewardLabel.gameObject.SetActive(value: true);
					return;
				}
				RewardEquipment rewardEquipment2 = rewardEquipment;
				if (!rewardEquipment2.IsConsumableReward(GameManager.Instance.modelManager))
				{
					equipmentCardContainer.SetActive(value: true);
					Helpers.DestroyOrCache(equipmentRandomButton);
					Helpers.DestroyOrCache(equipmentTokenButton);
					if (equipmentButton == null)
					{
						equipmentButton = Helpers.InstantiateWithComponent<EquipmentButton>(equipmentCardPrefab, equipmentCardContainer);
					}
					EquipmentDefinition equipmentDefinition = rewardEquipment2.EquipmentDefinition(GameManager.Instance.modelManager);
					bool flag = equipmentDefinition?.TraitsOverride != null && equipmentDefinition.TraitsOverride.Count > 0;
					equipmentButton.Setup(rewardEquipment2, allowClick: true, !flag);
				}
			}
			else
			{
				HelpersGfx.SetShopAtlasToSprite(rewardCurrency.CurrencyType, rewardSprite, uiShopAtlas, uiShopSurvivorTokensAtlas);
				rewardSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType, GameManager.Instance.playerModel);
				rewardSprite.gameObject.SetActive(value: true);
				rewardLabel.text = rewardCurrency.Amount.ToString();
				rewardLabel.gameObject.SetActive(value: true);
			}
		}

		public override void Clear()
		{
			TweenManager.ResetToBeginningTweenGroup(base.gameObject, ItemBoughtAndRemovedTweenGroup);
			base.Clear();
			if (equipmentButton != null && equipmentButton.gameObject != null)
			{
				Helpers.DestroyOrCache(equipmentButton.gameObject);
				equipmentButton = null;
			}
			if (equipmentTokenButton != null && equipmentTokenButton.gameObject != null)
			{
				Helpers.DestroyOrCache(equipmentTokenButton.gameObject);
				equipmentTokenButton = null;
			}
		}

		protected override void OnClickedTooltipButton(UIButtonExtended button)
		{
			base.OnClickedTooltipButton(button);
			if (equipmentButton != null)
			{
				equipmentButton.OnEquipmentButtonClicked();
			}
			else if (equipmentRandomButton != null)
			{
				equipmentRandomButton.OnButtonClicked(null);
			}
			else if (equipmentTokenButton != null)
			{
				equipmentTokenButton.OnEquipmentButtonClicked();
			}
			else
			{
				TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(reward));
			}
		}

		public override void AddListeners()
		{
			base.AddListeners();
			buyButton.SetClickCallback(OnBuyButtonClickEventHandler);
			UIEvent.OnUIEvent += OnUIEventHandler;
		}

		public override void RemoveListeners()
		{
			base.RemoveListeners();
			buyButton.RemoveClickCallback(OnBuyButtonClickEventHandler);
			UIEvent.OnUIEvent -= OnUIEventHandler;
		}

		private void GoBanana()
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

		private void OnBuyButtonClickEventHandler(UIButtonExtended button)
		{
			priceToPay = definition.Score;
			currencyToPay = CurrencyType.HillTopCoin;
			int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyToPay);
			if (currencyAmount >= priceToPay)
			{
				if ((reward is RewardEquipment rewardEquipment && !rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager)) || reward is RewardRandomEquipment)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
					equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
					equipmentUpgradePopup.ShowNextLevel = false;
					equipmentUpgradePopup.OpenForEquipmentInHillCoin(definition, reward, OpenConfirmationPopup);
					equipmentName = HelpersLocalization.GetRewardLocalizedName(reward, 0);
					CampHUD.Get().PauseCurrencyMeters = false;
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
				}
				else
				{
					equipmentName = string.Empty;
					OpenConfirmationPopup();
				}
			}
			else if (priceToPay <= 0)
			{
				ExecuteBuyCommand();
			}
			else
			{
				BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
				obj.SetYesContent(LocalizationManager.GetText("Banana.Guidance"), string.Empty, priceToPay, currencyToPay, new RewardCurrency
				{
					Amount = priceToPay - currencyAmount,
					CurrencyType = CurrencyType.HillTopCoin
				});
				obj.SetCallbacks(GoBanana);
				obj.Open();
			}
		}

		private void OpenConfirmationPopup()
		{
			BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
			obj.SetConfirmContent(LocalizationManager.GetText("Banana.Guidance"), equipmentName, priceToPay, currencyToPay, reward);
			obj.SetCallbacks(ExecuteBuyCommand);
			obj.Open();
		}

		private void ExecuteBuyCommand()
		{
			if (equipmentUpgradePopup != null)
			{
				equipmentUpgradePopup?.Close();
			}
			if (Helpers.ExecuteCommand(new BuyHillTopItemCommand(definition.UniqueId)) == TWDModelResult.OK)
			{
				HelpersUI.SetButtonState(buyButton, UIButtonColor.State.Disabled);
				ShowGoodies(reward);
			}
		}

		private void OnUIEventHandler(string type, object parameter)
		{
			if (type == "OnPopUpClose" && (parameter is IAPConfirmPopupNew || parameter is OpenLootInUi))
			{
				if (!GameManager.Instance.playerModel.HillTopStore.CanPurchaseItem(definition))
				{
					TweenManager.PlayTweenGroup(base.gameObject, ItemBoughtAndRemovedTweenGroup, forward: true, OnCompleteBoughtTween);
				}
				else
				{
					HelpersUI.SetButtonState(buyButton, UIButtonColor.State.Normal);
				}
			}
		}

		private void ShowGoodies(IReward reward)
		{
			UpdateUI();
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (!(reward is RewardCurrency currency))
			{
				if (!(reward is RewardEquipToken rewardEquipToken))
				{
					if (!(reward is RewardEquipment rewardEquipment))
					{
						if (reward is RewardRandomEquipment)
						{
							goto IL_00e3;
						}
						if (reward is RewardTimedBonus timedReward)
						{
							iAPConfirmPopupNew.OpenForTimedReward(timedReward);
							return;
						}
						if (reward is RewardOutfit rewardOutfit)
						{
							iAPConfirmPopupNew.OpenForOutfit(rewardOutfit);
							return;
						}
						if (reward is RewardMissingTokens rewardMissingTokens)
						{
							RewardCurrency rewardCurrency = new RewardCurrency();
							rewardCurrency.CurrencyType = rewardMissingTokens.RewardCurrencyType;
							rewardCurrency.Amount = GameManager.Instance.playerModel.BlackMarket.LastAmountMissingTokensGiven;
							rewardCurrency.IsDiamondExchange = false;
							iAPConfirmPopupNew.OpenForCurrency(rewardCurrency, isGift: false);
							return;
						}
						if (reward is RewardHeroSkin rewardHeroSkin)
						{
							iAPConfirmPopupNew.OpenForHeroSKin(rewardHeroSkin);
							return;
						}
					}
					else
					{
						if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
						{
							iAPConfirmPopupNew.OpenForConsumable(rewardEquipment);
							return;
						}
						if (!rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
						{
							goto IL_00e3;
						}
					}
					Debug.LogError("buysomething============");
				}
				else
				{
					iAPConfirmPopupNew.OpenForEquipmentToken(rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager));
				}
			}
			else
			{
				iAPConfirmPopupNew.OpenForCurrency(currency, isGift: false);
			}
			return;
			IL_00e3:
			iAPConfirmPopupNew.OpenForEquipment(GameManager.Instance.playerModel.LootManager.LastTradedEquipment);
		}
	}
}
