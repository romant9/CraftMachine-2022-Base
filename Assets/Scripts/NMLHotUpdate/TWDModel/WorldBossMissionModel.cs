using System.Collections.Generic;

namespace TWDModel
{
	public class WorldBossMissionModel : IMapMissionModel, IAttackTargetModel, IChallengeDebuffProvider
	{
		public int WorldBossBattlegroundDefinitionId;

		public string CapturePoint;

		public string Cell;

		public string MapIds;

		public string MissionId;

		public int EnemyLevel;

		public WorldBossMissionType WorldBossMissionType;

		public long BattleStartUtcMs;

		public long TimeLimitMs;

		private GameEconomyData _ged;

		public int MissionLevel => EnemyLevel;

		public int RequiredSurvivorLevel => EnemyLevel;

		public MissionDifficulty MissionDifficulty => MissionDifficulty.Normal;

		public int MaxTeamSize => 3;

		public GuildBattleMapMissionModel.MissionType Type => GuildBattleMapMissionModel.MissionType.Invalid;

		public bool IsDisabledOnGED => false;

		public int AttackTargetId
		{
			get
			{
				if (WorldBossBattlegroundDefinitionId == 0)
				{
					return GetRandomSeed();
				}
				return WorldBossBattlegroundDefinitionId;
			}
		}

		public bool IsInApocalyptiWeeklyChallenge => false;

		public bool IsBattleTimedOut(long nowUtcMs)
		{
			if (TimeLimitMs <= 0 || BattleStartUtcMs <= 0)
			{
				return false;
			}
			return nowUtcMs >= BattleStartUtcMs + TimeLimitMs;
		}

		public void Restore(GameEconomyData ged)
		{
			_ged = ged;
		}

		public bool HasValidMissionBinding()
		{
			return !string.IsNullOrEmpty(MissionId);
		}

		public static bool TryGetEnemyLevel(IMapMissionModel missionModel, out int enemyLevel)
		{
			if (!(missionModel is WorldBossMissionModel worldBossMissionModel))
			{
				enemyLevel = 0;
				return false;
			}
			enemyLevel = worldBossMissionModel.EnemyLevel;
			return true;
		}

		public static WorldBossMissionModel Create(WorldBossBattlegroundDefinition def, string capturePoint, string cell, GameEconomyData ged, WorldBossMissionType worldBossMissionType)
		{
			if (worldBossMissionType == WorldBossMissionType.BOSS)
			{
				return new WorldBossMissionModel
				{
					_ged = ged,
					WorldBossBattlegroundDefinitionId = def.ID,
					CapturePoint = capturePoint,
					Cell = cell,
					MapIds = def.MapIds,
					MissionId = ged.GetWorldBossMissionIdForTankBoss(def),
					EnemyLevel = def.EnemyLevel,
					WorldBossMissionType = WorldBossMissionType.BOSS
				};
			}
			return new WorldBossMissionModel
			{
				_ged = ged,
				WorldBossBattlegroundDefinitionId = def.ID,
				CapturePoint = capturePoint,
				Cell = cell,
				MapIds = def.MapIds,
				MissionId = ged.GetWorldBossMissionIdForCell(def, cell),
				EnemyLevel = def.EnemyLevel,
				WorldBossMissionType = worldBossMissionType
			};
		}

		public Cashier GetStartMissionCashier(TWDModelManager manager)
		{
			Cashier cashier = new Cashier(manager);
			cashier.AddItem(new CashierItem(PurchaseType.WorldBossAttackMission));
			return cashier;
		}

		public Cashier GetStartMissionExpertModeCashier(TWDModelManager twdManager)
		{
			return new Cashier(twdManager);
		}

		public SurvivalMissionConfig SolveSurvivalConfigForCurrentMission()
		{
			return null;
		}

		public bool IsUsingSurvivalConfig()
		{
			return false;
		}

		public MapMissionParameters ToMissionParameters()
		{
			return new MapMissionParameters
			{
				MissionId = MissionId,
				MissionLevel = EnemyLevel,
				IsDeadly = false,
				LootTag = DropEventDefinition.DropEventTag.None,
				RandomSeed = GetRandomSeed(),
				IsSurvival = false,
				IsPvP = (WorldBossMissionType == WorldBossMissionType.PVP),
				IsWorldBoss = true
			};
		}

		public int GetRandomSeed()
		{
			return ((CapturePoint ?? string.Empty) + "_" + (Cell ?? string.Empty)).GetHashCode();
		}

		public List<DifficultyIncrementalDebuff> GetChallengeDebuffs()
		{
			return GetWorldBossDebuffs();
		}

		public List<DifficultyIncrementalDebuff> GetWorldBossDebuffs()
		{
			List<DifficultyIncrementalDebuff> list = new List<DifficultyIncrementalDebuff>();
			if (_ged != null)
			{
				list.AddRange(_ged.GetWorldBossBattlegroundDefinitionById(WorldBossBattlegroundDefinitionId));
			}
			return list;
		}
	}
}
