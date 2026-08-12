using TWDModel;
using UnityEngine;

public class TokenConversionPopup : ConfirmationPopup
{
	[SerializeField]
	private UILabel tokenMeterValueLabel;

	[SerializeField]
	private UILabel buttonDiamondAmountLabel;

	[SerializeField]
	private UISprite buttonSprite;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel info;

	[SerializeField]
	private UIButton confirmButton;

	[SerializeField]
	private UIButton declineButton;

	private Callback confirmCallback;

	private Callback cancelCallback;

	private int animAmount;

	private bool isConfirmed;

	public void OpenForCurrency(int convertedAmount)
	{
		buttonDiamondAmountLabel.text = convertedAmount.ToString();
		buttonSprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Diamonds);
		title.text = LocalizationManager.GetText("SpeedupToken.Popup.Title.MaxAmount");
		info.text = LocalizationManager.GetText("SpeedupToken.Popup.Text.MaxAmountExchange", convertedAmount);
		Open();
		animAmount = convertedAmount;
		isConfirmed = false;
		SetButtonsResponsive(responsive: true);
	}

	public override void Open()
	{
		base.Open();
	}

	public void SetConversionCallbacks(Callback confirmCallback = null, Callback cancelCallback = null)
	{
		this.confirmCallback = confirmCallback;
		this.cancelCallback = cancelCallback;
	}

	public void ConfirmPressed()
	{
		EventManager.NotifyClick("ConvertToGoldPressed");
		SetButtonsResponsive(responsive: false);
		isConfirmed = true;
		RewardCurrency rewardCurrency = new RewardCurrency();
		rewardCurrency.CurrencyType = CurrencyType.Diamonds;
		rewardCurrency.Amount = animAmount;
		CampView.Instance.BuildingsHud.CreateCollectAnim(rewardCurrency.CurrencyType, null, rewardCurrency.Amount);
		base.Close();
		if (confirmCallback != null)
		{
			confirmCallback();
		}
	}

	public override void Close()
	{
		if (!isConfirmed)
		{
			EventManager.NotifyClick("CancelConversionToGold");
			base.Close();
			if (cancelCallback != null)
			{
				cancelCallback();
			}
		}
	}

	private void SetButtonsResponsive(bool responsive)
	{
		HelpersUI.SetButtonState(confirmButton, (!responsive) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		HelpersUI.SetButtonState(declineButton, (!responsive) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		if (confirmButton.TryGetComponent<BoxCollider>(out var component))
		{
			component.enabled = responsive;
		}
		if (declineButton.TryGetComponent<BoxCollider>(out var component2))
		{
			component2.enabled = responsive;
		}
	}
}
