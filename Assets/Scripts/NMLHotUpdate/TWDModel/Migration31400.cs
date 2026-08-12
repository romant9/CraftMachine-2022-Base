namespace TWDModel
{
	public class Migration31400 : TWDModelMigration
	{
		public Migration31400()
		{
			base.Version = "3.14.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.PrincessToken);
			return true;
		}
	}
}
