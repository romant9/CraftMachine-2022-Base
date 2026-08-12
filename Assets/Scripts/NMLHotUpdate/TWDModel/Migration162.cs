namespace TWDModel
{
	public class Migration162 : TWDModelMigration
	{
		public Migration162()
		{
			base.Version = "1.6.2";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			int num = 0;
			foreach (string boughtBundle in player.BundleManager.BoughtBundles)
			{
				if (boughtBundle != null)
				{
					BundleDefinition bundleDefinition = manager.GameEconomyData.LEGACY_GetBundleDefinition(boughtBundle);
					if (bundleDefinition != null && bundleDefinition.SurvivorSlots > 0)
					{
						num += bundleDefinition.SurvivorSlots;
					}
				}
			}
			if (num > 0)
			{
				player.SurvivorContainer.SurvivorSlotsUpgradeLevel -= num;
				if (player.SurvivorContainer.SurvivorSlotsUpgradeLevel < 0)
				{
					player.SurvivorContainer.SurvivorSlotsUpgradeLevel = 0;
				}
				player.SurvivorContainer.SurvivorGiftSlotsCount += num;
				manager.Debug.Log("Migration162: Converted " + num + " survivor slots to gift survivor slots");
			}
			return true;
		}
	}
}
