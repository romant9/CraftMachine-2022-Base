using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ItemListItemList : MonoBehaviour
{
	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private GameObject entryContainer;

	private readonly List<GameObject> entries = new List<GameObject>();

	private List<ItemDefinition> entriesDatas = new List<ItemDefinition>();

	private void ClearEntries()
	{
		for (int i = 0; i < entries.Count; i++)
		{
			NGUITools.Destroy(entries[i]);
		}
		entries.Clear();
	}

	public void UpdateUI()
	{
		ClearEntries();
		UITable component = entryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = entryContainer.GetComponentInParent<UIScrollView>();
		FreshListData();
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void FreshListData()
	{
		int count = entriesDatas.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = entryContainer.AddChild(entryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<ItemListItemButton>(out var component))
			{
				component.Setup(entriesDatas[i]);
			}
			entries.Add(gameObject);
		}
	}

	public void InitData(List<ItemDefinition> datas)
	{
		entriesDatas.Clear();
		entriesDatas = new List<ItemDefinition>(datas);
		UpdateUI();
	}

	private void ResetSelect()
	{
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].TryGetComponent<ItemListItemButton>(out var component))
			{
				component.SetSelectState(select: false);
			}
		}
	}

	public void FreshSelectData(ItemDefinition itemDefinition, List<string> foldedSubTypes)
	{
		if (itemDefinition == null)
		{
			return;
		}
		ResetSelect();
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].TryGetComponent<ItemListItemButton>(out var component))
			{
				component.FreshSelectData(itemDefinition, foldedSubTypes);
			}
		}
	}
}
