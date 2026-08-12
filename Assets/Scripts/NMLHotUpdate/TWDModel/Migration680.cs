namespace TWDModel
{
	public class Migration680 : TWDModelMigration
	{
		public Migration680()
		{
			base.Version = "6.8.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.TrainingTokenBP_N) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.TrainingTokenBP_N);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.EquipmentTokenBP_N) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.EquipmentTokenBP_N);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.HealingTokenBP_N) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.HealingTokenBP_N);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.BuildingTokenBP_N) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BuildingTokenBP_N);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.HillTopCoin) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.HillTopCoin);
				player.HillTopStore = new HillTopStore();
				player.HillTopStore.SetManager(manager);
				player.HillTopStore.Initialize();
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
