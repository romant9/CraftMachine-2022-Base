using TWDModel;
using UnityEngine;

public class CombatEndFlowStar : MonoBehaviour
{
	[SerializeField]
	private UILabel starLabel;

	[SerializeField]
	private GameObject CompletedParent;

	[SerializeField]
	private GameObject doubleStarReward;

	[SerializeField]
	private UISprite starSprite;

	public void Awake()
	{
		if (starLabel != null)
		{
			Helpers.GameObjectSetActive(starLabel.gameObject, value: false);
		}
		Helpers.GameObjectSetActive(CompletedParent, value: false);
	}

	public void SetStar(int starIndex, bool achieved = false)
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		MapMissionModel attackTargetMissionModel = GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionModel;
		MapMissionStars stars = attackTargetMissionModel.Stars;
		MissionData missionData = attackTargetMissionModel.MissionData;
		HelpersUI.SetSprite(starSprite, GetStarSprite(attackTargetMissionModel));
		if (stars != null && stars.Stars.Length > starIndex && missionData != null && starLabel != null)
		{
			CombatModel combat = GameManager.Instance.playerModel.Combat;
			bool value = combat != null && combat.MissionResult == ECombatResult.Successful && stars.Stars[starIndex];
			Helpers.GameObjectSetActive(CompletedParent, value);
			Helpers.GameObjectSetActive(starLabel.gameObject, value: true);
			MissionStarCondition[] conditions = missionData.MissionStarConditions.Conditions;
			starLabel.text = LocalizationManager.GetText("Map.Star.Condition." + conditions[starIndex].Type.ToString() + "{Parameter}", conditions[starIndex].Parameter);
			if (doubleStarReward != null && attackTargetMissionModel.IsInWeeklyChallenge)
			{
				NGUITools.SetActive(doubleStarReward, GameManager.Instance.playerModel.WeeklyChallenge.DoubleRewardsActive);
			}
		}
		else if (base.gameObject != null)
		{
			Debug.LogWarning("CombatEndFlowStar: Problems in updating star visiblity in obj: " + base.gameObject);
		}
	}

	private string GetStarSprite(MapMissionModel attackTargetMissionModel)
	{
		if (attackTargetMissionModel != null && attackTargetMissionModel.IsInApocalyptiWeeklyChallenge)
		{
			return "Ui_Mission_Star_Large_Apocalyptic_2";
		}
		return "Ui_Icon_Reward_Star";
	}
}
