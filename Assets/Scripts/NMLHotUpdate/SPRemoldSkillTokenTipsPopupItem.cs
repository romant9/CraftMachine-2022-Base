using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillTokenTipsPopupItem : MonoBehaviour
{
	[SerializeField]
	private UISprite skillBg;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UISprite class_Icon;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	private SPTraitsRemoldDefinitions definition;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public void Setup(SPTraitsRemoldDefinitions definition)
	{
		this.definition = definition;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (definition != null)
		{
			skillBg.color = Helpers.HexToColor(definition.Color);
			traitName.text = LocalizationManager.GetText(definition.SPTraitsName);
			HelpersUI.SetTraitsIconOnSprite(traitIcon, definition.SPTraitsIcon, definition.SPTraitsIconOnCloud);
			class_Icon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(definition.AvailableClass);
			starList.Setup(definition.Star);
			FreshListData();
		}
	}

	private void FreshListData()
	{
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		Dictionary<CurrencyType, int> makingCost = playerModel.ModSkillManager.GetMakingCost(definition.ID);
		if (makingCost != null && makingCost.Count > 0)
		{
			foreach (KeyValuePair<CurrencyType, int> item in makingCost)
			{
				GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
				if (gameObject.TryGetComponent<SPRemoldSkillTokenTipsPopupItemTokenItem>(out var component2))
				{
					component2.Setup(item.Key, item.Value);
				}
				Entries.Add(gameObject);
			}
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

	public void OnclickSkillItem()
	{
		UIEvent.Send("SPRemoldSkillTokenTipsPopupItemClick", definition.ID);
	}
}
