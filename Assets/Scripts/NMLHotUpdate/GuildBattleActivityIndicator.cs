using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleActivityIndicator : MonoBehaviour
{
	private const int MaxSelfStaleLiveMissionDataReportCount = 5;

	private static readonly Dictionary<string, int> reportedSelfStaleLiveMissionDataCounts = new Dictionary<string, int>();

	public void SectorActivityIndicatorCheck(GuildBattleModel currentBattleModel, GuildBattleMapSectorModel currentSectorModel)
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
		if (currentBattleModel.HasEnded() || currentSectorModel == null || currentSectorModel.IsCompleted())
		{
			return;
		}
		foreach (KeyValuePair<string, GuildBattleModel.LiveMissionData> item in currentBattleModel.LiveMissionDataPerPlayer)
		{
			if (item.Value.LastAttackedMissionId != null)
			{
				GuildBattleMapMissionModel missionModel = GuildWarHelper.GetCurrentMapModel().GetMissionModel(item.Value.LastAttackedMissionId);
				if (missionModel != null && missionModel.SectorIdOwner == currentSectorModel.SectorId && !missionModel.IsCompleted())
				{
					Helpers.GameObjectSetActive(base.gameObject, value: true);
					break;
				}
			}
		}
	}

	public void MissionActivityIndicatorCheck(GuildBattleModel currentBattleModel, GuildBattleMapMissionModel currentMissionModel, bool isPvPButton)
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
		if (currentBattleModel.HasEnded() || currentMissionModel == null)
		{
			return;
		}
		foreach (KeyValuePair<string, GuildBattleModel.LiveMissionData> item in currentBattleModel.LiveMissionDataPerPlayer)
		{
			if (!(item.Value.LastAttackedMissionId == currentMissionModel.Id))
			{
				continue;
			}
			if (currentMissionModel.Type == GuildBattleMapMissionModel.MissionType.PVP)
			{
				if (isPvPButton && item.Value.MissionState == GuildBattleMapMissionModel.MissionState.PVP)
				{
					Helpers.GameObjectSetActive(base.gameObject, value: true);
					ReportSelfStaleLiveMissionDataIfNeeded(currentBattleModel, currentMissionModel, item, isPvPButton);
				}
			}
			else
			{
				Helpers.GameObjectSetActive(base.gameObject, value: true);
				ReportSelfStaleLiveMissionDataIfNeeded(currentBattleModel, currentMissionModel, item, isPvPButton);
			}
		}
	}

	private void ReportSelfStaleLiveMissionDataIfNeeded(GuildBattleModel currentBattleModel, GuildBattleMapMissionModel currentMissionModel, KeyValuePair<string, GuildBattleModel.LiveMissionData> liveMissionData, bool isPvPButton)
	{
		if (currentBattleModel == null || currentMissionModel == null || liveMissionData.Value == null || GameManager.Instance == null)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null || liveMissionData.Key != playerModel.HashedId || (playerModel.Combat != null && playerModel.Combat.IsGuildBattleMission))
		{
			return;
		}
		string key = currentBattleModel.BattleId + ":" + currentBattleModel.TimeSlot + ":" + currentMissionModel.Id + ":" + liveMissionData.Value.MissionState.ToString() + ":" + isPvPButton;
		int value = 0;
		reportedSelfStaleLiveMissionDataCounts.TryGetValue(key, out value);
		if (value < 5)
		{
			reportedSelfStaleLiveMissionDataCounts[key] = value + 1;
			ReportClientErrorCommand reportClientErrorCommand = new ReportClientErrorCommand();
			reportClientErrorCommand.Level = ReportClientErrorCommand.LogLevel.Error;
			reportClientErrorCommand.Message = "GuildBattleActivityIndicator: self stale live mission data caused activity indicator. PlayerHashedId=" + playerModel.HashedId + ", BattleId=" + currentBattleModel.BattleId + ", TimeSlot=" + currentBattleModel.TimeSlot + ", MissionId=" + currentMissionModel.Id + ", MissionType=" + currentMissionModel.Type.ToString() + ", MissionState=" + liveMissionData.Value.MissionState.ToString() + ", IsPvPButton=" + isPvPButton + ", ReportCount=" + (value + 1);
			reportClientErrorCommand.GuildBattleActivityIndicatorMissionId = currentMissionModel.Id;
			if (GameManager.Instance.modelManager != null)
			{
				Helpers.ExecuteCommandDelayed(reportClientErrorCommand);
			}
			else
			{
				Debug.LogError("Client Error: " + reportClientErrorCommand.Message);
			}
		}
	}

	public void CheckIfMissionIsOccupied(GuildBattleModel currentBattleModel, GuildBattleMapMissionModel currentMissionModel, bool isPvPButton, Callback okCallback)
	{
		if ((currentBattleModel.HasEnded() && !HelpersModel.IsUnlockPVP) || currentMissionModel == null)
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, GuildBattleModel.LiveMissionData> item in currentBattleModel.LiveMissionDataPerPlayer)
		{
			if (!(item.Value.LastAttackedMissionId == currentMissionModel.Id))
			{
				continue;
			}
			if (currentMissionModel.Type == GuildBattleMapMissionModel.MissionType.PVP)
			{
				if (isPvPButton && item.Value.MissionState == GuildBattleMapMissionModel.MissionState.PVP)
				{
					dictionary.Add(item.Key, GameManager.Instance.playerModel.GuildModel.GetMemberInfo(item.Key).Name);
				}
			}
			else
			{
				dictionary.Add(item.Key, GameManager.Instance.playerModel.GuildModel.GetMemberInfo(item.Key).Name);
			}
		}
		if (dictionary.Count > 0)
		{
			var parent = OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainerTopCameras : null;
			GuildBattleSelectMissionConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleSelectMissionConfirmationPopup, parent) as GuildBattleSelectMissionConfirmationPopup;
			obj.SetCallbacks(okCallback);
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.EnterAnyway"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.ChooseAnother"));
			obj.SetPlayersList(dictionary);
			obj.AddListeners(currentMissionModel);
			obj.Open();
		}
		else
		{
			okCallback();
		}
	}
}
