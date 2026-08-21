namespace BaseModel
{
	public sealed class WorldBossSignUpCycleOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public int SignUpNumNeed { get; set; }

		public long CycleStartTimeUtcMs { get; set; }

		public long CycleEndTimeUtcMs { get; set; }

		public long SeasonStartTimeUtcMs { get; set; }

		public long SeasonEndTimeUtcMs { get; set; }

		public string GuildName { get; set; }

		public int MaxDifficulty { get; set; }

		public int MatchBeforeStart { get; set; }

		public int SignUpCloseTime { get; set; }

		public int DifficultyCloseTime { get; set; }

		public int StartDifficulty { get; set; }

		public int[] PassScoreDifficulties { get; set; }

		public long[] PassScoreValues { get; set; }
	}
}
