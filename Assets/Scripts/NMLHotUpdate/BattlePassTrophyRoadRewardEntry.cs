using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BattlePassTrophyRoadRewardEntry : MonoBehaviour
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

	[SerializeField]
	private SpeedUpTitle speedUpTitle;

	[SerializeField]
	private EquipmentTokenButton Apocalyptic_TokenEquipmentButton;

	[SerializeField]
	private GameObject skillParent;

	[SerializeField]
	private UISprite skillIcon;

	[SerializeField]
	private UISprite skillBgIcon;

	[SerializeField]
	private UISprite skillClassIcon;

	[SerializeField]
	private UITableList starList;

	private BattlePassModel battlePass;

	private int tierNo;

	private bool isPremium;

	private int rewardIndex;

	private bool isInteractable;

	private IReward reward;

	private bool IsClaimable => battlePass.IsClaimable(tierNo, isPremium, rewardIndex);

	private void Awake()
	{
		battlePass = GameManager.Instance.playerModel.BattlePass;
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
		speedUpTitle?.UpdateUI(reward);
	}

	public void RefreshState()
	{
		Helpers.GameObjectSetActive(claimedContainer, battlePass.IsClaimed(tierNo, isPremium, rewardIndex));
		Helpers.GameObjectSetActive(claimableStateContainer, IsClaimable || !isInteractable);
		Helpers.GameObjectSetActive(lockedStateContainer, battlePass.ReachedTier < tierNo || (isPremium && !battlePass.PremiumActive));
	}

	public void Click()
	{
		if (IsClaimable && isInteractable)
		{
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			IReward reward = this.reward;
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (reward is RewardEquipment rewardEquipment)
				{
					ExecuteClaimCommand();
					if (!rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager) && (bool)iAPConfirmPopupNew)
					{
						iAPConfirmPopupNew.OpenForEquipment(rewardEquipment.GivenEquipment);
						iAPConfirmPopupNew.SetContent(HelpersLocalization.GetEquipmentName(rewardEquipment.GivenEquipment), LocalizationManager.GetText("Popup.IAPConfirm.Message"));
						iAPConfirmPopupNew.DisableSkipButton();
					}
					return;
				}
				if (reward is RewardRandomEquipment)
				{
					ExecuteClaimCommand();
					if ((bool)iAPConfirmPopupNew)
					{
						iAPConfirmPopupNew.OpenForRewards(battlePass.LastClaimedRewards.RewardsList);
						iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.BattlePass.RandomEquipment.Title"), LocalizationManager.GetText("Popup.BattlePass.RandomEquipment.Subtitle"));
					}
					return;
				}
				if (reward is RewardEquipToken item)
				{
					ExecuteClaimCommand();
					List<IReward> list = new List<IReward>();
					list.Add(item);
					if ((bool)iAPConfirmPopupNew)
					{
						iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), LocalizationManager.GetText("Popup.IAPConfirm.Message"));
						iAPConfirmPopupNew.OpenForRewards(list);
					}
					return;
				}
				if (reward is RewardRemoldSkill item2)
				{
					ExecuteClaimCommand();
					if ((bool)iAPConfirmPopupNew)
					{
						List<IReward> rewards = new List<IReward> { item2 };
						iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), LocalizationManager.GetText("Popup.IAPConfirm.Message"));
						iAPConfirmPopupNew.OpenForRewards(rewards);
					}
					return;
				}
			}
			else
			{
				if (GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency.CurrencyType))
				{
					PlayerModel playerModel = GameManager.Instance.playerModel;
					int currencyAmount = playerModel.GetCurrencyAmount(rewardCurrency.CurrencyType);
					int max = playerModel.GetCurrency(rewardCurrency.CurrencyType).Max;
					int amount = currencyAmount + rewardCurrency.Amount - max;
					int num = GameManager.Instance.modelManager.GameEconomyData.CurrencyToDiamonds(rewardCurrency.CurrencyType, amount, GameManager.Instance.modelManager.Player);
					if (num > 0)
					{
						TokenConversionPopup obj = (TokenConversionPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TokenConversionPopup);
						obj.OpenForCurrency(num);
						obj.SetConversionCallbacks(ExecuteClaimCommand, RefreshState);
						return;
					}
				}
				CampView.Instance.BuildingsHud.CreateCollectAnim(rewardCurrency.CurrencyType, null, rewardCurrency.Amount);
			}
			ExecuteClaimCommand();
			return;
		}
		if (this.reward is RewardCurrency rewardCurrency2 && GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency2.CurrencyType))
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(this.reward));
		}
		if (this.reward is RewardRemoldSkill rewardRemoldSkill)
		{
			SPRemoldTraitsSkillMergedPopup sPRemoldTraitsSkillMergedPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillMergedPopup) as SPRemoldTraitsSkillMergedPopup;
			if (sPRemoldTraitsSkillMergedPopup != null)
			{
				SPTraitsRemoldDefinitions minRemoldDefinitionForGroup = Helpers.GetMinRemoldDefinitionForGroup(rewardRemoldSkill.SpRemoldSkillType);
				sPRemoldTraitsSkillMergedPopup.Setup(minRemoldDefinitionForGroup.ID);
				sPRemoldTraitsSkillMergedPopup.Open();
			}
		}
	}

	private void ExecuteClaimCommand()
	{
		Helpers.ExecuteCommand(new BattlePassClaimRewardCommand
		{
			TierNo = tierNo,
			IsPremium = isPremium,
			RewardIndex = rewardIndex
		});
		Helpers.InstantiateToParentAndLayer(isPremium ? premiumOpeningEffect : freemiumOpeningEffect, base.gameObject);
		Helpers.InstantiateToParentAndLayer(smokeyOpeningEffect, base.gameObject);
		RefreshState();
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
		Helpers.GameObjectSetActive(skillParent, value: false);
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
									if (!(reward is RewardEquipToken rewardEquipToken))
									{
										if (reward is RewardRemoldSkill rewardRemoldSkill)
										{
											SPTraitsRemoldDefinitions minRemoldDefinitionForGroup = Helpers.GetMinRemoldDefinitionForGroup(rewardRemoldSkill.SpRemoldSkillType);
											if (minRemoldDefinitionForGroup != null && !(skillParent == null) && !(skillIcon == null) && !(skillClassIcon == null) && !(skillBgIcon == null) && !(starList == null))
											{
												Helpers.GameObjectSetActive(skillParent, value: true);
												HelpersUI.SetTraitsIconOnSprite(skillIcon, minRemoldDefinitionForGroup.SPTraitsIcon, minRemoldDefinitionForGroup.SPTraitsIconOnCloud);
												skillClassIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(minRemoldDefinitionForGroup.AvailableClass);
												skillBgIcon.color = Helpers.HexToColor(minRemoldDefinitionForGroup.Color);
												starList.Setup(minRemoldDefinitionForGroup.Star);
											}
										}
									}
									else if (Helpers.IsApocalyptic(rewardEquipToken))
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
							amountLabel.text = HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration);
							Helpers.GameObjectSetActive(amountContainer, value: true);
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
				Helpers.GameObjectSetActive(amountContainer, rewardAmount > 0);
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
			Helpers.GameObjectSetActive(amountContainer, amount > 0);
			Helpers.GameObjectSetActive(currencyIconSprite, value: true);
			currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			if (amount == -1)
			{
				Helpers.GameObjectSetActive(amountContainer, value: true);
				amountLabel.text = HelpersLocalization.GetBundleTitleForIReward(rewardCurrency);
			}
		}
	}

	public void InfoButtonClick()
	{
		if (reward is RewardEquipment rewardEquipment)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
			if (rewardEquipment.RarityLevel >= 5)
			{
				EquipmentUpgradePopup equipmentUpgradePopup = ((rewardEquipment.GivenEquipment == null) ? Helpers.OpenEquipmentUpgradePopupPreview(rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager), rewardEquipment.RarityLevel) : Helpers.OpenEquipmentUpgradePopup(rewardEquipment.GivenEquipment));
				if (equipmentUpgradePopup != null)
				{
					equipmentUpgradePopup.ShowNextLevel = false;
				}
				return;
			}
			EquipmentUpgradePopup equipmentUpgradePopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentPreview) as EquipmentUpgradePopup;
			if (equipmentUpgradePopup2 != null)
			{
				PlayerModel playerModel = GameManager.Instance.playerModel;
				EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
				equipmentUpgradePopup2.ShowNextLevel = false;
				int equipmentStartingLevel = playerModel.LootManager.GetEquipmentStartingLevel(rewardEquipment.StartingLevelOffset, equipmentDefinition.SurvivorClass);
				equipmentUpgradePopup2.OpenForModel(playerModel.Equipment.GenerateAndInitializeEquipmentFromDefinition(equipmentDefinition.ID, rewardEquipment.RarityLevel, equipmentStartingLevel, new ModelRandom(0), startModel: false));
				equipmentUpgradePopup2.EnableOwnCloseArea(enable: true);
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
			EquipTokenDefinition equipTokenDefinition = GameManager.Instance.playerModel.gameEconomyData.GetEquipTokenDefinition(rewardEquipToken.EquipTokenId);
			if (equipTokenDefinition != null)
			{
				Helpers.OpenEquipmentUpgradePopupPreview(GameManager.Instance.playerModel.gameEconomyData.GetEquipmentDefinition(equipTokenDefinition.RelateEquipId), equipTokenDefinition.Star).ShowNextLevel = false;
			}
		}
	}

	public void UpdateApocalypticEffectUI()
	{
		Helpers.GameObjectSetActive(apocalypticEffect, value: false);
		Helpers.GameObjectSetActive(apocalypticIcon, value: false);
		Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: false);
		if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
		{
			Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: true);
			Apocalyptic_TokenEquipmentButton.SetUpForCampaign(rewardEquipToken);
		}
		if (battlePass != null)
		{
			Helpers.GameObjectSetActive(apocalypticEffect, battlePass.CanShowApocalypseEffect(tierNo, isPremium));
		}
	}
}
