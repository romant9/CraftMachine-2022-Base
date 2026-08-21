using System.Collections.Generic;

namespace BaseModel
{
	public sealed class WorldBossWithdrawCellOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public List<string> SurvivorIds { get; set; }

		public long WithdrawDurationMs { get; set; }

		public int BeforeProtectionSeconds { get; set; }

		public string ProtectionConfig { get; set; }

		public string TowerAConfig { get; set; }

		public string TowerAEffConfig { get; set; }

		public string TowerBConfig { get; set; }

		public string TowerBEffConfig { get; set; }

		public string DepotConfig { get; set; }

		public string DepotEffConfig { get; set; }

		public int DepotEffBossBattleTimeConfig { get; set; }
	}
}
