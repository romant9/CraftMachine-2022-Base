using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldTraitListItem : MonoBehaviour
{
	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private UISprite lockState;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UILabel traitDesc;

	private SPTraitSlot dataEntry;

	private SPTraitsRemoldDefinitions definition;

	private readonly List<GameObject> Entries = new List<GameObject>();

	public void Setup(SPTraitSlot dataEntry)
	{
		this.dataEntry = dataEntry;
		definition = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(dataEntry.ID);
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (dataEntry != null && definition != null)
		{
			bg.color = Helpers.HexToColor(definition.Color);
			Helpers.GameObjectSetActive(lockState, value: false);
			traitName.text = LocalizationManager.GetText(definition.SPTraitsName);
			FreshListData();
			if (dataEntry.IsMaxLevel())
			{
				level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLvMax");
			}
			else
			{
				level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", dataEntry.Level);
			}
			Helpers.GameObjectSetActive(traitDesc, value: false);
			if (!Helpers.IsSPRemoldEasy())
			{
				UILabel uILabel = traitDesc;
				string sPTraitsDesc = definition.SPTraitsDesc;
				object[] arguments = definition.SPTraitsLcValue.ToArray();
				uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
				Helpers.GameObjectSetActive(traitDesc, value: true);
			}
		}
	}

	private void FreshListData()
	{
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		int star = definition.Star;
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
}
