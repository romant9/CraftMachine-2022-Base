namespace TWDModel
{
	public class Migration160 : TWDModelMigration
	{
		public Migration160()
		{
			base.Version = "1.6.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player.OutpostModel == null)
			{
				player.OutpostModel = new OutpostModel();
				player.OutpostModel.Initialize();
			}
			CurrencyModel currencyModel = new CurrencyModel(CurrencyType.Outpost);
			currencyModel.SetManager(manager);
			player.Currencies.Add(currencyModel);
			return true;
		}
	}
}
