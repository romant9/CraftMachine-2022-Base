using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class WeeklyChallengesMapConfig
	{
		public int ID;

		public int ApocalypticMap;

		public List<int> DifficultyRange;

		public int MapID;

		public int MinDifficulty
		{
			get
			{
				if (DifficultyRange == null || DifficultyRange.Count == 0)
				{
					return 0;
				}
				return DifficultyRange[0];
			}
		}

		public int MaxDifficulty
		{
			get
			{
				if (DifficultyRange == null || DifficultyRange.Count < 2)
				{
					return 0;
				}
				return DifficultyRange[1];
			}
		}

		public bool ContainsDifficulty(int difficulty)
		{
			if (difficulty < MinDifficulty)
			{
				return false;
			}
			if (MaxDifficulty == -1)
			{
				return true;
			}
			return difficulty <= MaxDifficulty;
		}
	}
}
