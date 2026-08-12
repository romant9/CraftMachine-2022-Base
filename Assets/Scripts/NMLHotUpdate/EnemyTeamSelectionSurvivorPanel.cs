using TWDModel;
using UnityEngine;

public class EnemyTeamSelectionSurvivorPanel : MonoBehaviourExtended
{
	[SerializeField]
	private SurvivorCard[] survivorCards;

	[Tooltip("The GameObject that will contain the slots.")]
	public GameObject container;

	public GuildBattleMapMissionModel Model { get; set; }

	public void UpdateSlots()
	{
		if (Model == null || !Model.IsEnemyUnlocked())
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildBattlePvpTeam pvpTeamForMission = playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(Model.Id);
		playerModel.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission);
		if (pvpTeamForMission == null)
		{
			return;
		}
		for (int i = 0; i < survivorCards.Length; i++)
		{
			SurvivorMockData survivorModel = pvpTeamForMission.Survivors[i];
			SurvivorModel survivorModel2 = playerModel.SurvivorContainer.CreateSurvivorFromSurvivorMockData(survivorModel, GvGModelHelper.GetPlayerSpecificDifficulty(playerModel), preview: true);
			if (!OfflineManager.IsLoadDataManager)
				ActorView.PrepareActor(survivorModel2, isTransient: true);
			SurvivorCard survivorCard = survivorCards[i];
			survivorCard.Item = survivorModel2;
			if (OfflineManager.IsLoadDataManager)
				survivorCard.IsProtector = true;
			survivorCard.Locked = false;
            bool isDead = !HelpersModel.IsUnlockAllSectors && Model.SavedData.Contains(i);
            survivorCard.IsOutOfAction = isDead;
			survivorCard.IsMissionSurvivor = false;
			survivorCard.IsSurvivalMode = false;
			survivorCard.IsGuildWarMode = true;
			survivorCard.EnableEquipmentContainers(enable: true);
			survivorCard.Type = SurvivorCard.CardType.EnemyPreview;
			survivorCard.UpdateUI();
			survivorCard.ShowTeamSelection(LocalizationManager.GetText("Popup.TeamSelection.TapToReplace"));
			if (i == 0)
			{
				survivorCard.SetLeaderTraitVisual(visible: true);
			}
			else
			{
				survivorCard.SetLeaderTraitVisual(visible: false);
			}

			if (OfflineManager.IsLoadDataManager)
			{
				SurvivorStatisticsPanel survivorPanel = survivorCard.transform.GetComponentInChildren<SurvivorStatisticsPanel>();
				if (survivorPanel != null)
				{
					survivorPanel.SetInfo(survivorModel2);
				}
			}
		}
	}
}
