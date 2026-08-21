namespace TWDModel
{
	public class WorldBossSeasonCycleRecord
	{
		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public WorldBossSeasonCycleRecord()
		{
		}

		public WorldBossSeasonCycleRecord(int seasonId, int cycleId)
		{
			SeasonId = seasonId;
			CycleId = cycleId;
		}
	}
}
