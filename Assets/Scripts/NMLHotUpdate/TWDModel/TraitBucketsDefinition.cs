using System;

namespace TWDModel
{
	[Serializable]
	public class TraitBucketsDefinition
	{
		public enum BucketType
		{
			Locked = -1,
			Tactical = 0,
			LowLevel = 1,
			MidLevel = 2,
			HighLevel = 3,
			Epic = 4,
			Legendary = 5,
			Apocalyptic = 6,
			None = 7
		}

		public int RarityLevel;

		public bool IsLocked;

		public bool IsTactical;
	}
}
