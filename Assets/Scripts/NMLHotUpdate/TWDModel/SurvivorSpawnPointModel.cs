using System;

namespace TWDModel
{
	public class SurvivorSpawnPointModel : ActorSpawnPointModel
	{
		public string ActorID { get; set; }

		public int MinLevelOffset { get; set; }

		public int MaxLevelOffset { get; set; }

		public int RarityLevel { get; set; }

		public SurvivorClass SurvivorClass { get; set; }

		public string WeaponOverrideId { get; set; }

		public string ArmorOverrideId { get; set; }

		public int EquipmentLevel { get; set; }

		public int EquipmentRarityLevel { get; set; }

		public int RosterIndex { get; set; }

		public int MovementOverride { get; set; }

		public bool IsNotGivenToPlayer { get; set; }

		public SurvivorSpawnPointModel()
		{
		}

		public SurvivorSpawnPointModel(string viewId)
			: base(viewId)
		{
		}

		protected override int InternalSpawn(ActorModel instigator)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel.GetOccupier(base.Location.Coordinate) != null)
			{
				return 0;
			}
			PlayerModel player = base.manager.Player;
			player.gameEconomyData.GetSurvivorsMaxUpgradeLevel(SurvivorClass);
			int[] survivorStartingLevelsForMission = player.gameEconomyData.GetSurvivorStartingLevelsForMission(player.SelectedMissionDifficulty, RarityLevel);
			if (EquipmentLevel < 1)
			{
				EquipmentLevel = survivorStartingLevelsForMission[1];
			}
			SurvivorModel survivorModel = null;
			bool flag = false;
			if (SurvivorClass != SurvivorClass.None)
			{
				survivorModel = player.SurvivorContainer.CreateRandomSurvivor(0, survivorStartingLevelsForMission[0], survivorStartingLevelsForMission[1], RarityLevel, SurvivorClass, null, Math.Max(1, EquipmentLevel), EquipmentRarityLevel);
				if (base.Gender != ActorGender.NotSpecified)
				{
					survivorModel.Gender = base.Gender;
				}
			}
			else
			{
				ActorDefinition actorDefinition = base.manager.GameEconomyData.GetActorDefinition(ActorID);
				if (actorDefinition != null)
				{
					survivorModel = base.manager.Player.SurvivorContainer.CreateSurvivorFromDefinition(actorDefinition.ID, survivorStartingLevelsForMission[0] + MinLevelOffset, survivorStartingLevelsForMission[1] + MaxLevelOffset, RarityLevel, Math.Max(1, EquipmentLevel), EquipmentRarityLevel, player.PlayerRandom, WeaponOverrideId, ArmorOverrideId);
					SetOverrides(ref survivorModel);
					flag = true;
				}
			}
			survivorModel.ActorTag = base.SpawnTag;
			survivorModel.MissionFailCondition = base.MissionFailCondition;
			survivorModel.IsNotGivenToPlayer = IsNotGivenToPlayer;
			if (flag)
			{
				combatModel.AddExtraCombatSurvivor(survivorModel, base.Location.Coordinate, RosterIndex);
				if (RosterIndex == 0)
				{
					survivorModel.RegisterLeaderTraits();
				}
				combatModel.TurnManager.ResetActiveActor();
			}
			else
			{
				combatModel.AddExtraCombatSurvivor(survivorModel, base.Location.Coordinate);
			}
			return 1;
		}

		private void SetOverrides(ref SurvivorModel survivorModel)
		{
			if (MovementOverride > 0)
			{
				survivorModel.ApplyMovementModifier(MovementOverride);
			}
		}
	}
}
