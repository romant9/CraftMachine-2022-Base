using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class LoginSevenDayReward : MonoBehaviour
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
	private UISprite avatarIconSprite;

	[SerializeField]
	private UISprite avatarIconEffect;

	[SerializeField]
	private UITexture borderIcon;

	[SerializeField]
	private UISprite borderIconSprite;

	[SerializeField]
	private UISprite borderIconEffect;

	[SerializeField]
	private GameObject apocalypticEffect;

	[SerializeField]
	private GameObject apocalypticIcon;

	private SevenDayLoginDayItemModel _sevenDayLoginDayItemModel;

	private SevenDayLoginRewardType _sevenDayLoginRewardType;

	private SevenDayLoginRewardStatus _sevenDayLoginRewardStatus;

	private IReward _reward;

	public void UpdateUI(SevenDayLoginDayItemModel model, SevenDayLoginRewardType type)
	{
		if (model == null)
		{
			Debug.LogError("[LoginSevenDayReward] UpdateUI Failed!!!");
			return;
		}
		_sevenDayLoginDayItemModel = model;
		_sevenDayLoginRewardType = type;
		Helpers.GameObjectSetActive(premiumContainer, value: false);
		Helpers.GameObjectSetActive(premiumBg, value: false);
		Helpers.GameObjectSetActive(claimedContainer, value: false);
		Helpers.GameObjectSetActive(lockedStateContainer, value: false);
		Helpers.GameObjectSetActive(claimableStateContainer, value: false);
		Helpers.GameObjectSetActive(remedyStateContainer, value: false);
		switch (type)
		{
		case SevenDayLoginRewardType.Free:
			SetReward(model.FreeReward.Reward);
			_sevenDayLoginRewardStatus = model.FreeRewardStatus;
			switch (model.FreeRewardStatus)
			{
			case SevenDayLoginRewardStatus.ReadyToBeClaim:
				Helpers.GameObjectSetActive(claimableStateContainer, value: true);
				break;
			case SevenDayLoginRewardStatus.ReadyToBeRemedy:
				Helpers.GameObjectSetActive(remedyStateContainer, value: true);
				break;
			case SevenDayLoginRewardStatus.Claimed:
				Helpers.GameObjectSetActive(claimedContainer, value: true);
				break;
			case SevenDayLoginRewardStatus.Lock:
				Helpers.GameObjectSetActive(lockedStateContainer, value: true);
				break;
			}
			break;
		case SevenDayLoginRewardType.Premium:
			SetReward(model.PremiumReward.Reward);
			Helpers.GameObjectSetActive(premiumContainer, value: true);
			Helpers.GameObjectSetActive(premiumBg, value: true);
			_sevenDayLoginRewardStatus = model.PremiumRewardStatus;
			switch (model.PremiumRewardStatus)
			{
			case SevenDayLoginRewardStatus.ReadyToBeClaim:
				Helpers.GameObjectSetActive(claimableStateContainer, value: true);
				break;
			case SevenDayLoginRewardStatus.ReadyToBeRemedy:
				Helpers.GameObjectSetActive(remedyStateContainer, value: true);
				break;
			case SevenDayLoginRewardStatus.Claimed:
				Helpers.GameObjectSetActive(claimedContainer, value: true);
				break;
			case SevenDayLoginRewardStatus.Lock:
				Helpers.GameObjectSetActive(lockedStateContainer, value: true);
				break;
			}
			break;
		}
		UpdateApocalypticEffectUI();
	}

	private void SetReward(IReward reward, bool isSpecial = true)
	{
		_reward = reward;
		Helpers.GameObjectSetActive(amountContainer, value: false);
		Helpers.GameObjectSetActive(currencyIconSprite, value: false);
		Helpers.GameObjectSetActive(texture, value: false);
		Helpers.GameObjectSetActive(avatarIcon, value: false);
		Helpers.GameObjectSetActive(avatarIconSprite, value: false);
		Helpers.GameObjectSetActive(avatarIconEffect, value: false);
		Helpers.GameObjectSetActive(borderIcon, value: false);
		Helpers.GameObjectSetActive(borderIconSprite, value: false);
		Helpers.GameObjectSetActive(borderIconEffect, value: false);
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
					return;
				}
				if (rewardAvatars.Avatar >= 0 && avatarIcon != null)
				{
					AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(rewardAvatars.Avatar);
					if (avatarsDefinition != null)
					{
						if (!string.IsNullOrEmpty(avatarsDefinition.LocalImg))
						{
							HelpersUI.SetSprite(avatarIconSprite, avatarsDefinition.LocalImg);
							Helpers.GameObjectSetActive(avatarIconSprite, value: true);
							Helpers.GameObjectSetActive(avatarIconEffect, avatarsDefinition.LocalEffectType > 0);
							LoadImageFromCdn.LoadImageToTarget(avatarIcon, "", clearLocalCachedUrls: false, 10);
						}
						else
						{
							LoadImageFromCdn.LoadImageToTarget(avatarIcon, avatarsDefinition?.Image, clearLocalCachedUrls: false, 10);
						}
					}
				}
				else if (rewardAvatars.Border >= 0 && borderIcon != null)
				{
					BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(rewardAvatars.Border);
					if (bordersDefinition != null)
					{
						if (!string.IsNullOrEmpty(bordersDefinition.LocalImg))
						{
							HelpersUI.SetSprite(borderIconSprite, bordersDefinition.LocalImg);
							Helpers.GameObjectSetActive(borderIconSprite, value: true);
							Helpers.GameObjectSetActive(borderIconEffect, bordersDefinition.LocalEffectType > 0);
							LoadImageFromCdn.LoadImageToTarget(borderIcon, "", clearLocalCachedUrls: false, 10);
						}
						else
						{
							LoadImageFromCdn.LoadImageToTarget(borderIcon, bordersDefinition?.Image, clearLocalCachedUrls: false, 10);
						}
					}
				}
				Helpers.GameObjectSetActive(amountContainer, value: true);
				amountLabel.text = HelpersLocalization.GetBundleTitleForIReward(rewardAvatars);
				return;
			}
			if (rewardEquipment.IsConsumableReward(modelManager))
			{
				int amount2 = rewardEquipment.Amount;
				amountLabel.text = amount2.ToString();
				Helpers.GameObjectSetActive(amountContainer, amount2 > (IsLoadDataManager ? 1 : 0));
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
			Helpers.GameObjectSetActive(amountContainer, amount > (IsLoadDataManager ? 1 : 0));
			Helpers.GameObjectSetActive(currencyIconSprite, value: true);
			currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			if (amount == -1)
			{
				Helpers.GameObjectSetActive(amountContainer, value: true);
				amountLabel.text = HelpersLocalization.GetBundleTitleForIReward(rewardCurrency);
			}
		}
	}

	public void ClaimedClick()
	{
		if (_sevenDayLoginDayItemModel == null)
		{
			Debug.LogError("[LoginSevenDayReward] ClaimedClick Error: model null!!!");
			return;
		}
		switch (_sevenDayLoginRewardStatus)
		{
		case SevenDayLoginRewardStatus.ReadyToBeClaim:
			Claim();
			break;
		case SevenDayLoginRewardStatus.ReadyToBeRemedy:
		{
			SevenDayLoginPeriodModel sevenDayLoginPeriodModel = GameManager.Instance.playerModel?.SevenDayLoginManager?.CurrentPeriodModel;
			if (sevenDayLoginPeriodModel == null)
			{
				break;
			}
			if (sevenDayLoginPeriodModel.CanRemedy)
			{
				ConsumeCurrencyCommandUtils.Execute(new SevenDayLoginRemedyCommand(_sevenDayLoginDayItemModel.Day)
				{
					Cashier = sevenDayLoginPeriodModel.GetRemedyCashier()
				}, delegate(TWDModelResult result)
				{
					if (result == TWDModelResult.OK)
					{
						UIEvent.Send("UpdateLoginSevenDayEvent");
					}
				});
			}
			else
			{
				UIEvent.Send("ShowLoginSevenDayInfoEvent");
			}
			break;
		}
		}
	}

	private void Claim()
	{
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
				DebugTWD.Log("Is new RewardTimedBonus", DebugType.System);
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
						UpdateUI(_sevenDayLoginDayItemModel, _sevenDayLoginRewardType);
					});
					return;
				}
			}
			if (!IsLoadDataManager) CampView.Instance?.BuildingsHud?.CreateCollectAnim(rewardCurrency.CurrencyType, null, rewardCurrency.Amount);
		}
		ExecuteClaimCommand();
	}

	private void ExecuteClaimCommand()
	{
		if (Helpers.ExecuteCommand(new SevenDayLoginClaimRewardCommand(_sevenDayLoginDayItemModel.Day, _sevenDayLoginRewardType)) == TWDModelResult.OK)
		{
			Helpers.InstantiateToParentAndLayer((_sevenDayLoginRewardType == SevenDayLoginRewardType.Premium) ? premiumOpeningEffect : freemiumOpeningEffect, base.gameObject);
			Helpers.InstantiateToParentAndLayer(smokeyOpeningEffect, base.gameObject);
			UpdateUI(_sevenDayLoginDayItemModel, _sevenDayLoginRewardType);
		}
	}

	public void InfoButtonClick()
	{
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

	public void UpdateApocalypticEffectUI()
	{
		Helpers.GameObjectSetActive(apocalypticEffect, value: false);
		Helpers.GameObjectSetActive(apocalypticIcon, value: false);
		if (_reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
		{
			Helpers.GameObjectSetActive(apocalypticIcon, value: true);
		}
		SevenDaysRewardDefinition rewardDefinition = _sevenDayLoginDayItemModel.RewardDefinition;
		if (rewardDefinition != null)
		{
			bool value = false;
			switch (_sevenDayLoginRewardType)
			{
			case SevenDayLoginRewardType.Free:
				value = rewardDefinition.IsApocalypseFreeReward;
				break;
			case SevenDayLoginRewardType.Premium:
				value = rewardDefinition.IsApocalypsePremiumReward;
				break;
			}
			Helpers.GameObjectSetActive(apocalypticEffect, value);
		}
	}


	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion
}
