using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GoldRadioCallDetailPopupItemItem : MonoBehaviour
{
	[SerializeField]
	private UISprite spriteIcon;

	[SerializeField]
	private UILabel labelName;

	[SerializeField]
	private GameObject EntryContainer;

	private GameObject EntryPrefab;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public void Initialize()
	{
		EntryPrefab = Helpers.GameObjectChildItem(EntryContainer.gameObject);
	}

	public void SetInfo(List<ModSkillMode> slots, HashSet<string> upShowTypes = null)
	{
		ModSkillMode modSkillMode = slots.FirstOrDefault();
		if (modSkillMode != null)
		{
			spriteIcon.spriteName = HelpersGfx.GetSurvivorEventIconName(modSkillMode.SurvivorClass.ToString());
			labelName.text = HelpersLocalization.GetSurvivorClassName(modSkillMode.SurvivorClass).ToUpper();
		}
		FreshListData(slots, upShowTypes);
	}

	private void FreshListData(List<ModSkillMode> slots, HashSet<string> upShowTypes = null)
	{
		ClearEntries(Entries);
		UIGrid component = EntryContainer.GetComponent<UIGrid>();
		for (int i = 0; i < slots?.Count; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<GoldRadioCallDetailPopupItemItemItem>(out var component2))
			{
				component2.Initialize();
				bool showUp = IsInUpShow(slots[i], upShowTypes);
				component2.Setup(i, slots[i], -1, showUp);
				Entries.Add(gameObject);
			}
		}
		component.Reposition();
	}

	private static bool IsInUpShow(ModSkillMode mode, HashSet<string> upShowTypes)
	{
		if (mode == null || upShowTypes == null || upShowTypes.Count == 0)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(mode.Type))
		{
			return upShowTypes.Contains(mode.Type);
		}
		return false;
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
