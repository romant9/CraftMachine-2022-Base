namespace TWDModel
{
	public class Migration182 : TWDModelMigration
	{
		public Migration182()
		{
			base.Version = "1.8.2";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player != null && player.CurrentOutpostSeasonId == -1 && player.HasValidOutpost)
			{
				player.CurrentOutpostSeasonId = 0;
			}
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player.BundleManager != null && player.BundleManager.InitiatedLimitedBundles != null && player.BundleManager.InitiatedLimitedBundles.Count > 0)
			{
				player.BundleManager.InitiatedLimitedBundles.Clear();
			}
			return true;
		}
	}
}
