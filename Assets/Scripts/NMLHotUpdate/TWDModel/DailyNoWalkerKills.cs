using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyNoWalkerKills : DailyQuest
	{
		[JsonIgnore]
		private int targetCount = -1;

		public int InitialValue { get; set; }

		[JsonIgnore]
		public override bool IsValidForBonusStars
		{
			get
			{
				CombatModel combat = Player.Combat;
				if (combat != null)
				{
					return combat.MissionStatistics.WalkersKilled == 0;
				}
				return false;
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
		protected override bool InternalIsCompleted => GetProgressStep() >= TargetCount;

		protected override bool Init()
		{
			if (TargetCount >= 0)
			{
				InitialValue = Player.Blackboard.GetCounter("Counter.NumberMissionsCompletedNoWalkerKills");
				return true;
			}
			return false;
		}

		public override int GetProgressStep()
		{
			return Player.Blackboard.GetCounter("Counter.NumberMissionsCompletedNoWalkerKills") - InitialValue;
		}

		public override int GetProgressTarget()
		{
			return TargetCount;
		}
	}
}
