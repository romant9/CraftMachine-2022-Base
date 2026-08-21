namespace BaseModel
{
	public class WorldBossCapturePointStateModel
	{
		public string CapturePoint { get; set; }

		public string OwnerGroupId { get; set; }

		public string MajorityGroupId { get; set; }

		public long MajoritySinceUtcMs { get; set; }

		public string ProtectionOwnerGroupId { get; set; }

		public long ProtectionStartUtcMs { get; set; }

		public long ProtectionEndUtcMs { get; set; }

		public bool IsProtected { get; set; }

		public long ProtectionCountdownSinceUtcMs { get; set; }

		public long UpdatedUtcMs { get; set; }
	}
}
