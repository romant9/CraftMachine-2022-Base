using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class ThreeDayReward : MonoBehaviour
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
	private GameObject remedyStateContainer;

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
	private UITexture specialTokenTexture;

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
	private UITexture avatarIcon;

	[SerializeField]
	private UITexture borderIcon;

	[SerializeField]
	private GameObject apocalypticEffect;

	[SerializeField]
	private GameObject apocalypticIcon;

	private Rewards rewards;

	private IReward _reward;

	private int rewardsIndex = -1;

	private ThreeDayRewardStatus status = ThreeDayRewardStatus.Unlock;

	private bool IsValid
	{
		get
		{
			if (GameManager.Instance.playerModel.ThreeDayModel == null)
			{
				return false;
			}
			if (!GameManager.Instance.playerModel.ThreeDayModel.CanShowThreeDay)
			{
				return false;
			}
			return true;
		}
	}

	public void UpdateUI(Rewards rew, ThreeDayRewardStatus sts, int rewIndex)
	{
		if (IsValid)
		{
			rewards = rew;
			status = sts;
			rewardsIndex = rewIndex;
			Helpers.GameObjectSetActive(premiumContainer, value: true);
			Helpers.GameObjectSetActive(premiumBg, value: true);
			Helpers.GameObjectSetActive(claimedContainer, value: false);
			Helpers.GameObjectSetActive(lockedStateContainer, value: false);
			Helpers.GameObjectSetActive(claimableStateContainer, value: false);
			Helpers.GameObjectSetActive(remedyStateContainer, value: false);
			UpdateReward(rewards);
			UpdateStatus(sts);
		}
	}

	public void UpdateReward(Rewards reward)
	{
		SetReward(reward.RewardsList[0]);
	}

	private void UpdateStatus(ThreeDayRewardStatus sts)
	{
		switch (sts)
		{
		case ThreeDayRewardStatus.Lock:
			Helpers.GameObjectSetActive(lockedStateContainer, value: true);
			break;
		case ThreeDayRewardStatus.Rewarded:
			Helpers.GameObjectSetActive(claimedContainer, value: true);
			break;
		case ThreeDayRewardStatus.Unlock:
			Helpers.GameObjectSetActive(claimableStateContainer, value: true);
			break;
		}
	}

	public void ClaimedClick()
	{
		if (GameManager.Instance.playerModel.ThreeDayModel == null || !GameManager.Instance.playerModel.ThreeDayModel.CanShowThreeDay || !GameManager.Instance.playerModel.ThreeDayModel.HasBuy || status != ThreeDayRewardStatus.Unlock || rewardsIndex < 0)
		{
			return;
		}
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		IReward reward = _reward;
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
					iAPConfirmPopupNew.OpenForRewards(new List<IReward> { _reward });
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.BattlePass.RandomEquipment.Title"), LocalizationManager.GetText("Popup.BattlePass.RandomEquipment.Subtitle"));
				}
				return;
			}
			if (reward is RewardAvatars)
			{
				ExecuteClaimCommand();
				if ((bool)iAPConfirmPopupNew)
				{
					iAPConfirmPopupNew.OpenForRewards(new List<IReward> { _reward });
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title"), LocalizationManager.GetText("Popup.IAPConfirm.Message"));
				}
				return;
			}
			if (reward is RewardEquipToken rewardEquipToken)
			{
				ExecuteClaimCommand();
				if ((bool)iAPConfirmPopupNew)
				{
					iAPConfirmPopupNew.OpenForEquipmentToken(rewardEquipToken.GivenEquipmentToken);
					iAPConfirmPopupNew.SetContent(HelpersLocalization.GetEquipmentTokenName(rewardEquipToken.GivenEquipmentToken), LocalizationManager.GetText("Popup.IAPConfirm.Message"));
					iAPConfirmPopupNew.DisableSkipButton();
				}
				return;
			}
			if (reward is RewardTimedBonus timedReward)
			{
				ExecuteClaimCommand();
				if ((bool)iAPConfirmPopupNew)
				{
					iAPConfirmPopupNew.OpenForTimedReward(timedReward, "Popup.IAPConfirm.Title.GenericReward");
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
					obj.SetConversionCallbacks(ExecuteClaimCommand, delegate
					{
					});
					return;
				}
			}
			CampView.Instance.BuildingsHud.CreateCollectAnim(rewardCurrency.CurrencyType, null, rewardCurrency.Amount);
		}
		ExecuteClaimCommand();
	}

	private void ExecuteClaimCommand()
	{
		if (Helpers.ExecuteCommand(new ThreeDayRewardCommand(rewardsIndex)) == TWDModelResult.OK)
		{
			Helpers.InstantiateToParentAndLayer(premiumOpeningEffect, base.gameObject);
			Helpers.InstantiateToParentAndLayer(smokeyOpeningEffect, base.gameObject);
			UIEvent.Send("ThreeDayFreshEvent");
		}
	}

	private void SetReward(IReward reward, bool isSpecial = true)
	{
		_reward = reward;
		Helpers.GameObjectSetActive(amountContainer, value: false);
		Helpers.GameObjectSetActive(currencyIconSprite, value: false);
		Helpers.GameObjectSetActive(texture, value: false);
		Helpers.GameObjectSetActive(avatarIcon, value: false);
		Helpers.GameObjectSetActive(borderIcon, value: false);
		Helpers.GameObjectSetActive(classIconSprite, value: false);
		Helpers.GameObjectSetActive(infoButton, value: false);
		Helpers.GameObjectSetActive(specialArmorTexture, value: false);
		Helpers.GameObjectSetActive(specialTokenTexture, value: false);
		Helpers.GameObjectSetActive(specialWeaponTexture, value: false);
		Helpers.GameObjectSetActive(specialRewardContainer, value: false);
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (!(reward is RewardCurrency { Amount: var amount } rewardCurrency))
		{
			if (!(reward is RewardEquipment rewardEquipment))
			{
				if (!(reward is RewardAvatars rewardAvatars))
				{
					if (!(reward is RewardEquipToken rewardEquipToken))
					{
						if (!(reward is RewardRandomEquipment))
						{
							if (!(reward is RewardMissingTokens rewardMissingTokens))
							{
								if (!(reward is RewardTimedBonus rewardTimedBonus))
								{
									if (!(reward is RewardOutfit reward2))
									{
										if (reward is RewardHeroSkin rewardHeroSkin)
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
										HelpersGfx.GetIconNameForIReward(reward2, out var spriteName2, null, null, null, GameManager.Instance.playerModel);
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
								int amount2 = rewardCurrency2.Amount;
								amountLabel.text = amount2.ToString();
								Helpers.GameObjectSetActive(amountLabel, amount2 > 1);
								Helpers.GameObjectSetActive(currencyIconSprite, value: true);
								currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency2.CurrencyType);
							}
						}
						else
						{
							Helpers.GameObjectSetActive(currencyIconSprite, value: true);
							currencyIconSprite.spriteName = HelpersGfx.GetSpriteNameForLootType(DropEventDefinition.DropEventTag.PreferEquipment);
						}
					}
					else
					{
						EquipTokenItemModel equipTokenItemModel = rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager);
						if (equipTokenItemModel != null)
						{
							int amount2 = rewardEquipToken.RewardAmount;
							amountLabel.text = amount2.ToString();
							Helpers.GameObjectSetActive(amountContainer, amount2 > 1);
							Helpers.GameObjectSetActive(specialTokenTexture, value: true);
							Helpers.GameObjectSetActive(infoButton, value: true);
							specialTokenTexture.mainTexture = HelpersGfx.GetEquipmentTokenIconTexture(equipTokenItemModel.Definition);
						}
					}
				}
				else
				{
					if (rewardAvatars.Avatar >= 0 && avatarIcon != null)
					{
						AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(rewardAvatars.Avatar);
						LoadImageFromCdn.LoadImageToTarget(avatarIcon, avatarsDefinition?.Image, clearLocalCachedUrls: false, 10);
					}
					else if (rewardAvatars.Border >= 0 && borderIcon != null)
					{
						BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(rewardAvatars.Border);
						LoadImageFromCdn.LoadImageToTarget(borderIcon, bordersDefinition?.Image, clearLocalCachedUrls: false, 10);
					}
					Helpers.GameObjectSetActive(amountContainer, value: true);
					amountLabel.text = HelpersLocalization.GetBundleTitleForIReward(rewardAvatars);
				}
				return;
			}
			if (rewardEquipment.IsConsumableReward(modelManager))
			{
				int amount2 = rewardEquipment.Amount;
				amountLabel.text = amount2.ToString();
				Helpers.GameObjectSetActive(amountContainer, amount2 > 0);
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
		if (!IsValid)
		{
			return;
		}
		if (_reward is RewardEquipment rewardEquipment)
		{
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
		else if (_reward is RewardEquipToken rewardEquipToken)
		{
			RewardEquipment rewardEquipment2 = rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager).RewardEquipment;
			EquipmentUpgradePopup equipmentUpgradePopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			if (equipmentUpgradePopup2 != null)
			{
				equipmentUpgradePopup2.ShowNextLevel = false;
				equipmentUpgradePopup2.OpenForBundleReward(rewardEquipment2);
			}
		}
		else if (_reward is RewardOutfit rewardOutfit)
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
		else
		{
			if (!(_reward is RewardHeroSkin rewardHeroSkin))
			{
				return;
			}
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
	}
}
