using TWDModel;
using UnityEngine;

public class EnemyTeamInfoPopup : HUDElement
{
	[SerializeField]
	[Tooltip("The panel of the selected team.")]
	private EnemyTeamSelectionSurvivorPanel enemyTeamSelectionSelectedSurvivorPanel;

	[SerializeField]
	private GuildBattlePlayerInfo playerInfo;

	public override void Open()
	{
		base.Open();
		GuildBattleMapMissionModel groupModel = GetGroupModel<GuildBattleMapMissionModel>();
		if (groupModel != null)
		{
			playerInfo.Model = groupModel;
			playerInfo.UpdateUI();
			enemyTeamSelectionSelectedSurvivorPanel.Model = groupModel;
			enemyTeamSelectionSelectedSurvivorPanel.UpdateSlots();
		}
	}
}
