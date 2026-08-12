using TWDModel;
using UnityEngine;

public class GoreSettingPopup : HUDElement
{
	public GameObject GoreDisabledHighlight;

	public GameObject GoreEnabledHighlight;

	public UIButton GoreDisabledButton;

	public UIButton GoreEnabledButton;

	public UILabel GoreDisabledLabel;

	public UILabel GoreEnabledLabel;

	public UIButton ConfirmButton;

	private GoreSettingPopupSelection GoreSettingPopupSelection;

	public override void Start()
	{
		GoreSettingPopupSelection = GoreSettingPopupSelection.None;
		UpdateUI();
	}

	public override void UpdateUI()
	{
		if (GoreDisabledHighlight != null)
		{
			GoreDisabledHighlight.SetActive(GoreSettingPopupSelection == GoreSettingPopupSelection.Disabled);
		}
		if (GoreDisabledButton != null)
		{
			GoreDisabledButton.defaultColor = ((GoreSettingPopupSelection != GoreSettingPopupSelection.Enabled) ? new Color(1f, 1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f));
			GoreDisabledButton.UpdateColor(instant: true);
		}
		if (GoreDisabledLabel != null)
		{
			GoreDisabledLabel.color = ((GoreSettingPopupSelection != GoreSettingPopupSelection.Enabled) ? new Color(1f, 1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f));
		}
		if (GoreEnabledHighlight != null)
		{
			GoreEnabledHighlight.SetActive(GoreSettingPopupSelection == GoreSettingPopupSelection.Enabled);
		}
		if (GoreEnabledButton != null)
		{
			GoreEnabledButton.defaultColor = ((GoreSettingPopupSelection != GoreSettingPopupSelection.Disabled) ? new Color(1f, 1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f));
			GoreEnabledButton.UpdateColor(instant: true);
		}
		if (GoreEnabledLabel != null)
		{
			GoreEnabledLabel.color = ((GoreSettingPopupSelection != GoreSettingPopupSelection.Disabled) ? new Color(1f, 1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f));
		}
		if (ConfirmButton != null)
		{
			ConfirmButton.gameObject.SetActive(GoreSettingPopupSelection != GoreSettingPopupSelection.None);
		}
	}

	public void OnConfirm()
	{
		Helpers.ExecuteCommand(new ChangeGoreSettingCommand(GoreSettingPopupSelection == GoreSettingPopupSelection.Enabled));
		Close();
		if (AnalyticsManager.instance != null)
		{
			AnalyticsManager.instance.CreateEvent("GoreSettingPopup_Confirm").AddProperty("GoreEnabled", GoreSettingPopupSelection == GoreSettingPopupSelection.Enabled).Send();
		}
	}

	public void OnGoreDisabled()
	{
		GoreSettingPopupSelection = GoreSettingPopupSelection.Disabled;
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_card_click");
		}
		UpdateUI();
	}

	public void OnGoreEnabled()
	{
		GoreSettingPopupSelection = GoreSettingPopupSelection.Enabled;
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_card_click");
		}
		UpdateUI();
	}
}
