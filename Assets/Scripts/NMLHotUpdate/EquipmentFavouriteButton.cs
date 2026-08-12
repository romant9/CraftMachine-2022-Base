using TWDModel;
using UnityEngine;

public class EquipmentFavouriteButton : MonoBehaviour
{
	[SerializeField]
	private EquipmentUpgradePopup equipmentUpgradePopup;

	[SerializeField]
	private GameObject On;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEventHandler;
		UpdateVisibility();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEventHandler;
	}

	private void OnUIEventHandler(string type, object parameter)
	{
		if (type.Contains("Equipment"))
		{
			UpdateVisibility();
		}
	}

	public void OnFavouriteButtonClicked()
	{
		if (equipmentUpgradePopup.equipmentItemModel != null && Helpers.ExecuteCommand(new ToggleFavouriteForEquipment(equipmentUpgradePopup.equipmentItemModel)) == TWDModelResult.OK)
		{
			UpdateVisibility();
			equipmentUpgradePopup.UpdateUI();
			UIEvent.Send("OnEquipmentUpdated", equipmentUpgradePopup.equipmentItemModel);
		}
	}

	public void UpdateVisibility()
	{
		if (equipmentUpgradePopup == null)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			On.SetActive(equipmentUpgradePopup.equipmentItemModel?.IsFavourite ?? false);
		}
	}
}
