using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	internal class SurvivalCombatHelper
	{
		public static SurvivalMissionConfig.SurvivalObjectiveType GetSurvivalMissionObjectiveType(CombatModel combat)
		{
			if (combat != null)
			{
				if (combat.manager.CustomSurvivalMissionObjectiveType != SurvivalMissionConfig.SurvivalObjectiveType.Invalid)
				{
					return combat.manager.CustomSurvivalMissionObjectiveType;
				}
				if (combat.SurvivalMission == null)
				{
					return SurvivalMissionConfig.SurvivalObjectiveType.Invalid;
				}
				return combat.SurvivalMission.ObjectiveType;
			}
			return SurvivalMissionConfig.SurvivalObjectiveType.Invalid;
		}

		private static void ApplyPersistentVariableInt(CombatModel combat, SurvivalSavedMissionModel savedData, bool readOnly, string varName, int defaultValue)
		{
			int value = defaultValue;
			if (savedData != null && savedData.DoesSavedMissionDataExist)
			{
				value = savedData.GetPersistentIntVariableValue(varName);
			}
			combat.PersistentMissionVariableManager.SetIntVariableCreatingIfNecessary(varName, value, readOnly);
		}

		private static void ApplyPersistentVariableString(CombatModel combat, SurvivalSavedMissionModel savedData, bool readOnly, string varName, string defaultValue)
		{
			string value = defaultValue;
			if (savedData != null && savedData.DoesSavedMissionDataExist)
			{
				value = savedData.GetPersistentStringVariableValue(varName);
			}
			combat.PersistentMissionVariableManager.SetStringVariableCreatingIfNecessary(varName, value, readOnly);
		}

		public static void ApplySurvivalMissionConfigToPersistentVariables(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			ApplyPersistentVariableInt(combat, savedData, readOnly: false, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalKillCountLeft), combat.SurvivalMission.KillsRequired);
			ApplyPersistentVariableInt(combat, savedData, readOnly: false, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalInteractiveDuration), combat.SurvivalMission.InteractiveDuration);
			ApplyPersistentVariableInt(combat, savedData, readOnly: false, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalSurviveDuration), combat.SurvivalMission.SurviveDuration);
			ApplyPersistentVariableInt(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalSpawnerPower), combat.SurvivalMission.SpawnerCount);
			ApplyPersistentVariableInt(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalMissionOrderInSection), combat.SurvivalMission.MissionOrderInSection);
			ApplyPersistentVariableInt(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalAttemptNumber), 0);
			ApplyPersistentVariableInt(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalBurningPile), combat.SurvivalMission.IsWalkerTypeBurning(WalkerType.WalkerNormal) ? 1 : 0);
			ApplyPersistentVariableInt(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalThreatFrequency), combat.SurvivalMission.ThreatFrequency);
			ApplyPersistentVariableInt(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalThreatStart), combat.SurvivalMission.ThreatStart);
			int defaultValue = (int)((combat.manager.Player.WeeklySurvival != null) ? combat.manager.Player.WeeklySurvival.CurrentDifficulty : SurvivalDifficulty.None);
			ApplyPersistentVariableInt(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalDifficulty), defaultValue);
			if (combat.IsGuildBattleMission && combat.SurvivalMission.ObjectiveType == SurvivalMissionConfig.SurvivalObjectiveType.KillAllRaiders)
			{
				ApplyPersistentVariableString(combat, savedData, readOnly: true, PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.GvGEnemyPlayerName), GuildBattleCombatHelper.GetEnemyPlayerName(combat));
			}
		}

		private static bool IsSpawnPointExplicitWalkerType(ActorSpawnPointModel spawnPoint, WalkerType walkerType)
		{
			if (!(spawnPoint is WalkerSpawnPointModel))
			{
				return false;
			}
			WalkerSpawnPointModel walkerSpawnPointModel = (WalkerSpawnPointModel)spawnPoint;
			if (!walkerSpawnPointModel.UseOverrideWalkerType || walkerSpawnPointModel.OverrideWalkerType != walkerType)
			{
				return false;
			}
			return true;
		}

		private static bool IsSpawnPointTaggedExplicitWalkerType(ActorSpawnPointModel spawnPoint, WalkerType walkerType, int actorTag)
		{
			if (!IsSpawnPointExplicitWalkerType(spawnPoint, walkerType))
			{
				return false;
			}
			if (spawnPoint.SpawnTag != actorTag)
			{
				return false;
			}
			return true;
		}

		private static bool IsSpawnPointNonExplicitWalkerType(ActorSpawnPointModel spawnPoint)
		{
			if (!(spawnPoint is WalkerSpawnPointModel))
			{
				return false;
			}
			if (((WalkerSpawnPointModel)spawnPoint).UseOverrideWalkerType)
			{
				return false;
			}
			return true;
		}

		private static bool IsSpawnPointCountedNonExplicitWalkerType(ActorSpawnPointModel spawnPoint)
		{
			if (!IsSpawnPointNonExplicitWalkerType(spawnPoint))
			{
				return false;
			}
			if (!SurvivalMissionConfig.IsCountedActorTag(spawnPoint.SpawnTag))
			{
				return false;
			}
			return true;
		}

		private static bool IsSpawnPointExplicitRaiderType(ActorSpawnPointModel spawnPoint, SurvivorClass raiderType)
		{
			if (!(spawnPoint is RaiderSpawnPointModel))
			{
				return false;
			}
			if (((RaiderSpawnPointModel)spawnPoint).Class != raiderType)
			{
				return false;
			}
			return true;
		}

		private static bool IsSpawnPointTaggedExplicitRaiderType(ActorSpawnPointModel spawnPoint, SurvivorClass raiderType, int actorTag)
		{
			if (!IsSpawnPointExplicitRaiderType(spawnPoint, raiderType))
			{
				return false;
			}
			if (spawnPoint.SpawnTag != actorTag)
			{
				return false;
			}
			return true;
		}

		private static bool IsSpawnPointNonExplicitRaiderType(ActorSpawnPointModel spawnPoint)
		{
			if (!(spawnPoint is RaiderSpawnPointModel))
			{
				return false;
			}
			if (((RaiderSpawnPointModel)spawnPoint).Class != SurvivorClass.None)
			{
				return false;
			}
			return true;
		}

		private static bool IsSpawnPointCountedNonExplicitRaiderType(ActorSpawnPointModel spawnPoint)
		{
			if (!IsSpawnPointNonExplicitRaiderType(spawnPoint))
			{
				return false;
			}
			if (!SurvivalMissionConfig.IsCountedActorTag(spawnPoint.SpawnTag))
			{
				return false;
			}
			return true;
		}

		private static int GetCountForTypeInSurvival(CombatModel combat, WalkerType walkerType, int actorTag, out bool wasRoundedDown)
		{
			int num = 0;
			int num2 = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[i];
				if (!IsSpawnPointExplicitWalkerType(actorSpawnPointModel, walkerType))
				{
					continue;
				}
				if (actorSpawnPointModel.SpawnTag == actorTag)
				{
					if (actorSpawnPointModel.TotalSpawnCount > 0)
					{
						num += actorSpawnPointModel.TotalSpawnCount;
					}
					else
					{
						combat.Debug.LogError("Encountered a counted survival actor spawn point model with non-positive TotalSpawnCount. Positive spawn count is required for these spawners.");
					}
				}
				else if (actorSpawnPointModel.SpawnTag != 0 && SurvivalMissionConfig.IsCountedActorTag(actorSpawnPointModel.SpawnTag))
				{
					num2 += actorSpawnPointModel.TotalSpawnCount;
				}
			}
			int num3 = num + num2;
			FixedPoint fixedPoint = 0L;
			if (num3 > 0)
			{
				fixedPoint = new FixedPoint(num) / new FixedPoint(num3);
			}
			int numWalkersOfType = combat.SurvivalMission.GetNumWalkersOfType(walkerType);
			FixedPoint fixedPoint2 = fixedPoint * new FixedPoint(numWalkersOfType);
			int result = (int)fixedPoint2;
			wasRoundedDown = FixedPoint.Floor(fixedPoint2) != fixedPoint2;
			if (numWalkersOfType > 0 && num3 == 0)
			{
				combat.Debug.LogError("Survival config specifies walker type with enum " + walkerType.ToString() + ", but there no matching spawners for that in the map.");
			}
			return result;
		}

		private static int GetCountForTypeInSurvival(CombatModel combat, SurvivorClass raiderType, int actorTag, out bool wasRoundedDown)
		{
			int num = 0;
			int num2 = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[i];
				if (!IsSpawnPointExplicitRaiderType(actorSpawnPointModel, raiderType))
				{
					continue;
				}
				if (actorSpawnPointModel.SpawnTag == actorTag)
				{
					if (actorSpawnPointModel.TotalSpawnCount > 0)
					{
						num += actorSpawnPointModel.TotalSpawnCount;
					}
					else
					{
						combat.Debug.LogError("Encountered a counted survival actor spawn point model with non-positive TotalSpawnCount. Positive spawn count is required for these spawners.");
					}
				}
				else if (actorSpawnPointModel.SpawnTag != 0 && SurvivalMissionConfig.IsCountedActorTag(actorSpawnPointModel.SpawnTag))
				{
					num2 += actorSpawnPointModel.TotalSpawnCount;
				}
			}
			int num3 = num + num2;
			FixedPoint fixedPoint = 0L;
			if (num3 > 0)
			{
				fixedPoint = new FixedPoint(num) / new FixedPoint(num3);
			}
			int numRaidersByType = combat.SurvivalMission.GetNumRaidersByType(raiderType);
			FixedPoint fixedPoint2 = fixedPoint * new FixedPoint(numRaidersByType);
			int result = (int)fixedPoint2;
			wasRoundedDown = FixedPoint.Floor(fixedPoint2) != fixedPoint2;
			if (numRaidersByType > 0 && num3 == 0)
			{
				combat.Debug.LogError("Survival config specifies raider type with enum " + raiderType.ToString() + ", but there no matching spawners for that in the map.");
			}
			return result;
		}

		private static int ClampCountUsingSavedData(SurvivalSavedMissionModel savedData, WalkerType walkerType, int actorTag, int spawnCount)
		{
			if (savedData != null && savedData.DoesSavedMissionDataExist)
			{
				return savedData.ClampSpawnCountForWalker(spawnCount, walkerType, actorTag);
			}
			return spawnCount;
		}

		private static int ClampCountUsingSavedData(SurvivalSavedMissionModel savedData, SurvivorClass raiderType, int actorTag, int spawnCount)
		{
			if (savedData != null && savedData.DoesSavedMissionDataExist)
			{
				return savedData.ClampSpawnCountForRaider(spawnCount, raiderType, actorTag);
			}
			return spawnCount;
		}

		private static int CountNumberOfWalkerSpawners(CombatModel combat, WalkerType walkerType, int actorTag)
		{
			int num = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				if (IsSpawnPointTaggedExplicitWalkerType((ActorSpawnPointModel)models[i], walkerType, actorTag))
				{
					num++;
				}
			}
			return num;
		}

		private static int CountNumberOfRaiderSpawners(CombatModel combat, SurvivorClass raiderType, int actorTag)
		{
			int num = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				if (IsSpawnPointTaggedExplicitRaiderType((ActorSpawnPointModel)models[i], raiderType, actorTag))
				{
					num++;
				}
			}
			return num;
		}

		private static void ReassignWalkerSpawnerSpawnCounts(CombatModel combat, WalkerType walkerType, int actorTag, int totalWalkerCount, bool makeBoss, bool makeBurning)
		{
			int num = CountNumberOfWalkerSpawners(combat, walkerType, actorTag);
			if (num == 0)
			{
				if (totalWalkerCount > 0)
				{
					combat.Debug.LogWarning("Total " + totalWalkerCount + " walkers left unspawned with type " + walkerType.ToString() + " and actor tag " + actorTag + " as no such spawners in map.");
				}
				return;
			}
			int num2 = totalWalkerCount / num;
			int num3 = totalWalkerCount - num2 * num;
			int[] array = new int[num];
			for (int i = 0; i < num3; i++)
			{
				array[i] = 1;
			}
			combat.manager.Player.PlayerRandom.ShuffleArray(array);
			int num4 = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int j = 0; j < models.Count; j++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[j];
				if (IsSpawnPointTaggedExplicitWalkerType(actorSpawnPointModel, walkerType, actorTag))
				{
					int num5 = num2;
					if (num4 < array.Length)
					{
						num5 += array[num4];
					}
					WalkerSpawnPointModel walkerSpawnPointModel = (WalkerSpawnPointModel)actorSpawnPointModel;
					walkerSpawnPointModel.SpawnCountPerAction = num5;
					walkerSpawnPointModel.TotalSpawnCount = num5;
					if (makeBoss)
					{
						walkerSpawnPointModel.IsBoss = true;
					}
					if (makeBurning)
					{
						walkerSpawnPointModel.AdditionalTraits.Add("Burning");
					}
					combat.Debug.LogDebug("Reassigned spawn point for walkers of type " + walkerType.ToString() + " and actor tag " + actorTag + " spawner count to " + num5);
					num4++;
				}
			}
		}

		private static bool ReassignWalkerSpawnerSpawnCountsGvG(CombatModel combat, WalkerType walkerType, int actorTag, int totalWalkerCount, bool makeBoss, bool makeBurning)
		{
			int num = CountNumberOfWalkerSpawners(combat, walkerType, actorTag);
			if (num == 0)
			{
				if (totalWalkerCount > 0)
				{
					combat.Debug.LogWarning("Total " + totalWalkerCount + " walkers left unspawned with type " + walkerType.ToString() + " and actor tag " + actorTag + " as no such spawners in map.");
				}
				return false;
			}
			int num2 = totalWalkerCount / num;
			int num3 = totalWalkerCount - num2 * num;
			int[] array = new int[num];
			for (int i = 0; i < num3; i++)
			{
				array[i] = 1;
			}
			combat.manager.Player.PlayerRandom.ShuffleArray(array);
			int num4 = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			List<WalkerSpawnPointModel> list = new List<WalkerSpawnPointModel>();
			for (int j = 0; j < models.Count; j++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[j];
				if (IsSpawnPointTaggedExplicitWalkerType(actorSpawnPointModel, walkerType, actorTag))
				{
					int num5 = num2;
					if (num4 < array.Length)
					{
						num5 += array[num4];
					}
					WalkerSpawnPointModel walkerSpawnPointModel = (WalkerSpawnPointModel)actorSpawnPointModel;
					walkerSpawnPointModel.SpawnCountPerAction = num5;
					walkerSpawnPointModel.TotalSpawnCount = num5;
					if (makeBurning)
					{
						walkerSpawnPointModel.AdditionalTraits.Add("Burning");
					}
					if (makeBoss && num5 > 0)
					{
						list.Add(walkerSpawnPointModel);
					}
					combat.Debug.LogDebug("Reassigned spawn point for walkers of type " + walkerType.ToString() + " and actor tag " + actorTag + " spawner count to " + num5);
					num4++;
				}
			}
			if (list.Count > 0)
			{
				int randomInRange = combat.manager.Player.PlayerRandom.GetRandomInRange(0, list.Count - 1);
				list[randomInRange].IsBoss = true;
			}
			return list.Count > 0;
		}

		private static void ReassignRaiderSpawnerSpawnCounts(CombatModel combat, SurvivorClass raiderType, int actorTag, int totalRaiderCount, bool makeBoss, bool makeBurning)
		{
			int num = CountNumberOfRaiderSpawners(combat, raiderType, actorTag);
			if (num == 0)
			{
				if (totalRaiderCount > 0)
				{
					combat.Debug.LogWarning("Total " + totalRaiderCount + " raiders left unspawned with type " + raiderType.ToString() + " and actor tag " + actorTag + " as no such spawners in map.");
				}
			}
			else
			{
				ReassignRaiderSpawnerSpawnCountsInternal(num, combat, raiderType, actorTag, totalRaiderCount, makeBoss, makeBurning, checkSurvivorPlayerModel: false);
			}
		}

		private static void ReassignRaiderSpawnerSpawnCountsInternal(int numMatchingSpawners, CombatModel combat, SurvivorClass raiderType, int actorTag, int totalCount, bool makeBoss, bool makeBurning, bool checkSurvivorPlayerModel)
		{
			int num = totalCount / numMatchingSpawners;
			int num2 = totalCount - num * numMatchingSpawners;
			int[] array = new int[numMatchingSpawners];
			for (int i = 0; i < num2; i++)
			{
				array[i] = 1;
			}
			combat.manager.Player.PlayerRandom.ShuffleArray(array);
			int num3 = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int j = 0; j < models.Count; j++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[j];
				if (IsSpawnPointTaggedExplicitRaiderType(actorSpawnPointModel, raiderType, actorTag))
				{
					int num4 = num;
					if (num3 < array.Length)
					{
						num4 += array[num3];
					}
					RaiderSpawnPointModel raiderSpawnPointModel = (RaiderSpawnPointModel)actorSpawnPointModel;
					raiderSpawnPointModel.SpawnCountInUse = true;
					raiderSpawnPointModel.SpawnCountPerAction = num4;
					raiderSpawnPointModel.TotalSpawnCount = num4;
					raiderSpawnPointModel.SpawnUsed = num4 > 0;
					if (makeBurning)
					{
						raiderSpawnPointModel.AdditionalTraits.Add("Burning");
					}
					combat.Debug.LogDebug("Reassigned spawn point for raides spawner count to " + num4);
					num3++;
				}
			}
		}

		private static WalkerType[] GetRandomizedSpecialWalkerList(CombatModel combat, int[] countsForWalkerTypes)
		{
			int num = 0;
			for (int i = 0; i < SurvivalMissionConfig.SupportedWalkerTypes.Length; i++)
			{
				if (SurvivalMissionConfig.SupportedWalkerTypes[i] != WalkerType.WalkerNormal)
				{
					num += countsForWalkerTypes[i];
				}
			}
			WalkerType[] array = new WalkerType[num];
			int num2 = 0;
			for (int j = 0; j < SurvivalMissionConfig.SupportedWalkerTypes.Length; j++)
			{
				WalkerType walkerType = SurvivalMissionConfig.SupportedWalkerTypes[j];
				if (walkerType != WalkerType.WalkerNormal)
				{
					for (int k = 0; k < countsForWalkerTypes[j]; k++)
					{
						array[num2] = walkerType;
						num2++;
					}
				}
			}
			ShuffleArrayDeterministic(array, GetHashCodeForSurvivalMission(combat));
			return array;
		}

		private static SurvivorClass[] GetRandomizedRaiderList(CombatModel combat, int[] countsForRaiderTypes)
		{
			int num = 0;
			for (int i = 0; i < SurvivalMissionConfig.SupportedRaiderTypes.Length; i++)
			{
				num += countsForRaiderTypes[i];
			}
			SurvivorClass[] array = new SurvivorClass[num];
			int num2 = 0;
			for (int j = 0; j < SurvivalMissionConfig.SupportedRaiderTypes.Length; j++)
			{
				SurvivorClass survivorClass = SurvivalMissionConfig.SupportedRaiderTypes[j];
				for (int k = 0; k < countsForRaiderTypes[j]; k++)
				{
					array[num2] = survivorClass;
					num2++;
				}
			}
			ShuffleArrayDeterministic(array, GetHashCodeForSurvivalMission(combat));
			return array;
		}

		private static void ReAssignNonExplicitWalkerSpawners(CombatModel combat, int[] countsForWalkerTypes)
		{
			WalkerType[] randomizedSpecialWalkerList = GetRandomizedSpecialWalkerList(combat, countsForWalkerTypes);
			int num = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[i];
				if (IsSpawnPointCountedNonExplicitWalkerType(actorSpawnPointModel))
				{
					WalkerSpawnPointModel walkerSpawnPointModel = (WalkerSpawnPointModel)actorSpawnPointModel;
					walkerSpawnPointModel.UseOverrideWalkerType = true;
					if (randomizedSpecialWalkerList.Length != 0)
					{
						walkerSpawnPointModel.OverrideWalkerType = randomizedSpecialWalkerList[num];
						num = (num + 1) % randomizedSpecialWalkerList.Length;
					}
					else
					{
						walkerSpawnPointModel.OverrideWalkerType = WalkerType.WalkerTank;
					}
				}
			}
		}

		private static void ReAssignNonExplicitRaiderSpawners(CombatModel combat, int[] countsForRaiderTypes)
		{
			SurvivorClass[] randomizedRaiderList = GetRandomizedRaiderList(combat, countsForRaiderTypes);
			int num = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[i];
				if (IsSpawnPointCountedNonExplicitRaiderType(actorSpawnPointModel))
				{
					RaiderSpawnPointModel raiderSpawnPointModel = (RaiderSpawnPointModel)actorSpawnPointModel;
					if (randomizedRaiderList.Length != 0)
					{
						raiderSpawnPointModel.Class = randomizedRaiderList[num];
						num = (num + 1) % randomizedRaiderList.Length;
					}
					else
					{
						raiderSpawnPointModel.Class = SurvivorClass.Scout;
					}
				}
			}
		}

		public static void MakeAllSpawnersBurningForType(CombatModel combat, WalkerType walkerType)
		{
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[i];
				if (IsSpawnPointExplicitWalkerType(actorSpawnPointModel, walkerType))
				{
					actorSpawnPointModel.AdditionalTraits.Add("Burning");
				}
			}
		}

		private static int GetFnv1aHashCode(string str)
		{
			if (str == null)
			{
				return 0;
			}
			int length = str.Length;
			int num = length;
			for (int i = 0; i != length; i++)
			{
				num = (num ^ str[i]) * 16777619;
			}
			return num;
		}

		private static void ShuffleArrayDeterministic<T>(T[] arr, string stringForRandomSeedHash)
		{
			int fnv1aHashCode = GetFnv1aHashCode(stringForRandomSeedHash);
			ShuffleArrayDeterministic(arr, fnv1aHashCode);
		}

		private static void ShuffleArrayDeterministic<T>(T[] arr, int randomSeed)
		{
			new ModelRandom(randomSeed).ShuffleArray(arr);
		}

		private static int GetHashCodeForSurvivalMission(CombatModel combat)
		{
			return GetFnv1aHashCode(combat.SurvivalMission.ConfigName) ^ combat.SurvivalMissionConfigMissionOrderInSection;
		}

		private static void ApplySavedEnemyWalkerCounts(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			int[] array = new int[SurvivalMissionConfig.SupportedWalkerTypes.Length];
			for (int i = 0; i < SurvivalMissionConfig.SupportedWalkerTypes.Length; i++)
			{
				array[i] = combat.SurvivalMission.GetNumWalkersOfType(SurvivalMissionConfig.SupportedWalkerTypes[i]);
			}
			ReAssignNonExplicitWalkerSpawners(combat, array);
			int[] array2 = new int[SurvivalMissionConfig.CountedTags.Length * SurvivalMissionConfig.SupportedWalkerTypes.Length];
			for (int j = 0; j < SurvivalMissionConfig.SupportedWalkerTypes.Length; j++)
			{
				WalkerType walkerType = SurvivalMissionConfig.SupportedWalkerTypes[j];
				bool[] array3 = new bool[SurvivalMissionConfig.CountedTags.Length];
				int num = 0;
				int num2 = array[j];
				int num3 = 0;
				for (int k = 0; k < SurvivalMissionConfig.CountedTags.Length; k++)
				{
					int actorTag = SurvivalMissionConfig.CountedTags[k];
					bool wasRoundedDown = false;
					int countForTypeInSurvival = GetCountForTypeInSurvival(combat, walkerType, actorTag, out wasRoundedDown);
					array2[k + j * SurvivalMissionConfig.CountedTags.Length] = countForTypeInSurvival;
					num3 += countForTypeInSurvival;
					array3[k] = wasRoundedDown;
					if (wasRoundedDown)
					{
						num++;
					}
				}
				int num4 = num2 - num3;
				if (num4 < 0 || num4 > num)
				{
					combat.Debug.LogError("Survival tag count weighting logic bug!");
					num4 = 0;
				}
				if (num4 <= 0)
				{
					continue;
				}
				int[] array4 = new int[num];
				for (int l = 0; l < num4; l++)
				{
					array4[l] = 1;
				}
				ShuffleArrayDeterministic(array4, GetHashCodeForSurvivalMission(combat));
				int num5 = 0;
				for (int m = 0; m < SurvivalMissionConfig.CountedTags.Length; m++)
				{
					if (array3[m])
					{
						array2[m + j * SurvivalMissionConfig.CountedTags.Length] += array4[num5];
						num5++;
					}
				}
			}
			if (combat.IsGuildBattleMission)
			{
				ApplyConfigToSpawnersGvG(combat, array2, savedData);
			}
			else
			{
				ApplyConfigToSpawnersSurvival(combat, array2, savedData);
			}
		}

		private static void ApplyConfigToSpawnersSurvival(CombatModel combat, int[] countsForWalkers, SurvivalSavedMissionModel savedData)
		{
			for (int i = 0; i < SurvivalMissionConfig.SupportedWalkerTypes.Length; i++)
			{
				WalkerType walkerType = SurvivalMissionConfig.SupportedWalkerTypes[i];
				for (int j = 0; j < SurvivalMissionConfig.CountedTags.Length; j++)
				{
					int actorTag = SurvivalMissionConfig.CountedTags[j];
					countsForWalkers[j + i * SurvivalMissionConfig.CountedTags.Length] = ClampCountUsingSavedData(savedData, walkerType, actorTag, countsForWalkers[j + i * SurvivalMissionConfig.CountedTags.Length]);
					bool makeBoss = combat.SurvivalMission.IsWalkerTypeBoss(walkerType);
					bool flag = combat.SurvivalMission.IsWalkerTypeBurning(walkerType);
					ReassignWalkerSpawnerSpawnCounts(combat, walkerType, actorTag, countsForWalkers[j + i * SurvivalMissionConfig.CountedTags.Length], makeBoss, flag);
					if (flag)
					{
						MakeAllSpawnersBurningForType(combat, walkerType);
					}
				}
			}
		}

		private static void ApplyConfigToSpawnersGvG(CombatModel combat, int[] countsForWalkers, SurvivalSavedMissionModel savedData)
		{
			bool flag = false;
			for (int i = 0; i < SurvivalMissionConfig.SupportedWalkerTypes.Length; i++)
			{
				WalkerType walkerType = SurvivalMissionConfig.SupportedWalkerTypes[i];
				for (int j = 0; j < SurvivalMissionConfig.CountedTags.Length; j++)
				{
					int actorTag = SurvivalMissionConfig.CountedTags[j];
					countsForWalkers[j + i * SurvivalMissionConfig.CountedTags.Length] = ClampCountUsingSavedData(savedData, walkerType, actorTag, countsForWalkers[j + i * SurvivalMissionConfig.CountedTags.Length]);
					bool makeBoss = combat.SurvivalMission.IsWalkerTypeBoss(walkerType) && !flag;
					bool flag2 = combat.SurvivalMission.IsWalkerTypeBurning(walkerType);
					flag |= ReassignWalkerSpawnerSpawnCountsGvG(combat, walkerType, actorTag, countsForWalkers[j + i * SurvivalMissionConfig.CountedTags.Length], makeBoss, flag2);
					if (flag2)
					{
						MakeAllSpawnersBurningForType(combat, walkerType);
					}
				}
			}
		}

		private static void ApplySavedEnemyRaiderCounts(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			int[] array = new int[SurvivalMissionConfig.SupportedRaiderTypes.Length];
			for (int i = 0; i < SurvivalMissionConfig.SupportedRaiderTypes.Length; i++)
			{
				array[i] = combat.SurvivalMission.GetNumRaidersByType(SurvivalMissionConfig.SupportedRaiderTypes[i]);
			}
			ReAssignNonExplicitRaiderSpawners(combat, array);
			int[] array2 = new int[SurvivalMissionConfig.CountedTags.Length * SurvivalMissionConfig.SupportedRaiderTypes.Length];
			for (int j = 0; j < SurvivalMissionConfig.SupportedRaiderTypes.Length; j++)
			{
				SurvivorClass raiderType = SurvivalMissionConfig.SupportedRaiderTypes[j];
				bool[] array3 = new bool[SurvivalMissionConfig.CountedTags.Length];
				int num = 0;
				int num2 = array[j];
				int num3 = 0;
				for (int k = 0; k < SurvivalMissionConfig.CountedTags.Length; k++)
				{
					int actorTag = SurvivalMissionConfig.CountedTags[k];
					bool wasRoundedDown = false;
					int countForTypeInSurvival = GetCountForTypeInSurvival(combat, raiderType, actorTag, out wasRoundedDown);
					array2[k + j * SurvivalMissionConfig.CountedTags.Length] = countForTypeInSurvival;
					num3 += countForTypeInSurvival;
					array3[k] = wasRoundedDown;
					if (wasRoundedDown)
					{
						num++;
					}
				}
				int num4 = num2 - num3;
				if (num4 < 0 || num4 > num)
				{
					combat.Debug.LogError("Survival tag count weighting logic bug!");
					num4 = 0;
				}
				if (num4 <= 0)
				{
					continue;
				}
				int[] array4 = new int[num];
				for (int l = 0; l < num4; l++)
				{
					array4[l] = 1;
				}
				ShuffleArrayDeterministic(array4, GetHashCodeForSurvivalMission(combat));
				int num5 = 0;
				for (int m = 0; m < SurvivalMissionConfig.CountedTags.Length; m++)
				{
					if (array3[m])
					{
						array2[m + j * SurvivalMissionConfig.CountedTags.Length] += array4[num5];
						num5++;
					}
				}
			}
			for (int n = 0; n < SurvivalMissionConfig.SupportedRaiderTypes.Length; n++)
			{
				SurvivorClass survivorClass = SurvivalMissionConfig.SupportedRaiderTypes[n];
				for (int num6 = 0; num6 < SurvivalMissionConfig.CountedTags.Length; num6++)
				{
					int actorTag2 = SurvivalMissionConfig.CountedTags[num6];
					array2[num6 + n * SurvivalMissionConfig.CountedTags.Length] = ClampCountUsingSavedData(savedData, survivorClass, actorTag2, array2[num6 + n * SurvivalMissionConfig.CountedTags.Length]);
					bool makeBoss = false;
					bool makeBurning = combat.SurvivalMission.IsRaiderTypeBurning(survivorClass);
					ReassignRaiderSpawnerSpawnCounts(combat, survivorClass, actorTag2, array2[num6 + n * SurvivalMissionConfig.CountedTags.Length], makeBoss, makeBurning);
				}
			}
		}

		public static void ApplySavedEnemyCounts(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			ApplySavedEnemyWalkerCounts(combat, savedData);
			ApplySavedEnemyRaiderCounts(combat, savedData);
		}

		public static void ApplySavedPlayerCharacterStates(CombatModel combat, SurvivalCharacterContainerModel savedCharacters)
		{
			List<ActorModel> allActors = combat.GetAllActors();
			for (int i = 0; i < allActors.Count; i++)
			{
				ActorModel actorModel = allActors[i];
				if (!(actorModel is SurvivorModel) || actorModel.Faction != Faction.Survivor)
				{
					continue;
				}
				SurvivalCharacterStateModel survivorStateInSurvivalMode = savedCharacters.GetSurvivorStateInSurvivalMode((SurvivorModel)actorModel);
				int val = actorModel.Hitpoints;
				int num = 1;
				int chargeLevel = actorModel.ChargeMeter.ChargeLevel;
				if (survivorStateInSurvivalMode != null)
				{
					val = Math.Max((int)FixedPoint.Round(actorModel.MaxHitPoints * (survivorStateInSurvivalMode.HealthPercentage / new FixedPoint(100.0))), 1);
					num = survivorStateInSurvivalMode.StrugglesLeft;
					chargeLevel = Math.Min(survivorStateInSurvivalMode.ChargePoints, actorModel.ChargeMeter.MaxLevel);
					if (!string.IsNullOrEmpty(combat.manager.CustomSurvivalMissionConfigName))
					{
						val = combat.manager.Player.PlayerRandom.GetRandomInRange(1, actorModel.MaxHitPoints);
						num = combat.manager.Player.PlayerRandom.GetRandomInRange(0, 1);
						chargeLevel = combat.manager.Player.PlayerRandom.GetRandomInRange(0, actorModel.ChargeMeter.MaxLevel);
					}
				}
				else
				{
					combat.Debug.LogError("Failed to find matching survivor state for a survivor assigned to a survival combat mission.");
				}
				actorModel.SetHitpoints(val);
				actorModel.StrugglesLeft = num;
				actorModel.ChargeMeter.ChargeLevel = chargeLevel;
				actorModel.OnRedHealthBar = num <= 0;
				int maxShieldHitPoints = actorModel.MaxShieldHitPoints;
				int shieldHitPoints = actorModel.ShieldHitPoints;
				SurvivalCharacterShieldStateModel survivorShieldStateInSurvivalMode = savedCharacters.GetSurvivorShieldStateInSurvivalMode((SurvivorModel)actorModel);
				if (survivorShieldStateInSurvivalMode != null)
				{
					if (actorModel.MaxShieldHitPoints > 0 && survivorShieldStateInSurvivalMode.MaxShieldPoints > 0)
					{
						maxShieldHitPoints = survivorShieldStateInSurvivalMode.MaxShieldPoints;
						shieldHitPoints = survivorShieldStateInSurvivalMode.ShieldPoints;
						actorModel.MaxShieldHitPoints = maxShieldHitPoints;
						actorModel.ShieldHitPoints = shieldHitPoints;
					}
				}
				else
				{
					combat.Debug.LogError("Failed to find matching survivor shield state for a survivor assigned to a survival combat mission.");
				}
			}
		}

		public static void SavePlayerCharacterStates(CombatModel combat, SurvivalCharacterContainerModel savedCharacters)
		{
			foreach (SurvivorModel item in combat.MissionRoster)
			{
				SurvivalCharacterStateModel survivorStateInSurvivalMode = savedCharacters.GetSurvivorStateInSurvivalMode(item);
				if (survivorStateInSurvivalMode != null)
				{
					survivorStateInSurvivalMode.HealthPercentageBeforeCombat = (int)FixedPoint.Round(survivorStateInSurvivalMode.HealthPercentage);
					survivorStateInSurvivalMode.HealthPercentage = new FixedPoint((float)item.Hitpoints / (float)item.MaxHitPoints) * 100L;
					survivorStateInSurvivalMode.ChargePoints = item.ChargeMeter.ChargeLevel;
					survivorStateInSurvivalMode.StrugglesLeftBeforeCombat = survivorStateInSurvivalMode.StrugglesLeft;
					survivorStateInSurvivalMode.StrugglesLeft = item.StrugglesLeft;
					survivorStateInSurvivalMode.OutOfAction = item.IsDead || item.CombatEndCondition == CombatEndCondition.Incapacitated;
				}
				SurvivalCharacterShieldStateModel survivorShieldStateInSurvivalMode = savedCharacters.GetSurvivorShieldStateInSurvivalMode(item);
				if (survivorShieldStateInSurvivalMode != null && item.MaxShieldHitPoints > 0)
				{
					survivorShieldStateInSurvivalMode.MaxShieldPoints = item.MaxShieldHitPoints;
					survivorShieldStateInSurvivalMode.ShieldPoints = item.ShieldHitPoints;
				}
				item.FinishTimedEffect(interrupted: true);
			}
		}

		public static void IncreaseFailureCount(CombatModel combat)
		{
			string presetVariableName = PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalAttemptNumber);
			int intVariable = combat.PersistentMissionVariableManager.GetIntVariable(presetVariableName, 0);
			combat.PersistentMissionVariableManager.SetIntVariableNoNotificationAllowingReadOnly(presetVariableName, intVariable + 1);
		}

		public static void SavePersistentVariables(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			List<PersistentMissionVariable> allVariableValues = combat.PersistentMissionVariableManager.GetAllVariableValues();
			for (int i = 0; i < allVariableValues.Count; i++)
			{
				savedData.SetPersistentIntVariableValue(allVariableValues[i].Name, allVariableValues[i].ValueInt, allVariableValues[i].ReadOnly);
			}
		}

		public static void ApplyOpponentLevel(CombatModel combat)
		{
			MissionGenerationData missionGenerationData = combat.manager.Player.gameEconomyData.GetMissionGenerationData(combat.manager.Player.SelectedMissionDifficulty);
			int enemyLevel;
			bool flag = WorldBossMissionModel.TryGetEnemyLevel(combat.manager.Player.GetAttackTargetMissionModel(), out enemyLevel);
			List<ActorModel> allActors = combat.GetAllActors();
			for (int i = 0; i < allActors.Count; i++)
			{
				ActorModel actorModel = allActors[i];
				if (actorModel is RaiderModel && actorModel.Faction == Faction.Raider)
				{
					actorModel.Level = (flag ? enemyLevel : combat.manager.Player.PlayerRandom.GetRandomInRange(missionGenerationData.MinWalkerLevel, missionGenerationData.MaxWalkerLevel));
				}
				if (actorModel is WalkerModel && (actorModel.Faction == Faction.Walker || actorModel.Faction == Faction.Environmental))
				{
					actorModel.Level = (flag ? enemyLevel : combat.manager.Player.PlayerRandom.GetRandomInRange(missionGenerationData.MinWalkerLevel, missionGenerationData.MaxWalkerLevel));
				}
			}
		}

		public static void SaveEnemyCounts(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			List<ActorModel> allActors = combat.GetAllActors();
			int[] array = new int[SurvivalMissionConfig.CountedTags.Length * SurvivalMissionConfig.SupportedWalkerTypes.Length];
			int[] array2 = new int[SurvivalMissionConfig.CountedTags.Length * SurvivalMissionConfig.SupportedRaiderTypes.Length];
			string[] array3 = new string[SurvivalMissionConfig.SupportedRaiderTypes.Length];
			for (int i = 0; i < SurvivalMissionConfig.SupportedRaiderTypes.Length; i++)
			{
				array3[i] = "Default" + SurvivalMissionConfig.SupportedRaiderTypes[i];
			}
			for (int j = 0; j < SurvivalMissionConfig.CountedTags.Length; j++)
			{
				int num = SurvivalMissionConfig.CountedTags[j];
				for (int k = 0; k < allActors.Count; k++)
				{
					ActorModel actorModel = allActors[k];
					if (actorModel.ActorTag != num || actorModel.IsDead)
					{
						continue;
					}
					if (actorModel is RaiderModel && actorModel.Faction == Faction.Raider)
					{
						for (int l = 0; l < SurvivalMissionConfig.SupportedRaiderTypes.Length; l++)
						{
							if (actorModel.ActorDefinitionID == array3[l])
							{
								array2[j + l * SurvivalMissionConfig.CountedTags.Length]++;
								break;
							}
						}
					}
					if (!(actorModel is WalkerModel) || (actorModel.Faction != Faction.Walker && actorModel.Faction != Faction.Environmental))
					{
						continue;
					}
					for (int m = 0; m < SurvivalMissionConfig.SupportedWalkerTypes.Length; m++)
					{
						if (actorModel.ActorDefinitionID == SurvivalMissionConfig.SupportedWalkerTypes[m].ToString())
						{
							array[j + m * SurvivalMissionConfig.CountedTags.Length]++;
							break;
						}
					}
				}
			}
			for (int n = 0; n < SurvivalMissionConfig.CountedTags.Length; n++)
			{
				int actorTag = SurvivalMissionConfig.CountedTags[n];
				for (int num2 = 0; num2 < SurvivalMissionConfig.SupportedWalkerTypes.Length; num2++)
				{
					savedData.SaveCountForWalker(array[n + num2 * SurvivalMissionConfig.CountedTags.Length], SurvivalMissionConfig.SupportedWalkerTypes[num2], actorTag);
				}
				for (int num3 = 0; num3 < SurvivalMissionConfig.SupportedRaiderTypes.Length; num3++)
				{
					savedData.SaveCountForRaider(array2[n + num3 * SurvivalMissionConfig.CountedTags.Length], SurvivalMissionConfig.SupportedRaiderTypes[num3], actorTag);
				}
			}
		}
	}
}
