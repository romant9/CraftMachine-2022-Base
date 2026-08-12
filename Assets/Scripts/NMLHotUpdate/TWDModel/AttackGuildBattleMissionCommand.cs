using BaseModel;

namespace TWDModel
{
	public class AttackGuildBattleMissionCommand : ConsumeCurrencyCommand
	{
		public string UniqueMissionId { get; set; }

		public int SectorId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			tWDModelManager.Player.MapContainerModel.ClearAttackTargetMissionData();
			GuildModel guildModel = player.GuildModel;
			if (!player.IsGuildMember)
			{
				manager.Debug.LogError("AttackGuildBattleMissionCommand: Player is not a Guild Member");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (guildModel.GuildWarModel == null)
			{
				manager.Debug.LogError("AttackGuildBattleMissionCommand: GuildWarModel is null");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (guildModel.GuildWarModel.CurrentBattle == null)
			{
				manager.Debug.LogError("AttackGuildBattleMissionCommand: CurrentBattle is null");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (guildModel.GuildWarModel.CurrentBattle.CurrentMapModel == null)
			{
				manager.Debug.LogError("AttackGuildBattleMissionCommand: CurrentMapModel is null");
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			GuildBattleModel currentBattle = player.GuildWarModel.CurrentBattle;
			GuildBattleMapSectorModel sectorModel = currentBattle.CurrentMapModel.GetSectorModel(SectorId);
			GuildBattleMapMissionModel missionModel = currentBattle.CurrentMapModel.GetMissionModel(UniqueMissionId);
			if (missionModel == null)
			{
				manager.Debug.LogError("AttackGuildBattleMissionCommand: MapMission is null :" + UniqueMissionId + " " + SectorId);
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (sectorModel == null)
			{
				manager.Debug.LogError("AttackGuildBattleMissionCommand: MapSector is null :" + UniqueMissionId + " " + SectorId);
				tWDModelResult = TWDModelResult.Error;
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			tWDModelResult = player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.AttackMission(currentBattle.CurrentMapModel, missionModel, sectorModel, player.SurvivorContainer);
			if (tWDModelResult == TWDModelResult.OK)
			{
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
	}
}
