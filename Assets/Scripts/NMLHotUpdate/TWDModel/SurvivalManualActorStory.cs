using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SurvivalManualActorStory
	{
		public int ID;

		public string StoryActorID;

		public string LinkActorID;

		public int MemoryID;

		public int MemoryUnlockLevel;

		public string MemoryUnlockTime;

		public string MemoryInfo;

		public string MemoryImage;

		public int Attribute_hp_add;

		public int Attribute_attack_add;

		public int Attribute_attack_ratio;

		public int Attribute_hp_ratio;

		public int Attribute_critical;

		public int Attribute_dmg_critical_ratio;

		public int Attribute_dmg_total_ref_ratio;

		public string MemoryAttrUpgradeDesc;

		public string MemoryLockedTips;

		[JsonIgnore]
		public long StartMemoryUnlockTime
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(GameEconomyData.ParseDateTime(MemoryUnlockTime) - dateTime).TotalSeconds * 1000;
			}
		}
	}
}
