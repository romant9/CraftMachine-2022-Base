using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyCompleteChallengeMissions : DailyQuest
	{
		[JsonIgnore]
		private int targetCount = -1;

		public int InitialValue { get; set; }

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
		public override bool CanComplete
		{
			get
			{
				if (Player.WeeklyChallenge != null)
				{
					return Player.WeeklyChallenge.CanPlayWeeklyChallenge;
				}
				return false;
			}
		}

		[JsonIgnore]
		protected override bool InternalIsCompleted => GetProgressStep() >= TargetCount;

		protected override bool Init()
		{
			if (TargetCount >= 0)
			{
				InitialValue = Player.Blackboard.GetCounter("Counter.NumberChallengeMissionCompleted");
				return true;
			}
			return false;
		}

		public override int GetProgressStep()
		{
			return Player.Blackboard.GetCounter("Counter.NumberChallengeMissionCompleted") - InitialValue;
		}

		public override int GetProgressTarget()
		{
			return TargetCount;
		}
	}
}
