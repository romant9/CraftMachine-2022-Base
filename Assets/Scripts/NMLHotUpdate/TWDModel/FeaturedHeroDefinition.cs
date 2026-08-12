using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class FeaturedHeroDefinition
	{
		public string ActorDefinitionID;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		private long startTime;

		private long endTime;

		public string HeroSeasonIDArt;

		public int DamageBoostMultiplier;

		public int HealthBoostMultiplier;

		public int RarityModifier;

		public string BackgroundColorHex;

		public string GlowColorHex;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public bool IsActivePeriod(long utcTimeStamp)
		{
			if (utcTimeStamp >= StartTimeMilliseconds)
			{
				return utcTimeStamp < EndTimeMilliseconds;
			}
			return false;
		}

		public long TimeUntilStartMilliseconds(long utcTimeStamp)
		{
			return StartTimeMilliseconds - utcTimeStamp;
		}

		public long TimeUntilEndMilliseconds(long utcTimeStamp)
		{
			return EndTimeMilliseconds - utcTimeStamp;
		}

		public void SetStartTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void UpdateTraitDefinitionWithValues(TraitDefinition featuredBuffTrait)
		{
			int num = 0;
			if (featuredBuffTrait.Identifier == "FeaturedHeroBuff.Damage")
			{
				num = DamageBoostMultiplier;
			}
			else if (featuredBuffTrait.Identifier == "FeaturedHeroBuff.Health")
			{
				num = HealthBoostMultiplier;
			}
			else if (featuredBuffTrait.Identifier == "FeaturedHeroBuff.Rarity")
			{
				num = RarityModifier;
			}
			featuredBuffTrait.ConstructionParameters = new List<string>
			{
				ActorDefinitionID,
				num.ToString()
			};
		}
	}
}
