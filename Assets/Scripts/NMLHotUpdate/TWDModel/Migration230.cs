using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration230 : TWDModelMigration
	{
		public Migration230()
		{
			base.Version = "2.3.0";
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
				if (!(survivorModel.ActorDefinitionID.ToLower() == "hero_rick"))
				{
					continue;
				}
				ActorDefinition actorDefinition = manager.GameEconomyData.GetActorDefinition(survivorModel.ActorDefinitionID);
				if (survivorModel.UpgradeTraits != null && survivorModel.UpgradeTraits.Count > 1)
				{
					string text = null;
					for (int j = 0; j < ((survivorModel.TraitContainer.Traits != null) ? survivorModel.TraitContainer.Traits.Count : 0); j++)
					{
						TraitEntry traitEntry = survivorModel.TraitContainer.Traits[j];
						if (UpgradeTraitsData.StripTraitLevelIdentifier(traitEntry.TraitIdentifier) == "LeaderBuffCoverDamageReduction")
						{
							text = traitEntry.TraitIdentifier;
							break;
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						survivorModel.TraitContainer.RemoveTrait(text);
					}
					if (UpgradeTraitsData.StripTraitLevelIdentifier(survivorModel.UpgradeTraits[1].Identifier) == "LeaderBuffCoverDamageReduction" && actorDefinition != null && actorDefinition.UpgradeTraits != null && actorDefinition.UpgradeTraits.Count > 0)
					{
						string text2 = UpgradeTraitsData.CompileUpgradeTraitIdentifier(actorDefinition.UpgradeTraits[0], survivorModel.UpgradeTraits[1].RarityLevel, isLocked: false);
						TraitDefinition traitDefinition = manager.GameEconomyData.GetTraitDefinition(text2);
						if (traitDefinition != null)
						{
							survivorModel.UpgradeTraits[1].Identifier = traitDefinition.Identifier;
							break;
						}
						manager.Debug.LogError("Cannot migrate hero_rick leader trait, no trait found in game economy data for player=" + player.HashedId + " with trait id=" + text2);
					}
				}
				else
				{
					manager.Debug.LogError("Cannot migrate hero_rick leader trait, incorrect amount of traits for player=" + player.HashedId);
				}
			}
			for (int k = 0; k < player.SurvivorContainer.Survivors.Count; k++)
			{
				SurvivorModel survivorModel2 = player.SurvivorContainer.Survivors[k];
				survivorModel2.TraitContainer.RemoveTrait("PVP_RaiderVsSurvivorDamageResistance");
				survivorModel2.TraitContainer.RemoveTrait("PVP_SurvivorVsRaiderDamageResistance");
			}
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.TalkingDeadToken);
			RecalculatePlayerXp(player);
			RemoveInvalidMapMissionsAndGroups(player, manager);
			manager.SetModelHotfixApplied();
			return true;
		}

		private void RemoveInvalidMapMissionsAndGroups(PlayerModel player, TWDModelManager manager)
		{
			ModelList<MapMissionGroupModel> mapMissionGroups = player.MapContainerModel.MapMissionGroups;
			List<MapMissionGroupModel> list = new List<MapMissionGroupModel>();
			for (int i = 0; i < mapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = mapMissionGroups[i];
				if (mapMissionGroupModel == null)
				{
					manager.Debug.LogWarning("Null group encountered");
					continue;
				}
				MissionSpawnPointGroup missionSpawnPointGroup = mapMissionGroupModel.MissionSpawnPointGroup;
				if (missionSpawnPointGroup == null)
				{
					manager.Debug.LogWarning("Removing MapMissionGroup " + mapMissionGroupModel.MissionSpawnPointGroupId);
					list.Add(mapMissionGroupModel);
					continue;
				}
				List<MapMissionModel> list2 = new List<MapMissionModel>();
				for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[j];
					if (mapMissionModel == null)
					{
						manager.Debug.LogWarning("Null mission encountered in map " + missionSpawnPointGroup.MapId);
					}
					if (missionSpawnPointGroup.GetSpawnPointByMissionId(mapMissionModel.MissionId) == null)
					{
						manager.Debug.LogWarning("Removing MapMission " + mapMissionModel.MissionId + " from map " + missionSpawnPointGroup.MapId);
						list2.Add(mapMissionModel);
					}
				}
				for (int k = 0; k < list2.Count; k++)
				{
					mapMissionGroupModel.Missions.Remove(list2[k]);
				}
			}
			for (int l = 0; l < list.Count; l++)
			{
				mapMissionGroups.Remove(list[l]);
			}
		}

		private void RecalculatePlayerXp(PlayerModel player)
		{
			int level = player.Level;
			int xp = player.Xp;
			int num = 0;
			for (int i = 0; i < player.Camp.Buildings.Count; i++)
			{
				BuildingModel buildingModel = player.Camp.Buildings[i];
				if (buildingModel == null || buildingModel.BuildingType == null || buildingModel.BuildingType.Category != BuildingCategory.Building)
				{
					continue;
				}
				int level2 = buildingModel.Level;
				for (int j = 1; j <= level2; j++)
				{
					BuildingUpgradeLevel buildingUpgradeLevel = player.gameEconomyData.GetBuildingUpgradeLevel(buildingModel.TypeName, j);
					if (buildingUpgradeLevel != null)
					{
						num += buildingUpgradeLevel.AwardedXp;
					}
				}
			}
			PlayerLevelData[] playerLevelData = player.gameEconomyData.PlayerLevelData;
			PlayerLevelData playerLevelData2 = playerLevelData[0];
			int num2 = 1;
			while (num >= playerLevelData2.NextLevelXp)
			{
				num -= playerLevelData2.NextLevelXp;
				if (num2 >= playerLevelData.Length)
				{
					break;
				}
				playerLevelData2 = playerLevelData[num2];
				num2++;
			}
			player.manager.Debug.Log("Previous Level/XP: " + level + "/" + xp + " New Level/XP: " + num2 + "/" + num);
			player.SetLevelAndXp(num2, num);
		}
	}
}
