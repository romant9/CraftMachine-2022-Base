using System.Text;
using TWDModel;
using UnityEngine;

public class TooltipChallengeReward : TooltipTextbox
{
	[Tooltip("Star amount")]
	[SerializeField]
	private UILabel starValueLabel;

	[Tooltip("Desc of reward trade crates")]
	[SerializeField]
	private UILabel rewardTradeCrateLabel;

	[Tooltip("Preview of currency rewards")]
	[SerializeField]
	private RewardIcon rewardIcon;

	private StringBuilder builder;

	[SerializeField]
	private UILabel OverSpeedConvertedTxt;

	protected override void Deactivate()
	{
		base.Deactivate();
		if (builder != null)
		{
			builder.Length = 0;
			builder = null;
		}
	}

	public void UpdateWithParams(WeeklyChallengeReward weeklyChallengeReward, int starValue, int OverSpeedConvertedAmount = 0)
	{
		SetText(LocalizationManager.GetText("WeeklyChallenge.TradeCrateReward.Tootip.Title"));
		if (weeklyChallengeReward != null && weeklyChallengeReward.RewardEntries != null && weeklyChallengeReward.RewardEntries.RewardsList.Count > 0 && weeklyChallengeReward.RewardEntries.RewardsList[0] != null)
		{
			HelpersUI.SetContentToLabel(starValueLabel, starValue.ToString());
			IReward reward = weeklyChallengeReward.RewardEntries.RewardsList[0];
			UpdateCommon(reward, OverSpeedConvertedAmount);
		}
	}

	public void UpdateCommon(IReward reward, int OverSpeedConvertedAmount)
	{
		if (builder == null)
		{
			builder = new StringBuilder();
		}
		SetText(LocalizationManager.GetText("WeeklyChallenge.TradeCrateReward.Tootip.Title"));
		if (reward == null)
		{
			return;
		}
		if (reward is RewardCurrency || reward is RewardSkipChallange)
		{
			rewardIcon.SetReward(reward);
			if (OverSpeedConvertedAmount > 0)
			{
				Helpers.GameObjectSetActive(OverSpeedConvertedTxt, value: true);
				HelpersUI.SetContentToLabel(OverSpeedConvertedTxt, LocalizationManager.GetText("SpeedupToken.Popup.Text.MaxAmountExchangeChallenge", OverSpeedConvertedAmount));
			}
			else
			{
				Helpers.GameObjectSetActive(OverSpeedConvertedTxt, value: false);
			}
			Helpers.GameObjectSetActive(rewardIcon, value: true);
			Helpers.GameObjectSetActive(rewardTradeCrateLabel, value: false);
		}
		else if (reward is RewardEquipment)
		{
			rewardIcon.SetReward(reward);
			Helpers.GameObjectSetActive(rewardIcon, value: true);
			Helpers.GameObjectSetActive(rewardTradeCrateLabel, value: false);
		}
		else if (reward is RewardTradeCrate && builder != null)
		{
			Helpers.GameObjectSetActive(rewardIcon, value: false);
			builder.Length = 0;
			builder.Append("WeeklyChallenge.TradeCrateReward.Tootip.");
			builder.Append((reward as RewardTradeCrate).TradeCrateId);
			HelpersUI.SetContentToLabel(rewardTradeCrateLabel, LocalizationManager.GetText(builder.ToString()));
		}
		else if (reward is RewardAvatars rewardAvatars && builder != null)
		{
			Helpers.GameObjectSetActive(rewardIcon, value: false);
			builder.Length = 0;
			if (rewardAvatars.Avatar >= 0)
			{
				builder.Append("Popup.EmblemCreator.Icon");
			}
			else if (rewardAvatars.Border >= 0)
			{
				builder.Append("Popup.EmblemCreator.Border");
			}
			HelpersUI.SetContentToLabel(rewardTradeCrateLabel, LocalizationManager.GetText(builder.ToString()));
		}
	}
}
