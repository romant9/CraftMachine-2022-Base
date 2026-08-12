using TWDModel;
using UnityEngine;

public class EquipmentCard : UIListCard<EquipmentItemModel>
{
	[SerializeField]
	private UISprite equimentIcon;

	[SerializeField]
	private UILabel equimentNameLabel;

	[SerializeField]
	private UISprite propertyValueIcon;

	[SerializeField]
	private UISprite propertyValueBackground;

	[SerializeField]
	private UILabel propertyValueLabel;

	[SerializeField]
	private GameObject equipedGameObject;

	[SerializeField]
	private GameObject equipmentOwnerPictureContainer;

	[SerializeField]
	private UITexture equipmentOwnerPicture;

	[SerializeField]
	private TraitsContainer traitsContainer;

	private EquipmentComparator equipmentComparator;

	private TutorialArrowParent tutorialArrow;

	public EquipmentItemModel EquipmentItemEquiped { get; set; }

	public bool ShowItemComparison { get; set; }

	public bool ShowOwner { get; set; }

	public bool ShowEquiped { get; set; }

	public bool ShowTraits { get; set; }

	private void Awake()
	{
		ShowOwner = true;
		ShowEquiped = true;
		ShowTraits = true;
		tutorialArrow = GetComponentInChildren<TutorialArrowParent>();
	}

	public override void UpdateUI()
	{
		if (equipedGameObject != null)
		{
			if (ShowEquiped)
			{
				equipedGameObject.SetActive(base.Item == EquipmentItemEquiped);
			}
			else
			{
				equimentIcon.enabled = false;
				equipedGameObject.SetActive(value: false);
			}
		}
		traitsContainer.gameObject.SetActive(ShowTraits);
		if (base.Item == null || base.Item.Definition.Type == EquipmentType.Fists)
		{
			return;
		}
		equimentNameLabel.text = HelpersLocalization.GetEquipmentName(base.Item);
		EquipmentResourceEntry equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(base.Item);
		if (equipmentResourceEntry != null && !string.IsNullOrEmpty(equipmentResourceEntry.IconSprite))
		{
			equimentIcon.spriteName = equipmentResourceEntry.IconSprite;
		}
		if (propertyValueIcon != null)
		{
			propertyValueIcon.spriteName = HelpersGfx.GetEquipmentPropertyIconName(base.Item);
		}
		if (ShowItemComparison)
		{
			if (equipmentComparator == null)
			{
				equipmentComparator = new EquipmentComparator();
			}
			equipmentComparator.SetItems(EquipmentItemEquiped, base.Item);
			propertyValueBackground.color = equipmentComparator.DamageColor;
			propertyValueLabel.text = HelpersString.FormatNumberWithSign(equipmentComparator.DamageIncrease);
		}
		else
		{
			propertyValueBackground.color = Color.white;
			propertyValueLabel.text = base.Item.Damage.ToString();
		}
		if (equipmentOwnerPicture != null)
		{
			if (ShowOwner && base.Item.Owner != null)
			{
				equipmentOwnerPictureContainer.SetActive(value: true);
				equipmentOwnerPicture.mainTexture = PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorModel(base.Item.Owner));
			}
			else
			{
				equipmentOwnerPictureContainer.SetActive(value: false);
			}
		}
		traitsContainer.SetEquipmentTraits(base.Item);
		if (tutorialArrow != null)
		{
			tutorialArrow.Id = base.Item.Definition.ID;
		}
	}

	public override int GetSortValue()
	{
		if (ShowItemComparison && equipmentComparator != null)
		{
			return equipmentComparator.DamageIncrease;
		}
		return 0;
	}

	public void OnCardStateChanged()
	{
		if (UIToggle.current.value)
		{
			UIEvent.Send("OnNewEquipmentCardSelected", this);
			EventManager.NotifyClick(base.Item.Definition.ID);
		}
	}

	public void RemoveInteraction()
	{
		GetComponent<UIToggle>().enabled = false;
		GetComponent<UIButton>().enabled = false;
		GetComponent<UIButtonScale>().enabled = false;
	}
}
