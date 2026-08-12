using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivalConfirmationPopup : HUDElement
{
	[SerializeField]
	protected UILabel titleLabel;

	[SerializeField]
	private UILabel infoLabel1;

	[SerializeField]
	private UILabel infoLabel2;

	[SerializeField]
	private UILabel okButtonLabel;

	[SerializeField]
	private UILabel cancelButtonLabel;

	[SerializeField]
	private GameObject currency1Container;

	[SerializeField]
	private UILabel currency1AmountLabel;

	[SerializeField]
	private UISprite currency1Sprite;

	[SerializeField]
	private GameObject closeArea;

	[SerializeField]
	private UILabel difficultyLabel;

	[SerializeField]
	private UISprite difficultyLabelBg;

	[SerializeField]
	private GradientColor[] titleTextColors;

	[SerializeField]
	private GradientColor[] difficultyBgColors;

	protected Callback okCallback;

	private Callback cancelCallback;

	private const int maxCurrencies = 1;

	public override void Open()
	{
		base.Open();
		EnableCloseArea(enable: true);
	}

	public void SetContent(string title, string info1, string info2, SurvivalDifficulty survivalDifficulty)
	{
		if (title != null && titleLabel != null)
		{
			titleLabel.text = title;
		}
		if (infoLabel1 != null)
		{
			if (info1 != null)
			{
				infoLabel1.text = info1;
			}
			Helpers.GameObjectSetActive(infoLabel1, info1 != null);
		}
		if (infoLabel2 != null)
		{
			if (info2 != null)
			{
				infoLabel2.text = info2;
			}
			Helpers.GameObjectSetActive(infoLabel2, info2 != null);
		}
		SetDifficultyRelatedContent(survivalDifficulty);
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
				if (list.Count < 1)
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
			if (num > 0 && list.Count < 1)
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
			okButtonLabel.text = text;
		}
	}

	public void SetCancelButtonLabel(string text)
	{
		if (cancelButtonLabel != null)
		{
			cancelButtonLabel.text = text;
		}
	}

	public virtual void OkPressed()
	{
		EventManager.NotifyClick("OkPressed");
		base.Close();
		if (okCallback != null)
		{
			okCallback();
			okCallback = null;
		}
	}

	public override void Close()
	{
		base.Close();
		if (cancelCallback != null)
		{
			cancelCallback();
			cancelCallback = null;
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
		if (currencyTypes != null && currencyTypes.Count > 1)
		{
			Debug.LogWarning("Attempt to set more currencies to SurvivalConfirmationPopup than supported (only the supported amount will be shown).");
		}
		GameObject[] array = new GameObject[1] { currency1Container };
		for (int i = 0; i < 1; i++)
		{
			Helpers.GameObjectSetActive(array[i], currencyTypes != null && i < currencyTypes.Count);
		}
		if (currencyTypes == null || currencyAmounts == null)
		{
			return;
		}
		for (int j = 0; j < currencyTypes.Count; j++)
		{
			if (j == 0 && array[j] != null)
			{
				currency1Sprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyTypes[j]);
				currency1AmountLabel.text = Helpers.FormatNumber(currencyAmounts[j]);
			}
		}
	}

	public void EnableCloseArea(bool enable)
	{
		if (closeArea != null)
		{
			closeArea.SetActive(enable);
		}
	}

	public static void ShowPopup(string title, string message1, string message2, string okButtonLabel, Callback okCallback, SurvivalDifficulty survivalDifficulty, string cancelButtonLabel = null, Callback cancelCallback = null)
	{
		SurvivalConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalConfirmationPopup) as SurvivalConfirmationPopup;
		obj.SetContent(title, message1, message2, survivalDifficulty);
		obj.SetOkButtonLabel(okButtonLabel);
		obj.SetCancelButtonLabel(cancelButtonLabel);
		obj.SetCallbacks(okCallback, cancelCallback);
		obj.Open();
	}

	public static void ShowPopupGetText(string titleLocaleKey, string message1LocaleKey, string message2LocaleKey, string okButtonLocaleKey, Callback okCallback, SurvivalDifficulty survivalDifficulty, string cancelButtonLocaleKey = null, Callback cancelCallback = null)
	{
		string message = ((message1LocaleKey != null) ? LocalizationManager.GetText(message1LocaleKey) : null);
		string message2 = ((message2LocaleKey != null) ? LocalizationManager.GetText(message2LocaleKey) : null);
		string text = ((cancelButtonLocaleKey != null) ? LocalizationManager.GetText(cancelButtonLocaleKey) : null);
		ShowPopup(LocalizationManager.GetText(titleLocaleKey), message, message2, LocalizationManager.GetText(okButtonLocaleKey), okCallback, survivalDifficulty, text, cancelCallback);
	}

	private void SetDifficultyRelatedContent(SurvivalDifficulty survivalDifficulty)
	{
		titleLabel.gradientTop = titleTextColors[(int)(survivalDifficulty - 1)].GradientTop;
		titleLabel.gradientBottom = titleTextColors[(int)(survivalDifficulty - 1)].GradientBottom;
		difficultyLabelBg.gradientTop = difficultyBgColors[(int)(survivalDifficulty - 1)].GradientTop;
		difficultyLabelBg.gradientBottom = difficultyBgColors[(int)(survivalDifficulty - 1)].GradientBottom;
		int num = SurvivalMissionDifficultyLevelHelper.CalculateResultingSurvivalMissionLevel(GameManager.Instance.gameEconomyData, 0, GameManager.Instance.playerModel.CouncilLevel, survivalDifficulty) / 3;
		HelpersUI.SetContentToLabel(difficultyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SurvivalDifficulty.Label{Difficulty}", num));
	}
}
