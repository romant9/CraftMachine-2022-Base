namespace BaseModel
{
	public sealed class WorldBossCellStatusRequest
	{
		public string GroupId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string CapturePoint { get; set; }

		public string Cell { get; set; }
	}
}
