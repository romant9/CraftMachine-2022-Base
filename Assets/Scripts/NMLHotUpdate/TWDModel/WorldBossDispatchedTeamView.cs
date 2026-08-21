using System.Collections.Generic;

namespace TWDModel
{
	public class WorldBossDispatchedTeamView
	{
		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public List<string> SurvivorIds { get; set; }

		public int DefenderRemainingDurability { get; set; }

		public long OccupiedAtUtcMs { get; set; }

		public long DispatchedMs { get; set; }
	}
}
