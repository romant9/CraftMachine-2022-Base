namespace BaseModel
{
	public class WorldBossCellDefenderSnapshot
	{
		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public string OwnerGroupId { get; set; }

		public string OccupyingGroupId { get; set; }

		public string OccupyingPlayerHashedId { get; set; }

		public string DefenderInfo { get; set; }

		public long UpdatedUtcMs { get; set; }
	}
}
