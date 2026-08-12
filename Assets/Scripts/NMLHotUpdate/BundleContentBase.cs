using TWDModel;
using UnityEngine;

public class BundleContentBase : ShopCardBase<IReward>
{
	[SerializeField]
	private UILabel itemNameLabel;

	[SerializeField]
	private UISprite itemSprite;

	[SerializeField]
	private UIAtlas monochromeAtlas;

	[SerializeField]
	private UIAtlas shopAtlas;

	[SerializeField]
	private UIAtlas uiCampAtlas;

	[SerializeField]
	private GameObject equipmentContainer;

	[SerializeField]
	private GameObject equipmentTokenContainer;

	[SerializeField]
	private GameObject equipmentCardPrefab;

	[SerializeField]
	private GameObject equipmentTokenCardPrefab;

	[SerializeField]
	private GameObject equipmentRandomCardPrefab;

	[SerializeField]
	private UITexture consumableTexture;

	private EquipmentButton equipmentButton;

	private EquipmentTokenButton equipmentTokenButton;

	private EquipmentRandomButton equipmentRandomButton;

	[SerializeField]
	private GameObject baContent;

	[SerializeField]
	private UITexture avatarIcon;

	[SerializeField]
	private UITexture borderIcon;

	[SerializeField]
	private GameObject baSpriteContent;

	[SerializeField]
	private UISprite borderSprite;

	[SerializeField]
	private UISprite avatarSprite;

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

