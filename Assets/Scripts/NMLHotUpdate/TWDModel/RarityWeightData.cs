using System;

namespace TWDModel
{
	[Serializable]
	public class RarityWeightData
	{
		public float Common;

		public float Uncommon;

		public float Rare;

		public float Epic;

		public float Legendary;

		public int GetWeightCount()
		{
			return 5;
		}

		public float GetWeight(int i)
		{
			return i switch
			{
				0 => Common, 
				1 => Uncommon, 
				2 => Rare, 
				3 => Epic, 
				4 => Legendary, 
				_ => 0f, 
			};
		}
	}
}
