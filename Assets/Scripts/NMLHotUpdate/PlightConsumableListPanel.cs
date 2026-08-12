using System;
using System.Collections.Generic;
using TWDModel;

public class PlightConsumableListPanel : ScrollableListPanel<DifficultyIncrementalDebuff>
{
	protected override bool LastEntryAtTop => false;

	public void Init(List<DifficultyIncrementalDebuff> data)
	{
		SetCards(data);
	}

	public void InitHero(List<DifficultyIncrementalDebuff> data)
	{
		List<DifficultyIncrementalDebuff> list = new List<DifficultyIncrementalDebuff>();
		List<string> list2 = GroupCountAsString(data);
		int num = 0;
		foreach (string item in list2)
		{
			DifficultyIncrementalDebuff difficultyIncrementalDebuff = new DifficultyIncrementalDebuff();
			difficultyIncrementalDebuff.Identifier = item;
			difficultyIncrementalDebuff.Name = num.ToString();
			list.Add(difficultyIncrementalDebuff);
			num++;
		}
		SetCards(list);
	}

	public static List<string> GroupCountAsString(List<DifficultyIncrementalDebuff> data)
	{
		List<string> list = new List<string>();
		int num = 6;
		int count = data.Count;
		for (int i = 0; i < count; i += num)
		{
			int val = count - i;
			list.Add(Math.Min(num, val).ToString());
		}
		return list;
	}
}
