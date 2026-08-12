namespace TWDModel
{
	public class Migration090 : TWDModelMigration
	{
		public Migration090()
		{
			base.Version = "0.9.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (!player.Tutorial.HasCompletedPart("Phone"))
			{
				throw new MigrationResetRequiredException();
			}
			if (player.BundleManager == null)
			{
				player.BundleManager = new BundleManagerModel();
				player.BundleManager.SetManager(player.manager);
				player.BundleManager.Initialize();
			}
			return true;
		}
	}
}
