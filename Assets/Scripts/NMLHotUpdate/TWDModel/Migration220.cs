using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class Migration220 : TWDModelMigration
	{
		public Migration220()
		{
			base.Version = "2.2.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.TaraToken, CurrencyType.RositaToken);
			MigrateQuestData(player, manager);
			MigrateExistingHeroes(player, manager);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			MigrateEmptyMissionGroups(player, manager);
			player.MapContainerModel.SpawnSeasonEpisodes();
			MigrateEmptyHarderMission(player, manager);
			return true;
		}

		public static void MigrateEmptyHarderMission(PlayerModel player, TWDModelManager manager)
		{
			for (int i = 0; i < player.MapContainerModel.MapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = player.MapContainerModel.MapMissionGroups[i];
				if (mapMissionGroupModel.MissionSpawnPointGroup != null && mapMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Story && mapMissionGroupModel.AreAllStoryMissionsCompleted())
				{
					MapMissionGroupModel harderVersion = player.MapContainerModel.GetHarderVersion(mapMissionGroupModel);
					if (harderVersion != null && (harderVersion.Missions == null || harderVersion.Missions.Count == 0))
					{
						manager.Debug.Log("Migration added missions to " + harderVersion.MissionSpawnPointGroup.DisplayName + " difficulty " + harderVersion.MissionSpawnPointGroup.EpisodeDifficultyLevel);
						player.MapContainerModel.SpawnMissionsForGroup(harderVersion.MissionSpawnPointGroup);
					}
				}
			}
		}

		public static void MigrateEmptyMissionGroups(PlayerModel player, TWDModelManager manager)
		{
			List<MapMissionGroupModel> list = new List<MapMissionGroupModel>();
			for (int i = 0; i < player.MapContainerModel.MapMissionGroups.Count; i++)
			{
				if (manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(player.MapContainerModel.MapMissionGroups[i].MissionSpawnPointGroupId) == null && player.MapContainerModel.MapMissionGroups[i].Missions.Count == 0)
				{
					list.Add(player.MapContainerModel.MapMissionGroups[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				manager.Debug.Log("Migrate removed empty spawnpoint group " + list[j].MissionSpawnPointGroupId);
				player.MapContainerModel.MapMissionGroups.Remove(list[j]);
			}
		}

		public static void MigrateQuestData(PlayerModel player, TWDModelManager manager)
		{
			if (player.SurvivorContainer.StoryTeller != null && player.SurvivorContainer.StoryTeller.CurrentQuest is MissionQuest missionQuest && string.IsNullOrEmpty(missionQuest.MapId))
			{
				MissionSpawnPointGroup spawnPointGroup = manager.GameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(missionQuest.SpawnPointGroupId);
				if (spawnPointGroup != null)
				{
					missionQuest.MapId = spawnPointGroup.MapId;
					manager.Debug.Log("Migrated quest map to " + missionQuest.MapId);
				}
			}
		}

		private void MigrateExistingHeroes(PlayerModel player, TWDModelManager manager)
		{
			foreach (SurvivorModel item in player.SurvivorContainer.Survivors.Where((SurvivorModel i) => i.IsHero).ToList())
			{
				ActorDefinition definition = item.Definition;
				if (definition == null)
				{
					continue;
				}
				int num = item.TokensSpent;
				item.TokensSpent = 0;
				int survivorRarityLevel = item.SurvivorRarityLevel;
				item.StartingRarityLevel = definition.RarityLevel;
				item.SurvivorRarityLevel = definition.RarityLevel;
				item.SetManager(manager);
				for (int num2 = 0; num2 < item.UpgradeTraits.Count; num2++)
				{
					UpgradeTraitsData upgradeTraitsData = item.UpgradeTraits[num2];
					item.TraitContainer.RemoveTrait(upgradeTraitsData.Identifier);
				}
				item.InitUpgradeTraits();
				CurrencyType survivorTraitUpgradeCurrencyType = SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(item);
				if (survivorTraitUpgradeCurrencyType == CurrencyType.None)
				{
					continue;
				}
				Cashier upgradeTraitCashier = item.GetUpgradeTraitCashier();
				while (upgradeTraitCashier != null && upgradeTraitCashier.GetTotalCost(survivorTraitUpgradeCurrencyType) <= num)
				{
					if (item.CanUpgradeSurvivorRarity())
					{
						item.UpgradeSurvivorRarity(doNotInstantiateTrait: true);
					}
					else
					{
						item.UpgradeLowestLevelTrait(doNotInstantiateTrait: true);
					}
					item.TokensSpent += upgradeTraitCashier.GetTotalCost(survivorTraitUpgradeCurrencyType);
					num -= upgradeTraitCashier.GetTotalCost(survivorTraitUpgradeCurrencyType);
					upgradeTraitCashier = item.GetUpgradeTraitCashier();
				}
				if (num > 0)
				{
					player.GetCurrency(survivorTraitUpgradeCurrencyType)?.Add(num);
				}
				manager.Debug.Log("Migrated hero: " + definition.ID + ". Spent tokens amount: " + item.TokensSpent + " . Added remaining tokens: " + num + ". Previous rarity: " + survivorRarityLevel + ". New Rarity: " + item.SurvivorRarityLevel);
			}
		}
	}
}
