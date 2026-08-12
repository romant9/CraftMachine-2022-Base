using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration200 : TWDModelMigration
	{
		public Migration200()
		{
			base.Version = "2.0.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			ModelList<SurvivorModel> survivors = player.SurvivorContainer.Survivors;
			for (int i = 0; i < (survivors?.Count ?? 0); i++)
			{
				SurvivorModel survivor = survivors[i];
				MigrateSurviorUpgradeTraits(survivor);
			}
			if (player.AttackOutpostVisitLog != null && player.AttackOutpostVisitLog.Count > 0)
			{
				for (int j = 0; j < player.AttackOutpostVisitLog.Count; j++)
				{
					OutpostVisitEntry outpostVisitEntry = player.AttackOutpostVisitLog[j];
					outpostVisitEntry.SurvivorRarityLevels = new int[3] { 1, 1, 1 };
					outpostVisitEntry.OtherSurvivorRarityLevels = new int[3] { 1, 1, 1 };
				}
			}
			if (player.DefenseOutpostVisitLog != null && player.DefenseOutpostVisitLog.Count > 0)
			{
				for (int k = 0; k < player.DefenseOutpostVisitLog.Count; k++)
				{
					OutpostVisitEntry outpostVisitEntry2 = player.DefenseOutpostVisitLog[k];
					outpostVisitEntry2.SurvivorRarityLevels = new int[3] { 1, 1, 1 };
					outpostVisitEntry2.OtherSurvivorRarityLevels = new int[3] { 1, 1, 1 };
				}
			}
			MigrateRarityAndTraitsOnItems(player.Equipment.RangeWeapons);
			MigrateRarityAndTraitsOnItems(player.Equipment.MeleeWeapons);
			MigrateRarityAndTraitsOnItems(player.Equipment.Armors);
			ModelList<LootEntry> pendingTradeCrates = player.LootManager.PendingTradeCrates;
			MigrateLootEntriesRarity(pendingTradeCrates);
			ModelList<LootEntry> loots = player.LootManager.Loots;
			MigrateLootEntriesRarity(loots);
			ModelList<LootEntry> lootBoxesToOpen = player.LootBoxesToOpen;
			MigrateLootEntriesRarity(lootBoxesToOpen);
			MigrateLootEntriesRarity(player.PhoneCall.LootsList);
			List<LootEntry> pendingGuildGiftsLootToOpen = player.PendingGuildGiftsLootToOpen;
			MigrateLootEntriesRarity(pendingGuildGiftsLootToOpen);
			if (player.SurvivorContainer.DeadSurvivors != null && player.SurvivorContainer.DeadSurvivors.Count > 0)
			{
				for (int l = 0; l < player.SurvivorContainer.DeadSurvivors.Count; l++)
				{
					DeadSurvivorModel deadSurvivorModel = player.SurvivorContainer.DeadSurvivors[l];
					if (deadSurvivorModel != null && deadSurvivorModel.SurvivorModel != null)
					{
						MigrateSurviorUpgradeTraits(deadSurvivorModel.SurvivorModel);
					}
				}
			}
			if (player.LastOpenedGuildGiftLoot != null)
			{
				player.LastOpenedGuildGiftLoot.RewardedRarityLevel = (int)player.LastOpenedGuildGiftLoot.RewardedRarity;
			}
			if (player.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
			{
				player.Blackboard.ClearToggle("Toggle.ToggleUpdateInfoPopupShown");
			}
			Migration220.MigrateQuestData(player, manager);
			MigrateReceivedDarylTokensAndWeapon(player);
			return true;
		}

		private void MigrateSurviorUpgradeTraits(SurvivorModel survivor)
		{
			survivor.StartingRarityLevel = (int)survivor.StartingRarity;
			survivor.SurvivorRarityLevel = (int)survivor.SurvivorRarity;
			for (int i = 0; i < ((survivor.UpgradeTraits != null) ? survivor.UpgradeTraits.Count : 0); i++)
			{
				UpgradeTraitsData traitData = survivor.UpgradeTraits[i];
				MigrateUpgradeTraitsData(traitData);
			}
			for (int j = 0; j < ((survivor.EquipmentItems != null) ? survivor.EquipmentItems.Count : 0); j++)
			{
				EquipmentItemModel item = survivor.EquipmentItems[j];
				MigrateRarityAndTraitsOnItem(item);
			}
			for (int k = 0; k < ((survivor.TraitContainer.Traits != null) ? survivor.TraitContainer.Traits.Count : 0); k++)
			{
				TraitEntry traitEntry = survivor.TraitContainer.Traits[k];
				string newTraitId = GetNewTraitId(traitEntry.TraitIdentifier);
				traitEntry.TraitIdentifier = newTraitId;
			}
		}

		private void MigrateRarityAndTraitsOnItems(IList<EquipmentItemModel> items)
		{
			for (int i = 0; i < (items?.Count ?? 0); i++)
			{
				MigrateRarityAndTraitsOnItem(items[i]);
			}
		}

		private void MigrateRarityAndTraitsOnItem(EquipmentItemModel item)
		{
			for (int i = 0; i < ((item.UpgradeTraits != null) ? item.UpgradeTraits.Count : 0); i++)
			{
				UpgradeTraitsData traitData = item.UpgradeTraits[i];
				MigrateUpgradeTraitsData(traitData);
			}
			if (item.ChargeEquipment != null)
			{
				MigrateRarityAndTraitsOnItem(item.ChargeEquipment);
			}
			item.RarityLevel = (int)item.Rarity;
		}

		private void MigrateLootEntriesRarity(IList<LootEntry> lootEntries)
		{
			for (int i = 0; i < (lootEntries?.Count ?? 0); i++)
			{
				LootEntry lootEntry = lootEntries[i];
				if (lootEntry != null)
				{
					lootEntry.RewardedRarityLevel = (int)lootEntry.RewardedRarity;
					if (lootEntry.GeneratedEquipment != null)
					{
						lootEntry.GeneratedEquipment.RarityLevel = (int)lootEntry.GeneratedEquipment.Rarity;
						MigrateRarityAndTraitsOnItem(lootEntry.GeneratedEquipment);
					}
					if (lootEntry.GeneratedSurvivor != null)
					{
						MigrateSurviorUpgradeTraits(lootEntry.GeneratedSurvivor);
					}
				}
			}
		}

		private void MigrateUpgradeTraitsData(UpgradeTraitsData traitData)
		{
			if (traitData.BucketType == TraitBucketsDefinition.BucketType.Locked)
			{
				traitData.IsLocked = true;
			}
			else if (traitData.BucketType == TraitBucketsDefinition.BucketType.Tactical)
			{
				traitData.IsTactical = true;
			}
			else if (traitData.BucketType == TraitBucketsDefinition.BucketType.LowLevel)
			{
				traitData.RarityLevel = 0;
			}
			else if (traitData.BucketType == TraitBucketsDefinition.BucketType.MidLevel)
			{
				traitData.RarityLevel = 1;
			}
			else if (traitData.BucketType == TraitBucketsDefinition.BucketType.HighLevel)
			{
				traitData.RarityLevel = 2;
			}
			else if (traitData.BucketType == TraitBucketsDefinition.BucketType.Epic)
			{
				traitData.RarityLevel = 3;
			}
			else if (traitData.BucketType == TraitBucketsDefinition.BucketType.Legendary)
			{
				traitData.RarityLevel = 4;
			}
			string newTraitId = GetNewTraitId(traitData.Identifier);
			traitData.Identifier = newTraitId;
		}

		private string GetNewTraitId(string traitIdentifier)
		{
			return traitIdentifier.Replace(".LowLevel", ".Level0").Replace(".MidLevel", ".Level1").Replace(".HighLevel", ".Level2")
				.Replace(".Epic", ".Level3")
				.Replace(".Legendary", ".Level4");
		}

		private void MigrateReceivedDarylTokensAndWeapon(PlayerModel playerModel)
		{
			MapContainerModel mapContainerModel = playerModel.MapContainerModel;
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = mapContainerModel.GetMissionGroupModelForSpawnPointGroup(playerModel.gameEconomyData.GetOutpostTutorialSpawnPointGroup());
			for (int i = 0; i < mapContainerModel.MapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = mapContainerModel.MapMissionGroups[i];
				if (mapMissionGroupModel == null || mapMissionGroupModel.IsLocked || mapMissionGroupModel.IsWeeklyChallenge || mapMissionGroupModel.IsInApocalyptiWeeklyChallenge || mapMissionGroupModel == missionGroupModelForSpawnPointGroup)
				{
					continue;
				}
				for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[j];
					if (mapMissionModel == null || !mapMissionModel.IsCompleted || mapMissionModel.MissionData == null)
					{
						continue;
					}
					MissionRewards missionRewardsData = playerModel.gameEconomyData.GetMissionRewardsData(mapMissionModel.MissionData.DisplayTextID);
					if (missionRewardsData == null)
					{
						continue;
					}
					string rewardString = null;
					if (mapMissionModel.MissionSpawnPointGroup != null)
					{
						if (mapMissionModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 1)
						{
							rewardString = missionRewardsData.Reward;
						}
						else if (mapMissionModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 2)
						{
							rewardString = missionRewardsData.RewardLvl2;
						}
						else if (mapMissionModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 3)
						{
							rewardString = missionRewardsData.RewardLvl3;
						}
					}
					GiveDarylTokenOrCrossbowReward(playerModel, rewardString);
				}
			}
			if (playerModel.SurvivorContainer == null || (playerModel.SurvivorContainer.StoryTeller == null && playerModel.SurvivorContainer.StoryTeller2 == null))
			{
				return;
			}
			StoryTellerModel storyTellerModel = ((playerModel.SurvivorContainer.StoryTeller != null) ? playerModel.SurvivorContainer.StoryTeller : playerModel.SurvivorContainer.StoryTeller2);
			QuestDefinition questDefinition = null;
			if (storyTellerModel != null)
			{
				int num = -1;
				num = ((storyTellerModel.CurrentQuest == null) ? storyTellerModel.QuestIndex : (storyTellerModel.CurrentQuest.HasCompleted ? storyTellerModel.CurrentQuest.QuestDefinition.Order : (storyTellerModel.CurrentQuest.QuestDefinition.Order - 1)));
				num = Math.Min(num, playerModel.manager.GameEconomyData.GetHighestQuestOrder());
				if (num > -1)
				{
					questDefinition = playerModel.manager.GameEconomyData.GetQuestDefinition(num, 1);
				}
			}
			if (questDefinition == null)
			{
				return;
			}
			for (int k = 0; k <= questDefinition.Order; k++)
			{
				QuestDefinition questDefinition2 = playerModel.manager.GameEconomyData.GetQuestDefinition(k, 1);
				if (questDefinition2 != null && questDefinition2.IsAvailable)
				{
					GiveDarylTokenOrCrossbowReward(playerModel, questDefinition2.Rewards);
				}
			}
		}

		private void GiveDarylTokenOrCrossbowReward(PlayerModel playerModel, string rewardString)
		{
			if (playerModel == null || string.IsNullOrEmpty(rewardString))
			{
				return;
			}
			string[] array = rewardString.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					continue;
				}
				string[] array2 = array[i].Split('(');
				string text = array2[0].ToLowerInvariant();
				array2[1] = array2[1].Replace(")", "");
				if (text == CurrencyType.DarylToken.ToString().ToLowerInvariant())
				{
					int num = int.Parse(array2[1]);
					if (num > 0)
					{
						playerModel.manager.Debug.Log("Giving daryl tokens from quest or mission: " + num);
						playerModel.GetCurrency(CurrencyType.DarylToken).Add(num);
					}
				}
				else
				{
					if (!(text == "equip"))
					{
						continue;
					}
					string text2 = array2[1].Split(',')[0].ToLowerInvariant();
					if (!string.IsNullOrEmpty(playerModel.gameEconomyData.ConfigData.RewardedCrossbowForMigrationID) && text2.ToLowerInvariant().Equals(playerModel.gameEconomyData.ConfigData.RewardedCrossbowForMigrationID.ToLowerInvariant()))
					{
						playerModel.Blackboard.SetToggle("Toggle.PendingCrossbowToBeGiven");
						if (playerModel.SurvivorContainer != null)
						{
							playerModel.SurvivorContainer.PendingCrossbowToGiveRewardsString = array[i];
						}
						playerModel.manager.Debug.Log("Marking crossbow to be given once daryl is created");
					}
				}
			}
		}
	}
}
