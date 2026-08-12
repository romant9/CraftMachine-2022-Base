using System.Collections.Generic;

namespace TWDModel
{
	public class Migration6220 : TWDModelMigration
	{
		public Migration6220()
		{
			base.Version = "6.22.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.CustomizedBundleManager == null)
			{
				player.CustomizedBundleManager = new CustomizedBundleManager();
				player.CustomizedBundleManager.SetManager(manager);
				player.CustomizedBundleManager.Initialize();
				flag = true;
			}
			if (player.EndlessModeManager.EndlessModeZoneModel == null)
			{
				player.EndlessModeManager.EndlessModeZoneModel = new EndlessModeZoneModel
				{
					Id2ZoneIdDict = new Dictionary<int, int>()
				};
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
