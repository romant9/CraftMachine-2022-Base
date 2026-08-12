using UnityEngine;

public class WorkshopTokenEquipmentRowPanel : UIListCard<WorkshopEquipmentRow>
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

	private EquipmentTokenSelectionBox equipmentSelection;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		equipmentSelection = GetComponent<EquipmentTokenSelectionBox>();
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
		if (type == "OnPopUpClose" && parameter is WorkshopPopup && equipmentSelection != null)
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
			GetComponent<EquipmentTokenSelectionBox>().SetItemsForSurvivorClass(base.Item.SurvivorClass, base.Item.IsArmor);
			UpdateSize();
		}
	}

	public void UpdateSize()
	{
		int numberItems = GetComponent<EquipmentTokenSelectionBox>().NumberItems;
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
