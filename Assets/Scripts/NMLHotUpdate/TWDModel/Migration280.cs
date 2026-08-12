using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class Migration280 : TWDModelMigration
	{
		public Migration280()
		{
			base.Version = "2.8.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player.BundleManager.BoughtBundlesAmount != null)
			{
				Dictionary<string, int>.KeyCollection keys = player.BundleManager.BoughtBundlesAmount.Keys;
				if (keys.Contains("TWD_BUNDLE_ASSAULT"))
				{
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation4999");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation2999");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation1999");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation799");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation299");
				}
				else if (keys.Contains("TWD_BUNDLE_WARRIOR"))
				{
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation2999");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation1999");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation799");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation299");
				}
				else if (keys.Contains("TWD_BUNDLE_SHOOTER"))
				{
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation1999");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation799");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation299");
				}
				else if (keys.Contains("TWD_BUNDLE_SCARRED_VETERAN"))
				{
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation1999");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation799");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation299");
				}
				else if (keys.Contains("TWD_BUNDLE_BRUISER"))
				{
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation799");
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation299");
				}
				else if (keys.Contains("TWD_BUNDLE_FRESH_SURVIVOR"))
				{
					player.BundleManager.RotatingBundleManager.PurchasedRotations.Add("Rotation299");
				}
			}
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.JerryToken);
			if (player.WeeklyChallenge.PendingSkipTokens > 0 && player.WeeklyChallenge.PendingSkipTokensCollectedInChallengeId == 0)
			{
				player.WeeklyChallenge.PendingSkipTokensCollectedInChallengeId = player.WeeklyChallenge.Id;
			}
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			if (player.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
			{
				player.Blackboard.ClearToggle("Toggle.ToggleUpdateInfoPopupShown");
			}
			return true;
		}
	}
}
