namespace TWDModel
{
	public class Migration7210 : TWDModelMigration
	{
		public Migration7210()
		{
			base.Version = "7.21.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.WorldBossExchangeCoin) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.WorldBossExchangeCoin);
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
