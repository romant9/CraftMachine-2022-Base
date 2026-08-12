using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleSelectMissionConfirmationPopup : ConfirmationPopup
{
	[SerializeField]
	private List<GuildBattlePlayerLabel> playerLabels;

	[SerializeField]
	private UILabel morePlayersLabel;

	private GuildBattleMapMissionModel missionModel;

	private readonly string morePeopleInside = "GvG.EnterMissionConfirmationPopup.More";

	public void SetPlayersList(Dictionary<string, string> playersInMission)
	{
		Init();
		int num = 0;
		foreach (KeyValuePair<string, string> item in playersInMission)
		{
			if (num == playerLabels.Count)
			{
				HelpersUI.SetContentToLabel(morePlayersLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(morePeopleInside, playersInMission.Count - num));
				break;
			}
			GuildBattleParticipantInfo participantInfo = GameManager.Instance.playerModel.GuildModel.GuildBattleMatchmakingInfo.GetParticipantInfo(item.Key);
			if (participantInfo == null)
			{
				break;
			}
			playerLabels[num].SetPlayerData(item.Value, participantInfo.PlayerEmblem);
			Helpers.GameObjectSetActive(playerLabels[num], value: true);
			num++;
		}
	}

	private void Init()
	{
		for (int i = 0; i < playerLabels.Count; i++)
		{
			Helpers.GameObjectSetActive(playerLabels[i], value: false);
		}
		Helpers.GameObjectSetActive(morePlayersLabel, value: false);
	}

	public void AddListeners(GuildBattleMapMissionModel model)
	{
		missionModel = model;
		GuildBattleMapModel currentMapModel = GuildWarHelper.GetCurrentMapModel();
		if (currentMapModel != null)
		{
			currentMapModel.Changed -= OnBattleModelChange;
			currentMapModel.Changed += OnBattleModelChange;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnBattleModelChange;
			guildWarModel.Changed += OnBattleModelChange;
		}
	}

	private void RemoveListenters()
	{
		GuildBattleMapModel currentMapModel = GuildWarHelper.GetCurrentMapModel();
		if (currentMapModel != null)
		{
			currentMapModel.Changed -= OnBattleModelChange;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnBattleModelChange;
		}
	}

	private void OnBattleModelChange(TWDGroupModelChild model, string changed, object args)
	{
		if (((changed == "GuildBattleNonPvpCompletionAdded" || changed == "GuildBattlePvpCompletionAdded") && (missionModel.IsCompleted() || missionModel.IsMissionPveComplete())) || missionModel.SectorModelOwner.IsCompleted())
		{
			Close();
		}
	}

	private void OnDisable()
	{
		RemoveListenters();
	}
}
