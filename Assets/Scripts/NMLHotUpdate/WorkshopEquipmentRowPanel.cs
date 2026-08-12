using UnityEngine;

public class WorkshopEquipmentRowPanel : UIListCard<WorkshopEquipmentRow>
{
	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel EquipmentTypeName;

	[SerializeField]
	[Tooltip("Add some pixels to the background height.")]
	private int bgPadding;

	[SerializeField]
	private UIGrid weaponsGrid;

	[SerializeField]
	private UISprite background;

	private EquipmentSelectionBox equipmentSelection;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		equipmentSelection = GetComponent<EquipmentSelectionBox>();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void OnPoolReturn()
	{
		if (equipmentSelection != null)
		{
			equipmentSelection.ClearItems();
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewEquipmentSelected")
		{
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton.GetEquipment() != null)
			{
				Helpers.OpenEquipmentUpgradePopup(equipmentButton.GetEquipment());
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
			}
		}
		else if (type == "OnPopUpClose" && parameter is WorkshopPopup && equipmentSelection != null)
		{
			equipmentSelection.ClearItems();
		}
	}

	public override void UpdateUI()
	{
		if (base.Item != null)
		{
			if (classIcon != null)
			{
				HelpersGfx.UpdateSpriteAndKeepScale(classIcon, HelpersGfx.GetSurvivorClassIconName(base.Item.SurvivorClass));
			}
			if (EquipmentTypeName != null)
			{
				string text = (base.Item.IsArmor ? LocalizationManager.GetText("Popup.Workshop.Armors") : LocalizationManager.GetText("Popup.Workshop.Weapons"));
				EquipmentTypeName.text = text;
			}
			GetComponent<EquipmentSelectionBox>().SetItemsForSurvivorClass(base.Item.SurvivorClass, base.Item.IsArmor);
			UpdateSize();
		}
	}

	private void OnLanguageChanged()
	{
		UpdateUI();
	}

	public void UpdateSize()
	{
		int numberItems = GetComponent<EquipmentSelectionBox>().NumberItems;
		if (numberItems > 0)
		{
			float num = Mathf.Ceil((float)numberItems / (float)weaponsGrid.maxPerLine) * weaponsGrid.cellHeight + (float)bgPadding;
			BoxCollider component = GetComponent<BoxCollider>();
			Vector3 size = component.size;
			size.y = num;
			component.size = size;
			background.height = (int)num;
		}
	}
}
