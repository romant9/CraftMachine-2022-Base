using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class AddGuildBattleProgressionGroupCommand : TWDValidationGroupCommand
	{
		public int WarId { get; private set; }

		public int SectorId { get; private set; }

		public string UniqueMissionId { get; private set; }

		public ECombatResult Result { get; private set; }

		public bool PVPMissionPlayed { get; private set; }

		public List<int> SavedData { get; private set; }

		public bool RetriedMission { get; private set; }

		public AddGuildBattleProgressionGroupCommand()
		{
		}

		public AddGuildBattleProgressionGroupCommand(int warId, int sectorId, string uniqueMissionId, bool pvpMissionPlayed, ECombatResult result, bool retriedMission, List<int> savedData)
		{
			WarId = warId;
			SectorId = sectorId;
			UniqueMissionId = uniqueMissionId;
			Result = result;
			PVPMissionPlayed = pvpMissionPlayed;
			SavedData = savedData;
			RetriedMission = retriedMission;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			manager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] Validate START: GroupId={GroupId}, SenderId={SenderId}, MissionId={UniqueMissionId}, Result={Result}");
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("[AddGuildBattleProgressionGroupCommand] ERROR: No Guild found. GroupId: " + GroupId + ", SenderId: " + SenderId + ", MissionId: " + UniqueMissionId);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GuildWarModel == null)
			{
				manager.GvGLogError("[AddGuildBattleProgressionGroupCommand] ERROR: No GuildWarModel. GroupId: " + GroupId + ", SenderId: " + SenderId + ", MissionId: " + UniqueMissionId);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GuildWarModel.CurrentBattle == null)
			{
				manager.GvGLogError("[AddGuildBattleProgressionGroupCommand] ERROR: CurrentBattle is null. GroupId: " + GroupId + ", SenderId: " + SenderId + ", MissionId: " + UniqueMissionId);
				return TWDValidationCommandResult.Error;
			}
			bool flag = guildModel.GuildWarModel.CurrentBattle.HasEnded();
			manager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] Validate: BattleEnded={flag},");
			if (flag)
			{
				manager.GvGLogWarning($"[AddGuildBattleProgressionGroupCommand] SKIP: Battle already ended. GroupId: {GroupId}, SenderId: {SenderId}, MissionId: {UniqueMissionId}, Result: {Result}");
				return TWDValidationCommandResult.Canceled;
			}
			manager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] Validate OK: GroupId={GroupId}, SenderId={SenderId}, MissionId={UniqueMissionId}, Result={Result}");
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			tWDModelManager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] ExecuteInternal START: GroupId={GroupId}, SenderId={SenderId}, MissionId={UniqueMissionId}, Result={Result}");
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			bool flag = false;
			bool validResult = true;
			GuildBattleModel currentBattle = guildModel.GuildWarModel.CurrentBattle;
			GuildBattleMapMissionModel guildBattleMapMissionModel = null;
			if (currentBattle != null && currentBattle.CurrentMapModel != null)
			{
				guildBattleMapMissionModel = currentBattle.CurrentMapModel.GetMissionModel(UniqueMissionId);
				if (guildBattleMapMissionModel != null)
				{
					flag |= guildBattleMapMissionModel.UpdateSaveData(SavedData);
					tWDModelManager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] Updated save data: MissionId={UniqueMissionId}, SavedData count={SavedData?.Count ?? 0}");
				}
				else
				{
					tWDModelManager.Debug.LogWarning("[AddGuildBattleProgressionGroupCommand] Mission not found: MissionId=" + UniqueMissionId);
				}
				flag |= guildModel.GuildWarModel.CurrentBattle.UpdateLiveData(null, SenderId);
			}
			else
			{
				tWDModelManager.Debug.LogWarning($"[AddGuildBattleProgressionGroupCommand] Battle or CurrentMapModel is null: Battle={currentBattle != null}");
			}
			if (Result == ECombatResult.Successful)
			{
				flag = true;
				tWDModelManager.Debug.LogInfo("[AddGuildBattleProgressionGroupCommand] Processing successful result: MissionId=" + UniqueMissionId);
				if (guildBattleMapMissionModel != null && guildBattleMapMissionModel.IsEnemyUnlocked() && !PVPMissionPlayed)
				{
					validResult = false;
					tWDModelManager.Debug.LogInfo("[AddGuildBattleProgressionGroupCommand] Invalid result: enemy unlocked but PVP not played: MissionId=" + UniqueMissionId);
				}
				int num = ((guildBattleMapMissionModel != null) ? guildModel.GuildWarModel.CurrentBattle.GetGuildBattleMissionVictoryPoints(SectorId, PVPMissionPlayed, guildBattleMapMissionModel.AreaIndex) : 0);
				if (RetriedMission)
				{
					int num2 = (int)FixedPoint.Round(num * (tWDModelManager.GameEconomyData.GuildWarConfig.RetryMissionPenalty + 0.0001));
					num -= num2;
					tWDModelManager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] Retry penalty applied: OriginalVP={num + num2}, Penalty={num2}, FinalVP={num}");
				}
				int count = guildModel.GuildWarModel.CurrentBattle.VictoryPointsSectorRewardPerSector.Count;
				string text = guildModel.GuildWarModel.AddProgressionToMission(tWDModelManager, WarId, SectorId, UniqueMissionId, SenderId, validResult, num);
				int count2 = guildModel.GuildWarModel.CurrentBattle.VictoryPointsSectorRewardPerSector.Count;
				if (!string.IsNullOrEmpty(text))
				{
					tWDModelManager.GvGLogError("[AddGuildBattleProgressionGroupCommand] AddProgressionToMission FAILED: MissionId=" + UniqueMissionId + ", Error=" + text);
				}
				else
				{
					tWDModelManager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] AddProgressionToMission SUCCESS: MissionId={UniqueMissionId}, VP={num}, CompletedSectorsBefore={count}, CompletedSectorsAfter={count2}");
					guildModel.UpdateGuildBattleLeaderboards(tWDModelManager, SenderId, guildModel.Id, guildModel.Name, battleEnd: false, updateMembers: true, count != count2);
					if (tWDModelManager.ServerService != null && tWDModelManager.GetPlayer().HashedId == SenderId)
					{
						tWDModelManager.Metrics.AddFind().AddGuildVictoryPointsResources(num).AddMission()
							.AddGvG(fromPlayer: false)
							.AddGvGBattle(fromPlayer: false)
							.Send();
					}
				}
			}
			else
			{
				tWDModelManager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] Result not successful, skipping VP processing: MissionId={UniqueMissionId}, Result={Result}");
			}
			tWDModelManager.Debug.LogInfo($"[AddGuildBattleProgressionGroupCommand] ExecuteInternal END: GroupId={GroupId}, MissionId={UniqueMissionId}, NeedsSave={flag}");
			return flag;
		}
	}
}
