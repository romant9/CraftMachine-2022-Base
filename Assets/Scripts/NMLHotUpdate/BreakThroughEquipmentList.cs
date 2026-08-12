using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BreakThroughEquipmentList : MonoBehaviour
{
	[SerializeField]
	private GameObject BTEquipmentEntryPrefab;

	[SerializeField]
	private GameObject BTEquipmentEntryContainer;

	private EquipmentItemModel equipmentItemModel;

	private readonly List<GameObject> BTEquipmentEntries = new List<GameObject>();

	private int NeedApocalypticNum;

	private void ClearBTLevelEntries()
	{
		for (int i = 0; i < BTEquipmentEntries.Count; i++)
		{
			NGUITools.Destroy(BTEquipmentEntries[i]);
		}
		BTEquipmentEntries.Clear();
	}

	public void ClearData()
	{
		equipmentItemModel = null;
		NeedApocalypticNum = 0;
		ClearBTLevelEntries();
	}

	public void UpdateUI()
	{
		if (equipmentItemModel != null)
		{
			NeedApocalypticNum = equipmentItemModel.GetBreakThroughWeaponApocalypticNumber();
			ClearBTLevelEntries();
			UITable component = BTEquipmentEntryContainer.GetComponent<UITable>();
			UIScrollView componentInParent = BTEquipmentEntryContainer.GetComponentInParent<UIScrollView>();
			FreshListData();
			component.Reposition();
			componentInParent.ResetPosition();
		}
	}

	private void FreshListData()
	{
		List<EquipTokenItemModel> breakthroughConsumables = equipmentItemModel.GetBreakthroughConsumables();
		if (breakthroughConsumables == null || breakthroughConsumables.Count <= 0)
		{
			return;
		}
		foreach (EquipTokenItemModel item in breakthroughConsumables)
		{
			if (item == null || item.OwnedTokensAmount <= 0)
			{
				continue;
			}
			int ownedTokensAmount = item.OwnedTokensAmount;
			for (int i = 0; i < ownedTokensAmount; i++)
			{
				GameObject gameObject = BTEquipmentEntryContainer.AddChild(BTEquipmentEntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				if (gameObject.TryGetComponent<EquipmentTokenButton>(out var component))
				{
					component.Setup(item);
				}
				BTEquipmentEntries.Add(gameObject);
			}
		}
	}

	public void InitData(EquipmentItemModel itemModel)
	{
		equipmentItemModel = itemModel;
		UpdateUI();
	}

	public void AutoSelect()
	{
		ResetSelect();
		if (NeedApocalypticNum <= 0 || BTEquipmentEntries.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < NeedApocalypticNum; i++)
		{
			if (BTEquipmentEntries[i].TryGetComponent<EquipmentTokenButton>(out var component))
			{
				component.SetSelectState(select: true);
			}
		}
	}

	public void ResetSelect()
	{
		for (int i = 0; i < BTEquipmentEntries.Count; i++)
		{
			if (BTEquipmentEntries[i].TryGetComponent<EquipmentTokenButton>(out var component))
			{
				component.SetSelectState(select: false);
			}
		}
	}
}
