using BaseModel;
using TWDModel;
using UnityEngine;

public class PayButton : MonoBehaviour
{
	[SerializeField]
	private GameObject currency1Container;

	[SerializeField]
	private GameObject currency2Container;

	[SerializeField]
	private GameObject twoCurrenciesContainer;

	[SerializeField]
	private GameObject currency3Container;

	[SerializeField]
	private UILabel currency1AmountLabel;

	[SerializeField]
	private UILabel currency2AmountLabel;

	[SerializeField]
	private UILabel twoCurrencies1AmountLabel;

	[SerializeField]
	private UILabel twoCurrencies2AmountLabel;

	[SerializeField]
	private UILabel currency3AmountLabel;

	[SerializeField]
	private UISprite currency1Sprite;

	[SerializeField]
	private UISprite currency2Sprite;

	[SerializeField]
	private UISprite currency3Sprite;

	[SerializeField]
	private UISprite twoCurrencies1Sprite;

	[SerializeField]
	private UISprite twoCurrencies2Sprite;

	[SerializeField]
	[Tooltip("The GameObject that says you can use gold if you're out of the primary resource.")]
	private GameObject useGoldContainer;

	[SerializeField]
	private UILabel buildTimeLabel;

	[Tooltip("The label that says free if the item is free.")]
	[SerializeField]
	private UILabel freeLabel;

	[Tooltip("The label of the button.")]
	[SerializeField]
	private UILabel label;

	[SerializeField]
	[Tooltip("The label of the button if there is a time label.")]
	private UILabel labelWithTime;

	[SerializeField]
	[Tooltip("The background of the button. The one that is anchored to the label of the button.")]
	private UISprite labelButtonBackground;

	[SerializeField]
	[Tooltip("The height of the background of the label when there is no time in the button.")]
	private int labelButtonBackgroundHeightWhenNoTime;

	[Tooltip("The tint color for labels when the user has enough currency.")]
	[SerializeField]
	private Color availableCurrencyColor = Color.white;

	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	[SerializeField]
	private Color unavailableCurrencyColor = new Color(0.511f, 0.129f, 0.027f, 1f);

	private Cashier cashier;

	private string labelText;

	private int upgradeTime;

