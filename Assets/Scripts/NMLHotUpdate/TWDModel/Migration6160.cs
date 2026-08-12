namespace TWDModel
{
	public class Migration6160 : TWDModelMigration
	{
		public Migration6160()
		{
			base.Version = "6.16.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.RFMGiftManager == null)
			{
				player.RFMGiftManager = new RFMGiftManager();
				player.RFMGiftManager.SetManager(manager);
				player.RFMGiftManager.Initialize();
				flag = true;
			}
			if (player.NewbieSenvenQuest == null)
			{
				player.NewbieSenvenQuest = new NewbieSevenQuestModel();
				player.NewbieSenvenQuest.SetManager(manager);
				player.NewbieSenvenQuest.Initialize();
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.BuildingToken1min) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BuildingToken1min, CurrencyType.BuildingToken5min, CurrencyType.BuildingToken30min, CurrencyType.TrainingToken5min, CurrencyType.EquipmentToken1min, CurrencyType.EquipmentToken10min, CurrencyType.HealingToken1min, CurrencyType.HealingToken5min);
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
