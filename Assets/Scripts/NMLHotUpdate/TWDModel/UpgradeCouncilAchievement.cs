namespace TWDModel
{
	public class UpgradeCouncilAchievement : Achievement
	{
		public int TargetLevel;

		protected override bool InternalIsCompleted => (Player.Camp.GetBuilding("Council")?.Level ?? 0) >= TargetLevel;

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
			return Player.Camp.GetBuilding("Council")?.Level ?? 0;
		}

		public override int GetProgressTarget()
		{
			return TargetLevel;
		}
	}
}
