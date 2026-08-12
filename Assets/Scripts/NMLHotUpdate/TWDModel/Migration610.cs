using BaseModel;

namespace TWDModel
{
	public class Migration610 : TWDModelMigration
	{
		public Migration610()
		{
			base.Version = "6.1.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.QuinnToken);
			if (player.BundleManager.WebShopLootEntrys == null)
			{
				player.BundleManager.WebShopLootEntrys = new ModelList<LootEntry>();
				player.BundleManager.WebShopLootEntrys.SetManager(manager);
				player.BundleManager.WebShopLootEntrys.Initialize();
			}
			return true;
		}
	}
}
