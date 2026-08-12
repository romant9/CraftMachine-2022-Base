namespace TWDModel
{
	public class Migration6170 : TWDModelMigration
	{
		public Migration6170()
		{
			base.Version = "6.17.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.ActivityIntegrationManager == null)
			{
				player.ActivityIntegrationManager = new ActivityIntegrationManager();
				player.ActivityIntegrationManager.SetManager(manager);
				player.ActivityIntegrationManager.Initialize();
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
