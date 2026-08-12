namespace TWDModel
{
	public class Migration770 : TWDModelMigration
	{
		public Migration770()
		{
			base.Version = "7.7.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.LydiaToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.LydiaToken);
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
