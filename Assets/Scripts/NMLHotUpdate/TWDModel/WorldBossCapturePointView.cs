namespace TWDModel
{
	public class WorldBossCapturePointView
	{
		public WorldBossCapturePointState State { get; set; }

		public bool IsPve { get; set; }

		public int ClearedCells { get; set; }

		public int TotalCells { get; set; }

		public long ProtectionEndUtcMs { get; set; }

		public bool IsInBattle { get; set; }

		public string GroupId { get; set; }

		public bool MyUnlocked { get; set; }

		public bool OpponentUnlocked { get; set; }
	}
}
