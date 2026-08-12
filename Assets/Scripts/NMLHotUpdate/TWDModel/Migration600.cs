namespace TWDModel
{
	public class Migration600 : TWDModelMigration
	{
		public Migration600()
		{
			base.Version = "6.0.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CowboyNeganToken);
			return true;
		}
	}
}
