using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BadgeFilteringController : MonoBehaviour
{
	public BadgesFilteringGroupBase[] BadgeFilters;

	public Action<List<BadgeInfo>> OnBadgesUpdated;

	public void ForceUpdate()
	{
		BadgesFilteringGroupBase[] badgeFilters = BadgeFilters;
		for (int i = 0; i < badgeFilters.Length; i++)
		{
			badgeFilters[i].ForceUpdate();
		}
	}

	private void OnEnable()
	{
		BadgesFilteringGroupBase[] badgeFilters = BadgeFilters;
		foreach (BadgesFilteringGroupBase obj in badgeFilters)
		{
			obj.OnBadgesUpdated = (Action)Delegate.Combine(obj.OnBadgesUpdated, new Action(OnBadgeFilterUpdate));
		}
	}

	private void OnDisable()
	{
		BadgesFilteringGroupBase[] badgeFilters = BadgeFilters;
		foreach (BadgesFilteringGroupBase obj in badgeFilters)
		{
			obj.OnBadgesUpdated = (Action)Delegate.Remove(obj.OnBadgesUpdated, new Action(OnBadgeFilterUpdate));
		}
	}

	private void OnBadgeFilterUpdate()
	{
		if (OnBadgesUpdated == null)
		{
			return;
		}
		List<BadgeInfo> list = new List<BadgeInfo>();
		bool flag = false;
		BadgesFilteringGroupBase[] badgeFilters = BadgeFilters;
		foreach (BadgesFilteringGroupBase badgesFilteringGroupBase in badgeFilters)
		{
			if (!flag)
			{
				flag = true;
				list = badgesFilteringGroupBase.GetBadgesFiltered();
			}
			else
			{
				list = list.Intersect(badgesFilteringGroupBase.GetBadgesFiltered()).ToList();
			}
		}
		OnBadgesUpdated(list);
	}
}
