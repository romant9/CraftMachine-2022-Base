using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SurvivalManualDefinition
	{
		public int ID;

		public int Order;

		public string StoryQueueName;

		public string StoryQueueDesc;

		public string StoryQueueImage;

		public List<string> ActorList;

		public int ActorLevelAttrUpgrade;

		public string StoryQueueSkill;

		public string StoryQueueSkillName;

		public string StoryQueueSkillIcon;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StoryQueueShowTime;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string ActiveOpenTime;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string ActiveEndTime;

		public string ActiveDesc;

		public int ActiveType;

		public int SouvenirMedalLevel;

		public string SouvenirMedalIcon;

		public string SouvenirMedalDesc;

		[JsonIgnore]
		public long StoryShowTimeMilliseconds
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(StoryQueueShowTime) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public long StartTimeMilliseconds
		{
			get
			{
				if (ActiveType == 0)
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(ActiveOpenTime) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public long EndTimeMilliseconds
		{
			get
			{
				if (ActiveType == 0)
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(ActiveEndTime) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public bool HasDateLimit
		{
			get
			{
				if (ActiveType == 0)
				{
					return false;
				}
				if (StartTimeMilliseconds > 0)
				{
					return EndTimeMilliseconds > 0;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsActiveEvent => ActiveType != 0;
	}
}
