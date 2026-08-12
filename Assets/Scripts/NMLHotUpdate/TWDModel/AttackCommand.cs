using System;
using BaseModel;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class AttackCommand : ConsumeCurrencyCommand
	{
		public EndlessModeGameModeType EndlessModeGameModeType { get; set; }

		public AttackCommand()
		{
		}

		public AttackCommand(MapMissionModel mapMissionModel)
			: base(mapMissionModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			MapMissionModel model = tWDModelManager.GetModel<MapMissionModel>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			GuildBattleAttackTargetMissionData attackTargetMission = tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackTargetMission;
			if (attackTargetMission != null)
			{
				attackTargetMission.Clear();
				tWDModelManager.Player.ResetIAttackTargetMapMission();
			}
			if (model.IsEndlessMission && EndlessModeGameModeType != tWDModelManager.Player.EndlessModeManager.EndlessModeGameModeType)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = tWDModelManager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(model.MissionSpawnPointGroup);
			if (model.State != MapMissionState.Completed || model.MissionSpawnPointGroup.Category == MapCategory.Season)
			{
				tWDModelResult = tWDModelManager.Player.MapContainerModel.AttackMission(model, missionGroupModelForSpawnPointGroup);
				if (tWDModelResult == TWDModelResult.OK && model.MissionSpawnPointGroup.Category != MapCategory.Endless)
				{
					DropEventDefinition.DropEventType dropEventType = ((model.MissionData.MissionType == MissionType.Rescue) ? DropEventDefinition.DropEventType.MissionRescue : DropEventDefinition.DropEventType.MissionScavenge);
					int targetLevel = model.MissionLevel;
					if (dropEventType == DropEventDefinition.DropEventType.MissionRescue)
					{
						targetLevel = (int)Math.Ceiling((float)model.MissionLevel / 6f);
					}
					DropEventDefinition.DropEventContext context = (model.IsDeadly ? DropEventDefinition.DropEventContext.Deadly : model.DropContext);
					LootEntryGenParams lootParams = new LootEntryGenParams
					{
						eventType = dropEventType,
						targetLevel = targetLevel,
						tag = model.LootTag,
						context = context
					};
					if (tWDModelManager.Player.SurvivorContainer != null && tWDModelManager.Player.SurvivorContainer.CombatSurvivors != null && tWDModelManager.Player.SurvivorContainer.CombatSurvivors.Count > 0)
					{
						SurvivorModel firstSlotSurvivor = tWDModelManager.Player.SurvivorContainer.CombatSurvivors[0];
						AddTraitModifiers(ref lootParams, tWDModelManager.Player, firstSlotSurvivor);
					}
					tWDModelManager.Player.LootManager.ShuffleRewards(lootParams);
				}
			}
			tWDModelManager.Player.LastVisitDebugInfo = "";
			return new NGModelCommandRespond(this, tWDModelResult);
		}

		public static void AddTraitModifiers(ref LootEntryGenParams lootParams, PlayerModel player, SurvivorModel firstSlotSurvivor)
		{
			FixedPoint value = 0.0;
			if (player.AbilityManager.VisitParameter("AbilityModifierIncreaseHigherLevelEquipmentDropChance", ref value, firstSlotSurvivor))
			{
				lootParams.SetTraitModifier("AbilityModifierIncreaseHigherLevelEquipmentDropChance", value, DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon);
			}
			value = 0.0;
			if (player.AbilityManager.VisitParameter("AbilityModifierIncreaseAmountSuppliesDropChance", ref value, firstSlotSurvivor))
			{
				lootParams.SetTraitModifier("AbilityModifierIncreaseAmountSuppliesDropChance", value, DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies);
			}
			value = 0.0;
			if (player.AbilityManager.VisitParameter("AbilityModifierIncreaseAmountXpDropChance", ref value, firstSlotSurvivor))
			{
				lootParams.SetTraitModifier("AbilityModifierIncreaseAmountXpDropChance", value, DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints);
			}
		}
	}
}
