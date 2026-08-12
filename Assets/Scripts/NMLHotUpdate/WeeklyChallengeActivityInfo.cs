using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeActivityInfo : HUDElement
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UISprite professionalIcon;

	[SerializeField]
	private UISprite tokenIcon;

	[SerializeField]
	private UILabel tipsTitleLabel1;

	[SerializeField]
	private UILabel tipsDesLabel1;

	[SerializeField]
	private UILabel tipsDesLabel2;

	public override void Open()
	{
		base.Open();
		List<SurvivorClass> classes = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.GetClasses();
		if (classes.Count > 0)
		{
			string survivorClassName = HelpersLocalization.GetSurvivorClassName(classes[0]);
			HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Title", survivorClassName));
			HelpersUI.SetContentToLabel(tipsTitleLabel1, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Tips.Title1", survivorClassName));
			HelpersUI.SetContentToLabel(tipsDesLabel1, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Tips.Desc1", survivorClassName));
			HelpersUI.SetContentToLabel(tipsDesLabel2, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Tips.Desc2", survivorClassName));
			professionalIcon.spriteName = HelpersGfx.GetCurrencyIconName(SurvivorToken.GetClassAsCurrency(classes[0]));
			tokenIcon.spriteName = "UI_Icon_Resource_" + classes[0].ToString() + "Star";
		}
	}
}
