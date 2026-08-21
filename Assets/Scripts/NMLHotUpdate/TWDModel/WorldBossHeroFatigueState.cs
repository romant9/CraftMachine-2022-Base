using System.Collections.Generic;

namespace TWDModel
{
	public class WorldBossHeroFatigueState
	{
		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public Dictionary<string, WorldBossHeroFatigueEntry> Entries { get; set; }

		public WorldBossHeroFatigueState()
		{
			Entries = new Dictionary<string, WorldBossHeroFatigueEntry>();
		}

		public WorldBossHeroFatigueState(int seasonId, int cycleId)
			: this()
		{
			SeasonId = seasonId;
			CycleId = cycleId;
		}

		public bool IsForCycle(int seasonId, int cycleId)
		{
			if (SeasonId == seasonId)
			{
				return CycleId == cycleId;
			}
			return false;
		}
	}
}
