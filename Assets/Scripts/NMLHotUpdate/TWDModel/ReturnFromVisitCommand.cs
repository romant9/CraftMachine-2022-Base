using BaseModel;

namespace TWDModel
{
	public class ReturnFromVisitCommand : ModelCommand
	{
		private string GetYesNo(bool b)
		{
			if (!b)
			{
				return "No";
			}
			return "Yes";
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			CombatModel combat = playerModel.Combat;
			string text = playerModel.SelectedMissionId;
			string text2 = "None";
			if (combat != null && combat.IsPVPMission)
			{
				text = "PVP";
				if (combat.OutpostCombat != null)
				{
					text2 = ((!combat.OutpostCombat.IsFake) ? combat.OutpostCombat.DefenderHashedId : ("FAKE_" + combat.OutpostCombat.DefenderHashedId));
				}
			}
			string yesNo = GetYesNo(combat != null);
			string yesNo2 = GetYesNo(tWDModelManager.Disconnected);
			string text3 = ((combat != null) ? combat.MissionResult.ToString() : "None");
			int debugModelsCount = tWDModelManager.GetDebugModelsCount();
			if (combat != null)
			{
				int count = combat.ExtraSurvivors.Count;
				for (int i = 0; i < count; i++)
				{
					ActorModel actorModel = combat.ExtraSurvivors[i];
					if (!playerModel.SurvivorContainer.Survivors.Contains(actorModel as SurvivorModel) && !playerModel.SurvivorContainer.IsDead(actorModel as SurvivorModel))
					{
						for (int j = 0; j < actorModel.EquipmentItems.Count; j++)
						{
							playerModel.Equipment.RemoveEquipment(actorModel.EquipmentItems[j]);
						}
					}
				}
			}
			tWDModelManager.Player.MapContainerModel.ReturnFromCombat();
			if (combat != null && combat.IsGuildBattleMission)
			{
				tWDModelManager.Player.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.ReturnFromCombat();
			}
			if (!OfflineManager.IsLoadDataManager)
			{
				if ((combat != null && combat.IsPVPMission) || combat.IsFakePVPMission)
				{
					tWDModelManager.Player.ResolvePvPResult();
				}
				if (combat != null)
				{
					tWDModelManager.Player.SaveCombatHistory(combat.IsPVPMission);
				}
				if (tWDModelManager.ServerService != null)
				{
					tWDModelManager.ServerService.Save(SaveType.Player);
				}
			}
			int debugModelsCount2 = tWDModelManager.GetDebugModelsCount();
			tWDModelManager.Player.LastVisitDebugInfo = "EndVisit Mission:" + text + " Result:" + text3 + " Combat:" + yesNo + " Defender:" + text2 + " Disconnected:" + yesNo2 + " Models:" + debugModelsCount + "/" + debugModelsCount2;
			manager.Debug.Log(tWDModelManager.Player.LastVisitDebugInfo);
			if (!OfflineManager.IsLoadDataManager)
			{
				if (combat != null && combat.IsPVPMission && manager.ServerService != null)
				{
					manager.ServerService.FreeVisit();
				}
			}
			((TWDModelManager)manager).DeleteCombatModel();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
