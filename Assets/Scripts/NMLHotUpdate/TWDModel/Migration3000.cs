namespace TWDModel
{
	public class Migration3000 : TWDModelMigration
	{
		public Migration3000()
		{
			base.Version = "3.0.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.GuildBattleRP, CurrencyType.GvGGas, CurrencyType.BattlePass);
			player.GvGSeasonModelPlayer = new GvGSeasonModelPlayer();
			player.GvGSeasonModelPlayer.SetManager(manager);
			player.GvGSeasonModelPlayer.Initialize();
			player.GuildShopModel = new GuildShopModel();
			player.GuildShopModel.SetManager(manager);
			player.GuildShopModel.Initialize();
			player.PlayerEmblem = new PlayerEmblem();
			return true;
		}
	}
}
