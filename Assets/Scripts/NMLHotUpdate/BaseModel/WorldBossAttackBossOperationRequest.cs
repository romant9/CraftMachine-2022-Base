namespace BaseModel
{
	public sealed class WorldBossAttackBossOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string BossBattleId { get; set; }

		public long StartBattleUtcMs { get; set; }

		public long ParticipationScore { get; set; }
	}
}
