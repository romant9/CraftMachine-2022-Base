namespace TWDModel
{
	public class Migration2100 : TWDModelMigration
	{
		public Migration2100()
		{
			base.Version = "2.10.0";
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
			return true;
		}
	}
}
