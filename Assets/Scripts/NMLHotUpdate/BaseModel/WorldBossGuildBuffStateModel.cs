namespace BaseModel
{
	public class WorldBossGuildBuffStateModel
	{
		public string GroupId { get; set; }

		public bool OwnedTowerA { get; set; }

		public bool OwnedTowerB { get; set; }

		public bool OwnedDepot { get; set; }

		public string TowerAEffect { get; set; }

		public string TowerBEffect { get; set; }

		public string DepotEffect { get; set; }

		public int DepotBossBattleTimeEffect { get; set; }

		public long UpdatedUtcMs { get; set; }
	}
}
