using System.Collections.Generic;
using UnityEngine;

public class UITableList : MonoBehaviour
{
	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private int num;

	public void Setup(int num)
	{
		this.num = num;
		UpdateUI();
	}

	public void UpdateUI()
	{
		FreshListData();
	}

	private void FreshListData()
	{
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		for (int i = 0; i < num; i++)
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
}
