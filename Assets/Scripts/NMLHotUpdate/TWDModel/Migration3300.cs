namespace TWDModel
{
	public class Migration3300 : TWDModelMigration
	{
		public Migration3300()
		{
			base.Version = "3.3.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.TraitRerollToken);
			return true;
		}
	}
}
