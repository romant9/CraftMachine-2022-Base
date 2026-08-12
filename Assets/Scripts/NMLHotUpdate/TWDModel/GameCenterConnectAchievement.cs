namespace TWDModel
{
	public class GameCenterConnectAchievement : Achievement
	{
		public int TargetLevel;

		protected override bool InternalIsCompleted
		{
			get
			{
				if (Player != null)
				{
					return Player.Blackboard.IsToggleOn("Toggle.GameCenterConnected");
				}
				return false;
			}
		}

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
