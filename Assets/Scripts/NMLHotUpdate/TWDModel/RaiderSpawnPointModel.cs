using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class RaiderSpawnPointModel : ActorSpawnPointModel
	{
		public int ReplaceWithSurvivorPlayerIndex { get; set; }

		public SurvivorClass Class { get; set; }

		public string ActorClassID { get; set; }

		public string ActorID { get; set; }

		public AIMode AIMode { get; set; }

		public string WeaponOverrideId { get; set; }

		public string ArmorOverrideId { get; set; }

		public int EquipmentLevel { get; set; }

		public int EquipmentRarityLevel { get; set; }

		public bool SpawnCountInUse { get; set; }

		public RaiderVisualization RaiderVisualization { get; set; }

		public bool SpawnUsed { get; set; }

		public RaiderSpawnPointModel()
		{
			ReplaceWithSurvivorPlayerIndex = -1;
		}

		public RaiderSpawnPointModel(string viewId)
			: base(viewId)
		{
			ReplaceWithSurvivorPlayerIndex = -1;
		}

		protected override int InternalSpawn(ActorModel instigator)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (SpawnCountInUse && base.CurrentSpawnCount >= base.TotalSpawnCount)
			{
				return 0;
			}
			if (combatModel.GetOccupier(base.Location.Coordinate) != null)
			{
				return 0;
			}
			MissionGenerationData missionGenerationData = base.gameEconomyData.GetMissionGenerationData(base.manager.Player.SelectedMissionDifficulty);
			int enemyLevel;
			bool flag = WorldBossMissionModel.TryGetEnemyLevel(base.manager.Player.GetAttackTargetMissionModel(), out enemyLevel);
			int num = (flag ? enemyLevel : Math.Max(1, base.manager.Player.PlayerRandom.GetRandomInRange(missionGenerationData.MinWalkerLevel, missionGenerationData.MaxWalkerLevel) + base.LevelOffset));
			SpawnModifierState spawnModifiers = combatModel.SpawnModifiers;
			if (spawnModifiers != null)
			{
				if (spawnModifiers.UpgradeRaiderCount > 0)
				{
					spawnModifiers.UpgradeRaiderCount--;
					num++;
				}
				if (Class == SurvivorClass.Scout && spawnModifiers.PromoteMeleeRaiderCount > 0)
				{
					Class = SurvivorClass.Warrior;
					spawnModifiers.PromoteMeleeRaiderCount--;
				}
				else if (Class == SurvivorClass.Warrior && spawnModifiers.PromoteMeleeRaiderCount > 0)
				{
					Class = SurvivorClass.Bruiser;
					spawnModifiers.PromoteMeleeRaiderCount--;
				}
				else if (Class == SurvivorClass.Shooter && spawnModifiers.PromoteRangedRaiderCount > 0)
				{
					Class = SurvivorClass.Assault;
					spawnModifiers.PromoteRangedRaiderCount--;
				}
				else if (Class == SurvivorClass.Assault && spawnModifiers.PromoteRangedRaiderCount > 0)
				{
					Class = SurvivorClass.Hunter;
					spawnModifiers.PromoteRangedRaiderCount--;
				}
			}
			string factionName = combatModel.GetFactionName(Faction.Raider);
			factionName = ((factionName == "Raider") ? "" : ("_" + factionName));
			switch (Class)
			{
			case SurvivorClass.Assault:
				ActorClassID = "DefaultAssault" + factionName;
				ActorID = "DefaultAssault" + factionName;
				break;
			case SurvivorClass.Bruiser:
				ActorClassID = "DefaultBruiser" + factionName;
				ActorID = "DefaultBruiser" + factionName;
				break;
			case SurvivorClass.Hunter:
				ActorClassID = "DefaultHunter" + factionName;
				ActorID = "DefaultHunter" + factionName;
				break;
			case SurvivorClass.Scout:
				ActorClassID = "DefaultScout" + factionName;
				ActorID = "DefaultScout" + factionName;
				break;
			case SurvivorClass.Shooter:
				ActorClassID = "DefaultShooter" + factionName;
				ActorID = "DefaultShooter" + factionName;
				break;
			case SurvivorClass.Warrior:
				ActorClassID = "DefaultWarrior" + factionName;
				ActorID = "DefaultWarrior" + factionName;
				break;
			}
			ActorFootprint actorFootprint = null;
			FacingDirection facing = FacingDirection.North;
			ActorDefinition actorDefinition = base.gameEconomyData.GetActorDefinition(ActorClassID);
			if (actorDefinition != null && actorDefinition.FootprintWidth > 0)
			{
				actorFootprint = ActorFootprint.CreateFromActorDefinition(actorDefinition);
				facing = actorDefinition.InitialFacingDirection;
				if (base.UseSpawnRotationOverride)
				{
					facing = FacingDirections.FromRotationY(base.SpawnRotationY);
				}
				List<GridCoordinate> occupiedCells = actorFootprint.GetOccupiedCells(base.Location.Coordinate, facing);
				for (int i = 0; i < occupiedCells.Count; i++)
				{
					if (!combatModel.Grid.IsCoordinateValid(occupiedCells[i]) || combatModel.IsBlocked(occupiedCells[i]) || combatModel.GetOccupier(occupiedCells[i]) != null)
					{
						return 0;
					}
				}
			}
			if (flag)
			{
				num = enemyLevel;
			}
			ActorModel actorModel = CreateActorModel(combatModel, num);
			actorModel.SetupForCombat(combatModel);
			actorModel.AIDataModel.Alertness = base.Alertness;
			actorModel.AIDataModel.Mode = AIMode;
			if (base.UseSpawnRotationOverride)
			{
				actorModel.UseSpawnRotationOverride = true;
				actorModel.SpawnRotationY = base.SpawnRotationY;
			}
			if (actorModel is TankActorModel tankActorModel)
			{
				tankActorModel.IsBoss = true;
				tankActorModel.AIDataModel.Mode = AIMode.Stationary;
				tankActorModel.AIDataModel.Alertness = AIAlertness.Homing;
				if (actorFootprint != null)
				{
					tankActorModel.SetFootprint(actorFootprint);
					tankActorModel.Facing = facing;
				}
			}
			combatModel.UpdateOccupiers();
			return 1;
		}

		private ActorModel CreateActorModel(CombatModel combat, int raiderLevel)
		{
			if (ReplaceWithSurvivorPlayerIndex > -1)
			{
				PlayerModel player = base.manager.Player;
				WorldBossMissionModel worldBossMissionModel = player.GetAttackTargetMissionModel() as WorldBossMissionModel;
				GuildBattlePvpTeam guildBattlePvpTeam = ((worldBossMissionModel == null) ? ((player.GetAttackTargetMissionModel() is GuildBattleMapMissionModel guildBattleMapMissionModel) ? player.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattleMapMissionModel.Id) : null) : ((player.WorldBossModelManager != null) ? player.WorldBossModelManager.GetCurrentDefenderTeam() : null));
				if (guildBattlePvpTeam == null || ReplaceWithSurvivorPlayerIndex >= guildBattlePvpTeam.Survivors.Count)
				{
					ActorModel actorModel = combat.CreateActor(base.Location.Coordinate, Faction.Raider, raiderLevel, base.SpawnTag, WeaponOverrideId, ArmorOverrideId, EquipmentRarityLevel, ActorClassID, ActorID, base.Gender, WalkerVisualization.Normal, RaiderVisualization);
					actorModel.UseModularCharacter = false;
					return actorModel;
				}
				SurvivorMockData survivorMockData = guildBattlePvpTeam.Survivors[ReplaceWithSurvivorPlayerIndex];
				int survivorLevel = ((worldBossMissionModel != null) ? survivorMockData.Level : GvGModelHelper.GetPlayerSpecificDifficulty(player));
				SurvivorModel survivorModel = player.SurvivorContainer.CreateSurvivorFromSurvivorMockData(survivorMockData, survivorLevel);
				survivorModel.GridCoordinate = base.Location.Coordinate;
				survivorModel.Faction = Faction.Raider;
				survivorModel.ChangeFaction(Faction.Raider);
				survivorModel.UseModularCharacter = true;
				survivorModel.AIController.Enabled = true;
				survivorModel.GuildBattlePvPSurvivorIndex = ReplaceWithSurvivorPlayerIndex;
				if (ReplaceWithSurvivorPlayerIndex == 0 && base.manager.Player.Combat != null && (base.manager.Player.Combat.IsGuildBattleMission || base.manager.Player.Combat.IsWorldBossMission))
				{
					survivorModel.RegisterLeaderTraits();
				}
				combat.RegisterActor(survivorModel);
				return survivorModel;
			}
			ActorModel actorModel2 = combat.CreateActor(base.Location.Coordinate, Faction.Raider, raiderLevel, base.SpawnTag, WeaponOverrideId, ArmorOverrideId, EquipmentRarityLevel, ActorClassID, ActorID, base.Gender, WalkerVisualization.Normal, RaiderVisualization);
			actorModel2.UseModularCharacter = false;
			return actorModel2;
		}
	}
}
