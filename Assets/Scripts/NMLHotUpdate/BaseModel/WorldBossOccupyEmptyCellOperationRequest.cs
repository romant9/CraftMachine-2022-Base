using System.Collections.Generic;

namespace BaseModel
{
	public sealed class WorldBossOccupyEmptyCellOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public bool IsPVECapturePoint { get; set; }

		public bool CellHasNoBattle { get; set; }

		public string DefenderInfo { get; set; }

		public List<string> SurvivorIds { get; set; }

		public int EnemyDurability { get; set; }

		public int PVPGuardianLoss { get; set; }

		public int BeforeProtectionSeconds { get; set; }

		public string ProtectionConfig { get; set; }

		public string TowerAConfig { get; set; }

		public string TowerAEffConfig { get; set; }

		public string TowerBConfig { get; set; }

		public string TowerBEffConfig { get; set; }

		public string DepotConfig { get; set; }

		public string DepotEffConfig { get; set; }

		public int DepotEffBossBattleTimeConfig { get; set; }

		public string PlayerName { get; set; }

		public string PlayerEmblem { get; set; }
	}
}