	private void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed += OnPlayerChanged;
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed -= OnPlayerChanged;
		}
	}

	private void OnPlayerChanged(ModelObject model, string changed, object args)
	{
		//if (OfflineManager.IsLoadDataManager && OfflineManager.IsBatch) return;

		if (changed == "currencyChangedEvent")
		{
			UpdateUI(cashier, labelText, upgradeTime);
		}
	}

	public void UpdateUI(Cashier cashier, string labelText = null, int upgradeTime = -1, CurrencyType[] currenciesOrder = null, bool twoCurrenciesPayment = false)
	{
		if (cashier == null)
		{
			return;
		}
		this.cashier = cashier;
		this.labelText = labelText;
		this.upgradeTime = upgradeTime;
		int num = 0;
		if (currency3Container != null)
		{
			currency3Container.SetActive(value: false);
		}
		if (currency2Container != null)
		{
			currency2Container.SetActive(value: false);
		}
		if (currency1Container != null)
		{
			currency1Container.SetActive(value: false);
		}
		if (twoCurrenciesContainer != null)
		{
			twoCurrenciesContainer.SetActive(value: false);
		}
		bool active = true;
		if (currenciesOrder != null)
		{
			for (int i = 0; i < currenciesOrder.Length; i++)
			{
				int totalCost = cashier.GetTotalCost(currenciesOrder[i]);
				if (totalCost > 0)
				{
					active = false;
					SetCurrencyData(num, currenciesOrder[i], totalCost);
					num++;
				}
			}
		}
		else if (twoCurrenciesPayment)
		{
			CurrencyType[] array = new CurrencyType[2];
			int[] array2 = new int[2];
			int num2 = 0;
			for (int j = 0; j < (int)CurrencyType.Count; j++)
			{
				CurrencyType currencyType = (CurrencyType)j;
				int totalCost2 = cashier.GetTotalCost(currencyType);
				if (totalCost2 > 0)
				{
					active = false;
					array[num2] = currencyType;
					array2[num2] = totalCost2;
					num2++;
				}
				if (num2 == 2)
				{
					SetCurrencyData(array, array2);
					break;
				}
			}
		}
		else
		{
			for (int k = 0; k < (int)CurrencyType.Count; k++)
			{
				CurrencyType currencyType2 = (CurrencyType)k;
				int totalCost3 = cashier.GetTotalCost(currencyType2);
				if (totalCost3 > 0)
				{
					active = false;
					SetCurrencyData(num, currencyType2, totalCost3);
					num++;
				}
			}
		}
		bool flag = false;
		if (buildTimeLabel != null)
		{
			buildTimeLabel.gameObject.SetActive(upgradeTime > 0);
			if (upgradeTime > 0)
			{
				flag = true;
				buildTimeLabel.text = Helpers.FormatTime(ModelHelpers.SecondsToMilliSeconds(upgradeTime));
			}
		}
		if (labelText != null)
		{
			if (label != null)
			{
				label.gameObject.SetActive(value: true);
				label.text = labelText;
			}
			if (labelWithTime != null)
			{
				labelWithTime.text = labelText;
				labelWithTime.gameObject.SetActive(flag);
				label.gameObject.SetActive(!flag);
			}
		}
		if (freeLabel != null)
		{
			freeLabel.gameObject.SetActive(active);
		}
	}

	private void SetCurrencyData(int numberCurrencies, CurrencyType currencyType, int cost)
	{
		GameObject gameObject = ((numberCurrencies <= 0) ? currency1Container : ((numberCurrencies > 1) ? currency3Container : currency2Container));
		UILabel uILabel = ((numberCurrencies <= 0) ? currency1AmountLabel : ((numberCurrencies > 1) ? currency3AmountLabel : currency2AmountLabel));
		UISprite uISprite = ((numberCurrencies <= 0) ? currency1Sprite : ((numberCurrencies > 1) ? currency3Sprite : currency2Sprite));
		if (uILabel != null)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: true);
			}
			if (OfflineManager.IsFreeAll || cashier.CanPay(currencyType) || (OfflineManager.IsLoadDataManager && currencyType == CurrencyType.GvGMissionKey))
			{
				DebugTWD.LogMycode("...|| (OfflineManager.IsLoadDataManager && currencyType == CurrencyType.GvGMissionKey) || OfflineManager.IsFreeAll)");
				uILabel.color = availableCurrencyColor;
				Helpers.GameObjectSetActive(useGoldContainer, value: false);
			}
			else
			{
				uILabel.color = unavailableCurrencyColor;
				Helpers.GameObjectSetActive(useGoldContainer, cashier.CanConvertToDiamonds());
			}
			uILabel.text = Helpers.FormatNumber(cost);
			if (uILabel == currency1AmountLabel)
			{
				radioPrice = cost;
			}
		}
		if (uISprite != null)
		{
			uISprite.spriteName = HelpersGfx.GetCurrencyIconName(HelpersGfx.GetSPCurrencyType_N(currencyType));
		}
	}

	private void SetCurrencyData(CurrencyType[] currencyTypes, int[] cost)
	{
		if (currencyTypes.Length == 2 && cost.Length == 2)
		{
			Helpers.GameObjectSetActive(twoCurrenciesContainer, value: true);
			Helpers.GameObjectSetActive(useGoldContainer, cashier.CanPay(currencyTypes[0]) && cashier.CanPay(currencyTypes[1]));
			HelpersUI.SetContentToLabel(twoCurrencies1AmountLabel, Helpers.FormatNumber(cost[0]));
			HelpersUI.SetColor(twoCurrencies1AmountLabel, cashier.CanPay(currencyTypes[0]) ? availableCurrencyColor : unavailableCurrencyColor);
			twoCurrencies1Sprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyTypes[0]);
			HelpersUI.SetContentToLabel(twoCurrencies2AmountLabel, Helpers.FormatNumber(cost[1]));
			HelpersUI.SetColor(twoCurrencies2AmountLabel, cashier.CanPay(currencyTypes[1]) ? availableCurrencyColor : unavailableCurrencyColor);
			twoCurrencies2Sprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyTypes[1]);
		}
	}



	#region myparams
	public int radioPrice;
	#endregion
}
