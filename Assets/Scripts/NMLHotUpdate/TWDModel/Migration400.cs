namespace TWDModel
{
	public class Migration400 : TWDModelMigration
	{
		public Migration400()
		{
			base.Version = "4.0.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnEndlessModeMissions();
			player.EndlessModeManager = new EndlessModeManagerModel();
			player.EndlessModeManager.SetManager(manager);
			player.EndlessModeManager.Initialize();
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ShivaToken, CurrencyType.DogToken, CurrencyType.WhisperersMaskToken, CurrencyType.EndlessPassToken);
			return true;
		}
	}
}
