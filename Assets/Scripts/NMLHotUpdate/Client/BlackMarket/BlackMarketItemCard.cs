using TWDModel;
using UnityEngine;

namespace Client.BlackMarket
{
	public class BlackMarketItemCard : ShopCardBase<BlackMarketDefinition>
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
		private UILabel soldOutLabel;

		[SerializeField]
		private UILabel restockingLabel;

		[SerializeField]
		private UISprite priceSprite;

		[SerializeField]
		private UILabel freeLabel;

		[SerializeField]
		private ColorAsset enoughCurrencyColor;

		[SerializeField]
		private ColorAsset notEnoughCurrencyColor;

		[Header("Black market tokens")]
		[SerializeField]
		private UILabel blackMarketTokenGiven;

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

		[Header("Unlock/Upgrade hero controller")]
		[SerializeField]
		private BlackMarketCardUnlockHeroController unlockHeroController;

		private IReward reward;

		private int priceToPay;

		private CurrencyType currencyToPay;

		private string equipmentName;

		private EquipmentUpgradePopup equipmentUpgradePopup;

		private int convertedAmount;

		private BlackMarketDefinition definition => GetData();

		public override void SetData(BlackMarketDefinition data)
		{
			base.SetData(data);
			Rewards rewards = new Rewards(definition.Reward);
			reward = rewards.RewardsList[0];
			UpdateUI();
		}

		public override void UpdateUI()
		{
			UpdateName();
			UpdatePrice();
			UpdateSprite();
			UpdateBlackMarketTokensGiven();
			UpdateUnlockHeroPanel();
			UpdateBuyButton();
		}

		private void UpdateBuyButton()
		{
			bool flag = GameManager.Instance.playerModel.BlackMarket.CanPurchaseItem(definition);
			HelpersUI.SetButtonState(buyButton, (!flag) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
			restockingLabel.gameObject.SetActive(value: false);
			soldOutLabel.gameObject.SetActive(value: false);
			if (flag)
			{
				HelpersUI.SetButtonState(buyButton, UIButtonColor.State.Normal);
				return;
			}
			if (GameManager.Instance.playerModel.BlackMarket.IsUniqueItemAlreadySold(definition))
			{
				soldOutLabel.gameObject.SetActive(value: true);
			}
			else
			{
				restockingLabel.gameObject.SetActive(value: true);
			}
			HelpersUI.SetButtonState(buyButton, UIButtonColor.State.Disabled);
		}

		private void UpdateName()
		{
			string rewardLocalizedName = HelpersLocalization.GetRewardLocalizedName(reward, definition.UniqueId + GameManager.Instance.playerModel.BlackMarket.PurchaseHistory.Count);
			HelpersUI.SetContentToLabel(itemNameLabel, rewardLocalizedName);
		}

		private void UpdatePrice()
		{
			if (!GameManager.Instance.playerModel.BlackMarket.CanPurchaseItem(definition))
			{
				freeLabel.gameObject.SetActive(value: false);
				priceLabel.gameObject.SetActive(value: false);
				return;
			}
			CurrencyType currencyType = definition.GetCurrencyType();
			priceSprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyType);
			int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(currencyType);
			int price = definition.GetPrice(GameManager.Instance.modelManager);
			if (price <= 0)
			{
				freeLabel.gameObject.SetActive(value: true);
				priceLabel.gameObject.SetActive(value: false);
				return;
			}
			freeLabel.gameObject.SetActive(value: false);
			priceLabel.gameObject.SetActive(value: true);
			HelpersUI.SetContentToLabel(priceLabel, price.ToString());
			Color color = ((price <= currencyAmount) ? enoughCurrencyColor.Color : notEnoughCurrencyColor.Color);
			HelpersUI.SetColor(priceLabel, color);
		}

