using System.Collections;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class RouletteRewardCard : MonoBehaviour
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
	private GameObject amountContainer;

	[SerializeField]
	private GameObject premiumContainer;

	[SerializeField]
	private GameObject premiumBg;

	[SerializeField]
	private UITexture specialWeaponTexture;

	[SerializeField]
	private UITexture specialArmorTexture;

	[SerializeField]
	private GameObject specialRewardContainer;

	[SerializeField]
	private GameObject getAllContainer;

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
	private GameObject contentGo;

	[SerializeField]
	private EquipmentButton equipmentButton;

	[SerializeField]
	private GameObject smallDelContainer;

	[SerializeField]
	private GameObject largeDelContainer;

	[SerializeField]
	public GameObject highLightGo;

	[SerializeField]
	public GameObject getAllHighLightGo;

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
	private UITexture skinTexture;

	[SerializeField]
	public GameObject effect;

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

	[SerializeField]
	private GameObject skillCurrencyParent;

	[SerializeField]
	private UISprite skillCurrencyBg;

	[SerializeField]
	private UISprite skillCurrencyIcon;

	private IReward reward;

	public int amount { get; private set; }

	public void Bind(IReward reward, bool isSpecial = false, bool isPremium = false, bool isDel = false)
	{
		this.reward = reward;
		SetReward(reward, isSpecial, isPremium, isDel);
		Helpers.GameObjectSetActive(premiumContainer, isPremium);
		Helpers.GameObjectSetActive(premiumBg, isPremium);
		UpdateApocalypticEffectUI();
	}

	public void SetAmountContainerEnable(bool enable)
	{
		Helpers.GameObjectSetActive(amountContainer, enable);
	}

	private void SetReward(IReward reward, bool isSpecial = false, bool isPremium = false, bool isDel = false)
	{
		Helpers.GameObjectSetActive(amountContainer, value: false);
		Helpers.GameObjectSetActive(currencyIconSprite, value: false);
		Helpers.GameObjectSetActive(texture, value: false);
		Helpers.GameObjectSetActive(classIconSprite, value: false);
		Helpers.GameObjectSetActive(specialArmorTexture, value: false);
		Helpers.GameObjectSetActive(specialWeaponTexture, value: false);
		Helpers.GameObjectSetActive(specialRewardContainer, value: false);
		Helpers.GameObjectSetActive(getAllContainer, value: false);
		Helpers.GameObjectSetActive(contentGo, value: true);
		Helpers.GameObjectSetActive(equipmentButton, value: false);
		Helpers.GameObjectSetActive(smallDelContainer, value: false);
		Helpers.GameObjectSetActive(largeDelContainer, value: false);
		Helpers.GameObjectSetActive(avatarIcon, value: false);
		Helpers.GameObjectSetActive(avatarIconSprite, value: false);
		Helpers.GameObjectSetActive(avatarIconEffect, value: false);
		Helpers.GameObjectSetActive(borderIcon, value: false);
		Helpers.GameObjectSetActive(borderIconSprite, value: false);
		Helpers.GameObjectSetActive(borderIconEffect, value: false);
		Helpers.GameObjectSetActive(skinTexture, value: false);
		Helpers.GameObjectSetActive(skillParent, value: false);
		Helpers.GameObjectSetActive(skillCurrencyParent, value: false);
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		amount = 0;
		if (isDel)
		{
			Helpers.GameObjectSetActive(contentGo, value: false);
			if (reward is RewardEquipment rewardEquipment && !rewardEquipment.IsConsumableReward(modelManager))
			{
				Helpers.GameObjectSetActive(largeDelContainer, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(smallDelContainer, value: true);
			}
		}
		else if (reward != null)
		{
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (!(reward is RewardEquipment rewardEquipment2))
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
											if (!(reward is RewardAvatars rewardAvatars))
											{
												if (reward is RewardRemoldSkill rewardRemoldSkill)
												{
													SPTraitsRemoldDefinitions minRemoldDefinitionForGroup = Helpers.GetMinRemoldDefinitionForGroup(rewardRemoldSkill.SpRemoldSkillType);
													amount = rewardRemoldSkill.Amount;
													if (minRemoldDefinitionForGroup != null && !(skillParent == null) && !(skillIcon == null) && !(skillClassIcon == null) && !(skillBgIcon == null) && !(starList == null))
													{
														Helpers.GameObjectSetActive(skillParent, value: true);
														HelpersUI.SetTraitsIconOnSprite(skillIcon, minRemoldDefinitionForGroup.SPTraitsIcon, minRemoldDefinitionForGroup.SPTraitsIconOnCloud);
														skillClassIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(minRemoldDefinitionForGroup.AvailableClass);
														skillBgIcon.color = Helpers.HexToColor(minRemoldDefinitionForGroup.Color);
														starList.Setup(minRemoldDefinitionForGroup.Star);
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
											Helpers.GameObjectSetActive(amountLabel, value: true);
											amountLabel.text = HelpersLocalization.GetBundleTitleForIReward(rewardAvatars);
										}
										else if (Helpers.IsApocalyptic(rewardEquipToken))
										{
											EquipTokenDefinition equipTokenDefinition = modelManager.GameEconomyData.GetEquipTokenDefinition(rewardEquipToken.EquipTokenId);
											amount = rewardEquipToken.RewardAmount;
											amountLabel.text = amount.ToString();
											Helpers.GameObjectSetActive(amountContainer, amount > 1);
											Helpers.GameObjectSetActive(amountLabel, amount > 1);
											Helpers.GameObjectSetActive(texture, value: true);
											texture.mainTexture = HelpersGfx.GetEquipmentTokenIconTexture(equipTokenDefinition);
										}
									}
									else
									{
										HelpersGfx.GetIconNameForIReward(rewardHeroSkin, out var spriteName, null, null, null, GameManager.Instance.playerModel);
										if (!isSpecial)
										{
											if (OfflineManager.IsLoadDataManager && HeroSkinTexture != null)
											{
												var rewardName = rewardHeroSkin.PreferredOrder.First();
												if (rewardName.Contains('-')) rewardName = rewardName.Split('-').Last();
												UnityEngine.Object obj = UnityUtils.LoadFromAssetBundle(rewardName, "itemgraphics");
												if (obj != null)
												{
													Helpers.GameObjectSetActive(HeroSkinTexture, value: true);
													HeroSkinTexture.uvRect = new Rect(0, 0, 1, 1);
													HeroSkinTexture.mainTexture = (Texture)obj;
												}
												Helpers.GameObjectSetActive(amountContainer, value: true);
												Helpers.GameObjectSetActive(amountLabel, value: true);
												amountLabel.text = HelpersLocalization.GetBundleTitleForIReward(rewardHeroSkin);
											}
											else
											{
												Helpers.GameObjectSetActive(outfitIcon, value: true);
												outfitIcon.spriteName = spriteName;
											}
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
							amount = rewardCurrency2.Amount;
							amountLabel.text = amount.ToString();
							Helpers.GameObjectSetActive(amountLabel, amount > 1);
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
				else if (rewardEquipment2.IsConsumableReward(modelManager))
				{
					amount = rewardEquipment2.Amount;
					amountLabel.text = amount.ToString();
					Helpers.GameObjectSetActive(amountContainer, amount > 0);
					Helpers.GameObjectSetActive(texture, value: true);
					texture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment2);
				}
				else
				{
					RewardEquipment rewardEquipment3 = rewardEquipment2;
					if (!rewardEquipment3.IsConsumableReward(modelManager))
					{
						Helpers.GameObjectSetActive(contentGo, value: false);
						Helpers.GameObjectSetActive(equipmentButton, value: true);
						EquipmentDefinition equipmentDefinition = rewardEquipment3.EquipmentDefinition(GameManager.Instance.modelManager);
						bool flag = equipmentDefinition?.TraitsOverride != null && equipmentDefinition.TraitsOverride.Count > 0;
						equipmentButton.Setup(rewardEquipment3, allowClick: true, !flag);
					}
				}
				return;
			}
			amount = rewardCurrency.Amount;
			amountLabel.text = amount.ToString();
			Helpers.GameObjectSetActive(amountContainer, amount > 0);
			Helpers.GameObjectSetActive(amountLabel, amount > 0);
			if (rewardCurrency.CurrencyType.ToString().Contains("SkillToken"))
			{
				SPTraitsSkillKitTokenSet sPTraitsSkillKitTokenSetByID = GameManager.Instance.playerModel.gameEconomyData.GetSPTraitsSkillKitTokenSetByID(rewardCurrency.CurrencyType.ToString());
				if (sPTraitsSkillKitTokenSetByID != null)
				{
					Helpers.GameObjectSetActive(skillCurrencyParent, value: true);
					HelpersUI.SetTraitsIconOnSprite(skillCurrencyIcon, sPTraitsSkillKitTokenSetByID.TopIcon, sPTraitsSkillKitTokenSetByID.TopIconOnCloud);
					HelpersUI.SetSprite(skillCurrencyBg, sPTraitsSkillKitTokenSetByID.BGIcon);
				}
			}
			else
			{
				Helpers.GameObjectSetActive(currencyIconSprite, value: true);
				currencyIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			}
			if (amount == -1)
			{
				Helpers.GameObjectSetActive(amountContainer, value: true);
				amountLabel.text = HelpersLocalization.GetBundleTitleForIReward(rewardCurrency);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(getAllContainer, value: true);
			Helpers.GameObjectSetActive(contentGo, value: false);
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

	public void OnSkillButtonClick()
	{
		if (reward is RewardRemoldSkill rewardRemoldSkill)
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

	public void UpdateApocalypticEffectUI()
	{
		Helpers.GameObjectSetActive(apocalypticEffect, value: false);
		Helpers.GameObjectSetActive(apocalypticIcon, value: false);
		Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: false);
		if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
		{
			Helpers.GameObjectSetActive(texture.gameObject, value: false);
			Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: true);
			Apocalyptic_TokenEquipmentButton.SetUpForCampaign(rewardEquipToken);
		}
	}

	public void ShowEffect()
	{
		if (OfflineManager.IsNoEffects) return;
		StartCoroutine(EffectEnumerator());
	}

	private IEnumerator EffectEnumerator()
	{
		Helpers.GameObjectSetActive(effect, value: true);
		yield return new WaitForSeconds(0.5f);
		Helpers.GameObjectSetActive(effect, value: false);
	}


	#region myparams
	public UITexture HeroSkinTexture;
	#endregion
}
