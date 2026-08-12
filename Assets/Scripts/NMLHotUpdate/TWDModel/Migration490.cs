namespace TWDModel
{
	public class Migration490 : TWDModelMigration
	{
		public Migration490()
		{
			base.Version = "4.9.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			player.EndlessModeManager?.CurrentExpertModeHeroes?.Clear();
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnEndlessModeMissions();
			player.MapContainerModel.SpawnSeasonEpisodes();
			return true;
		}
	}
}
