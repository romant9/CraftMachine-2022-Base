using Epic.OnlineServices.P2P;
using System.Collections.Generic;
using System.Linq;

public class BadgeFilteringByLevel : BadgesFilteringGroupBase
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
			list[GetIndex(item.Model.EffectRoll)].Add(item);
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

	private int GetIndex(int level)
	{
		if (level >= 0 && level < 30) return 1;
		else if (level >= 30 && level < 50) return 2;
		else if (level >= 50 && level < 70) return 3;
		else if (level >= 70 && level < 88) return 4;
		else return 5;
	}
}
