using TWDModel;

public class SurvivorViewPopupEquipment : ListPanel
{
	private SurvivorModel survivorModel;

	private void Start()
	{
		CreateWeaponTypesPanel();
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewSurvivorSelected")
		{
			survivorModel = parameter as SurvivorModel;
			UpdateSlots();
		}
		else if (type == "OnNewEquipmentEquiped")
		{
			UpdateSlots();
		}
	}

	private void CreateWeaponTypesPanel()
	{
		CreateSlots(6);
		UpdateSlots();
	}

	private void UpdateSlots()
	{
		for (int i = 0; i < base.NumberSlots; i++)
		{
			EquipmentItemModel item = null;
			if (survivorModel != null)
			{
				item = survivorModel.GetEquipmentOfCategory((EquipmentCategory)i);
			}
			EquipmentCardSmall component = GetSlotAt(i).GetComponent<EquipmentCardSmall>();
			component.EquipmentCategory = (EquipmentCategory)i;
			component.Item = item;
			component.UpdateUI();
		}
	}
}
