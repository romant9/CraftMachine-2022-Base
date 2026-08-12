using TWDModel;
using UnityEngine;

public class BuyEnergyPopup : BuyResourcesPopup
{
	[SerializeField]
	private UILabel minutesAmount;

	[SerializeField]
	private UILabel minutesLabel;

	[SerializeField]
	private UILabel secondsAmount;

	[SerializeField]
	private UILabel secondsLabel;

	[SerializeField]
	private UILabel currentAmount;

	[SerializeField]
	private UILabel currentLabel;

	[SerializeField]
	private UILabel moreLabel;

	[SerializeField]
	private UILabel fullGasPriceLabel;

	[SerializeField]
	private UISprite fullGasPriceBackground;

	[SerializeField]
	private UISprite popupBackground;

	[SerializeField]
	private int popupWidthWithGasBundle = 855;

	[SerializeField]
	private int popupWidthWithoutGasBundle = 600;

	[SerializeField]
	private UITable buttonTable;

	[SerializeField]
	private GameObject fullGasBundleButton;

	[SerializeField]
	private UILabel payJustForMissionCost;

	[SerializeField]
	private Color notEnoughColor;

	private Cashier cashierRef;

	private CurrencyModel tokensCurrency;

	private BundleStoreDefinition fullGasBundleStoreDefinition;

	private BundleContentDefinition fullGasBundleContentDefinition;

	public static int MissionCostGold;

	public static bool IsPayOnlyMissionCostActive;

	public override void Open()
	{
		base.Open();
		if (minutesLabel != null)
		{
			minutesLabel.text = LocalizationManager.GetText("Popup.BuyEnergy.Minutes");
		}
		if (secondsLabel != null)
		{
			secondsLabel.text = LocalizationManager.GetText("Popup.BuyEnergy.Seconds");
		}
		if (moreLabel != null)
		{
			moreLabel.text = LocalizationManager.GetText("Popup.BuyEnergy.MoreGas");
		}
		if (currentLabel != null)
		{
			currentLabel.text = LocalizationManager.GetText("Popup.BuyEnergy.CurrentGas");
		}
		setFuelTime("", "");
		setFuelAmount("", "");
		string outOfGasPopupBundle = GameManager.Instance.gameEconomyData.ConfigData.OutOfGasPopupBundle;
		fullGasBundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(outOfGasPopupBundle);
		fullGasBundleContentDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleContentDefinition(outOfGasPopupBundle);
		bool flag = fullGasBundleStoreDefinition != null && GameManager.Instance.playerModel.BundleManager.CanBuyBundle(fullGasBundleStoreDefinition);
		fullGasBundleButton.SetActive(flag);
		buttonTable.Reposition();
		popupBackground.width = (flag ? popupWidthWithGasBundle : popupWidthWithoutGasBundle);
		UpdateFullGasPriceLabel();
		tokensCurrency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.ReplayToken);
	}

	public void OnBuyFullGasBundle()
	{
		GameManager.Instance.BundleSource = Metrics.BundleSource.MissionStart;
		GameManager.Instance.IAPManager.Buy(fullGasBundleStoreDefinition, fullGasBundleContentDefinition);
	}

	public override void SetContent(string title, string info, int amount, CurrencyType currencyType = CurrencyType.Diamonds)
	{
		title = LocalizationManager.GetText("Popup.BuyEnergy.Title");
		info = LocalizationManager.GetText("Popup.BuyEnergy.Info");
		base.SetContent(title, info, amount, currencyType);
	}

	public override void SetMissingCurrencies(Cashier cashier, bool showDiamonds)
	{
		base.SetMissingCurrencies(cashier, showDiamonds);
		cashierRef = cashier;
		if (payJustForMissionCost != null)
		{
			int missing = cashierRef.GetMissing(CurrencyType.ReplayToken);
			MissionCostGold = GameManager.Instance.gameEconomyData.ConfigData.ReplayTokensRechargePrice * missing;
			payJustForMissionCost.text = MissionCostGold.ToString();
			payJustForMissionCost.color = ((GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Diamonds) >= MissionCostGold) ? Color.white : notEnoughColor);
		}
	}

	public override void Update()
	{
		base.Update();
		if (cashierRef != null && cashierRef.GetMissing(CurrencyType.ReplayToken) <= 0)
		{
			Close();
		}
		if (tokensCurrency != null)
		{
			setFuelTime(Helpers.FormatTime(tokensCurrency.MillisecondsToNextRecharge, Helpers.TimeFormat.MinutesOnly), Helpers.FormatTime(tokensCurrency.MillisecondsToNextRecharge, Helpers.TimeFormat.SecondsOnly));
			setFuelAmount(tokensCurrency.Value.ToString() ?? "", tokensCurrency.Max.ToString() ?? "");
		}
		else
		{
			setFuelTime("00", "00");
		}
		if (!fullGasPriceLabel.isActiveAndEnabled && GameManager.Instance.IAPManager.IsInitialized())
		{
			UpdateFullGasPriceLabel();
		}
	}

	private void setFuelTime(string min, string sec)
	{
		if (minutesAmount != null && secondsAmount != null)
		{
			minutesAmount.text = min;
			secondsAmount.text = sec;
		}
	}

	private void setFuelAmount(string amount, string max)
	{
		if (currentAmount != null)
		{
			currentAmount.text = amount + "/" + max;
		}
	}

	private void UpdateFullGasPriceLabel()
	{
		if (fullGasPriceLabel != null && fullGasBundleContentDefinition != null)
		{
			if (GameManager.Instance.IAPManager.IsInitialized())
			{
				fullGasPriceLabel.text = GameManager.Instance.IAPManager.GetFormattedPrice(fullGasBundleContentDefinition.IAPProduct);
				Helpers.GameObjectSetActive(fullGasPriceLabel, value: true);
				Helpers.GameObjectSetActive(fullGasPriceBackground, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(fullGasPriceLabel, value: false);
				Helpers.GameObjectSetActive(fullGasPriceBackground, value: false);
			}
		}
	}

	public void BuyOnlyEnoughGasForMission()
	{
		IsPayOnlyMissionCostActive = true;
		OkPressed();
	}
}
