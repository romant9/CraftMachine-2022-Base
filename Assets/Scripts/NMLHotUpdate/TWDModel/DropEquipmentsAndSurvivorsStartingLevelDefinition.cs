using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class DropEquipmentsAndSurvivorsStartingLevelDefinition
	{
		public DropRewardType RewardType;

		public DropType DropType;

		public int ControlLevelMin;

		public int ControlLevelMax;

		public DropEventDefinition.DropEventTag Tag;

		[GEDListFromColumns]
		public List<int> Levels;

		public List<int> GetStartingLevelForRarity(int rarity)
		{
			if (rarity < 0 || Levels.Count < 2)
			{
				return new List<int> { 1, 1 };
			}
			int num = Math.Min(rarity, Levels.Count / 2 - 1);
			return new List<int>
			{
				Levels[num * 2],
				Levels[num * 2 + 1]
			};
		}
	}
}
