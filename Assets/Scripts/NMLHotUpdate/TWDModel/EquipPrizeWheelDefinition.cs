using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EquipPrizeWheelDefinition
	{
		public string Identifier;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public string SlotNumber;

		public string ButtonsLocKey;

		public string NameLocKey;

		public string HighlightedRewardTexture;

		public string HighlightedReward;

		public string DescLocKey;

		public int OncePrice;

		public int TenTimesPrice;

		public int TenTimesOriginalPrice;

		public int Order;

		public RadioType RadioType;

		public string cdnIcon;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public void SetTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - origin).TotalSeconds * 1000;
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - origin).TotalSeconds * 1000;
		}

		public bool IsOpen(long time)
		{
			if (startTime <= time)
			{
				return time <= endTime;
			}
			return false;
		}
	}
}
