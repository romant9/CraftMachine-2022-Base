using System;

namespace TWDModel
{
	[Serializable]
	public class RewardPicEntry
	{
		public int Count;

		public string SpriteName;

		public RewardPicEntry(int count, string spriteName)
		{
			Count = count;
			SpriteName = spriteName;
		}
	}
}
