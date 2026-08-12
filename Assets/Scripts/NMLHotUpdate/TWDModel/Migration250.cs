using System;
using System.Collections.Generic;
using System.Globalization;

namespace TWDModel
{
	public class Migration250 : TWDModelMigration
	{
		public Migration250()
		{
			base.Version = "2.5.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player.PendingIAPs == null)
			{
				player.PendingIAPs = new List<PendingPurchaseInfo>();
			}
			if (player.OutpostModel != null)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				long num = (long)(player.Created.ToUniversalTime() - dateTime).TotalSeconds;
				if (!string.IsNullOrEmpty(manager.GameEconomyData.ConfigData.OutpostMinimumWalkerChangeDate))
				{
					long num2 = (long)(DateTime.Parse(manager.GameEconomyData.ConfigData.OutpostMinimumWalkerChangeDate, new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal).ToUniversalTime() - dateTime).TotalSeconds;
					player.OutpostModel.MigrationAddMissingWalkers(num < num2);
				}
			}
			if (player.BundleManager != null)
			{
				player.BundleManager.CreateRotatingBundleManagerForOldPlayer();
			}
			switch (player.Language)
			{
			case "pt":
				player.Language = "pt-br";
				break;
			case "cn":
				player.Language = "zh-cn";
				break;
			case "zh":
				player.Language = "zh-tw";
				break;
			}
			return true;
		}
	}
}
