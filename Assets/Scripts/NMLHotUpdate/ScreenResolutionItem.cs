using UnityEngine;

public class ScreenResolutionItem : MonoBehaviour
{
	[SerializeField]
	private UILabel screenResolution;

	private SettingsPopup popup;

	private int screenResolutionKeyIndex;

	public void SetKey(SettingsPopup popup, int keyIndex)
	{
		this.popup = popup;
		screenResolutionKeyIndex = keyIndex;
		if (keyIndex == 0)
		{
			screenResolution.text = LocalizationManager.GetText("Popup.Settings.ScreenResolution.Default.EPIC");
		}
		else
		{
			screenResolution.text = GameManager.ScreenResolutionWidthArray[keyIndex] + "X" + GameManager.ScreenResolutionHeightArray[keyIndex];
		}
	}

	public void OnClick()
	{
		popup.SetScreenResolution(screenResolutionKeyIndex);
	}
}
