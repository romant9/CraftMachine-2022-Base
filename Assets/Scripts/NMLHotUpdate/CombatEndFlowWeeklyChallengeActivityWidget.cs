using TWDModel;
using UnityEngine;

public class CombatEndFlowWeeklyChallengeActivityWidget : CombatEndWidget
{
	[SerializeField]
	private UILabel TextLabel;

	[SerializeField]
	private UISprite CurrencyIconSprite;

	public override void Awake()
	{
		base.Awake();
		DebugClassString = "CombatEndFlowWeeklyChallengeActivityWidget";
		CurrencyType starCurrencyType = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.StarCurrencyType;
		string survivorClassName = HelpersLocalization.GetSurvivorClassName(GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.GetClasses()[0]);
		string currencyIconName = HelpersGfx.GetCurrencyIconName(starCurrencyType);
		if (TextLabel != null)
		{
			HelpersUI.SetContentToLabel(TextLabel, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Reward", survivorClassName));
		}
		if (CurrencyIconSprite != null)
		{
			CurrencyIconSprite.spriteName = currencyIconName;
		}
	}
}
