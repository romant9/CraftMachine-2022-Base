namespace BaseModel
{
	public class WorldBossClaimSettlementResult
	{
		public bool Success { get; set; }

		public string Message { get; set; }

		public int Difficulty { get; set; }

		public long MyGuildScore { get; set; }

		public long OpponentGuildScore { get; set; }

		public long PassScore { get; set; }
	}
}
