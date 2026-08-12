namespace TWDModel
{
	public class Migration3700 : TWDModelMigration
	{
		public Migration3700()
		{
			base.Version = "3.7.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			player.DailyLoginCalendar = new DailyLoginCampaignModel();
			player.DailyLoginCalendar.SetManager(manager);
			player.DailyLoginCalendar.Initialize();
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BetaToken);
			return true;
		}
	}
}
