using TWDModel;
using UnityEngine;

public class GuildBattleMissionInfoEnemyPlayer : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel enemyPlayerNameLabel;

	[SerializeField]
	private UISprite[] teamIcon;

	[SerializeField]
	private GameObject[] deadIndicators;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	[SerializeField]
	private GameObject playerDefeatedIndicator;

	public GuildBattleMapMissionModel Model { get; set; }

	public void UpdateUI()
	{
		if (Model == null || !Model.IsEnemyUnlocked())
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildBattlePvpTeam pvpTeamForMission = playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(Model.Id);
		if (pvpTeamForMission == null)
		{
			return;
		}
		GuildBattleParticipantInfo currentGuildBattlePlayerInfo = playerModel.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission);
		HelpersUI.SetContentToLabel(enemyPlayerNameLabel, GameManager.Instance.GetFilteredText(currentGuildBattlePlayerInfo.Name));
		if (playerEmblemIcon != null)
		{
			playerEmblemIcon.SetEmblem(currentGuildBattlePlayerInfo.PlayerEmblem);
		}
		for (int i = 0; i < 3; i++)
		{
			SurvivorMockData survivorModel = pvpTeamForMission.Survivors[i];
			SurvivorModel survivorModel2 = playerModel.SurvivorContainer.CreateSurvivorFromSurvivorMockData(survivorModel, GvGModelHelper.GetPlayerSpecificDifficulty(playerModel), preview: true);
			if (teamIcon[i] != null)
			{
				teamIcon[i].spriteName = HelpersGfx.GetSurvivorClassIconName(survivorModel2);
				if (i < deadIndicators.Length)
				{
					bool isDead = !HelpersModel.IsUnlockAllSectors && Model.SavedData.Contains(i);
					Helpers.GameObjectSetActive(deadIndicators[i], isDead);
				}
			}
		}
		bool isDefeated = !HelpersModel.IsUnlockAllSectors && Model.SavedData.Count >= 3;
		Helpers.GameObjectSetActive(playerDefeatedIndicator, isDefeated);
	}

	public void OnClickedInfoButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		GameObject parent = OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainerTopCameras : null;
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EnemyTeamInfoPopup, parent) as EnemyTeamInfoPopup).OpenForModel(Model);
		EventManager.NotifyClick("SelectTeam");
	}
}
