using System.Collections.Generic;
using UnityEngine;

public class EquipmentSelectionFilter : MonoBehaviour
{
	[SerializeField]
	private List<EquipmentSelectionFilterButton> filterButtons;

	private List<EquipmentAvailability> selectedAvailabilities = new List<EquipmentAvailability>();

	public void OnEquipmentFilterClicked(EquipmentSelectionFilterButton filterButton)
	{
		if (filterButton != null)
		{
			if (filterButton.EquipmentAvailability == EquipmentAvailability.All)
			{
				selectedAvailabilities.Clear();
				selectedAvailabilities.Add(EquipmentAvailability.All);
			}
			else if (selectedAvailabilities.Contains(filterButton.EquipmentAvailability))
			{
				selectedAvailabilities.Remove(filterButton.EquipmentAvailability);
			}
			else
			{
				selectedAvailabilities.Add(filterButton.EquipmentAvailability);
			}
			UIEvent.Send("OnClickEquipmentAvailabilityFilter", selectedAvailabilities);
			EventManager.NotifyClick("Equipment_Availability");
		}
	}

	public void SetAvailabilityFilter(List<EquipmentAvailability> availabilities)
	{
		selectedAvailabilities = availabilities;
		foreach (EquipmentSelectionFilterButton filterButton in filterButtons)
		{
			if (availabilities.Contains(filterButton.EquipmentAvailability))
			{
				filterButton.toggle.SetToggled(toggled: true);
			}
			else
			{
				filterButton.toggle.SetToggled(toggled: false);
			}
		}
	}
}
