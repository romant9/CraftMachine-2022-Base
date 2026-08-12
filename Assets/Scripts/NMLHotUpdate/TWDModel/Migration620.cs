namespace TWDModel
{
	public class Migration620 : TWDModelMigration
	{
		public Migration620()
		{
			base.Version = "6.2.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			if (player.GvGSeasonModelPlayer != null && player.GvGSeasonModelPlayer.StartedGvGSeasonId == 31)
			{
				player.GvGSeasonModelPlayer.StartedGvGSeasonId = 30;
				player.GuildShopModel.CurrentSeason = 30;
				player.GuildShopModel.GuildShopAvailableItems.Clear();
				player.GvGSeasonModelPlayer.GuildWarModelPlayer.StartedWarId = 100;
			}
			return true;
		}
	}
}
