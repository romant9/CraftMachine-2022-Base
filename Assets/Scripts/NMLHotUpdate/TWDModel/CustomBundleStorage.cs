using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class CustomBundleStorage
	{
		public int StorageID;

		public string Rewards;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
