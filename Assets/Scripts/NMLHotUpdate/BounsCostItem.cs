using TWDModel;
using UnityEngine;

public class BounsCostItem : MonoBehaviour
{
	[SerializeField]
	private UILabel costLabel;

	[SerializeField]
	private UISprite itemSprite;

	[SerializeField]
	private UIAtlas monochromeAtlas;

	public void Init(CurrencyType currencyType, int count)
	{
		if (currencyType == CurrencyType.None || count <= 0)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		string currencyIconName = HelpersGfx.GetCurrencyIconName(currencyType);
		HelpersUI.SetSpriteAndAtlas(itemSprite, currencyIconName, monochromeAtlas);
		int value = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
		if (costLabel != null)
		{
			if (IsEnough(currencyType, count))
			{
				costLabel.text = $"{value}/{count}";
			}
			else
			{
				costLabel.text = $"[ff0000]{value}[-]/{count}";
			}
		}
	}

	public bool IsEnough(CurrencyType currencyType, int count)
	{
		int value = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
		if (count > 0 && value > 0)
		{
			return count <= value;
		}
		return false;
	}
}
