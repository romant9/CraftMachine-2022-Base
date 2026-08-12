using UnityEngine;
using UnityEngine.UIElements;

public class PhoneMaxNotificationPanel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel MainLabel;

	[SerializeField]
	private UITexture BuildingTexture;

	public void UpdateUI()
	{
		int maxStartingLevelForSurvivor = HelpersBuilding.GetMaxStartingLevelForSurvivor(max: false);
		int maxStartingLevelForSurvivor2 = HelpersBuilding.GetMaxStartingLevelForSurvivor(max: true);
		string text = ((maxStartingLevelForSurvivor != maxStartingLevelForSurvivor2) ? (maxStartingLevelForSurvivor + "-" + maxStartingLevelForSurvivor2) : maxStartingLevelForSurvivor.ToString());
		HelpersUI.SetContentToLabel(MainLabel, LocalizationManager.GetText("Popup.StartPhoneCall.LevelMessage{Level}", text));
	}


	#region myparams
	[SerializeField]
	private UILabel SecondaryLabel;
	[SerializeField]
	public GameObject CheckCallObject;
	[SerializeField]
	private UILabel CheckCallLabel;
	#endregion

	#region mycode
	public void UpdateUICall(string text, bool isIncrement = false)
	{
		if (string.IsNullOrEmpty(text)) return;

		if (CheckCallLabel != null)
		{
			if (isIncrement)
			{
				CheckCallLabel.text += '\n' + text;
			}
			else
			{
				MainLabel.gameObject.SetActive(false);
				SecondaryLabel.gameObject.SetActive(false);
				CheckCallObject.SetActive(true);
				HelpersUI.SetContentToLabel(CheckCallLabel, text);
			}

			ResetScrollView(true);
		}
	}

	public void ResetUICall()
	{
		MainLabel.gameObject.SetActive(false);
		SecondaryLabel.gameObject.SetActive(false);
		CheckCallObject.SetActive(true);
		HelpersUI.SetContentToLabel(CheckCallLabel, "");
	}

	public void ResetScrollView(bool isInverted = false)
	{
		if (isInverted) CheckCallLabel.GetComponent<UIDragScrollView>().scrollView.ResetPositionInverted();
		else CheckCallLabel.GetComponent<UIDragScrollView>().scrollView.ResetPosition();
	}
	#endregion
}
