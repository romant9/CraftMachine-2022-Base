using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildBattleActiveBonusList : MonoBehaviour
{
	[SerializeField]
	private NUIScrollableList activeBonusesList;

	[SerializeField]
	private float bonusItemScale = 1f;

	[Tooltip("Affects the spacing between the items")]
	[SerializeField]
	private int bonusItemColliderSize = 90;

	private int numOfActiveBonuses;

	public bool UpdateActiveBonuses()
	{
		List<string> list = GuildWarHelper.GetActiveBonusesList();
		if (list != null && list.Count > numOfActiveBonuses)
		{
			numOfActiveBonuses = list.Count;
			if (activeBonusesList != null)
			{
				List<string> distinctActiveBonuses = UtilsList.CreateDistinctList(list);
				StartCoroutine(DelayedActiveBonusesInstantiation(distinctActiveBonuses));
			}
		}
		if (list != null)
		{
			return list.Count > 0;
		}
		return false;
	}

	public bool AreActiveBonusesAvailable()
	{
		List<string> list = GuildWarHelper.GetActiveBonusesList();
		if (list != null)
		{
			return list.Count > 0;
		}
		return false;
	}

	private IEnumerator DelayedActiveBonusesInstantiation(List<string> distinctActiveBonuses)
	{
		yield return null;
		if (distinctActiveBonuses.Count > activeBonusesList.currentItemsCount)
		{
			activeBonusesList.UpdateWithList(distinctActiveBonuses, HelpersGfx.BonusListElementPrefabPathString, null);
			foreach (NUIListItemBase currentItems in activeBonusesList.currentItemsList)
			{
				currentItems.transform.localScale = Vector3.one * bonusItemScale;
				if (currentItems.TryGetComponent<BoxCollider>(out var component))
				{
					component.size = Vector3.one * bonusItemColliderSize;
				}
			}
			yield return null;
			activeBonusesList.SortAndReset();
		}
		for (int i = 0; i < activeBonusesList.currentItemsCount; i++)
		{
			activeBonusesList.currentItemsList[i].UpdateUI();
		}
	}

	public void Clear()
	{
		numOfActiveBonuses = 0;
		if (activeBonusesList != null)
		{
			activeBonusesList.Clear();
		}
	}
}
