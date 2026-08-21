namespace BaseModel
{
	public sealed class WorldBossSelectDifficultyOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public long CycleStartTimeUtcMs { get; set; }

		public int DifficultyCloseTime { get; set; }

		public int Difficulty { get; set; }

		public int MemberRole { get; set; }

		public int MaxDifficulty { get; set; }

		public long PassScore { get; set; }
	}
}
