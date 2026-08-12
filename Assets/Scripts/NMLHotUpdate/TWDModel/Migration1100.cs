using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration1100 : TWDModelMigration
	{
		public Migration1100()
		{
			base.Version = "1.10.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			for (int i = 0; i < player.SurvivorContainer.Survivors.Count; i++)
			{
				SurvivorModel survivorModel = player.SurvivorContainer.Survivors[i];
				for (int j = 0; j < survivorModel.EquipmentItems.Count; j++)
				{
					survivorModel.EquipmentItems[j].SetManager(manager);
				}
			}
			MigrateRemovedAchievement(player, manager, "CompleteDeadlyMissions_1");
			MigrateRemovedAchievement(player, manager, "CompleteDeadlyMissions_2");
			MigrateRemovedAchievement(player, manager, "CompleteDeadlyMissions_3");
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CarolToken, CurrencyType.RickToken, CurrencyType.AbrahamToken, CurrencyType.NeganToken, CurrencyType.MichonneToken, CurrencyType.MorganToken, CurrencyType.MaggieToken, CurrencyType.JesusToken, CurrencyType.GlennToken, CurrencyType.DarylToken, CurrencyType.AssaultToken, CurrencyType.ScoutToken, CurrencyType.BruiserToken, CurrencyType.WarriorToken, CurrencyType.ShooterToken, CurrencyType.HunterToken, CurrencyType.CarlToken);
			ModelList<SurvivorModel> survivors = player.SurvivorContainer.Survivors;
			ModelRandom playerRandom = player.PlayerRandom;
			for (int k = 0; k < (survivors?.Count ?? 0); k++)
			{
				SurvivorModel survivorModel2 = survivors[k];
				playerRandom.Next();
				survivorModel2.TraitRandom = new ModelRandom(playerRandom.State);
				survivorModel2.StartingRarity = survivorModel2.SurvivorRarity;
				survivorModel2.GetTraits()?.Clear();
				List<UpgradeTraitsData> list = new List<UpgradeTraitsData>(survivorModel2.UpgradeTraits);
				List<UpgradeTraitsData> list2 = new List<UpgradeTraitsData>();
				for (int l = 0; l < list.Count; l++)
				{
					UpgradeTraitsData upgradeTraitsData = list[l];
					if (upgradeTraitsData.BucketType == TraitBucketsDefinition.BucketType.Tactical)
					{
						list2.Insert(0, upgradeTraitsData);
					}
					else if (upgradeTraitsData.BucketType == TraitBucketsDefinition.BucketType.MidLevel)
					{
						upgradeTraitsData.Identifier = SurvivorModel.GetUpgradedTraitIdentifier(upgradeTraitsData);
						upgradeTraitsData.BucketType = TraitBucketsDefinition.BucketType.HighLevel;
						list2.Add(upgradeTraitsData);
					}
					else if (upgradeTraitsData.BucketType == TraitBucketsDefinition.BucketType.HighLevel)
					{
						if (survivorModel2.SurvivorRarity == Rarity.Epic)
						{
							upgradeTraitsData.Identifier = SurvivorModel.GetUpgradedTraitIdentifier(upgradeTraitsData);
							upgradeTraitsData.BucketType = TraitBucketsDefinition.BucketType.Epic;
						}
						else
						{
							upgradeTraitsData.Identifier = SurvivorModel.GetUpgradedTraitIdentifier(upgradeTraitsData);
							upgradeTraitsData.BucketType = TraitBucketsDefinition.BucketType.Legendary;
						}
						list2.Add(upgradeTraitsData);
					}
					else
					{
						list2.Add(upgradeTraitsData);
					}
				}
				int num = manager.GameEconomyData.GetTotalInitialTraitCountForSurvivorRarityOnlyForBackwardCompatibility((int)survivorModel2.SurvivorRarity) - (list2.Count - 1);
				if (num < 0)
				{
					manager.Debug.LogError("1.10.0 Migration Error: Survivor [" + survivorModel2.ToString() + "] had too many traits! THIS SHOULD NOT HAPPEN!");
					return false;
				}
				TraitBucketsDefinition.BucketType lowestTraitLevelForSurvivorRarityForBackwardCompatibility = manager.GameEconomyData.GetLowestTraitLevelForSurvivorRarityForBackwardCompatibility((int)survivorModel2.SurvivorRarity);
				for (int m = 0; m < num; m++)
				{
					UpgradeTraitsData upgradeTraitsData2 = survivorModel2.GiveRandomUpgradeTraitForBackwardCompatibility(lowestTraitLevelForSurvivorRarityForBackwardCompatibility, survivorModel2.TraitRandom);
					if (manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData2.Identifier) == null)
					{
						manager.Debug.LogError("1.10.0 Migration Error: Survivor [" + survivorModel2.ToString() + "] generated an invalid new trait with identifier ['" + upgradeTraitsData2.Identifier + "']");
						return false;
					}
					list2.Add(upgradeTraitsData2);
				}
				if (list2.Count < survivorModel2.UpgradeTraits.Count)
				{
					manager.Debug.LogError("1.10.0 Migration Error: Survivor [" + survivorModel2.ToString() + "] has less traits in the end of migration! Count in the beginning: " + survivorModel2.UpgradeTraits.Count + " and in the end: " + list2.Count);
					return false;
				}
				survivorModel2.UpgradeTraits = list2;
				survivorModel2.UpgradeTraits.StableSort((UpgradeTraitsData a, UpgradeTraitsData b) => (a.BucketType < b.BucketType && a.BucketType != TraitBucketsDefinition.BucketType.Tactical) ? 1 : (-1));
				List<EquipmentItemModel> list3 = new List<EquipmentItemModel>(survivorModel2.EquipmentItems);
				for (int num2 = 0; num2 < list3.Count; num2++)
				{
					EquipmentItemModel equipmentItemModel = list3[num2];
					if (equipmentItemModel.manager != null)
					{
						survivorModel2.Unequip(equipmentItemModel);
						survivorModel2.Equip(equipmentItemModel);
						continue;
					}
					survivorModel2.EquipmentItems.Remove(equipmentItemModel);
					if (equipmentItemModel.Owner == survivorModel2)
					{
						equipmentItemModel.Owner = null;
					}
				}
			}
			TutorialModel tutorial = player.Tutorial;
			TutorialPartDefinition currentPartDefinition = tutorial.CurrentPartDefinition;
			if (currentPartDefinition != null && currentPartDefinition.Id == "InitialCombat")
			{
				tutorial.CurrentStep = 0;
			}
			else if (currentPartDefinition != null && currentPartDefinition.Id == "Phone")
			{
				if (tutorial.CurrentStep == 2 || tutorial.CurrentStep == 3)
				{
					tutorial.CurrentStep = 1;
				}
				else if (tutorial.CurrentStep > 3)
				{
					tutorial.CurrentStep--;
				}
			}
			return true;
		}

		private void MigrateRemovedAchievement(PlayerModel player, TWDModelManager manager, string name)
		{
			AchievementDefinition achievementDefinition = manager.GameEconomyData.GetAchievementDefinition(name);
			int deadlyMissionsCompleted = player.MissionStatistics.DeadlyMissionsCompleted;
			if (!player.Blackboard.IsToggleOn(achievementDefinition.BlackboardCompletedKey) && deadlyMissionsCompleted > 0)
			{
				int result = 0;
				if (int.TryParse(achievementDefinition.Params, out result))
				{
					FixedPoint fixedPoint = (FixedPoint)Math.Min(deadlyMissionsCompleted, result) / (FixedPoint)result;
					Rewards rewards = new Rewards(achievementDefinition.Reward, manager);
					rewards.MultiplyCurrencies(fixedPoint);
					IModelDebug debug = manager.Debug;
					string[] obj = new string[6] { "Migrating removed achievement ", achievementDefinition.ID, " completed:", null, null, null };
					FixedPoint fixedPoint2 = fixedPoint;
					obj[3] = fixedPoint2.ToString();
					obj[4] = " reward:";
					obj[5] = rewards?.ToString();
					debug.Log(string.Concat(obj));
					rewards.Give(manager);
					if (player.MigratedAchievementRewards == null)
					{
						player.MigratedAchievementRewards = new Rewards();
						player.MigratedAchievementRewards.Add(rewards);
					}
					else
					{
						player.MigratedAchievementRewards.MergeCurrencies(rewards);
					}
					player.Blackboard.SetToggle(achievementDefinition.BlackboardCompletedKey);
					player.Blackboard.SetToggle(achievementDefinition.BlackboardRewardClaimedKey);
				}
				else
				{
					manager.Debug.LogWarning("Could not migrate removed achievement " + name);
				}
			}
			player.Blackboard.SetToggle(achievementDefinition.BlackboardCounterKey + ".Hotfixed");
		}
	}
}
