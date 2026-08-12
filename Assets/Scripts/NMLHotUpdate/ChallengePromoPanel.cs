using TWDModel;
using UnityEngine;

public class ChallengePromoPanel : MonoBehaviour
{
	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private UILabel MessageLabel;

	public void UpdateWithData(WeeklyChallengeModel WeeklyChallenge)
	{
		if (WeeklyChallenge != null && TitleLabel != null && MessageLabel != null)
		{
			_ = base.gameObject.activeSelf;
		}
	}
}
