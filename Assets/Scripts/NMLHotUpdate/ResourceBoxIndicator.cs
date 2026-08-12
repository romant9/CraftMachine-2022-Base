using TWDModel;
using UnityEngine;

public class ResourceBoxIndicator : MonoBehaviour
{
	[Tooltip("Label for primary resource count.")]
	[SerializeField]
	private UILabel primaryAmountLabel;

	[Tooltip("Label for secondary resource count.")]
	[SerializeField]
	private UILabel secondaryAmountLabel;

	[SerializeField]
	private UISprite resourceIcon;

	private CurrencyType currencyType;

	public void Setup(CurrencyType currency, int primaryAmount, int secondaryAmount = -1)
	{
		if (resourceIcon != null)
		{
			resourceIcon.spriteName = HelpersGfx.GetCurrencyIconName(currency);
		}
		if (primaryAmountLabel != null)
		{
			primaryAmountLabel.text = primaryAmount.ToString();
		}
		if (secondaryAmountLabel != null)
		{
			if (secondaryAmount > -1)
			{
				secondaryAmountLabel.text = secondaryAmount.ToString();
			}
			else
			{
				secondaryAmountLabel.text = "";
			}
		}
		currencyType = currency;
	}

	public CurrencyType GetCurrency()
	{
		return currencyType;
	}

	public void SetPrimaryAmount(string amount)
	{
		if (primaryAmountLabel != null)
		{
			primaryAmountLabel.text = amount;
		}
	}

	public void SetSecondaryAmount(string amount)
	{
		if (secondaryAmountLabel != null)
		{
			secondaryAmountLabel.text = amount;
		}
	}
}
