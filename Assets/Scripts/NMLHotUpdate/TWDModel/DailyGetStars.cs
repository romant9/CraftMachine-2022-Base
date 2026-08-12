using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyGetStars : DailyQuest
	{
		[JsonIgnore]
		private int targetCount = -1;

		public int InitialStars { get; set; }

		[JsonIgnore]
		public int TargetCount
		{
			get
			{
				if (targetCount == -1)
				{
					int.TryParse(base.AchievementDefinition.Params, out targetCount);
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
				InitialStars = Player.MissionStatistics.Stars;
				return true;
			}
			return false;
		}

		public override int GetProgressStep()
		{
			return Player.MissionStatistics.Stars - InitialStars;
		}

		public override int GetProgressTarget()
		{
			return TargetCount;
		}
	}
}
