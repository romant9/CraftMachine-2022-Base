using TWDModel;
using UnityEngine;

public class GvGRewardsUI : MonoBehaviour
{
	[SerializeField]
	private UILabel victoryPointsLabel;

	[SerializeField]
	private UILabel RewardPointsLabel;

	[SerializeField]
	private UILabel extraVictoryPointsLabel;

	[SerializeField]
	private UILabel extraRewardPointsLabel;

	public void SetupForMission(GuildBattleMapMissionModel missionModel)
	{
		if (missionModel == null)
		{
			Helpers.GameObjectSetActive(this, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(this, value: true);
		}
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		int guildBattleMissionVictoryPoints = currentBattle.GetGuildBattleMissionVictoryPoints(missionModel.SectorModelOwner.SectorId, missionModel.IsEnemyUnlocked(), missionModel.AreaIndex);
		int personalGuildBattleMissionRewardPoints = currentBattle.GetPersonalGuildBattleMissionRewardPoints(missionModel.SectorModelOwner.SectorId, missionModel.IsEnemyUnlocked(), missionModel.AreaIndex);
		int num = 0;
		int num2 = 0;
		HelpersUI.SetContentToLabel(victoryPointsLabel, guildBattleMissionVictoryPoints.ToString());
		HelpersUI.SetContentToLabel(RewardPointsLabel, personalGuildBattleMissionRewardPoints.ToString());
		HelpersUI.SetContentToLabel(extraVictoryPointsLabel, "+" + num, num > 0);
		HelpersUI.SetContentToLabel(extraRewardPointsLabel, "+" + num2, num2 > 0);
	}
}
