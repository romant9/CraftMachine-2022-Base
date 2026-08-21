using System.Collections.Generic;

namespace TWDModel
{
	public class WorldBossReturningTeamView
	{
		public string ReturningTeamId { get; set; }

		public string CapturePoint { get; set; }

		public List<string> SurvivorIds { get; set; }

		public long StartUtcMs { get; set; }

		public long ReturnEndUtcMs { get; set; }

		public long RemainingMs { get; set; }

		public int InstantReturnGoldCost { get; set; }
	}
}
