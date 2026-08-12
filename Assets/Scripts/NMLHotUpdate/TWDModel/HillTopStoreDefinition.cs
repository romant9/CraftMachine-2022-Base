using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class HillTopStoreDefinition
	{
		public int UniqueId;

		public HillTopSlotType SlotType;

		public string Reward;

		public int Score;

		public int LimitNum;

		public string LocalizationKey;

		public string ImagePath;

		public int DisplayOrder;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
