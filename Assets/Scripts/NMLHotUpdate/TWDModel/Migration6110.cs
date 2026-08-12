namespace TWDModel
{
	public class Migration6110 : TWDModelMigration
	{
		public Migration6110()
		{
			base.Version = "6.11.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.SevenDayLoginManager == null)
			{
				player.SevenDayLoginManager = new SevenDayLoginManager();
				player.SevenDayLoginManager.SetManager(manager);
				player.SevenDayLoginManager.Initialize();
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SevenDayPremium) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SevenDayPremium);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SimonToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SimonToken);
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
