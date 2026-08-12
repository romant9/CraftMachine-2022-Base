namespace TWDModel
{
	public class Migration31000 : TWDModelMigration
	{
		public Migration31000()
		{
			base.Version = "3.10.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.UpdateCurrencyCapacity(CurrencyType.GvGGas);
			return true;
		}
	}
}
