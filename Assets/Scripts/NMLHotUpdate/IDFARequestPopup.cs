using TWDModel;
using UnityEngine;

public class IDFARequestPopup : HUDElement
{
	[SerializeField]
	private UILabel titleText;

	[SerializeField]
	private UILabel titleParagraphText1;

	[SerializeField]
	private UILabel titleParagraphText2;

	[SerializeField]
	private UILabel titleParagraphText3;

	[SerializeField]
	private UIButton continueButton;

	public const int TOSPosition = 1;

	public const int TutorialPosition = 2;

	private int openedPosition;

	private void OnEnable()
	{
		string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(GameManager.Instance.gameEconomyData.ConfigData.IDFAPopupHeader);
		HelpersUI.SetContentToLabel(titleText, localizedText);
		if ((bool)titleParagraphText1 && (bool)titleParagraphText2)
		{
			string localizedText2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(GameManager.Instance.gameEconomyData.ConfigData.IDFAPopupParagraph1);
			string localizedText3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(GameManager.Instance.gameEconomyData.ConfigData.IDFAPopupParagraph2);
			HelpersUI.SetContentToLabel(titleParagraphText1, localizedText2);
			HelpersUI.SetContentToLabel(titleParagraphText2, localizedText3);
		}
		string localizedText4 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(GameManager.Instance.gameEconomyData.ConfigData.IDFAPopupParagraph3);
		HelpersUI.SetContentToLabel(titleParagraphText3, localizedText4);
		continueButton.onClick.Add(new EventDelegate(OnButtonContinueClicked));
	}

	private void OnDisable()
	{
		continueButton.onClick.Remove(new EventDelegate(OnButtonContinueClicked));
	}

	public void Initialize(int position)
	{
		openedPosition = position;
		Helpers.ExecuteCommand(new SendIDFAMetricCommand("show", openedPosition));
	}

	private void OnButtonContinueClicked()
	{
		Helpers.ExecuteCommand(new SendIDFAMetricCommand("accept", openedPosition));
		GameManager.Instance.ShowNativeIDFAPopup(openedPosition);
		Close();
	}
}
