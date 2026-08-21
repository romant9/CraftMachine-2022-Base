namespace BaseModel
{
	public class WorldBossMatchSnapshot
	{
		public string MatchId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string GroupIdA { get; set; }

		public string GroupIdB { get; set; }

		public string GroupNameA { get; set; }

		public string GroupNameB { get; set; }

		public int BattleDifficulty { get; set; }

		public long PassScore { get; set; }

		public bool IsFakeBattle { get; set; }

		public long MatchedAtUtcMs { get; set; }
	}
}
