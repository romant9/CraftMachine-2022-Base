using TWDModel;
using UnityEngine;

public class BundlePopupCard : MonoBehaviour
{
	[SerializeField]
	private GameObject[] starsList;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite outfitIcon;

	[SerializeField]
	private UISprite rarityBg;

	[SerializeField]
	private UITexture iconTexture;

	[SerializeField]
	private UITexture iconArmorTexture;

	[SerializeField]
	private GameObject offerLabelContainer;

	[SerializeField]
	private UILabel offerLabel;

	[SerializeField]
	private GameObject exceedLimitContainer;

	[SerializeField]
	private UILabel exceedLimitLabel;

	[SerializeField]
	private UIAtlas monochromeAtlas;

	[SerializeField]
	private UIAtlas shopAtlas;

	[SerializeField]
	private UITexture BackgroundTexture;

	private string soundEventName;

	public static int IntroGroup = 10;

	public static int OutroGroup = 11;

	private string overrideRewardImageURL = "";

	private IReward targetReward;

	public void Setup(IReward reward, string extraLocalizationEntry, string overrideImageURL)
	{
		targetReward = reward;
		soundEventName = "global/bundle_card_generic";
		overrideRewardImageURL = overrideImageURL;
		if (outfitIcon != null)
		{
			outfitIcon.gameObject.SetActive(reward.Type == RewardType.Outfit);
		}
		if (iconTexture != null)
		{
			iconTexture.gameObject.SetActive(reward.Type == RewardType.Equipment);
		}
		if (iconArmorTexture != null)
		{
			iconArmorTexture.gameObject.SetActive(reward.Type == RewardType.Equipment);
		}
		if (starsList != null)
		{
			for (int i = 0; i < starsList.Length; i++)
			{
				if (starsList[i] != null)
				{
					starsList[i].SetActive(value: false);
				}
			}
		}
		if (offerLabelContainer != null)
		{
			offerLabelContainer.SetActive(!string.IsNullOrEmpty(extraLocalizationEntry));
			if (offerLabel != null && !string.IsNullOrEmpty(extraLocalizationEntry))
			{
				offerLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(extraLocalizationEntry);
			}
		}
		if (rarityBg != null)
		{
			rarityBg.gameObject.SetActive(value: false);
		}
		if (icon != null)
		{
			icon.gameObject.SetActive(value: false);
		}
		if (exceedLimitContainer != null)
		{
			exceedLimitContainer.gameObject.SetActive(value: false);
		}
		if (reward.Type == RewardType.Currency)
		{
			RewardCurrency rewardCurrency = reward as RewardCurrency;
			if (label != null)
			{
				if (rewardCurrency.Amount == -1)
				{
					label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardCurrencyTitle." + rewardCurrency.CurrencyType.ToString() + ".Full");
				}
				else
				{
					label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardCurrencyTitle." + rewardCurrency.CurrencyType.ToString() + "{Parameter}", rewardCurrency.Amount);
				}
			}
			if (icon != null)
			{
				icon.atlas = monochromeAtlas;
				icon.gameObject.SetActive(value: true);
				icon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			}
			if (rewardCurrency.Amount > 0 && GameManager.Instance.playerModel.GetCapacity(rewardCurrency.CurrencyType) < PlayerModel.UnlimitedCapacityAmount && exceedLimitContainer != null && exceedLimitLabel != null)
			{
				exceedLimitContainer.gameObject.SetActive(value: true);
				exceedLimitLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.ExceedLimitWarning");
			}
		}
		else if (reward.Type == RewardType.Equipment)
		{
			if (reward is RewardEquipment rewardEquipment)
			{
				setRarityStars(rewardEquipment.RarityLevel);
				if (rarityBg != null)
				{
					rarityBg.gameObject.SetActive(value: true);
					string equipmentBackgroundRaritySprite = GameManager.Instance.GetEquipmentBackgroundRaritySprite(rewardEquipment.RarityLevel);
					rarityBg.spriteName = equipmentBackgroundRaritySprite;
				}
				EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
				if (iconArmorTexture != null && equipmentDefinition != null && equipmentDefinition.Category == EquipmentCategory.Armor)
				{
					iconArmorTexture.gameObject.SetActive(value: true);
					iconTexture.gameObject.SetActive(value: false);
					iconArmorTexture.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(rewardEquipment.EquipmentId);
				}
				else if (iconTexture != null)
				{
					iconArmorTexture.gameObject.SetActive(value: false);
					iconTexture.gameObject.SetActive(value: true);
					iconTexture.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(rewardEquipment.EquipmentId);
				}
				if (label != null)
				{
					string equipmentName = HelpersLocalization.GetEquipmentName(rewardEquipment.EquipmentId);
					label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardEquipmentTitle{Parameter}", equipmentName);
				}
			}
		}
		else if (reward.Type == RewardType.RandomEquipment)
		{
			if (reward is RewardRandomEquipment rewardRandomEquipment)
			{
				setRarityStars(rewardRandomEquipment.RarityLevel);
				if (rarityBg != null)
				{
					rarityBg.gameObject.SetActive(value: true);
					string equipmentBackgroundRaritySprite2 = GameManager.Instance.GetEquipmentBackgroundRaritySprite(rewardRandomEquipment.RarityLevel);
					rarityBg.spriteName = equipmentBackgroundRaritySprite2;
				}
				if (icon != null)
				{
					icon.gameObject.SetActive(value: true);
					icon.spriteName = "Ui_Icon_BundleWeapon_" + rewardRandomEquipment.SurvivorClass;
				}
				if (label != null)
				{
					if (rewardRandomEquipment.SurvivorClass != SurvivorClass.None)
					{
						label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardRandomEquipmentTitle." + HelpersUI.GetRarityName(rewardRandomEquipment.RarityLevel) + "{SurvivorClass}", HelpersLocalization.GetSurvivorClassName(rewardRandomEquipment.SurvivorClass));
					}
					else
					{
						label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardRandomEquipmentTitle." + HelpersUI.GetRarityName(rewardRandomEquipment.RarityLevel));
					}
				}
			}
		}
		else if (reward.Type == RewardType.Outfit)
		{
			RewardOutfit rewardOutfit = reward as RewardOutfit;
			OutfitDefinition outfitDefinition = GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]);
			bool flag = !string.IsNullOrEmpty(outfitDefinition.BundleSprite);
			if (icon != null)
			{
				icon.gameObject.SetActive(!flag);
			}
			if (outfitDefinition != null)
			{
				if (outfitIcon != null)
				{
					outfitIcon.gameObject.SetActive(flag);
					if (flag)
					{
						outfitIcon.spriteName = outfitDefinition.BundleSprite;
					}
				}
				if (label != null)
				{
					string text = LocalizationManager.GetText(outfitDefinition.TitleLocalizationKey);
					label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardOutfitTitle{Parameter}", text);
				}
			}
		}
		else if (reward.Type == RewardType.SurvivorSlot)
		{
			if (reward is RewardSurvivorSlot rewardSurvivorSlot)
			{
				if (label != null)
				{
					label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardSurvivorSlotTitle{Parameter}", rewardSurvivorSlot.Amount.ToString());
				}
				if (icon != null)
				{
					icon.gameObject.SetActive(value: true);
					icon.spriteName = "Ui_Icon_Survivor_Empty";
				}
			}
		}
		else if (reward.Type == RewardType.Loot)
		{
			if (reward is RewardLootEntry rewardLootEntry && label != null)
			{
				label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardLootTitle." + rewardLootEntry.DropType);
			}
		}
		else if (reward.Type == RewardType.SurvivorClass)
		{
			if (reward is RewardSurvivorClass rewardSurvivorClass && label != null)
			{
				string survivorClassName = HelpersLocalization.GetSurvivorClassName(rewardSurvivorClass.SurvivorClass);
				label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardSurvivorClassTitle{Parameter}", survivorClassName);
			}
		}
		else if (reward.Type == RewardType.UnlockBuilding)
		{
			if (reward is RewardUnlockBuilding rewardUnlockBuilding && label != null)
			{
				string text2 = LocalizationManager.GetText("Building.Name." + rewardUnlockBuilding.BuildingTypeName);
				label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardUnlockBuildingTitle{Parameter}", text2);
			}
		}
		else if (reward.Type == RewardType.TimedBonus)
		{
			if (reward is RewardTimedBonus rewardTimedBonus)
			{
				if (label != null)
				{
					label.text = HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration);
				}
				if (icon != null)
				{
					icon.atlas = shopAtlas;
					icon.gameObject.SetActive(value: true);
					icon.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
				}
			}
		}
		else if (label != null)
		{
			label.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardTitle." + reward.Type);
		}
		base.gameObject.SetActive(value: false);
	}

	public void OnCardClick()
	{
		if (targetReward != null && targetReward.Type == RewardType.Equipment)
		{
			RewardEquipment reward = targetReward as RewardEquipment;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			obj.ShowNextLevel = false;
			obj.OpenForBundleReward(reward);
			CampHUD.Get().PauseCurrencyMeters = false;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
		}
	}

	public void StartIntro()
	{
		base.gameObject.SetActive(value: true);
		TweenManager.PlayTweenGroup(base.gameObject, IntroGroup);
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(soundEventName);
		}
		if (!(BackgroundTexture != null))
		{
			return;
		}
		BackgroundTexture.gameObject.SetActive(!string.IsNullOrEmpty(overrideRewardImageURL));
		if (!string.IsNullOrEmpty(overrideRewardImageURL))
		{
			LoadImageFromUrl component = GetComponent<LoadImageFromUrl>();
			if (component != null)
			{
				component.LoadImage(overrideRewardImageURL, BackgroundTexture, 1024);
			}
		}
	}

	public void StartOutro()
	{
		TweenManager.PlayTweenGroup(base.gameObject, OutroGroup);
	}

	private void setRarityStars(int rarityIndex)
	{
		if (starsList == null || starsList.Length == 0 || starsList.Length <= rarityIndex)
		{
			return;
		}
		for (int i = 0; i < starsList.Length; i++)
		{
			if (starsList[i] != null)
			{
				starsList[i].SetActive(i == rarityIndex);
			}
		}
	}
}
