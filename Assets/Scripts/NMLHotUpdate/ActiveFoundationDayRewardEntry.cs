using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class ActiveFoundationDayRewardEntry : MonoBehaviour
{
	[SerializeField]
	private UITexture texture;

	[SerializeField]
	private UISprite currencyIconSprite;

	[SerializeField]
	private UISprite classIconSprite;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private GameObject claimedContainer;

	[SerializeField]
	private GameObject amountContainer;

	[SerializeField]
	private GameObject lockedStateContainer;

	[SerializeField]
	private GameObject remedyStateContainer;

	[SerializeField]
	private GameObject claimableStateContainer;

	[SerializeField]
	private GameObject premiumContainer;

	[SerializeField]
	private GameObject premiumBg;

	[SerializeField]
	private GameObject infoButton;

	[SerializeField]
	private GameObject freemiumOpeningEffect;

	[SerializeField]
	private GameObject premiumOpeningEffect;

	[SerializeField]
	private GameObject smokeyOpeningEffect;

	[SerializeField]
	private UITexture specialWeaponTexture;

	[SerializeField]
	private UITexture specialArmorTexture;

	[SerializeField]
	private GameObject specialRewardContainer;

	[SerializeField]
	private GameObject specialRewardOutfitContainer;

	[SerializeField]
	private UILabel specialRewardOutfitSeasonLabel;

	[SerializeField]
	private UILabel specialRewardOutfitNameLabel;

	[SerializeField]
	private UISprite outfitIcon;

	[SerializeField]
	private GameObject apocalypticEffect;

	[SerializeField]
	private GameObject apocalypticIcon;

	private ActiveFoundationManager activeFoundation;

	private int tierNo;

	private bool isPremium;

	private int rewardIndex;

	private bool isInteractable;

	private IReward reward;

	private bool IsClaimable => activeFoundation.CurrentPeriodModel.IsClaimable(tierNo, isPremium);

	private bool IsRemedyable => activeFoundation.CurrentPeriodModel.IsRemedyable(tierNo, isPremium);

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "UpdateActiveFoundationDayEvent")
		{
			RefreshState();
		}
	}

	private void Awake()
	{
		activeFoundation = GameManager.Instance.playerModel.ActiveFoundationManager;
	}

	public void Bind(IReward reward, int tier, bool premium, int index, bool interactable = true, bool isSpecial = false)
	{
		this.reward = reward;
		tierNo = tier;
		isPremium = premium;
		rewardIndex = index;
		isInteractable = interactable;
		SetReward(reward, isSpecial);
		RefreshState();
		Helpers.GameObjectSetActive(premiumContainer, premium);
		Helpers.GameObjectSetActive(premiumBg, premium);
		UpdateApocalypticEffectUI();
	}

	public void BindNormalFree(IReward reward)
	{
		this.reward = reward;
		tierNo = -1;
		isPremium = false;
		rewardIndex = -1;
		isInteractable = false;
		SetReward(reward);
		RefreshState();
		Helpers.GameObjectSetActive(premiumContainer, value: false);
		Helpers.GameObjectSetActive(premiumBg, value: false);
		UpdateApocalypticEffectUI();
	}

	public void BindNormalPremium(IReward reward)
	{
		this.reward = reward;
		tierNo = -1;
		isPremium = true;
		rewardIndex = -1;
		isInteractable = false;
		SetReward(reward);
		RefreshState();
		Helpers.GameObjectSetActive(premiumContainer, value: true);
		Helpers.GameObjectSetActive(premiumBg, value: true);
		UpdateApocalypticEffectUI();
	}

	public void RefreshState()
	{
		Helpers.GameObjectSetActive(claimedContainer, value: false);
		Helpers.GameObjectSetActive(claimableStateContainer, value: false);
		Helpers.GameObjectSetActive(lockedStateContainer, value: false);
		Helpers.GameObjectSetActive(remedyStateContainer, value: false);
		switch (activeFoundation.CurrentPeriodModel.GetRewardStatus(tierNo, isPremium))
		{
		case ActiveFoundationRewardStatus.ReadyToBeClaim:
			Helpers.GameObjectSetActive(claimableStateContainer, value: true);
			break;
		case ActiveFoundationRewardStatus.ReadyToBeRemedy:
			Helpers.GameObjectSetActive(remedyStateContainer, value: true);
			break;
		case ActiveFoundationRewardStatus.Lock:
			Helpers.GameObjectSetActive(lockedStateContainer, value: true);
			break;
		case ActiveFoundationRewardStatus.Claimed:
			Helpers.GameObjectSetActive(claimedContainer, value: true);
			break;
		}
		UpdateApocalypticEffectUI();
	}

	public void Click()
	{
		if (IsClaimable)
		{
			Claim();
			UIEvent.Send("UpdateActiveFoundationDayEvent");
		}
		else if (IsRemedyable)
		{
			ActiveFoundationPeriodModel currentPeriodModel = activeFoundation.CurrentPeriodModel;
			if (currentPeriodModel == null)
			{
				return;
			}
			if (currentPeriodModel.CanRemedy)
			{
				ConsumeCurrencyCommandUtils.Execute(new ActiveFoundationRemedyCommand(tierNo + 1)
				{
					Cashier = currentPeriodModel.GetRemedyCashier()
				}, delegate(TWDModelResult result)
				{
					if (result == TWDModelResult.OK)
					{
						UIEvent.Send("UpdateActiveFoundationDayEvent");
					}
				});
			}
			else
			{
				UIEvent.Send("UpdateActiveFoundationDayEvent");
			}
		}
		else
		{
			if (reward is RewardCurrency rewardCurrency && GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency.CurrencyType))
			{
				TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(reward));
			}
			if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
			{
				TooltipManager.OpenTextBoxWithText(base.gameObject, LocalizationManager.GetText("Popup.Shop.Currency.EquipTokenBP.Tooltip"));
			}
		}
	}

	private void Claim()
	{
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		List<IReward> claimableRewards = activeFoundation.CurrentPeriodModel.GetClaimableRewards(tierNo);
		if ((bool)iAPConfirmPopupNew)
		{
			iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), LocalizationManager.GetText("Popup.IAPConfirm.Message"));
			iAPConfirmPopupNew.OpenForRewards(claimableRewards);
		}
		ExecuteClaimCommand();
	}

	private void ExecuteClaimCommand()
	{
		Helpers.ExecuteCommand(new ActiveFoundationClaimRewardCommand
		{
			Day = tierNo + 1
		});
		Helpers.InstantiateToParentAndLayer(isPremium ? premiumOpeningEffect : freemiumOpeningEffect, base.gameObject);
		Helpers.InstantiateToParentAndLayer(smokeyOpeningEffect, base.gameObject);
	}

	private void SetReward(IReward reward, bool isSpecial = false)
	{
		Helpers.GameObjectSetActive(amountContainer, value: false);
		Helpers.GameObjectSetActive(currencyIconSprite, value: false);
		Helpers.GameObjectSetActive(texture, value: false);
		Helpers.GameObjectSetActive(classIconSprite, value: false);
		Helpers.GameObjectSetActive(infoButton, value: false);
		Helpers.GameObjectSetActive(specialArmorTexture, value: false);
		Helpers.GameObjectSetActive(specialWeaponTexture, value: false);
		Helpers.GameObjectSetActive(specialRewardContainer, value: false);
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (!(reward is RewardCurrency { Amount: var amount } rewardCurrency))
		{
			if (!(reward is RewardEquipment rewardEquipment))
			{
				if (!(reward is RewardRandomEquipment))
				{
					if (!(reward is RewardMissingTokens rewardMissingTokens))
					{
						if (!(reward is RewardTimedBonus rewardTimedBonus))
						{
							if (!(reward is RewardOutfit rewardOutfit))
							{
								if (!(reward is RewardHeroSkin rewardHeroSkin))
								{
									if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
									{
										Helpers.GameObjectSetActive(infoButton, value: true);
										EquipTokenDefinition equipTokenDefinition = modelManager.GameEconomyData.GetEquipTokenDefinition(rewardEquipToken.EquipTokenId);
										int rewardAmount = rewardEquipToken.RewardAmount;
										amountLabel.text = rewardAmount.ToString();
										Helpers.GameObjectSetActive(amountContainer, rewardAmount > 1);
										Helpers.GameObjectSetActive(amountLabel, rewardAmount > 1);
										Helpers.GameObjectSetActive(texture, value: true);
										texture.mainTexture = HelpersGfx.GetEquipmentTokenIconTexture(equipTokenDefinition);
									}
								}
								else
								{
									Helpers.GameObjectSetActive(infoButton, value: true);
									HelpersGfx.GetIconNameForIReward(rewardHeroSkin, out var spriteName, null, null, null, GameManager.Instance.playerModel);
									if (!isSpecial)
									{
										Helpers.GameObjectSetActive(outfitIcon, value: true);
										outfitIcon.spriteName = spriteName;
										return;
									}
									Helpers.GameObjectSetActive(specialRewardContainer, value: true);
									Helpers.GameObjectSetActive(specialRewardOutfitContainer, value: true);
									HeroSkinDefinition skinDefinition = GameManager.Instance.gameEconomyData.GetSkinDefinition(rewardHeroSkin.PreferredOrder[0]);
									specialRewardOutfitNameLabel.text = LocalizationManager.GetText(skinDefinition.LocalizationKey);
									specialRewardOutfitSeasonLabel.text = LocalizationManager.GetText(skinDefinition.SeasonLocalizationKey);
								}
							}
							else
							{
								Helpers.GameObjectSetActive(infoButton, value: true);
								Helpers.GameObjectSetActive(outfitIcon, value: true);
								HelpersGfx.GetIconNameForIReward(rewardOutfit, out var spriteName2, null, null, null, GameManager.Instance.playerModel);
								outfitIcon.spriteName = spriteName2;
							}
						}
						else
						{
							Helpers.GameObjectSetActive(currencyIconSprite, value: true);
							currencyIconSprite.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
						}
					}
					else
					{
						RewardCurrency rewardCurrency2 = new RewardCurrency
						{
							CurrencyType = rewardMissingTokens.RewardCurrencyType,
							Amount = GameManager.Instance.playerModel.BlackMarket.LastAmountMissingTokensGiven,
							IsDiamondExchange = false
						};
						int rewardAmount = rewardCurrency2.Amount;
						amountLabel.text = rewardAmount.ToString();
						Helpers.GameObjectSetActive(amountLabel, rewardAmount > 1);
						Helpers.GameObjectSetActive(currencyIconSprite, value: true);
						currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency2.CurrencyType);
					}
				}
				else
				{
					Helpers.GameObjectSetActive(currencyIconSprite, value: true);
					currencyIconSprite.spriteName = HelpersGfx.GetSpriteNameForLootType(DropEventDefinition.DropEventTag.PreferEquipment);
				}
				return;
			}
			if (rewardEquipment.IsConsumableReward(modelManager))
			{
				int rewardAmount = rewardEquipment.Amount;
				amountLabel.text = rewardAmount.ToString();
				Helpers.GameObjectSetActive(amountContainer, rewardAmount > 1);
				Helpers.GameObjectSetActive(texture, value: true);
				texture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
				return;
			}
			RewardEquipment rewardEquipment2 = rewardEquipment;
			if (rewardEquipment2.IsConsumableReward(modelManager))
			{
				return;
			}
			if (!isSpecial)
			{
				Helpers.GameObjectSetActive(texture, value: true);
				Helpers.GameObjectSetActive(infoButton, value: true);
				texture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment2);
				return;
			}
			Helpers.GameObjectSetActive(specialRewardContainer, value: true);
			if (rewardEquipment2.EquipmentDefinition(GameManager.Instance.modelManager).Category == EquipmentCategory.Armor)
			{
				Helpers.GameObjectSetActive(specialArmorTexture, value: true);
				Helpers.GameObjectSetActive(infoButton, value: true);
				specialArmorTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment2);
			}
			else
			{
				Helpers.GameObjectSetActive(specialWeaponTexture, value: true);
				Helpers.GameObjectSetActive(infoButton, value: true);
				specialWeaponTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment2);
			}
		}
		else
		{
			amountLabel.text = amount.ToString();
			Helpers.GameObjectSetActive(amountContainer, amount > 1);
			Helpers.GameObjectSetActive(currencyIconSprite, value: true);
			currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
		}
	}

	public void InfoButtonClick()
	{
		if (reward is RewardEquipment rewardEquipment)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
			if (rewardEquipment.RarityLevel >= 5)
			{
				EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
				obj.OpenForBundleReward(rewardEquipment);
				obj.ShowNextLevel = false;
				return;
			}
			EquipmentUpgradePopup equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentPreview) as EquipmentUpgradePopup;
			if (equipmentUpgradePopup != null)
			{
				PlayerModel playerModel = GameManager.Instance.playerModel;
				EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
				equipmentUpgradePopup.ShowNextLevel = false;
				int equipmentStartingLevel = playerModel.LootManager.GetEquipmentStartingLevel(rewardEquipment.StartingLevelOffset, equipmentDefinition.SurvivorClass);
				equipmentUpgradePopup.OpenForModel(playerModel.Equipment.GenerateAndInitializeEquipmentFromDefinition(equipmentDefinition.ID, rewardEquipment.RarityLevel, equipmentStartingLevel, new ModelRandom(0), startModel: false));
				equipmentUpgradePopup.EnableOwnCloseArea(enable: true);
			}
		}
		else if (reward is RewardOutfit rewardOutfit)
		{
			SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
			if (survivorInfoPopup != null)
			{
				OutfitDefinition outfitDefinition = GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]);
				SurvivorModel survivorModel = GameManager.Instance.modelManager.Player.SurvivorContainer.Survivors.FirstOrDefault((SurvivorModel x) => !x.IsHero);
				if (survivorModel != null)
				{
					survivorInfoPopup.OpenForOutfitPreview(survivorModel, outfitDefinition);
				}
			}
		}
		else if (reward is RewardHeroSkin rewardHeroSkin)
		{
			SurvivorInfoPopup survivorInfoPopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
			if (!(survivorInfoPopup2 != null))
			{
				return;
			}
			HeroSkinDefinition skinDefinition = GameManager.Instance.gameEconomyData.GetSkinDefinition(rewardHeroSkin.PreferredOrder[0]);
			SurvivorModel survivorById = GameManager.Instance.playerModel.SurvivorContainer.GetSurvivorById(skinDefinition.HeroID);
			if (survivorById != null)
			{
				survivorInfoPopup2.OpenForHeroSkinPreview(survivorById, skinDefinition);
				return;
			}
			ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.ActorDefinitions.FirstOrDefault((ActorDefinition x) => x.ID == skinDefinition.HeroID);
			int highestLevelSurvivor = GameManager.Instance.playerModel.SurvivorContainer.GetHighestLevelSurvivor();
			if (actorDefinition != null)
			{
				SurvivorModel survivorModel2 = GameManager.Instance.playerModel.SurvivorContainer.CreateSurvivorFromDefinition(actorDefinition.ID, highestLevelSurvivor, highestLevelSurvivor, actorDefinition.RarityLevel, highestLevelSurvivor, actorDefinition.InitialEquipmentRarityLevel, new ModelRandom(), actorDefinition.InitialEquipmentsData[0].ID, actorDefinition.InitialEquipmentsData[1].ID, isMock: true);
				survivorModel2.SetupMockTraits();
				ActorView.PrepareActor(survivorModel2, isTransient: true);
				survivorInfoPopup2.OpenForHeroSkinPreview(survivorModel2, skinDefinition);
			}
		}
		else if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
		{
			EquipmentUpgradePopup obj2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			obj2.OpenForRewardEquipTokenApocalyptic(rewardEquipToken);
			obj2.ShowNextLevel = false;
		}
	}

	public void UpdateApocalypticEffectUI()
	{
		Helpers.GameObjectSetActive(apocalypticEffect, value: false);
		Helpers.GameObjectSetActive(apocalypticIcon, value: false);
		if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
		{
			Helpers.GameObjectSetActive(apocalypticIcon, value: true);
		}
		if (!isInteractable && activeFoundation != null && activeFoundation.CurrentPeriodModel != null)
		{
			Helpers.GameObjectSetActive(apocalypticEffect, activeFoundation.CurrentPeriodModel.CanShowApocalypseEffect(tierNo, isPremium));
		}
	}
}
