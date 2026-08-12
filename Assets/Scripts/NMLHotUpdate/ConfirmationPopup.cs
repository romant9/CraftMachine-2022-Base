using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ConfirmationPopup : HUDElement
{
	[SerializeField]
	protected UILabel titleLabel;

	[SerializeField]
	protected UILabel infoLabel;

	[SerializeField]
	private UILabel customColorInfoLabel;

	[SerializeField]
	private UILabel okButtonLabel;

	[SerializeField]
	private UILabel cancelButtonLabel;

	[SerializeField]
	private UITable currenciesTable;

	[SerializeField]
	private UITable[] currencyContainers;

	[SerializeField]
	private UILabel[] currencyAmountLabels;

	[SerializeField]
	private UISprite[] currencySprites;

	[SerializeField]
	private GameObject closeArea;

	protected Callback okCallback;

	private Callback cancelCallback;

	private const int maxCurrencies = 3;

	public override void Open()
	{
		base.Open();
		EnableCloseArea(enable: true);
	}

	public void SetDebugText()
	{
		infoLabel.maxLineCount = 10;
		infoLabel.width = 650;
		customColorInfoLabel.maxLineCount = 10;
		customColorInfoLabel.width = 650;
	}

	public void SetContent(string title, string info, bool useCustomColor = false)
	{
		UILabel uILabel = (useCustomColor ? customColorInfoLabel : infoLabel);
		UILabel uILabel2 = (useCustomColor ? infoLabel : customColorInfoLabel);
		if (title != null && titleLabel != null)
		{
			titleLabel.text = title;
		}
		if (info != null && uILabel != null)
		{
			LocalizationUIUpdater component = uILabel.GetComponent<LocalizationUIUpdater>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			uILabel.text = info;
			uILabel.gameObject.SetActive(value: true);
		}
		if (uILabel2 != null)
		{
			uILabel2.gameObject.SetActive(value: false);
		}
		SetCurrencies();
	}

	public virtual void SetMissingCurrencies(Cashier cashier, bool showDiamonds = true)
	{
		List<CurrencyType> list = new List<CurrencyType>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < (int)CurrencyType.Count; i++)
		{
			CurrencyType currencyType = (CurrencyType)i;
			if (currencyType == CurrencyType.Diamonds && !showDiamonds)
			{
				continue;
			}
			int num = Mathf.Abs(cashier.GetMissing(currencyType));
			if (num > 0)
			{
				if (currencyType == CurrencyType.ReplayToken)
				{
					CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.ReplayToken);
					num = currency.Max - currency.Value;
				}
				if (list.Count < 3)
				{
					list.Add(currencyType);
					list2.Add(num);
				}
			}
		}
		SetCurrencies(list, list2);
	}

	public void SetCurrencies(Cashier cashier)
	{
		List<CurrencyType> list = new List<CurrencyType>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < (int)CurrencyType.Count; i++)
		{
			CurrencyType currencyType = (CurrencyType)i;
			int num = Mathf.Abs(cashier.GetTotalCost(currencyType));
			if (num > 0 && list.Count < 3)
			{
				list.Add(currencyType);
				list2.Add(num);
			}
		}
		SetCurrencies(list, list2);
	}

	public void SetOkButtonLabel(string text)
	{
		if (okButtonLabel != null)
		{
			if (string.IsNullOrEmpty(text))
			{
				text = LocalizationManager.GetText("Button.Ok");
			}
			okButtonLabel.text = text;
		}
	}

	public void SetCancelButtonLabel(string text)
	{
		if (cancelButtonLabel != null)
		{
			if (string.IsNullOrEmpty(text))
			{
				text = LocalizationManager.GetText("Button.Cancel");
			}
			cancelButtonLabel.text = text;
		}
	}

	public virtual void OkPressed()
	{
		EventManager.NotifyClick("OkPressed");
		cancelCallback = null;
		base.Close();
		if (okCallback != null)
		{
			okCallback();
		}
	}

	public override void Close()
	{
		base.Close();
		if (cancelCallback != null)
		{
			cancelCallback();
		}
	}

	public void CancelPressed()
	{
		Close();
	}

	public void SetCallbacks(Callback okCallback = null, Callback cancelCallback = null)
	{
		this.okCallback = okCallback;
		this.cancelCallback = cancelCallback;
	}

	public void SetCurrencies(List<CurrencyType> currencyTypes = null, List<int> currencyAmounts = null)
	{
		for (int i = 0; i < 3 && i < currencyContainers.Length; i++)
		{
			if (currencyContainers[i] != null)
			{
				currencyContainers[i].gameObject.SetActive(currencyTypes != null && i < currencyTypes.Count);
			}
		}
		if (currencyTypes == null || currencyAmounts == null)
		{
			return;
		}
		for (int j = 0; j < currencyTypes.Count; j++)
		{
			if (currencySprites.Length > j && currencySprites[j] != null)
			{
				currencySprites[j].spriteName = HelpersGfx.GetCurrencyIconName(currencyTypes[j]);
			}
			if (currencyAmountLabels.Length > j && currencyAmountLabels[j] != null)
			{
				currencyAmountLabels[j].text = Helpers.FormatNumber(currencyAmounts[j]);
			}
		}
		UITable[] array = currencyContainers;
		foreach (UITable uITable in array)
		{
			if (!(uITable == null))
			{
				uITable.Reposition();
			}
		}
		if (currenciesTable != null)
		{
			currenciesTable.Reposition();
		}
	}

	public void EnableCloseArea(bool enable)
	{
		if (closeArea != null)
		{
			closeArea.SetActive(enable);
		}
	}

	public void TextOnlyResize()
	{
		infoLabel.height = 110;
		infoLabel.transform.localPosition = new Vector3(0f, 25f);
	}

	public static void ShowPopup(string title, string message, string okButtonLabel, Callback okCallback, string cancelButtonLabel = null, Callback cancelCallback = null)
	{
		ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		obj.SetContent(title, message);
		obj.TextOnlyResize();
		obj.SetOkButtonLabel(okButtonLabel);
		obj.SetCancelButtonLabel(cancelButtonLabel);
		obj.SetCallbacks(okCallback, cancelCallback);
		obj.Open();
	}

	public static void ShowPopupGetText(string title, string message, string okButtonLabel, Callback okCallback)
	{
		ShowPopup(LocalizationManager.GetText(title), LocalizationManager.GetText(message), LocalizationManager.GetText(okButtonLabel), okCallback);
	}
}
