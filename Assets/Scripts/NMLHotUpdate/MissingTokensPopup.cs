using TWDModel;
using UnityEngine;

public class MissingTokensPopup : HUDElement
{
	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private UILabel DescriptionLabel;

	[SerializeField]
	private UIButtonWithLabel OkButton;

	public bool IsHeroContent { get; set; }

	public bool IsHeroLocked { get; set; }

	public override void Open()
	{
		base.Open();
		if (OkButton != null)
		{
			OkButton.SetClickCallback(OnClickOk);
			OkButton.SetContentToLabelOne(LocalizationManager.GetText("Popup.MissingTokensPopup.ButtonOk"));
		}
		if (IsHeroContent)
		{
			HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText("Popup.MissingTokensPopup.Hero.Title"));
			if (IsHeroLocked)
			{
				HelpersUI.SetContentToLabel(DescriptionLabel, LocalizationManager.GetText("Popup.MissingTokensPopup.HeroUnlock.Description"));
			}
			else
			{
				HelpersUI.SetContentToLabel(DescriptionLabel, LocalizationManager.GetText("Popup.MissingTokensPopup.Hero.Description"));
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText("Popup.MissingTokensPopup.Survivor.Title"));
			HelpersUI.SetContentToLabel(DescriptionLabel, LocalizationManager.GetText("Popup.MissingTokensPopup.Survivor.Description"));
		}
	}

	public override void Close()
	{
		base.Close();
		Clean();
	}

	private void Clean()
	{
		if (OkButton != null)
		{
			OkButton.Clear();
		}
	}

	private void OnClickOk(UIButtonExtended button)
	{
		OpenForRadioForMissingTokens();
	}

	public static void OpenForRadioForMissingTokens()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.PhoneCall != null && playerModel.PhoneCall.LootsList != null && playerModel.PhoneCall.LootsList.Count > 0)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RadioSelectSurvivorPopup).Open();
		}
		else
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup).Open();
		}
	}
}
