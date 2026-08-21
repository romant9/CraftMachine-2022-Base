using System.Collections.Generic;

namespace TWDModel
{
	public class GuildBattleCombatHelper
	{
		public static List<int> CreateSaveData(CombatModel combat, GuildBattleMapMissionModel attackedMissionModel)
		{
			List<int> list = new List<int>();
			if (combat.manager.Player.GuildWarModel == null || combat.manager.Player.GuildWarModel.CurrentBattle == null || combat.manager.Player.GuildWarModel.CurrentBattle.CurrentMapModel == null)
			{
				return list;
			}
			if (combat.manager.Player.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(attackedMissionModel.Id) != null)
			{
				List<int> guildBattlePVPSurvivorsKilledIndices = combat.GuildBattlePVPSurvivorsKilledIndices;
				for (int i = 0; i < guildBattlePVPSurvivorsKilledIndices.Count; i++)
				{
					if (!list.Contains(guildBattlePVPSurvivorsKilledIndices[i]))
					{
						list.Add(guildBattlePVPSurvivorsKilledIndices[i]);
					}
				}
			}
			else
			{
				combat.manager.Debug.LogError("Team not found for " + attackedMissionModel.Id);
			}
			return list;
		}

		public static void ApplySavedEnemyCounts(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			ApplySavedEnemyRaiderCounts(combat, savedData);
		}

		private static void ApplySavedEnemyRaiderCounts(CombatModel combat, SurvivalSavedMissionModel savedData)
		{
			if (combat.SurvivalMission.HasAnySurvivorPlayer())
			{
				ReassignRaiderSpawnerSpawnCountsWithSurvivorPlayer(combat, combat.SurvivalMission.GetNumSurvivorPlayers());
			}
		}

		private static void ReassignRaiderSpawnerSpawnCountsWithSurvivorPlayer(CombatModel combat, int totalCount)
		{
			int num = CountNumberOfFreeRaiderSpawners(combat);
			if (num == 0)
			{
				if (totalCount > 0)
				{
					combat.Debug.LogWarning("Total " + totalCount + " survivalPlayers left unspawned as no such spawners in map.");
				}
			}
			else
			{
				ReassignRaiderSpawnerSpawnCountsWithSurvivorPlayerInternal(num, combat, totalCount);
			}
		}

		private static void ReassignRaiderSpawnerSpawnCountsWithSurvivorPlayerInternal(int numMatchingSpawners, CombatModel combat, int totalCount)
		{
			int num = totalCount / numMatchingSpawners;
			int num2 = totalCount - num * numMatchingSpawners;
			int[] array = new int[numMatchingSpawners];
			for (int i = 0; i < num2; i++)
			{
				array[i] = 1;
			}
			combat.manager.Player.PlayerRandom.ShuffleArray(array);
			PlayerModel player = combat.manager.Player;
			GuildBattleMapMissionModel guildBattleMapMissionModel = player.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
			GuildBattlePvpTeam guildBattlePvpTeam = ((guildBattleMapMissionModel != null) ? player.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattleMapMissionModel.Id) : null);
			List<int> list = guildBattleMapMissionModel?.SavedData;
			if (guildBattlePvpTeam == null)
			{
				return;
			}
			int num3 = 0;
			int num4 = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int j = 0; j < models.Count; j++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[j];
				if (!IsSpawnPointRaiderType(actorSpawnPointModel))
				{
					continue;
				}
				int num5 = num;
				if (num4 < array.Length)
				{
					num5 += array[num4];
				}
				RaiderSpawnPointModel raiderSpawnPointModel = (RaiderSpawnPointModel)actorSpawnPointModel;
				bool flag = false;
				if (num5 > 0 && num3 < guildBattlePvpTeam.Survivors.Count)
				{
					if (list == null || !list.Contains(num3))
					{
						flag = true;
					}
					raiderSpawnPointModel.ReplaceWithSurvivorPlayerIndex = (flag ? num3 : (-1));
					num3++;
				}
				raiderSpawnPointModel.SpawnUsed = flag;
				raiderSpawnPointModel.SpawnCountInUse = true;
				raiderSpawnPointModel.SpawnCountPerAction = (flag ? num5 : 0);
				raiderSpawnPointModel.TotalSpawnCount = (flag ? num5 : 0);
				num4++;
			}
		}

		private static int CountNumberOfFreeRaiderSpawners(CombatModel combat)
		{
			int num = 0;
			List<TWDModelObject> models = combat.GetModels<ActorSpawnPointModel>();
			for (int i = 0; i < models.Count; i++)
			{
				ActorSpawnPointModel actorSpawnPointModel = (ActorSpawnPointModel)models[i];
				if (IsSpawnPointRaiderType(actorSpawnPointModel) && !((RaiderSpawnPointModel)actorSpawnPointModel).SpawnUsed)
				{
					num++;
				}
			}
			return num;
		}

		private static bool IsSpawnPointRaiderType(ActorSpawnPointModel spawnPoint)
		{
			if (!(spawnPoint is RaiderSpawnPointModel))
			{
				return false;
			}
			return true;
		}

		public static string GetEnemyPlayerName(CombatModel combat)
		{
			PlayerModel player = combat.manager.Player;
			if (player.GetAttackTargetMissionModel() is GuildBattleMapMissionModel guildBattleMapMissionModel)
			{
				GuildBattlePvpTeam pvpTeamForMission = player.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattleMapMissionModel.Id);
				if (pvpTeamForMission != null)
				{
					GuildBattleParticipantInfo currentGuildBattlePlayerInfo = player.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission);
					if (currentGuildBattlePlayerInfo != null)
					{
						return currentGuildBattlePlayerInfo.Name;
					}
				}
			}
			return null;
		}
	}
}
