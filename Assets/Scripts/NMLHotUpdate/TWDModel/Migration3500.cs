namespace TWDModel
{
	public class Migration3500 : TWDModelMigration
	{
		public Migration3500()
		{
			base.Version = "3.5.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.GvGMissionKey);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			return true;
		}
	}
}
