namespace TWDModel
{
	public class Migration640 : TWDModelMigration
	{
		public Migration640()
		{
			base.Version = "6.4.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.Fairmoney) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.Fairmoney);
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
