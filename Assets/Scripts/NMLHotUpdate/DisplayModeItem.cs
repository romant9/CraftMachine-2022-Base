using UnityEngine;

public class DisplayModeItem : MonoBehaviour
{
	[SerializeField]
	private UILabel displayMode;

	private SettingsPopup popup;

	private int displayModeKeyIndex;

	public void SetKey(SettingsPopup popup, int keyIndex)
	{
		this.popup = popup;
		displayModeKeyIndex = keyIndex;
		displayMode.text = LocalizationManager.GetText(GameManager.Instance.DisplayModeKeyArray[keyIndex]);
	}

	public void OnClick()
	{
		popup.SetDisplayMode(displayModeKeyIndex);
	}
}
