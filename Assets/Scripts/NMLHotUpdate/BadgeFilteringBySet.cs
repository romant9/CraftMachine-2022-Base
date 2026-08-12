using System.Collections.Generic;
using System.Linq;

public class BadgeFilteringBySet : BadgesFilteringGroupBase
{
	protected override List<BadgeInfo> UpdateFilter(List<BadgeInfo> allBadges, bool[] states = null)
	{
		List<List<BadgeInfo>> list = new List<List<BadgeInfo>>();
		for (int i = 0; i < 6; i++)
		{
			list.Add(new List<BadgeInfo>());
		}
		list[0] = allBadges;
		foreach (BadgeInfo item in list[0])
		{
			list[(int)(item.Model.Type + 1)].Add(item);
		}
		for (int j = 0; j < LabelsGroup.Length; j++)
		{
			LabelsGroup[j].text = list[j].Count.ToString();
		}
		if (states == null)
		{
			return list[0];
		}
		List<BadgeInfo> list2 = new List<BadgeInfo>();
		for (int k = 0; k < states.Length; k++)
		{
			if (states[k])
			{
				list2.AddRange(list[k]);
			}
		}
		return list2.Distinct().ToList();
	}
}
