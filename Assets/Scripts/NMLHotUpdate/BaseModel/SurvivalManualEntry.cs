namespace BaseModel
{
	public sealed class SurvivalManualEntry
	{
		public string Id { get; set; }

		public long Score { get; set; }

		public long ScoreAt { get; set; }

		public string[] Tags { get; set; }

		public string Details { get; set; }
	}
}
