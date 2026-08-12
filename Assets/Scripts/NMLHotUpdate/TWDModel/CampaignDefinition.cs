using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class CampaignDefinition
	{
		public int Id;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string RewardsAvailableUtc;

		public string TokenIcon;

		public string VisualConfig;

		public string HighlightedRewardTexture;

		public string NameLocKey;

		public string DescLocKey;

		public string CaptionLocKey;

		public string ButtonLocKey;

		public string OwnScoreLocKey;

		public string CampaignTokenLocKey;

		public string CampaignTokenBundleLocKey;

		public bool DisableAutoCollectPostCampaign;

		private long startTime;

		private long endTime;

		private long rewardsEndTime;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		[JsonIgnore]
		public long RewardsAvailableMilliseconds => rewardsEndTime;

		public void SetStartAndEndTimes(DateTime origin)
		{
			if (!string.IsNullOrEmpty(StartTimeUtc) && !string.IsNullOrEmpty(EndTimeUtc))
			{
				startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - origin).TotalSeconds * 1000;
				endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - origin).TotalSeconds * 1000;
			}
			if (!string.IsNullOrEmpty(RewardsAvailableUtc))
			{
				rewardsEndTime = (long)(GameEconomyData.ParseDateTime(RewardsAvailableUtc) - origin).TotalSeconds * 1000;
			}
		}
	}
}
