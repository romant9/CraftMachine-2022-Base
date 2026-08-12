using TWDModel;
using UnityEngine;

public class BundleCard : MonoBehaviour
{
	private const string LABEL_NAME = "Label";

	private const string ICON_NAME = "Icon";

	private const string ICON_OUTFIT = "IconOutfit";

	private const string ICON_OUTFIT_GENERIC = "IconOutfitGeneric";

	private const string ICON_TEXTURE = "IconTexture";

	private const string RARITY_BG_NAME = "Rarity_Bg";

	private const string EQUIP_CARD_NAME = "Equipment_Card_Small";

	private const string RARITY_STARS_NAME = "Rarity_Stars";

	[SerializeField]
	private UISprite[] starsList;

	private UILabel label;

	private UISprite icon;

	private UISprite outfitIcon;

	private UISprite outfitIconGeneric;

	private UISprite bg;

	private UITexture iconTexture;

	private const string NAME = "BundleCard: ";

	public string Text { get; set; }

	public string IconSpriteName { get; set; }

	public ColorEntry RarityColor { get; set; }

	private void Start()
	{
		label = findComponentInChild<UILabel>("Label", base.gameObject);
		icon = findComponentInChild<UISprite>("Icon", base.gameObject, hideWarnings: true);
		bg = findComponentInChild<UISprite>("Rarity_Bg", base.gameObject, hideWarnings: true);
		update();
	}

	public void update()
	{
		if (label != null)
		{
			label.text = Text;
		}
		if (icon != null && IconSpriteName != null && IconSpriteName != "")
		{
			icon.spriteName = IconSpriteName;
		}
		if (bg != null && RarityColor != null)
		{
			bg.color = RarityColor.BackgroundColor;
		}
	}

	public void SetupSpecificEquipmentFromReward(RewardEquipment reward)
	{
		if (reward != null && !string.IsNullOrEmpty(reward.EquipmentId))
		{
			Text = HelpersLocalization.GetEquipmentName(reward.EquipmentId);
			EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(reward.EquipmentId);
			IconSpriteName = "Ui_Icon_BundleWeapon_" + equipmentDefinition.SurvivorClass;
			RarityColor = GameManager.Instance.GetRarityColorData(reward.RarityLevel);
			setRarityStars(reward.RarityLevel);
			iconTexture = findComponentInChild<UITexture>("IconTexture", base.gameObject, hideWarnings: true);
			if (iconTexture != null)
			{
				iconTexture.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(reward.EquipmentId);
			}
		}
	}

	public void SetupForOutfit(string outfitId)
	{
		OutfitDefinition outfitDefinition = GameManager.Instance.gameEconomyData.GetOutfitDefinition(outfitId);
		bool flag = !string.IsNullOrEmpty(outfitDefinition.BundleSprite);
		outfitIcon = findComponentInChild<UISprite>("IconOutfit", base.gameObject, hideWarnings: true);
		outfitIconGeneric = findComponentInChild<UISprite>("IconOutfitGeneric", base.gameObject, hideWarnings: true);
		if (outfitIconGeneric != null)
		{
			outfitIconGeneric.gameObject.SetActive(!flag);
		}
		if (outfitIcon != null)
		{
			outfitIcon.gameObject.SetActive(flag);
			if (flag)
			{
				outfitIcon.spriteName = outfitDefinition.BundleSprite;
			}
		}
		icon = findComponentInChild<UISprite>("Icon", base.gameObject, hideWarnings: true);
		if (icon != null)
		{
			icon.gameObject.SetActive(value: false);
		}
		setRarityStars(-1);
		RarityColor = GameManager.Instance.GetRarityColorData(3);
		if (string.IsNullOrEmpty(outfitId))
		{
			Text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.Outfit");
		}
		else
		{
			string text = outfitId;
			if (outfitDefinition != null)
			{
				text = LocalizationManager.GetText(outfitDefinition.TitleLocalizationKey);
			}
			Text = LocalizationManager.GetText("Bundle.Outfit.Description{Parameter}", text);
		}
		update();
	}

	public void setRarityStars(int rarity)
	{
		if (starsList == null || starsList.Length == 0)
		{
			return;
		}
		for (int i = 0; i < starsList.Length; i++)
		{
			if (starsList[i] != null && starsList[i].gameObject != null)
			{
				if (rarity >= i)
				{
					starsList[i].gameObject.SetActive(value: true);
				}
				else
				{
					starsList[i].gameObject.SetActive(value: false);
				}
			}
		}
	}

	private static T findComponentInChild<T>(string childName, GameObject parent, bool hideWarnings = false) where T : Component
	{
		Transform transform = null;
		T val = null;
		if (parent != null && childName != "")
		{
			transform = parent.transform.Find(childName);
			if (transform != null && transform.gameObject != null)
			{
				val = transform.gameObject.GetComponent<T>();
				if (val != null)
				{
					return val;
				}
				val = null;
				if (!hideWarnings)
				{
					Debug.LogWarning("BundleCard: Can not find component by name: " + val.name);
				}
			}
			else if (!hideWarnings)
			{
				Debug.LogWarning("BundleCard: Can not find child by name: " + childName + " in " + parent.name);
			}
		}
		else if (!hideWarnings)
		{
			Debug.LogWarning("BundleCard: one of parameters NULL!");
		}
		return null;
	}
}
