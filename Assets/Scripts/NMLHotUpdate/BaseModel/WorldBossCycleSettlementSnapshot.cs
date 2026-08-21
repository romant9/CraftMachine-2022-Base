namespace BaseModel
{
	public class WorldBossCycleSettlementSnapshot
	{
		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public long CycleEndUtcMs { get; set; }

		public string MatchId { get; set; }

		public string GroupIdA { get; set; }

		public string GroupIdB { get; set; }

		public string GroupNameA { get; set; }

		public string GroupNameB { get; set; }

		public string MyGroupId { get; set; }

		public string OpponentGroupId { get; set; }

		public int RewardDifficulty { get; set; }

		public long PassScore { get; set; }

		public long MyGuildScore { get; set; }

		public long OpponentGuildScore { get; set; }

		public bool IsVictory { get; set; }

		public long MyPassScoreReachedUtcMs { get; set; }

		public long OpponentPassScoreReachedUtcMs { get; set; }

		public long PlayerScore { get; set; }

		public long PlayerMaxDamage { get; set; }

		public long PlayerBattleCount { get; set; }

		public int CrossGuildScoreRank { get; set; }

		public bool HasClaimedSettlement { get; set; }
	}
}
