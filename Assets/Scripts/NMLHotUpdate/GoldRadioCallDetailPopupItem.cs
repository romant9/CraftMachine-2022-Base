using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GoldRadioCallDetailPopupItem : MonoBehaviour
{
	[SerializeField]
	private UITable table;

	[SerializeField]
	private UISprite sprite;

	[SerializeField]
	private List<GameObject> stars;

	[SerializeField]
	private UILabel labelTitle;

	[SerializeField]
	private GameObject EntryContainer;

	private GameObject EntryPrefab;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private Dictionary<SurvivorClass, List<ModSkillMode>> dictSlot = new Dictionary<SurvivorClass, List<ModSkillMode>>();

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public void Initialize()
	{
		EntryPrefab = Helpers.GameObjectChildItem(EntryContainer.gameObject);
	}

	public void SetInfo(int star, string text)
	{
		labelTitle.text = text;
		for (int i = 0; i < stars?.Count; i++)
		{
			Helpers.GameObjectSetActive(stars[i], i < star);
		}
		table.Reposition();
	}

	public void FreshListData(List<string> showList, HashSet<string> upShowTypes = null)
	{
		ClearEntries(Entries);
		if (showList != null)
		{
			List<ModSkillMode> slots = EquipSkillRecommendEquipModel.LoadConfigModSkillModes(showList, playerModel);
			FreshListData(slots, upShowTypes);
		}
	}

	public void FreshListData(List<ModSkillMode> slots, HashSet<string> upShowTypes = null)
	{
		dictSlot.Clear();
		foreach (ModSkillMode slot in slots)
		{
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = slot.GetSpTraitsDefaultTrait();
			if (spTraitsDefaultTrait != null)
			{
				if (!dictSlot.TryGetValue(spTraitsDefaultTrait.AvailableClass, out var value))
				{
					sprite.color = Helpers.HexToColor(spTraitsDefaultTrait.Color);
					value = new List<ModSkillMode>();
					dictSlot.Add(spTraitsDefaultTrait.AvailableClass, value);
				}
				value.Add(slot);
			}
		}
		foreach (KeyValuePair<SurvivorClass, List<ModSkillMode>> item in dictSlot)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<GoldRadioCallDetailPopupItemItem>(out var component))
			{
				component.Initialize();
				component.SetInfo(item.Value, upShowTypes);
				Entries.Add(gameObject);
			}
		}
		EntryContainer.GetComponent<UITable>().Reposition();
	}

	private void ClearEntries(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			NGUITools.Destroy(list[i]);
		}
		list.Clear();
	}
}
