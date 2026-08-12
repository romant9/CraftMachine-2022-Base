using BaseModel;

namespace TWDModel
{
	public class Migration3400 : TWDModelMigration
	{
		public Migration3400()
		{
			base.Version = "3.4.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.Combat.Environmentals = new ModelList<ActorModel>();
			}
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.AlphaToken);
			return true;
		}
	}
}