		private void UpdateSprite()
		{
			rewardSprite.gameObject.SetActive(value: false);
			rewardLabel.gameObject.SetActive(value: false);
			rewardConsumableTexture.gameObject.SetActive(value: false);
			equipmentCardContainer.SetActive(value: false);
			equipmentTokenCardContainer.SetActive(value: false);
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
								if (reward is RewardMissingTokens rewardMissingTokens)
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

		private void UpdateBlackMarketTokensGiven()
		{
			int num = definition.BlackMarketToken;
			if (reward is RewardMissingTokens rewardMissingTokens)
			{
				num *= rewardMissingTokens.GetTokenAmount(GameManager.Instance.modelManager);
			}
			blackMarketTokenGiven.transform.parent.gameObject.SetActive(num > 0);
			HelpersUI.SetContentToLabel(blackMarketTokenGiven, num.ToString());
		}

		private void UpdateUnlockHeroPanel()
		{
			unlockHeroController.gameObject.SetActive(value: false);
			ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(definition.ActorDefinitionID);
			if (actorDefinition == null)
			{
				return;
			}
			bool flag = GameManager.Instance.playerModel.SurvivorContainer.HasHero(definition.ActorDefinitionID);
			if (definition.MinStars > 0)
			{
				if (!flag)
				{
					unlockHeroController.SetActorDefinition(actorDefinition);
					unlockHeroController.ShowUpgradeRequirement(definition.MinStars - 1);
					unlockHeroController.ShowUnlockBottomPanel();
					unlockHeroController.gameObject.SetActive(value: true);
					return;
				}
				if (GameManager.Instance.playerModel.SurvivorContainer.GetSurvivorById(definition.ActorDefinitionID).SurvivorRarityLevel < definition.MinStars - 1)
				{
					ShowUpgradeHeroPanel(actorDefinition);
					return;
				}
			}
			else if (!flag && definition.NeedHeroUnlocked)
			{
				ShowUnlockHeroPanel(actorDefinition);
				return;
			}
			if (!(reward is RewardMissingTokens rewardMissingTokens))
			{
				return;
			}
			actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinitionForToken(rewardMissingTokens.RewardCurrencyType);
			flag = GameManager.Instance.playerModel.SurvivorContainer.HasHero(actorDefinition.ID);
			if (rewardMissingTokens.GetTokenAmount(GameManager.Instance.modelManager) < 0)
			{
				if (flag)
				{
					ShowUpgradeHeroPanel(actorDefinition);
					unlockHeroController.HideRequirements();
				}
				else
				{
					ShowUnlockHeroPanel(actorDefinition);
				}
			}
		}

		private void ShowUnlockHeroPanel(ActorDefinition actorDefinition)
		{
			unlockHeroController.SetActorDefinition(actorDefinition);
			unlockHeroController.ShowUnlockRequirement();
			unlockHeroController.ShowUnlockBottomPanel();
			unlockHeroController.gameObject.SetActive(value: true);
		}

		private void ShowUpgradeHeroPanel(ActorDefinition actorDefinition)
		{
			unlockHeroController.SetActorDefinition(actorDefinition);
			unlockHeroController.ShowUpgradeRequirement(definition.MinStars - 1);
			unlockHeroController.ShowUpgradeBottomPanel();
			unlockHeroController.gameObject.SetActive(value: true);
		}

		public override void Clear()
		{
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
			else if (equipmentTokenButton != null)
			{
				equipmentTokenButton.OnEquipmentButtonClicked();
			}
			else if (equipmentRandomButton != null)
			{
				equipmentRandomButton.OnButtonClicked(null);
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

		private void OnBuyButtonClickEventHandler(UIButtonExtended button)
		{
			priceToPay = definition.GetPrice(GameManager.Instance.modelManager);
			currencyToPay = definition.GetCurrencyType();
			if (GameManager.Instance.playerModel.GetCurrencyAmount(currencyToPay) >= priceToPay)
			{
				if ((reward is RewardEquipment rewardEquipment && !rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager)) || reward is RewardRandomEquipment)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
					equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
					equipmentUpgradePopup.ShowNextLevel = false;
					equipmentUpgradePopup.OpenForEquipmentInBlackMarket(definition, reward, OpenConfirmationPopup);
					equipmentName = HelpersLocalization.GetRewardLocalizedName(reward, definition.UniqueId + GameManager.Instance.playerModel.BlackMarket.PurchaseHistory.Count);
					CampHUD.Get().PauseCurrencyMeters = false;
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
				}
				else
				{
					equipmentName = string.Empty;
					OpenConfirmationPopup();
				}
			}
			else if (currencyToPay == CurrencyType.Diamonds)
			{
				MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, definition.GetPrice(GameManager.Instance.modelManager));
			}
			else if (currencyToPay == CurrencyType.Outpost)
			{
				int amount = priceToPay - GameManager.Instance.modelManager.Player.GetCurrencyAmount(currencyToPay);
				RewardCurrency rewardCurrency = new RewardCurrency();
				rewardCurrency.Amount = amount;
				rewardCurrency.CurrencyType = currencyToPay;
				convertedAmount = GameManager.Instance.gameEconomyData.CurrencyToDiamonds(currencyToPay, amount);
				BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
				obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.BuyMissingResources"), "", convertedAmount, CurrencyType.Diamonds, rewardCurrency);
				obj.SetCallbacks(CheckCanAfford);
				obj.Open();
			}
		}

		private void CheckCanAfford()
		{
			if (GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Diamonds) >= convertedAmount)
			{
				ExecuteBuyCommand();
			}
			else
			{
				MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, convertedAmount);
			}
		}

		private void OpenConfirmationPopup()
		{
			BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
			obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), equipmentName, priceToPay, currencyToPay, reward);
			obj.SetCallbacks(ExecuteBuyCommand);
			obj.Open();
		}

		private void ExecuteBuyCommand()
		{
			equipmentUpgradePopup?.Close();
			if (Helpers.ExecuteCommand(new BuyBlackMarketItemCommand(definition.UniqueId)) == TWDModelResult.OK)
			{
				BlackMarketShopController.Instance.UpdateUI();
				ShowGoodies(reward);
			}
		}

		private void OnUIEventHandler(string type, object parameter)
		{
			if (type == "OnPopUpClose")
			{
				UpdateUI();
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
						if (!(reward is RewardRandomEquipment))
						{
							if (!(reward is RewardTimedBonus timedReward))
							{
								if (!(reward is RewardOutfit rewardOutfit))
								{
									if (reward is RewardMissingTokens rewardMissingTokens)
									{
										RewardCurrency rewardCurrency = new RewardCurrency();
										rewardCurrency.CurrencyType = rewardMissingTokens.RewardCurrencyType;
										rewardCurrency.Amount = GameManager.Instance.playerModel.BlackMarket.LastAmountMissingTokensGiven;
										rewardCurrency.IsDiamondExchange = false;
										iAPConfirmPopupNew.OpenForCurrency(rewardCurrency, isGift: false);
									}
								}
								else
								{
									iAPConfirmPopupNew.OpenForOutfit(rewardOutfit);
								}
							}
							else
							{
								iAPConfirmPopupNew.OpenForTimedReward(timedReward);
							}
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
						if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
						{
							return;
						}
					}
					iAPConfirmPopupNew.OpenForEquipment(GameManager.Instance.playerModel.LootManager.LastTradedEquipment);
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
		}
	}
}
