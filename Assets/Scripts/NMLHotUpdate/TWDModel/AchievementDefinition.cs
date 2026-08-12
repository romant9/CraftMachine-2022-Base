using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class AchievementDefinition
	{
		public string ID;

		public AchievementType AchievementType;

		public string LocalizationKey;

		public string Reward;

		public string Class;

		public string Params;

		public string ExtParams;

		public string DependsOn;

		public int BonusStars;

		public string AppStoreID;

		public string GooglePlayID;

		public string EpicID;

		public string SteamID;

		[JsonIgnore]
		public bool HasDependsOn
		{
			get
			{
				if (DependsOn != null)
				{
					return DependsOn.Length > 0;
				}
				return false;
			}
		}

		[JsonIgnore]
		public string BlackboardCompletedKey => "Achievement." + ID + ".Completed";

		[JsonIgnore]
		public string BlackboardRewardClaimedKey => "Achievement." + ID + ".RewardClaimed";

		[JsonIgnore]
		public string BlackboardCounterKey => "Achievement." + ID + ".Counter";

		[JsonIgnore]
		public string TitleLocalizationKey => LocalizationKey + ".Title";

		[JsonIgnore]
		public string DescriptionLocalizationKey => LocalizationKey + ".Description";
	}
}
