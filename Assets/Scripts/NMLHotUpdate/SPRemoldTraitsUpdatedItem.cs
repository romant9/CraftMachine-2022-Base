using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldTraitsUpdatedItem : MonoBehaviour
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

	private SPTraitsRemoldDefinitions definition;

	private readonly List<GameObject> Entries = new List<GameObject>();

	public void Setup(string traitID)
	{
		definition = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(traitID);
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (definition != null)
		{
			bg.color = Helpers.HexToColor(definition.Color);
			traitName.text = LocalizationManager.GetText(definition.SPTraitsName);
			HelpersUI.SetTraitsIconOnSprite(traitIcon, definition.SPTraitsIcon, definition.SPTraitsIconOnCloud);
			FreshListData();
			if (definition.MaxLevel <= definition.Level)
			{
				level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLvMax");
			}
			else
			{
				level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", definition.Level);
			}
			UILabel uILabel = traitDesc;
			string sPTraitsDesc = definition.SPTraitsDesc;
			object[] arguments = definition.SPTraitsLcValue.ToArray();
			uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
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
