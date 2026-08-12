using TWDModel;
using UnityEngine;

public class SurvivalSkillContainer : MonoBehaviour
{
	[SerializeField]
	private SurvivalSkillList infoList;

	public void UpdateUI(ActorModel actor)
	{
		infoList.InitData(actor);
	}
}
