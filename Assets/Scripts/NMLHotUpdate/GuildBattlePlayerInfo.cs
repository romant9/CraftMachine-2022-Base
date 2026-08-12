using TWDModel;
using UnityEngine;

public class GuildBattlePlayerInfo : MonoBehaviourExtended
{
	public GuildBattleMapMissionModel Model;

	public bool IsEnemy;

	[SerializeField]
	private UILabel enemyPlayerNameLabel;

	[SerializeField]
	private UILabel enemyPlayerGuildLabel;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	public void UpdateUI()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildBattleParticipantInfo value = null;
		if (IsEnemy)
		{
			GuildBattlePvpTeam pvpTeamForMission = playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(Model.Id);
			if (pvpTeamForMission != null)
			{
				value = playerModel.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission);
			}
			HelpersUI.SetContentToLabel(enemyPlayerGuildLabel, GameManager.Instance.GetFilteredText(GuildWarHelper.GetCurrentOpponentGuildName()));
		}
		else
		{
			playerModel.GuildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot.TryGetValue(playerModel.HashedId, out value);
			HelpersUI.SetContentToLabel(enemyPlayerGuildLabel, GameManager.Instance.GetFilteredText(playerModel.GuildModel.Name));
		}
		if (value != null)
		{
			HelpersUI.SetContentToLabel(enemyPlayerNameLabel, GameManager.Instance.GetFilteredText(value.Name));
			if (playerEmblemIcon != null)
			{
				playerEmblemIcon.SetEmblem(value.PlayerEmblem);
			}
		}
	}

	#region mycode
	private void Start()
	{
		if (!OfflineManager.IsLoadDataManager) return;
		EnemyTeamInfoPopup popup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.EnemyTeamInfoPopup) as EnemyTeamInfoPopup;
		if (popup != null && popup.IsOpen)
		{
			var offset = new Vector3(122, 20, 0);
			enemyPlayerNameLabel.transform.localPosition += offset;
			enemyPlayerGuildLabel.transform.localPosition += offset;
			if (playerEmblemIcon) playerEmblemIcon.transform.localPosition += offset;
		}
	}
	#endregion
}
