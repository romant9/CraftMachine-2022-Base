namespace TWDModel
{
	public class Migration3200 : TWDModelMigration
	{
		public Migration3200()
		{
			base.Version = "3.2.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.EquipmentUpgradeToken, CurrencyType.ShooterMaggieToken);
			return true;
		}
	}
}
