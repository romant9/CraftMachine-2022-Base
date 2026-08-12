namespace TWDModel
{
	public class Migration6180 : TWDModelMigration
	{
		public Migration6180()
		{
			base.Version = "6.18.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.GauntletAaronToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.GauntletAaronToken);
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
