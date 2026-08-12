using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EquipmentTokenSelectionBox : MonoBehaviour
{
	[SerializeField]
	private GameObject equipmentItemPrefab;

	[SerializeField]
	private GameObject containerGrid;

	[SerializeField]
	private GameObject emptySlotPrefab;

	private List<GameObject> items = new List<GameObject>();

	private static int totalColumns = 3;

	private SurvivorModel selectedSurvivor;

	public int NumberItems => items.Count;

	public void SetItemsForSurvivorClass(SurvivorClass survivorClass, bool armorsOnly)
	{
		ClearItems();
		Dictionary<string, List<EquipTokenItemModel>> equipTokenItems = GameManager.Instance.playerModel.EquipTokenContainer.GetEquipTokenItems(survivorClass);
		if (!armorsOnly)
		{
			foreach (EquipTokenItemModel item in equipTokenItems["WeaponItemListKey"])
			{
				AddItem(item);
			}
		}
		else
		{
			foreach (EquipTokenItemModel item2 in equipTokenItems["ArmorItemListKey"])
			{
				AddItem(item2);
			}
		}
		FillEmptySlots();
		if (containerGrid != null)
		{
			containerGrid.GetComponent<UIGrid>().Reposition();
		}
	}

	private void FillEmptySlots()
	{
		if (emptySlotPrefab != null && containerGrid != null)
		{
			int num = (items.Count - 1) % totalColumns;
			int num2 = totalColumns - 1 - num;
			for (int i = 0; i < num2; i++)
			{
				GameObject gameObject = Helpers.InstantiateToParent(emptySlotPrefab, containerGrid);
				gameObject.SetLayerRecursively(containerGrid.layer);
				items.Add(gameObject);
			}
		}
	}

	private void AddItem(EquipTokenItemModel equipmentItemModel)
	{
		if (containerGrid != null)
		{
			GameObject gameObject = Helpers.InstantiateToParent(equipmentItemPrefab, containerGrid);
			gameObject.SetLayerRecursively(containerGrid.layer);
			gameObject.GetComponent<EquipmentTokenButton>().Setup(equipmentItemModel);
			items.Add(gameObject);
		}
	}

	public void ClearItems()
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i] != null && items[i].gameObject != null)
			{
				Helpers.DestroyOrCache(items[i]);
			}
		}
		items.Clear();
	}
}
