namespace TWDModel
{
	public class Migration31200 : TWDModelMigration
	{
		public Migration31200()
		{
			base.Version = "3.12.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BlackMarketToken);
			player.BlackMarket = new BlackMarket();
			player.BlackMarket.SetManager(manager);
			player.BlackMarket.Initialize();
			return true;
		}
	}
}
