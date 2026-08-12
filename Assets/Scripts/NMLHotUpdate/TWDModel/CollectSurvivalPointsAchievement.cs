namespace TWDModel
{
	public class CollectSurvivalPointsAchievement : Achievement
	{
		public int TargetValue;

		protected override bool InternalIsCompleted => base.Blackboard.GetCounter("Counter.SurvivalPoints.Collected") >= TargetValue;

		protected override bool Init()
		{
			TargetValue = 0;
			if (!int.TryParse(base.AchievementDefinition.Params, out TargetValue))
			{
				return false;
			}
			return true;
		}

		public override int GetProgressStep()
		{
			return base.Blackboard.GetCounter("Counter.SurvivalPoints.Collected");
		}

		public override int GetProgressTarget()
		{
			return TargetValue;
		}
	}
}
