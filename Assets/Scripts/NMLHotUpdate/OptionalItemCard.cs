using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class OptionalItemCard : MonoBehaviour
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
	private GameObject infoButton;

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
	private EquipmentTokenButton Apocalyptic_TokenEquipmentButton;

	[SerializeField]
	private UITexture avatarIcon;

	[SerializeField]
	private UITexture borderIcon;

	private IReward reward;

	[SerializeField]
	private GameObject randomEquipmentButtonPrefab;

	[SerializeField]
	private GameObject equipmentButtonPrefab;

	[SerializeField]
	private GameObject equipmentParent;

	[SerializeField]
	public Vector3 equipmentCardScale = new Vector3(0.5f, 0.5f, 1f);

	public void Init(IReward r)
	{
		reward = r;
		SetReward(r);
		UpdateApocalypticEffectUI();
		Helpers.GameObjectSetActive(lockedStateContainer, value: false);
		Helpers.GameObjectSetActive(claimedContainer, value: false);
	}

	private void SetReward(IReward reward, bool isSpecial = false)
	{
		Helpers.GameObjectSetActive(amountContainer, value: false);
		Helpers.GameObjectSetActive(currencyIconSprite, value: false);
		Helpers.GameObjectSetActive(texture, value: false);
		Helpers.GameObjectSetActive(avatarIcon, value: false);
		Helpers.GameObjectSetActive(borderIcon, value: false);
		Helpers.GameObjectSetActive(classIconSprite, value: false);
		Helpers.GameObjectSetActive(infoButton, value: false);
		Helpers.GameObjectSetActive(specialArmorTexture, value: false);
		Helpers.GameObjectSetActive(specialWeaponTexture, value: false);
		Helpers.GameObjectSetActive(specialRewardContainer, value: false);
		Helpers.GameObjectSetActive(equipmentParent, value: false);
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (!(reward is RewardCurrency { Amount: var amount } rewardCurrency))
		{
			if (!(reward is RewardEquipment rewardEquipment))
			{
				if (!(reward is RewardRandomEquipment rewardEquip))
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
										if (reward is RewardAvatars rewardAvatars)
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
									}
									else if (Helpers.IsApocalyptic(rewardEquipToken))
									{
										Helpers.GameObjectSetActive(infoButton, value: true);
										EquipTokenDefinition equipTokenDefinition = modelManager.GameEconomyData.GetEquipTokenDefinition(rewardEquipToken.EquipTokenId);
										int rewardAmount = rewardEquipToken.RewardAmount;
										amountLabel.text = rewardAmount.ToString();
										Helpers.GameObjectSetActive(amountContainer, rewardAmount > 1);
										Helpers.GameObjectSetActive(texture, value: true);
										texture.mainTexture = HelpersGfx.GetEquipmentTokenIconTexture(equipTokenDefinition);
									}
								}
								else
								{
									Helpers.GameObjectSetActive(infoButton, value: true);
									HelpersGfx.GetIconNameForIReward(rewardHeroSkin, out var _, null, null, null, GameManager.Instance.playerModel);
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
						Helpers.GameObjectSetActive(amountContainer, rewardAmount > 1);
						Helpers.GameObjectSetActive(currencyIconSprite, value: true);
						currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency2.CurrencyType);
					}
				}
				else
				{
					CreateEquipmentCardsAndSetActive(rewardEquip);
				}
			}
			else if (rewardEquipment.IsConsumableReward(modelManager))
			{
				int rewardAmount = rewardEquipment.Amount;
				amountLabel.text = rewardAmount.ToString();
				Helpers.GameObjectSetActive(amountContainer, rewardAmount > 0);
				Helpers.GameObjectSetActive(texture, value: true);
				texture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			}
			else
			{
				RewardEquipment rewardEquipment2 = rewardEquipment;
				if (!rewardEquipment2.IsConsumableReward(modelManager))
				{
					CreateEquipmentCardsAndSetActive(rewardEquipment2);
				}
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

	private void UpdateApocalypticEffectUI()
	{
		Helpers.GameObjectSetActive(apocalypticEffect, value: false);
		Helpers.GameObjectSetActive(apocalypticIcon, value: false);
		Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: false);
		if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
		{
			Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: true);
			Helpers.GameObjectSetActive(texture, value: false);
			Apocalyptic_TokenEquipmentButton.SetUpForCampaign(rewardEquipToken);
			Helpers.GameObjectSetActive(apocalypticEffect, value: true);
		}
	}

	public void SetSelectStatus(CustomRewardStatus status)
	{
		switch (status)
		{
		case CustomRewardStatus.Normal:
			Helpers.GameObjectSetActive(lockedStateContainer, value: false);
			Helpers.GameObjectSetActive(claimedContainer, value: false);
			break;
		case CustomRewardStatus.Selected:
			Helpers.GameObjectSetActive(lockedStateContainer, value: false);
			Helpers.GameObjectSetActive(claimedContainer, value: true);
			break;
		case CustomRewardStatus.CannotSelect:
			Helpers.GameObjectSetActive(lockedStateContainer, value: true);
			Helpers.GameObjectSetActive(claimedContainer, value: false);
			break;
		default:
			Helpers.GameObjectSetActive(lockedStateContainer, value: false);
			Helpers.GameObjectSetActive(claimedContainer, value: false);
			break;
		}
	}

	private void CreateEquipmentCardsAndSetActive(IReward rewardEquip)
	{
		Helpers.GameObjectSetActive(infoButton, value: true);
		if (rewardEquip is RewardRandomEquipment && randomEquipmentButtonPrefab != null)
		{
			Helpers.GameObjectSetActive(equipmentParent, value: true);
			GameObject gameObject = null;
			gameObject = ((!(equipmentParent != null) || equipmentParent.transform.childCount <= 0) ? Helpers.InstantiateToParent(randomEquipmentButtonPrefab, equipmentParent) : equipmentParent.transform.GetChild(0).gameObject);
			if (gameObject != null)
			{
				gameObject.transform.localScale = equipmentCardScale;
				EquipmentRandomButton component = gameObject.GetComponent<EquipmentRandomButton>();
				if (component != null)
				{
					component.Setup((RewardRandomEquipment)rewardEquip);
				}
				BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
			}
		}
		else
		{
			if ((rewardEquip is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager)) || !(rewardEquip is RewardEquipment) || !(equipmentButtonPrefab != null))
			{
				return;
			}
			Helpers.GameObjectSetActive(equipmentParent, value: true);
			GameObject gameObject2 = null;
			gameObject2 = ((!(equipmentParent != null) || equipmentParent.transform.childCount <= 0) ? Helpers.InstantiateToParent(equipmentButtonPrefab, equipmentParent) : equipmentParent.transform.GetChild(0).gameObject);
			if (gameObject2 != null)
			{
				gameObject2.transform.localScale = equipmentCardScale;
				EquipmentButton component3 = gameObject2.GetComponent<EquipmentButton>();
				if (component3 != null)
				{
					component3.Setup((RewardEquipment)rewardEquip);
				}
				BoxCollider component4 = gameObject2.GetComponent<BoxCollider>();
				if (component4 != null)
				{
					component4.enabled = false;
				}
			}
		}
	}
}
