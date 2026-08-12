using UnityEngine;

public class MissionObjectiveStar : MonoBehaviour
{
	public UILabel starLabel;

	public GameObject starAchieved;

	public void SetStar(MissionStarCondition starCondition, bool achieved)
	{
		starLabel.text = LocalizationManager.GetText("Map.Star.Condition." + starCondition.Type.ToString() + "{Parameter}", starCondition.Parameter);
		starAchieved.SetActive(achieved);
	}
}
