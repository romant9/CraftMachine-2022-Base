using System.Collections.Generic;
using System.Linq;

public class BadgeFilteringByEffect : BadgesFilteringGroupBase
{
	private readonly Dictionary<string, int> effectByID = new Dictionary<string, int>
	{
		{ "Health", 1 },
		{ "FlatHealth", 1 },
		{ "DamageReduction", 2 },
		{ "FlatDamageReduction", 2 },
		{ "Damage", 3 },
		{ "FlatDamage", 3 },
		{ "CritChance", 4 },
		{ "FlatCritChance", 4 },
		{ "CritDamage", 5 },
		{ "FlatCritDamage", 5 }
	};

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
			int index = (effectByID.ContainsKey(item.Model.EffectId) ? effectByID[item.Model.EffectId] : 0);
			list[index].Add(item);
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
