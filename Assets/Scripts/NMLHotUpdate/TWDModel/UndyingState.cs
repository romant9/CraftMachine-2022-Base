namespace TWDModel
{
	public class UndyingState
	{
		public bool IsUndying;

		public int ImmuneHitsRemaining;

		public int ImmuneRoundsRemaining;

		public bool BattleStartInitialized;

		public int TurnsUntilNextGrant;

		public int TotalGrantedCount;

		public int MaxTotalGrants;

		public void RecordStatus(UndyingState src)
		{
			IsUndying = src.IsUndying;
			ImmuneHitsRemaining = src.ImmuneHitsRemaining;
			ImmuneRoundsRemaining = src.ImmuneRoundsRemaining;
			BattleStartInitialized = src.BattleStartInitialized;
			TurnsUntilNextGrant = src.TurnsUntilNextGrant;
			TotalGrantedCount = src.TotalGrantedCount;
			MaxTotalGrants = src.MaxTotalGrants;
		}
	}
}
