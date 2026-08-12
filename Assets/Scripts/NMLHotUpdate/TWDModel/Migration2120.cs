namespace TWDModel
{
	public class Migration2120 : TWDModelMigration
	{
		public Migration2120()
		{
			base.Version = "2.12.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			if (player.Blackboard != null && player.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
			{
				player.Blackboard.ClearToggle("Toggle.ToggleUpdateInfoPopupShown");
			}
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ScoutRickToken, CurrencyType.HunterMorganToken, CurrencyType.ScoutDarylToken, CurrencyType.BruiserGlennToken);
			return true;
		}
	}
}
