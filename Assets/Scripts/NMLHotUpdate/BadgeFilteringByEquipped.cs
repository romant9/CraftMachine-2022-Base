using System.Collections.Generic;
using System.Linq;

public class BadgeFilteringByEquipped : BadgesFilteringGroupBase
{
	protected override List<BadgeInfo> UpdateFilter(List<BadgeInfo> allBadges, bool[] states = null)
	{
		List<BadgeInfo> list = allBadges.Where((BadgeInfo x) => !string.IsNullOrEmpty(x.OwnerName)).ToList();
		List<BadgeInfo> list2 = allBadges.Where((BadgeInfo x) => string.IsNullOrEmpty(x.OwnerName)).ToList();
		LabelsGroup[0].text = allBadges.Count.ToString();
		LabelsGroup[1].text = list.Count.ToString();
		LabelsGroup[2].text = list2.Count.ToString();
		if (states == null || states[0] || (states[1] && states[2]))
		{
			return allBadges;
		}
		if (states[1])
		{
			return list;
		}
		return list2;
	}
}
