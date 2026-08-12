using TWDModel;
using UnityEngine;

public class CurrencyAmountPanel : MonoBehaviour
{
	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite iconSprite;

	public UISprite Icon => iconSprite;

	public int Amount { get; private set; }

	public CurrencyType CurrencyType { get; private set; }

	public void Set(string type, int amount)
	{
		Amount = amount;
		Show(show: true);
		amountLabel.text = amount.ToString();
		iconSprite.spriteName = HelpersGfx.GetIconName(type);
	}

	public void Set(CurrencyType currencyType, int amount)
	{
		Amount = amount;
		CurrencyType = currencyType;
		Show(show: true);
		amountLabel.text = amount.ToString();
		iconSprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyType);
	}

	public void Show(bool show)
	{
		NGUITools.SetActiveChildren(base.gameObject, show);
	}
}
