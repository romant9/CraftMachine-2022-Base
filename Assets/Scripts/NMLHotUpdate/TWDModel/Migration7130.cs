namespace TWDModel
{
	public class Migration7130 : TWDModelMigration
	{
		public Migration7130()
		{
			base.Version = "7.13.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SPTraitsRemoldToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SPTraitsRemoldToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.SPTraitsUpgradeToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.SPTraitsUpgradeToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.GoldRadio) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.GoldRadio);
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
