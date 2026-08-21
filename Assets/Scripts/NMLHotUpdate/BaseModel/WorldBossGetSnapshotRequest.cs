namespace BaseModel
{
	public sealed class WorldBossGetSnapshotRequest
	{
		public string GroupId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public int SettlementSeasonId { get; set; }

		public int SettlementCycleId { get; set; }
	}
}
