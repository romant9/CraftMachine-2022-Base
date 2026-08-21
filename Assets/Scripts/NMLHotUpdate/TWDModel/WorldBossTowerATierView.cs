namespace TWDModel
{
	public class WorldBossTowerATierView
	{
		public long CurrentScorePerMinute { get; set; }

		public bool HasNextTier { get; set; }

		public double NextThresholdHours { get; set; }

		public long NextScorePerMinute { get; set; }
	}
}
