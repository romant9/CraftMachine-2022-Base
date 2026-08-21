using System.Collections.Generic;

namespace BaseModel
{
	public class WorldBossCellStateSnapshot
	{
		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public string OwnerGroupId { get; set; }

		public int Status { get; set; }

		public string OccupyingGroupId { get; set; }

		public string OccupyingPlayerHashedId { get; set; }

		public string OccupyingPlayerName { get; set; }

		public string OccupyingPlayerEmblem { get; set; }

		public string FightingGroupId { get; set; }

		public string FightingPlayerHashedId { get; set; }

		public int BattleCount { get; set; }

		public int RemainingDurability { get; set; }

		public bool PveCleared { get; set; }

		public int DefenderRemainingDurability { get; set; }

		public bool HasDefender { get; set; }

		public List<string> OccupyingSurvivorIds { get; set; }

		public long OccupiedAtUtcMs { get; set; }

		public long LastBattleStartUtcMs { get; set; }

		public int LastBattleTimeLimitMs { get; set; }

		public long UpdatedUtcMs { get; set; }

		public WorldBossCellStateSnapshot()
		{
			OccupyingSurvivorIds = new List<string>();
		}
	}
}
