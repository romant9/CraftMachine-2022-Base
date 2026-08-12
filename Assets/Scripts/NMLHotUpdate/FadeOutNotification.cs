using TWDModel;
using UnityEngine;

public class FadeOutNotification : HUDElement
{
	[SerializeField]
	private GameObject victoryFadeObject;

	[SerializeField]
	private GameObject failureFadeObject;

	[SerializeField]
	private GameObject outOfTimeFadeObject;

	[SerializeField]
	private GameObject pvpVictoryFadeOut;

	public void PlayFadeOut(ECombatResult combatResult, bool outOfTime, EventDelegate.Callback callback = null)
	{
		bool hasPvPRules = GameManager.Instance.playerModel.Combat.HasPvPRules;
		if (outOfTime)
		{
			outOfTimeFadeObject.SetActive(value: true);
			victoryFadeObject.SetActive(value: false);
			failureFadeObject.SetActive(value: false);
			pvpVictoryFadeOut.SetActive(value: false);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/combat_end_fade_defeat");
		}
		else
		{
			switch (combatResult)
			{
			case ECombatResult.Successful:
				if (hasPvPRules)
				{
					pvpVictoryFadeOut.SetActive(value: true);
					victoryFadeObject.SetActive(value: false);
					failureFadeObject.SetActive(value: false);
					outOfTimeFadeObject.SetActive(value: false);
				}
				else
				{
					victoryFadeObject.SetActive(value: true);
					failureFadeObject.SetActive(value: false);
					outOfTimeFadeObject.SetActive(value: false);
					pvpVictoryFadeOut.SetActive(value: false);
				}
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/combat_end_fade_victory");
				break;
			case ECombatResult.Failed:
				failureFadeObject.SetActive(value: true);
				victoryFadeObject.SetActive(value: false);
				outOfTimeFadeObject.SetActive(value: false);
				pvpVictoryFadeOut.SetActive(value: false);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/combat_end_fade_defeat");
				break;
			}
		}
		base.gameObject.SetActive(value: true);
		TweenManager.PlayTweenGroup(base.gameObject, 0, forward: true, callback);
	}
}
