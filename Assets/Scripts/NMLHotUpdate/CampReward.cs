using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CampReward : MonoBehaviour
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
	private GameObject infoButton;

	[SerializeField]
	private UISprite outfitIcon;

	[SerializeField]
	private GameObject apocalypticIcon;

	[SerializeField]
	private EquipmentTokenButton Apocalyptic_TokenEquipmentButton;

	private IReward reward;

	public void Bind(IReward reward)
	{
		this.reward = reward;
		SetReward(reward);
		UpdateApocalypticEffectUI();
	}

	private void SetReward(IReward reward)
	{
		Helpers.GameObjectSetActive(amountContainer, value: false);
		Helpers.GameObjectSetActive(currencyIconSprite, value: false);
		Helpers.GameObjectSetActive(texture, value: false);
		Helpers.GameObjectSetActive(classIconSprite, value: false);
		Helpers.GameObjectSetActive(infoButton, value: false);
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
									Helpers.GameObjectSetActive(outfitIcon, value: true);
									outfitIcon.spriteName = spriteName;
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
					Helpers.GameObjectSetActive(texture, value: true);
					Helpers.GameObjectSetActive(infoButton, value: true);
					texture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment2);
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

	public void UpdateApocalypticEffectUI()
	{
		Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: false);
		if (reward is RewardEquipToken rewardEquipToken && Helpers.IsApocalyptic(rewardEquipToken))
		{
			Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: true);
			Apocalyptic_TokenEquipmentButton.SetUpForCampaign(rewardEquipToken);
		}
	}
}
