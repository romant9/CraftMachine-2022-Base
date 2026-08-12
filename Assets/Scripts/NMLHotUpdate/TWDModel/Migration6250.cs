namespace TWDModel
{
	public class Migration6250 : TWDModelMigration
	{
		public Migration6250()
		{
			base.Version = "6.25.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.BulePrintToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BulePrintToken);
				flag = true;
			}
			if (flag)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			return flag;
		}
	}
}
