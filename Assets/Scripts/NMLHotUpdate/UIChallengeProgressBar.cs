using TWDModel;
using UnityEngine;

public class UIChallengeProgressBar : UIProgressBarExtended
{
	[SerializeField]
	private enum ProgressType
	{
		Personal = 0,
		Guild = 1
	}

	[Header("Personal or Guild")]
	[SerializeField]
	private ProgressType showProgress;

	private void Awake()
	{
		DebugIdString = "UIChallengeProgressBar";
	}

	public override void OnEnable()
	{
		base.OnEnable();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled && WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
		{
			WeeklyChallengeReward weeklyChallengeReward = null;
			WeeklyChallengeReward weeklyChallengeReward2 = null;
			if (showProgress == ProgressType.Personal)
			{
				weeklyChallengeReward = WeeklyChallengeHelper.GetNextReward(personal: true);
				weeklyChallengeReward2 = WeeklyChallengeHelper.GetLastReward(personal: true);
				num = WeeklyChallengeHelper.GetWeeklyChallengeModel().NumberStars;
			}
			else if (showProgress == ProgressType.Guild)
			{
				weeklyChallengeReward = WeeklyChallengeHelper.GetNextReward(personal: false);
				weeklyChallengeReward2 = WeeklyChallengeHelper.GetLastReward(personal: false);
				num = WeeklyChallengeHelper.GetWeeklyChallengeModel().NumberStarsGuild;
			}
			if (weeklyChallengeReward != null)
			{
				num2 = weeklyChallengeReward.Control;
			}
			if (weeklyChallengeReward2 != null)
			{
				num3 = weeklyChallengeReward2.Control;
			}
			if (num2 <= 0)
			{
				num2 = num;
			}
			if (progressBar != null)
			{
				progressBar.value = Mathf.InverseLerp(num3, num2, num);
			}
			HelpersUI.SetContentToLabel(progressBarLabel, num + "/" + num2);
			if (showProgress == ProgressType.Guild)
			{
				Helpers.GameObjectSetActive(base.gameObject, GameManager.Instance.playerModel.IsGuildMember);
			}
			else
			{
				Helpers.GameObjectSetActive(base.gameObject, value: true);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}

	public override void Clear()
	{
		base.Clear();
	}
}
