using System;

namespace TWDModel
{
	[Serializable]
	public class RandomTalentChart : IWeightedItem
	{
		public int Id;

		public string TraitsId;

		public int Weight;

		public int GetWeight()
		{
			return Weight;
		}
	}
}
