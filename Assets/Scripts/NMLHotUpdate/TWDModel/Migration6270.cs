namespace TWDModel
{
	public class Migration6270 : TWDModelMigration
	{
		public Migration6270()
		{
			base.Version = "6.27.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CroatToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CroatToken);
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
