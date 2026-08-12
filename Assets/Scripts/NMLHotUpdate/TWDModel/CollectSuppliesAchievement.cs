namespace TWDModel
{
	public class CollectSuppliesAchievement : Achievement
	{
		public int TargetValue;

		protected override bool InternalIsCompleted => base.Blackboard.GetCounter("Counter.Supplies.Collected") >= TargetValue;

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
			return base.Blackboard.GetCounter("Counter.Supplies.Collected");
		}

		public override int GetProgressTarget()
		{
			return TargetValue;
		}
	}
}
