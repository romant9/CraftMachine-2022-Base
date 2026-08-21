namespace BaseModel
{
	public sealed class WorldBossInstantReturnOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string CapturePoint { get; set; }

		public string ReturningTeamId { get; set; }
	}
}
