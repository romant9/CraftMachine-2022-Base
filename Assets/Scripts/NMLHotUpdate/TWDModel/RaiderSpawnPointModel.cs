using System;

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
			int num = Math.Max(1, base.manager.Player.PlayerRandom.GetRandomInRange(missionGenerationData.MinWalkerLevel, missionGenerationData.MaxWalkerLevel) + base.LevelOffset);
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
			ActorModel actorModel = CreateActorModel(combatModel, num);
			actorModel.SetupForCombat(combatModel);
			actorModel.AIDataModel.Alertness = base.Alertness;
			actorModel.AIDataModel.Mode = AIMode;
			combatModel.UpdateOccupiers();
			return 1;
		}

		private ActorModel CreateActorModel(CombatModel combat, int raiderLevel)
		{
			if (ReplaceWithSurvivorPlayerIndex > -1)
			{
				PlayerModel player = base.manager.Player;
				GuildBattleMapMissionModel guildBattleMapMissionModel = base.manager.Player.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
				SurvivorMockData survivorModel = player.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattleMapMissionModel.Id).Survivors[ReplaceWithSurvivorPlayerIndex];
				SurvivorModel survivorModel2 = player.SurvivorContainer.CreateSurvivorFromSurvivorMockData(survivorModel, GvGModelHelper.GetPlayerSpecificDifficulty(player));
				survivorModel2.GridCoordinate = base.Location.Coordinate;
				survivorModel2.Faction = Faction.Raider;
				survivorModel2.ChangeFaction(Faction.Raider);
				survivorModel2.UseModularCharacter = true;
				survivorModel2.AIController.Enabled = true;
				survivorModel2.GuildBattlePvPSurvivorIndex = ReplaceWithSurvivorPlayerIndex;
				if (ReplaceWithSurvivorPlayerIndex == 0 && base.manager.Player.Combat != null && base.manager.Player.Combat.IsGuildBattleMission)
				{
					survivorModel2.RegisterLeaderTraits();
				}
				combat.RegisterActor(survivorModel2);
				return survivorModel2;
			}
			ActorModel actorModel = combat.CreateActor(base.Location.Coordinate, Faction.Raider, raiderLevel, base.SpawnTag, WeaponOverrideId, ArmorOverrideId, EquipmentRarityLevel, ActorClassID, ActorID, base.Gender, WalkerVisualization.Normal, RaiderVisualization);
			actorModel.UseModularCharacter = false;
			return actorModel;
		}
	}
}
