using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EquipmentSelectionContainerView : MonoBehaviour
{
	private SurvivorCard selectedCard;

	private EquipmentButton selectedEquipmentButton;

	[SerializeField]
	private GameObject equipmentSelectionBoxPrefab;

	[SerializeField]
	private GameObject equipmentSelectionBox;
	private EquipmentItemModel currentComparingEquipment;

	private static Vector3 selectionBoxOffset = new Vector3(40f, 0f, 0f);

	private List<EquipmentAvailability> availabilityFilters = new List<EquipmentAvailability>();

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		DebugTWD.Log("EquipmentSelectionContainerView enable. " + this.name + ", " + equipmentSelectionBoxPrefab?.name);
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void OpenForSurvivorCard(SurvivorCard card, EquipmentButton equipmentButton)
	{
		selectedCard = card;
		selectedEquipmentButton = equipmentButton;
		if (equipmentButton.GetEquipment() != null)
		{
			DebugTWD.Log("OpenForSurvivorCard: "+ equipmentSelectionBoxPrefab?.name + " for " + equipmentButton.GetOwningSurvivor());

			currentComparingEquipment = equipmentButton.GetEquipment();
			CreateEquipmentSelectionBox(currentComparingEquipment, equipmentButton.GetOwningSurvivor(), availabilityFilters);
			equipmentSelectionBox.transform.localPosition = new Vector3(0f, 10000f);
			StartCoroutine(DelayAlignment());
			HUDElement component = base.gameObject.GetComponent<HUDElement>();
			if (component != null)
			{
				component.UpdateUI();
			}
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnSurvivorInfoClosed")
		{
			DestroyEquipmentSelectionBox();
		}
		if (selectedEquipmentButton == null)
		{
			return;
		}
		switch (type)
		{
		case "OnNewEquipmentSelected":
		{
			EquipmentButton selectedButton = parameter as EquipmentButton;
			List<EquipmentAvailability> availabilities = availabilityFilters;
			HandleEquipmentSelectionBox(selectedButton, availabilities);
			break;
		}
		case "OnNewSurvivorSelected":
			DestroyEquipmentSelectionBox();
			break;
		case "EquipmentSelectionClosed":
			currentComparingEquipment = null;
			DestroyEquipmentSelectionBox();
			if (selectedCard != null)
			{
				selectedCard.UpdateUI();
			}
			break;
		case "OnClickEquipmentAvailabilityFilter":
			if (parameter != null)
			{
				HandleEquipmentSelectionBox(availabilities: availabilityFilters = parameter as List<EquipmentAvailability>, selectedButton: selectedEquipmentButton);
			}
			break;
		}
	}

	private void HandleEquipmentSelectionBox(EquipmentButton selectedButton, List<EquipmentAvailability> availabilities, bool isFilter = false)
	{
		if (OfflineManager.IsLoadDataManager && !isFilter)
		{
			SurvivorModel model = selectedButton.GetOwningSurvivor();
			EquipmentItemModel model2 = selectedButton.GetEquipment();
			TWDModelResult result = model.Equip(model2);

			if (result == TWDModelResult.OK)
			{
				DebugTWD.Log("Equip is OK");
				model.ConfigureBaseAttributes();
				UIEvent.Send("OnNewEquipmentEquiped", selectedButton);
				DestroyEquipmentSelectionBox();
				return;
			}
		}
		else
		{
			if (Helpers.ExecuteCommand(new EquipItemCommand(selectedButton.GetOwningSurvivor(), selectedButton.GetEquipment())) == TWDModelResult.OK)
			{
				UIEvent.Send("OnNewEquipmentEquiped", selectedButton);
			}
		}
		if (selectedCard != null)
		{
			selectedCard.UpdateUI();
		}
		DestroyEquipmentSelectionBox();
		selectedEquipmentButton.RefreshEquipmentToCompare(currentComparingEquipment);
		currentComparingEquipment = selectedButton.GetEquipment();
		CreateEquipmentSelectionBox(selectedButton.GetEquipment(), selectedButton.GetOwningSurvivor(), availabilities);
		equipmentSelectionBox.GetComponent<EquipmentSelectionBox>().AlignToElement(selectedEquipmentButton.gameObject, selectionBoxOffset);
		HUDElement component = base.gameObject.GetComponent<HUDElement>();
		if (component != null)
		{
			component.UpdateUI();
		}
	}

	private IEnumerator DelayAlignment()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		if (equipmentSelectionBox != null)
		{
			equipmentSelectionBox.GetComponent<EquipmentSelectionBox>().AlignToElement(selectedEquipmentButton.gameObject, selectionBoxOffset);
		}
	}

	private void CreateEquipmentSelectionBox(EquipmentItemModel targetEquipment, SurvivorModel owningSurvivor, List<EquipmentAvailability> availabilities)
	{
		DestroyEquipmentSelectionBox();
		equipmentSelectionBox = Helpers.InstantiateToParent(equipmentSelectionBoxPrefab, base.gameObject);
		equipmentSelectionBox.GetComponent<EquipmentSelectionBox>().SetItems(targetEquipment, currentComparingEquipment, owningSurvivor, availabilities);
		equipmentSelectionBox.GetComponent<EquipmentSelectionFilter>().SetAvailabilityFilter(availabilities);
	}

	private void DestroyEquipmentSelectionBox()
	{
		if (equipmentSelectionBox != null)
		{
			Object.Destroy(equipmentSelectionBox);
		}
	}
}
