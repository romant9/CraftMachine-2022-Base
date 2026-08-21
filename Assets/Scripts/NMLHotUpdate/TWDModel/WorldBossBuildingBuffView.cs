namespace TWDModel
{
	public class WorldBossBuildingBuffView
	{
		public string CapturePoint { get; set; }

		public bool IsOccupiedByMe { get; set; }

		public bool IsActive { get; set; }

		public double CurrentThresholdHours { get; set; }

		public string CurrentValue { get; set; } = "0";

		public double NextThresholdHours { get; set; }

		public string NextValue { get; set; } = "0";

		public bool IsMaxTier { get; set; }

		public long ExtraBossBattleTimes { get; set; }
	}
}
