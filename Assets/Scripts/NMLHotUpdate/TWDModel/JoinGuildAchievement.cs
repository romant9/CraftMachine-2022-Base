namespace TWDModel
{
	public class JoinGuildAchievement : Achievement
	{
		protected override bool InternalIsCompleted => Player.IsGuildMember;

		protected override bool Init()
		{
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
