using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyMultiKill : DailyQuest
	{
		[JsonIgnore]
		private int multiKillThreshold = -1;

		[JsonIgnore]
		private int targetCount = -1;

		public int InitialValue { get; set; }

		public int[] InitialValues { get; set; }

		[JsonIgnore]
		public int MultiKillThreshold
		{
			get
			{
				if (multiKillThreshold == -1)
				{
					int.TryParse(base.AchievementDefinition.Params, out multiKillThreshold);
				}
				return multiKillThreshold;
			}
		}

		[JsonIgnore]
		public int TargetCount
		{
			get
			{
				if (targetCount == -1)
				{
					int.TryParse(base.AchievementDefinition.ExtParams, out targetCount);
				}
				return targetCount;
			}
		}

		[JsonIgnore]
		public override bool IsValidForBonusStars
		{
			get
			{
				CombatModel combat = Player.Combat;
				if (combat != null)
				{
					for (int i = MultiKillThreshold; i < 10; i++)
					{
						if (combat.MissionStatistics.GetMultiKillCount(i) > 0)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		protected override bool InternalIsCompleted => GetProgressStep() >= TargetCount;

		protected override bool Init()
		{
			if (MultiKillThreshold >= 0 && TargetCount >= 0)
			{
				InitialValues = new int[10];
				for (int i = 0; i < InitialValues.Length; i++)
				{
					InitialValues[i] = Player.MissionStatistics.GetMultiKillCount(i);
				}
				return true;
			}
			return false;
		}

		public override int GetProgressStep()
		{
			int num = 0;
			for (int i = MultiKillThreshold; i < InitialValues.Length; i++)
			{
				num += Player.MissionStatistics.GetMultiKillCount(i) - InitialValues[i];
			}
			return num;
		}

		public override int GetProgressTarget()
		{
			return TargetCount;
		}
	}
}
