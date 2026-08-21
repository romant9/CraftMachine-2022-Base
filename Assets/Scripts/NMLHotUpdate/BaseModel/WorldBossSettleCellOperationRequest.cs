using System.Collections.Generic;

namespace BaseModel
{
	public sealed class WorldBossSettleCellOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string CapturePoint { get; set; }

		public string Cell { get; set; }

		public bool IsWin { get; set; }

		public bool IsTimeout { get; set; }

		public bool IsPVECapturePoint { get; set; }

		public int EnemyDurability { get; set; }

		public int PVEGuardianLoss { get; set; }

		public int PVPGuardianLoss { get; set; }

		public int PVPGuardianDiePerHero { get; set; }

		public int KilledDefenderCount { get; set; }

		public long EndBattleUTCMs { get; set; }

		public string DefenderInfo { get; set; }

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

		public long WinScore { get; set; }

		public string PlayerName { get; set; }

		public string PlayerEmblem { get; set; }
	}
}
