namespace TWDModel
{
	public class Migration6150 : TWDModelMigration
	{
		public Migration6150()
		{
			base.Version = "6.15.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.ActiveFoundationManager == null)
			{
				player.ActiveFoundationManager = new ActiveFoundationManager();
				player.ActiveFoundationManager.SetManager(manager);
				player.ActiveFoundationManager.Initialize();
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ActiveFoundationPremium) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ActiveFoundationPremium);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ApocalypticSkipToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ApocalypticSkipToken);
				flag = true;
			}
			if (player.SubscriptionManager == null)
			{
				player.SubscriptionManager = new SubscriptionManager();
				player.SubscriptionManager.SetManager(manager);
				player.SubscriptionManager.Initialize();
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
