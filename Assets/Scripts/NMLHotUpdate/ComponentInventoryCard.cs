using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class ComponentInventoryCard : UIListCard<CurrencyModel>
{
	[SerializeField]
	private UILabel componentLabel;

	[SerializeField]
	private UILabel componentDescription;

	[SerializeField]
	private List<RecipeComponentView> components;

	public override void UpdateUI()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		CurrencyType type = base.Item.Type;
		string text = type.ToString().TrimEnd(new char[1] { '0' });
		if (componentLabel != null)
		{
			componentLabel.text = LocalizationManager.GetText("Component." + text + ".Name");
		}
		if (componentDescription != null)
		{
			componentDescription.text = LocalizationManager.GetText("Component." + text + ".Description");
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				CurrencyType currencyFromBaseAndRarity = ComponentHelper.GetCurrencyFromBaseAndRarity(type, i);
				if (currencyFromBaseAndRarity != CurrencyType.None)
				{
					int value;
					if (DataManager.Instance != null && !CraftSettings.Instance.IsRealPlayerData)
					{
						var currency = CraftSettings.Instance.Currency.FirstOrDefault(x => x.Type == currencyFromBaseAndRarity);
						value = currency != null ? currency.Value : CraftSettings.Instance.CurrencyCountMax;
					}
					else
					{
						value = playerModel.GetCurrency(currencyFromBaseAndRarity).Value;
					}
					components[i].Initialize(currencyFromBaseAndRarity, value);
					components[i].SetEnabled(value > 0);
				}
			}
		}
	}
}
