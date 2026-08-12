using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildShopItemPreview : HUDElement
{
	[Header("Tier Info")]
	[SerializeField]
	private GameObject tierContainer;

	[SerializeField]
	private UISprite tierIcon;

	[SerializeField]
	private UILabel tierNameLabel;

	[SerializeField]
	private UIProgressBar tierProgressBar;

	[SerializeField]
	private UILabel tierRequiredVpLabel;

	[SerializeField]
	private bool showOnlyRemainingVp;

	[Header("Item Info")]
	[SerializeField]
	private UIAtlas UIShopAtlas;

	[SerializeField]
	private UIAtlas UIShopSurivorTokensAtlas;

	[SerializeField]
	private GameObject shopUnlockContainer;

	[SerializeField]
	private UISprite currencyIcon;

	[SerializeField]
	private UILabel currencyAmount;

	[SerializeField]
	private UITexture crateTexture;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private GameObject equipmentContainer;

	[SerializeField]
	private GameObject equipmentCardPrefab;

	private GuildTierDefinition tierDefinition;

	private GuildShopDefinition itemReference;

	public void OpenForTier(int tierNumber)
	{
		tierDefinition = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(tierNumber);
		OpenForTier(tierDefinition);
	}

	public void OpenForTier(GuildTierDefinition tier)
	{
		tierDefinition = tier;
		SetGuildShopItemForTier(tierDefinition.Tier);
		UpdateUI();
	}

	public void OpenForGuildShopItem(GuildShopDefinition itemDefinition)
	{
		itemReference = itemDefinition;
		tierDefinition = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(itemReference.TierRequirement);
		UpdateUI();
	}

	private void SetGuildShopItemForTier(int tierNumber)
	{
		List<GuildShopDefinition> unlocksForTier = GameManager.Instance.playerModel.GuildShopModel.GetUnlocksForTier(tierNumber);
		if (unlocksForTier.Count > 0)
		{
			itemReference = unlocksForTier[0];
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateTierInfo();
		UpdateItemInfo();
	}

	private void UpdateTierInfo()
	{
		if (tierDefinition == null)
		{
			Helpers.GameObjectSetActive(tierContainer, value: false);
			return;
		}
		HelpersUI.SetSprite(tierIcon, tierDefinition.IconSprite);
		HelpersUI.SetContentToLabel(tierNameLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(tierDefinition.NameLocalizationKey));
		if (showOnlyRemainingVp)
		{
			HelpersUI.SetContentToLabel(tierRequiredVpLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GuildBattle.Tier.RemainingVp{parameter}", tierDefinition.VictoryPointsRequired));
		}
		else
		{
			HelpersUI.SetContentToLabel(tierRequiredVpLabel, tierDefinition.VictoryPointsRequired.ToString());
		}
		if (tierProgressBar != null)
		{
			tierProgressBar.Set(GuildTierHelper.GetCurrentProgressToNextTier());
		}
	}

	private void UpdateItemInfo()
	{
		if (itemReference == null || itemReference.ContentRewards.Count == 0)
		{
			Helpers.GameObjectSetActive(shopUnlockContainer, value: false);
			return;
		}
		Helpers.GameObjectSetActive(shopUnlockContainer, value: true);
		IReward reward = itemReference.ContentRewards.RewardsList[0];
		Helpers.GameObjectSetActive(currencyIcon, value: false);
		Helpers.GameObjectSetActive(currencyAmount, value: false);
		Helpers.GameObjectSetActive(crateTexture, value: false);
		Helpers.GameObjectSetActive(equipmentContainer, value: false);
		Helpers.GameObjectSetActive(consumableTexture, value: false);
		if (reward.Type == RewardType.Currency)
		{
			RewardCurrency rewardCurrency = (RewardCurrency)reward;
			HelpersGfx.SetShopAtlasToSprite(rewardCurrency.CurrencyType, currencyIcon, UIShopAtlas, UIShopSurivorTokensAtlas);
			HelpersUI.SetSprite(currencyIcon, HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType));
			HelpersUI.SetContentToLabel(currencyAmount, rewardCurrency.Amount.ToString());
		}
		else if (reward.Type == RewardType.TradeCrate)
		{
			if (crateTexture != null)
			{
				HelpersUI.SetTextureMaterial(crateTexture, HelpersGfx.GetTradeCrateMaterial(((RewardTradeCrate)reward).TradeCrateId));
			}
		}
		else if (reward.Type == RewardType.Equipment)
		{
			RewardEquipment rewardEquipment = reward as RewardEquipment;
			if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
			{
				Helpers.GameObjectSetActive(consumableTexture, base.transform);
				consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
				HelpersUI.SetContentToLabel(currencyAmount, rewardEquipment.Amount.ToString());
				return;
			}
			Helpers.GameObjectSetActive(equipmentContainer, value: true);
			EquipmentButton equipmentButton = Helpers.InstantiateWithComponent<EquipmentButton>(equipmentCardPrefab, equipmentContainer);
			if (equipmentButton != null)
			{
				EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
				bool flag = equipmentDefinition != null && equipmentDefinition.TraitsOverride != null && equipmentDefinition.TraitsOverride.Count > 0;
				equipmentButton.Setup(rewardEquipment, allowClick: false, !flag);
			}
		}
		else if (reward.Type == RewardType.RandomEquipment)
		{
			Helpers.GameObjectSetActive(equipmentContainer, value: true);
			RewardRandomEquipment rewardRandomEquipment = reward as RewardRandomEquipment;
			int levelOut = 0;
			EquipmentDefinition randomEquipmentDefinition = rewardRandomEquipment.GetRandomEquipmentDefinition(GameManager.Instance.modelManager, new ModelRandom(GameManager.Instance.playerModel.GuildShopModel.RandomSeed + itemReference.ID), out levelOut);
			EquipmentButton equipmentButton2 = Helpers.InstantiateWithComponent<EquipmentButton>(equipmentCardPrefab, equipmentContainer);
			if (equipmentButton2 != null)
			{
				equipmentButton2.Setup(randomEquipmentDefinition, rewardRandomEquipment.RarityLevel, levelOut);
			}
		}
	}
}
