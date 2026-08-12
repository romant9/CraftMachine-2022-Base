using TWDModel;
using UnityEngine;

public class EquipmentCardSmall : UIListCard<EquipmentItemModel>
{
	[SerializeField]
	[Tooltip("Container for when the card is not empty.")]
	private GameObject notEmptyContainer;

	[SerializeField]
	[Tooltip("Container for when the card is empty.")]
	private GameObject emptyCardContainer;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel statLabel;

	private TutorialArrowParent tutorialArrow;

	public EquipmentCategory EquipmentCategory { get; set; }

	private void Awake()
	{
		tutorialArrow = GetComponentInChildren<TutorialArrowParent>();
	}

	public override void UpdateUI()
	{
		if (base.Item == null || base.Item.Definition.Type == EquipmentType.Fists)
		{
			notEmptyContainer.SetActive(value: false);
			emptyCardContainer.SetActive(value: true);
		}
		else
		{
			notEmptyContainer.SetActive(value: true);
			emptyCardContainer.SetActive(value: false);
			EquipmentResourceEntry equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(base.Item);
			if (equipmentResourceEntry != null && !string.IsNullOrEmpty(equipmentResourceEntry.IconSprite))
			{
				icon.spriteName = equipmentResourceEntry.IconSprite;
			}
			statLabel.text = HelpersString.FormatNumberWithSign(base.Item.Damage);
			EquipmentCategory = base.Item.Definition.Category;
		}
		if (tutorialArrow != null)
		{
			tutorialArrow.Id = EquipmentCategory.ToString();
		}
	}

	public void OnClick()
	{
		UIEvent.Send("OnEquipmentCategoryClicked", EquipmentCategory);
		EventManager.NotifyClick(EquipmentCategory.ToString());
	}
}
