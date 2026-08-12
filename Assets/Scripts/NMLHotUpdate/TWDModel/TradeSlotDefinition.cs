using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TradeSlotDefinition
	{
		public int SlotId;

		public PriceCategory PriceCategory;

		public string Bucket;

		[NonSerialized]
		[JsonIgnore]
		public List<string> Buckets;

		public string UnlockRequirement;

		public List<int> GoldRepeat;

		public string EventControl;

		[NonSerialized]
		[JsonIgnore]
		public CurrencyType CurrencyUnlock;

		public int CurrencyUnlockAmount;

		public override string ToString()
		{
			return "TradeSlot: SlotId=" + SlotId + " PriceCategory=" + PriceCategory.ToString() + " Bucket=" + Bucket + " Buckets=" + Buckets?.ToString() + " UnlockRequirement=" + UnlockRequirement + " CurrencyUnlock=" + CurrencyUnlock.ToString() + " CurrencyAmount=" + CurrencyUnlockAmount;
		}

		public void Setup()
		{
			if (!string.IsNullOrEmpty(Bucket))
			{
				Buckets = Bucket.Split(';').ToList();
			}
			if (string.IsNullOrEmpty(UnlockRequirement))
			{
				return;
			}
			CurrencyUnlock = CurrencyType.None;
			if (UnlockRequirement.ToLowerInvariant().StartsWith("outpostrank"))
			{
				string[] array = UnlockRequirement.Split('(');
				CurrencyUnlockAmount = int.Parse(array[1].Replace(")", ""));
			}
			else if (!UnlockRequirement.ToLowerInvariant().StartsWith("debug"))
			{
				string[] array2 = UnlockRequirement.Split('(');
				if (array2[0] == "Gold")
				{
					CurrencyUnlock = CurrencyType.Diamonds;
				}
				else
				{
					CurrencyUnlock = (CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0]);
				}
				CurrencyUnlockAmount = int.Parse(array2[1].Replace(")", ""));
			}
		}
	}
}
