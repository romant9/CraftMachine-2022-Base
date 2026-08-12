using System.Collections.Generic;

namespace TWDModel
{
	public class Migration31600 : TWDModelMigration
	{
		public Migration31600()
		{
			base.Version = "3.16.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			ClearIDFAABTestFromData(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.YumikoToken);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BethToken);
			MigrationUtils.MigrateLeaderTrait(manager, "hero_negan", "LeaderBuffSurvivalInstinct");
			player.Blackboard.ClearToggle("HasSeenWhatsNewInGuildWars");
			return true;
		}

		private void ClearIDFAABTestFromData(PlayerModel player)
		{
			if (player.Blackboard == null)
			{
				return;
			}
			string key = "Toggle.ToggleIDFAAccepted";
			string key2 = "Toggle.ToggleIDFAIOSNativePaused";
			player.Blackboard.ToggleValues.Remove(key);
			player.Blackboard.ToggleValues.Remove(key2);
			string value = "IDFA.Group.";
			List<string> list = new List<string>();
			foreach (string key3 in player.Blackboard.UnlockValues.Keys)
			{
				if (key3.Contains(value))
				{
					list.Add(key3);
				}
			}
			foreach (string item in list)
			{
				player.Blackboard.UnlockValues.Remove(item);
			}
			list.Clear();
			foreach (string key4 in player.Blackboard.CounterValues.Keys)
			{
				if (key4.Contains(value))
				{
					list.Add(key4);
				}
			}
			foreach (string item2 in list)
			{
				player.Blackboard.CounterValues.Remove(item2);
			}
		}
	}
}
