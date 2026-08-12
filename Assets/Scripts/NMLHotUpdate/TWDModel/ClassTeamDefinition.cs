using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ClassTeamDefinition
	{
		public int ChallengeID;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUTC;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUTC;

		public string Class;

		public string Reward;

		public string Pic_Bg;

		public string Pic_Banner;

		public CurrencyType StarCurrencyType;

		private long startTime;

		private long endTime;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardsObj;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		public void SetStartTime(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUTC) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUTC) - origin).TotalSeconds * 1000;
		}

		public List<SurvivorClass> GetClasses()
		{
			List<SurvivorClass> list = new List<SurvivorClass>();
			if (string.IsNullOrWhiteSpace(Class))
			{
				return list;
			}
			string[] array = Class.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				if (!Enum.TryParse<SurvivorClass>(array[i].Trim(), out var result))
				{
					return new List<SurvivorClass>();
				}
				list.Add(result);
			}
			return list;
		}

		public void InitializeRewards(TWDModelManager manager)
		{
			if (manager == null || RewardsObj != null || string.IsNullOrEmpty(Reward))
			{
				return;
			}
			try
			{
				RewardsObj = new Rewards(Reward, manager);
			}
			catch (Exception)
			{
				RewardsObj = null;
			}
		}
	}
}
