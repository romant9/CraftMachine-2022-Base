namespace TWDModel
{
	public class Migration6210 : TWDModelMigration
	{
		public Migration6210()
		{
			base.Version = "6.21.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.PerlieToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.PerlieToken);
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
