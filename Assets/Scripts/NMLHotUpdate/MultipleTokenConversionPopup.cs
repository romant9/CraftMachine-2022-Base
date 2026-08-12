using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWDModel;
using UnityEngine;

public class MultipleTokenConversionPopup : ConfirmationPopup
{
	private UILabel buttonDiamondAmountLabel;

	[SerializeField]
	private UISprite buttonSprite;

	[SerializeField]
	private UIButton confirmButton;

	[SerializeField]
	private UILabel confirmButtonLabel;

	[SerializeField]
	private UILabel confirmWithoutDeclineButtonLabel;

	[SerializeField]
	private UIButton declineButton;

	[Header("Button containers")]
	[SerializeField]
	private GameObject canConvertLaterContainer;

	[SerializeField]
	private GameObject noLaterConversionContainer;

	[Header("Conversion List Elements")]
	[SerializeField]
	private UILabel conversionCurrencyLabel;

	[SerializeField]
	private UILabel convertedTotalsLabel;

	private List<TokenConversionData> tokens = new List<TokenConversionData>();

	private Callback confirmCallback;

	private Callback cancelCallback;

	private int totalConversionAmount;

	private bool isConfirmed;

	public void OpenForCurrencies(List<RewardCurrency> unclaimedRewards, bool shouldDisplayDeclineOption = true)
	{
		tokens = new List<TokenConversionData>();
		foreach (RewardCurrency reward in unclaimedRewards)
		{
			if (GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(reward.CurrencyType))
			{
				int num = GameManager.Instance.gameEconomyData.CurrencyToDiamonds(reward.CurrencyType, reward.Amount);
				TokenConversionData tokenConversionData = tokens.FirstOrDefault((TokenConversionData x) => x.CurrencyType == reward.CurrencyType);
				if (tokenConversionData != null)
				{
					tokenConversionData.Amount += reward.Amount;
					tokenConversionData.TotalConvertedAmount += num;
				}
				else
				{
					TokenConversionData item = new TokenConversionData(reward.CurrencyType, reward.Amount, num, num);
					tokens.Add(item);
				}
			}
		}
		totalConversionAmount = 0;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		foreach (TokenConversionData token in tokens)
		{
			stringBuilder.AppendLine(token.Amount + " " + HelpersLocalization.GetBattlePassTokenName(token.CurrencyType));
			stringBuilder2.AppendLine(token.TotalConvertedAmount + " " + LocalizationManager.GetText("Currency.Diamonds"));
			totalConversionAmount += token.TotalConvertedAmount;
		}
		if (Helpers.GameObjectSetActive(canConvertLaterContainer, shouldDisplayDeclineOption))
		{
			buttonDiamondAmountLabel = confirmButtonLabel;
		}
		if (Helpers.GameObjectSetActive(noLaterConversionContainer, !shouldDisplayDeclineOption))
		{
			buttonDiamondAmountLabel = confirmWithoutDeclineButtonLabel;
		}
		conversionCurrencyLabel.text = stringBuilder.ToString();
		convertedTotalsLabel.text = stringBuilder2.ToString();
		if (buttonDiamondAmountLabel != null)
		{
			buttonDiamondAmountLabel.text = totalConversionAmount.ToString();
		}
		buttonSprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Diamonds);
		SetButtonsResponsive(responsive: true);
		Open();
		isConfirmed = false;
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

	private void SetButtonsResponsive(bool responsive)
	{
		HelpersUI.SetButtonState(confirmButton, (!responsive) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		if (confirmButton.TryGetComponent<BoxCollider>(out var component))
		{
			component.enabled = responsive;
		}
		HelpersUI.SetButtonState(declineButton, (!responsive) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		if (declineButton.TryGetComponent<BoxCollider>(out var component2))
		{
			component2.enabled = responsive;
		}
	}

	public void ConfirmPressed()
	{
		EventManager.NotifyClick("ConvertToGoldPressed");
		isConfirmed = true;
		SetButtonsResponsive(responsive: false);
		RewardCurrency rewardCurrency = new RewardCurrency();
		rewardCurrency.Amount = totalConversionAmount;
		rewardCurrency.CurrencyType = CurrencyType.Diamonds;
		CampView.Instance?.BuildingsHud.CreateCollectAnim(rewardCurrency.CurrencyType, confirmButton.gameObject, rewardCurrency.Amount);
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
}
