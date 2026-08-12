namespace TWDModel
{
	public class BeginnerBattlePassInfo : TWDModelObject
	{
		public const int BeginnerBattlePassSeasonId = int.MaxValue;

		public BattlePassRewardDefinition[] CachedRewards;

		public BeginnerBattlePassState State { get; set; }

		public long StartTimestamp { get; set; }

		public long EndTimestamp { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public bool StartSeason(long now)
		{
			if (State == BeginnerBattlePassState.NotStarted)
			{
				State = BeginnerBattlePassState.Ongoing;
				StartTimestamp = now;
				EndTimestamp = now + UtilsDateTime.DayInMilliseconds * base.gameEconomyData.BeginnerBattlePassConfig.SeasonDuration;
				CachedRewards = base.gameEconomyData.BeginnerBattlePassRewardDefinitions;
				return true;
			}
			return false;
		}
	}
}
