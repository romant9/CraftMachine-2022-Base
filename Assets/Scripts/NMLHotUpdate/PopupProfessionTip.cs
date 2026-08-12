using TWDModel.ContentTypes;
using UnityEngine;

public class PopupProfessionTip : HUDElement
{
	[SerializeField]
	private UILabel bigTitleLabel;

	[SerializeField]
	private UILabel smallTitleLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	public void SetTipContent(EndlessModeGameModeType type)
	{
		switch (type)
		{
		case EndlessModeGameModeType.Normal:
			bigTitleLabel.color = new Color(0.7215686f, 1f, 0.7215686f, 1f);
			smallTitleLabel.color = new Color(0.7215686f, 1f, 0.7215686f, 1f);
			HelpersUI.SetContentToLabel(bigTitleLabel, LocalizationManager.GetText("SurvivalMode_Normal_Help_Title"));
			HelpersUI.SetContentToLabel(smallTitleLabel, LocalizationManager.GetText("SurvivalMode_Normal_Help_Title2"));
			HelpersUI.SetContentToLabel(descriptionLabel, LocalizationManager.GetText("SurvivalMode_Normal_Help_Desc"));
			break;
		case EndlessModeGameModeType.Expert:
			bigTitleLabel.color = new Color(1f, 0f, 0.05490196f, 1f);
			smallTitleLabel.color = new Color(1f, 0f, 0.05490196f, 1f);
			HelpersUI.SetContentToLabel(bigTitleLabel, LocalizationManager.GetText("SurvivalMode_Expert_Help_Title"));
			HelpersUI.SetContentToLabel(smallTitleLabel, LocalizationManager.GetText("SurvivalMode_Expert_Help_Title2"));
			HelpersUI.SetContentToLabel(descriptionLabel, LocalizationManager.GetText("SurvivalMode_Expert_Help_Desc"));
			break;
		}
	}
}
