using TWDModel;
using UnityEngine;

public class SpeedupPopup : ConfirmationPopup
{
	private Callback cancelCallback;

	private Callback tokenCallback;

	private Callback goldCallback;

	[SerializeField]
	private PayButton payButton;

	[SerializeField]
	private PayButton useTokenButton;

	[SerializeField]
	private HUDMeter diamondMeter;

	private Cashier goldCashier;

	private Cashier tokenCashier;

	private UIButton payUIButton;

	private UIButton useTokenUIButton;

	private int diamondAmount;

	public ConsumeCurrencyCommand consumeCurrencyCommand;

	private CurrencyType currencyTokenType;

	public override void Open()
	{
		base.Open();
		SetPayButtonEnabled(enable: true);
		SetUseTokenButtonEnabled(enable: true);
	}

	public virtual void UseTokenPressed()
	{
		if (!IsHaveAnySpeedUpToken())
		{
			TooltipManager.OpenTextBoxWithText(useTokenUIButton.gameObject, LocalizationManager.GetText("Tooltip.BattlePass.SpeedupToken.Missing"));
		}
		else
		{
			OpenSpeedupPopupTwo();
		}
	}

	public virtual void UseGoldPressed()
	{
		EventManager.NotifyClick("GoldPressed");
		cancelCallback = null;
		tokenCallback = null;
		Close();
		if (goldCallback != null && goldCashier.CanAfford())
		{
			goldCallback();
		}
		else
		{
			ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(diamondAmount);
		}
	}

	public virtual void SetContent(string title, string info, int amount, CurrencyType currencyType = CurrencyType.Diamonds)
	{
		SetContent(title, info);
		diamondAmount = amount;
		goldCashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.None, CurrencyType.Diamonds, amount);
		payButton.UpdateUI(goldCashier);
		tokenCashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.None, currencyType, 1);
		useTokenButton.UpdateUI(tokenCashier);
		currencyTokenType = currencyType;
	}

	public void SetSpeedupCallbacks(Callback tokenCallback = null, Callback goldCallback = null, Callback cancelCallback = null)
	{
		this.tokenCallback = tokenCallback;
		this.goldCallback = goldCallback;
		this.cancelCallback = cancelCallback;
	}

	private void SetPayButtonEnabled(bool enable)
	{
		if (payUIButton == null)
		{
			payUIButton = payButton.GetComponent<UIButton>();
		}
		HelpersUI.SetButtonState(payUIButton, (!enable) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
	}

	private void SetUseTokenButtonEnabled(bool enable)
	{
		if (useTokenUIButton == null)
		{
			useTokenUIButton = useTokenButton.GetComponent<UIButton>();
		}
		HelpersUI.SetButtonState(useTokenUIButton, (!enable) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
	}

	private void OpenSpeedupPopupTwo()
	{
		UIType uiType = UIType.SpeedupPopupTwo;
		SpeedupPopupTwo speedupPopupTwo = SingularityMonoBehaviour<HUDManager>.Instance.Get(uiType) as SpeedupPopupTwo;
		if (speedupPopupTwo != null)
		{
			speedupPopupTwo.consumeCurrencyCommand = consumeCurrencyCommand;
			speedupPopupTwo.InitData();
			speedupPopupTwo.Open();
		}
		cancelCallback = null;
		tokenCallback = null;
		Close();
	}

	private bool IsHaveAnySpeedUpToken()
	{
		switch (currencyTokenType)
		{
		case CurrencyType.BuildingTokenBP:
			GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken10min);
			GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken1h);
			GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken6h);
			GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken12h);
			GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken24h);
			GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingTokenBP);
			if (GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken10min).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken1h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken6h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken12h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken24h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingTokenBP).Value > 0)
			{
				return true;
			}
			break;
		case CurrencyType.TrainingTokenBP:
			if (GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken20min).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken1h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken3h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken8h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken16h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingTokenBP).Value > 0)
			{
				return true;
			}
			break;
		case CurrencyType.EquipmentTokenBP:
			if (GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken20min).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken1h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken3h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken7h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken14h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentTokenBP).Value > 0)
			{
				return true;
			}
			break;
		case CurrencyType.HealingTokenBP:
			if (GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken10min).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken1h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken2h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken4h).Value > 0 || GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingTokenBP).Value > 0)
			{
				return true;
			}
			break;
		}
		return false;
	}
}
