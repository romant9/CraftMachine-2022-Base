using System;

namespace TWDModel
{
	[Serializable]
	public class RewardShowPicEntry
	{
		public string PicId;

		public int Star;

		public int Count;

		public RewardShowPicEntry(string picId, int star, int count)
		{
			PicId = picId;
			Star = star;
			Count = count;
		}
	}
}