	public override void SetData(IReward data)
	{
		base.SetData(data);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		IReward data = GetData();
		if (data == null)
		{
			return;
		}
		if (baContent != null)
		{
			baContent.SetActive(value: false);
		}
		if (baSpriteContent != null)
		{
			baSpriteContent.SetActive(value: false);
		}
		Helpers.GameObjectSetActive(skillParent, value: false);
		Helpers.GameObjectSetActive(skillCurrencyParent, value: false);
		HelpersUI.SetContentToLabel(itemNameLabel, HelpersLocalization.GetBundleTitleForIReward(data));
		if (data.Type == RewardType.Equipment && data is RewardEquipment rewardEquipment)
		{
			Helpers.DestroyOrCache(equipmentRandomButton);
			Helpers.DestroyOrCache(equipmentTokenButton);
			if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
			{
				Helpers.DestroyOrCache(equipmentButton);
				consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			}
			else
			{
				if (equipmentButton == null)
				{
					equipmentButton = Helpers.InstantiateWithComponent<EquipmentButton>(equipmentCardPrefab, equipmentContainer);
				}
				if (equipmentButton != null)
				{
					equipmentButton.Setup(rewardEquipment);
				}
				Helpers.GameObjectSetActive(consumableTexture, value: false);
			}
			Helpers.GameObjectSetActive(itemSprite, value: false);
		}
		else if (data.Type == RewardType.EquipToken && data is RewardEquipToken upForCampaign)
		{
			Helpers.DestroyOrCache(equipmentRandomButton);
			Helpers.DestroyOrCache(equipmentButton);
			Helpers.GameObjectSetActive(consumableTexture, value: false);
			Helpers.GameObjectSetActive(itemSprite, value: false);
			if (equipmentTokenButton == null)
			{
				equipmentTokenButton = Helpers.InstantiateWithComponent<EquipmentTokenButton>(equipmentTokenCardPrefab, equipmentTokenContainer);
			}
			if (equipmentTokenButton != null)
			{
				equipmentTokenButton.SetUpForCampaign(upForCampaign);
			}
		}
		else if (data.Type == RewardType.RandomEquipment && data is RewardRandomEquipment reward)
		{
			Helpers.DestroyOrCache(equipmentButton);
			Helpers.DestroyOrCache(equipmentTokenButton);
			if (equipmentRandomButton == null)
			{
				equipmentRandomButton = Helpers.InstantiateWithComponent<EquipmentRandomButton>(equipmentRandomCardPrefab, equipmentContainer);
			}
			if (equipmentRandomButton != null)
			{
				equipmentRandomButton.Setup(reward);
			}
			Helpers.GameObjectSetActive(itemSprite, value: false);
			Helpers.GameObjectSetActive(consumableTexture, value: false);
		}
		else if (data.Type == RewardType.Avatars && data is RewardAvatars rewardAvatars)
		{
			Helpers.DestroyOrCache(equipmentButton);
			Helpers.DestroyOrCache(equipmentRandomButton);
			Helpers.DestroyOrCache(equipmentTokenButton);
			Helpers.GameObjectSetActive(itemSprite, value: false);
			baContent.SetActive(value: false);
			baSpriteContent.SetActive(value: false);
			avatarSprite.gameObject.SetActive(value: false);
			borderSprite.gameObject.SetActive(value: false);
			avatarIcon.gameObject.SetActive(value: false);
			borderIcon.gameObject.SetActive(value: false);
			if (rewardAvatars.Avatar >= 0 && avatarIcon != null)
			{
				avatarIcon.gameObject.SetActive(value: true);
				AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(rewardAvatars.Avatar);
				if (avatarsDefinition.LocalImg != null)
				{
					avatarSprite.spriteName = avatarsDefinition.LocalImg;
					avatarSprite.gameObject.SetActive(value: true);
					baSpriteContent.SetActive(value: true);
				}
				else
				{
					LoadImageFromCdn.LoadImageToTarget(avatarIcon, avatarsDefinition?.Image);
					baContent.SetActive(value: true);
				}
			}
			else if (rewardAvatars.Border >= 0 && borderIcon != null)
			{
				BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(rewardAvatars.Border);
				if (bordersDefinition.LocalImg != null)
				{
					borderSprite.spriteName = bordersDefinition.LocalImg;
					borderSprite.gameObject.SetActive(value: true);
					baSpriteContent.SetActive(value: true);
				}
				else
				{
					LoadImageFromCdn.LoadImageToTarget(borderIcon, bordersDefinition?.Image);
					baContent.SetActive(value: true);
				}
			}
		}
		else if (data is RewardRemoldSkill rewardRemoldSkill)
		{
			Helpers.DestroyOrCache(equipmentButton);
			Helpers.DestroyOrCache(equipmentRandomButton);
			Helpers.DestroyOrCache(equipmentTokenButton);
			Helpers.GameObjectSetActive(itemSprite, value: false);
			baContent.SetActive(value: false);
			baSpriteContent.SetActive(value: false);
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
		else if (data is RewardCurrency { CurrencyType: var currencyType } rewardCurrency && currencyType.ToString().Contains("SkillToken"))
		{
			Helpers.DestroyOrCache(equipmentButton);
			Helpers.DestroyOrCache(equipmentRandomButton);
			Helpers.DestroyOrCache(equipmentTokenButton);
			Helpers.GameObjectSetActive(itemSprite, value: false);
			baContent.SetActive(value: false);
			baSpriteContent.SetActive(value: false);
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
			Helpers.DestroyOrCache(equipmentButton);
			Helpers.DestroyOrCache(equipmentRandomButton);
			Helpers.DestroyOrCache(equipmentTokenButton);
			Helpers.GameObjectSetActive(consumableTexture, value: false);
			string spriteName = "";
			UIAtlas iconNameForIReward = HelpersGfx.GetIconNameForIReward(data, out spriteName, monochromeAtlas, shopAtlas, uiCampAtlas);
			HelpersUI.SetSpriteAndAtlas(itemSprite, spriteName, iconNameForIReward);
		}
	}

	public override void Clear()
	{
		base.Clear();
		Helpers.DestroyOrCache(equipmentButton);
		equipmentButton = null;
		Helpers.DestroyOrCache(equipmentTokenButton);
		equipmentTokenButton = null;
		if (equipmentRandomButton != null)
		{
			equipmentRandomButton.Clear();
			Helpers.DestroyOrCache(equipmentRandomButton);
			equipmentRandomButton = null;
		}
	}

	protected override void OnClickedTooltipButton(UIButtonExtended button)
	{
		base.OnClickedTooltipButton(button);
		IReward data = GetData();
		if (data == null)
		{
			return;
		}
		if (equipmentButton != null)
		{
			equipmentButton.OnEquipmentButtonClicked();
		}
		else if (equipmentTokenButton != null)
		{
			equipmentTokenButton.OnEquipmentButtonClicked();
		}
		else if (data is RewardRemoldSkill rewardRemoldSkill)
		{
			SPRemoldTraitsSkillMergedPopup sPRemoldTraitsSkillMergedPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillMergedPopup) as SPRemoldTraitsSkillMergedPopup;
			if (sPRemoldTraitsSkillMergedPopup != null)
			{
				SPTraitsRemoldDefinitions minRemoldDefinitionForGroup = Helpers.GetMinRemoldDefinitionForGroup(rewardRemoldSkill.SpRemoldSkillType);
				sPRemoldTraitsSkillMergedPopup.Setup(minRemoldDefinitionForGroup.ID);
				sPRemoldTraitsSkillMergedPopup.Open();
			}
		}
		else
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(data));
		}
		if (equipmentRandomButton != null)
		{
			equipmentRandomButton.OnButtonClicked(null);
		}
	}
}
