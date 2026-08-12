namespace TWDModel
{
	public class Migration450 : TWDModelMigration
	{
		public Migration450()
		{
			base.Version = "4.5.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CommonwealthArmorToken, CurrencyType.RainbowCatToken);
			player.EndlessModeManager.UpdateLastClaimedEndlessPassTimeStamp();
			MigrationUtils.MigrateLeaderTrait(manager, "hero_rosita", "LeaderBuffNeedOnlyOne");
			return true;
		}
	}
}
