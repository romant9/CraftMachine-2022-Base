using System.Collections.Generic;
using System.Linq;
using TWDModel;

public class BadgeFilteringByBonus : BadgesFilteringGroupBase
{
	protected override List<BadgeInfo> UpdateFilter(List<BadgeInfo> allBadges, bool[] states = null)
	{
		List<BadgeInfo> list = allBadges.Where((BadgeInfo x) => x.Model.BonusCondition.GetType() != typeof(ConstantBonusCondition)).ToList();
		List<BadgeInfo> list2 = allBadges.Where((BadgeInfo x) => x.Model.BonusCondition.GetType() == typeof(ConstantBonusCondition)).ToList();
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
