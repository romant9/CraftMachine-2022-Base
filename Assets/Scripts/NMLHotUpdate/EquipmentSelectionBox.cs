using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class EquipmentSelectionBox : MonoBehaviour
{
	public EquipmentItemModel Equipment;

	[SerializeField]
	private GameObject equipmentItemPrefab;

	[SerializeField]
	private GameObject equipmentItemNotAvailablePrefab;

	[SerializeField]
	private GameObject equipmentItemNotEquippedPrefab;

	[SerializeField]
	private GameObject equipmentItemAvailablePrefab;

	[SerializeField]
	private GameObject equipmentItemNotEquippedUnableToEquipPrefab;

	[SerializeField]
	private GameObject containerGrid;

	[SerializeField]
	private GameObject trianglePointer;

	[SerializeField]
	private GameObject emptySlotPrefab;

	[SerializeField]
	private GameObject listContentParent;

	private List<GameObject> items = new List<GameObject>();

	private static int totalColumns = 3;

	private SurvivorModel selectedSurvivor;

	public int NumberItems => items.Count;

	public void SetItems(EquipmentItemModel currentEquipment, EquipmentItemModel equipmentToCompare, SurvivorModel selectedSurvivor, List<EquipmentAvailability> availabilities)
	{
		this.selectedSurvivor = selectedSurvivor;
		ClearItems();
		List<EquipmentType> equipmentsUsableByClass = selectedSurvivor.manager.GameEconomyData.GetEquipmentsUsableByClass(selectedSurvivor.SurvivorClass);
		ModelList<EquipmentItemModel> equipmentsOfType = selectedSurvivor.manager.Player.Equipment.GetEquipmentsOfType(currentEquipment.Definition.Category, equipmentsUsableByClass.ToArray());
		List<EquipmentItemModel> list = new List<EquipmentItemModel>();
		list = (((availabilities.Count != 1 || availabilities[0] != EquipmentAvailability.All) && availabilities.Count != 0) ? FilterEquipmentAvailability(equipmentsOfType.ToList(), availabilities, currentEquipment, equipmentToCompare, selectedSurvivor) : equipmentsOfType.ToList());
		foreach (EquipmentItemModel item in SortEquipmentList(list, equippedAndHigherLevelItemsFirst: false))
		{
			if (customCanEquipCheck(selectedSurvivor, item) && item != equipmentToCompare && item.CanBeManipulated())
			{
				AddItem(item, equipmentToCompare, selectedSurvivor);
			}
		}
		FillEmptySlots();
		StartCoroutine(ResetScrollPosition());
	}

	public EquipmentItemModel SetItems(EquipmentItemModel currentEquipment, PlayerModel player, SurvivorClass survivorClass)
	{
		ClearItems();
		Helpers.GameObjectSetActive(equipmentItemPrefab, value: false);
		Helpers.GameObjectSetActive(equipmentItemNotEquippedPrefab, value: false);
		player.manager.GameEconomyData.GetEquipmentsUsableByClass(survivorClass);
		List<EquipmentItemModel> allRemoldEquipments = player.Equipment.GetAllRemoldEquipments();
		List<EquipmentItemModel> list = SortEquipmentList(allRemoldEquipments, equippedAndHigherLevelItemsFirst: true);
		List<EquipmentButton> list2 = new List<EquipmentButton>();
		foreach (EquipmentItemModel item3 in list)
		{
			if (survivorClass == SurvivorClass.None)
			{
				EquipmentButton item = AddItem(item3, null);
				list2.Add(item);
			}
			else if (item3.Definition.CanBeEquippedBySurvivorClass(survivorClass) && item3.CanBeManipulated())
			{
				EquipmentButton item2 = AddItem(item3, null);
				list2.Add(item2);
			}
		}
		if (containerGrid != null)
		{
			StartCoroutine(ResetScrollPosition());
		}
		EquipmentItemModel equipmentItemModel = null;
		foreach (EquipmentButton item4 in list2)
		{
			EquipmentItemModel equipment = item4.GetEquipment();
			if (equipment == currentEquipment)
			{
				item4.OnSelectionHighlight(isEnable: true);
				equipmentItemModel = equipment;
			}
			else
			{
				item4.OnSelectionHighlight(isEnable: false);
			}
		}
		if (equipmentItemModel == null)
		{
			list2.FirstOrDefault()?.OnSelectionHighlight(isEnable: true);
			equipmentItemModel = list2.FirstOrDefault()?.GetEquipment();
		}
		return equipmentItemModel;
	}

	private IEnumerator ResetScrollPosition()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		containerGrid?.GetComponent<UIGrid>()?.Reposition();
		UIScrollView uIScrollView = listContentParent?.GetComponentInChildren<UIScrollView>();
		uIScrollView?.ResetPosition();
		if (uIScrollView?.verticalScrollBar != null)
		{
			uIScrollView.verticalScrollBar.value = 0f;
		}
	}

	public void SetItemsForSurvivorClass(SurvivorClass survivorClass, bool armorsOnly)
	{
		ClearItems();
		if (!armorsOnly)
		{
			List<EquipmentItemModel> list = GameManager.Instance.playerModel.Equipment.MeleeWeapons.ToList();
			foreach (EquipmentItemModel item in SortWeaponBagList(list))
			{
				if (item.Definition.CanBeEquippedBySurvivorClass(survivorClass) && item.CanBeManipulated())
				{
					AddItem(item, null, null);
				}
			}
			List<EquipmentItemModel> list2 = GameManager.Instance.playerModel.Equipment.RangeWeapons.ToList();
			foreach (EquipmentItemModel item2 in SortWeaponBagList(list2))
			{
				if (item2.Definition.CanBeEquippedBySurvivorClass(survivorClass) && item2.CanBeManipulated())
				{
					AddItem(item2, null, null);
				}
			}
		}
		else
		{
			List<EquipmentItemModel> list3 = GameManager.Instance.playerModel.Equipment.Armors.ToList();
			foreach (EquipmentItemModel item3 in SortWeaponBagList(list3))
			{
				if (item3.Definition.CanBeEquippedBySurvivorClass(survivorClass) && item3.CanBeManipulated())
				{
					AddItem(item3, null, null);
				}
			}
		}
		FillEmptySlots();
		if (containerGrid != null)
		{
			containerGrid.GetComponent<UIGrid>().Reposition();
			containerGrid.GetComponentInParent<UIScrollView>()?.ResetPosition();
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

	private EquipmentButton AddItem(EquipmentItemModel equipmentItemModel, SurvivorModel survivorModel)
	{
		if (containerGrid != null)
		{
			GameObject prefab = equipmentItemPrefab;
			if (equipmentItemModel.Owner == null)
			{
				prefab = equipmentItemNotEquippedPrefab;
			}
			GameObject gameObject = Helpers.InstantiateToParent(prefab, containerGrid);
			Helpers.GameObjectSetActive(gameObject, value: true);
			gameObject.SetLayerRecursively(containerGrid.layer);
			EquipmentButton component = gameObject.GetComponent<EquipmentButton>();
			component.Setup(equipmentItemModel, null, survivorModel, "NewRecommendEquipmentSelected", showOwnerAndUpgradeIndicator: true);
			items.Add(gameObject);
			return component;
		}
		return null;
	}

	private void AddItem(EquipmentItemModel equipmentItemModel, EquipmentItemModel equipmentToCompare, SurvivorModel survivorModel)
	{
		if (containerGrid != null)
		{
			GameObject prefab = equipmentItemPrefab;
			if (equipmentToCompare != null)
			{
				prefab = ((equipmentItemModel.GetOwnerForClient() == null) ? ((survivorModel.Level >= equipmentItemModel.StartingLevel) ? equipmentItemNotEquippedPrefab : equipmentItemNotEquippedUnableToEquipPrefab) : ((equipmentItemModel.GetOwnerForClient().Level >= equipmentToCompare.StartingLevel && survivorModel.Level >= equipmentItemModel.StartingLevel) ? equipmentItemAvailablePrefab : equipmentItemNotAvailablePrefab));
			}
			GameObject gameObject = Helpers.InstantiateToParent(prefab, containerGrid);
			gameObject.SetLayerRecursively(containerGrid.layer);
			gameObject.GetComponent<EquipmentButton>().Setup(equipmentItemModel, null, survivorModel, "OnNewEquipmentSelected", showOwnerAndUpgradeIndicator: true);
			items.Add(gameObject);
		}
	}

	private List<EquipmentItemModel> FilterEquipmentAvailability(List<EquipmentItemModel> equipmentItemList, IEnumerable<EquipmentAvailability> availabilities, EquipmentItemModel currentEquipment, EquipmentItemModel equipmentToCompare, SurvivorModel selectedSurvivor)
	{
		List<EquipmentItemModel> first = new List<EquipmentItemModel>();
		List<EquipmentItemModel> second = new List<EquipmentItemModel>();
		List<EquipmentItemModel> second2 = new List<EquipmentItemModel>();
		List<EquipmentItemModel> second3 = new List<EquipmentItemModel>();
		if (availabilities.Contains(EquipmentAvailability.Available))
		{
			second2 = equipmentItemList.FindAll((EquipmentItemModel x) => x.GetOwnerForClient() != null && x.GetOwnerForClient().Level >= currentEquipment.StartingLevel && selectedSurvivor.Level >= x.StartingLevel);
		}
		if (availabilities.Contains(EquipmentAvailability.NotAvailable))
		{
			second = equipmentItemList.FindAll((EquipmentItemModel x) => (x.GetOwnerForClient() != null && x.GetOwnerForClient().Level < currentEquipment.StartingLevel) || selectedSurvivor.Level < x.StartingLevel);
		}
		if (availabilities.Contains(EquipmentAvailability.NotEquipped))
		{
			second3 = equipmentItemList.FindAll((EquipmentItemModel x) => x.GetOwnerForClient() == null && selectedSurvivor.Level >= x.StartingLevel);
		}
		return first.Union(second).Union(second2).Union(second3)
			.ToList();
	}

	private List<EquipmentItemModel> SortWeaponBagList(List<EquipmentItemModel> list)
	{
		List<EquipmentItemModel> list2 = new List<EquipmentItemModel>(list);
		list2.Sort(CompareWeaponBag);
		return list2;
	}

	private int CompareWeaponBag(EquipmentItemModel a, EquipmentItemModel b)
	{
		bool flag = a.Definition?.SwitchRemoldMode ?? false;
		bool flag2 = b.Definition?.SwitchRemoldMode ?? false;
		if (flag && !flag2)
		{
			return -1;
		}
		if (!flag && flag2)
		{
			return 1;
		}
		return CompareEquipmentEquippedAndHigherLevelFirst(a, b);
	}

	private List<EquipmentItemModel> SortEquipmentList(List<EquipmentItemModel> equipmentItemList, bool equippedAndHigherLevelItemsFirst)
	{
		if (equippedAndHigherLevelItemsFirst)
		{
			equipmentItemList.StableSort((EquipmentItemModel a, EquipmentItemModel b) => CompareEquipmentEquippedAndHigherLevelFirst(a, b));
		}
		else
		{
			equipmentItemList.StableSort((EquipmentItemModel a, EquipmentItemModel b) => CompareEquipmentByDamageOrDefensePreferEquippable(a, b));
		}
		return equipmentItemList;
	}

	private int CompareEquipmentEquippedAndHigherLevelFirst(EquipmentItemModel equipmentA, EquipmentItemModel equipmentB)
	{
		bool flag = equipmentA.GetOwnerForClient() != null;
		bool flag2 = equipmentB.GetOwnerForClient() != null;
		if (flag != flag2)
		{
			if (!flag)
			{
				return 1;
			}
			return -1;
		}
		if (equipmentA.IsFavourite != equipmentB.IsFavourite)
		{
			if (!equipmentA.IsFavourite)
			{
				return 1;
			}
			return -1;
		}
		int num = equipmentB.Level.CompareTo(equipmentA.Level);
		if (num != 0)
		{
			return num;
		}
		return CompareEquipmentByDamageOrDefense(equipmentA, equipmentB);
	}

	private int CompareEquipmentByDamageOrDefense(EquipmentItemModel equipmentA, EquipmentItemModel equipmentB)
	{
		if (equipmentA.IsFavourite != equipmentB.IsFavourite)
		{
			if (!equipmentA.IsFavourite)
			{
				return 1;
			}
			return -1;
		}
		if (equipmentA.Definition.Category == EquipmentCategory.Armor)
		{
			return equipmentB.Defense.CompareTo(equipmentA.Defense);
		}
		return equipmentB.Damage.CompareTo(equipmentA.Damage);
	}

	private int CompareEquipmentByDamageOrDefensePreferEquippable(EquipmentItemModel equipmentA, EquipmentItemModel equipmentB)
	{
		if (selectedSurvivor != null)
		{
			bool flag = selectedSurvivor.CanEquip(equipmentA);
			bool flag2 = selectedSurvivor.CanEquip(equipmentB);
			if (flag != flag2)
			{
				if (!flag)
				{
					return 1;
				}
				return -1;
			}
		}
		return CompareEquipmentByDamageOrDefense(equipmentA, equipmentB);
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

	private void SetItemsActivationState(List<UIWidget> widgets, bool activationState)
	{
		foreach (UIWidget widget in widgets)
		{
			if (widget != null)
			{
				widget.gameObject.SetActive(activationState);
			}
		}
	}

	public void OnClose(GameObject closeButtonObj)
	{
		UIEvent.Send("EquipmentSelectionClosed");
	}

	public void AlignToElement(GameObject target, Vector3 offset)
	{
		base.gameObject.transform.OverlayPosition(target.transform);
		base.gameObject.transform.localPosition += offset;
		Vector2 vector = Helpers.CalculateNguiScreenSize(base.gameObject);
		float num = 0f - vector.y * 0.5f - base.gameObject.transform.localPosition.y;
		if (listContentParent != null && trianglePointer != null && trianglePointer.GetComponent<UISprite>() != null && listContentParent.GetComponent<BoxCollider>() != null)
		{
			Vector3 size = listContentParent.GetComponent<BoxCollider>().size;
			bool num2 = (double)(base.gameObject.transform.localPosition.x + size.x) > (double)vector.y * 0.5;
			Vector2 localSize = trianglePointer.GetComponent<UISprite>().localSize;
			localSize.y -= 2f;
			Vector3 zero = Vector3.zero;
			if (num2)
			{
				trianglePointer.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
				zero.x = 0f - (localSize.y + size.x);
			}
			else
			{
				trianglePointer.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
				zero.x = localSize.y;
			}
			if (base.gameObject.transform.localPosition.y - size.y < 0f - vector.y * 0.5f)
			{
				zero.y = num + size.y;
			}
			else
			{
				zero.y = localSize.y;
			}
			listContentParent.transform.localPosition = zero;
		}
	}

	private bool customCanEquipCheck(SurvivorModel survivorModel, EquipmentItemModel item)
	{
		bool num = item.Definition.CanBeEquippedBySurvivorClass(survivorModel.SurvivorClass);
		bool flag = survivorModel.CanEquipDisregardingLevel(item);
		return num && flag;
	}
}
