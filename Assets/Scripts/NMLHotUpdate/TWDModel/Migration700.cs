namespace TWDModel
{
	public class Migration700 : TWDModelMigration
	{
		public Migration700()
		{
			base.Version = "7.0.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.QuickdrawCarolToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.QuickdrawCarolToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.EndlessPassExpertToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.EndlessPassExpertToken);
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
