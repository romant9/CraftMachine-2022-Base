namespace TWDModel
{
	public class Migration740 : TWDModelMigration
	{
		public Migration740()
		{
			base.Version = "7.4.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.BadgeToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BadgeToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.PastaToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.PastaToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.NotebookToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.NotebookToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.CapToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CapToken);
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
