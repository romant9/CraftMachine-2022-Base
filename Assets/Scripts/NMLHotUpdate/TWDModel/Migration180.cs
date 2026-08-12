using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration180 : TWDModelMigration
	{
		public Migration180()
		{
			base.Version = "1.8.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player != null && player.BundleManager != null)
			{
				if (player.BundleManager.ViewEquipment != null)
				{
					if (player.BundleManager.PendingViewEquipments == null)
					{
						player.BundleManager.PendingViewEquipments = new ModelList<EquipmentItemModel>();
					}
					player.BundleManager.PendingViewEquipments.Add(player.BundleManager.ViewEquipment);
					player.BundleManager.ViewEquipment = null;
				}
				if (player.BundleManager.ViewSurvivor != null)
				{
					if (player.BundleManager.PendingViewSurvivors == null)
					{
						player.BundleManager.PendingViewSurvivors = new ModelList<SurvivorModel>();
					}
					player.BundleManager.PendingViewSurvivors.Add(player.BundleManager.ViewSurvivor);
					player.BundleManager.ViewSurvivor = null;
				}
				if (!string.IsNullOrEmpty(player.BundleManager.ViewOutfit))
				{
					if (player.BundleManager.PendingViewOutfits == null)
					{
						player.BundleManager.PendingViewOutfits = new List<string>();
					}
					player.BundleManager.PendingViewOutfits.Add(player.BundleManager.ViewOutfit);
					player.BundleManager.ViewOutfit = null;
				}
			}
			if (player != null)
			{
				player.PreviousOutpostSeasonId = -1;
				player.CurrentOutpostSeasonId = ((!player.HasValidOutpost) ? (-1) : 0);
				DateTime dateTime = DateTime.UtcNow.AddMilliseconds(-manager.Time);
				long value = (long)(player.Created - dateTime).TotalSeconds;
				if (Math.Abs(value) > 60)
				{
					manager.Debug.LogWarning("Fixing invalid player creation time, difference " + value + " seconds.");
					player.Created = dateTime;
					manager.SetModelHotfixApplied();
				}
				if (player.LifeTime > manager.Time)
				{
					manager.Debug.LogWarning("Fixing invalid player lifetime, in the future by " + (player.LifeTime - manager.Time) + "seconds.");
					player.ResetLifetime(manager.Time);
					manager.SetModelHotfixApplied();
				}
				DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				long val = (long)(player.Created.ToUniversalTime() - dateTime2).TotalSeconds * 1000 + manager.Time;
				manager.Debug.Log("Constraining outpost timestamps to " + val);
				player.LastPvPAttackCompletionUtcTime = Math.Min(player.LastPvPAttackCompletionUtcTime, val);
				if (player.DefenseOutpostVisitLog != null)
				{
					for (int i = 0; i < player.DefenseOutpostVisitLog.Count; i++)
					{
						player.DefenseOutpostVisitLog[i].UtcTime = Math.Min(player.DefenseOutpostVisitLog[i].UtcTime, val);
					}
				}
				if (player.AttackOutpostVisitLog != null)
				{
					for (int j = 0; j < player.AttackOutpostVisitLog.Count; j++)
					{
						player.AttackOutpostVisitLog[j].UtcTime = Math.Min(player.AttackOutpostVisitLog[j].UtcTime, val);
					}
				}
			}
			return true;
		}
	}
}
