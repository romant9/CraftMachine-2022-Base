using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ItemListTypeList : MonoBehaviour
{
	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private GameObject entryContainer;

	private readonly List<GameObject> entries = new List<GameObject>();

	private List<TypeDefinition> entriesDatas = new List<TypeDefinition>();

	private void ClearEntries()
	{
		for (int i = 0; i < entries.Count; i++)
		{
			NGUITools.Destroy(entries[i]);
		}
		entries.Clear();
	}

	private void UpdateUI()
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
		if (entriesDatas == null || entriesDatas.Count <= 0)
		{
			return;
		}
		int count = entriesDatas.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = entryContainer.AddChild(entryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<ItemListTabButton>(out var component))
			{
				component.Setup(entriesDatas[i]);
			}
			entries.Add(gameObject);
		}
	}

	public void InitData(List<TypeDefinition> definitions)
	{
		entriesDatas = definitions;
		UpdateUI();
	}

	private void ResetSelect()
	{
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].TryGetComponent<ItemListTabButton>(out var component))
			{
				component.SetSelectState(select: false);
			}
		}
	}

	public void FreshSelectData(TypeDefinition typeDefinition)
	{
		if (typeDefinition == null)
		{
			return;
		}
		ResetSelect();
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].TryGetComponent<ItemListTabButton>(out var component))
			{
				component.FreshSelectData(typeDefinition);
			}
		}
	}
}
