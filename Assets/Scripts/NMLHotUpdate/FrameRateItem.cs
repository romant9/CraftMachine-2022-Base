using UnityEngine;

public class FrameRateItem : MonoBehaviour
{
	[SerializeField]
	private UILabel frameRate;

	private SettingsPopup popup;

	private int frameRateIndex;

	public void SetKey(SettingsPopup popup, int keyIndex)
	{
		this.popup = popup;
		frameRateIndex = keyIndex;
		frameRate.text = GameManager.Instance.FrameRateArray[keyIndex];
	}

	public void OnClick()
	{
		popup.SetFrameRate(frameRateIndex);
	}
}
