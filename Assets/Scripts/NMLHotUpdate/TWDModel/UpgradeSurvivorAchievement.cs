namespace TWDModel
{
	public class UpgradeSurvivorAchievement : Achievement
	{
		public int TargetLevel;

		protected override bool InternalIsCompleted => Player.SurvivorContainer.GetHighestSurvivorLevel() >= TargetLevel;

		protected override bool Init()
		{
			TargetLevel = 0;
			if (!int.TryParse(base.AchievementDefinition.Params, out TargetLevel))
			{
				return false;
			}
			return true;
		}

		public override int GetProgressStep()
		{
			if (!InternalIsCompleted)
			{
				return 0;
			}
			return 1;
		}

		public override int GetProgressTarget()
		{
			return 1;
		}
	}
}
