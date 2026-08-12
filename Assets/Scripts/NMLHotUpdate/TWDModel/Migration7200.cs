namespace TWDModel
{
	public class Migration7200 : TWDModelMigration
	{
		public Migration7200()
		{
			base.Version = "7.20.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ReturnMedal) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ReturnMedal);
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
