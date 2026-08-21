namespace BaseModel
{
	public sealed class WorldBossSettleBossOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string BossBattleId { get; set; }

		public bool IsWin { get; set; }

		public bool IsTimeout { get; set; }

		public long BossScore { get; set; }

		public long BossDamage { get; set; }

		public long EndBattleUtcMs { get; set; }
	}
}
