using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldTraitsInfoPopup : HUDElement
{
	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UILabel traitDesc;

	[SerializeField]
	private UISprite[] RedDots;

	private int currentIdx;

	private List<SPTraitSlot> slotDatas;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private SPTraitsRemoldDefinitions currentDefinition => GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(slotDatas[currentIdx].ID);

	public void Initialized(EquipmentItemModel equipmentItemModel, SPTraitSlot dataEntry)
	{
		if (equipmentItemModel == null || equipmentItemModel.SpEquipmentRemoldModel == null)
		{
			return;
		}
		List<SPTraitSlot> list = equipmentItemModel.SpEquipmentRemoldModel.SPTraitSlots;
		if (equipmentItemModel.SpEquipmentRemoldModel.HasPendingRemold)
		{
			list = equipmentItemModel.SpEquipmentRemoldModel.PendingSPTraitSlots;
		}
		if (list != null && list.Count > 0)
		{
			int num = list.FindIndex((SPTraitSlot x) => x != null && x.ID == dataEntry.ID);
			currentIdx = num;
			slotDatas = list;
			ResetRedDots();
			SetPage(currentIdx);
		}
	}

	private void ResetRedDots()
	{
		for (int i = 0; i < RedDots.Length; i++)
		{
			Helpers.GameObjectSetActive(RedDots[i], value: false);
		}
	}

	private void SetPage(int idx)
	{
		if (idx <= 0)
		{
			idx = 0;
		}
		bg.color = Helpers.HexToColor(currentDefinition.Color);
		traitName.text = LocalizationManager.GetText(currentDefinition.SPTraitsName);
		HelpersUI.SetTraitsIconOnSprite(traitIcon, currentDefinition.SPTraitsIcon, currentDefinition.SPTraitsIconOnCloud);
		FreshListData();
		if (slotDatas[currentIdx].IsMaxLevel())
		{
			level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLvMax");
		}
		else
		{
			level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", slotDatas[currentIdx].Level);
		}
		UILabel uILabel = traitDesc;
		string sPTraitsDesc = currentDefinition.SPTraitsDesc;
		object[] arguments = currentDefinition.SPTraitsLcValue.ToArray();
		uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
		Helpers.GameObjectSetActive(RedDots[currentIdx], value: true);
	}

	private void FreshListData()
	{
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		int star = currentDefinition.Star;
		for (int i = 0; i < star; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
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

	public void OnClickSwitchLeft()
	{
		Helpers.GameObjectSetActive(RedDots[currentIdx], value: false);
		currentIdx = ((currentIdx - 1 >= 0) ? (currentIdx - 1) : 5);
		SetPage(currentIdx);
	}

	public void OnClickSwitchRight()
	{
		Helpers.GameObjectSetActive(RedDots[currentIdx], value: false);
		currentIdx = ((currentIdx + 1 <= 5) ? (currentIdx + 1) : 0);
		SetPage(currentIdx);
	}
}
