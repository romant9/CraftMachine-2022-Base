namespace TWDModel
{
	public class KillWalkersAchievement : Achievement
	{
		public int TargetCount;

		protected override bool InternalIsCompleted => Player.MissionStatistics.WalkersKilled >= TargetCount;

		protected override bool Init()
		{
			TargetCount = 0;
			if (!int.TryParse(base.AchievementDefinition.Params, out TargetCount))
			{
				return false;
			}
			return true;
		}

		public override int GetProgressStep()
		{
			return Player.MissionStatistics.WalkersKilled;
		}

		public override int GetProgressTarget()
		{
			return TargetCount;
		}
	}
}
