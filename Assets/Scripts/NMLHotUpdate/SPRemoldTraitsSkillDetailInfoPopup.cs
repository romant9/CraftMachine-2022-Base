using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldTraitsSkillDetailInfoPopup : HUDElement
{
	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	private EquipmentItemModel equipmentItemModel;

	private readonly List<GameObject> Entries = new List<GameObject>();

	public void Setup(EquipmentItemModel equipmentItemModel)
	{
		this.equipmentItemModel = equipmentItemModel;
		UpdateUI();
	}

	public new void UpdateUI()
	{
		FreshListData(equipmentItemModel.ModSkillSlots);
	}

	public void FreshListData(ModSkillSlot[] modSkillSlots)
	{
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		for (int i = 0; i < modSkillSlots?.Length; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<SPRemoldTraitsSkillDetailInfoItem>(out var component2))
			{
				component2.Setup(modSkillSlots[i]);
			}
			Entries.Add(gameObject);
		}
		component.Reposition();
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}
}
