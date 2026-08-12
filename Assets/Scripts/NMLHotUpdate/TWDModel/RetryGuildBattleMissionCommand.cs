using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RetryGuildBattleMissionCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			if (player.Combat == null)
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: Combat is null");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			MissionRetryState combatRetryChoicePendingState = player.Combat.CombatRetryChoicePendingState;
			bool isPvPCombat = player.GuildBattlePlayer.AttackTargetMission.IsPvPCombat;
			List<int> collection = (isPvPCombat ? new List<int>(player.GuildBattlePlayer.AttackTargetMission.KilledPVPSurvivorsIndexes) : null);
			string attackMissionId = player.GuildBattlePlayer.AttackTargetMission.AttackMissionId;
			GuildBattleMapMissionModel missionModel = player.GuildWarModel.CurrentBattle.CurrentMapModel.GetMissionModel(attackMissionId);
			if (missionModel == null)
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: Player is not playing a GvG mission");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			GuildModel guildModel = player.GuildModel;
			if (!player.IsGuildMember)
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: Player is not a Guild Member");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (guildModel.GuildWarModel == null)
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: GuildWarModel is null");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (guildModel.GuildWarModel.CurrentBattle == null)
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: CurrentBattle is null");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (guildModel.GuildWarModel.CurrentBattle.CurrentMapModel == null)
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: CurrentMapModel is null");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (combatRetryChoicePendingState != MissionRetryState.Pending)
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: Combat is in an incorrect state");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (!player.GuildBattlePlayer.IsCurrentGuildBattle())
			{
				manager.Debug.LogError("RetryGuildBattleMissionCommand: Retrying a mission from a previous battle");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			SendEndCombatAnalytics(tWDModelManager);
			ReturnFromVisit(tWDModelManager);
			List<int> list = (isPvPCombat ? new List<int>(missionModel.SavedData) : null);
			if (isPvPCombat)
			{
				missionModel.SavedData = new List<int>(collection);
			}
			HealInjuredSurvivors(tWDModelManager);
			GuildBattleModel currentBattle = player.GuildWarModel.CurrentBattle;
			tWDModelResult = player.GuildBattlePlayer.ReplayMission(currentBattle.CurrentMapModel, missionModel, player.SurvivorContainer);
			if (tWDModelResult == TWDModelResult.OK)
			{
				if (list != null)
				{
					player.GuildBattlePlayer.AttackTargetMission.GuildSideKilledPVPSurvivorsIndexes = new List<int>(list);
				}
				DropEventDefinition.DropEventType eventType = DropEventDefinition.DropEventType.MissionScavenge;
				int missionLevel = missionModel.MissionLevel;
				DropEventDefinition.DropEventContext context = DropEventDefinition.DropEventContext.Deadly;
				LootEntryGenParams lootParams = new LootEntryGenParams
				{
					eventType = eventType,
					targetLevel = missionLevel,
					tag = DropEventDefinition.DropEventTag.None,
					context = context
				};
				if (tWDModelManager.Player.SurvivorContainer != null && tWDModelManager.Player.SurvivorContainer.CombatSurvivors != null && tWDModelManager.Player.SurvivorContainer.CombatSurvivors.Count > 0)
				{
					SurvivorModel firstSlotSurvivor = tWDModelManager.Player.SurvivorContainer.CombatSurvivors[0];
					AttackCommand.AddTraitModifiers(ref lootParams, tWDModelManager.Player, firstSlotSurvivor);
				}
			}
			tWDModelManager.Player.LastVisitDebugInfo = "";
			return new NGModelCommandRespond(this, tWDModelResult);
		}

		private void SendEndCombatAnalytics(TWDModelManager twdModelManager)
		{
			CombatModel combatModel = twdModelManager.CombatModel;
			combatModel.MissionStatistics.SetCombatResult(combatModel.PendingCombatResult, combatModel.IsDeadly, isWeeklyChallenge: false, notify: false);
			twdModelManager.Player.ReportMissionStatistics(combatModel.MissionStatistics, null);
			twdModelManager.Metrics.AddEnd().AddMission().AddMissionResult(twdModelManager.Player.Combat.PendingCombatResult)
				.AddGvG()
				.AddGvGBattle()
				.AddGvGPvPInfoIfNeeded()
				.AddCombatFailureReason(combatModel.CombatFailureReason)
				.AddEndCombatAnalyticsSource("RetryGuildBattleMissionCommand")
				.Send();
			for (int i = 0; i < combatModel.MissionRoster.Count; i++)
			{
				SurvivorModel survivorModel = combatModel.MissionRoster[i];
				if (survivorModel == null)
				{
					continue;
				}
				EquipmentItemModel weaponEquipment = survivorModel.GetWeaponEquipment();
				EquipmentItemModel equipmentOfCategory = survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor);
				if (weaponEquipment != null && equipmentOfCategory != null)
				{
					twdModelManager.Metrics.AddEnd().AddMission().AddMissionType()
						.AddGvGBattle();
					if (i == 0)
					{
						twdModelManager.Metrics.AddLeaderEvent(survivorModel);
					}
				}
				combatModel.SupportManager.TryGetSupport(i, out var combatSupportModel);
				twdModelManager.Metrics.AddSurvivor(survivorModel).AddSupportUnit(combatSupportModel?.SupportModel).AddSurvivorResult(survivorModel)
					.AddSupportResult(combatSupportModel)
					.AddEquipmentWeapon(weaponEquipment)
					.AddEquipmentArmor(equipmentOfCategory)
					.AddBadgeList(survivorModel.BadgeContainer.Badges, survivorModel, combatModel.MissionRoster)
					.Send();
			}
			twdModelManager.Metrics.AddFind();
			if (combatModel.MissionStatistics.ActualSuppliesAdded > 0 || combatModel.MissionStatistics.GetSuppliesOverflow() > 0)
			{
				twdModelManager.Metrics.PushResource(CurrencyType.Supplies, combatModel.MissionStatistics.ActualSuppliesAdded, combatModel.MissionStatistics.GetSuppliesOverflow());
			}
			if (combatModel.MissionStatistics.ActualSurvivalPointsAdded > 0 || combatModel.MissionStatistics.GetSPOverflow() > 0)
			{
				twdModelManager.Metrics.PushResource(CurrencyType.SurvivalPoints, combatModel.MissionStatistics.ActualSurvivalPointsAdded, combatModel.MissionStatistics.GetSPOverflow());
			}
			if (twdModelManager.Metrics.metricsResourcesData.HasResources())
			{
				twdModelManager.Metrics.AddResources().AddMission().AddMissionType()
					.AddWalkersKilled()
					.Send();
			}
			twdModelManager.Metrics.Reset();
		}

		private void ReturnFromVisit(TWDModelManager twdModelManager)
		{
			CombatModel combat = twdModelManager.Player.Combat;
			twdModelManager.Player.MapContainerModel.ReturnFromCombat();
			twdModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.ReturnFromCombat();
			if (combat != null)
			{
				twdModelManager.Player.SaveCombatHistory(combat.IsPVPMission);
			}
			twdModelManager.Player.DeleteCombatModel(notify: false, isForRetry: true);
			twdModelManager.ClearDelayedEvents();
		}

		private void HealInjuredSurvivors(TWDModelManager twdModelManager)
		{
			PlayerModel player = twdModelManager.Player;
			if (!(player.Camp.GetBuilding("MedicTent") is MedicTentModel medicTentModel))
			{
				return;
			}
			foreach (SurvivorModel combatSurvivor in player.SurvivorContainer.CombatSurvivors)
			{
				if (medicTentModel.TimedQueueModel.Exists(combatSurvivor))
				{
					TimedQueueItemModel queueItemFromItem = medicTentModel.TimedQueueModel.GetQueueItemFromItem(combatSurvivor);
					medicTentModel.TimedQueueModel.RemoveItemFromList(queueItemFromItem);
				}
			}
		}
	}
}
