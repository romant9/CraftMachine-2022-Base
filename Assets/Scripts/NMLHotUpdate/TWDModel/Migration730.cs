namespace TWDModel
{
	public class Migration730 : TWDModelMigration
	{
		public Migration730()
		{
			base.Version = "7.3.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.MTToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.MTToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.EXToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.EXToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.PrimarySupportTalentToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.PrimarySupportTalentToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.AdvancedSupportTalentToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.AdvancedSupportTalentToken);
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
