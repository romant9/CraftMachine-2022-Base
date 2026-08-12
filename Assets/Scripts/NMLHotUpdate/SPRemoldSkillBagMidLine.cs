using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillBagMidLine : MonoBehaviour
{
	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private List<ModSkillMode> modSkillModes;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private void Awake()
	{
		Helpers.GameObjectSetActive(EntryPrefab, value: false);
	}

	public void Setup(List<ModSkillMode> modSkillModes)
	{
		this.modSkillModes = modSkillModes;
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
		int count = modSkillModes.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<SPRemoldSkillBagMidItem>(out var component2))
			{
				component2.Setup(modSkillModes[i]);
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
